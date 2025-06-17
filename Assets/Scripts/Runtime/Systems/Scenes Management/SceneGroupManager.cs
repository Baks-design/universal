using System;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Universal.Runtime.Systems.ScenesManagement
{
    public class SceneGroupManager
    {
        SceneGroup ActiveSceneGroup;
        readonly AsyncOperationHandleGroup handleGroup = new(10);

        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        public async Awaitable LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            ActiveSceneGroup = group;

            var loadedScenes = new List<string>();

            await UnloadScenes();

            var sceneCount = SceneManager.sceneCount;
            for (var i = 0; i < sceneCount; i++)
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);

            var totalScenesToLoad = ActiveSceneGroup.Scenes.Count;
            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (var i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = group.Scenes[i];
                if (reloadDupScenes == false && loadedScenes.Contains(sceneData.Name))
                    continue;

                switch (sceneData.Reference.State)
                {
                    case SceneReferenceState.Regular:
                        var operation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                        operationGroup.Operations.Add(operation);
                        break;
                    case SceneReferenceState.Addressable:
                        var sceneHandle = Addressables.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                        handleGroup.Handles.Add(sceneHandle);
                        break;
                }

                OnSceneLoaded.Invoke(sceneData.Name);
            }

            // Wait until all AsyncOperations in the group are done
            while (!operationGroup.IsDone || !handleGroup.IsDone)
            {
                progress?.Report((operationGroup.Progress + handleGroup.Progress) / 2f);
                await Awaitable.WaitForSecondsAsync(3f); //to test loader
            }

            var activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));
            if (activeScene.IsValid())
                SceneManager.SetActiveScene(activeScene);

            OnSceneGroupLoaded.Invoke();
        }

        public async Awaitable UnloadScenes()
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;

            var sceneCount = SceneManager.sceneCount;
            for (var i = sceneCount - 1; i > 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded)
                    continue;

                var sceneName = sceneAt.name;
                if (sceneName.Equals(activeScene) || sceneName.Equals("Bootstrapper"))
                    continue;

                var isSceneInHandles = false;
                for (var i1 = 0; i1 < handleGroup.Handles.Count; i1++)
                {
                    var handle = handleGroup.Handles[i1];
                    if (handle.IsValid() && handle.Result.Scene.name == sceneName)
                    {
                        isSceneInHandles = true;
                        break;
                    }
                }
                if (isSceneInHandles)
                    continue;

                scenes.Add(sceneName);
            }

            // Create an AsyncOperationGroup
            var operationGroup = new AsyncOperationGroup(scenes.Count);

            for (var i = 0; i < scenes.Count; i++)
            {
                var scene = scenes[i];

                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null)
                    continue;

                operationGroup.Operations.Add(operation);

                OnSceneUnloaded.Invoke(scene);
            }

            for (var i = 0; i < handleGroup.Handles.Count; i++)
            {
                var handle = handleGroup.Handles[i];
                if (handle.IsValid())
                    Addressables.UnloadSceneAsync(handle);
            }
            handleGroup.Handles.Clear();

            // Wait until all AsyncOperations in the group are done
            while (!operationGroup.IsDone)
                await Awaitable.WaitForSecondsAsync(1f); // delay to avoid tight loop

            // Optional: UnloadUnusedAssets - unloads all unused assets from memory
            await Resources.UnloadUnusedAssets();
        }
    }

    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress
        {
            get
            {
                if (Operations.Count == 0)
                    return 0;

                var totalProgress = 0f;
                for (var i = 0; i < Operations.Count; i++)
                {
                    var operation = Operations[i];
                    totalProgress += operation.progress;
                }

                return totalProgress / Operations.Count;
            }
        }
        public bool IsDone
        {
            get
            {
                for (var i = 0; i < Operations.Count; i++)
                {
                    var operation = Operations[i];
                    if (!operation.isDone)
                        return false;
                }

                return true;
            }
        }

        public AsyncOperationGroup(int initialCapacity) => Operations = new List<AsyncOperation>(initialCapacity);
    }

    public readonly struct AsyncOperationHandleGroup
    {
        public readonly List<AsyncOperationHandle<SceneInstance>> Handles;

        public float Progress
        {
            get
            {
                if (Handles.Count == 0)
                    return 0;

                var totalProgress = 0f;
                for (var i = 0; i < Handles.Count; i++)
                {
                    var handle = Handles[i];
                    totalProgress += handle.PercentComplete;
                }

                return totalProgress / Handles.Count;
            }
        }
        public bool IsDone
        {
            get
            {
                if (Handles.Count == 0)
                    return true;

                for (var i = 0; i < Handles.Count; i++)
                {
                    var handle = Handles[i];
                    if (!handle.IsDone)
                        return false;
                }

                return true;
            }
        }

        public AsyncOperationHandleGroup(int initialCapacity)
        => Handles = new List<AsyncOperationHandle<SceneInstance>>(initialCapacity);
    }
}