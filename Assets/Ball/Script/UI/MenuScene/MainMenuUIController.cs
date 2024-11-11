using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static GameController;

public class MainMenuUIController : NetworkBehaviour
{
    public static MainMenuUIController Instance { get; private set; }

    public enum EMainMenuState
    {
        Home,
        Setting,
        Lobby,
    }

    public EMainMenuState State { get; private set; } = EMainMenuState.Home;

    public event EventHandler OnMenuStateChanged;
    public event EventHandler OnReadyChanged;

    [field: Header("Panel")]
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject lobbyPanel;

    // Player info
    public bool IsLocalPlayerReady { get; private set; } = false;
    private Dictionary<ulong, bool> playerReadyDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        playerReadyDict = new Dictionary<ulong, bool>();
    }

    public void SetLocalPlayerReady(bool isReady)
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDict[serverRpcParams.Receive.SenderClientId] = true;

        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

        bool isAllPlayerReady = true;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDict.ContainsKey(clientId) || !playerReadyDict[clientId])
            {
                isAllPlayerReady = false;
                break;
            }
        }

        if (isAllPlayerReady)
        {
            Debug.Log("All player ready: Game start");
            SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
        }
        else
        {
            Debug.Log("Waiting for player");
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDict[clientId] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetState(EMainMenuState newState)
    {
        State = newState;
        OnMenuStateChanged?.Invoke(this, EventArgs.Empty);

        switch (newState)
        {
            case EMainMenuState.Home:
                homePanel.SetActive(true);
                break;
            case EMainMenuState.Lobby:
                lobbyPanel.SetActive(true);
                break;
        }
    }

    public bool IsPlayerReady (ulong clientId)
    {
        if (!playerReadyDict.ContainsKey(clientId))
        {
            return false;
        }

        return playerReadyDict[clientId];
    }
}
