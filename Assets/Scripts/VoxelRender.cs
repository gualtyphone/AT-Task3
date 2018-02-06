using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class VoxelRender : MonoBehaviour {

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> UVs;

    VoxelData data;

    //BoxCollider coll;

    MeshCollider coll;

    public float scale = 1f;

    float adjScale;

    private void OnDrawGizmosSelected()
    { 
        Gizmos.color = Color.red;
        Vector3 center = new Vector3(transform.position.x + 7.5f, transform.position.y + 127.5f, transform.position.z + 7.5f);
        //Gizmos.DrawWireCube(center, new Vector3(16.0f, 256f, 16.0f));
    }

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        adjScale = scale * 0.5f;
        coll = GetComponent<MeshCollider>();

        //coll.isTrigger = true;
        coll.sharedMesh = GetComponent<MeshFilter>().mesh;
        //coll.size = new Vector3(16.0f, 256f, 16.0f);
    }

    // Use this for initialization
    public void Ready (VoxelData _data)
    {
        data = _data;
        GenerateVoxelMesh();
        UpdateMesh();
	}

    public void GenerateVoxelMesh()
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

    public void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = UVs.ToArray();
        mesh.RecalculateNormals();
        coll.sharedMesh = mesh;
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    Vector3 chunkPos = other.transform.position - transform.position;
    //    Vector3Int cPos = new Vector3Int(Mathf.FloorToInt(chunkPos.x), Mathf.FloorToInt(chunkPos.y), Mathf.FloorToInt(chunkPos.z));
    //    if(data.GetCell(cPos) != 0)
    //    {
    //        other.GetComponent<Rigidbody>().velocity = new Vector3(other.GetComponent<Rigidbody>().velocity.x, Mathf.Max(0.0f, other.GetComponent<Rigidbody>().velocity.y), other.GetComponent<Rigidbody>().velocity.z);
    //    }
    //}
}
