using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;
using Protocol;
using ProtoTable;
using UnityEngine.EventSystems;

namespace GameClient
{
    public class QuickBuyTimesFrame : ClientFrame
    {
        public enum eState
        {
            None,
            Success,
            Cancel,
        } 

        public eState state
        {
            private set; get;
        }

#region ExtraUIBind
        private Text mLeftcount = null;
        private Text mCoincount = null;
        private Text mCointype = null;
        private Button mCancel = null;
        private Button mOk = null;
        private Image mIcon = null;

        protected override void _bindExUI()
        {
            mLeftcount = mBind.GetCom<Text>("leftcount");
            mCoincount = mBind.GetCom<Text>("coincount");
            mCointype = mBind.GetCom<Text>("cointype");
            mCancel = mBind.GetCom<Button>("cancel");
            mCancel.onClick.AddListener(_onCancelButtonClick);
            mOk = mBind.GetCom<Button>("ok");
            mOk.onClick.AddListener(_onOkButtonClick);
            mIcon = mBind.GetCom<Image>("icon");
        }

        protected override void _unbindExUI()
        {
            mLeftcount = null;
            mCoincount = null;
            mCointype = null;
            mCancel.onClick.RemoveListener(_onCancelButtonClick);
            mCancel = null;
            mOk.onClick.RemoveListener(_onOkButtonClick);
            mOk = null;
            mIcon = null;
        }
#endregion    

#region Callback
        private void _onCancelButtonClick()
        {
            /* put your code in here */
            _onCancel();

        }
        private void _onOkButtonClick()
        {
            /* put your code in here */
            _onOK();
        }
#endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/CommonTimesQuickBuy";
        }

        protected override void _OnOpenFrame()
        {
            state = eState.None;
        }

        protected override void _OnCloseFrame()
        {
        }

        public void SetLeftCount(int leftCount)
        {
            mLeftcount.text = leftCount.ToString();
        }

        public void SetCostItem(int id, int count)
        {
            ItemTable needItemData = TableManager.instance.GetTableItem<ItemTable>(id);
            if (needItemData != null)
            {
                // mIcon.sprite = AssetLoader.instance.LoadRes(needItemData.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref mIcon, needItemData.Icon);
            }
            mCoincount.text = count.ToString();

        }

        private void _onOK()
        {
            state = eState.Success;
            frameMgr.CloseFrame(this);
        }

        private void _onCancel()
        {
            state = eState.Cancel;
            frameMgr.CloseFrame(this);
        }
    }
}
