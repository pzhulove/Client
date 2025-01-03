using System;
using System.Collections.Generic;
using UnityEngine;

public class DynamicShadow : MonoBehaviour
{
    public Clips m_ActorInfo;
    public string m_ActorName;
    public Animation m_TrackAnimation;

    private Material m_ShadowMaterial;
    private Dictionary<string, ShadowStruct> m_ShadowDic = new Dictionary<string, ShadowStruct>();
    public string m_ClipName;
    public ShadowStruct m_Shadow;
    private AnimationState m_Anim_State;
    private float m_ClipTime;
    private MaterialPropertyBlock m_prop = null;

    public float m_ShadowAngle;
    // Start is called before the first frame updat
    private MeshRenderer m_MeshRenderer;

    public int count;
    [Serializable]
    public struct ShadowStruct
    {
        public Texture2D shadowTex;
        public int totalFrame;


        public ShadowStruct(Texture2D shadowTex, int totalFrame)
        {
            this.shadowTex = shadowTex;
            this.totalFrame = totalFrame;
        }
    }

    public void _SetShadowInfo(Clips actorInfo,bool init=true)
    {
        if (m_ActorName == actorInfo.ActName)
        {
            m_ActorInfo = actorInfo;
            var infos = m_ActorInfo.clips;
            m_ShadowDic.Clear();
            for (int i = 0; i < infos.Count; i++)
            {
                var info = infos[i];
                if (!m_ShadowDic.ContainsKey(info.ClipName))
                {
                    m_ShadowDic.Add(info.ClipName, new ShadowStruct(info.ShadowTexture, info.FrameCounts));
                }
            }
        }

        count = m_ShadowDic.Count;
        if (init)
            changeAnimation(m_ActorInfo.clips[0].ClipName);
    }
    private void Start()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_ShadowMaterial = new Material(Shader.Find("Unlit/ShadowDeformer"));
        m_ShadowMaterial.SetFloat("_Degrees", m_ShadowAngle);
        GetComponent<MeshRenderer>().material = m_ShadowMaterial;
        m_prop = new MaterialPropertyBlock();
        
    }

    private void Update()
    {
        if(m_ClipName==null)
            return;
        if (m_ShadowDic.ContainsKey(m_ClipName))
        {
            m_Anim_State = m_TrackAnimation[m_ClipName];
        }
        if (m_Anim_State != null)
        {
            m_MeshRenderer.GetPropertyBlock(m_prop);
            m_ClipTime = m_Anim_State.normalizedTime;
            m_prop.SetFloat("_Index", m_Shadow.totalFrame * m_ClipTime);
            float dir= -m_TrackAnimation.transform.parent.parent.transform.localScale.x;
            if (dir > 0)
            {
                m_prop.SetFloat("_Face", 2);
            }
            else
            {
                m_prop.SetFloat("_Face", 0.5f);
            }
            m_MeshRenderer.SetPropertyBlock(m_prop);
        }
    }

    public void changeAnimation(string actname)
    {
        if (m_ShadowDic.Count == 0 )
        {
            _SetShadowInfo(m_ActorInfo,false);
        }
        if (m_ShadowDic.ContainsKey(actname))
        {

            m_ClipName = actname;
            m_Anim_State = m_TrackAnimation[m_ClipName];
            m_Shadow = m_ShadowDic[m_ClipName];
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_prop = new MaterialPropertyBlock();
            m_MeshRenderer.GetPropertyBlock(m_prop);
            if (m_Shadow.shadowTex == null)
                return;
            m_prop.SetTexture("_MainTex", m_Shadow.shadowTex);
            m_prop.SetInt("_FinalIndex", m_Shadow.totalFrame);
            m_MeshRenderer.SetPropertyBlock(m_prop);
        }
    }
}
