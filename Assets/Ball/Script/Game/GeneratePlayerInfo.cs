using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GeneratePlayerInfo : MonoBehaviour
{
    // [SerializeField] private static GameObject playerPrefab;

    // Generate player by TeamController
    // @param the team that want to generate
    public static PlayerInfo[] GetPlayerInfo(TeamController teamController)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("GeneratePlayerByTeam can only be called on the server.");
            return null;
        }

        EFormation formation = teamController.formation;
        switch (formation)
        {
            case EFormation.Formation_3_3_2:
                return CreateFormation(Formation.Formation_3_3_2).ToArray();
            default:
                Debug.Log("Formation invalid");
                return null;
        }
    }

    static List<PlayerInfo> CreateFormation(List<EPlayerRole> playerRole)
    {
        List<PlayerInfo> playerList = new List<PlayerInfo>();

        int numberOfFrontline = 0;
        int numberOfMidfield = 0;
        int numberOfDefender = 0;

        // Get the number of player in each role
        // useful for next calculation offset
        foreach (EPlayerRole role in playerRole)
        {
            switch (role)
            {
                case EPlayerRole.Goalkeeper:
                    break;

                case EPlayerRole.Midfielder:
                    numberOfMidfield++;
                    break;

                case EPlayerRole.Fullback:
                    numberOfDefender++;
                    break;

                case EPlayerRole.Winger:
                    numberOfFrontline++;
                    break;

                case EPlayerRole.Striker:
                    numberOfFrontline++;
                    break;

                default:
                    Debug.Log("Invalid role");
                    break;
            }
        }

        int delta = 0;
        EPlayerRole previosRole = EPlayerRole.Goalkeeper;

        foreach (EPlayerRole role in playerRole)
        {
            if (previosRole != role)
            {
                delta = 0;
                previosRole = role;
            }
            else
            {
                delta++;
            }

            switch (role)
            {
                // TODO Change hard code
                // Set the player position more dynamic
                // E.g: Defender will strict together more than midfielder
                case EPlayerRole.Goalkeeper:
                    playerList.Add(CreatePlayerInfo(role, new Vector2(0, 0), 1, 0));
                    break;

                case EPlayerRole.Fullback:
                    playerList.Add(CreatePlayerInfo(role, new Vector2(25, 0), numberOfDefender, delta));
                    break;

                case EPlayerRole.Midfielder:
                    playerList.Add(CreatePlayerInfo(role, new Vector2(50, 0), numberOfMidfield, delta));
                    break;

                case EPlayerRole.Striker:
                    playerList.Add(CreatePlayerInfo(role, new Vector2(80, 0), numberOfFrontline, delta));
                    break;

                case EPlayerRole.Winger:
                    playerList.Add(CreatePlayerInfo(role, new Vector2(70, 0), numberOfFrontline, delta));
                    break;
            }
        }

        return playerList;
        
    }

    static PlayerInfo CreatePlayerInfo(EPlayerRole role, Vector2 offset, int numberOfRolePlayer, int delta)
    {
        offset.y = (delta + 1) * (100 / (numberOfRolePlayer + 1));
        string name = role + " " + (delta + 1);

        return new PlayerInfo() { PlayerName = name, Role = role, Offset = offset};
    }

    //static GameObject CreatePlayer(EPlayerRole role, Vector2 offset, int numberOfRolePlayer, int delta)
    //{
    //    GameObject newPlayer = Instantiate(GameController.Instance.PlayerPrefab);
    //    PlayerController playerController = newPlayer.GetComponent<PlayerController>();
    //    playerController.SetRole(role);

    //    offset.y = (delta + 1) * (100 / (numberOfRolePlayer + 1));
    //    playerController.SetDefaultOffset(offset);

    //    NetworkObject networkObject = newPlayer.GetComponent<NetworkObject>();
    //    networkObject.Spawn();

    //    Debug.Log($"The ClientId for this NetworkObject is: {networkObject.OwnerClientId}");

    //    return newPlayer;
    //}
}
