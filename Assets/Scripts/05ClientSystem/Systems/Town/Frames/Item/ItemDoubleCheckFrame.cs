using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

namespace GameClient
{
    class ItemDoubleCheckData
    {
        public UnityAction<bool> mCallBack { get; set; }
        public string Desc { get; set; }
        public ItemData itemData { get; set; }
    }

    class ItemDoubleCheckFrame : ClientFrame
    {
        #region ExtraUIBind
        private Toggle mCanNotify = null;
        private Button mCancel = null;
        private Button mOK = null;
        private Text mText = null;
        private Button mShowTip = null;
        private HorizontalLayoutGroup mBtns = null;

        protected override void _bindExUI()
        {
            mCanNotify = mBind.GetCom<Toggle>("CanNotify");
            if (null != mCanNotify)
            {
                mCanNotify.onValueChanged.AddListener(_onCanNotifyToggleValueChange);
            }
            mCancel = mBind.GetCom<Button>("Cancel");
            if (null != mCancel)
            {
                mCancel.onClick.AddListener(_onCancelButtonClick);
            }
            mOK = mBind.GetCom<Button>("OK");
            if (null != mOK)
            {
                mOK.onClick.AddListener(_onOKButtonClick);
            }
            mText = mBind.GetCom<Text>("Text");

            mShowTip = mBind.GetCom<Button>("ShowTip");
            mShowTip.SafeRemoveAllListener();
            mShowTip.SafeAddOnClickListener(() =>
            {
                if (itemDataToUse != null && box2key != null && box2key.ContainsKey(itemDataToUse.TableID))
                {
                    ItemData itemData = ItemDataManager.GetInstance().GetItemByTableID(box2key[itemDataToUse.TableID]);
                    if (itemData == null)
                    {
                        itemData = ItemDataManager.CreateItemDataFromTable(box2key[itemDataToUse.TableID]);
                    }
                    if (itemData != null)
                    {
                        ItemTipManager.GetInstance().CloseAll();
                        ItemTipManager.GetInstance().ShowTip(itemData);
                    }
                }               
            });
            mBtns = mBind.GetCom<HorizontalLayoutGroup>("btns");
        }

        protected override void _unbindExUI()
        {
            if (null != mCanNotify)
            {
                mCanNotify.onValueChanged.RemoveListener(_onCanNotifyToggleValueChange);
            }
            mCanNotify = null;
            if (null != mCancel)
            {
                mCancel.onClick.RemoveListener(_onCancelButtonClick);
            }
            mCancel = null;
            if (null != mOK)
            {
                mOK.onClick.RemoveListener(_onOKButtonClick);
            }
            mOK = null;
            mText = null;
            mShowTip = null;
            mBtns = null;
        }
        #endregion

        #region Callback
        private void _onCanNotifyToggleValueChange(bool changed)
        {
            /* put your code in here */

        }
        private void _onCancelButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        private void _onOKButtonClick()
        {
            if (this.mCallBack != null)
            {
                this.mCallBack(this.mCanNotify.isOn);
            }
            frameMgr.CloseFrame(this);
        }
        #endregion
        private UnityAction<bool> mCallBack;

        // 使用幸运魔方时界面上额外有个按钮，可以展示幸运钥匙的tips add by qxy 2019-08-16
        private ItemData itemDataToUse = null;
        readonly static Dictionary<int, int> box2key = new Dictionary<int, int>()
        {
            { 800001055, 800001056 },
            { 800001126, 800001127 },
        };

//         const int luckyBoxTableID = 800001055;
//         const int luckyBoxKeyTableID = 800001056;

//         800001126  米亚的宝箱
// 800001127  米亚的钥匙

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Tip/ItemDoubleCheckFrame";
        }

        protected override void _OnOpenFrame()
        {
            ItemDoubleCheckData data = userData as ItemDoubleCheckData;
            if (data == null)
            {
                Logger.LogError("open ItemDoubleCheckFrame frame data is null");
                return;
            }

            mCallBack = data.mCallBack;
            mText.text = data.Desc.Replace("\\n", "\n");
            itemDataToUse = data.itemData;

            bool bShowTipsBtn = false;
            if(itemDataToUse != null && box2key != null && box2key.ContainsKey(itemDataToUse.TableID))
            {
                bShowTipsBtn = true;
            }
            if (bShowTipsBtn)
            {
                mShowTip.CustomActive(true);
                if(mBtns != null)
                {
                    mBtns.spacing = 5;
                }                
            }
            else
            {
                mShowTip.CustomActive(false);
                if(mBtns != null)
                {
                    mBtns.spacing = 100;
                }                
            }
        }        
    }
}
