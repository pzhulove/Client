using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class StrengthenTicketMixNeedItem : MonoBehaviour
    {
        [SerializeField] private ComItemNew mItem;
        [SerializeField] private GameObject mObjNothing;
        [SerializeField] private Text mTextDesp;
        [SerializeField] private Button mBtnCancel;
        private int mItemId = 0;
        private Action<int> mOnClickEvent;
        public void OnInit(int itemId, Action<int> onClickEvent)
        {
            mItemId = itemId;
            mOnClickEvent = onClickEvent;
            if (0 == mItemId)
            {
                _OnShowEmpty();
                return;
            }
            mItem.CustomActive(true);
            mBtnCancel.CustomActive(true);
            mObjNothing.CustomActive(false);
            var itemData = ItemDataManager.CreateItemDataFromTable(mItemId);
            if (null != itemData)
            {
                mItem.Setup(itemData);
                mTextDesp.SafeSetText(itemData.Name, itemData.TableData.Color);
            }
        }

        private void _OnShowEmpty()
        {
            mItem.CustomActive(false);
            mBtnCancel.CustomActive(false);
            mObjNothing.CustomActive(true);
            mTextDesp.SafeSetText(TR.Value("strengthen_merge_plase_select_ticket"));
        }

        public void onClickEvent()
        {
            if (null != mOnClickEvent)
                mOnClickEvent(mItemId);
        }
    }
}
