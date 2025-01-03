using System.Collections.Generic;
using GameClient;
using ProtoTable;

//生命源泉技能机制
public class Mechanism2019 : BeMechanism
{
    public Mechanism2019(int mid, int lv) : base(mid, lv) { }

    private List<int> buffInfoList = new List<int>();   //机制添加的时候添加的Buff信息列表
    private List<int> fakeRebornBuffList = new List<int>(); //复活以后添加的Buff信息列表
    private int invincibleBuffTime = 5000;

    private readonly int m_CanNotAttackBuffId = 101;
    private int m_CanNotAttackBuffTime = 0;      //添加不能攻击能力buff时间

    private Dictionary<int, int> buffInfoRadiusAddDic = new Dictionary<int, int>();

    public override void OnInit()
    {
        base.OnInit();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            buffInfoList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        for (int i = 0; i < data.ValueB.Count; i++)
        {
            fakeRebornBuffList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
        invincibleBuffTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        m_CanNotAttackBuffTime = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnReset()
    {
        buffInfoList.Clear();
        fakeRebornBuffList.Clear();
        buffInfoRadiusAddDic.Clear();
    }

    public override void OnStart()
    {
        base.OnStart();
        AddBuffInfo(true);
        SetEquipDataAdd();
        handleB = owner.RegisterEventNew(BeEventType.onHPChange, (args) =>
        {
            if (owner.GetEntityData().GetHP() <= 0)
                FakeReborn();
        });
    }

    public override void OnFinish()
    {
        base.OnFinish();
        AddBuffInfo(false);
    }

    //添加机制时添加的Buff信息
    private void AddBuffInfo(bool isAdd)
    {
        for (int i = 0; i < buffInfoList.Count; i++)
        {
            if (isAdd)
            {
                int buffInfoId = buffInfoList[i];
                BuffInfoData buffInfoData = new BuffInfoData(buffInfoId,level);
                if (buffInfoRadiusAddDic.ContainsKey(buffInfoId))
                    buffInfoData.buffTargetRangeRadius += buffInfoRadiusAddDic[buffInfoId];
                owner.buffController.TryAddBuff(buffInfoData, null, false, new VRate(), GetAttachBuffReleaser());
            }
            else
                owner.buffController.RemoveBuffByBuffInfoID(buffInfoList[i]);
        }
    }

    //假复活
    private void FakeReborn()
    {
        if (owner == null || owner.GetEntityData() == null)
            return;

        //添加FakeReborn事件
        owner.TriggerEventNew(BeEventType.OnFakeReborn, new EventParam(){m_Obj = owner});
        owner.buffController.RemoveAllAbnormalBuff();
        owner.GetEntityData().SetHP(1);
        owner.SetIsDead(false);
        for (int i = 0; i < fakeRebornBuffList.Count; i++)
        {
            BuffInfoData buffInfo = new BuffInfoData(fakeRebornBuffList[i],level);
            owner.buffController.TryAddBuff(buffInfo, null, false, new VRate(), GetAttachBuffReleaser());
        }
        if (m_CanNotAttackBuffTime != 0)
        {
            owner.buffController.TryAddBuff(m_CanNotAttackBuffId, m_CanNotAttackBuffTime);
        }
        owner.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_GET_UP, invincibleBuffTime);
        BeStateData state = new BeStateData((int)ActionState.AS_CASTSKILL) { _StateData = Global.FAKEREBORN_SKILL_ID };
        owner.sgForceSwitchState(state);
        //if (attachBuff != null)
        //    owner.buffController.RemoveBuff(attachBuff);
        RemoveAttachBuff();
    }

    /// <summary>
    /// 设置装备数据加成
    /// </summary>
    private void SetEquipDataAdd()
    {
        BeActor attachBuffReleaser = GetAttachBuffReleaser();
        if (attachBuffReleaser == null)
            return;
        List<BeMechanism> list = attachBuffReleaser.MechanismList;
        for(int i = 0; i < list.Count; i++)
        {
            var mechanism = list[i] as Mechanism2028;
            if (mechanism == null)
                continue;
            for(int j = 0; j < mechanism.impactBuffInfoList.Count; j++)
            {
                int buffInfoId = mechanism.impactBuffInfoList[j];
                int radius = mechanism.buffInfoRadiusAddRateList[j];
                if (!buffInfoRadiusAddDic.ContainsKey(buffInfoId))
                    buffInfoRadiusAddDic.Add(buffInfoId, radius);
                else
                    buffInfoRadiusAddDic[buffInfoId] += radius;
            }
        }
    }
}
