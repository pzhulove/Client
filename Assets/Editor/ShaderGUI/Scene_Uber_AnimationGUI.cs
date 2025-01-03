using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Scene_Uber_AnimationGUI : ShaderGUI
{
    MaterialProperty _DyeColor;
    MaterialProperty _Intensity;

    //Glow
    MaterialProperty _EnabledEmissive;
    MaterialProperty _EmissiveMap;
    MaterialProperty _EmissiveColor;
    MaterialProperty _EmissiveIntensity;
    MaterialProperty _Bias;
    MaterialProperty _TimeOnDuration;
    MaterialProperty _TimeOffDuration;
    MaterialProperty _BlinkingTimeOffsScale;
    MaterialProperty _NoiseAmount;

    //Vertex Animation
    MaterialProperty _VertexAnimation;
    MaterialProperty _MotionInvert;
    MaterialProperty _RandOffset;
    MaterialProperty _WindDir;
    MaterialProperty _BendScale;
    MaterialProperty _SwayFreq;

    MaterialEditor m_MaterialEditor;

    bool motionInvert;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        FindProperties(properties);
        m_MaterialEditor = materialEditor;
        Material material = materialEditor.target as Material;

        ShaderPropertiesGUI(material);
    }

    public void FindProperties(MaterialProperty[] props)
    {
        _DyeColor = FindProperty("_DyeColor", props);
        _Intensity = FindProperty("_Intensity", props);

        //Glow
        _EnabledEmissive = FindProperty("_EnabledEmissive", props);
        _EmissiveMap = FindProperty("_EmissiveMap", props);
        _EmissiveColor = FindProperty("_EmissiveColor", props);
        _EmissiveIntensity = FindProperty("_EmissiveIntensity", props);
        _Bias = FindProperty("_Bias", props);
        _TimeOnDuration = FindProperty("_TimeOnDuration", props);
        _TimeOffDuration = FindProperty("_TimeOffDuration", props);
        _BlinkingTimeOffsScale = FindProperty("_BlinkingTimeOffsScale", props);
        _NoiseAmount = FindProperty("_NoiseAmount", props);

        //Vertex Animation
        _VertexAnimation = FindProperty("_VertexAnimation", props);
        _MotionInvert = FindProperty("_MotionInvert", props);
        _RandOffset = FindProperty("_RandOffset", props);
        _WindDir = FindProperty("_WindDir", props);
        _BendScale = FindProperty("_BendScale", props);
        _SwayFreq = FindProperty("_SwayFreq", props);
    }

    public void ShaderPropertiesGUI(Material material)
    {
        EditorGUIUtility.labelWidth = 0;

        m_MaterialEditor.ShaderProperty(_DyeColor, "Main Color（主颜色）");
        m_MaterialEditor.ShaderProperty(_Intensity, "Intensity（主强度）");

        //Glow
        GUILayout.Space(10);
        EditorGUILayout.BeginVertical("box");
        m_MaterialEditor.ShaderProperty(_EnabledEmissive, "Emissive On/Off（自发光开关）");
        if (material.IsKeywordEnabled("EMISSIVE_TEXTURE"))
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(30);
            EditorGUILayout.BeginVertical();
            m_MaterialEditor.ShaderProperty(_EmissiveMap, "Emissive Texture（发光图）");
            m_MaterialEditor.ShaderProperty(_EmissiveColor, "Emissive Color（发光颜色）");
            m_MaterialEditor.ShaderProperty(_EmissiveIntensity, "Emissive Intensity（发光强度）");
            m_MaterialEditor.ShaderProperty(_Bias, "Bias（闪烁时波形Y向偏移）");
            m_MaterialEditor.ShaderProperty(_TimeOnDuration, "ON Duration（闪烁时亮着的时间）");
            m_MaterialEditor.ShaderProperty(_TimeOffDuration, "OFF Duration（闪烁时暗着的时间）");
            m_MaterialEditor.ShaderProperty(_BlinkingTimeOffsScale, "Blinking Time（闪烁的开始位置）");
            m_MaterialEditor.ShaderProperty(_NoiseAmount, "Noise Amount（闪烁时噪声的程度）");

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        //Vertex Animation
        GUILayout.Space(10);
        EditorGUILayout.BeginVertical("box");
        m_MaterialEditor.ShaderProperty(_VertexAnimation, "VertexAnimation On/Off（顶点动画开关）");
        if (material.IsKeywordEnabled("VERTEX_ANIMATION"))
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(30);
            EditorGUILayout.BeginVertical();
            //m_MaterialEditor.ShaderProperty(_MotionInvert, "Motion Invert（运动反向开关）");
            motionInvert = EditorGUILayout.Toggle("Motion Invert（运动反向开关）", material.GetFloat("_MotionInvert") == -1);
            material.SetFloat("_MotionInvert", motionInvert ? -1f : 1f);
            m_MaterialEditor.ShaderProperty(_RandOffset, "Random Offset（随机偏移）");
            m_MaterialEditor.ShaderProperty(_WindDir, "Wind Direction（摆动方向）");
            m_MaterialEditor.ShaderProperty(_BendScale, "Bend Scale（摆动强度）");
            m_MaterialEditor.ShaderProperty(_SwayFreq, "Sway Freq（摆动频率）");

            material.SetOverrideTag("DisableBatching", "true");
            material.SetOverrideTag("Queue", "Opaque");

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            material.SetOverrideTag("DisableBatching", "false");
            material.SetOverrideTag("Queue", "Opaque");
        }
        EditorGUILayout.EndVertical();
    }
}
