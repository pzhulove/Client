using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;
using System;
using ActivityLimitTime;
using LimitTimeGift;
using System.Collections;
using System.Text.RegularExpressions;

namespace GameClient
{
    public enum SevenDaysType
    {
        Login,
        Target,
        Gift,
        Charge,
        Max,
    }

    public class SevenDaysData
    {
        public List<ItemData> itemDatas = null;
        public TaskStatus taskStatus = TaskStatus.TASK_UNFINISH;
        public int activeId = -1;
        public string curValueStr = "0";
    }

    public class SevenDaysTargetData : SevenDaysData
    {
        public string desc = string.Empty;
        public ActiveTable activeTable;
        public string targetValue = "0";
        public string name = string.Empty;
        public int scoreNum = 0;
    }

    public class SevenDaysGiftData : SevenDaysData
    {
        public string desc = string.Empty;
        public string nowCostIconPath = string.Empty;
        public string nowCost = string.Empty;
        public int day = 0;
    }

    public class SevenDaysChargeData : SevenDaysData
    {
        public string targetCharge = "0";
        public ActiveTable activeTable = null;
    }

    public class SevenDaysScoreData : SevenDaysData
    {
        public int targetScore = 0;
    }

    public enum SevenDaysActiveStatus
    {
        Over,
        Start,
        Prepare,
    }

    public class SevendaysDataManager : DataManager<SevendaysDataManager>
    {
        private enum RegexType
        {
            init,
            update,
            mainInit,
        }

        private List<SevenDaysData> mSevenDaysLoginDatas = new List<SevenDaysData>();
        private Dictionary<int, List<SevenDaysTargetData>> mDicSevenDaysTargetDatas = new Dictionary<int, List<SevenDaysTargetData>>();     //key 第几天 value 七日礼包数据
        private List<SevenDaysGiftData> mSevenDaysGiftDatas = new List<SevenDaysGiftData>();
        private List<SevenDaysChargeData> mSevenDaysChargeDatas = new List<SevenDaysChargeData>();
        private List<SevenDaysScoreData> mSevenDaysScoreDatas = new List<SevenDaysScoreData>();       //七日积分
        private Dictionary<int, SevenDaysActiveStatus> mDicSevenDaysActiveStatus = new Dictionary<int, SevenDaysActiveStatus>();        //七日活动开启状态 key 模板id  value  开启状态

        private bool bInited = false;

        private const int mDescIndex = 0;
        private const int mNowCostNumIndex = 2;
        private const int mNowCostIcon = 5;
        private const int mUpdateDescIndex = 1;
        private const int mUpdateTargetValueIndex = 3;
        private const int mMainInitDescIndex = 2;
        private const int mMainInitNameIndex = 3;
        private const int mScoreItemId = 830000024;     //积分道具id

        private bool mIsShowLoginRedPoint = false;
        private List<bool> mIsShowTargetRedPoints = new List<bool>();
        private bool mIsShowChargeRedPoint = false;
        private bool mIsShowGiftRedPoint = false;
        private bool mIsShowScoreRedPoint = false;

        public const int activityTypeId = 9388;       //活动类型id
        public const int MaxDay = 7;
        public bool IsGiftCheck = false;

        public static Regex s_regex_tabinit = new Regex(@"<prefabkey=(\w+) localpath=([A-Za-z0-9/]+) type=(\w+) value=(.+)>");
        public static Regex s_regex_tabupdate = new Regex(@"<slider=([A-Za-z0-9/]+) k=(\w+) v=(.+) mode=(.+)>");
        public static Regex s_regex_tabmaininitdesc = new Regex(@"<Name=([A-Za-z0-9/]+) Type=(\w+) Value=(.+)>");

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.SevendaysDataManager;
        }

        //初始化
        public override void Initialize()
        {
            Clear();
            _LoadTable();
            _BindNetMsg();
        }

        public override void Clear()
        {
            _UnBindNetMsg();
            if (mSevenDaysLoginDatas != null)
            {
                mSevenDaysLoginDatas.Clear();
            }

            if (mDicSevenDaysTargetDatas != null)
            {
                mDicSevenDaysTargetDatas.Clear();
            }

            if (mSevenDaysGiftDatas != null)
            {
                mSevenDaysGiftDatas.Clear();
            }

            if (mSevenDaysChargeDatas != null)
            {
                mSevenDaysChargeDatas.Clear();
            }

            if (mSevenDaysScoreDatas != null)
            {
                mSevenDaysScoreDatas.Clear();
            }

            if (mDicSevenDaysActiveStatus != null)
            {
                mDicSevenDaysActiveStatus.Clear();
            }

            IsGiftCheck = false;
            bInited = false;
        }

