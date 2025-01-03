using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//元素镰虫-动作改变机制
public class Mechanism10010 : BeMechanism
{
    public Mechanism10010(int mid, int lv) : base(mid, lv) { }

    private bool _stateBroken;
    private VFactor _damageRate;
    private int _canTakeHitTimes;
    private int[] _changeActionsType;
    private string[] _changeActionsName;
    private int _enterBrokenNeedHitCount;
    private int _enterBrokenHitCount;
    private int _switchActionEffect;
    
    private bool _changeHurtActionFlag;

    public override void OnInit()
    {
        _changeActionsType = new int[data.ValueALength];
        _changeActionsName = new string[data.StringValueA.Count];
        
        for (int i = 0; i < data.ValueALength; i++)
        {
            _changeActionsType[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        }
        for (int i = 0; i < data.StringValueA.Count; i++)
        {
            _changeActionsName[i] = data.StringValueA[i];
        }

        _enterBrokenNeedHitCount = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        
        _damageRate = new VFactor(GlobalLogic.VALUE_1000 - TableManager.GetValueFromUnionCell(data.ValueC[0], level),
            GlobalLogic.VALUE_1000);

        _switchActionEffect = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        _stateBroken = false;
        _enterBrokenHitCount = 0;
        _changeHurtActionFlag = true;
        
        handleA = owner.RegisterEventNew(BeEventType.onBeExcuteGrab, param =>
        {
            EnterBrokenState();
        });
        
        handleB = owner.RegisterEventNew(BeEventType.onBeHitAfterFinalDamage, param =>
        {
            if (!_stateBroken)
            {
                param.m_Int *= _damageRate;
                if (owner.GetPosition().z > 0)
                {
                    _enterBrokenHitCount++;

                    if (_enterBrokenHitCount >= _enterBrokenNeedHitCount)
                    {
                        EnterBrokenState();
                    }

                    ChangeHurtAction();
                }
            }
        });
        
        handleC = owner.RegisterEventNew(BeEventType.onGetUp, param =>
        {
            RestoreNormalState();
        });
    }

    private void EnterBrokenState()
    {
        RestoreHurtAction();
        
        if (_stateBroken)
            return;
        _stateBroken = true;
        
        for (int i = 0; i < _changeActionsType.Length; i++)
        {
            ActionType actionType = (ActionType) _changeActionsType[i];
            owner.ReplaceActionName(actionType, _changeActionsName[i]);
            bool forceSwitch = actionType == ActionType.ActionType_FALL_UP;
            if (forceSwitch)
            {
                owner.PlayAction(actionType);
#if !LOGIC_SERVER
                owner.m_pkGeActor.CreateEffect(_switchActionEffect, Vec3.zero);
#endif
            }
        }
    }

    private void RestoreNormalState()
    {
        RestoreHurtAction();
        _enterBrokenHitCount = 0;

        if (!_stateBroken)
            return;
        _stateBroken = false;

        for (int i = 0; i < _changeActionsType.Length; i++)
        {
            owner.RestoreActionName((ActionType) _changeActionsType[i]);
        }
    }

    private void ChangeHurtAction()
    {
        if (_stateBroken || !_changeHurtActionFlag)
            return;

        _changeHurtActionFlag = false;
        owner.ReplaceActionName(ActionType.ActionType_HURT, "Beiji03");
        owner.ReplaceActionName(ActionType.ActionType_HURT1, "Beiji04");
    }

    private void RestoreHurtAction()
    {
        if (_changeHurtActionFlag)
            return;

        _changeHurtActionFlag = true;
        owner.RestoreActionName(ActionType.ActionType_HURT);
        owner.RestoreActionName(ActionType.ActionType_HURT1);
    }
    
}
