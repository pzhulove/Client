using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 给指定的角色加怨念值
/// </summary>
public class Mechanism2003 : BeMechanism
{
    public enum ResentmentType
    {
        ATTACK_TARGET,//攻击目标
        RESENTMENT_HIGH,//怨念值最高
        RESENTMENT_LOW,//怨念值最低
        RANDOM,//随机
        ALL,//全部
        SELF//自己
    }

    private ResentmentType type;
    private int resentmentValue;
    private readonly int mechanismID = 5300;

    private bool stopUpdateResentment = false;
    public Mechanism2003(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        type = (ResentmentType)TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        resentmentValue = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        stopUpdateResentment = TableManager.GetValueFromUnionCell(data.ValueC[0], level)==1;
    }

    public override void OnStart()
    {
        base.OnStart();
        _init();
    }

    /// <summary>
    /// 根据类型改变怨念值
    /// </summary>
    private void _init()
    {
        switch (type)
        {
            case ResentmentType.ATTACK_TARGET:
                handleA = OwnerRegisterEventNew(BeEventType.onHitOther, args =>
                //handleA = owner.RegisterEvent(BeEventType.onHitOther, (args) => 
                {
                    BeActor actor = args.m_Obj as BeActor;
                    if (actor != null && actor.isMainActor)
                    {
                        ChangeActorResentment(actor);
                    }
                });
                break;
            case ResentmentType.RESENTMENT_HIGH:
                ChangeActorResentment(GetResentmentActor());
                break;
            case ResentmentType.RESENTMENT_LOW:
                ChangeActorResentment(GetResentmentActor(false));
                break;
            case ResentmentType.RANDOM:
                ChangePlayerResentment();
                break;
            case ResentmentType.ALL:
                ChangePlayerResentment(false);
                break;
            case ResentmentType.SELF:
                ChangeActorResentment(owner);
                break;
            default:
                break;
        }
    }

    private void ChangePlayerResentment(bool random = true)
    {
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMainActor(list);
        if (random)
        {
            int index = owner.FrameRandom.InRange(0, list.Count - 1);
            ChangeActorResentment(list[index]);
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                ChangeActorResentment(list[i]);
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
    }

    /// <summary>
    /// 停止角色自动增加怨念值
    /// </summary>
    /// <param name="actor"></param>
    private void ChangeActorResentment(BeActor actor)
    {
        if (actor != null)
        {
            Mechanism2004 mechanism = actor.GetMechanism(mechanismID) as Mechanism2004;
            if (mechanism != null)
            {
                if (resentmentValue > 0 && mechanism.IsBetray()) { }
                else
                   mechanism.OnChangeResentmentValue(resentmentValue);
                if (stopUpdateResentment)
                {
                    mechanism.SetUpdateFlag(false);
                }
                else
                {
                    mechanism.SetUpdateFlag(true);
                }
            }
        }
    }

    private BeActor GetResentmentActor(bool high = true)
    {
        return owner.CurrentBeScene.GetResentmentActor(high);
    }
}
