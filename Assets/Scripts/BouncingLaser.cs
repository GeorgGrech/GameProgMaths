using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BouncingLaser : MonoBehaviour
{

    [Range(1,100)]
    [SerializeField] private int bounces;
    [SerializeField] private float normalScaler;

    private void OnDrawGizmos()
    {
        RaycastHit hit;

        Vector3 rayDirection = transform.right;
        Vector3 rayStart = transform.position;


        for (int i = 0; i <= bounces; i++)
        {

            if(Physics.Raycast(rayStart,rayDirection,out hit))
            {
                Vector3 normal;

                Handles.color = Color.red;
                Handles.DrawLine(rayStart, hit.point);
                normal = hit.normal;

                Handles.color = Color.green;
                Handles.DrawLine(hit.point, hit.point+normal*normalScaler);


                rayDirection = rayDirection - 2 * (Vector3.Dot(rayDirection, normal) * normal);
                rayStart = hit.point;

            }
        }
    }
}
