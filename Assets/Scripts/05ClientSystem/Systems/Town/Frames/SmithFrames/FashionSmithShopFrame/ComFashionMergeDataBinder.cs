using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Scripts.UI;
using DG.Tweening;
using UnityEngine.Events;
using Network;
using ProtoTable;
using System;

namespace GameClient
{
    [Serializable]
    public class ChangeFashionActivitySlotExpendBindGoldModel
    {
        public ItemTable.eSubType mSubType;
        public int mBindGoldCoinsNum;
    }
    public class ComFashionMergeDataBinder : MonoBehaviour
    {
        public GameObject[] goItemParents = new GameObject[0];
        public StateController mWindStatus;
        public StateController mMergeState;
        public StateController[] mSkyStatus = new StateController[0];
        public StateController[] mGoldSkyStatus = new StateController[0];
        public StateController[] mTypeRelationSkyStatus = new StateController[0];
        public GameObject[] mChecks = new GameObject[0];
        byte[] goldSkySlotValues = new byte[6];
        byte[] skySlotValues = new byte[6];
        byte[] nationalDaySlotValues = new byte[5];//国庆套槽
        public byte[] fashionAvatarDefaultTypes = new byte[6];
        int windIndex = 5;//must less than the length of both goldSkySlotValues and skySlotValues
        string mStrEnable = "Enable";
        string mStrDisable = "Disable";

        public StateController mLogicPartSelectNode;
        string mPartFashionWind = "wind";
        string mPartFashionHead = "head";
        string mPartFashionSash = "sash";
        string mPartFashionChest = "chest";
        string mPartFashionLeg = "leg";
        string mPartFashionEpaulet = "epaulet";

        public StateController mEffectState = null;
        public ComEffectLoader mEffectLoader = null;
        public StateController mEffectFlyState = null;
        public GameObject mFlyFixedItem = null;
        public float mFlyLength = 1.20f;
        public UnityEvent mOnFlyStart = null;
        public float mFadeLength = 0.80f;
        public float mFlightLength = 0.80f;
        public UnityEvent mOnFlightStart = null;
        public int mGridKeep = 3;

        public Text windProcess;
        public string windProcessString = "{0}/{1}";

        public UnityEvent mOnRedPointOn;
        public UnityEvent mOnRedPointOff;
        public Text mRedCount;

        private bool isLeftPos = true;
        public void _OnRedPointChanged(UIEvent uiEvent)
        {
            int cnt = FashionMergeManager.GetInstance().redCount;
            if(null != mRedCount)
            {
                mRedCount.text = cnt.ToString();
            }
            var action = cnt > 0 ? mOnRedPointOn : mOnRedPointOff;
            if(null != action)
            {
                action.Invoke();
            }
        }

        public string mPopContent = string.Empty;
        public void OnNotify()
        {
            SystemNotifyManager.SysNotifyTextAnimation(mPopContent);
        }

