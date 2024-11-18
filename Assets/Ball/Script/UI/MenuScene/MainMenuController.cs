//using System;
//using System.Collections.Generic;
//using Unity.Netcode;
//using UnityEngine;
//using UnityEngine.UI;

//public partial class MainMenuController : NetworkBehaviour
//{
//    public static MainMenuController Instance { get; private set; }

//    public enum EMainMenuState
//    {
//        Home,
//        Setting,
//        Lobby,
//    }

//    public EMainMenuState State { get; private set; } = EMainMenuState.Home;

//    public event EventHandler OnMenuStateChanged;
//    public event EventHandler OnReadyChanged;

//    [field: Header("Panel")]
//    [SerializeField] private GameObject homePanel;
//    [SerializeField] private GameObject lobbyPanel;

//    // Player info
//    public bool IsLocalPlayerReady { get; private set; } = false;
//    private Dictionary<ulong, bool> playerReadyDict;

//    public MainMenuController(EMainMenuState state, GameObject homePanel, GameObject lobbyPanel, bool isLocalPlayerReady, Dictionary<ulong, bool> playerReadyDict, Button btn)
//    {
//        State = state;
//        this.homePanel = homePanel;
//        this.lobbyPanel = lobbyPanel;
//        IsLocalPlayerReady = isLocalPlayerReady;
//        this.playerReadyDict = playerReadyDict;
//        this.btn = btn;
//    }

//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(this);
//        }
//        else
//        {
//            Instance = this;
//        }

//        playerReadyDict = new Dictionary<ulong, bool>();
//    }

//    public void SetLocalPlayerReady()
//    {
//        SetPlayerReadyServerRpc();
//    }

//    [ServerRpc(RequireOwnership = false)]
//    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
//    {
//        ulong clientId = serverRpcParams.Receive.SenderClientId;

//        if (!playerReadyDict.ContainsKey(clientId))
//        {
//            playerReadyDict[clientId] = true;
//        }
//        else
//        {
//            playerReadyDict[clientId] = !playerReadyDict[clientId];
//        }

//        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId, playerReadyDict[clientId]);

//        bool isAllPlayerReady = true;

//        foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
//        {
//            if (!playerReadyDict.ContainsKey(id) || !playerReadyDict[id])
//            {
//                isAllPlayerReady = false;
//                break;
//            }
//        }

//        if (isAllPlayerReady)
//        {
//            Debug.Log("All player ready: Game start");
//            SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
//        }
//        else
//        {
//            Debug.Log("Waiting for player");
//        }
//    }

//    [ClientRpc]
//    private void SetPlayerReadyClientRpc(ulong clientId, bool isReady)
//    {
//        playerReadyDict[clientId] = isReady;
//        OnReadyChanged?.Invoke(this, EventArgs.Empty);
//    }

//    public void SetState(EMainMenuState newState)
//    {
//        State = newState;
//        OnMenuStateChanged?.Invoke(this, EventArgs.Empty);

//        switch (newState)
//        {
//            case EMainMenuState.Home:
//                homePanel.SetActive(true);
//                break;
//            case EMainMenuState.Lobby:
//                lobbyPanel.SetActive(true);
//                break;
//        }
//    }

//    public bool IsPlayerReady(ulong clientId)
//    {
//        if (!playerReadyDict.ContainsKey(clientId))
//        {
//            return false;
//        }

//        return playerReadyDict[clientId];
//    }
//}
