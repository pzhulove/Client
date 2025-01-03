using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//圣骑士荣誉祝福装备机制 增加自己和队友的Buff时间(自己给队友添加的buff)
public class Mechanism2029: BeMechanism
{
    public Mechanism2029(int sid, int skillLevel) : base(sid, skillLevel){}

    private List<int> buffIdList = new List<int>();     //影响的BuffId列表
    private List<int> buffTimeAddRateList = new List<int>();     //增加的Buff时长(千分比)
    private List<int> buffTimeAddValueList = new List<int>();     //增加的Buff时长(固定值)

    private List<IBeEventHandle> handleList = new List<IBeEventHandle>();

    public override void OnInit()
    {
        base.OnInit();
        for(int i = 0; i < data.ValueA.Count; i++)
        {
            buffIdList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i],level));
        }

        for(int i = 0; i < data.ValueB.Count; i++)
        {
            buffTimeAddRateList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i],level));
        }

        for(int i = 0; i < data.ValueC.Count; i++)
        {
            buffTimeAddValueList.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }
    }

    public override void OnReset()
    {
        buffIdList.Clear();
        buffTimeAddRateList.Clear();
        buffTimeAddValueList.Clear();
        foreach (var item in handleList)
        {
            item.Remove();
        }
        handleList.Clear();
    }

    public override void OnStart()
    {
        base.OnStart();
        ThisDelayCaller.DelayCall(66,()=> 
        {
            RegisterBuffAdd();
        });
    }

    private void RegisterBuffAdd()
    {
        List<BeActor> list = new List<BeActor>();
        BeUtility.GetAllFriendPlayers(owner, list);
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                BeActor actor = list[i];
                var handle = actor.RegisterEventNew(BeEventType.onAddBuff, (args) =>
                {
                    BeBuff buff = (BeBuff)args.m_Obj;
                    //自己添加的并且是该装备会影响的Buff
                    if(buff != null && owner != null && buff.releaser == owner)
                    {
                        int index = buffIdList.FindIndex(x => { return x == buff.buffID; });
                        if (index != -1)
                        {
                            if (index < buffTimeAddRateList.Count)
                                buff.duration *= VFactor.one + VFactor.NewVFactor(buffTimeAddRateList[index], GlobalLogic.VALUE_1000);
                            if (index < buffTimeAddValueList.Count)
                                buff.duration += buffTimeAddValueList[index];
                        }
                    }
                });
                handleList.Add(handle);
            }
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        for(int i=0;i< handleList.Count; i++)
        {
            if (handleList[i] != null)
            {
                handleList[i].Remove();
                handleList[i] = null;
            }
        }
        handleList.Clear();
    }
}
