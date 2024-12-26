using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyMultiplayerManager : NetworkBehaviour
{
    [field: Header("Singleton")]
    public static LobbyMultiplayerManager Instance { get; private set; }

    [field: Header("Controller")]
    [SerializeField] private LobbyUIController lobbyUIController;

    [field: Header("EventHandler")]
    public event EventHandler OnPlayerInfoChanged;

    [field: Header("Script")]
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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        SetLobbyInfoServerRpc(); 
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetLobbyInfoServerRpc()
    {
        SetLobbyInfoClientRpc();
    }

    [ClientRpc]
    public void SetLobbyInfoClientRpc()
    {
        lobbyUIController.SetLobbyInfo();
    }
    
    public void SetLocalPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }


    [ServerRpc(RequireOwnership=false)]
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
        lobbyUIController.SetPlayerReadyUI(playerReadyDict);
        OnPlayerInfoChanged?.Invoke(this, EventArgs.Empty);
    }
}
