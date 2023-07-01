using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName ="New Level Color Data",menuName = "Texture Level/Level Color Data")]
public class SO_LevelColorData : ScriptableObject
{
    public bool is2DLevel;
    public Texture2D colorPalette;
    public PixelLevelMap[] maps;
    public List<ColorToObj2D> colorArray_2D;
    public List<ColorToObj3D> colorArray_3D;

    public void ColorUpdate()
    {
        if (is2DLevel)
        {
            colorArray_2D = new List<ColorToObj2D>();
            for (int x = 0; x < colorPalette.width; x++)
                colorArray_2D.Add(new ColorToObj2D
                {
                    color = colorPalette.GetPixel(x, 0),
                    colorObjs = null
                });
        }
        else
        {
            colorArray_3D = new List<ColorToObj3D>();
            for (int x = 0; x < colorPalette.width; x++)
                colorArray_3D.Add(new ColorToObj3D
                {
                    color = colorPalette.GetPixel(x, 0),
                    colorObjs = null
                });
        }
    }
}

[System.Serializable]
public class PixelLevelMap
{
    public Texture2D map;
    public string layerName;
}

[System.Serializable]
public class ColorToObj2D
{
    public Color color;
    public Color2DData[] colorObjs;
}

[System.Serializable]
public class ColorToObj3D
{
    public Color color;
    public Color3DData[] colorObjs;
}

[System.Serializable]
public class Color2DData
{
    public Sprite sprite;
    public int proportion;
}

[System.Serializable]
public class Color3DData
{
    public Mesh mesh;
    public Material material;
    public int proportion;
}

#if UNITY_EDITOR
[CustomEditor(typeof(SO_LevelColorData))]
public class LevelTypeEditor : Editor
{
    private SerializedProperty boolProp;
    private SerializedProperty paletteProp;
    private SerializedProperty mapsProp;
    private SerializedProperty color2DProp;
    private SerializedProperty color3DProp;

    private void OnEnable()
    {
        boolProp = serializedObject.FindProperty("is2DLevel");
        paletteProp = serializedObject.FindProperty("colorPalette");
        mapsProp = serializedObject.FindProperty("maps");
        color2DProp = serializedObject.FindProperty("colorArray_2D");
        color3DProp = serializedObject.FindProperty("colorArray_3D");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(boolProp);
        EditorGUILayout.PropertyField(paletteProp);
        EditorGUILayout.PropertyField(mapsProp);
        if (boolProp.boolValue) EditorGUILayout.PropertyField(color2DProp);
        else EditorGUILayout.PropertyField(color3DProp);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
