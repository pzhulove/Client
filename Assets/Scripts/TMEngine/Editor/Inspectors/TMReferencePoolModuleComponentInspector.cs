
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Tenmove.Runtime;
using Tenmove.Runtime.Unity;

namespace Tenmove.Editor.Unity
{
    [CustomEditor(typeof(TMReferencePoolModuleComponent))]
    internal sealed class ReferencePoolComponentInspector : ComponentInspector
    {
        private HashSet<string> m_OpenedItems = new HashSet<string>();
        private List<ReferencePoolBase> m_AllObjectPools = new List<ReferencePoolBase>(5);

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Available during runtime only.", MessageType.Info);
                return;
            }

            TMReferencePoolModuleComponent t = (TMReferencePoolModuleComponent)target;

            if (PrefabUtility.GetPrefabType(t.gameObject) != PrefabType.Prefab)
            {
                EditorGUILayout.LabelField("Object Pool Count", t.Count.ToString());

                t.GetAllObjectPools(m_AllObjectPools, true);
                foreach (ReferencePoolBase objectPool in m_AllObjectPools)
                {
                    _DrawObjectPool(objectPool);
                }
            }

            Repaint();
        }

        private void OnEnable()
        {

        }

        private void _DrawObjectPool(ReferencePoolBase objectPool)
        {
            string fullName = Tenmove.Runtime.Utility.Text.GetNameWithType(objectPool.ObjectType, objectPool.Name);
            bool lastState = m_OpenedItems.Contains(fullName);
            bool currentState = EditorGUILayout.Foldout(lastState, string.IsNullOrEmpty(objectPool.Name) ? "<Unnamed>" : objectPool.Name);
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
                    EditorGUILayout.LabelField("Type", objectPool.ObjectType.FullName);
                    EditorGUILayout.LabelField("Auto Release Interval", objectPool.AutoPurgeInterval.ToString());
                    EditorGUILayout.LabelField("Capacity", objectPool.Capacity.ToString());
                    EditorGUILayout.LabelField("Used Count", objectPool.ObjectCount.ToString());
                    EditorGUILayout.LabelField("Can Release Count", objectPool.CanReleasedCount.ToString());
                    EditorGUILayout.LabelField("Expire Time", objectPool.ExpireTime.ToString());
                    EditorGUILayout.LabelField("Priority", objectPool.Priority.ToString());
                    ObjectDesc[] objectInfos = objectPool.GetAllObjectInfos();
                    if (objectInfos.Length > 0)
                    {
                        foreach (ObjectDesc objectInfo in objectInfos)
                        {
                            EditorGUILayout.LabelField(objectInfo.Name, string.Format("{0}, {1}, {2}", objectInfo.IsLocked.ToString(), objectPool.AllowMultiRef ? objectInfo.SpawnCount.ToString() : objectInfo.IsInUse.ToString(), objectInfo.LastUseTime.ToString("yyyy-MM-dd HH:mm:ss")));
                        }
                    }
                    else
                    {
                        GUILayout.Label("Object Pool is Empty ...");
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Separator();
            }
        }
    }
}
