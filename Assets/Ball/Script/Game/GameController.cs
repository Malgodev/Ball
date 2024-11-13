using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    public static GameController Instance { get; private set; }
    public float CurrentFPS;

    [field: Header("Prefab")]
    [field: SerializeField] public GameObject Ball { get; private set; }
    [field: SerializeField] public GameObject PlayerPrefab { get; private set; }
    [field: SerializeField] public GameObject TeamPrefab { get; private set; }
    [field: SerializeField] public GameObject EmptyTeamPrefab { get; private set; }


    [field: Header("Enviroment")]
    [SerializeField] private GameObject teamOneGoal;
    [SerializeField] private GameObject teamTwoGoal;
    [SerializeField] private Transform upperWall;
    [SerializeField] private Transform lowerWall;
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform rightWall;
    [SerializeField] private GameObject field;

    [field: Header("Team controller")]
    [field: SerializeField] public TeamController TeamOneController { get; private set; }
    [field: SerializeField] public TeamController TeamTwoController { get; private set; }

    public PlayerController PlayerHasBall { get; private set; }

    [field: Header("Game controller")]
    public event EventHandler OnStateChanged;
    public event EventHandler OnInteractAction;
    public event EventHandler OnLocalPlayerReadyChange;

    // remove this
    public bool IsLocalPlayerReady { get; private set; } = false;
    private Dictionary<ulong, bool> playerReadyDict;


    public enum GameState
    {
        WaitingToStart,
        GamePlaying,
        GameOver,
    }

    [field: SerializeField] public NetworkVariable<GameState> State { get; private set; } = new NetworkVariable<GameState>(GameState.WaitingToStart);
    public NetworkVariable<float> GamePlayingTimer { get; private set; } = new NetworkVariable<float>(0f);
    private float MAXIMUM_PLAYING_TIME = 10f;

    private float waitingConnectTime = 120f;

    private void Awake()
    {
        Application.targetFrameRate = 60;

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

    // TODO This can put into player data -> select favorite color
    private void SetTeamColor(TeamController team, Color color)
    {
        foreach (GameObject player in team.PlayerList)
        {
            player.GetComponent<SpriteRenderer>().color = color;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        State.OnValueChanged += (previous, current) =>
        {
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        };

        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    // Default team one
    // Pos -23 5 -1
    // Rot 0 0 0
    // Sca 55 50 1
    // Default team two
    // Pos -23 -5 -1
    // Rot 0 0 180
    // Sca 55 50 1

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            UserData clientData = BallGameMultiplayer.Instance.GetUserDataByClientId(clientId);

            GameObject teamGameObject = Instantiate(EmptyTeamPrefab);
            teamGameObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            TeamController teamController = teamGameObject.GetComponent<TeamController>();

            if (clientId == 0)
            {
                teamController.SetTeamInfo(true, false);
                // teamController.SetTeamInfo(true, true);
                TeamOneController = teamController;
            }
            else
            {
                Debug.Log("spawn new stuff");
                teamController.SetTeamInfo(false, false);
                TeamTwoController = teamController;
            }
        }

        if (TeamTwoController == null)
        {
            GameObject teamGameObject = Instantiate(EmptyTeamPrefab);
            teamGameObject.GetComponent<NetworkObject>().Spawn(true);
            TeamController teamController = teamGameObject.GetComponent<TeamController>();

            teamController.SetTeamInfo(false, false);
            //teamController.SetIsTeamOne(false);
            //teamController.SetIsControlledPlayer(false);
            TeamTwoController = teamController;
        }
    }

    private void Start()
    {
        //SetTeamColor(teamOneController, Color.red);
        //SetTeamColor(teamTwoController, Color.green);


    }

    public void ReadyToPlay()
    {
        if (State.Value == GameState.WaitingToStart)
        {
            IsLocalPlayerReady = true;
            OnLocalPlayerReadyChange?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Update()
    {
        CurrentFPS = Mathf.Round(1.0f / Time.deltaTime);

        if (!IsServer)
        {
            return;
        }

        // TODO Code to change game behaviour based on state
        switch (State.Value)
        {
            case GameState.WaitingToStart:
                break;
            case GameState.GamePlaying:
                GamePlayingTimer.Value += Time.deltaTime;

                if (GamePlayingTimer.Value >= MAXIMUM_PLAYING_TIME)
                {
                    State.Value = GameState.GameOver;
                    // OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;
            case GameState.GameOver:
                break;
        }
    }



    public void SpawnPlayer(TeamController teamController)
    {
        if (teamController.IsTeamOne)
        {
            GeneratePlayer.GeneratePlayerByTeam(teamController);
            SetTeamController(teamController);
        }
        else
        {
            GeneratePlayer.GeneratePlayerByTeam(teamController);
            SetTeamController(teamController);
        }
    }

    public void StartCountdown()
    {

    }

    public void SetLocalPlayerReady(bool isReady)
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDict[serverRpcParams.Receive.SenderClientId] = true;

        bool isAllPlayerReady = true;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDict.ContainsKey(clientId) || !playerReadyDict[clientId])
            {
                isAllPlayerReady = false;
            }
        }

        if (isAllPlayerReady)
        {
            Debug.Log("All player ready: Game start");
            State.Value = GameState.GamePlaying;
        }
        else
        {
            Debug.Log("Waiting for player");
        }
    }

    public void SetTeamController(TeamController teamController)
    {
        if (teamController.IsTeamOne)
        {
            TeamOneController = teamController;
            SetTeamColor(TeamOneController, Color.red);
        }
        else
        {
            TeamTwoController = teamController;
            SetTeamColor(TeamTwoController, Color.green);
        }
    }

    public void SetPlayerHasBall(PlayerController player)
    {
        PlayerHasBall = player;

        if (player != null)
        {
            // TODO Change this
            StartCoroutine(player.DribblingBall());
        }
        else
        {
            PlayerHasBall = player;
        }
    }

    public ETeamHasBall GetTeamHasBall()
    {
        if (PlayerHasBall == null)
        {
            return ETeamHasBall.None;
        }

        if (PlayerHasBall.GetComponent<PlayerController>().IsTeamOne)
        {
            return ETeamHasBall.TeamOne;
        }
        else
        {
            return ETeamHasBall.TeamTwo;
        }
    }

    public List<GameObject> GetEnemyPlayer(bool isTeamOne)
    {
        if (isTeamOne)
        {
            return TeamTwoController.PlayerList;
        }
        else
        {
            return TeamOneController.PlayerList;
        }
    }

    public List<GameObject> GetFriendPlayer(bool isTeamOne)
    {
        if (!isTeamOne)
        {
            return TeamTwoController.PlayerList;
        }
        else
        {
            return TeamOneController.PlayerList;
        }
    }

    public float GetDistanceToGoal(bool isTeamOne, PlayerController playerController)
    {
        Vector2 goalPos = !isTeamOne ? teamOneGoal.transform.position : teamTwoGoal.transform.position;

        return Vector2.Distance(goalPos, playerController.transform.position);
    }

    public Vector2 GetDirectionToGoal(bool isTeamOne, PlayerController playerController)
    {
        Vector2 goalPos = !isTeamOne ? teamOneGoal.transform.position : teamTwoGoal.transform.position;
        Vector2 playerPos = playerController.transform.position;
        Vector2 shootingDirection = goalPos - playerPos;
        return shootingDirection.normalized;
    }

    public Transform GetGoal(bool isTeamOne)
    {
        return !isTeamOne ? teamOneGoal.transform : teamTwoGoal.transform;
    }
}
