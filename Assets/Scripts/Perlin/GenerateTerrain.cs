using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{

    [System.Serializable]
    public class NoiseParams
    {
        public float FrequencyScale;
        public float AmplitudeScale;
    }

    [Range(1, 1000)]
    public float Size = 100f;
    
    [Range(2, 255)]
    public int Segments = 100;

    public NoiseParams[] NoiseLayers;

    private Mesh myMesh = null;

    public bool clampBelowValue;
    public float clampValue;

    private void OnValidate()
    {
        GenerateMesh();
    }

    public void GenerateMesh()
    {   
        //Make sure the mesh exists and is cleared
        if(myMesh == null)
            myMesh = new Mesh();
        else
            myMesh.Clear();

        //List of vertices
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        //List of triangles
        List<int> triangles = new List<int>();

        //Loop
        for(int y_seg = 0; y_seg <= Segments; y_seg++)
        {
            for (int x_seg = 0; x_seg <= Segments; x_seg++)
            {
                float x = x_seg * (Size/(float)Segments);
                float y = y_seg * (Size/(float)Segments);

                //Plain Random noise
                //float z = Random.Range(0,1)*AmplitudeScaler;

                //Plan Random noise
                float z = 0;


                for(int i = 0; i<NoiseLayers.Length; i++)
                {
                    z += (Mathf.PerlinNoise(x / NoiseLayers[i].FrequencyScale, y / NoiseLayers[i].FrequencyScale) -.5f)
                        * NoiseLayers[i].AmplitudeScale;
                }
                 
                if(clampBelowValue && z < clampValue)
                {
                    z = clampValue;
                }

                Vector3 vert = new Vector3 (x, z, y);
                //Debug.Log(vert);
                //Add the vertex
                vertices.Add(vert);
                uvs.Add(new Vector2(x,y));
              

            }

        }

        for (int y_seg = 0; y_seg < Segments; y_seg++)
        {
            for (int x_seg = 0; x_seg < Segments; x_seg++)
            {
                int TopLeft = x_seg + y_seg * (Segments + 1);
                int TopRight = TopLeft + 1;
                int BotLeft = TopLeft + Segments + 1;
                int BotRight = BotLeft + 1;

                //1st triangle
                triangles.Add(TopLeft);
                triangles.Add(BotLeft);
                triangles.Add(TopRight);
                //2nd triangle
                triangles.Add(TopRight);
                triangles.Add(BotLeft);
                triangles.Add(BotRight);
            }

        }


        //Assign the vertices and triangles
        myMesh.SetVertices(vertices);
        myMesh.SetTriangles(triangles, 0);
        myMesh.SetUVs(0, uvs);
        myMesh.RecalculateNormals();

        //Assign the mesh
        GetComponent<MeshFilter>().sharedMesh = myMesh;
    }
}
