using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Network;
using Protocol;
using EJarType = ProtoTable.JarBonus.eType;

namespace GameClient
{
    public class JarBuyCost
    {
        /// <summary>
        /// 购买折扣
        /// </summary>
        public float fDiscount;

        /// <summary>
        /// 折扣剩余次数, 负数表示没有次数限制  
        /// </summary>
        public virtual int nRemainDiscountTime { get; set; }

        /// <summary>
        /// 折扣次数是否会重置
        /// </summary>
        public bool bDisTimeReset;

        /// <summary>
        /// 消耗的道具
        /// </summary>
        public ItemData item; 

        public int GetRealCostCount(int a_nBuyCount)
        {
            int nDiscountCount = 0;//折扣计算
            int nOrigCount = 0;//源自计数
            float TowDiscount = 1;
            if (nRemainDiscountTime < 0)
            {
                nDiscountCount = a_nBuyCount;
                nOrigCount = 0;
            }
            else if (nRemainDiscountTime > 0)
            {
                if (nRemainDiscountTime >= a_nBuyCount)
                {
                    if (a_nBuyCount==1)
                    {
                        nDiscountCount = a_nBuyCount;
                        nOrigCount = 0;
                        TowDiscount =1f;
                    }
                    else
                    {
                        nDiscountCount = a_nBuyCount;
                        nOrigCount = 0;
                        TowDiscount = 0.9f;
                    }
                   
                }
                else
                {
                    nDiscountCount = nRemainDiscountTime;
                    nOrigCount = a_nBuyCount - nRemainDiscountTime;
                    TowDiscount = 0.9f;
                }
            }
            else
            {
                if (a_nBuyCount==1)
                {
                    nDiscountCount = 0;
                    nOrigCount = a_nBuyCount;
                    TowDiscount = 1;
                }
                else
                {
                    nDiscountCount = 0;
                    nOrigCount = a_nBuyCount;
                    TowDiscount = 0.9f;
                }
               
            }

            int nCount = UnityEngine.Mathf.FloorToInt((nDiscountCount * item.Count * fDiscount + nOrigCount * item.Count)*TowDiscount);
            return nCount;
        }
    }

    public class ActivityJarBuyCost : JarBuyCost
    {
        protected int m_nJarID;
        protected int m_nRemainDiscountTime = -1;
        protected string m_strRemainTimeKey;

        public int nJarID
        {
            get
            {
                return m_nJarID;
            }
            set
            {
                m_nJarID = value;
                m_strRemainTimeKey = string.Format("jar_buy_dis_remain_{0}", m_nJarID);
            }
        }

        /// <summary>
        /// 折扣剩余次数, 负数表示没有次数限制  
        /// </summary>
        public override int nRemainDiscountTime
        {
            get
            {
                if (m_nRemainDiscountTime < 0)
                {
                    return m_nRemainDiscountTime;
                }
                else
                {
                    return CountDataManager.GetInstance().GetCount(m_strRemainTimeKey);
                }
            }
            set
            {
                m_nRemainDiscountTime = value;
            }
        }
    }

    public class JarBuyInfo
    {
        /// <summary>
        /// 购买次数
        /// </summary>
        public int nBuyCount;

        /// <summary>
        /// 最大免费次数
        /// </summary>
        public int nMaxFreeCount;

        /// <summary>
        /// 免费CD
        /// </summary>
        public int nFreeCD;

        /// <summary>
        /// 免费时间戳
        /// </summary>
        public virtual int nFreeTimestamp { get; set; }

        /// <summary>
        /// 免费次数
        /// </summary>
        public virtual int nFreeCount { get; set; }

        /// <summary>
        /// item1 | item2
        /// 或的关系，优先消耗第一种货币（道具）
        /// </summary>
        public List<JarBuyCost> arrCosts;
    }

    public class MagicJarBuyInfo : JarBuyInfo
    {
        int JarTableID = 0;

