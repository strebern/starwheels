﻿using System.Collections;
using UnityEngine;

namespace Items
{
    public class IonBeamLaserBehaviour : MonoBehaviour
    {
        //  [SerializeField] private ProjectileBehaviour projectileBehaviour;

        [SerializeField] private IonBeamLaserSettings ionBeamLaserSettings;
        [SerializeField] private GameObject EffectiveAOE;
        [SerializeField] private GameObject WarningPosition;
        [SerializeField] private AudioSource ExplosionSource;

        private Vector2 offset;
        private bool _damagePlayer = false;

        //CORE

        private void Awake()
        {
            ionBeamLaserSettings.onExplode = true;
            GameObject owner = GetComponent<Ownership>().gameObject;
            //float currentTimer = WarningPosition.transform.localScale.x;
        }

        private void Update()
        {
            offset = new Vector2(0, Time.deltaTime * ionBeamLaserSettings.SpeedOffset);
            if (EffectiveAOE != null)
                EffectiveAOE.GetComponent<Renderer>().material.mainTextureOffset += offset;

            if (WarningPosition != null)
            {
                if (WarningPosition.transform.localScale.x >= ionBeamLaserSettings.MaxWarningScale)
                {
                    if (WarningPosition.transform.localScale.x >= ionBeamLaserSettings.MaxWarningScale / 2)
                    {
                        GrowingAoeWarning(ionBeamLaserSettings.GrowingWarningSpeed / 2);
                    }
                    else
                    {
                        GrowingAoeWarning(ionBeamLaserSettings.GrowingWarningSpeed);
                    }
                }
                else
                {
                    if (ionBeamLaserSettings.onExplode)
                        Explosion();
                }
            }
        }

        // PUBLIC

        public void GrowingAoeWarning(float growSpeed)
        {
            float IncreaseSpeed = growSpeed * Time.deltaTime;
            WarningPosition.transform.localScale += new Vector3(-IncreaseSpeed, 0, -IncreaseSpeed);
        }

        public void Explosion()
        {
            if (ionBeamLaserSettings.onExplode)
            {
                Destroy(EffectiveAOE);
                Destroy(WarningPosition);
                StartCoroutine(ParticuleEffect());
                ionBeamLaserSettings.onExplode = false;
            }
        }

        //PRIVATE

        IEnumerator ParticuleEffect()
        {
            GetComponent<ParticleSystem>().Emit(3000);
            MyExtensions.Audio.PlayClipObjectAndDestroy(ExplosionSource);
            _damagePlayer = true;
            yield return new WaitForSeconds(0.1f);
            _damagePlayer = false;
            yield return new WaitForSeconds(1);
            Destroy(gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            /*
            if (other.gameObject != null)
            {
                if (DamagePlayer)
                {
                    //SendTargetOnHitEvent(other.gameObject);
                }
            }
            */
            if (_damagePlayer)
            {
                if (other.gameObject.CompareTag(Constants.Tag.HealthHitBox))
                {
                    Debug.Log("Hit");
                    //  projectileBehaviour.CheckCollision(other.gameObject);
                }
            }
        }
    }
}
