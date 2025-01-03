using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 机制描述:闪电链
*/

public class Mechanism26 : BeMechanism {

	int chainMaxCount = 7;
	string chainEffect = null;//"Effects/Chongwu/Perfab/sunlinglong_wanqianshenguang/sunlinglong_wanqianshenguang";
	VInt chainMaxDistance = VInt.one.i * 5;

	int hurtID = 15000;
	int attackCount = 4;

	readonly int attackDuration = GlobalLogic.VALUE_1000;
	readonly int CHAIN_NEXT_INTERVAL = GlobalLogic.VALUE_100;
	readonly int delay = 800;
	readonly string strNode = "[actor]Body";

	readonly VInt defaultHeight = VInt.Float2VIntValue(0.8f);
    GameObject attachRoot = null;

	enum ChainStat
	{
		NONE = 0,
		CHAINING = 1,
		FINISH = 2,
		OVER = 3
	}

	ChainStat stat = ChainStat.NONE;

	struct ChainTarget
	{
		public BeActor target;
		public GeEffectEx effect;
		public IBeEventHandle handle;
	}
		
	List<ChainTarget> targetList = new List<ChainTarget>();

	public Mechanism26(int mid, int lv):base(mid, lv){}

	DelayCaller delayCaller = new DelayCaller();

	public override void OnReset()
	{
		chainMaxCount = 7;
		chainEffect = null;
		chainMaxDistance = VInt.one.i * 5;
		hurtID = 0;
		attackCount = 0;
		stat = ChainStat.NONE;
		targetList.Clear();
	}

