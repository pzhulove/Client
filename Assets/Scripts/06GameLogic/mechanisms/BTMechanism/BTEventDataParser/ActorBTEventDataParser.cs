
using System.Collections.Generic;
using behaviac;
using ProtoTable;

namespace BehaviorTreeMechanism
{
    [BTEventType(behaviac.BTM_EventType.BeforeHitOther)]
    public class BTBeforeHitOtherDataParser : BTEventDataParser
    {
        public BTBeforeHitOtherDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetSkillId()
        {
            return ((EffectTable) m_Param.m_Obj2).SkillID;
        }

        public override int GetHurtId()
        {
            return (int) m_Param.m_Int2;
        }

        public override int GetAttackProcessId()
        {
            return m_Param.m_Int3;
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }

        public override int GetTargetId()
        {
            return ((BeActor) m_Param.m_Obj).GetPID();
        }
    }

    [BTEventType(behaviac.BTM_EventType.AfterHitOther)]
    public class BTAfterHitOtherDataParser : BTEventDataParser
    {
        public BTAfterHitOtherDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetSkillId()
        {
            return ((EffectTable) m_Param.m_Obj2).SkillID;
        }

        public override int GetHurtId()
        {
            return (int) m_Param.m_Int;
        }

        public override int GetAttackProcessId()
        {
            return m_Param.m_Int3;
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }

        public override int GetTargetId()
        {
            return ((BeActor) m_Param.m_Obj).GetPID();
        }
    }

    [BTEventType(behaviac.BTM_EventType.BeforeGetHit)]
    public class BTBeforeGetHitDataParser : BTEventDataParser
    {
        public BTBeforeGetHitDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetSkillId()
        {
            return ((EffectTable) m_Param.m_Obj3).SkillID;
        }

        public override int GetHurtId()
        {
            return (int) m_Param.m_Int4;
        }

        public override int GetAttackProcessId()
        {
            return m_Param.m_Int5;
        }

        public override int GetReleaseId()
        {
            return ((BeActor) m_Param.m_Obj).GetPID();
        }

        public override int GetTargetId()
        {
            return m_Param.m_SenderId;
        }
    }

    [BTEventType(behaviac.BTM_EventType.AfterGetHit)]
    public class BTAfterGetHitDataParser : BTEventDataParser
    {
        public BTAfterGetHitDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetSkillId()
        {
            return m_Param.m_Int2;
        }

        public override int GetHurtId()
        {
            return (int) m_Param.m_Int;
        }

        public override int GetAttackProcessId()
        {
            return m_Param.m_Int3;
        }

        public override int GetReleaseId()
        {
            return ((BeActor) m_Param.m_Obj).GetPID();
        }

        public override int GetTargetId()
        {
            return m_Param.m_SenderId;
        }
    }

    [BTEventType(behaviac.BTM_EventType.AddBuff)]
    public class BTAddBuffDataParser : BTEventDataParser
    {
        public BTAddBuffDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetBuffId()
        {
            return ((BeBuff) m_Param.m_Obj).buffID;
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }

        public override int GetTargetId()
        {
            return ((BeBuff) m_Param.m_Obj).owner.GetPID();
        }
    }

    [BTEventType(behaviac.BTM_EventType.DelBuff)]
    public class BTRemoveBuffDataParser : BTEventDataParser
    {
        public BTRemoveBuffDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetBuffId()
        {
            return ((BeBuff) m_Param.m_Obj).buffID;
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }

        public override int GetTargetId()
        {
            return ((BeBuff) m_Param.m_Obj).owner.GetPID();
        }
    }

    [BTEventType(behaviac.BTM_EventType.BeforeCreateEntity)]
    public class BTBeforeGenBulletDataParser : BTEventDataParser
    {
        public BTBeforeGenBulletDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetResId()
        {
            return m_Param.m_Int;
        }

        public override void CanLaunch(bool can)
        {
            m_Param.m_Bool = can;
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }
    }

