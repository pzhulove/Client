using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using Network;
using System;
using System.Reflection;

namespace GameClient
{
    public class PayReturnSpeacialItem
    {
        public class ResPath
        {
            public string iconPath;
            public string modelPath;
            public int awardItemId;

            public void Clear()
            {
                iconPath = "";
                modelPath = "";
                awardItemId = -1;
            }
        }

        public int payReturnActivityId;
        public List<ResPath> resPaths = new List<ResPath>();
        public bool bSplit = false;

        public void Clear()
        {
            payReturnActivityId = -1;
            if (resPaths != null)
            {
                for (int i = 0; i < resPaths.Count; i++)
                {
                    resPaths[i].Clear();
                }
                resPaths.Clear();
            }
            bSplit = false;
        }
    }

	public class PayManager : Singleton<PayManager> {

		public const int FIRST_PAY_ID = 8300;
		public const int CONSUME_PAY_ID = 8400;

		public const int FIRSY_PAY_SUB_ID = 8301;

        public const int MONTH_CARD_ACTIVITY_ID = 2500; //活动表ID
        public const int MONTH_CARD_TEMPLATE_ID = 6000; //活动模板表ID
        public const int MONTH_CARD_TYPE_CONFIG_ID = 9380; //活动模板表 ActivityTypeID

		public const byte STATUS_TASK_INIT = 0;
		public const byte STATUS_TASK_UNFINISH = 1;
		public const byte STATUS_TASK_CANGET = 2;	//可领取
		public const byte STATUS_TASK_OVER = 4;		//已完成

        string PAY_APP_NAME = "商城礼包";
        string PAY_APP_DESC = "购买可获得商城大礼包";

        public const int FIRST_PAT_RMB_NUM = 6;     //首充默认为6块


		Dictionary<int, Dictionary<uint,int>> exclusiveItems = new Dictionary<int, Dictionary<uint,int>>();
		int [] ids ={FIRST_PAY_ID, CONSUME_PAY_ID};
		int canGetActivityCount = 0;

		int savedOccu = -1;

		public string weaponPath = "";
		public int weaponStrengthLevel = 0;
		public uint weaponItemID = 0;

		public bool lastPayIsMonthCard = false;
		public bool lastMontchCardNeedOpenWindow = false;

		public override void Init ()
		{
            InitPayReturnDisplayTable();
		}

		public override void UnInit ()
		{
            ClearPayReturnDisplayTable();
            ClearCurrVipPrivilegeDescList();
		}

		public string GetFirstWeaponPath()
		{
			return weaponPath;
		}

		public int GetFirstWeaponStrengthLevel()
		{
			return weaponStrengthLevel;
		}

        /// <summary>
        /// 获取当前角色充值金额
        /// </summary>
        /// <returns></returns>
        public int GetCurrentRolePayMoney()
        {
            int payMoney = 0;
            var payConsumeItemList = GetConsumeItems(true);
            if (payConsumeItemList == null)
            {
                return payMoney;
            }
            if(payConsumeItemList.Count <= 0)
            {
                return payMoney;
            }
            var payActivityData = payConsumeItemList[payConsumeItemList.Count - 1];
            if (payActivityData == null)
            {
                return payMoney;
            }
            var payActivityTaskDatas = payActivityData.akActivityValues;
            if (payActivityTaskDatas != null && payActivityTaskDatas.Count > 0)
            {
                if(int.TryParse(payActivityTaskDatas[0].value, out payMoney))
                {
                    //return => out payMoney
                }
            }
            return payMoney;
        }

        public bool HasFirstPayFinish()
        {
            var var1 = ActiveManager.GetInstance().GetActiveData(FIRST_PAY_ID);
            if (var1 != null)
            {
                return var1.akChildItems[0].status > STATUS_TASK_CANGET;
            }

            return false;
        }

		public bool HasFirstPay()
		{
			var var1 = ActiveManager.GetInstance().GetActiveData(FIRST_PAY_ID);
			if (var1 != null)
			{
				return var1.akChildItems[0].status < STATUS_TASK_OVER;
			}
				
			return true;
		}

