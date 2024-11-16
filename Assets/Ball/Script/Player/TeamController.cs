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
    [field: SerializeField] public PlayerController ClosestPlayerToBall { get; private set; }

    [field: Header("User control")]
    [field: SerializeField] public bool IsControlledPlayer { get; private set; }
    [field: SerializeField] public PlayerController ControlledPlayer { get; private set; }

    private UserInput userInput;

    private int updateCounter = 0;

    public const string PLAYER_TAG = "Player";

    private void Awake()
    {
        PlayerList = new List<GameObject>();

        userInput = GetComponent<UserInput>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        ball = GameController.Instance.Ball;
        this.gameObject.name = "Team" + (IsTeamOne ? "One" : "Two");
    }

    // Mostly use for set player state
    // From that, player will automaticly move by that state
    private void Update()
    {
        if (GameController.Instance.State.Value != GameController.GameState.GamePlaying)
        {
            return;
        }

        if (IsControlledPlayer)
        {
            ControlPlayer();
        }

        updateCounter++;
        if (updateCounter >= 5)
        {
            updateCounter = 0;
            return;
        }

        DelayUpdate();
    }

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
            newClosestToBall.textColor = Color.green;
            if (ClosestPlayerToBall != null)
            {
                ClosestPlayerToBall.textColor = Color.white;
            }
            ClosestPlayerToBall = newClosestToBall;
        }
    }

    private void ControlPlayer()
    {
        if (ControlledPlayer == null)
        {
            return;
        }

        switch (userInput.InputState)
        {
            case EPlayerState.Run:
                ControlledPlayer.MoveByAxis(userInput.InputVector);
                break;
            case EPlayerState.Shot:
                ControlledPlayer.ShotBall(ball);
                userInput.InputState = EPlayerState.Run;
                break;
        }
    }

    private void DelayUpdate()
    {
        // Set player logic
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

        // Set controlled player
        SetControlledPlayer();
    }

    private void SetControlledPlayer()
    {
        // TODO Change to control to closest player to ball
        if (IsControlledPlayer)
        {
            SetControlledPlayer(PlayerList[PlayerList.Count - 1].GetComponent<PlayerController>());
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
    [ClientRpc]
    public void SetTeamInfoClientRpc(bool isTeamOne, bool isControlled)
    {
        IsTeamOne = isTeamOne;

        gameObject.name = "Team" + (IsTeamOne ? "One" : "Two");

        IsControlledPlayer = isControlled;

        formationController.InitFormationControllerClientRpc(IsTeamOne);
        formationAI.InitFormationAIClientRpc(IsTeamOne);
    }

    public void SetControlledPlayer(PlayerController player)
    {
        if (ControlledPlayer != null)
        {
            ControlledPlayer.SetPlayerState(EPlayerState.Run);
        }

        ControlledPlayer = player;

        ControlledPlayer.SetPlayerState(EPlayerState.Controlled);
    }

    [ClientRpc]
    public void SetPlayerListClientRpc()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag(PLAYER_TAG))
            {
                PlayerList.Add(child.gameObject);
            }
        }
    }
    #endregion
}