        public MagicJarBuyInfo(int JarID)
        {
            JarTableID = JarID;
        }

        public override int nFreeTimestamp
        {
            get
            {
                ProtoTable.JarBonus tabledata = TableManager.GetInstance().GetTableItem<ProtoTable.JarBonus>(JarTableID);
                if (tabledata == null)
                {
                    Logger.LogErrorFormat("Get JarDataManager nFreeTimestamp is wrong, JarTableID = {0}", JarTableID);
                    return 0;
                }

                return CountDataManager.GetInstance().GetCount(tabledata.NextFreeTimeCounterKey);
            }

            set
            {
                ProtoTable.JarBonus tabledata = TableManager.GetInstance().GetTableItem<ProtoTable.JarBonus>(JarTableID);
                if (tabledata == null)
                {
                    Logger.LogErrorFormat("Set JarDataManager nFreeTimestamp is wrong, JarTableID = {0}", JarTableID);
                    return;
                }

                CountDataManager.GetInstance().SetCount(tabledata.NextFreeTimeCounterKey, (uint)value);
            }
        }

        public override int nFreeCount
        {
            get
            {
                // 有免费的可能
                if (nMaxFreeCount > 0 && nFreeCD > 0)
                {
                    ProtoTable.JarBonus tabledata = TableManager.GetInstance().GetTableItem<ProtoTable.JarBonus>(JarTableID);
                    if(tabledata == null)
                    {
                        Logger.LogErrorFormat("Get JarDataManager nFreeCount is wrong, JarTableID = {0}", JarTableID);
                        return 0;
                    }

                    int nCount = CountDataManager.GetInstance().GetCount(tabledata.FreeNumCounterKey);

                    if (nCount >= 0 && nCount < nMaxFreeCount)
                    {
                        bool bChanged = false;
                        int nServerTime = (int)TimeManager.GetInstance().GetServerTime();
                        while (nFreeTimestamp <= nServerTime)
                        {
                            bChanged = true;
                            nCount += 1;
                            if (nCount < 0)
                            {
                                nCount = 0;
                            }
                            else if (nCount > nMaxFreeCount)
                            {
                                nCount = nMaxFreeCount;
                            }

                            if (nCount >= nMaxFreeCount)
                            {
                                break;
                            }

                            nFreeTimestamp += nFreeCD;
                        }
                        if (bChanged)
                        {
                            nFreeCount = nCount;
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.JarFreeTimeChanged);
                        }
                    }

                    return nCount;
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                int nCount = value;
                if (nCount < 0)
                {
                    nCount = 0;
                }
                else if (nCount > nMaxFreeCount)
                {
                    nCount = nMaxFreeCount;
                }

                ProtoTable.JarBonus tabledata = TableManager.GetInstance().GetTableItem<ProtoTable.JarBonus>(JarTableID);
                if (tabledata == null)
                {
                    Logger.LogErrorFormat("Set JarDataManager nFreeCount is wrong, JarTableID = {0}", JarTableID);
					return;
                }

                CountDataManager.GetInstance().SetCount(tabledata.FreeNumCounterKey, (uint)nCount);
            }
        }
    }

    public class JarData
    {
        public int nID;
        public EJarType eType;
        public string strName;
        public string strJarImagePath;
        public string strJarModelPath;
        public IList<int> arrFilters;
        public List<ItemSimpleData> arrBonusItems;
        public List<ItemSimpleData> arrRealBonusItems;
        public List<ItemData> arrBuyItems;
        public List<JarBuyInfo> arrBuyInfos;
    }

    public class JarBonus
    {
        public int nBonusID;
        public ItemData item;
        public bool bHighValue;
    }

    public class JarTreeNode
    {
        public JarTreeNode parent = null;
        public List<JarTreeNode> children = null;
        public int nKey = 0;
        public object value = null;

