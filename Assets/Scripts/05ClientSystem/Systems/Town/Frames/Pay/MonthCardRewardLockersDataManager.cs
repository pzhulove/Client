using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using Protocol;
using ProtoTable;
using System;

namespace GameClient
{
    public class MonthCardRewardLockersItem : IComparable<MonthCardRewardLockersItem>
    {
        public ItemData itemdata;
        public uint createTime;
        public bool isHignestGradeItem = false;

        public MonthCardRewardLockersItem(int itemTableId, int subQualityLv, int strengthenLv)
        {
            itemdata = ItemDataManager.CreateItemDataFromTable(itemTableId, subQualityLv, strengthenLv);
        }

        public MonthCardRewardLockersItem(Protocol.depositItem ritem)
        {
            if (ritem == null)
            {
                return;
            }
            this.createTime = ritem.createTime;
            Protocol.ItemReward itemReward = ritem.itemReward;
            if (itemReward != null)
            {
                itemdata = ItemDataManager.CreateItemDataFromTable((int)itemReward.id, (int)itemReward.qualityLv, (int)itemReward.strength);
                if (itemdata != null)
                {
                    itemdata.GUID = ritem.guid;
                    itemdata.Count = (int)itemReward.num;
                    itemdata.EquipType = (EEquipType)itemReward.equipType;
                    this.isHignestGradeItem = MonthCardRewardLockersDataManager.IsHighestGradeItem(itemdata);
                }
            }
        }

        public void Clear()
        {
            itemdata = null;
            createTime = 0;
            isHignestGradeItem = false;
        }

        public ulong GetItemGUID()
        {
            if (itemdata == null)
            {
                return 0;
            }
            return itemdata.GUID;
        }

        public int GetItemTableId()
        {
            if (itemdata == null)
            {
                return 0;
            }
            return itemdata.TableID;
        }

        public void UpdateItem(Protocol.depositItem ritem)
        {
            Protocol.ItemReward itemReward = ritem.itemReward;
            if (itemdata == null || ritem == null || itemReward == null)
            {
                return;
            }            
            if (ritem.guid != itemdata.GUID)
            {
                Logger.LogErrorFormat("[MonthCardRewardLockersItem] - UpdateItem guid diff !");
                return;
            }
            if (itemReward.id != itemdata.TableID)
            {
                Logger.LogErrorFormat("[MonthCardRewardLockersItem] - UpdateItem tableid diff !");
                return;
            }
            this.createTime = ritem.createTime;
            itemdata.SubQuality = (int)itemReward.qualityLv;
            itemdata.StrengthenLevel = (int)itemReward.strength;
            itemdata.Count = (int)itemReward.num;
            itemdata.EquipType = (EEquipType)itemReward.equipType;
            this.isHignestGradeItem = MonthCardRewardLockersDataManager.IsHighestGradeItem(itemdata);
        }

        public int CompareTo(MonthCardRewardLockersItem other)
        {
            if (null == other || null == other.itemdata)
            {
                return 1;
            }
            if (null == this.itemdata)
            {
                return 1;
            }
            return other.itemdata.Quality.CompareTo(this.itemdata.Quality) * 2 +        //1 降序  品质好的在前
                other.createTime.CompareTo(this.createTime);                            //2 降序，时间晚的在前


            //return (int)this.itemdata.Quality > (int)other.itemdata.Quality;      //升序
        }
    }

    public class MonthCardRewardLockersDataManager : DataManager<MonthCardRewardLockersDataManager>
    {
        public const int FULI_ACTIVITY_TYPE_ID = 9380;                   //活动模版表 ActivityTypeID                      
        public const int FULI_ACTIVITY_TEMPLATE_TABLE_ID = 6000;         //活动模板表 ID

        private bool m_IsInited = false;

        private List<MonthCardRewardLockersItem> m_RewardLocalItemList = null;
        private Protocol.depositItem[] m_ServerItemArray = null;

        private uint m_UniformExpiredLastTime;     //统一的过期剩余时间
        public uint UniformExpriedLastTime
        {
            get { return m_UniformExpiredLastTime; }
        }

        //受开关界面控制的红点 刷新标记
        //不同于一般满足条件就显示的红点
        private bool m_RedPointFlag = true;
        public bool RedPointFlag
        {
            get { return m_RedPointFlag; }
            set {
                if (m_RedPointFlag != value)
                {
                    m_RedPointFlag = value;
                    RefreshRedPoint();
                }
            }
        }

        private bool isRedPointShow = false;

        private bool isLastExpiredTimeOut = false;     //上一个刷新时间已经结束