        public void UpdateWindProcess()
        {
            if(null != windProcess)
            {
                int count = 0;
                if(FashionMergeManager.GetInstance().FashionType == FashionType.FT_SKY)
                {
                    for(int i = 0; i < skySlotValues.Length && i < 5; ++i)
                    {
                        if(0 != skySlotValues[i])
                        {
                            ++count;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < goldSkySlotValues.Length && i < 5; ++i)
                    {
                        if (0 != goldSkySlotValues[i])
                        {
                            ++count;
                        }
                    }
                }
                count = (int)IntMath.Clamp(count,0, 5);
                windProcess.text = string.Format(windProcessString, count, 5);
                windProcess.CustomActive(count < 5);
            }
        }

        public void FlyEffect2Slot(int iIndex, Vector3 start, ProtoTable.ItemTable.eSubType sub, float len = 5.0f, UnityEngine.Events.UnityAction onStart = null, UnityEngine.Events.UnityAction onEnd = null)
        {
            int slot = _GetSlotIDByType(sub);
            if (null != goItemParents && slot >= 0 && slot < goItemParents.Length)
            {
                FlyEffect(iIndex,start, goItemParents[slot].transform.position,len, onStart,onEnd);
            }
        }

        public void FlyEffect(int iIndex,Vector3 start,Vector3 end,float len = 5.0f, UnityEngine.Events.UnityAction onStart = null,UnityEngine.Events.UnityAction onEnd = null)
        {
            if(null != mEffectLoader)
            {
                InvokeMethod.RmoveInvokeIntervalCall(mEffectLoader);
                float time = Time.time;
                InvokeMethod.InvokeInterval(mEffectLoader, 0.0f, 0.03f, len, () =>
                    {
                        if (null != onStart)
                        {
                            onStart.Invoke();
                        }
                        mEffectLoader.LoadEffect(iIndex);
                        mEffectLoader.SetEffectPosition(iIndex, start);
                        mEffectLoader.ActiveEffect(iIndex);
                        if (null != mEffectFlyState)
                        {
                            mEffectFlyState.Key = mStrEnable;
                        }
                    },
                () =>
                {
                    if (null != mEffectLoader)
                    {
                        float endT = Mathf.Clamp01((Time.time - time) / len);
                        Vector3 target = Vector3.zero;
                        target.x = Mathf.Lerp(start.x, end.x, endT);
                        target.y = Mathf.Lerp(start.y, end.y, endT);
                        target.z = Mathf.Lerp(start.z, end.z, endT);
                        mEffectLoader.SetEffectPosition(iIndex, target);
                    }
                },
                () =>
                {
                    if (null != mEffectLoader)
                    {
                        mEffectLoader.SetEffectPosition(iIndex, end);
                        mEffectLoader.DeActiveEffect(iIndex);
                    }

                    if(null != onEnd)
                    {
                        onEnd.Invoke();
                    }
                });
            }
        }

        public void DisableFlyState()
        {
            if (null != mEffectFlyState)
            {
                mEffectFlyState.Key = mStrDisable;
            }
        }

        public Vector3 GetFlyFixedPosition()
        {
            if(null != mFlyFixedItem)
            {
                Vector3 position = mFlyFixedItem.transform.position;
                position.z = 0.0f;
                return position;
            }
            return Vector3.zero;
        }

        public string mSpecial0 = "Effects/UI/Prefab/EffUI_shizhuanghecheng/Prefab/EffUI_shizhuanghecheng_qianruzi";
        GameObject mGoSpecial0 = null;
        public string mSpecial1 = "Effects/UI/Prefab/EffUI_shizhuanghecheng/Prefab/EffUI_shizhuanghecheng_qianrufen";
        GameObject mGoSpecial1 = null;

        ComItem[] mComItems = new ComItem[6];

        public GameObject mGoPreviewParent = null;
        ComItem mComItemPreview = null;

        public GameObject mGoNormalPreviewParent = null;
        ComItem mComItemNormalPreview = null;

        public GameObject mGoActivityFashionBindGoldParent = null;
        ComItem mComItemActivityFashionBindGlod = null;
        public StateController mActivityFashionBindGlodCountStatus;
        public Text mActivityFashionBindGlodCount;
        public List<ChangeFashionActivitySlotExpendBindGoldModel> mBindGoldCoinList = new List<ChangeFashionActivitySlotExpendBindGoldModel>();//活动时装合每个部位合成消耗绑定金币的集合

        public GameObject mGoBoxParent = null;
        ComItem mComBoxItem = null;
        public StateController mBoxCountStatus;
        public Text mBoxCount;
        public Text mBoxItemName;

        public int[] ActivityFashionMergeBindGolds = new int[0];
        int _ActivityFashionMergeBindGoldIndex = 0;
        public int ActivityFashionMergeBindGoldIndex
        {
            get
            {
                return _ActivityFashionMergeBindGoldIndex;
            }
            set
            {
                _ActivityFashionMergeBindGoldIndex = value;
                if (mComItemActivityFashionBindGlod == null)
                {
                    mComItemActivityFashionBindGlod = ComItemManager.Create(mGoActivityFashionBindGoldParent);
                }

                _UpdateBindGoldItem();
            }
        }

        public int[] FashionMergeBoxIds = new int[0];
        int _FashionMergeItemIndex = 0;
        public int FashionMergeItemIndex
        {
            get
            {
                return _FashionMergeItemIndex;
            }

            set
            {
                _FashionMergeItemIndex = value;
                if (null == mComBoxItem)
                {
                    mComBoxItem = ComItemManager.Create(mGoBoxParent);
                }
                _UpdateMergeBoxItem();
            }
        }

        public StateController mLSlotStatus;
        public StateController mRSlotStatus;
        ComItem leftSlot;
        ComItem rightSlot;
        public GameObject goLeftParent;
        public GameObject goRightParent;
        static ItemData lValue = null;
        static ItemData rValue = null;
        public static ItemData SLValue
        {
            get
            {
                return lValue;
            }
            set
            {
                lValue = value;
            }
        }
        public static ItemData SRValue
        {
            get
            {
                return rValue;
            }
            set
            {
                rValue = value;
            }
        }

        public ItemData LValue
        {
            get
            {
                return lValue;
            }
            set
            {
                lValue = value;
                _UpdateSlotStatus();
            }
        }
        public ItemData RValue
        {
            get
            {
                return rValue;
            }
            set
            {
                rValue = value;
                _UpdateSlotStatus();
            }
        }

        bool mIntialized = false;
        List<ItemData> kEquipments = null;
        bool mLeft = false;

        public ComModelBinder mModelBinder = null;
        public Text mSkySuitName;

        public void SetBindItem(ulong guid)
        {

            try
            {
                if (RValue != null && RValue.GUID == guid)
                    return;

                if (LValue != null && LValue.GUID == guid)
                    return;

                LValue = ItemDataManager.GetInstance().GetItem(guid);

            }
            catch (Exception e)
            {
                //经常出现NULL异常
                Logger.LogErrorFormat("RValue is {0}, LValue is {1},this pointer is {2}, Exception is {3}, ", 
                    RValue,
                    LValue,
                    this,
                    e.ToString());
            }

        }

        public void PlayNormalSlotEffect(ProtoTable.ItemTable.eSubType sub)
        {
            int slot = _GetSlotIDByType(sub);

            if (slot < 0 || slot >= goItemParents.Length)
            {
                return;
            }

            if(null == mGoSpecial0)
            {
                mGoSpecial0 = AssetLoader.instance.LoadRes(mSpecial0, typeof(GameObject)).obj as GameObject;
            }

            if(null != mGoSpecial0)
            {
                Utility.AttachTo(mGoSpecial0, goItemParents[slot]);
            }

            if (null != mGoSpecial0)
            {
                mGoSpecial0.SetActive(false);
                mGoSpecial0.SetActive(true);
            }
        }

        public void PlaySpecialSlotEffect(ProtoTable.ItemTable.eSubType sub)
        {
            int slot = _GetSlotIDByType(sub);

            if (slot < 0 || slot >= goItemParents.Length)
            {
                return;
            }

            if (null == mGoSpecial1)
            {
                mGoSpecial1 = AssetLoader.instance.LoadRes(mSpecial1, typeof(GameObject)).obj as GameObject;
            }

            if (null != mGoSpecial1)
            {
                Utility.AttachTo(mGoSpecial1, goItemParents[slot]);
            }

            if (null != mGoSpecial1)
            {
                mGoSpecial1.SetActive(false);
                mGoSpecial1.SetActive(true);
            }
        }

        public void ClearSlotValues()
        {
            lValue = rValue = null;
            _UpdateSlotStatus();
        }

        public void PopItemComeLink()
        {
            if (_FashionMergeItemIndex >= 0 && _FashionMergeItemIndex < FashionMergeBoxIds.Length)
            {
                var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(FashionMergeBoxIds[_FashionMergeItemIndex]);
           
                if (_FashionMergeItemIndex == 0)
                {
                    _onFastBuy(item.ID);
                }
                else
                {
                    ItemComeLink.OnLink(item.ID, 0);
                }
         

            }
        }

        public void SetNextMergeBox()
        {
            if(FashionMergeBoxIds.Length > 0)
            {
                int iTargetValue = (_FashionMergeItemIndex + 1) % FashionMergeBoxIds.Length;
                FashionMergeItemIndex = iTargetValue;
            }
        }

        void _UpdateMergeBoxItem()
        {
            if (null != mComBoxItem)
            {
                int iOwnedCount = 0;
                int iNeedCount = 1;
                if (_FashionMergeItemIndex < 0 || _FashionMergeItemIndex >= FashionMergeBoxIds.Length)
                {
                    mComBoxItem.Setup(null, null);
                    iOwnedCount = 0;
                    if (null != mBoxItemName)
                    {
                        mBoxItemName.text = string.Empty;
                    }
                }
                else
                {
                    var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(FashionMergeBoxIds[_FashionMergeItemIndex]);
                    mComBoxItem.Setup(itemData, _OnMergeBoxClicked);
                    iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(FashionMergeBoxIds[_FashionMergeItemIndex]);
                    if (null != mBoxItemName)
                    {
                        if (null != itemData)
                        {
                            mBoxItemName.text = itemData.GetColorName();
                        }
                        else
                        {
                            mBoxItemName.text = string.Empty;
                        }
                    }
                }

                if (null != mComBoxItem)
                {
                    mComBoxItem.SetEnable(iNeedCount <= iOwnedCount);
                }

                if (null != mBoxCount)
                {
                    mBoxCount.text = iOwnedCount + "/" + iNeedCount;
                }

                if (null != mBoxCountStatus)
                {
                    mBoxCountStatus.Key = iNeedCount <= iOwnedCount ? mStrEnable : mStrDisable;
                }
            }
        }

        int GetSlotExpendBindGoldCount()
        {
            for (int i = 0; i < mBindGoldCoinList.Count; i++)
            {
                var item = mBindGoldCoinList[i];
                if (item.mSubType != FashionMergeManager.GetInstance().FashionPart)
                {
                    continue;
                }

                return item.mBindGoldCoinsNum;
            }

            return 0;
        }

        void _UpdateBindGoldItem()
        {
            if (null != mComItemActivityFashionBindGlod)
            {
                int iOwnedCount = 0;
                int iNeedCount = GetSlotExpendBindGoldCount();
                if (_ActivityFashionMergeBindGoldIndex < 0 || _ActivityFashionMergeBindGoldIndex >= ActivityFashionMergeBindGolds.Length)
                {
                    mComItemActivityFashionBindGlod.Setup(null, null);
                    iOwnedCount = 0;
                }
                else
                {
                    var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(ActivityFashionMergeBindGolds[_ActivityFashionMergeBindGoldIndex]);
                    mComItemActivityFashionBindGlod.Setup(itemData, _OnMergeBoxClicked);
                    iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(ActivityFashionMergeBindGolds[_ActivityFashionMergeBindGoldIndex],false);
                    
                }

                if (null != mComItemActivityFashionBindGlod)
                {
                    mComItemActivityFashionBindGlod.SetEnable(iNeedCount <= iOwnedCount);
                }

                if (null != mActivityFashionBindGlodCount)
                {
                    mActivityFashionBindGlodCount.text = iNeedCount.ToString();
                }

                if (null != mActivityFashionBindGlodCountStatus)
                {
                    mActivityFashionBindGlodCountStatus.Key = iNeedCount <= iOwnedCount ? mStrEnable : mStrDisable;
                }
            }
        }

        void _OnMergeBoxClicked(GameObject obj, ItemData item)
        {
            if(null != item)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        void _OnSlotItemClicked(GameObject obj, ItemData item)
        {
            if(null != obj)
            {
                bool bFindLeft = false;
                while(!bFindLeft && null != obj && null != obj.transform.parent)
                {
                    bFindLeft = obj == goLeftParent;
                    obj = obj.transform.parent.gameObject;
                }
                AddMergeFashion(bFindLeft);
            }
        }

        int _GetSlotIDByType(ProtoTable.ItemTable.eSubType eSubType)
        {
            int iSlotID = -1;
            switch (eSubType)
            {
                case ProtoTable.ItemTable.eSubType.FASHION_HAIR:
                    {
                        iSlotID = 5;
                    }
                    break;
                case ProtoTable.ItemTable.eSubType.FASHION_HEAD:
                    {
                        iSlotID = 0;
                    }
                    break;
                case ProtoTable.ItemTable.eSubType.FASHION_SASH:
                    {
                        iSlotID = 4;
                    }
                    break;
                case ProtoTable.ItemTable.eSubType.FASHION_CHEST:
                    {
                        iSlotID = 1;
                    }
                    break;
                case ProtoTable.ItemTable.eSubType.FASHION_LEG:
                    {
                        iSlotID = 3;
                    }
                    break;
                case ProtoTable.ItemTable.eSubType.FASHION_EPAULET:
                    {
                        iSlotID = 2;
                    }
                    break;
            }
            return iSlotID;
        }

        public void SetSlotItems(ItemData itemData, ComItem.OnItemClicked callback,ProtoTable.ItemTable.eSubType eSubType)
        {
            int iSlotID = _GetSlotIDByType(eSubType);

            if (iSlotID < 0 || iSlotID >= goItemParents.Length)
            {
                Logger.LogErrorFormat("slot id is invalid index is {0} but len = {1}",iSlotID, goItemParents.Length);
                return;
            }

            if(null == mComItems[iSlotID])
            {
                mComItems[iSlotID] = ComItemManager.Create(goItemParents[iSlotID]);
            }

            if(null != mComItems[iSlotID])
            {
                mComItems[iSlotID].Setup(itemData,callback);
            }
        }

        FashionType mFashionType = FashionType.FT_SKY;
        public void SetSkyMode(FashionType eFashionType)
        {
            mFashionType = eFashionType;
            if (null != mModelBinder)
            {
                mModelBinder.LoadAvatar(PlayerBaseData.GetInstance().JobTableID);
                mModelBinder.LoadWeapon();

                List<int> tableIds = GamePool.ListPool<int>.Get();
                FashionMergeManager.GetInstance().GetFashionItemsByTypeAndOccu(eFashionType, PlayerBaseData.GetInstance().JobTableID, FashionMergeManager.GetInstance().SkySuitID, ref tableIds);
                List<ItemData> datas = new List<ItemData>(tableIds.Count);
                for (int i = 0; i < tableIds.Count; ++i)
                {
                    ItemData data = ItemDataManager.GetInstance().GetCommonItemTableDataByID(tableIds[i]);
                    if (null != data)
                    {
                        datas.Add(data);
                    }
                }
                GamePool.ListPool<int>.Release(tableIds);

                mModelBinder.SetFashions(datas);
            }
        }

        ProtoTable.ItemTable.eSubType mFashionPart = ProtoTable.ItemTable.eSubType.FASHION_HEAD;
        public void SetSkyFashionPart(ProtoTable.ItemTable.eSubType eSubType)
        {
            if(mFashionPart != eSubType)
            {
                ClearSlotValues();
            }

            mFashionPart = eSubType;
            if (null != mLogicPartSelectNode)
            {
                switch (eSubType)
                {
                    case ProtoTable.ItemTable.eSubType.FASHION_HAIR:
                        {
                            mLogicPartSelectNode.Key = mPartFashionWind;
                        }
                        break;
                    case ProtoTable.ItemTable.eSubType.FASHION_HEAD:
                        {
                            mLogicPartSelectNode.Key = mPartFashionHead;
                        }
                        break;
                    case ProtoTable.ItemTable.eSubType.FASHION_SASH:
                        {
                            mLogicPartSelectNode.Key = mPartFashionSash;
                        }
                        break;
                    case ProtoTable.ItemTable.eSubType.FASHION_CHEST:
                        {
                            mLogicPartSelectNode.Key = mPartFashionChest;
                        }
                        break;
                    case ProtoTable.ItemTable.eSubType.FASHION_LEG:
                        {
                            mLogicPartSelectNode.Key = mPartFashionLeg;
                        }
                        break;
                    case ProtoTable.ItemTable.eSubType.FASHION_EPAULET:
                        {
                            mLogicPartSelectNode.Key = mPartFashionEpaulet;
                        }
                        break;
                }
            }
            _SyncSkyFashionPartItem(mFashionPart);
            _SyncNormalFashionPartItem(mFashionPart);
            if(CountDataManager.GetInstance().GetCount(CounterKeys.FASHION_MERGE_AUTO_EQUIP_STATE) == 1)
            {
                TryAutoEquipFashion();
            }
            
        }
        public void SetNormalFashionPart(ProtoTable.ItemTable.eSubType eSubType)
        {
            _SyncNormalFashionPartItem(mFashionPart);
        }
        public void SetSlotPart(ProtoTable.ItemTable.eSubType eSubType)
        {
            int iSlotID = _GetSlotIDByType(eSubType);
            if(iSlotID >= 0 && iSlotID < mChecks.Length)
            {
                for(int i = 0; i < mChecks.Length;++i)
                {
                    mChecks[i].CustomActive(iSlotID == i);
                }
            }
        }

        public void SetSkySuitName()
        {
            if (null != mSkySuitName)
            {
                string suitName = string.Empty;
                int key = FashionMergeManager.GetInstance().GetFashionByKey(FashionMergeManager.GetInstance().FashionType, PlayerBaseData.GetInstance().JobTableID, FashionMergeManager.GetInstance().SkySuitID, ProtoTable.ItemTable.eSubType.FASHION_HEAD);
                var skyItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(key);
                if(null != skyItem)
                {
                    suitName = skyItem.SuitName;
                }
                mSkySuitName.text = suitName;
            }
        }

        public void ForceUpdateSkyStatus()
        {
            for (int i = 0; i < mTypeRelationSkyStatus.Length; ++i)
            {
                var comStatus = mTypeRelationSkyStatus[i];
                if (null == comStatus)
                {
                    continue;
                }

                if (mFashionType == FashionType.FT_SKY)
                {
                    if (i < skySlotValues.Length)
                    {
                        comStatus.Key = skySlotValues[i] == 1 ? mStrEnable : mStrDisable;
                    }
                }
                else if (mFashionType == FashionType.FT_GOLD_SKY)
                {
                    if (i < goldSkySlotValues.Length)
                    {
                        comStatus.Key = goldSkySlotValues[i] == 1 ? mStrEnable : mStrDisable;
                    }
                }
                else if (mFashionType == FashionType.FT_NATIONALDAY)
                {
                    if (i < nationalDaySlotValues.Length)
                    {
                        comStatus.Key = nationalDaySlotValues[i] == 1 ? mStrEnable : mStrDisable;
                    }
                }
            }
        }

        public void UpdateSkyStatus()
        {
            _UpdateSkyStatus();
        }

        public void UpdateSlotSkyStatus(ProtoTable.ItemTable.eSubType eSubType)
        {
            int slotId = _GetSlotIDByType(eSubType);
            if(slotId >= 0 && slotId < goldSkySlotValues.Length)
            {
                goldSkySlotValues[slotId] = 0;
            }
            if (slotId >= 0 && slotId < skySlotValues.Length)
            {
                skySlotValues[slotId] = 0;
            }

            _AssignSpecificSkyValue(ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.Fashion, eSubType));
            _AssignSpecificSkyValue(ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.WearFashion,eSubType));

            if (slotId >= 0 && slotId < skySlotValues.Length && slotId < mSkyStatus.Length)
            {
                if (null != mSkyStatus[slotId])
                {
                    mSkyStatus[slotId].Key = skySlotValues[slotId] == 1 ? mStrEnable : mStrDisable;
                }
            }

            if (slotId >= 0 && slotId < goldSkySlotValues.Length && slotId < mGoldSkyStatus.Length)
            {
                if (null != mGoldSkyStatus[slotId])
                {
                    mGoldSkyStatus[slotId].Key = goldSkySlotValues[slotId] == 1 ? mStrEnable : mStrDisable;
                }
            }
        }

