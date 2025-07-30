using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Utilities.Tools.ServicesLocator;
using static Freya.Mathfs;

namespace Universal.Runtime.Utilities.Tools.Updates
{
    public class UpdateManager : MonoBehaviour, IUpdateManager
    {
        [SerializeField] FrameBudgetSettings frameBudgetSettings;
        readonly Dictionary<UpdatePriority, List<IUpdatable>> prioritizedUpdatables = new();
        readonly List<IFixedUpdatable> fixedUpdatables = new();
        readonly List<ILateUpdatable> lateUpdatables = new();
        readonly List<TimeSlicedUpdatableWrapper> timeSlicedUpdatables = new();
        readonly ConcurrentQueue<Action> asyncUpdateQueue = new();
        readonly Dictionary<string, UpdateGroup> updateGroups = new();
        readonly Dictionary<IUpdatable, string> updatableGroups = new();
        readonly List<UpdateProfileData> profilingData = new();
        float frameStartTime;
        struct TimeSlicedUpdatableWrapper
        {
            public ITimeSlicedUpdatable Updatable;
            public int ExecutionOrder;
        }
        struct UpdateGroup
        {
            public string Name;
            public bool IsActive;
        }
        struct UpdateProfileData
        {
            public IUpdatable Updatable;
            public string Name;
            public float LastExecutionTime;
            public float AverageTime;
            public float MaxTime;
            public int ExecutionCount;
        }

        void Awake()
        {
            ServiceLocator.Global.Register<IUpdateManager>(this);

            IList list = Enum.GetValues(typeof(UpdatePriority));
            for (var i = 0; i < list.Count; i++)
                prioritizedUpdatables[(UpdatePriority)list[i]] = new List<IUpdatable>();
        }

        #region IUpdateManager Implementation
        public void RegisterUpdatable(IUpdatable updatable, UpdatePriority priority = UpdatePriority.Normal)
        {
            if (updatable == null) return;

            prioritizedUpdatables[priority].Add(updatable);

            if (updatable is ITimeSlicedUpdatable timeSliced)
            {
                timeSlicedUpdatables.Add(new TimeSlicedUpdatableWrapper
                {
                    Updatable = timeSliced,
                    ExecutionOrder = timeSliced.ExecutionOrder
                });
                timeSlicedUpdatables.Sort((a, b) => a.ExecutionOrder.CompareTo(b.ExecutionOrder));
            }
        }

        public void RegisterFixedUpdatable(IFixedUpdatable fixedUpdatable)
        {
            if (fixedUpdatable != null && !fixedUpdatables.Contains(fixedUpdatable))
                fixedUpdatables.Add(fixedUpdatable);
        }

        public void RegisterLateUpdatable(ILateUpdatable lateUpdatable)
        {
            if (lateUpdatable != null && !lateUpdatables.Contains(lateUpdatable))
                lateUpdatables.Add(lateUpdatable);
        }

        public void AddToUpdateGroup(IUpdatable updatable, string groupName)
        {
            if (!updateGroups.ContainsKey(groupName))
                updateGroups[groupName] = new UpdateGroup { Name = groupName };
            updatableGroups[updatable] = groupName;
        }

        public void SetGroupActive(string groupName, bool active)
        {
            if (updateGroups.TryGetValue(groupName, out var group))
                group.IsActive = active;
        }

        public void UnregisterUpdatable(IUpdatable updatable)
        {
            foreach (var list in prioritizedUpdatables.Values) list.Remove(updatable);

            if (updatable is ITimeSlicedUpdatable)
            {
                for (var i = timeSlicedUpdatables.Count - 1; i >= 0; i--)
                {
                    if (timeSlicedUpdatables[i].Updatable == updatable)
                    {
                        timeSlicedUpdatables.RemoveAt(i);
                        break;
                    }
                }
            }

            updatableGroups.Remove(updatable);

            for (var i = profilingData.Count - 1; i >= 0; i--)
            {
                if (profilingData[i].Updatable == updatable)
                {
                    profilingData.RemoveAt(i);
                    break;
                }
            }
        }

        public void UnregisterFixedUpdatable(IFixedUpdatable fixedUpdatable)
        => fixedUpdatables.Remove(fixedUpdatable);

        public void UnregisterLateUpdatable(ILateUpdatable lateUpdatable)
        => lateUpdatables.Remove(lateUpdatable);

        public void ChangePriority(IUpdatable updatable, UpdatePriority newPriority)
        {
            if (updatable == null) return;

            // Remove from all priorities
            foreach (var list in prioritizedUpdatables.Values)
                list.Remove(updatable);

            // Add to new priority
            prioritizedUpdatables[newPriority].Add(updatable);
        }

