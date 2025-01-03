
using ProtoTable;
using System.Collections.Generic;
using GameClient;

//给目标添加混乱和可以攻击队友的能力
public class Mechanism1018 : BeMechanism
{
    public Mechanism1018(int mid, int lv) : base(mid, lv) { }

    private List<int> monsterTypeListPveList = new List<int>();    //受到影响的怪物类型列表
    private List<int> buffTimeList = new List<int>();           //Pve Buff存在时间列表
    private List<int> buffAddRateList = new List<int>();        //Pve Buff添加概率列表
    private int buffInfoId = 0;     //添加的BuffInfoId

    public static void MechanismPreloadRes(MechanismTable tableData)
    {
#if !LOGIC_SERVER
        PreloadManager.PreloadBuffInfoID(TableManager.GetValueFromUnionCell(tableData.ValueD[0],1),null,null);
#endif
    }

    public override void OnInit()
    {
        base.OnInit();
        monsterTypeListPveList.Clear();
        buffTimeList.Clear();
        buffAddRateList.Clear();
        
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            monsterTypeListPveList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }

        for (int i = 0; i < data.ValueB.Count; i++)
        {
            buffTimeList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }

        for (int i = 0; i < data.ValueC.Count; i++)
        {
            buffAddRateList.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }

        buffInfoId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        SetEquipAdd();
        handleA = OwnerRegisterEventNew(BeEventType.onHitOther, args =>
        //handleA = owner.RegisterEvent(BeEventType.onHitOther, (object[] args) =>
        {
            BeActor target = args.m_Obj as BeActor;
            AddBuff(target);
        });
    }

    private void AddBuff(BeActor target)
    {
        if (target == null || target.IsDead())
            return;
        int monsterType = target.GetEntityData().type;
        int index = monsterTypeListPveList.FindIndex(x => { return x == monsterType; });
        if (index == -1)
            return;
        BuffInfoData buffInfo = new BuffInfoData(buffInfoId);
        buffInfo.duration = buffTimeList[index];
        buffInfo.prob = buffAddRateList[index];

        target.buffController.TryAddBuff(buffInfo, null, false,new VRate(), GetAttachBuffReleaser());

    }

    //装备加成计算
    private void SetEquipAdd()
    {
        List<BeMechanism> list = owner.MechanismList;
        if (list == null)
            return;
        for (int i = 0; i < list.Count; i++)
        {
            var mechanism = list[i] as Mechanism2025;
            if (mechanism == null)
                continue;
            for(int j=0;j< buffAddRateList.Count; j++)
            {
                buffAddRateList[j] += mechanism.addRate;
            }

            for (int j = 0; j < buffTimeList.Count; j++)
            {
                buffTimeList[j] += mechanism.addBuffTime;
            }
        }
    }
}
