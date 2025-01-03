using Network;
using Protocol;
using UnityEngine;
using ProtoTable;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class Pk3v3TypeChooseFrame : ClientFrame
    {
        int iMinLv = 0;
        int iMinRankLv = 0;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3/Pk3v3TypeChooseFrame";
        }

        protected override void _OnOpenFrame()
        {
            InitInterface();
            BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
            UnBindUIEvent();
        }

        void ClearData()
        {
            iMinLv = 0;
            iMinRankLv = 0;
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3RoomInfoUpdate, OnPk3v3RoomInfoUpdate);
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3RoomInfoUpdate, OnPk3v3RoomInfoUpdate);
        }

        void OnPk3v3RoomInfoUpdate(UIEvent iEvent)
        {
            frameMgr.CloseFrame(this);
        }

        void InitInterface()
        {
            iMinLv = Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Duel);
            iMinRankLv = SeasonDataManager.GetInstance().GetMinRankID();
        }

        #region ExtraUIBind
        private Button mBtAmusement = null;
        private Button mBtMatch = null;
        private Button mBtClose = null;
        private Toggle mTgCreatePassword = null;
        private Text mPassword = null;
        private Text mPasswordTips = null;

        protected override void _bindExUI()
        {
            mBtAmusement = mBind.GetCom<Button>("btAmusement");
            mBtAmusement.onClick.AddListener(_onBtAmusementButtonClick);
            mBtMatch = mBind.GetCom<Button>("btMatch");
            mBtMatch.onClick.AddListener(_onBtMatchButtonClick);
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mTgCreatePassword = mBind.GetCom<Toggle>("tgCreatePassword");
            mTgCreatePassword.onValueChanged.AddListener(_onTgCreatePasswordToggleValueChange);
            mPassword = mBind.GetCom<Text>("Password");
            mPasswordTips = mBind.GetCom<Text>("PasswordTips");
        }

        protected override void _unbindExUI()
        {
            mBtAmusement.onClick.RemoveListener(_onBtAmusementButtonClick);
            mBtAmusement = null;
            mBtMatch.onClick.RemoveListener(_onBtMatchButtonClick);
            mBtMatch = null;
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mTgCreatePassword.onValueChanged.RemoveListener(_onTgCreatePasswordToggleValueChange);
            mTgCreatePassword = null;
            mPassword = null;
            mPasswordTips = null;
        }
        #endregion

        #region Callback
        private void _onBtAmusementButtonClick()
        {
            Pk3v3DataManager.GetInstance().SendCreateRoomReq(RoomType.ROOM_TYPE_THREE_FREE);
        }

        private void _onBtMatchButtonClick()
        {
            if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_3v3_TUMBLE))
            {
                SystemNotifyManager.SystemNotify(5);
                return;
            }
            Pk3v3DataManager.GetInstance().SendCreateRoomReq(RoomType.ROOM_TYPE_MELEE);
        }

        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }

        private void _onTgCreatePasswordToggleValueChange(bool changed)
        {
            Pk3v3DataManager.GetInstance().bHasPassword = changed;

            if(changed)
            {
                Pk3v3DataManager.GetInstance().PassWord = Pk3v3DataManager.GetInstance().RandPassWord().ToString();
            }
            else
            {
                Pk3v3DataManager.GetInstance().PassWord = "";
            }

            mPasswordTips.gameObject.CustomActive(!changed);
            mPassword.text = Pk3v3DataManager.GetInstance().PassWord;
        }
        #endregion
    }
}
