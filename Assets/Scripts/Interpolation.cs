using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEditor;
using UnityEngine;

public class Interpolation : MonoBehaviour
{

    public GameObject goA;
    public GameObject goB;
    public GameObject Player;
    public float Interp_time = 5.0f;

    [Range(0f, 10f)]
    public float elapasedTime;

    float t;
    private void DrawVector(Vector3 pos, Vector3 v, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawLine(pos, pos + v);

        //Arrowhead?
        Handles.color = c;

        //COmpute the "rough" endpoint for the cone
        //Normalize the vector (its magnitude becomes 1)
        Vector3 n = v.normalized; // Now the length is 35cm

        n = n * 0.35f;
        Handles.ConeHandleCap(0, pos + v - n, Quaternion.LookRotation(v), .5f, EventType.Repaint);

    }
    private void OnDrawGizmos()
    {
        DrawVector(Vector3.zero, goA.transform.position, Color.green);
        DrawVector(Vector3.zero, goB.transform.position, Color.red);

        //Draw the "parts" of vectors accourding to the interpolation
        DrawVectorParts(t);
    }
    void Start()
    {
        elapasedTime = 0.0f;
    }

    void DrawVectorParts(float t)
    {
        Vector3 partOfA = (1 - t) * goA.transform.position;
        Vector3 partOfB = t * goB.transform.position;

        //Draw the vector parts
        DrawVector(Vector3.zero, partOfA, Color.magenta);
        DrawVector(partOfA, partOfB, Color.magenta);
    }
    void Update()
    {
        //Lets get the elapsed time
        elapasedTime += Time.deltaTime;

        // Interpolate until Interp_time
        t = elapasedTime / Interp_time;

        //Clamp the t to the 1 (remember t has to be between 0 and 1)
        if (t > 1.0f)
            t = 1.0f;

        if (t < .5f)
        {
            t = 2 * t * t; //y = 2*x^2
        }
        else
        {
            t = 1 - 2 * (1 - t) * (1 - t); //y = 1 - 2*(1-x)^2
        }

        //Compute the interpolation f(t) = A*(1-t) + B*t
        Vector3 pos = (1 - t) * goA.transform.position + t * goB.transform.position;
        //Set the player position
        Player.transform.position = pos;


    }

}
