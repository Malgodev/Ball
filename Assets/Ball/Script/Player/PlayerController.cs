using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public EPlayerRole role { get; private set; }

    // The position of player in formation rectangle
    public Vector2 defaultOffset { get; private set; }

    // Movement
    [SerializeField] private float moveSpeed = 5f;

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

    }

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

        ball.GetComponent<BallMovement>().AddForce(100f, transform.right);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(BallMovement.BallTag))
        {
            GameController.Instance.SetPlayerHasBall(this);
        }
    }

    public void SetPosition(Vector2 targetPos)
    {
        transform.position = targetPos;
    }

    /// <summary>
    /// Try to move player to that position, each time move by direction to that pos * move speed of player
    /// </summary>
    public void MoveToPosition(Vector2 targetPos)
    {
        if (targetPos.Equals((Vector2)transform.position))
        {
            return;
        }

        Vector2 direc = targetPos -  (Vector2) transform.position;

        direc = direc.normalized;

        transform.rotation = GetRotationByDirection(direc);

        rb.MovePosition(rb.position + direc * moveSpeed * Time.fixedDeltaTime);

    }

    public void MoveByForce(Vector2 direction, float value)
    {
        transform.rotation = GetRotationByDirection(direction);

        rb.MovePosition(rb.position + direction * Mathf.Min(value, moveSpeed) * Time.fixedDeltaTime);
    }

    public void MoveByAxis(Vector2 movement)
    {
        transform.rotation = GetRotationByDirection(movement);

        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
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
    public void SetRole(EPlayerRole role)
    {
        this.role = role;
    }
}
