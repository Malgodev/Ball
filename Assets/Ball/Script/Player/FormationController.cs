using Unity.Netcode;
using UnityEngine;

public class FormationController : NetworkBehaviour
{
    public bool IsTeamOne = false;

    public Transform formationRectangle { get; private set; }
    [field: SerializeField] public Vector2 formationPosition { get; private set; }
    [field: SerializeField] public Vector2 formationScale { get; private set; }

    public ETeamState curState { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        IsTeamOne = transform.parent.GetComponent<TeamController>().IsTeamOne;

        if (IsTeamOne)
        {
            formationPosition = new Vector2(formationPosition.x + 10f, formationPosition.y);
        }

        formationRectangle = GetComponent<Transform>();

        formationPosition = transform.position;
        formationScale = transform.localScale;
    }

    private void Update()
    {
        Debug.Log("Updating " + formationPosition + " " + formationScale);
        formationRectangle.position = formationPosition;
        formationRectangle.localScale = formationScale;
    }

    public void SetFormationPosition(Vector2 newPosition)
    {
        //if (!Utils.IsGameObjectInsideRect(newPosition, formationScale, GameController.Instance.Field))
        //{
        //    newPosition = Utils.ClosestPositionToRect(newPosition, formationScale, GameController.Instance.Field);
        //}
        formationPosition = newPosition;
    }

    public void SetFormationScale(Vector2 newScale)
    {
        formationScale = newScale;
    }

    public void SetFormationRectangle(Vector2 newPosition, Vector2 newScale)
    {
        formationPosition = newPosition;
        formationScale = newScale;
    }
    
    public void SetFormationRectangle(float PossessionBalance, float Compression)
    {
        // SetFormationPosition(new Vector2(PossessionBalance * (55 - formationScale.x), 0));

        //Vector2 newPos = new Vector2();

        //Debug.Log(IsTeamOne + " " + PossessionBalance);

        //if (PossessionBalance <= 0f)
        //{
        //    newPos.x = Mathf.Lerp(LowerLimit, Middle, (PossessionBalance + 1f) / 1f);

        //}
        //else
        //{
        //    newPos.x = Mathf.Lerp(Middle, UpperLimit, PossessionBalance);
        //}

        //newPos.x = newPos.x + (IsTeamOne ? -1 : 1) * formationScale.x;

        //SetFormationPosition(newPos);

        Vector2 targetPos = new Vector2();
    }

    // offset is a percentage of formation rectangle
    public Vector2 GetWorldPositionByOffset(Vector2 offset)
    {
        Vector2 result = Vector2.zero;

        int delta = IsTeamOne ? 1 : -1;

        result.x = formationPosition.x + (offset.x / 100 * formationScale.x - formationScale.x / 2) * delta;
        result.y = formationPosition.y + (offset.y / 100 * formationScale.y - formationScale.y / 2);

        return result;
    }

    public Vector2 GetOffsetByWorldPosition(Vector2 worldPosition)
    {
        Vector2 offset = Vector2.zero;

        int delta = IsTeamOne ? 1 : -1;

        offset.x = ((worldPosition.x - formationPosition.x) / delta + formationScale.x / 2) * 100 / formationScale.x;
        offset.y = ((worldPosition.y - formationPosition.y) + formationScale.y / 2) * 100 / formationScale.y;

        return offset;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Color color = IsTeamOne ? Color.blue : Color.red;

        Miscellaneous.GizmosExtra.DrawWireRectangle(formationPosition, formationScale.x, formationScale.y, color);
    }
#endif
}