        private bool isLastExpiredTimeLessThan24H = false;  //上一个刷新时间 不足24h到期
        private bool hasActivityNotifiedLogin = false;       //本次登录 主城提示
        private int m_ExpiredTimeMinute = 1440;               //上一个刷新时间 不足24h * 60分钟 到期时间

        private bool isLastLockersEmpty = false; // 上一个刷新状态时的箱子是否是空的

        private int m_RedPointUpdateHour = 6;  //红点刷新时刻

        private int m_MonthCardRewardLockersGridCount = 30; //月卡翻牌奖励暂存箱格子数
        public int MonthCardRewardLockersGridCount
        {
            get { return m_MonthCardRewardLockersGridCount; }
        }

        public sealed override void Initialize()
        {
            if (m_IsInited)
            {
                return;
            }

            //TODO
            _BindNetEvent();

            _InitLocalData();

            m_IsInited = true;
        }

        public sealed override void Clear()
        {
            //TODO
            _UnBindNetEvent();

            _ClearLocalData();

            m_IsInited = false;
        }

        private void _ReleaseRewardItems()
        {
            if (m_RewardLocalItemList != null)
            {
                for (int i = 0; i < m_RewardLocalItemList.Count; i++)
                {
                    var item = m_RewardLocalItemList[i];
                    if (item == null) continue;
                    item.Clear();
                }
                m_RewardLocalItemList.Clear();
            }
        }

        #region PRIVATE METHODS

        private void _InitLocalData()
        {
            m_RewardLocalItemList = new List<MonthCardRewardLockersItem>();

            var monthCardGridCount = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_MONTH_CARD_DEPOSIT_GIRD);
            if (monthCardGridCount != null)
            {
                m_MonthCardRewardLockersGridCount = monthCardGridCount.Value;
            }
            var redPointUpdateTime = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_MONTH_CARD_RED_POINT_DAY_UPDATETIME);
            if (redPointUpdateTime != null)
            {
                m_RedPointUpdateHour = redPointUpdateTime.Value;
            }

