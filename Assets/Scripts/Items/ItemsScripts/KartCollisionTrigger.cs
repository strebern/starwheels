﻿using UnityEngine;
using Bolt;

namespace Items
{
    public class KartCollisionTrigger : EntityBehaviour<IKartState>
    {
        [SerializeField] private ItemCollision _itemCollision;

        [Header("Invincibility Condition")]
        [SerializeField] private Health.Health _health;

        public void CheckTargetInformationsBeforeSendingHitEvent(Collider target)
        {
            var itemCollisionTrigger = target.GetComponent<ItemCollisionTrigger>();

            if (itemCollisionTrigger.ItemCollision.HitsPlayer) // It is an item that damages the player
            {
                BoltEntity itemEntity = target.GetComponentInParent<BoltEntity>();
                Ownership itemOwnership = itemEntity.GetComponent<Ownership>();

                if (itemEntity.isAttached) // It is a concrete item & it is attached
                {
                    if ((int)itemOwnership.Team != state.Team)
                    {
                        if (!_health.IsInvincible) // The server checks that this kart is not invincible
                        {
                            SendPlayerHitEvent(itemOwnership);
                        }
                        DestroyColliderObject(target);
                    }
                }
            }
        }

        /*
        private void OnTriggerEnter(Collider other)
        {
            if (BoltNetwork.IsServer && entity.isAttached)
            {
                if (other.gameObject.CompareTag(Constants.Tag.ItemCollisionHitBox)) // It is an item collision
                {
                    var itemCollisionTrigger = other.GetComponent<ItemCollisionTrigger>();

                    if (itemCollisionTrigger.ItemCollision.HitsPlayer) // It is an item that damages the player
                    {
                        BoltEntity itemEntity = other.GetComponentInParent<BoltEntity>();
                        Ownership itemOwnership = itemEntity.GetComponent<Ownership>();

                        if (itemEntity.isAttached) // It is a concrete item & it is attached
                        {
                          //   Hit Ourself
                          //  if (itemState.OwnerID == state.OwnerID)
                          //  {
                          //      if (itemCollisionTrigger.ItemCollision.ItemName == ItemCollisionName.Disk)
                          //      {
                          //          if (other.GetComponentInParent<DiskBehaviour>().CanHitOwner)
                          //          {
                          //              if (!_health.IsInvincible) // The server checks that this kart is not invincible
                          //              {
                          //                  SendPlayerHitEvent(itemState);
                          //              }
                          //              DestroyColliderObject(other);
                          //          }
                          //      }
                          //  }

                            if ((int)itemOwnership.Team != state.Team)
                            {
                                if (!_health.IsInvincible) // The server checks that this kart is not invincible
                                {
                                    SendPlayerHitEvent(itemOwnership);
                                }
                                DestroyColliderObject(other);
                            }
                        }
                    }
                }
            }
        } */

        private void DestroyColliderObject(Collider other)
        {
            var otherItemCollision = other.GetComponent<ItemCollisionTrigger>().ItemCollision;
            if (otherItemCollision.ShouldBeDestroyed(_itemCollision)
                && otherItemCollision.ItemName != ItemCollisionName.IonBeamLaser) // The item should be destroyed
            {
                DestroyEntity destroyEntityEvent = DestroyEntity.Create();
                destroyEntityEvent.Entity = other.GetComponentInParent<BoltEntity>();
                destroyEntityEvent.Send();
            }
        }

        private void SendPlayerHitEvent(Ownership itemOwnership)
        {
            PlayerHit playerHitEvent = PlayerHit.Create();
            playerHitEvent.KillerID = itemOwnership.OwnerID;
            playerHitEvent.KillerName = itemOwnership.OwnerNickname;
            playerHitEvent.KillerTeam = (int) itemOwnership.Team;
            playerHitEvent.Item = itemOwnership.Label;
            playerHitEvent.VictimEntity = entity;
            playerHitEvent.VictimID = state.OwnerID;
            playerHitEvent.VictimName = GetComponentInParent<Multiplayer.PlayerInfo>().Nickname;
            playerHitEvent.VictimTeam = state.Team;
            playerHitEvent.Send();
        }
    }
}
