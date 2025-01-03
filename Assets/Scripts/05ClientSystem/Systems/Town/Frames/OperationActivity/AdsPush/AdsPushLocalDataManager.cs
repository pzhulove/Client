//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using GameClient;
//using UnityEngine.UI;
//using Network;
//using System;
//using Protocol;
//using ProtoTable;

//namespace AdsPush
//{
//    public class AdsPushFrameManager : Singleton<AdsPushFrameManager>
//    {
//        public System.Action OpenFrameLoopOver;

//        private List<AdsPushData> adsPushDataList;
//        private bool isLoopOpening;
//        private bool hasOpened;

//        private int frameIndex;
//        private int frameCount;
//        private AdsPushFrame currShowFrame;

//        private UnityEngine.Coroutine waitAdsFrameOpenLoop;

//        public override void Init()
//        {
//            // AdsPushDataManager.GetInstance().AddSyncAdsPushDataListener(AddAdsPushFrameInQueue);
//            isLoopOpening = true;
//            hasOpened = false;
//            frameIndex = -1;
//            frameCount = 0;
//            waitAdsFrameOpenLoop = null;
//        }

//        public override void UnInit()
//        {
//            //  AdsPushDataManager.GetInstance().RemoveAllSyncAdsPushDataListener();
//            isLoopOpening = false;
//            hasOpened = false;
//            frameIndex = -1;
//            frameCount = 0;
//            waitAdsFrameOpenLoop = null;
//        }

//        /// <summary>
//        /// 尝试打开广告推送面板
//        /// </summary>
//        /// <param name="onFrameOpenEnd"></param>
//        public void TryOpenAdsPushFrames(System.Action onFrameOpenEnd)
//        {
//            //正在引导时，不弹
//            if (NewbieGuideManager.GetInstance().IsGuidingControl())
//                return;
//            if (hasOpened)
//                return;
//            hasOpened = true;
//            this.OpenFrameLoopOver = onFrameOpenEnd;
//            if (AdsPushServerDataManager.GetInstance().IsPlayerLoginFirstToday())
//            {
//                SyncDatas();
//                OpenAdsFrameByQueue();
//            }
//            else
//            {
//                if (OpenFrameLoopOver != null)
//                    OpenFrameLoopOver();
//            }
//        }

//        private void SyncDatas()
//        {
//            AdsPushServerDataManager.GetInstance().SetAdsPushTableData();
//        }

//        private void OpenAdsFrameByQueue()
//        {
//            isLoopOpening = true;
//            adsPushDataList = AdsPushServerDataManager.GetInstance().adsPushDataList;
//            if (adsPushDataList != null)
//            {
//                if (adsPushDataList.Count == 0)
//                {
//                    if (OpenFrameLoopOver != null)
//                        OpenFrameLoopOver();
//                    return;
//                }
//                waitAdsFrameOpenLoop = GameFrameWork.instance.StartCoroutine(WaitAdsFrameOpenLoop());
//            }
//        }

//        /// <summary>
//        /// 停止之后的广告推送
//        /// </summary>
//        public void TryStopOpenAdsFrame()
//        {
//            if (currShowFrame == null)
//                return;
//            if (frameIndex == frameCount - 1 || currShowFrame.GetCurrFrameItem().currItemType == AdsPushType.Local_WithUrl)
//            {
//                isLoopOpening = false;
//                if (waitAdsFrameOpenLoop != null)
//                    GameFrameWork.instance.StopCoroutine(waitAdsFrameOpenLoop);
//                if (OpenFrameLoopOver != null)
//                    OpenFrameLoopOver();
//            }
//        }

//        IEnumerator WaitAdsFrameOpenLoop()
//        {
//            bool isNext = true;
//            currShowFrame = null;
//            frameIndex = -1;
//            frameCount = adsPushDataList.Count;
//            while (true && isLoopOpening)
//            {
//                if (isNext == false)
//                {
//                    if (currShowFrame == null)
//                        yield break;
//                    isNext = currShowFrame.IsDestroy;
//                    yield return null;
//                }
//                else
//                {
//                    frameIndex++;
//                    if (frameIndex >= frameCount)
//                        yield break;
//                    currShowFrame = ClientSystemManager.GetInstance().OpenFrame<AdsPushFrame>(FrameLayer.Middle) as AdsPushFrame;
//                    currShowFrame.SetAdsDataForThis(adsPushDataList[frameIndex]);
//                    isNext = currShowFrame.IsDestroy;
//                    yield return null;
//                }
//            }
//        }

