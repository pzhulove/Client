using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using Protocol;
using Network;
using System.Diagnostics;
using DG.Tweening;
using System;
using ProtoTable;

namespace GameClient
{
    public class DungeonDeadTowerFinishFrame : ClientFrame, IDungeonFinish
    {
        int NowLevel = 0;
#region Override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Finish/DungeonDeadTowerFinish";
        }

        protected override void _OnOpenFrame()
        {
            //PlayerBaseData.GetInstance().onLevelChanged += UpdateLevel;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, UpdateLevel);
            mState = eFinishState.None;
            mComItemCache.Clear();
        }

        protected override void _OnCloseFrame()
        {
            //PlayerBaseData.GetInstance().onLevelChanged -= UpdateLevel;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, UpdateLevel);
            for (int i = 0; i < mComItemCache.Count; ++i)
            {
                GameObject.Destroy(mComItemCache[i].gameObject);
            }

            mComItemCache.Clear();
        }

        void UpdateLevel(UIEvent uievent)
        {
            setContinueBt(NowLevel);
        }
#endregion
        #region ExtraUIBind
        private GameObject mSetCurrentTimeObject = null;
        private GameObject mSetBestTimeObject = null;
        private GameObject mGotList = null;
        private GameObject mContinueButton = null;
        private Text mLevel = null;
        private ComTime mCurrentTime = null;
        private ComTime mBestTime = null;
        private GameObject mGotListRoot = null;
        private Button mBack = null;
        private Button mContinue = null;
        private ComItemList mGotListItems = null;
        private GameObject mIsBestRecord = null;
        private GameObject mCannotContinue = null;
        private Text mCannotContinueText = null;
        private UIGray mContinueGray = null;
        private Text mContinueText = null;
        private GameObject mCannotContinueText1 = null;
        private GameObject mCannotContinueText2 = null;

        protected override void _bindExUI()
        {
            mSetCurrentTimeObject = mBind.GetGameObject("setCurrentTimeObject");
            mSetBestTimeObject = mBind.GetGameObject("setBestTimeObject");
            mGotList = mBind.GetGameObject("gotList");
            mContinueButton = mBind.GetGameObject("continueButton");
            mLevel = mBind.GetCom<Text>("level");
            mCurrentTime = mBind.GetCom<ComTime>("CurrentTime");
            mBestTime = mBind.GetCom<ComTime>("BestTime");
            mGotListRoot = mBind.GetGameObject("GotListRoot");
            mBack = mBind.GetCom<Button>("back");
            mBack.onClick.AddListener(_onBackButtonClick);
            mContinue = mBind.GetCom<Button>("continue");
            mContinue.onClick.AddListener(_onContinueButtonClick);
            mGotListItems = mBind.GetCom<ComItemList>("GotListItems");
            mIsBestRecord = mBind.GetGameObject("IsBestRecord");
            mCannotContinue = mBind.GetGameObject("CannotContinue");
            mCannotContinueText = mBind.GetCom<Text>("CannotContinueText");
            mContinueGray = mBind.GetCom<UIGray>("ContinueGray");
            mContinueText = mBind.GetCom<Text>("ContinueText");
            mCannotContinueText1 = mBind.GetGameObject("CannotContinueText1");
            mCannotContinueText2 = mBind.GetGameObject("CannotContinueText2");
        }

        protected override void _unbindExUI()
        {
            mSetCurrentTimeObject = null;
            mSetBestTimeObject = null;
            mGotList = null;
            mContinueButton = null;
            mLevel = null;
            mCurrentTime = null;
            mBestTime = null;
            mGotListRoot = null;
            mBack.onClick.RemoveListener(_onBackButtonClick);
            mBack = null;
            mContinue.onClick.RemoveListener(_onContinueButtonClick);
            mContinue = null;
            mGotListItems = null;
            mIsBestRecord = null;
            mCannotContinue = null;
            mCannotContinueText = null;
            mContinueGray = null;
            mContinueText = null;
            mCannotContinueText1 = null;
            mCannotContinueText2 = null;
        }
#endregion   


#region Callback
        private void _onBackButtonClick()
        {
            /* put your code in here */

            _onBack();
        }
        private void _onContinueButtonClick()
        {
            /* put your code in here */

            _onContinue();
        }
#endregion

        public enum eFinishState
        {
            None,
            Back,
            Continue
        }

        private eFinishState mState = eFinishState.None;

        public eFinishState state
        {
            get
            {
                return mState;
            }
        }

        public void SetLevel(int level)
        {
            if (level < 0)
            {
                //mDi.SetActive(false);
            }
            else
            {
                mLevel.text = level.ToString();
                NowLevel = level;
                setContinueBt(level);
            }
        }

        private void setContinueBt(int level)
        {
            mCannotContinue.CustomActive(false);
            mCannotContinueText1.CustomActive(false);
            mCannotContinueText2.CustomActive(false);
            //mContinueGray.enabled = false;
            //mContinueText.text = "继续挑战";
            if (level == ChapterUtility.kDeadTowerTopLevel)
            {
                return;
            }

            var DeathTowerAwardTableData = TableManager.GetInstance().GetTableItem<DeathTowerAwardTable>(level+1);
            if(DeathTowerAwardTableData == null)
            {
                Logger.LogErrorFormat("DeathTowerTableData is null");
                return;
            }
            int LimitLevel = DeathTowerAwardTableData.LimitLevel;
            
            if (LimitLevel != 0 && PlayerBaseData.GetInstance().Level<LimitLevel)
            {
                //mContinueGray.enabled = true;
                //mContinueText.text = string.Format("{0}级开启", LimitLevel);
                mCannotContinueText1.CustomActive(true);
                mCannotContinueText2.CustomActive(true);
                mCannotContinue.CustomActive(true);

                if (null != mCannotContinueText)
                {
                    mCannotContinueText.text = string.Format("{0}级开启",LimitLevel);
                }
            }
        }

        public void SetCurrentTime(int time)
        {
            if (time < 0)
            {
                mSetCurrentTimeObject.SetActive(false);
            }

            mCurrentTime.SetTime(time);
        }


        public void SetBestTime(int time)
        {
            if (time < 0)
            {
                mSetBestTimeObject.SetActive(false);
            }

            mBestTime.SetTime(time);
        }

        private List<ComItem> mComItemCache = new List<ComItem>();

        public void SetFinish(bool isFinish)
        {
            mContinueButton.SetActive(!isFinish);
            if (isFinish)
            {
            }
            else
            {
            }
        }

        public void SetName(string name)
        {
        }

        public void SetIsNewRecord(bool isNew)
        {
            mIsBestRecord.SetActive(isNew);
        }

        public void SetComItemList(ComItemList.Items[] list)
        {
            if (mGotListItems == null)
                return;
            mGotListItems.SetItems(list);
        }


        private void _onBack()
        {
            mState = eFinishState.Back;
            Close();
        }

        private void _onContinue()
        {
            mState = eFinishState.Continue;
            Close();
        }

        public void SetDrops(ComItemList.Items[] items)
        {
        }

        public void SetDiff(int diff)
        {
        }

        public void SetExp(ulong exp)
        {
        }
    }
}
