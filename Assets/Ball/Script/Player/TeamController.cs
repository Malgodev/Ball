using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace TeamController
{
    public class FormationRectangle
    {
        // Should rewrite to make this class base on GameObject rather than float value
        
        public GameObject square { get; private set; }

        // center X, Y allow team to move for defense and attack option
        public float centerX { get; private set; }
        public float centerY { get; private set; }

        // width, height allow team to compress into a smaller area for more aggresive play and vice versa
        public float width { get; private set; }
        public float height { get; private set; }

        // ratio between w,h and square scale
        private float ratio;

       public FormationRectangle(GameObject square, float centerX, float centerY, float width, float height)
       {
            this.square = square;
            this.centerX = centerX;
            this.centerY = centerY;
            this.width = width;
            this.height = height;

            ratio = square.transform.localScale.x / width;
       }

        public Vector3 GetWorldPositionByOffset(Vector3 offset)
        {
            Vector2 result = Vector2.zero;

            result.x = centerX + (offset.x / 100 * width * ratio) - width * ratio / 2;
            result.y = centerY + (offset.y / 100 * height * ratio) - height * ratio / 2;

            return result;
        }
    }

    public class TeamController : MonoBehaviour
    {
        [SerializeField] EFormation formation;

        [SerializeField] public FormationRectangle formationRectangle { get; private set; }

        [SerializeField] GameObject PlayerPrefab;

        [SerializeField] GameObject Square;

        List<GameObject> PlayerList;

        private void Awake()
        {
            formationRectangle = new FormationRectangle(Square, -15f, 0f, 100f, 100f);

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
            Square.transform.position = new Vector3(formationRectangle.centerX, formationRectangle.centerY, 0f);
            Square.transform.localScale.Scale(new Vector3(formationRectangle.width / 100, formationRectangle.height / 100, 1f));
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

                        PlayerList.Add(CreatePlayer(role, new Vector2(0, 0), numberOfDefender, delta));
                        break;

                    case EPlayerRole.Fullback:
                        PlayerList.Add(CreatePlayer(role, new Vector2(25, 0), numberOfDefender, delta));
                        break;

                    case EPlayerRole.Midfielder:
                        PlayerList.Add(CreatePlayer(role, new Vector2(50, 0), numberOfMidfield, delta));
                        break;

                    case EPlayerRole.Striker:
                        PlayerList.Add(CreatePlayer(role, new Vector2(80, 0), numberOfFrontline, delta));
                        break;

                    case EPlayerRole.Winger:
                        PlayerList.Add(CreatePlayer(role, new Vector2(70, 0), numberOfFrontline, delta));
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


            offset.y = (delta + 0.5f) * (formationRectangle.height / numberOfRolePlayer);
            playerController.SetDefaultOffset(offset);

            return newPlayer;
        }
    }
}
