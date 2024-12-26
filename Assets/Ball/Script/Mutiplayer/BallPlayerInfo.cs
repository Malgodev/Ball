using UnityEngine;

public class BallPlayerInfo : MonoBehaviour
{
    public static BallPlayerInfo Instance;

    private string playerId;
    [field: SerializeField] public string PlayerName { get; private set; }
    [field: SerializeField] public int PlayerElo { get; private set; }

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
    }


    private void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            int randomIndex = Random.Range(0, 26);
            PlayerName += (char)('A' + randomIndex);
        }

        PlayerElo = Random.Range(0, 1000000);
    }
}
