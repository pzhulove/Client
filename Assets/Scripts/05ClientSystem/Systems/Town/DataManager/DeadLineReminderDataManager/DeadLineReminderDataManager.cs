using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using Network;

namespace GameClient
{
    public enum DeadLineReminderType
    {
        DRT_NONE = 0,
        DRT_NOTIFYINFO,//通知信息
        DRT_LIMITTIMEITEM,//限时道具
    }

    public enum DeadLineTimePoint
    {
        Day,
        Hour
    }

    public class DeadLineReminderModel
    {
        public DeadLineReminderType type;
        public NotifyInfo info;
        public ItemData itemData;
    }    

    public class DeadLineNotifyFlag
    {
        public NotifyType notifyType;
        public bool isThisLoginNotified = false;
    }

    public class DeadLineReminderDataManager : DataManager<DeadLineReminderDataManager>
    {
        List<DeadLineReminderModel> mDeadLineReminderModelList = new List<DeadLineReminderModel>();

        List<DeadLineNotifyFlag> mDeadLineNotifyFlagList = new List<DeadLineNotifyFlag>();
        private float mUpdateTimer = 0f;
        private float mUpdateCD = 60f;
        private bool mUpdateFlag = false;
        private int mRefreshHour = 6;

        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public sealed override void Clear()
        {
            if (mDeadLineReminderModelList != null)
            {
                mDeadLineReminderModelList.Clear();
            }

            _UnBindNetMessage();
            _ClearDeadlineNotifyFlagList();
            mUpdateTimer = 0f;
        }

