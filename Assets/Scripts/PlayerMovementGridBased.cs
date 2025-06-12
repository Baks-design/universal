using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Universal
{
    public class PlayerMovementGridBased : MonoBehaviour
    {
        [Header("Step Settings")]
        [SerializeField] LayerMask collisionLayerMask;
        [SerializeField, Range(1f, 5f)] float maximumStepHeight = 2f;
        [SerializeField, Range(1, 5)] int QueueDepth = 1;
        [SerializeField, Range(0.5f, 5f)] float keyPressThresholdTime = 0f;
        [Header("Walk Settings")]
        [SerializeField, Range(1f, 5f)] float walkSpeed = 1f;
        [SerializeField, Range(1f, 10f)] float turnSpeed = 5f;
        [SerializeField] AnimationCurve walkSpeedCurve;
        [Header("Run Settings")]
        [SerializeField, Range(1f, 5f)] float runningSpeed = 1.5f;
        [SerializeField] AnimationCurve runningSpeedCurve;
        // Movement state
        bool isRunHold;
        float rotationTime, curveTime, stepTime, stepTimeCounter, currentSpeed, forwardKeyPressedTime;
        const float gridSize = 3f, RightHand = 90f, LeftHand = -RightHand, ApproximationThreshold = 0.025f;
        Vector3 moveFromPosition, moveTowardsPosition;
        Quaternion rotateFromDirection, rotateTowardsDirection;
        AnimationCurve currentAnimationCurve;
        // Cached references
        readonly Vector3 halfExtentsCache = new(
            gridSize / 2f - 0.1f, 1f,
            gridSize / 2f - 0.1f
        );
        Transform bodyTransform;
        InputAction move, run, turn;
        float deltaTime;
        Vector2 moveInput;
        float turnInput;

        public bool IsStationary => !IsMoving && !IsRotating;
        public bool IsMoveValid { get; private set; }
        public Queue<Action> MovementQueueAction { get; private set; }
        bool IsMoving => HeightInvariantVector(bodyTransform.position) != HeightInvariantVector(moveTowardsPosition);
        bool IsRotating => bodyTransform.rotation != rotateTowardsDirection;
        Vector3 CalculateForwardPosition => bodyTransform.forward * gridSize;
        Vector3 CalculateStrafePosition => bodyTransform.right * gridSize;

        public event Action OnBlocked = delegate { };
        public event Action OnEventIfTheCommandIsNotQueable = delegate { };

        #region Initialization
        void Awake()
        {
            bodyTransform = transform;
            move = InputSystem.actions.FindAction("Player/Move");
            run = InputSystem.actions.FindAction("Player/Run");
            turn = InputSystem.actions.FindAction("Player/Turn");
        }

        void OnEnable()
        {
            OnBlocked += FlushQueue;
            run.performed += ProcessRun;
        }

        void OnDisable()
        {
            OnBlocked -= FlushQueue;
            run.performed -= ProcessRun;
        }

        void OnDestroy()
        {
            OnBlocked -= FlushQueue;
            run.performed -= ProcessRun;
        }

        void ProcessRun(InputAction.CallbackContext context)
        => isRunHold = context.phase is InputActionPhase.Performed;

        void Start()
        {
            InitializeVariables();
            ResetKeyPressTimer();
        }

        void InitializeVariables()
        {
            IsMoveValid = true;
            moveTowardsPosition = bodyTransform.position;
            rotateTowardsDirection = bodyTransform.rotation;
            currentAnimationCurve = walkSpeedCurve;
            currentSpeed = walkSpeed;
            stepTime = 1f / gridSize;
            MovementQueueAction = new Queue<Action>(QueueDepth);
            isRunHold = false;
        }
        #endregion

        void Update()
        {
            UpdateVars();

            MovementHandle();
            RunMovementHandle();

            ProcessMovement();
            ProcessTurn();

            AnimateMovement();
            AnimateRotation();
        }

        void UpdateVars()
        {
            deltaTime = Time.deltaTime;
            moveInput = move.ReadValue<Vector2>();
            turnInput = turn.ReadValue<float>();
        }

        #region Movement Processing
        void MovementHandle()
        {
            if (!IsMoveValid || !IsStationary || MovementQueueAction.Count <= 0) return;
            MovementQueueAction.Dequeue().Invoke();
        }

        void RunMovementHandle()
        {
            if (!IsMoveValid) return;

            if (isRunHold)
                RunForward();
            else
                StopRunForward();
        }

        void RunForward()
        {
            forwardKeyPressedTime += deltaTime;
            if (forwardKeyPressedTime >= keyPressThresholdTime && MovementQueueAction.Count < QueueDepth)
            {
                SwitchToRunning();
                QueueCommand(() => MoveForward());
            }
        }

        void StopRunForward()
        {
            if (forwardKeyPressedTime < keyPressThresholdTime) return;
            FlushQueue();
        }

        void FlushQueue()
        {
            ResetKeyPressTimer();
            MovementQueueAction.Clear();
            SwitchToWalking();
        }

        void ResetKeyPressTimer() => forwardKeyPressedTime = 0f;

        void ProcessMovement()
        {
            switch (moveInput.y)
            {
                case > 0f:
                    QueueCommand(() => MoveForward());
                    break;
                case < 0f:
                    QueueCommand(() => MoveBackward());
                    break;
            }
            switch (moveInput.x)
            {
                case > 0f:
                    QueueCommand(() => StrafeRight());
                    break;
                case < 0f:
                    QueueCommand(() => StrafeLeft());
                    break;
            }
        }

        void ProcessTurn()
        {
            switch (turnInput)
            {
                case > 0f:
                    QueueCommand(() => TurnRight());
                    break;
                case < 0f:
                    QueueCommand(() => TurnLeft());
                    break;
            }
        }

        void QueueCommand(Action action)
        {
            if (!IsStationary || MovementQueueAction.Count >= QueueDepth)
            {
                OnEventIfTheCommandIsNotQueable.Invoke();
                return;
            }
            MovementQueueAction.Enqueue(action);
        }
        #endregion

        #region Movement Settings
        void SwitchToWalking() => UpdateMovementSettings(walkSpeedCurve, walkSpeed);
        void SwitchToRunning() => UpdateMovementSettings(runningSpeedCurve, runningSpeed);

        void UpdateMovementSettings(AnimationCurve newCurve, float newSpeed)
        {
            var currentPosition = currentAnimationCurve.Evaluate(curveTime);
            var newPosition = newCurve.Evaluate(curveTime);

            if (newPosition < currentPosition)
                curveTime = FindTimeForValue(currentPosition, newCurve);

            currentSpeed = newSpeed;
            currentAnimationCurve = newCurve;
        }

        float FindTimeForValue(float position, AnimationCurve curve)
        {
            var result = 1f;
            while (position < curve.Evaluate(result) && result > 0f)
                result -= ApproximationThreshold;
            return result;
        }
        #endregion

        #region Animation
        void AnimateRotation()
        {
            if (!IsRotating) return;

            rotationTime += deltaTime;
            bodyTransform.rotation = Quaternion.Slerp(
                rotateFromDirection, rotateTowardsDirection, rotationTime * turnSpeed
            );
            CompensateRoundingErrors();
        }

        void AnimateMovement()
        {
            if (!IsMoving) return;

            curveTime += deltaTime * currentSpeed;
            stepTimeCounter += deltaTime * currentSpeed;

            if (stepTimeCounter > stepTime)
            {
                stepTimeCounter = 0f;
            }

            // Calculate horizontal movement
            var currentPositionValue = currentAnimationCurve.Evaluate(curveTime);
            var heightVariance = HeightInvariantVector(moveTowardsPosition) - HeightInvariantVector(moveFromPosition);
            var targetHeading = Vector3.Normalize(heightVariance);
            var newPosition = moveFromPosition + targetHeading * (currentPositionValue * gridSize);

            // Improved ground detection and height adjustment
            var targetHeight = moveTowardsPosition.y; // Default to target height

            // Raycast from above to find ground
            if (Physics.Raycast(
                newPosition + Vector3.up * maximumStepHeight, Vector3.down,
                out var hit, maximumStepHeight * 2f, collisionLayerMask)
            )
                targetHeight = hit.point.y;

            newPosition.y = targetHeight; // Apply the correct height
            bodyTransform.position = newPosition;

            CompensateRoundingErrors();
        }

        void CompensateRoundingErrors()
        {
            if (bodyTransform.rotation == rotateTowardsDirection)
            {
                bodyTransform.rotation = rotateTowardsDirection;
                rotationTime = 0f;
            }

            var currentPosition = HeightInvariantVector(bodyTransform.position);
            var target = HeightInvariantVector(moveTowardsPosition);

            if (currentPosition == target)
            {
                currentPosition = HeightInvariantVector(moveTowardsPosition);
                currentPosition.y = bodyTransform.position.y;

                bodyTransform.position = currentPosition;
                curveTime = 0f;
                stepTimeCounter = 0f;
            }
        }

        Vector3 HeightInvariantVector(Vector3 inVector) => new(inVector.x, 0f, inVector.z);
        #endregion

        #region Movement Commands
        void MoveForward() => CollisionCheckedMovement(CalculateForwardPosition);
        void MoveBackward() => CollisionCheckedMovement(-CalculateForwardPosition);
        void StrafeRight() => CollisionCheckedMovement(CalculateStrafePosition);
        void StrafeLeft() => CollisionCheckedMovement(-CalculateStrafePosition);

        void CollisionCheckedMovement(Vector3 movementDirection)
        {
            if (!IsStationary) return;

            var targetPosition = moveTowardsPosition + movementDirection;
            if (FreeSpace(targetPosition))
            {
                moveFromPosition = bodyTransform.position;
                moveTowardsPosition = targetPosition;
            }
            else
                OnBlocked.Invoke();
        }

        bool FreeSpace(Vector3 targetPosition)
        {
            var center = Vector3.Lerp(moveTowardsPosition, targetPosition, 0.6f);
            return !Physics.CheckBox(center, halfExtentsCache, bodyTransform.rotation, collisionLayerMask);
        }
        #endregion

        #region Rotation Commands
        void TurnRight() => TurnEulerDegrees(RightHand);
        void TurnLeft() => TurnEulerDegrees(LeftHand);

        void TurnEulerDegrees(float eulerDirectionDelta)
        {
            if (IsRotating) return;

            rotateFromDirection = bodyTransform.rotation;
            rotateTowardsDirection *= Quaternion.Euler(0f, eulerDirectionDelta, 0f);
            rotationTime = 0f;
        }
        #endregion
    }
}