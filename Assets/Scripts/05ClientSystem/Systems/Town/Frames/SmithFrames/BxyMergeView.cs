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
    public class BxyMergeView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mBxyUIListScript;
        [SerializeField] private ComUIListScript mLowProbabilityUIListScript;
        [SerializeField] private ComUIListScript mHighProbabilityUIListScript;
        [SerializeField] private ComUIListScript m100ProbabilityUIListScript;
        [SerializeField] private BxyItem mBxyItemA;
        [SerializeField] private BxyItem mBxyItemB;
        [SerializeField] private ComMergeMoneyControl mComMergeMoneyControl;
        [SerializeField] private ComMergeMoneyControl mComMergeMoneyControl1;
        [SerializeField] private Button mBtnMergeCard;
        [SerializeField] private GameObject mMergePreviewRoot;
        [SerializeField] private GameObject mMergeDesc;
        [SerializeField] private GameObject mLowRoot;
        [SerializeField] private GameObject mHighRoot;
        [SerializeField] private GameObject m100Root;
        [SerializeField] private ComDropDownControl mBxyQulityDrop;
        [SerializeField] private int iMoneyID = 600000001;
        [SerializeField] private int iMoneyID2 = 910000024;

        private EnchantmentsFunctionData dataMerge = new EnchantmentsFunctionData();
        private List<ItemData> mAllBxyItems = new List<ItemData>();
        /// <summary>
        /// 低概率合成辟邪玉列表
        /// </summary>
        private List<ItemData> mLowProbabilityBxyItemList = new List<ItemData>();
        /// <summary>
        /// 高概率合成辟邪玉列表
        /// </summary>
        private List<ItemData> mHighProbabilityBxyItemList = new List<ItemData>();
        /// <summary>
        /// 必定合成辟邪玉列表
        /// </summary>
        private List<ItemData> m100ProbabilityBxyItemList = new List<ItemData>();

        /// <summary>
        /// 辟邪玉品质列表
        /// </summary>
        private List<ComControlData> mBxyQulityTabDataList = new List<ComControlData>();

        private int mCurrentSelectedBxyQuality = 0;//当前选择的品质
        private int iDefaultBxyQuality = 0;//默认品质

        private void Awake()
        {
            BindUIEvent();
            InitBxyUIListScript();
            InitLowProbabilityUIListScript();
            InitHighProbabilityUIListScript();
            Init100ProbabilityUIListScript();

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
            UnInitLowProbabilityUIListScript();
            UnInitHighProbabilityUIListScript();
            UnInit100ProbabilityUIListScript();
            dataMerge = null;
            mAllBxyItems.Clear();
            mCurrentSelectedBxyQuality = 0;

            if (mHighProbabilityBxyItemList != null)
                mHighProbabilityBxyItemList.Clear();

            if (mLowProbabilityBxyItemList != null)
                mLowProbabilityBxyItemList.Clear();

            if (m100ProbabilityBxyItemList != null)
                m100ProbabilityBxyItemList.Clear();
        }

        public void InitView()
        {
            InitBxyMergeView();
        }
        
        public void InitBxyMergeView()
        {
            mBxyItemA.InitBxyItem(OnBxyEmptyClick);
            mBxyItemB.InitBxyItem(OnBxyEmptyClick);
            mComMergeMoneyControl.SetState(ComMergeMoneyControl.CMMC.CMMC_NOT_ENOUGH);
            mComMergeMoneyControl.SetCost(iMoneyID, 0);

            mComMergeMoneyControl1.SetState(ComMergeMoneyControl.CMMC.CMMC_NOT_ENOUGH);
            mComMergeMoneyControl1.SetCost(iMoneyID2, 0);

            mBxyQulityTabDataList.Clear();
            mBxyQulityTabDataList.Add(new ComControlData(0, 0, "全部品质", true));
            mBxyQulityTabDataList.Add(new ComControlData(3, 3, "普通", false));
            mBxyQulityTabDataList.Add(new ComControlData(4, 4, "稀有", false));
            mBxyQulityTabDataList.Add(new ComControlData(5, 5, "传说", false));
            mBxyQulityTabDataList.Add(new ComControlData(6, 6, "史诗", false));

            InitBxyQulityDrop();

            LoadAllBxy();
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

        #region BindUIEvent 

        private void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnBxyMergeSuccess, OnSlotItemsMergeChanged);
        }

        private void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnBxyMergeSuccess, OnSlotItemsMergeChanged);
        }

        private void OnSlotItemsMergeChanged(UIEvent uiEvent)
        {
            dataMerge.leftItem = null;
            dataMerge.rightItem = null;
            mCurrentSelectedBxyQuality = 0;
            mMergePreviewRoot.CustomActive(false);
            mMergeDesc.CustomActive(true);
            mBxyItemA.Reset();
            mBxyItemB.Reset();

            UpdateMoneyInfo();
            LoadAllBxy();
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

        private void UnInitBxyUIListScript()
        {
            if (mBxyUIListScript != null)
            {
                mBxyUIListScript.onBindItem -= OnBindItemDelegate;
                mBxyUIListScript.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private BxyMergeItemElement OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<BxyMergeItemElement>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var element = item.gameObjectBindScript as BxyMergeItemElement;
            if (element != null && item.m_index >= 0 && item.m_index < mAllBxyItems.Count)
            {
                element.OnItemVisiable(mAllBxyItems[item.m_index], mCurrentSelectedBxyQuality, UpdatePutBxyInfo, dataMerge);
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
                
                // 身上穿的装备过滤掉
                if (itemData.PackageType == EPackageType.WearEquip)
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

        private void SetElementAmount()
        {
            mBxyUIListScript.SetElementAmount(mAllBxyItems.Count);
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

        #region LowProbabilityUIListScript

        private void InitLowProbabilityUIListScript()
        {
            if(mLowProbabilityUIListScript != null)
            {
                mLowProbabilityUIListScript.Initialize();
                mLowProbabilityUIListScript.onBindItem += OnBindLowProbabilityItem;
                mLowProbabilityUIListScript.onItemVisiable += OnLowProbabilityItemVisiable;
            }
        }

        private void Init100ProbabilityUIListScript()
        {
            if(m100ProbabilityUIListScript != null)
            {
                m100ProbabilityUIListScript.Initialize();
                m100ProbabilityUIListScript.onBindItem += OnBind100ProbabilityItem;
                m100ProbabilityUIListScript.onItemVisiable += On100ProbabilityItemVisiable;
            }
        }

        private void UnInitLowProbabilityUIListScript()
        {
            if (mLowProbabilityUIListScript != null)
            {
                mLowProbabilityUIListScript.onBindItem -= OnBindLowProbabilityItem;
                mLowProbabilityUIListScript.onItemVisiable -= OnLowProbabilityItemVisiable;
            }
        }

        private void UnInit100ProbabilityUIListScript()
        {
            if (m100ProbabilityUIListScript != null)
            {
                m100ProbabilityUIListScript.onBindItem -= OnBind100ProbabilityItem;
                m100ProbabilityUIListScript.onItemVisiable -= On100ProbabilityItemVisiable;
            }
        }

        private CommonNewItem OnBindLowProbabilityItem(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonNewItem>();
        }

        private CommonNewItem OnBind100ProbabilityItem(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonNewItem>();
        }

        private void OnLowProbabilityItemVisiable(ComUIListElementScript item)
        {
            var commonNewItem = item.gameObjectBindScript as CommonNewItem;
            if(commonNewItem != null && item.m_index >= 0 && item.m_index < mLowProbabilityBxyItemList.Count)
            {
                commonNewItem.InitItem(mLowProbabilityBxyItemList[item.m_index].TableID);
            }
        }

        private void On100ProbabilityItemVisiable(ComUIListElementScript item)
        {
            var commonNewItem = item.gameObjectBindScript as CommonNewItem;
            if(commonNewItem != null && item.m_index >= 0 && item.m_index < m100ProbabilityBxyItemList.Count)
            {
                commonNewItem.InitItem(m100ProbabilityBxyItemList[item.m_index].TableID);
            }
        }

        private void OnSetLowElementAmount()
        {
            if (mLowProbabilityUIListScript != null)
                mLowProbabilityUIListScript.SetElementAmount(mLowProbabilityBxyItemList.Count);

            if (mLowRoot != null)
                mLowRoot.CustomActive(mLowProbabilityBxyItemList.Count > 0 ? true : false);
        }
        #endregion

        #region HighProbabilityUIListScript

        private void InitHighProbabilityUIListScript()
        {
            if (mHighProbabilityUIListScript != null)
            {
                mHighProbabilityUIListScript.Initialize();
                mHighProbabilityUIListScript.onBindItem += OnBindHighProbabilityItem;
                mHighProbabilityUIListScript.onItemVisiable += OnHighProbabilityItemVisiable;
            }
        }

        private void UnInitHighProbabilityUIListScript()
        {
            if (mHighProbabilityUIListScript != null)
            {
                mHighProbabilityUIListScript.onBindItem -= OnBindHighProbabilityItem;
                mHighProbabilityUIListScript.onItemVisiable -= OnHighProbabilityItemVisiable;
            }
        }

        private CommonNewItem OnBindHighProbabilityItem(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonNewItem>();
        }

        private void OnHighProbabilityItemVisiable(ComUIListElementScript item)
        {
            var commonNewItem = item.gameObjectBindScript as CommonNewItem;
            if (commonNewItem != null && item.m_index >= 0 && item.m_index < mHighProbabilityBxyItemList.Count)
            {
                commonNewItem.InitItem(mHighProbabilityBxyItemList[item.m_index].TableID);
            }
        }

        private void OnSetHighElementAmount()
        {
            if (mHighProbabilityUIListScript != null)
                mHighProbabilityUIListScript.SetElementAmount(mHighProbabilityBxyItemList.Count);

            if (mHighRoot != null)
                mHighRoot.CustomActive(mHighProbabilityBxyItemList.Count > 0 ? true : false);
        }

        private void OnSet100ElementAmount()
        {
            if (m100ProbabilityUIListScript != null)
                m100ProbabilityUIListScript.SetElementAmount(m100ProbabilityBxyItemList.Count);

            if (m100Root != null)
                m100Root.CustomActive(m100ProbabilityBxyItemList.Count > 0 ? true : false);
        }
        #endregion

        private void UpdatePutBxyInfo(ItemData itemData, BxyMergeItemElement element)
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

            //如果同样的辟邪玉已全部放进去
            if (count >= allCount)
            {
                SystemNotifyManager.SysNotifyTextAnimation("放入失败，该辟邪玉已放入合成区");
                return;
            }

            mCurrentSelectedBxyQuality = (int)itemData.Quality;

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

            mBxyItemA.UpdateBxyItem(dataMerge.leftItem);
            mBxyItemB.UpdateBxyItem(dataMerge.rightItem);

            if (element != null)
            {
                element.SetCheckMaskRoot(true);
            }

            if (dataMerge.leftItem != null && dataMerge.rightItem != null)
            {
                mMergePreviewRoot.CustomActive(true);
                mMergeDesc.CustomActive(false);

                GetPreViewMagicCardList();
            }
            else
            {
                mMergePreviewRoot.CustomActive(false);
                mMergeDesc.CustomActive(true);
            }

            UpdateMoneyInfo();
            SetElementAmount();
        }

        private void UpdateMoneyInfo()
        {
            if (dataMerge.leftItem != null && dataMerge.rightItem != null)
            {
                int q1 = (int)dataMerge.leftItem.Quality;
                int q2 = (int)dataMerge.rightItem.Quality;
                int q3 = q1;
                if (q1 < q2)
                {
                    q3 = q2;
                }
                // ItemData item = dataMerge.leftItem != null ? dataMerge.leftItem : dataMerge.rightItem;

                mComMergeMoneyControl.SetState(ComMergeMoneyControl.CMMC.CMMC_ENOUGH);
                int count = GetMergeCardCost(q3);
                mComMergeMoneyControl.SetCost(iMoneyID, count);

                mComMergeMoneyControl1.SetState(ComMergeMoneyControl.CMMC.CMMC_ENOUGH);
                int count1 = GetMergeCardCost1(q3);
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
        /// <param name="isBxyA"></param>
        private void OnBxyEmptyClick(bool isBxyA)
        {
            if (isBxyA == true)
            {
                dataMerge.leftItem = null;
            }
            else
            {
                dataMerge.rightItem = null;
            }

            mMergePreviewRoot.CustomActive(false);
            mMergeDesc.CustomActive(true);

            if (dataMerge.leftItem == null && dataMerge.rightItem == null)
            {
                mCurrentSelectedBxyQuality = 0;
            }

            UpdateMoneyInfo();
            SetElementAmount();
        }

        private int GetMergeCardCost(int quality)
        {
            string[] infos = Global.BXY_INFO.Split('|');
            for (int i = 0; i < infos.Length; i++)
            {
                string[] strs = infos[i].Split(',');
                int q = -1;
                int.TryParse(strs[0], out q);
                if (q == quality)
                {
                    string[] tmp = strs[2].Split('_');
                    int num = -1;
                    int.TryParse(tmp[1], out num);
                    return num;
                }
            }
            return -1;
        }

        private int GetMergeCardCost1(int quality)
        {
            string[] infos = Global.BXY_INFO.Split('|');
            for (int i = 0; i < infos.Length; i++)
            {
                string[] strs = infos[i].Split(',');
                int q = -1;
                int.TryParse(strs[0], out q);
                if (q == quality)
                {
                    string[] tmp = strs[1].Split('_');
                    int num = -1;
                    int.TryParse(tmp[1], out num);
                    return num;
                }
            }
            return -1;
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
                //是否存在两个辟邪玉
                if (dataMerge.leftItem != null && dataMerge.rightItem != null)
                {
                    //品质大于紫色，进行提示
                   
                    //品质为橙色，进行提示
                    if (dataMerge.leftItem.Quality == ItemTable.eColor.YELLOW || dataMerge.rightItem.Quality == ItemTable.eColor.YELLOW)
                    {
                        SystemNotifyManager.SystemNotify(10714, _ConfirmToMergeBxy);
                    }
                    //品质为粉色，进行提示
                    else if (dataMerge.leftItem.Quality == ItemTable.eColor.PINK || dataMerge.rightItem.Quality == ItemTable.eColor.PINK)
                    {
                        SystemNotifyManager.SystemNotify(10713, _ConfirmToMergeBxy);
                    }
                    else
                    {
                         _ConfirmToMergeBxy();
                    }
                }
                else
                {
                    if (dataMerge.leftItem == null)
                    {
                        SystemNotifyManager.SystemNotify(4700008);
                    }
                    else
                    {
                        SystemNotifyManager.SystemNotify(4700009);
                    }
                }
            }
        }

        private void _ConfirmToMergeBxy()
        {
            int q1 = (int)dataMerge.leftItem.Quality;
            int q2 = (int)dataMerge.rightItem.Quality;
            int q3 = q1;
            if (q1 < q2)
            {
                q3 = q2;
            }
            List<CostItemManager.CostInfo> costInfos = new List<CostItemManager.CostInfo>();
            CostItemManager.CostInfo costInfo1 = new CostItemManager.CostInfo
            {
                nMoneyID = iMoneyID,
                nCount = GetMergeCardCost(q3),
            };
            CostItemManager.CostInfo costInfo2 = new CostItemManager.CostInfo
            {
                nMoneyID = iMoneyID2,
                nCount = GetMergeCardCost1(q3),
            };
            costInfos.Add(costInfo1);
            costInfos.Add(costInfo2);
            CostItemManager.GetInstance().TryCostMoneiesDefault(costInfos, OnConfirmBxyBindMethod);
        }

        //判断是否显示绑定方式不一致的提示
        private void OnConfirmBxyBindMethod()
        {
            //revert原来的合成方式，无论绑定OR非绑定，都统一合成绑定
            OnFinalSendBxyMergeReq();
        }

        //最终进行合成附魔卡
        private void OnFinalSendBxyMergeReq()
        {
            //检测背包是否为满
            if (PackageUtility.IsPackageFullByType(EPackageType.Bxy) == true)
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
            if (dataMerge.leftItem.Quality >= dataMerge.rightItem.Quality)
            {
                Global.bxyMergeGuid = dataMerge.leftItem.GUID;
                SendMergeBxy(dataMerge.leftItem.GUID, dataMerge.rightItem.GUID);  
            } 
            else
            {
                Global.bxyMergeGuid = dataMerge.rightItem.GUID;
                SendMergeBxy(dataMerge.rightItem.GUID, dataMerge.leftItem.GUID);
            }
            
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
                // 辟邪玉合成，flag=1
                kCmd.flag = 1;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kCmd);
            }
        }

        private void GetPreViewMagicCardList()
        {
            if (mHighProbabilityBxyItemList != null)
                mHighProbabilityBxyItemList.Clear();

            if (mLowProbabilityBxyItemList != null)
                mLowProbabilityBxyItemList.Clear();

            if (m100ProbabilityBxyItemList != null)
                m100ProbabilityBxyItemList.Clear();

            int min = 185000000;
            int max = 185000003;
            if (dataMerge.leftItem != null)
            {
                min = dataMerge.leftItem.TableID;
                max = dataMerge.leftItem.TableID;
            }

            if (dataMerge.rightItem != null && dataMerge.rightItem.TableID > min)
            {
                max = dataMerge.rightItem.TableID;
            }
            else
            {
                min = dataMerge.rightItem.TableID;
            }

            if (min == max && max == 185000003)
            {
                var mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(max);
                m100ProbabilityBxyItemList.Add(mItem);
            }
            else if (min == max && max != 185000003)
            {
                var mItem1 = ItemDataManager.GetInstance().GetCommonItemTableDataByID(max);
                mHighProbabilityBxyItemList.Add(mItem1);

                var mItem2 = ItemDataManager.GetInstance().GetCommonItemTableDataByID(max + 1);
                mLowProbabilityBxyItemList.Add(mItem2);
            }
            else
            {
                for (int i = min; i <= max; i++)
                {
                    var mItem1 = ItemDataManager.GetInstance().GetCommonItemTableDataByID(i);
                    mHighProbabilityBxyItemList.Add(mItem1);
                }
            }

            OnSetHighElementAmount();
            OnSetLowElementAmount();
            OnSet100ElementAmount();
        }
        #endregion
    }
}