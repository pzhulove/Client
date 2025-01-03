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
    public class JoinGuildDungeonActivityFrame : ClientFrame
    {     

        [UIControl("Join")]
        Button m_btnJoin;

        [UIControl("Close")]
        Button m_btnClose;
      
        const int ITEM_NUM = 4;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/JoinGuildDungeonActivity";
        }

        protected override void _OnOpenFrame()
        {
           if(m_btnJoin != null)
            {
                m_btnJoin.onClick.RemoveAllListeners();
                m_btnJoin.onClick.AddListener(() => 
                {
                    //ClientSystemManager.GetInstance().OpenFrame<GuildMyMainFrame>();
                    GuildDataManager.GetInstance().SwitchToGuildScene();
                    frameMgr.CloseFrame(this);
                });
            }

            if (m_btnClose != null)
            {
                m_btnClose.onClick.RemoveAllListeners();
                m_btnClose.onClick.AddListener(() =>
                {
                    frameMgr.CloseFrame(this);
                });              
            }

            UpdateAwardItems();          
        }

        protected override void _OnCloseFrame()
        {
          
        }

        public override bool IsNeedUpdate()
        {
            return false;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
           
        }

        private void UpdateAwardItems()
        {
            int iTableID = 0;
            Dictionary<int, object> dicts = TableManager.instance.GetTable<ActivityDungeonTable>();
            if (dicts == null)
            {
                Logger.LogErrorFormat("TableManager.instance.GetTable<ActivityDungeonTable>() error!!!");
                return;
            }

            var iter = dicts.GetEnumerator();
            while (iter.MoveNext())
            {
                ActivityDungeonTable adt = iter.Current.Value as ActivityDungeonTable;
                if (adt == null)
                {
                    continue;
                }

                if (adt.DungeonID == ActivityDungeonFrame.guildDungeonID)
                {
                    iTableID = adt.ID;
                    break;
                }
            }

            ActivityDungeonTable table = TableManager.GetInstance().GetTableItem<ActivityDungeonTable>(iTableID);
            if (table != null)
            {
                for (int i = 0; i < table.DropItems.Length; i++)
                {
                    if (i >= ITEM_NUM)
                    {
                        break;
                    }

                    int id = table.DropItems[i];
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(id);
                    if (itemData == null)
                    {
                        continue;
                    }                    

                    ComItem item = mBind.GetCom<ComItem>(string.Format("Item{0}", i));
                    if (item == null)
                    {
                        continue;
                    }

                    item.Setup(itemData, (var1, var2) =>
                    {
                        ItemTipManager.GetInstance().CloseAll();
                        ItemTipManager.GetInstance().ShowTip(var2);
                    });
                }
            }
        }
    }
}
