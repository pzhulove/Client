namespace BehaviorTreeMechanism
{
    [BTSceneEventType(behaviac.BTM_LevelEventType.AddBuff)]
    public class BTSceneAddBuffDataParser : BTEventDataParser
    {
        public BTSceneAddBuffDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetBuffId()
        {
            return ((BeBuff) m_Param.m_Obj).buffID;
        }

        public override int GetTargetId()
        {
            return ((BeActor) m_Param.m_Obj2).GetPID();
        }

        public override int GetReleaseId()
        {
            var releasr = ((BeBuff) m_Param.m_Obj).releaser;
            return releasr == null ? 0 : releasr.GetPID();
        }
    }

    [BTSceneEventType(behaviac.BTM_LevelEventType.DelBuff)]
    public class BTSceneRemoveBuffDataParser : BTEventDataParser
    {
        public BTSceneRemoveBuffDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetBuffId()
        {
            return ((BeBuff) m_Param.m_Obj).buffID;
        }

        public override int GetTargetId()
        {
            return ((BeActor) m_Param.m_Obj2).GetPID();
        }

        public override int GetReleaseId()
        {
            var releasr = ((BeBuff) m_Param.m_Obj).releaser;
            return releasr == null ? 0 : releasr.GetPID();
        }
    }
    
    [BTSceneEventType(behaviac.BTM_LevelEventType.AfterCreateEntity)]
    public class BTSceneAfterCreateEntityDataParser : BTEventDataParser
    {
        public BTSceneAfterCreateEntityDataParser(int eventType) : base(eventType)
        {
        }

        public override int GetEntityId()
        {
            return ((BeEntity) m_Param.m_Obj2).GetPID();
        }

        public override int GetResId()
        {
            return ((BeEntity) m_Param.m_Obj2).m_iResID;
        }

        public override int GetReleaseId()
        {
            return ((BeEntity) m_Param.m_Obj).GetPID();
        }
    }
}