using System;
using System.Collections.Generic;
using GameClient;


/// <summary>
/// 释放技能被抓取中断时强设AI目标
/// </summary>
public class Mechanism1055 : BeMechanism
{
    public Mechanism1055(int mid, int lv) : base(mid, lv) { }

    private List<int> skillList = new List<int>();    //指定技能ID列表
    private int time = 0; //强制设置AI目标时间(毫秒)
    private List<int> buffInfoList = new List<int>();   //给打断我技能的对象添加Buff信息

    private int curTime = 0;
    private bool assignAITargetFlag = false;

    public override void OnInit()
    {
        for(int i = 0; i < data.ValueA.Count; i++)
        {
            skillList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        time = TableManager.GetValueFromUnionCell(data.ValueB[0],level);
        for(int i = 0; i < data.ValueC.Count; i++)
        {
            buffInfoList.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }
    }

    public override void OnReset()
    {
        skillList.Clear();
        time = 0;
        buffInfoList.Clear();
        curTime = 0;
        assignAITargetFlag = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onBeExcuteGrab, BeExcuteGrab);
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateRestoreAITarget(deltaTime);
    }

    /// <summary>
    /// 恢复AI目标设定
    /// </summary>
    private void UpdateRestoreAITarget(int deltaTime)
    {
        if (!assignAITargetFlag)
            return;
        if (curTime >= time)
        {
            curTime = 0;
            assignAITargetFlag = false;
            owner.aiManager.ForceAssignAiTarget(null);
        }
        else
        {
            curTime += deltaTime;
        }
    }

    /// <summary>
    /// 监听技能被中断
    /// </summary>
    private void BeExcuteGrab(BeEvent.BeEventParam args)
    {
        BeActor graber = args.m_Obj as BeActor;
        if (graber == null)
            return;
        if (!owner.IsCastingSkill())
            return;
        int curSkillId = owner.GetCurSkillID();
        if (!skillList.Contains(curSkillId))
            return;
        SetAITarget(graber);
        AddBuff(graber);
    }

    /// <summary>
    /// 强制设置AI目标
    /// </summary>
    private void SetAITarget(BeActor graber)
    {
        if (time <= 0)
            return;
        curTime = 0;
        assignAITargetFlag = true;
        owner.aiManager.ForceAssignAiTarget(graber);
    }

    /// <summary>
    /// 添加指定Buff
    /// </summary>
    private void AddBuff(BeActor graber)
    {
        if (graber == null)
            return;
        for (int i = 0; i < buffInfoList.Count; i++)
        {
            graber.buffController.TryAddBuffInfo(buffInfoList[i], owner, level);
        }
    }
}

