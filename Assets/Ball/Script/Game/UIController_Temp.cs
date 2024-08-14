using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class UIController_Temp : MonoBehaviour
{
    [SerializeField] private Button ResetButton;


    private void Start()
    {
        ResetButton.onClick.AddListener(ResetScene);
    }

    void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