//        /*
//        IEnumerator WaitAdsFrameAllClose(AdsPushFrame adsFrame)
//        {
//            WaitAdsPushFrameClose wpfc = new WaitAdsPushFrameClose(adsFrame);
//            yield return wpfc;
//        }
//         * */
//    }

//    public class AdsPushServerDataManager : DataManager<AdsPushServerDataManager>
//    {
//        public List<AdsPushData> adsPushDataList;
//        public event System.Action ServerSyncAdsPushListener;
//        public uint ServerStartTime;

//        private List<AdsPushData> noUrlDataList;
//        private List<AdsPushData> withUrlDataList;
//        public List<LoginPushData> LoginPushData;
//        /// <summary>
//        /// 外部调用 - 无链接url
//        /// </summary>
//        /// <returns></returns>
//        public List<AdsPushData> GetNoUrlDataList()
//        {
//            return noUrlDataList;
//        }

//        public string GetLimitTimeActivityLoadingPath()
//        {
//            if (noUrlDataList != null)
//            {
//                if (noUrlDataList.Count > 0)
//                {
//                    return noUrlDataList[0].AdsPushRawData.LoadingPrefabUrl;
//                }
//            }
//            return "";
//        }

//        private uint lastLoginTime;                                       //用户上一次登录游戏的时间（本地数据） - 进入主城时间 时间戳 unix
//        public const string TEST_LAST_LOGIN_TIME = "TestLastLoginTime";
//        public override void Initialize()
//        {
//            //adsPushDataList = new List<AdsPushData>();
//            AdsPushFrameManager.instance.Init();
//            //NetProcess.AddMsgHandler(WorldNotifyGameStartTime.MsgID, OnRecStartServerTime);
//        }
//        public override void Clear()
//        {
//            //adsPushDataList = null;
//            AdsPushFrameManager.instance.UnInit();
//            //NetProcess.RemoveMsgHandler(WorldNotifyGameStartTime.MsgID, OnRecStartServerTime);
//        }

//        public void SetAdsPushTableData()
//        {
//            noUrlDataList = new List<AdsPushData>();
//            withUrlDataList = new List<AdsPushData>();
//            var adsPushTable = TableManager.GetInstance().GetTable<PushExhibitionTable>();
//            if (adsPushTable != null)
//            {
//                var enumerator = adsPushTable.GetEnumerator();
//                while (enumerator.MoveNext())
//                {
//                    PushExhibitionTable pushTable = enumerator.Current.Value as PushExhibitionTable;
//                    if (pushTable != null)
//                    {
//                        var adsRawData = new AdsPushRawData();
//                        adsRawData.DataId = (uint)pushTable.ID;
//                        adsRawData.AdsImgPath = pushTable.IconPath;
//                        adsRawData.AdsContentUrl = pushTable.LinkInfo;
//                        adsRawData.LoadingPrefabUrl = string.Compare(pushTable.LoadingIcon, "-") == 0 ? "" : pushTable.LoadingIcon;

//                        var linkType = GetCurrAdsDataFrameType(pushTable.LinkInfo);
//                        var adsPushData = new AdsPushData();
//                        adsPushData.AdsPushRawData = adsRawData;
//                        adsPushData.Type = string.Compare(adsRawData.AdsContentUrl, "-") == 0 ? AdsPushType.Local_NoUrl : AdsPushType.Local_WithUrl;//string.IsNullOrEmpty(adsRawData.AdsContentUrl)
//                        adsPushData.Priority = adsPushData.Type == AdsPushType.Local_NoUrl ? AdsPushPriority.First : AdsPushPriority.Second;
//                        //adsPushData.StartTime = (uint)pushTable.StartTime;
//                        //adsPushData.EndTime = (uint)pushTable.EndTime;
//                        // adsPushData.ShowLimitGrade = pushTable.FinishLevel;
//                        // adsPushData.AfterStartTimeDay = pushTable.AfterStartServer;
//                        // adsPushData.AfterStartTimeDays = pushTable.AfterStartServerDays;

