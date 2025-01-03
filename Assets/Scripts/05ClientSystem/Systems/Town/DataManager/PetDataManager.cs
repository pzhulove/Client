using Protocol;
using Network;
using System.Collections.Generic;
using ProtoTable;
using System;
using UnityEngine.UI;
using UnityEngine;

namespace GameClient
{
    public class PetDataManager : DataManager<PetDataManager>
    {
        bool m_bNetBind = false;

        float CheckHungerTime = 5.0f;
        int PetHungryLimit = 0;

        /// <summary>
        /// 未装备的宠物列表
        /// </summary>
        List<PetInfo> PetInfoList = new List<PetInfo>();

        /// <summary>
        /// 装备的宠物列表
        /// </summary>
        List<PetInfo> OnUsePetInfoList = new List<PetInfo>();

        PetInfo FollowPet = new PetInfo();
        PetInfo NewOpenPet = new PetInfo();

        bool bHasActivieFeedPet = false;

        float fTimeIntrval = 0.0f;

        private ulong selectPetIndex;
        public ulong SelectPetId
        {
            set { selectPetIndex = value; }
            get { return selectPetIndex; }
        }

        private PetInfo selectPetInfo;
        //不能直接使用，请使用SetPetData接口获取数据。
        public PetInfo SelectPetInfo
        {
            set { selectPetInfo = value; }
            get { return selectPetInfo; }
        }

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public override void Initialize()
        {
            Clear();
            _BindNetMsg();

            SystemValueTable valueData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PET_HUNGRY_LIMIT);
            if(valueData == null)
            {
                Logger.LogError("PetDataManager init failed : SystemValueTable data is null");
            }

            PetHungryLimit = valueData.Value;
        }

        public override void Clear()
        {
            _UnBindNetMsg();

            mIsUserClickFeedCount = false;
            mIsUserClickPetTab = false;
            mMaxGoldFeedNum = -1;

            PetInfoList.Clear();
            OnUsePetInfoList.Clear();

            SetPetData(FollowPet, new PetInfo());
            SetPetData(NewOpenPet, new PetInfo());

            bHasActivieFeedPet = false;
            fTimeIntrval = 0.0f;
            selectPetInfo = null;
        }

        public void ClearChijiPetData()
        {
            PetInfoList.Clear();
            OnUsePetInfoList.Clear();
            SetPetData(FollowPet, new PetInfo());
            SetPetData(NewOpenPet, new PetInfo());
        }

        void _BindNetMsg()
        {
            if (m_bNetBind == false)
            {            
                NetProcess.AddMsgHandler(SceneSyncPetList.MsgID, _OnNetSyncInitPetList);
                NetProcess.AddMsgHandler(SceneSyncPet.MsgID, _OnNetSyncUpdatePetList);
                NetProcess.AddMsgHandler(SceneSetPetFollowRes.MsgID, _OnNetSyncSetPetFollowRes);
                NetProcess.AddMsgHandler(SceneSetPetSoltRes.MsgID, _OnNetSyncSetPetSoltRes);
                NetProcess.AddMsgHandler(SceneFeedPetRes.MsgID, _OnNetSyncFeedPetRes);
                NetProcess.AddMsgHandler(SceneDeletePet.MsgID, _OnNetSyncDeletePetRes);                         
                NetProcess.AddMsgHandler(SceneSellPetRes.MsgID, _OnNetSyncSellPetRes);
                NetProcess.AddMsgHandler(SceneChangePetSkillRes.MsgID, _OnNetSyncChangePetSkillRes);
                NetProcess.AddMsgHandler(SceneDevourPetRes.MsgID, _OnNetSyncEatPetRes);
                
                m_bNetBind = true;
            }
        }

        void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(SceneSyncPetList.MsgID, _OnNetSyncInitPetList);
            NetProcess.RemoveMsgHandler(SceneSyncPet.MsgID, _OnNetSyncUpdatePetList);
            NetProcess.RemoveMsgHandler(SceneSetPetFollowRes.MsgID, _OnNetSyncSetPetFollowRes);
            NetProcess.RemoveMsgHandler(SceneSetPetSoltRes.MsgID, _OnNetSyncSetPetSoltRes);
            NetProcess.RemoveMsgHandler(SceneFeedPetRes.MsgID, _OnNetSyncFeedPetRes);
            NetProcess.RemoveMsgHandler(SceneDeletePet.MsgID, _OnNetSyncDeletePetRes);
            NetProcess.RemoveMsgHandler(SceneSellPetRes.MsgID, _OnNetSyncSellPetRes);
            NetProcess.RemoveMsgHandler(SceneChangePetSkillRes.MsgID, _OnNetSyncChangePetSkillRes);
            NetProcess.RemoveMsgHandler(SceneDevourPetRes.MsgID, _OnNetSyncEatPetRes);

