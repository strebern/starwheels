﻿using System.Collections;
using UnityEngine;
using Multiplayer;
using Multiplayer.Teams;
using Bolt;

namespace Items
{
    [RequireComponent(typeof(Collider))]
    public class RocketLockTarget : EntityBehaviour<IItemState>
    {
        [Header("Targeting system")]
        public float SecondsBeforeSearchingTarget;
        public GameObject ActualTarget = null;

        private float _actualTargetDistance = Mathf.Infinity;
        private bool _activated = false;

        private void Start()
        {
            StartCoroutine(LookForTarget());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == Constants.Tag.KartHealthHitBox && _activated && ActualTarget == null)
            {
                var otherPlayer = other.GetComponentInParent<Player>();
                if (state.Team != otherPlayer.Team.GetColor())
                {
                    ActualTarget = other.gameObject;
                    StartCoroutine(GetComponentInParent<RocketBehaviour>().StartQuickTurn());
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == Constants.Tag.KartHealthHitBox && _activated)
            {
                var otherPlayer = other.GetComponentInParent<Player>();
                if (state.Team != otherPlayer.Team.GetColor())
                {
                    if (IsKartIsCloserThanActualTarget(other.gameObject) || ActualTarget == null)
                    {
                        ActualTarget = other.gameObject;
                        _actualTargetDistance = Vector3.Distance(transform.position, ActualTarget.transform.position);
                        StartCoroutine(GetComponentInParent<RocketBehaviour>().StartQuickTurn());
                    }
                }
            }
        }

        IEnumerator LookForTarget()
        {
            _activated = false;
            yield return new WaitForSeconds(SecondsBeforeSearchingTarget); // For X seconds the rocket goes straight forward
            _activated = true;
        }

        public bool IsKartIsCloserThanActualTarget(GameObject kart)
        {
            return Vector3.Distance(transform.position, kart.transform.position) < _actualTargetDistance;
        }
    }
}
