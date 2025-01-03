using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    public class ChapterDropProgressFrame : ClientFrame
    {
        private UIGray[] uiGrayArray;
        private Image[] checkMarkArray;
        private int dungeonId;
        private bool resetFlag;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chapter/Normal/ChapterDropProgress";
        }
        #region ExtraUIBind
        private GameObject mRoom = null;
        private Text mTextNumber = null;
        private Button mButtonReset = null;
        private Button mButtonClose = null;
        private UIGray mUIGrayReset = null;

        protected override void _bindExUI()
        {
            mRoom = mBind.GetGameObject("Room");
            mTextNumber = mBind.GetCom<Text>("TextNumber");
            mButtonReset = mBind.GetCom<Button>("ButtonReset");
            if (null != mButtonReset)
            {
                mButtonReset.onClick.AddListener(_onButtonResetButtonClick);
            }
            mButtonClose = mBind.GetCom<Button>("ButtonClose");
            if (null != mButtonClose)
            {
                mButtonClose.onClick.AddListener(_onButtonCloseButtonClick);
            }
            mUIGrayReset = mBind.GetCom<UIGray>("UIGrayReset");
        }

        protected override void _unbindExUI()
        {
            mRoom = null;
            mTextNumber = null;
            if (null != mButtonReset)
            {
                mButtonReset.onClick.RemoveListener(_onButtonResetButtonClick);
            }
            mButtonReset = null;
            if (null != mButtonClose)
            {
                mButtonClose.onClick.RemoveListener(_onButtonCloseButtonClick);
            }
            mButtonClose = null;
            mUIGrayReset = null;
        }
        #endregion

        #region Callback
        private void _onButtonResetButtonClick()
        {
            int leftTime = _getLeftTimes();
            if (leftTime > 0 || (leftTime == 0 && ChapterNormalHalfFrame.IsYiJieDungeon(dungeonId) && ChapterNormalHalfFrame.IsCurrentDungeonInChallenge()))
            {
                _onSceneDungeonResetAreaIndex();
            }
        }
        private void _onButtonCloseButtonClick()
        {
            Close();
        }
        #endregion

        public void SetData(int dungeonId, uint areaIndex)
        {
            this.dungeonId = dungeonId;
            resetFlag = false;

            _setNumber();
            _setProgress(areaIndex);
        }

        private void _initFrame()
        {
            if (mRoom == null)
                return;

            uiGrayArray = new UIGray[5];
            checkMarkArray = new Image[5];
            for (int i = 0; i < 5; i++)
            {
                var room = Object.Instantiate(mRoom) as GameObject;
                room.CustomActive(true);
                room.name = string.Format("Room{0}", i);
                room.transform.SetParent(mRoom.transform.parent);
                room.transform.SetSiblingIndex(i);
                room.transform.localScale = Vector3.one;
                room.transform.localPosition = Vector3.zero;

                var box = room.transform.Find("Box");
                if (box != null)
                    uiGrayArray[i] = box.GetComponent<UIGray>();
                var check = room.transform.Find("Check");
                checkMarkArray[i] = check.GetComponent<Image>();
            }
        }

        private void _setNumber()
        {
            mTextNumber.text = string.Format(TR.Value("drop_progress_reset_text"), _getLeftTimes());
        }

        private int _getLeftTimes()
        {
            var finishedTimes = DungeonUtility.GetDungeonDailyFinishedTimes(dungeonId);
            var dailyMaxTime = DungeonUtility.GetDungeonDailyMaxTimes(dungeonId);
            var leftTimes = dailyMaxTime - finishedTimes;
            if (leftTimes < 0)
                leftTimes = 0;

            return leftTimes;
        }

        private void _setProgress(uint index)
        {
            mButtonReset.enabled = index != 0;
            mUIGrayReset.enabled = index == 0;

            for (int i = 0; i < 5; i++)
            {
                var flag = (index & 1) == 1;
                index >>= 1;
                uiGrayArray[i].enabled = flag;
                checkMarkArray[i].enabled = flag;
            }
        }

        private void _onSceneDungeonResetAreaIndex()
        {
            resetFlag = true;
            var req = new SceneDungeonResetAreaIndexReq();
            req.dungeonId = (uint)dungeonId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        private void _onCounterChanged(UIEvent ui)
        {
            if (resetFlag)
            {
                resetFlag = false;
                _setNumber();
                _setProgress(0);
            }
        }

        private void _bindEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CounterChanged, _onCounterChanged);
        }

        private void _unBindEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CounterChanged, _onCounterChanged);
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            _initFrame();
            _bindEvents();
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();

            _unBindEvents();
        }
    }
}