        private void _SyncSkyFashionPartItem(ProtoTable.ItemTable.eSubType eSubType)
        {
            ItemData itemData = null;
            int key = 0;
            if (FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                key = FashionMergeManager.GetInstance().GetFashionByKey(FashionType.FT_NATIONALDAY, PlayerBaseData.GetInstance().JobTableID, FashionMergeManager.GetInstance().SkySuitID, eSubType);
            }
            else
            {
                key = FashionMergeManager.GetInstance().GetFashionByKey(FashionType.FT_SKY, PlayerBaseData.GetInstance().JobTableID, FashionMergeManager.GetInstance().SkySuitID, eSubType);
            }
            
            if (0 != key)
            {
                itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(key);
            }

            if (null == mComItemPreview)
            {
                mComItemPreview = ComItemManager.Create(mGoPreviewParent);
            }

            if (null != mComItemPreview)
            {
                mComItemPreview.Setup(itemData, _OnFashionPartItemClicked);
            }
        }

        private void _SyncNormalFashionPartItem(ProtoTable.ItemTable.eSubType eSubType)
        {
            ItemData itemData = null;
            int key = FashionMergeManager.GetInstance().GetFashionByKey(FashionType.FT_NORMAL, PlayerBaseData.GetInstance().JobTableID, FashionMergeManager.GetInstance().NormalSuitID, eSubType);
            if (0 != key)
            {
                itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(key);
            }

            if (null == mComItemNormalPreview)
            {
                mComItemNormalPreview = ComItemManager.Create(mGoNormalPreviewParent);
            }

            if (null != mComItemNormalPreview)
            {
                mComItemNormalPreview.Setup(itemData, _OnNormalFashionPartItemClicked);
            }
        }

