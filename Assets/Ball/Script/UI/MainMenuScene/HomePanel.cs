using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
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
    [SerializeField] private Button rankingBtn;

    [field: Header("Panel")]
    [SerializeField] private GameObject simpleRankPanel;

    private void Start()
    {
        createLobbyBtn.onClick.AddListener(() =>
        {
            // Testing
            
            SceneLoader.Load(SceneLoader.Scene.LobbyScene);

            // CreateLobby();

            // MainMenuUIController.Instance.SetState(EMainMenuState.CreateLobby);
        });

        joinLobbyBtn.onClick.AddListener(() =>
        {
            MainMenuUIController.Instance.SetState(EMainMenuState.JoinLobby);
        });

        settingBtn.onClick.AddListener(() => {
            MainMenuUIController.Instance.SetState(EMainMenuState.Setting);
        });

        exitGameBtn.onClick.AddListener(ExitGame);

        rankingBtn.onClick.AddListener(() => {
            MainMenuUIController.Instance.SetState(EMainMenuState.Ranking);
        });
    }

    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "TEST";
            int maxPlayers = 2;

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

            Debug.Log($"Create Lobby {lobby.Name} Max players {lobby.MaxPlayers}");
        } 
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
