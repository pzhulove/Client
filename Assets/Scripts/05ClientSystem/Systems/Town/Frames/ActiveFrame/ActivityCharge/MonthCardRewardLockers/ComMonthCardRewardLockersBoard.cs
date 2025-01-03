using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class ComMonthCardRewardLockersBoard : MonoBehaviour
    {
        #region MODEL PARAMS

        private List<MonthCardRewardLockersItem> mLockersItems;

        #endregion

        #region VIEW PARAMS

        [SerializeField]
        private ComUIListScript mComUIGrids = null;

        #endregion

        #region PRIVATE METHODS

        private void Awake()
        {
            _InitItemGrids();
        }

        private void OnDestroy()
        {
            _UnInitGrids();
        }

        //当界面打开时 获取焦点时 尝试刷新道具和倒计时！！！
        void OnApplicationFocus(bool bAppFocus)
        {
            if (bAppFocus)
            {
                MonthCardRewardLockersDataManager.GetInstance().ReqMonthCardRewardLockersItems();
            }
        }

        private void _InitItemGrids()
        {
            if (null == mComUIGrids)
            {
                return;
            }
            if (mComUIGrids.IsInitialised())
            {
                return;
            }
            mComUIGrids.Initialize();
            mComUIGrids.onBindItem += _OnComUIGridsItemBind;
            mComUIGrids.onItemVisiable += _OnComUIGridsItemVisiable;
            mComUIGrids.OnItemUpdate += _OnComUIGridsItemUpdate;
            mComUIGrids.OnItemRecycle += _OnComUIGridsItemRecycle;

            mComUIGrids.SetElementAmount(GetItemGridTotalCount());
        }

        private void _UnInitGrids()
        {
            if (null != mComUIGrids)
            {
                mComUIGrids.onBindItem -= _OnComUIGridsItemBind;
                mComUIGrids.onItemVisiable -= _OnComUIGridsItemVisiable;
                mComUIGrids.OnItemUpdate -= _OnComUIGridsItemUpdate;
                mComUIGrids.OnItemRecycle -= _OnComUIGridsItemRecycle;
            }
        }

        private ComItemGrid _OnComUIGridsItemBind(GameObject go)
        {
            if (null == go)
            {
                return null;
            }

            return go.GetComponent<ComItemGrid>();
        }

        private void _OnComUIGridsItemVisiable(ComUIListElementScript item)
        {
            _RefreshGridsItem(item);
        }

        private void _OnComUIGridsItemUpdate(ComUIListElementScript item)
        {
            _RefreshGridsItem(item);
        }

        private void _RefreshGridsItem(ComUIListElementScript item)
        {
            if (null == item || mLockersItems == null)
            {
                return;
            }
            int i_Index = item.m_index;
            var grid = item.gameObjectBindScript as ComItemGrid;
            if (i_Index >= 0 && i_Index < mLockersItems.Count)
            {
                var indexItem = mLockersItems[i_Index];
                if (indexItem == null || indexItem.itemdata == null)
                {
                    return;
                }
                if (grid != null)
                {
                    grid.SetGridItemData(indexItem.itemdata, true, indexItem.isHignestGradeItem);
                    grid.SetGridActive(true);
                }
            }
            else
            {
                if (grid != null)
                {
                    grid.SetGridActive(false);
                }
            }
        }

        private void _OnComUIGridsItemRecycle(ComUIListElementScript item)
        {
            if (null == item)
            {
                return;
            }
            var grid = item.GetComponent<ComItemGrid>();
            if (grid != null)
            {
                grid.SetGridActive(false);
            }
        }

        #region EVENT


        #endregion

        #endregion

        #region  PUBLIC METHODS

        public void RefreshItemGrids(List<MonthCardRewardLockersItem> _lockersItems)
        {
            this.mLockersItems = _lockersItems;
          
            if (null != mComUIGrids)
            {
                mComUIGrids.UpdateElement();
            }
        }

        public int GetItemGridTotalCount()
        {
            return MonthCardRewardLockersDataManager.GetInstance().MonthCardRewardLockersGridCount;
        }

        #region PUBLIC STATIC METHODS

        #endregion

        #endregion
    }
}