        void _BindNetMsg()
        {

        }

        void _UnBindNetMsg()
        {

        }

        private void _LoadTable()
        {
            Dictionary<int, object> dic = TableManager.GetInstance().GetTable<SevenDaysActiveTable>();
            foreach (var key in dic.Keys)
            {
                if (dic[key] == null)
                {
                    continue;
                }

                SevenDaysActiveTable sevenDaysActiveTable = dic[key] as SevenDaysActiveTable;
                ActiveTable activeItem = TableManager.GetInstance().GetTableItem<ActiveTable>(key);

                if (activeItem != null && sevenDaysActiveTable != null)
                {
                    switch (sevenDaysActiveTable.SevenDaysActiveType)
                    {
                        case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Login:
                            {
                                SevenDaysData sevenDaysLoginData = new SevenDaysData();
                                sevenDaysLoginData.activeId = key;
                                int temp = 0;
                                sevenDaysLoginData.itemDatas = _GetItemDatas(activeItem, ref temp);
                                sevenDaysLoginData.taskStatus = TaskStatus.TASK_INIT;

                                mSevenDaysLoginDatas.Add(sevenDaysLoginData);
                            }
                            break;
                        case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Target:
                            {
                                int day = sevenDaysActiveTable.Day;
                                SevenDaysTargetData sevenDaysTargetData = new SevenDaysTargetData();
                                sevenDaysTargetData.activeId = key;
                                sevenDaysTargetData.itemDatas = _GetItemDatas(activeItem, ref sevenDaysTargetData.scoreNum);
                                sevenDaysTargetData.taskStatus = TaskStatus.TASK_INIT;
                                sevenDaysTargetData.desc = _GetActiveTableDesc(activeItem.InitDesc, mDescIndex);
                                sevenDaysTargetData.activeTable = activeItem;
                                sevenDaysTargetData.targetValue = _GetActiveTableDesc(activeItem.UpdateDesc, mUpdateDescIndex, mUpdateTargetValueIndex, RegexType.update);

                                ActiveMainTable activeMainTable = TableManager.GetInstance().GetTableItem<ActiveMainTable>(activeItem.TemplateID);
                                if (activeMainTable != null)
                                {
                                    sevenDaysTargetData.name = sevenDaysActiveTable.Name;
                                }

                                if (mDicSevenDaysTargetDatas.ContainsKey(day))
                                {
                                    mDicSevenDaysTargetDatas[day].Add(sevenDaysTargetData);
                                }
                                else
                                {
                                    mDicSevenDaysTargetDatas.Add(day, new List<SevenDaysTargetData>() { sevenDaysTargetData });
                                }
                            }
                            break;
                        case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Gift:
                            {
                                SevenDaysGiftData sevenDaysGiftData = new SevenDaysGiftData();
                                sevenDaysGiftData.activeId = key;
                                int temp = 0;
                                sevenDaysGiftData.itemDatas = _GetItemDatas(activeItem, ref temp);
                                sevenDaysGiftData.taskStatus = TaskStatus.TASK_INIT;
                                sevenDaysGiftData.desc = _GetActiveTableDesc(activeItem.InitDesc, mDescIndex);
                                sevenDaysGiftData.nowCost = _GetActiveTableDesc(activeItem.InitDesc, mNowCostNumIndex);
                                sevenDaysGiftData.nowCostIconPath = _GetActiveTableDesc(activeItem.InitDesc, mNowCostIcon);
                                sevenDaysGiftData.day = sevenDaysActiveTable.Day;

                                mSevenDaysGiftDatas.Add(sevenDaysGiftData);
                            }
                            break;
                        case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Charge:
                            {
                                SevenDaysChargeData sevenDaysChargeData = new SevenDaysChargeData();
                                sevenDaysChargeData.activeId = key;
                                int temp = 0;
                                sevenDaysChargeData.itemDatas = _GetItemDatas(activeItem, ref temp);
                                sevenDaysChargeData.taskStatus = TaskStatus.TASK_INIT;
                                sevenDaysChargeData.targetCharge = _GetActiveTableDesc(activeItem.UpdateDesc, mUpdateDescIndex, mUpdateTargetValueIndex, RegexType.update);
                                sevenDaysChargeData.activeTable = activeItem;

                                mSevenDaysChargeDatas.Add(sevenDaysChargeData);
                            }
                            break;
                        case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Score:
                            {
                                SevenDaysScoreData sevenDaysScoreData = new SevenDaysScoreData();
                                sevenDaysScoreData.activeId = key;
                                int temp = 0;
                                sevenDaysScoreData.itemDatas = _GetItemDatas(activeItem, ref temp);
                                sevenDaysScoreData.taskStatus = TaskStatus.TASK_INIT;
                                int.TryParse(_GetActiveTableDesc(activeItem.UpdateDesc, mUpdateDescIndex, mUpdateTargetValueIndex, RegexType.update), out sevenDaysScoreData.targetScore);

                                mSevenDaysScoreDatas.Add(sevenDaysScoreData);
                            }
                            break;
                    }
                }
            }

            mSevenDaysLoginDatas.Sort(_SortLoginDatas);
        }

