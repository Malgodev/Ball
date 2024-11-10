using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerState
{
    Run,
    Challenge,
    Shot,
    Pass,
    RunToBall,
    ReceiveBall,
}

public class PlayerController : MonoBehaviour
{
    // Player need time to reach maximum speed
    // and also need time to slower down

    public EPlayerRole Role { get; private set; }
    public EPlayerState PlayerState { get; private set; }

    // The position of player in formation rectangle
    public Vector2 defaultOffset { get; private set; }

    // Movement
    [SerializeField] private float moveSpeed = 10f;
    private static float moveSpeedScale = 250f;
    public float MoveableRadius;

    public Rigidbody2D rb { get; private set; }

    private GameObject ball;

    public float TimeToReachBall { get; private set; } = -1f;

    UserInput userInput;

    public Color textColor = Color.white;

    public bool IsTeamOne { get; private set; } = false;

    private bool isClosestToBall = false;

    public float DangerRate { get; private set; } = 0;
    public float DangerRadius = 10f;
    public const float DANGER_RATE_SPEED = 3f;

    const int UPDATED_FRAME_INTERVAL = 500;
    int frameCounter = 0;

    public Vector2 DeltaPosition;

    private Vector2 targetPosition;

    private int timeToPass = 60;

    private void Awake()
    {
        // IsTeamOne = transform.parent.GetComponent<TeamController>().IsTeamOne;

        rb = GetComponent<Rigidbody2D>();

        moveSpeed += UnityEngine.Random.Range(-.5f, .5f);

        // formationRectangle = GameSingleton.Instance.teamController.formationRectangle;
    }