    [BTEventType(behaviac.BTM_EventType.AfterCreateEntity)]
    public class BTAfterGenBulletDataParser : BTEventDataParser
    {
        public BTAfterGenBulletDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetEntityId()
        {
            return ((BeEntity) m_Param.m_Obj).GetPID();
        }

        public override int GetResId()
        {
            return ((BeEntity) m_Param.m_Obj).m_iResID;
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }
    }

    [BTEventType(behaviac.BTM_EventType.IsSkillCanSpell)]
    public class BTCanUseSkillDataParser : BTEventDataParser
    {
        public BTCanUseSkillDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetSkillId()
        {
            return m_Param.m_Int;
        }

        public override void CanUseSkill(bool canUseSkill)
        {
            m_Param.m_Bool = canUseSkill;
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }
    }

    [BTEventType(behaviac.BTM_EventType.FrameLabelTrig)]
    public class BTSkillFrameDataParser : BTEventDataParser
    {
        public BTSkillFrameDataParser(int eventType) : base(eventType)
        {
        }

        public override string GetFrameId()
        {
            return m_Param.m_String;
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }
    }

    [BTEventType(behaviac.BTM_EventType.BeforeDoAttack)]
    public class BTHurtEnterParser : BTEventDataParser
    {
        public BTHurtEnterParser(int eventType) : base(eventType)
        {
        }

        public override int GetHurtId()
        {
            return m_Param.m_Int;
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }

        public override int GetTargetId()
        {
            return ((BeEntity) m_Param.m_Obj).GetPID();
        }
    }

    [BTEventType(behaviac.BTM_EventType.AfterGetHitCalculateFinalDamageValve)]
    public class BTBeHitAfterFinalDamageDataParser : BTEventDataParser
    {
        public BTBeHitAfterFinalDamageDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetReleaseId()
        {
            return ((BeEntity) m_Param.m_Obj).GetPID();
        }

        public override int GetTargetId()
        {
            return m_Param.m_SenderId;
        }

        public override int GetHurtId()
        {
            return m_Param.m_Int2;
        }

        public override behaviac.MagicElementType GetMagicElementType()
        {
            var attacker = (BeEntity) m_Param.m_Obj;
            return (behaviac.MagicElementType) attacker.attribute.GetAttackElementType(GetHurtId());
        }

        public override int GetDamageValue()
        {
            return m_Param.m_Int;
        }

        public override void SetDamageValue(int value)
        {
            m_Param.m_Int = value;
        }

        public override int GetNormalDamageValue()
        {
            int damageValue = m_Param.m_Int;
            var attachDamage = GetAttachDamageValue();
            return damageValue - attachDamage;
        }

        public override void SetNormalDamageValue(int value)
        {
            m_Param.m_Int = value + GetAttachDamageValue();
        }

        public override int GetAttachDamageValue()
        {
            var attachValues = m_Param.m_Obj3 as List<int>;
            int value = 0;
            for (int i = 0; i < attachValues.Count; i++)
                value += attachValues[i];
            return value;
        }
    }

    [BTEventType(behaviac.BTM_EventType.SkillClickAgain)]
    public class BTClickSkillAgainDataParser : BTEventDataParser
    {
        public BTClickSkillAgainDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetSkillId()
        {
            return ((BeSkill) m_Param.m_Obj).skillID;
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }
    }

    [BTEventType(behaviac.BTM_EventType.RockerRelease)]
    public class BTReleaseJoystickDataParser : BTEventDataParser
    {
        public BTReleaseJoystickDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetSkillId()
        {
            return ((BeSkill) m_Param.m_Obj).skillID;
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }
    }

    [BTEventType(behaviac.BTM_EventType.AccessArea)]
    public class BTRangeInDataParser : BTEventDataParser
    {
        public BTRangeInDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }

        public override int GetTargetId()
        {
            return ((BeEntity) m_Param.m_Obj).GetPID();
        }

