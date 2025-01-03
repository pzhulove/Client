using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using UnityEngine.UI;
using System.ComponentModel;
using System;
using Network;
///////删除linq
using ProtoTable;
using Protocol;

namespace GameClient
{
    public class OPPOPrivilegeFrame :ClientFrame
    {

        public enum OPPOTABType
        {
            [System.ComponentModel.DescriptionAttribute("OPPO_PRIVILRGR")]
            OTT_PRIVILRGR = 0,
            [System.ComponentModel.DescriptionAttribute("OPPO_LUCKYGUY")]
            OTT_LUCKYGUY = 1,
            [System.ComponentModel.DescriptionAttribute("OPPO_AMBERPRIVILEGE")]
            OTT_AMBERPRIVILEGE = 2,
            [System.ComponentModel.DescriptionAttribute("OPPO_AMBERGIFTBAG")]
            OTT_AMBERGIFTBAG = 3,
            [System.ComponentModel.DescriptionAttribute("OPPO_DAILYCHECK")]
            OTT_DAILYCHECK =4,
            [System.ComponentModel.DescriptionAttribute("OPPO_GROWTHHAOLI")]
            OTT_OPPOGROWTHHAOLI = 5,
            OTT_COUNT,
        }
        OPPOTABType m_eFunctionType = OPPOTABType.OTT_DAILYCHECK;

        int[] activityIds = new int[] 
        {
            12000,//特权
            17000,//转盘
            27000,//琥珀特权
            20000,//琥珀礼包
            15000,//每日签到
            26000,//成长礼包
        };

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/OPPOPrivilegeFrame";
        }
        protected sealed override void _OnOpenFrame()
        {
            _InitOPPOTABTabs();
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;//让活动刷新的回调
            GameStatisticManager.GetInstance().DoStartOPPO(StartOPPOType.OPPOOPEN);
        }
        protected sealed override void _OnCloseFrame()
        {
            for (int i = 0; i < m_kFunctionObject.Length; i++)
            {
                m_kFunctionObject[i].Clear();
            }
            Array.Clear(m_acPrefabInits, 0, m_acPrefabInits.Length);
            m_akOPPOTABTabs.DestroyAllObjects();

            _RemoveAllButtonDelegates();
          
            ClearData();
            myDailSevenActivityList.Clear();
            myprivilegeActivityList.Clear();
            myLuckyGuyTaskPairList.Clear();
            itemdata = null;
            iCost = 0;
            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;//让活动刷新的回调
            mAmberPrivilegeView = null;
            mOPPOGrowthHaoLiView = null;
        }
        protected sealed override void _OnUpdate(float timeElapsed)
        {
            fTime += timeElapsed;

            if (fTime > 1)
            {
                IsUpdate = false;
                RefreshPriviPickUpBtn();
            }
        }
        public sealed override bool IsNeedUpdate()
        {
            return IsUpdate;
        }

        bool _CheckActivityIsOpen(int index)
        {
            if (index < 0 || index >= activityIds.Length)
            {
                return false;
            }

            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(activityIds[index]);
            if (activeData == null)
            {
                return false;
            }

            if (activeData.mainInfo.state != (byte)StateType.Running)
            {
                return false;
            }

            return true;
        }

        void _InitOPPOTABTabs()
        {
            m_eFunctionType = OPPOTABType.OTT_PRIVILRGR;
            GameObject goParent = Utility.FindChild(frame, "TabScrollView/ViewPort/Content");
            GameObject goPrefab = Utility.FindChild(goParent, "Filter");
            goPrefab.CustomActive(false);

            var OnFunctionLoad = Delegate.CreateDelegate(typeof(OPPOTABTab.OnFunctionLoad), this, "_OnFunctionLoad");
            for (int i = 0; i < (int)OPPOTABType.OTT_COUNT; ++i)
            {
                if (!_CheckActivityIsOpen(i))
                {
                    continue; 
                }

                m_akOPPOTABTabs.Create((OPPOTABType)i, new object[] { goParent, goPrefab, i, this, OnFunctionLoad });
                m_kFunctionObject[i].Clear();

                m_aInits[i] = false;
            }

            m_akOPPOTABTabs.Filter(null);

            m_akOPPOTABTabs.GetObject(m_eFunctionType).toggle.isOn = true;

        }

        const int m_iConfigPrefabCount = 6;
        string[] m_Prefabs = new string[m_iConfigPrefabCount]
        {
            "UIFlatten/Prefabs/Activity/Privilege", //0
            "UIFlatten/Prefabs/Activity/LuckyTurnTable", //1
            "UIFlatten/Prefabs/Activity/AmbergiftBag",//2
            "UIFlatten/Prefabs/Activity/DailyCheck", //3
            "UIFlatten/Prefabs/Activity/AmberPrivilegeView", //4
            "UIFlatten/Prefabs/Activity/OPPOGrowthHaoLiView", //5
        };

        char[] m_acPrefabInits = new char[m_iConfigPrefabCount];

        List<GameObject>[] m_kFunctionObject = new List<GameObject>[(int)OPPOTABType.OTT_COUNT]
        {
            new List<GameObject>(),
            new List<GameObject>(),
            new List<GameObject>(),
            new List<GameObject>(),
            new List<GameObject>(),
            new List<GameObject>(),
        };

        bool[] m_aInits = new bool[(int)OPPOTABType.OTT_COUNT];

        [UIObject("ChildFrame")]
        GameObject goChildFrame;

