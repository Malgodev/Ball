using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class BallGameLobby : MonoBehaviour
{
    public static BallGameLobby Instance { get; private set; }

    private Lobby lobby;
    private Lobby hostLobby;

    private float heartbeatTimer = 0;


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

        DontDestroyOnLoad(gameObject);
        InitUnityAuthentication();
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;

                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyPollUpdate()
    {

    }

    private async void InitUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options = new InitializationOptions();

            options.SetProfile(UnityEngine.Random.Range(0, 100000).ToString());

            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            string playerName = BallPlayerInfo.Instance.PlayerName;
            string playerElo = BallPlayerInfo.Instance.playerElo.ToString();

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Data = GetLobbyData(playerElo),
                Player = GetPlayer(playerName, playerElo)
            };

            lobby = await LobbyService.Instance.CreateLobbyAsync(
                lobbyName, BallGameMultiplayer.MAX_PLAYER_AMOUNT, createLobbyOptions);

            hostLobby = lobby;

            Debug.Log($"Create lobby: {lobbyName} \n " +
                $"Code: {lobby.LobbyCode} \n " +
                $"Id: {lobby.Id} \n" +
                $"Max player: {lobby.MaxPlayers}");

            BallGameMultiplayer.Instance.StartHost();
            SceneLoader.LoadNetwork(SceneLoader.Scene.LobbyScene);

            // LobbyController.Instance.SetState(LobbyController.EMainMenuStateTmp.Lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void QuickJoin()
    {
        try
        {
            lobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            // BallGameMultiplayer.Instance.StartClient();

            LobbyControllerExpired.Instance.SetState(LobbyControllerExpired.EMainMenuStateTmp.Lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {

            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode);

            Debug.Log("Joined Lobby with code: " + lobbyCode + "with name" + joinedLobby.Name);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinLobbyById(string lobbyId)
    {
        try
        {
            string playerName = BallPlayerInfo.Instance.PlayerName;
            string playerElo = BallPlayerInfo.Instance.playerElo.ToString();

            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer(playerName, playerElo)
            };


            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);

            BallGameMultiplayer.Instance.StartClient();
            SceneLoader.LoadNetwork(SceneLoader.Scene.LobbyScene);

            Debug.Log("Joined Lobby with code: " + lobbyId + "with name" + joinedLobby.Name);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private Dictionary<string, DataObject> GetLobbyData(string playerElo)
    {
        return new Dictionary<string, DataObject>
        {
            {"LobbyElo",
            new DataObject(DataObject.VisibilityOptions.Public, playerElo)}
        };
    }

    private Player GetPlayer(string playerName, string playerElo)
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {
                    "PlayerName",
                    new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
                },
                {
                    "PlayerElo",
                    new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerElo)
                }
            }
        };
    }

    public void PrintPlayer(Lobby lobby)
    {
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value + " " + player.Data["PlayerElo"].Value);
        }
    }
}
