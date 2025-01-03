using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ProtoTable;
using Protocol;

namespace GameClient
{
    class LevelGiftFrame : ClientFrame
    {
        string GiftElementPath = "UIFlatten/Prefabs/MainFrameTown/LevelGiftElement";
        GameObject LevelGiftElement = null;
        const int ElementSum = 17;
        int FirstPosIndex = 0;//现在第一个元素是数组中的第几个元素
        int NowToggleIndex = 0;//当前的toggle用的是哪个
        bool isSort = true;

        List<ActiveManager.ActivityData> MyActivityList = new List<ActiveManager.ActivityData>();
        List<AwardItemData> ItemdataList = new List<AwardItemData>();
        ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(4000);
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MainFrameTown/LevelGift";
        }
        protected override void _OnOpenFrame()
        {
            initdata();
            ClearMyList();
            SortMyList();
            LogErrorData();
            _SelectToggle();
			_RegistenUIEvent ();
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;
        }

        
        protected override void _OnCloseFrame()
        {
            ClearData();
			_UnRegistenUIEvent();
            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;
        }

		private void _RegistenUIEvent()
		{
			UIEventSystem.GetInstance ().RegisterEventHandler (EUIEventID.LeftSlip, _LeftSlip);
			UIEventSystem.GetInstance ().RegisterEventHandler (EUIEventID.RightSlip, _RightSlip);
		}
		private void _UnRegistenUIEvent()
		{
			UIEventSystem.GetInstance ().UnRegisterEventHandler (EUIEventID.LeftSlip, _LeftSlip);
			UIEventSystem.GetInstance ().UnRegisterEventHandler (EUIEventID.RightSlip, _RightSlip);
		}
		private void _LeftSlip(UIEvent uiEvent)
		{
			_onPreviousButtonClick();
		}
		private void _RightSlip(UIEvent uiEvent)
		{
			_onNextButtonClick ();
		}
        private void initdata()
        {
            FirstPosIndex = 0;
            NowToggleIndex = 0;
            isSort = true;
        }

        private void ClearMyList()
        {
            MyActivityList.Clear();
            for (int i = 0; i < ElementSum; i++)
            {
                MyActivityList.Add(activeData.akChildItems[i]);
            }
        }

        //讲链表按等级限制从小到大排序
        private void SortMyList()
        {
            MyActivityList.Sort((x, y) => {
                int result;
                if (x.activeItem.LevelLimit.CompareTo(y.activeItem.LevelLimit) < 0)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
                return result;
            });
        }

        private void LogErrorData()
        {
            for(int i=0;i<MyActivityList.Count;i++)
            {
                if(MyActivityList[i].activeItem.LevelLimit <= PlayerBaseData.GetInstance().Level)
                {
                    if(MyActivityList[i].status == (int)TaskStatus.TASK_UNFINISH)
                    {
                        Logger.LogErrorFormat("1--- levelLimit = {0},peopleLevel = {1},activeItemID = {2}", 
                            MyActivityList[i].activeItem.LevelLimit, PlayerBaseData.GetInstance().Level, MyActivityList[i].activeItem.ID);
                    }
                }
                if(MyActivityList[i].activeItem.LevelLimit > PlayerBaseData.GetInstance().Level)
                {
                    if(MyActivityList[i].status != (int)TaskStatus.TASK_UNFINISH)
                    {
                        Logger.LogErrorFormat("2--- levelLimit = {0},peopleLevel = {1},activeState = {2},activeItemID = {3}",
                            MyActivityList[i].activeItem.LevelLimit, PlayerBaseData.GetInstance().Level, MyActivityList[i].status, MyActivityList[i].activeItem.ID);
                    }
                }
            }
        }

