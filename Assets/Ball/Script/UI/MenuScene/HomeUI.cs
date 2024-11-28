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
            MainMenuController.Instance.SetState(MainMenuController.EMainMenuState.Lobby);
            BallGameMultiplayer.Instance.StartHost();
        });

        joinLobbyBtn.onClick.AddListener(() =>
        {
            MainMenuController.Instance.SetState(MainMenuController.EMainMenuState.Lobby);
            BallGameMultiplayer.Instance.StartClient();
        });
    }

    private void Start()
    {
        MainMenuController.Instance.OnMenuStateChanged += MainMenuController_OnMenuStateChanged;
    }

    private void MainMenuController_OnMenuStateChanged(object sender, System.EventArgs e)
    {
        // ? thừa
        if (MainMenuController.Instance.State == MainMenuController.EMainMenuState.Home)
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
