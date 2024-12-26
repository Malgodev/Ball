using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public struct LobbyPlayerInfo : IEquatable<LobbyPlayerInfo>, INetworkSerializable
{
    public string PlayerName;
    public int PlayerElo;
    public bool IsPlayerReady;

    public bool Equals(LobbyPlayerInfo other)
    {
        return PlayerName == other.PlayerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref PlayerElo);
        serializer.SerializeValue(ref IsPlayerReady);
    }

    public override string ToString()
    {
        return $"Player name: {PlayerName} Elo: {PlayerElo} IsReady: {IsPlayerReady}";
    }
}

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
    private NetworkVariable<LobbyPlayerInfo> playerOneInfo = new NetworkVariable<LobbyPlayerInfo>();
    private NetworkVariable<LobbyPlayerInfo> playerTwoInfo = new NetworkVariable<LobbyPlayerInfo>();

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

        OnPlayerInfoChanged += LobbyMultiplayerManager_OnPlayerInfoChanged;
    }
    private void LobbyMultiplayerManager_OnPlayerInfoChanged(object sender, EventArgs e)
    {
        lobbyUIController.SetPlayerInfoUI(playerOneInfo.Value, playerTwoInfo.Value);    
    }

    private void Start()
    {
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();


        playerOneInfo.OnValueChanged += (previous, current) =>
        {
            OnPlayerInfoChanged?.Invoke(this, EventArgs.Empty);
        };


        playerTwoInfo.OnValueChanged += (previous, current) =>
        {
            OnPlayerInfoChanged?.Invoke(this, EventArgs.Empty);
        };

        if (IsServer)
        {
            playerOneInfo.Value = new LobbyPlayerInfo
            {
                PlayerName = BallPlayerInfo.Instance.PlayerName,
                PlayerElo = BallPlayerInfo.Instance.PlayerElo,
                IsPlayerReady = false
            };

            playerTwoInfo.Value = new LobbyPlayerInfo
            {
                PlayerName = "Bot",
                PlayerElo = 0,
                IsPlayerReady = true
            };
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetLocalPlayerReadyServerRpc(string playerName)
    {
        NetworkVariable<LobbyPlayerInfo> localPlayer = GetLocalPlayer(playerName);

        Debug.Log(localPlayer.Value);

        if (localPlayer != null)
        {
            LobbyPlayerInfo updatedInfo = localPlayer.Value;
            updatedInfo.IsPlayerReady = !updatedInfo.IsPlayerReady;

            localPlayer.Value = updatedInfo;

            Debug.Log(updatedInfo + " \n" + localPlayer.Value + " " + IsServer);
        }
        else
        {
            Debug.LogError("SetLocalPlayerReady: Local player not found");
        }

        Debug.Log(localPlayer.Value);
    }

    public NetworkVariable<LobbyPlayerInfo> GetLocalPlayer(string playerName)
    {
        if (playerOneInfo.Value.PlayerName == playerName)
        {
            return playerOneInfo;
        }
        else if (playerTwoInfo.Value.PlayerName == playerName)
        {
            return playerTwoInfo;
        }
        else
        {
            Debug.LogError("Local player not found");
            return null;
        }
    }
}
