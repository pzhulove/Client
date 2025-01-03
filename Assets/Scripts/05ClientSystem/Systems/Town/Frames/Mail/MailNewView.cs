using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    public class MailNewView : MonoBehaviour
    {
        [Space(10)]
        [SerializeField]private ComUIListScript mMailMainTabList;
        [SerializeField]private ComUIListScript mMailUIListScript;
        [SerializeField]private GameObject mMailInfomationRoot;
        [SerializeField]private GameObject mNoMailTipRoot;
        [SerializeField]private GameObject mContent;
        [Space(10)][Header("Button")][SerializeField]private Button mBtnDeleteAll;
        [SerializeField]private Button mBtnAcceptAll;
        [SerializeField]private Button mBtnDelete;
        [SerializeField]private Button mBtnAccpet;
        [Space(10)][Header("StateController")]
        [SerializeField]private StateController mBtnStartContrl;
        [SerializeField]private string mAnnouncement;
        [SerializeField]private string mReward;
        [SerializeField]private string mGuild;
        [SerializeField]private CommonTabToggleGroup mCommonTabToggleGroup;
        
        private CommonTabToggleOnClick commonTabToggleOnClick;
        private List<MailTitleInfo> mSelfMailTitleInfoList = new List<MailTitleInfo>();
        private MailInfomationView mMailInfoMationView;
        private MailTabType mCurrentSelectMailTabType = MailTabType.MTT_NONE;

        private void Awake()
        {
            InitMailInfoMationView();
            RegisterUIEventHandler();
            InitBtnClick();
        }

        private void OnDestroy()
        {
            UnRegisterUIEventHandler();
            UnInitBtnClick();
        }

        private void InitMailInfoMationView()
        {
            var mUIPrefabWrapper = mMailInfomationRoot.GetComponent<UIPrefabWrapper>();
            if (mUIPrefabWrapper == null)
            {
                return;
            }

            var go = mUIPrefabWrapper.LoadUIPrefab();

            Utility.AttachTo(go, mMailInfomationRoot);
            if (mMailInfoMationView == null)
            {
                mMailInfoMationView = go.GetComponent<MailInfomationView>();
            }
        }
        
        public void InitView(CommonTabToggleOnClick onClick)
        {
            commonTabToggleOnClick = onClick;
            InitTabs();
            UpdateRedPoint();
        }

        /// <summary>
        /// 根据也签类型得到可领取邮件数量
        /// </summary>
        /// <returns></returns>
        private int GetOneKeyReceiveNum()
        {
            int OneKeyReceiveNum = 0;
            switch (MailDataManager.CurrentSelectMailTabType)
            {
                case MailTabType.MTT_NONE:
                    break;
                case MailTabType.MTT_ANNOUNCEMENT:
                    OneKeyReceiveNum = MailDataManager.AnnouncmentMailOneKeyReceiveNum;
                    break;
                case MailTabType.MTT_REWARD:
                    OneKeyReceiveNum = MailDataManager.RewardMailOneKeyReceiveNum;
                    break;
                case MailTabType.MTT_GUILD:
                    OneKeyReceiveNum = MailDataManager.GuildMailOneKeyReceiveNum;
                    break;
            }

            return OneKeyReceiveNum;
        }

        /// <summary>
        /// 根据类型得到可以一键删除的邮件数量
        /// </summary>
        /// <returns></returns>
        private int GetOneKeyDeleteMailNum()
        {
            int OneKeyDeleteMailNum = 0;

            switch (MailDataManager.CurrentSelectMailTabType)
            {
                case MailTabType.MTT_NONE:
                    break;
                case MailTabType.MTT_ANNOUNCEMENT:
                    OneKeyDeleteMailNum = MailDataManager.AnnouncementMailOneKeyDeleteNum;
                    break;
                case MailTabType.MTT_REWARD:
                    OneKeyDeleteMailNum = MailDataManager.RewardMailOneKeyDeleteNum;
                    break;
                case MailTabType.MTT_GUILD:
                    OneKeyDeleteMailNum = MailDataManager.GuildMailOneKeyDeleteNum;
                    break;
                default:
                    break;
            }

            return OneKeyDeleteMailNum;
        }

        private void InitBtnClick()
        {
            if (mBtnAcceptAll != null)
            {
                mBtnAcceptAll.onClick.RemoveAllListeners();
                mBtnAcceptAll.onClick.AddListener(() => 
                {
                    int OneKeyReceiveMailNum = GetOneKeyReceiveNum();
                    
                    if (OneKeyReceiveMailNum <= 0)
                    {
                        SystemNotifyManager.SystemNotify(1022);
                        return;
                    }

                    MailDataManager.GetInstance().OnSendReceiveMailReq(true, mCurSelectMailID);
                });
            }

            if (mBtnAccpet != null)
            {
                mBtnAccpet.onClick.RemoveAllListeners();
                mBtnAccpet.onClick.AddListener(() => 
                {
                    MailTitleInfo CurSelMailInfo = MailDataManager.GetInstance().FindMailTitleInfo(mSelfMailTitleInfoList, mCurSelectMailID);
                    if (CurSelMailInfo == null)
                    {
                        SystemNotifyManager.SystemNotify(1022);
                        return;
                    }

                    if (CurSelMailInfo.hasItem == 0)
                    {
                        SystemNotifyManager.SystemNotify(1021);
                        return;
                    }

                    MailDataManager.GetInstance().OnSendReceiveMailReq(false, mCurSelectMailID);
                });

                if (mBtnDeleteAll != null)
                {
                    mBtnDeleteAll.onClick.RemoveAllListeners();
                    mBtnDeleteAll.onClick.AddListener(() => 
                    {
                        int OneKeyDeleteMailNum = GetOneKeyDeleteMailNum();
                        if (OneKeyDeleteMailNum <= 0)
                        {
                            SystemNotifyManager.SystemNotify(1024);
                            return;
                        }

                        MailDataManager.GetInstance().OnSendDeleteMailReq(true, mCurSelectMailID);
                    });
                }

                if (mBtnDelete != null)
                {
                    mBtnDelete.onClick.RemoveAllListeners();
                    mBtnDelete.onClick.AddListener(() => 
                    {
                        MailTitleInfo CurSelMailInfo = MailDataManager.GetInstance().FindMailTitleInfo(mSelfMailTitleInfoList, mCurSelectMailID);
                        if (CurSelMailInfo == null)
                        {
                            SystemNotifyManager.SystemNotify(1024);
                            return;
                        }

                        if (CurSelMailInfo.hasItem == 1)
                        {
                            SystemNotifyManager.SystemNotify(1023);
                            return;
                        }

                        MailDataManager.GetInstance().OnSendDeleteMailReq(false, mCurSelectMailID);
                    });
                }
            }
        }

        private void UnInitBtnClick()
        {
            if (mBtnAcceptAll != null)
            {
                mBtnAcceptAll.onClick.RemoveAllListeners();
            }

            if (mBtnAccpet != null)
            {
                mBtnAccpet.onClick.RemoveAllListeners();
            }

            if (mBtnDeleteAll != null)
            {
                mBtnDeleteAll.onClick.RemoveAllListeners();
            }

            if (mBtnDelete != null)
            {
                mBtnDelete.onClick.RemoveAllListeners();
            }
        }

        private void RegisterUIEventHandler()
        {
            if (mMailUIListScript != null)
            {
                mMailUIListScript.Initialize();
                mMailUIListScript.onBindItem += OnBindMailItemDelegate;
                mMailUIListScript.onItemVisiable += OnBindMailItemVisiableDelegate;
                mMailUIListScript.onItemSelected += OnItemSelectedDelegate;
                mMailUIListScript.onItemChageDisplay += OnItemChangeDisplayDelegate;
            }
        }

        private void UnRegisterUIEventHandler()
        {
            if (mMailUIListScript != null)
            {
                mMailUIListScript.onBindItem -= OnBindMailItemDelegate;
                mMailUIListScript.onItemVisiable -= OnBindMailItemVisiableDelegate;
                mMailUIListScript.onItemSelected -= OnItemSelectedDelegate;
                mMailUIListScript.onItemChageDisplay -= OnItemChangeDisplayDelegate;
            }

            mMailUIListScript = null;
            
            mMailInfoMationView = null;
            mSelfMailTitleInfoList = null;
            mCurrentSelectMailTabType = MailTabType.MTT_NONE;
        }
        
        #region 主页签

        private void InitTabs()
        {
            mCommonTabToggleGroup.InitComTab(commonTabToggleOnClick, MailDataManager.DefaultMailMainTabIndex);
        }
        
        #endregion


        #region 邮件列表

        private UInt64 mCurSelectMailID = 0;

        private MailItem OnBindMailItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<MailItem>();
        }

        private void OnBindMailItemVisiableDelegate(ComUIListElementScript item)
        {
            var mMailItem = item.gameObjectBindScript as MailItem;
            if (mMailItem == null) return;

            if (mSelfMailTitleInfoList != null && item.m_index >= 0 && item.m_index < mSelfMailTitleInfoList.Count)
            {
                mMailItem.UpdateItemVisiable(mSelfMailTitleInfoList[item.m_index]);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var mMailItem = item.gameObjectBindScript as MailItem;
            if (mMailItem == null) return;

            if (mMailItem.GetMailTitleInfo.id == mCurSelectMailID)
            {
                return;
            }

            mCurSelectMailID = mMailItem.GetMailTitleInfo.id;
            
            if (mMailItem.GetMailTitleInfo.status == 0)
            {
                OnSendReadMailReq(mCurSelectMailID);
            }
            else
            {
                MailDataModel mMailDataModel = null;

                switch (MailDataManager.CurrentSelectMailTabType)
                {
                    case MailTabType.MTT_NONE:
                        break;
                    case MailTabType.MTT_ANNOUNCEMENT:
                        mMailDataModel = MailDataManager.GetInstance().FindMailDataModel(MailDataManager.GetInstance().mAnnouncementMailDataModelDict, mCurSelectMailID);
                        break;
                    case MailTabType.MTT_REWARD:
                        mMailDataModel = MailDataManager.GetInstance().FindMailDataModel(MailDataManager.GetInstance().mRewardMailDataModelDict, mCurSelectMailID);
                        break;
                    case MailTabType.MTT_GUILD:
                        mMailDataModel = MailDataManager.GetInstance().FindMailDataModel(MailDataManager.GetInstance().mGuildMailDataModelDict, mCurSelectMailID);
                        break;
                }

                if (mMailDataModel == null)
                {
                    OnSendReadMailReq(mCurSelectMailID);
                }
                else
                {
                    UpdateMailInfoMationView(mMailDataModel);
                }
            }
        }

        private void OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var mMailItem = item.gameObjectBindScript as MailItem;
            if (mMailItem == null) return;

            mMailItem.OnItemChangeDisplay(bSelected);
        }

        private void OnSendReadMailReq(UInt64 id)
        {
            MailDataManager.GetInstance().OnSendReadMailReq(id);
        }

        public void UpdateSelfMailTitleInfoList(List<MailTitleInfo> mailTitleInfoList)
        {
            mSelfMailTitleInfoList = new List<MailTitleInfo>();

            if (mCurrentSelectMailTabType != MailDataManager.CurrentSelectMailTabType)
            {
                mCurSelectMailID = 0;
                mCurrentSelectMailTabType = MailDataManager.CurrentSelectMailTabType;
            }

            if (mailTitleInfoList.Count > 0)
            {
                mSelfMailTitleInfoList = mailTitleInfoList;

                ResetMailUIListData();

                SetElementAmount(mSelfMailTitleInfoList.Count);

                SetSelectMailInfoItem();
            }
            else
            {
                SetElementAmount(0);
            }
            
            mContent.CustomActive(mSelfMailTitleInfoList.Count > 0);
        }

        public void SetElementAmount(int Count)
        {
            mMailUIListScript.SetElementAmount(Count);
        }

        public void UpdateMailTitleInfo(List<MailTitleInfo> mailTitleInfoList)
        {
            if (mailTitleInfoList.Count > 0)
            {
                mSelfMailTitleInfoList = mailTitleInfoList;

                SetElementAmount(mSelfMailTitleInfoList.Count);
            }
            else
            {
                SetElementAmount(0);
            }
        }

        public void UpdateNewMailNotify(List<MailTitleInfo> mailTitleInfoList)
        {
            if(mSelfMailTitleInfoList != null)
            {
                if (mSelfMailTitleInfoList.Count <= 0)
                {
                    UpdateSelfMailTitleInfoList(mailTitleInfoList);
                }
                else
                {
                    mSelfMailTitleInfoList = mailTitleInfoList;

                    ResetMailUIListData();

                    SetElementAmount(mSelfMailTitleInfoList.Count);
                }
            }
        }

        public void SetSelectMailInfoItem()
        {
            if (mMailUIListScript != null)
            {
                if (!mMailUIListScript.IsElementInScrollArea(0))
                {
                    mMailUIListScript.EnsureElementVisable(0);
                }
                mMailUIListScript.SelectElement(0);
            }
        }

        private void ResetMailUIListData()
        {
            mMailUIListScript.ResetSelectedElementIndex();
            mMailUIListScript.MoveElementInScrollArea(0, true);
        }

        #endregion

        public void UpdateMailInfoMationView(MailDataModel mailData)
        {
            if (mMailInfoMationView != null)
            {
                mMailInfoMationView.UpdateMailInfoMationView(mailData);
            }

            if (mailData.info != null)
            {
                mBtnAccpet.gameObject.CustomActive(mailData.info.hasItem == 1);
            }
        }

        /// <summary>
        /// 更新每个页签下显示的按钮
        /// </summary>
        /// <param name="CommonTabData"></param>
        public void UpdateBtnStatue(CommonTabData tabData)
        {
            if (tabData == null)
            {
                return;
            }

            switch ((MailTabType)tabData.id)
            {
                case MailTabType.MTT_NONE:
                    break;
                case MailTabType.MTT_ANNOUNCEMENT:
                    mBtnStartContrl.Key = mAnnouncement;
                    break;
                case MailTabType.MTT_REWARD:
                    mBtnStartContrl.Key = mReward;
                    break;
                case MailTabType.MTT_GUILD:
                    mBtnStartContrl.Key = mGuild;
                    break;
            }
        }

        public void UpdateRedPoint()
        {
            for (int i = 0; i <= (int)MailTabType.MTT_GUILD; i++)
            {
                if(mCommonTabToggleGroup != null)
                {
                    bool isFlag = MailDataManager.GetInstance().CheckRedPoint((MailTabType)i);
                    mCommonTabToggleGroup.OnSetRedPoint(i, isFlag);
                }
            }
        }
    }
}

