using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using System;
///////删除linq

namespace GameClient
{
    class DevelopGuidanceMainFrameData
    {
        public ProtoTable.GuidanceMainTable mainItem;
    }
    class DevelopGuidanceMainFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/DevelopGuidanceFrame/DevelopGuidanceMainFrame";
        }

        DevelopGuidanceMainFrameData data = null;
        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                int entranceId = 0;
                if(int.TryParse(strParam,out entranceId))
                {
                    var guidanceMainItem = TableManager.GetInstance().GetTableItem<ProtoTable.GuidanceMainTable>(entranceId);
                    if(guidanceMainItem != null)
                    {
                        DevelopGuidanceMainFrameData data = new DevelopGuidanceMainFrameData();
                        data.mainItem = guidanceMainItem;
                        ClientSystemManager.GetInstance().CloseFrame<DevelopGuidanceMainFrame>();
                        ClientSystemManager.GetInstance().OpenFrame<DevelopGuidanceMainFrame>(FrameLayer.Middle,data);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        protected override void _OnOpenFrame()
        {
            data = userData as DevelopGuidanceMainFrameData;
            m_kComGuidanceMainItems.Initialize(this, Utility.FindChild(frame, "GuidanceMainItems"));
            _InitMainItemTabs();
        }

        bool _CheckFunctionUnlocked(int iMainItemId)
        {
            var mainItem = TableManager.GetInstance().GetTableItem<ProtoTable.GuidanceMainTable>(iMainItemId);
            if(mainItem == null)
            {
                return false;
            }

            if(PlayerBaseData.GetInstance().Level < mainItem.UnLockLevel)
            {
                return false;
            }

            return true;
        }

        void _InitMainItemTabs()
        {
            GameObject goParent = Utility.FindChild(frame, "MainTabBG/ScrollView/ViewPort/MainTab");
            GameObject goPrefab = Utility.FindChild(frame, "MainTabBG/ScrollView/ViewPort/MainTab/Prefab");
            goPrefab.CustomActive(false);

            ScrollRect scrollRect = Utility.FindComponent<ScrollRect>(frame, "GuidanceMainItems/ScrollView");
            scrollRect.verticalScrollbar.value = 1.0f;

            var delTabSelected = Delegate.CreateDelegate(typeof(DevelopGuidanceTab.OnSelectedDelegate), this, "OnTabSelected");
            var mainItems = TableManager.GetInstance().GetTable<ProtoTable.GuidanceMainTable>().Values.ToList();
            for(int i = 0; i < mainItems.Count; ++i)
            {
                if(!_CheckFunctionUnlocked((mainItems[i] as ProtoTable.GuidanceMainTable).ID))
                {
                    continue;
                }
                m_akDevelopGuidanceTabs.Create(new object[] { goParent, goPrefab,new TabData { mainItem = mainItems[i] as ProtoTable.GuidanceMainTable }, delTabSelected });
            }

            if(data != null)
            {
                var find = m_akDevelopGuidanceTabs.ActiveObjects.Find(x =>
                {
                    return x.Value.mainItem.ID == data.mainItem.ID;
                });

                if(find != null)
                {
                    find.OnSelected();
                }
                else
                {
                    if (m_akDevelopGuidanceTabs.ActiveObjects.Count > 0)
                    {
                        m_akDevelopGuidanceTabs.ActiveObjects[0].OnSelected();
                    }
                }
            }
            else
            {
                if(m_akDevelopGuidanceTabs.ActiveObjects.Count > 0)
                {
                    m_akDevelopGuidanceTabs.ActiveObjects[0].OnSelected();
                }
            }
        }

        void OnTabSelected(TabData data)
        {
            if (data != null)
            {
                m_kComGuidanceMainItems.RefreshDatas(data.mainItem.ID);
            }
        }

        protected override void _OnCloseFrame()
        {
            DevelopGuidanceTab.Clear();
            m_akDevelopGuidanceTabs.DestroyAllObjects();
            data = null;

            m_kComGuidanceMainItems.UnInitialize();
        }

        CachedObjectListManager<DevelopGuidanceTab> m_akDevelopGuidanceTabs = new CachedObjectListManager<DevelopGuidanceTab>();
        GuidanceMainItemList m_kComGuidanceMainItems = new GuidanceMainItemList();

        [UIEventHandle("Title/Close")]
        void OnClickCloseFrame()
        {
            frameMgr.CloseFrame(this);
        }
    }
}