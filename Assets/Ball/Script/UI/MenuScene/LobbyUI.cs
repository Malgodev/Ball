//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using Unity.Collections.LowLevel.Unsafe;
//using Unity.Netcode;
//using UnityEngine;
//using UnityEngine.UI;

//public class LobbyUI : NetworkBehaviour
//{
//    [field: Header("Button")]

//    [SerializeField] private Button readyBtn;
//    [SerializeField] private Button returnBtn;

//    [field: Header("Player panel")]

//    [SerializeField] private TMP_Text playerOneInfoTxt;
//    [SerializeField] private TMP_Text playerOneIsReadyTxt;

//    [SerializeField] private TMP_Text playerTwoInfoTxt;
//    [SerializeField] private TMP_Text playerTwoIsReadyTxt;

//    [field: Header("Chat panel")]

//    private void Awake()
//    {
//        returnBtn.onClick.AddListener(() =>
//        {
//            NetworkManager.Singleton.Shutdown();
//            MainMenuController.Instance.SetState(MainMenuController.EMainMenuState.Home);
//        });

//        readyBtn.onClick.AddListener(() =>
//        {
//            MainMenuController.Instance.SetLocalPlayerReady();
//        });
//    }

//    private void Start()
//    {
//        MainMenuController.Instance.OnMenuStateChanged += MainMenuController_OnMenuStateChanged;
//        MainMenuController.Instance.OnReadyChanged += MainMenyController_OnReadyChanged;
//        BallGameMultiplayer.Instance.OnUserDataChanged += BallGameMultiplayer_OnUserDataChanged;
//        BallGameMultiplayer.Instance.OnUserDataChanged += MainMenyController_OnReadyChanged;
//    }

//    private void MainMenyController_OnReadyChanged(object sender, System.EventArgs e)
//    {
//        bool isPlayerOneReady = MainMenuController.Instance.IsPlayerReady(0);
//        bool isPlayerTwoReady = MainMenuController.Instance.IsPlayerReady(1);

//        playerOneIsReadyTxt.text = isPlayerOneReady ? "Ready" : "Not ready";
//        playerTwoIsReadyTxt.text = isPlayerTwoReady ? "Ready" : "Not ready";
//    }

//    private void BallGameMultiplayer_OnUserDataChanged(object sender, System.EventArgs e)
//    {
//        foreach (UserData userdata in BallGameMultiplayer.Instance.UserDataList)
//        {
//            if (userdata.clientId == 0)
//            {
//                playerOneInfoTxt.text = userdata.PlayerName.ToString();
//            }
//            else if (userdata.clientId == 1)
//            {
//                playerTwoInfoTxt.text = userdata.PlayerName.ToString();
//            }
//        }
//    }

//    public override void OnNetworkSpawn()
//    {
//        base.OnNetworkSpawn();

//        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
//    }

//    private void MainMenuController_OnMenuStateChanged(object sender, System.EventArgs e)
//    {
//        // ? thừa
//        if (MainMenuController.Instance.State == MainMenuController.EMainMenuState.Lobby)
//        {
//            Show();
//        }
//        else
//        {
//            Hide();
//        }
//    }

//    private void OnClientConnected(ulong clientId)
//    {
//        if (NetworkManager.Singleton.IsHost)
//        {
//            playerOneInfoTxt.text = "One";
//            playerOneIsReadyTxt.text = "Not ready";

//            // This if kinda useless
//            if (NetworkManager.Singleton.ConnectedClientsList.Count == 1)
//            {
//                playerTwoInfoTxt.text = "Bot";
//                playerTwoIsReadyTxt.text = "Ready";
//            }
//        }
//        else if (NetworkManager.Singleton.IsClient)
//        {
//            playerTwoInfoTxt.text = "Player " + NetworkManager.Singleton.LocalClientId;
//            playerTwoIsReadyTxt.text = "Not ready";
//        }
//    }

//    private void Show()
//    {
//        gameObject.SetActive(true);
//    }

//    private void Hide()
//    {
//        gameObject.SetActive(false);
//    }
//}
