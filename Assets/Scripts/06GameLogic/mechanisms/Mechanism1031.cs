using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制被抓取者随着自己一起移动
/// </summary>
public class Mechanism1031 : BeMechanism
{
    public Mechanism1031(int id, int level) : base(id, level) { }

    private VInt[] offsetArr = new VInt[3];      //相对于自己的位置偏移
    private int skillId = 20182;    //抓取的技能ID 

    private List<BeActor> targetList = new List<BeActor>(); //被抓取的目标列表 

    public override void OnInit()
    {
        base.OnInit();
        offsetArr[0] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        offsetArr[1] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[1], level), GlobalLogic.VALUE_1000);
        offsetArr[2] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[2], level), GlobalLogic.VALUE_1000);

        skillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        targetList.Clear();
    }

    public override void OnStart()
    {
        base.OnStart();
        RegisterExcuteGrab();
        RegisterEndGrab();
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateTargetPos();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        ResetData();
    }

    /// <summary>
    /// 监听执行抓取
    /// </summary>
    private void RegisterExcuteGrab()
    {
        handleA = owner.RegisterEventNew(BeEventType.onExcuteGrab, (args) => 
        {
            int id = args.m_Int;
            if(id== skillId)
            {
                ResetData();
                owner.grabController.GetGrabTargetList(targetList);
            }
        });
    }

    /// <summary>
    /// 监听释放抓取
    /// </summary>
    private void RegisterEndGrab()
    {
        handleB = owner.RegisterEventNew(BeEventType.onEndGrab, (args) =>
        {
            int id = args.m_Int;
            if (id == skillId)
            {
                owner.grabController.grabPos = false;
            }
        });
    }

    /// <summary>
    /// 实时设置目标的位置
    /// </summary>
    private void UpdateTargetPos()
    {
        if (targetList == null)
            return;
        owner.grabController.grabPos = true;
        for (int i = 0; i < targetList.Count; i++)
        {
            BeActor target = targetList[i];
            if (target == null || target.IsDead())
                continue;
            VInt3 targetPos = owner.GetPosition();
            targetPos.x += offsetArr[0].i;
            targetPos.y += offsetArr[1].i;
            targetPos.z += offsetArr[2].i;
            target.SetPosition(owner.GetPosition());
        }
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    private void ResetData()
    {
        targetList.Clear();
    }
}
