using System;
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

    public List<Vector3> PredictPos { get; private set; }
    private Vector3 initTransform;
    private int PredictFrameCount = 120;

    private int VELOCITY_SCALE = 20;
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
        if (rb.velocity.magnitude > 0 && PredictPos.Count == 0)
        {
            PredictPos = GenPredictionPos(rb.velocity);
        }
        else if (rb.velocity.magnitude > 0 && PredictPos.Count > 0)
        {
            PredictPos.RemoveRange(0, 1);
            PredictPos.Add(LastPredictionPos(rb.velocity));
        }
        else{
            PredictPos.Clear();
        }
    }

    public void AddForce(float Force, Vector3 Direction)
    {
        // this.transform.position += transform.right * 3f;

        rb.AddForce(Direction * Force * VELOCITY_SCALE);
    }

    public void AddForce(Vector2 Force)
    {
        rb.AddForce(Force);
    }

    public void StopForce()
    {
        rb.velocity = Vector2.zero;
    }

    List<Vector3> GenPredictionPos(Vector3 Velocity)
    {
        List<Vector3> calculatedPos = new List<Vector3>();

        Vector3 predictionPosition = transform.position;
        Vector3 predictionVelocity = Velocity;

        float deltaTime = Time.deltaTime;

        for (int i = 1; i <= PredictFrameCount; i++)
        {
            predictionVelocity *= 1 - (rb.drag * deltaTime);

            predictionPosition += predictionVelocity * deltaTime;

            calculatedPos.Add(predictionPosition);
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

        return predictionPosition;
    }

    private void OnDrawGizmos()
    {
        // NullReferenceException: Object reference not set to an instance of an object
        // BallMovement.OnDrawGizmos()(at Assets / Ball / Script / Ball / BallMovement.cs:99)
        // UnityEngine.GUIUtility:ProcessEvent(Int32, IntPtr, Boolean &)
        if (Application.isPlaying && Application.isEditor && PredictPos.Count > 0)
        {
            for (int i = 1; i <= PredictFrameCount / 5; i++)
            {
                GizmosExtra.DrawWireDisk(PredictPos[i * 5 - 1], 0.5f, Color.green);
            }
        }
    }
}
