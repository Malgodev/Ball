using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntry : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text scoreText;

    public void SetPlayerInfoByElo(int rank, string name, float score)
    {
        rankText.text = $"#{rank}";
        nameText.text = name;
        scoreText.text = score.ToString();
    }

    public void SetPlayerInfoByWins(int rank, string name, int score)
    {
        rankText.text = $"#{rank}";
        nameText.text = name;
        scoreText.text = score.ToString();
    }

    public void SetPlayerInfoByWinrate(int rank, string name, float score)
    {
        rankText.text = $"#{rank}";
        nameText.text = name;
        scoreText.text = score.ToString() + "%";
    }
}
