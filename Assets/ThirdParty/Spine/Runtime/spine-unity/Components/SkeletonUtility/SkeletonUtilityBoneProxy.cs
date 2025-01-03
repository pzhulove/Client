using UnityEngine;
using Spine;

namespace Spine.Unity {
	/// <summary>Sets a GameObject's transform to match a bone on a Spine skeleton.</summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Spine/SkeletonUtilityBoneProxy")]
    public class SkeletonUtilityBoneProxy : MonoBehaviour
    {
        public string boneName;
        public bool bUI;

        [System.NonSerialized] public SkeletonUtilityAttach skeletonUtility;
		[System.NonSerialized] public Bone bone;
        [System.NonSerialized] public bool valid = true;

        public void Reset () 
        {
			bone = null;
            valid = true;
		}

        void OnEnable () {
			skeletonUtility = transform.GetComponentInParent<SkeletonUtilityAttach>();

			if (skeletonUtility == null)
				return;

			skeletonUtility.RegisterBone(this);
			skeletonUtility.OnReset += HandleOnReset;
		}

		void HandleOnReset () {
			Reset();
		}

		void OnDisable () {
			if (skeletonUtility != null) {
				skeletonUtility.OnReset -= HandleOnReset;
				skeletonUtility.UnregisterBone(this);
			}
		}
        public void DoUpdate () 
        {
            if(!valid)
                return;

            float fScale = bUI ? 100.0f : 1.0f;

			var skeleton = skeletonUtility.skeleton;

			if (bone == null) 
            {
				if (string.IsNullOrEmpty(boneName)) return;
				bone = skeleton.FindBone(boneName);
				if (bone == null) 
                {
					Debug.LogError("Bone not found: " + boneName, this);
                    valid = false;
					return;
				}
			}

            transform.localPosition = new Vector3(bone.WorldX*fScale, bone.WorldY*fScale, 0);
			transform.localRotation = Quaternion.Euler(0, 0, bone.WorldRotationX - bone.shearX);
			transform.localScale = new Vector3(bone.WorldScaleX*fScale, bone.WorldScaleY*fScale, 1.0f);
		}
    }
}