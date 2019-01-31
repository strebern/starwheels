﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Bolt;
using UnityEngine;

namespace Abilities
{
    public class CloakPortalBehaviour : EntityBehaviour<IItemState>
    {
        [SerializeField] GameObject _targetPortal;

        [Header("Unity Events")]
        public UnityEvent AtStartCloakTeleport;
        public UnityEvent AtEndCloakTeleport;

        private KartMeshDisabler _kartMeshDisabler;
        private Health.Health _health;
        private Rigidbody _kartRigidbody;
        private CloakPortalsActivator _cloakPortalActivator;
        private Coroutine _cloakPortalCoroutine;
        private LineRenderer _lineRenderer;

        private GameObject _fakeKartVisualInPortal;
        private CloakPortalCameraBehaviour _portalCamera;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _cloakPortalActivator = GetComponentInParent<CloakPortalsActivator>();
        }

        private void Start()
        {
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _targetPortal.transform.position);
        }

        public void TeleportPlayerToTargetPortal(GameObject kart, GameObject targetPortal)
        {
            _health = kart.GetComponentInChildren<Health.Health>();
            _kartRigidbody = kart.GetComponentInChildren<Rigidbody>();
            _kartMeshDisabler = kart.GetComponentInChildren<CloakAbility>().KartMeshDisabler;

            _cloakPortalCoroutine = StartCoroutine(PortalTravelBehaviour(kart, targetPortal));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constants.Tag.Kart))
            {
                if (other.transform.root.gameObject.GetComponentInChildren<CloakAbility>() != null)
                {
                    if (other.transform.root.gameObject.GetComponentInChildren<CloakAbility>().CanUsePortals)
                    {
                        TeleportPlayerToTargetPortal(other.transform.root.gameObject, _targetPortal);
                    }
                }
            }
        }



        private void PortalCameraSwitch()
        {
            _portalCamera = GameObject.Find("PlayerCamera").GetComponent<CloakPortalCameraBehaviour>();
        }


        private IEnumerator PortalTravelBehaviour(GameObject kart, GameObject targetPortal)
        {
            kart.GetComponentInChildren<CloakAbility>().CanUsePortals = false;
            kart.GetComponent<Common.ControllableDisabler>().DisableAllInChildren();

            PortalCameraSwitch();

            _kartMeshDisabler.DisableKartMeshes(false);
            kart.GetComponentInChildren<CloakAbility>().CloakEffect.SetActive(false);
            _health.SetInvincibility();



            var _currentTimer = 0f;

            _fakeKartVisualInPortal = _cloakPortalActivator.FakeKartVisualInPortal;
            _fakeKartVisualInPortal.SetActive(true);
            _fakeKartVisualInPortal.transform.position = Vector3.zero;

            while (_currentTimer < _cloakPortalActivator.TravelTime)
            {
                Debug.Log(_currentTimer);
                _fakeKartVisualInPortal.transform.position = Vector3.Lerp(this.transform.position, _targetPortal.transform.position, _currentTimer / _cloakPortalActivator.TravelTime);
                _currentTimer += Time.deltaTime;
                yield return null;
            }

            _fakeKartVisualInPortal.SetActive(false);
            // yield return new WaitForSeconds(_cloakPortalActivator.TravelTime);

            var y = _targetPortal.transform.position.y + 0f;
            _kartRigidbody.transform.position = new Vector3(_targetPortal.transform.position.x, y, _targetPortal.transform.position.z);
            _kartRigidbody.transform.rotation = transform.rotation;
            _kartRigidbody.AddRelativeForce(new Vector3(0, 15000, 15000));

            _kartMeshDisabler.EnableKartMeshes(false);
            _health.UnsetInvincibility();
            kart.GetComponent<Common.ControllableDisabler>().EnableAllInChildren();
            kart.GetComponentInChildren<CloakAbility>().CloakEffect.SetActive(true);

            yield return new WaitForSeconds(_cloakPortalActivator.TimeToUseThisPortalAgain);
            kart.GetComponentInChildren<CloakAbility>().CanUsePortals = true;

        }
    }
}
