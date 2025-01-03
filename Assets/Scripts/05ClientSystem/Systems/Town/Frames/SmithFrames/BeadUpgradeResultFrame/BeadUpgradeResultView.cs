using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    public class BeadUpgradeResultView : MonoBehaviour
    {
        [SerializeField]
        private GameObject mItemParent;
        [SerializeField]
        private Text mItemName;
        [SerializeField]
        private Text mItemArrt;
        [SerializeField]
        private Button mOkBtn;

        ClientFrame clientFrame;
        public void InitView(ClientFrame clientFrame,BeadUpgradeResultData data)
        {
            this.clientFrame = clientFrame;
            ComItemNew mComItem = ComItemManager.CreateNew(mItemParent);
            ItemData mItemData = null;
            if (data.mountedType == (int)UpgradePrecType.Mounted)
            {
                mItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.mBeadID);
            }
            else
            {
                mItemData = ItemDataManager.GetInstance().GetItem(data.mBeadGUID);
            }
            if (mItemData != null)
            {
                mComItem.Setup(mItemData, Utility.OnItemClicked);
            }

            mItemName.text = mItemData.GetColorName();

            mItemArrt.text = string.Format("<color={0}>宝珠属性:</color>", "#0FCF6Aff");
            mItemArrt.text += BeadCardManager.GetInstance().GetAttributesDesc(mItemData.TableID);
            mItemArrt.text += "\n";
            if (data.mBuffID > 0)
            {
                mItemArrt.text += string.Format("附加属性:{0}", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(data.mBuffID));
            }
           
            if (mOkBtn != null)
            {
                mOkBtn.onClick.RemoveAllListeners();
                mOkBtn.onClick.AddListener(() => 
                {
                    (clientFrame as BeadUpgradeResultFrame).Close();
                });
            }
        }

        void OnDestroy()
        {
            if (mOkBtn != null)
            {
                mOkBtn.onClick.RemoveAllListeners();
            }
            this.clientFrame = null;
        }
    }
}
