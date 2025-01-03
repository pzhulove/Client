using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{ 
    public class ChapterBattlePotionSetUtiilty
    {
        private static void _send(int idx, uint id)
        {
            SceneSetDungeonPotionReq req = new SceneSetDungeonPotionReq
            {
                potionId = id,
                pos = (byte)idx
            };

            Logger.LogProcessFormat("[PotionSet] **发送 {0}, {1}", idx, id);
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        public static int _getItemIdx(uint id)
        {
            if (id == 0)
            {
                return -1;
            }

            List<uint> potions = PlayerBaseData.GetInstance().potionSets;

            for (int i = 0; i < potions.Count; ++i)
            {
                if (potions[i] == id)
                {
                    Logger.LogProcessFormat("[PotionSet] 已配置 {0}, {1}", i, id);
                    return i;
                }
            }

            return -1;
        }

        public static void Cancel(int idx)
        {
            _send(idx, 0);
        }

        public static void Save(int idx, uint id)
        {
            int lastIdx = _getItemIdx(id);

            if (lastIdx >= 0)
            {
                Swap(idx, lastIdx);
                //_send(lastIdx, 0);
            }
            else
            {
                _send(idx, id);
            }
        }

        public static bool Swap(int fstIdx, int sndIdx)
        {
            if (fstIdx < 0 || fstIdx >= PlayerBaseData.GetInstance().potionSets.Count)
            {
                Logger.LogErrorFormat("[PotionSet] 第1个参数越界 {0}", fstIdx);
                return false;
            }

            if (sndIdx < 0 || sndIdx >= PlayerBaseData.GetInstance().potionSets.Count)
            {
                Logger.LogErrorFormat("[PotionSet] 第2个参数越界 {0}", sndIdx);
                return false;
            }

            if (fstIdx == sndIdx)
            {
                Logger.LogProcessFormat("[PotionSet] 参数相等 {0}", sndIdx);
                return false;
            }

            List<uint> potions = PlayerBaseData.GetInstance().potionSets;

            uint fstPotion = potions[fstIdx];
            uint sndPotion = potions[sndIdx];

            _send(fstIdx, sndPotion);
            _send(sndIdx, fstPotion);
        
            return true;
        }

        public static ItemConfigTable GetItemConfigTalbeByID(int itemid)
        {
            var itemConfigTable = TableManager.instance.GetTable<ItemConfigTable>();
            var iter = itemConfigTable.GetEnumerator();
            while (iter.MoveNext())
            {
                ItemConfigTable table = iter.Current.Value as ItemConfigTable;

                if (null != table && table.ItemID == itemid)
                {
                    return table;
                }
            }

            return null;
        }

        public static IEnumerator QuickBuyPostion(int id, int count)
        {
            QuickBuyFrame quickBuyFrame = QuickBuyFrame.Open(QuickBuyFrame.eQuickBuyType.BuffDrug);

            quickBuyFrame.SetQuickBuyItem(id, count);

            yield return null;

            while (!QuickBuyFrame.IsOpen(QuickBuyFrame.eQuickBuyType.BuffDrug))
            {
                yield return null;
            }

            while (quickBuyFrame.state == QuickBuyFrame.eState.None)
            {
                yield return null;
            }

            if (quickBuyFrame.state == QuickBuyFrame.eState.Success)
            {
                MessageEvents events = new MessageEvents();

                SceneQuickBuyReq req = new SceneQuickBuyReq();
                SceneQuickBuyRes res = new SceneQuickBuyRes();              

                req.type = (byte)QuickBuyTargetType.QUICK_BUY_ITEM;
                req.param1 = (ulong)id;
                req.param2 = (uint)count;

                yield return MessageUtility.Wait<SceneQuickBuyReq, SceneQuickBuyRes>(ServerType.GATE_SERVER, events, req, res, false, 30.0f);

                if (events.IsAllMessageReceived())
                {
                    if (0 != res.result)
                    {
                        SystemNotifyManager.SystemNotify((int)res.result);
                        //Logger.LogErrorFormat("[PostinSet] 快速购买{0} {1} 失败, 错误码 {2}", id, count, res.result);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonQuickBuyPotionFail);
                    }
                    else
                    {
                        Logger.LogProcessFormat("[PostinSet] 快速购买{0} {1} 成功", id, count);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonQuickBuyPotionSuccess);
                    }
                }
                else
                {
                    Logger.LogErrorFormat("[PostinSet] 快速购买{0} {1} 失败, 消息超时", id, count);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonQuickBuyPotionFail);
                }
            }
            else
            {
                Logger.LogErrorFormat("[PostinSet] 快速购买{0} {1} 失败, 用户取消", id, count);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonQuickBuyPotionFail);
            }
        }
    }
}
