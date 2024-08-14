using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Something : MonoBehaviour
{
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        // Calculate the elapsed time since start
        float elapsedTime = Time.time - startTime;

        // Check if 5 seconds have passed
        if (elapsedTime >= 5f)
        {
            startTime = Time.time; // Reset the start time if needed

            Destroy(this.gameObject);
        }
    }
}
