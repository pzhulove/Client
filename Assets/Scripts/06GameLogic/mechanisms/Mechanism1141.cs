using System.Collections.Generic;
using GameClient;

/// <summary>
/// 替换特效机制
/// </summary>

public class Mechanism1141 : BeMechanism
{
    public Mechanism1141(int mid, int lv) : base(mid, lv) { }

    private List<int> _hurtIdList = new List<int>();
    private List<string> _replaceEffectPathList = new List<string>();

    public override void OnInit()
    {
        base.OnInit();
        _hurtIdList.Clear();
        _replaceEffectPathList.Clear();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            _hurtIdList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
            _replaceEffectPathList.Add(data.StringValueA[i]);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        if (owner.CurrentBeScene == null)
            return;
        handleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onHitEffect, OnChangeHitEffect);
    }

    private void OnChangeHitEffect(BeEvent.BeEventParam param)
    {
        //只监听自己召唤物
        var attacker = param.m_Obj as BeEntity;
        if (attacker == null)
            return;
        if (!owner.IsSameTopOwner(attacker))
            return;
        
        int hurtId = param.m_Int;
        int index = _hurtIdList.FindIndex(x => { return x == hurtId; });
        if (index < 0)
            return;
        param.m_String = _replaceEffectPathList[index];
    }
}