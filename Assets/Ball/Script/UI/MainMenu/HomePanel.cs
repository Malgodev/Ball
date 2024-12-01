using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MainMenuUIController;

public class HomePanel : BaseUIPanel
{
    [field: Header("UI Components")]
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button joinLobbyBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button exitGameBtn;

    [field: Header("Panel")]
    [SerializeField] private GameObject simpleRankPanel;

    private void Start()
    {
        createLobbyBtn.onClick.AddListener(() =>
        {
            MainMenuUIController.Instance.SetState(EMainMenuState.CreateLobby);
        });

        joinLobbyBtn.onClick.AddListener(() =>
        {
            MainMenuUIController.Instance.SetState(EMainMenuState.JoinLobby);
        });

        settingBtn.onClick.AddListener(() => {
            MainMenuUIController.Instance.SetState(EMainMenuState.Setting);
        });

        exitGameBtn.onClick.AddListener(ExitGame);
    }
    private void ExitGame()
    {
        Application.Quit();
    }
}
