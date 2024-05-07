using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;


[RequireComponent(typeof(MeshFilter))]
public class BezierCurve : MonoBehaviour
{
    [SerializeField] private List<Transform> controlPoints = new List<Transform>();
    [Range(0, 1)] [SerializeField] private float tValue = 0;
    [SerializeField] private Mesh2D shape2D;
    [Range(2, 32)] [SerializeField] private int segmentCount;
    Vector3 GetPos(int i) => controlPoints[i].position; // helper function

    private Mesh _mesh;

    private void Awake()
    {
        _mesh = new Mesh();
        _mesh.name = "Segment";
        
        
        GetComponent<MeshFilter>().sharedMesh = _mesh;
    }

    private void Update() => GenerateMesh();

    void GenerateMesh()
    {
        _mesh.Clear();
        
        // Vertices
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        for (int i = 0; i < segmentCount ; i++)
        {
            float t = i / (segmentCount - 1f);
            OrientedPoint fp = GetBezierForward(t);
            for (int j = 0; j < shape2D.VertexCount; j++)
            {
                vertices.Add(fp.LocalToWorldPosition(shape2D.vertices[j].point));
                normals.Add(fp.LocalToWorldVector(shape2D.vertices[j].normal));
            }
        }
        
        // Triangles
        List<int> triangles = new List<int>();
        for (int i = 0; i < segmentCount - 1; i++)
        {
            int rootIndex = i * shape2D.VertexCount;
            int nextIndex = (i + 1) * shape2D.VertexCount;
            
            // Lines
            for (int j = 0; j < shape2D.LineCount; j += 2)
            {
                int lineIndexA = shape2D.lineIndices[j];
                int lineIndexB = shape2D.lineIndices[j + 1];

                int currentA = rootIndex + lineIndexA;
                int currentB = rootIndex + lineIndexB;
                int nextA = nextIndex + lineIndexA;
                int nextB = nextIndex + lineIndexB;

                triangles.Add(currentA);
                triangles.Add(nextA);
                triangles.Add(nextB);

                triangles.Add(currentA);
                triangles.Add(nextB);
                triangles.Add(currentB);
            }        
        }
        
        _mesh.SetVertices(vertices);
        _mesh.SetNormals(normals);
        _mesh.SetTriangles(triangles, 0);

    }

    public void OnDrawGizmos()
    {
        foreach (var controlPoint in controlPoints)
            Gizmos.DrawSphere(controlPoint.position, 0.05f);
        
        Handles.DrawBezier(
            GetPos(0), GetPos(3), 
            GetPos(1), GetPos(2),
            Color.green, EditorGUIUtility.whiteTexture, 1f);

        OrientedPoint bezierPoint = GetBezierForward(tValue);
        
        Gizmos.DrawSphere(bezierPoint.Pos, 0.05f);
        Handles.PositionHandle(bezierPoint.Pos, bezierPoint.Rot);

        void DrawPoint(Vector2 localPos) => Gizmos.DrawSphere(bezierPoint.LocalToWorldPosition(localPos), 0.1f);

        Vector3[] vertices = shape2D.vertices.Select(v => bezierPoint.LocalToWorldPosition(v.point)).ToArray();
        
        // Draw road segment
        for (var i = 0; i < shape2D.lineIndices.Length; i += 2)
        {
            Vector3 a = vertices[shape2D.lineIndices[i]];
            Vector3 b = vertices[shape2D.lineIndices[i + 1]];
            
            Gizmos.DrawLine(a, b);

            DrawPoint(shape2D.vertices[i].point);
        }
    }
    OrientedPoint GetBezierForward(float t)
    {
        Vector3 p0 = GetPos(0);
        Vector3 p1 = GetPos(1);
        Vector3 p2 = GetPos(2);
        Vector3 p3 = GetPos(3);

        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);
        
        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        Vector3 pos = Vector3.Lerp(d, e, t);
        Vector3 tangent = (e-d).normalized;

        return new OrientedPoint(pos, tangent);

        for (int i = 0; i<controlPoints.Count; i++)
        {

        }
    }
}

