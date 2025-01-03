using Protocol;
using System.Runtime.InteropServices;

namespace GameClient
{
    public struct TaskInfoData
    {
        public int taskID;
        public int chapterID;
    }

    public struct TaskGuideDir
    {
        public bool bFaceRight;
        public TaskGuideArrow.TaskGuideDir eDir;
    }

    public struct PurchaseCommonData
    {
        public int iShopID;
        public int iGoodID;
        public int iItemID;
        public int iCount;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UIEventParams
    {
        [FieldOffset(0)]
        public int CommonIntTag;
        [FieldOffset(0)]
        public uint CommonUIntTag;
        [FieldOffset(0)]
        public ulong GUID;
        [FieldOffset(0)]
        public EPackageType PackType;

        [FieldOffset(0)]
        public TaskInfoData taskData;
        
        [FieldOffset(0)]
        public int CurrentSceneID;

        [FieldOffset(0)]
        public int CurrentSelectedID;

        [FieldOffset(0)]
        public BuyGoodsResult buyGoodsResult;

        [FieldOffset(0)]
        public PurchaseCommonData kPurchaseCommonData;
    }

    public class UIEvent
    {
        public EUIEventID EventID;
        public UIEventParams EventParams;
        public object Param1;
        public object Param2;
        public object Param3;
        public object Param4;
        public bool IsUsing;

        public void Initialize()
        {
            EventID = EUIEventID.Invalid;
            Param1 = null;
            Param2 = null;
            Param3 = null;
            Param4 = null;
            IsUsing = false;
        }
    }

    class UIEventDialogCallBack : UIEvent
    {
        public UIEventDialogCallBack(TaskDialogFrame.OnDialogOver callback)
        {
            this.callback = callback;
            EventID = EUIEventID.DlgCallBack;
            IsUsing = true;
        }
        public TaskDialogFrame.OnDialogOver callback;
    }

    class UIEventNpcIdChanged : UIEvent
    {
        public UIEventNpcIdChanged(System.Int32 npcID, System.Int32 npcResourceID, UnityEngine.Transform transform, UnityEngine.Camera eventCamera)
        {
            Initialize();
            this.npcID = npcID;
            this.npcResourceID = npcResourceID;
            this.transform = transform;
            this.eventCamera = eventCamera;
            EventID = EUIEventID.MenuIdChanged;
            IsUsing = true;
        }
        public System.Int32 npcID = 0;
        public System.Int32 npcResourceID = 0;
        public UnityEngine.Transform transform;
        public UnityEngine.Camera eventCamera;
    }

    class UIEventTaskNpcIdChanged : UIEvent
    {
        public UIEventTaskNpcIdChanged(System.Int32 npcID)
        {
            Initialize();
            this.npcID = npcID;
            EventID = EUIEventID.TaskNpcIdChanged;
            IsUsing = true;
        }
        public System.Int32 npcID = 0;
    }

    class UIEventRecievRecommendFriend : UIEvent
    {
        public UIEventRecievRecommendFriend(RelationData[] list)
        {
            Initialize();
            EventID = EUIEventID.OnRecievRecommendFriend;
            m_friendList = list;
        }

        public RelationData[] m_friendList;
    }

    class UIEventShowFriendSecMenu : UIEvent
    {
        public UIEventShowFriendSecMenu(RelationMenuData data)
        {
            Initialize();
            EventID = EUIEventID.OnShowFriendSecMenu;
            m_data = data;
        }
        
        public RelationMenuData m_data;
    }

    class UIEventDelPrivate : UIEvent
    {
        public UIEventDelPrivate(ulong uid)
        {
            Initialize();
            EventID = EUIEventID.OnDelPrivate;
            m_uid = uid;
        }

        public ulong m_uid;
    }

    class UIEventPrivateChat : UIEvent
    {
        public UIEventPrivateChat(RelationData data, bool recvChat)
        {
            Initialize();
            EventID = EUIEventID.OnPrivateChat;
            m_data = data;
            m_recvChat = recvChat;
        }

        public bool m_recvChat;
        public RelationData m_data;
    }

    class UIEventSwitchFriendTalk : UIEvent
    {
        public UIEventSwitchFriendTalk(ulong uid)
        {
            EventID = EUIEventID.OnShowFriendChat;
            m_uid = uid;
        }

        public ulong m_uid;
    }

    class UIEventRecvPrivateChat : UIEvent
    {
        public UIEventRecvPrivateChat(bool isRecv)
        {
            EventID = EUIEventID.OnRecvPrivateChat;
            _isRecv = isRecv;
        }

        public bool _isRecv;
    }

    class UIEventRecvQueryPlayer : UIEvent
    {
        public UIEventRecvQueryPlayer(PlayerWatchInfo info)
        {
            EventID = EUIEventID.OnRecvQueryPlayer;
            _info = info;
        }

        public PlayerWatchInfo _info;
    }

    class UIEventSpecialRedPointNotify : UIEvent
    {
        public int iMainId;
        public string prefabKey;
        public UIEventSpecialRedPointNotify(int iMainId,string prefabKey)
        {
            EventID = EUIEventID.ActivitySpecialRedPointNotify;
            this.iMainId = iMainId;
            this.prefabKey = prefabKey;
        }
    }
}
