using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 间隔时间 清除攻击列表 用于重复伤害
/// </summary>
public class Mechanism1136 : BeMechanism
{
    public Mechanism1136(int mid, int lv) : base(mid, lv) { }

    private int _hurtId;    //伤害触发效果ID
    private int _repeatTimeAcc; //重复攻击间隔(毫秒)
    private int _repeatAttackNum;   //重复攻击次数

    private int _curAttackTime;
    private int _curTimeAcc;

    public override void OnInit()
    {
        base.OnInit();
        _hurtId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        _repeatTimeAcc = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        _repeatAttackNum = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnReset()
    {
        _curAttackTime = 0;
        _curTimeAcc = 0;
    }

    public override void OnStart()
    {
        _InitData();
    }

    public override void OnUpdateImpactBySpeed(int deltaTime)
    {
        _CheckResetHurtList(deltaTime);
    }

    private void _InitData()
    {
        _curAttackTime = 0;
        _curTimeAcc = _repeatTimeAcc;

        _TriggerEvent();
    }

    /// <summary>
    /// 触发事件 方便装备监听事件修改数值
    /// </summary>
    private void _TriggerEvent()
    {
        /*var _triggerDataArr = new int[2];
        _triggerDataArr[0] = GlobalLogic.VALUE_1000;
        _triggerDataArr[1] = 0;
        owner.TriggerEvent(BeEventType.onRepeatAttackInterval, new object[] { _triggerDataArr, _hurtId });
        _repeatTimeAcc *= VFactor.NewVFactor(_triggerDataArr[0], GlobalLogic.VALUE_1000);
        _repeatAttackNum += _triggerDataArr[1];*/
        
        var eventDate = owner.TriggerEventNew(BeEventType.onRepeatAttackInterval, new EventParam(){m_Int = GlobalLogic.VALUE_1000, m_Int2 = 0, m_Int3 = _hurtId});
        _repeatAttackNum += eventDate.m_Int2;
        _repeatTimeAcc = _repeatTimeAcc * VFactor.NewVFactor(eventDate.m_Int, GlobalLogic.VALUE_1000);
    }

    /// <summary>
    /// 检测攻击列表重置
    /// </summary>
    private void _CheckResetHurtList(int deltaTime)
    {
        if (_curAttackTime >= _repeatAttackNum)
            return;
        _curTimeAcc += deltaTime;
        if(_curTimeAcc >= _repeatTimeAcc)
        {
            _curAttackTime++;
            owner.ResetDamageData();
            owner._calcAttack();
            _curTimeAcc -= _repeatTimeAcc;
        }
    }
}