using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SmoothingRecipe))]
public class SmoothingRecipeEditor : Editor {

    string[] string_choices = new[] { "Any Solid Block", "0", "1", "2" };
    string[] string_choices_to_block = new[] { "0", "1", "2" };

    public override void OnInspectorGUI()
    {
        var recipes = target as SmoothingRecipe;
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Randomness", style);
        recipes.randomnessEnabled = GUILayout.Toggle(recipes.randomnessEnabled, "Enable Randomness");
        GUILayout.Label("Min");
        recipes.MinRand = GUILayout.HorizontalSlider(recipes.MinRand, 0.0f, 10.0f);
        GUILayout.Label("Max");
        recipes.MaxRand = GUILayout.HorizontalSlider(recipes.MaxRand, 0.0f, 10.0f);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("-"))
        {
            recipes.recipes.RemoveAt(recipes.recipes.Count - 1);
        }

        if (GUILayout.Button("+"))
        {
            recipes.recipes.Add(new SingleRecipe());
        }

        GUILayout.EndHorizontal();
        int index = 0;
        foreach (var smoothingRecipe in recipes.recipes)
        {
            style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Recipe #" + index.ToString(), style);
            GUILayout.Label("Previous Layer:");

            GUILayout.BeginHorizontal();

            smoothingRecipe.blockLeftUp = EditorGUILayout.Popup(smoothingRecipe.blockLeftUp, string_choices);
            smoothingRecipe.blockUp = EditorGUILayout.Popup(smoothingRecipe.blockUp, string_choices);
            smoothingRecipe.blockRightUp = EditorGUILayout.Popup(smoothingRecipe.blockRightUp, string_choices);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            smoothingRecipe.blockLeftCent = EditorGUILayout.Popup(smoothingRecipe.blockLeftCent, string_choices);
            smoothingRecipe.blockCent = EditorGUILayout.Popup(smoothingRecipe.blockCent, string_choices);
            smoothingRecipe.blockRightCent = EditorGUILayout.Popup(smoothingRecipe.blockRightCent, string_choices);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            smoothingRecipe.blockLeftDown = EditorGUILayout.Popup(smoothingRecipe.blockLeftDown, string_choices);
            smoothingRecipe.blockDown = EditorGUILayout.Popup(smoothingRecipe.blockDown, string_choices);
            smoothingRecipe.blockRightDown = EditorGUILayout.Popup(smoothingRecipe.blockRightDown, string_choices);
            GUILayout.EndHorizontal();

            GUILayout.Label("To:");


            smoothingRecipe.newBlock = EditorGUILayout.Popup(smoothingRecipe.newBlock, string_choices_to_block);

            index++;
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("-"))
        {
            recipes.recipes.RemoveAt(recipes.recipes.Count - 1);
        }

        if (GUILayout.Button("+"))
        {
            recipes.recipes.Add(new SingleRecipe());
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Fill!"))
        {
            while (recipes.recipes.Count < 100)
                recipes.recipes.Add(new SingleRecipe());
        }

        if (GUILayout.Button("Randomize!"))
        {
            foreach (var smoothingRecipe in recipes.recipes)
            {
                smoothingRecipe.blockLeftUp = Random.Range(0, 2);
                smoothingRecipe.blockLeftCent = Random.Range(0, 2);
                smoothingRecipe.blockLeftDown = Random.Range(0, 2);
                smoothingRecipe.blockUp = Random.Range(0, 2);
                smoothingRecipe.blockCent = Random.Range(0, 2);
                smoothingRecipe.blockDown = Random.Range(0, 2);
                smoothingRecipe.blockRightUp = Random.Range(0, 2);
                smoothingRecipe.blockRightCent = Random.Range(0, 2);
                smoothingRecipe.blockRightDown = Random.Range(0, 2);

                smoothingRecipe.newBlock = Random.Range(1, string_choices_to_block.Length);


            }
        }
        // Save the changes back to the object
        EditorUtility.SetDirty(target);
    }
}
