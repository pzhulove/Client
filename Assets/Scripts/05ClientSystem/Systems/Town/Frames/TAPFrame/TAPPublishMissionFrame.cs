using Protocol;
using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TAPMissionReward
    {
        public int tableID;
        public int count;
        public TAPMissionReward(int tableID, int count)
        {
            this.tableID = tableID;
            this.count = count;
        }
    }

    public class TAPPublishMissionFrame : ClientFrame
    {
        //ulong pupilId = 0;
        RelationData relationData = null;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TAP/TAPPublishMissionFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            if (userData != null)
            {
                relationData = (RelationData)userData;
                //pupilId = (ulong)userData;
            }
            else
            {
                relationData = null;
            }

            _RegisterUIEvent();
            // _InitComUIList();
            // _InitData();
            _InitUI();
        }

        protected sealed override void _OnCloseFrame()
        {
            _ClearData();
            _ClearUI();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefr)
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateTAPPublishFrame, _OnUpdateTAPPublishFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTAPPublishMissionSuccess, _OnTAPPublishMissionSuccess);//发布任务成功的返回
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateTAPPublishFrame, _OnUpdateTAPPublishFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTAPPublishMissionSuccess, _OnTAPPublishMissionSuccess);
        }


        void _InitUI()
        {
            _UpdateUI();
            bool isTeacher = false;
            if (relationData == null)
            {
                TAPType myType = TAPNewDataManager.GetInstance().IsTeacher();
                if ((int)myType > 1)
                {
                    isTeacher = true;
                }
            }
            else
            {
                if (relationData.tapType == TAPType.Pupil)
                {
                    isTeacher = true;
                }
            }
         
        }

        void _UpdateUI()
        {
            mSearchTeacher.CustomActive(false);
            mSearchPupil.CustomActive(false);
            mTeacherText.CustomActive(false);
            mPupilText.CustomActive(false);
            bool isTeacher = false;
            if(relationData == null)
            {
                TAPType myType = TAPNewDataManager.GetInstance().IsTeacher();
                if ((int)myType > 1)
                {
                    isTeacher = true;
                }
            }
            else
            {
                if(relationData.tapType == TAPType.Pupil)
                {
                    isTeacher = true;
                }
            }
            if(isTeacher)
            {
                if(relationData == null||relationData.uid == 0)
                {
                    mSearchPupil.CustomActive(true);
                    mTitleText.text = TR.Value("tap_search_pupil_title_text");
                    mTeacherText.CustomActive(true);
                }
            }
            else
            {
                if(relationData == null || relationData.uid == 0)
                {
                    mSearchTeacher.CustomActive(true);
                    mTitleText.text = TR.Value("tap_search_teacher_title_text");
                    mPupilText.CustomActive(true);
                }

            }
        }

        void _ClearData()
        {
            // myRewardList.Clear();
        }

        void _ClearUI()
        {
            relationData = null;
        }

        #region 事件
        private void _OnUpdateTAPPublishFrame(UIEvent uiEvent)
        {
            RelationData tempRelationData = (RelationData)uiEvent.Param1;
            relationData = tempRelationData;
            _UpdateUI();
        }

        void _OnTAPPublishMissionSuccess(UIEvent uiEvent)
        {
            frameMgr.CloseFrame(this);
        }
        #endregion

        #region ExtraUIBind
		private Button mSearchTeacher = null;
		private Button mSearchPupil = null;
        private GameObject mSearchTeacherRedPoint = null;
        private GameObject mSearchPupilRedPoint = null;
        private Text mTitleText = null;
        private GameObject mTeacherText = null;
        private GameObject mPupilText = null;
		protected override void _bindExUI()
		{
			mSearchTeacher = mBind.GetCom<Button>("SearchTeacher");
			if (null != mSearchTeacher)
			{
				mSearchTeacher.onClick.AddListener(_onSearchTeacherButtonClick);
			}
			mSearchPupil = mBind.GetCom<Button>("SearchPupil");
			if (null != mSearchPupil)
			{
				mSearchPupil.onClick.AddListener(_onSearchPupilButtonClick);
			}
            mSearchTeacherRedPoint = mBind.GetGameObject("SearchTeacherRedPoint");
            mSearchPupilRedPoint = mBind.GetGameObject("SearchPupilRedPoint");
            mTitleText = mBind.GetCom<Text>("TitleText");
            mTeacherText = mBind.GetGameObject("TeacherText");
            mPupilText = mBind.GetGameObject("PupilText");
        }
		
		protected override void _unbindExUI()
		{
			if (null != mSearchTeacher)
			{
				mSearchTeacher.onClick.RemoveListener(_onSearchTeacherButtonClick);
			}
			mSearchTeacher = null;
			if (null != mSearchPupil)
			{
				mSearchPupil.onClick.RemoveListener(_onSearchPupilButtonClick);
			}
			mSearchPupil = null;
            mSearchTeacherRedPoint = null;
            mSearchPupilRedPoint = null;
            mTitleText = null;
            mTeacherText = null;
            mPupilText = null;
        }
		#endregion

        #region Callback

        private void _onSearchPupilButtonClick()
        {
            mSearchPupilRedPoint.CustomActive(false);
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.TeacherSoon)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_enter_searchFrame"));
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<TAPSearchFrame>(FrameLayer.Middle, 0);
            }
        }

        private void _onSearchTeacherButtonClick()
        {
            mSearchTeacherRedPoint.CustomActive(false);
            TAPNewDataManager.FindmasterIsSendServer = true;
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.TeacherSoon)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_enter_searchFrame"));
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<TAPSearchFrame>(FrameLayer.Middle, 0);
            }
        }
        #endregion
    }
}
