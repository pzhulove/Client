using Protocol;
using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum TAPGraduationType
    {
        None,
        TeacherGraduation = 1,
        TeacherGraduationLate = 2,
        PupilGraduationLate = 3,
    }

    public class TAPSubmitGraduationFrame : ClientFrame
    {
        RelationData relationData = null;//当前选中的relationdata
        TAPGraduationType tapGraduationType = TAPGraduationType.None;
        List<RelationData> graduationList = new List<RelationData>();
        List<Toggle> relationDataToggleList = new List<Toggle>();
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TAP/TAPSubmitGraduationFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            _InitData();
            _UpdateUI();
            if(relationDataToggleList.Count != 0)
            {
                relationDataToggleList[0].isOn = true;
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            _ClearData();
        }

        void _InitData()
        {
            relationDataToggleList.Clear();
            graduationList.Clear();
            _InitGraduationList();
            _InitComUIList();
        }
        void _InitGraduationList()
        {
            
            var teacherDatas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_MASTER);
            for(int i = 0;i<teacherDatas.Count;i++)
            {
                if(i > 0)
                {
                    continue;
                }
                teacherDatas[i].tapType = TAPType.Teacher;
                graduationList.Add(teacherDatas[i]);
            }
            var pupilDatas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
            for(int i = 0;i<pupilDatas.Count;i++)
            {
                pupilDatas[i].tapType = TAPType.Pupil;
                graduationList.Add(pupilDatas[i]);
            }
        }
        void _InitComUIList()
        {
            mTAPItemComUIList.Initialize();
            mTAPItemComUIList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdateRewardBind(item);
                }
            };
            mTAPItemComUIList.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }
            };
            mTAPItemComUIList.SetElementAmount(graduationList.Count);
        }

        void _UpdateRewardBind(ComUIListElementScript item)
        {
            ComCommonBind mBind = item.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
            if (item.m_index < 0)
            {
                return;
            }
            if (graduationList.Count <= 0)
            {
                return;
            }
            var tempRelationData = graduationList[item.m_index];
            if(tempRelationData == null)
            {
                return;
            }
            var mName = mBind.GetCom<Text>("Name");
            var mIcon = mBind.GetCom<Image>("Icon");
            var mPupilToggle = mBind.GetCom<Toggle>("PupilToggle");
            var mSelect = mBind.GetGameObject("Select");
            var mLevel = mBind.GetCom<Text>("Level");
            var mTeacherImage = mBind.GetGameObject("TeacherImage");
            var mPupilImage = mBind.GetGameObject("PupilImage");
            mName.text = tempRelationData.name;
            //level
            mLevel.text = tempRelationData.level.ToString();

            //People Icon
            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(tempRelationData.occu);

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

            //toggle
            mPupilToggle.onValueChanged.RemoveAllListeners();
            mPupilToggle.onValueChanged.AddListener((flag) =>
            {
                mSelect.CustomActive(flag);
                if(flag)
                {
                    relationData = tempRelationData;
                    _UpdateUI();
                }
            });
            relationDataToggleList.Add(mPupilToggle);

            if(tempRelationData.tapType == TAPType.Teacher)
            {
                mTeacherImage.CustomActive(true);
                mPupilImage.CustomActive(false);
            }
            else if(tempRelationData.tapType == TAPType.Pupil)
            {
                mTeacherImage.CustomActive(false);
                mPupilImage.CustomActive(true);
            }
            else
            {
                mTeacherImage.CustomActive(false);
                mPupilImage.CustomActive(false);
            }
        }
        void _UpdateUI()
        {
            mTips1.CustomActive(false);
            mTips2.CustomActive(false);
            mTips3.CustomActive(false);
            if(relationData == null)
            {
                return;
            }
            if (relationData.tapType == TAPType.Pupil)
            {
                
                if (relationData.isOnline == 0)
                {
                    tapGraduationType = TAPGraduationType.TeacherGraduationLate;
                    mTips2.CustomActive(true);
                }
                else
                {
                    tapGraduationType = TAPGraduationType.TeacherGraduation;
                    mTips1.CustomActive(true);
                }
            }
            else
            {
                tapGraduationType = TAPGraduationType.PupilGraduationLate;
                mTips3.CustomActive(true);
            }
        }

        void _ClearData()
        {
            relationDataToggleList.Clear();
            graduationList.Clear();
            tapGraduationType = TAPGraduationType.None;
        }

		#region ExtraUIBind
		private Button mClose = null;
		private GameObject mTips1 = null;
		private GameObject mTips2 = null;
		private GameObject mTips3 = null;
		private ComUIListScript mTAPItemComUIList = null;
		private Button mSubmit = null;
		
		protected override void _bindExUI()
		{
			mClose = mBind.GetCom<Button>("Close");
			if (null != mClose)
			{
				mClose.onClick.AddListener(_onCloseButtonClick);
			}
			mTips1 = mBind.GetGameObject("tips1");
			mTips2 = mBind.GetGameObject("tips2");
			mTips3 = mBind.GetGameObject("tips3");
			mTAPItemComUIList = mBind.GetCom<ComUIListScript>("TAPItemComUIList");
			mSubmit = mBind.GetCom<Button>("Submit");
			if (null != mSubmit)
			{
				mSubmit.onClick.AddListener(_onSubmitButtonClick);
			}
		}
		
		protected override void _unbindExUI()
		{
			if (null != mClose)
			{
				mClose.onClick.RemoveListener(_onCloseButtonClick);
			}
			mClose = null;
			mTips1 = null;
			mTips2 = null;
			mTips3 = null;
			mTAPItemComUIList = null;
			if (null != mSubmit)
			{
				mSubmit.onClick.RemoveListener(_onSubmitButtonClick);
			}
			mSubmit = null;
		}
		#endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        private void _onSubmitButtonClick()
        {
            /* put your code in here */
            switch(tapGraduationType)
            {
                case TAPGraduationType.TeacherGraduation:
                    {
                        TAPNewDataManager.GetInstance().SendGraduation(relationData.uid);
                        break;
                    }
                case TAPGraduationType.TeacherGraduationLate:
                    {
                        TAPNewDataManager.GetInstance().SendGraduationLate(relationData.uid);
                        break;
                    }
                case TAPGraduationType.PupilGraduationLate:
                    {
                        TAPNewDataManager.GetInstance().SendGraduationLate(relationData.uid);
                        break;
                    }
            }
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
