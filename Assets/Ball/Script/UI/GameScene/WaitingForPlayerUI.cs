using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WaitingForPlayerUI : NetworkBehaviour
{
    [SerializeField] private Button btn;

    private void Awake()
    {
        btn.onClick.AddListener(() =>
        {
            GameController.Instance.SetLocalPlayerReady(true);
        });
    }

    private void Start()
    {
        GameController.Instance.OnStateChanged += GameController_OnStateChanged;
        GameController.Instance.OnLocalPlayerReadyChange += GameController_OnLocalPlayerReadyChange;
    }

    private void GameController_OnLocalPlayerReadyChange(object sender, System.EventArgs e)
    {
        if (GameController.Instance.IsLocalPlayerReady)
        {
            Show();
        }
    }

    private void GameController_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameController.Instance.State.Value == GameController.GameState.GamePlaying)
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
