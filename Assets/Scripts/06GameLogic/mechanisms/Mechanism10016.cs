using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//怪物连线角色机制
//有任意玩家进入此一定范围内，就会立即在该玩家与该怪物之间产生连线
//如果玩家身上有某Buff，且连线超过一定时间，则该怪物死亡，同时给玩家添加某BuffInfo
public class Mechanism10016 : BeMechanism
{
	class NodeInfo
    {
		public int pid;
		public int timer;
		public GeEffectEx effect;

		public NodeInfo(int pid, GeEffectEx effect)
        {
			this.pid = pid;
			this.effect = effect;
			timer = 0;
        }

		public void SetEffectActive(bool active)
        {
			if (effect != null)
			{
				effect.SetVisible(active);
				if (active)
				{
					var goEffect = effect.GetRootNode();
					var bind = goEffect.GetComponentInChildren<ComCommonBind>();
					if (bind != null)
					{
						var com = bind.GetCom<LightningChain>("lcScript");
						if (com != null)
						{
							com.ForceUpdate();
						}
					}
				}
			}
		}

		public void Remove()
        {
			if (effect != null)
            {
				effect.Remove();
				effect = null;
            }
        }
    }

    string chainEffect;
	int distance;
	int time;
    int buffID;
    int addBuffInfoID;
	VInt defaultHeight = 0;

	List<NodeInfo> nodeInfoList = new List<NodeInfo>();

	readonly string strNode = "[actor]Body";

	public Mechanism10016(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
		OnFinish();
	}

    public override void OnInit()
    {
        chainEffect = data.StringValueA[0];
		distance = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000).i;
		time = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
		buffID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        addBuffInfoID = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
		var playerList = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < playerList.Count; i++)
        {
			var target = playerList[i].playerActor;
			if (target != null)
			{
				var effect = AddChain(owner, target);
				var node = new NodeInfo(target.GetPID(), effect);
				nodeInfoList.Add(node);
			}
		}
		OnUpdate(0);
	}

    public override void OnUpdate(int deltaTime)
    {
		for (int i = 0; i < nodeInfoList.Count; i++)
		{
			var node = nodeInfoList[i];
			var target = owner.CurrentBeScene.GetEntityByPID(node.pid) as BeActor;
			if (target != null)
			{
				if ((owner.GetPosition() - target.GetPosition()).magnitude < distance)
				{
					node.SetEffectActive(true);

					var buff = target.buffController.HasBuffByID(buffID);
					if (buff != null)
                    {
						node.timer += deltaTime;
						if (node.timer >= time)
                        {
							target.buffController.RemoveBuff(buff);
							target.buffController.TryAddBuff(addBuffInfoID);
							owner.DoDead();
						}
					}
					else
                    {
						node.timer = 0;
                    }
				}
				else
				{
					node.SetEffectActive(false);
				}
			}
		}
	}

	protected GeEffectEx AddChain(BeActor fromActor, BeActor toActor)
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
		return effect;
	}

	protected GameObject GetAttachGameObject(BeActor actor, string nodeName, ref bool noStrNode)
	{
		GameObject attachRoot = null;
#if !LOGIC_SERVER
		if (actor == null || actor.m_pkGeActor == null)
		{
			return null;
		}

		attachRoot = actor.m_pkGeActor.GetAttachNode(nodeName);
		if (attachRoot == null)
		{
			noStrNode = true;
			attachRoot = actor.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
		}
#endif
		return attachRoot;
	}

    public override void OnFinish()
    {
        for (int i = 0; i < nodeInfoList.Count; i++)
        {
			nodeInfoList[i].Remove();
        }
		nodeInfoList.Clear();
	}

}
