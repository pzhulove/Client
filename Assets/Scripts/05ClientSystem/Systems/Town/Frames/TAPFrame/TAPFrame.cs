using Protocol;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TAPFrame : ClientFrame
    {
        bool haveTrueTeacher = false;

        List<PupilItem> pupilList = new List<PupilItem>();//徒弟页签
        List<TeacherItem> teacherList = new List<TeacherItem>();//师傅页签
        List<ClassmateItem> classmateList = new List<ClassmateItem>();//同门页签

        RelationData curRelationData = new RelationData();//用于当前选中的relationData
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TAP/TAPFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            _RegisterUIEvent();
            _InitData();
            _InitUI();
        }

        protected sealed override void _OnCloseFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<TAPPublishMissionFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<TAPPublishMissionFrame>();
            }
            if(ClientSystemManager.GetInstance().IsFrameOpen<TAPLearningFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<TAPLearningFrame>();
            }
            _ClearData();
            _ClearUI();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshClassmateDic, _OnRefreshClassmate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRelationChanged, _OnRelationChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTapTeacherSubmitRedPoint, UpdatePupilToggle);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPupilDataUpdate, _OnPupilDataUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTeacherDataUpdate, _OnTeacherDataUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnShowPupilRealMenu, _OnShowPupilRealMenu);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnShowTeacherRealMenu, _OnShowPupilRealMenu);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCloseMenu, _OnCloseMenu);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnApplyPupilListChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnApplyTeacherListChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTAPApplyToggleRedPointUpdate, _OnRefreshInviteList); ;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTAPTeacherValueChange,_OnRelationValueUpdate);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshClassmateDic, _OnRefreshClassmate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRelationChanged, _OnRelationChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTapTeacherSubmitRedPoint, UpdatePupilToggle);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPupilDataUpdate, _OnPupilDataUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTeacherDataUpdate, _OnTeacherDataUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnShowPupilRealMenu, _OnShowPupilRealMenu);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnShowTeacherRealMenu, _OnShowPupilRealMenu);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCloseMenu, _OnCloseMenu);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnApplyPupilListChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnApplyTeacherListChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTAPApplyToggleRedPointUpdate, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTAPTeacherValueChange,_OnRelationValueUpdate);
        }

        void _InitData()
        {
            haveTrueTeacher = false;
            teacherList.Clear();
            pupilList.Clear();
            classmateList.Clear();
            curRelationData = new RelationData();
            TAPNewDataManager.GetInstance().GetClassmateInformation();
        }

        void _InitUI()
        {
            _UpdateTeacherPanel();
            _UpdatePupilPanel();
            _UpdateClassmatePanel();
            _UpdatePublishFrame();
            _UpdateToggleSelect();
            if (pupilList.Count == 0 && !haveTrueTeacher)
            {
                mPeopleRoot.CustomActive(false);
                mRewardRoot.CustomActive(true);                
                _InitRewardUI();
            }
            else
            {
                mPeopleRoot.CustomActive(true);
                mRewardRoot.CustomActive(false);
            }


            //刷新惩罚时间
            if((int)TAPNewDataManager.GetInstance().IsTeacher() == 3 && !haveTrueTeacher)
            {
                mPublishTime.text = TAPNewDataManager.GetInstance().getPublishTime(TAPType.Pupil);
            }
            else
            {
                if((int)TAPNewDataManager.GetInstance().IsTeacher() == 1)
                {
                    mPublishTime.text = TAPNewDataManager.GetInstance().getPublishTime(TAPType.Teacher);
                }
            }

            mRelationValue.SafeSetText(string.Format(TR.Value("relaton_goodteacher_value"), PlayerBaseData.GetInstance().GoodTeacherValue));
        }

        /// <summary>
        /// 初始化左侧的奖励界面
        /// </summary>
        void _InitRewardUI()
        {
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Pupil)
            {
                mTitleText.text = "我的师父";
            }
            else
            {
                mTitleText.text = "我的徒弟";
            }
        }


        void _ClearData()
        {
            haveTrueTeacher = false;
            teacherList.Clear();
            pupilList.Clear();
            classmateList.Clear();
            curRelationData = null;
        }

        void _ClearUI()
        {

        }

        /// <summary>
        /// 刷新师傅的左边页签
        /// </summary>
        void _UpdateTeacherPanel()
        {
            for (int i = 0;i< teacherList.Count;i++)
            {
                teacherList[i].DestoryGo();
                //teacherList.Remove(teacherList[i]);
            }
            teacherList.Clear();
            var data = RelationDataManager.GetInstance().GetTeacher();
            if(data != null)
            {
                _AddTeacher(data,true);
                haveTrueTeacher = true;
            }
            else
            {
                haveTrueTeacher = false;
                RelationData myRelationData = new RelationData();
                myRelationData.name = PlayerBaseData.GetInstance().Name;
                myRelationData.occu = (byte)PlayerBaseData.GetInstance().JobTableID;
                myRelationData.headFrame = (uint)HeadPortraitFrameDataManager.WearHeadPortraitFrameID;
                _AddTeacher(myRelationData,false);
            }
        }
        void _AddTeacher(RelationData relationData,bool haveTeacher)
        {
            //如果已经收徒，自己是师父的时候，不用再显示自己了。
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher)
            {
                var datas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
                if (datas != null && datas.Count > 0)
                {
                    return;
                }
            }
            TeacherItem teacherItem = new TeacherItem(relationData,haveTeacher,mMyPupilRoot);
            teacherList.Add(teacherItem);
            if (teacherItem.ThisGameObject != null)
            {
                Utility.AttachTo(teacherItem.ThisGameObject, mMyTeacherRoot);
            }
        }

        /// <summary>
        /// 刷新徒弟的左边页签,如果是当前选中的页签时，也要同事去刷新右边的任务内容
        /// </summary>
        void _UpdatePupilPanel()
        {
            var datas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);

            //相同的刷新，老数据多的，就销毁
            for (int i=0;i<pupilList.Count;i++)
            {
                pupilList[i].DestoryGo();
            }
            pupilList.Clear();
            //有新的就添加
            for(int i = 0;i<datas.Count;i++)
            {
                _AddPupil(datas[i]);
            }
            if(datas.Count == 0)
            {
                mMyPupilRoot.CustomActive(false);
            }
            else
            {
                mMyPupilRoot.CustomActive(true);
            }
            if(TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher && datas.Count < 2&&!haveTrueTeacher)
            {
                mSearchFriendGo.CustomActive(true);
                mSetInformation2.CustomActive(true);
            }
            else
            {
                mSearchFriendGo.CustomActive(false);
                mSetInformation2.CustomActive(false);
            }

            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Pupil && !haveTrueTeacher)
            {
                mSetInformation.CustomActive(true);
            }
            else
            {
                mSetInformation.CustomActive(false);
            }
            
            if (haveTrueTeacher)
            {
                mPupilTip.CustomActive(true);
                mPeopleTitle.SafeSetText("我的师父");
            }
            else
            {
                mPupilTip.CustomActive(false);
            }

            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher && datas.Count > 0)
            {
                mMasterTip.CustomActive(true);
                mPeopleTitle.SafeSetText(string.Format("我的徒弟({0}/{1})", datas.Count, 2));
                mTeacherScroll.CustomActive(true);
            }
            else
            {
                mMasterTip.CustomActive(false);
                mTeacherScroll.CustomActive(false);
            }

            UpdateApplyRedPoint();
        }

        void _AddPupil(RelationData relationData)
        {
            PupilItem pupilItem = new PupilItem(relationData, mMyPupilRoot);
            pupilList.Add(pupilItem);
            if (pupilItem.ThisGameObject != null)
            {
                Utility.AttachTo(pupilItem.ThisGameObject, mMyPupilRoot);
            }
        }

        /// <summary>
        /// 刷新发布任务的页签
        /// </summary>
        void _UpdatePublishFrame()
        {
            if (pupilList.Count == 0 && !haveTrueTeacher)
            {
                if (!ClientSystemManager.GetInstance().IsFrameOpen<TAPPublishMissionFrame>())
                {
                    ClientSystemManager.GetInstance().OpenFrame<TAPPublishMissionFrame>(mPublishMission);
                }
                else
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateTAPPublishFrame, null);
                }
            }
        }

        void UpdatePupilToggle(UIEvent uiEvent)
        {
            if (!haveTrueTeacher)
            {
                for (int i = 0; i < pupilList.Count; i++)
                {
                    if (pupilList[i] != null)
                    {
                        pupilList[i].UpdatePupilRedPoint();   
                    }
                }    
            }
        }

        void _UpdateToggleSelect()
        {
            if(haveTrueTeacher)
            {
                teacherList[0].SetToggleSelect();
                teacherList[0].UpdateSelect();
            }
            else if (pupilList.Count > 0)
            {
                bool haveData = false;
                for(int i = 0;i<pupilList.Count;i++)
                {
                    if(pupilList[i].GetUid() == curRelationData.uid)
                    {
                        pupilList[i].SetToggleSelect();
                        pupilList[i].UpdateSelect();
                        haveData = true;
                    }
                }
                if(!haveData)
                {
                    pupilList[0].SetToggleSelect();
                    pupilList[0].UpdateSelect();
                }
            }
            if (!haveTrueTeacher && pupilList.Count == 0)
            {
                mPublishMission.CustomActive(true);
                mLearningRoot.CustomActive(false);
                mApplyBtn.CustomActive(true);
            }
            else
            {
                mPublishMission.CustomActive(false);
                mLearningRoot.CustomActive(true);
                mApplyBtn.CustomActive(false);
            }
        }
        
        /// <summary>
        /// 刷新同门的左边页签
        /// </summary>
        void _UpdateClassmatePanel()
        {
            for(int i=0;i<classmateList.Count;i++)
            {
                classmateList[i].DestoryGo();
            }
            classmateList.Clear();
            var datas = TAPNewDataManager.GetInstance().GetClassmateRelationDic();
            foreach(KeyValuePair<ulong, ClassmateRelationData> current in datas)
            {
                if (current.Value.level != 0 && current.Value.isFinSch == 0)
                {
                    _AddClassmate(current.Value);
                }
            }
            if (classmateList.Count == 0)
            {
                mMyClassmatesRoot.CustomActive(false);
            }
            else
            {
                mMyClassmatesRoot.CustomActive(true);
            }
        }
        void _AddClassmate(ClassmateRelationData relationData)
        {
            ClassmateItem classmateItem = new ClassmateItem(relationData);
            classmateList.Add(classmateItem);
            if(classmateItem.ThisGameObject != null)
            {
                Utility.AttachTo(classmateItem.ThisGameObject, mMyClassmatesRoot);
            }
        }

        #region UIEVENT
        void _OnRefreshClassmate(UIEvent uiEvent)
        {
            _UpdateClassmatePanel();
        }

        void _OnRelationChanged(UIEvent uiEvent)
        {
            _UpdateTeacherPanel();
            _UpdatePupilPanel();
            _UpdatePublishFrame();
            _UpdateToggleSelect();
            if (pupilList.Count == 0 && !haveTrueTeacher)
            {
                mPeopleRoot.CustomActive(false);
                mRewardRoot.CustomActive(true);
                
                _InitRewardUI();
            }
            else
            {
                mPeopleRoot.CustomActive(true);
                mRewardRoot.CustomActive(false);
            }
        }


        void UpdateApplyRedPoint()
        {
            bool haveApply = TAPNewDataManager.GetInstance().HaveApplyRedPoint();
            mSearchBtnRedPoint.CustomActive(haveApply);
            mApplyRedPoint.CustomActive(haveApply);
        }


        void _OnRelationValueUpdate(UIEvent uiEvent)
        {
            //师徒值刷新
            mRelationValue.SafeSetText(string.Format(TR.Value("relaton_goodteacher_value"), PlayerBaseData.GetInstance().GoodTeacherValue) );
        }

        /// <summary>
        /// 徒弟的数据刷新，选中页签是哪个不刷新，里面每个页签的内容要刷新
        /// </summary>
        /// <param name="uiEvent"></param>徒弟的relationData
        void _OnPupilDataUpdate(UIEvent uiEvent)
        {
            mPublishMission.CustomActive(false);
            mLearningRoot.CustomActive(false);
            RelationData relationData = (RelationData)uiEvent.Param1;
            curRelationData = relationData;
            mTimeTips.SafeSetText(string.Format("你们在{0}成为师徒", Function.GetOneData((int)relationData.createTime)));
            if (relationData == null)
            {
                mPublishMission.CustomActive(true);
                if (!ClientSystemManager.GetInstance().IsFrameOpen<TAPPublishMissionFrame>())
                {
                    ClientSystemManager.GetInstance().OpenFrame<TAPPublishMissionFrame>(mPublishMission, relationData);
                }
                else
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateTAPPublishFrame, relationData);
                }
            }
            else
            {
                mLearningRoot.CustomActive(true);
                if (!ClientSystemManager.GetInstance().IsFrameOpen<TAPLearningFrame>())
                {
                    ClientSystemManager.instance.OpenFrame<TAPLearningFrame>(mLearningRoot, relationData);
                }
                else
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPLearningUpdate, relationData);
                }
            }
        }

        void _OnTeacherDataUpdate(UIEvent uiEvent)
        {
            mPublishMission.CustomActive(false);
            mLearningRoot.CustomActive(false);
            RelationData relationData = (RelationData)uiEvent.Param1;
            curRelationData = relationData;
            mTimeTips.SafeSetText(string.Format("你们在{0}成为师徒", Function.GetOneData((int)relationData.createTime)));
            if (relationData == null)
            {
                if (!ClientSystemManager.GetInstance().IsFrameOpen<TAPPublishMissionFrame>())
                {
                    ClientSystemManager.GetInstance().OpenFrame<TAPPublishMissionFrame>(mPublishMission, relationData);
                }
                else
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateTAPPublishFrame, relationData);
                }
                mPublishMission.CustomActive(true);
            }
            else
            {
                if (!ClientSystemManager.GetInstance().IsFrameOpen<TAPLearningFrame>())
                {
                    ClientSystemManager.instance.OpenFrame<TAPLearningFrame>(mLearningRoot, relationData);
                }
                else
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPLearningUpdate, relationData);
                }
                mLearningRoot.CustomActive(true);
            }
                
        }

        IClientFrame m_openMenu = null;
        protected void _OnShowPupilRealMenu(UIEvent uiEvent)
        {
            RelationMenuData data = uiEvent.Param1 as RelationMenuData;
            if (null != m_openMenu)
            {
                m_openMenu.Close(true);
                m_openMenu = null;
            }
            m_openMenu = frameMgr.OpenFrame<RelationMenuFram>(FrameLayer.Middle, data);
        }

        protected void _OnCloseMenu(UIEvent uiEvent)
        {
            if (m_openMenu != null)
            {
                frameMgr.CloseFrame(m_openMenu);
                m_openMenu = null;
            }
        }

        void _OnRefreshInviteList(UIEvent uiEvent)
        {
            UpdateApplyRedPoint();
        }
        #endregion


        #region ExtraUIBind
        private GameObject mMyTeacherRoot = null;
		private GameObject mMyClassmatesRoot = null;
		private GameObject mMyPupilRoot = null;
		private GameObject mSearchFriendGo = null;
		private Button mSearchPupilBt = null;
        private GameObject mLearningRoot = null;
		private GameObject mPublishMission = null;
		private GameObject mRewardRoot = null;
		private GameObject mPeopleRoot = null;
		private Text mTitleText = null;
		private Button mSetInformation = null;
		private Button mGoShop = null;
		private Button mSetInformation2 = null;
		private Text mPublishTime = null;
		private Text mTimeTips = null;
        private GameObject mSearchBtnRedPoint = null;
        private Button mApplyBtn = null;
        private GameObject mApplyRedPoint = null;
        private Text mRelationValue = null;
        private GameObject mPupilTip = null;
        private GameObject mMasterTip = null;
        private Text mPeopleTitle = null;
        private GameObject mTeacherScroll = null;


        protected override void _bindExUI()
		{
			mMyTeacherRoot = mBind.GetGameObject("MyTeacherRoot");
			mMyClassmatesRoot = mBind.GetGameObject("MyClassmatesRoot");
			mMyPupilRoot = mBind.GetGameObject("MyPupilRoot");
			mSearchFriendGo = mBind.GetGameObject("SearchFriendGo");
			mSearchPupilBt = mBind.GetCom<Button>("SearchPupilBt");
			if (null != mSearchPupilBt)
			{
				mSearchPupilBt.onClick.AddListener(_onSearchPupilBtButtonClick);
			}
            mLearningRoot = mBind.GetGameObject("LearningRoot");
			mPublishMission = mBind.GetGameObject("PublishMission");
			mRewardRoot = mBind.GetGameObject("RewardRoot");
			mPeopleRoot = mBind.GetGameObject("PeopleRoot");
			mTitleText = mBind.GetCom<Text>("TitleText");
			mSetInformation = mBind.GetCom<Button>("SetInformation");
			if (null != mSetInformation)
			{
				mSetInformation.onClick.AddListener(_onSetInformationButtonClick);
			}
			mGoShop = mBind.GetCom<Button>("GoShop");
			if (null != mGoShop)
			{
				mGoShop.onClick.AddListener(_onGoShopButtonClick);
			}
			mSetInformation2 = mBind.GetCom<Button>("SetInformation2");
			if (null != mSetInformation2)
			{
				mSetInformation2.onClick.AddListener(_onSetInformation2ButtonClick);
			}
			
			mPublishTime = mBind.GetCom<Text>("publishTime");
			mTimeTips = mBind.GetCom<Text>("TimeTips");
            mSearchBtnRedPoint = mBind.GetGameObject("SearchBtnRedPoint");
            mApplyBtn = mBind.GetCom<Button>("ApplyBtn");
            mApplyBtn.onClick.AddListener(_onApplyBtnButtonClick);
            mApplyRedPoint = mBind.GetGameObject("ApplyRedPoint");
            mRelationValue = mBind.GetCom<Text>("RelationValue");
            mPupilTip = mBind.GetGameObject("PupilTip");
            mMasterTip = mBind.GetGameObject("MasterTip");
            mPeopleTitle = mBind.GetCom<Text>("PeopleTitle");
            mTeacherScroll = mBind.GetGameObject("TeacherScroll");
        }
		
		protected override void _unbindExUI()
		{
			mMyTeacherRoot = null;
			mMyClassmatesRoot = null;
			mMyPupilRoot = null;
			mSearchFriendGo = null;
			if (null != mSearchPupilBt)
			{
				mSearchPupilBt.onClick.RemoveListener(_onSearchPupilBtButtonClick);
			}
			mSearchPupilBt = null;
            mSearchPupilBt = null;
			mLearningRoot = null;
			mPublishMission = null;
			mRewardRoot = null;
			mPeopleRoot = null;
			mTitleText = null;
			if (null != mSetInformation)
			{
				mSetInformation.onClick.RemoveListener(_onSetInformationButtonClick);
			}
			mSetInformation = null;
			if (null != mGoShop)
			{
				mGoShop.onClick.RemoveListener(_onGoShopButtonClick);
			}
			mGoShop = null;
			if (null != mSetInformation2)
			{
				mSetInformation2.onClick.RemoveListener(_onSetInformation2ButtonClick);
			}
			mSetInformation2 = null;
			mPublishTime = null;
			mTimeTips = null;
            mSearchBtnRedPoint = null;
            mApplyBtn.onClick.RemoveListener(_onApplyBtnButtonClick);
            mApplyBtn = null;
            mApplyRedPoint = null;
            mRelationValue = null;
            mPupilTip = null;
            mMasterTip = null;
            mPeopleTitle = null;
            mTeacherScroll = null;
        }
		#endregion

        #region Callback
        private void _onSearchPupilBtButtonClick()
        {
            /* put your code in here */
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.TeacherSoon)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_enter_searchFrame"));
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<TAPSearchFrame>(FrameLayer.Middle, 0);
            }
            //ClientSystemManager.GetInstance().OpenFrame<TAPSearchPupilFrame>(FrameLayer.Middle, null);
        }


        private void _onSearchTeacherButtonClick()
        {
            if(TAPNewDataManager.GetInstance().IsTeacher() == TAPType.TeacherSoon)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_enter_searchFrame"));
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<TAPSearchFrame>(FrameLayer.Middle,0);
                //ClientSystemManager.GetInstance().OpenFrame<TAPSearchTeacherFrame>(FrameLayer.Middle);
            }
        }

        private void _onSearchPupilButtonClick()
        {
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.TeacherSoon)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_enter_searchFrame"));
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<TAPSearchFrame>(FrameLayer.Middle, 0);
                //ClientSystemManager.GetInstance().OpenFrame<TAPSearchPupilFrame>(FrameLayer.Middle, null);
            }
        }

        private void _onSetInformationButtonClick()
        {
            
                ClientSystemManager.GetInstance().OpenFrame<TAPQuestionnaireFrame>();
            
        }

        private void _onGoShopButtonClick()
        {
            ShopDataManager.GetInstance().OpenShop(20);
        }

        private void _onSetInformation2ButtonClick()
        {
            if(haveTrueTeacher)
            {
                //有老师
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_haveteacher_cannot_go_questionframe"));
            }
            else if(!TAPNewDataManager.GetInstance().canSearchPupil())
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_havepupil_cannot_go_questionframe"));
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<TAPQuestionnaireFrame>();
            }
        }

        private void _onGoShop2ButtonClick()
        {
            ShopDataManager.GetInstance().OpenShop(20);
        }
        
        private void _onApplyBtnButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.GetInstance().OpenFrame<TAPApplyPupilFrame>();
        }
        #endregion
    }
}
