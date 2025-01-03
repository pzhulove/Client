using UnityEngine;
using System.Collections.Generic;
using System;

public enum SpriteAssetColor
{
    SAC_WHITE = 0,
    SAC_BLUE,
    SAC_PURPLE,
    SAC_PINK,
    SAC_ORANGE,
    SAC_GREEN,
    SAC_OTHER_NAME,
    SAC_SELF_NAME,
    SAC_WRAP_STONE_RED,
    SAC_WRAP_STONE_GREEN,
    SAC_WRAP_STONE_YELLOW,
    SAC_WRAP_STONE_BLUE,
    SAC_WRAP_STONE_BLACK,
    SAC_PINK_RED,       //红
    SAC_COUNT,
}

[Serializable]
public class SpriteAssetInfor
{
    /// <summary>  
    /// ID  
    /// </summary>  
    public int ID;
    /// <summary>  
    /// 名称  
    /// </summary>  
    public string name;
    /// <summary>  
    /// 中心点  
    /// </summary>  
    public Vector2 pivot;
    /// <summary>  
    ///坐标&宽高  
    /// </summary>  
    public Rect rect;
    /// <summary>  
    /// 精灵  
    /// </summary>  
    public Sprite sprite;

}

public class SpriteAsset : ScriptableObject
{
    /// <summary>  
    /// 图片资源  
    /// </summary>  
    public Texture texSource;
    /// <summary>  
    /// 所有sprite信息 SpriteAssetInfor类为具体的信息类  
    /// </summary>  
    public List<SpriteAssetInfor> listSpriteAssetInfor;
}