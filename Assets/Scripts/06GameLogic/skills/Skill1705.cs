using UnityEngine;
using System.Collections;

public class Skill1705 : BeSkill
{
	int buffID;
	int nextPhaseSkillID = 0;
	IBeEventHandle handler;
	IBeEventHandle handler2;
	int skillPhase = 0;

	string effect = "Effects/Common/Sfx/DiaoLuo/Eff_jinbi_tuowei";
	int tmpCount = 0;

	public Skill1705(int sid, int skillLevel):base(sid, skillLevel)
	{

	}

	public override void OnInit ()
	{
		if (skillData != null)
		{
			buffID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
			nextPhaseSkillID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
		}
	}

	public override void OnStart ()
	{
		if (skillPhase == 0)
		{
			skillPhase = 1;
			DoPhase1();
		}
		else if (skillPhase == 1)
		{
			
		}
	}

	void DoPhase1()
	{
		//Logger.LogErrorFormat("魔狱血煞 阶段1！！！！！");

		//buff时间到
		handler = owner.RegisterEventNew(BeEventType.onRemoveBuff, (args)=>{
			// int removedBuffID = (int)args[0];
            var removedBuffID = args.m_Int;
			if (removedBuffID == buffID)
			{
				PressAgainRelease();
			}
		});

		//吸血球
#if !SERVER_LOGIC 

		handler2 = owner.RegisterEventNew(BeEventType.onHitOther, (args)=>{
			// BeActor target = args[0] as BeActor;
            BeActor target = args.m_Obj as BeActor;
			if (target != null && target.buffController.HasBuffByType(BuffType.BLEEDING) != null)
			{
				tmpCount++;
				var startPos = target.GetPosition();
				var curPos = owner.GetPosition();

				Vector3 startV = new Vector3(15, 0, 8);
				if (tmpCount % 2 == 0){
					startV.z *= -1;
					startV.y = 10;
				}
				else {
					startV.y = -2;
				}
					
				if (startPos.x < curPos.x)
					startV.x *= -1;

				var trail = TrailManager.AddParabolaTrail(
					target.GetGePosition() + new Vector3(0, 0.6f, 0),
					owner,
					startV,
					Vector3.zero,
					effect);

				trail.totalDist = 20;
				trail.TotalTime = 2000;
			}
		});

 #endif

	}
		
	void DoReleaseNextSkill()
	{
		//Logger.LogErrorFormat("魔狱血煞 阶段2！！！！！");
		if (nextPhaseSkillID > 0)
		{
			//Logger.LogErrorFormat("魔狱血煞 阶段2释放技能成功！！！！！");

			DoFinish();

			owner.delayCaller.DelayCall(100, ()=>{
				
				owner.UseSkill(nextPhaseSkillID, true);
			});
		}
	}

	void DoFinish()
	{
		RemoveHandler();
		skillPhase = 0;
		owner.buffController.RemoveBuff(buffID);
		for(int i=0; i<skillData.ValueC.Count; ++i)
		{
			int tmpBuffID = TableManager.GetValueFromUnionCell(skillData.ValueC[i], level);
			if (tmpBuffID > 0)
			{
				owner.buffController.RemoveBuff(tmpBuffID);
			}
		}
		//Logger.LogErrorFormat("魔狱血煞 恢复阶段0！！！！！");
	}

	void RemoveHandler()
	{
		if (handler != null)
		{
			handler.Remove();
			handler = null;
		}

		if (handler2 != null)
		{
			handler2.Remove();
			handler2 = null;
		}
	}

	public override void OnCancel ()
	{
		DoFinish();
	}

	public override void OnClickAgain ()
	{
		DoReleaseNextSkill();
	}
}