	public override void OnInit ()
	{
		chainEffect = data.StringValueA[0];
		chainMaxCount = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		chainMaxDistance = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0], level),GlobalLogic.VALUE_1000);
		hurtID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
		attackCount = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
	}

	public override void OnStart ()
	{
		OnFinish();

		stat = ChainStat.CHAINING;

		var firstTarget = FindTarget(owner.GetPosition(), true);
		if (firstTarget != null)
		{
			AddChain(owner, firstTarget);

			owner.delayCaller.DelayCall(CHAIN_NEXT_INTERVAL, ()=>{
				ChainNext();
			});

		}
		else{
			//找不到面向的攻击对象
			#if !LOGIC_SERVER
			var effect = owner.m_pkGeActor.CreateEffect(chainEffect, null, attackDuration/(float)(GlobalLogic.VALUE_1000), Vec3.zero);

			var goEffect = effect.GetRootNode();

			bool noStrNode = false;
			var go = GetAttachGameObject(owner, strNode, ref noStrNode);

			Battle.GeUtility.AttachTo(goEffect, go);

			if (noStrNode)
			{
				var effPos = effect.GetPosition();
				effPos.z += defaultHeight.scalar;
				effect.SetPosition(effPos);
			}

			#endif
			stat = ChainStat.FINISH;
		}
	}

	public override void OnUpdate (int deltaTime)
	{
		delayCaller.Update(deltaTime);

		if (stat == ChainStat.FINISH)
		{
			stat = ChainStat.OVER;
			owner.delayCaller.DelayCall(delay, ()=>{
				Clear();	
			});
		}
	}

	protected void GetCurrentTargets(List<BeActor> list)
	{
		list.Clear();
		for(int i=0; i<targetList.Count; ++i)
		{
			list.Add(targetList[i].target);
		}
	}

	protected void ChainNext()
	{
		if (targetList.Count >= chainMaxCount || targetList.Count <= 0)
		{
			stat = ChainStat.FINISH;
			return;
		}

		var target = FindTarget(targetList[targetList.Count-1].target.GetPosition());
		if (target != null)
		{
			AddChain(targetList[targetList.Count-1].target, target);

			delayCaller.DelayCall(CHAIN_NEXT_INTERVAL, ()=>{
				ChainNext();
			});

		}
		else {
			stat = ChainStat.FINISH;
		}
	}

	protected BeActor FindTarget(VInt3 pos, bool first=false)
	{
		if (first)
		{
			//var target = owner.CurrentBeScene.FindNearestFacedTarget(owner, new VInt2(chainMaxDistance, chainMaxDistance));

			var target = owner.CurrentBeScene.FindNearestRangeTarget(pos, owner, chainMaxDistance);

			return target;
		}
		else {

			var list = GamePool.ListPool<BeActor>.Get();
			GetCurrentTargets(list);

			var target = owner.CurrentBeScene.FindNearestRangeTarget(pos, owner, chainMaxDistance, list);

			GamePool.ListPool<BeActor>.Release(list);

			return target;
		}
	
		return null;
	}

	protected GameObject GetAttachGameObject(BeActor actor, string nodeName, ref bool noStrNode)
	{

#if !SERVER_LOGIC
        if (actor == null || actor.m_pkGeActor == null)
            return null;
        attachRoot = actor.m_pkGeActor.GetAttachNode(nodeName);
        if (attachRoot == null)
        {
            noStrNode = true;
            attachRoot = actor.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
        }

#endif


        return attachRoot;
	}

	protected void AddChain(BeActor fromActor, BeActor toActor)
	{
		
		var effect = owner.m_pkGeActor.CreateEffect(chainEffect, null, 99999, Vec3.zero);

		#if !LOGIC_SERVER
		var goEffect = effect.GetRootNode();

		bool noStrNode = false;
		var go = GetAttachGameObject(fromActor, strNode, ref noStrNode);

		Battle.GeUtility.AttachTo(goEffect, go);

		if (noStrNode)
		{
			var effPos = effect.GetPosition();
			effPos.z += defaultHeight.scalar;
			effect.SetPosition(effPos);
		}
			
		var bind = goEffect.GetComponentInChildren<ComCommonBind>();
		if (bind != null)
		{
			var com = bind.GetCom<LightningChain>("lcScript");
			if (com != null)
			{
				bool hasStrNode = false;
				com.target = GetAttachGameObject(toActor, strNode, ref hasStrNode);
				com.ForceUpdate();
			}

			GameObject goNodeA = bind.GetGameObject("goNodeA");
			if (goNodeA != null)
			{
				var comOffset = goNodeA.GetComponent<OffsetChange>();
				if (comOffset == null)
				{
					comOffset = goNodeA.AddComponent<OffsetChange>();
					comOffset.LoopCount = 0;
					comOffset.AStartTime = 0;
					comOffset.AXSpeed = -5;
					comOffset.AYSpeed = 0;
					comOffset.BStartTime = 0;
					comOffset.BXSpeed = 0;
					comOffset.BYSpeed = 0;
				}
			}
		}
		#endif
		int attackInterval = IntMath.Float2Int(attackDuration/(float)(attackCount));

		if (attackCount <= 1) {
			var hitPos = toActor.GetPosition();
			hitPos.z += VInt.one.i;

			owner._onHurtEntity(toActor, hitPos, hurtID);
		} else {
			delayCaller.RepeatCall(attackInterval, ()=>{

				var hitPos = toActor.GetPosition();
				hitPos.z += VInt.one.i;

				owner._onHurtEntity(toActor, hitPos, hurtID);

			}, attackCount, true);
		}



		//不让怪物死
		var handle = toActor.RegisterEventNew(BeEventType.onDead, eventParam =>
		{
            bool isForce = eventParam.m_Bool2;
            if (!isForce)
            {
				eventParam.m_Bool = false;
            }
		});

		var element = new ChainTarget();
		element.effect = effect;
		element.target = toActor;
		element.handle = handle;

		targetList.Add(element);
	}


	public override void OnFinish ()
	{
		delayCaller.Clear();
		Clear();
	}

	protected void Clear()
	{
		
		for(int i=0; i<targetList.Count; ++i)
		{
			var chainTarget = targetList[i];
			chainTarget.handle.Remove();

            //为了解决闪电链特效不消失的bug 将特效销毁代码移动到上面
#if !LOGIC_SERVER
            try
            {
                var effect = chainTarget.effect;
                var obj = effect.GetRootNode();

                if (obj != null)
                {
                    var bind = obj.GetComponentInChildren<ComCommonBind>();
                    if (bind != null)
                    {
                        var com = bind.GetCom<LightningChain>("lcScript");
                        com.target = null;
                        com.SetVertexCount(0);
                    }
                }

                owner.m_pkGeActor.DestroyEffect(effect);
            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR && !LOGIC_SERVER
                Logger.LogErrorFormat("mechanism26 Clear err:{0}", e.ToString());
#endif
            }
#endif

            if (chainTarget.target.GetLifeState() != (int)EntityLifeState.ELS_ALIVE)
				continue;

			if (chainTarget.target.IsDead() || (chainTarget.target.GetEntityData() != null && chainTarget.target.GetEntityData().GetHP()<=0))
				chainTarget.target.DoDead();
		}

		targetList.Clear();
	}

}
