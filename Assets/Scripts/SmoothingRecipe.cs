using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SingleRecipe
{

    public int newBlock;

    public int blockLeftUp;
    public int blockUp;
    public int blockRightUp;

    public int blockLeftCent;
    public int blockCent;
    public int blockRightCent;

    public int blockLeftDown;
    public int blockDown;
    public int blockRightDown;
}

[CreateAssetMenu(fileName = "New Smoothing Recipe", menuName = "Smoothing Recipe")]
public class SmoothingRecipe : ScriptableObject {

    public List<SingleRecipe> recipes = new List<SingleRecipe>();

    float minRandomness;
    float maxRandomness;

    public float MinRand { get { return minRandomness; } set { minRandomness = value; maxRandomness = value > maxRandomness ? value : maxRandomness; } }
    public float MaxRand { get { return maxRandomness; } set { maxRandomness = value; minRandomness = value < minRandomness ? value : minRandomness; } }

    public bool randomnessEnabled;

    public int CheckRecipe(int ul, int uc, int ur,
                           int cl, int cc, int cr,
                           int dl, int dc, int dr)
    {
        foreach(var recipe in recipes)
        {
            if (((recipe.blockLeftUp == 0 && ul != 0) || (recipe.blockLeftUp == ul + 1)) &&
                ((recipe.blockLeftCent == 0 && cl != 0) || (recipe.blockLeftCent == cl + 1)) &&
                ((recipe.blockLeftDown == 0 && dl != 0) || (recipe.blockLeftDown == dl + 1)) &&
                ((recipe.blockRightUp == 0 && ur != 0) || (recipe.blockRightUp == ur + 1)) &&
                ((recipe.blockRightCent == 0 && cr != 0) || (recipe.blockRightCent == cr + 1)) &&
                ((recipe.blockRightDown == 0 && dr != 0) || (recipe.blockRightDown == dr + 1)) &&
                ((recipe.blockUp == 0 && uc != 0) || (recipe.blockUp == uc + 1)) &&
                ((recipe.blockCent == 0 && cc != 0) || (recipe.blockCent == cc + 1)) &&
                ((recipe.blockDown == 0 && dc != 0) || (recipe.blockDown == dc + 1)))
            {
                return recipe.newBlock;
            }
        }

        return 0;
    }

}
