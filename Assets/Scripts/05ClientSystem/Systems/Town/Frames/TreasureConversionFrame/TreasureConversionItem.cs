using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TreasureConversionItem : MonoBehaviour
    {
        [SerializeField] private Image mBackground;
        [SerializeField] private Image mIcon;
        [SerializeField] private Text mName;
        [SerializeField] private Text mAttr;
        [SerializeField] private Text mCount;
        [SerializeField] private Button mIconBtn;
        [SerializeField] private GameObject mCheckRoot;
        [SerializeField] private GameObject mNewMark;

        public ItemData TreasureItemData { get; set; }

        private void Awake()
        {
            if (mIconBtn != null)
            {
                mIconBtn.onClick.RemoveAllListeners();
                mIconBtn.onClick.AddListener(()=> { ItemTipManager.GetInstance().ShowTip(TreasureItemData); });
            }
        }

        public void OnItemVisiable(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            TreasureItemData = itemData;

            if (mBackground != null)
            {
                ETCImageLoader.LoadSprite(ref mBackground, itemData.GetQualityInfo().Background);
            }

            if (mIcon != null)
            {
                ETCImageLoader.LoadSprite(ref mIcon, itemData.Icon);
            }

            if (mCount != null)
            {
                if (itemData.ShowCount > 1)
                {
                    mCount.text = itemData.ShowCount.ToString();
                }
                else
                {
                    mCount.text = string.Empty;
                }
            }

            if (mName != null)
            {
                mName.text = itemData.GetColorName();
            }

            if (mAttr != null)
            {
                mAttr.text = BeadCardManager.GetInstance().GetAttributesDesc(itemData.TableID);
            }

            if (mNewMark != null)
            {
                if (itemData.IsNew)
                {
                    mNewMark.gameObject.CustomActive(true);
                    ItemDataManager.GetInstance().NotifyItemBeOld(itemData);

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.PackageMain);
                }
                else
                {
                    mNewMark.gameObject.CustomActive(false);
                }
            }
        }

        public void OnItemChangeDisplay(bool flag)
        {
            if (mCheckRoot != null)
            {
                mCheckRoot.CustomActive(flag);
            }
        }
    }
}