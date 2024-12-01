using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    public static MainMenuUIController Instance;

    public enum EMainMenuState
    {
        Home,
        Setting,
        CreateLobby,
        JoinLobby,
        Ranking,
        Login,
    }

    [SerializeField] private EMainMenuState curState;

    [field: Header("Panel")]
    [SerializeField] private BaseUIPanel homePanel;
    [SerializeField] private BaseUIPanel settingPanel;
    [SerializeField] private BaseUIPanel createLobbyPanel;
    [SerializeField] private BaseUIPanel joinLobbyPanel;
    [SerializeField] private BaseUIPanel rankingPanel;
    [SerializeField] private BaseUIPanel loginPanel;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetState(EMainMenuState newState)
    {

        ExitState();

        switch (newState)
        {
            case EMainMenuState.Home:
                homePanel.Show();
                break;
            case EMainMenuState.Setting:
                Debug.LogWarning("Not set Setting panel yet");
                break;
            case EMainMenuState.CreateLobby:
                break;
            case EMainMenuState.JoinLobby:
                break;
            case EMainMenuState.Ranking:
                break;
            case EMainMenuState.Login:
                break;
        }

        curState = newState;
    }

    public void ExitState()
    {
        switch (curState)
        {
            case EMainMenuState.Home:
                break;
            case EMainMenuState.Setting:
                break;
            case EMainMenuState.CreateLobby:
                break;
            case EMainMenuState.JoinLobby:
                break;
            case EMainMenuState.Ranking:
                break;
            case EMainMenuState.Login:
                break;
        }
    }
}
