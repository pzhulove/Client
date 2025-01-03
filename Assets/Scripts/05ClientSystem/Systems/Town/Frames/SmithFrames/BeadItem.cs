using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    class BeadItem : MonoBehaviour
    {
        public static BeadItemModel ms_select = null;
        [SerializeField]
        private ComCommonBind mBind;
        [SerializeField]
        private Text mName;
        [SerializeField]
        private Text mBeadAttr;
        [SerializeField]
        private Text mCheckName;
        [SerializeField]
        private Text mCheckBeadAttr;
        [SerializeField]
        private GameObject goItemParent;
        [SerializeField]
        private GameObject goCheckMark;
        [SerializeField]
        private ScrollRect mScrollRect;
        [SerializeField]
        private ScrollRect mCheckScrollRect;
        [HeaderAttribute("是否显示宝珠置换次数")]
        [SerializeField]
        private bool mIsShowBeadReplaceNumber = false;
        ComItemNew comItem;
        BeadItemModel model;
        public BeadItemModel Model
        {
            get
            {
                return model == null ? null : model;
            }
        }
       
        public void OnItemChangeDisplay(bool bSelected)
        {
            goCheckMark.CustomActive(bSelected);
        }

        public void OnItemVisible(BeadItemModel model)
        {
            this.model = model;
            string nomalHolePath = mBind.GetPrefabPath("nomalHole");
            mBind.ClearCacheBinds(nomalHolePath);
            ItemData mItemData = this.model.beadItemData;
            
            if (this.model.mountedType == (byte)UpgradePrecType.UnMounted)
            {
                
            }
            else
            {
                ComCommonBind mGameObjectBind = mBind.LoadExtraBind(nomalHolePath);
                if (mGameObjectBind != null)
                {
                    Utility.AttachTo(mGameObjectBind.gameObject, mBind.GetGameObject("BeadHoleRoot"));
                }

                ItemData mEquipItemData = this.model.equipItemData;
                if (mEquipItemData != null)
                {
                    GameObject mPos = mGameObjectBind.GetGameObject("ItemPos");
                    ComItemNew mComItem = ComItemManager.CreateNew(mPos);
                    mComItem.Setup(mEquipItemData, Utility.OnItemClicked);
                }
            }

            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(goItemParent);
            }

            comItem.Setup(mItemData, (GameObject obj, IItemDataModel item)=> 
            {
                if (item != null)
                {
                    mItemData.BeadAdditiveAttributeBuffID = model.buffID;
                    mItemData.BeadPickNumber = model.beadPickNumber;
                    mItemData.BeadReplaceNumber = model.replaceNumber;
                    ItemTipManager.GetInstance().ShowTip(item as ItemData);
                }
            });
            mName.text = mCheckName.text = mItemData.GetColorName();
            if (this.model.buffID > 0)
            {
                mBeadAttr.text = mCheckBeadAttr.text = BeadCardManager.GetInstance().GetAttributesDesc(mItemData.TableID) + "\n" +
                  string.Format("附加属性:{0}", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(this.model.buffID));
            }
            else
            {
                mBeadAttr.text = mCheckBeadAttr.text = BeadCardManager.GetInstance().GetAttributesDesc(mItemData.TableID);
            }

            ShowBeadReplaceRemainNumber();

            mScrollRect.verticalNormalizedPosition = 1;
            mCheckScrollRect.verticalNormalizedPosition = 1;
        }
        
        public void ShowBeadReplaceRemainNumber()
        {
            if (mIsShowBeadReplaceNumber)
            {
                string mReplaceNumberDes = BeadCardManager.GetInstance().GetBeadReplaceRemainNumber(this.model.beadItemData.TableID, this.model.replaceNumber);
                if (mReplaceNumberDes != "")
                {
                    mBeadAttr.text += "\n" + mReplaceNumberDes;
                }
            }
        }
    }
}
