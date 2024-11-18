using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.UI;
using Unity.VisualScripting;

public class RankingManager : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject playerEntryPrefab;

    public Button btnFilterByElo;
    public Button btnFilterByWins;
    public Button btnFilterByWinrate;
    public TMPro.TMP_Text leaderText;

    private int filter = 1;

    private const string eloApiUrl = "http://localhost:3000/api/players/rankingsByElo";
    private const string winsApiUrl = "http://localhost:3000/api/players/rankingsByWins";
    private const string winrateApiUrl = "http://localhost:3000/api/players/rankingsByWinrate";

    void Start()
    {
        StartCoroutine(GetAllPlayers());

        if (btnFilterByElo != null)
        {
            btnFilterByElo.onClick.AddListener(() =>
            {
                Debug.Log("Filter by elo");
                filter = 1;
                leaderText.text = "Elo";
                StartCoroutine(GetAllPlayers());
            });
        }

        if (btnFilterByWins != null)
        {
            btnFilterByWins.onClick.AddListener(() =>
            {
                Debug.Log("Filter by wins");
                filter = 2;
                leaderText.text = "Wins";
                StartCoroutine(GetAllPlayers());
            });
        }

        if (btnFilterByWinrate != null)
        {
            btnFilterByWinrate.onClick.AddListener(() =>
            {
                Debug.Log("Filter by winrate");
                filter = 3;
                leaderText.text = "Winrate";
                StartCoroutine(GetAllPlayers());
            });
        }
    }

    IEnumerator GetAllPlayers()
    {
        string apiUrl = eloApiUrl;

        if (filter == 1)
        {
            apiUrl = eloApiUrl;
        }
        else if (filter == 2)
        {
            apiUrl = winsApiUrl;
        }
        else if (filter == 3)
        {
            apiUrl = winrateApiUrl;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Failed to retrieve data: " + request.error);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                List<Player> players = JsonConvert.DeserializeObject<List<Player>>(jsonResponse);

                DisplayAllPlayers(players);
            }
        }
    }

    void DisplayAllPlayers(List<Player> players)
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count; i++)
        {
            GameObject entry = Instantiate(playerEntryPrefab, contentPanel);
            PlayerEntry playerEntry = entry.GetComponent<PlayerEntry>();

            int rank = i + 1;
            if(filter == 1)
            {
                playerEntry.SetPlayerInfoByElo(rank, players[i].username, players[i].eloRating);
            }
            else if (filter == 2)
            {
                playerEntry.SetPlayerInfoByWins(rank, players[i].username, players[i].wins);
            }
            else if (filter == 3)
            {
                playerEntry.SetPlayerInfoByWinrate(rank, players[i].username, players[i].winRate);
            }
        }
    }
}
