using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelTextureAtlas
{
    static int numberOfTexturesWidth = 4;
    static int numberOfTexturesHeight = 4;

    public static Vector2 getUVs(int blockType, int corner)
    {
        // UV Structure
        // 3--2
        // |  |
        // 0--1

        int blockW = blockType % numberOfTexturesHeight;
        int blockH = Mathf.FloorToInt(blockType / numberOfTexturesHeight);

        float UVx = (1.0f / numberOfTexturesWidth) * blockW;
        float UVy = 1.0f - ((1.0f / numberOfTexturesHeight) * blockH);

        return (new Vector2(UVx, UVy) + UVOffsets[corner]);
    }

    static Vector2[] UVOffsets =
    {
        new Vector2(0, 0),
        new Vector2(1.0f/(float)numberOfTexturesWidth, 0),
        new Vector2(1.0f/(float)numberOfTexturesWidth, -(1.0f/(float)numberOfTexturesHeight)),
        new Vector2(0, -(1.0f/(float)numberOfTexturesHeight))
    };
}
