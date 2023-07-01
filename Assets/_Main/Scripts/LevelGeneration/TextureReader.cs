using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TextureReader : MonoBehaviour
{
    public SO_LevelColorData colorData;
    public int textureWidth;

    #region Generate 2D Level
    [ContextMenu("Generate 2D Level")]
    public void Generate2DLevel()
    {
        for (int i = 0; i < colorData.maps.Length; i++)
        {
            Transform newLevelParent = new GameObject($"Level Parent{i + 1}").transform;
            for (int x = 0; x < colorData.maps[i].map.width; x++)
                for (int y = 0; y < colorData.maps[i].map.height; y++)
                {
                    Transform newTrans = GenerateTile(x, y, colorData.maps[i]);
                    if (newTrans != null) newTrans.SetParent(newLevelParent);
                }
        }
    }

    Transform GenerateTile(int x, int y, PixelLevelMap targetMap)
    {
        Color pixelColor = targetMap.map.GetPixel(x, y);
        float ratio = textureWidth / 100f;
        if (pixelColor.a == 0) return null;
        else foreach (ColorToObj2D colorObj in colorData.colorArray_2D)
            {
                if (colorObj.color.Equals(pixelColor))
                {
                    Vector2 pos = new Vector2(x * ratio, y * ratio);
                    GameObject go = new GameObject($"Tile{x}{y}");
                    go.transform.position = pos;
                    go.AddComponent<SpriteRenderer>().sprite = GetTileSprite(colorObj.colorObjs);
                    go.GetComponent<SpriteRenderer>().sortingLayerName = targetMap.layerName;
                    return go.transform;
                }
                else continue;
            }
        return null;
    }

    Sprite GetTileSprite(Color2DData[] targetList)
    {
        if (targetList.Length == 0)
        {
            Debug.LogError("Sprite Missed");
            return null;
        }
        else
        {
            if (targetList.Length == 1) return targetList[0].sprite;
            else
            {
                int total = 0;
                for (int i = 0; i < targetList.Length; i++)
                    total += targetList[i].proportion;
                int random = Random.Range(0, total+1);

                for (int i = 0; i < targetList.Length; i++)
                    if (random <= targetList[i].proportion) return targetList[i].sprite;
                    else random -= targetList[i].proportion;
            }
        }
        return null;
    }
    #endregion

    public void Generate3DLevel()
    {

    }

    [ContextMenu("Save Palette Color")]
    public void SavePaletteColor()
    {
        colorData.ColorUpdate();
    }
}

