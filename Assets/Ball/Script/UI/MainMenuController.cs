using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button btn;

    private void Awake()
    {
        btn.onClick.AddListener(GetOnButtonClicked);
    }

    private void GetOnButtonClicked()
    {
        Debug.Log("love you");
    }
}
