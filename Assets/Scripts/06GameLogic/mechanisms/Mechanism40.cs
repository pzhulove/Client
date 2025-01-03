using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
阵鬼罗刹附着机制
 */

public class Mechanism40 : BeMechanism
{
    protected string m_EffectPath = null;
    protected int m_AttachBuffId = 0;                                   //瘟疫附着BuffId
    protected List<int> m_RemoveBuffList = new List<int>();             //移除Buff

    protected List<GeEffectEx> m_EffectList = new List<GeEffectEx>();   //瘟疫特效列表
    protected int m_LastAttachNum = 0;                                  //上次附着数量
    protected GameObject attachRoot = null;

    public Mechanism40(int mid, int lv) : base(mid, lv){}

    public override void OnReset()
    {
        m_RemoveBuffList.Clear();
        m_EffectList.Clear();
        m_LastAttachNum = 0;
    }

    public override void OnInit()
    {
        m_EffectPath = data.StringValueA[0];
        m_AttachBuffId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);

        if (data.ValueB.Count > 0)
        {
            for (int j = 0; j < data.ValueB.Count; j++)
            {
                int removeBuffId = TableManager.GetValueFromUnionCell(data.ValueB[j], level);
                 m_RemoveBuffList.Add(removeBuffId);
            }
        }
    }

#if !LOGIC_SERVER
    public override void OnUpdate(int deltaTime)
    {
        if(GetAttachNum()>0)
        {
            AddEffect(GetAttachNum());
        }
    }
#endif

    //获取Buff附加数量
    protected int GetAttachNum()
    {
        int buffAttachNum = owner.buffController.GetBuffCountByID(m_AttachBuffId);
        return buffAttachNum;
    }

    protected void AddEffect(int attachNum)
    {
#if !LOGIC_SERVER
        if (attachNum == m_LastAttachNum)
            return;
        m_LastAttachNum = attachNum;
        HideOtherEffect(attachNum);
        GeEffectEx effect = null;
        GameObject root = null;
        for (int i = 0; i < attachNum; i++)
        {
            if (m_EffectList.Count > i)
            {
                effect = m_EffectList[i];
                effect.SetVisible(true);
            }
            else
            {
                effect = CreateEffect();
                m_EffectList.Add(effect);
                attachRoot = owner.m_pkGeActor.GetAttachNode("[actor]Orign");
                if (attachRoot == null) continue;
                Battle.GeUtility.AttachTo(effect.GetRootNode(), root);
            }
            SetEffectPos(effect, i);
        }
#endif
    }

#if !LOGIC_SERVER
    //设置小瘟疫的坐标
    protected void SetEffectPos(GeEffectEx effect,int index)
    {
        if (effect == null)
            return;
        Vector3 pos = Vector3.zero;
        Vector3 overheadPos = owner.m_pkGeActor.GetOverHeadPosition();
        switch (index)
        {
            case 0:
                pos = new Vector3( -0.4f, 0.4f, -0.5f);
                break;
            case 1:
                pos = new Vector3( -0.4f, overheadPos.y / 2, - 0.5f);
                break;
            case 2:
                pos = new Vector3( -0.4f, overheadPos.y - 0.4f, - 0.5f);
                break;
        }
        effect.SetLocalPosition(pos);
        int x = owner.GetFace() ? -1 : 1;
        effect.SetScale(x,1,1);
    }
#endif

    protected GeEffectEx CreateEffect()
    {
        var effect = owner.m_pkGeActor.CreateEffect(m_EffectPath, null, 99999, Vec3.zero, 1, 1, false, false, EffectTimeType.BUFF);
        return effect;
    }

    protected void HideOtherEffect(int currNum)
    {
        if(m_EffectList.Count>currNum)
        {
            for (int i = currNum - 1; i < m_EffectList.Count; i++)
            {
                m_EffectList[i].SetVisible(false);
            }
        }
    }

    protected void RemoveBuff()
    {
        for (int i = 0; i < m_RemoveBuffList.Count; i++)
        {
            owner.buffController.RemoveBuff(m_RemoveBuffList[0]);
        }
    }

    protected void RemoveEffect()
    {
        for (int i = 0; i < m_EffectList.Count; i++)
        {
            owner.m_pkGeActor.DestroyEffect(m_EffectList[i]);
        }
        m_EffectList.Clear();
    }

    public override void OnFinish()
    {
        RemoveBuff();
#if !LOGIC_SERVER
        RemoveEffect();
#endif
    }
}