using UnityEngine;
using System.Collections;
using Protocol;
using Network;
using System.Collections.Generic;
///////删除linq
using ProtoTable;
using System;

namespace GameClient
{
    class EnchantmentsCardData
    {
        public ulong guid;
        public int Count;
        public ItemData itemData;
        public ProtoTable.MagicCardTable magicItem;
        public ProtoTable.ItemTable item;
    }

    #region 附魔卡升级

    /// <summary>
    /// 附魔卡升级成功数据
    /// </summary>
    public class EnchantmentCardUpgradeSuccessData
    {
        public int mEnchantmentCardID;
        public int mEnchantmentCardLevel;
        public ulong mEnchantmentCardGUID;
        public ulong mEquipGUID;
        public bool isSuccess;//升级是否成功
    }

    /// <summary>
    /// 附魔卡item数据
    /// </summary>
    public class EnchantmentCardItemDataModel
    {
        /// <summary>
        /// //升级类型
        /// </summary>
        public UpgradePrecType mUpgradePrecType;
        /// <summary>
        /// //镶嵌附魔卡的装备
        /// </summary>
        public ItemData mEquipItemData;
        /// <summary>
        /// //附魔卡
        /// </summary>
        public ItemData mEnchantmentCardItemData;
        /// <summary>
        /// // 附魔卡数量
        /// </summary>
        public int mEnchantmentCardCount;
        /// <summary>
        /// 附魔卡品质
        /// </summary>
        public int mEchantmentCardQuality;
        /// <summary>
        /// 附魔卡阶段
        /// </summary>
        public int mEnchantmentCardStage;
        /// <summary>
        /// 附魔卡材料数量上限
        /// </summary>
        public int mEnchantmentCardCostMaxNum;
        /// <summary>
        /// 同名卡列表
        /// </summary>
        public List<int> mSameCardList;
        /// <summary>
        /// //消耗材料
        /// </summary>
        public ItemSimpleData mConsumableMaterialData;
    }

    /// <summary>
    /// 副卡数据
    /// </summary>
    public class EnchantmentCardViceCardData:IComparable<EnchantmentCardViceCardData>
    {
        /// <summary>
        /// 副卡ItemData
        /// </summary>
        public ItemData mViceCardItemData;

        /// <summary>
        /// 成功率
        /// </summary>
        public int mAllSuccessRate;

        /// <summary>
        /// 副卡数量
        /// </summary>
        public int mViceCardCount;

        public int CompareTo(EnchantmentCardViceCardData other)
        {
            if (mAllSuccessRate != other.mAllSuccessRate)
            {
                return mAllSuccessRate - other.mAllSuccessRate;
            }

            if (mViceCardItemData.mPrecEnchantmentCard != null && other.mViceCardItemData.mPrecEnchantmentCard != null)
            {
                if (mViceCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel != other.mViceCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel)
                {
                    return mViceCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel - other.mViceCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel;
                }
            }
            return mViceCardItemData.TableID - other.mViceCardItemData.TableID;
        }
    }
    
    /// <summary>
    /// 附魔卡成功率数据
    /// </summary>
    public class EnchantmentCardProbabilityData
    {
        /// <summary>
        /// 最小概率
        /// </summary>
        public int iMinProbability;
        /// <summary>
        /// 最大概率
        /// </summary>
        public int iMaxProbability;
        /// <summary>
        /// 成功率描述
        /// </summary>
        public string sSuccessRateDesc;
    }

    /// <summary>
    /// 副卡阶段数据
    /// </summary>
    public class EnchatmentCardViceCardStageData
    {
        /// <summary>
        /// 品质
        /// </summary>
        public int iQuality;
        /// <summary>
        /// 阶段
        /// </summary>
        public int iStage;
        /// <summary>
        /// 消耗数量
        /// </summary>
        public int iNumber;
    }
    #endregion

    class EnchantmentsCardManager : DataManager<EnchantmentsCardManager>
    {
        #region EnchantmentUpgrade
        //存储成功率描述
        List<EnchantmentCardProbabilityData> mEnchantmentCardProbabilityDataList = new List<EnchantmentCardProbabilityData>();
        /// <summary>
        /// 附魔卡升级表新数据
        /// </summary>
        List<MagicCardUpgradeTable> mMagicCardUpgradeTableList = new List<MagicCardUpgradeTable>();
        public bool IsNotShowGoldCoinTip = false;//是否显示金币提示界面
        public bool IsShowBindTip = false;//是否显示绑定与非绑定的提示界面
        public bool IsShowQualityTip = false; //是否显示品质提示界面
        public bool IsShowLevelTip = false; //是否显示等级提示界面
        #endregion

        #region delegate
        public delegate void OnUpdateCard(EnchantmentsCardData data);
        public delegate void OnTabMarkChanged(ulong guid);

        public OnUpdateCard onUpdateCard;
        public OnTabMarkChanged onTabMarkChanged;
        #endregion

        #region initTableMagicCard
        //orign data
        Dictionary<ulong, EnchantmentsCardData> m_akEnchantmentsCardDataDic = new Dictionary<ulong, EnchantmentsCardData>();
        //tableQualityData
        Dictionary<EnchantmentsType, List<EnchantmentsCardData>> m_akQuality2CardDic = new Dictionary<EnchantmentsType, List<EnchantmentsCardData>>();
        //tableID2origndata
        Dictionary<ulong, List<EnchantmentsCardData>> m_akTableID2CardDic = new Dictionary<ulong, List<EnchantmentsCardData>>();
        //tableID data
        Dictionary<ulong, EnchantmentsCardData> m_akTableGroup = new Dictionary<ulong, EnchantmentsCardData>();

        public List<EnchantmentsCardData> EnchantmentsCardDataList
        {
            get
            {
                return m_akTableGroup.Values.ToList();
            }
        }

        void _InitEnchantmentsCard()
        {
            m_akEnchantmentsCardDataDic.Clear();
            m_akQuality2CardDic.Clear();
            m_akTableID2CardDic.Clear();
            m_akTableGroup.Clear();

            var cardTable = TableManager.GetInstance().GetTable<ProtoTable.MagicCardTable>();
            var enumerator = cardTable.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var cardData = CreateEnchantmentsCardFromTable(enumerator.Current.Key);
                if(cardData != null)
                {
                    m_akTableGroup.Add((ulong)cardData.itemData.TableID, cardData);
                    _AddQualityCard(cardData);
                }
            }
        }

        void _AddQualityCard(EnchantmentsCardData data)
        {
            var eQuality = GetQuality(data.itemData.Quality);
            List<EnchantmentsCardData> outValue = null;
            if (!m_akQuality2CardDic.TryGetValue(eQuality, out outValue))
            {
                outValue = new List<EnchantmentsCardData>();
                m_akQuality2CardDic.Add(eQuality, outValue);
            }

            outValue.RemoveAll(x => { return x.itemData.TableID == data.itemData.TableID; });
            outValue.Add(data);
        }

