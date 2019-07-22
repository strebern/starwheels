﻿using UnityEngine;
using UnityEngine.Events;
using Bolt;

namespace Multiplayer
{
    public class GameReadyEventListener : GlobalEventListener
    {
        [Header("Unity Events")]
        public UnityEvent OnGameReady;

        [Header("Game Started")]
        [SerializeField] private BoolVariable _gameStartedVariable;

        // CORE

        private void Start()
        {
            if (_gameStartedVariable.Value == true && OnGameReady != null)
            {
                OnGameReady.Invoke();
            }
        }

        // BOLT

        public override void OnEvent(GameReady evnt)
        {
            if (OnGameReady != null)
            {
                OnGameReady.Invoke();
                Debug.LogError("GAME START");
            }
            _gameStartedVariable.Value = true;
        }

        public override void OnEvent(LobbyCountdown evnt)
        {
           // if (BoltNetwork.IsServer)
              //  Debug.LogError("Starts in " + evnt.Time);
        }
    }
}
