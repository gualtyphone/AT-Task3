using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chunk
{
    public Vector2 pos;
    public  int[] blocks;

    public Chunk()
    {
        blocks = new int[64000];
    }

    public void Draw()
    {

    }
}

public class LevelLoader : MonoBehaviour {

    [Header("2D", order =0)]
    [SerializeField]
    Texture2D texture;

    [SerializeField]
    List<Pair> mapBlocks;

    [Header("3D", order = 1)]
    [SerializeField]
    int depth;

    [SerializeField]
    List<Chunk> chunks;
    int chunkWidth;
    int chunkDepth;

    public void setBlock(Vector3 pos, int blockType)
    {
        Vector2 chunkPos = new Vector2(Mathf.FloorToInt(pos.x / 16.0f), Mathf.FloorToInt(pos.z / 16.0f));

        chunks.Find(c => c.pos == chunkPos);
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Chunk c in chunks)
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3(c.pos.x + 8.0f, 125.0f, c.pos.y + 8.0f);
            Gizmos.DrawWireCube(center, new Vector3(16.0f, 250.0f, 16.0f));
        }
    }

    // Use this for initialization
    void Start () {

        //Create the needed chunks
        chunkWidth = Mathf.CeilToInt(texture.width / 16.0f);
        chunkDepth = Mathf.CeilToInt(depth / 16.0f);
        for (int i = 0; i < chunkWidth; i++)
        {
            for (int j = 0; j < chunkDepth; j++)
            {
                Chunk c = new Chunk();
                c.pos = new Vector2(i * 16, j * 16);
                chunks.Add(c);
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
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
