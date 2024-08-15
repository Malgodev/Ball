using System.Collections;
using System.Collections.Generic;
using TeamController;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public EPlayerRole role { get; private set; }

    public Vector2 DefaultOffset { get; private set; } // offset from the center of the formation

    private FormationRectangle formationRectangle;

    private void Awake()
    {
        formationRectangle = GameSingleton.Instance.teamController.formationRectangle;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRole(EPlayerRole role)
    {
        this.role = role;
    }

    public void SetDefaultOffset(Vector2 DefaultOffset)
    {
        this.DefaultOffset = DefaultOffset;

        if (role == EPlayerRole.Goalkeeper)
        {
            transform.position = new Vector3(-50, 0, 0);
        }
        else
        {
            Vector3 defaultPos = formationRectangle.GetWorldPositionByOffset(this.DefaultOffset);

            transform.position = defaultPos;
        }

        // TO DO: Set the player position more dynamic
        // E.g: Defender will strict together more than midfielder
    }
}