        void _AddCard(EnchantmentsCardData data)
        {
            if(!m_akEnchantmentsCardDataDic.ContainsKey(data.itemData.GUID))
            {
                m_akEnchantmentsCardDataDic.Add(data.itemData.GUID, data);

                List<EnchantmentsCardData> outValue = null;
                if (!m_akTableID2CardDic.TryGetValue((ulong)data.itemData.TableID, out outValue))
                {
                    outValue = new List<EnchantmentsCardData>();
                    m_akTableID2CardDic.Add((ulong)data.itemData.TableID, outValue);
                }
                outValue.Add(data);

                if (m_akTableGroup.ContainsKey((ulong)data.itemData.TableID))
                {
                    m_akTableGroup.Remove((ulong)data.itemData.TableID);
                }
                m_akTableGroup.Add((ulong)data.itemData.TableID, data);

                _AddQualityCard(data);
            }
            else
            {
                //Logger.LogErrorFormat("try add repeat key to m_akEnchantmentsCardDataDic which key is {0}", data.itemData.GUID);
            }
        }

        void _RemoveCard(EnchantmentsCardData data)
        {
            if(m_akEnchantmentsCardDataDic.ContainsKey(data.itemData.GUID))
            {
                m_akEnchantmentsCardDataDic.Remove(data.itemData.GUID);

                List<EnchantmentsCardData> outValue = null;
                if(m_akTableID2CardDic.TryGetValue((ulong)data.itemData.TableID,out outValue))
                {
                    bool bFind = false;
                    for (int i = 0; i < outValue.Count; ++i)
                    {
                        if(outValue[i].itemData.GUID == data.itemData.GUID)
                        {
                            outValue.RemoveAt(i--);

                            if (m_akTableGroup.ContainsKey((ulong)data.itemData.TableID))
                            {
                                m_akTableGroup.Remove((ulong)data.itemData.TableID);
                            }
                            else
                            {
                                Logger.LogError("m_akTableGroup erase error!");
                            }

                            if (outValue.Count <= 0)
                            {
                                var cardData = CreateEnchantmentsCardFromTable((int)data.itemData.TableID);
                                if (cardData != null)
                                {
                                    m_akTableGroup.Add((ulong)cardData.itemData.TableID, cardData);
                                }
                                else
                                {
                                    Logger.LogErrorFormat("CreateEnchantmentsCardFromTable id = {0} failed !", data.itemData.TableID);
                                }
                            }
                            else
                            {
                                m_akTableGroup.Add((ulong)data.itemData.TableID, outValue[0]);
                            }

                            _AddQualityCard(m_akTableGroup[(ulong)data.itemData.TableID]);

                            bFind = true;
                            break;
                        }
                    }
                    if(!bFind)
                    {
                        Logger.LogError("m_akTableID2CardDic erase error!");
                    }
                }
                else
                {
                    Logger.LogError("m_akTableID2CardDic erase error!");
                }

            }
            else
            {
                //Logger.LogErrorFormat("_RemoveCard error data.guid = {0} is not in dic !", data.guid);
            }
        }

        public bool HasNewCard()
        {
            for(int i = 0; i < (int)EnchantmentsType.ET_COUNT; ++i)
            {
                if(HasNewQualityCard((EnchantmentsType)i))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasQualityCard(EnchantmentsType eEnchantmentsType)
        {
            return m_akQuality2CardDic.ContainsKey(eEnchantmentsType) && m_akQuality2CardDic[eEnchantmentsType].Count > 0;
        }

        public bool HasNewQualityCard(EnchantmentsType eEnchantmentsType)
        {
            if(HasQualityCard(eEnchantmentsType))
            {
                var datas = m_akQuality2CardDic[eEnchantmentsType];
                if(datas != null)
                {
                    for (int i = 0; i < datas.Count; ++i)
                    {
                        if(datas[i].itemData.IsNew)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void RemoveNewMark(ulong iTableID)
        {
            if(m_akTableGroup.ContainsKey(iTableID))
            {
                var current = m_akTableGroup[iTableID];
                current.itemData.IsNew = false;

                if(onTabMarkChanged != null)
                {
                    onTabMarkChanged.Invoke(iTableID);
                }
            }
        }
        #endregion

        #region process
        public override void Initialize()
        {
            RegisterNetHandler();
            _InitEnchantmentsCard();
            InitMagicCardProbabilityTable();
            InitMagicCardUpgradeTable();
            UnBindDelegate();
            BindDelegate();
        }

        protected  void BindDelegate()
        {
            ItemDataManager.GetInstance().onAddNewItem += this.OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem += this.OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem += this.OnUpdateItem;
        }

        protected  void UnBindDelegate()
        {
            ItemDataManager.GetInstance().onAddNewItem -= this.OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem -= this.OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem -= this.OnUpdateItem;
        }

        void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(SceneMagicCardCompRet.MsgID, OnRecvSceneMagicCardCompRet);
            NetProcess.AddMsgHandler(SceneAddMagicRet.MsgID, OnRecvSceneAddMagicRet);
            NetProcess.AddMsgHandler(SceneMagicCardUpgradeRes.MsgID, OnSceneMagicCardUpgradeRes);
        }

        void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(SceneMagicCardCompRet.MsgID, OnRecvSceneMagicCardCompRet);
            NetProcess.RemoveMsgHandler(SceneAddMagicRet.MsgID, OnRecvSceneAddMagicRet);
            NetProcess.RemoveMsgHandler(SceneMagicCardUpgradeRes.MsgID, OnSceneMagicCardUpgradeRes);
        }

        public override void Clear()
        {
            UnRegisterNetHandler();
            m_akEnchantmentsCardDataDic.Clear();
            m_akQuality2CardDic.Clear();
            m_akTableID2CardDic.Clear();
            m_akTableGroup.Clear();
            mEnchantmentCardProbabilityDataList.Clear();
            if (mMagicCardUpgradeTableList != null)
                mMagicCardUpgradeTableList.Clear();
            UnBindDelegate();
            IsNotShowGoldCoinTip = false;
            IsShowBindTip = false;
            IsShowQualityTip = false;
            IsShowLevelTip = false;
        }
        #endregion

        #region callback
        void OnAddNewItem(List<Item> items)
        {
            for(int i = 0; i < items.Count; ++i)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if(itemData != null && itemData.Type == ProtoTable.ItemTable.eType.EXPENDABLE &&
                    itemData.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard)
                {
                    EnchantmentsCardData cardData = CreateEnchantmentsCardFromNet(itemData.GUID);
                    if(cardData != null)
                    {
                        _AddCard(cardData);
                        if(onUpdateCard != null)
                        {
                            onUpdateCard(cardData);
                        }
                    }
                }
            }
        }

        void OnRemoveItem(ItemData itemData)
        {
            if (m_akEnchantmentsCardDataDic.ContainsKey(itemData.GUID))
            {
                _RemoveCard(m_akEnchantmentsCardDataDic[itemData.GUID]);
                if(onUpdateCard != null)
                {
                    onUpdateCard(m_akTableGroup[(ulong)itemData.TableID]);
                }
            }
            else
            {
                //Logger.LogErrorFormat("_RemoveCard error guid = {0} is not in dic !", uid);
            }
        }

        void OnUpdateItem(List<Item> items)
        {
            for (int i = 0; i < items.Count; ++i)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData != null && itemData.Type == ProtoTable.ItemTable.eType.EXPENDABLE &&
                    itemData.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard)
                {
                    EnchantmentsCardData cardData = CreateEnchantmentsCardFromNet(itemData.GUID);
                    if (cardData != null)
                    {
                        _RemoveCard(cardData);
                        _AddCard(cardData);
                        if (onUpdateCard != null)
                        {
                            onUpdateCard(cardData);
                        }
                    }
                }
            }
        }

        public static EnchantmentsType GetQuality(ProtoTable.ItemTable.eColor eColor)
        {
            for (int i = 0; i < (int)EnchantmentsType.ET_COUNT; ++i)
            {
                var type = typeof(EnchantmentsType);
                MapIndex mapIndex = Utility.GetEnumAttribute<EnchantmentsType, MapIndex>((EnchantmentsType)i);
                if (mapIndex.Index == (int)eColor)
                {
                    return (EnchantmentsType)i;
                }
            }

            return EnchantmentsType.ET_NORMAL;
        }

        public EnchantmentsCardData CreateEnchantmentsCardFromTable(int iTableID)
        {
            EnchantmentsCardData cardData = null; 
            var cardItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>(iTableID);
            if(cardItem != null)
            {
                var itemData = GameClient.ItemDataManager.CreateItemDataFromTable(iTableID);
                if(itemData != null)
                {
                    cardData = new EnchantmentsCardData();
                    cardData.Count = 0;
                    cardData.guid = (ulong)iTableID;
                    cardData.itemData = itemData;
                    cardData.magicItem = cardItem;
                    cardData.item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iTableID);
                }
                else
                {
                    Logger.LogErrorFormat("iTableID = {0} can not find in table itemTable!");
                }
            }
            return cardData;
        }

