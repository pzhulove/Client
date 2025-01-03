using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Animations;
using System.Text.RegularExpressions;

public class AnimatorDowngradeConverter : Editor
{
    //[MenuItem("Assets/Downgrade Animator")]
    static public void DowngradeAnimator()
    {
        Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
        
        for (int i = 0; i < selection.Length; ++i)
        {
            GameObject curPrefab = selection[i] as GameObject;// AssetDatabase.LoadAssetAtPath<GameObject>(prefabList[i]);
            if (null == curPrefab)
                continue;

            string curPrefabName = curPrefab.name;

            string animClipPath = AssetDatabase.GetAssetPath(curPrefab);
            animClipPath = Path.GetDirectoryName(animClipPath);

            GameObject instance = GameObject.Instantiate(curPrefab) as GameObject;

            _DowngradeAnimator(instance);

            PrefabUtility.ReplacePrefab(instance, curPrefab, ReplacePrefabOptions.ConnectToPrefab);
            //var newPrefab = PrefabUtility.CreatePrefab("Assets/tmp/Fx/" + prefab.name + ".prefab", instance);
            AssetDatabase.SaveAssets();
            GameObject.DestroyImmediate(instance);

            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.SaveAssets();
    }

    static public float _DowngradeAnimator(GameObject instance)
    {
        float timeLen = 0.0f;

        Animator[] animators = instance.GetComponentsInChildren<Animator>();
        for (int i = 0; i < animators.Length; i++)
        {
            var animator = animators[i];
            GameObject go = animator.gameObject;
            AnimatorCullingMode cullMode = animator.cullingMode;

            AnimationClip clip = null;
            var deps = EditorUtility.CollectDependencies(new Object[] { go });
            foreach (var dep in deps)
            {
                if (dep is AnimationClip)
                {
                    clip = (AnimationClip)dep;
                    break;
                }
            }

            if (clip == null)
                continue;

            clip = SaveLegacyAnimationCopy(clip);
            if (clip == null)
                continue;

            var animation = animator.gameObject.GetComponent<Animation>();
            if (animation == null)
                animation = animator.gameObject.AddComponent<Animation>();

            animation.clip = clip;

            switch(cullMode)
            {
                case AnimatorCullingMode.AlwaysAnimate:
                    animation.cullingType = AnimationCullingType.AlwaysAnimate;
                    break;
                case AnimatorCullingMode.CullUpdateTransforms:
                    animation.cullingType = AnimationCullingType.BasedOnRenderers;
                    break;
                default:
                    animation.cullingType = AnimationCullingType.AlwaysAnimate;
                    break;
            }
            //animation.cullingType = AnimationCullingType.BasedOnRenderers;
            animation.enabled = true;

            timeLen = Mathf.Max(clip.length, timeLen);
            Object.DestroyImmediate(animator);
        }

        return timeLen;
    }

    static Regex matSTRE = new Regex(@"(.*)_ST\.([xyzw])");
    static AnimationClip SaveLegacyAnimationCopy(AnimationClip clip)
    {
        string clipPath = AssetDatabase.GetAssetPath(clip);// "Assets/tmp/Fx/" + clip.name + ".anim";
        string newPath = Path.ChangeExtension(clipPath, null) + "_downgrade.anim";
        
        AnimationClip clipCopy = AssetDatabase.LoadAssetAtPath(newPath, typeof(AnimationClip)) as AnimationClip;
        if (clipCopy != null)
        {
            return clipCopy;
        }
        
        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(clip), newPath);
        AssetDatabase.ImportAsset(newPath);
        
        clipCopy = AssetDatabase.LoadAssetAtPath(newPath, typeof(AnimationClip)) as AnimationClip;
        if (clipCopy == null)
            return null;
        if (clip == null)
            return null;

        bool loop = AnimationUtility.GetAnimationClipSettings(clip).loopTime;

        //AnimationUtility.SetAnimationType(clipCopy, ModelImporterAnimationType.Legacy);
        clipCopy.legacy = true;

        var bindings = AnimationUtility.GetCurveBindings(clip);

        List<AnimationCurve> curves = new List<AnimationCurve>();

        for (int i = 0; i < bindings.Length; i++)
        {
            curves.Add(AnimationUtility.GetEditorCurve(clip, bindings[i]));
        }

        if (loop)
            clipCopy.wrapMode = WrapMode.Loop;
        else
            clipCopy.wrapMode = WrapMode.Default;

        for (int i = 0; i < bindings.Length; i++)
        {
            var binding = bindings[i];

            if (binding.type != typeof(MeshRenderer))
            {
                continue;
            }

            if (binding.propertyName.StartsWith("material"))
            {
                int len = "material".Length;

                binding.type = typeof(Material);

                binding.propertyName = binding.propertyName.Substring(len + 1);

                var match = matSTRE.Match(binding.propertyName);

                /*
                material._MainTex_ST.x -> _MainTex.scale.x
                material._MainTex_ST.y -> _MainTex.scale.y
                material._MainTex_ST.z -> _MainTex.offset.x
                material._MainTex_ST.w -> _MainTex.offset.y
                material._Color.r -> _Color.r
                material._Color.g -> _Color.g
                 */
                if (match.Success)
                {
                    string texName = match.Groups[1].Value;
                    string subscript = match.Groups[2].Value;

                    if (subscript == "x" || subscript == "y")
                    {
                        binding.propertyName = string.Format("{0}.scale.{1}", texName, subscript);
                    }
                    else if (subscript == "z")
                    {
                        binding.propertyName = string.Format("{0}.offset.x", texName);
                    }
                    else if (subscript == "w")
                    {
                        binding.propertyName = string.Format("{0}.offset.y", texName);
                    }
                }
            }

            bindings[i] = binding;
        }

        clipCopy.ClearCurves();

        for (int i = 0; i < bindings.Length; i++)
        {
            var binding = bindings[i];

            //Debug.Log(string.Format("curve keys{0} binding {1} {2} from clip {3}", curves[i].length, binding.path, binding.propertyName, clip.name));
            AnimationUtility.SetEditorCurve(clipCopy, binding, curves[i]);
        }

        EditorUtility.SetDirty(clipCopy);
        AssetDatabase.SaveAssets();

        return clipCopy;
    }



}
