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
    [field: Header("??")]
    [SerializeField] private Transform lobbiesHolder;

    [field: Header("UI Component")]
    [SerializeField] private Button returnBtn;

    [field: Header("Prefab")]
    [SerializeField] private GameObject lobbyInfoPrefab;

    private void Start()
    {
        returnBtn.onClick.AddListener(() =>
        {
            MainMenuUIController.Instance.SetState(EMainMenuState.Home);
        });

        ListLobbies();
    }

    private async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            foreach (Lobby lobby in queryResponse.Results)
            {

            }

            for (int i = 0; i < 30; i++)
            {
                GameObject lobbyInfo = Instantiate(lobbyInfoPrefab);
                lobbyInfo.transform.SetParent(lobbiesHolder, false);
                lobbyInfo.GetComponent<LobbyInfoController>().SetInfo(i.ToString(), i.ToString(), i);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