        public EnchantmentsCardData CreateEnchantmentsCardFromNet(ulong guid)
        {
            EnchantmentsCardData cardData = null;
            var itemData = ItemDataManager.GetInstance().GetItem(guid);
            if(itemData != null)
            {
                cardData = CreateEnchantmentsCardFromTable((int)itemData.TableID);
                if(cardData != null)
                {
                    cardData.itemData.GUID = itemData.GUID;
                    cardData.itemData.Count = itemData.Count;
                    cardData.Count = cardData.itemData.Count;
                }
            }
            if(cardData == null)
            {
                Logger.LogErrorFormat("CreateEnchantmentsCardFromNet guid = {0} failed!", guid);
            }
            return cardData;
        }

        public string GetAttributesDesc(int iID)
        {
            string ret = "";
            var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>(iID);
            if(magicItem == null)
            {
                return ret;
            }

            return MagicCardAndBeadCardGetAttributesDesc(magicItem.PropType,magicItem.PropValue,magicItem.BuffID,magicItem.SkillAttributes,ret);
        }

       public string MagicCardAndBeadCardGetAttributesDesc(IList<int> iPropType, IList<int> iPropValue, IList<int> iBuffID,string sStrSkillAttributes, string ret,bool mProminentAtt = false,bool isSubstitutionBox = false)
        {

            bool bHasContent = false;

            var kStringBuilder = StringBuilderCache.Acquire();
            kStringBuilder.Clear();
            if (iPropType.Count == iPropValue.Count)
            {
                if (iPropType.Count > 0)
                {
                    kStringBuilder.AppendFormat("<color={0}>", TR.Value("enchant_attribute_color"));
                }
                var attrs = iPropValue;
                for (int i = 0; i < iPropType.Count; ++i)
                {
                    if (attrs[i] != 0)
                    {
                        EServerProp eEServerProp = (EServerProp)iPropType[i];
                        MapEnum mapEnum = Utility.GetEnumAttribute<EServerProp, MapEnum>(eEServerProp);

                        if (mapEnum == null && iPropType[i] >= 18 && iPropType[i] <= 21)
                        {
                            mapEnum = new MapEnum((EEquipProp)(iPropType[i] - 18));
                        }

                        if (mapEnum == null && iPropType[i] >= (int)EServerProp.IRP_LIGHT && iPropType[i] <= (int)EServerProp.IRP_DARK)
                        {
                            mapEnum = new MapEnum(EEquipProp.Elements);
                        }

                        if (mapEnum != null)
                        {
                            EEquipProp eEEquipProp = mapEnum.Prop;
                            string strValue = Utility.GetEEquipProDesc(eEEquipProp, attrs[i]);

                            if (bHasContent)
                            {
                                kStringBuilder.Append("\n");
                            }
                            kStringBuilder.Append(strValue);
                            bHasContent = true;
                        }
                    }
                }

                if (iPropType.Count > 0)
                {
                    kStringBuilder.Append("</color>");
                }
            }

            if (!string.IsNullOrEmpty(sStrSkillAttributes))
            {
                if (bHasContent)
                {
                    kStringBuilder.Append("\n");
                }
                kStringBuilder.AppendFormat("<color={0}>{1}</color>", TR.Value("enchant_skills_color"), sStrSkillAttributes);
                bHasContent = true;
            }

            //本职业属性要突出显示的buffID
            int iBuffId = 0;
            //本职业属性突出显示为绿色  其他显示灰色
            //现用于称号宝珠区分职业加成技能
            if (mProminentAtt == true)
            {
                if (BeadCardManager.GetInstance().FindProminentAttID(iBuffID, ref iBuffId))
                {
                    var bufferitem = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(iBuffId);
                    if (null != bufferitem)
                    {
                        if (bufferitem.Description.Count > 0)
                        {
                            if (bHasContent)
                            {
                                kStringBuilder.Append("\n");
                            }
                            kStringBuilder.AppendFormat("<color={0}>{1}</color>", TR.Value("enchant_skills_color"), bufferitem.Description[0]);
                            bHasContent = true;
                        }
                    }
                }

                //没有打开替换宝珠界面
                if (!isSubstitutionBox)
                {
                    for (int i = 0; i < iBuffID.Count; ++i)
                    {
                        if (iBuffId == iBuffID[i])
                        {
                            continue;
                        }

                        var bufferitem = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(iBuffID[i]);
                        if (null != bufferitem)
                        {
                            if (bufferitem.Description.Count > 0)
                            {
                                if (bHasContent)
                                {
                                    kStringBuilder.Append("\n");
                                }
                                kStringBuilder.AppendFormat(TR.Value("tip_color_gray2", bufferitem.Description[0]));
                                bHasContent = true;
                            }
                        }
                    }
                }
              
            }
            else
            {
                for (int i = 0; i < iBuffID.Count; ++i)
                {
                    var bufferitem = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(iBuffID[i]);
                    if (null != bufferitem)
                    {
                        if (bufferitem.Description.Count > 0)
                        {
                            if (bHasContent)
                            {
                                kStringBuilder.Append("\n");
                            }
                            kStringBuilder.AppendFormat("<color={0}>{1}</color>", TR.Value("enchant_skills_color"), bufferitem.Description[0]);
                            bHasContent = true;
                        }
                    }
                }
            }
            
            ret = kStringBuilder.ToString();
            StringBuilderCache.Release(kStringBuilder);
            return ret;
        }