        void _OnFunctionLoad(OPPOTABType eOPPOTABType)
        {
            switch (eOPPOTABType)
            {

                case OPPOTABType.OTT_PRIVILRGR:
                    {
                        if (0 == m_acPrefabInits[0])
                        {
                            GameObject goPrivilege = AssetLoader.instance.LoadRes(m_Prefabs[0], typeof(GameObject)).obj as GameObject;
                            goPrivilege.name = "Privilege";
                            Utility.AttachTo(goPrivilege, goChildFrame);

                            m_kFunctionObject[(int)OPPOTABType.OTT_PRIVILRGR].Add(goPrivilege);
                            m_acPrefabInits[0] = (char)1;
                        }
                    }

                    break;
                case OPPOTABType.OTT_LUCKYGUY:
                    {
                        if (0 == m_acPrefabInits[1])
                        {
                            GameObject goLuckyTurnTable = AssetLoader.instance.LoadRes(m_Prefabs[1], typeof(GameObject)).obj as GameObject;
                            goLuckyTurnTable.name = "LuckyTurnTable";
                            Utility.AttachTo(goLuckyTurnTable, goChildFrame);

                            m_kFunctionObject[(int)OPPOTABType.OTT_LUCKYGUY].Add(goLuckyTurnTable);
                            m_acPrefabInits[1] = (char)1;
                        }
                    }
                    break;
                case OPPOTABType.OTT_AMBERGIFTBAG:
                    {
                        if (0==m_acPrefabInits[2])
                        {
                            GameObject goAmbergiftBag = AssetLoader.instance.LoadRes(m_Prefabs[2], typeof(GameObject)).obj as GameObject;
                            goAmbergiftBag.name = "AmbergiftBag";
                            Utility.AttachTo(goAmbergiftBag, goChildFrame);

                            m_kFunctionObject[(int)OPPOTABType.OTT_AMBERGIFTBAG].Add(goAmbergiftBag);
                            m_acPrefabInits[2] = (char)1;
                        }
                    }
                    break;
                case OPPOTABType.OTT_DAILYCHECK:
                    {
                        if (0 == m_acPrefabInits[3])
                        {
                           GameObject goDaily = AssetLoader.instance.LoadRes(m_Prefabs[3], typeof(GameObject)).obj as GameObject;
                            goDaily.name = "DailyCheck";
                            Utility.AttachTo(goDaily, goChildFrame);

                            m_kFunctionObject[(int)OPPOTABType.OTT_DAILYCHECK].Add(goDaily);
                            m_acPrefabInits[3] = (char)1;
                        }

                    }
                    break;
                case OPPOTABType.OTT_AMBERPRIVILEGE:
                    {
                        if (0 == m_acPrefabInits[4])
                        {
                            GameObject goAmberPrivilege = AssetLoader.instance.LoadRes(m_Prefabs[4], typeof(GameObject)).obj as GameObject;
                            goAmberPrivilege.name = "AmberPrivilege";
                            Utility.AttachTo(goAmberPrivilege, goChildFrame);
                            m_kFunctionObject[(int)OPPOTABType.OTT_AMBERPRIVILEGE].Add(goAmberPrivilege);
                            m_acPrefabInits[4] = (char)1;
                        }
                    }
                    break;
                case OPPOTABType.OTT_OPPOGROWTHHAOLI:
                    {
                        if (0 == m_acPrefabInits[5])
                        {
                            GameObject goOPPOGrowthHaoLiView = AssetLoader.instance.LoadRes(m_Prefabs[5], typeof(GameObject)).obj as GameObject;
                            if (goOPPOGrowthHaoLiView != null)
                            {
                                goOPPOGrowthHaoLiView.name = "OPPOGrowthHaoLiView";

                                Utility.AttachTo(goOPPOGrowthHaoLiView, goChildFrame);
                                m_kFunctionObject[(int)OPPOTABType.OTT_OPPOGROWTHHAOLI].Add(goOPPOGrowthHaoLiView);
                                m_acPrefabInits[5] = (char)1;
                            }
                        }
                    }
                    break;
            }
        }

        class OPPOTABTab : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            OPPOTABType eOPPOTABType;
            OPPOPrivilegeFrame frame;

            public OPPOTABType OPPOTABType
            {
                get { return eOPPOTABType; }
            }

            public Toggle toggle;
            Text labelMark;
            Text labelCheckMark;
            public delegate void OnFunctionLoad(OPPOTABType eOPPOTABType);
            public OnFunctionLoad onFunctionLoad;
            OPPOFunctionRedBinder functionRedBinder=null;

            public sealed override void OnDestroy()
            {
                goLocal = null;
                goPrefab = null;
                goParent = null;
                frame = null;
                labelMark = null;
                labelCheckMark = null;
                toggle.onValueChanged.RemoveAllListeners();
                toggle = null;
                onFunctionLoad = null;
                functionRedBinder = null;
            }

            public sealed override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                eOPPOTABType = (OPPOTABType)param[2];
                frame = (OPPOPrivilegeFrame)param[3];
                this.onFunctionLoad = param[4] as OnFunctionLoad;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    toggle = goLocal.GetComponent<Toggle>();
                    toggle.onValueChanged.AddListener((bool bValue) =>
                    {
                        if (bValue)
                        {
                            _OnSelected();
                        }
                    });

