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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator DribblingBall()
    {
        // TODO Change ball object logic, not stick to player -> ball will be kicked away and repeat

        while (GameController.Instance.PlayerHasBall.Equals(this))
        {
            Vector3 BallPos = transform.position;
            BallPos += transform.right * 1f;
            ball.transform.position = new Vector3(BallPos.x, BallPos.y, -1);
            ball.GetComponent<BallMovement>().StopForce();

            yield return null;
        }
    }


    public void ShotBall(GameObject ball)
    {
        // TODO Code to check how long the key has pressed -> convert to force
        // TODO Call team controller (game controller) to change ball owner

         ball.GetComponent<BallMovement>().AddForce(100f, transform.right);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(BallMovement.BallTag))
        {
            // TODO Call team controller (game controller) to change ball owner
            GameController.Instance.SetPlayerHasBall(this);
        }
    }

    public void SetPosition(Vector2 targetPos)
    {
        transform.position = targetPos;
    }

    public void MoveToPosition(Vector2 targetPos)
    {

    }

    public void MoveToPositionByAxis(Vector2 movement)
    {
        if (movement != Vector2.zero)
        {
            transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(movement.normalized, Vector2.right));
        }

        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    public void SetRole(EPlayerRole role)
    {
        this.role = role;
    }
}
