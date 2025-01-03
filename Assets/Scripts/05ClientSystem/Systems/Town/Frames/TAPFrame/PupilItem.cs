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
    class PupilItem
    {
        const string pupilItemPath = "UIFlatten/Prefabs/TAP/PupilItem";
        private Image mIcon;
        private Text mName;
        private Toggle mPupilToggle;
        private GameObject mSelect;
        private Text mOnline;
        private Button mBtnTalk;
        private Button mDonateEquip;
        private Text mLevel;
        private GameObject mRedPoint;
        private Button mDonateMoney;
        private UIGray mGray;
        private UIGray mGrayBtn;
        private ReplaceHeadPortraitFrame mReplaceHeadPortraitFrame;
        private ComPlayerVipLevelShow mVip;

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
        RelationData thisPupilData = null;
        GameObject pupilItemToggleGroup;
        public PupilItem(RelationData relationData, GameObject toggleGroup)
        {
            thisPupilData = relationData;
            pupilItemToggleGroup = toggleGroup;
            CreateGo();
        }
        private void CreateGo()
        {
            GameObject pupilItem = AssetLoader.instance.LoadResAsGameObject(pupilItemPath);
            if (pupilItem == null)
            {
                return;
            }
            var mBind = pupilItem.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
            mName = mBind.GetCom<Text>("Name");
            mIcon = mBind.GetCom<Image>("Icon");
            mPupilToggle = mBind.GetCom<Toggle>("PupilToggle");
            mSelect = mBind.GetGameObject("Select");
            mOnline = mBind.GetCom<Text>("Online");
            mBtnTalk = mBind.GetCom<Button>("BtnTalk");
            mDonateEquip = mBind.GetCom<Button>("DonateEquip");
            mLevel = mBind.GetCom<Text>("Level");
            mRedPoint = mBind.GetGameObject("RedPoint");
            mDonateMoney = mBind.GetCom<Button>("DonateMoney");
            mGray = mBind.GetCom<UIGray>("Gray");
            mGrayBtn = mBind.GetCom<UIGray>("GrayBtn");
            mReplaceHeadPortraitFrame = mBind.GetCom<ReplaceHeadPortraitFrame>("ReplaceHeadPortraitFrame");
            mVip = mBind.GetCom<ComPlayerVipLevelShow>("Vip");
            //Name
            ThisGameObject = pupilItem;

            var toggleGroup = pupilItemToggleGroup.GetComponent<ToggleGroup>();
            if (toggleGroup != null)
            {
                mPupilToggle.group = toggleGroup;
            }
            UpdateUI(thisPupilData);
        }

        public void UpdateUI(RelationData relationData)
        {
            thisPupilData = relationData;
            mName.text = thisPupilData.name;
            mOnline.CustomActive(true);
            bool bDonateEnable = (relationData.dayGiftNum > 0);
            if(thisPupilData.isOnline == 1)
            {
                if (thisPupilData.status == (byte)FriendMatchStatus.Idle)
                {
                    mOnline.text = "<color=#11EE11FF>在线</color>";
                    mGray.enabled = false;
                    if (bDonateEnable)
                    {
                        mDonateMoney.enabled = true;
                        mGrayBtn.enabled = false;
                    }
                    else
                    {
                        mDonateMoney.enabled = false;
                        mGrayBtn.enabled = true;
                    }
                }
                else if (thisPupilData.status == (byte)FriendMatchStatus.Busy)
                {
                    mOnline.text = "<color=#E95137FF>战斗中</color>";
                    mGray.enabled = false;
                    if (bDonateEnable)
                    {
                        mDonateMoney.enabled = true;
                        mGrayBtn.enabled = false;
                    }
                    else
                    {
                        mDonateMoney.enabled = false;
                        mGrayBtn.enabled = true;
                    }
                }
                else
                {
                    mOnline.text = "<color=#11EE11FF>在线</color>";
                    mGray.enabled = false;
                    if (bDonateEnable)
                    {
                        mDonateMoney.enabled = true;
                        mGrayBtn.enabled = false;
                    }
                    else
                    {
                        mDonateMoney.enabled = false;
                        mGrayBtn.enabled = true;
                    }
                }
            }
            else
            {
                mOnline.text = "<color=#99AABBFF>离线</color>";
                mGray.enabled = true;
                mGrayBtn.enabled = true;
            }
            
            mPupilToggle.onValueChanged.RemoveAllListeners();
            mPupilToggle.onValueChanged.AddListener((value) =>
            {
                mSelect.CustomActive(value);
                if(value)
                {
                    TAPNewDataManager.GetInstance().GetPupilMissionList(thisPupilData.uid);
                    thisPupilData.tapType = TAPType.Pupil;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnPupilDataUpdate, thisPupilData);
                }
            });

            if (mBtnTalk != null)
            {
                mBtnTalk.onClick.RemoveAllListeners();
                mBtnTalk.onClick.AddListener(() =>
                {
                    //TAPNewDataManager.GetInstance()._TalkToPeople(thisPupilData);
                    RelationMenuData menuData = new RelationMenuData();
                    menuData.m_data = thisPupilData;
                    menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_PUPIL_REAL;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnShowPupilRealMenu, menuData);
                });
            }
            
            mDonateEquip.onClick.RemoveAllListeners();
            mDonateEquip.onClick.AddListener(() =>
            {
                TAPDonateFrame.Open(thisPupilData);
            });
            //level
            mLevel.SafeSetText(string.Format("Lv.{0}",thisPupilData.level));

            //People Icon
            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(thisPupilData.occu);

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
            mRedPoint.CustomActive(TAPNewDataManager.GetInstance().HaveTAPDailyRedPointForID(relationData));

            //donateMoney
            mDonateMoney.SafeRemoveAllListener();
            mDonateMoney.SafeAddOnClickListener(OnSendGift);

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

        public void UpdatePupilRedPoint()
        {
            mRedPoint.CustomActive(TAPNewDataManager.GetInstance().HaveTAPDailyRedPointForID(thisPupilData));
        }

        public void DestoryGo()
        {
            GameObject.Destroy(ThisGameObject);
        }

        public bool toggleIsOn()
        {
            return mPupilToggle.isOn;
        }

        public void SetToggleSelect()
        {
            mPupilToggle.isOn = true;
        }

        public ulong GetUid()
        {
            return thisPupilData.uid;
        }

        /// <summary>
        /// 刷新这个页签下的数据
        /// </summary>
        public void UpdateSelect()
        {
            TAPNewDataManager.GetInstance().GetPupilMissionList(thisPupilData.uid);
            thisPupilData.tapType = TAPType.Pupil;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnPupilDataUpdate, thisPupilData);
        }

        private void OnSendGift()
        {
            if (null != thisPupilData)
            {
                if (thisPupilData.dayGiftNum <= 0)
                {
                    return;
                }

                if (thisPupilData.status == (byte)FriendMatchStatus.Offlie && thisPupilData.isOnline == 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_donate_failed_for_offline"));
                    return;
                }

                WorldRelationPresentGiveReq sendMsg = new WorldRelationPresentGiveReq();
                sendMsg.friendUID = thisPupilData.uid;
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
                    }
                    else
                    {
                        if(mDonateMoney != null)
                        {
                            mDonateMoney.enabled = false;
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