using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

[Serializable]
public class TeamController : MonoBehaviour
{
    private GameObject ball;

    [field: SerializeField] public EFormation formation { get; private set; }
    [field: SerializeField] public FormationController formationController { get; private set; }
    [field: SerializeField] public FormationAI formationAI { get; private set; }

    public List<GameObject> PlayerList { get; private set; }

    public PlayerController ControlledPlayer { get; private set; }
    public PlayerController ClosestPlayerToBall { get; private set; }
    [field: SerializeField] public bool IsControlledPlayer { get; private set; }
    private UserInput userInput;

    public bool IsTeamOne = false;

    private int updateCounter = 0;

    private void Awake()
    {
        PlayerList = new List<GameObject>();

        userInput = GetComponent<UserInput>();
    }

    private void Start()
    {
        ball = GameController.Instance.ball;
        formationController.IsTeamOne = IsTeamOne;
        formationAI.IsTeamOne = IsTeamOne;
    }

    private void Update()
    {
        updateCounter++;
        if (updateCounter >= 5)
        {
            updateCounter = 0;
            return;
        }

        foreach (GameObject player in PlayerList)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            Vector2 formationScale = formationController.formationScale;
            playerController.MoveableRadius = Mathf.Max(formationScale.x, formationScale.y) / 3;

            if (playerController == ControlledPlayer || playerController.Role == EPlayerRole.Goalkeeper)
            {
                continue;
            }
            Vector2 defaultTargetPos = formationController.GetWorldPositionByOffset(playerController.defaultOffset);
            GetTargetPositionByRole(player, defaultTargetPos);

            // playerController.MoveToPosition(defaultTargetPos);
        }
    }

    private void GetTargetPositionByRole(GameObject player, Vector2 defaultPos)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        EPlayerRole role = playerController.Role;
        float PossessionRate = formationAI.PossessionBalance;
        float DangerRate = playerController.DangerRate;

        PlayerController playerHasBall = GameController.Instance.PlayerHasBall;

        playerController.SetPlayerTargetPosition(defaultPos);

        if (playerController == playerHasBall)
        {
            if (PossessionRate >= 0.9f && GameController.Instance.GetDistanceToGoal(IsTeamOne, playerController) < 10f)
            {
                playerController.SetPlayerState(EPlayerState.Shot);
                // playerController.AutoPassBall(ball);
            }
            else if (DangerRate >= 0.5f)
            {
                playerController.SetPlayerState(EPlayerState.Pass);
                // playerController.AutoRun(ball);
            }
            else
            {
                playerController.SetPlayerState(EPlayerState.Run);
            }
        }
        else if (playerController == ClosestPlayerToBall && playerHasBall && PossessionRate <= -0.5f)
        {
            Vector2 targetPos = playerHasBall.transform.position + playerHasBall.transform.right;

            // playerController.MoveToPosition(targetPos, false);

            playerController.SetPlayerState(EPlayerState.Challenge);
        }
        else if (playerController == ClosestPlayerToBall && !playerHasBall)
        {
            playerController.SetPlayerState(EPlayerState.RunToBall);
            // playerController.MoveToPosition(ball.transform.position, false);
        }
        else
        {
            playerController.SetPlayerState(EPlayerState.Run);
        }
    }

    private Vector2 GetFullbackPosition(GameObject player, float PossessionRate, float DangerRate)
    {
        Vector2 targetPos = new Vector2();
        PlayerController playerController = player.GetComponent<PlayerController>();
        GameObject ball = GameController.Instance.ball;

        targetPos.y = ball.transform.position.y;    

        if (PossessionRate <= 0f)
        {
            targetPos.x = (IsTeamOne ? -1f : 1f) * playerController.MoveableRadius;
        }
        else
        {
            targetPos.x = (IsTeamOne ? 1f : -1f) * playerController.MoveableRadius;
        }

        return targetPos;
    }

    private void FixedUpdate()
    {
        PlayerController newClosestToBall = ClosestPlayerToBall;

        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerController playerController = PlayerList[i].GetComponent<PlayerController>();

            if (CompareClosestPlayerToBall(newClosestToBall, playerController))
            {
                newClosestToBall = playerController;
            }
        }
#if UNITY_EDITOR
        if (newClosestToBall != ClosestPlayerToBall)
        {
            newClosestToBall.textColor = Color.green;
            if (ClosestPlayerToBall != null)
            {
                ClosestPlayerToBall.textColor = Color.white;
            }
            ClosestPlayerToBall = newClosestToBall;
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

    public void SetPlayerIsControlled(PlayerController player)
    {
        ControlledPlayer = player;

        userInput.SetControlledPlayer(player);
    }

    public void SetPlayerList(List<GameObject> targetPlayerList)
    {
        PlayerList = targetPlayerList;

        foreach (GameObject player in PlayerList)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();

            player.transform.parent = this.transform;

            player.name = playerController.Role.ToString() + " " + PlayerList.IndexOf(player);
            
            Vector2 newPos = formationController.GetWorldPositionByOffset(playerController.defaultOffset);

            playerController.SetPosition(newPos);
        }

        if (IsControlledPlayer)
        {
            SetPlayerIsControlled(PlayerList[PlayerList.Count - 1].GetComponent<PlayerController>());
        }
    }
}
