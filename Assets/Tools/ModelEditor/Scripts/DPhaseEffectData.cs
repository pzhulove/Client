using UnityEngine;
using System.Collections;

[System.Serializable]
public struct DEffectParamDesc
{
}

[System.Serializable]
public struct DPhaseStageEffectChunk
{
    public string effectResPath;
    public Color effectColor;
}


[System.Serializable]
public struct DPhaseStageParamChunk
{
    public DAnimatParamDesc[] paramDesc;            /// 材质效果数据列表
    public DPhaseStageEffectChunk[] effectDesc;
    public bool needGlow;
    public Color glowColor;
}


[System.Serializable]
public struct DPhaseEffChunk
{
    public string name;                             /// 材质效果名
    public string shaderName;                       /// 材质效果使用的Shader名

    public DPhaseStageParamChunk[] phaseStageChunk; /// 
}


public class DPhaseEffectData : ScriptableObject
{
    public string[] phaseMatNameList;

    public DPhaseEffChunk[] phaseMatChunk;
}
