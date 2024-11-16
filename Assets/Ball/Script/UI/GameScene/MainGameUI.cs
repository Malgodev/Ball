using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainGameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeTxt;

    private void FixedUpdate()
    {
        timeTxt.text = Mathf.Floor(GameController.Instance.GamePlayingTimer.Value).ToString();
    }
}