//                        //是否在等级限制内  true - 在限制等级内 不显示 跳出
//                        //if (IsAdsPushInLevelLimit(adsPushData.ShowLimitGrade))
//                        //    continue;
//                        //是否在开服时间段内 false - 不在开服时间内 不显示 跳出
//                        //if (IsDuringStartServiceHotTime(adsPushData.AfterStartTimeDay, adsPushData.AfterStartTimeDays) == false)
//                        //{
//                        //    //是否在推送时间内  false - 不在推送时间内 不显示 跳出
//                        //    if (IsAdsDuringNowTime(adsPushData.StartTime, adsPushData.EndTime) == false)
//                        //        continue;
//                        //}

//                        if (noUrlDataList != null && withUrlDataList != null)
//                        {
//                            if (adsPushData.Priority == AdsPushPriority.First)
//                            {
//                                noUrlDataList.Add(adsPushData);
//                            }
//                            else if (adsPushData.Priority == AdsPushPriority.Second)
//                            {
//                                if (CheckFramesOpen(linkType))
//                                {
//                                    withUrlDataList.Add(adsPushData);
//                                }
//                            }
//                        }
//                    }
//                }
//                ResortAdsPushDataList();
//            }
//        }

//        private void ResortAdsPushDataList()
//        {
//            if (noUrlDataList == null || withUrlDataList == null)
//                return;
//            adsPushDataList = new List<AdsPushData>();
//            for (int i = 0; i < noUrlDataList.Count; i++)
//            {
//                adsPushDataList.Add(noUrlDataList[i]);
//            }

//            for (int j = 0; j < withUrlDataList.Count; j++)
//            {
//                adsPushDataList.Add(withUrlDataList[j]);
//            }

//        }

//        #region Check To Show Ads

//        public bool IsPlayerLoginFirstToday()
//        {
//            //无角色，新建角色时 不显示广告
//            if (CanGetRoleInfo() == false)
//                return false;
//            var testOfflineTime = GetCurrRoleOffLineTime();
//            DateTime nowZeroTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
//            DateTime lastLoginDate = AdsPushServerDataManager.GetInstance().TransTimeStampToDate(testOfflineTime);
//            if (DateTime.Compare(lastLoginDate, nowZeroTime) > 0)
//            {
//                return false;
//            }

//            return true;
//        }

//        private bool IsAdsDuringNowTime(uint startTime, uint endTime)
//        {
//            if (startTime <= 0 || endTime <= 0)
//            {
//                //Logger.LogError("[登录推送表生效时间 太小了]");
//                return false;
//            }
//            var start = AdsPushServerDataManager.GetInstance().TransTimeStampToDate(startTime);
//            var end = AdsPushServerDataManager.GetInstance().TransTimeStampToDate(endTime);
//            var currDay = DateTime.Now;
//            if (DateTime.Compare(start, currDay) <= 0 && DateTime.Compare(end, currDay) >= 0)
//            {
//                return true;
//            }
//            return false;
//        }

//        private bool IsAdsPushInLevelLimit(int adsLimitLevel)
//        {
//            //无角色，新建角色时 不显示广告 角色等级被限制
//            if (CanGetRoleInfo() == false)
//                return true;
//            var testLevel = ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin().level;
//            if (testLevel >= adsLimitLevel)
//                return false;
//            return true;
//        }

