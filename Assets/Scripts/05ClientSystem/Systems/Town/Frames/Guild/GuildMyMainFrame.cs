using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class GuildMyMainFrame : ClientFrame
    {
        public enum EOperateType
        {
            Invalid = -1,

            OpenBaseInfo = 0,
            OpenMembers,
            OpenBuilding,
            OpenBuildingTable,
            OpenBuildingWelfare,
            OpenBuildingStatue,
            OpenBuildingSkill,
            OpenBuildingShop,
            OpenManor,
            OpenGuildStore,
            OpenGuildCrossManor,
            OpenGuildRedPacket,
            OpenGuildEmblemLevel,
            OpenGuildBenefits,
            OpenGuildActivity,

            Count,
        }

        delegate void OperateFunc(params object[] a_arrParams);

        [UIControl("TabGroup/ScrollRect/Viewport/Tabs/Base", typeof(Toggle))]
        Toggle m_toggleBase;

        [UIControl("TabGroup/ScrollRect/Viewport/Tabs/Member", typeof(Toggle))]
        Toggle m_toggleMember;

        [UIControl("TabGroup/ScrollRect/Viewport/Tabs/Building", typeof(Toggle))]
        Toggle m_toggleBuilding;

        [UIControl("TabGroup/ScrollRect/Viewport/Tabs/Manor", typeof(Toggle))]
        Toggle m_toggleManor;

        [UIControl("TabGroup/ScrollRect/Viewport/Tabs/Storage", typeof(Toggle))]
        Toggle m_toggleStorage;

        [UIControl("TabGroup/ScrollRect/Viewport/Tabs/Shop")]
        Toggle m_toggleShop;

        [UIControl("TabGroup/ScrollRect/Viewport/Tabs/CrossManor", typeof(Toggle))]
        Toggle m_toggleCrossManor;

        [UIControl("TabGroup/ScrollRect/Viewport/Tabs/RedPacket", typeof(Toggle))]
        Toggle m_toggleRedPacket;

        [UIObject("TabGroup/ScrollRect/Viewport/Tabs/Base/RedPoint")]
        GameObject m_objBaseRedPoint;

        [UIObject("TabGroup/ScrollRect/Viewport/Tabs/Member/RedPoint")]
        GameObject m_objMemberRedPoint;

        [UIObject("TabGroup/ScrollRect/Viewport/Tabs/Building/RedPoint")]
        GameObject m_objBuildingRedPoint;

        [UIObject("TabGroup/ScrollRect/Viewport/Tabs/Manor/RedPoint")]
        GameObject m_objManorRedPoint;

        [UIObject("TabGroup/ScrollRect/Viewport/Tabs/Shop/RedPoint")]
        GameObject m_objShopRedPoint;

        [UIObject("TabGroup/ScrollRect/Viewport/Tabs/CrossManor/RedPoint")]
        GameObject m_objCrossManorRedPoint;

        [UIObject("TabGroup/ScrollRect/Viewport/Tabs/RedPacket/RedPoint")]
        GameObject m_objRedPacketRedPoint;

        [UIObject("TabGroup/Page")]
        GameObject m_objContentRoot;

        [UIControl("BG/Title/Moneys", typeof(ComConsumeItemGroup))]
        ComConsumeItemGroup m_comConsumeItemGroup;

        bool m_bToggleBlockSignal = false;
        IClientFrame m_baseFrame = null;
        IClientFrame m_memberFrame = null;
        IClientFrame m_buildingFrame = null;
        IClientFrame m_manorFrame = null;
        IClientFrame m_storageFrame = null;
        IClientFrame m_shopFrame = null;
        IClientFrame m_crossManorFrame = null;
        IClientFrame m_guildRedPackFrame = null;
        object m_buildingParam = null;

        private Toggle Emblem = null;
        private Image EmblemRedPoint = null;

        private Toggle Benefits = null;
        private Toggle Activity = null;
        private GameObject activityRedPoint = null;

        static bool ms_bFuncsInited = false;
        static OperateFunc[] ms_arrOperateFuncs = new OperateFunc[(int)EOperateType.Count];

        private float openShopFrameTime = 0.0f; // 用来限制商店标签按钮点击频率
        private const float openShopFrameCD = 1.0f;

        public static void OpenLinkFrame(string a_strParam)
        {
            try
            {
                string[] arrParams = a_strParam.Split('|');
                if (arrParams.Length > 0)
                {
                    #region init operate func
                    if (ms_bFuncsInited == false)
                    {
                        #region OpenBaseInfo
                        ms_arrOperateFuncs[(int)EOperateType.OpenBaseInfo] = (object[] a_arrParams) =>
                        {
                            if (GuildDataManager.GetInstance().HasSelfGuild())
                            {
                                ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                            }
                            else
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                            }
                        };
                        #endregion

                        #region OpenMembers
                        ms_arrOperateFuncs[(int)EOperateType.OpenMembers] = (object[] a_arrParams) =>
                        {
                            if (GuildDataManager.GetInstance().HasSelfGuild())
                            {
                                ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenMemberFrame);
                            }
                            else
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                            }
                        };
                        #endregion

                        #region OpenBuilding
                        ms_arrOperateFuncs[(int)EOperateType.OpenBuilding] = (object[] a_arrParams) =>
                        {
                            if (GuildDataManager.GetInstance().HasSelfGuild())
                            {
                                ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenBuildingFrame);
                            }
                            else
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                            }
                        };
                        #endregion

                        #region OpenBuildingTable
                        //ms_arrOperateFuncs[(int)EOperateType.OpenBuildingTable] = (object[] a_arrParams) =>
                        //{
                        //    if (GuildDataManager.GetInstance().HasSelfGuild())
                        //    {
                        //        ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                        //        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenBuildingFrame, GuildBuildingType.TABLE);
                        //    }
                        //    else
                        //    {
                        //        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                        //    }
                        //};
                        #endregion

                        #region OpenBuildingWelfare
                        //ms_arrOperateFuncs[(int)EOperateType.OpenBuildingWelfare] = (object[] a_arrParams) =>
                        //{
                        //    if (GuildDataManager.GetInstance().HasSelfGuild())
                        //    {
                        //        ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                        //        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenBuildingFrame, GuildBuildingType.WELFARE);
                        //    }
                        //    else
                        //    {
                        //        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                        //    }
                        //};
                        #endregion

                        #region OpenBuildingStatue
                        //ms_arrOperateFuncs[(int)EOperateType.OpenBuildingStatue] = (object[] a_arrParams) =>
                        //{
                        //    if (GuildDataManager.GetInstance().HasSelfGuild())
                        //    {
                        //        ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                        //        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenBuildingFrame, GuildBuildingType.STATUE);
                        //    }
                        //    else
                        //    {
                        //        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                        //    }
                        //};
                        #endregion

                        #region OpenBuildingSkill
                        //ms_arrOperateFuncs[(int)EOperateType.OpenBuildingSkill] = (object[] a_arrParams) =>
                        //{
                        //    if (GuildDataManager.GetInstance().HasSelfGuild())
                        //    {
                        //        ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                        //        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenBuildingFrame, GuildBuildingType.BATTLE);
                        //    }
                        //    else
                        //    {
                        //        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                        //    }
                        //};
                        #endregion

                        #region OpenBuildingShop
                        ms_arrOperateFuncs[(int)EOperateType.OpenBuildingShop] = (object[] a_arrParams) =>
                        {
                            if (GuildDataManager.GetInstance().HasSelfGuild())
                            {
                                //                                 int nShopLevel = GuildDataManager.GetInstance().myGuild.dictBuildings[Protocol.GuildBuildingType.SHOP].nLevel;
                                //                                 ProtoTable.GuildBuildingTable buildingTable = TableManager.GetInstance().GetTableItem<ProtoTable.GuildBuildingTable>(nShopLevel);
                                //                                 if (buildingTable != null)
                                //                                 {
                                //                                     ShopDataManager.GetInstance().OpenShop(buildingTable.ShopId, 0, -1, () =>
                                //                                     {
                                //                                         ClientSystemManager.instance.OpenFrame<GuildMyMainFrame>(GameClient.FrameLayer.Middle);
                                //                                         UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenBuildingFrame);
                                //                                     });
                                //                                     RedPointDataManager.GetInstance().ClearRedPoint(ERedPoint.GuildShop);
                                //                                 }
                                ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenShopFrame);
                            }
                            else
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                            }
                        };
                        #endregion

                        #region GuildStoreFrame
                        ms_arrOperateFuncs[(int)EOperateType.OpenGuildStore] = (object[] a_arrParams) =>
                        {
                            if (GuildDataManager.GetInstance().HasSelfGuild())
                            {
                                ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenStorageFrame);
                            }
                            else
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                            }
                        };
                        #endregion

                        #region OpenManor
                        ms_arrOperateFuncs[(int)EOperateType.OpenManor] = (object[] a_arrParams) =>
                        {
                            if (GuildDataManager.GetInstance().HasSelfGuild())
                            {
                                ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenManorFrame);
                            }
                            else
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                            }
                        };
                        #endregion

                        #region OpenCrossManor
                        ms_arrOperateFuncs[(int)EOperateType.OpenGuildCrossManor] = (object[] a_arrParams) =>
                        {
                            if (GuildDataManager.GetInstance().HasSelfGuild())
                            {
                                ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenCrossManorFrame);
                            }
                            else
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                            }
                        };
                        #endregion                        

                        #region OpenGuildRedPacket
                        ms_arrOperateFuncs[(int)EOperateType.OpenGuildRedPacket] = (object[] a_arrParams) =>
                        {
                            if (GuildDataManager.GetInstance().HasSelfGuild())
                            {
                                ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenRedPacketFrame);
                            }
                            else
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                            }
                        };
                        #endregion

                        #region OpenGuildEmblemLevel
                        ms_arrOperateFuncs[(int)EOperateType.OpenGuildEmblemLevel] = (object[] a_arrParams) =>
                        {
                            if (GuildDataManager.GetInstance().HasSelfGuild())
                            {
                                ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnOpenGuildEmblemLevelPage);
                            }
                            else
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                            }
                        };
                        #endregion

                        #region OpenGuildBenefits
                        ms_arrOperateFuncs[(int)EOperateType.OpenGuildBenefits] = (object[] a_arrParams) =>
                        {
                            if (GuildDataManager.GetInstance().HasSelfGuild())
                            {
                                ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnOpenGuildBenefitsPage);
                            }
                            else
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                            }
                        };
                        #endregion
                        #region OpenGuildBenefits
                        ms_arrOperateFuncs[(int)EOperateType.OpenGuildActivity] = (object[] a_arrParams) =>
                        {
                            if (GuildDataManager.GetInstance().HasSelfGuild())
                            {
                                ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>(FrameLayer.Middle);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnOpenGuildActivityPage);
                            }
                            else
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                            }
                        };
                        #endregion

                        ms_bFuncsInited = true;
                    }
                    #endregion

                    int nType = int.Parse(arrParams[0]);
                    if (nType >= 0 && nType < ms_arrOperateFuncs.Length)
                    {
                        OperateFunc func = ms_arrOperateFuncs[nType];
                        if (func != null)
                        {
                            func.Invoke(arrParams);
                        }
                        else
                        {
                            Logger.LogErrorFormat("跳转到公会，参数错误， 类型：{0}", nType);
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("跳转到公会，参数错误， 类型：{0}", nType);
                    }
                }
            }
            catch (System.Exception e)
            {
                Logger.LogError("PackageFrame.OpenLinkFrame : ==>" + e.ToString());
            }
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildMyMain";
        }

        protected sealed override void _OnOpenFrame()
        {
            _InitTabs();
            _RegisterUIEvent();

            _UpdateCrossGuildBattleTab();

            openShopFrameTime = 0.0f;
        }

        protected sealed override void _OnCloseFrame()
        {
            m_bToggleBlockSignal = false;
            _UnRegisterUIEvent();
            _CloseAll();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildMainFrameClose);

            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildMain);

            openShopFrameTime = 0.0f;
        }

        protected override void _bindExUI()
        {
            Emblem = mBind.GetCom<Toggle>("Emblem");
            Emblem.SafeAddOnValueChangedListener((bValue) => 
            {
                if(bValue)
                {                   
                    frameMgr.OpenFrame<GuildEmblemUpFrame>(m_objContentRoot);
                    EmblemRedPoint.CustomActive(false);
                }
                else
                {
                    frameMgr.CloseFrame<GuildEmblemUpFrame>();
                }
            });

            EmblemRedPoint = mBind.GetCom<Image>("EmblemRedPoint");

            Benefits = mBind.GetCom<Toggle>("Benefits");
            Benefits.SafeSetOnValueChangedListener((bValue) =>
            {
                if (bValue)
                {
                    frameMgr.OpenFrame<GuildBenefitsFrame>(m_objContentRoot);            
                }
                else
                {
                    frameMgr.CloseFrame<GuildBenefitsFrame>();
                }
            });

            Activity = mBind.GetCom<Toggle>("Activity");
            Activity.SafeSetOnValueChangedListener((bValue) => 
            {
                if (bValue)
                {
                    frameMgr.OpenFrame<GuildActivityFrame>(m_objContentRoot);
                }
                else
                {
                    frameMgr.CloseFrame<GuildActivityFrame>();
                }
            });

            activityRedPoint = mBind.GetGameObject("activityRedPoint");
        }

        protected override void _unbindExUI()
        {
            Emblem = null;
            EmblemRedPoint = null;

            Benefits = null;
            Activity = null;
            activityRedPoint = null;
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildCloseMainFrame, _OnCloseMainFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildOpenMemberFrame, _OnOpenMemberFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildOpenBuildingFrame, _OnOpenBuildingFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildOpenShopFrame, _OnOpenShopFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildOpenManorFrame, _OnOpenManorFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnOpenGuildHouseMain, _OnOpenGuildHouseMain);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildOpenStorageFrame, _OnGuildOpenStorageFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildOpenCrossManorFrame, _OnOpenCrossManorFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildEmblemLevelUp, _OnGuildEmblemLevelUp);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnOpenGuildEmblemLevelPage, _OnOpenGuildEmblemLevelPage);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildOpenShopRefreshConsumeItem, _OnGuildOpenShopRefreshConsumeItem);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnOpenGuildBenefitsPage, _OnOpenGuildBenefitsPage);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnOpenGuildActivityPage, _OnOpenGuildActivityPage);
            ServerSceneFuncSwitchManager.GetInstance().AddServerFuncSwitchListener(ServiceType.SERVICE_GUILD_CROSS_BATTLE, _OnCrossBattleStateSwitch);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildCloseMainFrame, _OnCloseMainFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildOpenMemberFrame, _OnOpenMemberFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildOpenBuildingFrame, _OnOpenBuildingFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildOpenShopFrame, _OnOpenShopFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildOpenManorFrame, _OnOpenManorFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnOpenGuildHouseMain, _OnOpenGuildHouseMain);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildOpenStorageFrame, _OnGuildOpenStorageFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildOpenCrossManorFrame, _OnOpenCrossManorFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildEmblemLevelUp, _OnGuildEmblemLevelUp);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnOpenGuildEmblemLevelPage, _OnOpenGuildEmblemLevelPage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildOpenShopRefreshConsumeItem, _OnGuildOpenShopRefreshConsumeItem);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnOpenGuildBenefitsPage, _OnOpenGuildBenefitsPage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnOpenGuildActivityPage, _OnOpenGuildActivityPage);

            ServerSceneFuncSwitchManager.GetInstance().RemoveServerFuncSwitchListener(ServiceType.SERVICE_GUILD_CROSS_BATTLE, _OnCrossBattleStateSwitch);
        }        

        void _InitTabs()
        {
            m_toggleBase.onValueChanged.RemoveAllListeners();
            m_toggleBase.onValueChanged.AddListener((bool a_bChecked) =>
            {
                if (m_bToggleBlockSignal == false)
                {
                    if (a_bChecked)
                    {
                        m_baseFrame = frameMgr.OpenFrame<GuildMyBaseFrame>(m_objContentRoot);
                    }
                    else
                    {
                        _CloseBasePage();
                    }
                }
            });

            m_toggleMember.onValueChanged.RemoveAllListeners();
            m_toggleMember.onValueChanged.AddListener((bool a_bChecked) =>
            {
                if (m_bToggleBlockSignal == false)
                {
                    if (a_bChecked)
                    {
                        OpenGuildMemberFrameData data = new OpenGuildMemberFrameData();
                        m_memberFrame = ClientSystemManager.GetInstance().OpenFrame<GuildMemberFrame>(m_objContentRoot, data);
                   
                    }
                    else
                    {
                        _CloseMemberPage();
                    }
                }
            });

            m_toggleBuilding.onValueChanged.RemoveAllListeners();
            m_toggleBuilding.onValueChanged.AddListener((bool a_bChecked) =>
            {
                if (m_bToggleBlockSignal == false)
                {
                    if (a_bChecked)
                    {
                        if (m_buildingFrame != null)
                        {
                            frameMgr.CloseFrame(m_buildingFrame);
                        }
                        m_buildingFrame = frameMgr.OpenFrame<GuildBuildingFrame>(m_objContentRoot, m_buildingParam);

                        m_objBuildingRedPoint.CustomActive(false);
                    }
                    else
                    {
                        _CloseBuildingPage();
                    }
                }
            });

            m_toggleManor.onValueChanged.RemoveAllListeners();
            m_toggleManor.onValueChanged.AddListener((bool a_bChecked) =>
            {
                if (m_bToggleBlockSignal == false)
                {
                    if (a_bChecked)
                    {
                        //m_manorFrame = frameMgr.OpenFrame<GuildManorFrame>(m_objContentRoot);
                        frameMgr.OpenFrame<GuildManorFrame>(FrameLayer.Middle);
                        NewMessageNoticeManager.GetInstance().RemoveNewMessageNoticeByTag("GuildBattle");
                    }
                    else
                    {
                        _CloseManorPage();
                    }
                }
            });

            m_toggleShop.onValueChanged.RemoveAllListeners();
            m_toggleShop.onValueChanged.AddListener((bool a_bChecked) =>
            {
                if (m_bToggleBlockSignal == false)
                {
                    if (a_bChecked)
                    {
                        if(openShopFrameTime > 0.0f && Time.realtimeSinceStartup - openShopFrameTime <= openShopFrameCD)
                        {
                            return;
                        }
                        openShopFrameTime = Time.realtimeSinceStartup;

                        if (m_shopFrame != null)
                        {
                            frameMgr.CloseFrame(m_shopFrame);
                        }
                        m_shopFrame = frameMgr.OpenFrame<GuildShopFrame>(m_objContentRoot);
                    }
                    else
                    {
                        _CloseShopPage();
                    }
                }
            });

            m_toggleStorage.onValueChanged.RemoveAllListeners();
            m_toggleStorage.onValueChanged.AddListener(_OnStorageToggleChanged);

            m_toggleCrossManor.onValueChanged.RemoveAllListeners();
            m_toggleCrossManor.onValueChanged.AddListener((bool a_bChecked) =>
            {
                if (m_bToggleBlockSignal == false)
                {
                    if (a_bChecked)
                    {
                        //m_crossManorFrame = frameMgr.OpenFrame<GuildCrossManorFrame>(m_objContentRoot);
                        frameMgr.OpenFrame<GuildCrossManorFrame>(FrameLayer.Middle);
                         //NewMessageNoticeManager.GetInstance().RemoveNewMessageNoticeByTag("GuildBattle");
                    }
                    else
                    {
                        _CloseCrossManorPage();
                    }
                }
            });


            m_bToggleBlockSignal = true;
            m_toggleBase.isOn = false;
            m_toggleMember.isOn = true;
            m_toggleBuilding.isOn = true;
            m_toggleManor.isOn = true;
            m_toggleStorage.isOn = true;
            m_toggleShop.isOn = true;
            m_toggleCrossManor.isOn = true;

            m_bToggleBlockSignal = false;
            m_toggleBase.isOn = true;

            _UpdateRedPoint();
        }

        void _OnStorageToggleChanged(bool a_bChecked)
        {
            if (m_bToggleBlockSignal == false)
            {
                if (a_bChecked)
                {
                    GuildStoreFrame.ansyOpen(null);
                }
                else
                {
                    _CloseStoragePage();
                }
            }
        }

        void _OpenBasePage()
        {
            if (m_toggleBase.isOn == false)
            {
                m_toggleBase.isOn = true;
            }
        }

        void _OpenMemberPage()
        {
            if (m_toggleMember.isOn == false)
            {
                m_toggleMember.isOn = true;
            }
        }

        void _OpenBuildingPage(object a_param)
        {
            m_buildingParam = a_param;
            if (m_toggleBuilding.isOn == false)
            {
                m_toggleBuilding.isOn = true;
            }
            else
            {
                m_toggleBuilding.onValueChanged.Invoke(true);
            }
        }

        void _OpenShopPage()
        {
            if (m_toggleShop.isOn == false)
            {
                m_toggleShop.isOn = true;
            }
        }

        void _OpenManorPage()
        {
            if (Activity.isOn == false)
            {
                Activity.isOn = true;
            }
        }

        void _OpenCrossManorPage()
        {
            if (Activity.isOn == false)
            {
                Activity.isOn = true;
            }
        }

        void _OpenGuildEmblemLevelPage()
        {
            if(Emblem != null)
            {
                if (Emblem.isOn == false)
                {
                    Emblem.isOn = true;
                }
            }            
        }

        void _OpenGuildBenefitsPage()
        {
            if(Benefits != null)
            {
                if (Benefits.isOn == false)
                {
                    Benefits.isOn = true;
                }
            }            
        }

        void _OpenGuildActivityPage()
        {
            if(Activity != null)
            {
                if (Activity.isOn == false)
                {
                    Activity.isOn = true;
                }
            }            
        }      
        void _OpenGuildRedPacketPage()
        {
            if (m_toggleRedPacket.isOn == false)
            {
                m_toggleRedPacket.isOn = true;
            }
        }

        void _CloseBasePage()
        {
            if (m_baseFrame != null)
            {
                frameMgr.CloseFrame(m_baseFrame);
                m_baseFrame = null;
            }
        }

        void _CloseMemberPage()
        {
            if (m_memberFrame != null)
            {
                frameMgr.CloseFrame(m_memberFrame);
                m_memberFrame = null;
            }
        }

        void _CloseBuildingPage()
        {
            if (m_buildingFrame != null)
            {
                frameMgr.CloseFrame(m_buildingFrame);
                m_buildingFrame = null;
                m_buildingParam = null;
            }
        }

        void _CloseManorPage()
        {
            if (m_manorFrame != null)
            {
                frameMgr.CloseFrame(m_manorFrame);
                m_manorFrame = null;
            }
        }

        void _CloseShopPage()
        {
            if (m_shopFrame != null)
            {
                frameMgr.CloseFrame(m_shopFrame);
                m_shopFrame = null;
                if(m_comConsumeItemGroup != null)
                {
                    m_comConsumeItemGroup.ResetOriginalItemIdsWithShow();
                }  
            }
        }

        void _CloseStoragePage()
        {
            if (m_storageFrame != null)
            {
                frameMgr.CloseFrame(m_storageFrame);
                m_storageFrame = null;
            }
        }

        void _CloseCrossManorPage()
        {
            if (m_crossManorFrame != null)
            {
                frameMgr.CloseFrame(m_crossManorFrame);
                m_crossManorFrame = null;
            }
        }


        void _CloseAll()
        {
            _CloseBasePage();
            _CloseMemberPage();
            _CloseBuildingPage();
            _CloseManorPage();
            _CloseShopPage();
            _CloseStoragePage();
            _CloseCrossManorPage();
            frameMgr.CloseFrame<GuildEmblemUpFrame>();
            frameMgr.CloseFrame<GuildBenefitsFrame>();
            frameMgr.CloseFrame<GuildActivityFrame>();
            frameMgr.CloseFrame<GuildCrossManorFrame>();
            frameMgr.CloseFrame<GuildManorFrame>();
            frameMgr.CloseFrame(this);
        }

        void _OnCloseMainFrame(UIEvent a_event)
        {
            _CloseAll();
        }

        void _OnOpenMemberFrame(UIEvent a_event)
        {
            _OpenMemberPage();
        }

        void _OnOpenBuildingFrame(UIEvent a_event)
        {
            _OpenBuildingPage(a_event.Param1);
        }

        void _OnOpenShopFrame(UIEvent a_event)
        {
            _OpenShopPage();
        }

        void _OnOpenManorFrame(UIEvent a_event)
        {
            _OpenManorPage();
        }

        void _OnGuildDismissed(UIEvent a_event)
        {
            frameMgr.CloseFrame(this);
        }

        void _UpdateRedPoint()
        {
            m_objBaseRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildBase));
            m_objBuildingRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildBuilding));
            m_objMemberRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildMember));
            m_objManorRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildManor));
            m_objShopRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildShop));
            m_objCrossManorRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildCrossManor));

            activityRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildManor) 
                || RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildCrossManor)
                || RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildTerrDayReward));            
        }

        void _OnRedPointChanged(UIEvent a_event)
        {
            _UpdateRedPoint();
        }

        void _OnGuildOpenStorageFrame(UIEvent a_event)
        {
            if (m_toggleStorage.isOn == false)
            {
                m_toggleStorage.isOn = true;
            }
        }

        void _OnOpenGuildHouseMain(UIEvent a_event)
        {
            if (m_storageFrame != null)
            {
                frameMgr.CloseFrame(m_storageFrame);
                m_storageFrame = null;
            }
            m_storageFrame = ClientSystemManager.GetInstance().OpenFrame<GuildStoreFrame>(m_objContentRoot, a_event.Param1);
        }

        void _OnOpenCrossManorFrame(UIEvent a_event)
        {
            _OpenCrossManorPage();
        }

        void _OnGuildEmblemLevelUp(UIEvent a_event)
        {

        }

        void _OnOpenGuildEmblemLevelPage(UIEvent a_event)
        {
            _OpenGuildEmblemLevelPage();
        }

        void _OnOpenGuildBenefitsPage(UIEvent a_event)
        {
            _OpenGuildBenefitsPage();
        }

        void _OnOpenGuildActivityPage(UIEvent a_event)
        {
            _OpenGuildActivityPage();
        }
		
		void _OnCrossBattleStateSwitch(ServerSceneFuncSwitch fSwitch)
        {
            if (fSwitch.sType != ServiceType.SERVICE_GUILD_CROSS_BATTLE)
            {
                return;
            }

            _UpdateCrossGuildBattleTab();
        }

        void _OnGuildOpenShopRefreshConsumeItem(UIEvent a_event)
        {
            if(a_event == null)
            {
                return;
            }
            var shopConsumeIds = a_event.Param1 as List<int>;
            if(shopConsumeIds == null || shopConsumeIds.Count <= 0)
            {
                return;
            }
            if(m_comConsumeItemGroup == null)
            {
                return; 
            }  
            m_comConsumeItemGroup.ResetSelectedItemIds(shopConsumeIds.ToArray(), false, true);
        }


        [UIEventHandle("RedEnvelope")]
        void _OnRedPacketClicked()
        {
            //frameMgr.OpenFrame<GuildRedPackFrame>(FrameLayer.Middle);
        }

        [UIEventHandle("BG/Title/Close")]
        void _OnCloseCliecked()
        {
            _CloseAll();
        }

        void _UpdateCrossGuildBattleTab()
        {
            return;

            m_toggleCrossManor.gameObject.CustomActive(!ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_GUILD_CROSS_BATTLE));

            if(ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_GUILD_CROSS_BATTLE))
            {
                ClientSystemManager.GetInstance().CloseFrame<GuildCrossManorFrame>();
                ClientSystemManager.GetInstance().CloseFrame<GuildCrossManorInfoFrame>();

                if(m_toggleCrossManor.isOn)
                {
                    OpenLinkFrame("0");
                }
            }
        }
    }
}
