using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameSingleton : MonoBehaviour
{
    public static GameSingleton Instance { get; private set; }

    public float CurrentFPS;

    [field: SerializeField] public GameObject ball { get; private set; }
    [field: SerializeField] public GameObject playerPrefab { get; private set; }

    [field: SerializeField] public TeamController teamOneController { get; private set; }
    [field: SerializeField] public TeamController teamTwoController { get; private set; }

    public PlayerController PlayerHasBall { get; private set; }

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

        // teamTwoController = GetComponent<TeamController>();

    }

    private void Start()
    {
        GeneratePlayer.GeneratePlayerByTeam(teamOneController);
    }

    private void Update()
    {
        CurrentFPS = 1.0f / Time.deltaTime;
    }

    public void SetPlayerHasBall(PlayerController player)
    {
        PlayerHasBall = player;
    }
}