//        //TODO 是否是在开服后的时间段内
//        private bool IsDuringStartServiceHotTime(int startDayIndex, int durationDayCount)
//        {
//            if (startDayIndex < 0 || durationDayCount < 0)
//            {
//                //Logger.LogError("[登录推送表开服时间 开始第几天和持续时间 太小了]");
//                return false;
//            }
//            if (ServerStartTime <= 0)
//            {
//                //Logger.LogError("[开服时间<=0]");
//                return false;
//            }
//            uint startServerAfterTime = (uint)(ServerStartTime + startDayIndex * 24 * 3600);
//            var startServerAfterTwoDay = TransTimeStampToDate(startServerAfterTime);
//            var startServerAfterDay = DateTime.Parse(startServerAfterTwoDay.ToString("MM_dd"));
//            var startServerAfterLastTwoDay = TransTimeStampToDate((uint)(startServerAfterTime + (durationDayCount) * 24 * 3600));
//            var startServerAfterLastDay = DateTime.Parse(startServerAfterLastTwoDay.ToString("MM_dd"));
//            //Logger.LogError("开服时间 = " + TransTimeStampToDate(ServerStartTime));
//            //Logger.LogError(string.Format("开服后第{0}天生效", startDayIndex) + "，日期是 = " + startServerAfterDay);
//            //Logger.LogError(string.Format("开服后开始生效后持续{0}天", durationDayCount) + "，最后一天日期是 = " + startServerAfterLastDay);
//            var currDay = DateTime.Now;
//            if (currDay > startServerAfterDay && currDay < startServerAfterLastDay)
//            {
//                return true;
//            }
//            return false;
//        }

//        bool CanGetRoleInfo()
//        {
//            var roleInfos = ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin();
//            if (roleInfos != null)
//            {
//                if (roleInfos.roleId != 0)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        uint GetCurrRoleOffLineTime()
//        {
//            if (CanGetRoleInfo())
//            {
//                return ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin().offlineTime;
//            }
//            return 0;
//        }

//        #region Receive Server Start Time
//        void OnRecStartServerTime(MsgDATA msg)
//        {
//            //WorldNotifyGameStartTime startServerTime = new WorldNotifyGameStartTime();
//            //startServerTime.decode(msg.bytes);
//            //ServerStartTime = startServerTime.time;
//            //Logger.LogError("当前服，开服时间 = " + TransTimeStampToDate(ServerStartTime));
//        }

//        public void RecStartServerTime(uint startServerTime)
//        {
//            ServerStartTime = startServerTime;
//            Logger.LogProcessFormat("当前服，开服时间 = " + TransTimeStampToDate(ServerStartTime));
//        }
//        #endregion

//        #endregion

//        #region CheckFrameOpen

//        private Type GetCurrAdsDataFrameType(string framelink)
//        {
//            if (string.IsNullOrEmpty(framelink))
//                return null;
//            var regex = new System.Text.RegularExpressions.Regex(@"<type=framename value=(.+)>");
//            bool isMatch = regex.IsMatch(framelink);
//            if (isMatch == false)
//                return null;
//            var match = regex.Match(framelink);
//            if (!string.IsNullOrEmpty(match.Groups[0].Value))
//            {
//                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
//                Type type = assembly.GetType(match.Groups[1].Value);
//                return type;
//            }
//            return null;
//        }

//        private bool CheckFramesOpen(Type frameType)
//        {
//            if (frameType == null)
//                return false;
//            if (frameType == typeof(ActivityJarFrame))
//            {
//                bool hasJar = JarDataManager.GetInstance().HasActivityJar() && Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.ActivityJar);
//                if (hasJar)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        #endregion

//        #region DateTimeHelper

//        public DateTime TransTimeStampToDate(UInt32 timeStamp)
//        {
//            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
//            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
//            return dt;
//        }

//        public string TransTimeStampToStr(UInt32 timeStamp)
//        {
//            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
//            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
//            // return string.Format("{0}年{1}月{2}日 {3:HH:mm}", dt.Year, dt.Month, dt.Day, dt);
//            return string.Format("{0}月{1}日{2:HH:mm}", dt.Month, dt.Day, dt);
//        }

//        public uint TransNowDateToStamp()
//        {
//            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
//            uint timeStamp = (uint)(DateTime.Now - startTime).TotalSeconds;
//            return timeStamp;
//        }

//        public uint TransTodayZeroDateToStamp()
//        {
//            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
//            System.DateTime todayZeroTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
//            uint timeStamp = (uint)(todayZeroTime - startTime).TotalSeconds;
//            return timeStamp;
//        }
//        public void SaveLastLogoutTimeLocalTest(string roleId)
//        {
//            //PlayerPrefs.SetInt(TEST_LAST_LOGIN_TIME, (int)TransNowDateToStamp());
//            PlayerPrefs.SetInt(TEST_LAST_LOGIN_TIME + roleId, (int)TransNowDateToStamp());
//            PlayerPrefs.Save();
//        }

