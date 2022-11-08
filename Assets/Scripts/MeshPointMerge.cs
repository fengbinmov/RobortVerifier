using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshPointMerge : MonoBehaviour
{
    public Mesh mesh1;
    public MeshFilter meshFilter;


    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uv = new List<Vector2>();
    List<int> triangles = new List<int>();

    [ContextMenu("Apply")]
    public void Apply() {

        var filter = GetComponent<MeshCollider>();
        var mesh = filter.sharedMesh;

        vertices.Clear();
        normals.Clear();
        uv.Clear();
        triangles.Clear();

        for (int i = 0; i < mesh.triangles.Length; i+=3)
        {
            int a = mesh.triangles[i];
            int b = mesh.triangles[i+1];
            int c = mesh.triangles[i+2];

            AddPoint(mesh.vertices[a], mesh.normals[a], mesh.uv[a]);
            AddPoint(mesh.vertices[b], mesh.normals[b], mesh.uv[b]);
            AddPoint(mesh.vertices[c], mesh.normals[c], mesh.uv[c]);
        }

        mesh1 = new Mesh();
        mesh1.vertices = vertices.ToArray();
        mesh1.normals = normals.ToArray();
        mesh1.triangles = triangles.ToArray();
        mesh1.uv = uv.ToArray();
        mesh1.triangles = triangles.ToArray();

        meshFilter.sharedMesh = mesh1;
    }

    private void AddPoint(Vector3 p,Vector3 n,Vector2 u) {

        int index = vertices.IndexOf(p);

        if (index > -1)
        {
            triangles.Add(index);
        }
        else
        {
            vertices.Add(p);
            normals.Add(n);
            uv.Add(u);

            triangles.Add(vertices.Count - 1);
        }
    }
}