        public string GetAttributesDesc(EnchantmentsCardData data)
        {
            return GetAttributesDesc(data.magicItem.ID);
        }

        public string GetCondition(int iMagicCard)
        {
            var kStringBuilder = StringBuilderCache.Acquire();
            kStringBuilder.AppendFormat("<color={0}>", TR.Value("enchant_condition_color"));
            //kStringBuilder.Append(TR.Value("enchantments_part"));
            //kStringBuilder.Append("(");

            var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>(iMagicCard);
            if(magicItem != null)
            {
                for (int i = 0; i < magicItem.Parts.Count; ++i)
                {
                    var describe = Utility.GetEnumDescription((EEquipWearSlotType)magicItem.Parts[i]);
                    kStringBuilder.Append(TR.Value(describe));
                    if (i != magicItem.Parts.Count - 1)
                    {
                        kStringBuilder.Append("、");
                    }
                }
            }

            //kStringBuilder.Append(")");
            kStringBuilder.Append("</color>");
            string ret = kStringBuilder.ToString();
            StringBuilderCache.Release(kStringBuilder);

            return ret;
        }

        public string GetCondition(EnchantmentsCardData data)
        {
            var kStringBuilder = StringBuilderCache.Acquire();
            kStringBuilder.Clear();
            kStringBuilder.AppendFormat("<color={0}>", TR.Value("enchant_condition_color"));
            //kStringBuilder.Append(TR.Value("enchantments_part"));
            //kStringBuilder.Append("(");

            for (int i = 0; i < data.magicItem.Parts.Count; ++i)
            {
                var describe = Utility.GetEnumDescription((EEquipWearSlotType)data.magicItem.Parts[i]);
                kStringBuilder.Append(TR.Value(describe));
                if (i != data.magicItem.Parts.Count - 1)
                {
                    kStringBuilder.Append("、");
                }
            }

            //kStringBuilder.Append(")");
            kStringBuilder.Append("</color>");
            string ret = kStringBuilder.ToString();
            StringBuilderCache.Release(kStringBuilder);

            return ret;
        }

