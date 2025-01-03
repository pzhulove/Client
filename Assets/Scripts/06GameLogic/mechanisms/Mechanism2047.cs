using System;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
//Boss 能量条机制
public class Mechanism2047 : BeMechanism
{
    VInt addSpeed = VInt.zero;
    VInt decSpeed = VInt.zero;
    List<VInt> energyUpLevelValue = new List<VInt>();//能量值上限阀值
    List<int> energyLevelBuffInfo = new List<int>();//能量值上限阀值对应buff信息
    List<int> energyLevelBuff = new List<int>();//能量值上限阀值对应buff
#if !LOGIC_SERVER
    List<UnityEngine.Color> colorList = new List<UnityEngine.Color>();
    private readonly uint[] colorValues = new uint[5] { 2583691392, 88769408, 3343253632, 1627416448, 2583691392};
    private TeamDungeonBattleFrame frame = null;
#endif
    VInt curEnergy = VInt.zero;
    int curLevel = -1;
    VInt initEnergy = 0;
    int curTime = 0;
    public Mechanism2047(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }
    public override void OnInit()
    {
        base.OnInit();
        int speed = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        addSpeed = VInt.NewVInt(speed, GlobalLogic.VALUE_10000);
        speed = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        decSpeed = VInt.NewVInt(speed, GlobalLogic.VALUE_10000);
        initEnergy = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[0], level), GlobalLogic.VALUE_1000);
        if (data.ValueD.Length > 0)
        {
            for(int i = 0; i < data.ValueD.Length;i++)
            {
                energyUpLevelValue.Add(VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueD[i], level),GlobalLogic.VALUE_1000));
            }
        }
        if(data.ValueE.Length > 0)
        {
            for (int i = 0; i < data.ValueE.Length; i++)
            {
                energyLevelBuffInfo.Add(TableManager.GetValueFromUnionCell(data.ValueE[i], level));
            }
        }
        if (data.ValueF.Length > 0)
        {
            for (int i = 0; i < data.ValueF.Length; i++)
            {
                energyLevelBuff.Add(TableManager.GetValueFromUnionCell(data.ValueF[i], level));
            }
        }
        curEnergy = initEnergy;
#if !LOGIC_SERVER
        if (colorValues.Length > 0)
        {
            for (int i = 0; i < colorValues.Length; i++)
            {
                var colorRef = colorValues[i];
                var color = new UnityEngine.Color( ((colorRef >> 24) & 0xff) / 255.0f,
                                                    ((colorRef >> 16) & 0xff) / 255.0f,
                                                    ((colorRef >> 8) & 0xff) / 255.0f,
                                                    (colorRef & 0xff) / 255.0f);
                colorList.Add(color);
            }
        }
#endif
    }

    public override void OnReset()
    {
        energyUpLevelValue.Clear();
        energyLevelBuffInfo.Clear();
        energyLevelBuff.Clear();

#if !LOGIC_SERVER
        colorList.Clear();
        frame = null;
#endif

        curEnergy = VInt.zero;
        curLevel = -1;
        initEnergy = 0;
        curTime = 0;
    }

public override void OnFinish()
    {
        base.OnFinish();
#if !LOGIC_SERVER
        if (frame != null)
        {
            frame.SetBossEnergyBarActive(false);
        }
#endif
    }
    public override void OnStart()
    {
        base.OnStart();
#if !LOGIC_SERVER
        if (frame == null)
        {
            frame = ClientSystemManager.instance.OpenFrame<TeamDungeonBattleFrame>(FrameLayer.Middle) as TeamDungeonBattleFrame;
        }

        if (frame != null)
        {
            frame.SetBossEnergyBarActive(true);
        }
#endif
        if (owner == null || owner.CurrentBeScene == null) return;
    }
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (owner == null || owner.CurrentBeScene == null) return;
        curTime += deltaTime;
        if (curTime < 1000)
        {
            return;
        }
        curTime -= 1000;

        //暴走状态
        if (curLevel == 0 || curLevel + 1 == energyUpLevelValue.Count)
        {
            var buffId = energyLevelBuff[curLevel];
            if (owner.buffController.HasBuffByID(buffId) != null)
            {
#if !LOGIC_SERVER
                if (frame != null)
                {
                    frame.SetBossEnergyValue(curEnergy.i / 100.0f, curLevel);
                }
#endif
                return;
            }
            curEnergy = initEnergy;
        }
        bool isDayTime = owner.CurrentBeScene.IsDayTime();
        //刷新能量值
        if (isDayTime)
        {
            if (curEnergy < VInt.one)
            {
                curEnergy = curEnergy + addSpeed;
                if (curEnergy > VInt.one)
                {
                    curEnergy = VInt.one;
                }
            }
        }
        else
        {
            if (curEnergy > 0)
            {
                curEnergy = curEnergy -  decSpeed;
                if (curEnergy < 0)
                {
                    curEnergy = 0;
                }
            }
        }

        //刷新等级 和对应等级的buff
        int level = curLevel;

        for (int i = 0; i < energyUpLevelValue.Count; i++)
        {
            if (curEnergy <= energyUpLevelValue[i])
            {
                level = i;
                if (curLevel != level)
                {
                    if (curLevel >= 0 && curLevel < energyLevelBuff.Count)
                    {
                        var buffId = energyLevelBuff[curLevel];
                        owner.buffController.RemoveBuff(buffId);
                    }
                    curLevel = level;
                  //  owner.TriggerEvent(BeEventType.onEnergyStatChange,new object[] { curLevel});
                    if(curLevel + 1 == energyUpLevelValue.Count)
                    {
                        owner.TriggerEventNew(BeEventType.onReachMaxEnergy, new EventParam());
                    }
                    if (curLevel >= 0 && curLevel < energyLevelBuffInfo.Count)
                    {
                        var buffInfoId = energyLevelBuffInfo[curLevel];
                        owner.buffController.TryAddBuff(buffInfoId);
                    }
#if !LOGIC_SERVER
                    if (curLevel >= 0 && curLevel < colorList.Count)
                    {
                        if(owner.m_pkGeActor != null)
                            owner.m_pkGeActor.SetEmissiveColor(colorList[curLevel]);
                    }
#endif
                }
                break;
            }
#if !LOGIC_SERVER
            if (frame != null)
            {
                frame.SetBossEnergyValue(curEnergy.i / 100.0f,curLevel);
            }
#endif
        }
      //  Logger.LogErrorFormat("curEnergy {0} addSpeed {1} decSpeed {2} curLevel {3} initEnergy {4}", curEnergy, addSpeed, decSpeed, curLevel, initEnergy);
    }
}
