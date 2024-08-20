using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FowardVector : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
    }
}
