using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shader类型
/// </summary>
public enum MatAnimatShaderType
{
    General,
    Outline
}

/// <summary>
/// 着色模型
/// </summary>
public enum MatAnimatShadeModel
{
    PBR,
    Cel,
    Dissolusion,
    Hair,
    SimulatePBR,
    Simple
}

[Serializable]
public class DMatAnimatFloatParam
{
    public string name;
    public float value;
}

[Serializable]
public class DMatAnimatVectorParam
{
    public string name;
    public Vector4 value;
}

[Serializable]
public class DMatAnimatColorParam
{
    public string name;
    public Color value;
}

[Serializable]
public class DMatAnimatTextureParam
{
    public string name;
    public string value;
}

[Serializable]
public class DMatAnimatData
{
    public string m_Name;
    public bool m_IsOpaque = true;
    public MatAnimatShaderType m_ShaderType;
    public List<string> m_keywords;
    public List<DMatAnimatFloatParam> m_FloatParams;
    public List<DMatAnimatVectorParam> m_VectorParams;
    public List<DMatAnimatColorParam> m_ColorParams;
    public List<DMatAnimatTextureParam> m_TextureParams;
}

public class DMatAnimatConfig : ScriptableObject
{
    public List<DMatAnimatData> m_Datas;
}
