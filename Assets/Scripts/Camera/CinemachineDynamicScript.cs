﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Controls;

public class CinemachineDynamicScript : MonoBehaviour
{
    [Range(7.5f, 15)] public float MaxDistanceCamInBoost;
    public float SpeedCamMovements;

    private CinemachineVirtualCamera cinemachine;
    private Coroutine cameraBoostCoroutine;
    private Coroutine cameraIonBeamBehaviour;
    public CinemachineTransposer transposer;
    private CinemachineComposer composer;

    private float currentTimer;

    private void Awake()
    {
        cinemachine = GetComponent<CinemachineVirtualCamera>();
        transposer = cinemachine.GetCinemachineComponent<CinemachineTransposer>();
        composer = cinemachine.GetCinemachineComponent<CinemachineComposer>();
    }

    public void IonBeamCameraControls(float horizontal, float vertical)
    {
        transposer.m_FollowOffset.z += horizontal * SpeedCamMovements * Time.deltaTime;
        transposer.m_FollowOffset.x += vertical * SpeedCamMovements * Time.deltaTime;

    }

    public void BoostCameraBehaviour()
    {
        if (cameraBoostCoroutine != null)
            StopCoroutine(cameraBoostCoroutine);
        cameraBoostCoroutine = StartCoroutine(CameraBoostBehaviour(-7.5f, -MaxDistanceCamInBoost, 0.5f));
        currentTimer = 0f;
    }

    public void SpeedOnCamBehaviour()
    {
        //TODO  effet de la vitesse du kart sur l'eloignement de la cam
    }

    public void AimAndFollow(bool value)
    {
        if (value)
        {
            cinemachine.AddCinemachineComponent<CinemachineComposer>();
            composer = cinemachine.GetCinemachineComponent<CinemachineComposer>();
            composer.m_ScreenY = 0.75f;
            composer.m_SoftZoneHeight = 0f;
            composer.m_SoftZoneWidth = 0f;
            composer.m_LookaheadSmoothing = 3f;
        }
        else
        {
            cinemachine.DestroyCinemachineComponent<CinemachineComposer>();
            IonBeamInputs.IonBeamControlMode = true;
        }
    }

    public void IonBeamCameraBehaviour(bool direction)
    {
        if (direction)
        {
            if (cameraIonBeamBehaviour != null)
                StopCoroutine(cameraIonBeamBehaviour);
            cameraIonBeamBehaviour = StartCoroutine(CameraIonBeamExpand(0, 200, 1f));
        }
        else
        {
            AimAndFollow(true);
            if (cameraIonBeamBehaviour != null)
                StopCoroutine(cameraIonBeamBehaviour);
            cameraIonBeamBehaviour = StartCoroutine(CameraIonBeamReset(-7.5f, 3, 1.5f));
        }
    }

    IEnumerator CameraIonBeamExpand(float endValueZ, float endValueY, float boostDuration)
    {
        float startDynamicCamValueZ = transposer.m_FollowOffset.z;
        float startDynamicCamValueY = transposer.m_FollowOffset.y;

        currentTimer = 0f;
        while (currentTimer < boostDuration)
        {
            transposer.m_FollowOffset.z = Mathf.Lerp(startDynamicCamValueZ, endValueZ, currentTimer / boostDuration);
            transposer.m_FollowOffset.y = Mathf.Lerp(startDynamicCamValueY, endValueY, currentTimer / boostDuration);
            currentTimer += Time.deltaTime;
            yield return null;
        }
        AimAndFollow(false);
    }

    IEnumerator CameraIonBeamReset(float returnValueZ, float returnValueY, float boostDuration)
    {
        float startDynamicCamValueX = transposer.m_FollowOffset.x;
        float startDynamicCamValueZ = transposer.m_FollowOffset.z;
        float startDynamicCamValueY = transposer.m_FollowOffset.y;

        currentTimer = 0f;

        while (currentTimer < boostDuration)
        {
            transposer.m_FollowOffset.x = Mathf.Lerp(startDynamicCamValueX, 0, currentTimer / boostDuration);
            transposer.m_FollowOffset.z = Mathf.Lerp(startDynamicCamValueZ, returnValueZ, currentTimer / boostDuration);
            transposer.m_FollowOffset.y = Mathf.Lerp(startDynamicCamValueY, returnValueY, currentTimer / boostDuration);
            currentTimer += Time.deltaTime;
            yield return null;
        }
        if (transposer.m_FollowOffset.y > returnValueY)
        {
            // Security for lack of precision of Time.deltaTime
            cameraIonBeamBehaviour = StartCoroutine(CameraIonBeamReset(returnValueZ, returnValueY, 0.5f));
        }
    }

    IEnumerator CameraBoostBehaviour(float startValue, float endValue, float boostDuration)
    {
        float startDynamicCamValue = transposer.m_FollowOffset.z;

        currentTimer = 0f;
        while (currentTimer < boostDuration)
        {
            transposer.m_FollowOffset.z = Mathf.Lerp(startDynamicCamValue, endValue, currentTimer / boostDuration);
            currentTimer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        currentTimer = 0f;
        while (currentTimer < (boostDuration * 5))
        {
            transposer.m_FollowOffset.z = Mathf.Lerp(endValue, startValue, currentTimer / (boostDuration * 5));
            currentTimer += Time.deltaTime;
            yield return null;
        }
    }
}