        void _OnNormalFashionPartItemClicked(GameObject obj, ItemData item)
        {
            ItemTipManager.GetInstance().ShowTip(item);
            // if(null == item)
            // {
            //     return;
            // }

            // List<ItemData> datas = new List<ItemData>();
            // for(int i = 1,key = 1; 0 != key; ++i)
            // {
            //     key = FashionMergeManager.GetInstance().GetFashionByKey(FashionType.FT_NORMAL, PlayerBaseData.GetInstance().JobTableID, i,(ProtoTable.ItemTable.eSubType)item.SubType);
            //     if(0 != key)
            //     {
            //         var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(key);
            //         if(null != itemData)
            //         {
            //             datas.Add(itemData);
            //         }
            //     }
            // }

            // datas.Sort((x, y) =>
            // {
            //     if(x.Quality != y.Quality)
            //     {
            //         return x.Quality - y.Quality;
            //     }
            //     return x.TableID - y.TableID;
            // });

            // ClientSystemManager.GetInstance().OpenFrame<FashionAvatarSeleteFrame>(FrameLayer.Middle, datas);
        }

        void _OnFashionPartItemClicked(GameObject obj, ItemData item)
        {
            if (null != item)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        public bool IsWindUnLock()
        {
            if (mFashionType == FashionType.FT_SKY)
            {
                return skySlotValues[windIndex] == 1;
            }
            else
            {
                return goldSkySlotValues[windIndex] == 1;
            }
        }

        void Awake()
        {
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged += _OnMoneyChanged;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnNormalFashionSelected, _OnNormalFashionSelected);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnConfirmToFashionMerge, _OnConfirmToFashionMerge);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnFashionMergeRedCounChanged, _OnRedPointChanged);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnFashionFastItemBuyFinished, OnFashionFastItemBuyFinished);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnFashionNormalItemSelected, OnFashionNormalItemSelected);
        }

        void Start()
        {
            _UpdateSkyStatus();
            _UpdateSlotStatus();
            
            if (FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                ActivityFashionMergeBindGoldIndex = 0;
                FashionMergeItemIndex = 1;
            }
            else
            {
                FashionMergeItemIndex = 0;
            }
            
        }

        void _AssignSpecificSkyValue(List<ulong> guids)
        {
            if (null != guids)
            {
                for (int i = 0; i < guids.Count; ++i)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(guids[i]);
                    if (null == itemData)
                    {
                        continue;
                    }

                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)itemData.TableID);
                    if (null == item)
                    {
                        continue;
                    }

                    int iSlotID = _GetSlotIDByType(item.SubType);
                    if (iSlotID < 0 || iSlotID >= skySlotValues.Length || iSlotID >= goldSkySlotValues.Length)
                    {
                        continue;
                    }

                    var skyItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(item.ID);
                    if (null == skyItem)
                    {
                        continue;
                    }
                    //job filter
                    if (skyItem.Occu != PlayerBaseData.GetInstance().JobTableID / 10)
                    {
                        continue;
                    }

                    if (skyItem.Type == 1)
                    {
                        skySlotValues[iSlotID] = 1;
                    }
                    else if (skyItem.Type == 2)
                    {
                        goldSkySlotValues[iSlotID] = 1;
                    }
                }
            }
        }

        void _AssignSkyValues(List<ulong> guids)
        {
            if (null != guids)
            {
                for (int i = 0; i < guids.Count; ++i)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(guids[i]);
                    if (null == itemData)
                    {
                        continue;
                    }
                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)itemData.TableID);
                    if (null == item)
                    {
                        continue;
                    }
                    int iSlotID = _GetSlotIDByType(item.SubType);
                    if (iSlotID < 0 || iSlotID >= skySlotValues.Length || iSlotID >= goldSkySlotValues.Length)
                    {
                        continue;
                    }

                    var skyItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(item.ID);
                    if (null == skyItem)
                    {
                        continue;
                    }
                    //job filter
                    if(skyItem.Occu != PlayerBaseData.GetInstance().JobTableID / 10)
                    {
                        continue;
                    }

                    if (skyItem.Type == 1)
                    {
                        skySlotValues[iSlotID] = 1;
                    }
                    else if (skyItem.Type == 2)
                    {
                        goldSkySlotValues[iSlotID] = 1;
                    }
                    else if (skyItem.Type == (int)FashionType.FT_NATIONALDAY)
                    {
                        nationalDaySlotValues[iSlotID] = 1;
                    }
                }
            }
        }

        void _UpdateSkyStatus()
        {
            for(int i = 0; i < goldSkySlotValues.Length; ++i)
            {
                goldSkySlotValues[i] = 0;
            }
            for (int i = 0; i < skySlotValues.Length; ++i)
            {
                skySlotValues[i] = 0;
            }
            for (int i = 0; i < nationalDaySlotValues.Length; i++)
            {
                nationalDaySlotValues[i] = 0;
            }

            _AssignSkyValues(ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Fashion));
            _AssignSkyValues(ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion));

            for (int i = 0; i < mSkyStatus.Length  && i < skySlotValues.Length; ++i)
            {
                if(null != mSkyStatus[i])
                {
                    mSkyStatus[i].Key = skySlotValues[i] == 1 ? mStrEnable : mStrDisable;
                }
            }

            for (int i = 0; i < mGoldSkyStatus.Length && i < goldSkySlotValues.Length; ++i)
            {
                if (null != mGoldSkyStatus[i])
                {
                    mGoldSkyStatus[i].Key = goldSkySlotValues[i] == 1 ? mStrEnable : mStrDisable;
                }
            }

            if (null != mWindStatus)
            {
                if (mFashionType != FashionType.FT_NATIONALDAY)
                {
                    if (mFashionType == FashionType.FT_SKY)
                    {
                        mWindStatus.Key = skySlotValues[windIndex] == 1 ? mStrEnable : mStrDisable;
                    }
                    else
                    {
                        mWindStatus.Key = goldSkySlotValues[windIndex] == 1 ? mStrEnable : mStrDisable;
                    }
                }
            }
        }

        public void AddFashionToMerge()
        {
            if (null == kEquipments)
            {
                kEquipments = new List<ItemData>();
            }

            ComFashionMergeItemEx.LoadAllEquipments(ref kEquipments, mFashionPart, isLeftPos);
            if (kEquipments.Count <= 0)
            {
                return;
            }
            if (mLeft)
            {
                LValue = kEquipments[0];
                if (null != mEffectLoader)
                {
                    mEffectLoader.DeActiveEffect(12);
                    mEffectLoader.LoadEffect(12);
                    mEffectLoader.ActiveEffect(12);
                }
            }
            else
            {
                RValue = kEquipments[0];
                if (null != mEffectLoader)
                {
                    mEffectLoader.DeActiveEffect(13);
                    mEffectLoader.LoadEffect(13);
                    mEffectLoader.ActiveEffect(13);
                }
            }
        }
        public void TryAutoEquipFashion()
        {
            if (null == kEquipments)
            {
                kEquipments = new List<ItemData>();
            }

            ComFashionMergeItemEx.LoadAllEquipments(ref kEquipments, mFashionPart, true);
            if(kEquipments == null)
            {
                return;
            }          

            // 这里剔除掉被锁住的时装
            for(int i = 0;i < kEquipments.Count;)
            {
                ItemData itemData = kEquipments[i];
                if(itemData == null || itemData.bFashionItemLocked)
                {
                    kEquipments.RemoveAt(i);
                    continue;
                }
                i++;
            }            

            if (!FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                if (null == LValue && kEquipments.Count > 0 && (null == RValue || kEquipments[0].GUID != RValue.GUID))
                {
                    LValue = kEquipments[0];
                    if (null != mEffectLoader)
                    {
                        mEffectLoader.DeActiveEffect(12);
                        mEffectLoader.LoadEffect(12);
                        mEffectLoader.ActiveEffect(12);
                    }
                }
                if (null == RValue && kEquipments.Count > 1 && (null == LValue || kEquipments[1].GUID != LValue.GUID))
                {
                    RValue = kEquipments[1];
                    if (null != mEffectLoader)
                    {
                        mEffectLoader.DeActiveEffect(13);
                        mEffectLoader.LoadEffect(13);
                        mEffectLoader.ActiveEffect(13);
                    }
                }
            }
            else
            {
                if (null == LValue && kEquipments.Count > 0)
                {
                    LValue = kEquipments[0];
                    if (null != mEffectLoader)
                    {
                        mEffectLoader.DeActiveEffect(12);
                        mEffectLoader.LoadEffect(12);
                        mEffectLoader.ActiveEffect(12);
                    }
                }
            }
                
        }
        public void AddMergeFashion(bool bLeft)
        {
            isLeftPos = bLeft;
            mLeft = bLeft;

            var fashionItemSelectedType = new FashionItemSelectedType
            {
                FashionPart = mFashionPart,
                IsLeft = bLeft,
            };
            ClientSystemManager.GetInstance()
                .OpenFrame<FashionMergeNewItemFrame>(FrameLayer.Middle, fashionItemSelectedType);
        }

        void _UpdateSlotStatus()
        {
            if(null != lValue)
            {
                lValue = ItemDataManager.GetInstance().GetItem(lValue.GUID);
            }
            if(null != lValue)
            {
                if (!Utility._CheckFashionCanMerge(lValue.GUID))
                {
                    lValue = null;
                }
            }

            if (!FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                if (null != rValue)
                {
                    rValue = ItemDataManager.GetInstance().GetItem(rValue.GUID);
                }
                if (null != rValue)
                {
                    if (!Utility._CheckFashionCanMerge(rValue.GUID))
                    {
                        rValue = null;
                    }
                }
            }
            
            if (null != mLSlotStatus)
            {
                if(lValue == null)
                {
                    mLSlotStatus.Key = mStrDisable;
                }
                else
                {
                    mLSlotStatus.Key = mStrEnable;
                }
            }
            if(null == leftSlot)
            {
                leftSlot = ComItemManager.Create(goLeftParent);
            }
            if(null != leftSlot)
            {
                if (null != LValue)
                    leftSlot.Setup(LValue, _OnSlotItemClicked);
                else
                    leftSlot.Setup(null, null);
            }

            if (!FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                if (null != mRSlotStatus)
                {
                    if (rValue == null)
                    {
                        mRSlotStatus.Key = mStrDisable;
                    }
                    else
                    {
                        mRSlotStatus.Key = mStrEnable;
                    }
                }
                if (null == rightSlot)
                {
                    rightSlot = ComItemManager.Create(goRightParent);
                }
                if (null != rightSlot)
                {
                    if (null != RValue)
                        rightSlot.Setup(RValue, _OnSlotItemClicked);
                    else
                        rightSlot.Setup(null, null);
                }
            }
        }

        public void UpdateSlotItems()
        {
            _UpdateSlotStatus();
        }

        void OnDestroy()
        {
            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged -= _OnMoneyChanged;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnNormalFashionSelected, _OnNormalFashionSelected);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnConfirmToFashionMerge, _OnConfirmToFashionMerge);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnFashionMergeRedCounChanged, _OnRedPointChanged);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnFashionFastItemBuyFinished, OnFashionFastItemBuyFinished);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnFashionNormalItemSelected, OnFashionNormalItemSelected);

            if (null != mEffectLoader)
            {
                InvokeMethod.RmoveInvokeIntervalCall(mEffectLoader);
            }

            for (int i = 0; i < mComItems.Length; ++i)
            {
                if(null != mComItems[i])
                {
                    ComItemManager.Destroy(mComItems[i]);
                    mComItems[i] = null;
                }
            }

            if(null != mComItemPreview)
            {
                ComItemManager.Destroy(mComItemPreview);
                mComItemPreview = null;
            }

            if (null != mComItemNormalPreview)
            {
                ComItemManager.Destroy(mComItemNormalPreview);
                mComItemNormalPreview = null;
            }

            if (null != mComBoxItem)
            {
                ComItemManager.Destroy(mComBoxItem);
                mComBoxItem = null;
            }

            if (null != leftSlot)
            {
                ComItemManager.Destroy(leftSlot);
                leftSlot = null;
            }

            if (null != rightSlot)
            {
                ComItemManager.Destroy(rightSlot);
                rightSlot = null;
            }

            if (mIntialized)
            {
                mIntialized = false;
            }

            if(null != kEquipments)
            {
                kEquipments.Clear();
                kEquipments = null;
            }
        }

        bool _CheckMerge(bool bNeedMsg)
        {
            if (null == LValue)
            {
                AddMergeFashion(true);
                //SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fashionMerge_select_left_fashion"));
                return false;
            }

            if (!FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                if (null == RValue)
                {
                    AddMergeFashion(false);
                    //SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fashionMerge_select_right_fashion"));
                    return false;
                }
            }
            else
            {
                var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(ActivityFashionMergeBindGolds[_ActivityFashionMergeBindGoldIndex]);
                int iBindGoldOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(ActivityFashionMergeBindGolds[_ActivityFashionMergeBindGoldIndex]);
                int iBindNeedCount = GetSlotExpendBindGoldCount();
                if (iBindGoldOwnedCount < iBindNeedCount)
                {
                    //ItemComeLink.OnLink(ActivityFashionMergeBindGolds[_ActivityFashionMergeBindGoldIndex],0);
                    string sContent = "{0}不足，请前往获取";
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(sContent, itemData.GetColorName()));
                    return false;
                }
            }
            
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Fashion);
            int iCount = 0;
            if (null != itemGuids)
            {
                iCount = itemGuids.Count;
            }
            if (PlayerBaseData.GetInstance().PackTotalSize[(int)EPackageType.Fashion] < iCount + mGridKeep)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fashionMerge_grid_is_not_enough", mGridKeep));
                return false;
            }

            if (null == mComBoxItem || null == mComBoxItem.ItemData)
            {
                Logger.LogErrorFormat("comBoxItem or itemdata is null !!!");
                return false;
            }

            if (null == mComItemNormalPreview || null == mComItemNormalPreview.ItemData)
            {
                Logger.LogErrorFormat("mComItemNormalPreview or itemdata is null !!!");
                return false;
            }

            var suitNormalSuitItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeTable>(mComItemNormalPreview.ItemData.TableID);
            if (null == suitNormalSuitItem)
            {
                Logger.LogErrorFormat("suitId is invalid with tableid = {0}", mComItemNormalPreview.ItemData.TableID);
                return false;
            }

            if (null == mComItemPreview || null == mComItemPreview.ItemData)
            {
                Logger.LogErrorFormat("skyTypeId is invalid with tableid = {0}", mComItemPreview.ItemData.TableID);
                return false;
            }

            var skyItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(mComItemPreview.ItemData.TableID);
            if (null == skyItem)
            {
                Logger.LogErrorFormat("skyTypeId is invalid with tableid = {0} can not find in FashionComposeSkyTable !!!", mComItemPreview.ItemData.TableID);
                return false;
            }

            int iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(mComBoxItem.ItemData.TableID);
            int iNeedCount = 1;
            if (iOwnedCount < iNeedCount)
            {
                if(bNeedMsg)
                {
                    if (_FashionMergeItemIndex == 0)
                    {
                        _onFastBuy(mComBoxItem.ItemData.TableID);
                    }
                    else
                    {
                        ItemComeLink.OnLink(mComBoxItem.ItemData.TableID, 0);
                    }
                    //ItemComeLink.OnLink(mComBoxItem.ItemData.TableID, 1, false);
                }
                return false;
            }

            var realMergeItem = ItemDataManager.GetInstance().GetItemByTableID(mComBoxItem.ItemData.TableID);
            if (null == realMergeItem)
            {
                Logger.LogErrorFormat("can not find merge item !!! for table id = {0}", mComBoxItem.ItemData.TableID);
                return false;
            }

            return true;
        }

        public void OnStopMerge()
        {
            if(null != mMergeState)
            {
                mMergeState.Key = mStrEnable;
            }
            if (null != mEffectState)
            {
                mEffectState.Key = mStrEnable;
            }
        }

        public void EnableMergeState()
        {
            if (null != mMergeState)
            {
                mMergeState.Key = mStrEnable;
            }
        }

        public void OnClickMergeFashion()
        {
            if (!FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                OnMergeFashionClick();
            }
            else
            {
                if (null == LValue)
                {
                    AddMergeFashion(true);
                    return;
                }

                int iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(mComBoxItem.ItemData.TableID);
                int iNeedCount = 1;
                if (iOwnedCount < iNeedCount)
                {
                    ItemComeLink.OnLink(mComBoxItem.ItemData.TableID, 0);
                    return;
                }

                int mNum = 0;
                var mChangeFashionActivityMergeTable = TableManager.GetInstance().GetTableItem<ChangeFashionActiveMergeTable>(mComItemNormalPreview.ItemData.TableID);
                if (mChangeFashionActivityMergeTable != null)
                {
                    string[] mStrings = mChangeFashionActivityMergeTable.GoldConsume.Split('_');
                    if (mStrings.Length > 0)
                    {
                        int.TryParse(mStrings[1], out mNum);
                    }
                }
                CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindGOLD);
                costInfo.nCount = mNum;
                CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                {
                    OnMergeFashionClick();
                });
            }
        }

        void OnMergeFashionClick()
        {
            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            if (!_CheckMerge(true))
            {
                return;
            }

            var skyItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(mComItemPreview.ItemData.TableID);
            if (null == skyItem)
            {
                Logger.LogErrorFormat("skyTypeId is invalid with tableid = {0} can not find in FashionComposeSkyTable !!!", mComItemPreview.ItemData.TableID);
                return;
            }

            var realMergeItem = ItemDataManager.GetInstance().GetItemByTableID(mComBoxItem.ItemData.TableID);
            if (null == realMergeItem)
            {
                Logger.LogErrorFormat("can not find merge item !!! for table id = {0}", mComBoxItem.ItemData.TableID);
                return;
            }

            string ret = string.Empty;

            if (!_HasNeedNotifyFashions(ref ret))
            {
                _OnConfirmToMerge();
                return;
            }

            SystemNotifyManager.SystemNotify(7029, null, _OnConfirmToMerge, new object[] { ret });
        }

        bool _HasNeedNotifyFashions(ref string ret)
        {
            bool bHas = false;

            if(null != LValue)
            {
                if(lValue.ThirdType == ProtoTable.ItemTable.eThirdType.FASHION_FESTIVAL)
                {
                    bHas = true;
                    if(null != ret)
                    {
                        ret = LValue.GetColorName();
                    }
                }
            }

            if (!FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                if (null != RValue)
                {
                    if (RValue.ThirdType == ProtoTable.ItemTable.eThirdType.FASHION_FESTIVAL)
                    {
                        bHas = true;
                        if (string.IsNullOrEmpty(ret))
                        {
                            ret = RValue.GetColorName();
                        }
                        else
                        {
                            ret = ret + "、" + RValue.GetColorName();
                        }
                    }
                }
            }
            
            return bHas;
        }

        void _OnConfirmToMerge()
        {
            if (null != mEffectState)
            {
                mEffectState.Key = mStrEnable;
            }

            if (null != mMergeState)
            {
                mMergeState.Key = mStrDisable;
            }
        }

        void _OnAddNewItem(List<Item> items)
        {
            //_UpdateSkyStatus();
            _UpdateMergeBoxItem();
            _UpdateSlotStatus();
        }
        void _OnRemoveItem(ItemData data)
        {
            //_UpdateSkyStatus();
            _UpdateMergeBoxItem();
            _UpdateSlotStatus();
        }
        void _OnUpdateItem(List<Item> items)
        {
            //_UpdateSkyStatus();
            _UpdateMergeBoxItem();
            _UpdateSlotStatus();
        }

        void _OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            _UpdateBindGoldItem();
        }

        void _OnNormalFashionSelected(UIEvent uiEvent)
        {
            ItemData itemData = uiEvent.Param1 as ItemData;
            if(null != itemData)
            {
                if(null != mComItemNormalPreview)
                {
                    mComItemNormalPreview.Setup(itemData, _OnNormalFashionPartItemClicked);
                }
            }
        }

        void _OnConfirmToFashionMerge(UIEvent uiEvent)
        {
            if(!_CheckMerge(false))
            {
                return;
            }

            var skyItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(mComItemPreview.ItemData.TableID);
            if (null == skyItem)
            {
                Logger.LogErrorFormat("skyTypeId is invalid with tableid = {0} can not find in FashionComposeSkyTable !!!", mComItemPreview.ItemData.TableID);
                return;
            }

            var realMergeItem = ItemDataManager.GetInstance().GetItemByTableID(mComBoxItem.ItemData.TableID);
            if (null == realMergeItem)
            {
                Logger.LogErrorFormat("can not find merge item !!! for table id = {0}", mComBoxItem.ItemData.TableID);
                return;
            }

            if (!FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                FashionMergeManager.GetInstance().SendFashionMerge(LValue.GUID, RValue.GUID, realMergeItem.GUID, skyItem.SuitID, mComItemNormalPreview.ItemData.TableID);
                WaitNetMessageManager.GetInstance().Wait(
                    SceneFashionMergeRes.MsgID,
                    FashionMergeManager.GetInstance().OnRecvSceneFashionMergeRes,
                    true, 15,
                    _onFashionMergeTimeOut
                );
            }
            else
            {
                FashionMergeManager.GetInstance().SendChangeFashionMerge(LValue.GUID, realMergeItem.GUID, (uint)mComItemNormalPreview.ItemData.TableID);
                WaitNetMessageManager.GetInstance().Wait(SceneFashionChangeActiveMergeRet.MsgID,
                    FashionMergeManager.GetInstance().OnRecvSceneFashionChangeActiveMergeRet,
                    true, 15,
                    _onFashionMergeTimeOut
                    );
            }
        }

        private void _onFashionMergeTimeOut()
        {
            EnableMergeState();
        }
       
        private void _onFastBuy(int id)
        {
            WorldGetMallItemByItemIdReq msg = new WorldGetMallItemByItemIdReq();
            msg.itemId = (uint)id;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldGetMallItemByItemIdRes>(msgRet =>
            {
                var mallItemInfo = msgRet.mallItem;
                ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, mallItemInfo);
            }, false);
        }

        #region FashionItem
        private void OnFashionNormalItemSelected(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            var script = uiEvent.Param1 as ComFashionMergeItemEx;
            if (null != script)
            {
                if (mLeft)
                {
                    LValue = script.ItemData;
                    if (null != mEffectLoader)
                    {
                        mEffectLoader.DeActiveEffect(12);
                        mEffectLoader.LoadEffect(12);
                        mEffectLoader.ActiveEffect(12);
                    }
                }
                else
                {
                    RValue = script.ItemData;
                    if (null != mEffectLoader)
                    {
                        mEffectLoader.DeActiveEffect(13);
                        mEffectLoader.LoadEffect(13);
                        mEffectLoader.ActiveEffect(13);
                    }
                }
            }
        }

        private void OnFashionFastItemBuyFinished(UIEvent uiEvent)
        {
            AddFashionToMerge();
        }
        #endregion

    }
}