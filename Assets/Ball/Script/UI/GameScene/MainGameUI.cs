using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainGameUI : MonoBehaviour
{
    public static MainGameUI Instance { get; private set; }

    [SerializeField] private TMP_Text timeTxt;
    [field: SerializeField] public FixedJoystick Joystick { get; private set; }
    [field: SerializeField] public Button ShotButton { get; private set; }
    [field: SerializeField] public Button PassButton { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void FixedUpdate()
    {
        timeTxt.text = Mathf.Floor(GameController.Instance.GamePlayingTimer.Value).ToString();
    }
}
