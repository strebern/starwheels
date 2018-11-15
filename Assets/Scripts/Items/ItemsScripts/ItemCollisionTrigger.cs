﻿using UnityEngine;
using UnityEngine.Events;
using Bolt;

namespace Items
{
    public class ItemCollisionTrigger : EntityBehaviour
    {
        [Header("Events")]
        public UnityEvent OnCollision;

        [Header("Me")]
        public ItemCollision ItemCollision;

        protected void OnTriggerEnter(Collider other)
        {
            if (BoltNetwork.isServer)
            {
                if (other.gameObject.CompareTag(Constants.Tag.CollisionHitBox))
                {
                    var otherItemCollision = other.GetComponent<ItemCollision>();

                    if (ItemCollision.ShouldBeDestroyed(otherItemCollision))
                    {
                        OnCollision.Invoke();
                        DestroySelf();
                    }
                }
            }
        }

        private void DestroySelf()
        {
            BoltNetwork.Destroy(gameObject);
        }
    }
}