        public JarTreeNode GetChild(int a_nKey)
        {
            if (children != null)
            {
                for (int i = 0; i < children.Count; ++i)
                {
                    if (children[i].nKey == a_nKey)
                    {
                        return children[i];
                    }
                }
            }
            return null;
        }

        public void AddChild(JarTreeNode a_node)
        {
            if (children == null)
            {
                children = new List<JarTreeNode>();
            }

            for (int i = 0; i < children.Count; ++i)
            {
                if (children[i].nKey == a_node.nKey)
                {
                    return;
                }
            }

            children.Add(a_node);
        }
    }

    class JarDataManager : DataManager<JarDataManager>
    {
        Dictionary<int, JarData> m_dictJarData = new Dictionary<int, JarData>();

        JarData m_magicJarData;
        JarData m_magicJarData_Lv55;
        JarTreeNode m_goldJarTreeRoot = null;

        string[] m_arrMainTypeDescs =
        {
            "goldjar_main_type_weapon",
            "goldjar_main_type_armor",
            "goldjar_main_type_jewelry"
        };

        string[] m_arrSubTypeDescs =
        {
            "goldjar_sub_type_huge_sword",
            "goldjar_sub_type_revolver",
            "goldjar_sub_type_staff",
            "goldjar_sub_type_cloth",
            "goldjar_sub_type_skin",
            "goldjar_sub_type_lightd",
            "goldjar_sub_type_heavy",
            "goldjar_sub_type_plate",
            "goldjar_sub_type_ring",
            "goldjar_sub_type_necklase",
            "goldjar_sub_type_bracelet",
        };
        bool m_bNotify = true;
        public bool isNotify { get { return m_bNotify; } set { m_bNotify = value; } }
        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.MagicJarDataManager;
        }

        public override void Initialize()
        {
            _InitJarTableData();
            _InitMagicJar();
            _InitGoldJar();
            m_bNotify = true;
        }

        public override void Clear()
        {
            _ClearJarTableData();
            _ClearMagicJar();
            _ClearGoldJar();
            m_bNotify = true;
        }

        public JarData GetJarData(int a_nID)
        {
            JarData jarData = null;
            m_dictJarData.TryGetValue(a_nID, out jarData);
            return jarData;
        }

        /// <summary>
        /// 每次登陆是否显示罐子Tips
        /// </summary>
        /// <returns></returns>
        public bool ShowJarTips()
        {
            int mYear = 0;
            int mMonth = 0;
            int mToday = 0;

            var sCurTime = TimeUtility.GetDateTimeByCommonType((int) TimeManager.GetInstance().GetServerTime());
            
            string[] sStrCount = sCurTime.Split('-');
            if (sStrCount.Length <= 0)
            {
                return false;
            }

            string sYear = sStrCount[0];
            string sMonth = sStrCount[1];
            string sDay= sStrCount[2];
            int.TryParse(sYear, out mYear);
            int.TryParse(sMonth, out mMonth);
            int.TryParse(sDay, out mToday);
            int mCurMonthTatleDays = DateTime.DaysInMonth(mYear, mMonth);
            int mDiff = mCurMonthTatleDays - mToday;

            if (mDiff > 2)
            {
                return false;
            }

            if (!Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Jar))
            {
                return false;
            }

