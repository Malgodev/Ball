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

    List<GameObject> PlayerList;

    public PlayerController ControlledPlayer { get; private set; }

    private void Awake()
    {
        PlayerList = new List<GameObject>();
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    public bool SetPlayerIsControlled(PlayerController player)
    {
        if (ControlledPlayer != null)
        {
            ControlledPlayer.SetIsControlled(false);
        }

        ControlledPlayer = player;
        ControlledPlayer.SetIsControlled(true);
        return true;
    }

    public void SetPlayerList(List<GameObject> targetPlayerList)
    {
        PlayerList = targetPlayerList;

        foreach (GameObject player in PlayerList)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();

            Vector2 newPos = formationController.GetWorldPositionByOffset(playerController.defaultOffset);

            playerController.SetPosition(newPos);

            Debug.Log(newPos);
        }
    }
}
