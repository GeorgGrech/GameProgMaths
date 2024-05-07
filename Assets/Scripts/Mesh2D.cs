using UnityEngine;

[CreateAssetMenu]
public class Mesh2D : ScriptableObject {

	// A 2D vertex
	[System.Serializable]
	public class Vertex {
		public Vector2 point;
		public Vector2 normal;
		public float u;
	}
	public int[] lineIndices;
	public Vertex[] vertices;

	public int VertexCount => vertices.Length;
	public int LineCount => lineIndices.Length;
}
