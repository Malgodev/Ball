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

    private void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
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

    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId, bool isReady)
    {
        playerReadyDict[clientId] = isReady;
        OnPlayerInfoChanged?.Invoke(this, EventArgs.Empty);
    }
}