        /// <summary>
        /// 解析活动表里的initdesc和updatedesc中的数据 比如initdesc中的描述 名字 折扣价  updatedesc中的充值目标
        /// </summary>
        /// <param name="activeTable"></param>
        /// <param name="firstIndex"></param>
        /// <param name="secondIndex"></param>
        /// <returns></returns>
        private string _GetActiveTableDesc(string strResolved, int firstIndex, int secondIndex = 4, RegexType regexType = RegexType.init)
        {
            string strDesc = string.Empty;

            if (string.IsNullOrEmpty(strResolved))
            {
                return strDesc;
            }

            var descs = strResolved.Split(new char[] { '\r', '\n' });
            if (descs != null && descs.Length > firstIndex && !string.IsNullOrEmpty(descs[firstIndex]))
            {
                Match match = null;
                if (regexType == RegexType.init)
                {
                    match = s_regex_tabinit.Match(descs[firstIndex]);
                }
                else if (regexType == RegexType.update)
                {
                    match = s_regex_tabupdate.Match(descs[firstIndex]);
                }
                else if (regexType == RegexType.mainInit)
                {
                    match = s_regex_tabmaininitdesc.Match(descs[firstIndex]);
                }
                if (match != null && match.Groups != null && match.Groups.Count > secondIndex)
                {
                    strDesc = match.Groups[secondIndex].Value;
                }
            }

            return strDesc;
        }

