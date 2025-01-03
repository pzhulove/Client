using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using Spine;
using System.Reflection;

namespace Spine.Unity.Editor 
{
	[CustomEditor(typeof(SkeletonUtilityAttach))]
	public class SkeletonUtilityAttachInspector : UnityEditor.Editor {

		SkeletonUtilityAttach skeletonUtility;
		Skeleton skeleton;
		SkeletonRenderer skeletonRenderer;
        SkeletonGraphic skeletonGraphic;
		bool isPrefab;

        string spawnBoneName;
		GUIContent SpawnHierarchyButtonLabel = new GUIContent("Spawn Hierarchy");

		void OnEnable () {
			skeletonUtility = (SkeletonUtilityAttach)target;
			skeletonRenderer = skeletonUtility.GetComponent<SkeletonRenderer>();
            skeletonGraphic = skeletonUtility.GetComponent<SkeletonGraphic>();
			skeleton = skeletonUtility.skeleton;

			if (skeleton == null) 
            {
                if(skeletonRenderer != null)
				{
                    skeletonRenderer.Initialize(false);
                    skeletonRenderer.LateUpdate();
                    skeleton = skeletonRenderer.skeleton;
                }
                else if(skeletonGraphic != null)
                {
                    skeletonGraphic.Initialize(false);
                    skeletonGraphic.LateUpdate();
                    skeleton = skeletonGraphic.Skeleton;
                }
			}

			//if (!skeletonRenderer.valid) return;

			isPrefab |= PrefabUtility.GetPrefabType(this.target) == PrefabType.Prefab;
		}
			
		public override void OnInspectorGUI () {
			if (isPrefab)
            {
				GUILayout.Label(new GUIContent("Cannot edit Prefabs"));
				return;
			}

			//if (!skeletonRenderer.valid)
             //{
			//	GUILayout.Label(new GUIContent("Spine Component invalid. Check Skeleton Data Asset."));
			//	return;	
			//}

            spawnBoneName = EditorGUILayout.TextField(spawnBoneName);

            if(GUILayout.Button("显示骨骼挂点"))
            {
                skeletonUtility.SpawnBone(spawnBoneName);
            }
		}


	}

}
