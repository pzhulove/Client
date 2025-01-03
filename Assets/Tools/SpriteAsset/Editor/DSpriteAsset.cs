using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


[System.Serializable]
public class DSpriteNode
{
    public Sprite sprite;
    public List<GameObject> refsPrefabs = new List<GameObject>();
}

public class DSpriteAsset : ScriptableObject
{
    public string   name;
    public List<Sprite> references = new List<Sprite>();
    public List<DSpriteNode> refs  = new List<DSpriteNode>();
}

