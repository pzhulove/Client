using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Tenmove.Runtime.Unity;
using System;
using System.Reflection;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;

namespace Tenmove.Editor.Unity
{
    public class EffectLightEditor : EditorWindow
    {
        const string CONST_LIGHT_POS = "_EffectPointLightPos";
        const string CONST_LIGHT_COLOR = "_EffectPointLightColor";
        const string CONST_LIGHT_INTENSITY = "_EffectPointLightIntensity";

        private GameObject m_Effect;
        private TMEffectLightComponent m_LightComponent;

        private Light m_FakeLight;

        [MenuItem("[TM工具集]/特效工具/特效光照配置")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow<EffectLightEditor>("特效光照配置");
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.update += _UpdateMaterialParam;
        }

        private void OnDisable()
        {
            EditorApplication.update -= _UpdateMaterialParam;
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            {
                m_Effect = EditorGUILayout.ObjectField("特效根节点：", m_Effect, typeof(GameObject), true) as GameObject;
                if(GUILayout.Button("刷新光照物体"))
                {
                    _RefreshLightComponent(m_Effect, ref m_LightComponent);
                }
            }
            if(EditorGUI.EndChangeCheck())
            {
                _RefreshLightComponent(m_Effect, ref m_LightComponent);
            }

            // 显示当前光照物体
            if(m_LightComponent != null)
            {
                Renderer[] lightRenderers = _GetLightRenderers(m_LightComponent);
                foreach(Renderer renderer in lightRenderers)
                {
                    EditorGUILayout.ObjectField(renderer.gameObject, typeof(GameObject), true);
                }
            }

            if(GUILayout.Button("创建光源"))
            {
                if(m_LightComponent != null)
                {
                    m_FakeLight = _CreateLight(m_Effect.transform, m_LightComponent);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "请选择特效根节点", "ok");
                }
            }

            if(GUILayout.Button("保存"))
            {
                _Save(m_Effect, ref m_LightComponent, m_FakeLight);
            }
        }

        /// <summary>
        /// 根据材质中光源的颜色、强度、位置参数创建光源，材质中存储的光源位置是模型空间中的坐标，需要根据物体的模型变换矩阵变换到世界空间
        /// </summary>
        /// <returns></returns>
        private Light _CreateLight(Transform parent, TMEffectLightComponent lightComponent)
        {
            if (m_FakeLight != null)
                DestroyImmediate(m_FakeLight.gameObject);

            GameObject fakeLightGO = new GameObject("PointLight");
            fakeLightGO.transform.SetParent(parent, false);
            // 使创建的光源不会被保存
            fakeLightGO.hideFlags = HideFlags.DontSave;
            Light pointLight = fakeLightGO.AddComponent<Light>();
            pointLight.type = LightType.Point;

            Renderer[] lightRenderers = _GetLightRenderers(lightComponent);
            if(lightRenderers != null && lightRenderers.Length > 0)
            {
                Renderer lightRenderer = lightRenderers[0];
                Material lightMaterial = lightRenderer.sharedMaterial;
                pointLight.color = lightMaterial.GetColor(CONST_LIGHT_COLOR);
                pointLight.intensity = lightMaterial.GetFloat(CONST_LIGHT_INTENSITY);
            }

            Vector3 lightLocalPos = _GetLightLocalPos(lightComponent);
            pointLight.transform.localPosition = lightLocalPos;

            return pointLight;
        }

        private void _UpdateMaterialParam()
        {
            if(m_LightComponent != null)
            {
                Renderer[] lightRenderers = _GetLightRenderers(m_LightComponent);

                if (lightRenderers != null && m_FakeLight != null)
                {
                    foreach (Renderer lightRenderer in lightRenderers)
                    {
                        Material material = lightRenderer.sharedMaterial;
                        material.SetVector(CONST_LIGHT_POS, m_FakeLight.transform.position);
                        material.SetColor(CONST_LIGHT_COLOR, m_FakeLight.color);
                        material.SetFloat(CONST_LIGHT_INTENSITY, m_FakeLight.intensity);
                    }
                }
            }
        }


