﻿using System.Collections.Generic;
using UnityEngine;
using Bolt;

namespace Multiplayer
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public class SpawnAssigner : GlobalEventListener
    {
        private List<GameObject> _initialBlueSpawns = new List<GameObject>();
        private List<GameObject> _initialRedSpawns = new List<GameObject>();
        private List<GameObject> _blueSpawns = new List<GameObject>();
        private List<GameObject> _redSpawns = new List<GameObject>();
        private int _playersCount = -1;
        private int _spawnsAssigned = 0;
        private bool _gameIsReady = false;
        private PlayerSettings _serverPlayerSettings;

        // CORE

        private void Awake()
        {
            _serverPlayerSettings = Resources.Load<PlayerSettings>("PlayerSettings");
        }

        // BOLT

        public override void SceneLoadLocalDone(string map, IProtocolToken token)
        {
            _redSpawns = new List<GameObject>(GameObject.FindGameObjectsWithTag(Constants.Tag.RedSpawn));
            Debug.Log("Red spawns : " + _redSpawns.Count);
            _initialRedSpawns = _redSpawns;
            Debug.Log("Red spawns : " + _initialRedSpawns.Count);
            _blueSpawns = new List<GameObject>(GameObject.FindGameObjectsWithTag(Constants.Tag.BlueSpawn));
            Debug.Log("Blue spawns : " + _blueSpawns.Count);
            _initialBlueSpawns = _blueSpawns;
            Debug.Log("Blue spawns : " + _initialBlueSpawns.Count);

            var myKart = BoltNetwork.Instantiate(BoltPrefabs.Kart);
            var serverColor = Teams.TeamsColors.GetTeamFromColor(_serverPlayerSettings.Team);
            var spawn = GetInitialSpawnPosition(serverColor);
            myKart.transform.position = spawn.transform.position;
            myKart.transform.rotation = spawn.transform.rotation;
            FindObjectOfType<CameraUtils.SetKartCamera>().SetKart(myKart);

            var roomToken = (Photon.RoomProtocolToken)token;
            if (System.Int32.TryParse(roomToken.RoomInfo, out _playersCount))
            {
                IncreaseSpawnCount();
            }
        }

        public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
        {
            var playerTeam = Teams.TeamsColors.GetTeamFromColor((Color)connection.UserData);
            AssignSpawn((int)connection.ConnectionId, playerTeam);
            IncreaseSpawnCount();
        }

        public override void OnEvent(KartDestroyed evnt)
        {
            AssignSpawn(evnt.ConnectionID);
        }

        // PUBLIC

        // PRIVATE

        private void AssignSpawn(int connectionID, Team team = Team.None)
        {
            PlayerSpawn playerSpawn = PlayerSpawn.Create();
            playerSpawn.ConnectionID = connectionID;
            var spawn = GetInitialSpawnPosition(team);
            playerSpawn.SpawnPosition = spawn.transform.position;
            playerSpawn.SpawnRotation = spawn.transform.rotation;
            playerSpawn.Send();
        }

        private GameObject GetInitialSpawnPosition(Team team)
        {
            if (_initialRedSpawns.Count > 0)
            {
                GameObject spawn = null;
                int randomIndex = 0;

                switch (team)
                {
                    case Team.Blue:
                        randomIndex = Random.Range(0, _initialBlueSpawns.Count);
                        spawn = _initialBlueSpawns[randomIndex];
                        _initialBlueSpawns.Remove(spawn);
                        break;
                    case Team.Red:
                        randomIndex = Random.Range(0, _initialRedSpawns.Count);
                        spawn = _initialRedSpawns[randomIndex];
                        _initialRedSpawns.Remove(spawn);
                        break;
                }
                return spawn;
            }
            return null;
        }

        private Vector3 GetSpawnPosition(Team team)
        {
            GameObject spawnPosition = null;
            int randomIndex = 0;

            switch (team)
            {
                case Team.Blue:
                    randomIndex = Random.Range(0, _blueSpawns.Count);
                    spawnPosition = _blueSpawns[randomIndex];
                    break;
                case Team.Red:
                    randomIndex = Random.Range(0, _redSpawns.Count);
                    spawnPosition = _redSpawns[randomIndex];
                    break;
            }
            return spawnPosition.transform.position;
        }

        private void IncreaseSpawnCount()
        {
            _spawnsAssigned++;
            if(_spawnsAssigned >= _playersCount)
            {
                _gameIsReady = true;
            }
        }
    }
}