//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class Skill1103 : BeSkill {

//	IBeEventHandle handler = null;
//	List<BeActor> deadMonsterList = new List<BeActor>();
//	string deadEffect = "Effects/Hero_Manyou/Fengkuangtulu/Prefab/Eff_fktl_baozha";

//	public Skill1103(int sid, int skillLevel):base(sid, skillLevel)
//	{

//	}

//	public override void OnStart ()
//	{
//		deadMonsterList.Clear();
//		RemoveHandler();
//        handler = owner.RegisterEventNew(BeEventType.onKill, args => 
//        {
//            //handler = owner.RegisterEvent(BeEventType.onKill, (object[] args)=>{
//			BeActor deadMonster = args.m_Obj as BeActor;
//			if (deadMonster != null)
//			{
//				//deadMonster.m_pkGeActor.ChangeSurface("血之狂暴", 2);
//				deadMonster.showDamageNumber = false;
//				owner.delayCaller.DelayCall(100, ()=>{
//					deadMonster.Pause(1000000);
//				});
//				deadMonsterList.Add(deadMonster);
//			}
//		});
//	}

//	public override void OnEnterPhase (int phase)
//	{
//		if (phase == 12)
//		{
//			if (deadMonsterList.Count > 0)
//			{
//				owner.delayCaller.DelayCall(350*2, ()=>{
//					DoExplodeDeadMonster();
//				});
//			}
//			else {
//				Cancel();
//				owner.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
//			}
//		}
//	}

//	public override void OnFinish ()
//	{
//		Restore();
//	}

//	public override void OnCancel ()
//	{
//		Restore();
//		for(int i=0; i<deadMonsterList.Count; ++i)
//		{
//			deadMonsterList[i].Resume();
//		}
//	}

//	public void DoExplodeDeadMonster()
//	{
//		for(int i=0; i<deadMonsterList.Count; ++i)
//		{
//			var monster = deadMonsterList[i];
//			var pos = monster.GetPosition();

//#if !LOGIC_SERVER
 

//			owner.CurrentBeScene.currentGeScene.CreateEffect(deadEffect, 0, pos.vec3);

// #endif

//			owner.delayCaller.DelayCall(300, ()=>{
//				monster.Resume();
//				monster.DoDead();
//			});

//		}
//	}

//	void Restore()
//	{
//		RemoveHandler();
//	}

//	void RemoveHandler()
//	{
//		if (handler != null)
//		{
//			handler.Remove();
//			handler = null;
//		}
//	}
//}
