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
    // 公会boss难度设置界面
    public class GuildDungeonBossDiffSetFrame : ClientFrame
    {
        #region inner define
      
        #endregion

        #region val
        List<int> itemDataList = null;
        #endregion

        #region ui bind
        ComUIListScript itemList = null;       
        Button Close = null;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildDungeonBossDiffSet";
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
            Close.SafeRemoveAllListener();
            Close.SafeAddOnClickListener(() => 
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
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonSetBossDiff, _OnUpdateItems);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonSetBossDiff, _OnUpdateItems);
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
                GuildDungeonBossDiffSetItem item = null;
                if (go)
                {
                    item = go.GetComponent<GuildDungeonBossDiffSetItem>();
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
                    GuildDungeonBossDiffSetItem item = var1.gameObjectBindScript as GuildDungeonBossDiffSetItem;
                    if (item != null)
                    {
                        item.SetUp(itemDataList[iIndex],this);
                    }
                }
            };
        }

        void InitTitleAndHelp()
        {

        }

        void CalItemDataList()
        {   
            itemDataList = new List<int>();
            if(itemDataList == null)
            {
                return;
            }

            Dictionary<int, object> dicts = TableManager.instance.GetTable<GuildDungeonTypeTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    GuildDungeonTypeTable adt = iter.Current.Value as GuildDungeonTypeTable;
                    if (adt == null)
                    {
                        continue;
                    }

                    itemDataList.Add(adt.dungeonType);
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
                itemList.SetElementAmount(0);
                itemList.SetElementAmount(itemDataList.Count);                
            }
        }

        #endregion

        #region ui event
        void _OnUpdateItems(UIEvent uiEvent)
        {
            UpdateItems();
            return;
        }

        #endregion
    }
}
