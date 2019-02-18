﻿using UnityEngine;
using Photon.Lobby;
using Bolt;
using Multiplayer;
using Multiplayer.Teams;
using System.Collections;

namespace Network
{
    public class KartMultiplayerSetup : EntityBehaviour<IKartState>
    {
        [SerializeField] private PlayerSettings _playerSettings;
        [SerializeField] private float _delayBeforeDestroyKart;

        private void Awake()
        {
            if (!BoltNetwork.IsConnected)
            {
                BoltLauncher.StartServer();
            }
        }

        // BOLT

        public override void Attached()
        {
            state.SetTransforms(state.Transform, transform);
            state.SetAnimator(GetComponentInChildren<Animator>());
            state.AddCallback("Team", ColorChanged);
            state.AddCallback("Nickname", NameChanged);

            if (entity.isOwner)
            {
                state.Team = _playerSettings.ColorSettings.BoltColor;
                state.Nickname = _playerSettings.Nickname;
                state.OwnerID = _playerSettings.ConnectionID;

                PlayerReady playerReadyEvent = PlayerReady.Create();
                playerReadyEvent.Team = state.Team;
                playerReadyEvent.Send();
            }

            var lobby = GameObject.Find("LobbyManager");
            if(lobby) lobby.SetActive(false);

            GetComponentInChildren<Camera>().enabled = entity.isOwner;
        }

        // PRIVATE

        private void ColorChanged()
        {
            GetComponent<Player>().Team = state.Team.GetTeam();
            var panel = GetComponentInChildren<Common.HUD.NicknamePanel>();
            if (panel) panel.SetFrameRendererColor(state.Team);
        }

        private void NameChanged()
        {
            GetComponent<Player>().Nickname = state.Nickname;
            var panel = GetComponentInChildren<Common.HUD.NicknamePanel>();
            if(panel) panel.SetName(state.Nickname);
        }
    }
}
