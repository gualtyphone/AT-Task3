using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pair
{
    public Color selectColor;
    public int correspondingCube;
}

public class VoxelLevelLoader : MonoBehaviour {

    [Header("2D", order = 0)]
    [SerializeField]
    Texture2D texture;

    [SerializeField]
    List<Pair> mapBlocks;

    [Header("3D", order = 1)]
    [SerializeField]
    int depth;

    [SerializeField]
    List<VoxelData> chunks;
    List<VoxelRender> chunkRenderers;
    int chunkWidth;
    int chunkHeight;
    int chunkDepth;

    [SerializeField]
    Material baseMat;

    public void setBlock(Vector3 pos, int blockType)
    {
        if (blockType > 0)
        {
            Vector3 chunkPos = new Vector3(Mathf.FloorToInt(pos.x / 16.0f), Mathf.FloorToInt(pos.y / 256f), Mathf.FloorToInt(pos.z / 16.0f));

            var foundChunk = chunks.Find(c => c.pos == (chunkPos * 16));

            foundChunk.SetCell(new Vector3Int(Mathf.FloorToInt(pos.x % 16), Mathf.FloorToInt(pos.y % 256), Mathf.FloorToInt(pos.z % 16)));
        }
        else
        {
            int i = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (VoxelData c in chunks)
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3(c.pos.x + 8.0f, c.pos.y + 128f, c.pos.z + 8.0f);
            Gizmos.DrawWireCube(center, new Vector3(16.0f, 256f, 16.0f));
        }
    }

    // Use this for initialization
    void Start()
    {
        chunks = new List<VoxelData>();
        chunkRenderers = new List<VoxelRender>();
        //Create the needed chunks
        chunkWidth = Mathf.CeilToInt(texture.width / 16.0f);
        chunkHeight = Mathf.CeilToInt(texture.height / 256f);
        chunkDepth = Mathf.CeilToInt(depth / 16.0f);
        for (int i = 0; i < chunkWidth; i++)
        {
            for (int w = 0; w < chunkHeight; w++)
            {
                for (int j = 0; j < chunkDepth; j++)
                {
                    VoxelData c = new VoxelData();
                    c.pos = new Vector3(i * 16, w * 256f, j * 16);
                    chunks.Add(c);
                }
            }
        }

        //Fill the Chunks

        int width = texture.width;
        int height = texture.height;

        int z = 0;
        for (z = 0; z < depth; z++)
        {

            int index = 0;
            foreach (var a in texture.GetPixels())
            {
                Vector3 pos = Vector3.zero;
                pos.x = index % width;
                pos.y = index / width;

                pos.z = z;

                float minDist = float.MaxValue;
                int closestBlock = mapBlocks[0].correspondingCube;

                foreach (var pair in mapBlocks)
                {
                    float currDist = Vector4.Distance(pair.selectColor, a);
                    if (currDist < minDist)
                    {
                        minDist = currDist;
                        closestBlock = pair.correspondingCube;
                    }
                }

                setBlock(pos, closestBlock);

                index++;
            }
        }

        foreach (var c in chunks)
        {
            var go = new GameObject();
            go.transform.parent = this.transform;
            go.AddComponent<VoxelRender>();
            go.GetComponent<MeshRenderer>().material = new Material(baseMat);
            var rend = go.GetComponent<VoxelRender>();
            rend.transform.position = c.pos;
            rend.Ready(c);
            chunkRenderers.Add(rend);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}

