using Network;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TAPSearchFrame : ClientFrame
    {
        int iChangeAllValue = 0;
        bool bForceDisable = true;
        bool m_bWaitSearchRet = false;

        List<RelationData> searchList = new List<RelationData>(); //查找的列表
        List<Toggle> mainToggle = new List<Toggle>();
        List<TAPSearchItem> tapApplyList = new List<TAPSearchItem>();
        const int peopleNum = 4;//每次加载四个
        int curTime = 0;//第n轮
        int curToggleIndex = 0;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TAP/TAPSearchFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            //默认打开第一个页签
            if (userData != null)
            {
                curToggleIndex = (int)userData;
            }
            else
            {
                curToggleIndex = 0;
            }
            _RegisterUIEvent();
            _InitData();
            _InitUI();
            _UpdateUI();
        }

        protected sealed override void _OnCloseFrame()
        {
            _ClearData();
            _ClearUI();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSearchedPupilListChanged, _OnSearchedPupilListChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSearchedTeacherListChanged, _OnSearchedTeacherListChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTAPApplyToggleRedPointUpdate, _OnTAPApplyToggleRedPointUpdate);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSearchedPupilListChanged, _OnSearchedPupilListChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSearchedTeacherListChanged, _OnSearchedTeacherListChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTAPApplyToggleRedPointUpdate, _OnTAPApplyToggleRedPointUpdate);
        }

        void _InitData()
        {
            curTime = 0;
            searchList.Clear();
            mainToggle.Clear();
            tapApplyList.Clear();
            List<RelationData> tempList = new List<RelationData>();
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher)
            {
                TAPNewDataManager.GetInstance().SendChangeSearchedPupilList(RelationFindType.Disciple);
                tempList = RelationDataManager.GetInstance().SearchedPupilList;
                mName.text = "收徒";
            }
            else if(TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Pupil)
            {
                if (TAPNewDataManager.FindmasterIsSendServer)
                {
                    TAPNewDataManager.GetInstance().SendChangeSearchedPupilList(RelationFindType.Master);
                }
                
                tempList = RelationDataManager.GetInstance().SearchedTeacherList;
                mName.text = "拜师";
            }
            else
            {
                return;
            }

            for(int i = 0;i<tempList.Count;i++)
            {
                searchList.Add(tempList[i]);
            }

            
        }

        void _InitUI()
        {
            mSearchItemRoot.CustomActive(false);
            _Func2RedPointUpdate();
        }

        void _ClearData()
        {
            curTime = 0;
            searchList.Clear();
            mainToggle.Clear();
            tapApplyList.Clear();
            TAPNewDataManager.FindmasterIsSendServer = false;
        }

        void _UpdateUI()
        {
            //GameObject.DestroyImmediate(mRight);
            for(int i = 0;i< tapApplyList.Count;i++)
            {
                tapApplyList[i].DestoryGo();
            }
            if(curTime * peopleNum >= searchList.Count)
            {
                curTime = 0;
            }
            int mLayer = 20; //模型需要的layer的值 每个模型不能相同
            if (searchList.Count == 0)
            {
                mTips.CustomActive(true);
            }
            else
            {
                mTips.CustomActive(false);
            }
            for (int i = curTime * peopleNum; i<searchList.Count;i++)
            {
                TAPSearchItem pupilApplyItem = new TAPSearchItem(searchList[i], mLayer);
                if (pupilApplyItem != null)
                {
                    tapApplyList.Add(pupilApplyItem);
                    Utility.AttachTo(pupilApplyItem.ThisGameObject, mRight);
                }
                mLayer++;
            }
            curTime++;
        }

        void _ClearUI()
        {
            // if(ClientSystemManager.GetInstance().IsFrameOpen<TAPApplyPupilFrame>())
            // {
            //     ClientSystemManager.GetInstance().CloseFrame<TAPApplyPupilFrame>();
            // }
        }
        void _Func2RedPointUpdate()
        {
            if (TAPNewDataManager.GetInstance().HaveApplyRedPoint())
            {
                mFunc2RedPoint.CustomActive(true);
            }
            else
            {
                mFunc2RedPoint.CustomActive(false);
            }
        }
        void _OnSearchedPupilListChanged(UIEvent uiEvent)
        {
            searchList.Clear();
            List<RelationData> tempList = new List<RelationData>();
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher)
            {
                tempList = RelationDataManager.GetInstance().SearchedPupilList;
                mName.text = "收徒";
                mTabText1.text = "查找徒弟";
            }
            else if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Pupil)
            {
                tempList = RelationDataManager.GetInstance().SearchedTeacherList;
                mName.text = "拜师";
                mTabText1.text = "查找师父";
            }
            else
            {
                return;
            }
            for(int i = 0;i<tempList.Count;i++)
            {
                searchList.Add(tempList[i]);
            }

            if (mInputField != null && string.IsNullOrEmpty(mInputField.text))
            {
                RelationData relation = null;
                for (int i = 0; i < tempList.Count; i++)
                {
                    if (tempList[i] != null)
                    {
                        if (tempList[i].name == mInputField.text)
                        {
                            relation = tempList[i];
                            break;
                        }
                    }
                }

                if (relation != null)
                {
                    ShowSearchFriend(relation);
                }
            }

            _UpdateUI();
        }

        void _OnSearchedTeacherListChanged(UIEvent uiEvent)
        {
            searchList.Clear();
            List<RelationData> tempList = new List<RelationData>();
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher)
            {
                tempList = RelationDataManager.GetInstance().SearchedPupilList;
            }
            else if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Pupil)
            {
                tempList = RelationDataManager.GetInstance().SearchedTeacherList;
            }
            else
            {
                return;
            }


            for (int i = 0;i<tempList.Count;i++)
            {
                searchList.Add(tempList[i]);
            }



            if (mInputField != null)
            {
                RelationData relation = null;
                for (int i = 0; i < tempList.Count; i++)
                {
                    if (tempList[i] != null)
                    {
                        if (tempList[i].name == mInputField.text)
                        {
                            relation = tempList[i];
                            break;
                        }
                    }
                }

                if (relation != null)
                {
                    ShowSearchFriend(relation);
                }
            }
            

            _UpdateUI();
        }


        void ShowSearchFriend(RelationData data)
        {
            mSearchItemRoot.CustomActive(false);
            if (null == data)
            {
                return;
            }

            mSearchItemRoot.CustomActive(true);
            if (mSearchItemInfo != null)
            {
                mSearchItemInfo.OnItemVisible(data);
            }
            SetAddBtn(data);
        }

        void SetAddBtn(RelationData data)
        {
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher)
            {
                mBtnAddText.SafeSetText("收徒");
                mBtnAdd.interactable = true;
                mBtnAdd.onClick.RemoveAllListeners();
                mBtnAdd.onClick.AddListener(() =>
                {
                    if (null != data)
                    {
                        TAPNewDataManager.GetInstance().SendApplyPupil(data.uid);
                        mBtnAdd.interactable = false;
                    }
                });
            }
            else
            {
                mBtnAddText.SafeSetText("拜师");
                mBtnAdd.interactable = true;
                mBtnAdd.onClick.RemoveAllListeners();
                mBtnAdd.onClick.AddListener(() =>
                {
                    if (null != data)
                    {
                        TAPNewDataManager.GetInstance().SendApplyTeacher(data.uid);
                        mBtnAdd.interactable = false;
                    }
                });
            }
            
        }

        void _OnTAPApplyToggleRedPointUpdate(UIEvent uiEvent)
        {
            _Func2RedPointUpdate();
        }

        

		#region ExtraUIBind
		private Button mBtnChangeAll = null;
		private Button mBtnAddAll = null;
		private Button mBtnSearch = null;
		private GameObject mRight = null;
		private UIGray mChangeAllGray = null;
		private Text mChangeAllText = null;
		private UIGray mAddAllGray = null;
		private InputField mInputField = null;
		private Button mBtnOpenQuestionaire = null;
		private Button mBtnOpenApply = null;
		private Button mClose = null;
		private Text mName = null;
		private GameObject mTips = null;
        private SetButtonCD mSetButtonCD = null;
        private Text mTabText1 = null;
        private GameObject mFunc2RedPoint = null;
        private Button mSearchClearBtn = null;
        private ComRecommendFriendInfo mSearchItemInfo = null;
        private RectTransform mSearchItemRoot = null;
        private Button mBtnAdd = null;
        private Text mBtnAddText = null;

        protected override void _bindExUI()
		{
			mBtnChangeAll = mBind.GetCom<Button>("BtnChangeAll");
			if (null != mBtnChangeAll)
			{
				mBtnChangeAll.onClick.AddListener(_onBtnChangeAllButtonClick);
			}
			mBtnAddAll = mBind.GetCom<Button>("BtnAddAll");
			if (null != mBtnAddAll)
			{
				mBtnAddAll.onClick.AddListener(_onBtnAddAllButtonClick);
			}
			mBtnSearch = mBind.GetCom<Button>("BtnSearch");
			if (null != mBtnSearch)
			{
				mBtnSearch.onClick.AddListener(_onBtnSearchButtonClick);
			}
			mRight = mBind.GetGameObject("Right");
			mChangeAllGray = mBind.GetCom<UIGray>("ChangeAllGray");
			mChangeAllText = mBind.GetCom<Text>("ChangeAllText");
			mAddAllGray = mBind.GetCom<UIGray>("AddAllGray");
			mInputField = mBind.GetCom<InputField>("InputField");
			mBtnOpenQuestionaire = mBind.GetCom<Button>("BtnOpenQuestionaire");
			if (null != mBtnOpenQuestionaire)
			{
				mBtnOpenQuestionaire.onClick.AddListener(_onBtnOpenQuestionaireButtonClick);
			}
			mBtnOpenApply = mBind.GetCom<Button>("BtnOpenApply");
			if (null != mBtnOpenApply)
			{
				mBtnOpenApply.onClick.AddListener(_onBtnOpenApplyButtonClick);
			}
			mClose = mBind.GetCom<Button>("Close");
			if (null != mClose)
			{
				mClose.onClick.AddListener(_onCloseButtonClick);
			}
			mName = mBind.GetCom<Text>("Name");
			mTips = mBind.GetGameObject("Tips");
            mSetButtonCD = mBind.GetCom<SetButtonCD>("SetButtonCD");
            mTabText1 = mBind.GetCom<Text>("TabText1");
            mFunc2RedPoint = mBind.GetGameObject("RedPoint");
            mSearchClearBtn = mBind.GetCom<Button>("SearchClearBtn");
            mSearchClearBtn.onClick.AddListener(_onSearchClearBtnButtonClick);
            mSearchItemInfo = mBind.GetCom<ComRecommendFriendInfo>("SearchItemInfo");
            mSearchItemRoot = mBind.GetCom<RectTransform>("SearchItemRoot");
            mBtnAdd = mBind.GetCom<Button>("BtnAdd");
            mBtnAddText = mBind.GetCom<Text>("BtnAddText");
        }
		
		protected override void _unbindExUI()
		{
			if (null != mBtnChangeAll)
			{
				mBtnChangeAll.onClick.RemoveListener(_onBtnChangeAllButtonClick);
			}
			mBtnChangeAll = null;
			if (null != mBtnAddAll)
			{
				mBtnAddAll.onClick.RemoveListener(_onBtnAddAllButtonClick);
			}
			mBtnAddAll = null;
			if (null != mBtnSearch)
			{
				mBtnSearch.onClick.RemoveListener(_onBtnSearchButtonClick);
			}
			mBtnSearch = null;
			mRight = null;
			mChangeAllGray = null;
			mChangeAllText = null;
			mAddAllGray = null;
			mInputField = null;
			if (null != mBtnOpenQuestionaire)
			{
				mBtnOpenQuestionaire.onClick.RemoveListener(_onBtnOpenQuestionaireButtonClick);
			}
			mBtnOpenQuestionaire = null;
			if (null != mBtnOpenApply)
			{
				mBtnOpenApply.onClick.RemoveListener(_onBtnOpenApplyButtonClick);
			}
			mBtnOpenApply = null;
			if (null != mClose)
			{
				mClose.onClick.RemoveListener(_onCloseButtonClick);
			}
			mClose = null;
			mName = null;
			mTips = null;
            mSetButtonCD = null;
            mTabText1 = null;
            mFunc2RedPoint = null;
            mSearchClearBtn.onClick.RemoveListener(_onSearchClearBtnButtonClick);
            mSearchClearBtn = null;
            mSearchItemInfo = null;
            mSearchItemRoot = null;
            mBtnAdd = null;
            mBtnAddText = null;
        }
		#endregion

        #region Callback
        private void _onBtnChangeAllButtonClick()
        {
            /* put your code in here */
            //mBtnChangeAll.enabled = false;
            //mChangeAllGray.enabled = true;
            //iChangeAllValue = 5;
            //if (null != mChangeAllText)
            //{
            //    mChangeAllText.text = TR.Value("relation_change_all_desc_param", iChangeAllValue);
            //}

            //InvokeMethod.RmoveInvokeIntervalCall(this);
            //InvokeMethod.InvokeInterval(this, 0.0f, 1, 5,
            //    null,
            //    () =>
            //    {
            //        --iChangeAllValue;
            //        if (null != mChangeAllText)
            //        {
            //            mChangeAllText.text = TR.Value("relation_change_all_desc_param", iChangeAllValue);
            //        }
            //    },
            //    () =>
            //    {
            //        mBtnChangeAll.enabled = true;
            //        mChangeAllGray.enabled = false;
            //        iChangeAllValue = 0;
            //        if (null != mChangeAllText)
            //        {
            //            mChangeAllText.text = TR.Value("relation_change_all_desc");
            //        }
            //    });

            //TAPDataManager.GetInstance().SendChangeSearchedTeacherList();
            //_UpdateUI();
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher)
            {
                TAPNewDataManager.GetInstance().SendChangeSearchedPupilList(RelationFindType.Disciple);
            }
            else if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Pupil)
            {
                TAPNewDataManager.GetInstance().SendChangeSearchedPupilList(RelationFindType.Master);
            }
            mSetButtonCD.StartBtCD();
        }
        private void _onBtnAddAllButtonClick()
        {
            /* put your code in here */
            bForceDisable = true;
            mAddAllGray.enabled = true;
            mBtnAddAll.enabled = false;
            var datas = RelationDataManager.GetInstance().SearchedTeacherList;
            for (int i = 0; i < datas.Count; ++i)
            {
                var curData = datas[i] as RelationData;
                if (null == curData)
                {
                    continue;
                }
                string name = curData.name;
                ulong uid = curData.uid;
                InvokeMethod.Invoke(this, i * 0.20f, () =>
                {
                    TAPNewDataManager.GetInstance().AddQueryInfo(uid);

                    TAPNewDataManager.GetInstance().SendApplyTeacher(uid);

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAskTeacherMsgSended, uid);
                });
            }
            InvokeMethod.Invoke(this, 0.20f * datas.Count, () =>
            {
                bForceDisable = false;
            });
        }
        private void _onBtnSearchButtonClick()
        {
            /* put your code in here */
            if (string.IsNullOrEmpty(mInputField.text))
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_search_name_empty"));
                return;
            }

            if (m_bWaitSearchRet)
            {
                return;
            }
            m_bWaitSearchRet = true;
            InvokeMethod.Invoke(this, 0.50f, () =>
            {
                m_bWaitSearchRet = false;
            });

            WorldQueryPlayerReq req = new WorldQueryPlayerReq();
            req.name = mInputField.text;
            OtherPlayerInfoManager.GetInstance().QueryPlayerType = WorldQueryPlayerType.WQPT_TEACHER;
            req.roleId = 0;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }
        private void _onBtnOpenQuestionaireButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.GetInstance().OpenFrame<TAPQuestionnaireFrame>();
        }
        private void _onBtnOpenApplyButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.GetInstance().OpenFrame<TAPApplyPupilFrame>();
        }
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }

        private void _onSearchClearBtnButtonClick()
        {
            /* put your code in here */
            mSearchItemRoot.CustomActive(false);
        }
        #endregion
    }
}
