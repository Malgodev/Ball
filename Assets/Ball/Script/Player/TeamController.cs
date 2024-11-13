using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.LowLevel;

[Serializable]
public class TeamController : NetworkBehaviour
{
    [field: SerializeField] public bool IsTeamOne { get; private set; } = false;
    [field: Header("Enviroment")]
    private GameObject ball;
    private GameObject targetGoal;

    [field: Header("Formation")]
    [field: SerializeField] public EFormation formation { get; private set; }
    [field: SerializeField] public FormationController formationController { get; private set; }
    [field: SerializeField] public FormationAI formationAI { get; private set; }

    [field: Header("Player")]
    [field: SerializeField] public List<GameObject> PlayerList { get; private set; }
    [field: SerializeField] public PlayerController ControlledPlayer { get; private set; }
    [field: SerializeField] public PlayerController ClosestPlayerToBall { get; private set; }

    [field: Header("User control")]
    [field: SerializeField] public bool IsControlledPlayer { get; private set; }
    private UserInput userInput;

    private int updateCounter = 0;

    public const string PLAYER_TAG = "Player";

    private void Awake()
    {
        PlayerList = new List<GameObject>();

        userInput = GetComponent<UserInput>();

    }

    private void Start()
    {
        // ball = GameController.Singleton.Ball;
        // goal == get target goal;
        formationController.IsTeamOne = IsTeamOne;
        formationAI.IsTeamOne = IsTeamOne;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // GameController.Singleton.SpawnPlayer(this);
        ball = GameController.Instance.Ball;

        this.gameObject.name = "Team" + (IsTeamOne ? "One" : "Two");

        // SetAllTeamPlayer();

        //if (IsControlledPlayer)
        //{
        //    SetControlledPlayer(PlayerList[PlayerList.Count - 1].GetComponent<PlayerController>());
        //}
    }

    // Mostly use for set player state
    // From that, player will automaticly move by that state
    private void Update()
    {
        if (IsOwner)
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                this.gameObject.name = "Bruh";
            }
            else
            {
                this.gameObject.name = "Not bruh";
            }
        }

        if (GameController.Instance.State.Value != GameController.GameState.GamePlaying)
        {
            return;
        }
        
        updateCounter++;
        if (updateCounter >= 5)
        {
            updateCounter = 0;
            return;
        }

        PlayerController playerHasBall = GameController.Instance.PlayerHasBall;

        foreach (GameObject player in PlayerList)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            Vector2 formationScale = formationController.formationScale;
            playerController.MoveableRadius = Mathf.Max(formationScale.x, formationScale.y) / 3;

            if (playerController == ControlledPlayer || playerController.Role == EPlayerRole.Goalkeeper)
            {
                continue;
            }

            Vector2 defaultTargetPos = formationController.GetWorldPositionByOffset(playerController.DefaultOffset);
            GetTargetState(playerController, defaultTargetPos, playerHasBall);
        }
    }

    private void GetTargetState(PlayerController playerController, Vector2 defaultPos, PlayerController playerHasBall)
    {
        // TODO Make this code dynamic based on player role
        float PossessionRate = formationAI.PossessionBalance;
        float DangerRate = playerController.DangerRate;

        playerController.SetPlayerTargetPosition(defaultPos);

        if (playerController == playerHasBall)
        {
            // Here to, change singleton to get distance from current goal
            if (PossessionRate >= 0.9f && GameController.Instance.GetDistanceToGoal(IsTeamOne, playerController) < 10f)
            {
                playerController.SetPlayerState(EPlayerState.Shot);
            }
            else if (DangerRate >= 0.5f)
            {
                playerController.SetPlayerState(EPlayerState.Pass);
            }
            else
            {
                playerController.SetPlayerState(EPlayerState.Run);
            }
        }
        else if (playerController == ClosestPlayerToBall && playerHasBall && PossessionRate <= -0.5f)
        {
            playerController.SetPlayerState(EPlayerState.Challenge);
        }
        else if (playerController == ClosestPlayerToBall && !playerHasBall)
        {
            playerController.SetPlayerState(EPlayerState.RunToBall);
        }
        else
        {
            playerController.SetPlayerState(EPlayerState.Run);
        }
    }


    // Set other player field
    private void FixedUpdate()
    {
        PlayerController newClosestToBall = ClosestPlayerToBall;

        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerController playerController = PlayerList[i].GetComponent<PlayerController>();

            if (playerController.Role == EPlayerRole.Goalkeeper)
            {
                continue;
            }

            if (CompareClosestPlayerToBall(newClosestToBall, playerController))
            {
                newClosestToBall = playerController;
            }
        }

        if (newClosestToBall != ClosestPlayerToBall)
        {
            ClosestPlayerToBall = newClosestToBall;
        }

