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
    public class Pk3v3RoomSettingFrame : ClientFrame
    {
        const int RankTypeNum = 6;

        string RoomName = "";

        int MinLv = 0;
        int MinRankLv = 0;

        bool bSetMinLv = false;
        bool bSetMinRankLv = false;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3/Pk3v3RoomSettingFrame";
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
            RoomName = "";

            MinLv = 0;
            MinRankLv = 0;

            bSetMinLv = false;
            bSetMinRankLv = false;
        }

        protected void BindUIEvent()
        {
            mRoomName.onValueChanged.AddListener(OnRoomNameInputEnd);
        }

        protected void UnBindUIEvent()
        {
            mRoomName.onValueChanged.RemoveListener(OnRoomNameInputEnd);
        }

        void OnRoomNameInputEnd(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            RoomName = name;
        }

        [UIEventHandle("middle/Levelselect/LevelList/Level{0}", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<int, bool>), 1, RankTypeNum)]
        void OnSwitchRankLvType(int iIndex, bool value)
        {
            if(!bSetMinRankLv)
            {
                mRankLvListObj.CustomActive(false);
                return;
            }

            if (iIndex < 0 || !value)
            {
                return;
            }

            MinRankLv = Pk3v3DataManager.GetInstance().GetRankLvByIndex(iIndex);

            mRankText.text = SeasonDataManager.GetInstance().GetSimpleRankName(MinRankLv);
            mRankLvListObj.CustomActive(false);
            mDownGo.CustomActive(false);
            mUpGo.CustomActive(true);
        }

        void InitInterface()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if(roomInfo == null || roomInfo.roomSimpleInfo == null)
            {
                Logger.LogError("roomInfo is null in pk3v3RoomSettingFrame");
                return;
            }

            Pk3v3RoomSettingData settingdata = null;

            if(!Pk3v3DataManager.GetInstance().RoomSettingData.TryGetValue((RoomType)roomInfo.roomSimpleInfo.roomType, out settingdata))
            {
                Logger.LogError("RoomSettingData is null in pk3v3RoomSettingFrame");
                return;
            }  

            mRoomNum.text = roomInfo.roomSimpleInfo.id.ToString();
            mRoomName.text = roomInfo.roomSimpleInfo.name;

            mPasswordSet.isOn = (bool)(roomInfo.roomSimpleInfo.isPassword > 0);
            mPassword.text = Pk3v3DataManager.GetInstance().PassWord;

            mMinLvSet.isOn = roomInfo.roomSimpleInfo.isLimitPlayerLevel > 0;
            MinLv = roomInfo.roomSimpleInfo.limitPlayerLevel;
            mMinLvText.text = MinLv.ToString();

            mMinRankSet.isOn = roomInfo.roomSimpleInfo.isLimitPlayerSeasonLevel > 0;
            MinRankLv = (int)roomInfo.roomSimpleInfo.limitPlayerSeasonLevel;
            mRankText.text = SeasonDataManager.GetInstance().GetSimpleRankName(MinRankLv);         
            
            if(roomInfo.roomSimpleInfo.isLimitPlayerLevel == 0)
            {
                mDefaultLevel.CustomActive(true);
            }
            else if(roomInfo.roomSimpleInfo.isLimitPlayerLevel == 1)
            {
                mDefaultLevel.CustomActive(false);
            }   
            if(roomInfo.roomSimpleInfo.isLimitPlayerSeasonLevel == 0)
            {
                mDefaultSeasonLevel.CustomActive(true);
            }   
            else if(roomInfo.roomSimpleInfo.isLimitPlayerSeasonLevel == 1)
            {
                mDefaultSeasonLevel.CustomActive(false);
            } 
        }

        void _changeLevelSelect(bool isOpen)
        {
            mUpGo.CustomActive(isOpen);
            var levelTextRectTransform = mRankText.gameObject.GetComponent<RectTransform>();
            if (levelTextRectTransform == null)
            {
                return;
            }
            if (isOpen)
            {
                levelTextRectTransform.anchoredPosition = new Vector2(-54, 0);
            }
            else
            {
                levelTextRectTransform.anchoredPosition = new Vector2(0, 0);
            }
        }

        #region ExtraUIBind
        private Button mBtClose = null;
        private Button mBtOK = null;
        private Text mRoomNum = null;
        private InputField mPassword = null;
        private Toggle mPasswordSet = null;
        private Toggle mMinLvSet = null;
        private Button mReduce = null;
        private Button mAdd = null;
        private Toggle mMinRankSet = null;
        private Button mBtRankSel = null;
        private InputField mRoomName = null;
        private Text mMinLvText = null;
        private Text mRankText = null;
        private GameObject mRankLvListObj = null;
        private GameObject mUpGo = null;
        private GameObject mDownGo = null;
        private Button mCloseLevelList = null;
        private GameObject mDefaultPassword = null;
        private GameObject mDefaultLevel = null;
        private GameObject mDefaultSeasonLevel = null;

        protected override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mBtOK = mBind.GetCom<Button>("btOK");
            mBtOK.onClick.AddListener(_onBtOKButtonClick);
            mRoomNum = mBind.GetCom<Text>("RoomNum");
            mPassword = mBind.GetCom<InputField>("Password");
            mPasswordSet = mBind.GetCom<Toggle>("PasswordSet");
            mPasswordSet.onValueChanged.AddListener(_onPasswordSetToggleValueChange);
            mMinLvSet = mBind.GetCom<Toggle>("MinLvSet");
            mMinLvSet.onValueChanged.AddListener(_onMinLvSetToggleValueChange);
            mReduce = mBind.GetCom<Button>("Reduce");
            mReduce.onClick.AddListener(_onReduceButtonClick);
            mAdd = mBind.GetCom<Button>("Add");
            mAdd.onClick.AddListener(_onAddButtonClick);
            mMinRankSet = mBind.GetCom<Toggle>("MinRank");
            mMinRankSet.onValueChanged.AddListener(_onMinRankSetToggleValueChange);
            mBtRankSel = mBind.GetCom<Button>("btRankSel");
            mBtRankSel.onClick.AddListener(_onBtRankSelButtonClick);
            mRoomName = mBind.GetCom<InputField>("RoomName");
            mMinLvText = mBind.GetCom<Text>("MinLvText");
            mRankText = mBind.GetCom<Text>("RankText");
            mRankLvListObj = mBind.GetGameObject("RankLvListObj");
            mUpGo = mBind.GetGameObject("upGo");
            mDownGo = mBind.GetGameObject("downGo");
            mCloseLevelList = mBind.GetCom<Button>("CloseLevelList");
            mCloseLevelList.onClick.AddListener(_OnCloseLevelListClick);
            mDefaultPassword = mBind.GetGameObject("DefaultPassword");
            mDefaultLevel = mBind.GetGameObject("DefaultLevel");
            mDefaultSeasonLevel = mBind.GetGameObject("DefaultSeasonLevel");
        }

        protected override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mBtOK.onClick.RemoveListener(_onBtOKButtonClick);
            mBtOK = null;
            mRoomNum = null;
            mPassword = null;
            mPasswordSet.onValueChanged.RemoveListener(_onPasswordSetToggleValueChange);
            mPasswordSet = null;
            mMinLvSet.onValueChanged.RemoveListener(_onMinLvSetToggleValueChange);
            mMinLvSet = null;
            mReduce.onClick.RemoveListener(_onReduceButtonClick);
            mReduce = null;
            mAdd.onClick.RemoveListener(_onAddButtonClick);
            mAdd = null;
            mMinRankSet.onValueChanged.RemoveListener(_onMinRankSetToggleValueChange);
            mMinRankSet = null;
            mBtRankSel.onClick.RemoveListener(_onBtRankSelButtonClick);
            mBtRankSel = null;
            mRoomName = null;
            mMinLvText = null;
            mRankText = null;
            mRankLvListObj = null;
            mUpGo = null;
            mDownGo = null;
            mCloseLevelList.onClick.RemoveListener(_OnCloseLevelListClick);
            mCloseLevelList = null;
            mDefaultPassword = null;
            mDefaultLevel = null;
            mDefaultSeasonLevel = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        private void _OnCloseLevelListClick()
        {
            mRankLvListObj.CustomActive(false);
            mDownGo.CustomActive(false);
            mUpGo.CustomActive(true);
        }
        private void _onBtOKButtonClick()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();
            if(roomInfo == null)
            {
                Logger.LogError("roomInfo is null when save room setting data.");
                return;
            }

            Pk3v3RoomSettingData settingdata = null;

            if (!Pk3v3DataManager.GetInstance().RoomSettingData.TryGetValue((RoomType)roomInfo.roomSimpleInfo.roomType, out settingdata))
            {
                Logger.LogErrorFormat("RoomSettingData is null when TryGetValue : {0}", (RoomType)roomInfo.roomSimpleInfo.roomType);
                return;
            }

            WorldUpdateRoomReq req = new WorldUpdateRoomReq();

            req.roomId = roomInfo.roomSimpleInfo.id;
            req.roomType = roomInfo.roomSimpleInfo.roomType;
            req.name = roomInfo.roomSimpleInfo.name;
            req.password = Pk3v3DataManager.GetInstance().PassWord;
            req.isLimitPlayerLevel = roomInfo.roomSimpleInfo.isLimitPlayerLevel;
            req.isLimitPlayerSeasonLevel = roomInfo.roomSimpleInfo.isLimitPlayerSeasonLevel;
            req.limitPlayerLevel = roomInfo.roomSimpleInfo.limitPlayerLevel;
            req.limitPlayerSeasonLevel = roomInfo.roomSimpleInfo.limitPlayerSeasonLevel;

            bool bNeedSend = false;

            if(RoomName != "" && RoomName != roomInfo.roomSimpleInfo.name)
            {    
                req.name = RoomName;
                bNeedSend = true;
            }

            if(mPassword.text != Pk3v3DataManager.GetInstance().PassWord)
            {
                req.password = mPassword.text;
                bNeedSend = true;

                Pk3v3DataManager.GetInstance().bHasPassword = (mPassword.text != "");
                Pk3v3DataManager.GetInstance().PassWord = mPassword.text;             
            }

            bool isLimitPlayerLevel = roomInfo.roomSimpleInfo.isLimitPlayerLevel > 0;
            if (bSetMinLv != isLimitPlayerLevel)
            {
                if(bSetMinLv)
                {
                    req.isLimitPlayerLevel = 1;
                }
                else
                {
                    req.isLimitPlayerLevel = 0;
                }
              
                bNeedSend = true;

                settingdata.bSetMinLv = bSetMinLv;

                PlayerLocalSetting.SetValue(Pk3v3DataManager.GetInstance().GetPk3v3LocalDataKey((RoomType)roomInfo.roomSimpleInfo.roomType, "bSetMinLv"), bSetMinLv.ToString());
            }

            bool isLimitPlayerSeasonLevel = roomInfo.roomSimpleInfo.isLimitPlayerSeasonLevel > 0;
            if (bSetMinRankLv != isLimitPlayerSeasonLevel)
            {
                if(bSetMinRankLv)
                {
                    req.isLimitPlayerSeasonLevel = 1;
                }
                else
                {
                    req.isLimitPlayerSeasonLevel = 0;
                }
               
                bNeedSend = true;

                settingdata.bSetMinRankLv = bSetMinRankLv;

                PlayerLocalSetting.SetValue(Pk3v3DataManager.GetInstance().GetPk3v3LocalDataKey((RoomType)roomInfo.roomSimpleInfo.roomType, "bSetMinRankLv"), bSetMinRankLv.ToString());
            }

            if(MinLv != roomInfo.roomSimpleInfo.limitPlayerLevel)
            {
                req.limitPlayerLevel = (ushort)MinLv;
                bNeedSend = true;

                settingdata.MinLv = MinLv;

                PlayerLocalSetting.SetValue(Pk3v3DataManager.GetInstance().GetPk3v3LocalDataKey((RoomType)roomInfo.roomSimpleInfo.roomType, "MinLv"), MinLv.ToString());
            }

            if (MinRankLv != roomInfo.roomSimpleInfo.limitPlayerSeasonLevel)
            {
                req.limitPlayerSeasonLevel = (uint)MinRankLv;
                bNeedSend = true;

                settingdata.MinRankLv = MinRankLv;

                PlayerLocalSetting.SetValue(Pk3v3DataManager.GetInstance().GetPk3v3LocalDataKey((RoomType)roomInfo.roomSimpleInfo.roomType, "MinRankLv"), MinRankLv.ToString());
            }

            if(bNeedSend)
            {
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
            }

            PlayerLocalSetting.SaveConfig();

            //这里是同步更改的房间名字到房间预制体上
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Set3v3RoomName, mRoomName.text);

            //这里是同步密码到房间预制体上
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Set3v3RoomPassword);
            frameMgr.CloseFrame(this);
        }
        private void _onPasswordSetToggleValueChange(bool changed)
        {
            if(changed)
            {
                mPassword.text = Pk3v3DataManager.GetInstance().RandPassWord().ToString();
                mDefaultPassword.CustomActive(false);
            }
            else
            {
                mDefaultPassword.CustomActive(true);
                mPassword.text = "";
            }
        }
        private void _onMinLvSetToggleValueChange(bool changed)
        {
            bSetMinLv = changed;

            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (bSetMinLv)
            {
                mDefaultLevel.CustomActive(false);
                MinLv = roomInfo.roomSimpleInfo.limitPlayerLevel;
            }
            else
            {
                mDefaultLevel.CustomActive(true);
                MinLv = Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Duel);
            }

            mMinLvText.text = MinLv.ToString();
        }
        private void _onReduceButtonClick()
        {
            if(!bSetMinLv)
            {
                return;
            }

            if(MinLv <= Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Duel))
            {
                return;
            }

            MinLv -= 5;

            mMinLvText.text = MinLv.ToString();
        }
        private void _onAddButtonClick()
        {
            if(!bSetMinLv)
            {
                return;
            }

            if(MinLv >= 60)
            {
                return;
            }

            MinLv += 5;

            mMinLvText.text = MinLv.ToString();
        }
        private void _onMinRankSetToggleValueChange(bool changed)
        {
            bSetMinRankLv = changed;
            _changeLevelSelect(changed);
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if(bSetMinRankLv)
            {
                mDefaultSeasonLevel.CustomActive(false);
                MinRankLv = (int)roomInfo.roomSimpleInfo.limitPlayerSeasonLevel;
            }
            else
            {
                mDefaultSeasonLevel.CustomActive(true);
                MinRankLv = SeasonDataManager.GetInstance().GetMinRankID();               
            }

            mRankText.text = SeasonDataManager.GetInstance().GetSimpleRankName(MinRankLv);
        }
        private void _onBtRankSelButtonClick()
        {
            if (bSetMinRankLv)
            {
                mRankLvListObj.CustomActive(!mRankLvListObj.activeSelf);
                mUpGo.CustomActive(!mRankLvListObj.activeSelf);
                mDownGo.CustomActive(mRankLvListObj.activeSelf);
            }
        }
        #endregion
    }
}
