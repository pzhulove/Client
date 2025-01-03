using UnityEngine;
using System.Collections.Generic;

/*
 * 秘术师-元素点燃机制
*/
public class Mechanism54 : BeMechanism
{
    protected int[] m_PveAttriBuffInfoArray = new int[4];   //对应属性添加的BuffInfoId(PVE)
    protected int[] m_PvpAttriBuffInfoArray = new int[4];   //对应属性添加的BuffInfoId(PVP)
#if !LOGIC_SERVER
    protected int[] m_PveAttriBuffArray = new int[4];       //对应属性添加的BuffId(PVE)
    protected int[] m_PvpAttriBuffArray = new int[4];       //对应属性添加的BuffId(PVP)
#endif

    protected int m_LastSkillAttri = 0;                     //上一次释放技能属性
    protected IBeEventHandle m_SkillCastHandle = null;       //监听技能释放
#if !LOGIC_SERVER
    protected string[] m_TopEffectPath = new string[4];     //元素点燃头顶特效 光 火 冰 暗
    protected string[] m_BodyEffectPath = new string[4];    //元素点燃身边特效 光 火 冰 暗
    protected string[] m_BodyBuffEffectPath = new string[4];//元素点燃Buff身边特效 光 火 冰 暗
    protected GeEffectEx m_LastEffect = null;               //上一次显示属性特效
#endif
    protected IBeEventHandle m_BuffStartHandle = null;       //Buff开始
    protected IBeEventHandle m_BuffFinishHandle = null;      //Buff结束
    protected IBeEventHandle m_BuffRefreshHandle = null;     //Buff刷新

    public Mechanism54(int mid, int lv) : base(mid, lv){}

    public override void OnReset()
    {
        m_LastSkillAttri = 0;

        BeUtility.ResetIntArray(m_PveAttriBuffInfoArray);
        BeUtility.ResetIntArray(m_PvpAttriBuffInfoArray);
#if !LOGIC_SERVER
        BeUtility.ResetIntArray(m_PveAttriBuffArray);
        BeUtility.ResetIntArray(m_PvpAttriBuffArray);
#endif        

        m_SkillCastHandle = null; 
#if !LOGIC_SERVER        
        m_LastEffect = null;
#endif        
        m_BuffStartHandle = null;
        m_BuffFinishHandle = null; 
        m_BuffRefreshHandle = null; 
    }
    public override void OnInit()
    {
        if (data.ValueA.Count > 0)
        {
            for (int i = 0; i < data.ValueA.Count; i++)
            {
                int buffInfoId = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
                if (buffInfoId != 0)
                {
                    m_PveAttriBuffInfoArray[i] = buffInfoId;
                }
            }
        }

        if (data.ValueB.Count > 0)
        {
            for (int i = 0; i < data.ValueB.Count; i++)
            {
                int buffInfoId = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
                if (buffInfoId != 0)
                {
                    m_PvpAttriBuffInfoArray[i] = buffInfoId;
                }
            }
        }
#if !LOGIC_SERVER
        if (data.ValueC.Count > 0)
        {
            for (int i = 0; i < data.ValueC.Count; i++)
            {
                int buffId = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
                if (buffId != 0)
                {
                    m_PveAttriBuffArray[i] = buffId;
                }
            }
        }

        if (data.ValueD.Count > 0)
        {
            for (int i = 0; i < data.ValueD.Count; i++)
            {
                int buffId = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
                if (buffId != 0)
                {
                    m_PvpAttriBuffArray[i] = buffId;
                }
            }
        }

        m_TopEffectPath[0] = "Effects/Hero_Yuansu/Yuansudianran/Prefab/Eff_yuansudianran_guang_zi";
        m_TopEffectPath[1] = "Effects/Hero_Yuansu/Yuansudianran/Prefab/Eff_yuansudianran_huo_zi";
        m_TopEffectPath[2] = "Effects/Hero_Yuansu/Yuansudianran/Prefab/Eff_yuansudianran_bing_zi";
        m_TopEffectPath[3] = "Effects/Hero_Yuansu/Yuansudianran/Prefab/Eff_yuansudianran_an_zi";

        m_BodyEffectPath[0] = "Effects/Hero_Yuansu/Yuansudianran/Prefab/Eff_yuansudianran_buff_guang";
        m_BodyEffectPath[1] = "Effects/Hero_Yuansu/Yuansudianran/Prefab/Eff_yuansudianran_buff_huo";
        m_BodyEffectPath[2] = "Effects/Hero_Yuansu/Yuansudianran/Prefab/Eff_yuansudianran_buff_bing";
        m_BodyEffectPath[3] = "Effects/Hero_Yuansu/Yuansudianran/Prefab/Eff_yuansudianran_buff_an";

        m_BodyBuffEffectPath[0] = "Effects/Hero_Yuansu/Yuansudianran/Prefab/Eff_yuansudianran_guang";
        m_BodyBuffEffectPath[1] = "Effects/Hero_Yuansu/Yuansudianran/Prefab/Eff_yuansudianran_huo";
        m_BodyBuffEffectPath[2] = "Effects/Hero_Yuansu/Yuansudianran/Prefab/Eff_yuansudianran_bing";
        m_BodyBuffEffectPath[3] = "Effects/Hero_Yuansu/Yuansudianran/Prefab/Eff_yuansudianran_an";
#endif
    }

