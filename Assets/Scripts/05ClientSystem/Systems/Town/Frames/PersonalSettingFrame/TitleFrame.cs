using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;

namespace GameClient
{
    public class TitleFrame : ClientFrame
    {
        #region ExtraUIBind
        private TitleView mTitleView = null;

        protected override void _bindExUI()
        {
            mTitleView = mBind.GetCom<TitleView>("TitleView");
        }

        protected override void _unbindExUI()
        {
            mTitleView = null;
        }
        #endregion
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/PersonalSettingFrame/TitleFrame";
        }

        protected override void _OnOpenFrame()
        {
            _bindExUI();
            _BindUIEvent();
            mTitleView.InitUI(0);//初始化
            mTitleView.UpdateToggleUI(0);//选择第一个页签
        }

        protected override void _OnCloseFrame()
        {
            _unbindExUI();
            _UnBindUIEvent();
        }


        #region ui event
        void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TitleDataUpdate, _UpdateTitleFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TitleGuidUpdate, _UpdateMyTitleUI);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TitleModeUpdate, _UpdateTitleMode);
        }

        void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TitleDataUpdate, _UpdateTitleFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TitleGuidUpdate, _UpdateMyTitleUI);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TitleModeUpdate, _UpdateTitleMode);
        }
        
        void _UpdateTitleFrame(UIEvent uiEvent)
        {
            mTitleView.UpdateToggleUI(-1);
        }

        void _UpdateMyTitleUI(UIEvent uiEvent)
        {
            mTitleView.UpdateToggleUI(-1);
        }

        void _UpdateTitleMode(UIEvent uiEvent)
        {
            mTitleView._InitModel();
        }
        #endregion
    }
}

