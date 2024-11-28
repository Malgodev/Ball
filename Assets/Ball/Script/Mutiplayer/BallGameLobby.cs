using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class BallGameLobby : MonoBehaviour
{
    public static BallGameLobby Instance { get; private set; }

    private Lobby joinedLobby;

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
    }


    private async void InitUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options= new InitializationOptions(); 

            options.SetProfile(Random.Range(0, 100000).ToString()); 

            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, BallGameMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
            {
                IsPrivate = isPrivate
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e); 
        }
        {

        }

    }
}
