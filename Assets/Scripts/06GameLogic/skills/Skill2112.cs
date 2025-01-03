using UnityEngine;
using System.Collections;

public class Skill7274 : Skill2112
{
    public Skill7274(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        distancex = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueA[0], level), GlobalLogic.VALUE_1000);
        distancey = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueA[1], level), GlobalLogic.VALUE_1000);

        effectPath = "Effects/Monster_Renzhe/Prefab/Eff_renzhe_yanwu";
        isCreatePoppet = false;
    }
}

public class Skill2112 : BeSkill
{
    protected string effectPath;
    protected VInt distancex = VInt.zero;
    protected VInt distancey = VInt.zero;
    protected bool isCreatePoppet = false;

    int effectIDFriend = 0;
    int effectIDSummon = 0;

    int[] ids = null;

#if !LOGIC_SERVER
    private IBeEventHandle m_BtnRestoreHandle = null;
#endif

    public Skill2112(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        int entityCount = skillData.ValueA.Count;
        ids = new int[skillData.ValueA.Count];
        for(int i=0; i< entityCount; ++i)
        {
            ids[i] = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
        }

        distancex = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueB[0], level),1000);
        distancey = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueB[1], level),1000);

        effectIDFriend = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        effectIDSummon = TableManager.GetValueFromUnionCell(skillData.ValueC[1], level);

        effectPath = "Effects/Hero_Mage/Caoren/Prefab/Eff_Jiekebaodan_yan";
        isCreatePoppet = true;
    }

    public override void OnPostInit()
    {
#if !LOGIC_SERVER
        if (null != m_BtnRestoreHandle)
            m_BtnRestoreHandle.Remove();
        if (null != owner)
            m_BtnRestoreHandle = owner.RegisterEventNew(BeEventType.onMechanism2050RestoreBtn, (args)=>{
                var canUse = owner.CanUseSkill(skillID);
                if(button != null && button.gameObject.activeInHierarchy != canUse)
                {
                    button.gameObject.CustomActive(canUse);
                }
            });
#endif
    }

    public override void OnStart()
    {
        owner.CurrentBeScene.currentGeScene.CreateEffect(effectPath, 0, owner.GetPosition().vec3);


        var pos = owner.GetPosition();
        pos.z = Mathf.Max(pos.z, VInt.Float2VIntValue(1.5f));

        if (isCreatePoppet)
            CreatePoppet(pos);

        if (owner.IsInMoveDirection())
        {
            // 支持移动摇杆设置稻草人位置
            var dir = InputManager.MoveDir2ToMoveDir(InputManager.GetDir8(owner.MoveDirectionDegree() * 15));
            ChangePositionWithManual(dir);
        }
        else
        {
            BeAIManager.MoveDir dir = (BeAIManager.MoveDir)FrameRandom.InRange(0, (int)BeAIManager.MoveDir.COUNT);
            ChangePosition(dir);
        }
    }

    private void ChangePositionWithManual(BeAIManager.MoveDir dir )
    {
        var pos = owner.GetPosition();
        pos.x += BeAIManager.DIR_VALUE2[(int)dir, 0] * distancex.i;
        pos.y += BeAIManager.DIR_VALUE2[(int)dir, 1] * distancey.i;
        pos.z = 0;
        var desGrid = TestDestination(owner.GetPosition(), pos, (BeAIManager.MoveDir)dir, (int)Mathf.Max(distancex.i/BeAIWalkCommand.grid.x, distancey.i/BeAIWalkCommand.grid.y));
        VInt3 destPos = owner.CurrentBeScene.CalPositionByGrid(desGrid);
        owner.SetPosition(destPos);
        owner.ClearMoveSpeed();
    }
    
    void ChangePosition(BeAIManager.MoveDir d)
    {
        var pos = owner.GetPosition();
        pos.x += BeAIManager.DIR_VALUE2[(int)d, 0] * distancex.i;
        pos.y += BeAIManager.DIR_VALUE2[(int)d, 1] * distancey.i;
        pos.z = 0;

        var start = owner.GetPosition();
        start.z = 0;

		var target = owner.CurrentBeScene.FindTarget(owner, VInt.one.i * 20);
		if (target == null)
			target = owner;
		var dest = GetDestination(owner.GetPosition(), distancex, distancey, target);
        //Logger.LogErrorFormat("pos:start({0},{1}) dest({2},{3}), cal({4},{5})", start.x, start.y, pos.x, pos.y, destPos.x, destPos.y);

		owner.SetPosition(dest);

        owner.ClearMoveSpeed();
    }

	VInt3 GetDestination(VInt3 start, VInt dx, VInt dy, BeEntity target)
	{
		var targetPos = target.GetPosition();
		targetPos.z = 0;
		start.z = 0;

		VInt3 finalDesPos = VInt3.zero;
		int maxDistance = -1;
		for(int i=0; i<(int)BeAIManager.MoveDir.COUNT; ++i)
		{
			var desPos = start;
			desPos.x += BeAIManager.DIR_VALUE2[i, 0] * dx.i;
			desPos.y += BeAIManager.DIR_VALUE2[i, 0] * dy.i;
			desPos.z = 0;

			var desGrid = TestDestination(start, desPos, (BeAIManager.MoveDir)i, (int)Mathf.Max(distancex.i/BeAIWalkCommand.grid.x, distancey.i/BeAIWalkCommand.grid.y));
			VInt3 destPos = owner.CurrentBeScene.CalPositionByGrid(desGrid);
            int distance = (targetPos - destPos).magnitude;
			if (distance > maxDistance)
			{
				maxDistance = distance;
				finalDesPos = destPos;
			}
		}

		return finalDesPos;
	}

    void CreatePoppet(VInt3 pos)
    {
        BeObject obj = owner.CurrentBeScene.AddLogicObject(ids[FrameRandom.InRange(0, 3)], owner.m_iCamp);
        obj.SetPosition(pos);

        IBeEventHandle handler = null;

        handler = obj.RegisterEventNew(BeEventType.onHitOther, (args) =>
        {
			obj.ForceDoDead();

            // BeActor target = args[0] as BeActor;
            BeActor target = args.m_Obj as BeActor;

            if (target != null)
            {
                if (target.GetEntityData().isSummonMonster)
                    AddBuff(target, effectIDSummon);
                else
                    AddBuff(target, effectIDFriend);
            }

            //Logger.LogErrorFormat("add buff to {0}", target.GetName());

            handler.Remove();
        });
    }

    new void AddBuff(BeActor target, int effectID)
    {
        var data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(effectID);
        if (data != null && data.BuffID > 0)
        {
            int buffLevel = level;
            int buffDuration = 0;
            if (BattleMain.IsChijiNeedReplaceHurtId(effectID, battleType))
            {
                var chijiEffectMapTable = TableManager.instance.GetTableItem<ProtoTable.ChijiEffectMapTable>(effectID);
                buffDuration = TableManager.GetValueFromUnionCell(chijiEffectMapTable.AttachBuffTime, buffLevel);
            }
            else
            {
                buffDuration = TableManager.GetValueFromUnionCell(data.AttachBuffTime, buffLevel);
            }
            target.buffController.TryAddBuff(data.BuffID, buffDuration, buffLevel);
        }
    }

	public override bool CanUseSkill()
    {
#if UNITY_EDITOR && DEBUG_SETTING
        if (Global.Settings.m_isQTESkillTest)
            return base.CanUseSkill();
#endif
        return base.CanUseSkill() && CheckSpellCondition((ActionState)owner.sgGetCurrentState())&&!HaveBuff();
    }

    /*
    抓取，冰冻，眩晕，石化，睡眠，束缚下也无法使用
    */
    public override bool CheckSpellCondition(ActionState state)
    {
#if UNITY_EDITOR && DEBUG_SETTING
        if (Global.Settings.m_isQTESkillTest)
            return true;
#endif
        bool flag =
            owner.stateController.HasBuffState(BeBuffStateType.FROZEN) ||
            owner.stateController.HasBuffState(BeBuffStateType.STUN) ||
            owner.stateController.HasBuffState(BeBuffStateType.STONE) ||
            owner.stateController.HasBuffState(BeBuffStateType.SLEEP) ||
            owner.stateController.HasBuffState(BeBuffStateType.STRAIN);

        bool flag2 =
            owner.sgGetCurrentState() == (int)ActionState.AS_GRABBED ||
            owner.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL ||
            owner.sgGetCurrentState() == (int)ActionState.AS_BIRTH ||
            owner.sgGetCurrentState() == (int)ActionState.AS_JUMPBACK ||
            owner.sgGetCurrentState() == (int)ActionState.AS_WIN;

        return !flag2 && !flag && owner.IsInPassiveState() && !owner.stateController.WillBeGrab();
    }
    /// <summary>
    /// 胜利之矛的buff
    /// </summary>
    /// <returns></returns>
    private bool HaveBuff()
    {
        BeBuff buff = owner.buffController.GetBuffList().Find((x) => { return x.buffID == 370500 || x.buffID == 370501; });
        return buff != null;
    }
    DGrid TestDestination(VInt3 start, VInt3 dest, BeAIManager.MoveDir dir, int maxStep)
    {
        DGrid startPos = owner.CurrentBeScene.CalGridByPosition(start);
        DGrid destPos = owner.CurrentBeScene.CalGridByPosition(dest);

        var data = owner.CurrentBeScene.mBlockInfo;

        int xrange = owner.CurrentBeScene.sceneData.GetLogicX();
        int yrange = owner.CurrentBeScene.sceneData.GetLogicZ();

        destPos.x = Mathf.Clamp(destPos.x, 0, xrange - 1);
        destPos.y = Mathf.Clamp(destPos.y, 0, yrange - 1);
        DGrid lastMove = startPos;
        int index = 1;
        do
        {
            int x = (int)startPos.x + BeAIManager.DIR_VALUE2[(int)dir, 0] * index;
            int y = (int)startPos.y + BeAIManager.DIR_VALUE2[(int)dir, 1] * index;
            index++;
			if (x >= 0 && x < xrange && y >= 0 && y < yrange && data[y * xrange + x] == 0 && index<=maxStep )
            {
                DGrid grid = new DGrid(x, y);
                VInt3 pos = owner.CurrentBeScene.CalPositionByGrid(grid);
                if (!owner.CurrentBeScene.IsInBlockPlayer(pos))
                {
                    lastMove = grid;
                    if (x == (int)destPos.x && y == (int)destPos.y)
                        break;
                }
            }
            else
                break;

        } while (true);


        //Logger.LogErrorFormat("start({0},{1}) dest({2},{3}), cal({4},{5})", startPos.x, startPos.y, destPos.x, destPos.y, lastMove.x, lastMove.y);

        return lastMove;
    }

    public override void OnFinish()
    {
        base.OnFinish();
        ClearProtectData();
    }

    /// <summary>
    /// 如果是在倒地状态下释放该技能 则清除保护数据
    /// </summary>
    protected void ClearProtectData()
    {
        if (owner.CurrentBeBattle == null)
            return;
        // if (owner.CurrentBeBattle.FunctionIsOpen(GameClient.BattleFlagType.Skill2112Protect))
        //     return;
        if (!owner.HasTag((int)AState.AST_FALLGROUND) && !owner.HasTag((int)AState.ACS_FALL))
            return;
        owner.protectManager.ClearGroundProtect();
        owner.protectManager.DelayClearFallProtect();
    }

// #if !LOGIC_SERVER
//     private void OnBtnRestore(object[] args)
//     {
//         var canUse = owner.CanUseSkill(skillID);
//         if(button != null && button.gameObject.activeInHierarchy != canUse)
//         {
//             button.gameObject.CustomActive(canUse);
//         }
//     }
// #endif
}
