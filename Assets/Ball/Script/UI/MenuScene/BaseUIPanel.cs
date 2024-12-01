using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseUIPanel : NetworkBehaviour
{
    [SerializeField] private GameObject panel;

    private void Start()
    {
        
    }

    private void Show()
    {
        panel.SetActive(true);
    }

    private void Hide()
    {
        panel.SetActive(false);
    }
}
