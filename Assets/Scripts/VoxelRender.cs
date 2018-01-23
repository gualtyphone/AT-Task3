using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelRender : MonoBehaviour {

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> UVs;

    public float scale = 1f;

    float adjScale;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        adjScale = scale * 0.5f;
    }

    // Use this for initialization
    public void Ready (VoxelData data)
    {
        GenerateVoxelMesh(data);
        UpdateMesh();
	}

    void GenerateVoxelMesh(VoxelData data)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        UVs = new List<Vector2>();

        for (int y = 0; y < data.Height; y++)
        {
            for (int z = 0; z < data.Depth; z++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    if (data.GetCell(new Vector3Int(x, y, z)) == 0)
                    {
                        continue;
                    }
                    MakeCube(adjScale, new Vector3((float)x * scale, (float)y * scale, (float)z * scale), new Vector3Int(x, y, z), data);
                }
            }
        }
    }

    void MakeCube(float cubeScale, Vector3 cubePos, Vector3Int pos, VoxelData data)
    {
        for (int i = 0; i < 6; i++)
        {
            if (data.GetNeighbor(pos, (Direction)i) == 0)
            {
                MakeFace((Direction)i, cubeScale, cubePos, data.GetCell(pos));
            }
        }
    }

    void MakeFace(Direction dir, float faceScale, Vector3 facePos, int cubeType)
    {
        vertices.AddRange(CubeMeshData.faceVertices(dir, faceScale, facePos));
        UVs.Add(VoxelTextureAtlas.getUVs(cubeType, 0));
        UVs.Add(VoxelTextureAtlas.getUVs(cubeType, 1));
        UVs.Add(VoxelTextureAtlas.getUVs(cubeType, 2));
        UVs.Add(VoxelTextureAtlas.getUVs(cubeType, 3));


        int vCount = vertices.Count;

        // Indices Structure
        // 3--2
        // |  |
        // 0--1

        triangles.Add(vCount - 4);
        triangles.Add(vCount - 4 + 1);
        triangles.Add(vCount - 4 + 2);
        triangles.Add(vCount - 4);
        triangles.Add(vCount - 4 + 2);
        triangles.Add(vCount - 4 + 3);
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = UVs.ToArray();
        mesh.RecalculateNormals();
    }
}