            var expiredTime = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_MONTH_CARD_DEPOSIT_TIPS_TIME);
            if (expiredTime != null)
            {
                m_ExpiredTimeMinute = expiredTime.Value;
            }

            isLastLockersEmpty = IsMonthCardRewardLockersEmpty();
        }

        private void _ClearLocalData()
        {
            _ReleaseRewardItems();

            m_ServerItemArray = null;

            m_UniformExpiredLastTime = 0;

            isRedPointShow = false;

            isLastExpiredTimeOut = false;

            isLastExpiredTimeLessThan24H = false;
            hasActivityNotifiedLogin = false;

            isLastLockersEmpty = false;

            m_RedPointUpdateHour = 6;

            //默认准备刷新小红点
            m_RedPointFlag = true;
        }

        private void _BindNetEvent()
        {
            NetProcess.AddMsgHandler(SceneItemDepositRes.MsgID, _OnSceneItemDepositRes);
            NetProcess.AddMsgHandler(SceneItemDepositGetRes.MsgID, _OnSceneItemDepositGetRes);
            NetProcess.AddMsgHandler(SceneItemDepositExpire.MsgID, _OnSceneItemDepositExpire);
        }

        private void _UnBindNetEvent()
        {
            NetProcess.RemoveMsgHandler(SceneItemDepositRes.MsgID, _OnSceneItemDepositRes);
            NetProcess.RemoveMsgHandler(SceneItemDepositGetRes.MsgID, _OnSceneItemDepositGetRes);
            NetProcess.RemoveMsgHandler(SceneItemDepositExpire.MsgID, _OnSceneItemDepositExpire);
        }

        private void _OnSceneItemDepositRes(MsgDATA msg)
        {
            if (null == msg)
            {
                return;
            }
            SceneItemDepositRes ret = new SceneItemDepositRes();
            ret.decode(msg.bytes);
            depositItem[] retItems = ret.depositItemList;

            uint currServerTime = TimeManager.GetInstance().GetServerTime();

            //Logger.LogError("### currServerTime : " + currServerTime);
            //Logger.LogError("### currServerTime : " + Function.ConvertIntDateTime(currServerTime).ToString("yyyy_MM_dd_HH:mm:ss"));

            //设置过期剩余时间
            if (ret.expireTime > 0)
            {
                m_UniformExpiredLastTime = ret.expireTime - currServerTime;
                //Logger.LogError("### expireTime : " + ret.expireTime);
                //Logger.LogError("### expireTime : " + Function.ConvertIntDateTime(ret.expireTime).ToString("yyyy_MM_dd_HH:mm:ss"));

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMonthCardRewardCDUpdate);
            }
            else
            {
                //注意清0
                m_UniformExpiredLastTime = 0;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMonthCardRewardCDUpdate);
            }

            if (m_RewardLocalItemList == null)
            {
                m_RewardLocalItemList = new List<MonthCardRewardLockersItem>();
            }
            //else
            //{
            //    _ReleaseRewardItems();
            //}

            if (retItems != null && retItems.Length > 0)
            {
                m_ServerItemArray = retItems;

                if (m_RewardLocalItemList != null)
                {
                    m_RewardLocalItemList.RemoveAll(_FindItemGuidInServerItem);
                }

                MonthCardRewardLockersItem localItem = null;
                for (int i = 0; i < retItems.Length; i++)
                {
                    depositItem ritem = retItems[i];
                    if (ritem == null)
                        continue;
                    localItem = _GetMonthCardRewardLockersItemByGUID(ritem.guid);
                    if (localItem == null)
                    {
                        localItem = new MonthCardRewardLockersItem(ritem);
                        if (m_RewardLocalItemList != null)
                        {
                            m_RewardLocalItemList.Add(localItem);
                        }
                    }
                    else
                    {
                        localItem.UpdateItem(ritem);
                    }
                }

                if (m_RewardLocalItemList != null)
                {
                    //按创建时间 降序排 最近的放在最前
                    m_RewardLocalItemList.Sort((x, y) => x.CompareTo(y));
                }
            }
            else
            {
                _ReleaseRewardItems();
            }

            //箱子状态变化时 尝试提示红点
            if (IsMonthCardRewardLockersEmpty() != isLastLockersEmpty)
            {
                RefreshRedPoint();
                isLastLockersEmpty = IsMonthCardRewardLockersEmpty();
            }

            //尝试刷新主城通知
            _UpdateActivityNotice();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMonthCardRewardUpdate);
        }

        private MonthCardRewardLockersItem _GetMonthCardRewardLockersItemByGUID(ulong itemGUID)
        {
            if (m_RewardLocalItemList == null || m_RewardLocalItemList.Count <= 0)
            {
                return null;
            }
            for (int i = 0; i < m_RewardLocalItemList.Count; i++)
            {
                var item = m_RewardLocalItemList[i];
                if (item == null || item.itemdata == null)
                    continue;
                if (item.itemdata.GUID == itemGUID)
                {
                    return item;
                }
            }
            return null;
        }

        private bool _FindItemGuidInServerItem(MonthCardRewardLockersItem localItem)
        {
            bool bFind = false;
            if (localItem == null)
            {
                return bFind;
            }
            if (m_ServerItemArray == null || m_ServerItemArray.Length <= 0)
            {
                return bFind;
            }
            for (int j = 0; j < m_ServerItemArray.Length; j++)
            {
                var rItem = m_ServerItemArray[j];
                if (rItem == null)
                    continue;
                if (localItem.GetItemGUID() == rItem.guid)
                {
                    bFind = true;
                    break;
                }
            }
            return !bFind;
        }

        private void _OnSceneItemDepositGetRes(MsgDATA msg)
        {
            if (null == msg)
            {
                return;
            }
            SceneItemDepositGetRes ret = new SceneItemDepositGetRes();
            ret.decode(msg.bytes);
            if (ret.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)ret.retCode);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMonthCardRewardAccquired);
            }
        }

        private void _OnSceneItemDepositExpire(MsgDATA msg)
        {
            if(null == msg)
            {
                return;
            }
            SceneItemDepositExpire ret = new SceneItemDepositExpire();
            ret.decode(msg.bytes);
            if (ret.oddExpireTime == 0)
            {
                //有物品过期
                isLastExpiredTimeOut = true;
                //尝试刷红点标记
                m_RedPointFlag = true;

                //物品已过期 重置状态
                isLastExpiredTimeLessThan24H = false;
            }
            else if(ret.oddExpireTime == m_ExpiredTimeMinute * 60)
            {
                isLastExpiredTimeLessThan24H = true;                
            }

            _OnUpdateActivityNotice();
        }

        private void _UpdateActivityNotice()
        {
            if (m_UniformExpiredLastTime > 0 && m_UniformExpiredLastTime <= m_ExpiredTimeMinute * 60)
            {
                isLastExpiredTimeLessThan24H = true;
            }
            else
            {
                isLastExpiredTimeLessThan24H = false;
            }
            _OnUpdateActivityNotice();
        }

        /// <summary>
        /// 主城 活动通知列表 刷新
        /// </summary>
        private void _OnUpdateActivityNotice()
        {
            bool hasHighestGradeItem = _CheckLocalItemHasHighestGrade();
            if (!hasHighestGradeItem)
            {
                return;
            }
            NotifyInfo nInfo = new NotifyInfo
            {
                type = (uint)NotifyType.NT_MONTH_CARD_REWARD_EXPIRE_24H
            };
            if (isLastExpiredTimeLessThan24H)
            {
                if (!hasActivityNotifiedLogin)
                {
                    ActivityNoticeDataManager.GetInstance().AddActivityNotice(nInfo);
                    DeadLineReminderDataManager.GetInstance().AddActivityNotice(nInfo);
                    hasActivityNotifiedLogin = true; //本次登录已提示
                }
            }
            else
            {
                ActivityNoticeDataManager.GetInstance().DeleteActivityNotice(nInfo);
                DeadLineReminderDataManager.GetInstance().DeleteActivityNotice(nInfo);
            }
        }

        private bool _CheckLocalItemHasHighestGrade()
        {
            if (m_RewardLocalItemList == null || m_RewardLocalItemList.Count <= 0)
            {
                return false;
            }
            for (int i = 0; i < m_RewardLocalItemList.Count; i++)
            {
                var localItem = m_RewardLocalItemList[i];
                if (localItem != null && localItem.isHignestGradeItem)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion


        #region PUBLIC METHOEDs

        public List<MonthCardRewardLockersItem> GetMonthCardRewardLockersItems()
        {
            return m_RewardLocalItemList;
        }

        public bool IsMonthCardRewardLockersEmpty()
        {
            if (m_RewardLocalItemList == null || m_RewardLocalItemList.Count <= 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断新道具是否能够进入暂存箱
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool IsNewItemQualityAbleToEnterLockers(ProtoTable.ItemTable.eColor color)
        {
            if (m_RewardLocalItemList == null || m_RewardLocalItemList.Count <= 0)
            {
                return true;
            }
           
            int lowestQualityLevel = (int)GetRewardLockersItemLowestQuality();
            if ((int)color > lowestQualityLevel && m_RewardLocalItemList.Count >= m_MonthCardRewardLockersGridCount)
            {
                //当箱子满时 并且 当待进入的道具品质大于箱子内道具的最低品质时  可以进入
                return true;
            }
            else if (m_RewardLocalItemList.Count < m_MonthCardRewardLockersGridCount)
            {
                //箱子为满时 ！！！
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// 根据现有排序规则 获取最低品质
        /// 
        /// </summary>
        /// <returns></returns>
        public ProtoTable.ItemTable.eColor GetRewardLockersItemLowestQuality()
        {
            if (m_RewardLocalItemList == null || m_RewardLocalItemList.Count <= 0)
            {
                return ProtoTable.ItemTable.eColor.CL_NONE;
            }

            int count = m_RewardLocalItemList.Count;
            ItemData lastItemData =  m_RewardLocalItemList[count - 1].itemdata;
            if (lastItemData == null)
            {
                return ProtoTable.ItemTable.eColor.CL_NONE;
            }
            return lastItemData.Quality;
        }

        public void ReqMonthCardRewardLockersItems()
        {
            SceneItemDepositReq req = new SceneItemDepositReq();
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void ReqGetMonthCardRewardLockersItems()
        {
            if (m_RewardLocalItemList == null || m_RewardLocalItemList.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < m_RewardLocalItemList.Count; i++)
            {
                var tempItem = m_RewardLocalItemList[i];
                if (tempItem == null || tempItem.itemdata == null)
                    continue;
                ReqGetMonthCardRewardLockertItem(tempItem.itemdata.GUID);
            }
        }

        public void ReqGetMonthCardRewardLockertItem(ulong itemGUID)
        {
            SceneItemDepositGetReq req = new SceneItemDepositGetReq();
            req.guid = itemGUID;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 今天（依据可配置的时间点） 
        /// 是否显示过红点
        /// </summary>
        /// <returns></returns>
        public bool HasRedPointFirstShowedToday()
        {
            int tempTime = GetTempTimeStamp();
            int refreshTime = GetRefreshTimeStamp();
            if (tempTime >= refreshTime)
            {
                return true;
            }
            return false;
        }

        public bool IsRedPointShow()
        {
            isRedPointShow = false;
            //当奖励暂存箱中存有物品，主界面福利按钮、月卡返利按钮、奖励暂存箱按钮上会出现小红点
            //红点一天只显示一次
            bool hasRedPointShowedToday = HasRedPointFirstShowedToday();
            if (m_RedPointFlag &&
                !hasRedPointShowedToday &&
                m_RewardLocalItemList != null && m_RewardLocalItemList.Count > 0)
            {
                isRedPointShow = true;
            }
            //特殊情况：当天之前储存的到期，又重新获得物品存入，激活新的过期倒计时，会再次显示小红点
            else if (m_RedPointFlag &&
				HasNewRewardAndLastExpriedTimeOut())
            {
                isRedPointShow = true;             
            }
            else if (HasRewardToAccquire())
            {
 				isRedPointShow = true;  
            }

            return isRedPointShow;
        }

        /// <summary>
        ///  当天之前储存的到期，又重新获得物品存入，激活新的过期倒计时，会再次显示红点
        /// </summary>
        /// <returns></returns>
        public bool HasNewRewardAndLastExpriedTimeOut()
        {
            if (isLastExpiredTimeOut &&
                m_RewardLocalItemList != null && m_RewardLocalItemList.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否有奖励可领
        /// </summary>
        /// <returns></returns>
        public bool HasRewardToAccquire()
        {
            if (PayManager.GetInstance().HasMonthCardEnabled() &&
                m_RewardLocalItemList != null && m_RewardLocalItemList.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 本日查看奖励暂存箱界面后，小红点消失  前提是有小红点时
        /// </summary>
        public void ResetRedPoint()
        {
            if (RedPointFlag && isRedPointShow)
            {
                //红点状态改变时 记录时间
                SaveCurrTimeStamp();

                RedPointFlag = false;
            }
        }

        public void RefreshRedPoint()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMonthCardRewardRedPointReset);
        }

        #region Time Utility

        private int GetTempTimeStamp()
        {
            return PlayerPrefsManager.GetInstance().GetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.MonthCardRewardRedPointUpdateTime);
        }

        private void SaveCurrTimeStamp()
        {
            int currTimeStamp = GetCurrTimeStamp();
            PlayerPrefsManager.GetInstance().SetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.MonthCardRewardRedPointUpdateTime, currTimeStamp);
        }

        private int GetCurrTimeStamp()
        {
            //获取当前时间点
            int currTimeStamp = (int)TimeManager.GetInstance().GetServerTime();
            //Logger.LogErrorFormat("[MonthCardRewardLockersDataManager] - GetCurrTimeStamp is {0}", currTimeStamp);
            return currTimeStamp;
        }

        private int GetCurrTimeHour()
        {
            double currTimeStamp = TimeManager.GetInstance().GetServerDoubleTime();
            System.DateTime currDateTime = Function.ConvertIntDateTime(currTimeStamp);
            int currHour = currDateTime.Hour;
            return currHour;
        }

        private System.DateTime GetCurrDateTime()
        {
            double currTimeStamp = TimeManager.GetInstance().GetServerDoubleTime();
            System.DateTime currDateTime = Function.ConvertIntDateTime(currTimeStamp);
            return currDateTime;
        }

        private int GetRefreshTimeStamp()
        {
            int currHour = GetCurrTimeHour();
            System.DateTime currDateTime = GetCurrDateTime();
            System.DateTime refreshDateTime;
            int refreshDateTimeStamp = 0;
            // 还没到今天的刷新时间 用昨天的刷新时间判断
            if (m_RedPointUpdateHour >= currHour)
            {
                refreshDateTime = Function.GetYesterdayGivenHourTime(m_RedPointUpdateHour);
            }
            else //到了今天刷新时间点 用今天的时间
            {
                refreshDateTime = Function.GetTodayGivenHourTime(m_RedPointUpdateHour);
            }
            refreshDateTimeStamp = System.Convert.ToInt32(Function.ConvertDateTimeInt(refreshDateTime));
            return refreshDateTimeStamp;
        }

        #endregion


        #region PUBLIC STATIC METHODS

        /// <summary>
        /// 判断道具是否为极品道具
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        public static bool IsHighestGradeItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return false;
            }
            bool isEquipHighestGrade = false;
            bool isTreasInTable = false;
            var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemData.TableID);
            if (itemTable != null)
            {
                isTreasInTable = itemTable.IsTreas == 1;

                if (itemTable.Type == ProtoTable.ItemTable.eType.EQUIP)
                {
                    isEquipHighestGrade = EquipHandbookDataManager.GetInstance().IsHighestGradeItem(itemData.TableID);
                }
            }
            return itemData.IsTreasure || isTreasInTable || isEquipHighestGrade;
        }

        #endregion

        #endregion
    }
}
