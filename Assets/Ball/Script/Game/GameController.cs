using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Singleton { get; private set; }

    public float CurrentFPS;


    [field: SerializeField] public GameObject Ball { get; private set; }
    [field: SerializeField] public GameObject PlayerPrefab { get; private set; }


    [field: Header("Enviroment")]
    [SerializeField] private GameObject teamOneGoal;
    [SerializeField] private GameObject teamTwoGoal;
    [SerializeField] private Transform upperWall;
    [SerializeField] private Transform lowerWall;
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform rightWall;
    [SerializeField] private GameObject field;

    [field: SerializeField] public TeamController TeamOneController { get; private set; }
    [field: SerializeField] public TeamController TeamTwoController { get; private set; }

    public PlayerController PlayerHasBall { get; private set; }


    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (Singleton != null && Singleton != this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
        }
    }

    // TODO This can put into player data -> select favorite color
    private void SetTeamColor(TeamController team, Color color)
    {
        foreach (GameObject player in team.PlayerList)
        {
            player.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private void Start()
    {
        //GeneratePlayer.GeneratePlayerByTeam(teamOneController);
        //GeneratePlayer.GeneratePlayerByTeam(teamTwoController);

        //SetTeamColor(teamOneController, Color.red);
        //SetTeamColor(teamTwoController, Color.green);
    }

    private void Update()
    {
        CurrentFPS = Mathf.Round(1.0f / Time.deltaTime);
    }

    public void SpawnPlayer(TeamController teamController)
    {
        if (teamController.IsTeamOne)
        {
            GeneratePlayer.GeneratePlayerByTeam(teamController);
            SetTeamController(teamController);
        }
        else
        {
            GeneratePlayer.GeneratePlayerByTeam(teamController);
            SetTeamController(teamController);
        }
    }


    public void SetTeamController(TeamController teamController)
    {
        if (teamController.IsTeamOne)
        {
            TeamOneController = teamController;
            SetTeamColor(TeamOneController, Color.red);
        }
        else
        {
            TeamTwoController = teamController;
            SetTeamColor(TeamTwoController, Color.green);
        }
    }

    public void SetPlayerHasBall(PlayerController player)
    {
        PlayerHasBall = player;
 
        if (player != null)
        {
            // TODO Change this
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
            return TeamTwoController.PlayerList;
        }else
        {
            return TeamOneController.PlayerList;
        }
    }

    public List<GameObject> GetFriendPlayer(bool isTeamOne)
    {
        if (!isTeamOne)
        {
            return TeamTwoController.PlayerList;
        }
        else
        {
            return TeamOneController.PlayerList;
        }
    }

    public float GetDistanceToGoal(bool isTeamOne, PlayerController playerController)
    {
        Vector2 goalPos = !isTeamOne ? teamOneGoal.transform.position : teamTwoGoal.transform.position;

        return Vector2.Distance(goalPos, playerController.transform.position);
    }

    public Vector2 GetDirectionToGoal(bool isTeamOne, PlayerController playerController)
    {
        Vector2 goalPos = !isTeamOne ? teamOneGoal.transform.position : teamTwoGoal.transform.position;
        Vector2 playerPos = playerController.transform.position;
        Vector2 shootingDirection = goalPos - playerPos;
        return shootingDirection.normalized;
    }

    public Transform GetGoal(bool isTeamOne)
    {
        return !isTeamOne ? teamOneGoal.transform : teamTwoGoal.transform;
    }
}
