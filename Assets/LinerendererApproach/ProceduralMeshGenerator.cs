using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ProceduralMeshGenerator : MonoBehaviour
{
    void Start()
    {
        // Get the MeshFilter component
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        // Create a new Mesh object
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Define the vertices (points) of the quad
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-1, 0, -1);
        vertices[1] = new Vector3(-1, 0, 1);
        vertices[2] = new Vector3(1, 0, 1);
        vertices[3] = new Vector3(1, 0, -1);

        // Define the triangles to connect the vertices
        // Triangles are defined by three vertices, so a quad needs two triangles
        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2; // First triangle: 0, 1, 2

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3; // Second triangle: 0, 2, 3

        // Define the UV coordinates for texturing
        // UVs are 2D vectors for mapping textures onto the mesh
        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 1);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(1, 0);

        // Assign the vertex, triangle, and UV data to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        // Recalculate normals to ensure correct lighting and shading
        mesh.RecalculateNormals();
    }
}
