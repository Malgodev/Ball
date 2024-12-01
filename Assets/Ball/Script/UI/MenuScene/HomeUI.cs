using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : NetworkBehaviour
{
    [SerializeField] private TMP_InputField nameField;

    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button joinLobbyBtn;

    private void Awake()
    {
        createLobbyBtn.onClick.AddListener(() =>
        {
            BallGameLobby.Instance.CreateLobby("Dead", false);

            //MainMenuController.Instance.SetState(MainMenuController.EMainMenuState.Lobby);
            //BallGameMultiplayer.Instance.StartHost();
        });

        joinLobbyBtn.onClick.AddListener(() =>
        {
            BallGameLobby.Instance.QuickJoin();
            //MainMenuController.Instance.SetState(MainMenuController.EMainMenuState.Lobby);
            //BallGameMultiplayer.Instance.StartClient();
        });
    }

    private void Start()
    {
        LobbyController.Instance.OnMenuStateChanged += MainMenuController_OnMenuStateChanged;
    }

    private void MainMenuController_OnMenuStateChanged(object sender, System.EventArgs e)
    {
        // ? thừa
        if (LobbyController.Instance.State == LobbyController.EMainMenuStateTmp.Home)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
