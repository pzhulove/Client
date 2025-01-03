using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 关卡气流机制
/// </summary>
public class Mechanism1036 : BeMechanism
{
    /// <summary>
    /// 风向
    /// </summary>
    public enum WindDir
    {
        NONE = -1,
        LEFT = 0,
        RIGHT = 1,
    }

    public Mechanism1036(int id, int level) : base(id, level) { }

    // X轴顺风时增加的速度和逆风时减少的速度
    private VInt[] xAddSpeedArr = new VInt[2];
    // 释放技能时X轴顺风时增加的速度和逆风时减少的速度
    private VInt[] xSkillAddSpeedArr = new VInt[2];
    // 监听的技能ID
    private List<int> skillIdList = new List<int>();
    private int useSkillId = 0; //有风的时候释放的技能ID

    private bool useSkillFlag = false;

    public override void OnInit()
    {
        xAddSpeedArr[0] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        xAddSpeedArr[1] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[1], level), GlobalLogic.VALUE_1000);

        xSkillAddSpeedArr[0] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0], level), GlobalLogic.VALUE_1000);
        xSkillAddSpeedArr[1] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[1], level), GlobalLogic.VALUE_1000);

        for (int i = 0; i < data.ValueC.Count; i++)
        {
            skillIdList.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }
        useSkillId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnReset()
    {
        skillIdList.Clear();
        useSkillFlag = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        if (skillIdList.Count > 0)
        {
            handleA = owner.RegisterEventNew(BeEventType.onCastSkill, OnSkillStart);
            handleB = owner.RegisterEventNew(BeEventType.onCastSkillFinish, OnSkillFinish);
            handleC = owner.RegisterEventNew(BeEventType.onSkillCancel, OnSkillFinish);
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateChangeXSpeed();
    }

    /// <summary>
    /// 监听技能释放
    /// </summary>
    private void OnSkillStart(BeEvent.BeEventParam args)
    {
        if (!skillIdList.Contains(args.m_Int))
            return;
        useSkillFlag = true;
    }

    /// <summary>
    /// 监听技能取消或者结束
    /// </summary>
    private void OnSkillFinish(BeEvent.BeEventParam args)
    {
        if (!skillIdList.Contains(args.m_Int))
            return;
        useSkillFlag = false;
    }

    /// <summary>
    /// 改变X轴的额外速度
    /// </summary>
    private void UpdateChangeXSpeed()
    {
        VInt[] addSpeedArr = useSkillFlag ? xSkillAddSpeedArr : xAddSpeedArr;
        WindDir windDir = GetLevelWindDir();
        int moveSpeed = 0;
        switch (windDir)
        {
            case WindDir.NONE:
                {
                    moveSpeed = 0;
                }
                break;
            case WindDir.LEFT:
                {
                    moveSpeed = owner.GetFace() ? -addSpeedArr[0].i : -addSpeedArr[1].i;
                }
                break;
            case WindDir.RIGHT:
                {
                    moveSpeed = owner.GetFace() ? addSpeedArr[1].i : addSpeedArr[0].i;
                }
                break;
        }
        owner.extraSpeed.x = moveSpeed;
        UseSkill(windDir);
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    private void UseSkill(WindDir windDir)
    {
        if (useSkillId == 0)
            return;
        if (windDir == WindDir.NONE)
            return;
        if (!owner.CanUseSkill(useSkillId))
            return;
        if(windDir == WindDir.LEFT)
        {
            owner.SetFace(true);
        }
        else if(windDir == WindDir.RIGHT)
        {
            owner.SetFace(false);
        }
        if (!owner.IsDead())
        {
            owner.UseSkill(useSkillId);
        }
    }

    /// <summary>
    /// 获取关卡气流方向
    /// </summary>
    private WindDir GetLevelWindDir()
    {
        if (owner.CurrentBeBattle == null)
            return WindDir.NONE;
        if (owner.CurrentBeBattle.LevelMgr == null)
            return WindDir.NONE;
        return (WindDir)owner.CurrentBeBattle.LevelMgr.GetCounter((int)LevelManager.CounterTypeId.WIND_DIR);
    }
}
