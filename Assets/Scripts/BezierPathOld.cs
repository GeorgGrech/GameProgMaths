using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BezierPathOld : MonoBehaviour
{

    [SerializeField] private GameObject car;

    [SerializeField]
    public Mesh2D Roadshape;
    [Range(0.1f, 100f)]
    public float RoadScaler = 1.0f;


    public bool DrawSegments = false;
    public bool DrawBezier = false;

    public List<BezierPoint> points = new List<BezierPoint>();

    public bool ClosedPath = false;
    public bool closedPathChecker = false;

    //[Range(100, 1000)]
    //public int Segments = 500;

    ///[Range(0, 1000)]
    //public int CurrentSegment = 0;

    [Range(0f, 1f)]
    public float TSimulate = 0.0f;

    [Range(3, 255)]
    public int Slices = 32;

    public GameObject MyObject;


    public Mesh mesh;

    public Texture roadTexture;

    private GameObject spawnedCar;

    OrientedPoint getBezierOrientedPoint(float t, Vector3 first_a, Vector3 first_c,
                                    Vector3 second_c, Vector3 second_a)
    {
        OrientedPoint op;

        Vector3 a = Vector3.Lerp(first_a, first_c, t);
        Vector3 b = Vector3.Lerp(first_c, second_c, t);
        Vector3 c = Vector3.Lerp(second_c, second_a, t);

        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        Vector3 bez = Vector3.Lerp(d, e, t);

        op.Pos = bez;

        Quaternion rotation = Quaternion.LookRotation(e - d);
        op.Rot = rotation;

        return op;
    }

    private void OnDrawGizmos()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        else
        {
            mesh.Clear();
        }
        if (!spawnedCar)
        {
            spawnedCar = GameObject.Instantiate(car, points[0].getAnchorPoint(), Quaternion.identity);
        }

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();


        // How many BezierPoints there are?
        int n = points.Count;
        int nSegments = n - 1;

        // Loop from first item in the array until 2nd last item
        // Because we are using index i+1 inside the loop
        for (int i = 0; i < n - 1; i++)
        {
            // I need 2 bezier points: i & i+1
            Vector3 first_anchor = points[i].getAnchorPoint();
            Vector3 second_anchor = points[i + 1].getAnchorPoint();

            Vector3 first_control = points[i].getSecondControlPoint();
            Vector3 second_control = points[i + 1].getFirstControlPoint();

            if (DrawBezier)
                Handles.DrawBezier(first_anchor, second_anchor, first_control, second_control,
                Color.green, null, 3f);
        }
        if (ClosedPath)
        {
            
            // The first bezier points is at: n-1 (???)
            // The second point is at index: 0
            Vector3 first_anchor = points.Last().getAnchorPoint();
            Vector3 second_anchor = points.First().getAnchorPoint();

            Vector3 first_control = points.Last().getSecondControlPoint();
            Vector3 second_control = points.First().getFirstControlPoint();

            if (!closedPathChecker)
            {
                //points.Add(points.Last());
                points.Add(points.First());
                closedPathChecker = true;    
            }

            Handles.DrawBezier(first_anchor, second_anchor, first_control, second_control,
                Color.green, null, 3f);
        }

        // Loop through the slices
        for (int slice = 0; slice <= Slices; slice++)
        {

            float TSlice = (float)slice / Slices;  // 0.0f ... 1.0f
            float TSliceNext = (float)(slice + 1) / Slices;  // 0.0f ... 1.0f

            int seg_start = Mathf.FloorToInt(TSlice * nSegments);
            int seg_start_next = Mathf.FloorToInt(TSliceNext * nSegments);
            if (seg_start >= nSegments)
            {
                seg_start = nSegments - 1;
            }
            if (seg_start_next >= nSegments)
            {
                seg_start_next = nSegments - 1;
            }


            Vector3 first_a = points[seg_start].getAnchorPoint();
            Vector3 second_a = points[seg_start + 1].getAnchorPoint();
            Vector3 first_c = points[seg_start].getSecondControlPoint();
            Vector3 second_c = points[seg_start + 1].getFirstControlPoint();

            Vector3 first_a_next = points[seg_start_next].getAnchorPoint();
            Vector3 second_a_next = points[seg_start_next + 1].getAnchorPoint();
            Vector3 first_c_next = points[seg_start_next].getSecondControlPoint();
            Vector3 second_c_next = points[seg_start_next + 1].getFirstControlPoint();


            float TActual = (float)nSegments * TSlice - seg_start * 1.0f;
            float TActualNext = (float)nSegments * TSliceNext - seg_start_next * 1.0f;

            //float TActual = (float) (nSegments) * (TSimulate - seg_start*1.0f / (float)(nSegments));
            // The 1st version:
            //float TActual = (TSimulate - seg_start*1.0f / (float)(nSegments)) / (1.0f / (float)(nSegments));

            OrientedPoint op = getBezierOrientedPoint(TActual, first_a, first_c, second_c, second_a);
            OrientedPoint op_next = getBezierOrientedPoint(TActualNext, first_a_next, first_c_next, second_c_next, second_a_next);

            //Gizmos.color = Color.red;
            for (int i = 0; i < Roadshape.vertices.Length; i++)
            {
                int j = i + 2;
                j = j % Roadshape.vertices.Length;
                Vector3 roadpoint = Roadshape.vertices[i].point;
                Vector3 roadpoint_next = Roadshape.vertices[j].point;

                Vector3 transformed_point = op.LocalToWorldPosition(roadpoint * RoadScaler);
                vertices.Add(transformed_point);
                normals.Add(op.LocalToWorldVector(Roadshape.vertices[i].normal));

                float t = slice / (Slices - 1f);
                uvs.Add(new Vector2(Roadshape.vertices[i].u,t));

                Vector3 transformed_point_next = op.LocalToWorldPosition(roadpoint_next * RoadScaler);

                Vector3 transformed_point_following = op_next.LocalToWorldPosition(roadpoint * RoadScaler);

                //Gizmos.DrawSphere(transformed_point, HandleUtility.GetHandleSize(transformed_point) * 0.1f);
                if (DrawSegments)
                {
                    Handles.color = Color.white;
                    Handles.DrawLine(transformed_point, transformed_point_next, 3f);
                    Handles.DrawLine(transformed_point, transformed_point_following, 1f);
                }
            }

            // Triangles
            for (int i = 0; i < Roadshape.vertices.Length - 2; i += 2)
            {
                // hack hack
                if (slice == Slices)
                {
                    break;
                }

                int first_start = slice * Roadshape.vertices.Length + i + 1;
                int first_end = first_start + 1;

                int second_start = first_start + Roadshape.vertices.Length;
                int second_end = first_end + Roadshape.vertices.Length;

                // 1st triangle
                triangles.Add(first_start);
                triangles.Add(second_start);
                triangles.Add(second_end);

                // 2nd triangle
                triangles.Add(first_start);
                triangles.Add(second_end);
                triangles.Add(first_end);
            }

            if (slice < Slices)
            {
                // Special case, loop around the 2D mesh
                int index_start = slice * Roadshape.vertices.Length + 15;
                int index_end = slice * Roadshape.vertices.Length;

                int next_start = (slice + 1) * Roadshape.vertices.Length + 15;
                int next_end = (slice + 1) * Roadshape.vertices.Length;

                // 1st triangle
                triangles.Add(index_start);
                triangles.Add(next_start);
                triangles.Add(next_end);

                // 2nd triangle
                triangles.Add(index_start);
                triangles.Add(next_end);
                triangles.Add(index_end);

            }

            // Loop from first item in the array until 2nd last item
            // Because we are using index i+1 inside the loop
            for (int i = 0; i < n - 1; i++)
            {
                // I need 2 bezier points: i & i+1
                Vector3 first_anchor = points[i].getAnchorPoint();
                Vector3 second_anchor = points[i + 1].getAnchorPoint();

                Vector3 first_control = points[i].getSecondControlPoint();
                Vector3 second_control = points[i + 1].getFirstControlPoint();

                if (DrawBezier)
                    Handles.DrawBezier(first_anchor, second_anchor, first_control, second_control,
                    Color.green, null, 3f);
            }


        }

        if(ClosedPath)
            spawnedCar.transform.position = CalculateBezierPoint(TSimulate, points[0].getAnchorPoint(), points[0].getFirstControlPoint(), points[0].getSecondControlPoint(), points[points.Count - 2].getAnchorPoint());
        else
            spawnedCar.transform.position = CalculateBezierPoint(TSimulate, points[0].getAnchorPoint(), points[0].getFirstControlPoint(), points[0].getSecondControlPoint(), points[points.Count - 1].getAnchorPoint());

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);

        GetComponent<MeshFilter>().sharedMesh = mesh;   
        GetComponent<MeshRenderer>().material= CreateMaterial(roadTexture);   

        //}
        
    }


    public Material CreateMaterial(Texture texture)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.mainTexture = texture;
        return mat; 
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 first_a, Vector3 first_c,
                                Vector3 second_c, Vector3 second_a)
    {
        Vector3 a = Vector3.Lerp(first_a, first_c, t);
        Vector3 b = Vector3.Lerp(first_c, second_c, t);
        Vector3 c = Vector3.Lerp(second_c, second_a, t);

        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        Vector3 bez = Vector3.Lerp(d, e, t);
        return bez;
    }
}