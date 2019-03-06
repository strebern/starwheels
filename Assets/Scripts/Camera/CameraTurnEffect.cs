﻿using UnityEngine;
using Cinemachine;

namespace CameraUtils
{
    public class CameraTurnEffect : MonoBehaviour, IControllable
    {
        [SerializeField] private bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        private string _turnCamInputName = "RightJoystickHorizontal";
        private CinemachineOrbitalTransposer _orbiter;
        private CinemachineComposer _composer;
        private CinemachineVirtualCamera _cinemachine;

        private float _controlAxisValueMin;
        private float _controlAxisValueMax;

        private void Awake()
        {
            _cinemachine = GetComponentInParent<CinemachineVirtualCamera>();
            _orbiter = _cinemachine.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            _composer = _cinemachine.GetCinemachineComponent<CinemachineComposer>();
            _controlAxisValueMin = _orbiter.m_XAxis.m_MinValue;
            _controlAxisValueMax = _orbiter.m_XAxis.m_MaxValue;
        }

        private void Start()
        {
            _orbiter.m_XAxis.m_InputAxisName = _turnCamInputName;
        }

        private void Update()
        {
            MapInputs();
        }

        // PUBLIC

        public void MapInputs()
        {
            if (Enabled)
            {
                if (Input.GetButtonDown(Constants.Input.ResetCamera))
                {
                    CameraReset();
                }

                ClampMaxAngle(Input.GetAxis(Constants.Input.TurnCamera));

                if (Input.GetAxis(Constants.Input.UpAndDownCamera) >= 0.1f)
                {
                    if (Mathf.Abs(_composer.m_TrackedObjectOffset.y) <= 3)
                        _composer.m_TrackedObjectOffset.y += 0.2f;
                }
                else if (Input.GetAxis(Constants.Input.UpAndDownCamera) <= -0.1f)
                {
                    if (Mathf.Abs(_composer.m_TrackedObjectOffset.y) <= 3)
                        _composer.m_TrackedObjectOffset.y -= 0.2f;
                }
                else
                {
                    _composer.m_TrackedObjectOffset.y = 0;
                }

                WhenToRecenterEnableCam(Input.GetAxis(Constants.Input.TurnCamera));
            }
        }

        public void DisableTurnEffectInput()
        {
            _orbiter.m_XAxis.m_InputAxisName = "";
            _orbiter.m_XAxis.m_MinValue = 0;
            _orbiter.m_XAxis.m_MaxValue = 0;
        }

        public void EnableTurnEffectInput()
        {
            _orbiter.m_XAxis.m_InputAxisName = _turnCamInputName;
            _orbiter.m_XAxis.m_MinValue = _controlAxisValueMin;
            _orbiter.m_XAxis.m_MaxValue = _controlAxisValueMax;
        }

        public void CenterCamera()
        {
            _orbiter.m_XAxis.Value = 0;
        }

        // PRIVATE


        private void ClampMaxAngle(float xAxisValue)
        {
            _orbiter.m_XAxis.m_MinValue = -Mathf.Abs(xAxisValue) * 125;
            _orbiter.m_XAxis.m_MaxValue = Mathf.Abs(xAxisValue) * 125;
        }

        private void WhenToRecenterEnableCam(float value)
        {
            if (Mathf.Abs(_orbiter.m_XAxis.Value) >= 1f)
                _orbiter.m_RecenterToTargetHeading.m_enabled = true;
            else
                _orbiter.m_RecenterToTargetHeading.m_enabled = false;
        }

        private void CameraReset()
        {
            _orbiter.m_XAxis.Value = 0;
        }
    }
}
