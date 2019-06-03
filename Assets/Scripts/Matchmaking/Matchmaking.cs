﻿using System;
using System.Linq;
using SW.Matchmaking;
using UnityEngine;
using Bolt;
using Bolt.Utils;
using UdpKit;

public static class SWMatchmaking
{
    public static void CreateLobby()
    {
        if (!BoltNetwork.IsRunning && !BoltNetwork.IsServer)
        {
            BoltLauncher.StartServer();
            Debug.Log("Starting Bolt as server...");
        }
        else
        {
            Debug.LogWarning("Can't create a lobby if you are already a server.");
        }
    }

    public static void SetLobbyData(LobbyData lobbyData)
    {
        if (BoltNetwork.IsRunning && BoltNetwork.IsServer)
        {
            BoltNetwork.RegisterTokenClass<LobbyToken>();

            LobbyToken token = new LobbyToken().BuildData(lobbyData);
            BoltNetwork.SetServerInfo(lobbyData.ServerName, token);
            Debug.Log("Lobby data set.");
        }
        else
        {
            Debug.LogWarning("Can't set data if Bolt is not running.");
        }
    }

    public static LobbyToken GetLobbyToken(Guid lobbyID)
    {
        return (LobbyToken)BoltNetwork.SessionList[lobbyID].GetProtocolToken();
    }

    public static int GetCurrentLobbyPlayerCount()
    {
        return BoltNetwork.Clients.Count();
    }

    public static void JoinRandomLobby()
    {
        System.Random rnd = new System.Random();
        var randomSessionNumber = rnd.Next(BoltNetwork.SessionList.Count);
        var count = 0;
        foreach (var session in BoltNetwork.SessionList)
        {
            if (count == randomSessionNumber)
            {
                BoltNetwork.Connect(session.Value);
            }
            else
            {
                count++;
            }
        }
    }

    public static void JoinLobby(Guid id, IProtocolToken connectToken = null)
    {
        BoltNetwork.Connect(BoltNetwork.SessionList[id], connectToken);
    }

    public static void JoinLobby(string serverName, IProtocolToken connectToken = null)
    {
        foreach (var session in BoltNetwork.SessionList)
        {
            var lobbyToken = SWMatchmaking.GetLobbyToken(session.Key);
            Debug.Log("Server name : " + lobbyToken.ServerName);
            if (lobbyToken.ServerName.Equals(serverName))
            {
                BoltNetwork.Connect(BoltNetwork.SessionList[session.Key], connectToken);
            }
        }
    }

    public static void JoinLobby(UdpSession udpSession, IProtocolToken connectToken = null)
    {
        BoltNetwork.Connect(udpSession, connectToken);
    }

    public static int GetMyBoltId() // En faire une extension de boltnetwork
    {
        if (BoltNetwork.IsConnected)
        {
            if (BoltNetwork.IsServer) // We are the server
            {
                return 0;
            }
            else // We are a client
            {
                return (int)BoltNetwork.Server.ConnectionId;
            }
        }
        else
        {
            Debug.LogWarning("Can't get your Bolt ID if you are not connected to Bolt.");
            return -1;
        }
    }

    public static Guid GetBoltSessionID(Guid udpSessionGuid)
    {
        foreach (var session in BoltNetwork.SessionList)
        {
            Debug.Log("Session ID : " + session.Value.Id);
            if (session.Value.Id == udpSessionGuid)
            {
                return session.Key;
            }
        }
        return new Guid();
    }
}

