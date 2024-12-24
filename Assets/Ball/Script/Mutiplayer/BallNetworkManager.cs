using System;
using UnityEngine;
using System.Diagnostics;
using Unity.Netcode;

public class BallNetworkManager : NetworkManager
{
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerConnected;
        }
    }

    private void OnServerStarted()
    {
    }

    private void OnPlayerConnected(ulong clientId)
    {
        UnityEngine.Debug.Log("New player connected " + clientId); 
    }
}
