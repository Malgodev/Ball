using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : NetworkBehaviour
{
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button joinLobbyBtn;

    private void Awake()
    {
        createLobbyBtn.onClick.AddListener(() =>
        {
            MainMenuUIController.Instance.SetState(MainMenuUIController.EMainMenuState.Lobby);
            BallGameMultiplayer.Instance.StartHost();
        });

        joinLobbyBtn.onClick.AddListener(() =>
        {
            MainMenuUIController.Instance.SetState(MainMenuUIController.EMainMenuState.Lobby);
            BallGameMultiplayer.Instance.StartClient();
        });
    }

    private void Start()
    {
        MainMenuUIController.Instance.OnMenuStateChanged += MainMenuController_OnMenuStateChanged;
    }

    private void MainMenuController_OnMenuStateChanged(object sender, System.EventArgs e)
    {
        // ? thừa
        if (MainMenuUIController.Instance.State == MainMenuUIController.EMainMenuState.Home)
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
