﻿using UnityEngine;
using UnityEngine.Events;
using Common.PhysicsUtils;
using Bolt;

namespace Steering
{
    public class SteeringWheel : EntityBehaviour<IKartState>, IControllable
    {
        public enum TurnState { NotTurning, Left, Right }

        [Header("Turn Torques")]
        public SteeringWheelSettings Settings;

        [Header("States")]
        public TurnState TurningState = TurnState.NotTurning;
        public bool InversedDirections = false;

        [Header("Options")]
        [Tooltip("This is an optional field to make turn possible only if grounded.")]
        public GroundCondition _groundCondition;
        [Tooltip("This is an optional field to make turn possible only if a certain speed is reached.")]
        public Engine.Engine _engine;

        [Header("Events")]
        public UnityEvent<TurnState> OnTurn;

        private Rigidbody _rb;
        private float _turnValue;

        // CORE

        private void Awake()
        {
            _rb = GetComponentInParent<Rigidbody>();
        }

        // PUBLIC

        public void MapInputs()
        {
            _turnValue = Input.GetAxis(Constants.Input.TurnAxis);
        }

        public override void Attached()
        {
            if (!entity.isControlled && entity.isOwner)
            {
                entity.TakeControl();
            }
        }

        public override void SimulateController()
        {
            MapInputs();

            IKartCommandInput input = KartCommand.Create();
            input.Turn = _turnValue;

            entity.QueueInput(input);
        }

        public override void ExecuteCommand(Command command, bool resetState)
        {
            KartCommand cmd = (KartCommand)command;

            if (resetState)
            {
                Debug.LogWarning("Applying Engine Correction");
            }
            else
            {
                var rb = _rb;
                rb = TurnUsingTorque(cmd.Input.Turn,rb);
                cmd.Result.Velocity = rb.velocity;
            }
        }
        public Rigidbody TurnUsingTorque(float turnValue, Rigidbody rb)
        {
            if (CanTurn())
            {
                SetTurnState(turnValue);
                turnValue = InversedTurnValue(turnValue);

                if (_groundCondition != null)
                {
                    if (_groundCondition.Grounded)
                    {
                        rb.AddRelativeTorque(Vector3.up * turnValue * Settings.TurnTorque, ForceMode.Force);
                        //OnTurn.Invoke(TurningState);
                    }
                }
                else
                {
                    rb.AddRelativeTorque(Vector3.up * turnValue * Settings.TurnTorque, ForceMode.Force);
                    OnTurn.Invoke(TurningState);
                }
            }
            return rb;
        }

        public void TurnUsingTorque(float turnValue)
        {
            if (CanTurn())
            {
                SetTurnState(turnValue);
                turnValue = InversedTurnValue(turnValue);

                if (_groundCondition != null)
                {
                    if (_groundCondition.Grounded)
                    {
                        _rb.AddRelativeTorque(Vector3.up * turnValue * Settings.TurnTorque, ForceMode.Force);
                        //OnTurn.Invoke(TurningState);
                    }
                }
                else
                {
                    _rb.AddRelativeTorque(Vector3.up * turnValue * Settings.TurnTorque, ForceMode.Force);
                    OnTurn.Invoke(TurningState);
                }
            }
        }

        public void InverseDirections()
        {
            InversedDirections = !InversedDirections;
            Debug.Log("Inversing directions !");
        }

        // PRIVATE

        private void TurnSlowDown(float turnAxis)
        {
            if (TurningState != TurnState.NotTurning)
            {
                float slowdownForce = Settings.SlowdownTurnValue * -Mathf.Abs(turnAxis);
                _rb.AddForce(transform.forward * slowdownForce);
            }
        }

        private void SetTurnState(float turnValue)
        {
            if (turnValue > 0)
            {
                TurningState = TurnState.Right;
            }
            else if (turnValue < 0)
            {
                TurningState = TurnState.Left;
            }
            else
            {
                TurningState = TurnState.NotTurning;
            }
        }

        private bool CanTurn()
        {
            if (_engine.CurrentSpeed > Settings.MinimumSpeedToTurn)
            {
                return true;
            }
            else if (_engine.CurrentSpeed < Settings.MinimumBackSpeedToTurn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private float InversedTurnValue(float value)
        {
            return _engine.CurrentMovingDirection == Engine.MovingDirection.Backward ? -value : value;
        }
    }
}