        public bool HasSecondPay()
        {
            //var var1 = ActiveManager.GetInstance().GetActiveData(FIRST_PAY_ID);
            //if (var1 != null)
            //{
            //    return var1.akChildItems[5].status < STATUS_TASK_OVER;
            //}
            //return true;

            List<ActiveManager.ActivityData> items = PayManager.GetInstance().GetConsumeItems(); //不包括firstPayConsume !!!
            if (items == null)
            {
                Logger.LogErrorFormat("Item is null");
                return false;
            }
            if (items.Count <= 0)
            {
                Logger.LogError("items count is zero");
                return false;
            }
            return items[4].status < STATUS_TASK_OVER;
        }

        public bool HasConsumePay()
		{
			var var1 = ActiveManager.GetInstance().GetActiveData(CONSUME_PAY_ID);
			if (var1 != null)
			{
				for(int i=0; i<var1.akChildItems.Count; ++i)
					if (var1.akChildItems[i].status < STATUS_TASK_OVER)
						return true;
			}
			return false;
		}

        /// <summary>
        /// 是否购买过月卡
        /// </summary>
        /// <returns></returns>
        public bool HasBoughtMonthCard()
        {
            bool bBought = false;
            var activityData = ActiveManager.GetInstance().GetChildActiveData(MONTH_CARD_ACTIVITY_ID);
            if (activityData != null && activityData.status != (int)Protocol.TaskStatus.TASK_UNFINISH &&
                activityData.status != (int)Protocol.TaskStatus.TASK_INIT)
            {
                bBought = true;                
            }
            return bBought;
        }


        /// <summary>
        /// 月卡是否可用 已购买 并且 不过期
        /// </summary>
        /// <returns></returns>
        public bool HasMonthCardEnabled()
        {
            bool bEnabled = false;
            var activityData = ActiveManager.GetInstance().GetChildActiveData(MONTH_CARD_ACTIVITY_ID);
            if (activityData != null && activityData.status != (int)Protocol.TaskStatus.TASK_UNFINISH &&
                activityData.status != (int)Protocol.TaskStatus.TASK_INIT)
            {
                int iRdValue = ActiveManager.GetInstance().GetActiveItemValue(MONTH_CARD_ACTIVITY_ID, "rd");
                if (iRdValue > 0 || PlayerBaseData.GetInstance().MonthCardLv > TimeManager.GetInstance().GetServerTime())
                {
                    bEnabled = true;
                }
            }
            return bEnabled;
        }

        /// <summary>
        /// 获取补充的充值回馈道具 - 职业表
        /// 
        /// 时装和首充武器
        /// </summary>
	    void GetExclusiveItems()
		{
			exclusiveItems.Clear();

			var data = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID);
			if (data != null)
			{
				if (data.PayItems.Count >= 1 && data.PayItems[0].Length > 1)
				{
					for(int i=0; i<data.PayItems.Count; ++i)
					{
						var tokens = data.PayItems[i].Split(new char[] { '-' });
						if (tokens.Length == 2)
						{
							//task id
							int taskID = 0;
							try {
								taskID = Convert.ToInt32(tokens[0], 10);
							}
							catch (Exception err)
							{
								Logger.LogErrorFormat("err:{0}", err);
							}

							Dictionary<uint, int> items = new Dictionary<uint, int>();

							//items
							var tokens2 = tokens[1].Split(new char[] {','});
							for(int j=0; j<tokens2.Length; ++j)
							{
								var tokens3 = tokens2[j].Split(new char[] {'_'});
								uint itemID = Convert.ToUInt32(tokens3[0], 10);
								int itemNum = Convert.ToInt32(tokens3[1], 10);

								items.Add(itemID, itemNum);
							}

							exclusiveItems.Add(taskID, items);
						}
					}
				}

				if (data.FirstPayWeapon.Length > 1)
				{
					var tokens = data.FirstPayWeapon.Split(new char[] { '_' });
					uint itemID = Convert.ToUInt32(tokens[0], 10);
					weaponStrengthLevel = Convert.ToInt32(tokens[1], 10);
					weaponPath = Utility.GetItemModulePath((int)itemID);
					weaponItemID = itemID;
				}
			}
		}

