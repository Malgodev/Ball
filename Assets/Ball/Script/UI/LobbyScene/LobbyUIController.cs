using Malgo.UI;
using SocketIOClient;
using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyUIController : MonoBehaviour
{
    private string playerId = "Player1"; // Sample playerId
    private string playerName = "Malgo"; // Sample playerName
    private string roomId = "match_123";
    private Queue<Action> mainThreadActions = new Queue<Action>();

    private SocketIOUnity socket;


    [Header("Lobby Info")]
    // TODO Change the textfield to inputfield => user can customize room name.
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private ToggleSwitch isPrivate;
    

    [Header("Button")]

    [SerializeField] private Button readyBtn;
    [SerializeField] private Button returnBtn;

    [Header("Player panel")]
    [SerializeField] private TMP_Text playerOneInfoTxt;
    [SerializeField] private TMP_Text playerOneIsReadyTxt;

    [SerializeField] private TMP_Text playerTwoInfoTxt;
    [SerializeField] private TMP_Text playerTwoIsReadyTxt;
    
    [Header("Chat panel")]
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Transform chatContainer;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private Button buttonSend;



    private void Start()
    {
        roomName.text = BallPlayerInfo.Instance.PlayerName;
        socket = new SocketIOUnity("http://localhost:3000");
        Debug.Log("Connecting to chat server...");

        socket.On("connect", (sender) => Debug.Log("Connected to server"));
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("Connected to chat server");
            JoinRoom();
        };

        socket.On("receive_message", (response) =>
        {
            EnqueueMainThreadAction(() => DisplayMessage(response));
        });

        socket.Connect();

        buttonSend.onClick.AddListener(SendMessage);
        readyBtn.onClick.AddListener(() =>
        {
            LobbyMultiplayerManager.Instance.SetLocalPlayerReady();
        });

    }

    public void SetLobbyInfo()
    {
        roomName.text = BallGameLobby.Instance.LobbyName;

        List<PlayerInfo> playerList = BallGameLobby.Instance.GetPlayerInfoList();

        foreach (PlayerInfo playerInfo in playerList)
        {
            Debug.Log(playerInfo);
        }

        playerOneInfoTxt.text = playerList[0].PlayerName;
        playerTwoInfoTxt.text = playerList[1].PlayerName;
    }
    

    public void SetPlayerReadyUI(Dictionary<ulong, bool> playerReadyDict)
    {
        foreach (ulong clientId in playerReadyDict.Keys)
        {
            if (clientId == 0)
            {
                playerOneIsReadyTxt.text = playerReadyDict[clientId] ? "Ready" : "Not ready";
            }
            else
            {
                playerTwoIsReadyTxt.text = playerReadyDict[clientId] ? "Ready" : "Not ready";
            }
        }
    }

    private void setupChat()
    {
        
    }

    private void EnqueueMainThreadAction(Action action)
    {
        lock (mainThreadActions)
        {
            mainThreadActions.Enqueue(action);
        }
    }

    private void Update()
    {
        // Run all actions queued for the main thread
        while (mainThreadActions.Count > 0)
        {
            var action = mainThreadActions.Dequeue();
            action.Invoke();
        }
    }

    private void JoinRoom()
    {
        var data = new { roomId, playerId, playerName }; // Include playerName when joining
        socket.Emit("create_room", data);
    }

    private void SendMessage()
    {
        if (string.IsNullOrEmpty(chatInput.text)) return;

        var message = chatInput.text;
        var data = new { roomId, playerId, playerName, message }; // Include playerName when sending

        socket.Emit("send_message", data);

        chatInput.text = string.Empty;
    }

    private void DisplayMessage(SocketIOResponse response)
    {
        string jsonString = $"{response}";
        Debug.Log($"Received message: {jsonString}");
        string playerId = ExtractJsonValue(jsonString, "playerId");
        string playerName = ExtractJsonValue(jsonString, "playerName");
        string message = ExtractJsonValue(jsonString, "message");

        if (!string.IsNullOrEmpty(playerId) && !string.IsNullOrEmpty(playerName) && !string.IsNullOrEmpty(message))
        {
            DisplayMessage(playerId, playerName, message);
        }
        else
        {
            Debug.Log("Extracted data is null/empty");
        }
    }

    private void DisplayMessage(string playerId, string playerName, string message)
    {
        Debug.Log($"Displaying message from {playerName}: {message}");

        try
        {
            var messageGO = Instantiate(messagePrefab, chatContainer);

            if (messageGO == null)
            {
                Debug.LogError("Failed to instantiate message prefab.");
                return;
            }

            var messageText = messageGO.GetComponent<TextMeshProUGUI>();

            if (messageText == null)
            {
                Debug.LogError("TextMeshProUGUI component not found on the message GameObject.");
                return;
            }

            // Display message with player name and message
            messageText.text = $"{playerName}: {message}";
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in DisplayMessage: {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        socket.Disconnect();
    }

    // Extracts value from the JSON string by key (playerId, playerName, message, etc.)
    string ExtractJsonValue(string json, string key)
    {
        int keyIndex = json.IndexOf(key);

        if (keyIndex == -1)
            return null;

        int valueStartIndex = json.IndexOf(":", keyIndex) + 1;

        char endChar = key == "message" ? '"' : ',';

        int valueEndIndex = json.IndexOf(key == "message" ? '"' : ',', valueStartIndex);

        if (key == "message")
        {
            valueEndIndex = json.IndexOf("\"", valueStartIndex + 1);
        }
        else
        {
            valueEndIndex = json.IndexOf(",", valueStartIndex);
            if (valueEndIndex == -1)
                valueEndIndex = json.IndexOf("}", valueStartIndex);
        }

        string value = json.Substring(valueStartIndex, valueEndIndex - valueStartIndex).Trim('\"');

        return value;
    }
}
