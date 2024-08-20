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
    }
    private void FixedUpdate()
    {
        if (isControlled)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    IEnumerator DribblingBall()
    {
        // TODO Change ball object logic
        GameObject ballObject = GameSingleton.Instance.ball;
        BallMovement ballMovement = ballObject.GetComponent<BallMovement>();

        ballObject.transform.position = new Vector2(transform.position.x, transform.position.y);

        ballObject.transform.position += transform.forward * 1f;

        ballMovement.StopForce();

       while (isBallOwner)
       {
            float PlayerVelocity = rb.velocity.magnitude;
            Vector2 PlayerDirection = rb.velocity.normalized;

            ballMovement.AddForce(PlayerVelocity, PlayerDirection);
            yield return null;
       }
    }


    public void SetRole(EPlayerRole role)
    {
        this.role = role;
    }


    public void SetIsControlled(bool IsBallOwner)
    {
        this.isControlled = IsBallOwner;
    }
}