        public override int GetAreaId()
        {
            return ((BeRange) m_Param.m_Obj2).GetPID();
        }
    }

    [BTEventType(behaviac.BTM_EventType.LeaveArea)]
    public class BTRangeOutDataParser : BTEventDataParser
    {
        public BTRangeOutDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }

        public override int GetTargetId()
        {
            return ((BeEntity) m_Param.m_Obj).GetPID();
        }

        public override int GetAreaId()
        {
            return ((BeRange) m_Param.m_Obj2).GetPID();
        }
    }

    [BTEventType(behaviac.BTM_EventType.InArea)]
    public class BTRangeInsideDataParser : BTEventDataParser
    {
        public BTRangeInsideDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }

        public override int GetTargetId()
        {
            return ((BeEntity) m_Param.m_Obj).GetPID();
        }

        public override int GetAreaId()
        {
            return ((BeRange) m_Param.m_Obj2).GetPID();
        }
    }

    [BTEventType(behaviac.BTM_EventType.BeforeCreateMonster)]
    public class BTBeforeSummonDataParser : BTEventDataParser
    {
        public BTBeforeSummonDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }

        public override int GetMonsterId()
        {
            return m_Param.m_Int;
        }
    }

    [BTEventType(behaviac.BTM_EventType.AfterCreateMonster)]
    public class BTAfterSummonDataParser : BTEventDataParser
    {
        public BTAfterSummonDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }

        public override int GetMonsterId()
        {
            return ((BeEntity) m_Param.m_Obj).GetEntityData().monsterID;
        }

        public override int GetTargetId()
        {
            return ((BeEntity) m_Param.m_Obj).GetPID();
        }

        public override int GetSkillId()
        {
            return m_Param.m_Int;
        }
    }

    [BTEventType(behaviac.BTM_EventType.UnitIsDead)]
    public class BTOnDeadDataParser : BTEventDataParser
    {
        public BTOnDeadDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }

        public override int GetMonsterId()
        {
            return ((BeEntity) m_Param.m_Obj).GetEntityData().monsterID;
        }
    }

    [BTEventType(behaviac.BTM_EventType.UnitMoveEnd)]
    public class BTOnAIMoveEndParser : BTEventDataParser
    {
        public BTOnAIMoveEndParser(int eventType) : base(eventType)
        {
        }

        public override int GetReleaseId()
        {
            return m_Param.m_SenderId;
        }
    }
    
    [BTEventType(behaviac.BTM_EventType.CastSkill)]
    public class BTCastSkillDataParser : BTEventDataParser
    {
        public BTCastSkillDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetReleaseId() {
            return m_Param.m_SenderId;
        }

        public override int GetSkillId()
        {
            return m_Param.m_Int;
        }
    }
    
    [BTEventType(behaviac.BTM_EventType.CancelSkill)]
    public class BTCancelSkillDataParser : BTEventDataParser
    {
        public BTCancelSkillDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetReleaseId() {
            return m_Param.m_SenderId;
        }

        public override int GetSkillId()
        {
            return m_Param.m_Int;
        }
    }
    
    [BTEventType(behaviac.BTM_EventType.CastSkillFinish)]
    public class BTFinishSkillDataParser : BTEventDataParser
    {
        public BTFinishSkillDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetReleaseId() {
            return m_Param.m_SenderId;
        }

        public override int GetSkillId()
        {
            return m_Param.m_Int;
        }
    }
    
    [BTEventType(behaviac.BTM_EventType.KilledUnit)]
    public class BTOnSelfKillDataParser : BTEventDataParser
    {
        public BTOnSelfKillDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetReleaseId() {
            return m_Param.m_SenderId;
        }

        public override int GetTargetId()
        {
            return ((BeEntity) m_Param.m_Obj).GetPID();
        }
    }
    
    [BTEventType(behaviac.BTM_EventType.XInBlock)]
    public class BTXInBlockDataParser : BTEventDataParser
    {
        public BTXInBlockDataParser(int eventType) : base(eventType) { }
    }
    
    [BTEventType(behaviac.BTM_EventType.YInBlock)]
    public class BTYInBlockDataParser : BTEventDataParser
    {
        public BTYInBlockDataParser(int eventType) : base(eventType) { }
    }
}