        private void _RefreshLightComponent(GameObject effect, ref TMEffectLightComponent lightComponent)
        {
            lightComponent = effect.GetComponent<TMEffectLightComponent>();

            Renderer[] renderers = effect.GetComponentsInChildren<Renderer>();
            List<Renderer> lightRenderers = new List<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                Material material = renderer.sharedMaterial;
                if (material != null)
                {
                    if (material.HasProperty(CONST_LIGHT_POS) && material.HasProperty(CONST_LIGHT_COLOR) && material.HasProperty(CONST_LIGHT_INTENSITY))
                    {
                        lightRenderers.Add(renderer);
                    }
                }
            }


            if (lightComponent != null)
            {
                if (lightRenderers.Count > 0)
                    _SetLightRenderers(lightComponent, lightRenderers.ToArray());
                else
                {
                    Destroy(lightComponent);
                    lightComponent = null;
                }
            }
            else
            {
                if (lightRenderers.Count > 0)
                {
                    lightComponent = effect.AddComponent<TMEffectLightComponent>();
                    _SetLightRenderers(lightComponent, lightRenderers.ToArray());
                }
            }
        }

        private void _Save(GameObject effect, ref TMEffectLightComponent lightComponent, Light light)
        {
            if(effect != null && light != null)
            {
                _RefreshLightComponent(effect, ref lightComponent);
                Renderer[] lightRenderers = _GetLightRenderers(lightComponent);
                if(lightRenderers.Length > 0)
                {
                    foreach(Renderer renderer in lightRenderers)
                    {
                        Material material = renderer.sharedMaterial;
                        material.SetColor(CONST_LIGHT_COLOR, light.color);
                        material.SetFloat(CONST_LIGHT_INTENSITY, light.intensity);
                        EditorUtility.SetDirty(material);
                    }

                    _SetLightLocalPos(lightComponent, lightComponent.transform.worldToLocalMatrix.MultiplyPoint(light.transform.position));

                    string path = string.Empty;

                    var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                    if (prefabStage != null)
                    {
                        path = prefabStage.prefabAssetPath;
                        prefabStage.ClearDirtiness();
                    }
                    AssetDatabase.SaveAssets();

                    GameObject effectPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(effect);
                    if (effectPrefab != null)
                        path = AssetDatabase.GetAssetPath(effectPrefab);

                    if(string.IsNullOrEmpty(path))
                    {
                        EditorUtility.DisplayDialog("Error", "Prefab保存失败", "ok");
                    }

                    PrefabUtility.SaveAsPrefabAsset(effect, path);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "LightComponent or light is null", "ok");
            }
        }

        private Renderer[] _GetLightRenderers(TMEffectLightComponent lightComponent)
        {
            Type lightComponentType = typeof(TMEffectLightComponent);
            FieldInfo lightRenderersFiled = lightComponentType.GetField("m_LightRenderers", BindingFlags.NonPublic | BindingFlags.Instance);
            return lightRenderersFiled.GetValue(lightComponent) as Renderer[];
        }

        private void _SetLightRenderers(TMEffectLightComponent lightComponent, Renderer[] lightRenderers)
        {
            Type lightComponentType = typeof(TMEffectLightComponent);
            FieldInfo lightRenderersFiled = lightComponentType.GetField("m_LightRenderers", BindingFlags.NonPublic | BindingFlags.Instance);
            lightRenderersFiled.SetValue(lightComponent, lightRenderers);
        }

        private Vector3 _GetLightLocalPos(TMEffectLightComponent lightComponent)
        {
            Type lightComponentType = typeof(TMEffectLightComponent);
            FieldInfo lightLocalPosFiled = lightComponentType.GetField("m_LightLocalPos", BindingFlags.NonPublic | BindingFlags.Instance);
            return (Vector3)lightLocalPosFiled.GetValue(lightComponent);
        }

        private void _SetLightLocalPos(TMEffectLightComponent lightComponent, Vector3 lightLocalPos)
        {
            Type lightComponentType = typeof(TMEffectLightComponent);
            FieldInfo lightLocalPosFiled = lightComponentType.GetField("m_LightLocalPos", BindingFlags.NonPublic | BindingFlags.Instance);
            lightLocalPosFiled.SetValue(lightComponent, lightLocalPos);
        }
    }
}
