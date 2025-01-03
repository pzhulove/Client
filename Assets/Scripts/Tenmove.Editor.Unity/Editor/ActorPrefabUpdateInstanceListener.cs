using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;

namespace Tenmove.Editor.Unity
{
    /// <summary>
    /// 监听Prefab修改保存事件，离线保存角色相关的一些数据
    /// </summary>
    public class ActorPrefabUpdateInstanceListener
    {
        private static bool m_PrefabMode;

        // 脚本重新编译后需要重新注册事件
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            ClearInitializeCallback();
            StartInitializeOnLoadMethod();
        }

        private static void ClearInitializeCallback()
        {
            PrefabUtility.prefabInstanceUpdated -= RefreshPrefabInstance;

            // Prefab模式下编辑保存Prefab，如果场景中没有该Prefab的Instance不会触发上面的回调，会触发下面的回调。
            PrefabStage.prefabSaving -= RefreshPrefab;

            PrefabStage.prefabStageOpened -= PrefabStageOpened;
            PrefabStage.prefabStageClosing -= PrefabStageClosed;
        }

        [InitializeOnLoadMethod]
        static void StartInitializeOnLoadMethod()
        {
            PrefabUtility.prefabInstanceUpdated += RefreshPrefabInstance;

            // Prefab模式下编辑保存Prefab，如果场景中没有该Prefab的Instance不会触发上面的回调，会触发下面的回调。
            PrefabStage.prefabSaving += RefreshPrefab;

            PrefabStage.prefabStageOpened += PrefabStageOpened;
            PrefabStage.prefabStageClosing += PrefabStageClosed;
        }

        static void PrefabStageOpened(PrefabStage stage)
        {
            m_PrefabMode = true;
        }

        static void PrefabStageClosed(PrefabStage stage)
        {
            m_PrefabMode = false;
        }

        static void RefreshPrefabInstance(GameObject instance)
        {
            if (m_PrefabMode)
                return;


            // 更新武器强化保存的数据
            GeAttachPhaseProxy attachPhaseProxy = instance.GetComponent<GeAttachPhaseProxy>();
            if (attachPhaseProxy != null)
            {
                attachPhaseProxy.InitInEditor();

                PrefabUtility.prefabInstanceUpdated -= RefreshPrefabInstance;
                string prefabPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(instance));
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                PrefabUtility.prefabInstanceUpdated += RefreshPrefabInstance;
            }

            // 更新GeMeshDescProxy
            GeMeshDescProxy[] geMeshDescProxys = instance.GetComponentsInChildren<GeMeshDescProxy>();
            if(geMeshDescProxys != null && geMeshDescProxys.Length > 0)
            {
                foreach(GeMeshDescProxy geMeshDescProxy in geMeshDescProxys)
                {
                    geMeshDescProxy.RebakeAnimat();
                }

                PrefabUtility.prefabInstanceUpdated -= RefreshPrefabInstance;
                string prefabPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(instance));
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                PrefabUtility.prefabInstanceUpdated += RefreshPrefabInstance;
            }
        }

        static void RefreshPrefab(GameObject instance)
        {
            // 更新武器强化保存的数据
            GeAttachPhaseProxy attachPhaseProxy = instance.GetComponent<GeAttachPhaseProxy>();
            if (attachPhaseProxy != null)
            {
                attachPhaseProxy.InitInEditor();
            }

            // 更新GeMeshDescProxy
            GeMeshDescProxy[] geMeshDescProxys = instance.GetComponentsInChildren<GeMeshDescProxy>();
            if (geMeshDescProxys != null && geMeshDescProxys.Length > 0)
            {
                foreach (GeMeshDescProxy geMeshDescProxy in geMeshDescProxys)
                {
                    geMeshDescProxy.RebakeAnimat();
                }
            }
        }
    }
}
