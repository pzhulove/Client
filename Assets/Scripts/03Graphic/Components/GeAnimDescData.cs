using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GeAnimDescData : ScriptableObject
{
    [SerializeField]
    public GeAnimDesc[] animDescArray = new GeAnimDesc[0];

    [SerializeField]
    public string[] animDataResFile = new string[0];


#if UNITY_EDITOR
    private class AnimClipDesc
    {
        public AnimationClip m_AnimClip = null;
        public string m_AnimClipFile = null;
    }

    private class AnimFBXDesc
    {
        public Animation m_Anim = null;
        public string m_AnimFile = null;
    }

    public void GenAnimDesc()
    {
        List<GeAnimDesc> animDescs = new List<GeAnimDesc>();
        HashSet<string> animationClipNames = new HashSet<string>();

        List<AnimFBXDesc> animList = new List<AnimFBXDesc>();
        List<AnimClipDesc> animClipList = new List<AnimClipDesc>();

        for (int i = 0, icnt = animDataResFile.Length; i < icnt; ++i)
        {
            string ext = Path.GetExtension(animDataResFile[i]);
            // 不再查找fbx的动画
            //if (ext.Contains("fbx") || ext.Contains("FBX"))
            //{
            //    GameObject animGO = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine("Assets/Resources/", animDataResFile[i]));
            //    if (null == animGO) continue;

            //    AnimFBXDesc curAnimFBX = new AnimFBXDesc();
            //    curAnimFBX.m_Anim = animGO.GetComponent<Animation>();
            //    curAnimFBX.m_AnimFile = animDataResFile[i];
            //    animList.Add(curAnimFBX);
            //}
            //else if (ext.Contains("anim") || ext.Contains("ANIM"))
            if (ext.Contains("anim") || ext.Contains("ANIM"))
            {
                AnimationClip animClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine("Assets/Resources/", animDataResFile[i]));
                if (null == animClip) continue;

                AnimClipDesc curAnimClipDesc = new AnimClipDesc();
                curAnimClipDesc.m_AnimClip = animClip;
                curAnimClipDesc.m_AnimClipFile = animDataResFile[i];
                animClipList.Add(curAnimClipDesc);
            }
        }

        for (int i = 0, icnt = animClipList.Count; i < icnt; ++i)
        {
            AnimClipDesc curAnimClipDesc = animClipList[i];

            if (!animationClipNames.Contains(curAnimClipDesc.m_AnimClip.name))
            {
                animDescs.Add(new GeAnimDesc(curAnimClipDesc.m_AnimClip.name, curAnimClipDesc.m_AnimClip.name.GetHashCode(), curAnimClipDesc.m_AnimClip.wrapMode == WrapMode.Loop ? GeAnimClipPlayMode.AnimPlayLoop : GeAnimClipPlayMode.AnimPlayOnce, curAnimClipDesc.m_AnimClip.length, curAnimClipDesc.m_AnimClip, curAnimClipDesc.m_AnimClipFile));
                animationClipNames.Add(curAnimClipDesc.m_AnimClip.name);
            }
        }


        Animation[] animInstList = new Animation[animList.Count];
        for (int i = 0, icnt = animInstList.Length; i < icnt; ++i)
            animInstList[i] = animList[i].m_Anim;

        if (animList.Count > 0)
        {
            for (int i = 0; i < animList.Count; ++i)
            {
                foreach (AnimationState state in animList[i].m_Anim)
                {
                    if (!animationClipNames.Contains(state.name))
                    {
                        animDescs.Add(new GeAnimDesc(state.name, state.name.GetHashCode(), state.wrapMode == WrapMode.Loop ? GeAnimClipPlayMode.AnimPlayLoop : GeAnimClipPlayMode.AnimPlayOnce, _CalculateAnimTimeLength(animInstList, state.name), state.clip, animList[i].m_AnimFile));
                        animationClipNames.Add(state.name);
                    }
                }
            }
        }

        animDescArray = animDescs.ToArray();
    }

    private float _CalculateAnimTimeLength(Animation[] anis, string name)
    {
        float timeLen = 0.0f;
        for (int i = 0; i < anis.Length; ++i)
        {
            if (null == anis[i])
                continue;

            AnimationClip curClip = anis[i].GetClip(name);
            if (null != curClip)
                timeLen = Mathf.Max(curClip.length, timeLen);
        }

        return timeLen;
    }
#endif
}
