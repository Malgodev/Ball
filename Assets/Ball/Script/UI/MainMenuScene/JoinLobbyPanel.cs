using System;
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
    }

    private void OnEnable()
    {
        ListLobbies();
    }

    private async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            ClearLobbies();

            foreach (Lobby lobby in queryResponse.Results)
            {
                GameObject lobbyInfo = Instantiate(lobbyInfoPrefab);
                lobbyInfo.transform.SetParent(lobbiesHolder, false);
                lobbyInfo.GetComponent<LobbyInfoController>().SetInfo(
                    lobby.Id, lobby.Name, lobby.Data["LobbyElo"].Value);

            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void ClearLobbies()
    {
        foreach (Transform child in lobbiesHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