#if UNITY_EDITOR
        if (newClosestToBall != ClosestPlayerToBall)
        {
            newClosestToBall.textColor = Color.green;
            if (ClosestPlayerToBall != null)
            {
                ClosestPlayerToBall.textColor = Color.white;
            }
        }
#endif
    }

    private bool CompareClosestPlayerToBall(PlayerController currentPlayer, PlayerController targetPlayer)
    {
        if (currentPlayer == null)
        {
           return true;
        }

        if (currentPlayer.TimeToReachBall == -1)
        {
            return true;
        }

        if (currentPlayer.TimeToReachBall > targetPlayer.TimeToReachBall)
        {
            return true;
        }

        return false;
    }

    #region Setter
    public void SetTeamInfo(bool isTeamOne, bool isControlled)
    {
        IsTeamOne = isTeamOne;

        gameObject.name = "Team" + (IsTeamOne ? "One" : "Two");

        IsControlledPlayer = isControlled;

        formationController.InitFormationController(IsTeamOne);
        formationAI.InitFormationAI(IsTeamOne);

        // formationController.
    }


    public void SetControlledPlayer(PlayerController player)
    {
        if (ControlledPlayer != null)
        {
            ControlledPlayer.SetPlayerState(EPlayerState.Run);
        }

        ControlledPlayer = player;

        userInput.SetControlledPlayer(player);
        ControlledPlayer.SetPlayerState(EPlayerState.Controlled);
    }

    public void SetAllTeamPlayer()
    {
        PlayerList = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);

            GameObject childObject = childTransform.gameObject;

            if (childObject.tag == PLAYER_TAG)
            {
                PlayerList.Add(childObject);
            }
        }
    }

    public void SetIsControlledPlayer(bool isControlledPlayer)
    {
        IsControlledPlayer = isControlledPlayer;
        // SetControlledPlayer(null);
    }

    public void SetFormationRec(bool isTeamOne)
    {
        Vector2 pos;
        Quaternion rot;
        Vector2 scale;

        if (isTeamOne)
        {
            pos = new Vector2(-23, 0);
            rot = Quaternion.Euler(0, 0, 0);
            scale = new Vector2(55, 50);
        }
        else
        {
            pos = new Vector2(23, 0);
            rot = Quaternion.Euler(0, 0, 180);
            scale = new Vector2(55, 50);
        }

        formationController.SetFormationRectTransform(pos, rot, scale);
    }

    // Outdated
    public void SetPlayerList(List<GameObject> targetPlayerList)
    {
        PlayerList = targetPlayerList;

        foreach (GameObject player in PlayerList)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();

            NetworkObject networkObject = player.GetComponent<NetworkObject>();

            if (networkObject != null && networkObject.IsSpawned)
            {
                player.transform.SetParent(this.transform);
            }
/*            else
            {
                networkObject?.Spawn();
                player.transform.SetParent(this.transform);
            }*/

            player.name = playerController.Role.ToString() + " " + PlayerList.IndexOf(player);

            Vector2 newPos = formationController.GetWorldPositionByOffset(playerController.DefaultOffset);
            playerController.SetPosition(newPos);
        }

        if (IsControlledPlayer)
        {
            SetControlledPlayer(PlayerList[PlayerList.Count - 1].GetComponent<PlayerController>());
        }
    }
    #endregion
}
