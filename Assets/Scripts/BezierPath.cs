using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BezierPath : MonoBehaviour
{

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


    [Range(3, 255)]
    public int Slices = 32;

    public GameObject MyObject;


    public Mesh mesh;

    public Texture roadTexture;

    [Space(10)]
    [Header("Car related vars")]
    [SerializeField] private GameObject car;
    public float carSpeed = 5;
    public Vector3 carOffset;
    public Vector3 cameraOffset;

    [Range(0f, 1f)]
    public float TSimulate = 0.0f;

    private GameObject spawnedCar;

    [Space(10)]
    [Header("Lamps")]
    public GameObject lamp;
    public Vector3 lampPosOffset;
    public int lampSpawnFrequency;

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


    private void Start()
    {

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

            //Handles.DrawBezier(first_anchor, second_anchor, first_control, second_control,
            //  Color.green, null, 3f);
        }

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
            Camera.main.transform.parent = spawnedCar.transform;
            Camera.main.transform.localPosition = cameraOffset;
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

            //if (DrawBezier)
              //  Handles.DrawBezier(first_anchor, second_anchor, first_control, second_control,
                //Color.green, null, 3f);
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

            //Handles.DrawBezier(first_anchor, second_anchor, first_control, second_control,
              //  Color.green, null, 3f);
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

            if(slice % lampSpawnFrequency == 0) //Spawn lamp
            {
                GameObject spawnedLamp = Instantiate(lamp,op.Pos,Quaternion.identity);
                spawnedLamp.transform.position += lampPosOffset;
                spawnedLamp.transform.LookAt(op.Pos);
                spawnedLamp.transform.rotation = new Quaternion(0, spawnedLamp.transform.rotation.y, 0, 0);
            }

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
                uvs.Add(new Vector2(Roadshape.vertices[i].u, t));

                Vector3 transformed_point_next = op.LocalToWorldPosition(roadpoint_next * RoadScaler);

                Vector3 transformed_point_following = op_next.LocalToWorldPosition(roadpoint * RoadScaler);

                //Gizmos.DrawSphere(transformed_point, HandleUtility.GetHandleSize(transformed_point) * 0.1f);
                /*if (DrawSegments)
                {
                    Handles.color = Color.white;
                    Handles.DrawLine(transformed_point, transformed_point_next, 3f);
                    Handles.DrawLine(transformed_point, transformed_point_following, 1f);
                }*/
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

                /*if (DrawBezier)
                    Handles.DrawBezier(first_anchor, second_anchor, first_control, second_control,
                    Color.green, null, 3f);*/
            }


        }

        /*if (ClosedPath)
            spawnedCar.transform.position = CalculateBezierPoint(TSimulate, points[0].getAnchorPoint(), points[0].getFirstControlPoint(), points[0].getSecondControlPoint(), points[points.Count - 2].getAnchorPoint());
        else
            spawnedCar.transform.position = CalculateBezierPoint(TSimulate, points[0].getAnchorPoint(), points[0].getFirstControlPoint(), points[0].getSecondControlPoint(), points[points.Count - 1].getAnchorPoint());*/

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);

        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshRenderer>().material = CreateMaterial(roadTexture);

        //}

    }

    private void Update()
    {
        if (spawnedCar)
        {
            TSimulate += Time.deltaTime * carSpeed;
            if(TSimulate >= 1)
            {
                TSimulate = 0;
            }

            Vector3 tangent = GetTangentOnPath(TSimulate, points, ClosedPath);


            spawnedCar.transform.position = GetPointOnPath(TSimulate, points, ClosedPath)+carOffset;
            Quaternion rotation = Quaternion.LookRotation(tangent, Vector3.up); // Use Vector3.up as a default up vector
            spawnedCar.transform.rotation = rotation;

        }
    }

    /*
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
        
    }*/


    public Material CreateMaterial(Texture texture)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.mainTexture = texture;
        return mat; 
    }

    /*private Vector3 CalculateBezierPoint(float t, Vector3 first_a, Vector3 first_c,
                                Vector3 second_c, Vector3 second_a)
    {
        Vector3 a = Vector3.Lerp(first_a, first_c, t);
        Vector3 b = Vector3.Lerp(first_c, second_c, t);
        Vector3 c = Vector3.Lerp(second_c, second_a, t);

        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        Vector3 bez = Vector3.Lerp(d, e, t);
        return bez;
    }*/

    // Function to calculate a point on a cubic Bezier curve
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // Use De Casteljau's algorithm to get the interpolated point
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; // (1-t)^3 * p0
        p += 3 * uu * t * p1; // 3 * (1-t)^2 * t * p1
        p += 3 * u * tt * p2; // 3 * (1-t) * t^2 * p2
        p += ttt * p3;        // t^3 * p3

        return p;
    }

    // Function to get the point on the entire Bezier path given a parameter t between 0 and 1
    public Vector3 GetPointOnPath(float t, List<BezierPoint> points, bool closedPath)
    {
        int n = points.Count;
        if (n < 2)
        {
            throw new System.Exception("The path must contain at least two Bezier points.");
        }

        // Get the total number of segments
        int nSegments = n;

        // Ensure t is between 0 and 1
        t = Mathf.Clamp01(t);

        // Calculate which segment we are on
        float segmentLength = 1f / nSegments;
        int segmentIndex = Mathf.FloorToInt(t / segmentLength);

        // Calculate the local t value within the segment
        float localT = (t - segmentIndex * segmentLength) / segmentLength;

        // Get the corresponding points for the segment
        int p1Index = segmentIndex;
        int p2Index = (segmentIndex + 1) % n;

        BezierPoint p1 = points[p1Index];
        BezierPoint p2 = points[p2Index];

        Vector3 anchor1 = p1.getAnchorPoint();
        Vector3 control1 = p1.getSecondControlPoint();
        Vector3 control2 = p2.getFirstControlPoint();
        Vector3 anchor2 = p2.getAnchorPoint();

        // Calculate the point on the cubic Bezier curve
        return CalculateBezierPoint(localT, anchor1, control1, control2, anchor2);
    
    }

    // Function to calculate a cubic Bezier tangent
    private Vector3 CalculateBezierTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // Derivative formula for cubic Bezier
        return 3 * Mathf.Pow(1 - t, 2) * (p1 - p0) +
               6 * (1 - t) * t * (p2 - p1) +
               3 * Mathf.Pow(t, 2) * (p3 - p2);
    }

    // Function to get the tangent on the entire Bezier path
    public Vector3 GetTangentOnPath(float t, List<BezierPoint> points, bool closedPath)
    {
        int n = points.Count;
        if (n < 2)
        {
            throw new System.Exception("The path must contain at least two Bezier points.");
        }

        int nSegments = closedPath ? n : n - 1;
        t = Mathf.Clamp01(t);

        float segmentLength = 1f / nSegments;
        int segmentIndex = Mathf.FloorToInt(t / segmentLength);

        float localT = (t - segmentIndex * segmentLength) / segmentLength;

        int p1Index = segmentIndex;
        int p2Index = (segmentIndex + 1) % n;

        BezierPoint p1 = points[p1Index];
        BezierPoint p2 = points[p2Index];

        Vector3 anchor1 = p1.getAnchorPoint();
        Vector3 control1 = p1.getSecondControlPoint();
        Vector3 control2 = p2.getFirstControlPoint();
        Vector3 anchor2 = p2.getAnchorPoint();

        // Calculate the tangent on the cubic Bezier curve
        return CalculateBezierTangent(localT, anchor1, control1, control2, anchor2);
    }
}