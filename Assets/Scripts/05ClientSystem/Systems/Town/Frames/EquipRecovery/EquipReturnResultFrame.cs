using UnityEngine.UI;
using Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using EItemType = ProtoTable.ItemTable.eType;
using ProtoTable;
using Protocol;

namespace GameClient
{
    class EquipReturnResultItem
    {
        public ItemData itemdata;
        public int Score;
    }
    class EquipReturnResultFrame : ClientFrame
    {
        EquipReturnResultItem resultItem = new EquipReturnResultItem();
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/EquipRecovery/EquipReturnResultFrame";
        }
        protected sealed override void _OnOpenFrame()
        {
            if(userData == null)
            {
                frameMgr.CloseFrame(this);
                return;
            }
            resultItem = userData as EquipReturnResultItem;
            _InitUI();
        }
        protected sealed override void _OnCloseFrame()
        {

        }

        void _InitUI()
        {
            //初始化icon
            ComItem comitem = mItemRoot.GetComponentInChildren<ComItem>();
            if (comitem == null)
            {
                comitem = CreateComItem(mItemRoot);
            }
            int result = resultItem.itemdata.TableID;
            comitem.Setup(resultItem.itemdata, (GameObject Obj, ItemData sitem) => { _OnShowTips(result); });

            mItemScore.text = resultItem.Score.ToString();

            mName.text = resultItem.itemdata.Name;
        }


        void _OnShowTips(int result)
        {
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result);
            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }

        #region ExtraUIBind
        private GameObject mItemRoot = null;
        private Text mItemScore = null;
        private Button mOkBtn = null;
        private Text mName = null;

        protected override void _bindExUI()
        {
            mItemRoot = mBind.GetGameObject("ItemRoot");
            mItemScore = mBind.GetCom<Text>("ItemScore");
            mOkBtn = mBind.GetCom<Button>("okBtn");
            if (null != mOkBtn)
            {
                mOkBtn.onClick.AddListener(_onOkBtnButtonClick);
            }
            mName = mBind.GetCom<Text>("name");
        }

        protected override void _unbindExUI()
        {
            mItemRoot = null;
            mItemScore = null;
            if (null != mOkBtn)
            {
                mOkBtn.onClick.RemoveListener(_onOkBtnButtonClick);
            }
            mOkBtn = null;
            mName = null;
        }
        #endregion

        #region Callback
        private void _onOkBtnButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
