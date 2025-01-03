using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class Skill2104 : BeSkill
{
    int eid_owner;
    int eid_summonMonster;
    int eid_summonBattleBuff;

    IBeEventHandle handler;
    IBeEventHandle handler2;
    IBeEventHandle handler3;
    public Skill2104(int sid, int skillLevel):base(sid, skillLevel)
    {
        
    }

    public override void OnInit()
    {
        eid_owner = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        eid_summonMonster = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        eid_summonBattleBuff = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
    }

    public override void OnStart()
    {
        if (IsAlreadyCasted())
        {
            CancelSkill();
        }
        else
        {
            DoWork();
        }
    }

    void DoWork()
    {
        if (eid_owner > 0)
        {
            var data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(eid_owner);
            if (data != null)
            {
                //Logger.LogErrorFormat("try add summoner buff");
                owner.TryAddBuff(data);

                var buff = owner.buffController.HasBuffByID(data.BuffID);
                if (buff != null)
                {
                    buff.RegisterEventNew(BeEventType.onBuffFinish, args =>
                    {
                        CancelSkill();
                    });
                }
            }
        }


		{
			List<BeActor> monsters = GamePool.ListPool<BeActor>.Get();

			owner.CurrentBeScene.GetSummonBySummoner(monsters, owner);
			if (eid_summonMonster > 0 && monsters.Count > 0)
			{
				//Logger.LogErrorFormat("try add summon monster buff");
				var data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(eid_summonMonster);
				for (int i = 0; i < monsters.Count; ++i)
				{
					AddBuff(monsters[i], data);
				}
			}

			GamePool.ListPool<BeActor>.Release(monsters);
		}


        handler = owner.RegisterEventNew(BeEventType.onHit, args =>
        //handler = owner.RegisterEvent(BeEventType.onHit, (object[] args) =>
        {
           // Logger.LogErrorFormat("try add summon monster battle buff");
				List<BeActor> summon = GamePool.ListPool<BeActor>.Get();
				owner.CurrentBeScene.GetSummonBySummoner(summon, owner);
	            if (eid_summonBattleBuff > 0 && summon.Count > 0)
	            {
	                var data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(eid_summonBattleBuff);
	                for (int i = 0; i < summon.Count; ++i)
	                {
                    //summon[i].TryAddBuff(data);
                    int buffTime = 0;
                    if (BattleMain.IsChijiNeedReplaceHurtId(eid_summonBattleBuff, battleType))
                    {
                        var chijiEffectMapTable = TableManager.instance.GetTableItem<ProtoTable.ChijiEffectMapTable>(eid_summonBattleBuff);
                        buffTime = TableManager.GetValueFromUnionCell(chijiEffectMapTable.AttachBuffTime, level);
                    }
                    else
                    {
                        buffTime = TableManager.GetValueFromUnionCell(data.AttachBuffTime, level);
                    }
                    summon[i].buffController.TryAddBuff(data.BuffID, buffTime, level);
                }
	            }

				GamePool.ListPool<BeActor>.Release(summon);
        });

        handler2 = owner.RegisterEventNew(BeEventType.onSummon, (args) =>
        {
            var summon = args.m_Obj as BeActor;
            var data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(eid_summonMonster);
            if (summon != null && data != null)
            {
                //summon.TryAddBuff(data);
                AddBuff(summon, data);
            }
        });

        handler3 = owner.RegisterEventNew(BeEventType.onDead, eventParam =>
        {
            CancelSkill();
        });
    }

    public void AddBuff(BeActor summon, ProtoTable.EffectTable data)
    {
        //summon.TryAddBuff(data);      //因为没有成长 所以更换方式
        int buffTime = 0;
        if (BattleMain.IsChijiNeedReplaceHurtId(data.ID, battleType))
        {
            var chijiEffectMapTable = TableManager.instance.GetTableItem<ProtoTable.ChijiEffectMapTable>(data.ID);
            buffTime = TableManager.GetValueFromUnionCell(chijiEffectMapTable.AttachBuffTime, level);
        }
        else
        {
            buffTime = TableManager.GetValueFromUnionCell(data.AttachBuffTime, level);
        }
        summon.buffController.TryAddBuff(data.BuffID, buffTime, level);
        var buff = summon.buffController.HasBuffByID(data.BuffID);
        if (buff != null)
        {
			summon.aiManager.SetForceFollow(true);

			summon.RegisterEventNew(BeEventType.onBuffFinish, args =>
            {
                int buffID = args.m_Int;
				if (buffID == data.BuffID)
				{
					summon.aiManager.SetForceFollow(false);
				}
            });

			summon.RegisterEventNew(BeEventType.onRemoveBuff, (args) =>
			{
				int buffID = (int)args.m_Int;
				if (buffID == data.BuffID)
				{
					summon.aiManager.SetForceFollow(false);
				}
			});
        }
    }

    bool IsAlreadyCasted()
    {
        var data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(eid_owner);
        if (data != null)
        {
            return owner.buffController.HasBuffByID(data.BuffID) != null;
        }

        return false;
    }

    void CancelSkill()
    {
        var data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(eid_owner);
        if (data != null)
        {
            owner.buffController.RemoveBuff(data.BuffID);
        }

        data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(eid_summonMonster);
        if (data != null)
        {
            var sdata = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(eid_summonMonster);

			List<BeActor> monsters = GamePool.ListPool<BeActor>.Get();

			owner.CurrentBeScene.GetSummonBySummoner(monsters, owner);
            for(int i=0; i<monsters.Count; ++i)
            {
                monsters[i].buffController.RemoveBuff(sdata.BuffID);
            }

			GamePool.ListPool<BeActor>.Release(monsters);
        }

        RemoveHandler();
    }

    void RemoveHandler()
    {
        if (handler != null) {
            handler.Remove();
            handler = null;
        }

        if (handler2 != null)
        {
            handler2.Remove();
            handler2 = null;
        }

        if (handler3 != null)
        {
            handler3.Remove();
            handler3 = null;
        }
    }
}
