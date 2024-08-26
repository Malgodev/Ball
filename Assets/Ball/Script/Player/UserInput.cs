using System;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    public Vector2 inputVector { get; private set; }

    public event Action OnShotBall;

    private void Awake()
    {
        inputVector = Vector2.zero;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.J))
        {
            ShotBall();
        }
    }

    public void ShotBall()
    {
        OnShotBall?.Invoke();
        // TODO Code to check how long the key has pressed -> convert to force

        /*        isBallOwner = false;

                ballObject.GetComponent<BallMovement>().AddForce(100f, transform.right);*/
    }
}
