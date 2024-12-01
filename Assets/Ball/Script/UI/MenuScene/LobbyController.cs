using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyController : NetworkBehaviour
{
    public static LobbyController Instance { get; private set; }

    public enum EMainMenuStateTmp
    {
        Home,
        Setting,
        Lobby,
    }

    public EMainMenuStateTmp State { get; private set; } = EMainMenuStateTmp.Home;

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

    public void SetLocalPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        if (!playerReadyDict.ContainsKey(clientId))
        {
            playerReadyDict[clientId] = true;
        }
        else
        {
            playerReadyDict[clientId] = !playerReadyDict[clientId];
        }

        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId, playerReadyDict[clientId]);

        bool isAllPlayerReady = true;

        foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDict.ContainsKey(id) || !playerReadyDict[id])
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
    private void SetPlayerReadyClientRpc(ulong clientId, bool isReady)
    {
        playerReadyDict[clientId] = isReady;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetState(EMainMenuStateTmp newState)
    {
        State = newState;
        OnMenuStateChanged?.Invoke(this, EventArgs.Empty);

        switch (newState)
        {
            case EMainMenuStateTmp.Home:
                homePanel.SetActive(true);
                break;
            case EMainMenuStateTmp.Lobby:
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