        /// <summary>
        /// 获取传入档位对应的 充值回馈奖励数据
        /// 包括充值活动奖励道具 - 活动表
        /// 包括充值时装 - 职业表
        /// 
        /// 不包括首充武器  - 职业表
        /// 不包括充值基本道具（如月卡或点券等）- 充值商城表
        /// </summary>
        /// <param name="activity">一个充值档位对应的活动数据</param>
        /// <returns></returns>
		public Dictionary<uint, int> GetAwardItems(ActiveManager.ActivityData activity)
		{
			if (savedOccu != PlayerBaseData.GetInstance().JobTableID)
			{
				savedOccu = PlayerBaseData.GetInstance().JobTableID;
				GetExclusiveItems();
			}

			Dictionary<uint, int> awards = null;
			int taskID = activity.ID;
			awards = activity.GetAwards();
			if (awards != null)
			{
				if (exclusiveItems.ContainsKey(taskID))
				{
					Dictionary<uint, int>.Enumerator enumerator = exclusiveItems[taskID].GetEnumerator();
					while (enumerator.MoveNext())
					{
						uint key = enumerator.Current.Key;
						int value = enumerator.Current.Value;

						if (awards.ContainsKey(key))
							awards[key] += value;
						else
							awards.Add(key, value);
					}
				}
			}

			return awards;
		}

        /// <summary>
        /// 快捷方法 - 获取首充回馈奖励 
        /// 包括充值奖励 - 活动表
        /// 包括充值时装 - 职业表
        /// 
        /// 不包括首充武器  - 职业表
        /// 不包括首充武器和基本奖励 ：点券  - 充值商城表
        /// </summary>
        /// <returns></returns>
		public Dictionary<uint, int> GetFirstPayItems()
		{
			Dictionary<uint, int> awards = null;
			int taskID = 0;

			var var1 = ActiveManager.GetInstance().GetActiveData(FIRST_PAY_ID);
			if (var1 != null && var1.akChildItems[0] != null)
			{
				awards = GetAwardItems(var1.akChildItems[0]);
			}

			return awards;
		}

        /// <summary>
        /// 获取全部充值活动奖励 的档位数据 - 活动表
        /// 
        /// 不包括充值时装  - 职业表
        /// 不包括首充武器  - 职业表
        /// 不包括充值基本道具（如月卡或点券等） - 充值商城表
        /// </summary>
        /// <param name="includeFirst">是否包含首充数据</param>
        /// <returns></returns>
		public List<ActiveManager.ActivityData> GetConsumeItems(bool includeFirst=false)
		{
			List<ActiveManager.ActivityData> ret = new List<ActiveManager.ActivityData>();

			if (includeFirst)
			{
				var var = ActiveManager.GetInstance().GetActiveData(FIRST_PAY_ID);
				if (var != null && var.akChildItems[0] != null)
					ret.Add(var.akChildItems[0]);
			}
				

			var var1 = ActiveManager.GetInstance().GetActiveData(CONSUME_PAY_ID);
			if (var1 != null)
			{
				for(int i=0; i<var1.akChildItems.Count; ++i)
					ret.Add(var1.akChildItems[i]);
			}

			return ret;
		}

