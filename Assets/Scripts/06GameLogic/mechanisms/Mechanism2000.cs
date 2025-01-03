using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//异界装备机制 改变白虎的发射间隔
public class Mechanism2000 : BeMechanism
{
    public Mechanism2000(int mid, int lv) : base(mid, lv) { }

    protected int[] reduceCDTimeArr =  new int[4];        //减少的发射时间间隔
    protected readonly int mechanismId = 5186;   //白虎机制
    protected readonly int mechanismMonsterId = 93030031; //白虎机制挂载怪物ID  
    protected BeActor mechanismMonster = null;
	protected List<IBeEventHandle> handleList = new List<IBeEventHandle>();

    public override void OnInit()
    {
        base.OnInit();
        if(data.ValueA.Count > 0)
        {
            for (int i = 0; i < data.ValueA.Count; i++)
            {
                reduceCDTimeArr[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            }
        }
    }

    public override void OnReset()
    {
        reduceCDTimeArr = new int[4];
        mechanismMonster = null;
        removeHandle();
    }

    public override void OnStart()
    {
        base.OnStart();
        if (owner.CurrentBeScene == null)
            return;;
        FindMechanismMonster();
        sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onSummon, (args) =>
        {
            BeActor monster = (BeActor)args.m_Obj;
            if (monster != null && monster.GetEntityData().monsterID == mechanismMonsterId && monster.m_iCamp == owner.m_iCamp)
            {
                var handle = monster.RegisterEventNew(BeEventType.onAddMechanism, args1 =>
                {
                    int id = args1.m_Int;
                    if(id == mechanismId)
                    {
                        mechanismMonster = monster;
                        ChangeCDTime(true);
                    }
                });
                handleList.Add(handle);
            }
        });
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RestoreCDTime();
        removeHandle();
    }

    void removeHandle()
    {
        for (int i = 0; i < handleList.Count; i++)
        {
            var handle = handleList[i];
            if (handle != null)
            {
                handle.Remove();
                handle = null;
            }
        }
    }

    //寻找白虎机制挂载怪物
    protected void FindMechanismMonster()
    {
        if (mechanismMonster != null)
            return;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMonsterByIDAndCamp(list, mechanismMonsterId, owner.GetCamp());
        if (list != null && list.Count > 0)
        {
            mechanismMonster = list[0];
        }
        GamePool.ListPool<BeActor>.Release(list);
        ChangeCDTime();
    }

    //恢复设置
    protected void RestoreCDTime()
    {
        if (mechanismMonster == null)
            return;
        ChangeCDTime();
    }

    //改变CD时间
    protected void ChangeCDTime(bool isAdd = false)
    {
        if (mechanismMonster == null)
            return;
        Mechanism140 mechanism = mechanismMonster.GetMechanism(mechanismId) as Mechanism140;
        if (mechanism == null)
            return;
        for(int i= 0;i< reduceCDTimeArr.Length; i++)
        {
            if(isAdd)
                mechanism.createCDArr[i] -= reduceCDTimeArr[i];
            else
                mechanism.createCDArr[i] += reduceCDTimeArr[i];
        }
    }
}
