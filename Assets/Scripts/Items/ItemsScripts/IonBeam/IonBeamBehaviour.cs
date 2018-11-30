﻿using UnityEngine;
using Controls;
using CameraUtils;
using System.Collections;
using Steering;
using Engine;
using Bolt;

namespace Items
{
    public class IonBeamBehaviour : EntityBehaviour
    {
        [SerializeField] private GameObject ionBeamLaserPrefab;

        private GameObject _ionBeamOwner;
        private IonBeamInputs _ionBeamInputs;
        private IonBeamCamera _ionBeamCam;
        private bool _isFiring = false;

        [Header("Sounds")]
        public AudioSource LaunchSource;

        //CORE

        private void Awake()
        {
            _ionBeamCam = GameObject.Find("PlayerCamera").GetComponent<IonBeamCamera>();
            _ionBeamInputs = GetComponent<IonBeamInputs>();
        }

        public void Start()
        {
            _ionBeamOwner = GetComponent<Ownership>().OwnerKartRoot;

            if (entity.isOwner)
            {
                _ionBeamCam.IonBeamCameraBehaviour(true);
                EnableIonInputs();
            }
        }
        /*
        private void Update()
        {
            transform.position = _ionBeamCam.transposer.transform.position;
        }
        */
        public override void SimulateController()
        {
            transform.position = _ionBeamCam.Transposer.transform.position;
        }

        //PUBLIC

        public void EnableIonInputs()
        {
            _ionBeamCam.GetComponent<IonBeamCamera>().enabled = true;
            _ionBeamCam.GetComponent<CameraTurnEffect>().DisableTurnEffectInput();
            _ionBeamCam.GetComponent<CameraTurnEffect>().CenterCamera();
            StartCoroutine(DelayBeforeDisablePlayerInputs());
        }

        public void FireIonBeam()
        {
            if (!_isFiring && entity.isOwner)
            {
                Vector3 camPosition = _ionBeamCam.Transposer.transform.position;

                var IonBeam = BoltNetwork.Instantiate(ionBeamLaserPrefab, new Vector3(camPosition.x, 0, camPosition.z), Quaternion.identity);

                Ownership IonOwnership = GetComponent<Ownership>();
                Ownership itemOwnership = IonBeam.GetComponent<Ownership>();

                itemOwnership.OwnerKartRoot = IonOwnership.OwnerKartRoot;
                itemOwnership.Team = IonOwnership.Team;

                IonBeam.transform.position = new Vector3(_ionBeamCam.transform.position.x, IonBeam.transform.position.y, _ionBeamCam.transform.position.z);
                MyExtensions.AudioExtensions.PlayClipObjectAndDestroy(LaunchSource);
                _ionBeamCam.Composer.enabled = true;
                _ionBeamCam.IonBeamCameraBehaviour(false);
                if (entity.isOwner)
                    StartCoroutine(DelayBeforeInputsChange());
                _isFiring = true;
            }
        }

        //PRIVATE

        IEnumerator DelayBeforeDisablePlayerInputs()
        {
            _ionBeamOwner.GetComponentInChildren<SteeringWheel>().CanSteer = false;
            _ionBeamOwner.GetComponentInChildren<SteeringWheel>().ResetAxisValue();
            _ionBeamOwner.GetComponentInChildren<EngineBehaviour>().enabled = false;
            while (!_ionBeamCam.IsCameraOnTop())
            {
                yield return new WaitForSeconds(0.1f);
            }
            _ionBeamInputs.enabled = true;
        }

        IEnumerator DelayBeforeInputsChange()
        {
            yield return new WaitForSeconds(1);
            _ionBeamCam.GetComponent<IonBeamCamera>().enabled = false;
            _ionBeamInputs.enabled = false;
            _ionBeamOwner.GetComponentInChildren<SteeringWheel>().CanSteer = true;
            _ionBeamOwner.GetComponentInChildren<EngineBehaviour>().enabled = true;
            _ionBeamCam.GetComponent<CameraTurnEffect>().EnableTurnEffectInput();
            BoltNetwork.Destroy(gameObject);
        }
    }
}
