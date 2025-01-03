using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using UnityEngine.UI;

namespace GameClient
{
    class TAPApplyPupilFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TAPSystem/TAPApplyPupilFrame";
        }

        //[UIControl("Root", typeof(StateController))]
        //StateController comState;

        //[UIControl("Root/Friends", typeof(ComUIListScript))]
        //ComUIListScript comPupilApplyList;

        protected override void _OnOpenFrame()
        {
            _AddButton("Close", () =>
             {
                 frameMgr.CloseFrame(this);
                 UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnApplyPupilListChanged);
             });
            _AddButton("Root/BtnClearAll", _ClearAll);

            comPupilApplyList.Initialize();
            comPupilApplyList.onBindItem = (GameObject go) =>
            {
                if (null != go)
                {
                    return go.GetComponent<ComApplyPupilInfo>();
                }
                return null;
            };
            comPupilApplyList.onItemVisiable = (ComUIListElementScript item) =>
            {
                if(null != item)
                {
                    var script = item.gameObjectBindScript as ComApplyPupilInfo;
                    if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher)
                    {
                        var datas = RelationDataManager.GetInstance().ApplyPupils;
                        if (null != script && item.m_index >= 0 && item.m_index < datas.Count)
                        {
                            script.OnItemVisible(datas[item.m_index]);
                        }
                    }
                    else if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Pupil)
                    {
                        var datas = RelationDataManager.GetInstance().ApplyTeachers;
                        if (null != script && item.m_index >= 0 && item.m_index < datas.Count)
                        {
                            script.OnItemVisible(datas[item.m_index]);
                        }
                    }
                    
                }
            };

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnApplyPupilListChanged, _OnApplyPupilListChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnApplyTeacherListChanged, _OnApplyTeacherListChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnShowPupilApplyMenu, _OnShowPupilApplyMenu);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCloseMenu, _OnCloseMenu);

            RelationDataManager.GetInstance().RemoveApplyPupilNotify();

            _updateData();
        }

        void _OnApplyPupilListChanged(UIEvent uiEvent)
        {
            _updateData();
        }

        void _OnApplyTeacherListChanged(UIEvent uiEvent)
        {
            _updateData();
        }

        #region Menu
        IClientFrame m_openMenu = null;
        protected void _OnShowPupilApplyMenu(UIEvent uiEvent)
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
        #endregion

        void _ClearAll()
        {
            RelationDataManager.GetInstance().RefuseAllApplyPupils();
            RelationDataManager.GetInstance().RefuseAllApplyTeachers();
        }

        void _updateData()
        {
            List<RelationData> datas = new List<RelationData>();
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher)
            {
                datas = RelationDataManager.GetInstance().ApplyPupils;
            }
            else if(TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Pupil)
            {
                datas = RelationDataManager.GetInstance().ApplyTeachers;
            }
            if (datas.Count > 0)
            {
                comState.Key = "has_apply";
            }
            else
            {
                comState.Key = "no_apply";
            }
            if(null != comPupilApplyList)
            {
                comPupilApplyList.SetElementAmount(datas.Count);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPApplyToggleRedPointUpdate);
        }

        protected override void _OnCloseFrame()
        {
            if(null != comPupilApplyList)
            {
                comPupilApplyList.onBindItem = null;
                comPupilApplyList.onItemVisiable = null;
                comPupilApplyList = null;
            }
            if (m_openMenu != null)
            {
                frameMgr.CloseFrame(m_openMenu);
                m_openMenu = null;
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnApplyPupilListChanged, _OnApplyPupilListChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnApplyTeacherListChanged, _OnApplyTeacherListChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnShowPupilApplyMenu, _OnShowPupilApplyMenu);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCloseMenu, _OnCloseMenu);
        }
		#region ExtraUIBind
		private ComUIListScript comPupilApplyList = null;
		private StateController comState = null;
        private Button mClose = null;
		
		protected override void _bindExUI()
		{
			comPupilApplyList = mBind.GetCom<ComUIListScript>("comPupilApplyList");
			comState = mBind.GetCom<StateController>("comState");
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
		}
		
		protected override void _unbindExUI()
		{
			comPupilApplyList = null;
			comState = null;
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
		}
		#endregion
        
        #region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            if (m_openMenu != null)
            {
                frameMgr.CloseFrame(m_openMenu);
                m_openMenu = null;
            }
        }
        #endregion
    }
}
