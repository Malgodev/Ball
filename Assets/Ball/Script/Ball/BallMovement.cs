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


    private List<Vector3> PredictPos;
    private Vector3 initTransform;
    private int PredictFrameCount = 120;

    public bool isDribbling = false;

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


/*        if (Input.GetMouseButtonDown(0)) 
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Physics2D.OverlapCircle(mousePos, 0.1f))
            {
                initTransform = mousePos;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float Force = Vector3.Distance(initTransform, mousePos) * -20;

            AddForce(Force, (mousePos - initTransform).normalized);
        }*/

/*        if (PredictPos.Count > 0)
        {
            PredictPos.RemoveAt(0);
            PredictPos.Add(LastPredictionPos(rb.velocity));

        }
        else if (PredictPos.Count == 0 && rb.velocity != Vector2.zero)
        {
            PredictPos = GenPredictionPos(rb.velocity);
        }*/
    }

    public void AddForce(float Force, Vector3 Direction)
    {
        rb.AddForce(Direction * Force);
    }

    public void StopForce()
    {
        rb.velocity = Vector2.zero;
        // ?
    }

    List<Vector3> GenPredictionPos(Vector3 Velocity)
    {
        List<Vector3> calculatedPos = new List<Vector3>();

        Vector3 predictionPosition = transform.position;
        Vector3 predictionVelocity = Velocity;

        float deltaTime = Time.deltaTime;

        for (int i = 1; i <= PredictFrameCount; i++)
        {
            predictionVelocity *= 1 - (0.4f * deltaTime);

            predictionPosition += predictionVelocity * deltaTime;

            Instantiate(BallPrefab, predictionPosition, Quaternion.identity);

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

        Instantiate(BallPrefab, predictionPosition, Quaternion.identity);

        return predictionPosition;
    }
}
