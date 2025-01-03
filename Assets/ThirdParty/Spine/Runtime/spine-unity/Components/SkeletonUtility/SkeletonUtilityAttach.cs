using System.Collections.Generic;
using Spine;
using UnityEngine;

namespace Spine.Unity 
{
    [RequireComponent(typeof(ISkeletonAnimation))]
	[ExecuteInEditMode]
    public class SkeletonUtilityAttach: MonoBehaviour 
    {
        public delegate void SkeletonUtilityDelegate ();
		public event SkeletonUtilityDelegate OnReset;
		public Transform boneRoot;

        [HideInInspector]
		public SkeletonRenderer skeletonRenderer;
		[HideInInspector]
		public ISkeletonAnimation skeletonAnimation;



        [System.NonSerialized]
		public List<SkeletonUtilityBoneProxy> utilityBones = new List<SkeletonUtilityBoneProxy>();

        [System.NonSerialized]
        private Skeleton m_Skeleton;
		public Skeleton skeleton
        {
            get
            {
                if(m_Skeleton != null)
                    return m_Skeleton;

                if(skeletonAnimation != null)
                    m_Skeleton = skeletonAnimation.Skeleton;

                return m_Skeleton;
            }
        }

        void OnEnable () 
        {
			if (skeletonRenderer == null) 
            {
				skeletonRenderer = GetComponent<SkeletonRenderer>();
			}

            if (skeletonRenderer != null)
            {
                skeletonRenderer.OnRebuild -= HandleRendererReset;
                skeletonRenderer.OnRebuild += HandleRendererReset;
            }

            if (skeletonAnimation == null)
            {
				skeletonAnimation = GetComponent<SkeletonAnimation>();
				//if (skeletonAnimation == null)
				//	skeletonAnimation = GetComponent<SkeletonAnimator>();

                //if (skeletonAnimation == null)
				//	skeletonAnimation = GetComponent<SkeletonGraphicMultiObject>();

                if (skeletonAnimation == null)
					skeletonAnimation = GetComponent<SkeletonGraphic>();
			}

            if (skeletonAnimation != null)
            {
                skeletonAnimation.UpdateComplete += UpdateWorld;
            }   
        }

        void OnDisable () 
        {

            if (skeletonRenderer != null)
			    skeletonRenderer.OnRebuild -= HandleRendererReset;

			if (skeletonAnimation != null) 
            {
				skeletonAnimation.UpdateComplete -= UpdateWorld;
			}
		}

        public void RegisterBone (SkeletonUtilityBoneProxy bone) {
			if (utilityBones.Contains(bone))
				return;

			utilityBones.Add(bone);
		}

		public void UnregisterBone (SkeletonUtilityBoneProxy bone) {
			utilityBones.Remove(bone);
		}

        public Transform GetBoneRoot () {
			if (boneRoot != null)
				return boneRoot;

			boneRoot = new GameObject("SkeletonUtilityAttach-Root").transform;
			boneRoot.parent = transform;
			boneRoot.localPosition = Vector3.zero;
			boneRoot.localRotation = Quaternion.identity;
			boneRoot.localScale = Vector3.one;

			return boneRoot;
		}

        void HandleRendererReset (SkeletonRenderer r) {
			if (OnReset != null)
				OnReset();
		}

        void UpdateWorld (ISkeletonAnimation anim) 
        {
			for (int i = 0, n = utilityBones.Count; i < n; i++)
				utilityBones[i].DoUpdate();
		}

        public GameObject SpawnBone (string boneName) 
        {
            if(skeleton == null)
            {
                Debug.LogError("skeleton not found: ", this);
                return null;
            }

            if (string.IsNullOrEmpty(boneName)) return null;

            for (int i = 0, n = utilityBones.Count; i < n; i++)
			{
                if(utilityBones[i].boneName == boneName)
                {
                   return null; 
                }
            }	

            Bone bone = skeleton.FindBone(boneName);
			if (bone == null) 
            {
                Debug.LogError("Bone not found: " + boneName, this);
                return null;
            }

            Transform rootBone = GetBoneRoot ();

			GameObject go = new GameObject(boneName);
			go.transform.parent = rootBone;

			SkeletonUtilityBoneProxy b = go.AddComponent<SkeletonUtilityBoneProxy>();
			b.skeletonUtility = this;

            b.bUI = skeletonAnimation as SkeletonGraphic; // || skeletonAnimation as SkeletonGraphicMultiObject;

			b.Reset();
			b.bone = bone;
			b.boneName = boneName;
			b.valid = true;

			return go;
		}
    }
}