﻿using System.Collections.Generic;
using UnityEngine;

namespace SW.Matchmaking
{
    [CreateAssetMenu(menuName = "Lobby Data")]
    public class LobbyData : ScriptableObject
    {
        [Header("General Info")]
        public string ServerName;
        public bool Public;
        public int MaxPlayers;

        [Header("Gamemode Info")]
        public List<string> GamemodePool;
        public string ChosenGamemode;

        [Header("Map Info")]
        public Dictionary<string, List<string>> MapPool;
        public string ChosenMapName;

        public void SetRandomGamemode()
        {
            ChosenGamemode = PickRandomGamemodeFromPool();
            Debug.Log("Random gamemode set : " + ChosenGamemode);
        }

        public void AddGamemode(string gamemode)
        {
            if (!GamemodePool.Contains(gamemode))
            {
                GamemodePool.Add(gamemode);
            }
        }

        public void RemoveGamemode(string gamemode)
        {
            if (GamemodePool.Contains(gamemode))
            {
                GamemodePool.Remove(gamemode);
            }
        }

        public string PickRandomGamemodeFromPool()
        {
            return GamemodePool[Random.Range(0, GamemodePool.Count)];
        }

        public void SetRandomMap()
        {
            ChosenMapName = PickRandomMapNameFromPool(ChosenGamemode);
            Debug.Log("Random gamemode set : " + ChosenMapName);
        }

        public void AddMap(string gamemode, string map)
        {
            if (MapPool.ContainsKey(gamemode) && !MapPool[gamemode].Contains(map))
            {
                MapPool[gamemode].Add(map);
            }
        }

        public void RemoveMap(string gamemode, string map)
        {
            if (MapPool.ContainsKey(gamemode) && MapPool[gamemode].Contains(map))
            {
                MapPool[gamemode].Remove(map);
            }
        }

        public string PickRandomMapNameFromPool(string gamemode)
        {
            if (MapPool[gamemode].Count > 0)
            {
                return MapPool[gamemode][Random.Range(0, MapPool[gamemode].Count)];
            }
            else
            {
                Debug.LogError("MapPool is empty!");
                return null;
            }
        }
    }
}
