using System;
using System.Collections.Generic;
using Scripts.UI;

namespace GameClient
{
    //所有道具roll点界面
    public class DungeonRollFrame : ClientFrame
    {
        ComUIListScript mRollItems = null;
        public enum ROLLITEM_STAT
        {
            NONE, //
            HUM,  //谦让
            SCORE //获得分数
        };
        class RollItemInfo
        {
            public ItemData item = null;
            public ROLLITEM_STAT stat = ROLLITEM_STAT.NONE;
            public Protocol.RollItemInfo itemData = null;
            public int score = 0;
        };
        List<RollItemInfo> mRollItemDetailList = new List<RollItemInfo>();
        float curTime = 10.0f;
        float tableTime = 10.0f;//表格时间
        float maxTime = 100.0f; //最大等待时间
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Reward/RollItemFrame";
        }
        protected override void _bindExUI()
        {
            mRollItems = mBind.GetCom<ComUIListScript>("RollItemList");
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (maxTime < 0.0f)
            {
                Logger.LogError("等待时间超长了");
                return;
            }
            maxTime -= timeElapsed;
            if (maxTime < 0.0f)
            {
                Close();
                return;
            }
            if (curTime < 0.0f) return;
            curTime -= timeElapsed;
            if(curTime <= 0.0f)
            {
                CheckItemStat();
                //时间到了，等待服务器结果，其间不能再进行任何操作
                if(mRollItems != null)
                {
                    mRollItems.UpdateElement();
                }
              
            }
        }
        //时间到了以后检查是否还有其他未roll点的道具，如果有默认为谦让
        private void CheckItemStat()
        {
            var iter = mRollItemDetailList.GetEnumerator();
            while (iter.MoveNext())
            {
                var rollItem = iter.Current;
                if (rollItem != null)
                {
                    if (rollItem.stat == ROLLITEM_STAT.NONE)
                    {
                        SetHum(rollItem);
                    }
                }
            }
        }
        //发送谦让协议
        private void SetHum(RollItemInfo item)
        {
            if(item != null && item.itemData != null)
            {
                var req = new Protocol.WorldDungeonRollItemReq
                {
                    dropIndex = item.itemData.rollIndex,
                    opType = (byte)Protocol.RollOpTypeEnum.RIE_MODEST
                };
                //Logger.LogErrorFormat("SetHum Index {0} opType {1}", req.dropIndex, req.opType);
                Network.NetManager.instance.SendCommand(Network.ServerType.GATE_SERVER, req);
            }
        }

