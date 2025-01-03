using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MaskEx : Mask
{
    protected override void Start()
    {
        base.Start();

        RectTransform rectTransform = transform as RectTransform;
        m_MinX = rectTransform.rect.x + transform.position.x;
        m_MinY = rectTransform.rect.y + transform.position.y;
        m_MaxX = m_MinX + rectTransform.rect.width * rectTransform.lossyScale.x;
        m_MaxY = m_MinY + rectTransform.rect.height * rectTransform.lossyScale.y;

        Init();
    }

    public void Init()
    {
        //这里 100  是因为ugui默认的缩放比例是100  你也可以去改这个值，但是我觉得最好别改。
        ParticleSystem[] particlesSystems = transform.parent.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < particlesSystems.Length; ++i)
        {
            Renderer psr = particlesSystems[i].GetComponent<Renderer>();
            psr.sharedMaterial.SetFloat("_MinX", m_MinX);
            psr.sharedMaterial.SetFloat("_MinY", m_MinY);
            psr.sharedMaterial.SetFloat("_MaxX", m_MaxX);
            psr.sharedMaterial.SetFloat("_MaxY", m_MaxY);
        }

        MeshRenderer[] amr = transform.parent.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < amr.Length; ++i)
        {
            Material[] am = amr[i].materials;
            for(int j = 0; j < am.Length; ++ j)
            {
                am[j].SetFloat("_MinX", m_MinX);
                am[j].SetFloat("_MinY", m_MinY);
                am[j].SetFloat("_MaxX", m_MaxX);
                am[j].SetFloat("_MaxY", m_MaxY);
            }
        }
    }

    protected float m_MinX = -10.0f;
    protected float m_MinY = -10.0f;
    protected float m_MaxX =  10.0f;
    protected float m_MaxY =  10.0f;
}