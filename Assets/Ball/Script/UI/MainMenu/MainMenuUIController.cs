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
                settingPanel.Show();
                break;
            case EMainMenuState.CreateLobby:
                createLobbyPanel.Show();
                break;
            case EMainMenuState.JoinLobby:
                joinLobbyPanel.Show();  
                break;
            case EMainMenuState.Ranking:
                break;
            case EMainMenuState.Login:
                break;
        }

        curState = newState;
    }

    private void ExitState()
    {
        switch (curState)
        {
            case EMainMenuState.Home:
                homePanel.Hide();
                break;
            case EMainMenuState.Setting:
                settingPanel.Hide();
                break;
            case EMainMenuState.CreateLobby:
                createLobbyPanel.Hide();    
                break;
            case EMainMenuState.JoinLobby:
                joinLobbyPanel.Hide();
                break;
            case EMainMenuState.Ranking:
                break;
            case EMainMenuState.Login:
                break;
        }
    }
}
