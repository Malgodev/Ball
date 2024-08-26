using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[Serializable]
public class TeamController : MonoBehaviour
{
    [field: SerializeField] public EFormation formation { get; private set; }
    [field: SerializeField] public FormationController formationController { get; private set; }

    private UserInput userInput;

    List<GameObject> PlayerList;

    public PlayerController ControlledPlayer { get; private set; }

    private void Awake()
    {
        PlayerList = new List<GameObject>();

        userInput = GetComponent<UserInput>();
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
            Debug.Log(userInput.inputVector);
        }
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

            Vector2 newPos = formationController.GetWorldPositionByOffset(playerController.defaultOffset);

            playerController.SetPosition(newPos);
        }

        SetPlayerIsControlled(PlayerList[PlayerList.Count - 1].GetComponent<PlayerController>());
    }
}
