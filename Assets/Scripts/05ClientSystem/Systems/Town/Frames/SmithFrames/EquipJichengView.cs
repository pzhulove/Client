using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using System;
using Protocol;
using Network;

namespace GameClient
{
    public class EquipJichengView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mEquipJichengUIListScript;
        [SerializeField] private EquipJichengItem mEquipJichengItemA;
        [SerializeField] private EquipJichengItem mEquipJichengItemB;
        [SerializeField] private ComMergeMoneyControl mComMergeMoneyControl;
        [SerializeField] private ComMergeMoneyControl mComMergeMoneyControl1;
        [SerializeField] private Button mBtnMergeCard;
        [SerializeField] private ComDropDownControl mEquipJichengQulityDrop;
        [SerializeField] private int iMoneyID = 600000001;
        [SerializeField] private int iMoneyID2 = 900000084;

        private EnchantmentsFunctionData dataMerge = new EnchantmentsFunctionData();
        private List<ItemData> mAllEquipJichengItems = new List<ItemData>();

        /// <summary>
        /// 装备继承品质列表
        /// </summary>
        private List<ComControlData> mEquipJichengQulityTabDataList = new List<ComControlData>();

        private int mCurrentSelectedEquipJichengSubType = 0;//当前选择的部位
        private int mCurrentSelectedEquipJichengQuality = 0;//当前选择的品质
        private int iDefaultEquipJichengQuality = 0;//默认品质

        private void Awake()
        {
            BindUIEvent();
            InitEquipJichengUIListScript();
            if (mBtnMergeCard != null)
            {
                mBtnMergeCard.onClick.RemoveAllListeners();
                mBtnMergeCard.onClick.AddListener(OnMergeCardClick);
            }
        }

        private void OnDestroy()
        {
            UnBindUIEvent();
            UnInitEquipJichengUIListScript();
            dataMerge = null;
            mAllEquipJichengItems.Clear();
            mCurrentSelectedEquipJichengSubType = 0;
            mCurrentSelectedEquipJichengQuality = 0;
        }

        public void InitView()
        {
            InitEquipJichengView();
        }
        
        public void InitEquipJichengView()
        {
            mEquipJichengItemA.InitEquipJichengItem(OnEquipJichengEmptyClick);
            mEquipJichengItemB.InitEquipJichengItem(OnEquipJichengEmptyClick);
            mComMergeMoneyControl.SetState(ComMergeMoneyControl.CMMC.CMMC_NOT_ENOUGH);
            mComMergeMoneyControl.SetCost(iMoneyID, 0);

            mComMergeMoneyControl1.SetState(ComMergeMoneyControl.CMMC.CMMC_NOT_ENOUGH);
            mComMergeMoneyControl1.SetCost(iMoneyID2, 0);

            mEquipJichengQulityTabDataList.Clear();
            mEquipJichengQulityTabDataList.Add(new ComControlData(0, 0, "全部品质", true));
            mEquipJichengQulityTabDataList.Add(new ComControlData(1, 1, "白色", false));
            mEquipJichengQulityTabDataList.Add(new ComControlData(2, 2, "蓝色", false));
            mEquipJichengQulityTabDataList.Add(new ComControlData(3, 3, "紫色", false));
            mEquipJichengQulityTabDataList.Add(new ComControlData(4, 4, "绿色", false));
            mEquipJichengQulityTabDataList.Add(new ComControlData(5, 5, "粉色", false));
            mEquipJichengQulityTabDataList.Add(new ComControlData(6, 6, "橙色", false));

            InitEquipJichengQulityDrop();

            LoadAllEquipJicheng();
        }

        private void InitEquipJichengQulityDrop()
        {
            if (mEquipJichengQulityTabDataList != null && mEquipJichengQulityTabDataList.Count > 0)
            {
                var equipJichengQulityTabData = mEquipJichengQulityTabDataList[0];
                for (int i = 0; i < mEquipJichengQulityTabDataList.Count; i++)
                {
                    if (iDefaultEquipJichengQuality == mEquipJichengQulityTabDataList[i].Id)
                    {
                        equipJichengQulityTabData = mEquipJichengQulityTabDataList[i];
                        break;
                    }
                }

                if (mEquipJichengQulityDrop != null)
                {
                    mEquipJichengQulityDrop.InitComDropDownControl(equipJichengQulityTabData, mEquipJichengQulityTabDataList, OnEquipJichengQulityDropDownItemClicked);
                }
            }
        }

        private void OnEquipJichengQulityDropDownItemClicked(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            //品质相同，直接返回
            if (iDefaultEquipJichengQuality == comControlData.Id)
                return;

            //赋值选中的品质
            iDefaultEquipJichengQuality = comControlData.Id;

            //根据选中的品质进行更新 
            LoadAllEquipJicheng();
        }

        #region BindUIEvent 

