using System.Collections.Generic;
using GameClient;

/*
/// <summary>
/// 选中目标机制 同时给目标添加一个标记Buff
/// 选中优先级 玩家、boss、精英、小怪
///
/// 配置：A:子弹数
///      B:视野
///      C：扎堆判定距离
///      D:手动CD|自动CD
/// </summary>
public class Mechanism1125 : LockAttackMechanism
{
    public Mechanism1125(int mid, int lv) : base(mid, lv) { }

    private int _totalBulletNum = 3;    //总的子弹数量
    private int _findMostRadius = 20000; // 扎堆半径
    private int _chaseSight = 100000;    // 视野范围
    private int _skillId = 1414;
    private int _entityId = 64014;
    private int _ManualCDTime = 750;
    private int _AutoCDTime = 1500;

    private int _curAttackTime = 0;

    private int _curBulletNum;    //当前使用子弹数量
    private BeEntity _attackTarget = null;
    private Skill1403 _skill;
    private BeActor _topActor;


    public override void OnInit()
    {
        base.OnInit();
        if (data.ValueA.Count > 0)
        {
            _totalBulletNum = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        }
        if (data.ValueB.Count > 0)
        {
            _chaseSight = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        }
        if (data.ValueC.Count > 0)
        {
            _findMostRadius = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }
        if (data.ValueD.Count == 2)
        {
            _ManualCDTime = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
            _AutoCDTime = TableManager.GetValueFromUnionCell(data.ValueD[1], level);
        }
        _curAttackTime = _AutoCDTime;
    }

    public override void OnStart()
    {
        base.OnStart();
        InitEventRegister();
        _skill = GetSkill();
    }

    private Skill1403 GetSkill()
    {
        if (_topActor == null)
        {
            _topActor = owner.GetTopOwner(owner) as BeActor;
        }
        if (_topActor != null)
        {
            var skill = _topActor.GetSkill(1403);
            if (skill != null)
            {
                return skill as Skill1403;
            }
        }

        return null;
    }

    private void InitEventRegister()
    {
        if (owner.CurrentBeScene != null)
        {
            handleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onChangeStartPos, _OnChangeStartPos);
        }
    }

    /// <summary>
    /// 监听子弹创建
    /// </summary>
    private void _OnChangeStartPos(BeEvent.BeEventParam param)
    {
        var entity = param.m_Obj as BeEntity;
        if (entity == null || !entity.IsSameTopOwner(owner))
            return;
        if (entity.m_iResID != _entityId)
            return;
        
        if (_attackTarget != null )
        {
            param.m_Vint3 = _attackTarget.GetPosition();
            param.m_Vint3.z = 0;
        }
        
        if (_curBulletNum >= _totalBulletNum)
            owner.DoDead();
    }

    /// <summary>
    /// 攻击目标为：有集火标记的单位＞范围内怪物最多的点＞敌方玩家＞BOSS＞精英（深渊怪物）＞普通小怪
    /// </summary>
    /// <returns></returns>
    private BeEntity FindTarget()
    {
        // 集火优先
        var target = FindForceTarget();
        if (target != null)
        {
            return target;
        }

        // 找扎堆的怪
        target = FindMostTarget();
        if (target != null)
        {
            return target;
        }

        // 优先级查找
        return FindPriority();
    }

    
    /// <summary>
    /// 范围内的集火单位
    /// </summary>
    /// <returns></returns>
    private BeEntity FindForceTarget()
    {
        if (ForceTarget != null && !ForceTarget.IsDeadOrRemoved())
        {
            if (ForceTarget.GetDistance(owner) <= _chaseSight)
            {
                return ForceTarget;
            }

            return null;
        }

        return null;
    }

    /// <summary>
    /// 最扎堆的怪物
    /// </summary>
    /// <returns></returns>
    private BeEntity FindMostTarget()
    {
        if (owner.CurrentBeScene == null)
            return null;
        var targets = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(targets, owner, _chaseSight);
        int most = 0;
        BeEntity reslut = null;
        for (int i = 0; i < targets.Count; i++)
        {
            var centerActor = targets[i];
            var count = 1;
            for (int j = 0; j < targets.Count; j++)
            {
                if(j == i)
                    continue;

                var radiusActor = targets[j];

                if (radiusActor.GetDistance(centerActor) <= _findMostRadius)
                {
                    count++;
                }
            }

            if (count > most)
            {
                most = count;
                reslut = centerActor;
            }
        }
        GamePool.ListPool<BeActor>.Release(targets);

        // 3个才算扎堆
        if (most >= 3)
        {
            return reslut;
        }

        return null;
    }

    private BeEntity FindPriority()
    {
        if (owner.CurrentBeScene == null)
            return null;

        return owner.CurrentBeScene.FindTargetByPriority(owner, _chaseSight);
    }
    /// <summary>
    /// 点击技能按钮主动进行攻击
    /// </summary>
    public void DoAttack()
    {
        if (CanUseSkill())
        {
            var findTarget = FindTarget();
            if (findTarget != null)
            {
                _curBulletNum++;
                _attackTarget = findTarget;
                _curAttackTime = 0;

                owner.UseSkill(_skillId, true);
                if (_curBulletNum < _totalBulletNum)
                {
                    ShowSpellBar(true);
                }
                else
                {
                    ShowSpellBar(false);
                }
            }            
        }
    }

    public bool CanUseSkill()
    {
        if (_curBulletNum >= _totalBulletNum)
            return false;

        if (owner.IsDeadOrRemoved())
            return false;

        if (owner.IsPassiveState())
            return false;

        if (_topActor != null &&
            _topActor.IsPassiveState())
            return false;
                
        if (!owner.CanUseSkill(_skillId))
            return false;

        return HasTargets();
    }

    public bool HasTargets()
    {
        if (owner.CurrentBeScene == null)
            return false;
        
        var targets = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(targets, owner, _chaseSight);
        int count = targets.Count;
        GamePool.ListPool<BeActor>.Release(targets);

        return count > 0;
    }

    public bool CanUpdateCD()
    {
        if(owner.sgGetCurrentState() == (int)ActionState.AS_BIRTH || 
           owner.sgGetCurrentState() == (int)ActionState.AS_DEAD)
            return false;

        if (_curBulletNum >= _totalBulletNum)
            return false;
        return true;
    }
    
    public override void OnUpdate(int deltaTime)
    {
        if (CanUpdateCD())
        {
            UpdateAttack(deltaTime);
        }
    }
    private void UpdateAttack(int deltaTime)
    {
        if (_curAttackTime >= _AutoCDTime)
        {
            DoAttack();
        }
        else
        {
            if (_skill != null)
            {
                _skill.LightSkillButton(_curAttackTime > _ManualCDTime && CanUseSkill());
            }
            _curAttackTime += deltaTime;
        }
    }

    public void ShowSpellBar(bool active)
    {
#if !LOGIC_SERVER
        // TODO 等UI换新蓄力条
        if (active)
        {
            owner.StartSpellBar(eDungeonCharactorBar.RayDrop, _AutoCDTime);
        }
        else
        {
            owner.StopSpellBar(eDungeonCharactorBar.RayDrop);
        }
#endif
    }
}*/