using System;
using Unity.Netcode;
using UnityEngine;

public class UserInput : NetworkBehaviour
{
    private PlayerController controlledPlayer;

    public Vector2 inputVector { get; private set; }

    private void Awake()
    {
        controlledPlayer = null;

        inputVector = Vector2.zero;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (controlledPlayer == null)
        {
            return;
        }

        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.J))
        {
            ShotBall();
        }
    }

    private void FixedUpdate()
    {
        if (controlledPlayer == null)
        {
            return;
        }

        controlledPlayer.MoveByAxis(inputVector);
    }

    public void ShotBall()
    {
        if (controlledPlayer == GameController.Singleton.PlayerHasBall)
        {
            controlledPlayer.ShotBall(GameController.Singleton.Ball);
        }
    }

    public void SetControlledPlayer(PlayerController playerController)
    {
        controlledPlayer = playerController;
    }
}
