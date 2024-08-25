using System;
using System.Collections;
using System.Collections.Generic;
using TeamController;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public EPlayerRole role { get; private set; }

    public Vector2 defaultOffset { get; private set; } // offset from the center of the formation

    private FormationRectangle formationRectangle;

    [SerializeField] private bool isControlled = false;
    [SerializeField] private bool isBallOwner = false;


/*     IEnumerator DribblingBall;*/

    // Movement
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;

    GameObject ballObject = null;

    Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        formationRectangle = GameSingleton.Instance.teamController.formationRectangle;
    }

    public void SetDefaultOffset(Vector2 DefaultOffset)
    {
        this.defaultOffset = DefaultOffset;

        if (role == EPlayerRole.Goalkeeper)
        {
            transform.position = new Vector3(-50, 0, 0);
        }
        else
        {
            Vector3 defaultPos = formationRectangle.GetWorldPositionByOffset(this.defaultOffset);

            transform.position = defaultPos;
        }

        // TO DO: Set the player position more dynamic
        // E.g: Defender will strict together more than midfielder
    }

    void Start()
    {
        ballObject = GameSingleton.Instance.ball;

        StartCoroutine(DribblingBall());
    }

    // Update is called once per frame
    void Update()
    {
        if (isControlled)
        {
            UserControl();
        }
        else
        {

        }
    }

    private void UserControl()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.J)) 
        {
            ShotBall();
        }
    }
    private void FixedUpdate()
    {
        if (isControlled)
        {

            if (movement != Vector2.zero)
            {
                transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(movement.normalized, Vector2.right));
            }

            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    IEnumerator DribblingBall()
    {
        // TODO Change ball object logic
        BallMovement ballMovement = ballObject.GetComponent<BallMovement>();

        while (isBallOwner)
        {
            Vector3 BallPos = transform.position;
            BallPos += transform.right * 1f;
            ballObject.transform.position = new Vector2(BallPos.x, BallPos.y);
            ballMovement.StopForce();
            yield return null;
        }
    }

    public void ShotBall()
    {
        // TODO Code to check how long the key has pressed -> convert to force

        isBallOwner = false;

        ballObject.GetComponent<BallMovement>().AddForce(100f, transform.right);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(BallMovement.BallTag))
        {
            isBallOwner = true;
            StartCoroutine(DribblingBall());
        }
    }

    public void SetRole(EPlayerRole role)
    {
        this.role = role;
    }

    public void SetIsControlled(bool isControlled)
    {
        this.isControlled = isControlled;
    }

    public void SetIsBallOwner(bool isBallOwner)
    {
        this.isBallOwner = isBallOwner;
    }
}
