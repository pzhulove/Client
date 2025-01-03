using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public enum EModelPartChannel
{
    eModelHead      , /// 头部
    eModelUpperPart , /// 上半身
    eModelLowerPart , /// 下半身
    eModelShoulder  , /// 肩膀
    eModelWings     , /// 翅膀
    
    eModelWholeBody , /// 整体

    eMaxChannelNum  ,
}


[System.Serializable]
public struct DModelPartChunk
{
    public DAssetObject partAsset;
    public EModelPartChannel partChannel;
};

public enum EModelAttachment
{
    eAttachLeftHand     ,
    eAttachRightHand    ,
    eAttachLeftChest    ,
    eAttachLeftOverHead ,

    eMaxAttachNum       ,
}

[System.Serializable]
public struct DModelAttachment
{
    public DAssetObject attahcmentAsset;
}

[System.Serializable]
public struct DModelAttachmentChunk
{
    public DModelAttachment[] attachments;
};

[System.Serializable]
public struct DModelAnimClipChunk
{
};

[System.Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct DAnimatParamData
{
    [FieldOffset(0)]
    public float _float;
    [FieldOffset(0)]
    public Color _color;
    [FieldOffset(0)]
    public Vector4 _vec4;
}

[System.Serializable]
public struct DAnimatParamObj
{
    public DAssetObject _texAsset;
}

public enum AnimatParamType
{
    Color = 0,
    Vector = 1,
    Float = 2,
    Range = 3,
    TexEnv = 4
}

[System.Serializable]
public struct DAnimatParamDesc
{
    public string paramName;            /// 参数名
    public DAnimatParamData paramData;  /// 数据型参数数据
    public DAnimatParamObj paramObj;    /// 对象性参数数据
    public AnimatParamType paramType;   /// 参数类型枚举
}

[System.Serializable]
public struct DAnimatChunk
{
    public string name;                     /// 材质效果名
    public string shaderName;               /// 材质效果使用的Shader名
    public DAnimatParamDesc[] paramDesc;    /// 材质效果数据列表
}

[System.Serializable]
public struct DBlockChunk
{
    public int gridWidth;
    public int gridHeight;
    public byte[] gridBlockData;
    public Vector3 boundingBoxMin;
    public Vector3 boundingBoxMax;
}
#if LOGIC_SERVER
public class DModelData 
#else
public class DModelData : ScriptableObject
#endif
{
    [System.NonSerialized]
    public static readonly string[] kPartChannelLabel = new string[]
    {
    "头部部件",
    "上身部件",
    "下身部件",
    "肩膀",
    "翅膀",
    "全身",
    "",
    };

    /// <summary>
    ///  TO DO: 后续做智能关联需要
    /// </summary>
    public static readonly string[] kPartChannelName = new string[]
    {
    "Head",
    "Body",
    "Pant",
    "Shoulder",
    "Wings",
    "",
    "",
    };

    public string modelDataName;
    public DAssetObject modelAvatar;
    public Vector3 modelScale = Vector3.one;
    public Vector3 previewLightDir = Vector3.down;
    public Color previewAmbient = Color.black;

    public DModelPartChunk[] partsChunk = new DModelPartChunk[0];
    public DModelAttachmentChunk attachChunk;
    public DModelAnimClipChunk animClipChunk;
    public DAnimatChunk[] animatChunk = new DAnimatChunk[0];
    public DBlockChunk blockGridChunk;
};