        public void UpdateRecvSceneSyncActiveTaskListNormal(SceneSyncActiveTaskList kRect)
        {
            Logger.LogProcessFormat("OnRecvSceneSyncActiveTaskList!");

            bool isSevendaysUpdate = false;

            for (int i = 0; i < kRect.tasks.Length; ++i)
            {
                var current = kRect.tasks[i];
                SevenDaysActiveTable sevenDaysActiveTable = TableManager.GetInstance().GetTableItem<SevenDaysActiveTable>((int)current.taskID);
                if (sevenDaysActiveTable == null)
                {
                    continue;
                }

                isSevendaysUpdate = true;

                string curValueStr = string.Empty;
                if (current.akMissionPairs != null && current.akMissionPairs.Length > 0 && current.akMissionPairs[0] != null)
                {
                    curValueStr = current.akMissionPairs[0].value;
                }

                _UpdateActiveTask(sevenDaysActiveTable, current.status, curValueStr);
            }

            if (isSevendaysUpdate)
            {
                mIsShowLoginRedPoint = _IsShowRedPoint(mSevenDaysLoginDatas);
                mIsShowTargetRedPoints.Clear();
                for (int i = 1; i < MaxDay; i++)
                {
                    mIsShowTargetRedPoints.Add(_IsTargetShowRedPointByDay(i));
                }
                mIsShowGiftRedPoint = _IsGiftShowRedPoint();
                mIsShowChargeRedPoint = _IsShowRedPoint(mSevenDaysChargeDatas);
                mIsShowScoreRedPoint = _IsShowRedPoint(mSevenDaysScoreDatas);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SevenDaysActivityUpdate);
            }
        }

        /// <summary>
        /// 活动状态有变化的时候同步一遍变化的活动
        /// </summary>
        /// <param name="res"></param>
        public void UpdateRecvSceneNotifyActiveTaskStatus(SceneNotifyActiveTaskStatus res)
        {
            SevenDaysActiveTable sevenDaysActiveTable = TableManager.GetInstance().GetTableItem<SevenDaysActiveTable>((int)res.taskId);

            if (sevenDaysActiveTable == null)
            {
                return;
            }

            _UpdateActiveTask(sevenDaysActiveTable, res.status);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SevenDaysActivityUpdate, sevenDaysActiveTable.SevenDaysActiveType);
            _UpdateRedPointShowData(sevenDaysActiveTable);
        }

        /// <summary>
        /// 活动进度有变化的时候同步一遍变化的活动
        /// </summary>
        /// <param name="data"></param>
        public void UpdateSevenDaysSceneNotifyActiveTaskVar(SceneNotifyActiveTaskVar kRecv)
        {
            SevenDaysActiveTable sevenDaysActiveTable = TableManager.GetInstance().GetTableItem<SevenDaysActiveTable>((int)kRecv.taskId);
            if (sevenDaysActiveTable == null)
            {
                return;
            }

            _UpdateActiveTask(sevenDaysActiveTable, -1, kRecv.val);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SevenDaysActivityUpdate, sevenDaysActiveTable.SevenDaysActiveType);
            _UpdateRedPointShowData(sevenDaysActiveTable);
        }


        /// <summary>
        /// 七日活动开启状态变更
        /// </summary>
        /// <param name="kRecv"></param>
        public void UpdateRecvSevendaysNotifyClientActivity(WorldNotifyClientActivity kRecv)
        {
            Logger.LogProcessFormat("_OnRecvSevendaysNotifyClientActivity {0}", ObjectDumper.Dump(kRecv));
            int iID = (int)kRecv.id;

            if (mDicSevenDaysActiveStatus.ContainsKey(iID))
            {
                mDicSevenDaysActiveStatus[iID] = (SevenDaysActiveStatus)kRecv.type;
            }
        }

        /// <summary>
        /// 七日活动开启状态初始化
        /// </summary>
        /// <param name="activities"></param>
        public void UpdateRecvSevenDaysSyncClientActivitiesNormal(SceneSyncClientActivities activities)
        {
            Logger.LogProcessFormat("_OnRecvSevenDaysSyncClientActivitiesNormal {0}", ObjectDumper.Dump(activities));

            mDicSevenDaysActiveStatus.Clear();
            for (int i = 0; i < activities.activities.Length; ++i)
            {
                if (activities.activities[i] == null)
                {
                    continue;
                }

                int templateId = (int)activities.activities[i].id;
                ActiveMainTable activeMainTable = TableManager.GetInstance().GetTableItem<ActiveMainTable>(templateId);
                if (activeMainTable == null || activeMainTable.ActiveTypeID != activityTypeId)
                {
                    continue;
                }

                if (mDicSevenDaysActiveStatus.ContainsKey(templateId))
                {
                    mDicSevenDaysActiveStatus[templateId] = (SevenDaysActiveStatus)activities.activities[i].state;
                }
                else
                {
                    mDicSevenDaysActiveStatus.Add(templateId, (SevenDaysActiveStatus)activities.activities[i].state);
                }
            }
        }

        private void _UpdateRedPointShowData(SevenDaysActiveTable sevenDaysActiveTable)
        {
            if (sevenDaysActiveTable == null)
            {
                return;
            }

            switch (sevenDaysActiveTable.SevenDaysActiveType)
            {
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Login:
                    {
                        mIsShowLoginRedPoint = _IsShowRedPoint(mSevenDaysLoginDatas);
                    }
                    break;
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Target:
                    {
                        int day = sevenDaysActiveTable.Day;
                        if (mIsShowTargetRedPoints != null && mIsShowTargetRedPoints.Count >= day)
                        {
                            mIsShowTargetRedPoints[day - 1] = _IsTargetShowRedPointByDay(day);
                        }
                    }
                    break;
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Gift:
                    {
                        mIsShowGiftRedPoint = _IsGiftShowRedPoint();
                    }
                    break;
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Charge:
                    {
                        mIsShowChargeRedPoint = _IsShowRedPoint(mSevenDaysChargeDatas);
                    }
                    break;
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Score:
                    {
                        mIsShowScoreRedPoint = _IsShowRedPoint(mSevenDaysScoreDatas);
                    }
                    break;
            }
        }

        private bool _IsShowRedPoint<T>(List<T> dataList) where T : SevenDaysData
        {
            if (dataList == null)
            {
                return false;
            }

            foreach (var v in dataList)
            {
                if (v == null)
                {
                    continue;
                }

                if (v.taskStatus == TaskStatus.TASK_FINISHED)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 七日目标红点显示 day表示第几天
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private bool _IsTargetShowRedPointByDay(int day)
        {
            if (mDicSevenDaysTargetDatas == null || !mDicSevenDaysTargetDatas.ContainsKey(day) || mDicSevenDaysTargetDatas[day] == null)
            {
                return false;
            }

            foreach (var v in mDicSevenDaysTargetDatas[day])
            {
                if (v == null)
                {
                    continue;
                }

                if (v.taskStatus == TaskStatus.TASK_FINISHED)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 七日礼包红点显示
        /// </summary>
        /// <returns></returns>
        private bool _IsGiftShowRedPoint()
        {
            if (mSevenDaysGiftDatas == null)
            {
                return false;
            }

            if (IsGiftCheck)
            {
                return false;
            }

            foreach (var v in mSevenDaysGiftDatas)
            {
                if (v == null)
                {
                    continue;
                }

                if (v.taskStatus == TaskStatus.TASK_FINISHED)
                {
                    return true;
                }
            }

            return false;
        }

        //七日积分红点显示判断
        private bool _IsScoreShowRedPoint()
        {
            if (mSevenDaysScoreDatas == null)
            {
                return false;
            }

            foreach (var v in mSevenDaysScoreDatas)
            {
                if (v == null)
                {
                    continue;
                }

                if (v.taskStatus == TaskStatus.TASK_FINISHED)
                {
                    return true;
                }
            }

            return false;
        }

        private void _UpdateActiveTask(SevenDaysActiveTable sevenDaysActiveTable, int status, string curValueStr = "")
        {
            if (sevenDaysActiveTable == null)
            {
                return;
            }

            switch (sevenDaysActiveTable.SevenDaysActiveType)
            {
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Login:
                    {
                        _ChangeTaskStatus(mSevenDaysLoginDatas, sevenDaysActiveTable.ID, status);
                    }
                    break;
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Target:
                    {
                        int day = sevenDaysActiveTable.Day;
                        if (mDicSevenDaysTargetDatas.ContainsKey(day))
                        {
                            _ChangeTaskStatus(mDicSevenDaysTargetDatas[day], sevenDaysActiveTable.ID, status, curValueStr);
                        }
                    }
                    break;
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Gift:
                    {
                        _ChangeTaskStatus(mSevenDaysGiftDatas, sevenDaysActiveTable.ID, status);
                    }
                    break;
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Charge:
                    {
                        _ChangeTaskStatus(mSevenDaysChargeDatas, sevenDaysActiveTable.ID, status, curValueStr);
                    }
                    break;
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Score:
                    {
                        _ChangeTaskStatus(mSevenDaysScoreDatas, sevenDaysActiveTable.ID, status, curValueStr);
                    }
                    break;
            }
        }

        private void _ChangeTaskStatus<T>(List<T> sevenDays, int taskId, int taskStatus, string curValueStr = "") where T : SevenDaysData
        {
            if (sevenDays == null)
            {
                return;
            }

            foreach (var v in sevenDays)
            {
                if (v.activeId == taskId)
                {
                    if (taskStatus >= 0)
                    {
                        v.taskStatus = (TaskStatus)taskStatus;
                    }

                    if (!string.IsNullOrEmpty(curValueStr))
                    {
                        v.curValueStr = curValueStr;
                    }
                }
            }
        }

        private int _SortLoginDatas(SevenDaysData first, SevenDaysData second)
        {
            return first.activeId - second.activeId;
        }

        private Dictionary<uint, int> _GetAwards(ActiveTable activeItem)
        {
            Dictionary<uint, int> finalAwards = new Dictionary<uint, int>();

            if (activeItem.Awards.Length > 1)
            {
                var awards = activeItem.Awards.Split(new char[] { ',' });
                for (int i = 0; i < awards.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(awards[i]))
                    {
                        var substrings = awards[i].Split(new char[] { '_' });
                        if (substrings.Length == 2)
                        {
                            int id = int.Parse(substrings[0]);
                            int iCount = int.Parse(substrings[1]);

                            finalAwards.Add((uint)id, iCount);
                        }
                    }
                }
            }

            return finalAwards;
        }

        /// <summary>
        /// 解析奖励列表  scorenum表示奖励中是否有积分，如果有则将scorenum设为积分数量，并在列表中剔除积分道具
        /// </summary>
        /// <param name="activeTable"></param>
        /// <param name="scoreNum"></param>
        /// <returns></returns>
        private List<ItemData> _GetItemDatas(ActiveTable activeTable, ref int scoreNum)
        {
            Dictionary<uint, int> dicAwards = _GetAwards(activeTable);
            List<ItemData> itemDatas = new List<ItemData>();
            if (dicAwards != null)
            {
                foreach (uint key in dicAwards.Keys)
                {
                    if (key == mScoreItemId)
                    {
                        scoreNum = dicAwards[key];
                    }
                    else
                    {
                        ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)key);
                        itemData.Count = dicAwards[key];
                        itemDatas.Add(itemData);
                    }
                }
            }

            return itemDatas;
        }

        private int _SortDatas(SevenDaysData first, SevenDaysData second)
        {
            if (first.taskStatus == TaskStatus.TASK_FINISHED && second.taskStatus != TaskStatus.TASK_FINISHED)
            {
                return -1;
            }

            if (first.taskStatus != TaskStatus.TASK_FINISHED && second.taskStatus == TaskStatus.TASK_FINISHED)
            {
                return 1;
            }

            return first.taskStatus - second.taskStatus;
        }

        #region 获取数据列表接口
        /// <summary>
        /// 获取七日登陆数据列表
        /// </summary>
        /// <returns></returns>
        public List<SevenDaysData> GetSevenDaysLoginDatas()
        {
            List<SevenDaysData> datas = new List<SevenDaysData>();
            
            if (null == mSevenDaysLoginDatas)
            {
                return datas;
            }

            foreach (var v in mSevenDaysLoginDatas)
            {
                datas.Add(v);
            }

            return datas;
        }

        /// <summary>
        /// 获取七日目标数据列表 day表示第几天
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public List<SevenDaysTargetData> GetSevenDaysTargetDatas(int day)
        {
            List<SevenDaysTargetData> sevenDaysDatas = new List<SevenDaysTargetData>(); 

            if (mDicSevenDaysTargetDatas == null)
            {
                return sevenDaysDatas;
            }

            foreach (var key in mDicSevenDaysTargetDatas.Keys)
            {
                if (key == day)
                {
                    foreach (var data in mDicSevenDaysTargetDatas[key])
                    {
                        if (data == null)
                        {
                            continue;
                        }

                        sevenDaysDatas.Add(data);
                    }
                }
            }

            sevenDaysDatas.Sort(_SortDatas);
            return sevenDaysDatas;
        }

        /// <summary>
        /// 获取七日特惠礼包数据列表
        /// </summary>
        /// <returns></returns>
        public List<SevenDaysGiftData> GetSevenDaysGiftDatas()
        {
            List<SevenDaysGiftData> datas = new List<SevenDaysGiftData>();

            if (null == mSevenDaysGiftDatas)
            {
                return datas;
            }

            foreach (var v in mSevenDaysGiftDatas)
            {
                datas.Add(v);
            }

            return datas;
        }

        /// <summary>
        /// 获取可购买的最小的天数索引
        /// </summary>
        /// <returns></returns>
        public int GetMinUnlockGiftDays()
        {
            if (null == mSevenDaysGiftDatas)
            {
                return -1;
            }

            int index = 0;
            foreach (var v in mSevenDaysGiftDatas)
            {
                if (v == null)
                {
                    continue;
                }

                if (v.taskStatus == TaskStatus.TASK_FINISHED)
                {
                    return index + 1;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// 获取可购买的七日礼包数据列表
        /// </summary>
        /// <returns></returns>
        public List<SevenDaysGiftData> GetSevenDaysGiftFinishedDatas()
        {
            List<SevenDaysGiftData> datas = new List<SevenDaysGiftData>();

            if (null == mSevenDaysGiftDatas)
            {
                return datas;
            }

            foreach (var v in mSevenDaysGiftDatas)
            {
                if (v.taskStatus == TaskStatus.TASK_FINISHED)
                {
                    datas.Add(v);
                }
            }

            return datas;
        }

        /// <summary>
        /// 获取七日充值礼包数据列表
        /// </summary>
        /// <returns></returns>
        public List<SevenDaysChargeData> GetSevenDaysChargeDatas()
        {
            List<SevenDaysChargeData> datas = new List<SevenDaysChargeData>();

            if (null == mSevenDaysChargeDatas)
            {
                return datas;
            }

            foreach (var v in mSevenDaysChargeDatas)
            {
                if (v == null)
                {
                    continue;
                }

                datas.Add(v);
            }

            datas.Sort(_SortDatas);
            return datas;
        }

        /// <summary>
        /// 获取七日积分数据列表
        /// </summary>
        /// <returns></returns>
        public List<SevenDaysScoreData> GetSevenDaysScoreDatas()
        {
            List<SevenDaysScoreData> datas = new List<SevenDaysScoreData>();

            if (null == mSevenDaysScoreDatas)
            {
                return datas;
            }

            foreach (var v in mSevenDaysScoreDatas)
            {
                datas.Add(v);
            }

            return datas;
        }
        #endregion

        public bool IsTheDayUnLock(int day)
        {
            if (mSevenDaysLoginDatas == null || mSevenDaysLoginDatas.Count <= day - 1 || mSevenDaysLoginDatas[day - 1] == null)
            {
                return false;
            }

            return mSevenDaysLoginDatas[day - 1].taskStatus != TaskStatus.TASK_INIT;
        }


        /// <summary>
        /// 判断七日活动是否非关闭状态
        /// </summary>
        /// <returns></returns>
        public bool IsSevenDaysActiveOpen()
        {
            if (mDicSevenDaysActiveStatus == null)
            {
                return false;
            }

            foreach (var v in mDicSevenDaysActiveStatus.Values)
            {
                if (v != SevenDaysActiveStatus.Over)
                {
                    return true;
                }
            }

            return false;
        }

        #region 红点判断

        /// <summary>
        /// 七日登陆有无红点
        /// </summary>
        /// <returns></returns>
        public bool IsLoginShowRedPoint()
        {
            return mIsShowLoginRedPoint;
        }

        /// <summary>
        /// 七日目标红点显示
        /// </summary>
        /// <returns></returns>
        public bool IsTargetShowRedPoint()
        {
            if (mIsShowTargetRedPoints == null)
            {
                return false || mIsShowScoreRedPoint;
            }

            foreach(var v in mIsShowTargetRedPoints)
            {
                if (v)
                {
                    return true;
                }
            }

            return false || mIsShowScoreRedPoint;
        }

        /// <summary>
        /// 七日目标红点显示 day表示第几天
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public bool IsTargetShowRedPointByDay(int day)
        {
            if (mIsShowTargetRedPoints == null || mIsShowTargetRedPoints.Count < day)
            {
                return false;
            }

            return mIsShowTargetRedPoints[day - 1];
        }

        /// <summary>
        /// 七日特惠红点显示
        /// </summary>
        /// <returns></returns>
        public bool IsPreferentialShowRedPoint()
        {
            return mIsShowGiftRedPoint || mIsShowChargeRedPoint;
        }

        /// <summary>
        /// 七日礼包红点显示
        /// </summary>
        /// <returns></returns>
        public bool IsGiftShowRedPoint()
        {
            return mIsShowGiftRedPoint;
        }

        /// <summary>
        /// 七日累充红点显示
        /// </summary>
        /// <returns></returns>
        public bool IsChargeShowRedPoint()
        {
            return mIsShowChargeRedPoint;
        }

        /// <summary>
        /// 七日活动红点是否显示
        /// </summary>
        /// <returns></returns>
        public bool IsSevenDaysShowRedPoint()
        {
            return mIsShowGiftRedPoint || mIsShowChargeRedPoint || mIsShowLoginRedPoint || IsTargetShowRedPoint();
        }

        /// <summary>
        /// 七日礼包选中该页签设置该页签相关红点为false
        /// </summary>
        public void SetGiftRedPointFlag()
        {
            IsGiftCheck = true;
            mIsShowGiftRedPoint = false;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SevenDaysActivityUpdate, SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Gift);
        }

        #endregion

        public void SubmitTask(int id, int param)
        {
            SceneActiveTaskSubmit req = new SceneActiveTaskSubmit();
            req.taskId = (uint)id;
            req.param1 = (uint)param;
            
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }
    }
}