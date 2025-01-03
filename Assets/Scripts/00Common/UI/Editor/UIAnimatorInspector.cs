using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using GameClient;
/// <summary>
/// UI动画组件显示
/// </summary>
[CustomEditor(typeof(UIAnimator))]
public class UIAnimatorInspector : Editor
{
    private SerializedObject m_Object;

    private SerializedProperty m_PredefineFadeIn;
    private SerializedProperty m_PredefineFadeOut;
    private SerializedProperty mCustomFadeIn;
    private SerializedProperty mCustomFadeOut;
    private SerializedProperty mIsUseCustomAnimation;
    private bool m_UsePredefineAnimation = true;
    private bool m_UseCustomAnimation;

    private List<string> m_PredefineAnimations = new List<string>();
    private string[] m_sPredefineAnimations;

    protected void OnEnable()
    {
        m_Object = new SerializedObject(target);

        m_PredefineFadeIn = m_Object.FindProperty("m_PredefineFadeIn");
        m_PredefineFadeOut = m_Object.FindProperty("m_PredefineFadeOut");
        mCustomFadeIn = m_Object.FindProperty("mCustomFadeIn");
        mCustomFadeOut = m_Object.FindProperty("mCustomFadeOut");
        mIsUseCustomAnimation = m_Object.FindProperty("mIsCustom");
        m_UseCustomAnimation = mIsUseCustomAnimation.boolValue;
        _LoadUIRoot();
        _Reload();
    }

    private int _GetAnimationIndex(string name)
    {
        if(name != null)
        { 
            for(int i = 0; i < m_PredefineAnimations.Count; ++i)
            {
                if(m_PredefineAnimations[i] == name)
                {
                    return i;
                }
            }
        }

        return 0;
    }

    private void _LoadUIRoot()
    {
        m_PredefineAnimations.Clear();
        m_PredefineAnimations.Add("");

        string[] predefinieAnimations = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/UIFlatten/Prefabs/Animation" });
        if(predefinieAnimations != null)
        {
            foreach (var itr in predefinieAnimations)
            {
                m_PredefineAnimations.Add(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(itr)));
            }
        }

/*
        GameObject uiRoot = Resources.Load("Base/UI/Prefabs/Root/UIRoot") as GameObject;
        if (uiRoot != null)
        {
            UIPredefineAnimations com = uiRoot.GetComponentInChildren<UIPredefineAnimations>();
            if(com != null)
            {
                foreach(var itr in com.m_PredefineAnimations)
                {
                    m_PredefineAnimations.Add(Path.GetFileNameWithoutExtension(itr.m_Res));
                }
            }
        }*/

        m_sPredefineAnimations = m_PredefineAnimations.ToArray();
    }

    protected void _Reload()
    {
        UIAnimator targAnimation = (UIAnimator)target;
 
        if (!string.IsNullOrEmpty(targAnimation.m_PredefineFadeIn) || !string.IsNullOrEmpty(targAnimation.m_PredefineFadeOut))
        {
            m_UsePredefineAnimation = true;
        }
        else
        {
            m_UsePredefineAnimation = false;
        }
    }

    private bool mCustomValue = false;
    public override void OnInspectorGUI()
    {
        UIAnimator animator = target as UIAnimator;
        bool changed = false;
        bool isCustom = serializedObject.FindProperty("mIsCustom").boolValue;
        if (!m_UsePredefineAnimation || isCustom)
        {
            base.OnInspectorGUI();
            if (isCustom != mCustomValue)
            {
                mCustomValue = isCustom;
                if (!mCustomValue)
                {
                    m_UsePredefineAnimation = true;
                }
            }
        }
        else
        {
            mCustomValue = serializedObject.FindProperty("mIsCustom").boolValue;
            m_UseCustomAnimation = GUILayout.Toggle(m_UseCustomAnimation, "使用自定义动画");
            m_UsePredefineAnimation = GUILayout.Toggle(m_UsePredefineAnimation, "使用预定义动画");
            if (m_UsePredefineAnimation)
            {
                int fadeIn = _GetAnimationIndex(animator.m_PredefineFadeIn);

                int newFadeIn = EditorGUILayout.Popup("预定义淡入动画", fadeIn, m_sPredefineAnimations);
                if (newFadeIn != fadeIn)
                {
                    m_PredefineFadeIn.stringValue = m_sPredefineAnimations[newFadeIn];
                    changed = true;
                }

                int fadeOut = _GetAnimationIndex(animator.m_PredefineFadeOut);

                int newFadeOut = EditorGUILayout.Popup("预定义淡出动画", fadeOut, m_sPredefineAnimations);
                if (newFadeOut != fadeOut)
                {
                    m_PredefineFadeOut.stringValue = m_sPredefineAnimations[newFadeOut];
                    changed = true;
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(target);
                m_Object.ApplyModifiedProperties();
            }
        }
/*
        else
        {
            GUILayout.Label("该组件会自动收集UI Prefab中的Dotween动画和Animator动画：\n" +
            "\n其中Dotween动画如果没有名称，则认为是淡出动画。UIAnimator会为其" +
            "\n创建一个对应的淡入动画，UI界面打开和关闭时自动播放淡入和淡出动画" +
            "\n有名称认为是自定义动画（不会在界面打开关闭时自动播放），可调用UI界面" +
            "\n的PlayCustomAnimation进行播放。\n" +
            "\nAnimator动画中定义名为FadeIn和FadeOut的状态机对应淡入和淡出动画，" +
            "\n其他名称的状态机视为自定义动画，同样用PlayCustomAnimation进行播放。");
        }*/


    }
}