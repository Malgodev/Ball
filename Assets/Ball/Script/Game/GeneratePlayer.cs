using System.Collections.Generic;
using UnityEngine;

public class GeneratePlayer : MonoBehaviour
{
    // [SerializeField] private static GameObject playerPrefab;

    // Generate player by TeamController
    // @param the team that want to generate
    public static void GeneratePlayerByTeam(TeamController teamController)
    {
        EFormation formation = teamController.formation;
        switch (formation)
        {
            case EFormation.Formation_3_3_2:
                teamController.SetPlayerList(CreateFormation(Formation.Formation_3_3_2));
                break;

            default:
                Debug.Log("Formation invalid");
                break;
        }
    }

    static List<GameObject> CreateFormation(List<EPlayerRole> playerRole)
    {
        List<GameObject> playerList = new List<GameObject>();

        // TODO Rewrite algothrim, maybe factory design pattern

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
                    playerList.Add(CreatePlayer(role, new Vector2(0, 0), 1, 0));
                    break;

                case EPlayerRole.Fullback:
                    playerList.Add(CreatePlayer(role, new Vector2(25, 0), numberOfDefender, delta));
                    break;

                case EPlayerRole.Midfielder:
                    playerList.Add(CreatePlayer(role, new Vector2(50, 0), numberOfMidfield, delta));
                    break;

                case EPlayerRole.Striker:
                    playerList.Add(CreatePlayer(role, new Vector2(80, 0), numberOfFrontline, delta));
                    break;

                case EPlayerRole.Winger:
                    playerList.Add(CreatePlayer(role, new Vector2(70, 0), numberOfFrontline, delta));
                    break;
            }
        }

        return playerList;
        
    }

    static GameObject CreatePlayer(EPlayerRole role, Vector2 offset, int numberOfRolePlayer, int delta)
    {
        GameObject newPlayer = Instantiate(GameController.Instance.playerPrefab);
        PlayerController playerController = newPlayer.GetComponent<PlayerController>();
        playerController.SetRole(role);

        offset.y = (delta + 1) * (100 / (numberOfRolePlayer + 1));
        playerController.SetDefaultOffset(offset);

        return newPlayer;
    }
}
