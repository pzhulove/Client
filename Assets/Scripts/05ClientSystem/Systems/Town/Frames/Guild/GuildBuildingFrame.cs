using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{

    class GuildBuildingFrame : ClientFrame
    {
        class BuildingInfo
        {
            public GuildBuildingType eBuildType;
            public string strPath;
            public ERedPoint eRedPointType;
            public Type frameType;
            public Toggle toggle;
            public GameObject objRedPoint;
            public Text labLevel;
        }

        [UIObject("TabGroup/Page")]
        GameObject m_objContentRoot;

        BuildingInfo[] m_arrBuildingInfos =
        {
            new BuildingInfo { eBuildType = GuildBuildingType.MAIN,     strPath = "MainCity",    eRedPointType = ERedPoint.GuildMainCity,   frameType = typeof(GuildMainCityFrame)  },
            //new BuildingInfo { eBuildType = GuildBuildingType.SHOP,     strPath = "Shop",    eRedPointType = ERedPoint.GuildShop,       frameType = typeof(GuildShopFrame)  },
            //new BuildingInfo { eBuildType = GuildBuildingType.WELFARE,  strPath = "Welfare", eRedPointType = ERedPoint.GuildWelfare,    frameType = typeof(GuildWelfareFrame)  },
            //new BuildingInfo { eBuildType = GuildBuildingType.STATUE,   strPath = "Statue",  eRedPointType = ERedPoint.GuildStatue,     frameType = typeof(GuildStatueFrame)  },
            //new BuildingInfo { eBuildType = GuildBuildingType.BATTLE,    strPath = "Skill",   eRedPointType = ERedPoint.GuildSkill,      frameType = typeof(GuildSkillFrame)  },
            //new BuildingInfo { eBuildType = GuildBuildingType.TABLE,    strPath = "Table",   eRedPointType = ERedPoint.GuildTable,      frameType = typeof(GuildTableFrame)  },
        };

        const string m_strPath = "TabGroup/Tabs/";
        GuildBuildingType m_currBuildingType = GuildBuildingType.MAIN;
        bool m_bToggleBlockSignal = false;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildBuilding";
        }

        protected override void _OnOpenFrame()
        {
            if (userData != null)
            {
                m_currBuildingType = (GuildBuildingType)userData;
            }
            
            _InitTabs();
            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            for (int i = 0; i < m_arrBuildingInfos.Length; ++i)
            {
                if (m_arrBuildingInfos[i].eBuildType == m_currBuildingType)
                {
					frameMgr.CloseFrameByType(m_arrBuildingInfos[i].frameType, false);
                }
            }

            m_currBuildingType = GuildBuildingType.MAIN;
            m_bToggleBlockSignal = false;

            for (int i = 0; i < m_arrBuildingInfos.Length; ++i)
            {
                m_arrBuildingInfos[i].toggle = null;
                m_arrBuildingInfos[i].objRedPoint = null;
            }
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildCloseMainFrame, _OnCloseMainFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildUpgradeBuildingSuccess, _OnBuildLevelChanged);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildCloseMainFrame, _OnCloseMainFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildUpgradeBuildingSuccess, _OnBuildLevelChanged);
        }

        void _InitTabs()
        {
            m_bToggleBlockSignal = true;

            for (int i = 0; i < m_arrBuildingInfos.Length; ++i)
            {
                BuildingInfo info = m_arrBuildingInfos[i];
                GameObject objRoot = m_objContentRoot;
                string strPath = m_strPath + info.strPath;

                info.toggle = Utility.GetComponetInChild<Toggle>(frame, strPath);
                info.toggle.onValueChanged.RemoveAllListeners();
                info.toggle.onValueChanged.AddListener((bool a_bChecked) =>
                {
                    if (m_bToggleBlockSignal == false)
                    {
                        if (a_bChecked)
                        {
                            m_currBuildingType = info.eBuildType;
                            frameMgr.OpenFrame(info.frameType, objRoot);
                        }
                        else
                        {
							frameMgr.CloseFrameByType(info.frameType, false);
                        }
                    }
                });

                info.objRedPoint = Utility.FindGameObject(frame, strPath + "/RedPoint");
                info.objRedPoint.SetActive(RedPointDataManager.GetInstance().HasRedPoint(info.eRedPointType));
                //info.labLevel = Utility.GetComponetInChild<Text>(frame, strPath + "/Level");
                //info.labLevel.text = string.Format("Lv.{0}", GuildDataManager.GetInstance().GetBuildingLevel(info.eBuildType));
            }

            
            for (int i = 0; i < m_arrBuildingInfos.Length; ++i)
            {
                BuildingInfo info = m_arrBuildingInfos[i];
                if (info.eBuildType == m_currBuildingType)
                {
                    info.toggle.isOn = false;
                }
                else
                {
                    info.toggle.isOn = true;
                }
            }

            m_bToggleBlockSignal = false;
            BuildingInfo currentBuildingInfo = _GetBuildingInfo(m_currBuildingType);
            if (currentBuildingInfo != null)
            {
                currentBuildingInfo.toggle.isOn = true;
            }
        }

        BuildingInfo _GetBuildingInfo(GuildBuildingType a_type)
        {
            for (int i = 0; i < m_arrBuildingInfos.Length; ++i)
            {
                if (m_arrBuildingInfos[i].eBuildType == a_type)
                {
                    return m_arrBuildingInfos[i];
                }
            }
            return null;
        }

        void _CloseAll()
        {
            for (int i = 0; i < m_arrBuildingInfos.Length; ++i)
            {
                BuildingInfo info = m_arrBuildingInfos[i];
				frameMgr.CloseFrameByType(info.frameType, false);
            }
            frameMgr.CloseFrame(this);
        }

        void _OnCloseMainFrame(UIEvent a_event)
        {
            _CloseAll();
        }

        void _OnGuildDismissed(UIEvent a_event)
        {
            _CloseAll();
        }

        void _OnRedPointChanged(UIEvent a_event)
        {
            for (int i = 0; i < m_arrBuildingInfos.Length; ++i)
            {
                BuildingInfo info = m_arrBuildingInfos[i];
                info.objRedPoint.SetActive(RedPointDataManager.GetInstance().HasRedPoint(info.eRedPointType));
            }
        }

        //void _OnBuildLevelChanged(UIEvent a_event)
        //{
        //    for (int i = 0; i < m_arrBuildingInfos.Length; ++i)
        //    {
        //        BuildingInfo info = m_arrBuildingInfos[i];
        //        info.labLevel.text = string.Format("Lv.{0}", GuildDataManager.GetInstance().GetBuildingLevel(info.eBuildType));
        //    }
        //}
    }
}
