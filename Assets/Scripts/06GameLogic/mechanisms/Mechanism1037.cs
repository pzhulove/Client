using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

/// <summary>
/// 限时击杀机制
/// </summary>
public class Mechanism1037 : BeMechanism
{
    public Mechanism1037(int id, int level) : base(id, level) { }

    private int limitTime = 0;      //限制时间
    private int limitNum = 0;       //限制数量

    private int curTime = 0;
    private int curNum = 0;

    private bool isEnd = false;
    private TeamDungeonBattleFrame frame = null;

    public override void OnInit()
    {
        limitTime = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        limitNum = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        curTime = 0;
        curNum = 0;
        isEnd = false;
        frame = null;
    }

    public override void OnStart()
    {
        InitFrame();
        handleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onMonsterDead, MonsterDead);
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (curTime >= limitTime)
        {
            SendFightEnd(false);
        }
        else
        {
            RefreshProgress();
            curTime += deltaTime;
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        CloseSpecialFrame();
    }

    /// <summary>
    /// 怪物死亡
    /// </summary>
    private void MonsterDead(BeEvent.BeEventParam args)
    {
        var actor = args.m_Obj as BeActor;
        if (actor == null)
            return;
        if (actor.m_iCamp != owner.m_iCamp)
            return;
        curNum += 1;
        RefreshKillNum();
        if (curNum >= limitNum)
        {
            SendFightEnd(true);
        }
    }

    /// <summary>
    /// 发送关卡通关或者失败的消息
    /// </summary>
    /// <param name="isSuccess"></param>
    private void SendFightEnd(bool isSuccess)
    {
        if (isEnd)
            return;
        isEnd = true;
        if (owner.CurrentBeBattle == null)
        {
            Logger.LogProcessFormat("Battle为空");
            return;
        }
        var battle = owner.CurrentBeBattle as GameClient.PVEBattle;
        if (battle == null)
        {
            Logger.LogProcessFormat("PVEBattle为空");
            return;
        }
        battle.SendLimitDungeonFightEnd(isSuccess);
    }

    /// <summary>
    /// 初始化UI界面
    /// </summary>
    private void InitFrame()
    {
#if !LOGIC_SERVER
        //隐藏关卡时间
        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDungeonScore>();
        if (battleUI != null)
        {
            battleUI.HideFightingTime();
        }

        if (frame == null)
        {
            frame = ClientSystemManager.instance.OpenFrame<TeamDungeonBattleFrame>(FrameLayer.Middle) as TeamDungeonBattleFrame;
        }

        if (frame != null)
        {
            frame.SetKillNum(0, limitNum);
        }
#endif
    }


    private void CloseSpecialFrame()
    {
#if !LOGIC_SERVER
        if (frame != null)
        {
            frame.Close();
        }
#endif
    }

    /// <summary>
    /// 刷新击杀数量
    /// </summary>
    private void RefreshKillNum()
    {
#if !LOGIC_SERVER
        if (frame != null)
        {
            frame.SetKillNum(curNum, limitNum);
        }
#endif
    }

    /// <summary>
    /// 刷新进度条
    /// </summary>
    private void RefreshProgress()
    {
#if !LOGIC_SERVER
        //刷新进度条UI
        if (frame != null)
        {
            frame.SetKillTime(limitTime - curTime);
        }
#endif
    }
}
