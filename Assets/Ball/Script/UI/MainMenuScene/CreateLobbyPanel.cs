using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MainMenuUIController;

public class CreateLobbyPanel : BaseUIPanel
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
