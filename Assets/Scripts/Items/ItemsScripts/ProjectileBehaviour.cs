﻿using UnityEngine;
using System.Collections;
using Multiplayer;

namespace Items
{
    [RequireComponent(typeof(AudioSource))]
    public class ProjectileBehaviour : NetworkDestroyable
    {
        #region Variables
        [Header("Owner")]
        public Ownership Ownership;

        [Header("Projectile parameters")]
        public float Speed;
        public bool DestroyAfterHit = true;

        [Header("Ground parameters")]
        public float DistanceForGrounded;
        public float LocalGravity;

        [Header("Particles Effects")]
        public ParticleSystem CollisionParticles;
        public int ParticlesToEmitOnHit = 2000;

        [Header("Sounds")]
        public AudioSource LaunchSource;
        public AudioSource FlySource;
        public AudioSource PlayerHitSource;
        public AudioSource CollisionSource;

        [Header("Invincibility")]
        [SerializeField] private const float _ownerImmunityDuration = 0.5f;
        [SerializeField] private bool _ownerImmune = true;

        protected Rigidbody rb;
        #endregion

        // CORE

        protected void Awake()
        {
            rb = GetComponent<Rigidbody>();
            Ownership = GetComponent<Ownership>();
        }

        protected void Start()
        {
            StartCoroutine(StartOwnerImmunity());
        }

        protected void Update()
        {
            CheckGrounded();
        }

        protected void FixedUpdate()
        {
            NormalizeSpeed();
            ApplyLocalGravity();
        }

        // PUBLIC

        #region Audio
        protected void PlayLaunchSound()
        {
            LaunchSource.Play();
        }

        protected void PlayFlySound()
        {
            FlySource.loop = true;
            FlySource.Play();
        }

        protected void PlayPlayerHitSound()
        {
            MyExtensions.AudioExtensions.PlayClipObjectAndDestroy(PlayerHitSource);
        }

        protected void PlayCollisionSound()
        {
            MyExtensions.AudioExtensions.PlayClipObjectAndDestroy(CollisionSource);
        }
        #endregion

        public void CheckCollision(GameObject kartCollisionObject)
        {
            var otherPlayer = kartCollisionObject.GetComponentInParent<PlayerSettings>();

            if (Ownership.IsNotSameTeam(otherPlayer) || (Ownership.IsMe(otherPlayer.gameObject) && !_ownerImmune))
            {
                Debug.LogErrorFormat("{0} was hit with {1} hp", otherPlayer.Nickname, kartCollisionObject.GetComponent<Health.Health>().state.Health);
                kartCollisionObject.GetComponent<Health.Health>().LoseHealth();
                OnHit();
            }
        }

        // PROTECTED

        protected void NormalizeSpeed()
        {
            var newVelocity = rb.velocity;
            newVelocity.y = 0;
            newVelocity = newVelocity.normalized * Speed;
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;
        }

        // PRIVATE

        private void OnHit()
        {
            CollisionParticles.Emit(ParticlesToEmitOnHit);
            PlayPlayerHitSound();
            if (DestroyAfterHit)
            {
                DestroyObject();
            }
        }

        private IEnumerator StartOwnerImmunity()
        {
            _ownerImmune = true;
            yield return new WaitForSeconds(_ownerImmunityDuration);
            _ownerImmune = false;
        }

        private void CheckGrounded()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, DistanceForGrounded, 1 << LayerMask.NameToLayer(Constants.Layer.Ground)))
            {
                rb.useGravity = false;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                transform.position = new Vector3(transform.position.x, hit.point.y + DistanceForGrounded - 0.1f, transform.position.z);
            }
            else
            {
                rb.useGravity = true;
            }
        }
        private void ApplyLocalGravity()
        {
            if (rb.useGravity == true)
            {
                rb.AddForce(Vector3.down * LocalGravity);
            }
        }
    }
}
