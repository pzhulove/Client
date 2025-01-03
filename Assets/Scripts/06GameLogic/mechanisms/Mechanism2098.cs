using System;
using GameClient;
using System.Collections.Generic;

/// <summary>
/// 路霸技能释放控制机制
/// 配置：A:子弹数
///      B：射击技能ID|更换子弹技能ID
///      C：子弹的资源ID
/// </summary>
public class Mechanism2098 : BeMechanism
{
    public Mechanism2098(int mid, int lv) : base(mid, lv){}
    
    private int _bulletMaxNum = 19;
    private int _shejiSkillId = 1417;
    private int _genhuanSkillId = 1416;
    private int _bulletResId = 64003;

    private int _curBulletNum = 0;

    public override void OnInit()
    {
        base.OnInit();
        if(data.ValueA.Count > 0)
        {
            _bulletMaxNum = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        }
        if(data.ValueB.Count == 2)
        {
            _shejiSkillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
            _genhuanSkillId = TableManager.GetValueFromUnionCell(data.ValueB[1], level);
        }
        if(data.ValueC.Count > 0)
        {
            _bulletResId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }
    }

    public override void OnReset()
    {
        _bulletMaxNum = 19;
        _shejiSkillId = 1417;
        _genhuanSkillId = 1416;
        _bulletResId = 64003;

        _curBulletNum = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        _InitEventRegister();
    }

    /// <summary>
    /// 初始化事件监听
    /// </summary>
    private void _InitEventRegister()
    {
        handleA = owner.RegisterEventNew(BeEventType.onStateChange, _OnStateChange);
        handleB = owner.RegisterEventNew(BeEventType.onAfterGenBullet, _OnAfterGenBullet);
    }

    /// <summary>
    /// 监听状态变化
    /// </summary>
    private void _OnStateChange(GameClient.BeEvent.BeEventParam param)
    {
        ActionState state = (ActionState)param.m_Int;
        if (state != ActionState.AS_IDLE)
            return;
        owner.delayCaller.DelayCall((33), UseSkill);
    }

    /// <summary>
    /// 监听子弹的创建
    /// </summary>
    /// <param name="args"></param>
    private void _OnAfterGenBullet(BeEvent.BeEventParam eventParam)
    {
        var projectile = eventParam.m_Obj as BeProjectile;
        if (projectile == null)
            return;
        if (projectile.m_iResID != _bulletResId)
            return;
        if (_curBulletNum < _bulletMaxNum)
        {
            _curBulletNum++;
            return;
        }
        _curBulletNum = 0;
        if (owner.CanUseSkill(_genhuanSkillId))
            owner.UseSkill(_genhuanSkillId);
    }

    private void UseSkill()
    {
        if (owner == null || owner.IsDeadOrRemoved())
            return;
        if (!owner.CanUseSkill(_shejiSkillId))
            return;
        owner.UseSkill(_shejiSkillId);
    }
}