using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    enum GiftType
    {
        None = 0,
        UplevelGift,
        OnLineGift
    }

    class UpLevelGiftFrame : ClientFrame
    {
        const int iShowGiftNum = 5;

        GiftType eGiftType = GiftType.None;

        List<AwardItemData> ItemdataList = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MainFrameTown/UpLevelGiftFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                eGiftType = (GiftType)userData;
            }

            ActiveManager.ActiveData activeData = null;

            if(eGiftType == GiftType.UplevelGift)
            {             
                activeData = ActiveManager.GetInstance().GetActiveData(4000);
            }
            else if(eGiftType == GiftType.OnLineGift)
            {
                mTitle.text = TR.Value("OnlineGiftTitle");
                mWords.text = TR.Value("OnlineGiftWords");

                activeData = ActiveManager.GetInstance().GetActiveData(5000);
            }

            if(activeData == null)
            {
                return;
            }

            var acts = activeData.akChildItems;

            if(acts == null)
            {
                return;
            }

            int iCanReceiveIdx = -1;
            int iUnFinishIdx = -1;

            Utility.CalShowUplevelGiftIndex(activeData, ref iCanReceiveIdx, ref iUnFinishIdx);

            int iIndex = 0;

            if(iCanReceiveIdx != -1)
            {
                iIndex = iCanReceiveIdx;
            }
            else if(iUnFinishIdx != -1)
            {
                iIndex = iUnFinishIdx;
            }

            if (iCanReceiveIdx == -1 && iUnFinishIdx == -1)
            {
                mGiftTitle.text = "已没有可领取奖励";
            }
            else
            {
                if(eGiftType == GiftType.UplevelGift)
                {
                    if (PlayerBaseData.GetInstance().Level >= activeData.akChildItems[iIndex].activeItem.LevelLimit)
                    {
                        mGiftTitle.text = string.Format(TR.Value("LevelupFinishGift"), activeData.akChildItems[iIndex].activeItem.LevelLimit);
                    }
                    else
                    {
                        mGiftTitle.text = string.Format(TR.Value("LevelupUnfinishGift"), activeData.akChildItems[iIndex].activeItem.LevelLimit);
                    }
                }
                else if(eGiftType == GiftType.OnLineGift)
                {
                    int iDayOnlineTime = Utility.GetDayOnLineTime();
                    int iNeedTime = int.Parse(activeData.akChildItems[iIndex].activeItem.Param0);

                    if (iDayOnlineTime >= iNeedTime * 60)
                    {
                        mGiftTitle.text = string.Format(TR.Value("OnlineFinishGift"), iNeedTime);
                    }
                    else
                    {
                        mGiftTitle.text = string.Format(TR.Value("OnlineUnfinishGift"), iNeedTime);
                    }
                }
            }

            ItemdataList = ActiveManager.GetInstance().GetActiveAwards(activeData.akChildItems[iIndex].ID);

            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Welfare) || activeData.akChildItems[iIndex].status != (int)Protocol.TaskStatus.TASK_FINISHED)
            {
                UIGray gray = mBtReceive.GetComponent<UIGray>();
                gray.enabled = true;

                mBtReceive.interactable = false;
            }

            if(ItemdataList != null)
            {
                for (int i = 0; i < GiftRoot.Length; i++)
                {
                    if (i < ItemdataList.Count)
                    {
                        ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(ItemdataList[i].ID);
                        if (ItemDetailData == null)
                        {
                            continue;
                        }

                        GiftRoot[i].gameObject.SetActive(true);

                        ItemDetailData.Count = ItemdataList[i].Num;

                        ComItem comitem = CreateComItem(GiftRoot[i].gameObject);

                        int idx = i;
                        comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { OnShowItemDetailData(idx); });

                        //GiftText[i].text = ItemDetailData.Name;
                    }
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
        }

        void ClearData()
        {
            eGiftType = GiftType.None;
            ItemdataList = null;
        }

        void OnShowItemDetailData(int iIndex)
        {
            if(iIndex < 0 || iIndex >= ItemdataList.Count)
            {
                return;
            }

            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(ItemdataList[iIndex].ID);
            if (ItemDetailData == null)
            {
                return;
            }

            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }

        [UIControl("Back/GiftList/Panel/pos{0}", typeof(RectTransform), 1)]
        RectTransform[] GiftRoot = new RectTransform[iShowGiftNum];

        [UIControl("Back/GiftList/Panel/pos{0}/Text", typeof(Text), 1)]
        Text[] GiftText = new Text[iShowGiftNum];

        #region ExtraUIBind
        private Text mTitle = null;
        private Text mGiftTitle = null;
        private Image mGiftTitleBack = null;
        private Button mBtReceive = null;
        private Button mBtClose = null;
        private Text mWords = null;

        protected override void _bindExUI()
        {
            mTitle = mBind.GetCom<Text>("Title");
            mGiftTitle = mBind.GetCom<Text>("GiftTitle");
            mGiftTitleBack = mBind.GetCom<Image>("GiftTitleBack");
            mBtReceive = mBind.GetCom<Button>("BtReceive");
            mBtReceive.onClick.AddListener(_onBtReceiveButtonClick);
            mBtClose = mBind.GetCom<Button>("BtClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mWords = mBind.GetCom<Text>("Words");
        }

        protected override void _unbindExUI()
        {
            mTitle = null;
            mGiftTitle = null;
            mGiftTitleBack = null;
            mBtReceive.onClick.RemoveListener(_onBtReceiveButtonClick);
            mBtReceive = null;
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mWords = null;
        }
        #endregion

        #region Callback
        private void _onBtReceiveButtonClick()
        {
            if(eGiftType == GiftType.UplevelGift)
            {
                ActiveManager.GetInstance().OpenActiveFrame(9380, 4000);
            }
            else if(eGiftType == GiftType.OnLineGift)
            {
                ActiveManager.GetInstance().OpenActiveFrame(9380, 5000);
            }
           
            Close();
        }

        private void _onBtCloseButtonClick()
        {
            Close();
        }
        #endregion
    }
}
