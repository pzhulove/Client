using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComTitleItem : MonoBehaviour
    {
        public GameObject goItemParent;
        public Text textMark;
        public GameObject goSelectMark;
        public GameObject goAnimationParent;
        public Text unAcquiredName;
        public SpriteAniRenderChenghao comAniRender;
        public Text timeLimit;
        public UIGray comGray;

        ComTitleItemData mData;
        ComItem comItem;
        ItemData itemData;

        public void OnItemChangeDisplay(bool bSelected)
        {
            goSelectMark.CustomActive(bSelected);
        }

        public void OnItemVisible(ComTitleItemData data)
        {
            mData = data;
            if (null == comItem)
            {
                comItem = ComItemManager.Create(goItemParent);
            }
            itemData = data.itemData;
            if (itemData == null)
            {
                itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mData.itemTable.ID);
            }
            comItem.Setup(itemData, null);
            if (gameObject != null && null != mData.itemTable)
            {
                gameObject.name = mData.itemTable.ID.ToString();
            }
            if (null != comAniRender)
            {
                comAniRender.SetEnable(false);
                if(null != mData.itemData)
                {
                    if(null != mData.itemTable && mData.itemTable.Path2.Count == 4)
                    {
                        int id = 0;
                        float fValue = 0.0f;
                        if(int.TryParse(mData.itemTable.Path2[2],out id) && float.TryParse(mData.itemTable.Path2[3],out fValue))
                        {
                            comAniRender.Reset(mData.itemTable.Path2[0], mData.itemTable.Path2[1], id,fValue, mData.itemTable.ModelPath);
                            comAniRender.SetEnable(true);
                        }
                    }
                }
            }

            if(null == mData.itemData)
            {
                if (null != unAcquiredName)
                {
                    if (null != mData.itemTable)
                    {
                        unAcquiredName.text = mData.itemTable.Name;
                    }
                }
            }

            unAcquiredName.CustomActive(null == mData.itemData);
            if(null != comGray)
            {
                comGray.enabled = null == mData.itemData;
            }

            bool bCanTrade = CanTrade;
            bool bEquiped = Equiped;
            bool bHasExtra = HasExtra;
            textMark.CustomActive(bEquiped || bHasExtra);
            if (null != mData.itemData && mData.itemData.PackageType == EPackageType.WearEquip)
            {
                textMark.text = TR.Value("title_has_equiped");
            }
            else if (bHasExtra)
            {
                textMark.text = TR.Value("title_has_owned");
            }
            timeLimit.CustomActive(mData.eType == TittleComeType.TCT_TIMELIMITED);

            InvokeMethod.RmoveInvokeIntervalCall(this);
            if (mData.eType == TittleComeType.TCT_TIMELIMITED)
            {
                _UpdateTimeLimitDesc();
                InvokeMethod.InvokeInterval(this, 0.33f, 0.33f, 99999999.0f, null, _UpdateTimeLimitDesc, null);
            }
        }

        void _UpdateTimeLimitDesc()
        {
            if (null != mData.itemData && null != timeLimit)
            {
                timeLimit.text = mData.itemData.GetTimeLeftDescByDay();
            }
        }

        public bool CanTrade
        {
            get
            {
                return null != mData.itemData && TittleBookManager.GetInstance().CanTrade(mData.itemData);
            }
        }

        public bool Equiped
        {
            get
            {
                return null != mData.itemData && mData.itemData.PackageType == EPackageType.WearEquip;
            }
        }

        public bool HasExtra
        {
            get
            {
                return null != mData.itemData && TittleBookManager.GetInstance().HasExtraTitle(mData.itemData);
            }
        }

        void OnDestroy()
        {
            InvokeMethod.RmoveInvokeIntervalCall(this);

            if (null != comItem)
            {
                ComItemManager.Destroy(comItem);
                comItem = null;
            }
            mData.itemData = null;
            mData.itemTable = null;
        }
    }
}