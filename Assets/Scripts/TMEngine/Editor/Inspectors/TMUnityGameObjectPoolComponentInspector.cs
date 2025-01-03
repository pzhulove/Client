using System.Collections.Generic;

using UnityEditor;

using Tenmove.Runtime;
using Tenmove.Runtime.Unity;
using Tenmove.Edtior;
using Tenmove.Editor.Unity;

namespace Tenmove.Edtior.Unity
{
    [CustomEditor(typeof(TMUnityGameObjectPoolComponent))]
    internal sealed class TMUnityGameObjectPoolComponentInspector : ComponentInspector
    {
        private HashSet<string> m_OpenedItems = new HashSet<string>();
        private string m_Filter = "";

        private void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Available during runtime only.", MessageType.Info);
                return;
            }

            TMUnityGameObjectPoolComponent t = (TMUnityGameObjectPoolComponent)target;

            bool needFilter = false;
            if (PrefabUtility.GetPrefabType(t.gameObject) != PrefabType.Prefab)
            {
                EditorGUILayout.LabelField("Game Object Assets Count", t.GameObjectAssetCount.ToString());

                m_Filter = EditorGUILayout.TextField("Filter:", m_Filter);
                if (!string.IsNullOrEmpty(m_Filter))
                    needFilter = true;
                List<UnityGameObjectPoolInfo> poolInfoList = FrameStackList<UnityGameObjectPoolInfo>.Acquire();
                t.GetAllPoolInfo(ref poolInfoList);
                for(int i = 0,icnt = poolInfoList.Count;i<icnt;++i)
                {
                    UnityGameObjectPoolInfo curPool = poolInfoList[i];
                    if( !needFilter || curPool.m_PrefabResPath.Contains(m_Filter, System.StringComparison.OrdinalIgnoreCase))
                         _DrawObjectPool(curPool);
                }

                FrameStackList<UnityGameObjectPoolInfo>.Recycle(poolInfoList);
            }

            Repaint();
        }

        private void _DrawObjectPool(UnityGameObjectPoolInfo poolInfo)
        {
            string fullName = poolInfo.m_PrefabResPath;
            bool lastState = m_OpenedItems.Contains(fullName);
            bool currentState = EditorGUILayout.Foldout(lastState, string.IsNullOrEmpty(fullName) ? "<Unnamed>" : fullName);
            if (currentState != lastState)
            {
                if (currentState)
                {
                    m_OpenedItems.Add(fullName);
                }
                else
                {
                    m_OpenedItems.Remove(fullName);
                }
            }

            if (currentState)
            {
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.LabelField("Prefab Path:", poolInfo.m_PrefabResPath);
                    EditorGUILayout.LabelField("Prefab Usage:", poolInfo.m_ObjectUsage.ToString());
                    EditorGUILayout.LabelField("Priority:", poolInfo.m_Priority.ToString());
                    EditorGUILayout.LabelField("Expire Time:", poolInfo.m_ExpireTime.ToString());
                    EditorGUILayout.LabelField("Reserve Count", poolInfo.m_ReserveCount.ToString());
                    EditorGUILayout.LabelField("Unused Object Count", poolInfo.m_UnusedObjectCount.ToString());
                    EditorGUILayout.LabelField("Using Object Object", poolInfo.m_UsingObjectCount.ToString());
                    EditorGUILayout.LabelField("Acquire Count", poolInfo.m_AcquireCount.ToString());
                    EditorGUILayout.LabelField("Recycle Count", poolInfo.m_RecycleCount.ToString());
                    EditorGUILayout.LabelField("Create Count", poolInfo.m_CreateCount.ToString());
                    EditorGUILayout.LabelField("Release Count", poolInfo.m_ReleaseCount.ToString());

                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Separator();
            }
        }
    }
}