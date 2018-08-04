﻿using UnityEngine;

namespace FX
{
    public class KartEffects : BaseKartComponent
    {
        [HideInInspector] public ParticleSystem smokeLeftWheel;
        [HideInInspector] public ParticleSystem smokeRightWheel;

        public ParticleSystem[] Lifes;
        public ParticleSystem[] LifeBursts;
        [Space(10)]
        [Header("Particles")]
        public ParticleSystem MainJumpParticles;
        public ParticleSystem JumpReloadParticles;
        public int NumberOfParticles = 300;

        private new void Awake()
        {
            base.Awake();
            StopSmoke();

            // Events
            kartEvents.OnJump += MainJumpParticlesEmit;
            kartEvents.OnDoubleJumpReset += ReloadJumpParticlesEmit;

            kartEvents.OnHealthLoss += HealthParticlesManagement;

            kartEvents.OnDriftStart += StartSmoke;
            kartEvents.OnDriftReset += StopSmoke;
            kartEvents.OnDriftReset += () => SetWheelsColor(Color.white);

            kartEvents.OnDriftOrange += () => SetWheelsColor(Color.yellow);
            kartEvents.OnDriftRed += () => SetWheelsColor(Color.red);
            kartEvents.OnDriftBoost += () => SetWheelsColor(Color.green);
        }

        public void StopSmoke()
        {
            smokeLeftWheel.Stop(true);
            smokeRightWheel.Stop(true);
        }
        public void StartSmoke()
        {
            if (!smokeLeftWheel.isPlaying)
                smokeLeftWheel.Play(true);
            if (!smokeRightWheel.isPlaying)
                smokeRightWheel.Play(true);
        }

        public void HealthParticlesManagement(int health)
        {
            Lifes[kartHub.kartHealthSystem.Health].Stop(true);
            LifeBursts[health].Play();
        }

        public void ResetLives()
        {
            foreach (ParticleSystem ps in Lifes)
            {
                ps.Play();
            }
        }

        public void MainJumpParticlesEmit()
        {
            MainJumpParticles.Emit(NumberOfParticles);
        }

        public void ReloadJumpParticlesEmit()
        {
            JumpReloadParticles.Emit(NumberOfParticles);
        }       

        public void SetWheelsColor(Color color)
        {
            var leftWheelMain = smokeLeftWheel.main;
            var rightWheelMain = smokeRightWheel.main;

            leftWheelMain.startColor = color;
            rightWheelMain.startColor = color;
        }
    }
}

