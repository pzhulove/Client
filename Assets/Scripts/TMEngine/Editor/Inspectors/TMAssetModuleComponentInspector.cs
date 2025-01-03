
using System.Reflection;
using UnityEditor;
using Tenmove.Runtime;
using Tenmove.Runtime.Unity;
using Tenmove.Editor.Unity.Widgets;
using System.Collections.Generic;
using UnityEngine;

namespace Tenmove.Editor.Unity
{
    [CustomEditor(typeof(TMAssetModuleComponent))]
    internal sealed class AssetModuleComponentInspector : ComponentInspector
    {
        delegate ObjectDesc[] _GetObjectInfo(TMAssetModuleComponent component);

        private SerializedProperty m_AssetMode = null;
        private SerializedProperty m_ResourceOnlyRootFolder = null;
        UnityEngine.Object m_ResourceOnlyRootFolderObj = null;
        private SerializedProperty m_UseTempPath = null;
        private SerializedProperty m_UnloadUnusedAssetsInterval = null;
        private SerializedProperty m_AssetPoolAutoPurgeInterval = null;
        private SerializedProperty m_AssetExpireTime = null;
        private SerializedProperty m_AssetPoolPriority = null;

        private SerializedProperty m_AssetPackagePoolAutoPurgeInterval = null;
        private SerializedProperty m_AssetPackageExpireTime = null;
        private SerializedProperty m_AssetPackagePoolPriority = null;
        
        private SerializedProperty m_UpdateRetryCount = null;
        
        private SerializedProperty m_AssetLoadAgentCount = null;
        private SerializedProperty m_AssetLoadAgentExtraCount = null;
        
        private TypeDropList<TMResAsyncLoader> m_ResAsyncLoaderType = new TypeDropList<TMResAsyncLoader>();
        private TypeDropList<TMResSyncLoader> m_ResSyncLoaderType = new TypeDropList<TMResSyncLoader>();
        private TypeDropList<AssetByteLoader> m_AssetByteLoaderType = new TypeDropList<AssetByteLoader>();

        private bool m_AssetFoldoutState = false;
        private bool m_AssetPackageFoldoutState = false;

        private string m_ViewDepentIt = string.Empty;
        private string m_ViewItDepent = string.Empty;

        private string m_AssetSearchFilter = string.Empty;
        private string m_PackageSearchFilter = string.Empty;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            TMAssetModuleComponent t = (TMAssetModuleComponent)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EnumHelper<AssetMode> assetMode = new EnumHelper<AssetMode>(t.AssetMode);
                bool isPackage = assetMode.HasFlag(AssetMode.Package);
                bool isUpdatable = assetMode.HasFlag(AssetMode.Updatable);

                EditorGUILayout.BeginVertical();
                isPackage = EditorGUILayout.Toggle("Load From Package", isPackage);
                isUpdatable = EditorGUILayout.Toggle("Updatable", isUpdatable);
                if (!isPackage)
                    isUpdatable = false;
                EditorGUILayout.EndVertical();

                if (!EditorApplication.isPlaying || PrefabUtility.GetPrefabType(t.gameObject) == PrefabType.Prefab)
                {
                    if (isPackage)
                        assetMode.AddFlag(AssetMode.Package);
                    else
                        assetMode.RemoveFlag(AssetMode.Package);

                    if (isUpdatable)
                        assetMode.AddFlag(AssetMode.Updatable);
                    else
                        assetMode.RemoveFlag(AssetMode.Updatable);

                    m_AssetMode.intValue = assetMode;
                }

                m_UseTempPath.boolValue = EditorGUILayout.Toggle("Use temp path", t.UseTempPath);

                m_ResourceOnlyRootFolderObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Runtime.Utility.Path.Combine("Assets/Resources",m_ResourceOnlyRootFolder.stringValue));
                UnityEngine.Object folder = EditorGUILayout.ObjectField("Resource Only Root Folder:", m_ResourceOnlyRootFolderObj, typeof(Object),false);
                if(m_ResourceOnlyRootFolderObj != folder)
                    m_ResourceOnlyRootFolder.stringValue = AssetDatabase.GetAssetPath(folder).Replace("Assets/Resources/", null);
            }
            EditorGUI.EndDisabledGroup();

            float unloadUnusedAssetsInterval = EditorGUILayout.Slider("Unload Unused Assets Interval", m_UnloadUnusedAssetsInterval.floatValue, 0f, 3600f);
            if (unloadUnusedAssetsInterval != m_UnloadUnusedAssetsInterval.floatValue)
            {
                if (EditorApplication.isPlaying)
                {
                    t.UnloadUnusedAssetsInterval = unloadUnusedAssetsInterval;
                }
                else
                {
                    m_UnloadUnusedAssetsInterval.floatValue = unloadUnusedAssetsInterval;
                }
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            {
                float assetAutoReleaseInterval = EditorGUILayout.DelayedFloatField("Asset Auto Release Interval", m_AssetPoolAutoPurgeInterval.floatValue);
                if (assetAutoReleaseInterval != m_AssetPoolAutoPurgeInterval.floatValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetPoolAutoPurgeInterval = assetAutoReleaseInterval;
                    }
                    else
                    {
                        m_AssetPoolAutoPurgeInterval.floatValue = assetAutoReleaseInterval;
                    }
                }

                float assetExpireTime = EditorGUILayout.DelayedFloatField("Asset Expire Time", m_AssetExpireTime.floatValue);
                if (assetExpireTime != m_AssetExpireTime.floatValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetExpireTime = assetExpireTime;
                    }
                    else
                    {
                        m_AssetExpireTime.floatValue = assetExpireTime;
                    }
                }

                int assetPriority = EditorGUILayout.DelayedIntField("Asset Priority", m_AssetPoolPriority.intValue);
                if (assetPriority != m_AssetPoolPriority.intValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetPoolPriority = assetPriority;
                    }
                    else
                    {
                        m_AssetPoolPriority.intValue = assetPriority;
                    }
                }

                float resourceAutoReleaseInterval = EditorGUILayout.DelayedFloatField("AssetPackage Auto Release Interval", m_AssetPackagePoolAutoPurgeInterval.floatValue);
                if (resourceAutoReleaseInterval != m_AssetPackagePoolAutoPurgeInterval.floatValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetPackagePoolAutoPurgeInterval = resourceAutoReleaseInterval;
                    }
                    else
                    {
                        m_AssetPackagePoolAutoPurgeInterval.floatValue = resourceAutoReleaseInterval;
                    }
                }

                float resourceExpireTime = EditorGUILayout.DelayedFloatField("AssetPackage Expire Time", m_AssetPackageExpireTime.floatValue);
                if (resourceExpireTime != m_AssetPackageExpireTime.floatValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetPackageExpireTime = resourceExpireTime;
                    }
                    else
                    {
                        m_AssetPackageExpireTime.floatValue = resourceExpireTime;
                    }
                }

                int resourcePriority = EditorGUILayout.DelayedIntField("AssetPackage Priority", m_AssetPackagePoolPriority.intValue);
                if (resourcePriority != m_AssetPackagePoolPriority.intValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetPackagePoolPriority = resourcePriority;
                    }
                    else
                    {
                        m_AssetPackagePoolPriority.intValue = resourcePriority;
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                m_ResAsyncLoaderType.Draw();
                m_ResSyncLoaderType.Draw();
                m_AssetByteLoaderType.Draw();

                m_AssetLoadAgentCount.intValue = EditorGUILayout.IntSlider("Load Resource Agent Helper Count", m_AssetLoadAgentCount.intValue, 1, 64);
                m_AssetLoadAgentExtraCount.intValue = EditorGUILayout.IntSlider("Extra Agent Helper Count For Each LoadPriority", m_AssetLoadAgentExtraCount.intValue, 1, 10);
            }
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying && PrefabUtility.GetPrefabType(t.gameObject) != PrefabType.Prefab)
            {
                EditorGUILayout.LabelField("Read Only Path", t.ReadOnlyPath.ToString());
                EditorGUILayout.LabelField("Read Write Path", t.ReadWritePath.ToString());
                EditorGUILayout.LabelField("Current Variant", t.CurrentVariant ?? "<Unknwon>");
                EditorGUILayout.LabelField("Applicable Game Version", t.ApplicationVersion ?? "<Unknwon>");
                EditorGUILayout.LabelField("Internal Resource Version", t.InternalResourceVersion.ToString());
                EditorGUILayout.LabelField("Asset Count", t.AssetCount.ToString());
                EditorGUILayout.LabelField("Asset Package Count", t.AssetPackageCount.ToString());
                EditorGUILayout.LabelField("Remote Resource Server URL", (t.RemoteResServerURL ?? "<Empty>").ToString());
                if (0 != (t.AssetMode & (uint)AssetMode.Updatable))
                {
                    EditorGUILayout.LabelField("Update Waiting Count", t.UpdateWaitingCount.ToString());
                    EditorGUILayout.LabelField("Updating Count", t.UpdatingCount.ToString());
                }
                EditorGUILayout.LabelField("Load Agent Total Count", t.LoadAgentTotalCount.ToString());
                EditorGUILayout.LabelField("Load Agent Free Count", t.LoadAgentFreeCount.ToString());
                EditorGUILayout.LabelField("Load Agent Working Count", t.LoadAgentWorkingCount.ToString());
                EditorGUILayout.LabelField("Load Task Waiting Count", t.LoadWaitingTaskCount.ToString());
            }

            if (!EditorApplication.isPlaying)
                EditorGUILayout.HelpBox("Asset state is available during runtime only.", MessageType.Info);
            else
            {
                _DisplayAssetPoolState(t);
                _DisplayAssetPackagePoolState(t);
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            _RefreshTypeNames();
        }

        private void OnEnable()
        {
            m_AssetMode = serializedObject.FindProperty("m_AssetMode");
            m_ResourceOnlyRootFolder = serializedObject.FindProperty("m_ResourceOnlyRootFolder");
            m_UseTempPath = serializedObject.FindProperty("m_UseTempPath");
            m_UnloadUnusedAssetsInterval = serializedObject.FindProperty("m_UnloadUnusedAssetsInterval");
            m_AssetPoolAutoPurgeInterval = serializedObject.FindProperty("m_AssetPoolAutoPurgeInterval");
            m_AssetExpireTime = serializedObject.FindProperty("m_AssetExpireTime");
            m_AssetPoolPriority = serializedObject.FindProperty("m_AssetPoolPriority");
            m_AssetPackagePoolAutoPurgeInterval = serializedObject.FindProperty("m_AssetPackagePoolAutoPurgeInterval");
            m_AssetPackageExpireTime = serializedObject.FindProperty("m_AssetPackageExpireTime");
            m_AssetPackagePoolPriority = serializedObject.FindProperty("m_AssetPackagePoolPriority");
            m_UpdateRetryCount = serializedObject.FindProperty("m_UpdateRetryCount");
            m_AssetLoadAgentCount = serializedObject.FindProperty("m_AssetLoadAgentCount");
            m_AssetLoadAgentExtraCount = serializedObject.FindProperty("m_AssetLoadAgentExtraCount");

            m_ResAsyncLoaderType.Init(serializedObject, "m_ResAsyncLoaderTypeName");
            m_ResSyncLoaderType.Init(serializedObject, "m_ResSyncLoaderTypeName");
            m_AssetByteLoaderType.Init(serializedObject, "m_AssetByteLoaderTypeName");

            _RefreshTypeNames();
        }

        private void _RefreshTypeNames()
        {
            m_ResAsyncLoaderType.Refresh();
            m_ResSyncLoaderType.Refresh();
            m_AssetByteLoaderType.Refresh();

            serializedObject.ApplyModifiedProperties();
        }

        private void _DisplayAssetPoolState(TMAssetModuleComponent component)
        {
            bool lastState = m_AssetFoldoutState;
            m_AssetFoldoutState = EditorGUILayout.Foldout(lastState, "Asset pool objects Info");

            if (m_AssetFoldoutState)
            {
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.LabelField("Type", "Asset Pool");
                    EditorGUILayout.LabelField("Auto Release Interval", component.AssetPoolAutoPurgeInterval.ToString());
                    EditorGUILayout.LabelField("Loaded Count", component.AssetLoadedCount.ToString());
                    EditorGUILayout.LabelField("Can Release Count", component.AssetCanReleaseCount.ToString());
                    EditorGUILayout.LabelField("Expire Time", component.AssetExpireTime.ToString());
                    EditorGUILayout.LabelField("Priority", component.AssetPoolPriority.ToString());

                    ObjectDesc[] objectInfos = null;
                    if (null != component)
                        objectInfos = component.AssetPoolObjectInfos;
                    
                    if (null != objectInfos && objectInfos.Length > 0)
                    {
                        EditorGUILayout.LabelField("Item Name:", "IsLocked,          Last Use Time, Spawn Count");
                        foreach (ObjectDesc objectInfo in objectInfos)
                        {
                            EditorGUILayout.LabelField(objectInfo.Name, string.Format("     {0},{1},       {2}", objectInfo.IsLocked.ToString(), objectInfo.LastUseTime.ToString("yyyy-MM-dd HH:mm:ss"), objectInfo.SpawnCount.ToString()));
                        }
                    }
                    else
                    {
                        GUILayout.Label("Asset Pool is Empty ...");
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Separator();
            }
        }

        private void _DisplayAssetPackagePoolState(TMAssetModuleComponent component)
        {
            bool lastState = m_AssetPackageFoldoutState;
            m_AssetPackageFoldoutState = EditorGUILayout.Foldout(lastState, "Asset package pool objects Info");

            if (m_AssetPackageFoldoutState)
            {
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.LabelField("Type", "Asset package Pool");
                    EditorGUILayout.LabelField("Auto Release Interval", component.AssetPackagePoolAutoPurgeInterval.ToString());
                    EditorGUILayout.LabelField("Loaded Count", component.AssetPackageLoadedCount.ToString());
                    EditorGUILayout.LabelField("Can Release Count", component.AssetPackageCanReleaseCount.ToString());
                    EditorGUILayout.LabelField("Expire Time", component.AssetPackageExpireTime.ToString());
                    EditorGUILayout.LabelField("Priority", component.AssetPackagePoolPriority.ToString());

                    ObjectDesc[] objectInfos = null;
                    if (null != component)
                        objectInfos = component.AssetPackagePoolObjectInfos;

                    if (null != objectInfos && objectInfos.Length > 0)
                    {
                        EditorGUILayout.LabelField("Item Name:", "IsLocked,          Last Use Time, Spawn Count");
                        foreach (ObjectDesc objectInfo in objectInfos)
                        {
                            EditorGUILayout.LabelField(objectInfo.Name, string.Format("     {0},{1},       {2}", objectInfo.IsLocked.ToString(), objectInfo.LastUseTime.ToString("yyyy-MM-dd HH:mm:ss"), objectInfo.SpawnCount.ToString()));
                        }
                    }
                    else
                    {
                        GUILayout.Label("Asset Pool is Empty ...");
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Separator();
            }
        }


        private void _DisplayAssetPackageNameList(List<ITMAssetPackage> packages)
        {
            if (null != packages && packages.Count > 0)
            {
                EditorGUILayout.BeginVertical("box");
                {
                    foreach (ITMAssetPackage package in packages)
                        EditorGUILayout.LabelField(package.Name);
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}
