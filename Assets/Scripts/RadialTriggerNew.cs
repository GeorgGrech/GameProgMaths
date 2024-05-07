using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class RadialTriggerNew : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private Transform player;
    [SerializeField] private Transform lookAt;
    //[SerializeField] private float lookValue;
    [SerializeField] private float lookAngle;
    [SerializeField] private float height;

    [SerializeField] GameObject[] lights;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }



    private void DrawVector(Vector3 pos, Vector3 v, Color c, float thickness = 0.0f)
    {
        //Gizmos.color = c;
        //Gizmos.DrawLine(pos, pos + v);
        // Arrow head?
        Handles.color = c;
        Handles.DrawLine(pos, pos + v, thickness);

        // Compute the "rough" endpoint for the cone
        // Normalize the vector (its magnitude becomes 1)
        Vector3 n = v.normalized;
        n = n * 0.35f; // Now the length is 35cm

        Handles.ConeHandleCap(0, pos + v - n, Quaternion.LookRotation(v), 0.5f, EventType.Repaint);

    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            if (Vector3.Distance(transform.position, player.position) <= radius)
            {
                if(WithinLookAngle())
                {
                    EnableLights(true);
                }
                else
                {
                    EnableLights(false);
                }
            }
            else
            {
                EnableLights(false);
            }
        }
    }

    void EnableLights(bool enable)
    {
        foreach (GameObject light in lights)
        {
            light.SetActive(enable);
        }
    }

    private void OnDrawGizmos()
    {
        if (player)
        {
            //DrawVector(Vector3.zero, player.position, Color.white, 1.5f);
            //DrawVector(Vector3.zero, transform.position, Color.white, 1.5f);
        
            if(Vector3.Distance(transform.position, player.position) > radius)
            {
                DrawVector(transform.position, player.position - transform.position, Color.green, .1f);
                Handles.color = Color.green;
            }
            else
            {
                DrawVector(transform.position, player.position - transform.position, Color.red, .1f);
                Handles.color= Color.red;
            }

            Handles.DrawWireDisc(transform.position, new Vector3(0, 1, 0), radius);

            DrawVector(transform.position, lookAt.position - transform.position, Color.magenta, .1f);

            if (WithinLookAngle())
            {
                DrawVector(transform.position, lookAt.position - transform.position, Color.red, .1f);
            }
            else
            {
                DrawVector(transform.position, lookAt.position - transform.position, Color.blue, .1f);
            }

        }

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position - Vector3.up * height / 2f, Vector3.up, radius, 2f);
        Handles.DrawWireDisc(transform.position + Vector3.up * height / 2f, Vector3.up, radius, 2f);
            
        Quaternion q_rot = Quaternion.Euler(0f,lookAngle,0f);
        Vector3 rotated = q_rot * GetDirection(lookAt);
        DrawVector(transform.position,rotated*radius,Color.magenta, 2f);
        q_rot = Quaternion.Euler(0f,-lookAngle,0f);
        Vector3 rotated_too = q_rot* GetDirection(lookAt) ;
        DrawVector(transform.position, rotated_too * radius, Color.magenta, 2f);
            

    }

    private Vector3 GetDirection(Transform lookObject)
    {
        Vector3 dir = lookObject.position - transform.position;
        Vector3 normalisedDir = dir.normalized;

        return normalisedDir;
    }

    private bool WithinLookAngle()
    {
        Vector3 l = GetDirection(lookAt);

        Vector3 n = GetDirection(player);

        //float dotProduct = Vector3.Dot(l, n);
        float dotProduct = Mathf.Cos(Mathf.Deg2Rad * lookAngle);

        return Vector3.Dot(l, n) >= dotProduct;
    }
}
