﻿using UnityEngine;
using Bolt;

namespace Gamemodes.Totem
{
    [DisallowMultipleComponent]
    public class TotemSpawner : GlobalEventListener
    {
        private bool _totemInstantiated = false;

        // DEBUG

        private void Update()
        {
            if(BoltNetwork.IsServer && Input.GetKeyDown(KeyCode.Alpha5))
            {
                RespawnTotem();
            }
        }

        // BOLT

        public override void BoltStartDone()
        {
            InstantiateTotem();
        }

        public override void SceneLoadLocalDone(string map)
        {
            InstantiateTotem();
        }

        // PUBLIC

        public void RespawnTotem()
        {
            var totem = GameObject.FindGameObjectWithTag(Constants.Tag.Totem);
            totem.GetComponent<TotemOwnership>().UnsetOwner();
            totem.GetComponent<TotemPhysics>().SetVelocityToZero();
            totem.GetComponent<TotemPhysics>().EnableCollider(true);
            totem.transform.position = transform.position;

            TotemThrown totemThrownEvent = TotemThrown.Create();
            totemThrownEvent.OwnerID = -1;
            totemThrownEvent.Send();
        }

        private void InstantiateTotem()
        {
            if(BoltNetwork.IsConnected && BoltNetwork.IsServer && !_totemInstantiated)
            {
                BoltNetwork.Instantiate(BoltPrefabs.Totem, transform.position, transform.rotation);
                _totemInstantiated = true;
            }
        }
    }
}
