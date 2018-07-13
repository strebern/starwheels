﻿using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Items;

namespace HUD
{
    public class HUDUpdater : MonoBehaviour
    {
        public Text SpeedText;
        public Text TimeText;
        public Text FPSText;

        public Texture MineImage;
        public Texture RocketImage;
        public Texture DiskImage;
        public Texture NitroImage;


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
                FPSText.text = "FPS : " + 1.0f / Time.deltaTime;
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void UpdateItems()
        {
            // To Do
        }
    }
}