using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace TeamController
{
    public class FormationRectangle
    {
        // center X, Y allow team to move for defense and attack option
        public float centerX { get; private set; }
        public float centerY { get; private set; }

        // width, height allow team to compress into a smaller area for more aggresive play and vice versa
        public float width { get; private set; }
        public float height { get; private set; }

       public FormationRectangle(float centerX, float centerY, float width, float height)
        {
            this.centerX = centerX;
            this.centerY = centerY;
            this.width = width;
            this.height = height;
        }
    }

    public class TeamController : MonoBehaviour
    {
        [SerializeField] EFormation formation;

        [SerializeField] public FormationRectangle formationMovement { get; private set; }

        [SerializeField] GameObject PlayerPrefab;

        [SerializeField] GameObject Square;

        List<GameObject> PlayerList;

        private void Awake()
        {
            formationMovement = new FormationRectangle(-15f, 0f, 50f, 60f);

            PlayerList = new List<GameObject>();

#if UNITY_EDITOR
            Square.SetActive(true);
#endif
        }

        void Start()
        {

            switch (formation)
            {

               case EFormation.Formation_3_3_2:
                    CreateFormation(Formation.Formation_3_3_2);

                    break;

                default:
                    Debug.Log("Not choose formation yet");
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            Square.transform.position = new Vector3(formationMovement.centerX, formationMovement.centerY, 0f);
            Square.transform.localScale = new Vector3(formationMovement.width, formationMovement.height, 1f);
#endif
        }

        void CreateFormation(List<EPlayerRole> playerRole)
        {
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



            // 
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
                    case EPlayerRole.Goalkeeper:

                        PlayerList.Add(CreatePlayer(role, new Vector2(-2, 0), numberOfDefender, delta));
                        break;

                    case EPlayerRole.Fullback:
                        PlayerList.Add(CreatePlayer(role, new Vector2(-1, 0), numberOfDefender, delta));
                        break;

                    case EPlayerRole.Midfielder:
                        PlayerList.Add(CreatePlayer(role, new Vector2(0, 0), numberOfMidfield, delta));
                        break;

                    case EPlayerRole.Striker:
                        PlayerList.Add(CreatePlayer(role, new Vector2(1, 0), numberOfFrontline, delta));
                        break;

                    case EPlayerRole.Winger:
                        PlayerList.Add(CreatePlayer(role, new Vector2(0.5f, 0), numberOfFrontline, delta));
                        break;

                }
            }

            /*            GameObject newPlayer = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity); 
                        newPlayer.GetComponent<PlayerController>().SetRole(playerRole);

                        foreach (GameObject player in PlayerList)
                        {
                            if (player.GetComponent<PlayerController>().role == playerRole)
                            {
                                Debug.Log("Player already exist");
                                return null;
                            }
                        }*/

        }

/*                            case EPlayerRole.Goalkeeper:
                        newPlayer = Instantiate(PlayerPrefab);
        playerController = newPlayer.GetComponent<PlayerController>();

                        playerController.SetRole(role);
                        playerController.SetDefaultOffset(new Vector2(-2f, 0f));

                        PlayerList.Add(newPlayer);
                        break;*/
        GameObject CreatePlayer(EPlayerRole role, Vector2 offset, int numberOfRolePlayer, int delta)
        {
            GameObject newPlayer = Instantiate(PlayerPrefab);
            PlayerController playerController = newPlayer.GetComponent<PlayerController>();
            playerController.SetRole(role);

            offset.y = numberOfRolePlayer - delta - Mathf.Ceil(numberOfRolePlayer / 2);
            playerController.SetDefaultOffset(offset);

            return newPlayer;
        }
    }
}