    public override void OnStart()
    {
        RemoveHandle();
#if !LOGIC_SERVER
        CreateBodyEffect();
#endif
        m_SkillCastHandle = owner.RegisterEventNew(BeEventType.onCastSkill, args =>
        {
            int castSkillID = args.m_Int;
            BeSkill skill = owner.GetSkill(castSkillID);
            int skillAttri = skill.skillData.SkillAttri;
            if (skillAttri != 0)
            {
                if (skillAttri != m_LastSkillAttri)
                {
                    int index = skillAttri - 1;
#if !LOGIC_SERVER
                    SetTopEffect(index);
#endif
                    int buffInfoId = BattleMain.IsModePvP(battleType) ? m_PvpAttriBuffInfoArray[index] : m_PveAttriBuffInfoArray[index];
                    BuffInfoData buffInfo = new BuffInfoData(buffInfoId, level);
                    owner.buffController.TryAddBuff(buffInfo);
                    m_LastSkillAttri = skill.skillData.SkillAttri;
                }
            }
        });

#if !LOGIC_SERVER
        m_BuffStartHandle = owner.RegisterEventNew(BeEventType.onBuffStart, args => 
        {
            int buffId = args.m_Int;
            int index = GetBuffIndex(buffId);
            if (index != -1)
            {
                ChangeEffect(m_BodyEffectPath[index], m_BodyBuffEffectPath[index]);
            }
        });

        m_BuffRefreshHandle = owner.RegisterEventNew(BeEventType.onBuffRefresh, args => 
        {
            int buffId = args.m_Int;
            int index = GetBuffIndex(buffId);
            if (index != -1)
            {
                ChangeEffect(m_BodyEffectPath[index], m_BodyBuffEffectPath[index]);
            }
        });

        m_BuffFinishHandle = owner.RegisterEventNew(BeEventType.onBuffFinish, args =>
        {
            int buffId = args.m_Int;
            int index = GetBuffIndex(buffId);
            if (index != -1)
            {
                ChangeEffect(m_BodyBuffEffectPath[index], m_BodyEffectPath[index]);
            }
        });
#endif
    }

#if !LOGIC_SERVER
    protected void SetTopEffect(int index)
    {
        if (m_LastEffect != null)
        {
            owner.m_pkGeActor.DestroyEffect(m_LastEffect);
        }

        GeEffectEx effect = CreateEffect(m_TopEffectPath[index]);
        m_LastEffect = effect;
    }

    //创建元素属性对应的头顶特效
    protected GeEffectEx CreateEffect(string path)
    {
        GeEffectEx effect = null;
        if (owner.m_pkGeActor != null)
        {
            effect = owner.m_pkGeActor.CreateEffect(path, null, 9999999, new Vec3(0, 0, 0), 1, 1, true, owner.GetFace(), EffectTimeType.BUFF);
            if (effect != null)
            {
                Battle.GeUtility.AttachTo(effect.GetRootNode(), owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root));
            }
        }
        return effect;
    }

    protected void CreateBodyEffect()
    {
        if (owner.m_pkGeActor == null)
            return;
        for (int i=0;i< m_BodyEffectPath.Length; i++)
        {
            CreateEffect(m_BodyEffectPath[i]);
        }
    }

    protected void ChangeEffect(string orignPath,string replacePath)
    {
        GeEffectEx effect = owner.m_pkGeActor.GetEffectManager().GetEffectByName(orignPath);
        if (effect != null)
        {
            owner.m_pkGeActor.DestroyEffect(effect);
            effect = null;
        }

        GeEffectEx newEffect = owner.m_pkGeActor.GetEffectManager().GetEffectByName(replacePath);
        if (newEffect != null)
        {
            owner.m_pkGeActor.DestroyEffect(newEffect);
            newEffect = null;
        }

        CreateEffect(replacePath);
    }
    
    protected int GetBuffIndex(int buffId)
    {
        int index = -1;
        for(int i=0;i< m_PveAttriBuffArray.Length; i++)
        {
            if(m_PveAttriBuffArray[i]== buffId)
            {
                index = i;
            }
        }

        for(int j = 0; j < m_PvpAttriBuffArray.Length; j++)
        {
            if (m_PvpAttriBuffArray[j] == buffId)
            {
                index = j;
            }
        }
        return index;
    }
#endif

    //删除特效
    protected void DestroyEffect(string path)
    {
#if !LOGIC_SERVER
        GeEffectEx newEffect = owner.m_pkGeActor.GetEffectManager().GetEffectByName(path);
        if (newEffect != null)
        {
            owner.m_pkGeActor.DestroyEffect(newEffect);
            newEffect = null;
        }
#endif
    }

    public override void OnFinish()
    {
        RemoveHandle();
        RemoveEffect();
    }

    protected void RemoveHandle()
    {

        if (m_SkillCastHandle != null)
        {
            m_SkillCastHandle.Remove();
            m_SkillCastHandle = null;
        }

        if (m_BuffStartHandle != null)
        {
            m_BuffStartHandle.Remove();
            m_BuffStartHandle = null;
        }

        if (m_BuffRefreshHandle != null)
        {
            m_BuffRefreshHandle.Remove();
        }

        if (m_BuffFinishHandle != null)
        {
            m_BuffFinishHandle.Remove();
            m_BuffFinishHandle = null;
        }
    }

    protected void RemoveEffect()
    {
        m_LastSkillAttri = 0;
#if !LOGIC_SERVER
        if (m_LastEffect != null)
        {
            owner.m_pkGeActor.DestroyEffect(m_LastEffect);
            m_LastEffect = null;
        }

        for(int i = 0; i < m_TopEffectPath.Length; i++)
        {
            DestroyEffect(m_TopEffectPath[i]);
        }

        for (int i = 0; i < m_BodyEffectPath.Length; i++)
        {
            DestroyEffect(m_BodyEffectPath[i]);
        }

        for (int i = 0; i < m_BodyBuffEffectPath.Length; i++)
        {
            DestroyEffect(m_BodyBuffEffectPath[i]);
        }
#endif
    }
}
