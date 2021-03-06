﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer;

namespace SWExtensions
{
    public static class SystemExtensions
    {
        public static Guid ToGuid(this int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
    }

    public class MathExtensions
    {
        public static float RemapValue(float actualMin, float actualMax, float targetMin, float targetMax, float val)
        {
            return targetMin + (targetMax - targetMin) * ((val - actualMin) / (actualMax - actualMin));
        }
    }

    public static class TeamExtensions
    {
        public static List<PlayerInfo> GetTeammates(this PlayerInfo playerSettings)
        {
            var teammates = new List<PlayerInfo>();
            var allPlayers = MonoBehaviour.FindObjectsOfType<PlayerInfo>();

            foreach (PlayerInfo player in allPlayers)
            {
                if (player.Team == playerSettings.Team)
                {
                    teammates.Add(player);
                }
            }
            return teammates;
        }

        public static PlayerInfo GetNextTeammate(this PlayerInfo playerSettings, PlayerInfo currentTeammate)
        {
            var teammates = playerSettings.GetTeammates();
            for (int i = 0; i < teammates.Count; i++)
            {
                if (teammates[i] == currentTeammate)
                {
                    return teammates[(i + 1) % teammates.Count];
                }
            }
            return null;
        }

        public static PlayerInfo PickRandomTeammate(this PlayerInfo playerSettings)
        {
            var teammates = playerSettings.GetTeammates();
            var rand = UnityEngine.Random.Range(0, teammates.Count);
            return teammates[rand];
        }
    }

    public static class KartExtensions
    {
        public static GameObject GetMyKart()
        {
            var allKarts = GameObject.FindGameObjectsWithTag(Constants.Tag.Kart);
            foreach (GameObject kart in allKarts)
            {
                var entity = kart.GetComponent<BoltEntity>();
                if (entity.isOwner)
                {
                    return kart;
                }
            }
            return null;
        }

        public static GameObject GetKartWithID(int id)
        {
            var allKarts = GameObject.FindGameObjectsWithTag(Constants.Tag.Kart);
            foreach (GameObject kart in allKarts)
            {
                var entity = kart.GetComponent<BoltEntity>();
                if(entity.GetState<IKartState>().OwnerID == id)
                {
                    return kart;
                }
            }
            return null;
        }

        public static GameObject GetKartRoot(this Component component)
        {
            GameObject result = component.GetComponentInParent<PlayerInfo>().gameObject;
            if(result == null)
            {
                Debug.LogError("Could not find the kart root. The component may not be from the kart prefab.");
            }
            return result;
        }

        public static List<GameObject> GetTeamKarts(this PlayerInfo playerSettings)
        {
            var teamKarts = new List<GameObject>();
            var allKarts = GameObject.FindGameObjectsWithTag(Constants.Tag.Kart);
            foreach (GameObject kart in allKarts)
            {
                var kartPlayer = kart.GetComponent<PlayerInfo>();
                if (kartPlayer.Team == playerSettings.Team && kartPlayer != playerSettings)
                {
                    teamKarts.Add(kart);
                }
            }
            return teamKarts;
        }

        public static GameObject GetNextTeamKart(this PlayerInfo playerSettings, GameObject currentTeamKart)
        {
            var teamKarts = playerSettings.GetTeamKarts();
            for (int i = 0; i < teamKarts.Count; i++)
            {
                if (teamKarts[i] == currentTeamKart)
                {
                    return teamKarts[(i + 1) % teamKarts.Count];
                }
            }
            return null;
        }

        public static GameObject PickRandomTeamKart(this PlayerInfo playerSettings)
        {
            var teamKart = playerSettings.GetTeamKarts();

            if (teamKart.Count > 0)
            {
                var rand = UnityEngine.Random.Range(0, teamKart.Count);
                return teamKart[rand];
            }
            return null;
        }
    }

    public static class TMProExtensions
    {
        public static void ChangeTMProDropdownValue(this TMPro.TMP_Dropdown dropdown, string value)
        {
            for (var i = 0; i < dropdown.options.Count; i++)
            {
                if (dropdown.options[i].text == value)
                {
                    dropdown.value = i;
                }
            }
        }
    }

    public static class ComponentExtensions
    {
        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(original, null), null);
            }
            return dst as T;
        }

        public static void CopyAndPasteAudioSource(AudioSource original, GameObject destination)
        {
            destination.AddComponent<AudioSource>();
            AudioSource audioSource = destination.GetComponent<AudioSource>();
            audioSource.bypassEffects = original.bypassEffects;
            audioSource.bypassListenerEffects = original.bypassListenerEffects;
            audioSource.bypassReverbZones = original.bypassReverbZones;
            audioSource.clip = original.clip;
            audioSource.dopplerLevel = original.dopplerLevel;
            audioSource.ignoreListenerPause = original.ignoreListenerPause;
            audioSource.ignoreListenerVolume = original.ignoreListenerVolume;
            audioSource.loop = original.loop;
            audioSource.maxDistance = original.maxDistance;
            audioSource.minDistance = original.minDistance;
            audioSource.mute = original.mute;
            audioSource.outputAudioMixerGroup = original.outputAudioMixerGroup;
            audioSource.playOnAwake = original.playOnAwake;
            audioSource.panStereo = original.panStereo;
            audioSource.priority = original.priority;
            audioSource.pitch = original.pitch;
            audioSource.rolloffMode = original.rolloffMode;
            audioSource.reverbZoneMix = original.reverbZoneMix;
            audioSource.spatialBlend = original.spatialBlend;
            audioSource.spatialize = original.spatialize;
            audioSource.spatializePostEffects = original.spatializePostEffects;
            audioSource.spread = original.spread;
            audioSource.time = original.time;
            audioSource.timeSamples = original.timeSamples;
            audioSource.velocityUpdateMode = original.velocityUpdateMode;
            audioSource.volume = original.volume;
        }
    }

    public class AudioExtensions
    {
        public static void PlayClipObjectAndDestroy(AudioSource audioSource)
        {
            if (audioSource != null)
            {
                GameObject oneShotObject = new GameObject("One shot sound from " + audioSource.name);
                audioSource.transform.SetParent(oneShotObject.transform);
                foreach (var audio in audioSource.GetComponents<AudioSource>())
                {
                    audio.Stop();
                }
                audioSource.Play();
                MonoBehaviour.Destroy(oneShotObject, audioSource.clip.length + 1f);
            }
            else
            {
              //  Debug.Log("Cannot play an audio source that has been destroyed");
            }
        }
    }
}
