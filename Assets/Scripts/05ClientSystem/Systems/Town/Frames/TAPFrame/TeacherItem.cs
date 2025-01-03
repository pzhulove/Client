using UnityEngine.UI;
using Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using EItemType = ProtoTable.ItemTable.eType;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    class TeacherItem
    {
        const string TeacherItemPath = "UIFlatten/Prefabs/TAP/TeacherItem";
        private Image mIcon;
        private Text mName;
        private Toggle mTeacherToggle;
        private GameObject mSelect;
        private Text mOnline;
        private Button mBtnTalk;
        private Text mLevel;
        private GameObject mTitleImage;
        private Button mDonateMoney;
        private UIGray mGray;
        private UIGray mGrayBtn;
        private ReplaceHeadPortraitFrame mReplaceHeadPortraitFrame;
        private ComPlayerVipLevelShow mVip;

        private bool haveTeacher;
        RelationData thisTeacherData = null;
        GameObject teacherItemToggleGroup;
        private GameObject thisGameObject;
        public GameObject ThisGameObject
        {
            get
            {
                return thisGameObject;
            }
            set
            {
                thisGameObject = value;
            }
        }
        public TeacherItem(RelationData relationData,bool haveTrueTeacher,GameObject toggleGroup)
        {
            haveTeacher = haveTrueTeacher;
            thisTeacherData = relationData;
            teacherItemToggleGroup = toggleGroup;
            CreateGo(relationData);
        }
        private void CreateGo(RelationData relationData)
        {
            GameObject TeacherItem = AssetLoader.instance.LoadResAsGameObject(TeacherItemPath);
            if (TeacherItem == null)
            {
                return;
            }
            var mBind = TeacherItem.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
            mName = mBind.GetCom<Text>("Name");
            mIcon = mBind.GetCom<Image>("Icon");
            mTeacherToggle = mBind.GetCom<Toggle>("TeacherToggle");
            mSelect = mBind.GetGameObject("Select");
            mOnline = mBind.GetCom<Text>("Online");
            mBtnTalk = mBind.GetCom<Button>("BtnTalk");
            mLevel = mBind.GetCom<Text>("Level");
            mTitleImage = mBind.GetGameObject("titleImage");
            mDonateMoney = mBind.GetCom<Button>("DonateMoney");
            mGray = mBind.GetCom<UIGray>("Gray");
            mGrayBtn = mBind.GetCom<UIGray>("GrayBtn");
            mReplaceHeadPortraitFrame = mBind.GetCom<ReplaceHeadPortraitFrame>("ReplaceHeadPortraitFrame");
            mVip = mBind.GetCom<ComPlayerVipLevelShow>("Vip");
            //Name
            mName.SafeSetText(relationData.name);

            mLevel.SafeSetText(PlayerBaseData.GetInstance().Level.ToString());

            //People Icon
            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(relationData.occu);

            if (null != mIcon)
            {
                string path = "";
                if (null != jobItem)
                {
                    ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobItem.Mode);
                    if (resData != null)
                    {
                        path = resData.IconPath;
                    }
                }
                ETCImageLoader.LoadSprite(ref mIcon, path);
            }
            ThisGameObject = TeacherItem;


            mTeacherToggle.onValueChanged.RemoveAllListeners();
            mBtnTalk.CustomActive(false);
            mOnline.CustomActive(false);
            mTitleImage.CustomActive(false);
            if (haveTeacher)
            {
                mTitleImage.CustomActive(true);
                mBtnTalk.CustomActive(true);
                mTeacherToggle.onValueChanged.AddListener((value) =>
                {
                    mSelect.CustomActive(value);
                    if(true)
                    {
                        thisTeacherData.tapType = TAPType.Teacher;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTeacherDataUpdate, thisTeacherData);
                    }
                });
                var toggleGroup = teacherItemToggleGroup.GetComponent<ToggleGroup>();
                if (toggleGroup != null)
                {
                    mTeacherToggle.group = toggleGroup;
                }
                if (mBtnTalk)
                {
                    mBtnTalk.onClick.RemoveAllListeners();
                    mBtnTalk.onClick.AddListener(() =>
                    {
                        //TAPNewDataManager.GetInstance()._TalkToPeople(thisTeacherData);
                        RelationMenuData menuData = new RelationMenuData();
                        menuData.m_data = thisTeacherData;
                        menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_TEACHER_REAL;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnShowTeacherRealMenu, menuData);
                    });
                }
                
                mOnline.CustomActive(true);
                bool bDonateEnable = (relationData.dayGiftNum > 0);
                if(thisTeacherData.isOnline == 1)
                {
                    if (thisTeacherData.status == (byte)FriendMatchStatus.Idle)
                    {
                        mOnline.SafeSetText("<color=#11EE11FF>在线</color>");
                        if (mGray != null)
                        {
                            mGray.enabled = false;
                        }
                        if (bDonateEnable)
                        {
                            if (mDonateMoney != null)
                            {
                                mDonateMoney.enabled = true;
                            }
                            if (mGrayBtn != null)
                            {
                                mGrayBtn.enabled = false;
                            }
                        }
                        else
                        {
                            if (mDonateMoney != null)
                            {
                                mDonateMoney.enabled = false;
                            }
                            if (mGrayBtn != null)
                            {
                                mGrayBtn.enabled = true;
                            }
                        }
                    }
                    else if (thisTeacherData.status == (byte)FriendMatchStatus.Busy)
                    {
                        mOnline.SafeSetText("<color=#E95137FF>战斗中</color>");
                        if (mGray != null)
                        {
                            mGray.enabled = false;
                        }
                        if (bDonateEnable)
                        {
                            if (mDonateMoney != null)
                            {
                                mDonateMoney.enabled = true;
                            }
                            if (mGrayBtn != null)
                            {
                                mGrayBtn.enabled = false;
                            }
                        }
                        else
                        {
                            if (mDonateMoney != null)
                            {
                                mDonateMoney.enabled = false;
                            }
                            if (mGrayBtn != null)
                            {
                                mGrayBtn.enabled = true;
                            }
                        }
                    }
                    else
                    {
                        mOnline.SafeSetText("<color=#11EE11FF>在线</color>");
                        if (mGray != null)
                        {
                            mGray.enabled = false;
                        }
                        if (bDonateEnable)
                        {
                            if (mDonateMoney != null)
                            {
                                mDonateMoney.enabled = true;

                            }
                            if (mGrayBtn != null)
                            {
                                mGrayBtn.enabled = false;
                            }
                        }
                        else
                        {
                            if (mDonateMoney != null)
                            {
                                mDonateMoney.enabled = false;
                            }
                            if (mGrayBtn != null)
                            {
                                mGrayBtn.enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    mOnline.SafeSetText("<color=#99AABBFF>离线</color>");
                    if (mGray != null)
                    {
                        mGray.enabled = true;

                    }
                    if (mGrayBtn != null)
                    {
                        mGrayBtn.enabled = true;
                    }
                }

                //level
                mLevel.SafeSetText(relationData.level.ToString());

                //donateMoney
                mDonateMoney.CustomActive(true);
                mDonateMoney.SafeRemoveAllListener();
                mDonateMoney.SafeAddOnClickListener(OnSendGift);
                
            }
            else
            {
                mDonateMoney.CustomActive(false);
            }

            if (mReplaceHeadPortraitFrame != null)
            {
                if (relationData.headFrame != 0)
                {
                    mReplaceHeadPortraitFrame.ReplacePhotoFrame((int)relationData.headFrame);
                }
                else
                {
                    mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                }
            }

            if (mVip != null)
            {
                mVip.SetVipLevel(relationData.vipLv);
            }
        }

        public void DestoryGo()
        {
            GameObject.Destroy(ThisGameObject);
        }

        public bool toggleIsOn()
        {
            return mTeacherToggle.isOn;
        }

        public void SetToggleSelect()
        {
            mTeacherToggle.isOn = true;
        }

        public ulong GetUid()
        {
            return thisTeacherData.uid;
        }
        public void UpdateSelect()
        {
            thisTeacherData.tapType = TAPType.Teacher;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTeacherDataUpdate, thisTeacherData);
        }
        private void OnSendGift()
        {
            if (null != thisTeacherData)
            {
                if (thisTeacherData.dayGiftNum <= 0)
                {
                    return;
                }

                if (thisTeacherData.status == (byte)FriendMatchStatus.Offlie)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_donate_failed_for_offline"));
                    return;
                }

                WorldRelationPresentGiveReq sendMsg = new WorldRelationPresentGiveReq();
                sendMsg.friendUID = thisTeacherData.uid;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);

                WaitNetMessageManager.GetInstance().Wait(WorldRelationPresentGiveRes.MsgID, (MsgDATA data) =>
                {
                    if (data == null)
                    {
                        return;
                    }

                    WorldRelationPresentGiveRes ret = new WorldRelationPresentGiveRes();
                    if(ret != null)
                    {
                        ret.decode(data.bytes);

                        if (ret.code != (uint)ProtoErrorCode.SUCCESS)
                        {
                            var table = TableManager.GetInstance().GetTableItem<CommonTipsDesc>((int)ret.code);
                            if (table != null)
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(table.Descs);
                            }
                        }
                        else
                        {
                            if(mDonateMoney!=null)
                            {
                                mDonateMoney.enabled = false;
                            }
                            var donateMoneyGray = mDonateMoney.GetComponent<UIGray>();
                            if (donateMoneyGray != null)
                            {
                                donateMoneyGray.enabled = true;
                            }
                        }
                    }
                    

                }, false);

            }
        }
    }
}