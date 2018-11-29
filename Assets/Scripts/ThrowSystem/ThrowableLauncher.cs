﻿using UnityEngine;

namespace ThrowingSystem
{
    [RequireComponent(typeof(ThrowPositions))]
    public class ThrowableLauncher : Bolt.EntityBehaviour
    {
        [Header("LaunchType: Straight")]
        public float Speed;

        [Header("LaunchType: Arc")]
        public float ForwardThrowingForce;
        public float TimesLongerThanHighThrow;

        [SerializeField] Items.ShockwaveEffectBehaviour shockwaveEffectBehaviour;

        private ThrowPositions _itemPositions;

        // CORE

        private void Awake()
        {
            _itemPositions = GetComponent<ThrowPositions>();
        }

        // PUBLIC

        public void Throw(Throwable throwable, Direction throwingDirection)
        {
            switch (throwable.ThrowableType)
            {
                case ThrowableType.Arc:
                    ArcThrow(throwable, throwingDirection);
                    break;
                case ThrowableType.Straight:
                    StraightThrow(throwable, throwingDirection);
                    break;
                case ThrowableType.Drop:
                    if (throwingDirection == Direction.Forward)
                    {
                        ArcThrow(throwable, throwingDirection);
                    }
                    else
                    {
                        Drop(throwable, throwingDirection);
                    }
                    break;
            }
        }

        public Direction GetThrowingDirection() // TO DO BETTER
        {
            Direction direction = Direction.Default;

            if (Input.GetButton(Constants.Input.UseItemForward)) direction = Direction.Forward;
            else if (Input.GetButton(Constants.Input.UseItemBackward)) direction = Direction.Backward;
            else if (Input.GetAxis(Constants.Input.UpAndDownAxis) > 0.3f) direction = Direction.Forward;
            else if (Input.GetAxis(Constants.Input.UpAndDownAxis) < -0.3f) direction = Direction.Backward;

            return direction;
        }

        // PRIVATE

        private void ArcThrow(Throwable throwable, Direction throwingDirection)
        {
            Rigidbody rb = throwable.GetComponent<Rigidbody>();
            Vector3 rot = Vector3.zero;

            if (throwingDirection == Direction.Forward || throwingDirection == Direction.Default)
            {
                throwable.transform.position = _itemPositions.FrontPosition.position;
                rot = new Vector3(0, transform.rotation.eulerAngles.y, 0);
                var aimVector = transform.forward;
                rb.AddForce((aimVector + transform.up / TimesLongerThanHighThrow) * ForwardThrowingForce, ForceMode.Impulse);
            }
            else if (throwingDirection == Direction.Backward)
            {
                Drop(throwable, throwingDirection);
            }
            throwable.transform.rotation = Quaternion.Euler(rot);
            StartLaunchItemEvent(throwable.transform.position, throwable.transform.rotation, throwable.name);
        }

        private void StraightThrow(Throwable throwable, Direction throwingDirection)
        {
            Rigidbody rb = throwable.GetComponent<Rigidbody>();
            Vector3 rot = Vector3.zero;

            if (throwingDirection == Direction.Forward || throwingDirection == Direction.Default)
            {
                throwable.transform.position = _itemPositions.FrontPosition.position;
                rot = new Vector3(0, transform.rotation.eulerAngles.y, 0);
                rb.velocity = transform.forward.normalized * Speed;
            }
            else if (throwingDirection == Direction.Backward)
            {
                throwable.transform.position = _itemPositions.BackPosition.position;
                rot = new Vector3(0, transform.rotation.eulerAngles.y + 180, 0);
                rb.velocity = -transform.forward.normalized * Speed;
            }
            throwable.transform.rotation = Quaternion.Euler(rot);
            StartLaunchItemEvent(throwable.transform.position, throwable.transform.rotation, throwable.name);
        }

        private void Drop(Throwable throwable, Direction throwingDirection)
        {
            Vector3 rot = Vector3.zero;
            if (throwingDirection == Direction.Forward)
            {
                throwable.transform.position = _itemPositions.FrontPosition.position;
            }
            else if (throwingDirection == Direction.Backward || throwingDirection == Direction.Default)
            {
                throwable.transform.position = _itemPositions.BackPosition.position;
            }
            throwable.transform.rotation = Quaternion.Euler(rot);
            StartLaunchItemEvent(throwable.transform.position,throwable.transform.rotation,throwable.name);
        }

        private void StartLaunchItemEvent(Vector3 position,Quaternion rotation, string itemName)
        {
            var launchEvent = PlayerLaunchItem.Create(entity);
            launchEvent.Position = position;
            launchEvent.Rotation = rotation;
            launchEvent.ItemName = itemName;
            launchEvent.Entity = GetComponentInParent<BoltEntity>();
            launchEvent.Send();
        }
    }
}
