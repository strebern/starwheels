﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Bolt;
using Multiplayer.Teams;

namespace SW.Matchmaking
{
    [DisallowMultipleComponent]
    public class LobbyPlayerInfoEntry : GlobalEventListener
    {
        [Header("Information")]
        public string Nickname;
        public int Team;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _nicknameText;
        [SerializeField] private Image _teamColorImage;

        [Header("Ressources")]
        [SerializeField] private GamemodesTeamsListSettings _gamemodesTeamsListSettings;

        //CORE

        private void Awake()
        {
            _teamColorImage.color = Color.grey;
            Team = 0;
        }

        //BOLT

        public override void OnEvent(UpdateTeamColorInLobby evnt)
        {
            Team = evnt.PlayerTeamColor;
            SetTeamColorDisplay(evnt.PlayerNickname, evnt.PlayerTeamColor.ToTeam());
        }

        // PUBLIC

        public void SetNickname(string nickname)
        {
            Nickname = nickname;
            _nicknameText.text = nickname;
        }

        public void ChangeTeamColorRequest()
        {
            Debug.LogError("COLOR REQUEST");
            TeamColorChangeRequest teamColorChangeRequest = TeamColorChangeRequest.Create();
            teamColorChangeRequest.PlayerNickname = Nickname;

            if (_teamColorImage.color == Color.grey)
            {
                teamColorChangeRequest.PlayerActualTeam = 0;
            }
            else
            {
                teamColorChangeRequest.PlayerActualTeam = Team;
            }

            teamColorChangeRequest.Send();
        }

        public void ResetToNoColor()
        {
            _teamColorImage.color = Color.grey;
        }

        public void SetTeamColorDisplay(string nickname, Team team)
        {
            if (Nickname == nickname)
            {
                foreach (TeamsListSettings teamList in _gamemodesTeamsListSettings.TeamsLists)
                {
                    foreach (TeamColorSettings teamColor in teamList.TeamsList)
                    {
                        if (team.ToString() == teamColor.TeamName)
                        {
                            _teamColorImage.color = teamColor.MenuColor;
                        }
                    }
                }
            }
        }
    }
}
