using System.Collections.Generic;
using behaviac;
using GameClient;
using ProtoTable;

namespace BehaviorTreeMechanism
{
    /// <summary>
    /// 事件数据解析器
    /// 
    /// 注意：需添加对应的标签，用于导出事件可访问的接口信息文件。用于过滤BT编辑器的接口访问。
    /// 如果修改了接口，需执行-[TM工具集]/[机制编辑器]/导出事件接口信息-导出文件，AI编辑器会自动识别接口访问信息
    /// </summary>
    public abstract class BTEventDataParser
    {
        protected BeEvent.BeEventParam m_Param;

        protected BTEventDataParser(int eventType)
        {
            m_EventType = eventType;
        }

        protected readonly int m_EventType;
        public int EventType => m_EventType;

        /// <summary>
        /// 事件数据填充接口
        /// </summary>
        /// <param name="param"></param>
        public void Full(object param)
        {
            m_Param = param as BeEvent.BeEventParam;
        }

        [BTEventData("GetEventinfo_SkillID")]
        public virtual int GetSkillId()
        {
            Logger.LogErrorFormat("未实现 GetSkillId 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("GetEventinfo_EffectID")]
        public virtual int GetHurtId()
        {
            Logger.LogErrorFormat("未实现 GetHurtId 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("GetEventinfo_BuffID")]
        public virtual int GetBuffId()
        {
            Logger.LogErrorFormat("未实现 GetBuffId 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("GetEventinfo_EntityHandle")]
        public virtual int GetEntityId()
        {
            Logger.LogErrorFormat("未实现 GetEntityId 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("GetEventinfo_EntityID")]
        public virtual int GetResId()
        {
            Logger.LogErrorFormat("未实现 GetResId 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("SetEventinfo_IsEntityCanCreate")]
        public virtual void CanLaunch(bool can)
        {
            Logger.LogErrorFormat("未实现 CanLaunch 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
        }

        [BTEventData("SetEventinfo_IsSkillCanSpell")]
        public virtual void CanUseSkill(bool canUseSkill)
        {
            Logger.LogErrorFormat("未实现 CanUseSkill 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
        }

        [BTEventData("GetEventinfo_FreamLabel")]
        public virtual string GetFrameId()
        {
            Logger.LogErrorFormat("未实现 GetFrameId 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return string.Empty;
        }

        [BTEventData("GetEventinfo_SkillHandle")]
        public virtual int GetAttackProcessId()
        {
            Logger.LogErrorFormat("未实现 GetAttackProcessId 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("GetEventinfo_SourceHandle")]
        public virtual int GetReleaseId()
        {
            Logger.LogErrorFormat("未实现 GetReleaseId 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("GetEventinfo_TargetHandle")]
        public virtual int GetTargetId()
        {
            Logger.LogErrorFormat("未实现 GetTargetId 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("GetEventinfo_MagicElementType")]
        public virtual behaviac.MagicElementType GetMagicElementType()
        {
            Logger.LogErrorFormat("未实现 GetMagicElementType 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("GetEventinfo_DamageValue")]
        public virtual int GetDamageValue()
        {
            Logger.LogErrorFormat("未实现 GetDamageValue 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("SetEventinfo_DamageValue")]
        public virtual void SetDamageValue(int value)
        {
            Logger.LogErrorFormat("未实现 SetDamageValue 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
        }

        public virtual int GetAttachDamageValue()
        {
            Logger.LogErrorFormat("未实现 GetAttachDamageValue 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("GetEventinfo_NormalDamageValue")]
        public virtual int GetNormalDamageValue()
        {
            Logger.LogErrorFormat("未实现 GetNormalDamageValue 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("SetEventinfo_NormalDamageValue")]
        public virtual void SetNormalDamageValue(int value)
        {
            Logger.LogErrorFormat("未实现 SetNormalDamageValue 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
        }

        [BTEventData("GetEventinfo_AreaHandle")]
        public virtual int GetAreaId()
        {
            Logger.LogErrorFormat("未实现 GetAreaId 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }

        [BTEventData("GetEventinfo_MonsterID")]
        public virtual int GetMonsterId()
        {
            Logger.LogErrorFormat("未实现 GetMonsterId 接口,请确认使用是否在合理的事件环境下:{0}", m_EventType);
            return 0;
        }
    }
}