using System;
using Unity.Netcode;
using UnityEngine;

public class UserInput : NetworkBehaviour
{
    public Vector2 InputVector { get; private set; }

    public EPlayerState InputState = EPlayerState.Run;

    private void Awake()
    {
        InputVector = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkObject.IsOwner)
        {
            return;
        }

        InputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.J))
        {
            ShotBall();
        }
    }

    private void FixedUpdate()
    {
        //if (controlledPlayer == null)
        //{
        //    return;
        //}

        //controlledPlayer.MoveByAxis(InputVector);
    }

    private void ShotBall()
    {
        InputState = EPlayerState.Shot;
    }
}
