using UnityEngine;
using System.Collections;
using ProtoTable;
using GameClient;

//2019.7月职业平衡，仅对雷光珠技能做特殊处理，雷光珠出发效果dealhit仅对浮空状态敌人生效
public class Skill2007 : BeSkill
{
    private BeEvent.BeEventHandleNew hitEffectHandle = null;

    public Skill2007(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnStart()
    {
        if (BattleMain.IsModePvP(owner.battleType))
        {
            RemoveHandler();

            hitEffectHandle = owner.RegisterEventNew(BeEventType.onHitEffect, _OnHitEffect);

            //hitEffectHandle = owner.RegisterEvent(BeEventType.onHitEffect, (object[] args) =>
            //{
            //    bool[] hittEffects = (bool[])args[0];
            //    int hurtid = (int)args[1];
            //    EffectTable hurtData = args[2] as EffectTable;
            //    BeEntity pkEntity = args[3] as BeEntity;
            //    if (hurtData == null || pkEntity == null)
            //    {
            //        return;
            //    }
            //    if ((hurtid == 20070 || hurtid == 20071) && !(hurtData.HitDeadFall > 0 && pkEntity.IsDead()) && !(pkEntity.HasTag((int)AState.ACS_FALL) && !pkEntity.HasTag((int)AState.AST_FALLGROUND) || pkEntity.GetPosition().z > 0))
            //    {
            //        hittEffects[0] = true;
            //    }
            //});
        }
    }

    private void _OnHitEffect(BeEvent.BeEventParam param)
    {
        bool hittEffects = param.m_Bool;
        int hurtid = param.m_Int;
        EffectTable hurtData = param.m_Obj as EffectTable;
        BeEntity pkEntity = param.m_Obj2 as BeEntity;
        if (hurtData == null || pkEntity == null)
        {
            return;
        }
        if ((hurtid == 20070 || hurtid == 20071) && !(hurtData.HitDeadFall > 0 && pkEntity.IsDead()) && !(pkEntity.HasTag((int)AState.ACS_FALL) && !pkEntity.HasTag((int)AState.AST_FALLGROUND) || pkEntity.GetPosition().z > 0))
        {
            param.m_Bool = true;
        }
    }

    void RemoveHandler()
    {
        if (hitEffectHandle != null) {
            hitEffectHandle.Remove();
            hitEffectHandle = null;
        }
    }
}
