using UnityEngine;
using System.Collections;
using Protocol;
using Network;
using System.Collections.Generic;

namespace GameClient
{
    class LinkManager : DataManager<LinkManager>
    {
        #region delegate
        public delegate void OnAddLinkItem(ItemData itemData);

        public OnAddLinkItem onAddLinkItem;
        #endregion

        #region process
        public override void Initialize()
        {
            RegisterNetHandler();
        }

        public override void Clear()
        {
            UnRegisterNetHandler();
            m_kCurLinkItem = null;
        }

        void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(WorldChatLinkDataRet.MsgID, OnRecvWorldChatLinkDataRet);
            
        }

        void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(WorldChatLinkDataRet.MsgID, OnRecvWorldChatLinkDataRet);
         
        }
        #endregion

        #region LinkData
        ItemData m_kCurLinkItem;
        public ActorShowEquipData AttachDatas = null;

       // [MessageHandle(WorldChatLinkDataRet.MsgID)]
        void OnRecvWorldChatLinkDataRet(MsgDATA msg)
        {
            ChatLinkDecoder.Decode(msg);
        }

        void _OnAddLinkItem(ItemData itemData)
        {
            m_kCurLinkItem = itemData;
            if (onAddLinkItem != null)
            {
                onAddLinkItem(itemData);
            }
            TipFuncButon tempFunc = null;
            List<TipFuncButon> funcs = new List<TipFuncButon>();
            var dataItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)itemData.TableID);
            if (dataItem != null && dataItem.ComeLink != null && dataItem.ComeLink.Count > 0)
            {
                tempFunc = new TipFuncButon
                {
                    text = TR.Value("tip_try_get_item"),
                    callback = _OnTryGetItem
                };
                funcs.Add(tempFunc);
            }
            if (null == AttachDatas)
            {
                var emptySuit = EquipSuitDataManager.GetInstance().CreateEmptyEquipSuitObj(itemData.SuitID);
                if(funcs.Count > 0)
                {
                    ItemTipManager.GetInstance().ShowOtherPlayerTip(m_kCurLinkItem, emptySuit, funcs);
                }
                else
                {
                    ItemTipManager.GetInstance().ShowOtherPlayerTip(m_kCurLinkItem, emptySuit);
                }
            }
            else
            {
                EquipSuitObj suitObj = null;
                AttachDatas.m_dictEquipSuitObjs.TryGetValue(m_kCurLinkItem.SuitID, out suitObj);
                if(funcs.Count > 0)
                {
                    ItemTipManager.GetInstance().ShowOtherPlayerTip(m_kCurLinkItem, suitObj, funcs);
                }
                else
                {
                    ItemTipManager.GetInstance().ShowOtherPlayerTip(m_kCurLinkItem, suitObj);
                }

                AttachDatas = null;
            }
        }
        void _OnTryGetItem(ItemData item, object data)
        {
            if (item != null)
            {
                var dataItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)item.TableID);
                ItemComeLink.OnLink(item.TableID, 0, false, null, false, dataItem.bNeedJump > 0);
                ItemTipManager.GetInstance().CloseAll();
            }
        }

        class ItemLinkData
        {
            public byte type;
            public List<ItemData> itemDatas;
        }

        class ChatLinkDecoder
        {
            public static byte Decode(MsgDATA msgData)
            {
                int iPos = 0;
                byte type = 0;
                BaseDLL.decode_int8(msgData.bytes, ref iPos, ref type);
                switch (type)
                {
                    case (byte)'I':
                        DecodeItemLink(iPos, msgData);
                        break;
                    case (byte)'R':
                        DecodeRetinueLink(iPos, msgData);
                        break;

                }

                return 0;
            }

            static void DecodeItemLink(int iPos, MsgDATA msgData)
            {
                var items = ItemDecoder.Decode(msgData.bytes, ref iPos, msgData.bytes.Length);
                if (items != null && items.Count > 0)
                {
                    var itemData = ItemDataManager.GetInstance().CreateItemDataFromNet(items[0]);
                    if (itemData != null)
                    {
                        LinkManager.GetInstance()._OnAddLinkItem(itemData);
                    }
                }
            }

            static void DecodeRetinueLink(int iPos, MsgDATA msgData)
            {
                RetinueInfo retinueInfo = new RetinueInfo();
                retinueInfo.decode(msgData.bytes, ref iPos);
            }

        }
        #endregion
    }
}