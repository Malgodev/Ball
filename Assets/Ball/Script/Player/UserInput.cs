using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UserInput : NetworkBehaviour
{
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private Button shotBtn;
    [SerializeField] private Button passBtn;
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
        if (InputVector == Vector2.zero)
        {
            InputVector = new Vector2(joystick.Horizontal, joystick.Vertical);
        }


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

    private void PassBall()
    {
        InputState = EPlayerState.Pass;
    }

    public void SetUserInput()
    {
        joystick = MainGameUI.Instance.Joystick;
        shotBtn = MainGameUI.Instance.ShotButton;
        passBtn = MainGameUI.Instance.PassButton;

        shotBtn.onClick.AddListener(() =>
        {
            ShotBall();
        });

        passBtn.onClick.AddListener(() =>
        {
            PassBall();
        });
        // GameObject.FindGameObjectWithTag("UserControl").GetComponent<FixedJoystick>();
    }
}
