using UnityEngine;
using System.Collections;
using Protocol;
using Network;
using System.Collections.Generic;
///////删除linq
using System;

namespace GameClient
{
    public class MagicBoxDataManager : DataManager<MagicBoxDataManager>
    {
        public override void Clear()
        {
            itemRrewardList.Clear();
            itemDoubleRewardList.Clear();
        }

        public override void Initialize()
        {
           
        }
        #region 开魔盒请求
        public void SendCSOpenMagBoxReq(ulong itemUid, byte isBatch)
        {
            CSOpenMagBoxReq req = new CSOpenMagBoxReq();

            req.itemUid = itemUid;
            req.isBatch = isBatch;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

       public List<ItemData> itemRrewardList = new List<ItemData>();

       public List<ItemData> itemDoubleRewardList = new List<ItemData>();
        
        public void AnsyOpenMagBox(UnityEngine.Events.UnityAction callback, ulong item,int times)
        {
            if (times > 10)
            {
                times = 10;
            }

            if (times > 0)
            {
                --times;
                SendCSOpenMagBoxReq(item, 0);

                WaitNetMessageManager.GetInstance().Wait(SCOpenMagBoxRes.MsgID,(MsgDATA data) =>
                {
                    if (data == null)
                    {
                        PackageDataManager.GetInstance().ResetMagicBoxAndMagicHammer();
                        return;
                    }

                    SCOpenMagBoxRes msgRet = new SCOpenMagBoxRes();

                    int nPos = 0;
                    msgRet.decode(data.bytes, ref nPos);
                    //nPos++;
                    List<Item> items = ItemDecoder.Decode(data.bytes, ref nPos, data.bytes.Length);
                    if (items == null)
                    {
                        PackageDataManager.GetInstance().ResetMagicBoxAndMagicHammer();
                        return;
                    }

                    if (msgRet.retCode != (uint)ProtoErrorCode.SUCCESS)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.retCode);
                    }
                    else
                    {
                        int index = 0;
                        ProtoTable.ItemTable itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(800002001);
                        if (itemTable == null)
                        {
                            return;
                        }
                        ProtoTable.JarBonus jarItem = TableManager.GetInstance().GetTableItem<ProtoTable.JarBonus>(itemTable.ID);

                        if (jarItem != null)
                        {
                            //+3时序问题,导致第一次返回的数为0
                            int count = (CountDataManager.GetInstance().GetCount(jarItem.CounterKey)+jarItem.ComboBuyNum) / jarItem.ComboBuyNum;
                            int multiple = Mathf.CeilToInt((float)count / jarItem.ExBonusNum_1 + 0.1f);

                            if (multiple < 1)
                            {
                                index = jarItem.ExBonusNum_1 - count ;
                            }
                            else
                            {
                                index = multiple * jarItem.ExBonusNum_1 - count;
                            }
                        }

                        if (index == jarItem.ExBonusNum_1)
                        {
                            _ResItemList(msgRet, items, itemDoubleRewardList);
                        }
                        else
                        {
                            _ResItemList(msgRet, items, itemRrewardList);
                        }

                        AnsyOpenMagBox(callback, item, times);
                    }
                },false);
            }
            else
            {
                if(null != callback)
                {
                    callback();
                }
            }
        }

        #endregion

        void _ResItemList(SCOpenMagBoxRes msgRet, List<Item> items,List<ItemData> itemDataList)
        {
            for (int i = 0; i < msgRet.rewards.Length; i++)
            {
                OpenJarResult reward = msgRet.rewards[i];

                ProtoTable.JarItemPool table = TableManager.GetInstance().GetTableItem<ProtoTable.JarItemPool>((int)reward.jarItemId);

                if (table == null)
                {
                    Logger.LogError("can’t find JarItemPool....");
                }

                ItemData itemData = null;
                for (int j = 0; j < items.Count; j++)
                {
                    if (table.ItemID == items[j].dataid)
                    {
                        items[j].num -= (ushort)table.ItemNum;

                        itemData = ItemDataManager.GetInstance().CreateItemDataFromNet(items[j]);
                        itemData.Count = table.ItemNum;
                        if (itemData == null)
                        {
                            continue;
                        }

                        if (items[j].num <= 0)
                        {
                            items.RemoveAt(j);
                        }
                        break;
                    }
                }

                if (itemData == null)
                {
                    itemData = ItemDataManager.CreateItemDataFromTable((int)table.ItemID);
                    itemData.Count = table.ItemNum;
                }

                itemDataList.Add(itemData);
            }
        }
    }
}

