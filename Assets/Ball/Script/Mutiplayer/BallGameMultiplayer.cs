using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BallGameMultiplayer : NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 2;

    public static BallGameMultiplayer Instance { get; private set; }

    public NetworkList<UserData> UserDataList { get; private set; }
    public event EventHandler OnUserDataChanged;

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

        DontDestroyOnLoad(gameObject);

        UserDataList = new NetworkList<UserData>();
        UserDataList.OnListChanged += UserDataList_OnListChanged;
    }

    private void UserDataList_OnListChanged(NetworkListEvent<UserData> changeEvent)
    {
        OnUserDataChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        FixedString64Bytes name = clientId == 0 ? "One" : "Two";

        UserDataList.Add(new UserData
        {
            clientId = clientId,
            PlayerName = name,
        });
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public UserData GetUserDataByClientId(ulong clientId)
    {
        return UserDataList[(int) clientId];
    }
}
