using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComConsumeItemModel
    {
        public ComCommonConsume comConsume;
        public int index;
    }

    /// <summary>
    /// Toggle Group
    /// </summary>
    public class ComConsumeItemGroup : MonoBehaviour
    {
        [SerializeField]
        private List<ComCommonConsume> mComConsumeGroup;
        [SerializeField]
        private bool bGroupEnable = true;

        // private bool[] mConsumeItemActivatedStatuses = null;
        private List<ComConsumeItemModel> mConsumeItemModels = new List<ComConsumeItemModel>();
        private List<int> mOriginalConsumeIds = new List<int>();

        #region Model Params
        
        #endregion
        
        #region View Params    
        
        #endregion
        
        #region PRIVATE METHODS
       
        private void Awake() 
        {
            if(mComConsumeGroup != null)
            {
                //mConsumeItemActivatedStatuses = new bool[mComConsumeGroup.Count];
                for (int i = 0; i < mComConsumeGroup.Count; i++)
                {
                    var comConsume = mComConsumeGroup[i];
                    if(comConsume == null)
                        continue;
                    if(!mOriginalConsumeIds.Contains(comConsume.mItemID))
                    {
                        mOriginalConsumeIds.Add(comConsume.mItemID);
                    }

                    mConsumeItemModels.Add(new ComConsumeItemModel(){comConsume = comConsume, index = i});
                }
            }   
        }

        private void OnDestroy()
        {
            //mConsumeItemActivatedStatuses = null;   
            if(mOriginalConsumeIds != null)
            {
                mOriginalConsumeIds.Clear();
            }

            if(mConsumeItemModels != null)
            {
                mConsumeItemModels.Clear();
            }
        }
        
        private void _SetIndexConsumeItemActivated(int index, bool bActivated)
        {
            // if(mConsumeItemActivatedStatuses != null && index < mConsumeItemActivatedStatuses.Length && index >= 0)
            // {
            //     mConsumeItemActivatedStatuses[index] = bActivated;
            // }
            if(mConsumeItemModels != null && mConsumeItemModels.Count > 0)
            {
                for (int i = 0; i < mConsumeItemModels.Count; i++)
                {
                    var data = mConsumeItemModels[i];
                    if(data != null && data.index == index)
                    {
                        data.comConsume.CustomActive(bActivated);
                    }
                }
            }
        }

        private void _SetSelectedConsumeActive(ComConsumeItemModel model, bool bActivated)
        {
            if(mConsumeItemModels != null && mConsumeItemModels.Count > 0)
            {
                for (int i = 0; i < mConsumeItemModels.Count; i++)
                {
                    var data = mConsumeItemModels[i];
                    if(data != null && data == model)
                    {
                        data.comConsume.CustomActive(bActivated);
                    }
                }
            }
        }

        private bool _GetIndexConsumeItemActivated(int index)
        {
            // if(mConsumeItemActivatedStatuses != null && index < mConsumeItemActivatedStatuses.Length && index >= 0)
            // {
            //     return mConsumeItemActivatedStatuses[index];
            // }
            if(mConsumeItemModels != null && mConsumeItemModels.Count > 0)
            {
                for (int i = 0; i < mConsumeItemModels.Count; i++)
                {
                    var data = mConsumeItemModels[i];
                    if(data != null && data.index == index)
                    {
                        return data.comConsume.gameObject.activeSelf;
                    }
                }
            }
            return false;
        }

        private bool _GetSelectedConsumeItemActivated(ComConsumeItemModel model)
        {
            if(model != null && model.comConsume != null)
            {
                return model.comConsume.gameObject.activeSelf;
            }
            return false;
        }

        private ComConsumeItemModel _CheckConsumeItemGroupHasSameItemId(int itemId)
        {
            if(mConsumeItemModels == null)
            {
                return null;
            }
            for (int i = 0; i < mConsumeItemModels.Count; i++)
            {
                var data = mConsumeItemModels[i];
                if(data == null || data.comConsume == null)
                {
                    continue;
                }
                if(data.comConsume.mItemID == itemId)
                {
                    return data;
                }
            }
            return null;
        }

        #endregion
        
        #region  PUBLIC METHODS

        /// <summary>
        /// 获取最初的消耗品ids
        /// </summary>
        /// <returns></returns>
        public List<int> GetOriginalConsumeIds()
        {
            return mOriginalConsumeIds;
        }

        public List<ComCommonConsume> GetConsumeGroup()
        {
            return mComConsumeGroup;
        }

        public bool GetGroupEnable()
        {
            return bGroupEnable;
        }

        public void SetItemActiveByItemId(int itemId, bool active)
        {
            if (!bGroupEnable)
            {
                return;
            }
            if (mConsumeItemModels == null)
            {
                return;
            }
            if(itemId <= 0)
            {
                return;
            }
            for (int i = 0; i < mConsumeItemModels.Count; i++)
            {
                var data = mConsumeItemModels[i];
                if (data == null || data.comConsume == null)
                {
                    continue;
                }
                if (data.comConsume.mItemID == itemId)
                {
                    _SetSelectedConsumeActive(data, active);
                    break;
                }
            }
        }

        public void SetAllItemActive(bool active)
        {
            if (!bGroupEnable)
            {
                return;
            }
            if (mConsumeItemModels == null)
            {
                return;
            }
            for (int i = 0; i < mConsumeItemModels.Count; i++)
            {
                var data = mConsumeItemModels[i];
                _SetSelectedConsumeActive(data, active);
            }
        }

        /// <summary>
        /// 重置新的消耗品ids 可能会替换最初的消耗品ids
        /// </summary>
        /// <param name="itemIds"></param>
        /// <param name="needitemShow"></param>
        /// <param name="iType"></param>
        /// <param name="iCountType"></param>
        public void _ResetSelectedItemIds(int[] itemIds, bool needitemShow, 
                    ComCommonConsume.eType iType = ComCommonConsume.eType.Item, 
                    ComCommonConsume.eCountType iCountType = ComCommonConsume.eCountType.Fatigue)
        {
            if (!bGroupEnable)
            {
                return;
            }
            if(itemIds == null || itemIds.Length == 0)
            {
                return;
            }
            if (mConsumeItemModels == null)
            {
                return;
            }

            if(itemIds.Length > mConsumeItemModels.Count)
            {
                Logger.LogError("[ComConsumeItemGroup] - ResetSelectedItemIds, out params consume itemIds length > mComConsume Items count!!!");
                return;
            }

            for (int i = 0; i < itemIds.Length; i++)
            {
                var data = _CheckConsumeItemGroupHasSameItemId(itemIds[i]);
                if(data != null)
                {
                    data.index = i;
                    _SetSelectedConsumeActive(data, needitemShow);       
                    continue;
                }
                
                bool bSet = false;
                for (int j = 0; j < mConsumeItemModels.Count; j++)
                {
                    var tempData = mConsumeItemModels[j];
                    if(tempData == null || tempData.comConsume == null)
                    {
                        continue;
                    }
                    //判断是否被激活状态
                    //if(_GetIndexConsumeItemActivated(j))
                    if(_GetSelectedConsumeItemActivated(tempData))
                    {
                        continue;
                    }
                    //判断已经激活的ID是否已经存在了 存在就不设了
                    if(bSet)
                    {
                        continue;
                    }
                
                    tempData.comConsume.SetData(iType, iCountType, itemIds[i]);
                    tempData.index = i;                    
                    bSet = true;
                    
                    _SetSelectedConsumeActive(tempData, needitemShow);
                }
            }
			
            for (int i = 0; i < mConsumeItemModels.Count; i++)
            {
                var data = mConsumeItemModels[i];
                if(data == null || data.comConsume == null)
                {
                    continue;
                }
                data.comConsume.transform.SetSiblingIndex(data.index);
            }
        }

        /// <summary>
        /// 重置选择的道具ids
        /// </summary>
        /// <param name="itemIds">指定道具ids</param>
        /// <param name="needAcc">是否累加，不删除已有的</param>
        /// <param name="needShow">是否显示</param>
        public void ResetSelectedItemIds(int[] itemIds, bool needAcc=false, bool needShow = false, 
                    ComCommonConsume.eType iType = ComCommonConsume.eType.Item, 
                    ComCommonConsume.eCountType iCountType = ComCommonConsume.eCountType.Fatigue)
        {
            if (!bGroupEnable)
            {
                return;
            }
            if(itemIds == null || itemIds.Length <= 0)
            {
                return;
            }
            if(!needAcc)
            {
                SetAllItemActive(false);
            }
            _ResetSelectedItemIds(itemIds, needShow, iType, iCountType);
        }

        /// <summary>
        /// 重置 预制体最初的ids 请核对预制体配置的ids
        /// </summary>
        public void ResetOriginalItemIdsWithShow(bool needShow = true, 
                    ComCommonConsume.eType iType = ComCommonConsume.eType.Item, 
                    ComCommonConsume.eCountType iCountType = ComCommonConsume.eCountType.Fatigue)
        {
            if (!bGroupEnable)
            {
                return;
            }
            if(mOriginalConsumeIds != null && mOriginalConsumeIds.Count > 0)
            {
                SetAllItemActive(false);
                _ResetSelectedItemIds(mOriginalConsumeIds.ToArray(), needShow, iType, iCountType);
            }            
        }
        
        #endregion
    }
}