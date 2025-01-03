using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SimplePBR_GUI : ShaderGUI
{
    //Texture
    MaterialProperty _Albedo;
    MaterialProperty _Bump;
    MaterialProperty _Param;
    MaterialProperty _LitSphere;
    MaterialProperty _Mask;

    //LightPos
    MaterialProperty _LightPos;
    MaterialProperty _LightIntensity;
    MaterialProperty _ShadowColor;

    //SpecEffect
    MaterialProperty _BaseColor;
    MaterialProperty _Metalness;
    MaterialProperty _Roughness;

    MaterialProperty _DirLightColor;
    MaterialProperty _SpecIntensity;
    MaterialProperty _ClothSpec;

    //SkinParam
    MaterialProperty _SSSSkin;
    MaterialProperty _FresnelTrans;
    MaterialProperty _FresnelColor;
    MaterialProperty _SkinBright;
    MaterialProperty _SkinSpec;

    //EnvLight
    MaterialProperty _EnvReflect;
    MaterialProperty _EnvExposure;
    MaterialProperty _EnvRotation;

    //Emissive
    MaterialProperty _Emissive;
    MaterialProperty _EmissiveColor;
    MaterialProperty _EmissiveExposure;
    MaterialProperty _Blinking;
    MaterialProperty _TimeOnDuration;
    MaterialProperty _TimeOffDuration;
    MaterialProperty _BlinkingTimeOffsScale;
    MaterialProperty _NoiseAmount;

    //ClolorStyle
    MaterialProperty _ClolorStyle;
    MaterialProperty _ColorSelect;

    //SelfShadow
    MaterialProperty _SelfShadowColor;
    MaterialProperty _SelfShadowBias;
    MaterialProperty _SelfShadowIntensity;

    MaterialEditor m_MaterialEditor;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        FindProperties(properties);

        m_MaterialEditor = materialEditor;
        Material material = materialEditor.target as Material;

        ShaderPropertiesGUI(material);
    }

    public void FindProperties(MaterialProperty[] props)
    {
        _Albedo = FindProperty("_Albedo", props);
        _Bump = FindProperty("_Bump", props);
        _Param = FindProperty("_Param", props);
        _LitSphere = FindProperty("_LitSphere", props);
        _Mask = FindProperty("_Mask", props);

        //LightPos
        _LightPos = FindProperty("_LightPos", props);
        _LightIntensity = FindProperty("_LightIntensity", props);
        _ShadowColor = FindProperty("_ShadowColor", props);

        //SpecEffect
        _BaseColor = FindProperty("_BaseColor", props);
        _Metalness = FindProperty("_Metalness", props);
        _Roughness = FindProperty("_Roughness", props);

        _DirLightColor = FindProperty("_DirLightColor", props);
        _SpecIntensity = FindProperty("_SpecIntensity", props);
        _ClothSpec = FindProperty("_ClothSpec", props);

        //SkinParam
        _SSSSkin = FindProperty("_SSSSkin", props);
        _FresnelTrans = FindProperty("_FresnelTrans", props);
        _FresnelColor = FindProperty("_FresnelColor", props);
        _SkinBright = FindProperty("_SkinBright", props);
        _SkinSpec = FindProperty("_SkinSpec", props);

        //EnvLight
        _EnvReflect = FindProperty("_EnvReflect", props);
        _EnvExposure = FindProperty("_EnvExposure", props);
        _EnvRotation = FindProperty("_EnvRotation", props);

        //Emissive
        _Emissive = FindProperty("_Emissive", props);
        _EmissiveColor = FindProperty("_EmissiveColor", props);
        _EmissiveExposure = FindProperty("_EmissiveExposure", props);
        _Blinking = FindProperty("_Blinking", props);
        _TimeOnDuration = FindProperty("_TimeOnDuration", props);
        _TimeOffDuration = FindProperty("_TimeOffDuration", props);
        _BlinkingTimeOffsScale = FindProperty("_BlinkingTimeOffsScale", props);
        _NoiseAmount = FindProperty("_NoiseAmount", props);

        //ClolorStyle
        _ClolorStyle = FindProperty("_ClolorStyle", props);
        _ColorSelect = FindProperty("_ColorSelect", props);

        //SelfShadow
        _SelfShadowColor = FindProperty("_SelfShadowColor", props);
        _SelfShadowBias = FindProperty("_SelfShadowBias", props);
        _SelfShadowIntensity = FindProperty("_SelfShadowIntensity", props);
    }

    public void ShaderPropertiesGUI(Material material)
    {
        EditorGUIUtility.labelWidth = 0;
        GUILayout.Space(4);
        EditorGUILayout.BeginVertical("Box");

        m_MaterialEditor.ShaderProperty(_Albedo, "主纹理贴图");
        m_MaterialEditor.ShaderProperty(_Bump, "法线贴图");
        m_MaterialEditor.ShaderProperty(_Param, "高光贴图");
        m_MaterialEditor.ShaderProperty(_LitSphere, "反射环境图");
        m_MaterialEditor.ShaderProperty(_Mask, "遮罩贴图");

        EditorGUILayout.EndVertical();

        //LightPos
        GUILayout.Space(6);
        EditorGUILayout.BeginVertical("Box");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_LightPos, "灯光方向及强度");
        //GUILayout.Space(4);

        m_MaterialEditor.ShaderProperty(_LightIntensity, "灯光强度");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_ShadowColor, "阴影颜色");

        GUILayout.Space(4);
        EditorGUILayout.EndVertical();

        //SpecEffect
        GUILayout.Space(6);
        EditorGUILayout.BeginVertical("Box");

        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_BaseColor, "主纹理颜色");
        GUILayout.Space(4);

        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_Metalness, "金属度");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_Roughness, "粗糙度");
        //GUILayout.Space(4);

        GUILayout.Space(4);

        EditorGUILayout.BeginHorizontal("Box");
        //GUILayout.Space(30);
        EditorGUILayout.BeginVertical();
        GUILayout.Space(4);

        m_MaterialEditor.ShaderProperty(_DirLightColor, "高光颜色");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_SpecIntensity, "高光强度");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_ClothSpec, "衣服高光强度");
        GUILayout.Space(4);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();


        //SkinParam
        GUILayout.Space(6);
        EditorGUILayout.BeginVertical("Box");

        m_MaterialEditor.ShaderProperty(_SSSSkin, "是否使用真实皮肤效果");
        if (material.IsKeywordEnabled("_SSSSkin"))
        {
            GUILayout.Space(4);
            m_MaterialEditor.ShaderProperty(_FresnelTrans, "皮肤Fresnel偏移");
            GUILayout.Space(4);
            m_MaterialEditor.ShaderProperty(_FresnelColor, "皮肤半透颜色");
        }
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_SkinBright, "皮肤灯光强度");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_SkinSpec, "皮肤高光强度");
        GUILayout.Space(4);

        EditorGUILayout.EndVertical();


        //EnvLight
        GUILayout.Space(6);
        EditorGUILayout.BeginVertical("Box");
        //GUILayout.Space(4);

        m_MaterialEditor.ShaderProperty(_EnvReflect, "是否使用环境反射");
        if (material.IsKeywordEnabled("_EnvReflect"))
        {
            GUILayout.Space(4);
            m_MaterialEditor.ShaderProperty(_EnvExposure, "环境反射强度");
            GUILayout.Space(4);
            m_MaterialEditor.ShaderProperty(_EnvRotation, "旋转环境反射图");
            GUILayout.Space(4);
        }

        //GUILayout.Space(4);
        EditorGUILayout.EndVertical();


        //_Emissive
        GUILayout.Space(6);
        EditorGUILayout.BeginVertical("Box");
        m_MaterialEditor.ShaderProperty(_Emissive, "是否使用自发光");
        if (material.IsKeywordEnabled("_Emissive"))
        {

            GUILayout.Space(4);
            m_MaterialEditor.ShaderProperty(_EmissiveColor, "自发光颜色");
            GUILayout.Space(4);
            m_MaterialEditor.ShaderProperty(_EmissiveExposure, "自发光强度");
            GUILayout.Space(4);

            EditorGUILayout.BeginHorizontal("Box");
            //GUILayout.Space(30);
            EditorGUILayout.BeginVertical();
            m_MaterialEditor.ShaderProperty(_Blinking, "是否使用闪烁");
            if (material.IsKeywordEnabled("_Blinking"))
            {
                GUILayout.Space(4);
                m_MaterialEditor.ShaderProperty(_TimeOnDuration, "闪烁时亮着的时间");
                GUILayout.Space(4);
                m_MaterialEditor.ShaderProperty(_TimeOffDuration, "闪烁时暗着的时间");
                GUILayout.Space(4);
                m_MaterialEditor.ShaderProperty(_BlinkingTimeOffsScale, "闪烁的开始位置");
                GUILayout.Space(4);
                m_MaterialEditor.ShaderProperty(_NoiseAmount, "闪烁时噪声的程度");

                GUILayout.Space(4);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();


        //ClolorStyle
        GUILayout.Space(6);
        EditorGUILayout.BeginVertical("Box");
        m_MaterialEditor.ShaderProperty(_ClolorStyle, "颜色类型1");
        if (material.IsKeywordEnabled("_ClolorStyle"))
        {

            GUILayout.Space(4);
            m_MaterialEditor.ShaderProperty(_ColorSelect, "颜色类型选择");
            GUILayout.Space(4);

        }
        EditorGUILayout.EndVertical();

        //SelfShadow
        GUILayout.Space(6);
        EditorGUILayout.BeginVertical("Box");
        m_MaterialEditor.ShaderProperty(_SelfShadowColor, "自阴影颜色");
        m_MaterialEditor.ShaderProperty(_SelfShadowBias, "自阴影偏移值");
        m_MaterialEditor.ShaderProperty(_SelfShadowIntensity, "自阴影强度");
        EditorGUILayout.EndVertical();
    }

}
