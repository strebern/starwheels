﻿using Bolt;
using UnityEngine;
using UnityEngine.Events;

namespace Engine
{
    public enum MovingDirection { NotMoving, Forward, Backward }

    public class Engine : EntityBehaviour<IKartState>, IControllable
    {
        [Header("Forces")]
        public EngineSettings Settings;

        [Header("Events")]
        public FloatEvent OnVelocityChange;

        [Header("States")]
        public MovingDirection CurrentMovingDirection = MovingDirection.NotMoving;

        [HideInInspector] public float CurrentSpeed;

        private Rigidbody _rb;

        private float _forwardValue;
        private float _backwardValue;

        // CORE

        private void Awake()
        {
            _rb = GetComponentInParent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            ClampMagnitude();
            CurrentSpeed = transform.InverseTransformDirection(_rb.velocity).z;
            CheckMovingDirection();
            OnVelocityChange.Invoke(_rb.velocity.magnitude);
        }

        private void ClampMagnitude()
        {
            if (Settings.MaxMagnitude > 0)
                _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, Settings.MaxMagnitude);
        }

        // PUBLIC

        public void MapInputs()
        {
            _forwardValue =  Input.GetAxis(Constants.Input.Accelerate);
            _backwardValue =  Input.GetAxis(Constants.Input.Decelerate);
        }

        public override void Attached()
        {
            if (!entity.isControlled)
            {
                entity.TakeControl();
            }
        }

        public override void SimulateController()
        {
            MapInputs();

            var position = transform.position; position.x += 1f;
            transform.position = position;

            IKartCommandInput input = KartCommand.Create();
            input.Forward = _forwardValue;
            input.Backward = _backwardValue;

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
                Accelerate(cmd.Input.Forward);
                Decelerate(cmd.Input.Backward);
                cmd.Result.Velocity = _rb.velocity;
            }
        }

        // PRIVATE

        private void Accelerate(float value)
        {
            _rb.AddRelativeForce(Vector3.forward * value * Settings.SpeedForce, ForceMode.Force);
        }

        private void Decelerate(float value)
        {
            _rb.AddRelativeForce(Vector3.back * value * Settings.SpeedForce / Settings.DecelerationFactor, ForceMode.Force);
        }

        private void CheckMovingDirection()
        {
            if(CurrentSpeed > 0 && CurrentMovingDirection != MovingDirection.Forward)
            {
                CurrentMovingDirection = MovingDirection.Forward;
            }
            else if(CurrentSpeed < 0 && CurrentMovingDirection != MovingDirection.Backward)
            {
                CurrentMovingDirection = MovingDirection.Backward;
            }
            else if(CurrentSpeed == 0 && CurrentMovingDirection != MovingDirection.NotMoving)
            {
                CurrentMovingDirection = MovingDirection.NotMoving;
            }
        }
    }
}
