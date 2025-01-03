using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//圣翼人族-降落机制
public class Mechanism10011 : BeMechanism
{
    public Mechanism10011(int mid, int lv) : base(mid, lv) { }

    private int _castSkillNeedHeight;
    private int _castSkllId;
    private int _skillCD;
    private int _skillCDTimer;
    private bool _checkSkillFlag;

    public override void OnInit()
    {
        _castSkillNeedHeight = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        _castSkllId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        _skillCD = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnStart()
    {
        _checkSkillFlag = true;
        _skillCDTimer = _skillCD;
    }

    public override void OnUpdate(int deltaTime)
    {
        _skillCDTimer += deltaTime;
        if (_skillCDTimer < _skillCD)
            return;
        if (owner.GetPosition().z > _castSkillNeedHeight)
        {
            if (_checkSkillFlag && owner.HasTag((int) AState.ACS_FALL))
            {
                _checkSkillFlag = false;
                _skillCDTimer = 0;
                owner.moveZSpeed = 0;
                owner.SetTag((int) AState.ACS_FALL, false);

                if (_castSkllId > 0)
                {
                    owner.UseSkill(_castSkllId, true);
                }
            }
        }
        else if (!_checkSkillFlag && owner.GetPosition().z == 0)
        {
            _checkSkillFlag = true;
        }
        
    }
    
}
