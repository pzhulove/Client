using FlatBuffers;
using GameClient;
using UnityEngine;

/// <summary>
/// 帧标签控制场景特效显示
/// </summary>
public class Mechanism1117 : BeMechanism
{
    public Mechanism1117(int mid, int lv) : base(mid, lv) { }
    
#if !LOGIC_SERVER
    private string m_EffectPath = "Effects/Hero_Gungirl/Gungirl_jsuexing/Perfab/Eff_BLMG_juexing01_beijing";
    private string m_StartFrame = null;
    private string m_EndFrame = null;
    private float m_EffectScale = 1f;

    private GeEffectEx m_SceneEffect = null;

    public override void OnInit()
    {
        m_EffectPath  = data.StringValueA[0];
        m_StartFrame = TableManager.GetValueFromUnionCell(data.ValueA[0], level).ToString();
        m_EndFrame = TableManager.GetValueFromUnionCell(data.ValueB[0], level).ToString();
        if (data.ValueC.Count > 0)
        {
            m_EffectScale = TableManager.GetValueFromUnionCell(data.ValueC[0], level) / 1000f;
        }
    }

    public override void OnReset()
    {
        m_EffectScale = 1f;
        m_SceneEffect = null;
    }

    public override void OnStart()
    {
        if(owner == null)
            return;

        handleA = owner.RegisterEventNew(BeEventType.onSkillCurFrame, OnSkillCurFrame);
        //handleA = owner.RegisterEvent(BeEventType.onSkillCurFrame, OnSkillCurFrame);
    }
    
    public override void OnFinish()
    {
        ResetEffect();
    }

    private void OnSkillCurFrame(BeEvent.BeEventParam param)
    {
        string flag = param.m_String;
        if (flag == m_StartFrame)
        {
            ResetEffect();
            ShowEffect();
        }
        else if (flag == m_EndFrame)
        {
            ResetEffect();
        }
    }

    private void ShowEffect()
    {
        if (!owner.isLocalActor)
            return;
        if (owner == null || owner.CurrentBeScene == null || owner.CurrentBeScene.currentGeScene == null)
            return;
        
        m_SceneEffect = owner.CurrentBeScene.currentGeScene.CreateEffect(m_EffectPath, 100, GetSceneCenterPos(), m_EffectScale, 1f, true);
        
        if (m_SceneEffect != null)
        {
            int x = owner.GetFace() ? -1 : 1;
            m_SceneEffect.SetScale(m_EffectScale * x, m_EffectScale, m_EffectScale);
        }
    }
    

    private void ResetEffect()
    {
        if (!owner.isLocalActor)
            return;
        
        if (m_SceneEffect != null)
        {
            if (owner == null || owner.CurrentBeScene == null || owner.CurrentBeScene.currentGeScene == null)
                return;
            
            owner.CurrentBeScene.currentGeScene.DestroyEffect(m_SceneEffect);
            m_SceneEffect = null;
        }
    }

    private Vec3 GetSceneCenterPos()
    {
        var vec = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0);
        Ray ray = Camera.main.ScreenPointToRay(vec);
        var t = -ray.origin.y / ray.direction.y;
        var worldPos = ray.GetPoint(t);
        var pos = new Vec3(worldPos.x, owner.CurrentBeScene.logicZSize.fy, 0);
        return pos;
    }
#endif
}

