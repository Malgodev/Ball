using Unity.Netcode;
using UnityEngine;

public class FormationController : NetworkBehaviour
{
    public bool IsTeamOne = false;

    public Transform formationRectangle { get; private set; }
    [field: SerializeField] public Vector2 formationPosition { get; private set; }
    [field: SerializeField] public Vector2 formationScale { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        formationRectangle = this.transform;
    }

    [ClientRpc]
    public void InitFormationControllerClientRpc(bool isTeamOne)
    {
        IsTeamOne = isTeamOne;

        if (isTeamOne)
        {
            formationPosition = new Vector3(-23, 5, -1);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            formationScale = new Vector3(55, 50, 0);
        }
        else
        {
            formationPosition = new Vector3(23, 5, -1);
            transform.rotation = Quaternion.Euler(0, 0, 180);
            formationScale = new Vector3(55, 50, 0);
        }
    }

    private void Update()
    {
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

    public void SetFormationRectTransform(Vector2 pos, Quaternion rot, Vector2 scale)
    {
        formationRectangle.position = pos;
        formationPosition = pos;
        formationRectangle.rotation = rot;
        formationScale = scale;
        formationRectangle.localScale = scale;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Color color = IsTeamOne ? Color.blue : Color.red;

        Miscellaneous.GizmosExtra.DrawWireRectangle(formationPosition, formationScale.x, formationScale.y, color);
    }
#endif
}
