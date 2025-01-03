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
    public class GuildMyRedPacketRecordFrame : ClientFrame
    {
        #region inner def
        public class GuildMyRedPackRecordData
        {

        }
        #endregion

        #region val    

        List<GuildMyRedPackRecordData> dataList = null;
        #endregion

        #region ui bind
        private ComUIListScript itemList = null;
        private ScrollRect recordItems = null;
        private Text recordContent = null;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildMyRedPacketRecord";
        }

        protected override void _OnOpenFrame()
        {
            BindUIEvent();

            dataList = new List<GuildMyRedPackRecordData>();
            UpdateItemList();

            UpdateRecords();
        }

        protected override void _OnCloseFrame()
        {
            dataList = null;
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            itemList = mBind.GetCom<ComUIListScript>("itemList");
            recordItems = mBind.GetCom<ScrollRect>("recordItems");
            recordContent = mBind.GetCom<Text>("recordContent");
        }

        protected override void _unbindExUI()
        {
            itemList = null;
            recordItems = null;
            recordContent = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            
        }

        void UnBindUIEvent()
        {
           
        }

        void UpdateListItem(ComUIListElementScript item)
        {

        }

        void UpdateItemList()
        {
            if(itemList == null)
            {
                return;
            }

            if(dataList == null)
            {
                return;
            }

            itemList.Initialize();
            itemList.onBindItem = (item) => 
            {
                return null;
            };

            itemList.onItemVisiable = (item) => 
            {
                UpdateListItem(item);
            };

            itemList.OnItemUpdate = (item) => 
            {
                UpdateListItem(item);
            };

            itemList.UpdateElementAmount(dataList.Count);
        }

        string GetMoneyName(int moneyID)
        {
            ItemTable itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(moneyID);
            if(itemTable != null)
            {
                return itemTable.Name;
            }

            return "";
        }

        void UpdateRecords()
        {
            if(recordContent == null)
            {
                return;
            }

            string content = "";
            var dicts = RedPackDataManager.GetInstance().GetRedPacketRecords();
            if(dicts == null)
            {
                return;
            }

            List<RedPacketRecord> redPacketRecords = new List<RedPacketRecord>();
            if(redPacketRecords == null)
            {
                return;
            }

            var iter = dicts.GetEnumerator();
            while (iter.MoveNext())
            {
                RedPacketRecord adt = iter.Current.Value as RedPacketRecord;
                if (adt == null)
                {
                    continue;
                }

                redPacketRecords.Add(adt);                
            }

            redPacketRecords.Sort((a, b) => 
            {
                return b.time.CompareTo(a.time);
            });

            for(int i = 0;i< redPacketRecords.Count;i++)
            {
                RedPacketRecord redPacketRecord = redPacketRecords[i];
                if (redPacketRecord == null)
                {
                    continue;
                }

                DateTime dateTime = TimeUtility.GetDateTimeByTimeStamp((int)redPacketRecord.time);
                content += string.Format(TR.Value("guild_red_packet_record_time"), dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute);
                content += "\n";

                if (redPacketRecord.isBest == 0)
                {
                    content += TR.Value("guild_red_packet_record_info", redPacketRecord.redPackOwnerName, redPacketRecord.moneyNum, GetMoneyName((int)redPacketRecord.moneyId));
                }
                else
                {
                    content += TR.Value("guild_red_packet_record_best", redPacketRecord.redPackOwnerName, redPacketRecord.moneyNum, GetMoneyName((int)redPacketRecord.moneyId));
                }
                content += "\n";
            }

            recordContent.SafeSetText(content);
        }  
        #endregion

        #region ui event       

        #endregion
    }
}