            return true;
        }

        public bool CheckRedPoint(EJarType a_eType)
        {
            var iter = m_dictJarData.GetEnumerator();
            while (iter.MoveNext())
            {
                JarData jarData = iter.Current.Value;
                if (jarData != null && jarData.eType == a_eType)
                {
                    for (int j = 0; j < jarData.arrBuyInfos.Count; ++j)
                    {
                        if (CheckRedPoint(jarData, jarData.arrBuyInfos[j]))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool CheckRedPoint(JarData a_data, JarBuyInfo a_buyInfo)
        {
            if (a_buyInfo.nFreeCount > 0)
                return true;
            if (a_buyInfo.arrCosts.Count > 0 && a_buyInfo.arrCosts[0].item.Count < 1)
                return true;
            /*
            if (
                a_buyInfo.arrCosts.Count > 0 &&
                a_buyInfo.arrCosts[0].item.Count <= ItemDataManager.GetInstance().GetOwnedItemCount((int)a_buyInfo.arrCosts[0].item.TableID)
                )
            {
                return true;
            }
             * */
            //             else
            //             {
            //                 if (a_buyInfo.cost.Count <= ItemDataManager.GetInstance().GetOwnedItemCount((int)a_buyInfo.cost.TableID, true))
            //                 {
            //                     if (a_data.limitTimes > 0)
            //                     {
            //                         if (CountDataManager.GetInstance().GetCount(a_data.limitkey) < a_data.limitTimes)
            //                         {
            //                             return true;
            //                         }
            //                     }
            //                 }
            //             }

            return false;
        }
      
        // 增加一个运营活动id参数
        // 此参数默认为0（无效),玩家在神器罐派对活动期间开罐子需要发送此id（服务器用于区分是使用普通的折扣，还是用活动期间的折扣）
        // add by qxy 2018-12-19
        public void RequestBuyJar(JarData a_data, JarBuyInfo a_buyInfo,uint opActId = 0)
        {
            SceneUseMagicJarReq msg = new SceneUseMagicJarReq();
            msg.type = (UInt32)a_data.nID;
            msg.combo = (byte)a_buyInfo.nBuyCount;
            msg.opActId = opActId;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait(SceneUseMagicJarRet.MsgID, (MsgDATA data) =>
            {
                if (data == null)
                {
                    return;
                }

                SceneUseMagicJarRet msgRet = new SceneUseMagicJarRet();
                int nPos = 0;
                msgRet.decode(data.bytes, ref nPos);
                //nPos++;
                List<Item> items = ItemDecoder.Decode(data.bytes, ref nPos, data.bytes.Length);


                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MagicJarUseFail);
                }
                else
                {
                    RedPointDataManager.GetInstance().NotifyRedPointChanged();
                    List<JarBonus> arrBonus = new List<JarBonus>();

                    {
                        JarBonus jarBonus = new JarBonus();
                        jarBonus.nBonusID = 0;
                        jarBonus.item = ItemDataManager.CreateItemDataFromTable((int)msgRet.baseItem.id);
                        jarBonus.item.Count = (int)msgRet.baseItem.num;
                        jarBonus.bHighValue = false;
                        arrBonus.Add(jarBonus);
                    }

						for(int i=0; i<msgRet.getItems.Length; ++i)
						{
							OpenJarResult reward = msgRet.getItems[i];
							JarBonus jarBonus = new JarBonus();
							jarBonus.nBonusID = (int)reward.jarItemId;

							ProtoTable.JarItemPool table = TableManager.GetInstance().GetTableItem<ProtoTable.JarItemPool>((int)reward.jarItemId);

							ItemData itemData = null;
							for(int j=0; j<items.Count; ++j)
							{
								if (table.ItemID == items[j].dataid)
								{
									items[j].num -= (ushort)table.ItemNum;

									//Logger.LogErrorFormat("jar result id:{0} num:{1} leftNum:{2}", table.ItemID, table.ItemNum, items[j].num);

									itemData = ItemDataManager.GetInstance().CreateItemDataFromNet(items[j]);
									itemData.Count = table.ItemNum;;

									if (items[j].num <= 0)
										items.RemoveAt(j);

									break;
								}
							}

							if (itemData == null)
							{
								//Logger.LogErrorFormat("jar not find!!!! result id:{0} num:{1} ", table.ItemID, table.ItemNum);
								itemData = ItemDataManager.CreateItemDataFromTable((int)table.ItemID);
								itemData.Count = table.ItemNum;
							}
							
							jarBonus.item = itemData;
							jarBonus.bHighValue = table.ShowEffect == 1;
							//jarBonus.bHighValue = true;
							arrBonus.Add(jarBonus);

						}

                    ShowItemsFrameData frameData = new ShowItemsFrameData();
                    frameData.data = a_data;
                    frameData.items = arrBonus;
                    frameData.buyInfo = a_buyInfo;
                    frameData.scoreItemData = ItemDataManager.CreateItemDataFromTable((int)msgRet.getPointId);
                    if (frameData.scoreItemData != null)
                    {
                        frameData.scoreItemData.Count = (int)msgRet.getPoint;
                    }
                    frameData.scoreRate = (int)msgRet.crit;

                    ClientSystemManager.GetInstance().OpenFrame<JarBuyResultFrame>(FrameLayer.Middle, frameData);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MagicJarUseSuccess, frameData);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.EquipRecovery);
                }
            }, true, -1);
        }

        public void RequestQuaryJarShopSocre()
        {
            SceneJarPointReq kSend = new SceneJarPointReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, kSend);

            WaitNetMessageManager.GetInstance().Wait<SceneJarPointRes>(msgRet =>
            {
                PlayerBaseData.GetInstance().GoldJarScore = msgRet.goldPoint;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GoldJarScoreChanged);
                PlayerBaseData.GetInstance().MagicJarScore = msgRet.magPoint;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MagicJarScoreChanged);
            });
        }

        public void RequestJarBuyRecord(int a_nJarID)
        {
            WorldOpenJarRecordReq msg = new WorldOpenJarRecordReq();
            msg.jarId = (uint)a_nJarID;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldOpenJarRecordRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.JarOpenRecordUpdate, msgRet);
            }, false);
        }


        protected JarBuyCost _CreateJarBuyCost(ProtoTable.JarBonus a_table, bool a_bIsNeedExItem, bool a_bBuyOne)
        {
            switch (a_table.Type)
            {
                case EJarType.FashionJar:
                case EJarType.WingJar:
                case EJarType.EquipJar:
                    {
                        if (a_bIsNeedExItem == false)
                        {
                            ActivityJarBuyCost activityCost = new ActivityJarBuyCost();
                            activityCost.nJarID = a_table.ID;
                            activityCost.nRemainDiscountTime = 0;
                            activityCost.bDisTimeReset = a_table.ActJarDisReset != 0;
                            activityCost.item = ItemDataManager.CreateItemDataFromTable(a_table.BuyMoneyType);
                            activityCost.item.Count = a_table.MoneyValue;
                            activityCost.fDiscount = a_table.SingleBuyDiscount / 100.0f;
                            return activityCost;
                        }
                        break;
                    }
            }
            
            // 货币
            if (a_bIsNeedExItem == true)
            {
                JarBuyCost cost = new JarBuyCost();
                cost.item = ItemDataManager.CreateItemDataFromTable(a_table.ExItemID);
                cost.item.Count = a_table.ExItemCostNum;
                cost.fDiscount = 1.0f;
                cost.nRemainDiscountTime = -1;
                cost.bDisTimeReset = false;
                return cost;
            }
            else
            {
                ItemData item = ItemDataManager.CreateItemDataFromTable(a_table.BuyMoneyType);
                if (item != null)
                {
                    JarBuyCost cost = new JarBuyCost();
                    cost.item = item;
                    cost.item.Count = a_table.MoneyValue;
                    cost.fDiscount = a_bBuyOne ? 1.0f : a_table.ComboBuyDiscount / 100.0f;
                    cost.nRemainDiscountTime = -1;
                    cost.bDisTimeReset = false;
                    return cost;
                }
                else
                {
                    return null;
                }
            }
        }

        protected void _InitJarTableData()
        {
            m_dictJarData.Clear();

            int Job = PlayerBaseData.GetInstance().JobTableID;

            // 读取奖池展示表
            Dictionary<int, List<ItemSimpleData>> dicItemsPool = new Dictionary<int, List<ItemSimpleData>>();

            var iterItemPoolTable = TableManager.GetInstance().GetTable<ProtoTable.JarItemShowPool>().GetEnumerator();
            while (iterItemPoolTable.MoveNext())
            {
                ProtoTable.JarItemShowPool table = iterItemPoolTable.Current.Value as ProtoTable.JarItemShowPool;

                if (table.Visible.Count >=1 )
                {
                    for (int i = 0; i < table.Visible.Count; i++)
                    {
                        if (table.Visible[i] == Job ||table.Visible[i] == 0)
                        {
                            ItemSimpleData simpledata = new ItemSimpleData();

                            simpledata.ItemID = table.ItemID;
                            simpledata.Count = table.ItemNum;

                            if (dicItemsPool.ContainsKey(table.JarType) == false)
                            {
                                dicItemsPool.Add(table.JarType, new List<ItemSimpleData>());
                            }

                            dicItemsPool[table.JarType].Add(simpledata);
                        }
                    }                  
                }          
            }

            // 读取奖池表
            Dictionary<int, List<ItemSimpleData>> dicRealItemsPool = new Dictionary<int, List<ItemSimpleData>>();

            var iterTable = TableManager.GetInstance().GetTable<ProtoTable.JarItemPool>().GetEnumerator();
            while (iterTable.MoveNext())
            {
                ProtoTable.JarItemPool table = iterTable.Current.Value as ProtoTable.JarItemPool;

                ItemSimpleData simpledata = new ItemSimpleData();

                simpledata.ItemID = table.ItemID;
                simpledata.Count = table.ItemNum;

                if (dicRealItemsPool.ContainsKey(table.JarType) == false)
                {
                    dicRealItemsPool.Add(table.JarType, new List<ItemSimpleData>());
                }

                dicRealItemsPool[table.JarType].Add(simpledata);
            }

            // 对奖池道具排序
            //var iterItemPool = dicItemsPool.GetEnumerator();
            //while (iterItemPool.MoveNext())
            //{
            //    List<ItemData> value = iterItemPool.Current.Value as List<ItemData>;
            //    if (value != null)
            //    {
            //        value.Sort((ItemData a_itemLeft, ItemData a_itemRight) =>
            //        {
            //            if (a_itemLeft.Quality > a_itemRight.Quality)
            //            {
            //                return -1;
            //            }
            //            else
            //            {
            //                return 1;
            //            }
            //        });
            //    }
            //}

            // 读取罐子表
            var iter = TableManager.GetInstance().GetTable<ProtoTable.JarBonus>().GetEnumerator();
            while (iter.MoveNext())
            {
                ProtoTable.JarBonus table = iter.Current.Value as ProtoTable.JarBonus;
                if (table == null)
                {
                    continue;
                }

                JarData data = new JarData();

                data.nID = table.ID;
                data.eType = table.Type;
                data.strName = table.Name;
                data.arrFilters = table.Filter;
                data.strJarImagePath = table.JarImage;
                data.strJarModelPath = table.JarEffect;

                if (dicItemsPool.ContainsKey(data.nID))
                {
                    data.arrBonusItems = dicItemsPool[data.nID];
                }
                else
                {
                    data.arrBonusItems = new List<ItemSimpleData>();
                }

                if (dicRealItemsPool.ContainsKey(data.nID))
                {
                    data.arrRealBonusItems = dicRealItemsPool[data.nID];
                }
                else
                {
                    data.arrRealBonusItems = new List<ItemSimpleData>();
                }

                data.arrBuyInfos = new List<JarBuyInfo>();
                if (table.ComboBuyNum > 0)
                {
                    // 购买一次
                    {
                        JarBuyInfo buyInfo = null;

                        if (table.Type == EJarType.MagicJar || table.Type == EJarType.MagicJar_Lv55)
                        {
                            buyInfo = new MagicJarBuyInfo(table.ID);
                        }
                        else
                        {
                            buyInfo = new JarBuyInfo();
                        }
                        buyInfo.nMaxFreeCount = table.MaxFreeCount;
                        buyInfo.nFreeCD = table.FreeCD;
                        //buyInfo.nFreeTimestamp = 0;
                        //buyInfo.nFreeCount = 0;
                        buyInfo.nBuyCount = 1;
                        buyInfo.arrCosts = new List<JarBuyCost>();
                        // 凭证道具
                        if (table.NeedExItem == 1)
                        {
                            JarBuyCost cost = _CreateJarBuyCost(table, true, true);
                            if (cost != null)
                            {
                                buyInfo.arrCosts.Add(cost);
                            }
                        }
                        // 货币
                        {
                            JarBuyCost cost = _CreateJarBuyCost(table, false, true);
                            if (cost != null)
                            {
                                buyInfo.arrCosts.Add(cost);
                            }
                        }
                        data.arrBuyInfos.Add(buyInfo);
                    }
                    // 购买多次
                    {
                        JarBuyInfo buyInfo = new JarBuyInfo();
                        buyInfo.nBuyCount = table.ComboBuyNum;
                        buyInfo.arrCosts = new List<JarBuyCost>();
                        // 凭证道具（不打折）
                        if (table.NeedExItem == 1)
                        {
                            JarBuyCost cost = _CreateJarBuyCost(table, true, false);
                            if (cost != null)
                            {
                                buyInfo.arrCosts.Add(cost);
                            }
                        }
                        // 货币
                        {
                            JarBuyCost cost = _CreateJarBuyCost(table, false, false);
                            if (cost != null)
                            {
                                buyInfo.arrCosts.Add(cost);
                            }
                        }
                        data.arrBuyInfos.Add(buyInfo);
                    }
                }

                data.arrBuyItems = new List<ItemData>();
                for (int i = 0; i < table.GetItemsAndNum.Count; ++i)
                {
                    string[] temps = table.GetItemsAndNum[i].Split(',');
                    ItemData item = ItemDataManager.CreateItemDataFromTable(int.Parse(temps[0]));
                    if (null != item)
                    {
                        item.Count = int.Parse(temps[1]);
                        data.arrBuyItems.Add(item);
                    }
                    else
                    {
                        Logger.LogErrorFormat("[罐子] {0} {1} 格式解析出错 {2}, 无法找到 {3} 的道具", table.ID, table.Name, table.GetItemsAndNum[i], temps.Length > 0 ? temps[0] : "数组为空");
                    }
                }

                m_dictJarData.Add(data.nID, data);
            }
        }

        protected void _ClearJarTableData()
        {
            m_dictJarData.Clear();
        }

        #region magic jar
        public JarData GetMagicJarData()
        {
            return m_magicJarData;
        }

        public JarData GetMagicJarData_Lv55()
        {
            return m_magicJarData_Lv55;
        }

        protected void _InitMagicJar()
        {
            if (m_dictJarData == null)
            {
                return;
            }

            m_magicJarData = null;
            m_magicJarData_Lv55 = null;

            var iter = m_dictJarData.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.Value.eType == EJarType.MagicJar)
                {
                    m_magicJarData = iter.Current.Value;
                }
                else if(iter.Current.Value.eType == EJarType.MagicJar_Lv55)
                {
                    m_magicJarData_Lv55 = iter.Current.Value;
                }
            }
        }

        protected void _ClearMagicJar()
        {
            m_magicJarData = null;
            m_magicJarData_Lv55 = null;
        }
        #endregion

        #region gold jar
        public JarData GetGoldJarData(int a_nfilterType, int a_nFilterLevel)
        {
            JarTreeNode jarTreeNode = _GetChildTreeNode(m_goldJarTreeRoot, a_nfilterType);
            if (jarTreeNode != null)
            {
                jarTreeNode = _GetChildTreeNode(jarTreeNode, a_nFilterLevel);
                if (jarTreeNode != null)
                {
                    return jarTreeNode.value as JarData;
                }
            }
            return null;
        }

        public bool CheckGoldJarLevelRedPoint(int a_nSubType, int a_nLevel)
        {
            JarData jarData = GetGoldJarData(a_nSubType, a_nLevel);
            for (int i = 0; i < jarData.arrBuyInfos.Count; ++i)
            {
                if (CheckRedPoint(jarData, jarData.arrBuyInfos[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public List<JarTreeNode> GetGoldJarLevels(int a_nfilterType)
        {
            JarTreeNode jarTreeNode = _GetChildTreeNode(m_goldJarTreeRoot, a_nfilterType);
            if (jarTreeNode != null)
            {
                return jarTreeNode.children;
            }
            return null;
        }

        public string GetGoldJarMainTypeDesc(int a_nType)
        {
            int nIdx = a_nType - 1;
            if (nIdx >= 0 && nIdx < m_arrMainTypeDescs.Length)
            {
                return TR.Value(m_arrMainTypeDescs[nIdx]);
            }
            return string.Empty;
        }

        public string GetGoldJarSubTypeDesc(int a_nType)
        {
            int nIdx = a_nType - 1;
            if (nIdx >= 0 && nIdx < m_arrSubTypeDescs.Length)
            {
                return TR.Value(m_arrSubTypeDescs[nIdx]);
            }
            return string.Empty;
        }

        protected void _InitGoldJar()
        {
            if (m_dictJarData == null)
            {
                return;
            }

            m_goldJarTreeRoot = new JarTreeNode();

            var iter = m_dictJarData.GetEnumerator();
            while (iter.MoveNext())
            {
                JarData jarData = iter.Current.Value;
                if (jarData.eType == EJarType.GoldJar)
                {
                    JarTreeNode parentNode = m_goldJarTreeRoot;
                    for (int i = 0; i < jarData.arrFilters.Count; ++i)
                    {
                        JarTreeNode childNode = parentNode.GetChild(jarData.arrFilters[i]);
                        if (childNode == null)
                        {
                            childNode = new JarTreeNode();
                            childNode.parent = parentNode;
                            childNode.children = null;
                            childNode.nKey = jarData.arrFilters[i];
                            childNode.value = i == jarData.arrFilters.Count - 1 ? jarData : null;
                            parentNode.AddChild(childNode);
                        }
                        else
                        {
                            // 父节点的value值必须为null
                            if (i < jarData.arrFilters.Count - 1)
                            {
                                childNode.value = null;
                            }
                        }

                        parentNode = childNode;
                    }
                }
            }
        }

        void _ClearGoldJar()
        {
            m_goldJarTreeRoot = null;
        }

        JarTreeNode _GetChildTreeNode(JarTreeNode a_parentNode, int a_nKey)
        {
            if (a_parentNode != null && a_parentNode.children != null)
            {
                for (int i = 0; i < a_parentNode.children.Count; ++i)
                {
                    if (a_parentNode.children[i].nKey == a_nKey)
                    {
                        return a_parentNode.children[i];
                    }
                }
            }
            return null;
        }
        #endregion

        #region activity jar
        public bool HasActivityJar()
        {
            var iter = TableManager.GetInstance().GetTable<ProtoTable.ActivityJarTable>().GetEnumerator();
            while (iter.MoveNext())
            {
                ActivityInfo info = null;
                ActiveManager.GetInstance().allActivities.TryGetValue(iter.Current.Key, out info);
                if (info != null && info.state != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsActivityJar(int a_nActivityID)
        {
            var iter = TableManager.GetInstance().GetTable<ProtoTable.ActivityJarTable>().GetEnumerator();
            while (iter.MoveNext())
            {
                if (a_nActivityID == iter.Current.Key)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

    }
}
