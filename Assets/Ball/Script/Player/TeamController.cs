using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TeamController : MonoBehaviour
{
    private GameObject ball;

    [field: SerializeField] public EFormation formation { get; private set; }
    [field: SerializeField] public FormationController formationController { get; private set; }

    List<GameObject> PlayerList;

    public PlayerController ControlledPlayer { get; private set; }
    public PlayerController closestPlayerToBall { get; private set; }
    [field: SerializeField] public bool isControlledPlayer { get; private set; }
    private UserInput userInput;

    private void Awake()
    {
        PlayerList = new List<GameObject>();

        userInput = GetComponent<UserInput>();
    }

    private void Start()
    {
        ball = GameController.Instance.ball;
    }

    private void Update()
    {
        foreach (GameObject player in PlayerList)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (playerController == ControlledPlayer || playerController.role == EPlayerRole.Goalkeeper)
            {
                continue;
            }

            Vector2 targetPos = formationController.GetWorldPositionByOffset(playerController.defaultOffset);

            playerController.MoveToPosition(targetPos);
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerController playerController = PlayerList[i].GetComponent<PlayerController>();
            
            if (playerController == CompareclosestPlayerToBall(closestPlayerToBall, playerController))
            {
#if UNITY_EDITOR
                playerController.textColor = Color.green;
                if (closestPlayerToBall != null)
                {
                    closestPlayerToBall.textColor = Color.white;
                }
#endif
                closestPlayerToBall = playerController;
            }

/*            if (closestPlayerToBall.timeToReachBall == -1 ||
                closestPlayerToBall.timeToReachBall > playerController.timeToReachBall)
            {
#if UNITY_EDITOR
                closestPlayerToBall.textColor = Color.white;
                playerController.textColor = Color.green;
#endif
                closestPlayerToBall = playerController;
            }*/
        }
    }

    private PlayerController CompareclosestPlayerToBall(PlayerController currentPlayer, PlayerController targetPlayer)
    {
        if (currentPlayer == null)
        {
           return targetPlayer;
        }

        if (currentPlayer.timeToReachBall == -1)
        {
            return targetPlayer;
        }

        if (currentPlayer.timeToReachBall > targetPlayer.timeToReachBall)
        {
            return targetPlayer;
        }

        return currentPlayer;
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

            player.name = playerController.role.ToString() + " " + PlayerList.IndexOf(player);
            
            Vector2 newPos = formationController.GetWorldPositionByOffset(playerController.defaultOffset);

            playerController.SetPosition(newPos);
        }

        if (isControlledPlayer)
        {
            SetPlayerIsControlled(PlayerList[PlayerList.Count - 1].GetComponent<PlayerController>());
        }
    }
}
