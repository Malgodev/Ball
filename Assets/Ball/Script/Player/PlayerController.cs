using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum EPlayerState
{
    Run,
    Challenge,
    Shot,
    Pass,
    RunToBall,
    ReceiveBall,
    Controlled,
}

public class PlayerController : NetworkBehaviour
{
    // TODO Player need time to reach maximum speed
    // and also need time to slower down
    private GameObject ball;

    [field: Header("Component")]
    public Rigidbody2D rb { get; private set; }

    [field: Header("Team info")]
    public bool IsTeamOne { get; private set; } = false;

    [field: Header("Player info")]
    [field: SerializeField] public EPlayerRole Role { get; private set; }
    [field: SerializeField] public EPlayerState PlayerState { get; private set; }
    [field: SerializeField] public Vector2 DefaultOffset { get; private set; } // The position of player in formation rectangle
    [SerializeField] private float moveSpeed = 10f;
    public float MoveableRadius;
    private const float MOVE_SPEED_SCALE = 250f;

    [field: Header("Ingame player field")]
    [field: SerializeField] public float TimeToReachBall { get; private set; } = -1f;
    [field: SerializeField] public float DangerRate { get; private set; } = 0;
    [SerializeField] private float DangerRadius = 10f;

    [field: Header("Player setting")]
    private int frameCounter = 0;
    private const int UPDATED_FRAME_INTERVAL = 500;
    private const float DANGER_RATE_SPEED = 3f;
    public Vector2 DeltaPosition;
    public Color textColor = Color.white;


    private Vector2 targetPosition;
    private PlayerController controlledPlayer;

    private void Awake()
    {
        IsTeamOne = transform.parent.GetComponent<TeamController>().IsTeamOne;

        rb = GetComponent<Rigidbody2D>();

        moveSpeed += UnityEngine.Random.Range(-.5f, .5f);
    }

    private void Start()
    {
        DeltaPosition = new Vector2(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5));

        // it is what it is
        DefaultOffset = transform.parent.GetComponent<TeamController>().
            formationController.GetOffsetByWorldPosition(this.transform.position);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        IsTeamOne = transform.parent.GetComponent<TeamController>().IsTeamOne;
        ball = GameController.Singleton.Ball;
    }

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
        Collider2D[] colliderNear = Physics2D.OverlapCircleAll(transform.position, DangerRadius);

        int playerInFront = 0;
        bool isGetChallenge = false;

        foreach (Collider2D collider in colliderNear)
        {
            PlayerController playerController = collider.GetComponent<PlayerController>();

            if (collider.tag == "Player" && IsEnemyInFront(playerController))
            {
                isGetChallenge = (playerController.PlayerState == EPlayerState.Challenge);
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
                ShotBall(GameController.Singleton.Ball);
                break;
            case EPlayerState.Pass:
                PassBall(GameController.Singleton.Ball);
                break;
            case EPlayerState.RunToBall:
                RunToBall();
                break;
            case EPlayerState.Controlled:
                return;
        }
    }

    private float GetTimeToReachBall()
    {
        BallMovement ballMovement = GameController.Singleton.Ball.GetComponent<BallMovement>();

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

        rb.AddForce(direc * moveSpeed * MOVE_SPEED_SCALE * Time.fixedDeltaTime);
    }

    public void MoveByAxis(Vector2 movement)
    {
        transform.rotation = GetRotationByDirection(movement);

        // rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        rb.AddForce(movement.normalized * moveSpeed * MOVE_SPEED_SCALE * Time.fixedDeltaTime);
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

    public IEnumerator DribblingBall()
    {
        // TODO Change ball object logic, not stick to player -> ball will be kicked away and repeat

        PlayerController playerHasBall = GameController.Singleton.PlayerHasBall;


        while (playerHasBall && playerHasBall.Equals(this))
        {
            playerHasBall = GameController.Singleton.PlayerHasBall;

            Vector3 BallPos = transform.position;
            BallPos += transform.right * 1f;
            ball.transform.position = new Vector3(BallPos.x, BallPos.y, -1);
            ball.GetComponent<BallMovement>().StopForce();

            yield return null;
        }
    }

    public void ChallengeBall()
    {
        PlayerController playerHasBall = GameController.Singleton.PlayerHasBall;

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

    // TODO Code to check how long the key has pressed -> convert to force
    public void ShotBall(GameObject ball)
    {
        GameController.Singleton.SetPlayerHasBall(null);

        Vector2 shootingDirection = GameController.Singleton.GetDirectionToGoal(IsTeamOne, this);

        transform.rotation = GetRotationByDirection(shootingDirection);

        rb.velocity = Vector2.zero;

        StopAllCoroutines();

        ball.GetComponent<BallMovement>().AddForce(50f, shootingDirection);
    }


    // TODO Code to check how long the key has pressed -> convert to force
    public void PassBall(GameObject ball, GameObject targetPass = null)
    {
        if (targetPass == null)
        {
            targetPass = GetBestPlayerToPass();
        }

        Vector2 passingDirection = (targetPass.transform.position - transform.position);
        passingDirection.Normalize();

        GameController.Singleton.SetPlayerHasBall(null);

        transform.rotation = GetRotationByDirection(passingDirection);

        rb.velocity = Vector2.zero;

        StopAllCoroutines();
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

    #region Setter
    public void SetDefaultOffset(Vector2 DefaultOffset)
    {
        this.DefaultOffset = DefaultOffset;
    }

    public void SetPlayerTargetPosition(Vector2 target)
    {
        targetPosition = target;
    }

    public void SetPlayerState(EPlayerState playerState)
    {
        PlayerState = playerState;
    }

    public void SetPosition(Vector2 targetPos)
    {
        transform.position = targetPos;
    }

    public void SetRole(EPlayerRole role)
    {
        this.Role = role;
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(BallMovement.BallTag))
        {
            GameController.Singleton.SetPlayerHasBall(this);
        }
    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        if (!Application.isPlaying)
//        {
//            return;
//        }

    //        string str = "";
    //        // Player velocity
    //        float vel = rb.velocity.magnitude;
    //        str += PlayerState + " ";
    //        str += (Mathf.Floor(vel * 100) / 100).ToString() + " ";
    //        str += (Mathf.Floor(DangerRate * 100) / 100).ToString() + " ";
    //        str += GetTimeToReachBall() != -1 ? Mathf.Round(GetTimeToReachBall() * 100) / 100 : " ?";

    //        Miscellaneous.GizmosExtra.DrawString(str, transform.position, textColor, Color.black);

    //        // GizmosExtra.DrawWireDisk(transform.position, DangerRadius, Color.red);
    //    }
    //#endif
}