        public sealed override void Initialize()
        {
            _BindNetMessage();
            _InitDeadlineNotifyFlagList();
            mUpdateFlag = ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(ServiceType.SERVICE_CURRENCY_DEADLINE_CHECK);
            var systemValueTable = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType3.SVT_CURRENCY_QUICK_TIPS_REFRESH_CD);
            if (systemValueTable != null)
            {
                mUpdateCD = systemValueTable.Value;
            }
            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType2.SVT_DAILY_TODO_REFRESH_TIME);
            if (systemValue != null)
            {
                mRefreshHour = systemValue.Value;
            }
        }

        public sealed override void Update(float a_fTime)
        {
            if (!mUpdateFlag)
            {
                return;
            }
            if (ClientSystemManager.GetInstance().CurrentSystem == null ||
               (ClientSystemManager.GetInstance().CurrentSystem is ClientSystemTown) == false)
            {
                return;
            }
            mUpdateTimer += a_fTime;
            if (mUpdateTimer >= mUpdateCD)
            {
                CheckCurrencyDeadlineStatus();
                mUpdateTimer = 0f;
            }
        }

        /// <summary>
        /// 绑定网络消息
        /// </summary>
        private void _BindNetMessage()
        {
            NetProcess.AddMsgHandler(SceneUpdateNotifyList.MsgID, OnAddNotify);
            ServerSceneFuncSwitchManager.GetInstance().AddServerFuncSwitchListener(ServiceType.SERVICE_CURRENCY_DEADLINE_CHECK, _OnServiceFuncChange);
        }

        /// <summary>
        /// 解绑网络消息
        /// </summary>
        private void _UnBindNetMessage()
        {
            NetProcess.RemoveMsgHandler(SceneUpdateNotifyList.MsgID, OnAddNotify);
            ServerSceneFuncSwitchManager.GetInstance().RemoveServerFuncSwitchListener(ServiceType.SERVICE_CURRENCY_DEADLINE_CHECK, _OnServiceFuncChange);
        }

        private void _OnServiceFuncChange(ServerSceneFuncSwitch ssfSwitch)
        {
            if (ssfSwitch.sType == ServiceType.SERVICE_CURRENCY_DEADLINE_CHECK)
            {
                mUpdateFlag = ssfSwitch.sIsOpen;
            }
        }

        public List<DeadLineReminderModel> GetDeadLineReminderModelList()
        {
            if (mDeadLineReminderModelList == null)
            {
                mDeadLineReminderModelList = new List<DeadLineReminderModel>();
            }

            return mDeadLineReminderModelList;
        }

        public void Add(DeadLineReminderModel model)
        {
            mDeadLineReminderModelList.Add(model);
        }

        public void RemoveAll(ulong guid)
        {
            mDeadLineReminderModelList.RemoveAll(x =>
            {
                if (x.type != DeadLineReminderType.DRT_LIMITTIMEITEM)
                {
                    return false;
                }

                if (x.itemData == null)
                {
                    return true;
                }

                return x.itemData.GUID == guid;
            });
        }

        public void Sort()
        {
            mDeadLineReminderModelList.Sort((var1, var2) =>
            {
                if (var1.type == var2.type && var1.itemData != null && var2.itemData != null)
                {
                    int nTime1, nTime2;
                    {
                        bool bStartCountdown;
                        var1.itemData.GetTimeLeft(out nTime1, out bStartCountdown);
                    }
                    {
                        bool bStartCountdown;
                        var2.itemData.GetTimeLeft(out nTime2, out bStartCountdown);
                    }

                    if (nTime1 <= 0)
                    {
                        if (nTime2 > 0)
                        {
                            return -1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        if (nTime2 <= 0)
                        {
                            return 1;
                        }
                        else
                        {
                            return nTime1 - nTime2;
                        }
                    }
                }

                return var1.type.CompareTo(var2.type);
            });
        }

        public sealed override void OnBindEnterGameMsg()
        {
            EnterGameBinding eb = new EnterGameBinding();
            eb.id = SceneInitNotifyList.MsgID;

            try
            {
                eb.method = OnInitNotifyInfos;
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("错误!! 绑定消息{0}(ID:{1})到方法", ProtocolHelper.instance.GetName(eb.id), eb.id);
            }

            m_arrEnterGameBindings.Add(eb);
        }

        private void OnInitNotifyInfos(MsgDATA data)
        {
            SceneInitNotifyList msgData = new SceneInitNotifyList();
            msgData.decode(data.bytes);

            for (int i = 0; i < msgData.notifys.Length; i++)
            {
                NotifyInfo info = msgData.notifys[i];
                if (info == null)
                {
                    continue;
                }

                if (!IsDeadlineReminderType((NotifyType)info.type))
                {
                    continue;
                }

                DeadLineReminderModel model = new DeadLineReminderModel()
                {
                    type = DeadLineReminderType.DRT_NOTIFYINFO,
                    info = info
                };

                mDeadLineReminderModelList.Add(model);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DeadLineReminderChanged);
        }

        private void OnAddNotify(MsgDATA msg)
        {
            SceneUpdateNotifyList msgData = new SceneUpdateNotifyList();
            msgData.decode(msg.bytes);

            AddActivityNotice(msgData.notify);
        }

        public void AddActivityNotice(NotifyInfo a_info)
        {
            var notifyType = (NotifyType)a_info.type;

            if (!IsDeadlineReminderType(notifyType))
            {
                return;
            }

            if (_IsDeadLineNotified(notifyType))
            {
                return;
            }

            if (_IsDeadLineNotifiedToday(notifyType))
            {
                return;
            }

            bool bFind = false;

            for (int i = 0; i < mDeadLineReminderModelList.Count; i++)
            {
                if (mDeadLineReminderModelList[i].type == DeadLineReminderType.DRT_NOTIFYINFO && mDeadLineReminderModelList[i].info.type == a_info.type && mDeadLineReminderModelList[i].info.param == a_info.param)
                {
                    mDeadLineReminderModelList[i].info = a_info;
                    bFind = true;

                    break;
                }
            }

            if (!bFind)
            {
                DeadLineReminderModel model = new DeadLineReminderModel()
                {
                    type = DeadLineReminderType.DRT_NOTIFYINFO,
                    info = a_info
                };

                mDeadLineReminderModelList.Add(model);

                _UpdateDeadLineNotifyFlagList(notifyType, true);

                _SaveDeadLineNotifyTimestamp(notifyType);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DeadLineReminderChanged);
            }
        }

        public void DeleteActivityNotice(NotifyInfo a_info)
        {
            for (int i = 0; i < mDeadLineReminderModelList.Count; i++)
            {
                if (mDeadLineReminderModelList[i].type == DeadLineReminderType.DRT_NOTIFYINFO && mDeadLineReminderModelList[i].info.type == a_info.type && mDeadLineReminderModelList[i].info.param == a_info.param)
                {
                    mDeadLineReminderModelList.RemoveAt(i);
                    SendMsgRemoveNotice(a_info);
                    break;
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DeadLineReminderChanged);
        }

        public void DeleteLimitTimeItem(ItemData itemData)
        {
            for (int i = 0; i < mDeadLineReminderModelList.Count; i++)
            {
                if (mDeadLineReminderModelList[i].type != DeadLineReminderType.DRT_LIMITTIMEITEM)
                {
                    continue;
                }

                if (mDeadLineReminderModelList[i].itemData.GUID != itemData.GUID)
                {
                    continue;
                }

                mDeadLineReminderModelList.RemoveAt(i);

                SendMsgRemoveLimitTimeItem(itemData);
                break;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DeadLineReminderChanged);
        }

        /// <summary>
        /// 删除失效道具
        /// </summary>
        public void DeleteFailureItem()
        {
            mDeadLineReminderModelList.RemoveAll(x =>
            {
                if (x.type != DeadLineReminderType.DRT_LIMITTIMEITEM)
                {
                    return false;
                }

                if (x.itemData == null)
                {
                    return true;
                }

                int nTimeLeft;
                bool bStartCountdown;
                x.itemData.GetTimeLeft(out nTimeLeft, out bStartCountdown);
                if (bStartCountdown == true && nTimeLeft <= 0)
                {
                    SendMsgRemoveLimitTimeItem(x.itemData);
                    return true;
                }

                return false;
            });

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DeadLineReminderChanged);
        }

        /// <summary>
        /// 删除通知信息
        /// </summary>
        /// <param name="a_info"></param>
        private void SendMsgRemoveNotice(NotifyInfo a_info)
        {
            if (a_info != null)
            {
                SceneDeleteNotifyList msg = new SceneDeleteNotifyList();
                msg.notify = a_info;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        /// <summary>
        /// 删除限时道具
        /// </summary>
        /// <param name="itemData"></param>
        private void SendMsgRemoveLimitTimeItem(ItemData itemData)
        {
            if (itemData != null)
            {
                SceneDeleteNotifyList kSend = new SceneDeleteNotifyList();
                kSend.notify.type = (int)NotifyType.NT_TIME_ITEM;
                kSend.notify.param = itemData.GUID;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
            }
        }

        /// <summary>
        /// 是否属于倒计时提示类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDeadlineReminderType(NotifyType type)
        {
            if (type == NotifyType.NT_MAGIC_INTEGRAL_EMPTYING ||
                type == NotifyType.NT_MONTH_CARD_REWARD_EXPIRE_24H ||
                type == NotifyType.NT_ADVENTURE_TEAM_BOUNTY_EMPTYING ||
                type == NotifyType.NT_ADVENTURE_TEAM_INHERIT_BLESS_EMPTYING ||
                type == NotifyType.NT_ADVENTURE_PASS_CARD_COIN_EMPTYING)
            {
                return true;
            }
            return false;
        }

        public string GetDeadlineReminderItemIcon(NotifyType type)
        {
            string iconPath = string.Empty;
            var currencyQuickTipsTable = TableManager.GetInstance().GetTable<ProtoTable.CurrencyQuickTipsTable>();
            if (currencyQuickTipsTable == null)
            {
                return iconPath;
            }
            var enumerator = currencyQuickTipsTable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var table = enumerator.Current.Value as ProtoTable.CurrencyQuickTipsTable;
                if (table == null)
                    continue;
                if ((int)table.NotifyType == (int)type)
                {
                    return table.NotifyIcon;
                }
            }
            return iconPath;
        }

        private void _InitDeadlineNotifyFlagList()
        {
            if (mDeadLineNotifyFlagList == null)
            {
                mDeadLineNotifyFlagList = new List<DeadLineNotifyFlag>();
            }

            for (int i = 0; i < (int)NotifyType.NT_MAX; i++)
            {
                var type = (NotifyType)i;
                if (!IsDeadlineReminderType(type))
                {
                    continue;
                }
                DeadLineNotifyFlag flag = new DeadLineNotifyFlag()
                {
                    notifyType = type,
                    isThisLoginNotified = false
                };
                mDeadLineNotifyFlagList.Add(flag);
            }            
        }

        private void _ClearDeadlineNotifyFlagList()
        {
            if (mDeadLineNotifyFlagList != null)
            {
                mDeadLineNotifyFlagList.Clear();
            }
        }

        private void _UpdateDeadLineNotifyFlagList(NotifyType type, bool isNotified)
        {
            if (mDeadLineNotifyFlagList == null)
            {
                return;
            }

            for (int i = 0; i < mDeadLineNotifyFlagList.Count; i++)
            {
                var flagObj = mDeadLineNotifyFlagList[i];
                if (flagObj == null)
                    continue;
                if (flagObj.notifyType == type)
                {
                    flagObj.isThisLoginNotified = isNotified;
                    break;
                }
            }
        }

        private bool _IsDeadLineNotified(NotifyType type)
        {
            if (mDeadLineNotifyFlagList == null)
            {
                return false;
            }

            for (int i = 0; i < mDeadLineNotifyFlagList.Count; i++)
            {
                var flagObj = mDeadLineNotifyFlagList[i];
                if (flagObj == null)
                    continue;
                if (flagObj.notifyType == type)
                {
                    return flagObj.isThisLoginNotified;
                }
            }
            return false;
        }

        private bool _IsDeadLineNotifiedToday(NotifyType type)
        {
            int localTime = PlayerPrefsManager.GetInstance().GetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.CurrencyDeadLineTipsTime, string.Format("NotifyType{0}",(int)type));
            if (localTime > Function.GetRefreshHourTimeStamp(mRefreshHour))
            {
                return true;
            }
            return false;
        }

        private void _SaveDeadLineNotifyTimestamp(NotifyType type)
        {
            int currentTimestamp = Function.GetCurrTimeStamp();
            PlayerPrefsManager.GetInstance().SetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.CurrencyDeadLineTipsTime, currentTimestamp, string.Format("NotifyType{0}", (int)type));
        }


        #region Currency Quick tips  货币快捷提示

        public void CheckCurrencyDeadlineStatus()
        {
            var currencyQuickTipsTable = TableManager.GetInstance().GetTable<ProtoTable.CurrencyQuickTipsTable>();
            if (currencyQuickTipsTable == null)
            {
                return;
            }
            var enumerator = currencyQuickTipsTable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var table = enumerator.Current.Value as ProtoTable.CurrencyQuickTipsTable;
                if (table == null)
                    continue;
                if(table.NotifyType == ProtoTable.CurrencyQuickTipsTable.eNotifyType.NT_ADVENTURE_PASS_CARD_COIN_EMPTYING)
                {
                    if(!IsAdventurnPassCardSeasonInDeadLine(table))
                    {
                        continue;
                    }
                }
                else
                {
                if (!_IsOwnedSpecialCurrency(table.NotifyType))
                {
                    if (!_IsOwnedCurrency(table.ID))
                        continue;
                    }
                }
                NotifyInfo noticeData = new NotifyInfo
                {
                    type = (uint)table.NotifyType
                };
                if (_IsInDeadline(table))
                {
                    AddActivityNotice(noticeData);
                }
                else
                {
                    DeleteActivityNotice(noticeData);
                }
            }
        }

        private bool _IsOwnedCurrency(int itemTableId)
        {
            bool isOwned = false;
            if (ItemDataManager.GetInstance().GetOwnedItemCount(itemTableId) > 0)
            {
                isOwned = true;
            }
            return isOwned;
        }

        private bool _IsOwnedSpecialCurrency(ProtoTable.CurrencyQuickTipsTable.eNotifyType notifyType)
        {
            if (notifyType == ProtoTable.CurrencyQuickTipsTable.eNotifyType.NT_ADVENTURE_TEAM_INHERIT_BLESS_EMPTYING)
            {
                ulong ownPassBlessCount = AdventureTeamDataManager.GetInstance().GetAdventureTeamPassBlessCount();
                ulong ownPassBlessExp = AdventureTeamDataManager.GetInstance().GetAdventureTeamPassBlessExp();
                ulong unitPassBlessExp = AdventureTeamDataManager.GetInstance().GetAdventureTeamPassBlessUnitExp();
                if (ownPassBlessCount > 0 || ownPassBlessExp >= unitPassBlessExp)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsAdventurnPassCardSeasonInDeadLine(ProtoTable.CurrencyQuickTipsTable currencyQuickTipsTable)
        {
            if(currencyQuickTipsTable == null || currencyQuickTipsTable.NotifyType != ProtoTable.CurrencyQuickTipsTable.eNotifyType.NT_ADVENTURE_PASS_CARD_COIN_EMPTYING)
            {
                return false;
            }
            if(AdventurerPassCardDataManager.GetInstance().SeasonID == 0)
            {
                return false;
            }
            // 赛季开启中，但是功能不可用，说明玩家没有解锁该功能，原因有2: 
            // 1,玩家等级不到 2，赛季结束前X天登录游戏是无法解锁通行证的
            // 上述两种情况都不需要提示玩家赛季快结束了
            if(AdventurerPassCardDataManager.GetInstance().CardLv == 0)
            {
                return false;
            }
            uint endTime = (uint)(TimeManager.GetInstance().GetServerTime() + currencyQuickTipsTable.PromptTime * 3600 * 24);
            return endTime >= AdventurerPassCardDataManager.GetInstance().GetSeasonEndTime();
        }
        private bool _IsInDeadline(ProtoTable.CurrencyQuickTipsTable table)
        {
            if(table != null && table.NotifyType == ProtoTable.CurrencyQuickTipsTable.eNotifyType.NT_ADVENTURE_PASS_CARD_COIN_EMPTYING)
            {
                return IsAdventurnPassCardSeasonInDeadLine(table);
            }
            bool isIn = false;
            if (table == null)
            {
                return isIn;
            }

            int day = _GetTimeFromTable(table.ResetTimePoint, table.ResetType, DeadLineTimePoint.Day);
            int hour = _GetTimeFromTable(table.ResetTimePoint, table.ResetType, DeadLineTimePoint.Hour);
            DateTime[] durationDT = _GetDurtionDateTime(table.ResetType, day, hour);

            if (durationDT == null || durationDT.Length != 2)
            {
                return isIn;
            }

            DateTime thisDT = durationDT[0];
            DateTime nextDT = durationDT[1];
            DateTime currDT = Function.GetCurrDateTime();
            DateTime thisBeforeDT = thisDT.AddDays(-table.PromptTime);
            DateTime nextBeforeDT = nextDT.AddDays(-table.PromptTime);


            if (DateTime.Compare(nextBeforeDT, thisDT) < 0)
            {
                Logger.LogErrorFormat("[DeadLineReminderDateManager] - _GetDeadlineStatus, 注意检查【货币快捷提示表】， 下一次重置时间的前置时间点 在 本次重置时间点 之前了 ！！！");
                return isIn;
            }

            if (DateTime.Compare(currDT, thisBeforeDT) < 0)
            {
                isIn = false;
            }
            else if ((DateTime.Compare(currDT, thisBeforeDT) >= 0) && (DateTime.Compare(currDT, thisDT) < 0))
            {
                isIn = true;
            }
            else if ((DateTime.Compare(currDT, thisDT) >= 0) && (DateTime.Compare(currDT, nextBeforeDT) < 0))
            {
                isIn = false;
            }
            else if ((DateTime.Compare(currDT, nextBeforeDT) >= 0))
            {
                isIn = true;
            }



            return isIn;
        }

        private int _GetTimeFromTable(string tableTimes, 
            ProtoTable.CurrencyQuickTipsTable.eResetType type, 
            DeadLineTimePoint timePoint)
        {
            int day, hour = 0;

            if (string.IsNullOrEmpty(tableTimes))
            {
                return 0;
            }

            if (type == ProtoTable.CurrencyQuickTipsTable.eResetType.None)
            {
                return 0;
            }

            if (type == ProtoTable.CurrencyQuickTipsTable.eResetType.Daily)
            {
                if (int.TryParse(tableTimes, out hour))
                {
                    return hour;
                }
            }

            string[] timeArray = tableTimes.Split('|');
            if (timeArray == null || timeArray.Length != 2)
            {
                return 0;
            }
            if (timePoint == DeadLineTimePoint.Day && int.TryParse(timeArray[0], out day))
            {
                return day;
            }
            else if (timePoint == DeadLineTimePoint.Hour && int.TryParse(timeArray[1], out hour))
            {
                return hour;
            }
            return 0;
        }

        private DateTime[] _GetDurtionDateTime(ProtoTable.CurrencyQuickTipsTable.eResetType type, int day, int hour)
        {
            DateTime start = Function.GetCurrDateTime(), end = Function.GetCurrDateTime();
            DateTime tempStart, tempEnd;
            switch (type)
            {
                case ProtoTable.CurrencyQuickTipsTable.eResetType.Monthly:
                    tempStart = Function.GetThisMonthdayDateTime(day);
                    start = new DateTime(tempStart.Year, tempStart.Month, tempStart.Day, hour, 0 ,0);
                    tempEnd = Function.GetNextMonthdayDateTime(day);
                    end = new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day, hour, 0, 0);
                    break;
                case ProtoTable.CurrencyQuickTipsTable.eResetType.Weekly:
                    if (day == 7)
                    {
                        day = 0;
                    } else if (day < 0)
                    {
                        day = 1;
                    } else if (day > 7)
                    {
                        day = 7;
                    }
                    tempStart = Function.GetThisWeekdayDateTime((DayOfWeek)day);
                    start = new DateTime(tempStart.Year, tempStart.Month, tempStart.Day, hour, 0, 0);
                    tempEnd = Function.GetNextWeekdayDateTime((DayOfWeek)day);
                    end = new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day, hour, 0, 0);
                    break;
                case ProtoTable.CurrencyQuickTipsTable.eResetType.Daily:
                    tempStart = Function.GetCurrDateTime();
                    start = new DateTime(tempStart.Year, tempStart.Month, tempStart.Day, hour, 0, 0);
                    break;
            }
            return new DateTime[] { start, end };
        }

        #endregion

    }
}