        //打开界面的时候通过等级和未领取的礼包判断进入在第几页
        private void _SelectToggle()
        {
            bool flag = false;
            for (int i = 0; i < ElementSum; i++)
            {
                if (MyActivityList[i].status == (int)TaskStatus.TASK_FINISHED)
                {
                    _updateElementData((int)(i / 4) * 4);
                    toggle[(int)(i / 4)].isOn = true;
                    flag = true;
                    break;
                }
            }
            if (flag == false)
            {
                for (int i = 0; i < ElementSum; i++)
                {
                    if (MyActivityList[i].activeItem.LevelLimit > PlayerBaseData.GetInstance().Level)
                    {
                        _updateElementData((int)(i / 4) * 4);
                        toggle[(int)(i / 4)].isOn = true;
                        break;
                    }
                }
            }
        }

        //让活动刷新的回调
        private void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            ClearMyList();
            SortMyList();
            LogErrorData();
            _updateBtStatus();
        }
        private void ClearData()
        {
            if(ItemdataList != null)
            {
                ItemdataList.Clear();
            }
            if(MyActivityList != null)
            {
                MyActivityList.Clear();
            }
        }

        private void _updateBtStatus()
        {
            for(int i= FirstPosIndex;i<FirstPosIndex+4;i++)
            {
                if (MyActivityList[i].status == (int)TaskStatus.TASK_OVER)
                {

                    comlevelele[i - FirstPosIndex].Accomplish.CustomActive(true);
                    comlevelele[i - FirstPosIndex].ReceiveGray.enabled = true;
                    comlevelele[i - FirstPosIndex].Uncomplete.CustomActive(false);
                    comlevelele[i - FirstPosIndex].ReceiveText.text = "已领取";
                }
                else if(MyActivityList[i].status == (int)TaskStatus.TASK_FINISHED)
                {
                    comlevelele[i - FirstPosIndex].Accomplish.CustomActive(false);
                    comlevelele[i - FirstPosIndex].ReceiveGray.enabled = false;
                    comlevelele[i - FirstPosIndex].Uncomplete.CustomActive(false);
                    comlevelele[i - FirstPosIndex].ReceiveText.text = "可领取";
                }
                else if(MyActivityList[i].status == (int)TaskStatus.TASK_UNFINISH)
                {
                    comlevelele[i - FirstPosIndex].Accomplish.CustomActive(false);
                    comlevelele[i - FirstPosIndex].ReceiveGray.enabled = true;
                    comlevelele[i - FirstPosIndex].Uncomplete.CustomActive(true);
                }
            }
            
        }
        
        private void _updateElementData(int index)
        {
            //一定至少加载四个且不会越界
            if (index < 0)
            {
                index = 0;
            }
            if (index > 13)
            {
                index = 13;
            }
            FirstPosIndex = index;
            for (int i = 0; i < 4; i++)
            {
                int index_i = FirstPosIndex + i;
                int tableID = MyActivityList[index_i].ID;
                ItemdataList = ActiveManager.GetInstance().GetActiveAwards(MyActivityList[FirstPosIndex + i].ID);
                if(ItemdataList == null)
                {
                    Logger.LogErrorFormat("ItemdataList is null");
                    return;
                }
                ComItem comitem = null;
                //ComItem.Destroy(comitem);
                _updateBtStatus();
                comlevelele[i].LevelLimit.text = string.Format("{0}级可领取", MyActivityList[FirstPosIndex + i].activeItem.LevelLimit);
                comlevelele[i].LevelTitle.text = string.Format("{0}级礼包", MyActivityList[FirstPosIndex + i].activeItem.LevelLimit);
                comlevelele[i].icon0.gameObject.CustomActive(false);
                comlevelele[i].text0.gameObject.CustomActive(false);
                comlevelele[i].icon1.gameObject.CustomActive(false);
                comlevelele[i].text1.gameObject.CustomActive(false);
                comlevelele[i].icon2.gameObject.CustomActive(false);
                comlevelele[i].text2.gameObject.CustomActive(false);
                for (int j = 0; j < ItemdataList.Count; j++)
                {
                    ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(ItemdataList[j].ID);
                    if (null == ItemDetailData)
                    {
                        Logger.LogErrorFormat("ItemData is null");
                        return;
                    }
                    ItemDetailData.Count = ItemdataList[j].Num;
                    ItemDetailData.EquipType = (EEquipType)ItemdataList[j].EquipType;
                    ItemDetailData.StrengthenLevel = ItemdataList[j].StrengthenLevel;
                    
                    if (j == 0)
                    {
                        comitem = comlevelele[i].icon0.gameObject.GetComponentInChildren<ComItem>();
                        if (comitem == null)
                        {
                            comitem = CreateComItem(comlevelele[i].icon0.gameObject);
                        }
                        
                        comlevelele[i].icon0.gameObject.CustomActive(true);
                        comlevelele[i].text0.text = ItemDetailData.Name;
                        comlevelele[i].text0.gameObject.CustomActive(true);
                    }
                    else if (j == 1)
                    {
                        comitem = comlevelele[i].icon1.gameObject.GetComponentInChildren<ComItem>();
                        if (comitem == null)
                        {
                            comitem = CreateComItem(comlevelele[i].icon1.gameObject);
                        }
                        comlevelele[i].icon1.gameObject.CustomActive(true);
                        comlevelele[i].text1.text = ItemDetailData.Name;
                        comlevelele[i].text1.gameObject.CustomActive(true);
                    }
                    else if (j == 2)
                    {
                        comitem = comlevelele[i].icon2.gameObject.GetComponentInChildren<ComItem>();
                        if (comitem == null)
                        {
                            comitem = CreateComItem(comlevelele[i].icon2.gameObject);
                        }
                        comlevelele[i].icon2.gameObject.CustomActive(true);
                        comlevelele[i].text2.text = ItemDetailData.Name;
                        comlevelele[i].text2.gameObject.CustomActive(true);
                    }
                    if (comitem == null)
                    {
                        Logger.LogErrorFormat("comitem is null");
                        return;
                    }
                    int index_j = j;
                    comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { OnShowItemDetailData(index_i, index_j); });

                    //特殊处理修改等级的字体
                    comitem.labLevel.fontSize = 30;
                }
                comlevelele[i].Receive.onClick.RemoveAllListeners();
                comlevelele[i].Receive.onClick.AddListener(() =>
                {
                    ActiveManager.GetInstance().SendSubmitActivity(tableID);
                });
            }
        }
        void OnShowItemDetailData(int iIndex,int jIndex)
        {
            if (iIndex < 0 || iIndex >= ElementSum)
            {
                Logger.LogErrorFormat("iIndex out of range");
                return;
            }
            ItemdataList = ActiveManager.GetInstance().GetActiveAwards(MyActivityList[iIndex].ID);
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(ItemdataList[jIndex].ID);
            if (ItemDetailData == null)
            {
                Logger.LogErrorFormat("ItemDerailData is null");
                return;
            }

            ItemDetailData.Count = ItemdataList[jIndex].Num;
            ItemDetailData.EquipType = (EEquipType)ItemdataList[jIndex].EquipType;
            ItemDetailData.StrengthenLevel = ItemdataList[jIndex].StrengthenLevel;

            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }

        [UIControl("Middle/pos{0}", typeof(RectTransform), 0)]
        RectTransform[] ElementPos = new RectTransform[4];
        [UIControl("Middle/pos{0}/LevelGiftElement", typeof(ComLevelGiftEle), 0)]
        ComLevelGiftEle[] comlevelele = new ComLevelGiftEle[4];
        [UIControl("Middle/tabs/Toggle{0}", typeof(Toggle), 0)]
        Toggle[] toggle = new Toggle[5];

        #region ExtraUIBind
        private Button mCloseframe = null;
        private GameObject mContent = null;
        private GameObject mpos0 = null;
        private GameObject mpos1 = null;
        private GameObject mpos2 = null;
        private GameObject mpos3 = null;
        private Button mNext = null;
        private Button mPrevious = null;
        private Toggle mToggle0 = null;
        private Toggle mToggle1 = null;
        private Toggle mToggle2 = null;
        private Toggle mToggle3 = null;
        private Toggle mToggle4 = null;

        protected override void _bindExUI()
        {
            mCloseframe = mBind.GetCom<Button>("closeframe");
            mCloseframe.onClick.AddListener(_onCloseframeButtonClick);
            mContent = mBind.GetGameObject("Content");
            mpos0 = mBind.GetGameObject("pos0");
            mpos1 = mBind.GetGameObject("pos1");
            mpos2 = mBind.GetGameObject("pos2");
            mpos3 = mBind.GetGameObject("pos3");
            mNext = mBind.GetCom<Button>("next");
            mNext.onClick.AddListener(_onNextButtonClick);
            mPrevious = mBind.GetCom<Button>("previous");
            mPrevious.onClick.AddListener(_onPreviousButtonClick);
            mToggle0 = mBind.GetCom<Toggle>("toggle0");
            mToggle0.onValueChanged.AddListener(_onToggle0ToggleValueChange);
            mToggle1 = mBind.GetCom<Toggle>("toggle1");
            mToggle1.onValueChanged.AddListener(_onToggle1ToggleValueChange);
            mToggle2 = mBind.GetCom<Toggle>("toggle2");
            mToggle2.onValueChanged.AddListener(_onToggle2ToggleValueChange);
            mToggle3 = mBind.GetCom<Toggle>("toggle3");
            mToggle3.onValueChanged.AddListener(_onToggle3ToggleValueChange);
            mToggle4 = mBind.GetCom<Toggle>("toggle4");
            mToggle4.onValueChanged.AddListener(_onToggle4ToggleValueChange);
        }

        protected override void _unbindExUI()
        {
            mCloseframe.onClick.RemoveListener(_onCloseframeButtonClick);
            mCloseframe = null;
            mContent = null;
            mpos0 = null;
            mpos1 = null;
            mpos2 = null;
            mpos3 = null;
            mNext.onClick.RemoveListener(_onNextButtonClick);
            mNext = null;
            mPrevious.onClick.RemoveListener(_onPreviousButtonClick);
            mPrevious = null;
            mToggle0.onValueChanged.RemoveListener(_onToggle0ToggleValueChange);
            mToggle0 = null;
            mToggle1.onValueChanged.RemoveListener(_onToggle1ToggleValueChange);
            mToggle1 = null;
            mToggle2.onValueChanged.RemoveListener(_onToggle2ToggleValueChange);
            mToggle2 = null;
            mToggle3.onValueChanged.RemoveListener(_onToggle3ToggleValueChange);
            mToggle3 = null;
            mToggle4.onValueChanged.RemoveListener(_onToggle4ToggleValueChange);
            mToggle4 = null;
        }
        #endregion

        #region Callback
        private void _onCloseframeButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        private void _onNextButtonClick()
        {
            /* put your code in here */
            //_updateElementData(FirstPosIndex + 4);
            if (NowToggleIndex + 1 < 5)
            {
                ++NowToggleIndex;
                toggle[NowToggleIndex].isOn = true;
            }

        }
        private void _onPreviousButtonClick()
        {
            /* put your code in here */
            //_updateElementData(FirstPosIndex - 4);
            if (NowToggleIndex - 1 >= 0)
            {
                --NowToggleIndex;
                toggle[NowToggleIndex].isOn = true;
            }

        }

        private void _onToggle0ToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed)
            {
                isSort = true;
                _updateElementData(0);
                NowToggleIndex = 0;
            }
        }
        private void _onToggle1ToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed)
            {
                if (isSort)
                {
                    _updateElementData(4);
                }
                else
                {
                    _updateElementData(1);
                }
                NowToggleIndex = 1;
            }

        }
        private void _onToggle2ToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed)
            {
                if (isSort)
                {
                    _updateElementData(8);
                }
                else
                {
                    _updateElementData(5);
                }
                NowToggleIndex = 2;
            }
        }
        private void _onToggle3ToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed)
            {
                if (isSort)
                {
                    _updateElementData(12);
                }
                else
                {
                    _updateElementData(9);
                }
                NowToggleIndex = 3;
            }
        }
        private void _onToggle4ToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed)
            {
                isSort = false;
                _updateElementData(16);
                NowToggleIndex = 4;
            }
        }
        #endregion
    }
}