//        public uint GetLastLoginTimeLocalTest(string roleId)
//        {
//            if (PlayerPrefs.HasKey(TEST_LAST_LOGIN_TIME + roleId))
//                return (uint)PlayerPrefs.GetInt(TEST_LAST_LOGIN_TIME + roleId);
//            return TransTodayZeroDateToStamp();
//        }

//        #endregion
//    }

//    public enum AdsPushType
//    {
//        None = 0,
//        Local_NoUrl = 1,       //本地图片路径 + 无前往链接
//        Local_WithUrl = 2,       //本地图片路径 + 带前往链接
//        Online_Url = 3        //远端链接
//    }
//    public enum AdsPushPriority
//    {
//        First = 0,
//        Second,
//        Third
//    }
//    public class AdsPushRawData
//    {
//        public uint DataId { get; set; }                         //内容id（读推送表）
//        public string AdsImgPath { get; set; }                   //显示图片路径
//        public string AdsContentIndex { get; set; }              //前往链接索引id（读链接表）
//        public string AdsContentUrl { get; set; }                //前往链接地址
//        public string LoadingPrefabUrl { get; set; }             //登录loading图片
//    }

//    public class LoginPushData
//    {
//        public string loadingIconPath { get; set; }

//    }

//    public class AdsPushData
//    {
//        public AdsPushRawData AdsPushRawData { get; set; }
//        public AdsPushType Type { get; set; }
//        public AdsPushPriority Priority { get; set; }
//        public uint StartTime { get; set; }                       //当前推送广告开始时间
//        public uint EndTime { get; set; }
//        // public bool HasShowedDaliy { get; set; }
//        public int ShowLimitGrade { get; set; }
//        public int AfterStartTimeDay { get; set; }
//        public int AfterStartTimeDays { get; set; }
//    }

//    public class WaitAdsPushFrameClose : BaseCustomEnum<bool>, IEnumerator
//    {
//        protected AdsPushFrame adsFrame = null;
//        public WaitAdsPushFrameClose(AdsPushFrame adsFrame)
//        {
//            this.adsFrame = adsFrame;
//        }

//        public bool MoveNext()
//        {
//            return adsFrame.IsDestroy == false;
//        }
//        public void Reset()
//        { }
//        public object Current
//        {
//            get { return null; }
//        }
//    }

//    public class WaitHttpDownloadImage : BaseCustomEnum<HTTPAsyncRequest.eState>, IEnumerator
//    {
//        protected string url = null;
//        protected HTTPAsyncRequest req = null;

//        public WaitHttpDownloadImage(string url)
//        {
//            this.url = url;//VoiceDataHelper.ReceiveHttpUrl+key;
//            this.req = new HTTPAsyncRequest();
//            this.req.SendHttpRequst(url, 5000);
//            Logger.LogProcessFormat("[WaitHttpPublishContent] 开始 url {0}, {1}", url, req.GetState());
//        }

//        public string GetResultString()
//        {
//            Logger.LogProcessFormat("[WaitHttpPublishContent] String 获取值 url {0}, {1}", url, req.GetState());
//            if (null != req && req.GetState() == HTTPAsyncRequest.eState.Success)
//            {
//                return req.GetResString();
//            }
//            return null;
//        }

//        public byte[] GetResultByteArray()
//        {
//            Logger.LogProcessFormat("[WaitHttpPublishContent] ByteArray 获取值 url {0}, {1}", url, req.GetState());
//            if (null != req && req.GetState() == HTTPAsyncRequest.eState.Success)
//            {
//                //return VoiceDataHelper.Base64DecodeToBytes(req.GetResString());
//                return System.Text.Encoding.UTF8.GetBytes(req.GetResString());
//            }
//            return null;
//        }

//        public bool MoveNext()
//        {
//            mResult = req.GetState();
//            if (mResult == HTTPAsyncRequest.eState.Error)
//            {
//                Logger.DisplayLog("[下载失败]");
//            }
//            return null != req && req.GetState() == HTTPAsyncRequest.eState.Wait;
//        }

//        public void Reset()
//        {
//        }

//        public object Current
//        {
//            get { return null; }
//        }
//    }
//}
