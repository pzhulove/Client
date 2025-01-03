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
    public class SinanView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mBxyUIListScript;
        [SerializeField] private ComUIListScript mSinanUIListScript;
        [SerializeField] private SinanItem mSinanItemA;
        [SerializeField] private BxyItem mBxyItemB;
        [SerializeField] private Button mBtnMergeCard;
        [SerializeField] private GameObject mMergeDesc;
        [SerializeField] private ComDropDownControl mBxyQulityDrop;
        [SerializeField] private ComDropDownControl mSinanQulityDrop;

        private EnchantmentsFunctionData dataMerge = new EnchantmentsFunctionData();
        private List<ItemData> mAllBxyItems = new List<ItemData>();

        private List<ItemData> mAllSinanItems = new List<ItemData>();

        /// <summary>
        /// 辟邪玉品质列表
        /// </summary>
        private List<ComControlData> mBxyQulityTabDataList = new List<ComControlData>();

        /// <summary>
        /// 司南品质列表
        /// </summary>
        private List<ComControlData> mSinanQulityTabDataList = new List<ComControlData>();

        private int mCurrentSelectedBxyQuality = 0;//当前选择的品质
        private int iDefaultBxyQuality = 0;//默认品质

        private int mCurrentSelectedSinanQuality = 0;//当前选择的品质
        private int iDefaultSinanQuality = 0;//默认品质

        private void Awake()
        {
            BindUIEvent();
            InitBxyUIListScript();
            InitSinanUIListScript();

            if (mBtnMergeCard != null)
            {
                mBtnMergeCard.onClick.RemoveAllListeners();
                mBtnMergeCard.onClick.AddListener(OnMergeCardClick);
            }
        }

        private void OnDestroy()
        {
            UnBindUIEvent();
            UnInitBxyUIListScript();
            UnInitSinanUIListScript();
            dataMerge = null;
            mAllBxyItems.Clear();
            mAllSinanItems.Clear();
            mCurrentSelectedBxyQuality = 0;
            mCurrentSelectedSinanQuality = 0;

        }

        public void InitView()
        {
            InitBxyMergeView();
        }
        
        public void InitBxyMergeView()
        {
            mSinanItemA.InitSinanItem(OnSinanEmptyClick);
            mBxyItemB.InitBxyItem(OnBxyEmptyClick);

            mBxyQulityTabDataList.Clear();
            mBxyQulityTabDataList.Add(new ComControlData(0, 0, "全部品质", true));
            mBxyQulityTabDataList.Add(new ComControlData(3, 3, "普通", false));
            mBxyQulityTabDataList.Add(new ComControlData(4, 4, "稀有", false));
            mBxyQulityTabDataList.Add(new ComControlData(5, 5, "传说", false));
            mBxyQulityTabDataList.Add(new ComControlData(6, 6, "史诗", false));

            mSinanQulityTabDataList.Clear();
            mSinanQulityTabDataList.Add(new ComControlData(0, 0, "全部品阶", true));
            mSinanQulityTabDataList.Add(new ComControlData(3, 3, "1-3阶", false));
            mSinanQulityTabDataList.Add(new ComControlData(4, 4, "4-6阶", false));
            mSinanQulityTabDataList.Add(new ComControlData(5, 5, "7-9阶", false));
            mSinanQulityTabDataList.Add(new ComControlData(6, 6, "10-12阶", false));
            

            InitBxyQulityDrop();
            InitSinanQulityDrop();


            LoadAllBxy();
            LoadAllSinan();
        }

        private void InitBxyQulityDrop()
        {
            if (mBxyQulityTabDataList != null && mBxyQulityTabDataList.Count > 0)
            {
                var bxyQulityTabData = mBxyQulityTabDataList[0];
                for (int i = 0; i < mBxyQulityTabDataList.Count; i++)
                {
                    if (iDefaultBxyQuality == mBxyQulityTabDataList[i].Id)
                    {
                        bxyQulityTabData = mBxyQulityTabDataList[i];
                        break;
                    }
                }

                if (mBxyQulityDrop != null)
                {
                    mBxyQulityDrop.InitComDropDownControl(bxyQulityTabData, mBxyQulityTabDataList, OnBxyQulityDropDownItemClicked);
                }
            }
        }

        private void InitSinanQulityDrop()
        {
            if (mSinanQulityTabDataList != null && mSinanQulityTabDataList.Count > 0)
            {
                var sinanQulityTabData = mSinanQulityTabDataList[0];
                for (int i = 0; i < mSinanQulityTabDataList.Count; i++)
                {
                    if (iDefaultSinanQuality == mSinanQulityTabDataList[i].Id)
                    {
                        sinanQulityTabData = mSinanQulityTabDataList[i];
                        break;
                    }
                }

                if (mSinanQulityDrop != null)
                {
                    mSinanQulityDrop.InitComDropDownControl(sinanQulityTabData, mSinanQulityTabDataList, OnSinanQulityDropDownItemClicked);
                }
            }
        }

        private void OnBxyQulityDropDownItemClicked(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            //品质相同，直接返回
            if (iDefaultBxyQuality == comControlData.Id)
                return;

            //赋值选中的品质
            iDefaultBxyQuality = comControlData.Id;

            //根据选中的品质进行更新 
            LoadAllBxy();
        }

        private void OnSinanQulityDropDownItemClicked(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            //品质相同，直接返回
            if (iDefaultSinanQuality == comControlData.Id)
                return;

            //赋值选中的品质
            iDefaultSinanQuality = comControlData.Id;

            //根据选中的品质进行更新 
            LoadAllSinan();
        }

        #region BindUIEvent 

        private void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSinanSuccess, OnSlotItemsMergeChanged);
        }

        private void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSinanSuccess, OnSlotItemsMergeChanged);
        }

        private void OnSlotItemsMergeChanged(UIEvent uiEvent)
        {
            dataMerge.leftItem = null;
            dataMerge.rightItem = null;
            mCurrentSelectedBxyQuality = 0;
            mCurrentSelectedSinanQuality = 0;
            mMergeDesc.CustomActive(true);
            mSinanItemA.Reset();
            mBxyItemB.Reset();

            LoadAllBxy();
            LoadAllSinan();
        }
        #endregion

        #region  BxyUIListScript

        private void InitBxyUIListScript()
        {
            if (mBxyUIListScript != null)
            {
                mBxyUIListScript.Initialize();
                mBxyUIListScript.onBindItem += OnBindItemDelegate;
                mBxyUIListScript.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void InitSinanUIListScript()
        {
            if (mSinanUIListScript != null)
            {
                mSinanUIListScript.Initialize();
                mSinanUIListScript.onBindItem += OnBindItemDelegate1;
                mSinanUIListScript.onItemVisiable += OnItemVisiableDelegate1;
            }
        }

        private void UnInitBxyUIListScript()
        {
            if (mBxyUIListScript != null)
            {
                mBxyUIListScript.onBindItem -= OnBindItemDelegate;
                mBxyUIListScript.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private void UnInitSinanUIListScript()
        {
            if (mSinanUIListScript != null)
            {
                mSinanUIListScript.onBindItem -= OnBindItemDelegate1;
                mSinanUIListScript.onItemVisiable -= OnItemVisiableDelegate1;
            }
        }

        private BxyMergeItemElement OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<BxyMergeItemElement>();
        }

        private SinanItemElement OnBindItemDelegate1(GameObject itemObject)
        {
            return itemObject.GetComponent<SinanItemElement>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var element = item.gameObjectBindScript as BxyMergeItemElement;
            if (element != null && item.m_index >= 0 && item.m_index < mAllBxyItems.Count)
            {
                element.OnItemVisiable(mAllBxyItems[item.m_index], mCurrentSelectedBxyQuality, UpdatePutBxyInfo, dataMerge);
            }
        }

        private void OnItemVisiableDelegate1(ComUIListElementScript item)
        {
            var element = item.gameObjectBindScript as SinanItemElement;
            if (element != null && item.m_index >= 0 && item.m_index < mAllSinanItems.Count)
            {
                element.OnItemVisiable(mAllSinanItems[item.m_index], mCurrentSelectedSinanQuality, UpdatePutSinanInfo, dataMerge);
            }
        }

        public void LoadAllBxy()
        {
            if (mAllBxyItems == null)
            {
                mAllBxyItems = new List<ItemData>();
            }

            mAllBxyItems.Clear();

            var itemIds = ItemDataManager.GetInstance().GetItemsByType(ProtoTable.ItemTable.eType.EQUIP);
            for (int i = 0; i < itemIds.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(itemIds[i]);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.SubType != (int)ProtoTable.ItemTable.eSubType.ST_BXY_EQUIP)
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

                if (iDefaultBxyQuality != 0)
                {
                    if ((int)itemData.Quality != iDefaultBxyQuality)
                        continue;
                }

                mAllBxyItems.Add(itemData);
            }

            mAllBxyItems.Sort(Sort);

            SetElementAmount();
        }

        public void LoadAllSinan()
        {
            if (mAllSinanItems == null)
            {
                mAllSinanItems = new List<ItemData>();
            }

            mAllSinanItems.Clear();

            var itemIds = ItemDataManager.GetInstance().GetItemsByType(ProtoTable.ItemTable.eType.MATERIAL);
            for (int i = 0; i < itemIds.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(itemIds[i]);
                if (itemData == null)
                {
                    continue;
                }
                if (itemData.SubType != (int)ProtoTable.ItemTable.eSubType.ST_SINAN)
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

                if (iDefaultSinanQuality != 0)
                {
                    if ((int)itemData.Quality != iDefaultSinanQuality)
                        continue;
                }

                mAllSinanItems.Add(itemData);
            }

            mAllSinanItems.Sort(Sort);

            SetElementAmount1();
        }

        private void SetElementAmount()
        {
            mBxyUIListScript.SetElementAmount(mAllBxyItems.Count);
        }

        private void SetElementAmount1()
        {
            mSinanUIListScript.SetElementAmount(mAllSinanItems.Count);
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

        private void UpdatePutBxyInfo(ItemData itemData, BxyMergeItemElement element)
        {
            if (itemData == null)
            {
                return;
            }

            if (dataMerge.rightItem != null)
            {
                SystemNotifyManager.SysNotifyTextAnimation("选择数量已达上限");
                return;
            }

            int allCount = itemData.Count;
            int count = 0;
            if (dataMerge.rightItem != null)
            {
                if (dataMerge.rightItem.GUID == itemData.GUID)
                {
                    count++;
                }
            }

            //如果同样的辟邪玉已全部放进去
            if (count >= allCount)
            {
                SystemNotifyManager.SysNotifyTextAnimation("放入失败，该辟邪玉已放入融合区");
                return;
            }

            mCurrentSelectedBxyQuality = (int)itemData.Quality;
            dataMerge.rightItem = itemData;

            mBxyItemB.UpdateBxyItem(dataMerge.rightItem);

            if (element != null)
            {
                element.SetCheckMaskRoot(true);
            }

            if (dataMerge.leftItem != null)
            {
                mMergeDesc.CustomActive(false);
            }
            else
            {
                mMergeDesc.CustomActive(true);
            }

            SetElementAmount();
        }

        private void UpdatePutSinanInfo(ItemData itemData, SinanItemElement element)
        {
            if (itemData == null)
            {
                return;
            }

            if (dataMerge.leftItem != null)
            {
                SystemNotifyManager.SysNotifyTextAnimation("选择数量已达上限");
                return;
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

            //如果同样的司南已全部放进去
            if (count >= allCount)
            {
                SystemNotifyManager.SysNotifyTextAnimation("放入失败，该司南已放入融合区");
                return;
            }

            mCurrentSelectedSinanQuality = (int)itemData.Quality;
            dataMerge.leftItem = itemData;

            mSinanItemA.UpdateSinanItem(dataMerge.leftItem);

            if (element != null)
            {
                element.SetCheckMaskRoot(true);
            }

            if (dataMerge.leftItem != null)
            {
                mMergeDesc.CustomActive(false);
            }
            else
            {
                mMergeDesc.CustomActive(true);
            }

            SetElementAmount1();
        }

        /// <summary>
        /// 清空槽位数据
        /// </summary>
        /// <param name="isBxyA"></param>
        private void OnBxyEmptyClick(bool isBxyA)
        {
            dataMerge.rightItem = null;

            if (dataMerge.rightItem == null)
            {
                mCurrentSelectedBxyQuality = 0;
            }

            SetElementAmount();
        }

        /// <summary>
        /// 清空槽位数据
        /// </summary>
        /// <param name="isBxyA"></param>
        private void OnSinanEmptyClick()
        {
            dataMerge.leftItem = null;

            mMergeDesc.CustomActive(true);

            if (dataMerge.leftItem == null)
            {
                mCurrentSelectedSinanQuality = 0;
            }

            SetElementAmount1();
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

            OnClickFunctionRonghe();

            InvokeMethod.Invoke(this, 0.50f, () =>
            {
                m_bInMerge = false;
            });

        }

        private void OnClickFunctionRonghe()
        {
            if (dataMerge != null)
            {
                //是否存在司南和辟邪玉
                if (dataMerge.leftItem != null && dataMerge.rightItem != null)
                {
                    //辟邪玉品质为橙色，进行提示
                    if (dataMerge.rightItem.Quality == ItemTable.eColor.YELLOW)
                    {
                        SystemNotifyManager.SystemNotify(10716, _ConfirmToRonghe);
                    }
                    //品质为粉色，进行提示
                    else if (dataMerge.rightItem.Quality == ItemTable.eColor.PINK)
                    {
                        SystemNotifyManager.SystemNotify(10715, _ConfirmToRonghe);
                    }
                    else
                    {
                         _ConfirmToRonghe();
                    }
                }
                else
                {
                    if (dataMerge.leftItem == null)
                    {
                        SystemNotifyManager.SystemNotify(4700010);
                    }
                    else
                    {
                        SystemNotifyManager.SystemNotify(4700011);
                    }
                }
            }
        }

        private void _ConfirmToRonghe()
        {
            List<CostItemManager.CostInfo> costInfos = new List<CostItemManager.CostInfo>();
            CostItemManager.GetInstance().TryCostMoneiesDefault(costInfos, OnConfirmBindMethod);
        }

        //判断是否显示绑定方式不一致的提示
        private void OnConfirmBindMethod()
        {
            //revert原来的合成方式，无论绑定OR非绑定，都统一合成绑定
            OnFinalSendRongheReq();
        }

        //最终进行合成附魔卡
        private void OnFinalSendRongheReq()
        {
            //检测背包是否为满
            if (PackageUtility.IsPackageFullByType(EPackageType.Sinan) == true)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("bxy_merge_package_is_full"));
                return;
            }

            OnSendBxyMergeReq();
        }

        //最终发送合成的消息
        private void OnSendBxyMergeReq()
        {
            if (dataMerge == null || dataMerge.leftItem == null || dataMerge.rightItem == null) return;

            Global.sinanRongheGuid = dataMerge.leftItem.GUID;
            SendMergeBxy(dataMerge.leftItem.GUID, dataMerge.rightItem.GUID);  
        }

        public void SendMergeBxy(ulong leftcardid, ulong rightcardid)
        {
            SceneMagicCardCompReq kCmd = new SceneMagicCardCompReq();
            var leftItem = ItemDataManager.GetInstance().GetItem(leftcardid);
            var rightItem = ItemDataManager.GetInstance().GetItem(rightcardid);
            if (leftItem != null && rightItem != null)
            {
                kCmd.cardA = leftItem.GUID;
                kCmd.cardB = rightItem.GUID;
                // 司南融合，flag=2
                kCmd.flag = 2;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kCmd);
            }
        }

        private void OnMagicCardMergeLevelTipSetting(bool value)
        {
            MagicCardMergeDataManager.GetInstance().IsNotShowMagicCardMergeLevelTip = value;
        }

        #endregion
    }
}