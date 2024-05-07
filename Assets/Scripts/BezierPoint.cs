using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BezierPoint : MonoBehaviour
{
    //[SerializeField] Transform anchor;
    [SerializeField] Transform[] controls = new Transform[2];

    public bool NeedToUpdateMesh = false;

    public Vector3 getAnchorPoint() => transform.position;
    public Vector3 getFirstControlPoint() => controls[0].position;
    public Vector3 getSecondControlPoint() => controls[1].position;

    private void OnDrawGizmos()
    {
        CorrectSecondControlPoint();
        Gizmos.color = Color.white;
        Gizmos.DrawLine(getFirstControlPoint(), getAnchorPoint());
        Gizmos.DrawLine(getAnchorPoint(),getSecondControlPoint());
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(getFirstControlPoint(), .2f * HandleUtility.GetHandleSize(getFirstControlPoint()));
        Gizmos.DrawSphere(getSecondControlPoint(), .2f * HandleUtility.GetHandleSize(getSecondControlPoint()));
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(getAnchorPoint(), .2f * HandleUtility.GetHandleSize(getAnchorPoint()));
    }

    //Allows for perfectly smooth bends. The issue is that only controlPoint1 can be moved manually.
    private void CorrectSecondControlPoint()
    {
        controls[1].localPosition = -controls[0].localPosition;
    }
}
