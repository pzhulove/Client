using UnityEngine;
using System.Collections;

public class OnClickActive : MonoBehaviour
{
    public enum AttachParamsType
    {
        APT_NONE = 0,
        APT_CHECK_CONDITION,
    }

    public enum OnClickActiveType
    {
        OCAT_INVALID = -1,
        OCAT_GO,
        OCAT_ACQUIRED,
        OCAT_EVENT,
        OCAT_COUNT,
    }

    public enum NodeType
    {
        NT_ROOT = 0,
        NT_CHILD,
    }

    public enum BindStatus
    {
        BS_INIT = 0,
        BS_UNFINISH = 1,
        BS_FINISH=2,
    }

    public enum EventType
    {
        EventType_Invalid = -1,
        EventType_OpenSignFrame = 0,//补签
        EventType_OpenSeventDayAwardFrame,//打开第七天奖励界面
        EventType_NormalAcquireAward,//福利界面的正常找回
        EventType_PerfectAcquireAward,//福利界面的完美找回
        EventType_Pl_Normal_AcquireAward,//疲劳正常补偿
        EventType_Pl_Perfect_AcquireAward,//疲劳完美补偿
        EventType_Diamond_BeVip,//成为VIP用户
    }

    public enum OnClickCloseType
    {
        OCCT_NONE = 0,
        OCCT_PRE,
        OCCT_AFT,
    }

    public EventType m_eEventType;

    public OnClickActiveType m_eOnClickActiveType = OnClickActiveType.OCAT_INVALID;
    public NodeType m_eNodeType = NodeType.NT_ROOT;
    public BindStatus m_eBindStatus = BindStatus.BS_UNFINISH;
    public AttachParamsType m_eAttachParamsType = AttachParamsType.APT_NONE;
    public OnClickCloseType m_eOnClickCloseType = OnClickCloseType.OCCT_NONE;
}