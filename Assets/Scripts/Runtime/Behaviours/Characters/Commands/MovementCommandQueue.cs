using System.Collections.Generic;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class MovementCommandQueue : MonoBehaviour
    {
        [SerializeField] int queueDepth = 3;
        [SerializeField] float runActivationTime = 1f;
        readonly Queue<IMovementCommand> commandQueue = new();
        readonly MoveForwardCommand moveForwardCommand = new();
        IGridMover mover;
        float forwardHoldTime;
        bool isForwardHeld;

        void Awake() => TryGetComponent(out mover);

        void Update()
        {
            if (isForwardHeld)
            {
                forwardHoldTime += Time.deltaTime;
                if (commandQueue.Count < queueDepth && !mover.IsMoving && !mover.IsRotating)
                    EnqueueCommand(moveForwardCommand);
                
            }

            if (mover.IsMoving || mover.IsRotating || commandQueue.Count <= 0) return;
            var nextCommand = commandQueue.Dequeue();
            mover.TryExecuteCommand(nextCommand);
        }

        public void EnqueueCommand(IMovementCommand command)
        {
            if (commandQueue.Count >= queueDepth) return;
            commandQueue.Enqueue(command);
        }

        public void ClearQueue()
        {
            commandQueue.Clear();
            forwardHoldTime = 0f;
        }

        public void HandleForwardPress()
        {
            isForwardHeld = true;
            forwardHoldTime = 0f;
            EnqueueCommand(moveForwardCommand);
        }

        public void HandleForwardRelease()
        {
            isForwardHeld = false;
            if (forwardHoldTime >= runActivationTime)
                ClearQueue();
            forwardHoldTime = 0f;
        }
    }
}