		public void GetRewards(int activity)
		{
			ActiveManager.GetInstance().SendSubmitActivity(activity);

			ClientSystemManager.GetInstance().delayCaller.DelayCall(1000, ()=>{
				UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnPayResultNotify, "10");	
			});

		}

		public bool CanGetRewards(int activity = 0)
		{
			for(int i=0; i<ids.Length; ++i)
			{
				int id = ids[i];
				var activeData = ActiveManager.GetInstance().GetActiveData(id);
                if(activeData == null || activeData.akChildItems == null)
                {
                    continue;
                }
				for(int j=0; j< activeData.akChildItems.Count; ++j)
					if ((activity==0?true:activity == activeData.akChildItems[j].ID) && activeData.akChildItems[j].status == STATUS_TASK_CANGET)
						return true;
			}
			return false;
		}

		int GetFinishActivityNum()
		{
			int sum = 0;
			for(int i=0; i<ids.Length; ++i)
			{
				int id = ids[i];
				var var1 = ActiveManager.GetInstance().GetActiveData(id);
				if (var1 != null)
				{
					for(int j=0; j<var1.akChildItems.Count; ++j)
						if (var1.akChildItems[j].status >= STATUS_TASK_CANGET)
							sum++;
				}
			}

			return sum;
		}

		public void RecordActivity()
		{
			canGetActivityCount = GetFinishActivityNum();
		}

		public bool HasNewActivityFinish()
		{
			int tmp = GetFinishActivityNum();
			return tmp > canGetActivityCount;
		}

        public int GetCurrFinishActivityNum()
        {
            return GetFinishActivityNum();
        }

        #region 支付返利展示

        // key  : 充值返利活动 id  /  value : 各职业不同的展示道具id集合 (只使用其中一个)
        private Dictionary<int, PayReturnSpeacialItem> mPayReturnSpecialItemDic = new Dictionary<int, PayReturnSpeacialItem>();

        public void InitPayReturnDisplayTable()
        {
            var chargeDisplayTable = TableManager.GetInstance().GetTable<ProtoTable.ChargeDisplayTable>();
            var enumerator = chargeDisplayTable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PayReturnSpeacialItem sItem = new PayReturnSpeacialItem();

                var value = enumerator.Current.Value as ProtoTable.ChargeDisplayTable;
                if (value == null)
                {
                    continue;
                }
                int payActId = value.ActivityID;
                sItem.payReturnActivityId = payActId;

                ProtoTable.ChargeDisplayTable.eType resType = value.Type;
                string itemId = value.ItemID;
                if (string.IsNullOrEmpty(itemId))
                {
                    continue;
                }

                if (sItem.resPaths == null)
                {
                    sItem.resPaths = new List<PayReturnSpeacialItem.ResPath>();
                }

                string[] itemIds = itemId.Split('|');
                if (itemIds == null || itemIds.Length == 0)
                {
                    sItem.bSplit = false;
                    PayReturnSpeacialItem.ResPath resPath = GetPayReturnResPathItem(itemId, resType, value.IconPath,value.ModelPath);
                    sItem.resPaths = new List<PayReturnSpeacialItem.ResPath>() { resPath };
                }
                else
                {
                    sItem.bSplit = true;
                    var resPaths = new List<PayReturnSpeacialItem.ResPath>();
                    for (int i = 0; i < itemIds.Length; i++)
                    {
                        PayReturnSpeacialItem.ResPath resPath = GetPayReturnResPathItem(itemIds[i], resType, value.IconPath, value.ModelPath);
                        if (!sItem.resPaths.Contains(resPath))
                        {
                            sItem.resPaths.Add(resPath);
                        }
                    }
                }
                if (mPayReturnSpecialItemDic == null)
                {
                    return;
                }
                if (mPayReturnSpecialItemDic.ContainsKey(payActId))
                {
                    var list = mPayReturnSpecialItemDic[payActId].resPaths;
                    if (list != null && sItem.resPaths != null)
                    {
                        list.AddRange(sItem.resPaths);
                    }
                }
                else
                {
                    mPayReturnSpecialItemDic.Add(payActId, sItem);
                }
            }
        }

        PayReturnSpeacialItem.ResPath GetPayReturnResPathItem(string itemIdStr, ProtoTable.ChargeDisplayTable.eType resType, string iconPath, string modelPath)
        {
            var resPath = new PayReturnSpeacialItem.ResPath();
            int tId = -1;
            if (int.TryParse(itemIdStr, out tId))
            {
                if (tId > 0)
                {
                    resPath.awardItemId = tId;
                    if (resType == ProtoTable.ChargeDisplayTable.eType.Texture)
                    {
                        resPath.iconPath = iconPath;
                    }
                    else if (resType == ProtoTable.ChargeDisplayTable.eType.Model)
                    {
                        resPath.modelPath = modelPath;
                    }
                }
            }
            return resPath;
        }

        public void ClearPayReturnDisplayTable()
        {
            if (mPayReturnSpecialItemDic != null)
            {
                foreach (var mItem in mPayReturnSpecialItemDic)
                {
                    PayReturnSpeacialItem item = mItem.Value;
                    if (item != null)
                    {
                        item.Clear();
                    }
                }
                mPayReturnSpecialItemDic.Clear();
            }
        }

        public string GetPayReturnSpecialResPath(int payReturnActivityId, List<AwardItemData> totalAwardItemdataList)
        {
            if (mPayReturnSpecialItemDic == null)
            {
                Logger.LogError("Pay Return Special Display Item Dictionary is null");
                return "";
            }
            if (totalAwardItemdataList == null)
            {
                return "";
            }

            if (!mPayReturnSpecialItemDic.ContainsKey(payReturnActivityId))
            {
                Logger.LogErrorFormat("Pay Return Special Display Item Dictionary can not find activity id  : {0}", payReturnActivityId);
                return "";
            }
            PayReturnSpeacialItem specialItem = mPayReturnSpecialItemDic[payReturnActivityId];
            if (specialItem == null)
            {
                Logger.LogErrorFormat("Pay Return Special Display Item Dictionary activity id : {0} . value is null", payReturnActivityId);
                return "";
            }

            if (specialItem.resPaths == null)
            {
                Logger.LogErrorFormat("Pay Return Special Display Item Dictionary activity id : {0} . value awardItem is null", payReturnActivityId);
                return "";
            }

            int specialItemIdsCount = specialItem.resPaths.Count;

            for (int i = 0; i < specialItemIdsCount; i++)
            {
                var sItemResPath = specialItem.resPaths[i];
                if (sItemResPath == null)
                {
                    continue;
                }
                int insideId = sItemResPath.awardItemId;
                for (int j = 0; j < totalAwardItemdataList.Count; j++)
                {
                    int outsideId = totalAwardItemdataList[j].ID;
                    if (insideId == outsideId)
                    {
                        string modelPath = sItemResPath.modelPath;
                        string iconPath = sItemResPath.iconPath;

                        if (!string.IsNullOrEmpty(modelPath) && !string.IsNullOrEmpty(iconPath))
                        {
                            Logger.LogErrorFormat("Charge Display Table is error , iconPath is {0} , modelPath is {1}, should one is null", modelPath, iconPath);
                            return "";
                        }

                        if (!string.IsNullOrEmpty(modelPath))
                        {
                            if (specialItemIdsCount > 1 && (i + 1) <= specialItemIdsCount  && specialItem.bSplit)
                            {
                                modelPath = string.Format(modelPath, i + 1, i + 1);
                            }
                            else if (specialItem.bSplit == false)
                            {
                                return modelPath;
                            }
                            return modelPath;
                        }
                        if (!string.IsNullOrEmpty(iconPath))
                        {
                            if (specialItemIdsCount > 1 && (i + 1) <= specialItemIdsCount && specialItem.bSplit)
                            {
                                iconPath = string.Format(iconPath, i + 1, i + 1);
                            }
                            else if (specialItem.bSplit == false)
                            {
                                return iconPath;
                            }
                            return iconPath;
                        }
                    }
                }
            }
            return "";
        }

        public int GetPayReturnSpecialResID(int payReturnActivityId, List<AwardItemData> totalAwardItemdataList)
        {
            if (mPayReturnSpecialItemDic == null)
            {
                Logger.LogError("Pay Return Special Display Item Dictionary is null");
                return -1;
            }
            if (totalAwardItemdataList == null)
            {
                return -1;
            }

            if (!mPayReturnSpecialItemDic.ContainsKey(payReturnActivityId))
            {
                Logger.LogErrorFormat("Pay Return Special Display Item Dictionary can not find activity id  : {0}", payReturnActivityId);
                return -1;
            }
            PayReturnSpeacialItem specialItem = mPayReturnSpecialItemDic[payReturnActivityId];
            if (specialItem == null)
            {
                Logger.LogErrorFormat("Pay Return Special Display Item Dictionary activity id : {0} . value is null", payReturnActivityId);
                return -1;
            }

            if (specialItem.resPaths == null)
            {
                Logger.LogErrorFormat("Pay Return Special Display Item Dictionary activity id : {0} . value awardItem Ids is null", payReturnActivityId);
                return -1;
            }

            int specialItemIdsCount = specialItem.resPaths.Count;

            for (int i = 0; i < specialItemIdsCount; i++)
            {
                var sItemResPath = specialItem.resPaths;
                if (sItemResPath == null)
                {
                    continue;
                }
                int insideId = sItemResPath[i].awardItemId;
                for (int j = 0; j < totalAwardItemdataList.Count; j++)
                {
                    int outsideId = totalAwardItemdataList[j].ID;
                    if (insideId == outsideId)
                    {
                        return outsideId;
                    }
                }
            }
            return -1;
        }

        #endregion

        #region 支付特权描述

        private List<VipDescData> preVipDescDataList = new List<VipDescData>();
        private List<VipDescData> currVipDescDataList = new List<VipDescData>();
        public List<VipDescData> GetPrivilegeDescDataListByVipLevel(int vipLevel)
        {
            ClearCurrVipPrivilegeDescList();

            int preVipLevel = -1;
            if (vipLevel > 0)
            {
                preVipLevel = vipLevel - 1;
            }

            var systemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_VIPLEVEL_MAX);

            if (systemValueTableData == null)
            {
                return currVipDescDataList;
            }

            if (vipLevel > systemValueTableData.Value)
            {
                Logger.LogErrorFormat("[PayManager GetPrivilegeDescDataListByVipLevel] - vip level is more than system max vip level !!!");
                return currVipDescDataList;
            }

            var vipPrivilegeTableData = TableManager.GetInstance().GetTable<ProtoTable.VipPrivilegeTable>();

            if (vipPrivilegeTableData == null)
            {
                return currVipDescDataList;
            }

            var enumerator = vipPrivilegeTableData.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var item = enumerator.Current.Value as ProtoTable.VipPrivilegeTable;

                if (item == null)
                {
                    continue;
                }

                VipDescData pData = null;
                if (preVipLevel >= 0)
                {
                    pData = ReadVipPrivilegeDesdByVipLevel(item, preVipLevel);
                }

                var cData = ReadVipPrivilegeDesdByVipLevel(item, vipLevel);
                if (string.IsNullOrEmpty(cData.desc))
                {
                    continue;
                }
                bool bFindInPre = false;

                if (pData != null && !string.IsNullOrEmpty(pData.desc))
                {
                    //！！！ 当前特权类的bSpecialDisplay 判断优先于 是否包含了前一个特权类的特权 这一判断  ！！！
                    //如果当前特权类 是需要特殊显示的  则不会被 是否包含了前一个特权类的特权 这个判断屏蔽掉了
                    if (pData.desc.Equals(cData.desc) && cData.bSpecialDisplay == false)
                    {
                        bFindInPre = true;
                    }
                }

                if (currVipDescDataList != null && !currVipDescDataList.Contains(cData) && !bFindInPre)
                {
                    currVipDescDataList.Add(cData);
                }
            }
            return currVipDescDataList;
        }

        private VipDescData ReadVipPrivilegeDesdByVipLevel(ProtoTable.VipPrivilegeTable vipPrivilegeTableItem, int vipLevel)
        {
            VipDescData data = new VipDescData();

            PropertyInfo info = vipPrivilegeTableItem.GetType().GetProperty(string.Format("VIP{0}", vipLevel), (BindingFlags.Instance | BindingFlags.Public));
            if (info == null)
            {
                return data;
            }

            int iOriData = (int)info.GetValue(vipPrivilegeTableItem, null);
            //Logger.LogErrorFormat("!!! [GetPrivilegeDescDataListByVipLevel] vip privilege data is {0}", iOriData);

            if (iOriData <= 0)
            {
                return data;
            }
            if (vipLevel > 0)
            {
                if (vipPrivilegeTableItem.Type == ProtoTable.VipPrivilegeTable.eType.PK_MONEY_LIMIT)
                {
                    ProtoTable.SystemValueTable valuedata = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_PKCOIN_MAX);
                    if (valuedata != null)
                    {
                        iOriData += valuedata.Value;
                    }
                }
                else if (vipPrivilegeTableItem.Type == ProtoTable.VipPrivilegeTable.eType.MYSTERIOUS_SHOP_REFRESH_NUM)
                {
                    ProtoTable.SystemValueTable valuedata = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>(((int)ProtoTable.SystemValueTable.eType.SVT_SHOP_REFRESH));
                    if (valuedata != null)
                    {
                        iOriData += valuedata.Value;
                    }
                }
                else if (vipPrivilegeTableItem.Type == ProtoTable.VipPrivilegeTable.eType.None)
                {
                    iOriData = iOriData - 1;
                }
            }

            if (data == null)
            {
                return data;
            }

            if (vipPrivilegeTableItem.DataType == ProtoTable.VipPrivilegeTable.eDataType.FLOAT)
            {
                data.desc = string.Format(vipPrivilegeTableItem.Description, iOriData / 10.0f) + "%";
            }
            else
            {
                data.desc = string.Format(vipPrivilegeTableItem.Description, iOriData);
            }
            data.icon = vipPrivilegeTableItem.IconPath;
            if (CheckVipPrivilegeDescToDisplay(vipPrivilegeTableItem.VIPDisplay, vipLevel))
            {
                data.bSpecialDisplay = true;
            }
            data.index = vipPrivilegeTableItem.DisplayIndex;

            return data;
        }

        private void ClearCurrVipPrivilegeDescList()
        {
            if (currVipDescDataList != null)
            {
                currVipDescDataList.Clear();
            }
            if (preVipDescDataList != null)
            {
                preVipDescDataList.Clear();
            }
        }

        private bool CheckVipPrivilegeDescToDisplay(string displayVipLevels ,int vipLevel)
        {
            if (vipLevel < 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(displayVipLevels))
            {
                return false;
            }

            string currVipLevel = vipLevel.ToString();
            string[] levelArray = displayVipLevels.Split('|');
            if (levelArray == null || levelArray.Length == 0 )
            {
                if (displayVipLevels.Equals(currVipLevel))
                {
                    return true;
                }
            }
            for (int i = 0; i < levelArray.Length; i++)
            {
                if (levelArray[i].Equals(currVipLevel))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        public void DoPay(int payItemID, int price, Protocol.ChargeMallType mallType=Protocol.ChargeMallType.Charge)
		{
            // SystemNotifyManager.SysNotifyFloatingEffect("内部测试版本暂不提供充值功能!!!");
            GameFrameWork.instance.StartCoroutine(MyPay(payItemID, price));
            // return;
			// GameFrameWork.instance.StartCoroutine(Pay(payItemID, price, mallType));
		}

        private string GetTimeStamp()
        {
            return ((System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
        }

        public IEnumerator MyPay(int payItemID, int price)
		{
            string url = Global.PAY_ADDRESS + "query?account=" + Global.USERNAME + "&serverid=" + ClientApplication.adminServer.id + "&guid=";

            RoleInfo[] roleInfos = ClientApplication.playerinfo.roleinfo;
            RoleInfo currentRole = roleInfos[ClientApplication.playerinfo.curSelectedRoleIdx];
            url = url + currentRole.strRoleId;
            url = url + "&productId=" + payItemID;
            BaseWaitHttpRequest request = new BaseWaitHttpRequest();
            request.url = url;
            yield return request;
            if (BaseWaitHttpRequest.eState.Success == request.GetResult())
            {
                string resText = request.GetResultString();
                Hashtable ret = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode(resText);
                int retValue = Int32.Parse(ret["ret"].ToString());
                if(retValue != 0)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(ret["msg"].ToString());
                    yield break;
                }
                else
                {
                    string timestamp = GetTimeStamp();   
                    string payUrl = Global.PAY_ADDRESS + "doPay?account=" + Global.USERNAME + "&serverid=" + ClientApplication.adminServer.id + "&guid=";
                    payUrl = payUrl + currentRole.strRoleId;
                    payUrl = payUrl + "&productId=" + payItemID;
                    payUrl = payUrl + "&timestamp=" + timestamp;
                    string signKey = "guituald-sign";
                    string signStr = Global.USERNAME + "|" + ClientApplication.adminServer.id + "|" + currentRole.strRoleId + "|" + timestamp + signKey;
                    string sign = DungeonUtility.GetMD5Str(System.Text.Encoding.UTF8.GetBytes(signStr)).ToLower();
                    payUrl = payUrl + "&sign=" + sign;
                    Logger.LogError("=============>payUrl=" + payUrl);
                    Application.OpenURL(payUrl);
                }
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect("请求服务器失败!");
                yield break;
            }
        }

		public IEnumerator Pay(int payItemID, int price, Protocol.ChargeMallType mallType=Protocol.ChargeMallType.Charge)
		{
			var msgEvent = new MessageEvents();
			var req = new WorldBillingChargeReq();
			req.mallType = (byte)mallType;
			req.goodsId = (uint)payItemID;

			var res = new WorldBillingChargeRes();

			yield return MessageUtility.Wait<WorldBillingChargeReq, WorldBillingChargeRes>(ServerType.GATE_SERVER, msgEvent, req, res, true);

			if (msgEvent.IsAllMessageReceived())
			{
				if (res.result == 0)
					_DoPay(payItemID, price, mallType);
				else
				{
					if ((uint)Protocol.ProtoErrorCode.BILLING_GOODS_NUM_LIMIT == res.result)
					{
						if (mallType == ChargeMallType.Charge)
							SystemNotifyManager.SystemNotify(1121);//该额度今日充值次数已用尽！
						else if (mallType == ChargeMallType.Packet)
							SystemNotifyManager.SystemNotify(1204);//今日购买次数已用尽！
					}
					else {
						if (mallType == ChargeMallType.Charge)
							SystemNotifyManager.SystemNotify(2600002);//充值失败
						else if (mallType == ChargeMallType.Packet)
							SystemNotifyManager.SystemNotify(1039);//购买失败
					}
				}
			}
		}

		private void _DoPay(int payItemID, int price, Protocol.ChargeMallType mallType=Protocol.ChargeMallType.Charge)
		{
			RecordActivity();

			string role = ClientApplication.playerinfo.roleinfo[ClientApplication.playerinfo.curSelectedRoleIdx].strRoleId;

            string cookie = string.Format("{0},{1},{2}", (int)mallType, payItemID, role);
            var realPrice = price.ToString();
            if (Global.Settings.isUsingSDK/* && Global.Settings.isPaySDKDebug*/)
            {
#if PAY_TEST
                realPrice = "1";
#endif
            }
                


            if (SDKInterface.IsNewSDKChannelPay() == false)
            {
#if APPLE_STORE
                SetPayItemNameAndDesc(payItemID, mallType);
				SDKInterface.instance.Pay(realPrice, cookie, (int)ClientApplication.playerinfo.serverID, ClientApplication.playerinfo.openuid, PAY_APP_NAME, PAY_APP_DESC);
#else
                SDKInterface.Instance.Pay(realPrice, cookie, (int)ClientApplication.playerinfo.serverID, ClientApplication.playerinfo.openuid);
#endif
            }
            else
            {
                cookie = string.Format("{0},{1},{2},{3},{4}", (int)mallType, payItemID, role, ClientApplication.playerinfo.openuid, (int)ClientApplication.playerinfo.serverID);
                //SDK 友盟 多渠道 扩展支付接口调用
                SetPayItemNameAndDesc(payItemID, mallType);
                string requestId = SDKInterface.Instance.GenerateRequestPayID(role);
                SDKInterface.Instance.Pay(requestId, realPrice,
                    (int)ClientApplication.playerinfo.serverID, ClientApplication.playerinfo.openuid,
                    role, (int)mallType, payItemID, PAY_APP_NAME, PAY_APP_DESC, cookie);
            }

			//不开启SDK接入，直接发消息给服务器充值
			if (!Global.Settings.isUsingSDK || Application.platform == RuntimePlatform.WindowsEditor)
			{
				var req = new SceneChat();
				req.channel = 1;
				req.targetId = 0;
                req.voiceKey = "";
                req.word = string.Format("!!charge order={0} id={1} money={2} mallType={3}", UnityEngine.Random.Range(1, 9999999999), payItemID, price, (int)mallType);

				NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);

                ClientSystemManager.GetInstance().delayCaller.DelayCall(500, () =>
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnPayResultNotify, "0");
                });
            }
        }

        private void SetPayItemNameAndDesc(int payItemId, Protocol.ChargeMallType mallType)
        {
            var payTypeTable = TableManager.GetInstance().GetTableItem<ProtoTable.ChargeMallTable>(payItemId);
            if (payTypeTable != null)
            {
                PAY_APP_NAME = payTypeTable.Name;
                PAY_APP_DESC = payTypeTable.Desc;
            }
            else
            {
                PAY_APP_NAME = "商城礼包";
                PAY_APP_DESC = "购买可获得商城大礼包";
            }
        }
    }
}


