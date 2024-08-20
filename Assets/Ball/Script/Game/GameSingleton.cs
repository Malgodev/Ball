using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameSingleton : MonoBehaviour
{
    public static GameSingleton Instance { get; private set; }

    public TeamController.TeamController teamController { get; private set; }

    public float CurrentFPS;

    [field: SerializeField] public GameObject ball { get; private set; }
    

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

        teamController = GetComponent<TeamController.TeamController>();
    }

    private void Update()
    {
        CurrentFPS = 1.0f / Time.deltaTime;
    }
}
