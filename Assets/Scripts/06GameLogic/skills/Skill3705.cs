using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

//胜利之矛 将矛攻击到的目标瞬移到指定位置
public class Skill3705 : BeSkill
{
    public Skill3705(int sid, int skillLevel) : base(sid, skillLevel) { }

    private int[] effectIdArr = new int[] { 37050, 37051, 37052, 37053 };     //监听的触发效果ID(Pve非蓄力|蓄力,Pvp非蓄力|蓄力)
    private VInt teleportDis = 0;    //面朝方向的瞬移距离

    private int[] teleportCountArr = new int[2] { 10, 16 };      //瞬移单位数量(不蓄力|蓄力)
    private int[] buffIdArr = new int[2] { 370500, 370501 };    //强制切换状态BuffId
    private int[] buffTimeArr = new int[4] { 700, 1100, 450, 1100};     //Buff时间(Pve非蓄力|蓄力,Pvp非蓄力|蓄力)
    private int effectId = 0;       //当前监听的伤害触发效果ID
    private int maxTeleportCount = 0;
    private int curTeleportCount = 0;   //当前瞬移玩家数量
    private int curBuffId = 0;
    private int curBuffTime = 0;    //Buff时间

    private VInt3 curPos = VInt3.zero;  //释放技能时玩家的位置
    private bool curFace = false;  //释放技能时玩家的朝向

    private int[] equipTeleportCountArr = new int[2];       //装备加成的瞬移玩家数量

    private int protectBuffInfoId = 0;

    public override void OnInit()
    {
        base.OnInit();
        for (int i = 0; i < skillData.ValueA.Count; i++)
        {
            effectIdArr[i] = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
        }
        teleportDis = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueB[0], level), GlobalLogic.VALUE_1000);

        if (skillData.ValueC.Length > 0)
        {
            protectBuffInfoId = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        }
    }

    public override void OnStart()
    {
        curTeleportCount = 0;
        SetEquipAdd();
        curPos = owner.GetPosition();
        curFace = owner.GetFace();
        handleA = owner.RegisterEventNew(BeEventType.onHitOtherAfterHurt, (args) =>
        {
            // int hurtId = (int)args[1];
            // BeActor actor = (BeActor)args[0];
	    int hurtId = args.m_Int;
            BeActor actor = args.m_Obj as BeActor;
            InitChargeData();
            AddSwitchStateBuff(hurtId, actor);
            AddProtectBuff(actor);
        });
    }

    //初始化数据
    private void InitChargeData()
    {
        if (!BattleMain.IsModePvP(battleType))
        {
            effectId = charged ? effectIdArr[1] : effectIdArr[0];
        }
        else
        {
            effectId = charged ? effectIdArr[3] : effectIdArr[2];
        }
        maxTeleportCount = charged ? teleportCountArr[1] + equipTeleportCountArr[1] : teleportCountArr[0] + equipTeleportCountArr[0];
        curBuffId = charged ? buffIdArr[1] : buffIdArr[0];

        if (!BattleMain.IsModePvP(battleType))
        {
            curBuffTime = charged ? buffTimeArr[1] : buffTimeArr[0];
        }
        else
        {
            curBuffTime = charged ? buffTimeArr[3] : buffTimeArr[2];
        }
    }

    //添加一个强制切换人物形态的buff 同时瞬移
    private void AddSwitchStateBuff(int Id, BeActor actor)
    {
        if (Id != effectId)
            return;
        if (actor == null || actor.IsDead())
            return;
        //不能瞬移被抓取的目标
        if (actor.grabController.isAbsorb)
            return;
        //不能瞬移霸体怪物
        if (actor.buffController == null || actor.buffController.HaveBatiBuff() || actor.buffController.HasBuffByID((int)GlobalBuff.GEDANG) != null)
            return;
        if (actor.stateController == null || !actor.stateController.HasBornAbility(BeAbilityType.FALLGROUND) || !actor.stateController.HasBornAbility(BeAbilityType.FLOAT))
            return;
        if (actor.protectManager != null && actor.protectManager.IsInGroundProtect())
            return;
        if (!actor.stateController.HasBuffState(BeBuffStateType.FROZEN) && !actor.stateController.HasBuffState(BeBuffStateType.STONE))
            actor.buffController.TryAddBuff(curBuffId, curBuffTime);
        MoveTo(actor);
    }

    /// <summary>
    /// 移动到某个位置
    /// </summary>
    private void MoveTo(BeActor target)
    {
        VInt3 targetPos = target.GetPosition();
        int dis = Mathf.Abs(curFace ? targetPos.x - curPos.x : curPos.x - targetPos.x);
        if (dis > teleportDis)
            return;
        int time = 200;
        var targetNewPos = curPos;
        targetNewPos.x += curFace ? -teleportDis.i : teleportDis.i;
        targetNewPos.y = targetPos.y;
        if (curPos.z > 0)
            targetNewPos.z = 0;
        BeActionActorFilter filter = new BeActionActorFilter();
        filter.Init(true, true, true, true, true);
        BeMoveTo moveTo = BeMoveTo.Create(target, time, targetPos, targetNewPos, false, filter);
        target.actionManager.RunAction(moveTo);
    } 

    //装备加成计算
    private void SetEquipAdd()
    {
        List<BeMechanism> list = owner.MechanismList;
        if (list == null)
            return;
        equipTeleportCountArr[0] = 0;
        equipTeleportCountArr[1] = 0;
        for (int i = 0; i < list.Count; i++)
        {
            var mechanism = list[i] as Mechanism2024;
            if (mechanism == null)
                continue;
            equipTeleportCountArr[0] = mechanism.addCountArr[0];
            equipTeleportCountArr[1] += mechanism.addCountArr[1];
        }
    }

    private void AddProtectBuff(BeActor target)
    {
        if (!BattleMain.IsModePvP(battleType))
            return;

        if (target == null || protectBuffInfoId == 0 || !charged)
            return;

        // 浮空时
        if (target.HasTag((int)AState.ACS_FALL))
        {
            target.buffController.TryAddBuffInfo(protectBuffInfoId, owner, level);
        }
    }
}
