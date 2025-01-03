using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnEnchantmentCardViceCardItemClick(EnchantmentCardViceCardItem viceCardItem);
    public class EnchantmentCardViceCardItem : MonoBehaviour
    {
        [SerializeField]private Text mViceCardName;
        [SerializeField]private Text mViceCardArrt;
        [SerializeField]private Text mViceCardCount;
        [SerializeField]private Text mViceCardSuccessRate;
        [SerializeField]private GameObject mViceCardItemRoot;
        [SerializeField]private GameObject mCheckMarkGo;
        [SerializeField]private Button mItemBtn;
        [SerializeField]private string sSuccessRateDesc = "成功率：{0}";

        private ComItemNew mViceCardComItem;
        private EnchantmentCardViceCardData viceCardData;
        public EnchantmentCardViceCardData ViceCardData
        {
            get { return viceCardData; }
        }
        private OnEnchantmentCardViceCardItemClick mOnEnchantmentCardViceCardItemClick;

        private void Awake()
        {
            if(mItemBtn != null)
            {
                mItemBtn.onClick.RemoveAllListeners();
                mItemBtn.onClick.AddListener(OnItemBtnClick);
            }
        }

        public void OnItemVisiable(EnchantmentCardViceCardData viceCardData, OnEnchantmentCardViceCardItemClick callBack, List<EnchantmentCardViceCardData> putCardList)
        {
            if (viceCardData == null || 
                viceCardData.mViceCardItemData == null ||
                viceCardData.mViceCardItemData.mPrecEnchantmentCard == null)
            {
                return;
            }

            this.viceCardData = viceCardData;
            mOnEnchantmentCardViceCardItemClick = callBack;

            if (mViceCardComItem == null)
            {
                mViceCardComItem = ComItemManager.CreateNew(mViceCardItemRoot);
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(viceCardData.mViceCardItemData.TableID);

            if (mViceCardComItem != null)
            {
                mViceCardComItem.Setup(itemData, (GameObject, IItemDataModel) => 
                {
                    ItemTipManager.GetInstance().ShowTip(viceCardData.mViceCardItemData);
                });
            }
            
            if (mViceCardName != null)
            {
                mViceCardName.text = viceCardData.mViceCardItemData.GetColorName();
            }

            if (mViceCardArrt != null)
            {
                mViceCardArrt.text = EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc(viceCardData.mViceCardItemData.mPrecEnchantmentCard.iEnchantmentCardID, viceCardData.mViceCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
            }

            if (mViceCardCount != null)
            {
                if (viceCardData.mViceCardCount > 1)
                {
                    mViceCardCount.text = viceCardData.mViceCardCount.ToString();
                }
                else
                {
                    mViceCardCount.text = string.Empty;
                }
            }
            
            if (mViceCardSuccessRate != null)
            {
                mViceCardSuccessRate.text =string.Format(sSuccessRateDesc, EnchantmentsCardManager.GetInstance().GetEnchantmentCardProbabilityDesc(viceCardData.mAllSuccessRate));
            }

            UpdateCheckMark(putCardList);
        }

        private void UpdateCheckMark(List<EnchantmentCardViceCardData> putCardList)
        {
            if (putCardList != null && putCardList.Count > 0)
            {
                bool isFind = false;
                for (int i = 0; i < putCardList.Count; i++)
                {
                    var item = putCardList[i];
                    if (item == null)
                    {
                        continue;
                    }

                    if (item.mViceCardItemData == null)
                        continue;

                    if (item.mViceCardItemData.TableID != viceCardData.mViceCardItemData.TableID)
                    {
                        continue;
                    }

                    if (item.mViceCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel != viceCardData.mViceCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel)
                        continue;

                    isFind = true;
                    break;
                }

                if (isFind)
                {
                    OnItemChangeDisplay(true);
                }
                else
                {
                    OnItemChangeDisplay(false);
                }
            }
            else
            {
                OnItemChangeDisplay(false);
            }
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            if (mCheckMarkGo != null)
            {
                mCheckMarkGo.CustomActive(bSelected);
            }
        }

        private void OnItemBtnClick()
        {
            if(mOnEnchantmentCardViceCardItemClick != null)
            {
                mOnEnchantmentCardViceCardItemClick.Invoke(this);
            }
        }

        public void OnDestroy()
        {
            if (mItemBtn != null)
            {
                mItemBtn.onClick.RemoveListener(OnItemBtnClick);
            }

            mViceCardComItem = null;
        }
    }
}
