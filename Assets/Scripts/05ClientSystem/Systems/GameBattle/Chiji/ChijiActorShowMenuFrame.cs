using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class ChijiOtherPlayerData
    {
        ///  public BeTownPlayerData beTownPlayerData;
        public BeFighterData beTownPlayerData;
        public Transform transform;
    }

    public class ChijiActorShowMenuFrame : ClientFrame
    {
        ChijiOtherPlayerData otherPlayerData = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiActorShowMenuFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                otherPlayerData = userData as ChijiOtherPlayerData;
            }

            _InitInterface();
            _SetFramePosition(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }

        protected override void _OnCloseFrame()
        {
            _ClearData();
        }

        private void _ClearData()
        {
            if(otherPlayerData != null)
            {
                otherPlayerData = null;
            }
        }

        private void _InitInterface()
        {
            if (mName != null)
            {
                mName.text = otherPlayerData.beTownPlayerData.Name;
            }

            string strPath = "";
            JobTable jobTable = TableManager.GetInstance().GetTableItem<JobTable>(otherPlayerData.beTownPlayerData.JobID);
            if (jobTable == null)
            {
                Logger.LogErrorFormat("JobTable not Find ID = {0}", otherPlayerData.beTownPlayerData.JobID);
            }
            else
            {
                ResTable resTable = TableManager.GetInstance().GetTableItem<ResTable>(jobTable.Mode);
                if (resTable != null)
                {
                    strPath = resTable.IconPath;
                }
            }

            if (mJobName != null)
            {
                mJobName.text = jobTable.Name;
            }

            if (mPortrait != null)
            {
                ETCImageLoader.LoadSprite(ref mPortrait, strPath);
            }
        }

        private void _SetFramePosition(Vector2 pos)
        {
            RectTransform mRectContent = mContent.GetComponent<RectTransform>();
            RectTransform mRectParent = mRectContent.transform.parent as RectTransform;
            Vector2 mLoaclPos;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mRectParent, pos,ClientSystemManager.GetInstance().UICamera, out mLoaclPos))
            {
                Vector2 mBorder = new Vector2(10.0f, 10.0f);
                float fXMin = mBorder.x;
                float fXMax = mRectParent.rect.size.x - mBorder.x - mRectContent.rect.size.x;
                float fYMax = mBorder.y;
                float fYMin = -(mRectParent.rect.size.y - mBorder.y - mRectContent.rect.size.y);

                mLoaclPos.x = Mathf.Clamp(mLoaclPos.x, fXMin, fXMax);
                mLoaclPos.y = Mathf.Clamp(mLoaclPos.y, fYMin, fYMax);

                mRectContent.anchoredPosition = mLoaclPos;
            }
        }

        #region ExtraUIBind
        private Button mBtClose = null;
        private Button mBtPk = null;
        private Image mPortrait = null;
        private Text mName = null;
        private Text mJobName = null;
        private GameObject mContent = null;

        protected override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            if (null != mBtClose)
            {
                mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            }
            mBtPk = mBind.GetCom<Button>("btPk");
            if (null != mBtPk)
            {
                mBtPk.onClick.AddListener(_onBtPkButtonClick);
            }
            mPortrait = mBind.GetCom<Image>("portrait");
            mName = mBind.GetCom<Text>("name");
            mJobName = mBind.GetCom<Text>("jobName");
            mContent = mBind.GetGameObject("Content");
        }

        protected override void _unbindExUI()
        {
            if (null != mBtClose)
            {
                mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            }
            mBtClose = null;
            if (null != mBtPk)
            {
                mBtPk.onClick.RemoveListener(_onBtPkButtonClick);
            }
            mBtPk = null;
            mPortrait = null;
            mName = null;
            mJobName = null;
            mContent = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }

        private void _onBtPkButtonClick()
        {
            ClientSystemGameBattle currentBattle = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (currentBattle != null && currentBattle.MainPlayer != null && otherPlayerData != null && otherPlayerData.beTownPlayerData != null)
            {
                ChijiDataManager.GetInstance().SendBattlePkSomeOneReq(otherPlayerData.beTownPlayerData.GUID, currentBattle.MainPlayer.GetPKDungeonID());
            }

            _onBtCloseButtonClick();
        }    
        #endregion
    }
}
