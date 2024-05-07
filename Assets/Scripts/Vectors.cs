using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Vectors : MonoBehaviour
{
    public GameObject my_object;
    private void DrawVector(Vector3 pos, Vector3 v, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawLine(pos, pos+v);
        //Handles.DrawLine(pos, pos+v);
        Handles.color = c;
        //Normalise the Vector (its magnitude becomes 1)
        Vector3 n = v.normalized;
        n = n * .35f;//Now the length is 35cm 
        Handles.ConeHandleCap(0, pos + v - n, Quaternion.LookRotation(v), .5f, EventType.Repaint);
    }

    private void OnDrawGizmos()
    {
        //X-axis
        DrawVector(Vector3.zero, new Vector3(5,0,0), Color.red);
        //Y-axis
        DrawVector(Vector3.zero, new Vector3(0,5,0), Color.green);

        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(Vector3.zero, new Vector3(0,5,0));

        //Vector
        DrawVector(new Vector3(3, 3, 0), new Vector3(4, 3, 0), Color.magenta);

        //Unit circle
    }
}
