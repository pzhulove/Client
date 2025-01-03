using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class GuildMainCityFrame : ClientFrame
    {
        class GuildBuildingInfo
        {
            public GuildBuildingData data;
            public Text labLevel;
            public Text labCostCount;
            public ComButtonEnbale comLevelupEnable;
            public List<GameObject> arrObjLevelup = new List<GameObject>();
            public List<GameObject> arrObjMax = new List<GameObject>();
            public Text labMax;
        }
        List<GuildBuildingInfo> m_arrBuildingInfos = new List<GuildBuildingInfo>();

        private GameObject buildingRedPoint = null;
        private GameObject honourRedPoint = null;
        private GameObject bossDiffSetRedPoint = null;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildMainCity";
        }

        private delegate bool CheckBuildingIsUnLock(); // 建筑是否解锁了
        protected override void _OnOpenFrame()
        {
            _UpdateBuildings();
            _UpdateRedPoint();
            _RegisterUIEvent();
        }
        protected override void _OnCloseFrame()
        {
            m_arrBuildingInfos.Clear();
            _UnRegisterUIEvent();
        }
        protected override void _bindExUI()
        {
            buildingRedPoint = mBind.GetGameObject("buildingRedPoint");
            honourRedPoint = mBind.GetGameObject("honourRedPoint");
            bossDiffSetRedPoint = mBind.GetGameObject("bossDiffSetRedPoint");
        }
        protected override void _unbindExUI()
        {
            buildingRedPoint = null;
            honourRedPoint = null;
            bossDiffSetRedPoint = null;
        }
        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildUpgradeBuildingSuccess, _OnUpgradeBuildingSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
        }
        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildUpgradeBuildingSuccess, _OnUpgradeBuildingSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
        }
        void _UpdateRedPoint()
        {
            buildingRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildBuildingManager));
            honourRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildEmblem));
            bossDiffSetRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildSetBossDiff));
        }
        void _OnRedPointChanged(UIEvent a_event)
        {
            _UpdateRedPoint();
        }
        void _UpdateBuildings()
        {
            m_arrBuildingInfos.Clear();
            Dictionary<GuildBuildingType, GameObject> dictObjBuildingRoot = new Dictionary<GuildBuildingType, GameObject>();
            dictObjBuildingRoot.Add(GuildBuildingType.MAIN, Utility.FindGameObject(frame, "ScrollView/Viewport/Content/MainCity"));
            //dictObjBuildingRoot.Add(GuildBuildingType.WELFARE, Utility.FindGameObject(frame, "ScrollView/Viewport/Content/Welfare"));
            //dictObjBuildingRoot.Add(GuildBuildingType.STATUE, Utility.FindGameObject(frame, "ScrollView/Viewport/Content/Statue"));
            //dictObjBuildingRoot.Add(GuildBuildingType.TABLE, Utility.FindGameObject(frame, "ScrollView/Viewport/Content/Table"));
            dictObjBuildingRoot.Add(GuildBuildingType.SHOP, Utility.FindGameObject(frame, "ScrollView/Viewport/Content/Shop"));
            //dictObjBuildingRoot.Add(GuildBuildingType.BATTLE, Utility.FindGameObject(frame, "ScrollView/Viewport/Content/Skill"));
            dictObjBuildingRoot.Add(GuildBuildingType.HONOUR, Utility.FindGameObject(frame, "ScrollView/Viewport/Content/Honour"));
            dictObjBuildingRoot.Add(GuildBuildingType.FETE, Utility.FindGameObject(frame, "ScrollView/Viewport/Content/Fete"));
            //             foreach(var obj in dictObjBuildingRoot)
            //             {
            //                 obj.Value.CustomActive(false);

            Dictionary<GuildBuildingType, UnityEngine.Events.UnityAction> btnCallBack = new Dictionary<GuildBuildingType, UnityEngine.Events.UnityAction>();
            if (btnCallBack != null)
            {
                btnCallBack[GuildBuildingType.MAIN] = () =>
                {
                    ClientSystemManager.GetInstance().OpenFrame<GuildBuildingManagerFrame>();
                    if (buildingRedPoint.activeSelf)
                    {
                        GuildDataManager.GetInstance().checkedLvUpBulilding = true;
                    }
                    buildingRedPoint.CustomActive(false);
                };
                btnCallBack[GuildBuildingType.SHOP] = () =>
                {
                    //GuildMyMainFrame.OpenLinkFrame(((int)GuildMyMainFrame.EOperateType.OpenBuildingShop).ToString());
                    int nShopLevel = GuildDataManager.GetInstance().myGuild.dictBuildings[GuildBuildingType.SHOP].nLevel;
                    ProtoTable.GuildBuildingTable buildingTable = TableManager.GetInstance().GetTableItem<ProtoTable.GuildBuildingTable>(nShopLevel);
                    if (buildingTable != null)
                    {                       
                        ShopNewDataManager.GetInstance().OpenShopNewFrame(buildingTable.ShopId);                    
                    }
                };
                btnCallBack[GuildBuildingType.HONOUR] = () =>
                {
                    if (!(GuildDataManager.GetInstance().GetGuildLv() >= GuildDataManager.GetInstance().GetEmblemLvUpGuildLvLimit()
                    && PlayerBaseData.GetInstance().Level >= GuildDataManager.GetInstance().GetEmblemLvUpPlayerLvLimit()))
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("emblem_unlock_condition", GuildDataManager.GetInstance().GetEmblemLvUpGuildLvLimit(), GuildDataManager.GetInstance().GetEmblemLvUpPlayerLvLimit()));
                        return;
                    }
                    ClientSystemManager.GetInstance().OpenFrame<GuildEmblemUpFrame>();
                    if (honourRedPoint.activeSelf)
                    {
                        GuildDataManager.GetInstance().checkedLvUpEmblem = true;
                    }
                    honourRedPoint.CustomActive(false);
                };
                btnCallBack[GuildBuildingType.FETE] = () =>
                {
                    if (!(GuildDataManager.GetInstance().GetGuildLv() >= GuildDataManager.GetGuildDungeonActivityGuildLvLimit()))
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guild_build_boss_lv_set_condition", GuildDataManager.GetGuildDungeonActivityGuildLvLimit()));
                        return;
                    }
                    ClientSystemManager.GetInstance().OpenFrame<GuildDungeonBossDiffSetFrame>();
                    if (bossDiffSetRedPoint.activeSelf)
                    {
                        GuildDataManager.GetInstance().checkedSetBossDiff = true;
                    }
                    bossDiffSetRedPoint.CustomActive(false);
                };
            }

            Dictionary<GuildBuildingType, CheckBuildingIsUnLock> btnConditon = new Dictionary<GuildBuildingType, CheckBuildingIsUnLock>();
            if (btnConditon != null)
            {
                btnConditon[GuildBuildingType.HONOUR] = () =>
                {
                    return ((GuildDataManager.GetInstance().GetGuildLv() >= GuildDataManager.GetInstance().GetEmblemLvUpGuildLvLimit()
                    && PlayerBaseData.GetInstance().Level >= GuildDataManager.GetInstance().GetEmblemLvUpPlayerLvLimit()));
                };
                btnConditon[GuildBuildingType.FETE] = () =>
                {
                    return ((GuildDataManager.GetInstance().GetGuildLv() >= GuildDataManager.GetGuildDungeonActivityGuildLvLimit()));
                };
            }

            if(GuildDataManager.GetInstance().myGuild != null && GuildDataManager.GetInstance().myGuild.dictBuildings != null)
            {
                var iter = GuildDataManager.GetInstance().myGuild.dictBuildings.GetEnumerator();
                while (iter.MoveNext())
                {
                    if (dictObjBuildingRoot.ContainsKey(iter.Current.Key) == false)
                    {
                        continue;
                    }

                    GuildBuildingInfo info = new GuildBuildingInfo();
                    info.data = iter.Current.Value;
                    GameObject objRoot = dictObjBuildingRoot[info.data.eType];
                    objRoot.CustomActive(true);
                    info.labLevel = Utility.GetComponetInChild<Text>(objRoot, "Level");
                    //info.labCostCount = Utility.GetComponetInChild<Text>(objRoot, "LevelUp/Count");
                    //info.comLevelupEnable = Utility.GetComponetInChild<ComButtonEnbale>(objRoot, "LevelUp");
                    //info.arrObjLevelup.Add(Utility.FindGameObject(objRoot, "LevelUp/Text"));
                    //info.arrObjLevelup.Add(Utility.FindGameObject(objRoot, "LevelUp/Icon"));
                    //info.arrObjLevelup.Add(Utility.FindGameObject(objRoot, "LevelUp/Count"));
                    //info.arrObjMax.Add(Utility.FindGameObject(objRoot, "LevelUp/Max"));
                    //info.labMax = Utility.GetComponetInChild<Text>(objRoot, "LevelUp/Max");

                    Button btnUpgrade = Utility.GetComponetInChild<Button>(objRoot, "LevelUp");
                    btnUpgrade.onClick.RemoveAllListeners();
                    btnUpgrade.onClick.AddListener(() =>
                    {
                        if (btnCallBack != null)
                        {
                            btnCallBack[info.data.eType].Invoke();
                        }
                        //SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("guild_upgrade_building_ask", info.data.nUpgradeCost),() => { GuildDataManager.GetInstance().UpgradeBuilding(info.data.eType, info.data.nUpgradeCost); });
                    });

                    UIGray uiGray = objRoot.SafeAddComponent<UIGray>(false);
                    if (uiGray != null)
                    {
                        uiGray.bEnabled2Text = false;
                        uiGray.enabled = false;
                        uiGray.enabled = GuildDataManager.GetInstance().GetGuildLv() < GuildDataManager.GetInstance().GetUnLockBuildingNeedMainCityLv(info.data.eType); // 图标是否置灰只与主城等级有关
                    }
                    if (btnConditon != null && btnConditon.ContainsKey(info.data.eType))
                    {
                        btnUpgrade.SafeSetGray(!btnConditon[info.data.eType].Invoke());
                    }

                    m_arrBuildingInfos.Add(info);
                }
            }

            for (int i = 0; i < m_arrBuildingInfos.Count; ++i)
            {
                GuildBuildingData data = m_arrBuildingInfos[i].data;
                GuildBuildingInfo info = m_arrBuildingInfos[i];

                int nCurrentLevel = data.nLevel;
                int nMaxLevel = data.nMaxLevel;
                if (nMaxLevel <= 0)
                {
                    nCurrentLevel = 1;
                    nMaxLevel = 1;
                }

                info.labLevel.text = string.Format("Lv.{0}", nCurrentLevel);
                info.labLevel.CustomActive(GuildDataManager.GetInstance().GetGuildLv() >= GuildDataManager.GetInstance().GetUnLockBuildingNeedMainCityLv(info.data.eType)); // 建筑未解锁则不显示等级

                if (nCurrentLevel >= nMaxLevel)
                {
                    //_SetObjsActive(info.arrObjLevelup, true);
                    //_SetObjsActive(info.arrObjMax, true);
                    //info.comLevelupEnable.SetEnable(false);

                    if (data.nUnlockMaincityLevel > 0)
                    {
                        //info.labMax.text = TR.Value("guild_building_need_high_maincity", data.nUnlockMaincityLevel);
                    }
                    else
                    {
                        //info.labMax.text = TR.Value("guild_building_already_max");
                    }
                }
                else
                {
                    //_SetObjsActive(info.arrObjLevelup, true);
                    //_SetObjsActive(info.arrObjMax, false);
                    //info.comLevelupEnable.SetEnable(true);
                    //info.labCostCount.text = data.nUpgradeCost.ToString();
                }

                if (GuildDataManager.GetInstance().HasPermission(EGuildPermission.UpgradeBuilding) == false)
                {
                    //info.comLevelupEnable.SetEnable(false);
                }
            }
        }

        void _OnUpgradeBuildingSuccess(UIEvent a_event)
        {
            _UpdateBuildings();
            _UpdateRedPoint();
        }

        void _SetObjsActive(List<GameObject> a_arrObjs, bool a_bActive)
        {
            for (int i = 0; i < a_arrObjs.Count; ++i)
            {
                a_arrObjs[i].SetActive(a_bActive);
            }
        }
    }
}
