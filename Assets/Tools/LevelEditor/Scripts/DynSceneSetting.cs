using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class DynLightmapData
{
    public Texture2D lightmapFar;
    public Texture2D lightmapNear;
}


[System.Serializable]
public class DRenderSettings
{
    public bool fog;
    public FogMode fogMode;
    public Color fogColor;
    public float fogDensity;
    public float fogStartDistance;
    public float fogEndDistance;
    public UnityEngine.Rendering.AmbientMode ambientMode;
    public Color ambientSkyColor;
    public Color ambientEquatorColor;
    public Color ambientGroundColor;
    public Color ambientLight;
    public float ambientIntensity;
}


public class DynSceneSetting : ScriptableObject
{
    public DynLightmapData[] lightmaps = new DynLightmapData[0];
    public LightmapsMode lightmapsMode;
    public LightProbes lightProbes;
    public DRenderSettings renderSetting = new DRenderSettings();

#if UNITY_EDITOR

    public void Snap()
    {
        List<DynLightmapData> lsList = new List<DynLightmapData>();
        var ls = UnityEngine.LightmapSettings.lightmaps;
        for (int i = 0; i < ls.Length; ++i)
        {
            DynLightmapData data = new DynLightmapData();
			data.lightmapNear = ls[i].lightmapDir;
			data.lightmapFar = ls[i].lightmapColor;
            lsList.Add(data);
        }

        lightmaps = lsList.ToArray();

        lightmapsMode = UnityEngine.LightmapSettings.lightmapsMode;
        lightProbes = UnityEngine.LightmapSettings.lightProbes;

        EditorUtility.SetDirty(this);

        GameObject[] sceneRoots = GameObject.FindGameObjectsWithTag("Scene");

        for (int i = 0; i < sceneRoots.Length; ++i)
        {
            var root = sceneRoots[i];

            //Clear GameObjectSettingSerialize
            GameObjectSettingSerialize[] gss = root.GetComponentsInChildren<GameObjectSettingSerialize>();
            for (int g = 0; g < gss.Length; g++)
            {
                GameObject.DestroyImmediate(gss[g]);
            }

            //Reset GameObjectSettingSerialize
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

            for (int j = 0; j < renderers.Length; ++j)
            {
                var render = renderers[j];

                if (render.lightmapIndex >= 0)
                {
                    GameObjectSettingSerialize setting = render.gameObject.AddComponent<GameObjectSettingSerialize>();
                    setting.SnapSetting();
                }
            }


            PrefabTools.ApplyPrefab(root);
        }

        renderSetting.fog = RenderSettings.fog;
        renderSetting.fogMode = RenderSettings.fogMode;
        renderSetting.fogColor = RenderSettings.fogColor;
        renderSetting.fogDensity = RenderSettings.fogDensity;
        renderSetting.fogStartDistance = RenderSettings.fogStartDistance;
        renderSetting.fogEndDistance = RenderSettings.fogEndDistance;
        renderSetting.ambientMode = RenderSettings.ambientMode;
        renderSetting.ambientSkyColor = RenderSettings.ambientSkyColor;
        renderSetting.ambientEquatorColor = RenderSettings.ambientEquatorColor;
        renderSetting.ambientGroundColor = RenderSettings.ambientGroundColor;
        renderSetting.ambientLight = RenderSettings.ambientLight;
        renderSetting.ambientIntensity = RenderSettings.ambientIntensity;

    }

#endif

    public void Apply()
    {
        List<LightmapData> lsList = new List<LightmapData>();

        if (lightmaps != null)
        {
            for (int i = 0; i < lightmaps.Length; ++i)
            {
                LightmapData data = new LightmapData();
				data.lightmapDir = lightmaps[i].lightmapNear;
				data.lightmapColor = lightmaps[i].lightmapFar;

              //  Logger.LogFormat("{0} --  {1} set Light Texture\n", data.lightmapNear, data.lightmapFar);

                lsList.Add(data);
            }
        }

        UnityEngine.LightmapSettings.lightmaps = lsList.ToArray();

        UnityEngine.LightmapSettings.lightmapsMode = lightmapsMode;
        UnityEngine.LightmapSettings.lightProbes = lightProbes;

//         RenderSettings.fog = renderSetting.fog;
//         RenderSettings.fogMode = renderSetting.fogMode;
//         RenderSettings.fogColor = renderSetting.fogColor;
//         RenderSettings.fogDensity = renderSetting.fogDensity;
//         RenderSettings.fogStartDistance = renderSetting.fogStartDistance;
//         RenderSettings.fogEndDistance = renderSetting.fogEndDistance;
//         RenderSettings.ambientMode = renderSetting.ambientMode;
//         RenderSettings.ambientSkyColor = renderSetting.ambientSkyColor;
//         RenderSettings.ambientEquatorColor = renderSetting.ambientEquatorColor;
//         RenderSettings.ambientGroundColor = renderSetting.ambientGroundColor;
//         RenderSettings.ambientLight = renderSetting.ambientLight;
//         RenderSettings.ambientIntensity = renderSetting.ambientIntensity;
    }
}