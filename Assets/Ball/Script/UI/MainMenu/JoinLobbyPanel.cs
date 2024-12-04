using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using static MainMenuUIController;

public class JoinLobbyPanel : BaseUIPanel
{
    [SerializeField] private Button returnBtn;

    private void Start()
    {
        returnBtn.onClick.AddListener(() =>
        {
            MainMenuUIController.Instance.SetState(EMainMenuState.Home);
        });

        // ListLobbies();
    }

/*    private async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log($"Lobbies found: {queryResponse.Results.Count}");


            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log($"Lobby {lobby.Name} Max players {lobby.MaxPlayers}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }*/
}
