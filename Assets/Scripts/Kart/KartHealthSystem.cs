﻿using UnityEngine;


namespace Kart
{
    public class KartHealthSystem : MonoBehaviour
    {
        public int Health = 3;

        private KartEffects kartEffects;
        private KartOrientation KartOrientation;

        private void Awake()
        {
            kartEffects = GetComponentInChildren<KartEffects>();
            KartOrientation = GetComponent<KartOrientation>();
        }

        public void HealtLoss()
        {
            Health--;
            KartOrientation.LooseHealth(2.5f);
            kartEffects.HealthParticlesManagement(Health);
        }
    }
}
