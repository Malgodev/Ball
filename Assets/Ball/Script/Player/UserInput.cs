using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UserInput : NetworkBehaviour
{
    public Vector2 InputVector { get; private set; }

    public EPlayerState InputState = EPlayerState.Run;

    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private Button shotBtn;
    [SerializeField] private Button passBtn;

    private void Awake()
    {
        InputVector = Vector2.zero;
    }

    private void Start()
    {
        joystick = GameController.Instance.joystick;
        passBtn = GameController.Instance.passBtn;
        shotBtn = GameController.Instance.shotBtn;

        shotBtn.onClick.AddListener(ShotBall);

        passBtn.onClick.AddListener(ShotBall);
    }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkObject.IsOwner)
        {
            return;
        }

        InputVector = joystick.Direction;

        if (InputVector == Vector2.zero)
        {
            InputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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
}
