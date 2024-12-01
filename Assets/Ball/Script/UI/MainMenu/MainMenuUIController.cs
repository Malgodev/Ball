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

    [field: Header("UI Components")]
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button joinLobbyBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button exitGameBtn;

    [SerializeField] private EMainMenuState curState;

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

    private void SetState(EMainMenuState newState)
    {

    }

    private void ExitState()
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
