using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Text.RegularExpressions;

// 检查特效Animator的工具
public class AnimatorCheck : EditorWindow
{
    private string m_SrcDir = "";
    private StringBuilder m_stringBuilder = new StringBuilder();
    private string m_Warning = "";
    private Vector2 scrollPos = new Vector2();
    private bool m_ConvertToAnimation;

    [MenuItem("[TM工具集]/ArtTools/检查特效Animator")]
    static void OpenWindow()
    {
        // Get existing open window or if none, make a new one:
        AnimatorCheck window = (AnimatorCheck)EditorWindow.GetWindow(typeof(AnimatorCheck));
        window.titleContent = new GUIContent("AnimatorCheck");
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("检查路径", GUILayout.Width(150));
        EditorGUILayout.TextField(m_SrcDir);
        if (GUILayout.Button("Open", GUILayout.Width(60)))
        {
            m_SrcDir = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath + "/Resources/Effects", "");
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("开始检查"))
        {
            CheckAnimator();
        }

        m_ConvertToAnimation = EditorGUILayout.Toggle("转成Animation", m_ConvertToAnimation);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Warning Prefabs:");
        GUILayout.Space(10);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUIStyle style = new GUIStyle(EditorStyles.textArea);
        style.wordWrap = true;
        style.fixedHeight = position.height - 30;
        m_Warning = EditorGUILayout.TextArea(m_Warning, style);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void CheckAnimator()
    {
        m_stringBuilder.Clear();
        m_Warning = "";
        if (m_SrcDir == "")
            return;

        string[] searchFolder = new string[] { m_SrcDir.Substring(m_SrcDir.IndexOf("Assets")) };
        var prefabs = AssetDatabase.FindAssets("t:prefab", searchFolder);

        float fProgress = 0;

        try
        {
            for (int i = 0; i < prefabs.Length; ++i)
            {
                var path = AssetDatabase.GUIDToAssetPath(prefabs[i]);

                fProgress += 1.0F;
                string title = "正在检查( " + i + " of " + prefabs.Length + " )";

                EditorUtility.DisplayProgressBar(title, path, fProgress / prefabs.Length);
                GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (root)
                {
                    CheckAnimator(path, "", root);
                }
            }
        }
        catch(Exception e)
        {
            EditorUtility.ClearProgressBar();
            throw e;
        }

        EditorUtility.ClearProgressBar();
        if (m_ConvertToAnimation)
        {
            AssetDatabase.SaveAssets();
        }

        m_Warning = m_stringBuilder.ToString();
    }

    private void CheckAnimator(string prefabName, string parentName, GameObject obj)
    {

        /*
                Animator animator = obj.GetComponent<Animator>();
                if (animator != null)
                {
                    m_stringBuilder.AppendFormat("{0} : {1}/{2}{3}", prefabName, parentName, obj.name, Environment.NewLine);

                }

                for (int i = 0; i < obj.transform.childCount; ++i)
                {
                    CheckAnimator(prefabName, parentName + "/" + obj.name, obj.transform.GetChild(i).gameObject);
                }*/
                 
        Animator animator = obj.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            m_stringBuilder.AppendFormat("{0}{1}", prefabName.Replace("Assets/Resources/", null), Environment.NewLine);

            if (m_ConvertToAnimation)
            {
                DowngradeAnimator(obj);
            }
        }
    }

    public static void DowngradeAnimator(GameObject curPrefab)
    {
        string curPrefabName = curPrefab.name;

        string animClipPath = AssetDatabase.GetAssetPath(curPrefab);
        animClipPath = Path.GetDirectoryName(animClipPath);

        GameObject instance = GameObject.Instantiate(curPrefab) as GameObject;

        _DowngradeAnimator(instance);

        PrefabUtility.ReplacePrefab(instance, curPrefab, ReplacePrefabOptions.ConnectToPrefab);
        //var newPrefab = PrefabUtility.CreatePrefab("Assets/tmp/Fx/" + prefab.name + ".prefab", instance);
        AssetDatabase.SaveAssets();
        GameObject.DestroyImmediate(instance);

    }

    public static float _DowngradeAnimator(GameObject instance)
    {
        float timeLen = 0.0f;

        Animator[] animators = instance.GetComponentsInChildren<Animator>();
        for (int i = 0; i < animators.Length; i++)
        {
            var animator = animators[i];
            GameObject go = animator.gameObject;
            AnimatorCullingMode cullMode = animator.cullingMode;
            if(animator.runtimeAnimatorController == null)
            {
                continue;
            }

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            Animation animation = null;
            for (int j = 0; j < clips.Length; j++)
            {
                AnimationClip curClip = clips[j];
                if (curClip == null)
                    continue;
                curClip = SaveLegacyAnimationCopy(curClip);
                if (curClip == null)
                    continue;
                animation = animator.gameObject.GetComponent<Animation>();
                if (animation == null)
                    animation = animator.gameObject.AddComponent<Animation>();
                if (animation.clip == null)
                    animation.clip = curClip;
                animation.AddClip(curClip, curClip.name);
                timeLen = Mathf.Max(curClip.length, timeLen);
            }
            if (animation == null)
                continue;
            switch (cullMode)
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
            DestroyImmediate(animator);
        }
        return timeLen;
    }

    private static Regex matSTRE = new Regex(@"(.*)_ST\.([xyzw])");

    private static AnimationClip SaveLegacyAnimationCopy(AnimationClip clip)
    {
        if (clip == null)
            return null;
        string clipPath = AssetDatabase.GetAssetPath(clip);// "Assets/tmp/Fx/" + clip.name + ".anim";
        string newPath = Path.ChangeExtension(clipPath, null) + "_downgrade.anim";

        AnimationClip clipCopy = AssetDatabase.LoadAssetAtPath(newPath, typeof(AnimationClip)) as AnimationClip;
        if (clipCopy != null)
        {
            return clipCopy;
        }
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
        EditorCurveBinding[] bindings2 = AnimationUtility.GetObjectReferenceCurveBindings(clip);
        if (bindings2.Length > 0)
        {
            foreach (EditorCurveBinding bind in bindings2)
            {
                // 弹窗提示
                EditorUtility.DisplayDialog("转换失败", string.Format("{0}中针对{1}的操作致使动画转换失败", clip.name + ".anim", (bind.type + "->" + bind.propertyName)), "关闭");
            }
            return null;
        }
        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(clip), newPath);
        AssetDatabase.ImportAsset(newPath);

        clipCopy = AssetDatabase.LoadAssetAtPath(newPath, typeof(AnimationClip)) as AnimationClip;
        if (clipCopy == null)
            return null;

        bool loop = AnimationUtility.GetAnimationClipSettings(clip).loopTime;
        //AnimationUtility.SetAnimationType(clipCopy, ModelImporterAnimationType.Legacy);
        clipCopy.legacy = true;

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