        protected override void _unbindExUI()
        {
            mRollItems = null;
        }
        private int SortItemData(RollItemInfo a, RollItemInfo b)
        {
            if (a == null || b == null || a.item == null || b.item == null) return 0;
            if (a.item.Quality > b.item.Quality) return 1;
            if (a.item.Quality == b.item.Quality)
            {
                if (a.item.TableID < b.item.TableID)
                {
                    return 1;
                }
                else if(a.item.TableID > b.item.TableID)
                {
                    return -1;
                }
                return 0;
            }
            return -1;
        }
        private void TestInit()
        {
            int[] itemIds = new int[] { 110210001,
                                        120510001,
                                        131210001,
                                        131510001,
                                        151810001,
                                        163510001,
                                        176132001,
                                        145460001};
            int[] itemIds1 = new int[] { 115210001,  //微光大剑
                                        120510001,  //  短刃重剑
                                        120610001,  //  锋利大剑
                                        135410004,  //  末日决战
                                        151210001,  //  英魂佩剑
                                        151810001,  //  云歌剑刃
                                        165410002,  //  巨龙裁决者
                                        142540002 };//神奇布甲外衣
            int[] itemIds2 = new int[] {    134744003  ,  //夜灵昼虎布甲长靴
                                        135444002  ,  //晨光墨魂布甲长靴
                                        135444003  ,  //晨光昼虎布甲长靴
                                        135444004  ,  //月神之羽短靴
                                        151444001  ,  //灵纹布鞋
                                        142541002  ,  //神奇布甲护腿
                                        142542002  ,  //神奇布甲护肩
                                        142043002 };  //神奇布甲腰带
            for (int i = 0; i < itemIds2.Length;i++)
            {
                mRollItemDetailList.Add( new RollItemInfo
                                        {
                                            item = GameClient.ItemDataManager.CreateItemDataFromTable(itemIds2[i])
                                        });
            }
        }
        protected override void _OnOpenFrame()
        {
            _RegisterUIEvent();
            //地下城roll掉落roll时间(秒)
            var systemValueItem = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>(631);
            if(systemValueItem != null)
            {
                tableTime = Convert.ToSingle(systemValueItem.Value);
                curTime = tableTime;
            }
            mRollItemDetailList.Clear();
            //   TestInit();
            var info = BattleDataManager.GetInstance().rollRewardItem;
            if (info != null)
            {
                for (int i = 0; i < info.Length; i++)
                {
                    var rollItem = info[i];
                    if (rollItem != null && rollItem.dropItem != null)
                    {
                        mRollItemDetailList.Add(new RollItemInfo
                        {
                            item = GameClient.ItemDataManager.CreateItemDataFromTable((int)rollItem.dropItem.itemId),
                            itemData = rollItem
                        });
                    }
                    else
                    {
                        Logger.LogErrorFormat("session id {0} rollitem {1} is invalid", ClientApplication.playerinfo != null ? ClientApplication.playerinfo.session : 0, i);
                    }
                }
            }
            mRollItemDetailList.Sort(SortItemData);
            _InitUI();
        }
        private void OnItemData(ComUIListElementScript item)
        {
            if (item.m_index >= 0 && item.m_index < mRollItemDetailList.Count)
            {
                ComRollItem comUI = item.GetComponent<ComRollItem>();
                if (comUI != null)
                {
                    RollItemInfo curData = mRollItemDetailList[(mRollItemDetailList.Count - item.m_index) - 1];
                    if (curData == null) return;
                    comUI.Init(curData.item,curTime, tableTime, curData.score, curData.stat);
                    if(curData.stat == ROLLITEM_STAT.NONE)
                    {
                        if (comUI.btnHum != null)
                        {
                            comUI.btnHum.enabled = true;
                            comUI.btnHum.onClick.RemoveAllListeners();
                            if (curTime > 0.0f)
                            {
                                comUI.btnHum.onClick.AddListener(() =>
                                {
                                    SystemNotifyManager.SysNotifyMsgBoxCancelOk(TR.Value("humility_rollitem_msg_box"), null, () =>
                                    {
                                        SetHum(curData);
                                    });

                                });
                            }
                            //else
                            //{
                            //    comUI.btnHum.enabled = false;
                            //}
                        }
                        if (comUI.btnRoll != null)
                        {
                            if (curTime > 0.0f)
                            {
                                comUI.btnRoll.enabled = true;
                                comUI.btnRoll.onClick.RemoveAllListeners();
                                comUI.btnRoll.onClick.AddListener(() =>
                                {
                                    if (curData != null && curData.itemData != null)
                                    {
                                        //发送roll点协议
                                        var req = new Protocol.WorldDungeonRollItemReq
                                        {
                                            dropIndex = curData.itemData.rollIndex,
                                            opType = (byte)Protocol.RollOpTypeEnum.RIE_NEED
                                        };
                                     //   Logger.LogErrorFormat("SetRoll Index {0} opType {1}", req.dropIndex, req.opType);
                                        Network.NetManager.instance.SendCommand(Network.ServerType.GATE_SERVER, req);
                                    }
                                });
                            }
                            //else
                            //{
                            //    comUI.btnRoll.enabled = false;
                            //}
                        }
                    }
                    
                }         
            }
        }
        public override bool IsNeedUpdate()
        {
            return true;
        }
        private void _InitUI()
        {
            if (mRollItems == null) return;
            mRollItems.Initialize();
            mRollItems.onItemVisiable = OnItemData;
            mRollItems.SetElementAmount(mRollItemDetailList.Count);
            mRollItems.OnItemUpdate = OnItemData;
        }

        protected override void _OnCloseFrame()
        {
            _UnRegisterUIEvent();
            if (GameClient.ItemTipManager.GetInstance() != null)
                GameClient.ItemTipManager.GetInstance().CloseAll();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpateRollItem, _OnUpdateRollItem);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpateRollItem, _OnUpdateRollItem);
        }
        //刷新roll道具后的状态
        void _OnUpdateRollItem(UIEvent uiEvent)
        {
            var res = uiEvent.Param1 as Protocol.WorldDungeonRollItemRes;
            if (res != null)
            {
            //    Logger.LogErrorFormat("WorldDungeonRollItemRes {0} {1}", res.dropIndex, res.point);
                var iter = mRollItemDetailList.GetEnumerator();
                while(iter.MoveNext())
                {
                    var rollItem = iter.Current;
                    if(rollItem.itemData != null&& rollItem.itemData.rollIndex == res.dropIndex)
                    {
                        if(res.opType == (byte)Protocol.RollOpTypeEnum.RIE_MODEST)
                        {
                            rollItem.stat = ROLLITEM_STAT.HUM;
                        }
                        else if(res.opType == (byte)Protocol.RollOpTypeEnum.RIE_NEED)
                        {
                            rollItem.stat = ROLLITEM_STAT.SCORE;
                        }
                        rollItem.score = (int)res.point;
                        if(mRollItems != null)
                        {
                            mRollItems.UpdateElement();
                        }
                        return;
                    }
                }
            }
        }
    }
}
