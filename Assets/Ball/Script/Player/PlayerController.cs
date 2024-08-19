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

    [SerializeField] private bool isBallOwner = false;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;

    Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        formationRectangle = GameSingleton.Instance.teamController.formationRectangle;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isBallOwner)
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
        if (isBallOwner)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
            Debug.Log(movement);
        }
    }

    public void SetRole(EPlayerRole role)
    {
        this.role = role;
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

    public void SetIsBallOwner(bool IsBallOwner)
    {
        this.isBallOwner = IsBallOwner;
    }
}
