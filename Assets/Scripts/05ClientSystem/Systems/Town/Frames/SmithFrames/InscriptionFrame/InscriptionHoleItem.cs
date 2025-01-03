using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public delegate void OnSelectedInscriptionItemData(InscriptionHoleData inscriptionHoleData);
    public class InscriptionHoleItem : MonoBehaviour
    {
        [SerializeField] private StateController mStateControl;
        [SerializeField] private Button mCanOpenHoleBtn;
        [SerializeField] private Toggle mSelectedTog;
        [SerializeField] private Image mExpendIconBG;
        [SerializeField] private Image mExpendIcon;
        [SerializeField] private Image mTapperIconBG;
        [SerializeField] private Image mTapperIcon;//开孔器图标
        [SerializeField] private Image mCanOpenHoleIcon;//孔颜色
        [SerializeField] private Image mCanBeSetIcon;//孔颜色
        [SerializeField] private Image mToSetIncriptionQualityBox;
        [SerializeField] private Image mToSetIncriptionIcon;
        [SerializeField] private Text mExpendCount;
        [SerializeField] private Text mTapperExpendCount;
        [SerializeField] private Text mToSetInscriptionName;
        [SerializeField] private Text mToSetInscriptionAttr;
        [SerializeField] private GameObject mSelectedRoot;
        [SerializeField] private GameObject mGoldRoot;
        [SerializeField] private GameObject mTapperRoot;
        [SerializeField] private Button mCataclasticHarvestingBtn;

        private InscriptionHoleData mData;//孔数据
        private InscriptionMosaicState mState;//铭文孔状态
        private int iInscriptionHoleIndex;//铭文孔索引
        private ItemData mCurrentSelectedItemData;//当前选择的装备
        private InscriptionHoleSetTable inscriptionHoleSetTable;//铭文孔镶嵌
        private OnSelectedInscriptionItemData mOnSelectedInscriptionItemData;
        private void Awake()
        {
            if (mSelectedTog != null)
            {
                mSelectedTog.onValueChanged.RemoveAllListeners();
                mSelectedTog.onValueChanged.AddListener(OnSelectedInscriptionHoleTogClick);
            }

            if (mCanOpenHoleBtn != null)
            {
                mCanOpenHoleBtn.onClick.RemoveAllListeners();
                mCanOpenHoleBtn.onClick.AddListener(OnOpenInscriptionHoleClick);
            }

            if(mCataclasticHarvestingBtn != null)
            {
                mCataclasticHarvestingBtn.onClick.RemoveAllListeners();
                mCataclasticHarvestingBtn.onClick.AddListener(OnCataclasticHarvestingBtnClick);
            }
        }

        private void OnDestroy()
        {
            mData = null;
            mState = InscriptionMosaicState.None;
            iInscriptionHoleIndex = -1;
            mCurrentSelectedItemData = null;
            inscriptionHoleSetTable = null;
            mOnSelectedInscriptionItemData = null;
        }

        public void OnItemVisiable(InscriptionHoleData data,ItemData currentSelectedItemData,ToggleGroup group,bool isSelected, OnSelectedInscriptionItemData callBack)
        {
            mData = data;
            if (mSelectedTog != null)
            {
                mSelectedTog.group = group;
            }
            
            mCurrentSelectedItemData = currentSelectedItemData;
            iInscriptionHoleIndex = data.Index;
            mOnSelectedInscriptionItemData = callBack;
            
            UpdateState();
            UpdateInterface();

            if (mSelectedTog != null)
            {
                if (isSelected)
                {
                    if (mSelectedTog.isOn == true)
                        OnSelectedInscriptionHoleTogClick(isSelected);
                    else
                        mSelectedTog.isOn = true;
                }
                else
                {
                    mSelectedTog.isOn = false;
                }
            }
        }

        private void UpdateState()
        {
            inscriptionHoleSetTable = TableManager.GetInstance().GetTableItem<InscriptionHoleSetTable>(mData.Type);

            //未开孔
            if (mData.IsOpenHole == false)
            {
                mStateControl.Key = "CanOpenHole";
                mState = InscriptionMosaicState.CanOpenHole;
            }
            else
            {
                if (mData.InscriptionId == 0)
                {
                    mStateControl.Key = "CanBeSet";
                    mState = InscriptionMosaicState.CanBeSet;
                }
                else if (mData.InscriptionId > 0)
                {
                    mStateControl.Key = "HasBeenSet";
                    mState = InscriptionMosaicState.HasBeenSet;
                }
            }
        }

        private void UpdateInterface()
        {
            mSelectedTog.CustomActive(true);
            mCataclasticHarvestingBtn.CustomActive(false);
            switch (mState)
            {
                case InscriptionMosaicState.None:
                    break;
                case InscriptionMosaicState.CanOpenHole:
                    {
                        mSelectedTog.CustomActive(false);
                        InitCanOpenHoleInfo();
                    }
                    break;
                case InscriptionMosaicState.CanBeSet:
                    {
                        InitCanBeSetInfo();
                    }
                    break;
                case InscriptionMosaicState.HasBeenSet:
                    {
                        InitHasBeenSetInfo();
                    }
                    break;
            }
        }

        #region  CanOpenHoleInfo
        private void InitCanOpenHoleInfo()
        {
            if (inscriptionHoleSetTable != null)
            {
                if (mCanOpenHoleIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref mCanOpenHoleIcon, inscriptionHoleSetTable.InscriptionHolePicture);
                }
            }

            EquipInscriptionHoleData mConsume = InscriptionMosaicDataManager.GetInstance().GetOpenInscriptionHoleConsume(mCurrentSelectedItemData);
            //如果开孔器和金币表格中都填了
            if (mConsume.iItemConsume != null && mConsume.iGoldConsume != null)
            {
                int itemConsumeAllCount = ItemDataManager.GetInstance().GetItemCount(mConsume.iItemConsume.itemId);
                //如果开孔器为0
                if (itemConsumeAllCount <= 0)
                {
                    int goldConsumeAllCount = ItemDataManager.GetInstance().GetOwnedItemCount(mConsume.iGoldConsume.itemId);
                    if (goldConsumeAllCount > 0 && goldConsumeAllCount >= mConsume.iGoldConsume.count)
                    {
                        SetConsumeInfo(mConsume.iGoldConsume, false);
                    }
                    else
                    {
                        SetConsumeInfo(mConsume.iItemConsume, true);
                    }
                }
                else
                {
                    if (itemConsumeAllCount >= mConsume.iItemConsume.count)
                    {
                        SetConsumeInfo(mConsume.iItemConsume, false);
                    }
                    else
                    {
                        SetConsumeInfo(mConsume.iItemConsume, true);
                    }
                }
            }
            else
            {
                //表中填了开孔器，未填金币
                if (mConsume.iItemConsume != null && mConsume.iGoldConsume == null)
                {
                    int itemConsumeAllCount = ItemDataManager.GetInstance().GetItemCount(mConsume.iItemConsume.itemId);
                    if (itemConsumeAllCount >= mConsume.iItemConsume.count)
                    {
                        SetConsumeInfo(mConsume.iItemConsume, false);
                    }
                    else
                    {
                        SetConsumeInfo(mConsume.iItemConsume, true);
                    }
                }else if (mConsume.iItemConsume == null && mConsume.iGoldConsume != null)
                {//填了金币，未填开孔器
                    int goldConsumeAllCount = ItemDataManager.GetInstance().GetOwnedItemCount(mConsume.iGoldConsume.itemId);
                    if (goldConsumeAllCount > 0 && goldConsumeAllCount >= mConsume.iGoldConsume.count)
                    {
                        SetConsumeInfo(mConsume.iGoldConsume, false);
                    }
                    else
                    {
                        SetConsumeInfo(mConsume.iGoldConsume, true);
                    }
                }
            }
        }

        private void SetConsumeInfo(InscriptionConsume consume, bool isShowRed = false)
        {
            var itemData = ItemDataManager.CreateItemDataFromTable(consume.itemId);
            if (itemData == null)
            {
                Logger.LogErrorFormat("ItemTable id is Null id = {0}", consume.itemId);
            }

            if (itemData.SubType == (int)ItemTable.eSubType.GOLD)
            {
                mGoldRoot.CustomActive(true);
                mTapperRoot.CustomActive(false);

                if (mExpendIconBG != null)
                {
                    ETCImageLoader.LoadSprite(ref mExpendIconBG, itemData.GetQualityInfo().Background);
                }

                if (mExpendIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref mExpendIcon, itemData.Icon);
                }

                if (mExpendCount != null)
                {
                    mExpendCount.text = consume.count.ToString();
                }

                if (isShowRed == true)
                {
                    mExpendCount.color = Color.red;
                }
                else
                {
                    mExpendCount.color = Color.white;
                }
            }
            else
            {
                mGoldRoot.CustomActive(false);
                mTapperRoot.CustomActive(true);

                if (mTapperIconBG != null)
                {
                    ETCImageLoader.LoadSprite(ref mTapperIconBG, itemData.GetQualityInfo().Background);
                }

                if (mTapperIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref mTapperIcon, itemData.Icon);
                }

                if (isShowRed == true)
                {
                    mTapperExpendCount.text = TR.Value("Inscription_Expend_Red", itemData.GetColorName(), consume.count);
                }
                else
                {
                    mTapperExpendCount.text = TR.Value("Inscription_Expend_Green", itemData.GetColorName(), consume.count);
                }
            }
            
        }

        private void OnOpenInscriptionHoleClick()
        {
            EquipInscriptionHoleData mConsume = InscriptionMosaicDataManager.GetInstance().GetOpenInscriptionHoleConsume(mCurrentSelectedItemData);
            if (mConsume != null)
            { //如果开孔器和金币表格中都填了
                if (mConsume.iItemConsume != null && mConsume.iGoldConsume != null)
                {
                    int itemConsumeAllCount = ItemDataManager.GetInstance().GetItemCount(mConsume.iItemConsume.itemId);
                    //如果开孔器为0
                    if (itemConsumeAllCount <= 0)
                    {
                        int goldConsumeAllCount = ItemDataManager.GetInstance().GetOwnedItemCount(mConsume.iGoldConsume.itemId);
                        if (goldConsumeAllCount > 0 && goldConsumeAllCount >= mConsume.iGoldConsume.count)
                        {
                            OnSendEquipmentOpenInscriptionHole(mConsume.iGoldConsume);
                        }
                        else
                        {
                            //开孔器不足的情况弹开孔器的快捷购买
                            ItemComeLink.OnLink(mConsume.iItemConsume.itemId, mConsume.iItemConsume.count - itemConsumeAllCount, true, OnSceneEquipInscriptionOpenReq);
                        }
                    }
                    else
                    {
                        if (itemConsumeAllCount >= mConsume.iItemConsume.count)
                        {
                            OnSendEquipmentOpenInscriptionHole(mConsume.iItemConsume);
                        }
                        else
                        {
                            //开孔器不足的情况弹开孔器的快捷购买
                            ItemComeLink.OnLink(mConsume.iItemConsume.itemId, mConsume.iItemConsume.count - itemConsumeAllCount, true, OnSceneEquipInscriptionOpenReq);
                        }
                    }
                }
                else
                {
                    //表中填了开孔器，未填金币
                    if (mConsume.iItemConsume != null && mConsume.iGoldConsume == null)
                    {
                        int itemConsumeAllCount = ItemDataManager.GetInstance().GetItemCount(mConsume.iItemConsume.itemId);
                        if (itemConsumeAllCount >= mConsume.iItemConsume.count)
                        {
                            OnSendEquipmentOpenInscriptionHole(mConsume.iItemConsume);
                        }
                        else
                        {
                            //开孔器不足的情况弹开孔器的快捷购买
                            ItemComeLink.OnLink(mConsume.iItemConsume.itemId, mConsume.iItemConsume.count - itemConsumeAllCount, true, OnSceneEquipInscriptionOpenReq);
                        }
                    }
                    else if (mConsume.iItemConsume == null && mConsume.iGoldConsume != null)
                    {//填了金币，未填开孔器
                        int goldConsumeAllCount = ItemDataManager.GetInstance().GetOwnedItemCount(mConsume.iGoldConsume.itemId);
                        if (goldConsumeAllCount > 0 && goldConsumeAllCount >= mConsume.iGoldConsume.count)
                        {
                            OnSendEquipmentOpenInscriptionHole(mConsume.iGoldConsume);
                        }
                        else
                        {
                            //金币不足的情况弹金币的快捷购买
                            ItemComeLink.OnLink(mConsume.iGoldConsume.itemId, mConsume.iGoldConsume.count - goldConsumeAllCount, true, OnSceneEquipInscriptionOpenReq);
                        }
                    }
                }
            }
            else
            {
#if MG_TEST
               Logger.LogErrorFormat("InscriptionHoleItem Method [OnOpenInscriptionHoleClick] mConsume = null itemID = {0}",mCurrentSelectedItemData.TableID);
#endif
            }
        }

        private void OnSendEquipmentOpenInscriptionHole(InscriptionConsume consume)
        {
            if(consume == null)
            {
                return;
            }

            var itemData = ItemDataManager.CreateItemDataFromTable(consume.itemId);
            if (itemData == null)
            {
                return;
            }

            string content = string.Format("是否花费{0}*{1}开启一个插槽?", itemData.GetColorName(), consume.count);

            SystemNotifyManager.SysNotifyMsgBoxOkCancel(content, OnSceneEquipInscriptionOpenReq);
        }

        private void OnSceneEquipInscriptionOpenReq()
        {
            if(mCurrentSelectedItemData == null)
            {
                return;
            }

            InscriptionMosaicDataManager.GetInstance().OnSceneEquipInscriptionOpenReq(mCurrentSelectedItemData.GUID, (uint)iInscriptionHoleIndex);
        }
        #endregion

        #region CanBeSet

        private void InitCanBeSetInfo()
        {
            if (inscriptionHoleSetTable != null)
            {
                if (mCanBeSetIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref mCanBeSetIcon, inscriptionHoleSetTable.InscriptionHolePicture);
                    mCanBeSetIcon.SetNativeSize();
                }
            }
        }
        #endregion

        #region HasBeenSet

        private void InitHasBeenSetInfo()
        {
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(mData.InscriptionId);
            if (itemData != null)
            {
                if (mToSetIncriptionQualityBox != null)
                {
                    mToSetIncriptionQualityBox.color = itemData.GetQualityInfo().Col;
                }

                if (mToSetIncriptionIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref mToSetIncriptionIcon, itemData.Icon);
                }

                if (mToSetInscriptionName != null)
                {
                    mToSetInscriptionName.text = itemData.GetColorName();
                }

                if (mToSetInscriptionAttr != null)
                {
                    mToSetInscriptionAttr.text = InscriptionMosaicDataManager.GetInstance().GetInscriptionAttributesDesc(itemData.TableID);
                }

                mCataclasticHarvestingBtn.CustomActive(true);
            }
        }
        #endregion
        
        private void OnSelectedInscriptionHoleTogClick(bool value)
        {
            if(value)
            {
                if (mOnSelectedInscriptionItemData != null)
                    mOnSelectedInscriptionItemData.Invoke(mData);
            }
        }

        private void OnCataclasticHarvestingBtnClick()
        {
            InscriptionExtractItemData inscriptionExtractItemData = new InscriptionExtractItemData();
            inscriptionExtractItemData.equipmentItem = mCurrentSelectedItemData;
            inscriptionExtractItemData.inscriptionItem = ItemDataManager.CreateItemDataFromTable(mData.InscriptionId);
            inscriptionExtractItemData.index = mData.Index;

            if(ClientSystemManager.GetInstance().IsFrameOpen<InscriptionOperationFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<InscriptionOperationFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<InscriptionOperationFrame>(FrameLayer.Middle, inscriptionExtractItemData);
        }
    }
}