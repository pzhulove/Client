using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;

namespace GameClient
{
    // 公会建筑管理界面
    public class GuildBuildingManagerFrame : ClientFrame
    {
        #region inner define
        #endregion

        #region val
        List<GuildBuildingData> itemDataList = null;      

        #endregion

        #region ui bind
        ComUIListScript itemList = null;
        Button Close = null;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildBuildingManager";
        }

        protected override void _OnOpenFrame()
        {           
            itemDataList = null;           

            InitTitleAndHelp();
            InitItems();      
            UpdateItems();

            BindUIEvent();            
        }

        protected override void _OnCloseFrame()
        {            
            itemDataList = null;                 

            UnBindUIEvent();         
        }

        protected override void _bindExUI()
        {
            itemList = mBind.GetCom<ComUIListScript>("itemList");           

            Close = mBind.GetCom<Button>("Close");
            Close.SafeSetOnClickListener(() => 
            {
                frameMgr.CloseFrame(this);
            });
        }

        protected override void _unbindExUI()
        {
            itemList = null;          
            Close = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildUpgradeBuildingSuccess, _OnUpgradeBuildingSuccess);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildUpgradeBuildingSuccess, _OnUpgradeBuildingSuccess);
        }        

        void InitItems()
        {
            if(itemList == null)
            {
                return;
            }

            itemList.Initialize();

            itemList.onBindItem = (GameObject go) =>
            {
                GuildBuildingItem item = null;
                if (go)
                {
                    item = go.GetComponent<GuildBuildingItem>();
                }
               
                return item;
            };

            itemList.onItemVisiable = (var1) =>
            {
                if (var1 == null)
                {
                    return;
                }

                int iIndex = var1.m_index;
                if (iIndex >= 0 && itemDataList != null && iIndex < itemDataList.Count)
                {                   
                    GuildBuildingItem item = var1.gameObjectBindScript as GuildBuildingItem;
                    if (item != null)
                    {
                        item.SetUp(itemDataList[iIndex]);
                    }
                }
            };
        }

        void InitTitleAndHelp()
        {
            
        }

        void CalItemDataList()
        {
            itemDataList = null;

            GuildMyData guildMyData = GuildDataManager.GetInstance().myGuild;
            if(guildMyData != null && guildMyData.dictBuildings != null)
            {
                itemDataList = new List<GuildBuildingData>();

                var dicts = guildMyData.dictBuildings;
                if (dicts != null)
                {
                    var iter = dicts.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        GuildBuildingData data = iter.Current.Value as GuildBuildingData;
                        if (data == null)
                        {
                            continue;
                        }

                        GuildBuildInfoTable table = GuildDataManager.GetInstance().GetGuildBuildInfoTable(data.eType);
                        if(table == null)
                        {
                            continue;
                        }

                        if(table.isOpen)
                        {
                            itemDataList.Add(data);
                        }
                    }
                }
            }            

            return;
        }

        void UpdateItems()
        {
            if(itemList == null)
            {
                return;
            }

            CalItemDataList();

            if(itemDataList != null)
            {
                itemList.SetElementAmount(itemDataList.Count);                
            }
        }

        #endregion

        #region ui event
        void _OnUpgradeBuildingSuccess(UIEvent a_event)
        {
            UpdateItems();
        }

        #endregion
    }
}