            m_bNetBind = false;
        }

        void _OnNetSyncInitPetList(MsgDATA msg)
        {
            SceneSyncPetList msgData = new SceneSyncPetList();
            msgData.decode(msg.bytes);

            PetInfoList.Clear();
            OnUsePetInfoList.Clear();

            for(int i = 0; i < msgData.petList.Length; i++)
            {
                bool bFind = false;

                for(int j = 0; j < msgData.battlePets.Length; j++)
                {
                    if (msgData.petList[i].id == msgData.battlePets[j])
                    {
                        OnUsePetInfoList.Add(msgData.petList[i]);

                        bFind = true;
                        break;
                    }        
                }

                if(bFind)
                {
                    continue;
                }

                PetInfoList.Add(msgData.petList[i]);
            }

            for(int i = 0; i < OnUsePetInfoList.Count; i++)
            {
                if(OnUsePetInfoList[i].id == msgData.followPetId)
                {
                    SetPetData(FollowPet, OnUsePetInfoList[i]);
                    break;
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PetInfoInited);
        }

        void _OnNetSyncUpdatePetList(MsgDATA msg)
        {
			int pos = 0;
			Pet pet = PetDecoder.Decode(msg.bytes, ref pos, msg.bytes.Length);
        
			if(pet.id == FollowPet.id)
            {
				UpdateSyncPetData(FollowPet, pet);

				for (int i = 0; i < pet.dirtyFields.Count; ++i)
				{
					if (pet.dirtyFields[i] == (int)PetObjectAttr.POA_HUNGER)
					{
						UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FollowPetSatietyChanged);
					}
				}

			}

            bool bFind = false;
            for(int i = 0; i < OnUsePetInfoList.Count; i++)
            {
				if (OnUsePetInfoList[i].id == pet.id)
				{
					UpdateSyncPetData(OnUsePetInfoList[i], pet);
					bFind = true;
					break;
				}
            }

            if(!bFind)
            {
				for (int i = 0; i < PetInfoList.Count; i++)
				{
					if (PetInfoList[i].id == pet.id)
					{
						UpdateSyncPetData(PetInfoList[i], pet);
						bFind = true;
						break;
					}
				}
			}

            if(!bFind)
            {
				PetInfo petInfo = new PetInfo();
				petInfo.id = pet.id;
				petInfo.dataId = pet.dataId;
				UpdateSyncPetData(petInfo, pet);
				PetInfoList.Add(petInfo);
				SetPetData(NewOpenPet, petInfo);
			}

			UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PetItemsInfoUpdate);
        }

        void _OnNetSyncSetPetFollowRes(MsgDATA msg)
        {
            SceneSetPetFollowRes msgData = new SceneSetPetFollowRes();
            msgData.decode(msg.bytes);

            if((ProtoErrorCode)msgData.result != ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }

            if(msgData.petId <= 0)
            {
                SetPetData(FollowPet, new PetInfo());
            }
            else
            {
                for (int i = 0; i < OnUsePetInfoList.Count; i++)
                {
                    if (OnUsePetInfoList[i].id != msgData.petId)
                    {
                        continue;
                    }

                    SetPetData(FollowPet, OnUsePetInfoList[i]);

                    break;
                }
            }
            
            if(msgData.petId > 0)
            {
                SystemNotifyManager.SystemNotify(8508);
            }
        }

        void _OnNetSyncSetPetSoltRes(MsgDATA msg)
        {
            SceneSetPetSoltRes msgData = new SceneSetPetSoltRes();
            msgData.decode(msg.bytes);

            if ((ProtoErrorCode)msgData.result != ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }

            PetInfo NotUsePetInfo = null;
            PetInfo NewUsePetInfo = null;

            int iChangedSlotIndex = 0;
            bool bIsWear = false;

            for (int i = 0; i < OnUsePetInfoList.Count; i++)
            {
                bool bFind = false;

                for (int j = 0; j < msgData.battlePets.Length; j++)
                {
                    if(OnUsePetInfoList[i].id == msgData.battlePets[j])
                    {
                        bFind = true;
                        break;
                    }
                }

                if (OnUsePetInfoList[i].id == msgData.followPetId)
                {
                    SetPetData(FollowPet, OnUsePetInfoList[i]);
                }

                if (!bFind)
                {
                    NotUsePetInfo = new PetInfo();
                    SetPetData(NotUsePetInfo, OnUsePetInfoList[i]);
                    PetInfoList.Add(NotUsePetInfo);

                    PetTable NotUsePetData = TableManager.GetInstance().GetTableItem<PetTable>((int)NotUsePetInfo.dataId);
                    if(NotUsePetData == null)
                    {
                        continue;
                    }

                    iChangedSlotIndex = (int)NotUsePetData.PetType - 1;

                    break;
                }
            }

            for (int i = 0; i < msgData.battlePets.Length; i++)
            {
                bool bFind = false;
                int iIndex = 0;

                for (int j = 0; j < PetInfoList.Count; j++)
                {
                    if (msgData.battlePets[i] == PetInfoList[j].id)
                    {
                        bFind = true;
                        iIndex = j;

                        break;
                    }
                }

                if (bFind)
                {
                    if (PetInfoList[iIndex].id == msgData.followPetId)
                    {
                        SetPetData(FollowPet, PetInfoList[iIndex]);
                    }

                    NewUsePetInfo = new PetInfo();
                    SetPetData(NewUsePetInfo, PetInfoList[iIndex]);

                    OnUsePetInfoList.Add(NewUsePetInfo);
                    PetInfoList.RemoveAt(iIndex);

                    PetTable NewOnUsePetData = TableManager.GetInstance().GetTableItem<PetTable>((int)NewUsePetInfo.dataId);
                    if (NewOnUsePetData == null)
                    {
                        continue;
                    }

                    iChangedSlotIndex = (int)NewOnUsePetData.PetType - 1;
                    bIsWear = true;

                    break;
                }
            }

            if(NotUsePetInfo != null)
            {
                for (int i = 0; i < OnUsePetInfoList.Count; i++)
                {
                    if (NotUsePetInfo.id == OnUsePetInfoList[i].id)
                    {
                        OnUsePetInfoList.RemoveAt(i);
                        break;
                    }
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PetSlotChanged, iChangedSlotIndex, bIsWear);

            // 显示属性变化
            if (NotUsePetInfo != null || NewUsePetInfo != null)
            {
                ShowPetPropertyChange(NotUsePetInfo, NewUsePetInfo);
            }           
        }

        void _OnNetSyncFeedPetRes(MsgDATA msg)
        {
            SceneFeedPetRes msgData = new SceneFeedPetRes();
            msgData.decode(msg.bytes);

            if ((ProtoErrorCode)msgData.result != ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }

            if (msgData.value == 0)
            {
                SystemNotifyManager.SystemNotify(8507);
            }
           
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PetFeedSuccess, msgData.value, msgData.isCritical);
        }

        void _OnNetSyncDeletePetRes(MsgDATA msg)
        {
            SceneDeletePet msgData = new SceneDeletePet();
            msgData.decode(msg.bytes);

            for(int i = 0; i < PetInfoList.Count; i++)
            {
                if(PetInfoList[i].id == msgData.id)
                {
                    PetInfoList.RemoveAt(i);
                    break;
                }
            }
        }

        void _OnNetSyncSellPetRes(MsgDATA msg)
        {
            SceneSellPetRes msgData = new SceneSellPetRes();
            msgData.decode(msg.bytes);

            if ((ProtoErrorCode)msgData.result != ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }

            SystemNotifyManager.SysNotifyFloatingEffect("宠物出售成功");
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PetItemsInfoUpdate);
        }

        void _OnNetSyncChangePetSkillRes(MsgDATA msg)
        {
            SceneChangePetSkillRes msgData = new SceneChangePetSkillRes();
            msgData.decode(msg.bytes);

            if ((ProtoErrorCode)msgData.result != ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }

            SystemNotifyManager.SysNotifyFloatingEffect("修改成功");
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PetPropertyReselect, msgData.petId);       
        }

        void _OnNetSyncEatPetRes(MsgDATA msg)
        {
            SceneDevourPetRes msgData = new SceneDevourPetRes();
            msgData.decode(msg.bytes);

            if ((ProtoErrorCode)msgData.result != ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }

            byte IsCritical = 0;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EatPetSuccess, msgData.exp, IsCritical);
        }

        /// <summary>
        /// 获取未装备的宠物列表
        /// </summary>
        /// <returns></returns>
        public List<PetInfo> GetPetList()
        {
            return PetInfoList;
        }

        /// <summary>
        /// 携带的宠物列表
        /// </summary>
        /// <returns></returns>
        public List<PetInfo> GetOnUsePetList()
        {
            return OnUsePetInfoList;
        }

        private int mMaxGoldFeedNum = -1; 

        protected int maxGoldFeedNum
        {
            get
            {
                if (mMaxGoldFeedNum < 0)
                {
                    SystemValueTable goldFeedValue = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PET_GOLD_FEED_MAX);

                    if (null != goldFeedValue)
                    {
                        mMaxGoldFeedNum = goldFeedValue.Value;
                    }
                }

                return mMaxGoldFeedNum;
            }
        }

        /// <summary>
        /// 选中的宠物是否有金币喂养次数
        /// </summary>
        /// <returns></returns>
        public bool IsSelectPetsContainGoldFeedCount()
        {
            List<PetInfo> onUse = GetOnUsePetList();
            
            if (null == onUse)
            {
                return false;
            }
            for (int i = 0; i < onUse.Count; ++i)
            {
                int maxLevel = int.MaxValue;

                PetTable petTable = TableManager.instance.GetTableItem<PetTable>((int)onUse[i].dataId);

                if (null != petTable)
                {
                    maxLevel = petTable.MaxLv;
                }

                if (SelectPetId == onUse[i].id && onUse[i].goldFeedCount < maxGoldFeedNum && onUse[i].level < maxLevel)
                {
                    return true;
                }
            }

            return false;
        }

        public int GetPetsContainGoldFeedCountTypeIndex()
        {
            List<PetInfo> onUse = GetOnUsePetList();
            
            if (null == onUse)
            {
                return -1;
            }

            for (int i = 0; i < onUse.Count; ++i)
            {
                int maxLevel = int.MaxValue;

                PetTable petTable = TableManager.instance.GetTableItem<PetTable>((int)onUse[i].dataId);

                if (null != petTable)
                {
                    maxLevel = petTable.MaxLv;
                }

                if (onUse[i].goldFeedCount < maxGoldFeedNum && onUse[i].level < maxLevel)
                {
                    return (int)petTable.PetType - 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// 所有携带宠物只要有一个可以升级有就返回true
        /// </summary>
        /// <returns></returns>
        public bool IsOnUsePetsContainGoldFeedCount()
        {
            List<PetInfo> onUse = GetOnUsePetList();

            if (null == onUse)
            {
                return false;
            }

            for (int i = 0; i < onUse.Count; ++i)
            {
                int maxLevel = int.MaxValue;

                PetTable petTable = TableManager.instance.GetTableItem<PetTable>((int)onUse[i].dataId);

                if (null != petTable)
                {
                    maxLevel = petTable.MaxLv;
                }

                if (onUse[i].goldFeedCount < maxGoldFeedNum && onUse[i].level < maxLevel)
                {
                    return true;
                }
            }

            return false;
        }

        private bool mIsUserClickFeedCount = false;
        /// <summary>
        /// 是否已经点击了喂养
        /// </summary>
        public bool IsUserClickFeedCount
        {
            get { return mIsUserClickFeedCount;  }
            set { mIsUserClickFeedCount = value; }
        }

        /// <summary>
        /// 是否需要显示喂养红点,面向一次面向所有宠物
        /// </summary>
        /// <returns></returns>
        public bool IsNeedShowOnUsePetsRedPoint()
        {
            return !IsUserClickFeedCount && IsOnUsePetsContainGoldFeedCount();
        }

        /// <summary>
        /// 是否需要显示喂养红点,面向现在选中的宠物
        /// </summary>
        /// <returns></returns>
        public bool SelectPetsNeedShowRedPoint()
        {
            return !IsUserClickFeedCount && IsSelectPetsContainGoldFeedCount();
        }

        /// <summary>
        /// 是否含有新的宠物蛋
        /// </summary>
        /// <returns></returns>
        public bool IsContainNewPetEgg()
        {
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);

            List<ulong> temp = new List<ulong>();

            for (int i = 0; i < itemGuids.Count; ++i)
            {
                ulong cur = itemGuids[i];
                ItemData curitem = ItemDataManager.GetInstance().GetItem(cur);
                if (curitem == null)
                {
                    continue;
                }

                if (IsPetEggItem(curitem.TableID) && curitem.IsNew)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否装备满了宠物
        /// </summary>
        /// <returns></returns>
        public bool IsContainEquipPetPosition()
        {
            List<PetInfo> pets = GetOnUsePetList();

            if (null == pets)
            {
                return false;
            }

            return pets.Count >= 3;
        }

        public bool IsContainFitPet2Equip()
        {
            List<PetInfo> onUse = GetOnUsePetList();

            if (null == onUse)
            {
                return false;
            }

            List<PetTable.ePetType> fitType = new List<PetTable.ePetType>();

            fitType.Add(PetTable.ePetType.PT_ATTACK);
            fitType.Add(PetTable.ePetType.PT_PROPERTY);
            fitType.Add(PetTable.ePetType.PT_SUPPORT);

            for (int i = 0; i < onUse.Count; i++)
            {
                PetTable petTb = TableManager.instance.GetTableItem<PetTable>((int)onUse[i].dataId);
                if (null != petTb)
                {
                    fitType.Remove(petTb.PetType);
                }
            }

            if (fitType.Count <= 0)
            {
                return false;
            }

            List<PetInfo> petInfos = GetPetList();

            for (int i = 0; i < petInfos.Count; ++i)
            {
                PetTable petTb = TableManager.instance.GetTableItem<PetTable>((int)petInfos[i].dataId);

                if (null != petTb)
                {
                    for (int j = 0; j < fitType.Count; j++)
                    {
                        if (fitType[j] == petTb.PetType)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #region IsPetEggInfo
        private class PetQueryInfo
        {
            public PetQueryInfo(int id, PetTable.ePetType type, int petEggId)
            {
                this.id = id;
                this.type = type;
                this.petEggId = petEggId;
            }

            public int id { get; private set; }
            public PetTable.ePetType type { get; private set; }
            public int petEggId { get; private set; }
        }

        private List<PetQueryInfo> mPetQueryInfo = null;//  new List<PetQueryInfo>();

        private void _initPetEGGIDMinMax()
        {
            if (null != mPetQueryInfo)
            {
                return;
            }

            mPetQueryInfo = new List<PetQueryInfo>();

            var petTb = TableManager.GetInstance().GetTable<PetTable>();
            var enumer = petTb.GetEnumerator();

            while (enumer.MoveNext())
            {
                var data = enumer.Current.Value as PetTable;

                if (data == null)
                {
                    continue;
                }

                mPetQueryInfo.Add(new PetQueryInfo(data.ID, data.PetType, data.PetEggID));
            }
        }

        /// <summary>
        /// 判断宠物蛋类型
        /// </summary>
        /// <param name="petItemId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsPetEggItemType(int petItemId, PetTable.ePetType type)
        {
            _initPetEGGIDMinMax();

            for (int i = 0; i < mPetQueryInfo.Count; ++i)
            {
                if (mPetQueryInfo[i].petEggId == petItemId && mPetQueryInfo[i].type == type )
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsPetEggItem(int itemid)
        {
            _initPetEGGIDMinMax();

            for (int i = 0; i < mPetQueryInfo.Count; ++i)
            {
                if (mPetQueryInfo[i].petEggId == itemid)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        /// <summary>
        /// 是否需要显示宠物蛋页签红点
        /// </summary>
        /// <returns></returns>
        public bool IsNeedShowPetEggRedPoint()
        {
            return IsContainNewPetEgg();
        }

        public bool IsNeedShowPetRedPoint()
        {
            if (IsUseClickPetTab)
            {
                return false;
            }

            return IsContainFitPet2Equip();
        }

        private bool mIsUserClickPetTab = false; 

        /// <summary>
        /// 用户是否点击宠物页签
        /// </summary>
        public bool IsUseClickPetTab
        {
            get { return mIsUserClickPetTab; }
            set { mIsUserClickPetTab = value; }
        }

        public bool IsSelfPet(PetItemTipsData petItem)
        {
            if(null != petItem && null != petItem.petinfo)
            {
                for (int i = 0; i < OnUsePetInfoList.Count; i++)
                {
                    if (OnUsePetInfoList[i].id == petItem.petinfo.id)
                    {
                        return true;
                    }
                }

                for (int i = 0; i < PetInfoList.Count; i++)
                {
                    if (PetInfoList[i].id == petItem.petinfo.id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public PetItemTipsData GetSelfOnUsePetByType(PetItemTipsData petItem)
        {
            if(petItem == null)
            {
                return null;
            }

            PetTable PetItemTableData = TableManager.GetInstance().GetTableItem<PetTable>((int)petItem.petinfo.dataId);
            if (PetItemTableData == null)
            {
                return null;
            }

            // 传进来的宠物信息是否是身上的宠物
            for (int i = 0; i < OnUsePetInfoList.Count; i++)
            {
                if(OnUsePetInfoList[i].id == petItem.petinfo.id)
                {
                    return null;
                }
            }

            // 身上的槽位是否有与传进来的宠物类型相同的宠物
            for (int i = 0; i < OnUsePetInfoList.Count; i++)
            {
                PetTable PetTableData = TableManager.GetInstance().GetTableItem<PetTable>((int)OnUsePetInfoList[i].dataId);
                if(PetTableData == null)
                {
                    continue;
                }

                if (PetTableData.PetType == PetItemTableData.PetType)
                {
                    PetItemTipsData NeedPetData = new PetItemTipsData();
                    NeedPetData.petinfo = new PetInfo();

                    SetPetData(NeedPetData.petinfo, OnUsePetInfoList[i]);
                    NeedPetData.petTipsType = PetTipsType.OnUsePetTip;
                    NeedPetData.PlayerJobID = PlayerBaseData.GetInstance().JobTableID;

                    return NeedPetData;
                }
            }

            return null;
        }

        public PetInfo GetFollowPet()
        {
            return FollowPet;
        }

        public bool IsFollowPetHungry()
        {
            if(FollowPet == null)
            {
                return false;
            }

            if(FollowPet.id <= 0 || FollowPet.dataId <= 0)
            {
                return false;
            }

            return FollowPet.hunger <= PetHungryLimit;
        }

        public PetInfo GetNewOpenPet()
        {
            return NewOpenPet;
        }

        public void SetActiveFeed(bool bActive)
        {
            bHasActivieFeedPet = bActive;
        }
        
        public bool GetIsActiveFeed()
        {
            return bHasActivieFeedPet;
        }

        public string GetColorName(string name, PetTable.eQuality PetQuality)
        {
            ItemData.QualityInfo qualityInfo = ItemData.GetQualityInfo((ItemTable.eColor)PetQuality);

            if(qualityInfo != null)
            {
                return TR.Value("common_color_text", qualityInfo.ColStr, name);
            }

            return "";
        }

        public string GetQualityDesc(PetTable.eQuality PetQuality)
        {
            ItemData.QualityInfo qualityInfo = ItemData.GetQualityInfo((ItemTable.eColor)PetQuality);

            if (qualityInfo != null)
            {
                return string.Format("<color={0}>{1}</color>", qualityInfo.ColStr, qualityInfo.Desc);
            }

            return "";
        }

        public string GetQualityTipTitleBackGround(PetTable.eQuality PetQuality)
        {
            ItemData.QualityInfo qf = ItemData.GetQualityInfo((ItemTable.eColor)PetQuality);

            if (null == qf)
            {
                return string.Empty;
            }

            return qf.TipTitleBackGround;
        }

        public Color GetQualityTipTitleBackGroundByColor(PetTable.eQuality PetQuality)
        {
            ItemData.QualityInfo qf = ItemData.GetQualityInfo((ItemTable.eColor)PetQuality);

            if (null == qf)
            {
                return new Color();
            }

            return qf.Col;
        }

        public string GetQualityIconBack(PetTable.eQuality PetQuality)
        {
            ItemData.QualityInfo qualityInfo = ItemData.GetQualityInfo((ItemTable.eColor)PetQuality);

            if(qualityInfo != null)
            {
                return qualityInfo.Background;
            }

            return "";
        }

        public string GetQualityTitleBack(PetTable.eQuality PetQuality)
        {
            ItemData.QualityInfo qualityInfo = ItemData.GetQualityInfo((ItemTable.eColor)PetQuality);

            if (qualityInfo != null)
            {
                return qualityInfo.TitleBG;
            }

            return "";
        }

        public string GetPetTypeDesc(PetTable.ePetType petType)
        {
            if(petType == PetTable.ePetType.PT_ATTACK)
            {
                return "主动型";
            }
            else if(petType == PetTable.ePetType.PT_PROPERTY)
            {
                return "技能型";
            }
            else if(petType == PetTable.ePetType.PT_SUPPORT)
            {
                return "辅助型";
            }
            else
            {
                return "";
            }
        }

        public string GetPetTypeIconPath(PetTable.ePetType petType)
        {
            if (petType == PetTable.ePetType.PT_ATTACK)
            {
                return "UI/Image/NewPacked/Beibao.png:Beibao_Pet_Zhiye_01";
            }
            else if (petType == PetTable.ePetType.PT_PROPERTY)
            {
                return "UI/Image/NewPacked/Beibao.png:Beibao_Pet_Zhiye_02";
            }
            else if (petType == PetTable.ePetType.PT_SUPPORT)
            {
                return "UI/Image/NewPacked/Beibao.png:Beibao_Pet_Zhiye_03";
            }
            else
            {
                return "";
            }
        }

        public int GetMaxShowStarNums(int iMaxLevel)
        {
            return iMaxLevel / 10;
        }

        public int ShowPetHalfStarNum(int iPetLevel)
        {
            return (iPetLevel / 5);
        }

        public void UpdateStarsShow(PetTable petData, PetInfo CurSelPetInfo,int addLevel,ref UIGray[] starsGray, ref Image[] HalfStars, ref Image[] HalfShadowStars, bool bWillPlayStarEffect = false, int iStarGrayIndex = -1)
        {
            int iMaxShowStarNum = GetMaxShowStarNums(petData.MaxLv);
            int iShowStarNum = ShowPetHalfStarNum(CurSelPetInfo.level);
            int iPreShowLevel = CurSelPetInfo.level + addLevel;
            iPreShowLevel = IntMath.Min(iPreShowLevel,petData.MaxLv);
            int iMaxStartHalfNum = ShowPetHalfStarNum(petData.MaxLv);
            int iPreShowStartNum = ShowPetHalfStarNum(iPreShowLevel);
            iPreShowStartNum = IntMath.Min(iMaxStartHalfNum,iPreShowStartNum);

            for (int i = 0; i < starsGray.Length; i++)
            {
                if (i < iMaxShowStarNum)
                {
                    if (i * 2 < iShowStarNum)
                    {
                        HalfStars[i * 2].gameObject.CustomActive(true);
                    }
                    else
                    {
                        HalfStars[i * 2].gameObject.CustomActive(false);
                    }

                    if(i * 2 < HalfShadowStars.Length)
                    {
                        if (i * 2 < iPreShowStartNum)
                        {
                            HalfShadowStars[i * 2].gameObject.CustomActive(true);
                        }
                        else
                        {
                            HalfShadowStars[i * 2].gameObject.CustomActive(false);
                        }
                    }

                    if (bWillPlayStarEffect && iStarGrayIndex == i)
                    {
                        //HalfStars[i * 2].gameObject.CustomActive(false);
                        HalfStars[i * 2 + 1].gameObject.CustomActive(false);
                        //HalfShadowStars[i * 2].gameObject.CustomActive(true);
                        if ((i * 2 + 1) < HalfShadowStars.Length)
                        {
                            HalfShadowStars[i * 2 + 1].gameObject.CustomActive(true);
                        }
                    }
                    else
                    {
                        if ((i * 2 + 1) < iShowStarNum)
                        {
                            HalfStars[i * 2 + 1].gameObject.CustomActive(true);
                        }
                        else
                        {
                            HalfStars[i * 2 + 1].gameObject.CustomActive(false);
                        }

                        if ((i * 2 + 1) < HalfShadowStars.Length)
                        {
                            if ((i * 2 + 1) < iPreShowStartNum)
                            {
                                HalfShadowStars[i * 2 + 1].gameObject.CustomActive(true);
                            }
                            else
                            {
                                HalfShadowStars[i * 2 + 1].gameObject.CustomActive(false);
                            }
                        }
                    }

                    if (starsGray[i] != null && starsGray[i].gameObject != null)
                        starsGray[i].gameObject.CustomActive(true);
                }
                else
                {
                    HalfStars[i * 2].gameObject.CustomActive(false);
                    HalfStars[i * 2 + 1].gameObject.CustomActive(false);
                    if(i * 2 < HalfShadowStars.Length)
                    {
                        HalfShadowStars[i * 2].gameObject.CustomActive(false);
                    }
                    if (i * 2 + 1< HalfShadowStars.Length)
                    {
                        HalfShadowStars[i * 2 + 1].gameObject.CustomActive(false);
                    }

                    if (starsGray[i] != null && starsGray[i].gameObject != null)
                        starsGray[i].gameObject.CustomActive(false);
                }
            }
        }

        public void UpdatePetStarImage(int iGrayStarIndex, ref Image[] HalfStars)
        {
            for(int i = 0; i < HalfStars.Length; i++)
            {
                if(i == iGrayStarIndex * 2 + 1)
                {
                    HalfStars[i].gameObject.CustomActive(true);
                    break;
                }
            }
        } 

        public int GetFeedNeedMoney(PetInfo SelPetInfo, PetFeedTable.eType Feedtype, ref int Exp)
        {
            PetFeedTable FeedData = TableManager.GetInstance().GetTableItem<PetFeedTable>((int)Feedtype);
            if (FeedData == null)
            {
                return -1;
            }

            int iCount = 0;

            if (Feedtype == PetFeedTable.eType.PET_FEED_GOLD)
            {
                if (SelPetInfo.goldFeedCount >= FeedData.ConsumeItem.Count)
                {
                    iCount = FeedData.ConsumeItem.Count - 1;
                }
            }
            else if (Feedtype == PetFeedTable.eType.PET_FEED_POINT)
            {
                if (SelPetInfo.pointFeedCount >= FeedData.ConsumeItem.Count)
                {
                    iCount = FeedData.ConsumeItem.Count - 1;
                }
            }
            else
            {

            }

            string data = "";

            if (Feedtype == PetFeedTable.eType.PET_FEED_GOLD)
            {
                data = FeedData.ConsumeItem[iCount];
            }
            else if (Feedtype == PetFeedTable.eType.PET_FEED_POINT)
            {
                data = FeedData.ConsumeItem[iCount];
            }

            string[] DataList = data.Split('_');

            if (DataList.Length < 3)
            {
                return -1;
            }

            int iNeedMoney = int.Parse(DataList[1]);
            Exp = int.Parse(DataList[2]);

            return iNeedMoney;
        }

        public int GetPetFoodNum(ref int FoodItemTableID)
        {
            List<ulong> PetFoodItemIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Pet);
            if (PetFoodItemIDs == null)
            {
                return 0;
            }

            int iCount = 0;
            for (int i = 0; i < PetFoodItemIDs.Count; i++)
            {
                ItemData data = ItemDataManager.GetInstance().GetItem(PetFoodItemIDs[i]);
                if (data == null)
                {
                    continue;
                }

                if (i == 0)
                {
                    FoodItemTableID = data.TableID;
                }

                iCount += data.Count;
            }

            return iCount;
        }

        public string GetPetPropertyTips(PetTable Petdata, int CurPetLevel = 1)
        {
            string content = "";

            //content = "<color=#50ef44ff>[属性加成]</color>\n";
            content += SkillDataManager.GetInstance().UpdatePetSkillDescription(Petdata.InnateSkill, (byte)CurPetLevel, (byte)CurPetLevel);

            return content;
        }

        public string GetPetCurSkillTips(PetTable Petdata, int JobTableID, int CurSkillIndex = 0, int CurPetLevel = 1,bool bNeedHead = true)
        {
            string content = "";

            int iCurSkillID = GetPetSkillIDByIndex(Petdata, CurSkillIndex, JobTableID);
            if(iCurSkillID < 0)
            {
                return content;
            }

            SkillTable skillData = TableManager.GetInstance().GetTableItem<SkillTable>(iCurSkillID);
            if (skillData == null)
            {
                return content;
            }

            List<int> SkillIds = GetPetSkillIDsByJob(Petdata, JobTableID);
            if (SkillIds.Count <= 0)
            {
                return content;
            }

            if (bNeedHead)
            {
                if (Petdata.PetType == PetTable.ePetType.PT_ATTACK)
                {
                    content = string.Format("<color=#c37631ff>[已选技能]</color><color=#5fa523ff>\n[{0}]</color>\n", skillData.Name);
                }
                else
                {
                    content = string.Format("<color=#c37631ff>[已选择属性项]</color><color=#5fa523ff>\n[{0}]</color>\n", skillData.Name);
                }
            }
            else
            {
                content = string.Format("<color=#CFCFCFFF>{0}</color>\n", skillData.Name);
            }
            int tempSkillId = 0;
            if(CurSkillIndex >= 0 && CurSkillIndex < SkillIds.Count)
            {
                tempSkillId = SkillIds[CurSkillIndex];
            }
            else
            {
                tempSkillId = SkillIds[0];
            }
            content += SkillDataManager.GetInstance().UpdatePetSkillDescription(tempSkillId, (byte)CurPetLevel, (byte)CurPetLevel);

            return content;
        }

        public string GetCanSelectSkillTips(PetTable Petdata, int PlayerJobID, int CurSkillIndex = 0, int CurPetLevel = 1, bool bNeedHead = true)
        {
            string content = "";

            if (bNeedHead)
            {
                if (CurSkillIndex != 0)
                {
                    if (Petdata.PetType == PetTable.ePetType.PT_ATTACK)
                    {
                        content = "<color=#686868FF>[可选技能]</color>\n";
                    }
                    else
                    {
                        content = "<color=#686868FF>[可重选属性项]</color>\n";
                    }
                }
                else
                {
                    if (Petdata.PetType == PetTable.ePetType.PT_ATTACK)
                    {
                        content = "<color=#5fa523ff>[可选技能]</color>\n";
                    }
                    else
                    {
                        content = "<color=#5fa523ff>[可重选属性项]</color>\n";
                    }
                }
            }
            

            List<int> SkillIds = GetPetSkillIDsByJob(Petdata, PlayerJobID);
            if (SkillIds.Count <= 0)
            {
                return content;
            }

            for (int i = 0; i < SkillIds.Count; i++)
            {
                if (i == CurSkillIndex)
                {
                    continue;
                }
                
                SkillDescriptionTable desData = TableManager.GetInstance().GetTableItem<SkillDescriptionTable>(SkillIds[i]);
                if (desData == null)
                {
                    continue;
                }

                content += string.Format("<color=#CFCFCFFF>{0}</color>\n", desData.Name);

                List<string> deslist = SkillDataManager.GetInstance().GetPetSkillDesList(desData.ID, (byte)CurPetLevel);

                for(int j = 0; j < deslist.Count; j++)
                {
                    content += string.Format("{0}\n", deslist[j]);
                }
            }

            //if (CurSkillIndex != 0)
            //{
            //    content += "</color>";
            //}

            //这里处理技能配置一个的时候 “可选技能：” 或者 “可选属性：”描述不显示
            if (content.Length <= 31 || content.Length <= 23)
            {
                content = "";
            }

            return content;
        }

        public int GetPetSkillIDByIndex(PetTable Petdata, int iIndex, int JobTableID)
        {
            List<int> skillIDs = new List<int>();

            for (int i = 0; i < Petdata.Skills.Count; i++)
            {
                SkillTable skilldata = TableManager.GetInstance().GetTableItem<SkillTable>(Petdata.Skills[i]);

                if (skilldata == null)
                {
                    continue;
                }

                bool bContain = false;
                for (int j = 0; j < skilldata.JobID.Count; j++)
                {
                    if(skilldata.JobID[0] == 0)
                    {
                        bContain = true;
                        break;
                    }

                    if (skilldata.JobID[j] == JobTableID)
                    {
                        bContain = true;
                        break;
                    }
                }

                if (bContain)
                {
                    skillIDs.Add(Petdata.Skills[i]);
                }
            }

            if(skillIDs.Count <= 0 || iIndex >= skillIDs.Count)
            {
                return skillIDs[0];
                //return -1;
            }

            return skillIDs[iIndex];
        }

        public List<int> GetPetSkillIDsByJob(PetTable Petdata, int PlayerJobID)
        {
            List<int> skillIDs = new List<int>();

            for (int i = 0; i < Petdata.Skills.Count; i++)
            {
                SkillTable skilldata = TableManager.GetInstance().GetTableItem<SkillTable>(Petdata.Skills[i]);

                if (skilldata == null)
                {
                    continue;
                }

                bool bContain = false;
                for (int j = 0; j < skilldata.JobID.Count; j++)
                {
                    if(skilldata.JobID[0] == 0)
                    {
                        bContain = true;
                        break;
                    }

                    if (skilldata.JobID[j] == PlayerJobID)
                    {
                        bContain = true;
                        break;
                    }
                }

                if (bContain)
                {
                    skillIDs.Add(Petdata.Skills[i]);
                }
            }

            return skillIDs;
        }

        public static int GetPetSkillIDByJob(int iPetTableID, int iJobID, int iIndex)
        {
            PetTable Petdata = TableManager.GetInstance().GetTableItem<PetTable>(iPetTableID);
            if (Petdata == null)
            {
                return -1;
            }

            List<int> skillIDs = new List<int>();

            for (int i = 0; i < Petdata.Skills.Count; i++)
            {
                SkillTable skilldata = TableManager.GetInstance().GetTableItem<SkillTable>(Petdata.Skills[i]);

                if (skilldata == null)
                {
                    continue;
                }

                bool bContain = false;
                for (int j = 0; j < skilldata.JobID.Count; j++)
                {
                    if (skilldata.JobID[0] == 0)
                    {
                        bContain = true;
                        break;
                    }

                    if (skilldata.JobID[j] == iJobID)
                    {
                        bContain = true;
                        break;
                    }
                }

                if (bContain)
                {
                    skillIDs.Add(Petdata.Skills[i]);
                }
            }

            if (skillIDs.Count <= 0 || iIndex >= skillIDs.Count)
            {
                return -1;
            }

            return skillIDs[iIndex];
        }

        public List<PetInfo> GetPetSortListBySortType(List<PetInfo> UnSortPetInfoList, ref int iCount, PetItemSortType SortType = PetItemSortType.QualitySort, int MaxNum = 0)
        {
            List<PetInfo> SortPetList = new List<PetInfo>();

            int iMaxNum = UnSortPetInfoList.Count;
            if(MaxNum != 0)
            {
                iMaxNum = MaxNum;
            }

            for (int i = 0; i < UnSortPetInfoList.Count && i < iMaxNum; i++)
            {
                PetTable UnSortPetTableData = TableManager.GetInstance().GetTableItem<PetTable>((int)UnSortPetInfoList[i].dataId);
                if (UnSortPetTableData == null)
                {
                    continue;
                }

                PetInfo petinfo = new PetInfo();
                SetPetData(petinfo, UnSortPetInfoList[i]);

                if (SortPetList.Count <= 0)
                {
                    SortPetList.Add(petinfo);
                }
                else
                {
                    for (int j = 0; j < SortPetList.Count; j++)
                    {
                        PetTable SortPetTableData = TableManager.GetInstance().GetTableItem<PetTable>((int)SortPetList[j].dataId);
                        if (SortPetTableData == null)
                        {
                            continue;
                        }

                        if (SortType == PetItemSortType.QualitySort)
                        {
                            if (UnSortPetTableData.Quality > SortPetTableData.Quality)
                            {
                                SortPetList.Insert(j, petinfo);
                                break;
                            }
                        }
                        else if (SortType == PetItemSortType.PetTypeSort)
                        {
                            if (UnSortPetTableData.PetType < SortPetTableData.PetType)
                            {
                                SortPetList.Insert(j, petinfo);
                                break;
                            }
                        }

                        if (j == SortPetList.Count - 1)
                        {
                            SortPetList.Add(petinfo);
                            break;
                        }
                    }
                }

                iCount++;
            }

            for (int i = 0; i < SortPetList.Count; i++)
            {
                PetTable PreData = TableManager.GetInstance().GetTableItem<PetTable>((int)SortPetList[i].dataId);
                if (PreData == null)
                {
                    continue;
                }

                for (int j = i + 1; j < SortPetList.Count; j++)
                {
                    PetTable NextData = TableManager.GetInstance().GetTableItem<PetTable>((int)SortPetList[j].dataId);
                    if (NextData == null)
                    {
                        continue;
                    }

                    bool bCanExchange = false;

                    if (SortType == PetItemSortType.QualitySort)
                    {
                        if (NextData.Quality == PreData.Quality && SortPetList[j].level > SortPetList[i].level)
                        {
                            bCanExchange = true;
                        }
                    }
                    else if (SortType == PetItemSortType.PetTypeSort)
                    {
                        if (NextData.PetType == PreData.PetType && SortPetList[j].level > SortPetList[i].level)
                        {
                            bCanExchange = true;
                        }
                    }

                    if (bCanExchange)
                    {
                        PetInfo Preinfo = new PetInfo();
                        SetPetData(Preinfo, SortPetList[i]);

                        PetInfo Nextinfo = new PetInfo();
                        SetPetData(Nextinfo, SortPetList[j]);

                        SortPetList[i] = Nextinfo;
                        SortPetList[j] = Preinfo;
                    }
                }
            }

            return SortPetList;
        }

		public void SetPetItemData(GameObject root, PetInfo petinfo, int PlayerJobID, PetTipsType petTipsType = PetTipsType.None, bool bIsCoverState = false)
        {
			if (root == null)
            {
                return;
            }

            var BindData = root.GetComponent<ComCommonBind>();
            if (BindData == null)
            {
                return;
            }

            var petData = TableManager.GetInstance().GetTableItem<PetTable>((int)petinfo.dataId);
			if (petData == null)
            {
                return;
            }
				
            Image IconRoot = BindData.GetCom<Image>("IconRoot");
            Image ItemIcon = BindData.GetCom<Image>("Icon");
            Button btPetItem = BindData.GetCom<Button>("btPetItem");
            Text Level = BindData.GetCom<Text>("Level");
            Image TypeIcon = BindData.GetCom<Image>("TypeIcon");
            // GameObject StarsRoot = BindData.GetGameObject("StarsRoot");
            Image Cover = BindData.GetCom<Image>("Cover");

            if (!string.IsNullOrEmpty(petData.IconPath) && petData.IconPath != "-")
            {
                // ItemIcon.sprite = AssetLoader.instance.LoadRes(petData.IconPath, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref ItemIcon, petData.IconPath);
            }

            string sIconBack = GetQualityIconBack(petData.Quality);
            if (!string.IsNullOrEmpty(sIconBack) && sIconBack != "-")
            {
                // IconRoot.sprite = AssetLoader.instance.LoadRes(sIconBack, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref IconRoot, sIconBack);
            }

            string sTypeIcon = GetPetTypeIconPath(petData.PetType);
            if (!string.IsNullOrEmpty(sTypeIcon) && sTypeIcon != "-")
            {
                // TypeIcon.sprite = AssetLoader.instance.LoadRes(sTypeIcon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref TypeIcon, sTypeIcon);
            }

            Level.text = string.Format("Lv.{0}", petinfo.level);

            IconRoot.gameObject.CustomActive(true);

            if (petTipsType != PetTipsType.OnUsePetTip)
            {
                btPetItem.onClick.AddListener(() => { OnShowPetTips(petinfo, PlayerJobID, petTipsType); });
            }
           
            //Image[] stars = StarsRoot.GetComponentsInChildren<Image>(true);
            //int iShowStarNum = ShowPetHalfStarNum(petinfo.level);
            //for (int i = 0; i < stars.Length; i++)
            //{
            //    if (i < iShowStarNum)
            //    {
            //        stars[i].gameObject.CustomActive(true);
            //    }
            //    else
            //    {
            //        stars[i].gameObject.CustomActive(false);
            //    }
            //}

            if(Cover != null)
            {
                if (bIsCoverState)
                {
                    Cover.gameObject.CustomActive(true);
                }
                else
                {
                    Cover.gameObject.CustomActive(false);
                }
            }         
        }

		public void OnShowPetTips(PetInfo petinfo, int PlayerJobID, PetTipsType petTipsType = PetTipsType.None)
		{
            ItemTipManager.GetInstance().CloseAll();

			PetItemTipsData TipsData = new PetItemTipsData();
            		
			TipsData.petinfo = new PetInfo();
			SetPetData(TipsData.petinfo, petinfo);

            TipsData.petTipsType = petTipsType;
            TipsData.PlayerJobID = PlayerJobID;
            TipsData.bFunc = IsSelfPet(TipsData);

            var compareData = GetSelfOnUsePetByType(TipsData);
            if(null != compareData)
            {
                compareData.bFunc = false;
            }

            ItemTipManager.GetInstance().ShowPetTips(TipsData, compareData);
		}

        public void ShowPetPropertyChange(PetInfo bef, PetInfo aft)
        {
            var data = GetPetPropChanges(bef, aft);

            if (data != null)
            {
                PopUpChangedAttrbutes(data);
            }
        }

        List<BetterEquipmentData> GetPetPropChanges(PetInfo bef, PetInfo aft)
        {
            List<BetterEquipmentData> data = new List<BetterEquipmentData>();

            if(bef == null && aft == null)
            {
                return data;
            }

            PetTable BefData = null;
            PetTable AftData = null;

            List<string> BefDesList = null;
            List<string> AftDesList = null;

            List<float> BefDesDataList = null;
            List<float> AftDesDataList = null;

            if (bef == null)
            {
                AftData = TableManager.GetInstance().GetTableItem<PetTable>((int)aft.dataId);

                if(AftData == null)
                {
                    return data;
                }

                AftDesList = SkillDataManager.GetInstance().GetSkillDesList(AftData.InnateSkill, (byte)aft.level);

                for(int i = 0; i < AftDesList.Count; i++)
                {
                    string[] splitstr = AftDesList[i].Split(':');

                    if(splitstr.Length >= 2)
                    {
                        BetterEquipmentData newData = new BetterEquipmentData();

                        newData.PreData = "";
                        newData.CurData = splitstr[0] + ":+" + splitstr[1];

                        data.Add(newData);
                    }
                }
            }
            else if(aft == null)
            {
                BefData = TableManager.GetInstance().GetTableItem<PetTable>((int)bef.dataId);

                if(BefData == null)
                {
                    return data;
                }

                BefDesList = SkillDataManager.GetInstance().GetSkillDesList(BefData.InnateSkill, (byte)bef.level);

                for (int i = 0; i < BefDesList.Count; i++)
                {
                    string[] splitstr = BefDesList[i].Split(':');

                    if (splitstr.Length >= 2)
                    {
                        BetterEquipmentData newData = new BetterEquipmentData();

                        newData.PreData = splitstr[0] + ":-" + splitstr[1];
                        newData.CurData = "";

                        data.Add(newData);
                    }
                }
            }
            else
            {
                BefData = TableManager.GetInstance().GetTableItem<PetTable>((int)bef.dataId);
                AftData = TableManager.GetInstance().GetTableItem<PetTable>((int)aft.dataId);
                
                if(BefData == null || AftData == null)
                {
                    return data;
                }

                BefDesList = SkillDataManager.GetInstance().GetSkillDesList(BefData.InnateSkill, (byte)bef.level);
                AftDesList = SkillDataManager.GetInstance().GetSkillDesList(AftData.InnateSkill, (byte)aft.level);

                BefDesDataList = SkillDataManager.GetInstance().GetSkillDataList(BefData.InnateSkill, (byte)bef.level);
                AftDesDataList = SkillDataManager.GetInstance().GetSkillDataList(AftData.InnateSkill, (byte)aft.level);

                List<int> CommonPropertyIdxList = new List<int>();
                for(int i = 0; i < BefDesList.Count; i++)
                {
                    string[] Befsplitstr = BefDesList[i].Split(':');

                    if (Befsplitstr.Length < 2)
                    {
                        continue;
                    }

                    bool bFind = false;
                    for (int j = 0; j < AftDesList.Count; j++)
                    {
                        string[] Aftsplitstr = AftDesList[j].Split(':');

                        if(Aftsplitstr.Length < 2)
                        {
                            CommonPropertyIdxList.Add(j);
                            continue;
                        }

                        if (string.Equals(Befsplitstr[0], Aftsplitstr[0]))
                        {
                            CommonPropertyIdxList.Add(j);

                            if (BefDesDataList[i] == AftDesDataList[j])
                            {
                                bFind = true;
                                break;
                            }

                            BetterEquipmentData newData = new BetterEquipmentData();

                            newData.PreData = Befsplitstr[0] + ":";

                            if(BefDesDataList[i] > AftDesDataList[j])
                            {
                                newData.CurData = "-" + Utility.GetStringByFloat(BefDesDataList[i] - AftDesDataList[j]);
                            }
                            else
                            {
                                newData.CurData = "+" + Utility.GetStringByFloat(AftDesDataList[j] - BefDesDataList[i]);
                            }

                            data.Add(newData);

                            bFind = true;
                            break;
                        }
                    }

                    if(!bFind)
                    {
                        BetterEquipmentData newData = new BetterEquipmentData();

                        newData.PreData = Befsplitstr[0] + ":-" + Befsplitstr[1];
                        newData.CurData = "";

                        data.Add(newData);
                    }
                }

                for(int i = 0; i < AftDesList.Count; i++)
                {
                    bool bFind = false;

                    for(int j = 0; j < CommonPropertyIdxList.Count; j++)
                    {
                        if(i == CommonPropertyIdxList[j])
                        {
                            bFind = true;
                            break;
                        }
                    }

                    if(!bFind)
                    {
                        string[] Aftsplitstr = AftDesList[i].Split(':');

                        if (Aftsplitstr.Length < 2)
                        {
                            continue;
                        }

                        BetterEquipmentData newData = new BetterEquipmentData();

                        newData.PreData = "";
                        newData.CurData = Aftsplitstr[0] + ":+" + Aftsplitstr[1];

                        data.Add(newData);
                    }
                }
            }

            return data;
        }

        void PopUpChangedAttrbutes(List<BetterEquipmentData> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                string formatString = "";

                if (data[i].PreData == "")
                {
                    formatString = data[i].CurData;
                }
                else if(data[i].CurData == "")
                {
                    formatString = data[i].PreData;
                }
                else
                {
                    formatString = data[i].PreData + data[i].CurData;
                }

                SystemNotifyManager.SysNotifyFloatingEffect(formatString, CommonTipsDesc.eShowMode.SI_QUEUE);
            }
        }

        public void SetPetData(PetInfo ReceivePetData, PetInfo GivePetData)
        {
            ReceivePetData.id = GivePetData.id;
            ReceivePetData.dataId = GivePetData.dataId;
            ReceivePetData.level = GivePetData.level;
            ReceivePetData.exp = GivePetData.exp;
            ReceivePetData.skillIndex = GivePetData.skillIndex;
            ReceivePetData.hunger = GivePetData.hunger;
            ReceivePetData.goldFeedCount = GivePetData.goldFeedCount;
            ReceivePetData.pointFeedCount = GivePetData.pointFeedCount;
            ReceivePetData.selectSkillCount = GivePetData.selectSkillCount;
            ReceivePetData.petScore = GivePetData.petScore;
            //Logger.LogErrorFormat("{0}", ReceivePetData.id, );
        }

		public void UpdateSyncPetData(PetInfo ReceivePetData, Pet GivePetData)
		{
			for (int i = 0; i < GivePetData.dirtyFields.Count; ++i)
			{
				if (GivePetData.dirtyFields[i] == (int)PetObjectAttr.POA_EXP)
				{
					ReceivePetData.exp = GivePetData.exp;
                }
				else if (GivePetData.dirtyFields[i] == (int)PetObjectAttr.POA_LEVEL)
				{
					ReceivePetData.level = GivePetData.level;
				}
				else if (GivePetData.dirtyFields[i] == (int)PetObjectAttr.POA_HUNGER)
				{
					ReceivePetData.hunger = GivePetData.hunger;
				}
				else if (GivePetData.dirtyFields[i] == (int)PetObjectAttr.POA_SKILL_INDEX)
				{
					ReceivePetData.skillIndex = GivePetData.skillIndex;
				}
				else if (GivePetData.dirtyFields[i] == (int)PetObjectAttr.POA_GOLD_FEED_COUNT)
				{
					ReceivePetData.goldFeedCount = GivePetData.goldFeedCount;
				}
				else if (GivePetData.dirtyFields[i] == (int)PetObjectAttr.POA_POINT_FEED_COUNT)
				{
					ReceivePetData.pointFeedCount = GivePetData.pointFeedCount;
                }
                else if (GivePetData.dirtyFields[i] == (int)PetObjectAttr.POA_SELECT_SKILL_COUNT)
                {
                    ReceivePetData.selectSkillCount = GivePetData.selectSkillCount;
                }
                else if (GivePetData.dirtyFields[i] == (int)PetObjectAttr.POA_PET_SCORE)
                {
                    ReceivePetData.petScore = GivePetData.petScore;
                }
            }
		}

        public void SendFeedPetReq(PetFeedTable.eType type, UInt64 PetGUID, bool isAutoSend = false)
        {
            SceneFeedPetReq req = new SceneFeedPetReq();

            req.id = PetGUID;
            req.feedType = (byte)type;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            if (type == PetFeedTable.eType.PET_FEED_ITEM)
            {
                PetInfo FollowPet = GetFollowPet();

                if (PetGUID == FollowPet.id)
                {
                    if(isAutoSend)
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayActiveFeedPetAction);
                    }
                    else
                    {
                        SetActiveFeed(true);
                    }      
                }
            }
        }

        public void OnUpdate(float timeElapsed)
        {
            fTimeIntrval += timeElapsed;

            if(fTimeIntrval > CheckHungerTime)
            {
                fTimeIntrval = 0.0f;

                ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                if(systemTown == null)
                {
                    return;
                }

                if(OnUsePetInfoList == null || OnUsePetInfoList.Count <= 0)
                {
                    return;
                }

                int iFoodID = 0;
                int iFoodCount = GetPetFoodNum(ref iFoodID);

                for (int i = 0; i < OnUsePetInfoList.Count; i++)
                {
                    if (OnUsePetInfoList[i].hunger < PetHungryLimit && iFoodCount > 0)
                    {
                        SendFeedPetReq(PetFeedTable.eType.PET_FEED_ITEM, OnUsePetInfoList[i].id, true);
                        iFoodCount--;
                    }
                }
            }
        }
    }
}