        public string GetDefaultDescribe(EnchantmentsCardData data)
        {
            string condition = GetCondition(data);
            string attrDesc = GetEnchantmentCardAttributesDesc(data.magicItem.ID,data.itemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
            string ret = condition;
            if(!string.IsNullOrEmpty(attrDesc) && !string.IsNullOrEmpty(ret))
            {
                ret += "\n" + attrDesc;
                return ret;
            }

            if(!string.IsNullOrEmpty(attrDesc))
            {
                return attrDesc;
            }

            return ret;
        }
        #endregion

        #region netmsg
        public void SendAddMagic(ulong cardid,ulong itemid)
        {
            SceneAddMagicReq kCmd = new SceneAddMagicReq();
            var leftItem = ItemDataManager.GetInstance().GetItem(cardid);
            var rightItem = ItemDataManager.GetInstance().GetItem(itemid);
            if(leftItem != null && rightItem != null)
            {
                kCmd.cardUid = leftItem.GUID;
                kCmd.itemUid = rightItem.GUID;
                kCmd.itemUid1 = rightItem.GUID;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kCmd);
            }
        }
        public void SendMergeCard(ulong leftcardid, ulong rightcardid)
        {
            SceneMagicCardCompReq kCmd = new SceneMagicCardCompReq();
            var leftItem = ItemDataManager.GetInstance().GetItem(leftcardid);
            var rightItem = ItemDataManager.GetInstance().GetItem(rightcardid);
            if (leftItem != null && rightItem != null)
            {
                kCmd.cardA = leftItem.GUID;
                kCmd.cardB = rightItem.GUID;
                // 附魔卡合成flag=0
                kCmd.flag = 0;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kCmd);
            }
        }

        //[MessageHandle(SceneMagicCardCompRet.MsgID)]
        void OnRecvSceneMagicCardCompRet(MsgDATA msgData)
        {
            SceneMagicCardCompRet kRet = new SceneMagicCardCompRet();
            kRet.decode(msgData.bytes);

            if(kRet.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)kRet.code);
            }
            else
            {
                if (kRet.cardLev == 99)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnBxyMergeSuccess);
                }
                else if (kRet.cardLev == 100)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSinanSuccess);
                }
                else if (kRet.cardLev == 101)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnEquipJichengSuccess);
                }
                else
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMergeSuccess);
                }

                EnchantResultFrame.EnchantResultFrameData data = new EnchantResultFrame.EnchantResultFrameData();
                data.bMerge = true;
                data.iCardTableID = (int)kRet.cardId;
                data.iCardLevel = kRet.cardLev;
                data.itemData = GameClient.ItemDataManager.CreateItemDataFromTable((int)kRet.cardId);

                if (ClientSystemManager.GetInstance().IsFrameOpen<EnchantResultFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<EnchantResultFrame>();
                }
                ClientFrame.OpenTargetFrame<EnchantResultFrame>(FrameLayer.Middle, data);
            }
        }

        //[MessageHandle(SceneAddMagicRet.MsgID)]
        void OnRecvSceneAddMagicRet(MsgDATA msgData)
        {
            SceneAddMagicRet kRet = new SceneAddMagicRet();
            kRet.decode(msgData.bytes);

            if (kRet.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)kRet.code);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAddMagicSuccess);

                EnchantResultFrame.EnchantResultFrameData data = new EnchantResultFrame.EnchantResultFrameData();
                data.bMerge = false;
                data.iCardTableID = (int)kRet.cardId;
                data.iCardLevel = kRet.cardLev;
                data.itemData = ItemDataManager.GetInstance().GetItem(kRet.itemUid);

                if (ClientSystemManager.GetInstance().IsFrameOpen<EnchantResultFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<EnchantResultFrame>();
                }
                ClientFrame.OpenTargetFrame<EnchantResultFrame>(FrameLayer.Middle, data);
            }
        }
        #endregion

        #region EnchantmentCardUpgrade

        /// <summary>
        /// 附魔卡阶段类型
        /// </summary>
        public enum EnchantmentCardStageType
        {
            All = 0,
            One,
            Two,
            Three,
            Count
        }

        /// <summary>
        /// 附魔卡品质Tab数据
        /// </summary>
        public class EnchantmentCardQualityTabData : ComControlData
        {
            public EnchantmentCardQualityTabData(int index, int id, string name, bool isSelected)
                : base(index, id, name, isSelected)
            {

            }
        }

        /// <summary>
        /// 附魔卡阶段Tab数据
        /// </summary>
        public class EnchantmentCardStageTabData : ComControlData
        {
            public EnchantmentCardStageTabData(int index, int id, string name, bool isSelected)
                : base(index, id, name, isSelected)
            {

            }
        }

        private void InitMagicCardUpgradeTable()
        {
            if (mMagicCardUpgradeTableList == null)
                mMagicCardUpgradeTableList = new List<MagicCardUpgradeTable>();

            mMagicCardUpgradeTableList.Clear();

            var enumerator = TableManager.GetInstance().GetTable<MagicCardUpgradeTable>().GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current.Value as MagicCardUpgradeTable;
                if (current == null)
                {
                    continue;
                }

                mMagicCardUpgradeTableList.Add(current);
            }
        }

        private void InitMagicCardProbabilityTable()
        {
            mEnchantmentCardProbabilityDataList.Clear();
            var mMagicCardProbabilityTable = TableManager.GetInstance().GetTable<MagicCardProbabilityTable>().GetEnumerator();
            while (mMagicCardProbabilityTable.MoveNext())
            {
                var current = mMagicCardProbabilityTable.Current.Value as MagicCardProbabilityTable;
                if (current == null)
                {
                    continue;
                }

                EnchantmentCardProbabilityData data = new EnchantmentCardProbabilityData();
                data.iMinProbability = current.MinProbability;
                data.iMaxProbability = current.MaxProbability;
                data.sSuccessRateDesc = current.SuccessName;

                mEnchantmentCardProbabilityDataList.Add(data);
            }
        }

        /// <summary>
        /// 得到附魔卡（副卡）成功率
        /// </summary>
        /// <returns></returns>
        public string GetEnchantmentCardProbabilityDesc(int currentRate)
        {
            string rateDesc = "";
            for (int i = 0; i < mEnchantmentCardProbabilityDataList.Count; i++)
            {
                var data = mEnchantmentCardProbabilityDataList[i];

                if (data.iMinProbability < currentRate && currentRate <= data.iMaxProbability)
                {
                    return data.sSuccessRateDesc;
                }
            }

            if (mEnchantmentCardProbabilityDataList.Count > 0)
                rateDesc = mEnchantmentCardProbabilityDataList[0].sSuccessRateDesc;

            return rateDesc;
        }

        /// <summary>
        /// 得到附魔卡列表
        /// </summary>
        /// <returns></returns>
        public List<EnchantmentCardItemDataModel> LoadEnchantmentCardItems()
        {
            List<EnchantmentCardItemDataModel> mEnchantmentCardItemList = new List<EnchantmentCardItemDataModel>();

            List<EnchantmentCardItemDataModel> mEquipEnchantmentCardItems = AddEnchantmentCardItems(EPackageType.Equip);
            List<EnchantmentCardItemDataModel> mWearEquipEnchantmentCardItems = AddEnchantmentCardItems(EPackageType.WearEquip);
            List<EnchantmentCardItemDataModel> mEnchantmentCardItems = AddEnchantmentCardItems(EPackageType.Consumable);
            mEnchantmentCardItemList.AddRange(mEquipEnchantmentCardItems);
            mEnchantmentCardItemList.AddRange(mWearEquipEnchantmentCardItems);
            mEnchantmentCardItemList.Sort(SortEnchantmentCardItems);
            mEnchantmentCardItems.Sort(SortEnchantmentCardItems);
            mEnchantmentCardItemList.AddRange(mEnchantmentCardItems);

            return mEnchantmentCardItemList;
        }

        public List<EnchantmentCardItemDataModel> AddEnchantmentCardItems(EPackageType ePackageType)
        {
            List<EnchantmentCardItemDataModel> mEnchantmentCardItems = new List<EnchantmentCardItemDataModel>();

            var uids = ItemDataManager.GetInstance().GetItemsByPackageType(ePackageType);

            if (ePackageType == EPackageType.Equip || ePackageType == EPackageType.WearEquip)
            {
                for (int i = 0; i < uids.Count; i++)
                {
                    ItemData mEquipItemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (mEquipItemData == null)
                    {
                        continue;
                    }

                    if (mEquipItemData.mPrecEnchantmentCard == null)
                    {
                        continue;
                    }

                    if (mEquipItemData.mPrecEnchantmentCard.iEnchantmentCardID == 0)
                    {
                        continue;
                    }

                    var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)mEquipItemData.mPrecEnchantmentCard.iEnchantmentCardID);
                    if (itemTable == null)
                    {
                        continue;
                    }

                    var magicCardTable = TableManager.GetInstance().GetTableItem<MagicCardTable>((int)mEquipItemData.mPrecEnchantmentCard.iEnchantmentCardID);
                    if (magicCardTable == null)
                    {
                        continue;
                    }

                    if (magicCardTable.MaxLevel <= 0)
                    {
                        continue;
                    }

                    EnchantmentCardItemDataModel enchantmentCardItem = new EnchantmentCardItemDataModel();
                    enchantmentCardItem.mUpgradePrecType = UpgradePrecType.Mounted;
                    enchantmentCardItem.mEquipItemData = mEquipItemData;
                    enchantmentCardItem.mEnchantmentCardItemData = ItemDataManager.CreateItemDataFromTable((int)mEquipItemData.mPrecEnchantmentCard.iEnchantmentCardID);
                    enchantmentCardItem.mEnchantmentCardItemData.mPrecEnchantmentCard = mEquipItemData.mPrecEnchantmentCard;
                    enchantmentCardItem.mEchantmentCardQuality = magicCardTable.Color;
                    enchantmentCardItem.mEnchantmentCardStage = magicCardTable.Stage;
                    enchantmentCardItem.mSameCardList = LoadSameMagicCardList(magicCardTable.SameCardID);

                    var upgradeTable = FindMagicCardUpgradeTable(magicCardTable.MagicCardTableID, enchantmentCardItem.mEnchantmentCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
                    if (upgradeTable != null)
                    {
                        enchantmentCardItem.mEnchantmentCardCostMaxNum = upgradeTable.MaxNum;
                        enchantmentCardItem.mConsumableMaterialData = new ItemSimpleData();
                        enchantmentCardItem.mConsumableMaterialData.ItemID = upgradeTable.CostItemId;
                        enchantmentCardItem.mConsumableMaterialData.Count = upgradeTable.CostNum;
                    }

                    mEnchantmentCardItems.Add(enchantmentCardItem);
                }
            }
            else
            {
                for (int i = 0; i < uids.Count; i++)
                {
                    ItemData mEnchantmentCardItemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (mEnchantmentCardItemData == null)
                    {
                        continue;
                    }

                    if (mEnchantmentCardItemData.SubType != (int)ItemTable.eSubType.EnchantmentsCard)
                    {
                        continue;
                    }

                    var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)mEnchantmentCardItemData.TableID);
                    if (itemTable == null)
                    {
                        continue;
                    }

                    var magicCardTable = TableManager.GetInstance().GetTableItem<MagicCardTable>((int)mEnchantmentCardItemData.TableID);
                    if (magicCardTable == null)
                    {
                        continue;
                    }

                    if (magicCardTable.MaxLevel <= 0)
                    {
                        continue;
                    }

                    EnchantmentCardItemDataModel enchantmentCardItem = new EnchantmentCardItemDataModel();
                    enchantmentCardItem.mUpgradePrecType = UpgradePrecType.UnMounted;
                    enchantmentCardItem.mEquipItemData = null;
                    enchantmentCardItem.mEnchantmentCardItemData = mEnchantmentCardItemData;
                    enchantmentCardItem.mEnchantmentCardItemData.mPrecEnchantmentCard.iEnchantmentCardID = mEnchantmentCardItemData.TableID;
                    enchantmentCardItem.mEnchantmentCardCount = mEnchantmentCardItemData.Count;
                    enchantmentCardItem.mEchantmentCardQuality = magicCardTable.Color;
                    enchantmentCardItem.mEnchantmentCardStage = magicCardTable.Stage;
                    enchantmentCardItem.mSameCardList = LoadSameMagicCardList(magicCardTable.SameCardID);

                    var upgradeTable = FindMagicCardUpgradeTable(magicCardTable.MagicCardTableID, enchantmentCardItem.mEnchantmentCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
                    if (upgradeTable != null)
                    {
                        enchantmentCardItem.mEnchantmentCardCostMaxNum = upgradeTable.MaxNum;
                        enchantmentCardItem.mConsumableMaterialData = new ItemSimpleData();
                        enchantmentCardItem.mConsumableMaterialData.ItemID = upgradeTable.CostItemId;
                        enchantmentCardItem.mConsumableMaterialData.Count = upgradeTable.CostNum;
                    }

                    mEnchantmentCardItems.Add(enchantmentCardItem);
                }
            }

            return mEnchantmentCardItems;
        }

        private List<int> LoadSameMagicCardList(IList<string> list)
        {
            List<int> dataList = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                string id = list[i];

                int cardId = 0;
                if(int.TryParse(id,out cardId))
                {
                    dataList.Add(cardId);
                }
            }

            return dataList;
        }

        /// <summary>
        /// 找附魔卡升级表新数据
        /// </summary>
        /// <param name="magicCardTableId"></param>
        /// <param name="magicCardLevel"></param>
        /// <returns></returns>
        private MagicCardUpgradeTable FindMagicCardUpgradeTable(int magicCardTableId,int magicCardLevel)
        {
            for (int i = 0; i < mMagicCardUpgradeTableList.Count; i++)
            {
                var data = mMagicCardUpgradeTableList[i];
                if (data == null)
                    continue;

                if (data.MagicCardTableID != magicCardTableId)
                    continue;

                if ((data.Level - 1) != magicCardLevel)
                    continue;

                return data;
            }

            return null;
        }

        /// <summary>
        /// 附魔卡品质页签数据
        /// </summary>
        /// <returns></returns>
        public List<ComControlData> GetEnchantmentCardQualityTabDataList()
        {
            List<ComControlData> tabDataList = new List<ComControlData>();

            for (int i = 0; i < (int)ItemTable.eColor.YELLOW; i++)
            {
                if (i == (int)ItemTable.eColor.GREEN)
                {
                    continue;
                }
                
                int index = i;
                string name = GetQualityName((ItemTable.eColor)i);
                EnchantmentCardQualityTabData tabData = new EnchantmentCardQualityTabData(index, index, name, index == 0);
                tabDataList.Add(tabData);
            }

            return tabDataList;
        }

        /// <summary>
        /// 附魔卡阶段页签数据
        /// </summary>
        /// <returns></returns>
        public List<ComControlData> GetEnchantmentCardStageTabDataList()
        {
            List<ComControlData> tabDataList = new List<ComControlData>();

            for (int i = 0; i < (int)EnchantmentCardStageType.Count; i++)
            {
                int index = i;
                string name = GetStageName((EnchantmentCardStageType)i);
                EnchantmentCardStageTabData tabData = new EnchantmentCardStageTabData(index, index, name, index == 0);
                tabDataList.Add(tabData);
            }

            return tabDataList;
        }

        public static string GetQualityName(ItemTable.eColor color)
        {
            string name = string.Empty;
            switch (color)
            {
                case ItemTable.eColor.CL_NONE:
                    name = "全部品质";
                    break;
                case ItemTable.eColor.WHITE:
                    name = TR.Value("ItemTable.eColor.WHITE");
                    break;
                case ItemTable.eColor.BLUE:
                    name = TR.Value("ItemTable.eColor.BLUE");
                    break;
                case ItemTable.eColor.PURPLE:
                    name = TR.Value("ItemTable.eColor.PURPLE");
                    break;
                case ItemTable.eColor.GREEN:
                    name = TR.Value("ItemTable.eColor.GREEN");
                    break;
                case ItemTable.eColor.PINK:
                    name = TR.Value("ItemTable.eColor.PINK");
                    break;
                case ItemTable.eColor.YELLOW:
                    name = TR.Value("ItemTable.eColor.YELLOW");
                    break;
            }

            return name;
        }

        private string GetStageName(EnchantmentCardStageType stageType)
        {
            string name = string.Empty;
            switch (stageType)
            {
                case EnchantmentCardStageType.All:
                    name = TR.Value("enchantmentCard_Stage_All");
                    break;
                case EnchantmentCardStageType.One:
                    name = TR.Value("enchantmentCard_Stage_One");
                    break;
                case EnchantmentCardStageType.Two:
                    name = TR.Value("enchantmentCard_Stage_Two");
                    break;
                case EnchantmentCardStageType.Three:
                    name = TR.Value("enchantmentCard_Stage_Three");
                    break;
                case EnchantmentCardStageType.Count:
                    break;
            }

            return name;
        }

        /// <summary>
        /// 根据主卡得到附魔卡升级表格数据
        /// </summary>
        /// <param name="MainCardItemData"></param>
        /// <returns></returns>
        public MagicCardUpdateTable GetMagicCardUpdateTable(ItemData MainCardItemData)
        {
            MagicCardUpdateTable MainMagicCardUpdateTable = null;

            var enumerator = TableManager.GetInstance().GetTable<MagicCardUpdateTable>().GetEnumerator();
            while (enumerator.MoveNext())
            {
                var table = enumerator.Current.Value as MagicCardUpdateTable;
                if (table == null)
                {
                    continue;
                }

                if (table.MagicCardID != MainCardItemData.mPrecEnchantmentCard.iEnchantmentCardID)
                {
                    continue;
                }

                if (MainCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel < table.MinLevel)
                {
                    continue;
                }

                if (MainCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel > table.MaxLevel)
                {
                    continue;
                }

                MainMagicCardUpdateTable = table;
                break;
            }

            return MainMagicCardUpdateTable;
        }

        /// <summary>
        /// 得到副卡列表
        /// </summary>
        /// <param name="cardItemDataModel">主卡数据</param>
        /// <returns></returns>
        public List<EnchantmentCardViceCardData> GetEnchantmentCardViceCardDatas(EnchantmentCardItemDataModel cardItemDataModel)
        {
            if (cardItemDataModel == null)
                return null;

            if (cardItemDataModel.mEnchantmentCardItemData == null)
                return null;
            
            ////副卡列表
            List<EnchantmentCardViceCardData> viceCardDatas = new List<EnchantmentCardViceCardData>();
            var uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);
            for (int i = 0; i < uids.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.SubType != (int)ItemTable.eSubType.EnchantmentsCard)
                {
                    continue;
                }

                if (itemData.mPrecEnchantmentCard == null)
                    continue;
                
                bool isSameCard = false;//是否是同名卡

                if(cardItemDataModel.mSameCardList != null)
                {
                    int sameCardId = cardItemDataModel.mSameCardList.Find(x => { return x == itemData.TableID; });//判断副卡跟主卡是否是同名卡（sameCardId == 0表示不是同名卡）
                    if(sameCardId != 0)
                    {
                        isSameCard = true;
                    }
                }
                
                if (isSameCard && itemData.mPrecEnchantmentCard.iEnchantmentCardLevel > cardItemDataModel.mEnchantmentCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel)
                {
                    continue;
                }
                
                var ViceCardMagicCardTable = TableManager.GetInstance().GetTableItem<MagicCardTable>(itemData.TableID);
                if (ViceCardMagicCardTable == null)
                {
                    continue; 
                }

                //不是同名卡再判断是否是同档次的附魔卡
                if (!isSameCard)
                {
                    if (cardItemDataModel.mEchantmentCardQuality != ViceCardMagicCardTable.Color)
                        continue;

                    if (cardItemDataModel.mEnchantmentCardStage != ViceCardMagicCardTable.Stage)
                        continue;
                }

                EnchantmentCardViceCardData viceCardData = new EnchantmentCardViceCardData();

                viceCardData.mViceCardItemData = itemData;
                viceCardData.mViceCardItemData.mPrecEnchantmentCard.iEnchantmentCardID = itemData.TableID;

                MagicCardUpgradeTable magicCardUpgradeTable = FindMagicCardUpgradeTable(ViceCardMagicCardTable.MagicCardTableID, itemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
                if(magicCardUpgradeTable == null)
                {
                    continue;
                }

                //是同名卡使用同名卡概率
                if(isSameCard)
                {
                    viceCardData.mAllSuccessRate = magicCardUpgradeTable.SameCardAddRate;
                }
                else
                {
                    viceCardData.mAllSuccessRate = magicCardUpgradeTable.SameColorAndStageCardAddRate;
                }

                if (cardItemDataModel.mUpgradePrecType == UpgradePrecType.Mounted)
                {
                    viceCardData.mViceCardCount = itemData.Count;
                }
                else
                {
                    if (cardItemDataModel.mEnchantmentCardItemData.GUID == itemData.GUID)
                    {
                        viceCardData.mViceCardCount = itemData.Count - 1;
                    }
                    else
                    {
                        viceCardData.mViceCardCount = itemData.Count;
                    }
                }

                if (viceCardData.mViceCardCount <= 0)
                {
                    continue;
                }

                viceCardDatas.Add(viceCardData);
            }

            return viceCardDatas;
        }
      
        int SortEnchantmentCardItems(EnchantmentCardItemDataModel left, EnchantmentCardItemDataModel right)
        {
            if (left.mEnchantmentCardItemData.Quality != right.mEnchantmentCardItemData.Quality)
            {
                return (int)right.mEnchantmentCardItemData.Quality - (int)left.mEnchantmentCardItemData.Quality;
            }
            return right.mEnchantmentCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel - left.mEnchantmentCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel;
        }

        /// <summary>
        /// 检查附魔卡等级是否满级
        /// </summary>
        /// <param name="currentEnchantmentCardItemData"></param>
        /// <returns></returns>
        public bool CheckEnchantmentCardLevelIsFull(EnchantmentCardItemDataModel currentEnchantmentCardData)
        {
            bool isFullLevel = false;
            if (currentEnchantmentCardData == null)
            {
                isFullLevel = false;
            }

            int magicCardID = 0;
            if (currentEnchantmentCardData.mUpgradePrecType == UpgradePrecType.Mounted)
            {
                magicCardID = currentEnchantmentCardData.mEnchantmentCardItemData.mPrecEnchantmentCard.iEnchantmentCardID;
            }
            else
            {
                magicCardID = currentEnchantmentCardData.mEnchantmentCardItemData.TableID;
            }

            var magicCardTable = TableManager.GetInstance().GetTableItem<MagicCardTable>(magicCardID);
            if (magicCardTable == null)
            {
                isFullLevel = false;
            }

            if (currentEnchantmentCardData.mEnchantmentCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel >= magicCardTable.MaxLevel)
            {
                isFullLevel = true;
            }

            return isFullLevel;
        }

        /// <summary>
        /// 根据附魔卡等级得到附魔卡属性描述 isShowCeil 是否显示上限
        /// </summary>
        /// <returns></returns>
        public string GetEnchantmentCardAttributesDesc(int enchantmentCardID,int enchantmentCardLevel,bool isShowCeil = false)
        {
            string attr = "";
            var magicCardTable = TableManager.GetInstance().GetTableItem<MagicCardTable>(enchantmentCardID);
            if (magicCardTable == null)
            {
                return attr;
            }

            bool bHasContent = false;
            var stringBuilder = StringBuilderCache.Acquire();
            stringBuilder.Clear();

            if (magicCardTable.PropType.Count == magicCardTable.PropValue.Count)
            {
                if (magicCardTable.PropType.Count > 0)
                {
                    stringBuilder.AppendFormat("<color={0}>", TR.Value("enchant_attribute_color"));
                }

                var attrs = magicCardTable.PropValue;
                var upValues = magicCardTable.UpValue;
                for (int i = 0; i < magicCardTable.PropType.Count; i++)
                {
                    if (attrs[i] != 0)
                    {
                        EServerProp eEServerProp = (EServerProp)magicCardTable.PropType[i];
                        MapEnum mapEnum = Utility.GetEnumAttribute<EServerProp, MapEnum>(eEServerProp);

                        if (mapEnum == null && magicCardTable.PropType[i] >= 18 && magicCardTable.PropType[i] <= 21)
                        {
                            mapEnum = new MapEnum((EEquipProp)(magicCardTable.PropType[i] - 18));
                        }

                        if (mapEnum == null && magicCardTable.PropType[i] >= (int)EServerProp.IRP_LIGHT && magicCardTable.PropType[i] <= (int)EServerProp.IRP_DARK)
                        {
                            mapEnum = new MapEnum(EEquipProp.Elements);
                        }

                        if (mapEnum != null)
                        {
                            EEquipProp eEEquipProp = mapEnum.Prop;
                            //根据等级算出最终值
                            int attrValue = attrs[i] + upValues[i] * enchantmentCardLevel;
                            string strValue = Utility.GetEEquipProDesc(eEEquipProp, attrValue);

                            if (bHasContent)
                            {
                                stringBuilder.Append("\n");
                            }
                            stringBuilder.Append(strValue);
                            //显示上限值
                            if (isShowCeil == true)
                            {
                                if (magicCardTable.MaxLevel > 0 && enchantmentCardLevel < magicCardTable.MaxLevel)
                                {
                                    int ceilValue = attrs[i] + upValues[i] * magicCardTable.MaxLevel;
                                    string strCeilVlue = Utility.GetEquipProCeilValueDesc(eEEquipProp, ceilValue);
                                    stringBuilder.AppendFormat("<color={0}>(上限：{1})</color>", TR.Value("tip_color_normal_noparm"), strCeilVlue);
                                }
                            }
                            bHasContent = true;
                        }
                    }
                }

                if (magicCardTable.PropType.Count > 0)
                {
                    stringBuilder.Append("</color>");
                }
            }

            if (!string.IsNullOrEmpty(magicCardTable.SkillAttributes))
            {
                if (bHasContent)
                {
                    stringBuilder.Append("\n");
                }
                stringBuilder.AppendFormat("<color={0}>{1}</color>", TR.Value("enchant_skills_color"), magicCardTable.SkillAttributes);
                bHasContent = true;
            }

            if (enchantmentCardLevel > 0)
            {
                var bufferitem = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(magicCardTable.UpBuffID[enchantmentCardLevel - 1]);
                if (null != bufferitem)
                {
                    if (bufferitem.Description.Count > 0)
                    {
                        if (bHasContent)
                        {
                            stringBuilder.Append("\n");
                        }
                        stringBuilder.AppendFormat("<color={0}>{1}</color>", TR.Value("enchant_skills_color"), bufferitem.Description[0]);
                        bHasContent = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < magicCardTable.BuffID.Count; i++)
                {
                    var bufferitem = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(magicCardTable.BuffID[i]);
                    if (null != bufferitem)
                    {
                        if (bufferitem.Description.Count > 0)
                        {
                            if (bHasContent)
                            {
                                stringBuilder.Append("\n");
                            }
                            stringBuilder.AppendFormat("<color={0}>{1}</color>", TR.Value("enchant_skills_color"), bufferitem.Description[0]);
                            bHasContent = true;
                        }
                    }
                }
            }

            attr = stringBuilder.ToString();
            StringBuilderCache.Release(stringBuilder);
            return attr;
        }

        /// <summary>
        /// 附魔卡升级请求
        /// </summary>
        /// <param name="enchantmentCardItem">主卡数据</param>
        /// <param name="viceCardItem">副卡数据</param>
        public void OnSceneMagicCardUpgradeReq(EnchantmentCardItemDataModel enchantmentCardItem, List<EnchantmentCardViceCardData> viceCardItems)
        {
            if (enchantmentCardItem == null || viceCardItems == null || viceCardItems.Count <= 0)
            {
                return;
            }

            if (enchantmentCardItem.mUpgradePrecType == UpgradePrecType.Mounted && enchantmentCardItem.mEquipItemData == null)
            {
                Logger.LogErrorFormat("EnchantmentsCardManager [OnSceneMagicCardUpgradeReq] enchantmentCardItem.mEquipItemData = null");
                return;
            }

            if (enchantmentCardItem.mEnchantmentCardItemData == null)
            {
                Logger.LogErrorFormat("EnchantmentsCardManager [OnSceneMagicCardUpgradeReq] enchantmentCardItem.mEnchantmentCardItemData = null");
                return;
            }
            
            SceneMagicCardUpgradeReq req = new SceneMagicCardUpgradeReq();
            //如果是镶嵌身上的附魔卡
            if (enchantmentCardItem.mUpgradePrecType == UpgradePrecType.Mounted)
            {
                req.equipUid = enchantmentCardItem.mEquipItemData.GUID;
                req.cardId = (uint)enchantmentCardItem.mEnchantmentCardItemData.TableID;
            }
            else
            {
                req.upgradeUid = enchantmentCardItem.mEnchantmentCardItemData.GUID;
            }
            
            ulong[] tempMaterialItemVec = new ulong[viceCardItems.Count];
            
            for (int i = 0; i < viceCardItems.Count; i ++)
            {
                var viceCardData = viceCardItems[i];
                if (viceCardData == null)
                    continue;

                var viceCardItemData = viceCardData.mViceCardItemData;
                if (viceCardItemData == null)
                    continue;

                tempMaterialItemVec[i] = viceCardItemData.GUID;
            }

            req.materialItemVec = tempMaterialItemVec;

            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 升级附魔卡返回
        /// </summary>
        private void OnSceneMagicCardUpgradeRes(MsgDATA msg)
        {
            SceneMagicCardUpgradeRes res = new SceneMagicCardUpgradeRes();
            res.decode(msg.bytes);

            EnchantmentCardUpgradeSuccessData data = null;

            if (res.code != 0)
            {
                if (res.code == (int)ProtoErrorCode.ITEM_MAGIC_UP_FAIL)
                {
                    data = new EnchantmentCardUpgradeSuccessData();
                    data.isSuccess = false;
                    data.mEnchantmentCardID = (int)res.cardTypeId;
                    data.mEnchantmentCardLevel = res.cardLev;

                    OpenEquipUpgradeResutFrame(data);
                }
                else
                {
                    SystemNotifyManager.SystemNotify((int)res.code);
                }
            }
            else
            {
                data = new EnchantmentCardUpgradeSuccessData();
                data.isSuccess = true;
                data.mEnchantmentCardID = (int)res.cardTypeId;
                data.mEnchantmentCardLevel = res.cardLev;
                data.mEnchantmentCardGUID = res.cardGuid;
                data.mEquipGUID = res.equipUid;

                OpenEquipUpgradeResutFrame(data);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnEnchantmentCardUpgradeRetun,data);
        }

        private void OpenEquipUpgradeResutFrame(EnchantmentCardUpgradeSuccessData mData)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<EquipUpgradeResultFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<EquipUpgradeResultFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<EquipUpgradeResultFrame>(FrameLayer.Middle, mData);
        }

        /// <summary>
        /// 检查附魔卡是否可以升级
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckEnchantmentCardIsUpgrade(ItemData item)
        {
            var magicCardTable = TableManager.GetInstance().GetTableItem<MagicCardTable>(item.TableID);
            if (magicCardTable != null)
            {
                return magicCardTable.MaxLevel > 0 && item.mPrecEnchantmentCard.iEnchantmentCardLevel < magicCardTable.MaxLevel;
            }

            return false;
        }


        /// <summary>
        /// 检查升级主卡是否只用同名卡为材料
        /// </summary>
        /// <returns></returns>
        public bool CheckMainEnchantmentCardIsOnlyUseSameCard(ItemData mainCard)
        {
            MagicCardUpdateTable MainMagicCardUpdateTable = GetMagicCardUpdateTable(mainCard);
            if (MainMagicCardUpdateTable == null)
            {
                return false;
            }

            if (MainMagicCardUpdateTable.UpgradeMaterials_1.Length > 0)
            {
                return false;
            }

            if (MainMagicCardUpdateTable.UpgradeMaterials_2.Length > 0)
            {
                return false;
            }

            if (MainMagicCardUpdateTable.UpgradeMaterials_3.Length > 0)
            {
                return false;
            }

            if (MainMagicCardUpdateTable.SameCardID.Length <= 0)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}