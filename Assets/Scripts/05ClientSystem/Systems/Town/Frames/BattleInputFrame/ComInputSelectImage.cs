using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;
[Serializable]
public struct InputSelectImage
{
    public string name;
    public Sprite sprite;
}
public class ComInputSelectImage : MonoBehaviour
{
    public InputSelectImage[] SelectImages;

    public Sprite GetSprite(string name)
    {
        foreach (var item in SelectImages)
        {
            if (item.name == name || item.name + "(Clone)" == name)
            {
                return item.sprite;
            }
        }

        return null;
    }
}
