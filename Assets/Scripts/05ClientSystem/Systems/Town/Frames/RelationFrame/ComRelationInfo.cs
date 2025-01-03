using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    class ComRelationInfo : MonoBehaviour
    {
        public static RelationData ms_selected = null;
        public static string ms_icon_master = "UI/Image/NewPacked/Shejiao.png:Shejiao_Biaoqian_Shifu";
        public static string ms_icon_pupil = "UI/Image/NewPacked/Shejiao.png:Shejiao_Biaoqian_Tudi";
        public static string ms_icon_relation= "UI/Image/NewPacked/Shejiao.png:Shejiao_Biaoqian_Haoyou";
        public static string ms_icon_stranger = "UI/Image/NewPacked/Shejiao.png:Shejiao_Biaoqian_Moshengren";
        public UIGray jobIIconGray;
        public Image jobIcon;
        public Text roleName;
        public Text remarkName;//备注不为空，这里显示原来的名字
        public Text jobName;
        public Text roleLvAndJob;
        public ComPlayerVipLevelShow vip;
        public UIGray grayGive;
        public Button btnGive;
        public GameObject goBusy;
        public GameObject goFree;
        public GameObject goOffLine;
        public Button btnMenu;
        public GameObject goChatRedPoint;
        public Image RelationFlag;
        public Text LeaveDesc;
        public GameObject goCheckMark;
        public GameObject goHuiGuiIcon;
        public Button btnHuiGui;
        public ReplaceHeadPortraitFrame mReplaceHeadPortraitFrame;
        InviteFriendData inviteData;
        RelationData value;
        RelationTabType eRelationTabType = RelationTabType.RTT_COUNT;

        public RelationData RelationData
        {
            get
            {
                return value == null ? null : value;
            }
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            goCheckMark.CustomActive(bSelected);
        }

        void _OnDonateAllSended(UIEvent uiEvent)
        {
            if(null != uiEvent)
            {
                var relation = uiEvent.Param1 as Relation;
                if(null != relation && null != value && relation.uid == value.uid)
                {
                    if (null != btnGive)
                    {
                        btnGive.enabled = false;
                    }
                    if (null != grayGive)
                    {
                        grayGive.enabled = true;
                    }
                }
            }
        }

        void _UpdateLeaveDesc()
        {
            if(null != LeaveDesc)
            {
                if(null != value)
                {
                    if (value.offlineTime == 0)
                    {
                        LeaveDesc.text = TR.Value("relation_leave_un_time");
                    }
                    else
                    {
                        var delta = TimeManager.GetInstance().GetServerTime() > value.offlineTime ? TimeManager.GetInstance().GetServerTime() - value.offlineTime : 0;
                        if (delta < 3600)
                        {
                            var value = delta / 60;
                            value = (uint)IntMath.Clamp(value, 1, 60);
                            LeaveDesc.text = TR.Value("relation_leave_m", value);
                        }
                        else if (delta < 86400)
                        {
                            LeaveDesc.text = TR.Value("relation_leave_h", delta / 3600);
                        }
                        else if (delta < 7 * 86400)
                        {
                            LeaveDesc.text = TR.Value("relation_leave_d", delta / 86400);
                        }
                        else
                        {
                            //var dateTime = Function.ConvertIntDateTime(value.offlineTime);
                            LeaveDesc.text = TR.Value("relation_leave_fixed_date");
                        }
                    }
                }
            }
        }


        public void OnRemoveFriend()
        {
            if(value.type == (int)RelationType.RELATION_FRIEND)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("relation_confirm_to_del_friend"), () =>
                {
                    RelationDataManager.GetInstance().DelFriend(value.uid);
                });
            }
            else if(value.type == (int)RelationType.RELATION_MASTER ||
                value.type == (int)RelationType.RELATION_DISCIPLE)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("relation_confirm_to_del_aop"), () =>
                {
                    RelationDataManager.GetInstance().DelRelation(value.uid,(RelationType)value.type);
                });
            }
        }

        public void OnPopupMenu()
        {
            if (value != null)
            {
                RelationMenuData menuData = new RelationMenuData();
                menuData.m_data = value;
                
                if (eRelationTabType == RelationTabType.RTT_RECENTLY)
                {
                    menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_PRIVAT_CHAT;
                }
                else if (eRelationTabType == RelationTabType.RTT_FRIEND)
                {
                    menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_COMMON;
                }
                else if (eRelationTabType == RelationTabType.RTT_BLACK)
                {
                    menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_BLACK;
                }
               
                UIEventSystem.GetInstance().SendUIEvent(new UIEventShowFriendSecMenu(menuData));
            }
          
        }

        public void OnHuiGuiBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<FriendBackBuffBonusFrame>(FrameLayer.Middle,value);
        }

        void OnChat(bool bValue)
        {
            if (bValue)
            {
                if (value != null)
                {
                    RelationDataManager.GetInstance().OnAddPriChatList(value, false);

                    UIEventSystem.GetInstance().SendUIEvent(new UIEventPrivateChat(value, false));
                }
            }
        }

        public void OnSendGift()
        {
            if(null != value)
            {
                if(value.dayGiftNum <= 0)
                {
                    return;
                }

                if(value.status == (byte)FriendMatchStatus.Offlie)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_donate_failed_for_offline"));
                    return;
                }

                WorldRelationPresentGiveReq sendMsg = new WorldRelationPresentGiveReq();
                sendMsg.friendUID = value.uid;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);

                WaitNetMessageManager.GetInstance().Wait(WorldRelationPresentGiveRes.MsgID, (MsgDATA data) =>
                {
                    if (data == null)
                    {
                        return;
                    }

                    WorldRelationPresentGiveRes ret = new WorldRelationPresentGiveRes();
                    ret.decode(data.bytes);

                    if (ret.code != (uint)ProtoErrorCode.SUCCESS)
                    {
                        var table = TableManager.GetInstance().GetTableItem<CommonTipsDesc>((int)ret.code);
                        if (table != null)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(table.Descs);
                        }
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSendFriendGift);
                    }
                    else
                    {
                        btnGive.enabled = false;
                        grayGive.enabled = true;
                    }

                }, false);

            }
        }

        public static void SendGift(RelationData value)
        {
            if (null != value)
            {
                WorldRelationPresentGiveReq sendMsg = new WorldRelationPresentGiveReq();
                sendMsg.friendUID = value.uid;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);
            }
        }


        public void OnTalkToFriend()
        {
            RelationDataManager.GetInstance().OnAddPriChatList(value, false);
            ChatManager.GetInstance().OpenPrivateChatFrame(value);
            ClientSystemManager.instance.CloseFrame<RelationFrameNew>();
            UIEventSystem.GetInstance().SendUIEvent(new UIEventPrivateChat(value, true));
        }

        public void OnRefuse()
        {
            if(null != inviteData)
            {
                SceneReply sendMsg = new SceneReply();
                sendMsg.type = (byte)RequestType.RequestFriend;
                sendMsg.requester = inviteData.requester;
                sendMsg.result = 0;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);

                RelationDataManager.GetInstance().DelInviter(inviteData.requester);
            }
        }

        public void OnAccept()
        {
            if (null != inviteData)
            {
                SceneReply sendMsg = new SceneReply();
                sendMsg.type = (byte)RequestType.RequestFriend;
                sendMsg.requester = inviteData.requester;
                sendMsg.result = 1;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);

                RelationDataManager.GetInstance().DelInviter(inviteData.requester);
            }
        }

        public void OnRemoveBlack()
        {
            if(null != value)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("relation_confirm_to_del_black"), () =>
                {
                    RelationDataManager.GetInstance().DelBlack(value.uid);
                });
            }
        }


        public void OnItemVisible(RelationData friendInfo, RelationTabType eRelationTabType)
        {
            value = null;
            inviteData = null;
            InvokeMethod.RmoveInvokeIntervalCall(this);

            if (null != btnMenu)
            {
                btnMenu.onClick.RemoveListener(OnPopupMenu);
            }

            this.value = friendInfo;
            this.eRelationTabType = eRelationTabType;

            if (null != btnMenu)
            {
                if(null != this.value)
                {
                    btnMenu.onClick.AddListener(OnPopupMenu);
                }
            }
            
            if (goHuiGuiIcon != null)
            {
                goHuiGuiIcon.CustomActive(value.isRegress == 1);
            }

            if (btnHuiGui != null)
            {
                btnHuiGui.CustomActive(value.isRegress == 1);
                btnHuiGui.onClick.RemoveListener(OnHuiGuiBtnClick);
                btnHuiGui.onClick.AddListener(OnHuiGuiBtnClick);
            }
            string path = "";
            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(friendInfo.occu);
            if (null != jobItem)
            {
                ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobItem.Mode);
                if (resData != null)
                {
                    path = resData.IconPath;
                }
                jobName.SafeSetText(jobItem.Name);
            }
            // jobIcon.sprite = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref jobIcon, path);

            if (jobIIconGray != null)
            {
                jobIIconGray.enabled = false;
                bool isFlag = value.status == (byte)FriendMatchStatus.Offlie;
                jobIIconGray.enabled = isFlag;
            }

            if (friendInfo.remark != null && friendInfo.remark != "")
            {
                roleName.text = friendInfo.remark;
                remarkName.text = string.Format("({0})", friendInfo.name);
            }
            else
            {
                roleName.text = friendInfo.name;
                remarkName.text = "";
            }
           
            if (null != jobItem)
            {
                roleLvAndJob.text = string.Format("Lv.{0}", friendInfo.level);
            }
            
            vip.SetVipLevel(friendInfo.vipLv);

            bool bGiveBtnIsShow = value.type != (byte)RelationType.RELATION_NONE && value.type != (byte)RelationType.RELATION_STRANGER && value.type != (byte)RelationType.RELATION_BLACKLIST;
            btnGive.CustomActive(bGiveBtnIsShow);
            bool bDonateEnable = (friendInfo.dayGiftNum > 0 && value.status != (byte)FriendMatchStatus.Offlie );
            btnGive.enabled = true;
            grayGive.enabled = !bDonateEnable;
            _UpdateLeaveDesc();
            if (eRelationTabType == RelationTabType.RTT_RECENTLY)
            {
                if (value.type == (byte)RelationType.RELATION_STRANGER ||
                             value.type == (int)RelationType.RELATION_NONE)
                {
                    goBusy.CustomActive(false);
                    goFree.CustomActive(value.isOnline > 0);
                    goOffLine.CustomActive(value.isOnline < 1);

                    if (value.isOnline == 0)
                    {
                        InvokeMethod.InvokeInterval(this, 1.0f, 1.0f, 999999.0f, null, _UpdateLeaveDesc, null);
                    }

                    if (jobIIconGray != null)
                    {
                        jobIIconGray.enabled = false;
                        bool isFlag = value.isOnline < 1;
                        jobIIconGray.enabled = isFlag;
                    }
                }
                else
                {
                    goBusy.CustomActive(value.status == (byte)FriendMatchStatus.Busy);
                    goFree.CustomActive(value.status == (byte)FriendMatchStatus.Idle);
                    goOffLine.CustomActive(value.status == (byte)FriendMatchStatus.Offlie);

                    if (value.status == (byte)FriendMatchStatus.Offlie)
                    {
                        InvokeMethod.InvokeInterval(this, 1.0f, 1.0f, 999999.0f, null, _UpdateLeaveDesc, null);
                    }
                }
            }
            else
            {
                goBusy.CustomActive(value.status == (byte)FriendMatchStatus.Busy);
                goFree.CustomActive(value.status == (byte)FriendMatchStatus.Idle);
                goOffLine.CustomActive(value.status == (byte)FriendMatchStatus.Offlie);

                if (value.status == (byte)FriendMatchStatus.Offlie)
                {
                    InvokeMethod.InvokeInterval(this, 1.0f, 1.0f, 999999.0f, null, _UpdateLeaveDesc, null);
                }
            }


            if (eRelationTabType == RelationTabType.RTT_RECENTLY)
            {
                goChatRedPoint.CustomActive(RelationDataManager.GetInstance().GetPriDirtyByUid(value.uid));
            }
            else
            {
                goChatRedPoint.CustomActive(false);
            }

            if (eRelationTabType == RelationTabType.RTT_BLACK)
            {
                vip.CustomActive(false);
            }
            else
            {
                vip.CustomActive(true);
            }

            if (null != RelationFlag)
            {
                if(null != value)
                {
                    if(value.type == (byte)RelationType.RELATION_MASTER)
                    {
                        // RelationFlag.sprite = AssetLoader.instance.LoadRes(ms_icon_master,typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref RelationFlag, ms_icon_master);
                        RelationFlag.CustomActive(true);
                    }
                    else if (value.type == (byte)RelationType.RELATION_DISCIPLE)
                    {
                        // RelationFlag.sprite = AssetLoader.instance.LoadRes(ms_icon_pupil,typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref RelationFlag, ms_icon_pupil);
                        RelationFlag.CustomActive(true);
                    }
                    else if (value.type == (byte)RelationType.RELATION_STRANGER ||
                             value.type == (int)RelationType.RELATION_NONE)
                    {
                        ETCImageLoader.LoadSprite(ref RelationFlag, ms_icon_stranger);
                        RelationFlag.CustomActive(true);
                    }
                    else if (value.type == (byte)RelationTabType.RTT_FRIEND && value.mark == (byte)RelationMarkFlag.RELATION_MUTUAL_FRIEND)
                    {
                        ETCImageLoader.LoadSprite(ref RelationFlag, ms_icon_relation);
                        RelationFlag.CustomActive(true);
                    }
                    else
                    {
                        RelationFlag.sprite = null;
                        RelationFlag.CustomActive(false);
                    }
                }
                else
                {
                    RelationFlag.sprite = null;
                    RelationFlag.CustomActive(false);
                }
            }

            if (mReplaceHeadPortraitFrame != null)
            {
                if (value.headFrame != 0)
                {
                    mReplaceHeadPortraitFrame.ReplacePhotoFrame((int)value.headFrame);
                }
                else
                {
                    mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                }
            }
        }

        public void OnItemVisible(InviteFriendData inviteFriendData)
        {
            value = null;
            inviteData = null;

            if(null != btnMenu)
            {
                btnMenu.onClick.RemoveListener(OnPopupMenu);
            }

            this.inviteData = inviteFriendData;

            if (null != btnMenu)
            {
                if(null != this.inviteData)
                {
                    btnMenu.onClick.AddListener(OnPopupMenu);
                }
            }

            roleName.text = inviteFriendData.requesterName;

            string path = "";
            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(inviteFriendData.requesterOccu);
            if (null != jobItem)
            {
                ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobItem.Mode);
                if (resData != null)
                {
                    path = resData.IconPath;
                }
            }
            // jobIcon.sprite = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref jobIcon, path);

            if (null != jobItem)
            {
                roleLvAndJob.text = string.Format("Lv.{0} {1}", inviteFriendData.requesterLevel, jobItem.Name);
            }

            vip.SetVipLevel(inviteFriendData.vipLv);
            
            goBusy.CustomActive(false);
            goFree.CustomActive(false);
            goOffLine.CustomActive(false);

            var relation = RelationDataManager.GetInstance().GetRelationByRoleID(inviteFriendData.requester);
            RelationType eRelationType = RelationType.RELATION_STRANGER;
            if(null != relation)
            {
                eRelationType = (RelationType)relation.type;
            }

            if (null != RelationFlag)
            {
                if (eRelationType == RelationType.RELATION_MASTER)
                {
                    // RelationFlag.sprite = AssetLoader.instance.LoadRes(ms_icon_master, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref RelationFlag, ms_icon_master);
                    RelationFlag.CustomActive(true);
                }
                else if (eRelationType == RelationType.RELATION_DISCIPLE)
                {
                    // RelationFlag.sprite = AssetLoader.instance.LoadRes(ms_icon_pupil, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref RelationFlag, ms_icon_pupil);
                    RelationFlag.CustomActive(true);
                }
                else
                {
                    RelationFlag.sprite = null;
                    RelationFlag.CustomActive(false);
                }
            }
        }

        void Start()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleChatDirtyChanged, _OnRoleChatDirtyChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnQueryIntervalChanged, _OnQueryIntervalChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDonateAllSended, _OnDonateAllSended);
            
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleChatDirtyChanged, _OnRoleChatDirtyChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnQueryIntervalChanged, _OnQueryIntervalChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnDonateAllSended, _OnDonateAllSended);
        }
        
        void _OnRoleChatDirtyChanged(UIEvent uiEvent)
        {
            if (null != value && this.eRelationTabType == RelationTabType.RTT_RECENTLY)
            {
                ulong uid = (ulong)uiEvent.Param1;
                bool bDirty = (bool)uiEvent.Param2;

                if(value.uid == uid)
                {
                    goChatRedPoint.CustomActive(RelationDataManager.GetInstance().GetPriDirtyByUid(value.uid));
                }
            }
            else
            {
                goChatRedPoint.CustomActive(false);
            }
        }

        void _OnQueryIntervalChanged(UIEvent uiEvent)
        {
            if(this.value != null)
            {
                OnItemVisible(this.value, this.eRelationTabType);
            }
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleChatDirtyChanged, _OnRoleChatDirtyChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnQueryIntervalChanged, _OnQueryIntervalChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDonateAllSended, _OnDonateAllSended);
            InvokeMethod.RmoveInvokeIntervalCall(this);
            value = null;
            inviteData = null;
            if(null != btnMenu)
            {
                btnMenu.onClick.RemoveListener(OnPopupMenu);
            }

        }
    }
}