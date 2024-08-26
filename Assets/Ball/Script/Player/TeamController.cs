using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TeamController : MonoBehaviour
{
    private GameObject ball;

    [field: SerializeField] public EFormation formation { get; private set; }
    [field: SerializeField] public FormationController formationController { get; private set; }

    private UserInput userInput;

    List<GameObject> PlayerList;

    public PlayerController ControlledPlayer { get; private set; }

    private void Awake()
    {
        PlayerList = new List<GameObject>();

        userInput = GetComponent<UserInput>();

        userInput.OnShotBall += ShotBall;
    }

    private void Start()
    {
        ball = GameController.Instance.ball;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (ControlledPlayer != null && userInput != null)
        {
            // ControlledPlayer.MoveToPosition();
            ControlledPlayer.MoveToPositionByAxis(userInput.inputVector);
        }
    }

    private void ShotBall()
    {
        // wtf is this code?
        // Shoud I covert this to singletone
        GameController.Instance.PlayerHasBall.ShotBall(ball);
    }

    public void SetPlayerIsControlled(PlayerController player)
    {
        ControlledPlayer = player;
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

        SetPlayerIsControlled(PlayerList[PlayerList.Count - 1].GetComponent<PlayerController>());
    }
}
