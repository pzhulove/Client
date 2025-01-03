using System.Collections.Generic;

/// <summary>
/// 狂战觉醒被动  
/// </summary>
public class Mechanism149 : BeMechanism
{
    public Mechanism149(int mid, int lv) : base(mid, lv) { }

    int cd = GlobalLogic.VALUE_10000;
    bool inCD = false;
    readonly int entityID = 61001;
    List<int> hpList = new List<int>();
    List<int> rateList = new List<int>();

    public override void OnReset()
    {
        inCD = false;
        hpList.Clear();
        rateList.Clear();
    }
    public override void OnInit()
    {
        base.OnInit(); 
        cd = TableManager.GetValueFromUnionCell(data.ValueA[0], level);

        if (data.ValueC.Count > 0)
        {
            for (int i = 0; i < data.ValueC.Count; i++)
            {
                hpList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));

            }
        }
        if (data.ValueC.Count > 0)
        {
            for (int i = 0; i < data.ValueC.Count; i++)
            {
                rateList.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
            }
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = OwnerRegisterEventNew(BeEventType.onHitOther, args =>
        //handleA = owner.RegisterEvent(BeEventType.onHitOther, (args) =>
        {
            if (inCD || hpList.Count != 4 || rateList.Count != 4) return;
            BeActor target = args.m_Obj as BeActor;
            if (target != null && target.stateController.HasBuffState(BeBuffStateType.BLEEDING))
            {
                VFactor rate = target.GetEntityData().GetHPRate();
                bool hit = false;
                if (rate <= VFactor.NewVFactor(hpList[0], 100))
                {
                    if (owner.FrameRandom.Range100() <= rateList[0])
                        hit = true;
                }
                else if (rate <= VFactor.NewVFactor(hpList[1], 100))
                {
                    if (owner.FrameRandom.Range100() <= rateList[1])
                        hit = true;
                }
                else if (rate <= VFactor.NewVFactor(hpList[2], 100))
                {
                    if (owner.FrameRandom.Range100() <= rateList[2])
                        hit = true;
                }
                else if (rate <= VFactor.NewVFactor(hpList[3], 100))
                {
                    if (owner.FrameRandom.Range100() <= rateList[3])
                        hit = true;
                }
                if (hit)
                {
                    inCD = true;
                    owner.AddEntity(entityID, target.GetPosition(), level, 1200);
                    hit = false;
                }
            }
        });
    }

    int timer = 0;
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (inCD)
        {
            timer += deltaTime;
            if (timer >= cd)
            {
                inCD = false;
                timer = 0;
            }
        }
    }
}
