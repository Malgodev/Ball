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
            BallGameLobby.Instance.CreateLobby(BallPlayerInfo.Instance.PlayerName, false);

            // SceneLoader.Load(SceneLoader.Scene.LobbyScene);
            // MainMenuUIController.Instance.SetState(EMainMenuState.CreateLobby);
        });

        joinLobbyBtn.onClick.AddListener(() =>
        {
            MainMenuUIController.Instance.SetState(EMainMenuState.JoinLobby);
        });

        settingBtn.onClick.AddListener(() =>
        {
            MainMenuUIController.Instance.SetState(EMainMenuState.Setting);
        });

        exitGameBtn.onClick.AddListener(ExitGame);

        rankingBtn.onClick.AddListener(() =>
        {
            MainMenuUIController.Instance.SetState(EMainMenuState.Ranking);
        });
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
