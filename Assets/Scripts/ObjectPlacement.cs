using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField] private float handleLength;
    [SerializeField] private float handleThickness;
    [SerializeField] private GameObject myObject;
    private void OnDrawGizmos()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Handles.color = Color.magenta;
            Handles.DrawLine(transform.position, hit.point, handleThickness);

            Handles.color = Color.green;
            Handles.DrawLine(hit.point, hit.point + handleLength * hit.normal, handleThickness);

            Vector3 right = Vector3.Cross(hit.normal, transform.forward);
            Handles.color = Color.red;
            Handles.DrawLine(hit.point, hit.point + handleLength * right, handleThickness);

            Vector3 forward = Vector3.Cross(hit.normal, right);
            Handles.color = Color.blue;
            Handles.DrawLine(hit.point, hit.point + handleLength * forward, handleThickness);

            if (myObject)
            {
                myObject.transform.position = hit.point;
                Quaternion rot = Quaternion.LookRotation(forward, hit.normal);
                myObject.transform.rotation = rot;
            }

        }
    }
}
