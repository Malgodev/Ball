using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class BallGameLobby : MonoBehaviour
{
    public static BallGameLobby Instance { get; private set; }

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";


    private Lobby lobby;
    private Lobby hostLobby;

    public bool IsHost 
    {
        get { return hostLobby != null; }
        private set { }
    }

    public string LobbyName
    {
        get { return lobby.Name; }
    }

    private float heartbeatTimer = 0;
    private float pollTimer = 0;

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
        HandleLobbyPollUpdate();
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
        if (hostLobby != null)
        {
            pollTimer -= Time.deltaTime;
            if (pollTimer < 0f)
            {
                float pollTimerMax = 3f;

                pollTimer = pollTimerMax;

                try
                {
                    lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogError(e);
                }
            }
        }
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

    public async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(BallGameMultiplayer.MAX_PLAYER_AMOUNT - 1);

            return allocation;
        } 
        catch (RelayServiceException e)
        {
            Debug.LogError(e);

            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            string playerName = BallPlayerInfo.Instance.PlayerName;
            string playerElo = BallPlayerInfo.Instance.PlayerElo.ToString();

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

            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetReplayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            BallGameMultiplayer.Instance.StartHost(); 
            SceneLoader.LoadNetwork(SceneLoader.Scene.LobbyScene);

            // LobbyController.Instance.SetState(LobbyController.EMainMenuStateTmp.Lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async Task<string> GetReplayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return default;
        }
    }

    public async void QuickJoin()
    {
        try
        {
            lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
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
            string playerElo = BallPlayerInfo.Instance.PlayerElo.ToString();

            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer(playerName, playerElo)
            };


            lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);

            JoinAllocation joinAllocation = await JoinRelay(lobby.Data[KEY_RELAY_JOIN_CODE].Value );

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            BallGameMultiplayer.Instance.StartClient();
            SceneLoader.LoadNetwork(SceneLoader.Scene.LobbyScene);

            Debug.Log("Joined Lobby with code: " + lobbyId + " with name: " + lobby.Name);
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

    public List<PlayerInfo> GetPlayerInfoList()
    {
        List<PlayerInfo> playerList = new List<PlayerInfo>();

        foreach (Player player in lobby.Players)
        {
            PlayerInfo playerInfo = new PlayerInfo
            {
                PlayerName = player.Data["PlayerName"].Value,
                PlayerElo = int.Parse(player.Data["PlayerElo"].Value)
            };

            playerList.Add(playerInfo);
        }

        return playerList;
    }

    private async void LeaveLobbyAsync()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId);
            Debug.Log("Left the lobby.");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to leave lobby: {e.Message}");
        }
    }
}