        private void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnEquipJichengSuccess, OnSlotItemsMergeChanged);
        }

        private void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnEquipJichengSuccess, OnSlotItemsMergeChanged);
        }

        private void OnSlotItemsMergeChanged(UIEvent uiEvent)
        {
            dataMerge.leftItem = null;
            dataMerge.rightItem = null;
            mCurrentSelectedEquipJichengSubType = 0;
            mCurrentSelectedEquipJichengQuality = 0;
            mEquipJichengItemA.Reset();
            mEquipJichengItemB.Reset();

            UpdateMoneyInfo();
            LoadAllEquipJicheng();
        }
        #endregion

        #region  BxyUIListScript

        private void InitEquipJichengUIListScript()
        {
            if (mEquipJichengUIListScript != null)
            {
                mEquipJichengUIListScript.Initialize();
                mEquipJichengUIListScript.onBindItem += OnBindItemDelegate;
                mEquipJichengUIListScript.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitEquipJichengUIListScript()
        {
            if (mEquipJichengUIListScript != null)
            {
                mEquipJichengUIListScript.onBindItem -= OnBindItemDelegate;
                mEquipJichengUIListScript.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private EquipJichengItemElement OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<EquipJichengItemElement>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var element = item.gameObjectBindScript as EquipJichengItemElement;
            if (element != null && item.m_index >= 0 && item.m_index < mAllEquipJichengItems.Count)
            {
                element.OnItemVisiable(mAllEquipJichengItems[item.m_index], mCurrentSelectedEquipJichengSubType, mCurrentSelectedEquipJichengQuality, UpdatePutEquipJichengInfo, dataMerge);
            }
        }

        public void LoadAllEquipJicheng()
        {
            if (mAllEquipJichengItems == null)
            {
                mAllEquipJichengItems = new List<ItemData>();
            }

            mAllEquipJichengItems.Clear();

            var itemIds = ItemDataManager.GetInstance().GetItemsByType(ProtoTable.ItemTable.eType.EQUIP);
            for (int i = 0; i < itemIds.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(itemIds[i]);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.SubType == (int)ProtoTable.ItemTable.eSubType.ST_BXY_EQUIP)
                {
                    continue;
                }

                if (itemData.PackageType == EPackageType.Storage)
                {
                    continue;
                }

                if (itemData.PackageType == EPackageType.RoleStorage)
                {
                    continue;
                }
                
                // 身上穿的装备过滤掉
                if (itemData.PackageType == EPackageType.WearEquip)
                {
                    continue;
                }

                if (iDefaultEquipJichengQuality != 0)
                {
                    if ((int)itemData.Quality != iDefaultEquipJichengQuality)
                        continue;
                }

                mAllEquipJichengItems.Add(itemData);
            }

            mAllEquipJichengItems.Sort(Sort);

            SetElementAmount();
        }

        private void SetElementAmount()
        {
            mEquipJichengUIListScript.SetElementAmount(mAllEquipJichengItems.Count);
        }

        public int Sort(ItemData left, ItemData right)
        {
            if (left.Quality != right.Quality)
            {
                return (int)right.Quality - (int)left.Quality;
            }

            return right.LevelLimit - left.LevelLimit;
        }
        #endregion

        private void UpdatePutEquipJichengInfo(ItemData itemData, EquipJichengItemElement element)
        {
            if (itemData == null)
            {
                return;
            }

            if (dataMerge.leftItem != null && dataMerge.rightItem != null)
            {
                SystemNotifyManager.SysNotifyTextAnimation("选择数量已达上限");
                return;
            }

            if (mCurrentSelectedEquipJichengSubType != 0)
            {
                //放入的装备部位与选中的装备部位不一致
                if (mCurrentSelectedEquipJichengSubType != (int)itemData.SubType)
                {
                    SystemNotifyManager.SysNotifyTextAnimation("该装备与已放入的装备部位不同，无法放入");
                    return;
                }
            }

            if (mCurrentSelectedEquipJichengQuality != 0)
            {
                //放入的装备部位与选中的装备品质不一致
                if (mCurrentSelectedEquipJichengQuality != (int)itemData.Quality)
                {
                    SystemNotifyManager.SysNotifyTextAnimation("该装备与已放入的装备品质不同，无法放入");
                    return;
                }
            }

            int allCount = itemData.Count;
            int count = 0;
            if (dataMerge.leftItem != null)
            {
                if (dataMerge.leftItem.GUID == itemData.GUID)
                {
                    count++;
                }
            }

            if (dataMerge.rightItem != null)
            {
                if (dataMerge.rightItem.GUID == itemData.GUID)
                {
                    count++;
                }
            }

            //如果同样的装备已全部放进去
            if (count >= allCount)
            {
                SystemNotifyManager.SysNotifyTextAnimation("放入失败，该装备已放入继承区");
                return;
            }

            mCurrentSelectedEquipJichengSubType = (int)itemData.SubType;

            mCurrentSelectedEquipJichengQuality = (int)itemData.Quality;

            bool isCardA = false;

            if (dataMerge.leftItem == null)
            {
                isCardA = true;
            }
           
            if (isCardA == true)
            {
                dataMerge.leftItem = itemData;
            }
            else
            {
                dataMerge.rightItem = itemData;
            }

            mEquipJichengItemA.UpdateEquipJichengItem(dataMerge.leftItem);
            mEquipJichengItemB.UpdateEquipJichengItem(dataMerge.rightItem);

            if (element != null)
            {
                element.SetCheckMaskRoot(true);
            }

            UpdateMoneyInfo();
            SetElementAmount();
        }

        private void UpdateMoneyInfo()
        {
            if (dataMerge.leftItem != null && dataMerge.rightItem != null)
            {
                mComMergeMoneyControl.SetState(ComMergeMoneyControl.CMMC.CMMC_ENOUGH);
                int count = GetMergeCardCost();
                mComMergeMoneyControl.SetCost(iMoneyID, count);

                mComMergeMoneyControl1.SetState(ComMergeMoneyControl.CMMC.CMMC_ENOUGH);
                int count1 = GetMergeCardCost1();
                mComMergeMoneyControl1.SetCost(iMoneyID2, count1);
            }
            else
            {
                mComMergeMoneyControl.SetState(ComMergeMoneyControl.CMMC.CMMC_NOT_ENOUGH);
                mComMergeMoneyControl1.SetState(ComMergeMoneyControl.CMMC.CMMC_NOT_ENOUGH);
            }
        }

        /// <summary>
        /// 清空槽位数据
        /// </summary>
        /// <param name="IsEquipJichengA"></param>
        private void OnEquipJichengEmptyClick(bool IsEquipJichengA)
        {
            if (IsEquipJichengA == true)
            {
                dataMerge.leftItem = null;
            }
            else
            {
                dataMerge.rightItem = null;
            }

            if (dataMerge.leftItem == null && dataMerge.rightItem == null)
            {
                mCurrentSelectedEquipJichengSubType = 0;
                mCurrentSelectedEquipJichengQuality = 0;
            }

            UpdateMoneyInfo();
            SetElementAmount();
        }

        private int GetMergeCardCost()
        {
            return 100000;
        }

        private int GetMergeCardCost1()
        {
            return 1;
        }

        #region ButtonClick

        bool m_bInMerge = false;
        private void OnMergeCardClick()
        {
            if (m_bInMerge)
            {
                return;
            }
            m_bInMerge = true;

            OnClickFunctionMerge();

            InvokeMethod.Invoke(this, 0.50f, () =>
            {
                m_bInMerge = false;
            });

        }

        private void OnClickFunctionMerge()
        {
            if (dataMerge != null)
            {
                //是否存在两个装备
                if (dataMerge.leftItem != null && dataMerge.rightItem != null)
                {
                    SystemNotifyManager.SystemNotify(10717, _ConfirmToJicheng);
                }
                else
                {
                    if (dataMerge.leftItem == null)
                    {
                        SystemNotifyManager.SystemNotify(4700012);
                    }
                    else
                    {
                        SystemNotifyManager.SystemNotify(4700013);
                    }
                }
            }
        }

        private void _ConfirmToJicheng()
        {
            List<CostItemManager.CostInfo> costInfos = new List<CostItemManager.CostInfo>();
            CostItemManager.CostInfo costInfo1 = new CostItemManager.CostInfo
            {
                nMoneyID = iMoneyID,
                nCount = GetMergeCardCost(),
            };
            CostItemManager.CostInfo costInfo2 = new CostItemManager.CostInfo
            {
                nMoneyID = iMoneyID2,
                nCount = GetMergeCardCost1(),
            };
            costInfos.Add(costInfo1);
            costInfos.Add(costInfo2);
            CostItemManager.GetInstance().TryCostMoneiesDefault(costInfos, OnConfirmJichengBindMethod);
        }

        //判断是否显示绑定方式不一致的提示
        private void OnConfirmJichengBindMethod()
        {
            //revert原来的合成方式，无论绑定OR非绑定，都统一合成绑定
            OnFinalSendJichengReq();
        }

        //最终进行合成附魔卡
        private void OnFinalSendJichengReq()
        {
            OnSendJichengReq();
        }

        //最终发送合成的消息
        private void OnSendJichengReq()
        {
            if (dataMerge == null || dataMerge.leftItem == null || dataMerge.rightItem == null) return;
            SendJicheng(dataMerge.leftItem.GUID, dataMerge.rightItem.GUID);  
            Global.jichengGuid = dataMerge.rightItem.GUID;
        }

        public void SendJicheng(ulong leftcardid, ulong rightcardid)
        {
            SceneMagicCardCompReq kCmd = new SceneMagicCardCompReq();
            var leftItem = ItemDataManager.GetInstance().GetItem(leftcardid);
            var rightItem = ItemDataManager.GetInstance().GetItem(rightcardid);
            if (leftItem != null && rightItem != null)
            {
                kCmd.cardA = leftItem.GUID;
                kCmd.cardB = rightItem.GUID;
                // 装备继承，flag=3
                kCmd.flag = 3;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kCmd);
            }
        }

        #endregion
    }
}