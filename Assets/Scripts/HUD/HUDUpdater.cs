﻿using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace HUD
{
    public class HUDUpdater : MonoBehaviour
    {
        public Text SpeedText;
        public Text TimeText;

        private Rigidbody kartRigidBody;

        private void Start()
        {
            kartRigidBody = GameObject.FindGameObjectWithTag(Constants.KartTag).GetComponent<Rigidbody>();
            StartCoroutine(UpdateRoutine());
        }

        IEnumerator UpdateRoutine()
        {
            while (Application.isPlaying)
            {
                TimeText.text = "Time : " + Time.time;
                SpeedText.text = "Speed : " + kartRigidBody.velocity.magnitude;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}