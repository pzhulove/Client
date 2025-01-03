using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using ProtoTable;

//正义审判
public class Skill3717 : BeSkill
{
    public Skill3717(int sid, int skillLevel) : base(sid, skillLevel) { }

    private int[] summonEntityIdArr = new int[2];     //后跳取消技能时召唤的天使长矛ID(PVE|PVP)
    private int[] hurtIdArr = new int[2];           //天使长矛触发效果ID(PVE|PVP)
    private int[] monsterIdArr = new int[2];      //魔法阵怪物ID（PVE|PVP�?

    private int[] boomEntityIdArr = new int[2] {63654, 63667 };    //(PVE|PVP)

    private int summonPhase = 0;    //召唤的时候的技能阶�?
    private int mechanismTime = 0;    //记录时间 用于伤害计算
    private bool deadFlag = false;  //怪物死亡标志


    private List<IBeEventHandle> handleList = new List<IBeEventHandle>();

    private int maxTime = 3584;     //法阵最大时�?

    public static void SkillPreloadRes(SkillTable tableData)
    {
#if !LOGIC_SERVER
        if (BattleMain.instance == null) return;

        if (BattleMain.IsModePvP(BattleMain.battleType))
            PreloadManager.PreloadResID(TableManager.GetValueFromUnionCell(tableData.ValueA[1], 1), null, null,true);
        else
            PreloadManager.PreloadResID(TableManager.GetValueFromUnionCell(tableData.ValueA[0], 1), null, null,true);
#endif
    }

    public override void OnInit()
    {
        base.OnInit();
        summonEntityIdArr[0] = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        summonEntityIdArr[1] = TableManager.GetValueFromUnionCell(skillData.ValueA[1], level);

        hurtIdArr[0] = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        hurtIdArr[1] = TableManager.GetValueFromUnionCell(skillData.ValueB[1], level);

        monsterIdArr[0] = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        monsterIdArr[1] = TableManager.GetValueFromUnionCell(skillData.ValueC[1], level);

        startEnterNextFlag = "371701";
        endEnterNextFlag = "371702";
    }

    public override void OnStart()
    {
        base.OnStart();
        mechanismTime = 0;
        deadFlag = false;

        RemoveHandles();

        var handle1 = owner.RegisterEventNew(BeEventType.onAfterGenBullet, args => 
        {
            BeProjectile projectile = args.m_Obj as BeProjectile;
            if(projectile!=null && (projectile.m_iResID == boomEntityIdArr[0] || projectile.m_iResID == boomEntityIdArr[1]))
            {
                var handle2 = projectile.RegisterEventNew(BeEventType.onAfterCalFirstDamage, args1 =>
                {
                    int hurtId = args1.m_Int2;
                    ChangeHurtDamage(hurtId, args1);
                });
                handleList.Add(handle2);
            }
        });
        handleList.Add(handle1);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        SetMonsterDead();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        SetMonsterDead();
    }

    public override void OnEnterNextPhase(int phase)
    {
        base.OnEnterNextPhase(phase);
        summonPhase = phase;
        owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
        Cancel();
    }

    //改变天使长矛的伤�?
    private void ChangeHurtDamage(int hurtId, BeEvent.BeEventParam param)
    {
        int curHurtId = BattleMain.IsModePvP(battleType) ? hurtIdArr[1] : hurtIdArr[0];
        if (curHurtId != hurtId)
            return;
        if (summonPhase == 0)
            param.m_Int *= 2;
        else
        {
            VFactor rate = new VFactor(mechanismTime, maxTime);
            param.m_Int += param.m_Int * rate;
        }
    }

    //设置魔法阵怪物死亡
    private void SetMonsterDead()
    {
        if (owner == null || owner.CurrentBeScene == null)
            return;
        if (deadFlag)
            return;
        deadFlag = true;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        int monsterId = BattleMain.IsModePvP(battleType) ? monsterIdArr[1] : monsterIdArr[0];
        owner.CurrentBeScene.FindActorById2(list, monsterId);
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Mechanism23 mechanism = GetMechanism(list[i]);
                if (mechanism != null)
                {
                    mechanismTime = mechanism.GetRunningTime();
                    //list[i].buffController.RemoveBuff(mechanism.attachBuff);
                    mechanism.RemoveAttachBuff();
                }
                list[i].DoDead();
                //召唤爆炸实体
                int entityId = BattleMain.IsModePvP(battleType) ? summonEntityIdArr[1] : summonEntityIdArr[0];
                owner.AddEntity(entityId, list[i].GetPosition());
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
    }

    //获取怪物身上挂载的杀意波动机�?
    private Mechanism23 GetMechanism(BeActor actor)
    {
        if (actor == null || actor.IsDead())
            return null;
        if (actor.MechanismList == null)
            return null;
        Mechanism23 mechanism = null;
        for (int i = 0; i < actor.MechanismList.Count; i++)
        {
            mechanism = actor.MechanismList[i] as Mechanism23;
            if (mechanism != null)
                break;
        }
        return mechanism;
    }

    private void RemoveHandles()
    {
        for(int i=0;i< handleList.Count; i++)
        {
            handleList[i].Remove();
            handleList[i] = null;
        }
        handleList.Clear();
    }
}
