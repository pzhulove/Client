using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StarryScrollGUI : ShaderGUI
{
    //Texture
    MaterialProperty _StarryMap;
    MaterialProperty _Bump;
    MaterialProperty _Noise;
    MaterialProperty _ScrollUV;
    MaterialProperty _AlbedoCol;
    MaterialProperty _Distortion;
    MaterialProperty _StarryInt;
    MaterialProperty _NoiseInt;
    MaterialProperty _EmissionColor;
    MaterialProperty _Emission;
    MaterialProperty _NormalStyle;
    MaterialProperty _FresnelOffset;
    MaterialProperty _FresnelBase;
    MaterialProperty _FresnelScale;
    MaterialProperty _FresnelBase2;
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
        _StarryMap = FindProperty("_StarryMap", props);
        _Bump = FindProperty("_Bump", props);
        _Noise = FindProperty("_Noise", props);
        _ScrollUV = FindProperty("_ScrollUV", props);
        _AlbedoCol = FindProperty("_AlbedoCol", props);
        _Distortion = FindProperty("_Distortion", props);
        _StarryInt = FindProperty("_StarryInt", props);
        _NoiseInt = FindProperty("_NoiseInt", props);
        _EmissionColor = FindProperty("_EmissionColor", props);
        _Emission = FindProperty("_Emission", props);
        _NormalStyle = FindProperty("_NormalStyle", props);
        _FresnelOffset = FindProperty("_FresnelOffset", props);
        _FresnelBase = FindProperty("_FresnelBase", props);
        _FresnelScale = FindProperty("_FresnelScale", props);
        _FresnelBase2 = FindProperty("_FresnelBase2", props);
    }

    public void ShaderPropertiesGUI(Material material)
    {
        EditorGUIUtility.labelWidth = 0;
        GUILayout.Space(4);
        EditorGUILayout.BeginVertical("Box");

        m_MaterialEditor.ShaderProperty(_StarryMap, "星空贴图");
        m_MaterialEditor.ShaderProperty(_Bump, "法线贴图");
        m_MaterialEditor.ShaderProperty(_Noise, "扰动贴图");

        EditorGUILayout.EndVertical();

        //LightPos
        GUILayout.Space(6);
        EditorGUILayout.BeginVertical("Box");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_ScrollUV, "星空及扰动位移速率");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_AlbedoCol, "Fresnel颜色");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_Distortion, "星空扭曲");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_StarryInt, "星空亮度");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_NoiseInt, "扰动亮度");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_EmissionColor, "自发光颜色");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_Emission, "自发光强度");
        GUILayout.Space(4);

        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.BeginVertical();
        m_MaterialEditor.ShaderProperty(_NormalStyle, "法线类型");
        GUILayout.Space(4);
        m_MaterialEditor.ShaderProperty(_FresnelOffset, "Fresnel偏移");

        if (material.IsKeywordEnabled("_NormalStyle"))
        {
            GUILayout.Space(4);
            m_MaterialEditor.ShaderProperty(_FresnelBase, "Fresnel过度");
            GUILayout.Space(4);
            m_MaterialEditor.ShaderProperty(_FresnelScale, "Fresnel大小");

        }
        else
        {
            GUILayout.Space(4);
            m_MaterialEditor.ShaderProperty(_FresnelBase2, "fresnel过度2");
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

    }

}
