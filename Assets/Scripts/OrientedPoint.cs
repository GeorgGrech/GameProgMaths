using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OrientedPoint
{
     public Vector3 Pos;
     public Quaternion Rot;

     public OrientedPoint(Vector3 pos, Quaternion rot)
     {
          Pos = pos;
          Rot = rot;
     }
     public OrientedPoint(Vector3 pos, Vector3 forward)
     {
          Pos = pos;
          Rot = Quaternion.LookRotation(forward);
     }

     public Vector3 LocalToWorldPosition(Vector3 localSpacePos)
     {
          return Pos + Rot * localSpacePos;
     }
     
     public Vector3 LocalToWorldVector(Vector3 localSpacePos)
     {
          return Rot * localSpacePos;
     }
     
}

