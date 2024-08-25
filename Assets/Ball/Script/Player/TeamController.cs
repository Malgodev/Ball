using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    [SerializeField] EFormation formation;
    [SerializeField] public FormationRectangle formationRectangle { get; private set; }

    [SerializeField] GameObject PlayerPrefab;

    [SerializeField] GameObject Square;



    List<GameObject> PlayerList;
    public PlayerController PlayerHasBall { get; private set; }
    public PlayerController ControlledPlayer { get; private set; }

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

    void Update()
    {
        // show formation rectangle, only work in editor
#if UNITY_EDITOR
        Square.transform.position = new Vector3(formationRectangle.centerX, formationRectangle.centerY, 0f);
        Square.transform.localScale.Scale(new Vector3(formationRectangle.width / 100, formationRectangle.height / 100, 1f));

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetPlayerHasBall(PlayerList[PlayerList.Count - 2].GetComponent<PlayerController>());
            PlayerList[PlayerList.Count - 2].GetComponent<PlayerController>().SetIsControlled(true);

            SetPlayerHasBall(PlayerList[PlayerList.Count - 2].GetComponent<PlayerController>());
            PlayerList[PlayerList.Count - 2].GetComponent<PlayerController>().SetIsControlled(true);
        }
#endif
    }




    public bool SetPlayerHasBall(PlayerController player)
    {
        if (PlayerHasBall != null)
        {
            PlayerHasBall.SetIsBallOwner(false);

        }

        PlayerHasBall = player;
        PlayerHasBall.SetIsBallOwner(true);
        return true;
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
}
