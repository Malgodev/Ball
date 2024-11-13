using System;
using Unity.Netcode;
using UnityEngine;

public enum ETeamHasBall
{
    TeamOne,
    TeamTwo,
    None
}

public enum ETeamState
{
    Attacking, // Nhỏ lại về sân bạn
    Defending, // Nhỏ lại về sân nhà
    TransitionDefending, // Hình chữ nhật to ra
    Countering,
    Neutral
}

public class FormationAI : NetworkBehaviour
{
    bool isHasBall = false;
    public bool IsTeamOne = false;

    const int UPDATED_FRAME_INTERVAL = 5;
    int frameCounter = 0;

    private FormationController formationController;
    private ETeamState teamState;

    [Header("Formation Settings")]
    public float FormationCompression = 0f;                   // How much team compresses when defending
    public float NorLengthLimit = 55f;
    public float NorWidthLimit = 50f;
    public float MinLengthLimit = 20f;
    public float MaxLengthLimit = 100f;
    public float MinWidthLimit = 30f;
    public float MaxWidthLimit = 70f;
    public const float COMPRESSION_SMOOTHING_SPEED = 5f;


    [Header("AI Setting")]
    public float DangerRate;

    [Header("Team Behavior")]
    [Range(-1f, 1f)] public float PossessionBalance = 0f;     // 1 = full possession, -1 = opponent has ball
    public const float POSSESSION_SMOOTHING_SPEED = 5f;
    private float smoothedPossessionBalance = 0f;

    private Transform ball;

    float LowerLimit = -55f;
    float Middle = 0f;
    float UpperLimit = 55f;

    float TopLimit = 35f;
    float BottomLimit = -35f;

    float delta = 1f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        formationController = GetComponent<FormationController>();
        ball = GameController.Instance.Ball.transform;
    }

    // TODO make this sync on every client
    public void InitFormationAI(bool isTeamOne)
    {
        IsTeamOne = isTeamOne;
        if (!IsTeamOne)
        {
            LowerLimit = -LowerLimit;
            Middle = -Middle;
            UpperLimit = -UpperLimit;
            delta = -delta;
        }
    }

    private void Update()
    {
        return;

        if (frameCounter++ >= UPDATED_FRAME_INTERVAL)
        {
            UpdatePossessionBalance();
            UpdateCompressionBalance();
            frameCounter = 0;
        }
    }

    private void FixedUpdate()
    {
        return;

        formationController.SetFormationPosition(ConvertPossessionToPosition(smoothedPossessionBalance));
        formationController.SetFormationScale(ConvertStateToScale(teamState));

        // ConvertStateToScale(teamState);
    }
    private void UpdateCompressionBalance()
    {
        ETeamHasBall teamHasBall = GameController.Instance.GetTeamHasBall();

        bool havingBall = (teamHasBall == ETeamHasBall.TeamOne && IsTeamOne) || 
                (teamHasBall == ETeamHasBall.TeamTwo && !IsTeamOne);

        if (havingBall && smoothedPossessionBalance <= 0)
        {
            teamState = ETeamState.Countering;
        }
        else if (havingBall && smoothedPossessionBalance > 0)
        {
            teamState = ETeamState.Attacking;
        }
        else if (!havingBall && smoothedPossessionBalance <= 0)
        {
            teamState = ETeamState.Defending;
        }
        else if (!havingBall && smoothedPossessionBalance > 0)
        {
            teamState = ETeamState.TransitionDefending;
        }
    }

    private Vector2 ConvertStateToScale(ETeamState teamState)
    {
        Vector2 scale = formationController.formationScale;

        switch (teamState)
        {
            case ETeamState.Attacking:
                scale.x = Mathf.Lerp(MinLengthLimit, NorLengthLimit, Time.deltaTime * COMPRESSION_SMOOTHING_SPEED);
                scale.y = Mathf.Lerp(NorWidthLimit, NorWidthLimit, Time.deltaTime * COMPRESSION_SMOOTHING_SPEED);
                break;
            case ETeamState.Defending:
                scale.x = Mathf.Lerp(MinLengthLimit, NorLengthLimit, Time.deltaTime * COMPRESSION_SMOOTHING_SPEED);
                scale.y = Mathf.Lerp(NorWidthLimit, MinWidthLimit, Time.deltaTime * COMPRESSION_SMOOTHING_SPEED);
                break;
            case ETeamState.TransitionDefending:
                scale.x = Mathf.Lerp(NorLengthLimit, MaxLengthLimit, Time.deltaTime * COMPRESSION_SMOOTHING_SPEED);
                scale.y = Mathf.Lerp(NorWidthLimit, MinWidthLimit, Time.deltaTime * COMPRESSION_SMOOTHING_SPEED);
                break;
            case ETeamState.Countering:
                scale.x = Mathf.Lerp(NorLengthLimit, MaxLengthLimit, Time.deltaTime * COMPRESSION_SMOOTHING_SPEED);
                scale.y = Mathf.Lerp(NorWidthLimit, NorWidthLimit, Time.deltaTime * COMPRESSION_SMOOTHING_SPEED);
                break;
        }

        return scale;
    }

    private Vector2 ConvertPossessionToPosition(float smoothedPossessionBalance)
    {
        Vector2 scale = formationController.formationScale;
        Vector2 target = new Vector2();

        float deltaX = (IsTeamOne ? scale.x : -scale.x) / 2;
        float deltaY = scale.y / 2;

        if (smoothedPossessionBalance <= 0f)
        {
            target.x = Mathf.Lerp(LowerLimit + deltaX, Middle, (smoothedPossessionBalance + 1f) / 1f);
        }
        else
        {
            target.x = Mathf.Lerp(Middle, UpperLimit - deltaX, smoothedPossessionBalance);
        }

        Transform ballTransform = GameController.Instance.Ball.transform;

        target.y = ballTransform.position.y;

        target.y = Mathf.Clamp(target.y, BottomLimit + deltaY, TopLimit - deltaY);

        return target;
    }

    private void UpdatePossessionBalance()
    {
        // TODO Put this into switch case of ETeamState

        ETeamHasBall teamHasBall = GameController.Instance.GetTeamHasBall();

        if (teamHasBall == ETeamHasBall.None)
        {
            float fieldPositionFactor = Mathf.InverseLerp(LowerLimit, UpperLimit, ball.position.x) * 2 - 1f;
            PossessionBalance = Mathf.Lerp(PossessionBalance, fieldPositionFactor, Time.deltaTime * POSSESSION_SMOOTHING_SPEED);

        }
        else if ((teamHasBall == ETeamHasBall.TeamOne && IsTeamOne) 
                || (teamHasBall == ETeamHasBall.TeamTwo && !IsTeamOne))
        {
            PossessionBalance = Mathf.Lerp(PossessionBalance, 1f, Time.deltaTime * POSSESSION_SMOOTHING_SPEED);
        }
        else
        {
            PossessionBalance = Mathf.Lerp(PossessionBalance, -1f, Time.deltaTime * POSSESSION_SMOOTHING_SPEED);
        }

        smoothedPossessionBalance = Mathf.Lerp(
            smoothedPossessionBalance,
            PossessionBalance,
            Time.deltaTime * POSSESSION_SMOOTHING_SPEED
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string str = smoothedPossessionBalance.ToString();
        Vector2 location = new Vector2(55 * (IsTeamOne ? -1 : 1), 55);

        Miscellaneous.GizmosExtra.DrawString(str, location, Color.green, Color.black);
    }
#endif
}
