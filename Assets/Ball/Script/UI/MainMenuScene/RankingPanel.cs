using UnityEngine;
using UnityEngine.UI;
using static MainMenuUIController;

public class RankingPanel : BaseUIPanel
{
    [SerializeField] private Button returnBtn;

    private void Start()
    {
        returnBtn.onClick.AddListener(() =>
        {
            MainMenuUIController.Instance.SetState(EMainMenuState.Home);
        });
    }
}
