﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using CameraUtils;
using Kart;

namespace GameModes
{
    public enum GameMode { None, ClassicBattle, BankRobbery, GoldenTotem }

    public class GameModeBase : MonoBehaviour
    {
        #region Variables
        //public static GameModeBase Instance;
        public static GameMode CurrentGameMode;

        public bool GameStarted = false;

        [SerializeField] private float countdownSeconds = 3f;

        private GameObject[] _spawns;
        #endregion

        // CORE

        protected void Start()
        {
            _spawns = GameObject.FindGameObjectsWithTag(Constants.Tag.Spawn);

            SpawnKart();
        }

        // PUBLIC

        public void RespawnKart()
        {
            //
        }

        // PROTECTED

        protected virtual void InitializeGame()
        {
            // To Implement in concrete Game Modes
        }

        protected virtual void ResetGame()
        {
            // To Implement in concrete Game Modes
        }

        // PRIVATE

        private IEnumerator StartCountdown()
        {
            yield return new WaitForSeconds(countdownSeconds);
            InitializeGame();
            GameStarted = true;
        }


        private void SpawnKart()
        {
            /*
             * int playerId = PhotonNetwork.LocalPlayer.ActorNumber;
            var initPos = _spawns[playerId].transform.position;
            var initRot = _spawns[playerId].transform.rotation;
            var kart = PhotonNetwork.Instantiate("Kart", initPos, initRot, 0);

            var cinemachineDynamicScript = FindObjectOfType<CinemachineDynamicScript>();
            cinemachineDynamicScript.SetKart(kart);

            StartCoroutine(LoadGameHUD(kart));
            */
        }

        private IEnumerator LoadGameHUD(GameObject kart)
        {
            AsyncOperation loadLevel = SceneManager.LoadSceneAsync(Constants.Scene.GameHUD, LoadSceneMode.Additive);
            while (!loadLevel.isDone)
            {
                yield return null;
            }
            //FindObjectOfType<HUD.GameHUD>().ObserveKart(kart);
        }
    }
}
