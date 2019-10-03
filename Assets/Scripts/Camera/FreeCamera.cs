﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

namespace CameraUtils
{
    public class FreeCamera : MonoBehaviour, IControllable, IObserver
    {
        [SerializeField] private bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        [SerializeField] private bool _switchInputs;

        [SerializeField] private float _horizontalSpeed;
        [SerializeField] private float _verticalSpeed;
        [SerializeField] private float _rotateYSpeed;
        [SerializeField] private float _rotateXSpeed;
        [SerializeField] private float _forwardSpeed;
        [SerializeField] private float _backwardSpeed;
        [SerializeField] private float _maximalDistance = 150.0f;
        [SerializeField] private float _maximalHeigh = 50.0f;
        [SerializeField] private float _minimalHeigh = 50.0f;
        [SerializeField] private float _rayDist = 1.0f;
        [SerializeField] private LayerMask _rayMask;

        private GameObject _kart;

        //CORE

        private void Update()
        {
            MapInputs();
        }

        //PUBLIC

        public void Observe(GameObject kartroot)
        {
            _kart = kartroot;
        }

        public void MapInputs()
        {
            if (Enabled)
            {
                transform.eulerAngles += new Vector3(0, Input.GetAxis(Constants.Input.TurnCamera) * _rotateXSpeed, 0);
                transform.eulerAngles += new Vector3(-Input.GetAxis(Constants.Input.UpAndDownCamera) * _rotateYSpeed, 0, 0);
                if (Input.GetAxis(Constants.Input.TurnCamera) == 0)
                {
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        transform.eulerAngles += new Vector3(0, -_rotateXSpeed, 0);
                    }
                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        transform.eulerAngles += new Vector3(0, _rotateXSpeed, 0);
                    }
                }


                if (_switchInputs)
                {
                    transform.Translate(Vector3.forward * Input.GetAxis(Constants.Input.Accelerate) * _forwardSpeed);
                    transform.Translate(Vector3.back * Input.GetAxis(Constants.Input.Decelerate) * _backwardSpeed);
                    transform.Translate(Vector3.right * Input.GetAxis(Constants.Input.TurnAxis) * _horizontalSpeed);
                    transform.Translate(Vector3.up * Input.GetAxis(Constants.Input.UpAndDownAxis) * _verticalSpeed);
                }
                else
                {
                    transform.Translate(Vector3.forward * Input.GetAxis(Constants.Input.UpAndDownAxis) * _forwardSpeed);
                    transform.Translate(Vector3.right * Input.GetAxis(Constants.Input.TurnAxis) * _horizontalSpeed);
                    transform.Translate(Vector3.up * Input.GetAxis(Constants.Input.StickTiggersUp) * _verticalSpeed);
                    transform.Translate(Vector3.down * Input.GetAxis(Constants.Input.StickTiggersDown) * _verticalSpeed);

                    if (Input.GetAxis(Constants.Input.UpAndDownAxis) == 0)
                    {
                        if (Input.GetKey(KeyCode.Z))
                        {
                            transform.Translate(Vector3.forward * _forwardSpeed);
                        }
                        if (Input.GetKey(KeyCode.S))
                        {
                            transform.Translate(Vector3.back * _forwardSpeed);
                        }
                    }
                    if (Input.GetAxis(Constants.Input.TurnAxis) == 0)
                    {
                        if (Input.GetKey(KeyCode.A))
                        {
                            transform.Translate(Vector3.left * _forwardSpeed);
                        }
                        if (Input.GetKey(KeyCode.E))
                        {
                            transform.Translate(Vector3.right * _forwardSpeed);
                        }
                    }

                    if (Input.GetAxis(Constants.Input.Accelerate) == 0)
                    {
                        if (Input.GetKey(KeyCode.Space))
                        {
                            transform.Translate(Vector3.up * _verticalSpeed);
                        }
                    }

                    if (Input.GetAxis(Constants.Input.Decelerate) == 0)
                    {
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            transform.Translate(Vector3.down * _verticalSpeed);
                        }
                    }
                }

                if (transform.position.x < -_maximalDistance)
                {
                    transform.position = new Vector3(-_maximalDistance, transform.position.y, transform.position.z);
                }
                else if (transform.position.x > _maximalDistance)
                {
                    transform.position = new Vector3(_maximalDistance, transform.position.y, transform.position.z);
                }

                if (transform.position.z < -_maximalDistance)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, -_maximalDistance);
                }
                else if (transform.position.z > _maximalDistance)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, _maximalDistance);
                }

                if (transform.position.y > _maximalHeigh)
                {
                    transform.position = new Vector3(transform.position.x, _maximalHeigh, transform.position.z );
                }
                else if (transform.position.y < -_minimalHeigh)
                {
                    transform.position = new Vector3(transform.position.x, -_minimalHeigh, transform.position.z);
                }
            }
        }

        public void DisableKartControls()
        {
            _kart.GetComponentInChildren<EngineBehaviour>().Enabled = false;
        }
        public void EnableKartControls()
        {
            _kart.GetComponentInChildren<EngineBehaviour>().Enabled = true;
        }

        private bool testForObstacles(Vector3 _direction) // return true if obstacle
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position, _direction, out _hit, _rayDist, _rayMask ))
            {
                if (_hit.collider == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
