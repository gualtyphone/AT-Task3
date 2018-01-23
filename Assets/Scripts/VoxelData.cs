using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VoxelData {

    [SerializeField]
    int[,,] data = new int[16, 256, 16];

    public Vector3 pos;

    public int Width
    {
        get { return data.GetLength(0); }
    }

    public int Height
    {
        get { return data.GetLength(1); }
    }

    public int Depth
    {
        get { return data.GetLength(2); }
    }

    public int GetCell(Vector3Int pos)
    {
        return data[pos.x, pos.y, pos.z];
    }

    public int GetNeighbor(Vector3Int pos, Direction dir)
    {
        Vector3Int offsetToCheck = offsets[(int)dir];
        Vector3Int neighborCoord = pos + offsetToCheck;

        if (neighborCoord.x < 0 || neighborCoord.x >= Width ||
            neighborCoord.y < 0 || neighborCoord.y >= Height ||
            neighborCoord.z < 0 || neighborCoord.z >= Depth)
        {
            return 0;
        }
        else
        {
            return GetCell(neighborCoord);
        }
    }

    public void SetCell(Vector3Int pos)
    {
        data[pos.x, pos.y, pos.z] = 1;
    }

    Vector3Int[] offsets =
    {
        new Vector3Int( 0,  0,  1),
        new Vector3Int( 1,  0,  0),
        new Vector3Int( 0,  0, -1),
        new Vector3Int(-1,  0,  0),
        new Vector3Int( 0,  1,  0),
        new Vector3Int( 0, -1,  0)
    };


}  
    

public enum Direction
{
    North, 
    East,
    South, 
    West, 
    Up, 
    Down
}