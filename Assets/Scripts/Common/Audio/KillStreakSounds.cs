﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using TMPro;
using SWExtensions;
using Multiplayer;

public class KillStreakSounds : GlobalEventListener
{
    [SerializeField] private float _killStreakTimer;
    [SerializeField] private AudioClip[] _killStreakAudioclips;
    [SerializeField] private GameObject[] _killStreakVisuals;

    [SerializeField] private GameObject _threeKillsRemainingVisual;
    [SerializeField] private GameObject _isPutInJailVisual;
    [SerializeField] private GameObject _blueJailOpenVisual;
    [SerializeField] private GameObject _redJailOpenVisual;

    private AudioSource _audioSource;
    private Dictionary<int, int> PlayerCumulativeScoreEntries = new Dictionary<int, int>();
    private Dictionary<int, float> PlayerTimer = new Dictionary<int, float>();
    private bool _isFirstBloodOccured = false;

    //CORE

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    //BOLT

    public override void OnEvent(PlayerHit evnt)
    {
        if (PlayerCumulativeScoreEntries.Count == 0 && !_isFirstBloodOccured)
        {
            _audioSource.clip = _killStreakAudioclips[6];
            _audioSource.Play();

            _killStreakVisuals[6].GetComponentInChildren<TextMeshProUGUI>().text = evnt.KillerName;

            _killStreakVisuals[6].SetActive(true);
            StartCoroutine(DisableEventsVisuals(_killStreakVisuals[6], 3));
            _isFirstBloodOccured = true;
        }

        if (PlayerCumulativeScoreEntries.ContainsKey(evnt.VictimID))
        {
            RemoveFromAllDictionnaries(evnt.VictimID);
        }

        if (PlayerCumulativeScoreEntries.ContainsKey(evnt.KillerID))
        {
            AddCumulativeScore(evnt.KillerID);
            CheckCumulativeScore(evnt.KillerID, evnt.KillerName);
        }
        else
        {
            PlayerCumulativeScoreEntries.Add(evnt.KillerID, 1);
            PlayerTimer.Add(evnt.KillerID, Time.time);
        }
    }

    public override void OnEvent(ScoreIncreased evnt)
    {
        if (evnt.Score == 13)
        {
            _threeKillsRemainingVisual.SetActive(true);
            StartCoroutine(DisableEventsVisuals(_threeKillsRemainingVisual, 3));
        }
    }
        public override void OnEvent(JailButtonPushed evnt)
    {
        if(evnt.Team == "Blue")
        {
            _blueJailOpenVisual.SetActive(true);
            StartCoroutine(DisableEventsVisuals(_blueJailOpenVisual, 3));
        }
        else
        {
            _redJailOpenVisual.SetActive(true);
            StartCoroutine(DisableEventsVisuals(_redJailOpenVisual, 3));
        }
    }

    public override void OnEvent(KartForcedToJail evnt)
    {
        _isPutInJailVisual.SetActive(false);

        GameObject kart = KartExtensions.GetKartWithID(evnt.PlayerID);
        _isPutInJailVisual.GetComponentInChildren<TextMeshProUGUI>().text = kart.GetComponent<PlayerInfo>().Nickname;

        _isPutInJailVisual.SetActive(true);
        StartCoroutine(DisableEventsVisuals(_redJailOpenVisual, 3));
    }

    //PRIVATE

    private void CheckCumulativeScore(int playerEntry, string playerNickname)
    {
        if (PlayerCumulativeScoreEntries[playerEntry] >= 2 && Time.time - PlayerTimer[playerEntry] <= _killStreakTimer)
        {
            //  Debug.LogError("Set new Time - LastHit Was : " + (Time.time - PlayerTimer[playerEntry]));
            PlayCumulativeAudioClip(PlayerCumulativeScoreEntries[playerEntry] - 2, playerNickname);
            PlayerTimer[playerEntry] = Time.time;
        }
        else if(Time.time - PlayerTimer[playerEntry] > _killStreakTimer)
        {
         //   Debug.LogError("RemoveFromDictionaries - LastHit Was : " + (Time.time - PlayerTimer[playerEntry]));
            RemoveFromAllDictionnaries(playerEntry);
        }
    }

    private void AddCumulativeScore(int playerEntry)
    {
        if (PlayerCumulativeScoreEntries[playerEntry] <= 6)
            PlayerCumulativeScoreEntries[playerEntry] += 1;
    }

    private void PlayCumulativeAudioClip(int cumulativeScore, string playerNickname)
    {
        _audioSource.clip = _killStreakAudioclips[cumulativeScore];
        _audioSource.Play();

        foreach(GameObject go in _killStreakVisuals)
        {
            go.SetActive(false);
        }
        _killStreakVisuals[cumulativeScore].GetComponentInChildren<TextMeshProUGUI>().text = playerNickname;
        _killStreakVisuals[cumulativeScore].SetActive(true);
        StartCoroutine(DisableEventsVisuals(_killStreakVisuals[cumulativeScore], 3));
    }

    private void RemoveFromAllDictionnaries(int playerEntry)
    {
        PlayerCumulativeScoreEntries.Remove(playerEntry);
        PlayerTimer.Remove(playerEntry);
    }

    private IEnumerator DisableEventsVisuals(GameObject countdownGameobject, float timer)
    {
        yield return new WaitForSeconds(timer);
        countdownGameobject.SetActive(false);
    }
}
