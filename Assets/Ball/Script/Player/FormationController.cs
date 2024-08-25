using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class FormationController : MonoBehaviour
{
    [field: SerializeField] public Transform formationRectangle { get; private set; }

    public Vector2 formationPosition { get; private set; }
    public Vector2 formationScale { get; private set; }

    private void Awake()
    {
#if UNITY_EDITOR
        this.GetComponent<SpriteRenderer>().enabled = true;
#endif
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

    /*    // Should rewrite to make this class base on GameObject rather than float value

        public GameObject square { get; private set; }

        // center X, Y allow team to move for defense and attack option
        public float centerX { get; private set; }
        public float centerY { get; private set; }

        // width, height allow team to compress into a smaller area for more aggresive play and vice versa
        public float width { get; private set; }
        public float height { get; private set; }

        // ratio between w,h and square scale
        private float ratio;

        public FormationController(GameObject square, float centerX, float centerY, float width, float height)
        {
            this.square = square;
            this.centerX = centerX;
            this.centerY = centerY;
            this.width = width;
            this.height = height;

            ratio = square.transform.localScale.x / width;
        }

        public Vector3 GetWorldPositionByOffset(Vector3 offset)
        {
            Vector2 result = Vector2.zero;

            result.x = centerX + (offset.x / 100 * width * ratio) - width * ratio / 2;
            result.y = centerY + (offset.y / 100 * height * ratio) - height * ratio / 2;

            return result;
        }*/
}
