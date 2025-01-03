using System.Collections.Generic;
using GameClient;
using ProtoTable;


//守护徽章机制 为队友分担伤害
public class Mechanism2016 : BeMechanism
{
    public Mechanism2016(int mid, int lv) : base(mid, lv) { }

    private VFactor singleHurtRate = VFactor.zero;  //为单个队友每次承担的伤害比例
    private int totalHurtMaxLimit = 0;      //承担的伤害上限

    private int totalHurtValue = 0;     //承担的总的伤害数值
    List<IBeEventHandle> handleList = new List<IBeEventHandle>();

    public override void OnInit()
    {
        base.OnInit();
        singleHurtRate = new VFactor(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        totalHurtMaxLimit = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        totalHurtValue = 0;
        RemoveHandle();
    }

    public override void OnStart()
    {
        base.OnStart();
        SetEquipAdd();
        RegisterHurtEvent();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RemoveHandle();
    }

    private void RegisterHurtEvent()
    {
        if (owner == null)
            return;
        List<BeActor> list = new List<BeActor>();
        BeUtility.GetAllFriendPlayers(owner, list);
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                BeActor actor = list[i];
                if (actor != owner)
                {
                    IBeEventHandle handle = actor.RegisterEventNew(BeEventType.onBeHitAfterFinalDamage, args =>
                    //BeEventHandle handle = actor.RegisterEvent(BeEventType.onBeHitAfterFinalDamage, (object[] args) =>
                    {
                        //int[] value = (int[])args[0];
                        int hurtId = args.m_Int2;
                        if (args.m_Int == 0)
                            return;
                        if (!BeUtility.NeedShareBySGSH(hurtId, actor))
                            return;
                        int shareHurt = args.m_Int * singleHurtRate;
                        if ((totalHurtValue + shareHurt) < totalHurtMaxLimit)
                        {
                            totalHurtValue += shareHurt;
                            args.m_Int -= shareHurt;
                            owner.DoHurt(shareHurt, null, HitTextType.NORMAL, null, HitTextType.FRIEND_HURT);
                        }
                    });
                    handleList.Add(handle);
                }
            }
        }
    }

    private void RemoveHandle()
    {
        for (int i = 0; i < handleList.Count; i++)
        {
            if (handleList[i] != null)
            {
                handleList[i].Remove();
                handleList[i] = null;
            }
        }
        handleList.Clear();
    }

    //装备加成计算
    private void SetEquipAdd()
    {
        BeActor attachBuffReleaser = GetAttachBuffReleaser();
        if (attachBuffReleaser == null)
            return;
        List<BeMechanism> list = attachBuffReleaser.MechanismList;
        if (list == null)
            return;
        for (int i = 0; i < list.Count; i++)
        {
            var mechanism = list[i] as Mechanism2026;
            if (mechanism == null)
                continue;
            totalHurtMaxLimit *= (VFactor.one + mechanism.singleHurtRateAdd);
        }
    }
}
