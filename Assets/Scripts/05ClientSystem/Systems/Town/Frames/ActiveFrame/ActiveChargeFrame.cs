using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
///////删除linq
using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Collections;
using System.Reflection;

namespace GameClient
{
    public class ActiveSpecialFrameInfo
    {
        public int id;
        public System.Type type;
    }

    public class ValueObject
    {
        public string kOrgValue;
        public string kDefault;
        public string kKey;
    }


    public class ActiveChargeFrame : ClientFrame
    {
        public static ActiveChargeFrame activeChargeFrame = null;
        public const int monthlySignInActiveID = 3000;
        public const int warriorRecruitActiveID = 8800;
        public const int fatigueGetBackID = 8100;
        public const int rewardGetBackID = 8200;
        public const int levelActiveActiveID = 4000;
        public const int onlineActiveActiveID = 5000;
        public const int monthlyCardActiveID = 6000;
        public const int onlineActiveCumulativeDaysID = 5100;

        public static void OpenLinkFrame(string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                return;
            }

            //为什么这样配置？而不是9380|8600这样配置参数。
            //避免其他的表中存在\n,\r这样格式的配置
            var tokens = value.Split(new char[] { '\r', '\n' });
            if (tokens.Length != 2)
            {
                tokens = value.Split('|');
            }

            if (tokens.Length != 2)
            {
                return;
            }

            int iConfigId = 0;
            int iTabId = 0;
            if(!int.TryParse(tokens[0],out iConfigId) ||
                !int.TryParse(tokens[1],out iTabId))
            {
                return;
            }

            ActiveManager.GetInstance().OpenActiveFrame(iConfigId,iTabId);
        }

        ActiveManager.ActiveFrameConfig m_kData;
        public override string GetPrefabPath()
        {
            var config = ActiveManager.GetInstance().PopAcitveFrameConfig();
            if(config != null)
            {
                return config.prefabpath;
            }
            return "";
        }

        #region childSpecialActive
        static ActiveSpecialFrameInfo[] ms_activeSpecialFrameDic = new ActiveSpecialFrameInfo[]
        {
            new ActiveSpecialFrameInfo { id=6000, type = typeof(MonthCardActive) }
        };

        List<ActiveSpecialFrame> m_akChildActiveFrames = new List<ActiveSpecialFrame>();

        ActiveSpecialFrame _GetSpecialFrame(int iActiveID)
        {
            var find = m_akChildActiveFrames.Find(x =>
            {
                return x.ActiveID == iActiveID;
            });
            return find;
        }
        ActiveSpecialFrame _TryAddSpecialFrame(int iActiveID)
        {
            ActiveSpecialFrameInfo findAssembly = null;
            for(int i = 0; i < ms_activeSpecialFrameDic.Length; ++i)
            {
                if(ms_activeSpecialFrameDic[i] != null && ms_activeSpecialFrameDic[i].id == iActiveID)
                {
                    findAssembly = ms_activeSpecialFrameDic[i];
                    break;
                }
            }

            if(findAssembly == null)
            {
                return null;
            }

            var find = m_akChildActiveFrames.Find(x =>
            {
                return x.ActiveID == iActiveID;
            });

            if(find != null)
            {
                return find;
            }

            var activeMain = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveMainTable>(iActiveID);
            if (activeMain == null)
            {
                return null;
            }

            if (!ActiveManager.GetInstance().allActivities.ContainsKey(iActiveID))
            {
                return null;
            }

            var activeData = ActiveManager.GetInstance().GetActiveData(iActiveID);
            if (activeData == null)
            {
                return null;
            }

            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            find = assembly.CreateInstance(findAssembly.type.FullName) as ActiveSpecialFrame;
            if(find == null)
            {
                return null;
            }

            find.Intialize(this,frame, iActiveID);
            find.data = activeData;
            find.activityInfo = activeData.mainInfo;
            m_akChildActiveFrames.Add(find);

            return find;
        }
        void _TryCloseSpecialFrame(int iActiveID)
        {
            var find = m_akChildActiveFrames.Find(x =>
            {
                return x.data.iActiveID == iActiveID;
            });

            if(find != null)
            {
                find.OnDestroy();
                find.UnInitialize();
                m_akChildActiveFrames.Remove(find);
            }
        }
        void _CloseChildActive()
        {
            for(int i = 0; i < m_akChildActiveFrames.Count; ++i)
            {
                if(m_akChildActiveFrames[i] != null)
                {
                    m_akChildActiveFrames[i].OnDestroy();
                    m_akChildActiveFrames[i] = null;
                }
            }
            m_akChildActiveFrames.Clear();
        }
        #endregion

