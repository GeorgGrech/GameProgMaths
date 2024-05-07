using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MyMesh : MonoBehaviour
{
    [Range(0, 100)]
    public int Segments = 20;

    [Range(0, 100)]
    public float Radius = 20;
    private void Awake()
    {
        //GenerateMesh();
    }

    void GenerateMesh()
    {
        //A simple square
        Vector3[] newVertices = new Vector3[4];
        newVertices[0] = new Vector3(0, 0, 0);
        newVertices[1] = new Vector3(1, 0, 0);
        newVertices[2] = new Vector3(0.5f, 1, 0);
        newVertices[3] = new Vector3(1.5f, 1, 0);

        int[] newTriangles = new int[6];
        //First triangle
        newTriangles[0] = 0;
        newTriangles[1] = 1;
        newTriangles[2] = 2;
        //Second triangle
        newTriangles[3] = 1;
        newTriangles[4] = 3;
        newTriangles[5] = 2;

        //Create the mesh
        Mesh mesh = new Mesh();
        mesh.SetVertices(newVertices);
        mesh.SetTriangles(newTriangles,0);
        mesh.RecalculateNormals();

        //Get the mesh filter and set mesh
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void GenerateDisc()
    {
        Vector3 center = transform.position;
        //The list for vertices 
        List<Vector3> vertices = new List<Vector3>();
        //Center of the disc is the first one
        vertices.Add(center);
        //Compute and add the other vertices
        for (int i = Segments - 1; i >= 0; i--)
        {
            float angle = 2.0f * Mathf.PI * i / (float)Segments;
            float x  = Mathf.Cos(angle)*Radius;
            float y  = Mathf.Sin(angle)*Radius;
            vertices.Add(new Vector3(x, y, 0f));
        }

        List<int> tris = new List<int>();
        for (int i = Segments - 1 - 1; i >= 0; i--)
        {
            //1. 
            tris.Add(0);
            //2.
            tris.Add(i + 1);
            //3.
            tris.Add(i + 2);
        }
        //Last triangle is a special case
        tris.Add(0);
        tris.Add(Segments);
        tris.Add(1);

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(tris,0);
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Segments; i++)
        {
            float angle = 2.0f * Mathf.PI * i / (float)Segments;
            float x = Mathf.Cos(angle) * Radius;
            float y = Mathf.Sin(angle) * Radius;
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position + new Vector3(x, y, 0), .2f);
        }
    }

    private void OnValidate()
    {
        GenerateDisc();
    }
}
