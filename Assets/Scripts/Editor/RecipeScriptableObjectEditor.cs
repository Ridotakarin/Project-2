using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Recipe))]
public class RecipeScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Recipe recipe = (Recipe)target;

        GUIStyle boldLabel = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter
         };

        // QUANTITY
        EditorGUILayout.LabelField("QUANTITY", boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Quantity"), new GUIContent("Quantity"), true);
        EditorGUILayout.Space();

        // OUTPUT
        EditorGUILayout.LabelField("OUTPUT", boldLabel);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        DrawItemSlot(recipe.itemOutput, "itemOutput");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // RECIPE (3x3 Grid)
        EditorGUILayout.LabelField("RECIPE", boldLabel);
        for (int x = 0; x < 3; x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = 0; y < 3; y++)
            {
                string propertyName = $"item_{x}{y}";
                DrawItemSlot(GetItemByName(recipe, propertyName), propertyName);
            }
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawItemSlot(Item item, string propertyName)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(150));
        if (item?.image != null)
        {
            Rect spriteRect = item.image.rect;
            Texture2D texture = item.image.texture;

            Rect texCoords = new Rect(
                spriteRect.x / texture.width,
                spriteRect.y / texture.height,
                spriteRect.width / texture.width,
                spriteRect.height / texture.height
            );

            GUI.DrawTextureWithTexCoords(
                GUILayoutUtility.GetRect(150, 150),
                texture,
                texCoords   
            );
        }
        else
        {
            GUILayout.Box("", GUILayout.Width(150), GUILayout.Height(150));
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();
    }

    private Item GetItemByName(Recipe recipe, string propertyName)
    {
        return (Item)typeof(Recipe).GetField(propertyName)?.GetValue(recipe);
    }
}