        protected override void _OnOpenFrame()
        {
            m_kData = userData as ActiveManager.ActiveFrameConfig;
            m_akActivities.Clear();
            ActiveItemObject.Clear();
            ActiveChargeTabObject.Clear();
            ActiveObject.Clear();
            _InitActiveObjects();
            _InitTabs();

            ActiveManager.GetInstance().onActivityUpdate += OnActivityUpdate;
            ActiveManager.GetInstance().onAddMainActivity += OnAddMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity += OnUpdateMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity += OnRemoveMainActivity;

            var binder = new ActiveManager.VarBinder();
            binder.Analysis();

            activeChargeFrame = this;
			
			BindServerSwitchEvent();
			ActivityDataManager.GetInstance().SendMonthlySignInQuery();
        }    



        public override bool RemoveRefOnClose()
        {
            return true;
        }

        protected override void _OnCloseFrame()
        {
            _CloseChildActive();

            ActiveChargeTabObject.Clear();
            ActiveItemObject.Clear();
            ActiveObject.Clear();
            m_akTabObjects.DestroyAllObjects();
            var itr0 = m_akActivities.GetEnumerator();
            while (itr0.MoveNext())
            {
                var itr1 = (itr0.Current.Value as Dictionary<string, CachedObjectDicManager<int, ActiveItemObject>>).GetEnumerator();
                while (itr1.MoveNext())
                {
                    var dic = itr1.Current.Value as CachedObjectDicManager<int, ActiveItemObject>;
                    dic.DestroyAllObjects();
                }
            }
            m_akActivities.Clear();
            m_akActiveObjects.DestroyAllObjects();

            ActiveManager.GetInstance().onActivityUpdate -= OnActivityUpdate;
            ActiveManager.GetInstance().onAddMainActivity -= OnAddMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity -= OnUpdateMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity -= OnRemoveMainActivity;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.WelfareFrameClose);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.WelfActivityRedPoint);

            activeChargeFrame = null;

            UnBindServerSwitchEvent();
        }

        void OnAddMainActivity(ActiveManager.ActiveData data)
        {
            if(data != null && data.mainItem.ActiveTypeID == m_kData.iConfigID)
            {
                if(!m_akTabObjects.HasObject(data.iActiveID))
                {
                    m_akTabObjects.Create(data.iActiveID, new object[] { m_goTabParent, m_goTabPrefab, data, this });
                }
                else
                {
                    Logger.LogError("OnAddMainActivity add repeated!");
                }
            }
        }

        void OnUpdateMainActivity(ActiveManager.ActiveData data)
        {
            if (data != null && data.mainItem.ActiveTypeID == m_kData.iConfigID)
            {
                if (m_akTabObjects.HasObject(data.iActiveID))
                {
                    m_akTabObjects.RefreshObject(data.iActiveID, new object[] { m_goTabParent, m_goTabPrefab, data, this });

                    var tabObject = m_akTabObjects.GetObject(data.iActiveID);
                    if(tabObject.m_kToggle.isOn && m_akActiveObjects.HasObject(data.iActiveID))
                    {
                        m_akActiveObjects.RefreshObject(data.iActiveID);
                    }
                }
                else
                {
                    Logger.LogError("OnAddMainActivity add repeated!");
                }

                var specialFrame = _GetSpecialFrame(data.iActiveID);
                if(specialFrame != null)
                {
                    specialFrame.data = data;
                    specialFrame.activityInfo = data.mainInfo;
                    specialFrame.OnUpdate();
                }
            }
        }

        void OnRemoveMainActivity(ActiveManager.ActiveData activeData)
        {
            if(m_akActiveObjects.HasObject(activeData.iActiveID))
            {
                var child = Utility.FindChild(m_goActivitiesParent, activeData.mainItem.templateName);
                if(child != null)
                {
                    var activeObject = m_akActiveObjects.GetObject(activeData.iActiveID);
                    if (m_akActivities.ContainsKey(activeData.iActiveID))
                    {
                        m_akActivities.Remove(activeData.iActiveID);
                    }
                    //注意这里只要移除引用就行，因为m_akActivities所含结点是ActiveObject的子结点
                }
                _TryCloseSpecialFrame(activeData.iActiveID);
                m_akActiveObjects.DestroyObject(activeData.iActiveID);
            }

            if (m_akTabObjects.HasObject(activeData.iActiveID))
            {
                var tabObject = m_akTabObjects.GetObject(activeData.iActiveID);
                if(tabObject != null && tabObject == ActiveChargeTabObject.Selected)
                {
                    ActiveChargeTabObject.Clear();
                }
                m_akTabObjects.RecycleObject(activeData.iActiveID);

                _SetDefaultTabs();
            }
        }

        void OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            if (data != null)
            {
                ProtoTable.ActiveTable activeItem = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveTable>(data.ID);
                if (activeItem != null)
                {
                    ActiveManager.ActiveData activeData = null;
                    if (ActiveManager.GetInstance().ActiveDictionary.TryGetValue(activeItem.TemplateID, out activeData))
                    {
                        if (m_akTabObjects.HasObject(activeData.iActiveID))
                        {
                            m_akTabObjects.RefreshObject(activeData.iActiveID, 
                                new object[] { m_goTabParent, m_goTabPrefab, activeData, this });
                        }

                        if (m_akActiveObjects.HasObject(activeItem.TemplateID))
                        {
                            m_akActiveObjects.RefreshObject(activeItem.TemplateID);
                        }

                        Dictionary<string, CachedObjectDicManager<int, ActiveItemObject>> cachedTemplates = null;
                        if (m_akActivities.TryGetValue(activeData.iActiveID, out cachedTemplates))
                        {
                            var enumerator = cachedTemplates.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                enumerator.Current.Value.RefreshObject(activeItem.ID);

                                _Sort(enumerator.Current.Value);
                            }
                        }
                    }
                }
            }
        }

        void _Sort(CachedObjectDicManager<int, ActiveItemObject> tables)
        {
            var lists = tables.ActiveObjects.Values.ToList();
            lists.Sort(ActiveItemObject.Cmp);
            for(int i = 0; i < lists.Count; ++i)
            {
                lists[i].SetAsLastSibling();
            }
        }

        #region activeTabs

        GameObject m_goTabParent;
        GameObject m_goTabPrefab;
        CachedObjectDicManager<int, ActiveChargeTabObject> m_akTabObjects = new CachedObjectDicManager<int, ActiveChargeTabObject>();
        int _SortActivities(ActiveManager.ActiveData left, ActiveManager.ActiveData right)
        {
            if (left.iActiveSortID != right.iActiveSortID)
            {
                return left.iActiveSortID - right.iActiveSortID;
            }
            return left.iActiveID - right.iActiveID;
        }

        void _InitTabs()
        {
            if(m_kData == null || m_kData.templates.Count <= 0)
            {
                Logger.LogErrorFormat("data error!!!");
                return;
            }
            var tokens = m_kData.templates[0].ActiveFrameTabPath.Split(new char[] { '\r', '\n' });
            if(tokens.Length != 2)
            {
                Logger.LogErrorFormat("data error!!!");
                return;
            }

            m_akTabObjects.Clear();

            m_goTabParent = Utility.FindChild(tokens[0], frame);
            if (m_goTabParent == null)
                m_goTabParent = Utility.FindThatChild(tokens[0], frame);

            m_goTabPrefab = Utility.FindChild(tokens[1], frame);
            if (m_goTabPrefab == null)
            {
                m_goTabPrefab = Utility.FindThatChild("tab", frame);
            }
           
            m_goTabPrefab.CustomActive(false);
            m_goActivitiesParent = Utility.FindChild(frame, "Activities");

            for(int i = 0; i < m_goActivitiesParent.transform.childCount;++i)
            {
                m_goActivitiesParent.transform.GetChild(i).gameObject.CustomActive(false);
            }

            UpdateOpActivity();
            var activities = ActiveManager.GetInstance().ActiveDictionary.Values.ToList();
            DealWithSpecialActivity();
            activities.Sort(_SortActivities);
            for(int i = 0; i < activities.Count; ++i)
            {
                //if(activities[i].mainInfo.state != 0)
                {
                    var find = m_kData.templates.Find(x => { return x.ID == activities[i].iActiveID; });
                    if (find != null && !IsActiveSwitchOff(activities[i].iActiveID))
                    {
                        //特殊处理，活动ID是7800充值大礼活动 用黄色页签
                        if (activities[i].iActiveID == 7800)
                        {
                            var tokenss = m_kData.templates[m_kData.templates.Count - 1].ActiveFrameTabPath.Split(new char[] { '\r', '\n' });
                            if (tokenss.Length != 2)
                            {
                                Logger.LogErrorFormat("data error!!!");
                                return;
                            }

                            m_goTabParent = Utility.FindChild(tokenss[0], frame);
                            if (m_goTabParent == null)
                                m_goTabParent = Utility.FindThatChild(tokenss[0], frame);

                            m_goTabPrefab = Utility.FindChild(tokenss[1], frame);
                            if (m_goTabPrefab == null)
                            {
                                m_goTabPrefab = Utility.FindThatChild("rechargetab", frame);
                            }

                            m_goTabPrefab.CustomActive(false);
                        }
                        else if (activities[i].iActiveID == fatigueGetBackID)//精力找回合并到奖励找回
                        {
                            continue;
                        }

#if APPLE_STORE
                        //IOS屏蔽功能 理财计划入口
                        if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_GIFT))
                        {
                            if (activities[i].iActiveID == FinancialPlanDataManager.GetInstance().ActivityTemplateId)
                            {
                                continue;
                            }
                        }
#endif

                        m_akTabObjects.Create(activities[i].iActiveID, new object[] { m_goTabParent, m_goTabPrefab, activities[i], this });
                    }
                }
            }

            _SetDefaultTabs();
        }

        //这里需要提供超链接
        void _SetDefaultTabs()
        {
            if (ActiveChargeTabObject.Selected == null && m_akTabObjects.ActiveObjects.Count > 0)
            {
                if(m_akTabObjects.HasObject(m_kData.iLinkTemplateID))
                {
                    m_akTabObjects.GetObject(m_kData.iLinkTemplateID).m_kToggle.isOn = true;

                    if(m_kData.iLinkTemplateID == warriorRecruitActiveID)
                    {
                        if (tabScrollRect != null)
                            tabScrollRect.verticalNormalizedPosition = 0;
                    }
                }
                else
                {
                    var values = m_akTabObjects.ActiveObjects.Values.ToList();
                    values[0].m_kToggle.isOn = true;
                }
            }
        }

        GameObject m_goActivitiesParent;

        Dictionary<int, Dictionary<string,CachedObjectDicManager<int, ActiveItemObject>>> m_akActivities = new Dictionary<int, Dictionary<string, CachedObjectDicManager<int, ActiveItemObject>>>();

        IEnumerator _AnsyCreatePrefabs(ActiveManager.ActiveData activeData, Dictionary<string, CachedObjectDicManager<int, ActiveItemObject>> outValue,
            GameObject goCurrent)
        {
            if (activeData.prefabs != null)
            {
                int k = 0;
                var enumerator = activeData.prefabs.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current.Value;

                    CachedObjectDicManager<int, ActiveItemObject> activePrefabObject = null;
                    if (!outValue.TryGetValue(current.key, out activePrefabObject))
                    {
                        activePrefabObject = new CachedObjectDicManager<int, ActiveItemObject>();
                        activePrefabObject.Clear();
                        outValue.Add(current.key, activePrefabObject);
                    }

                    GameObject goParent = Utility.FindChild(goCurrent, current.parent);
                    GameObject goCurrentPrefab = Utility.FindChild(goParent, current.local);
                    goCurrentPrefab.CustomActive(false);

                    activeData.akChildItems.Sort(ActiveItemObject.Cmp);

                    for (int j = 0; j < activeData.akChildItems.Count; ++j)
                    {
                        var childItem = activeData.akChildItems[j];
                        if (childItem.activeItem.PrefabKey == current.key)
                        {
                            activePrefabObject.Create(childItem.ID, new object[] { goParent, goCurrentPrefab, childItem, activeData, current.key,this });
                            ++k;
                            yield return Yielders.EndOfFrame;
                            //if(k % 5 == 0)
                            //{
                            //    yield return Yielders.EndOfFrame;
                            //}
                        }
                    }
                }
            }
            yield return Yielders.EndOfFrame;
        }

        public void SetTarget(ActiveManager.ActiveData activeData)
        {
            try
            {
                if(activeData.iActiveID== 8700)//每日礼包埋点
                {
                    Utility.DoStartFrameOperation("ActiveFuliFrame", "DailyPackageToggle");
                }else if(activeData.iActiveID== 8600)//理财计划添加埋点
                {
                    Utility.DoStartFrameOperation("ActiveFuliFrame", "ManagingMoneyToggle");
                }else if(activeData.iActiveID>=7100&& activeData.iActiveID<=7700)//7日狂欢的埋点
                {
                    Utility.DoStartFrameOperation("ActiveFuliFrame", string.Format("7Days{0}", activeData.iActiveID));
                }
                GameObject goLocal = null;
                GameObject goPrefab = null;
                if(!m_akActiveObjects.HasObject(activeData.iActiveID))
                {
                    if(activeData.mainItem.bUseTemplate == 1)
                    {
                        goLocal = Utility.FindChild(frame, activeData.mainItem.templateName);

                        if (goLocal != null)
                        {
                            UIPrefabWrapper uiPrefabWrapper = goLocal.GetComponent<UIPrefabWrapper>();
                            if(uiPrefabWrapper != null)
                            {
                                GameObject prefab = uiPrefabWrapper.LoadUIPrefab();
                                if(prefab != null)
                                {
                                    prefab.transform.SetParent(goLocal.transform.parent,false);
                                    GameObject.Destroy(goLocal);

                                    goLocal = prefab;

                                    //初始化View
                                    var activityChargeBaseView = prefab.GetComponent<ActivityChargeBaseView>();
                                    if(activityChargeBaseView != null)
                                        activityChargeBaseView.InitView(uiPrefabWrapper.IntParam);
                                }
                            }
                        }
                    }
                    else
                    {
                        goPrefab = Utility.FindChild(frame, activeData.mainItem.templateName);
                        goPrefab.CustomActive(false);
                    }

                    m_akActiveObjects.Create(activeData.iActiveID, new object[] { goLocal, goPrefab, activeData });

                    var specialframe = _TryAddSpecialFrame(activeData.iActiveID);
                    if (specialframe != null)
                    {
                        specialframe.OnCreate();
                    }

                    // 新版月签到活动不走原来的活动流程，但是活动id依然保留
                    // 不再使用原来的流程创建ui对象
                    if(activeData.iActiveID != monthlySignInActiveID && activeData.iActiveID != onlineActiveActiveID && onlineActiveCumulativeDaysID != activeData.iActiveID
                        && activeData.iActiveID != fatigueGetBackID && rewardGetBackID != activeData.iActiveID && monthlyCardActiveID != activeData.iActiveID
                        && levelActiveActiveID != activeData.iActiveID)
                    {
                    GameObject goCurrent = m_akActiveObjects.GetObject(activeData.iActiveID).gameObject;
                    if (goCurrent != null)
                    {
                        string templateRealName = "";
                        var tokenedName = activeData.mainItem.templateName.Split('/');
                        if(tokenedName.Length > 0)
                        {
                            templateRealName = tokenedName[tokenedName.Length - 1];
                        }
                        goCurrent.name = templateRealName + activeData.iActiveID;
                        Dictionary<string, CachedObjectDicManager<int, ActiveItemObject>> outValue = null;

                        if (!m_akActivities.TryGetValue(activeData.iActiveID, out outValue))
                        {
                            outValue = new Dictionary<string, CachedObjectDicManager<int, ActiveItemObject>>();
                            m_akActivities.Add(activeData.iActiveID, outValue);

                            StartCoroutine(_AnsyCreatePrefabs(activeData,outValue, goCurrent));
                            }
                        }
                    }
                }

                if (m_akActiveObjects.HasObject(activeData.iActiveID))
                {
                    m_akActiveObjects.Filter(new object[] { activeData.iActiveID });
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
        #endregion
        #region activeItemObjects
        enum PathKeyType
        {
            PKT_GLOBAL = 0,//从根结点查找
            PKT_LOCAL,//以ActiveItemObject goLocal为父结点查找
        }
        
        #endregion

        #region activeObjects
        CachedObjectDicManager<int, ActiveObject> m_akActiveObjects = new CachedObjectDicManager<int, ActiveObject>();

        void _InitActiveObjects()
        {
            m_akActiveObjects.Clear();
        }
        #endregion

        private void DealWithSpecialActivity()
        {
            //理财副本的标签位置变化
            var allActivities = ActiveManager.GetInstance().ActiveDictionary.Values.ToList();
            foreach (var curActivity in allActivities)
            {
                if (curActivity.iActiveID == FinancialPlanDataManager.GetInstance().ActivityTemplateId)
                {
                    if (FinancialPlanDataManager.GetInstance().IsAlreadyReceivedAllReward())
                    {
                        curActivity.iActiveSortID = 100;
                    }
                }//勇士招募的标签变化
                else if (curActivity.iActiveID == warriorRecruitActiveID)
                {
                    if (WarriorRecruitDataManager.GetInstance().CheckWarriorRecruitActiveIsOpen())
                    {
                        curActivity.iActiveSortID = 101;
                    }
                }
            }
        }

        private void UpdateOpActivity()
        {
            //添加或者删除运营活动ID，对应的信息，如：新人周签到或者活动周签到

            if (WeekSignInUtility.IsWeekSignInOpenByWeekSignInType(WeekSignInType.NewPlayerWeekSignIn) == true)
            {
                ActiveManager.GetInstance().AddOneActiveData((int)WeekSignInDataManager.NewPlayerWeekSignInOpActTypeId);
            }
            else
            {
                ActiveManager.GetInstance()
                    .RemoveOneActiveData((int) WeekSignInDataManager.NewPlayerWeekSignInOpActTypeId);
            }

            if (WeekSignInUtility.IsWeekSignInOpenByWeekSignInType(WeekSignInType.ActivityWeekSignIn) == true)
            {
                ActiveManager.GetInstance().AddOneActiveData((int) WeekSignInDataManager.ActivityWeekSignInOpActTypeId);
            }
            else
            {
                ActiveManager.GetInstance()
                    .RemoveOneActiveData((int) WeekSignInDataManager.ActivityWeekSignInOpActTypeId);
            }

            ActiveManager.GetInstance().AddOneActiveData(monthlySignInActiveID);
            
            if (WarriorRecruitDataManager.GetInstance().CheckWarriorRecruitActiveIsOpen())
            {
                ActiveManager.GetInstance().AddOneActiveData(warriorRecruitActiveID);
            }
        }


        [UIEventHandle("BG/close")]
        void OnFunction()
        {
            frameMgr.CloseFrame(this);
        }

        [UIControl("tabsList",typeof(ScrollRect))]
        ScrollRect tabScrollRect;

        public static void CloseMe()
        {
            if(activeChargeFrame != null)
            {
                ClientSystemManager.GetInstance().CloseFrame(activeChargeFrame.GetFrameName());
                activeChargeFrame = null;
            }
        }

        #region ActiveFuliTab Server Switch 

        bool IsActiveSwitchOff(int activeId)
        {
            bool bOff = false;
            if (activeId == DailyChargeRaffleDataManager.ACTIVITY_TEMPLATE_ID)
            {
                bOff = ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(Protocol.ServiceType.SERVEICE_DAY_CHARGE_WELFARE);
                return bOff;
            }
            return bOff;
        }

        void BindServerSwitchEvent()
        {
            ServerSceneFuncSwitchManager.GetInstance().AddServerFuncSwitchListener(Protocol.ServiceType.SERVEICE_DAY_CHARGE_WELFARE, _OnActiveSwitch);
        }
        void UnBindServerSwitchEvent()
        {
            ServerSceneFuncSwitchManager.GetInstance().RemoveServerFuncSwitchListener(Protocol.ServiceType.SERVEICE_DAY_CHARGE_WELFARE, _OnActiveSwitch);
        }
        void _OnActiveSwitch(ServerSceneFuncSwitch ssfs)
        {
            int dailyChargeActiveId = DailyChargeRaffleDataManager.ACTIVITY_TEMPLATE_ID;
            if (ssfs.sType == Protocol.ServiceType.SERVEICE_DAY_CHARGE_WELFARE)
            {
                if (!ssfs.sIsOpen)
                {
                    OnRemoveActiveTab(dailyChargeActiveId);
                }
            }
        }

        void OnRemoveActiveTab(int activeId)
        {
            if (m_akTabObjects == null)
            {
                return;
            }
            if (m_akTabObjects.HasObject(activeId))
            {
                var tabObject = m_akTabObjects.GetObject(activeId);
                if (tabObject != null && tabObject == ActiveChargeTabObject.Selected)
                {
                    ActiveChargeTabObject.Clear();
                }
                m_akTabObjects.RecycleObject(activeId);

                _SetDefaultTabs();
            }
        }

        #endregion
    }
}