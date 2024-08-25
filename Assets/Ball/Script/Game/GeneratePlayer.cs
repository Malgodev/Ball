using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePlayer : MonoBehaviour
{
    [SerializeField] private static GameObject playerPrefab;

    public static List<PlayerController> GeneratePlayerByFormation(EFormation formation)
    {
        /*        switch (formation)
                {
                    case EFormation.Formation_3_3_2:
                        CreateFormation(Formation.Formation_3_3_2);
                        break;

                    default:
                        Debug.Log("Formation invalid");
                        break;
                }*/

        Debug.Log("Spawn yay");

        return null;
    }

    void CreateFormation(List<EPlayerRole> playerRole)
    {
        PlayerController playerList = new List<PlayerController>();

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
                case EPlayerRole.Goalkeeper:
                    playerList.Add(CreatePlayer(role, new Vector2(0, 0), numberOfDefender, delta));
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

        // TODO Add check to make sure there is only 1 ball owner at a time
/*#if UNITY_EDITOR
        playerList[playerList.Count - 1].GetComponent<PlayerController>().SetIsControlled(true);

        SetPlayerHasBall(playerList[playerList.Count - 1].GetComponent<PlayerController>());
#endif*/
    }

    GameObject CreatePlayer(EPlayerRole role, Vector2 offset, int numberOfRolePlayer, int delta)
    {
        GameObject newPlayer = Instantiate(playerPrefab);
        PlayerController playerController = newPlayer.GetComponent<PlayerController>();
        playerController.SetRole(role);

        offset.y = (delta + 0.5f) * (formationRectangle.height / numberOfRolePlayer);
        playerController.SetDefaultOffset(offset);

        return newPlayer;
    }
}
