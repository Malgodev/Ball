using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player need time to reach maximum speed
    // and also need time to slower down

    public EPlayerRole role { get; private set; }

    // The position of player in formation rectangle
    public Vector2 defaultOffset { get; private set; }

    // Movement
    [SerializeField] private float moveSpeed = 10f;
    private static float moveSpeedScale = 250f;

    public Rigidbody2D rb { get; private set; }

    private GameObject ball;

    UserInput userInput;

    public IEnumerator dribblingBall { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        ball = GameController.Instance.ball;

        // formationRectangle = GameSingleton.Instance.teamController.formationRectangle;
    }

    public void SetDefaultOffset(Vector2 DefaultOffset)
    {
        this.defaultOffset = DefaultOffset;
    }

    void Start()
    {
        dribblingBall = DribblingBall();
    }

    // Update is called once per frame
    void Update()
    {
        SpeedControl();
    }

    private void FixedUpdate()
    {
        // TimeToReachBall();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(BallMovement.BallTag))
        {
            GameController.Instance.SetPlayerHasBall(this);
        }
    }

    private float TimeToReachBall()
    {
        BallMovement ballMovement = GameController.Instance.ball.GetComponent<BallMovement>();

        if (ballMovement.PredictPos.Count == 0)
        {
            return -1;  
        }

        float radius = 0;
        float frame = 0;
        
        // TODO hard code 12
        // Possible solution: using deltatime to check if 2sec has pass
        for (int i = 1; i <= 120; i++)
        {
            // function to calculate moveable position in next frame
            radius += moveSpeed * Time.fixedDeltaTime;
            frame = i;

            if (Vector3.Distance(this.transform.position, ballMovement.PredictPos[i - 1]) < radius)
            {
                return frame;
            }
        }

        return frame;
    }

    #region Initialization
    public void SetPosition(Vector2 targetPos)
    {
        transform.position = targetPos;
    }

    public void SetRole(EPlayerRole role)
    {
        this.role = role;
    }
    #endregion

    // Put this into other player script
    #region Movement controller

    /// <summary>
    /// Try to move player to that position, each time move by direction to that pos * move speed of player
    /// </summary>
    public void MoveToPosition(Vector2 targetPos)
    {
        if (Vector2.Distance(targetPos, (Vector2) transform.position) < 0.1f)
        {
            return;
        }

        Vector2 direc = targetPos -  (Vector2) transform.position;

        direc = direc.normalized;

        transform.rotation = GetRotationByDirection(direc);

        rb.AddForce(direc * moveSpeed * moveSpeedScale * Time.fixedDeltaTime);

    }

    public void MoveByForce(Vector2 direction, float value)
    {
        transform.rotation = GetRotationByDirection(direction);

        rb.AddForce(direction * Mathf.Min(value, moveSpeed) * moveSpeedScale * Time.fixedDeltaTime);
    }

    public void MoveByAxis(Vector2 movement)
    {
        transform.rotation = GetRotationByDirection(movement);

        // rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        rb.AddForce(movement.normalized * moveSpeed * moveSpeedScale * Time.fixedDeltaTime);
    }


    public Quaternion GetRotationByDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            float angle = Vector2.SignedAngle(direction, Vector2.right);

            angle = Mathf.Round(angle / 45.0f) * 45.0f;

            return Quaternion.Euler(0, 0, -Vector2.SignedAngle(direction, Vector2.right));
        }
        else
        {
            return this.transform.rotation;
        }
    }

    private void SpeedControl()
    {
        if (rb.velocity.magnitude > moveSpeed)
        {
            rb.velocity = rb.velocity.normalized * moveSpeed;
        }
    }

    #endregion

    #region Ball controller

    public IEnumerator DribblingBall()
    {
        // TODO Change ball object logic, not stick to player -> ball will be kicked away and repeat

        PlayerController playerHasBall = GameController.Instance.PlayerHasBall;


        while (playerHasBall && playerHasBall.Equals(this))
        {
            playerHasBall = GameController.Instance.PlayerHasBall;

            Vector3 BallPos = transform.position;
            BallPos += transform.right * 1f;
            ball.transform.position = new Vector3(BallPos.x, BallPos.y, -1);
            ball.GetComponent<BallMovement>().StopForce();

            // Debug.Log((playerHasBall == null).ToString() + " " + playerHasBall.Equals(this));

            yield return null;
        }
    }

    public void ShotBall(GameObject ball)
    {
        // TODO Code to check how long the key has pressed -> convert to force

        GameController.Instance.SetPlayerHasBall(null);

        ball.GetComponent<BallMovement>().AddForce(500f, transform.right);
    }
    #endregion

    private void OnDrawGizmos()
    {
        string str = "";
        // Player velocity
        float vel = rb.velocity.magnitude;
        str += (Mathf.Floor(vel * 100) / 100).ToString() + " ";
        str += TimeToReachBall() != -1 ? TimeToReachBall() : "";

        GizmosExtra.DrawString(str, transform.position, Color.white, Color.black);
    }
}
