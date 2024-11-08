using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public float CurrentFPS;

    [field: SerializeField] public GameObject ball { get; private set; }
    [field: SerializeField] public GameObject playerPrefab { get; private set; }

    [field: SerializeField] public GameObject teamOneGoal { get; private set; }
    [field: SerializeField] public GameObject teamTwoGoal { get; private set; }

    [field: SerializeField] public TeamController teamOneController { get; private set; }
    [field: SerializeField] public TeamController teamTwoController { get; private set; }

    public PlayerController PlayerHasBall { get; private set; }

    public GameObject Field;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        teamOneController.IsTeamOne = true;
        teamTwoController.IsTeamOne = false;



        // teamTwoController = GetComponent<TeamController>();
    }

    private void SetTeamColor(TeamController team, Color color)
    {
        foreach (GameObject player in team.PlayerList)
        {
            player.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private void Start()
    {
        GeneratePlayer.GeneratePlayerByTeam(teamOneController);
        GeneratePlayer.GeneratePlayerByTeam(teamTwoController);

        SetTeamColor(teamOneController, Color.red);
        SetTeamColor(teamTwoController, Color.green);
    }

    private void Update()
    {
        CurrentFPS = 1.0f / Time.deltaTime;
    }


    public void SetPlayerHasBall(PlayerController player)
    {
        PlayerHasBall = player;

        if (player != null)
        {
            StartCoroutine(player.DribblingBall());
        }
        else
        {
            PlayerHasBall = player;
        }
    }

    public ETeamHasBall GetTeamHasBall()
    {
        if (PlayerHasBall == null)
        {
            return ETeamHasBall.None;
        }
        
        if (PlayerHasBall.GetComponent<PlayerController>().IsTeamOne)
        {
            return ETeamHasBall.TeamOne;
        }
        else
        {
            return ETeamHasBall.TeamTwo;
        }
    }

    public List<GameObject> GetEnemyPlayer(bool isTeamOne)
    {
        if (isTeamOne)
        {
            return teamTwoController.PlayerList;
        }else
        {
            return teamOneController.PlayerList;
        }
    }

    public List<GameObject> GetFriendPlayer(bool isTeamOne)
    {
        if (!isTeamOne)
        {
            return teamTwoController.PlayerList;
        }
        else
        {
            return teamOneController.PlayerList;
        }
    }

    public float GetDistanceToGoal(bool isTeamOne, PlayerController playerController)
    {
        Vector2 goalPos = !isTeamOne ? teamOneGoal.transform.position : teamTwoGoal.transform.position;

        return Vector2.Distance(goalPos, playerController.transform.position);
    }
}
