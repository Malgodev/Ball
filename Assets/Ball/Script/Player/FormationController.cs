using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class FormationController : MonoBehaviour
{
    public Transform formationRectangle { get; private set; }

    public Vector2 formationPosition { get; private set; }
    public Vector2 formationScale { get; private set; }

    private void Awake()
    {
#if UNITY_EDITOR
        this.GetComponent<SpriteRenderer>().enabled = true;
#endif

        formationRectangle = transform;

        // TODO Hard code
        formationPosition = new Vector2(-23, 0);
        formationScale = new Vector2(55, 50);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        formationRectangle.position = formationPosition;
        formationRectangle.localScale = formationScale;
    }

    public void SetFormationPosition(Vector2 newPosition)
    {
        formationPosition = newPosition;
    }

    public void SetFormationScale(Vector2 newScale)
    {
        formationScale = newScale;
    }

    // offset is a percentage of formation rectangle
    public Vector2 GetWorldPositionByOffset(Vector2 offset)
    {
        Vector2 result = Vector2.zero;

        result.x = formationPosition.x + (offset.x / 100 * formationScale.x) - formationScale.x / 2;
        result.y = formationPosition.y + (offset.y / 100 * formationScale.y) - formationScale.y / 2;

        return result;
    }
}