                    var path = Utility.GetEnumDescription(eOPPOTABType);
                    labelMark = Utility.FindComponent<Text>(goLocal, "Text");
                    labelMark.text = TR.Value(path);
                    labelCheckMark = Utility.FindComponent<Text>(goLocal, "CheckMark/Text");
                    labelCheckMark.text = TR.Value(path);
                    functionRedBinder = goLocal.GetComponent<OPPOFunctionRedBinder>();
                }
                Enable();
                _Update();
            }

            public sealed override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }

            public sealed override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }

            public sealed override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }
            public sealed override void OnRefresh(object[] param)
            {
                _Update();
            }

            public sealed override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }

            void _Update()
            {
                if (eOPPOTABType==OPPOTABType.OTT_PRIVILRGR)
                {
                    functionRedBinder.AddCheckFunction(OPPOFunctionRedBinder.OPPOFunctionType.OFT_PRIVILRGR);
                }
                else if (eOPPOTABType == OPPOTABType.OTT_LUCKYGUY)
                {
                    functionRedBinder.AddCheckFunction(OPPOFunctionRedBinder.OPPOFunctionType.OFT_LUCKYGUY);
                }
                else if (eOPPOTABType == OPPOTABType.OTT_DAILYCHECK)
                {
                    functionRedBinder.AddCheckFunction(OPPOFunctionRedBinder.OPPOFunctionType.OFT_DAILYCHECK);
                }
                else if (eOPPOTABType == OPPOTABType.OTT_AMBERGIFTBAG)
                {
                    functionRedBinder.AddCheckFunction(OPPOFunctionRedBinder.OPPOFunctionType.OFT_AMBERGIFTBAG);
                }
                else if (eOPPOTABType == OPPOTABType.OTT_AMBERPRIVILEGE)
                {
                    functionRedBinder.AddCheckFunction(OPPOFunctionRedBinder.OPPOFunctionType.OFT_AMBERPRIVILEGE);
                }
                else if (eOPPOTABType == OPPOTABType.OTT_OPPOGROWTHHAOLI)
                {
                    functionRedBinder.AddCheckFunction(OPPOFunctionRedBinder.OPPOFunctionType.OFT_OPPOGROWTHHAOLI);
                }
                else
                {
                    functionRedBinder.ClearCheckFunctions();
                }
            }

            void _OnSelected()
            {
                if (null != onFunctionLoad)
                {
                    onFunctionLoad(eOPPOTABType);
                    onFunctionLoad = null;
                }
                frame.OnFunctionChanged(eOPPOTABType);
            }
        }

        CachedObjectDicManager<OPPOTABType, OPPOTABTab> m_akOPPOTABTabs = new CachedObjectDicManager<OPPOTABType, OPPOTABTab>();

        void ClearData()
        {
            DailyItemList.Clear();
            ProgresssList.Clear();
            GaoLiangs.Clear();
            CountersignList.Clear();
            Receives.Clear();
            ItemPosList.Clear();
            SignList.Clear();
            GrayLisy.Clear();
            ItemCountList.Clear();
            myActivityList.Clear();
            CtsSignBtnList.Clear();
        }

        void OnFunctionChanged(OPPOTABType eOPPOTABType)
        {
            m_eFunctionType = eOPPOTABType;
            for (int i = 0; i < m_kFunctionObject.Length; ++i)
            {
                if (m_eFunctionType != (OPPOTABType)i)
                {
                    for (int j = 0; j < m_kFunctionObject[i].Count; j++)
                    {
                        m_kFunctionObject[i][j].CustomActive(false);
                    }
                }
            }

            for (int i = 0; i < m_kFunctionObject[(int)eOPPOTABType].Count; i++)
            {
                m_kFunctionObject[(int)eOPPOTABType][i].CustomActive(true);
            }
            if (!m_aInits[(int)m_eFunctionType])
            {
                if (m_eFunctionType == OPPOTABType.OTT_DAILYCHECK)
                {
                    ClearData();
                    DailyItems = Utility.FindChild(frame, "ChildFrame/DailyCheck/Items");
                    Slider = Utility.FindChild(frame, "ChildFrame/DailyCheck/Buttom/Slider");
                    Box = Utility.FindChild(frame, "ChildFrame/DailyCheck/Buttom/Box");
                    ThisWeekNum = Utility.FindComponent<Text>(frame, "ChildFrame/DailyCheck/ThisWeek/Num");
                    LeakageSignNum = Utility.FindComponent<Text>(frame, "ChildFrame/DailyCheck/LeakageSign/Num");
                    RetroactiveGray = Utility.FindComponent<UIGray>(frame, "ChildFrame/DailyCheck/Button");
                    RetroactiveBtn= Utility.FindComponent<Button>(frame, "ChildFrame/DailyCheck/Button");
                    isDail = true;
                    _FinAllComponents();
                    CreatDailItem();
                    GetDailSevenItemData();
                    UpdateCompomentState();

                    _AddButton("ChildFrame/DailyCheck/Button", OnRapidRetroactiveClick);
                }
                else if (m_eFunctionType == OPPOTABType.OTT_PRIVILRGR)
                {
                    m_comRewardItemList = Utility.FindComponent<ComUIListScript>(frame, "ChildFrame/Privilege/Items");
                    gray = Utility.FindComponent<UIGray>(frame, "ChildFrame/Privilege/OK");
                    OKText = Utility.FindComponent<Text>(frame, "ChildFrame/Privilege/OK/Text");
                   
                    _InitRewardItemList();
                    GetPriviItemData();
                    _AddButton("ChildFrame/Privilege/OK", OnOKButtonClick);
                }
                else if (m_eFunctionType == OPPOTABType.OTT_LUCKYGUY)
                {
                    Count = Utility.FindComponent<Text>(frame, "ChildFrame/LuckyTurnTable/Count");
                    TotalCount = Utility.FindComponent<Text>(frame, "ChildFrame/LuckyTurnTable/TotalCount");
                    items = Utility.FindChild(frame, "ChildFrame/LuckyTurnTable/Items");
                    startGray = Utility.FindComponent<UIGray>(frame, "ChildFrame/LuckyTurnTable/Start");
                    startBtn = Utility.FindComponent<Button>(frame, "ChildFrame/LuckyTurnTable/Start");
                    zhizhen = Utility.FindChild(frame, "ChildFrame/LuckyTurnTable/ZhiZhen");
                    theLuckyRoller = zhizhen.GetComponent<TheLuckyRoller>();
                    zhizhen.transform.eulerAngles = new UnityEngine.Vector3(0, 0, 0);
                    bIsLucky = true;
                    GetLuckyGuyItemData();
                    UpdateState();
                    CreatLuckyItems();
                    _AddButton("ChildFrame/LuckyTurnTable/Start", OnRaffleButtonClick);
                }
                else if (m_eFunctionType==OPPOTABType.OTT_AMBERGIFTBAG)
                {
                    ComCommonBind comBind = Utility.FindComponent<ComCommonBind>(frame, "ChildFrame/AmbergiftBag");
                    if (comBind != null)
                    {
                        okBtnText = comBind.GetCom<Text>("OKBtnText");
                        okBtn = comBind.GetCom<Button>("OKBtn");
                        okBtn.onClick.RemoveAllListeners();
                        okBtn.onClick.AddListener(() =>
                        {
                            ActiveManager.GetInstance().SendSubmitActivity(ActivityID);
                            GameStatisticManager.GetInstance().DoStartOPPO(StartOPPOType.OPPOAMBERGIFT, StrAmberLevel);
                        });
                        AmberGiftBagComUIListScript = comBind.GetCom<ComUIListScript>("AmberComUIListScript");
                        desText = comBind.GetCom<Text>("Des");
                        okBtnGray = comBind.GetCom<UIGray>("OKBtnUIGray");
                    }

                    _InitAmberGiftBagInfo();
                }
                else if (m_eFunctionType == OPPOTABType.OTT_AMBERPRIVILEGE)
                {
                    mAmberPrivilegeView = Utility.FindComponent<AmberPrivilegeView>(frame, "ChildFrame/AmberPrivilege");
                    if (mAmberPrivilegeView != null)
                    {
                        mAmberPrivilegeView.InitView();
                    }
                }
                else if (m_eFunctionType == OPPOTABType.OTT_OPPOGROWTHHAOLI)
                {
                    mOPPOGrowthHaoLiView = Utility.FindComponent<OPPOGrowthHaoLiView>(frame, "ChildFrame/OPPOGrowthHaoLiView");
                    if (mOPPOGrowthHaoLiView != null)
                    {
                        mOPPOGrowthHaoLiView.InitView();
                    }
                }

                m_aInits[(int)m_eFunctionType] = true;
            }

            if (m_eFunctionType == OPPOTABType.OTT_LUCKYGUY)
            {
                bIsLucky = true;
                if (zhizhen != null)
                {
                    zhizhen.transform.eulerAngles = new UnityEngine.Vector3(0, 0, 0);
                }
            }
            else
            {
                bIsLucky = false;
            }
        }


        #region AmberGiftBag
        public enum AmberGiftBagType
        {
            AGBT_NONE=20001,//不是琥珀
            AGBT_GREENPEARL=20002,//绿珀
            AGBT_BLUEPEARL=20003,//蓝珀
            AGBT_GOLDPEARL = 20004,//金珀
            AGBT_REDPEARL =20005,//红珀
            AGBT_PURPLEPEARL=20006,//紫珀
        }

        int IActivitytEmplateID = 20000;
        int ActivityID=0;
        byte StatuIndex = 0;
        string StrAmberLevel = "";
        Text okBtnText;
        Button okBtn;
        ComUIListScript AmberGiftBagComUIListScript;
        Text desText;
        UIGray okBtnGray;

        void _InitAmberGiftBagInfo()
        {
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(IActivitytEmplateID);
            if (activeData != null)
            {
                for (int i = 0; i < activeData.akChildItems.Count; i++)
                {
                    if (activeData.akChildItems[i].status <= (int)TaskStatus.TASK_UNFINISH)
                    {
                        continue;
                    }
                    ActivityID = activeData.akChildItems[i].ID;
                    StatuIndex = activeData.akChildItems[i].status;
                    StrAmberLevel = GetAmberLevel(ActivityID);
                    if (desText != null)
                    {
                        if (ActivityID == (int)AmberGiftBagType.AGBT_NONE)
                        {
                            desText.text = StrAmberLevel;
                        }
                        else
                        {
                            desText.text = TR.Value("oppo_amberlevel_des", StrAmberLevel);
                        }
                    }
                    _InitAmberGiftBagReward(ActivityID);
                    _InitAmberGiftBagState(StatuIndex);
                }
            }
        }

        void _InitAmberGiftBagState(byte byIndex)
        {
            {
                if (byIndex == (byte)TaskStatus.TASK_OVER)
                {
                    if (okBtnGray != null && okBtnText != null)
                    {
                        okBtnGray.enabled = true;
                        okBtnText.text = "已领取";
                    }
                }
                else if (byIndex == (byte)TaskStatus.TASK_FINISHED)
                {
                    if (okBtnGray != null && okBtnText != null)
                    {
                        okBtnGray.enabled = false;
                        okBtnText.text = "领取";
                    }
                }
                else if (byIndex == (byte)TaskStatus.TASK_UNFINISH)
                {
                    if (okBtnGray != null && okBtnText != null)
                    {
                        okBtnGray.enabled = true;
                        okBtnText.text = "领取";
                    }
                }
                else if (byIndex == (byte)TaskStatus.TASK_FAILED)
                {
                    if (okBtnGray != null && okBtnText != null)
                    {
                        okBtnGray.enabled = true;
                        okBtnText.text = "领取";
                    }
                }
            }
        }

        string GetAmberLevel(int index)
        {
            if (index == (int)AmberGiftBagType.AGBT_NONE)
            {
                return TR.Value("oppo_notamber");
            }
            else if (index==(int)AmberGiftBagType.AGBT_GREENPEARL)
            {
                return TR.Value("oppo_greenpearl");
            }
            else if (index==(int)AmberGiftBagType.AGBT_BLUEPEARL)
            {
                return TR.Value("oppo_bluepearl");
            }
            else if (index==(int)AmberGiftBagType.AGBT_GOLDPEARL)
            {
                return TR.Value("oppo_goldpearl");
            }
            else if (index==(int)AmberGiftBagType.AGBT_REDPEARL)
            {
                return TR.Value("oppo_redpearl");
            }
            else if (index == (int)AmberGiftBagType.AGBT_PURPLEPEARL)
            {
                return TR.Value("oppo_purplepearl");
            }
            else
            {
                return TR.Value("oppo_notamber");
            }
        }

        void _InitAmberGiftBagReward(int id)
        {
            List<AwardItemData> items = ActiveManager.GetInstance().GetActiveAwards(id);

            if (items != null && AmberGiftBagComUIListScript != null)
            {
                AmberGiftBagComUIListScript.Initialize();

                AmberGiftBagComUIListScript.onBindItem = var =>
                {
                    return CreateComItem(Utility.FindGameObject(var, "itemPos"));
                };

                AmberGiftBagComUIListScript.onItemVisiable = var =>
                {
                    if (items != null)
                    {
                        if (var.m_index >= 0 && var.m_index < items.Count)
                        {
                            ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(items[var.m_index].ID);
                            itemData.Count = items[var.m_index].Num;

                            ComItem comItem = var.gameObjectBindScript as ComItem;
                            comItem.Setup(itemData, (var1, var2) =>
                            {
                                ItemTipManager.GetInstance().ShowTip(var2);
                            });

                            Utility.GetComponetInChild<Text>(var.gameObject, "Name").text = itemData.GetColorName();
                        }
                    }
                };
                AmberGiftBagComUIListScript.SetElementAmount(items.Count);
            }

        }

        #endregion

        #region  Privilege
        public const int privilegeRwerdID = 12001;
        public const int privilegeID = 12000;
        bool IsUpdate = false;
        float fTime = 0.0f;
        ComUIListScript m_comRewardItemList;
        UIGray gray;
        Text OKText;
        List<ActiveManager.ActivityData> myprivilegeActivityList = new List<ActiveManager.ActivityData>();

        void RefreshPriviPickUpBtn()
        {
            if (!_CheckPrivilrge())
            {
                return;
            }
            IsStartGameFromCenter();
        }
        public bool _CheckPrivilrge()
        {
            int index = 0;
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(privilegeID);

            if (activeData == null)
            {
                return false;
            }
            for (int i = 0; i < activeData.akChildItems.Count; i++)
            {
                if (activeData.akChildItems[i].status == (int)TaskStatus.TASK_FINISHED)
                {
                    index++;
                }
            }
            if (index > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        void IsStartGameFromCenter()
        {
            if (SDKInterface.Instance.IsStartFromGameCenter())
            {
                gray.enabled = false;
                OKText.text = "领取";
            }
            else
            {
                gray.enabled = false;
                OKText.text = "前往游戏中心";
            }
        }
       
        void OnOKButtonClick()
        {
            if (SDKInterface.Instance.IsStartFromGameCenter())
            {
                ActiveManager.GetInstance().SendSubmitActivity(privilegeRwerdID);

                GameStatisticManager.GetInstance().DoStartOPPO(StartOPPOType.OPPOPRIVILEGE);
            }
            else
            {
                IsUpdate = true;
                SDKInterface.Instance.OpenGameCenter();
                GameStatisticManager.GetInstance().DoStartOPPO(StartOPPOType.OPPOJUMPGAMECENTER);
            }
               
        }
        void GetPriviItemData()
        {
            myprivilegeActivityList.Clear();
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(privilegeID);

            if (activeData == null)
            {
                Logger.LogErrorFormat("activeData is null");
                return;
            }

            for (int i = 0; i < activeData.akChildItems.Count; i++)
            {
                myprivilegeActivityList.Add(activeData.akChildItems[i]);
            }
            UpdataPrivileState();
        }
        void UpdataPrivileState()
        {
            for (int i = 0; i < myprivilegeActivityList.Count; i++)
            {
                if (myprivilegeActivityList[i].status == (int)TaskStatus.TASK_OVER)
                {
                    gray.enabled = true;
                    OKText.text = "已领取";
                }
                else if (myprivilegeActivityList[i].status == (int)TaskStatus.TASK_FINISHED)
                {
                    IsStartGameFromCenter();

                }
                else if (myprivilegeActivityList[i].status == (int)TaskStatus.TASK_UNFINISH)
                {
                    gray.enabled = false;
                    OKText.text = "领取";
                }
            }
        }

        void _InitRewardItemList()
        {
            List<AwardItemData> items = ActiveManager.GetInstance().GetActiveAwards(privilegeRwerdID);

            if (items == null)
            {
                Logger.LogErrorFormat("PrivilegeItems is null ...");
                return;
            }

            m_comRewardItemList.Initialize();

            m_comRewardItemList.onBindItem = var =>
            {
                return CreateComItem(Utility.FindGameObject(var, "itemPos"));
            };

            m_comRewardItemList.onItemVisiable = var =>
            {
                if (items != null)
                {
                    if (var.m_index >= 0 && var.m_index < items.Count)
                    {
                        ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(items[var.m_index].ID);
                        itemData.Count = items[var.m_index].Num;

                        ComItem comItem = var.gameObjectBindScript as ComItem;
                        comItem.Setup(itemData, (var1, var2) =>
                        {
                            ItemTipManager.GetInstance().ShowTip(var2);
                        });

                        Utility.GetComponetInChild<Text>(var.gameObject, "Name").text = itemData.GetColorName();
                    }
                }
            };

            m_comRewardItemList.SetElementAmount(items.Count);

        }
        #endregion

        #region LuckyTurn
        public const int tableID = 10001;
        public const int luckyGuyActiveID = 17001;
        public const int luckyTemPlateID = 17000;
        int totleCount = -1;
        bool bIsLucky = false;
        Text TotalCount;
        Text Count;
        GameObject items;
        UIGray startGray;
        Button startBtn;
        GameObject zhizhen;
        TheLuckyRoller theLuckyRoller;
        DrawRetData drawRetData = null;
        List<TaskPair> myLuckyGuyTaskPairList = new List<TaskPair>();

       
        class DrawRetData
        {
           public uint drawPrizeTableId;//表ID
           public uint rewardId; //抽到的奖励
        }
        
        void CreatLuckyItems()
        {
            TheyLuckyData luckyData = OPPOPrivilegeDataManager.GetInstance().GetTheLuckyData(tableID);
            if (luckyData == null)
            {
                Logger.LogErrorFormat("luckyData is Null");
            }
            for (int i = 0; i < luckyData.itemData.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(luckyData.itemData[i].ID);
                itemData.Count = luckyData.itemData[i].Num;

                ComItem comItem = CreateComItem(Utility.FindChild(frame, string.Format("ChildFrame/LuckyTurnTable/Items/Pos{0}", i)));
                comItem.Setup(itemData, (var1, var2) =>
                {
                    ItemTipManager.GetInstance().ShowTip(var2);
                });
            }
        }

        void StartRotaryTable()
        {
            GetLuckyGuyItemData();
            DrawPrizeTable drawPrizeTabel = TableManager.GetInstance().GetTableItem<DrawPrizeTable>(tableID);
            if (drawPrizeTabel == null)
            {
                Logger.LogErrorFormat("DrawPrizeTabl is null");
                return;
            }

            if (myLuckyGuyTaskPairList.Count > 0 && bIsLucky)
            {
                int Pos = 0;
                for (int i = 0; i < myLuckyGuyTaskPairList.Count; i++)
                {
                    if (myLuckyGuyTaskPairList[i].key == drawPrizeTabel.RewardIDKey)
                    {

                        int.TryParse(myLuckyGuyTaskPairList[i].value, out Pos);
                        theLuckyRoller.RotateUp(8, Pos, true, GetItem);
                    }
                }
                
            }
        }

        void  OnRaffleButtonClick()
        {
            ActiveManager.GetInstance().SendSubmitActivity(luckyGuyActiveID);
            startBtn.image.raycastTarget = false;

            GameStatisticManager.GetInstance().DoStartOPPO(StartOPPOType.OPPOLUCKYTABLE);
        }

        void GetLuckyGuyItemData()
        {
            myLuckyGuyTaskPairList.Clear();
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(luckyTemPlateID);

            if (activeData == null)
            {
                Logger.LogErrorFormat("activeData is null");
                return;
            }

            for (int i = 0; i < activeData.akChildItems.Count; i++)
            {
                for (int j = 0; j < activeData.akChildItems[i].akActivityValues.Count; j++)
                {
                    myLuckyGuyTaskPairList.Add(activeData.akChildItems[i].akActivityValues[j]);
                }
            }
        }

        void ClearLuckyGuyRewardData()
        {
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(luckyTemPlateID);

            if (activeData == null)
            {
                Logger.LogErrorFormat("activeData is null");
            }

            DrawPrizeTable drawPrizeTabel = TableManager.GetInstance().GetTableItem<DrawPrizeTable>(tableID);
            if (drawPrizeTabel == null)
            {
                Logger.LogErrorFormat("DrawPrizeTabl is null");
            }

            for (int i = 0; i < activeData.akChildItems.Count; i++)
            {
                for (int j = 0; j < activeData.akChildItems[i].akActivityValues.Count; j++)
                {
                    if (activeData.akChildItems[i].akActivityValues[j].key == drawPrizeTabel.RewardIDKey)
                    {
                        activeData.akChildItems[i].akActivityValues.Remove(activeData.akChildItems[i].akActivityValues[j]);
                    }
                }
            }
        }

        void UpdateState()
        {
            DrawPrizeTable drawPrizeTabel = TableManager.GetInstance().GetTableItem<DrawPrizeTable>(tableID);
            if (drawPrizeTabel == null)
            {
                Logger.LogErrorFormat("DrawPrizeTabl is null");
                return;
            }

            GetLuckyGuyItemData();

            int count = 0;
            int totleCount = 0;
            for (int i = 0; i < myLuckyGuyTaskPairList.Count; i++)
            {
               if (myLuckyGuyTaskPairList[i].key == drawPrizeTabel.RestCountKey)
               {
                    int.TryParse(myLuckyGuyTaskPairList[i].value,out count);
               }
               else if (myLuckyGuyTaskPairList[i].key == drawPrizeTabel.ContinuousKey)
               {
                    int.TryParse(myLuckyGuyTaskPairList[i].value, out totleCount);
                }
                
            }
            //int count = OPPOPrivilegeDataManager.GetInstance().surplusNum;
            
            //totleCount = OPPOPrivilegeDataManager.GetInstance().totalNum;

            if (Count != null)
            {
                Count.text = count.ToString();
            }

            if (TotalCount != null)
            {
                TotalCount.text = totleCount.ToString();
            }

            if (count > 0)
            {

                if (totleCount == drawPrizeTabel.ContinuousDays && count != 0)
                {
                    SystemNotifyManager.SystemNotify(9202);
                }

                if (startGray != null && startBtn != null)
                {
                    startGray.enabled = false;
                    startBtn.image.raycastTarget = true;
                }

            }
            else
            {
                if (startGray != null && startBtn != null)
                {
                    startGray.enabled = true;
                    startBtn.image.raycastTarget = false;
                }

            }
        }
        void GetItem()
        {
            GetLuckyGuyItemData();
            DrawPrizeTable drawPrizeTabel = TableManager.GetInstance().GetTableItem<DrawPrizeTable>(tableID);
            if (drawPrizeTabel == null)
            {
                Logger.LogErrorFormat("DrawPrizeTabl is null");
                return;
            }
            if (bIsLucky)
            {
                int Pos = 0;
                if (myLuckyGuyTaskPairList.Count > 0)
                {

                    for (int i = 0; i < myLuckyGuyTaskPairList.Count; i++)
                    {
                        if (myLuckyGuyTaskPairList[i].key == drawPrizeTabel.RewardIDKey)
                        {
                            int.TryParse(myLuckyGuyTaskPairList[i].value, out Pos);
                            RewardPoolTable poolItem = TableManager.GetInstance().GetTableItem<RewardPoolTable>(Pos);
                            if (poolItem == null)
                            {
                                Logger.LogErrorFormat(" RewardPoolTable No Find..");
                                continue;
                            }
                            ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(poolItem.ItemID);
                            if (itemData == null)
                            {
                                Logger.LogErrorFormat("ItemData is Null");
                            }
                            GameClient.SystemNotifyManager.SysNotifyFloatingEffect(itemData.GetColorName() + "*" + poolItem.ItemNum, CommonTipsDesc.eShowMode.SI_QUEUE, poolItem.ItemID);
                            UpdateState();
                            ClearLuckyGuyRewardData();
                            startBtn.image.raycastTarget = true;
                        }
                    }
                }
                
            }
            
        }
       
        #endregion

        #region Daily
        public const int num = 7;
        public const int dailID = 15000;
        public const int dailSevenID = 16000;
        public const int dailSevenActivityID = 16001;
        public const string retroactiveDes = "您可以花费{0}{1}补签{2}天，是否确认？";
        int iCost = 0;
        bool isDail = false;
        ItemData itemdata = null;
        GameObject DailyItems;
        List<GameObject> DailyItemList = new List<GameObject>();
        GameObject Slider;
        List<GameObject> ProgresssList = new List<GameObject>();
        GameObject Box;
        ComCommonBind boxComBind;
        Image boxImag;
        GameObject boxTeXiao;
        Button boxBtn;
        UIGray boxBtnGray;
        GameObject selectGo;
        Text ThisWeekNum;
        Text LeakageSignNum;
        UIGray RetroactiveGray;
        Button RetroactiveBtn;
        List<GameObject> GaoLiangs = new List<GameObject>();
        List<GameObject> CountersignList = new List<GameObject>();
        List<Button> CtsSignBtnList = new List<Button>();
        List<GameObject> Receives = new List<GameObject>();
        List<GameObject> ItemPosList = new List<GameObject>();
        List<Text> ItemCountList = new List<Text>();
        List<Button> SignList = new List<Button>();
        List<UIGray> GrayLisy = new List<UIGray>();
        List<ActiveManager.ActivityData> myActivityList = new List<ActiveManager.ActivityData>();
        List<ActiveManager.ActivityData> myDailSevenActivityList = new List<ActiveManager.ActivityData>();


        void GetSignItemAndNumber()
        {
            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT__RP_OPPOSIGHIN_COST_ITEMID);
            if (functionData != null)
            {
                itemdata = ItemDataManager.GetInstance().GetCommonItemTableDataByID(functionData.Value);
            }
            var IcostNum = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_OPPOSIGHIN_FREE_NUM);
            if (IcostNum != null)
            {
                iCost = IcostNum.Value;
            }
        }
        void OnRapidRetroactiveClick()
        {
            GetDailItemData();
            List<uint> akTaskIDs = new List<uint>();
            
            List<ActiveManager.ActivityData> data = new List<ActiveManager.ActivityData>();

            data.Clear();

            for (int i = 0; i < myActivityList.Count; i++)
            {
                if (myActivityList[i].status == (int)TaskStatus.TASK_FAILED)
                {
                    data.Add(myActivityList[i]);
                }
            }
            akTaskIDs.Clear();
            for (int i = 0; i < data.Count; i++)
            {
                akTaskIDs.Add((uint)data[i].ID);
            }
            string ncontent = string.Format(retroactiveDes, iCost* akTaskIDs.Count, itemdata.Name, akTaskIDs.Count);
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(ncontent,()=> 
            {
                if (akTaskIDs.Count>0)
                {
                    if ((int)PlayerBaseData.GetInstance().BindTicket >= iCost * akTaskIDs.Count)
                    {
                        SceneActiveTaskSubmitRp kSend = new SceneActiveTaskSubmitRp();
                        kSend.taskId = akTaskIDs.ToArray();

                        Logger.LogProcessFormat("[activity]SceneActiveTaskSubmitRp Array count = {0}", akTaskIDs.Count);

                        NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
                    }
                    else
                    {
                        SystemNotifyManager.SystemNotify(9206);
                    }
                }
            });
        }

        void GetDailItemData()
        {
            myActivityList.Clear();
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(dailID);
            for (int i = 0; i < activeData.akChildItems.Count; i++)
            {
                myActivityList.Add(activeData.akChildItems[i]);
            }
        }
        void GetDailSevenItemData()
        {
            myDailSevenActivityList.Clear();
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(dailSevenID);
            for (int i = 0; i < activeData.akChildItems.Count; i++)
            {
                myDailSevenActivityList.Add(activeData.akChildItems[i]);
            }
        }

        void CreatDailItem()
        {
            GetDailItemData();
            for (int i = 0; i < myActivityList.Count; i++)
            {
                List<AwardItemData> awardList = ActiveManager.GetInstance().GetActiveAwards(myActivityList[i].ID);
                if (awardList == null)
                {
                    continue;
                }
                for (int j = 0; j < awardList.Count; j++)
                {
                    ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(awardList[j].ID);
                    itemData.Count = 0;
                    if (itemData == null)
                    {
                        continue;
                    }
                    if (awardList[j].Num > 1)
                    {
                        ItemCountList[i].text = awardList[j].Num.ToString();
                    }
                    else
                    {
                        ItemCountList[i].CustomActive(false);
                    }
                    ComItem comitem = CreateComItem(ItemPosList[i].gameObject);
                    comitem.Setup(itemData, (var1, var2) =>
                    {
                        ItemTipManager.GetInstance().ShowTip(var2);
                    });
                }

                int index = i;
                SignList[i].onClick.RemoveAllListeners();
                SignList[index].onClick.AddListener(() =>
                {
                    ActiveManager.GetInstance().SendSubmitActivity(myActivityList[index].ID);

                    GameStatisticManager.GetInstance().DoStartOPPO(StartOPPOType.OPPOSIGE);
                });

                CtsSignBtnList[i].onClick.RemoveAllListeners();
                CtsSignBtnList[index].onClick.AddListener(() =>
                {
                    List<uint> akTaskIDs = new List<uint>();

                    akTaskIDs.Clear();
                    if (myActivityList[index].status == (int)TaskStatus.TASK_FAILED)
                    {
                        akTaskIDs.Add((uint)myActivityList[index].ID);
                    }
                    string ncontent = string.Format(retroactiveDes, iCost, itemdata.Name, akTaskIDs.Count);
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(ncontent, () =>
                    {

                        if ((int)PlayerBaseData.GetInstance().BindTicket >= iCost* akTaskIDs.Count)
                        {
                            SceneActiveTaskSubmitRp kSend = new SceneActiveTaskSubmitRp();
                            kSend.taskId = akTaskIDs.ToArray();

                            Logger.LogProcessFormat("[activity]SceneActiveTaskSubmitRp Array count = {0}", akTaskIDs.Count);

                            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
                        }
                        else
                        {
                            SystemNotifyManager.SystemNotify(9206);
                        }
                    });
                });
            }
        }
        void UpdateCompomentState()
        {
            Hide();
            GetDailItemData();
            int SignedNumber = 0;
            int LeakageSigneNumber = 0;
            for (int i = 0; i < myActivityList.Count; i++)
            {
                if (myActivityList[i].status == (int)TaskStatus.TASK_OVER)
                {
                    if (GrayLisy.Count!=0 && Receives.Count!=0)
                    {
                        if (GrayLisy[i] != null && Receives[i] != null)
                        {
                            GrayLisy[i].enabled = true;
                            Receives[i].CustomActive(true);
                            SignedNumber++;
                        }
                    }
                   
                }
                else if (myActivityList[i].status == (int)TaskStatus.TASK_FINISHED)
                {
                    if (GaoLiangs.Count != 0&& SignList.Count !=0 && GrayLisy.Count !=0)
                    {
                        if (GaoLiangs[i] != null && SignList[i] != null && GrayLisy[i] != null)
                        {
                            GaoLiangs[i].CustomActive(true);
                            SignList[i].CustomActive(true);
                            GrayLisy[i].enabled = false;
                        }
                    }
                       
                }
                else if (myActivityList[i].status == (int)TaskStatus.TASK_FAILED)
                {
                    if (CountersignList.Count != 0&& GrayLisy.Count !=0)
                    {
                        if (CountersignList[i] != null && GrayLisy[i] != null)
                        {
                            CountersignList[i].CustomActive(true);
                            GrayLisy[i].enabled = false;
                            GrayLisy[i].enabled = true;
                            LeakageSigneNumber++;
                        }
                    }
                   
                }
                else if (myActivityList[i].status == (int)TaskStatus.TASK_UNFINISH)
                {
                    if (CountersignList.Count !=0 && GrayLisy.Count !=0 && GaoLiangs.Count != 0&& SignList.Count !=0&& Receives.Count !=0)
                    {
                        if (CountersignList[i] != null && GrayLisy[i] != null && GaoLiangs[i] != null && SignList[i] != null && Receives[i] != null)
                        {
                            Receives[i].CustomActive(false);
                            CountersignList[i].CustomActive(false);
                            SignList[i].CustomActive(false);
                            GaoLiangs[i].CustomActive(false);
                            GrayLisy[i].enabled = false;
                        }
                    }
                   
                }
            }

            for (int i = 0; i < myDailSevenActivityList.Count; i++)
            {
                if (myDailSevenActivityList[i].status == (int)TaskStatus.TASK_OVER)
                {
                    if (boxComBind != null && boxBtn != null && boxTeXiao != null && boxBtnGray != null)
                    {
                        boxComBind.GetSprite("open", ref boxImag);
                        boxBtn.enabled = false;
                        boxBtnGray.enabled = false;
                        boxBtnGray.enabled = true;
                        boxTeXiao.CustomActive(false);
                    }
                }
                else if (myDailSevenActivityList[i].status == (int)TaskStatus.TASK_FINISHED && boxBtnGray != null)
                {
                    if (boxComBind != null && boxBtn != null && boxTeXiao != null)
                    {
                        boxComBind.GetSprite("close", ref boxImag);
                        boxBtn.enabled = true;
                        boxBtnGray.enabled = false;
                        boxTeXiao.CustomActive(true);
                    }
                }
                else if (myDailSevenActivityList[i].status == (int)TaskStatus.TASK_UNFINISH)
                {
                    if (boxComBind != null && boxBtn != null && boxTeXiao != null&& boxBtnGray!=null)
                    {
                        boxComBind.GetSprite("close", ref boxImag);
                        boxBtn.enabled = true;
                        boxBtnGray.enabled = false;
                        boxTeXiao.CustomActive(false);
                    }
                }
            }

            if (ThisWeekNum != null)
            {
                ThisWeekNum.text =SignedNumber.ToString()+"次";
            }
            if (LeakageSignNum !=null)
            {
                LeakageSignNum.text = LeakageSigneNumber.ToString()+"次";
            }

            if (LeakageSigneNumber > 0)
            {
                if (RetroactiveGray != null && RetroactiveBtn !=null)
                {
                    RetroactiveGray.enabled = false;
                    RetroactiveBtn.enabled = true;
                }
            }
            else
            {
                if (RetroactiveGray != null && RetroactiveBtn != null)
                {
                    RetroactiveGray.enabled = true;
                    RetroactiveBtn.enabled = false;
                }
              
            }

            for (int i = 0; i < SignedNumber; i++)
            {
                if (ProgresssList[i]!= null)
                {
                    ProgresssList[i].CustomActive(true);
                }
            }

            if (SignedNumber == 7)
            {
                
                if (boxBtn != null)
                {
                    boxBtn.onClick.RemoveAllListeners();
                    boxBtn.onClick.AddListener(() =>
                    {
                        ActiveManager.GetInstance().SendSubmitActivity(dailSevenActivityID);
                    });
                }
            }
            else
            {
              
                if (boxBtn != null)
                {
                    boxBtn.onClick.RemoveAllListeners();
                    boxBtn.onClick.AddListener(() =>
                    {
                        GetDailSevenItemData();
                        for (int i = 0; i < myDailSevenActivityList.Count; i++)
                        {
                            List<AwardItemData> awardList = ActiveManager.GetInstance().GetActiveAwards(myDailSevenActivityList[i].ID);
                            if (awardList == null)
                            {
                                continue;
                            }
                            ActiveAwardFrameData data = new ActiveAwardFrameData();
                            data.title = "七日签到宝箱";
                            data.datas = awardList;
                            ClientSystemManager.GetInstance().OpenFrame<ActiveAwardFrame>(FrameLayer.Middle, data);
                        }
                    });
                    
                }
            }
        }

        void _FinAllComponents()
        {
            for (int i = 0; i < num; i++)
            {
                GameObject goObj = Utility.FindChild(frame, string.Format("ChildFrame/DailyCheck/Items/Item{0}", i));
                if (goObj == null)
                {
                    continue;
                }
                DailyItemList.Add(goObj);
            }

            for (int i = 0; i < DailyItemList.Count; i++)
            {
                ComCommonBind comBind = DailyItemList[i].GetComponent<ComCommonBind>();
                if (comBind == null)
                {
                    continue;
                }
                GameObject gaoliang = comBind.GetGameObject("Highlight");
                if (gaoliang != null)
                {
                    GaoLiangs.Add(gaoliang);
                }
                GameObject countersign = comBind.GetGameObject("Countersign");
                if (countersign != null)
                {
                    CountersignList.Add(countersign);
                }
                GameObject receive = comBind.GetGameObject("Receive");
                if (receive != null)
                {
                    Receives.Add(receive);
                }
                GameObject itemGo = comBind.GetGameObject("ItemPos");
                if (itemGo != null)
                {
                    ItemPosList.Add(itemGo);
                }
                Button signBtn = comBind.GetCom<Button>("Sign");
                if (signBtn != null)
                {
                    SignList.Add(signBtn);
                }
                UIGray gray = comBind.GetCom<UIGray>("InfoUIGray");
                if (gray != null)
                {
                    GrayLisy.Add(gray);
                }
                Text countText = comBind.GetCom<Text>("Count");
                if (countText != null)
                {
                    ItemCountList.Add(countText);
                }
                Button CtsSignBtn = comBind.GetCom<Button>("CtsSignBtn");
                if (CtsSignBtn != null)
                {
                    CtsSignBtnList.Add(CtsSignBtn);
                }
            }
            for (int i = 0; i < num; i++)
            {
                GameObject Go = Utility.FindChild(frame, string.Format("ChildFrame/DailyCheck/Buttom/Slider/Di{0}", i));
                if (Go == null)
                {
                    continue;
                }
                ProgresssList.Add(Go);
            }

            Hide();
            _TreasureBoxStatus();
            GetSignItemAndNumber();
        }
        void Hide()
        {
            for (int i = 0; i < GaoLiangs.Count; i++)
            {
                GaoLiangs[i].CustomActive(false);
            }
            for (int i = 0; i < CountersignList.Count; i++)
            {
                CountersignList[i].CustomActive(false);
            }

            for (int i = 0; i < Receives.Count; i++)
            {
                Receives[i].CustomActive(false);
            }
            for (int i = 0; i < SignList.Count; i++)
            {
                SignList[i].CustomActive(false);
            }

            for (int i = 0; i < ProgresssList.Count; i++)
            {
                ProgresssList[i].CustomActive(false);
            }
        }
        void _TreasureBoxStatus()
        {
            boxComBind = Box.GetComponent<ComCommonBind>();
            if (boxComBind != null)
            {
                boxImag = boxComBind.GetCom<Image>("BoxState");
                selectGo = boxComBind.GetGameObject("image");
                boxTeXiao = boxComBind.GetGameObject("TeXiao");
                if (boxTeXiao != null)
                {
                    boxTeXiao.CustomActive(false);
                }
                //boxComBind.GetSprite("close", ref boxImag);
                boxBtn = boxComBind.GetCom<Button>("Button");
                boxBtnGray = boxComBind.GetCom<UIGray>("BoxGray");
            }
        }
        #endregion

        #region AmberPrivilege
        AmberPrivilegeView mAmberPrivilegeView;
        #endregion
        #region OPPOGrowthHaoLi
        OPPOGrowthHaoLiView mOPPOGrowthHaoLiView;
        #endregion
        private void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            GetPriviItemData();
            GetDailSevenItemData();
            UpdateCompomentState();
            StartRotaryTable();
            UpdateState();
            _InitAmberGiftBagInfo();

            if (mAmberPrivilegeView != null)
            {
                mAmberPrivilegeView.UpdateElementAmount();
            }

            if (mOPPOGrowthHaoLiView != null)
            {
                mOPPOGrowthHaoLiView.UpdateElementAmount();
            }
        }

        List<object> m_kButtons = new List<object>();
        new void _AddButton(string path, UnityEngine.Events.UnityAction events)
        {
            var button = Utility.FindComponent<Button>(frame, path);
            if (null != button)
            {
                button.onClick.AddListener(events);
                m_kButtons.Add(button);
            }
        }

        void _RemoveAllButtonDelegates()
        {
            for (int i = 0; i < m_kButtons.Count; i++)
            {
                (m_kButtons[i] as Button).onClick.RemoveAllListeners();
            }
            m_kButtons.Clear();
        }

        [UIEventHandle("close")]
        void OnCloseClick()
        {
            frameMgr.CloseFrame(this);
        }
    }
}