        public void QueueAsyncUpdate(Action updateAction)
        {
            if (updateAction == null) return;

            asyncUpdateQueue.Enqueue(updateAction);
        }
        #endregion

        #region Update Execution
        void Update()
        {
            frameStartTime = Time.realtimeSinceStartup;

            ExecutePriorityGroup(UpdatePriority.Early, frameBudgetSettings.earlyUpdateMaxMs);
            ExecutePriorityGroup(UpdatePriority.Normal, frameBudgetSettings.normalUpdateMaxMs);
            ExecuteTimeSlicedUpdates();
            ExecutePriorityGroup(UpdatePriority.Late, frameBudgetSettings.lateUpdateMaxMs);
            ExecutePriorityGroup(UpdatePriority.UI, frameBudgetSettings.uiUpdateMaxMs);

            ProcessAsyncUpdates();
        }

        void FixedUpdate()
        {
            for (var i = 0; i < fixedUpdatables.Count; i++)
                fixedUpdatables[i].OnFixedUpdate();
        }

        void LateUpdate()
        {
            for (var i = 0; i < lateUpdatables.Count; i++)
                lateUpdatables[i].OnLateUpdate();
        }

        void ExecutePriorityGroup(UpdatePriority priority, float maxTime)
        {
            if (!prioritizedUpdatables.TryGetValue(priority, out var updatables)) return;

            var elapsed = 0f;
            for (var i = 0; i < updatables.Count && elapsed < maxTime; i++)
            {
                var updatable = updatables[i];

                if (updatableGroups.TryGetValue(updatable, out var groupName) && !updateGroups[groupName].IsActive)
                    continue;

                if (updatable is IConditionalUpdatable conditional && !conditional.ShouldUpdate())
                    continue;

                ProfileExecution(updatable, () => updatable.OnUpdate());
                elapsed = Time.realtimeSinceStartup - frameStartTime;
            }
        }

        void ExecuteTimeSlicedUpdates()
        {
            for (var i = 0; i < timeSlicedUpdatables.Count; i++)
            {
                var wrapper = timeSlicedUpdatables[i];
                if (wrapper.Updatable.IsComplete) continue;

                ProfileExecution(wrapper.Updatable, () => wrapper.Updatable.OnUpdate());
                if (Time.realtimeSinceStartup - frameStartTime > frameBudgetSettings.normalUpdateMaxMs)
                    break;
            }
        }

        void ProcessAsyncUpdates()
        {
            while (asyncUpdateQueue.TryDequeue(out var action))
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Logging.LogError($"Async update failed: {e}");
                }
            }
        }
        #endregion

        #region Profile
        void ProfileExecution(IUpdatable updatable, Action updateAction)
        {
            var startTime = Time.realtimeSinceStartup;
            updateAction.Invoke();
            var duration = Time.realtimeSinceStartup - startTime;

            var index = -1;
            for (var i = 0; i < profilingData.Count; i++)
            {
                if (profilingData[i].Updatable == updatable)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                profilingData.Add(new UpdateProfileData
                {
                    Updatable = updatable,
                    Name = updatable.GetType().Name,
                    LastExecutionTime = duration,
                    AverageTime = duration,
                    MaxTime = duration,
                    ExecutionCount = 1
                });
            else
            {
                var data = profilingData[index];
                data.LastExecutionTime = duration;
                data.ExecutionCount++;
                data.AverageTime = (data.AverageTime * (data.ExecutionCount - 1) + duration) / data.ExecutionCount;
                data.MaxTime = Max(data.MaxTime, duration);
                profilingData[index] = data;
            }
        }

        public void DisplayProfilingResults()
        {
            profilingData.Sort((a, b) => b.AverageTime.CompareTo(a.AverageTime));

            var sb = new StringBuilder("Update Profiling Results:\n");
            for (var i = 0; i < profilingData.Count; i++)
            {
                var data = profilingData[i];
                sb.AppendLine($"{data.Name}: Avg={data.AverageTime:F4}ms, Max={data.MaxTime:F4}ms, Count={data.ExecutionCount}");
            }
            Logging.Log(sb.ToString());
        }
        #endregion

        void OnDestroy()
        {
            foreach (var list in prioritizedUpdatables.Values) list.Clear();
            fixedUpdatables.Clear();
            lateUpdatables.Clear();
            timeSlicedUpdatables.Clear();
            asyncUpdateQueue.Clear();
            updateGroups.Clear();
            updatableGroups.Clear();
            profilingData.Clear();
        }
    }
}