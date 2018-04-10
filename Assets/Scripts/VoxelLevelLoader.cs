using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    [Header("Terraria File", order = 1)]
    BinaryReader binaryReader;

    [Header("3D", order = 2)]
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

    [SerializeField]
    SmoothingRecipe recipe;

    public void setBlock(Vector3 pos, int blockType)
    {
        if (blockType > 0)
        {
            Vector3 chunkPos = new Vector3(Mathf.FloorToInt(pos.x / 16.0f), Mathf.FloorToInt(pos.y / 256f), Mathf.FloorToInt(pos.z / 16.0f));

            var foundChunk = chunks.Find(c => c.pos == (chunkPos * 16));

            foundChunk.SetCell(new Vector3Int(Mathf.FloorToInt(pos.x % 16), Mathf.FloorToInt(pos.y % 256), Mathf.FloorToInt(pos.z % 16)), blockType);
        }
    }

    public int getBlock(Vector3 pos)
    {
        Vector3 chunkPos = new Vector3(Mathf.FloorToInt(pos.x / 16.0f), Mathf.FloorToInt(pos.y / 256f), Mathf.FloorToInt(pos.z / 16.0f));

        var foundChunk = chunks.Find(c => c.pos == (chunkPos * 16));
        if (foundChunk == null)
        {
            return 0;
        }
        return foundChunk.GetCell(new Vector3Int(Mathf.FloorToInt(pos.x % 16), Mathf.FloorToInt(pos.y % 256), Mathf.FloorToInt(pos.z % 16)));
    }    

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 0.0f;
        //TextAsset ta = Resources.Load("WLD files/TestWLD") as TextAsset;
        //Debug.Log(ta.bytes);
        //Stream s = new MemoryStream(ta.bytes);
        //World world = new World();
        //world.Read(s);

        //return;

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
                for (int j = -chunkDepth; j < chunkDepth; j++)
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

        StartCoroutine(CreateWorld(width, height));
    }

    IEnumerator CreateWorld(int width, int height)
    {
        CreateBaseLayer(width);
        LoadChunks();
        //yield return new WaitForEndOfFrame();
        for (int z = 0; z < depth; z++)
        {
            if (recipe != null)
            {
                OneStepSmoothing(z, width, height, 1, recipe);
            }
            else
            {
                OneStepSmoothing(z, width, height, 1);
            }
            //UpdateAllChunks();
            //yield return new WaitForEndOfFrame();
        }


        for (int z = 0; z > -depth; z--)
        {
            if (recipe != null)
            {
                OneStepSmoothing(z, width, height, -1, recipe);
            }
            else
            {
                OneStepSmoothing(z, width, height, -1);
            }
            //UpdateAllChunks();
            //yield return new WaitForEndOfFrame();
        }

        UpdateAllChunks();
        Time.timeScale = 1.0f;

        yield return null;
    }

    void LoadChunks()
    {
        foreach (var c in chunks)
        {
            var go = new GameObject();
            go.name = "Chunk";
            go.transform.parent = this.transform;
            go.AddComponent<VoxelRender>();
            go.GetComponent<MeshRenderer>().material = new Material(baseMat);
            var rend = go.GetComponent<VoxelRender>();
            rend.transform.localPosition = c.pos;
            rend.Ready(c);
            chunkRenderers.Add(rend);
        }
    }

    void UpdateAllChunks()
    {
        foreach (var c in chunkRenderers)
        {
            c.GenerateVoxelMesh();
            c.UpdateMesh();
        }
    }

    void CreateBaseLayer(int width)
    {
        //Base Layer
        int index = 0;
        foreach (var a in texture.GetPixels())
        {
            Vector3 pos = Vector3.zero;
            pos.x = index % width;
            pos.y = index / width;

            pos.z = 0;

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

    void OneStepSmoothing(int previousLayer, int width, int height, int direction)
    {
        //Super-Simple Ruleset
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 checkPos = new Vector3(x, y, previousLayer);
                Vector3 newPos = new Vector3(x, y, previousLayer + direction);
                int blockType = 0;

                int numberOfNeighborsInPreviousLayer = 0;
                if (getBlock(checkPos) == 0)
                {
                    setBlock(newPos, 0);
                }
                else
                {
                    numberOfNeighborsInPreviousLayer += getBlock(checkPos + new Vector3(1, 0, 0)) == 0 ? 0 : 1;
                    numberOfNeighborsInPreviousLayer += getBlock(checkPos + new Vector3(-1, 0, 0)) == 0 ? 0 : 1;
                    numberOfNeighborsInPreviousLayer += getBlock(checkPos + new Vector3(0, 1, 0)) == 0 ? 0 : 1;
                    numberOfNeighborsInPreviousLayer += getBlock(checkPos + new Vector3(0, -1, 0)) == 0 ? 0 : 1;
                    numberOfNeighborsInPreviousLayer += getBlock(checkPos + new Vector3(1, 1, 0)) == 0 ? 0 : 1;
                    numberOfNeighborsInPreviousLayer += getBlock(checkPos + new Vector3(1, -1, 0)) == 0 ? 0 : 1;
                    numberOfNeighborsInPreviousLayer += getBlock(checkPos + new Vector3(-1, 1, 0)) == 0 ? 0 : 1;
                    numberOfNeighborsInPreviousLayer += getBlock(checkPos + new Vector3(-1, -1, 0)) == 0 ? 0 : 1;

                    numberOfNeighborsInPreviousLayer += Random.Range(-2, 3);

                    // if number of blocks around this on previous layer < 4
                    if (numberOfNeighborsInPreviousLayer < 5)
                    {
                        // Set block to air
                        setBlock(newPos, 0);
                    }
                    else
                    {
                        // If number of blocks sorrounding is > 5
                        // Set block to full
                        setBlock(newPos, getBlock(checkPos));
                    }         
                }
            }
        }
    }

    void OneStepSmoothing(int previousLayer, int width, int height, int direction, SmoothingRecipe recipe)
    {
        //Super-Simple Ruleset
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 checkPos = new Vector3(x, y, previousLayer);
                Vector3 newPos = new Vector3(x, y, previousLayer + direction);
                int blockType = recipe.CheckRecipe(getBlock(checkPos + new Vector3(-1,  1, 0)), getBlock(checkPos + new Vector3( 0,  1, 0)), getBlock(checkPos + new Vector3( 1,  1, 0)),
                                                   getBlock(checkPos + new Vector3(-1,  0, 0)), getBlock(checkPos + new Vector3( 0,  0, 0)), getBlock(checkPos + new Vector3( 1,  0, 0)),
                                                   getBlock(checkPos + new Vector3(-1, -1, 0)), getBlock(checkPos + new Vector3( 0, -1, 0)), getBlock(checkPos + new Vector3( 1, -1, 0)));

                setBlock(newPos, blockType);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

