using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

struct PredictionBall
{
    private Vector2 PredictionPosition;
    private Vector2 PredictionVelocity;
}

public class BallMovement : MonoBehaviour
{
    [SerializeField] private GameObject BallPrefab;
    [SerializeField] private Rigidbody2D rb;

    public static string BallTag = "Ball";

    private List<Vector3> PredictPos;
    private Vector3 initTransform;
    private int PredictFrameCount = 120;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PredictPos = new List<Vector3>();
    }

    void Start()
    {
        initTransform = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude > 0)
        {
            PredictPos = GenPredictionPos(transform.position, rb.velocity);
        }
        else
        {
            PredictPos.Clear();
        }
    }

    public void AddForce(float Force, Vector3 Direction)
    {
        rb.AddForce(Direction * Force);
    }

    public void StopForce()
    {
        rb.velocity = Vector2.zero;
    }

    List<Vector3> GenPredictionPos(Vector3 position, Vector3 velocity)
    {
        List<Vector3> calculatedPos = new List<Vector3>();

        Vector3 curPos = transform.position;
        Vector3 curVelocity = velocity;


        float deltaTime = Time.deltaTime;

        for (int i = 1; i <= PredictFrameCount; i++)
        {
            Vector3 predictionVelocity = curVelocity * i * (1 - (rb.drag * deltaTime));
            Vector3 predictionPosition = curPos + (i * deltaTime * predictionVelocity);

            if (i % 5 == 0)
            {

            }

        }

        return calculatedPos;
    }

    Vector3 LastPredictionPos(Vector3 Velocity)
    {
        float deltaTime = Time.deltaTime;

        Vector3 predictionPosition = transform.position;
        Vector3 predictionVelocity = Velocity;

        predictionVelocity *= math.pow((1 - (0.4f * deltaTime)), 120);

        predictionPosition = PredictPos[PredictPos.Count - 1] + predictionVelocity * deltaTime;

        Instantiate(BallPrefab, predictionPosition, Quaternion.identity);

        return predictionPosition;
    }
}
