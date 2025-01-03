using UnityEngine;
using System.Collections;
using GameClient;

public class Skill2004 : BeSkill
{
    int friendBuffID;
    int summonMonsterBuffID;

    BeEvent.BeEventHandleNew handler;

    public Skill2004(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        friendBuffID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        summonMonsterBuffID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
    }

    public override void OnStart()
    {
        RemoveHandler();

        handler = owner.RegisterEventNew(BeEventType.onHitOther, _OnHitOther);
    }
    
    private void _OnHitOther(GameClient.BeEvent.BeEventParam param)
    {
        BeActor actor = param.m_Obj as BeActor;
        int hurtid = param.m_Int;
        ProtoTable.EffectTable data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtid);

        int buffDuration = 0;
        if (data != null)
        {
            int skillLevel = owner.GetSkillLevel(data.SkillID);
            buffDuration = 0;
            if (BattleMain.IsChijiNeedReplaceHurtId(hurtid, battleType))
            {
                var chijiEffectMapTable = TableManager.instance.GetTableItem<ProtoTable.ChijiEffectMapTable>(hurtid);
                buffDuration = TableManager.GetValueFromUnionCell(chijiEffectMapTable.AttachBuffTime, skillLevel);
            }
            else
            {
                buffDuration = TableManager.GetValueFromUnionCell(data.AttachBuffTime, skillLevel);
            }
        }

        if (actor != null)
        {
            if (actor.m_iCamp == owner.m_iCamp)
            {
                //召唤兽
                if (actor.GetEntityData().isSummonMonster)
                {
                    // Logger.LogErrorFormat("try add buff to summonMonster: id {0} time{1}", summonMonsterBuffID, buffDuration);
                    actor.buffController.TryAddBuff(summonMonsterBuffID, buffDuration, level);
                }
                //友军
                else
                {
                    //Logger.LogErrorFormat("try add buff to friend: id {0} time{1}", friendBuffID, buffDuration);
                    actor.buffController.TryAddBuff(friendBuffID, buffDuration, level);
                }
            }
        }
    }

    public override void OnFinish()
    {
        RemoveHandler();
    }

    public override void OnCancel()
    {
        RemoveHandler();
    }

    void RemoveHandler()
    {
        if (handler != null) { 
            handler.Remove();
            handler = null;
        }
    }
}