    private void Start()
    {
        IsTeamOne = transform.parent.GetComponent<TeamController>().IsTeamOne;

        ball = GameController.Instance.ball;

        DeltaPosition = new Vector2(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5));
    }

    public void SetDefaultOffset(Vector2 DefaultOffset)
    {
        this.defaultOffset = DefaultOffset;
    }

    // Update is called once per frame
    void Update()
    {
        SpeedControl();
        
        UpdateDangerRate();

        if (frameCounter++ >= UPDATED_FRAME_INTERVAL)
        {
            DeltaPosition = new Vector2(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5));
            frameCounter = 0;
        }
    }

    private void UpdateDangerRate()
    {
        // List<GameObject> enemyPlayers = GameController.Instance.GetEnemyPlayer(IsTeamOne);

        Collider2D[] colliderNear = Physics2D.OverlapCircleAll(transform.position, DangerRadius);

        int playerInFront = 0;
        bool isGetChallenge = false;

        foreach (Collider2D collider in colliderNear)
        {
            PlayerController playerController = collider.GetComponent<PlayerController>();


            if (collider.tag == "Player" && IsEnemyInFront(playerController))
            {
                if (playerController.PlayerState == EPlayerState.Challenge)
                {
                    isGetChallenge = true;
                }

                playerInFront++;
            }
        }

        if (isGetChallenge)
        {
            DangerRate = Mathf.Lerp(DangerRate, 1, Time.deltaTime * DANGER_RATE_SPEED);
        }
        else if (!isGetChallenge && playerInFront > 0)
        {
            float targetDangerRate = Mathf.Clamp(playerInFront / 3f, 0.3f, 1f);
            DangerRate = Mathf.Lerp(DangerRate, targetDangerRate, Time.deltaTime * DANGER_RATE_SPEED * playerInFront);
        }
        else
        {
            DangerRate = Mathf.Lerp(DangerRate, -1, Time.deltaTime * DANGER_RATE_SPEED);
        }
    }

    private bool IsEnemyInFront(PlayerController targetPlayer)
    {
        if (this.IsTeamOne == targetPlayer.IsTeamOne)
        {
            return false;
        }

        if (this.gameObject == targetPlayer)
        {
            return false;
        }

        Vector2 direction = targetPlayer.transform.position - transform.position;
        float angle = Vector2.Angle(transform.right, direction);

        return angle <= 90f;
    }

    private void FixedUpdate()
    {
        if (this == GameController.Instance.teamOneController.ControlledPlayer)
        {
            return;
        }

        switch (PlayerState)
        {
            case EPlayerState.Run:
                if (Role != EPlayerRole.Goalkeeper)
                {
                    MoveToPosition(targetPosition);
                }

                break;
            case EPlayerState.Challenge:
                ChallengeBall();
                break;
            case EPlayerState.Shot:
                ShotBall(GameController.Instance.ball);
                break;
            case EPlayerState.Pass:
                PassBall(GameController.Instance.ball);
                break;
            case EPlayerState.RunToBall:
                RunToBall();
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(BallMovement.BallTag))
        {
            GameController.Instance.SetPlayerHasBall(this);
        }
    }

    private float GetTimeToReachBall()
    {
        BallMovement ballMovement = GameController.Instance.ball.GetComponent<BallMovement>();

        if (ballMovement.PredictPos.Count == 0)
        {
            float distanceToBall = Vector3.Distance(this.transform.position, ballMovement.transform.position);
            TimeToReachBall = distanceToBall / moveSpeed;
            return TimeToReachBall;
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
                TimeToReachBall = frame;
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
        this.Role = role;
    }
    #endregion

    // Put this into other player script
    #region Movement controller

    /// <summary>
    /// Try to move player to that position, each time move by direction to that pos * move speed of player
    /// </summary>
    public void MoveToPosition(Vector2 targetPos, bool isDelta = true)
    {
        if (isDelta)
        {
            targetPos += DeltaPosition;
        }

        Vector2 currentPosition = transform.position;
        Vector2 direc = targetPos - currentPosition;

        direc = direc.normalized;

        if (Vector2.Distance(currentPosition, targetPos) > MoveableRadius)
        {
            targetPos = currentPosition + direc * MoveableRadius;
        }

        if (Vector2.Distance(targetPos, currentPosition) < 0.5f)
        {
            return;
        }


        transform.rotation = GetRotationByDirection(direc);

        rb.AddForce(direc * moveSpeed * moveSpeedScale * Time.fixedDeltaTime);
    }

    public void MoveByAxis(Vector2 movement)
    {

        transform.rotation = GetRotationByDirection(movement);

        // rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        rb.AddForce(movement.normalized * moveSpeed * moveSpeedScale * Time.fixedDeltaTime);

        // move player to position, should change it to better movement.
        // attack fb, chuyen len phia tren striker
        // def fb, chan bong cua thang striker

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

    public void ChallengeBall()
    {
        PlayerController playerHasBall = GameController.Instance.PlayerHasBall;

        if (!playerHasBall)
        {
            MoveToPosition(targetPosition);
            return;
        }

        Vector2 targetPos = playerHasBall.transform.position + playerHasBall.transform.right;

        MoveToPosition(targetPos, false);
    }

    public void RunToBall()
    {
        MoveToPosition(ball.transform.position, false);
    }

    public void ShotBall(GameObject ball)
    {
        // TODO Code to check how long the key has pressed -> convert to force

        GameController.Instance.SetPlayerHasBall(null);

        Transform goal = GameController.Instance.GetGoal(IsTeamOne);


        Vector2 shootingDirection = goal.position - transform.position;
        shootingDirection.Normalize();

        transform.rotation = GetRotationByDirection(shootingDirection);

        rb.velocity = Vector2.zero;

        StopAllCoroutines();

        ball.GetComponent<BallMovement>().AddForce(50f, shootingDirection);
        GameController.Instance.SetPlayerHasBall(null);
    }


    public void PassBall(GameObject ball, GameObject targetPass = null)
    {
        if (targetPass == null)
        {
            targetPass = GetBestPlayerToPass();
        }

        Vector2 passingDirection = (targetPass.transform.position - transform.position);
        passingDirection.Normalize();

        // TODO Code to check how long the key has pressed -> convert to force

        transform.rotation = GetRotationByDirection(passingDirection);

        rb.velocity = Vector2.zero;

        StopAllCoroutines();
        GameController.Instance.SetPlayerHasBall(null);
        ball.GetComponent<BallMovement>().AddForce(50f, passingDirection);
    }

    private GameObject GetBestPlayerToPass()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 30f);

        List<GameObject> possiblePlayer = new List<GameObject>();

        GameObject bestPlayer = null;
        float minDangerRate = 1f;

        foreach (Collider2D collider in colliders)
        {
            if (collider.tag != "Player")
            {
                continue;
            }

            PlayerController playerController = collider.GetComponent<PlayerController>();

            if (IsTeamOne != playerController.IsTeamOne)
            {
                continue;
            }

            if (playerController.Role != EPlayerRole.Goalkeeper && playerController.DangerRate <= minDangerRate) 
            {
                bestPlayer = playerController.gameObject;
            }
        }


        return bestPlayer;
    }

    #endregion

    public void SetPlayerTargetPosition(Vector2 target)
    {
        targetPosition = target;
    }


    public void SetPlayerState(EPlayerState playerState)
    {
        PlayerState = playerState;
    }
/*#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string str = "";
        // Player velocity
        float vel = rb.velocity.magnitude;
        str += PlayerState + " ";
        str += (Mathf.Floor(vel * 100) / 100).ToString() + " ";
        str += (Mathf.Floor(DangerRate * 100) / 100).ToString() + " ";
        str += GetTimeToReachBall() != -1 ? Mathf.Round(GetTimeToReachBall() * 100) / 100 : " ?";

        GizmosExtra.DrawString(str, transform.position, textColor, Color.black);

        // GizmosExtra.DrawWireDisk(transform.position, DangerRadius, Color.red);
    }
#endif*/
}
