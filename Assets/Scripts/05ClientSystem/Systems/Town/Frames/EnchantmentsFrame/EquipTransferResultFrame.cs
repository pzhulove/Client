using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EquipTransferResultFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/EquipTransferResultFrame";
        }

        #region ExtraUIBind
        private GameObject mItemRoot = null;
        private Text mItemName = null;
        private Button mOK = null;

        protected override void _bindExUI()
        {
            mItemRoot = mBind.GetGameObject("itemRoot");
            mItemName = mBind.GetCom<Text>("itemName");
            mOK = mBind.GetCom<Button>("OK");
            mOK.onClick.AddListener(_onOKButtonClick);
        }

        protected override void _unbindExUI()
        {
            mItemRoot = null;
            mItemName = null;
            mOK.onClick.RemoveListener(_onOKButtonClick);
            mOK = null;
        }
        #endregion

        #region Callback
        private void _onOKButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.instance.CloseFrame(this);
        }
        #endregion

        protected override void _OnOpenFrame()
        {
            ItemData itemData = this.userData as ItemData;
            if (null == itemData)
            {
                return;
            }

            ComItemNew item = CreateComItemNew(mItemRoot);
            item.Setup(itemData, null);

            mItemName.text = itemData.GetColorName();
        }
    }
}