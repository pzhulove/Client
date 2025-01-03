using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    public class AchievementGroupDataManager :  DataManager<AchievementGroupDataManager>
    {
        public override void Initialize()
        {
            NetProcess.RemoveMsgHandler(SceneAchievementScoreRewardRes.MsgID, _OnRecvSceneAchievementScoreRewardRes);
        }

        public override void Clear()
        {
            NetProcess.AddMsgHandler(SceneAchievementScoreRewardRes.MsgID, _OnRecvSceneAchievementScoreRewardRes);
        }

        void _OnRecvSceneAchievementScoreRewardRes(MsgDATA msg)
        {
            SceneAchievementScoreRewardRes recv = new SceneAchievementScoreRewardRes();
            recv.decode(msg.bytes);

            if(recv.result != 0)
            {

            }
        }

        public static void OnLink2FixedAchievementItem(string param)
        {
            if(!string.IsNullOrEmpty(param))
            {
                int iId = 0;
                if(int.TryParse(param,out iId))
                {
                    OnLink2FixedAchievementItemById(iId);
                }
            }
        }

        public static void OnLink2FixedAchievementItemById(int iId)
        {
            var subItem = TableManager.GetInstance().GetTableItem<ProtoTable.AchievementGroupSubItemTable>(iId);
            if (null == subItem)
            {
                //Logger.LogErrorFormat("id = {0} can not be found in AchievementGroupSubItemTable !!!", iId);
                return;
            }

            int iTabId = GetInstance().GetBelongTabByID(iId);
            if (0 == iTabId)
            {
                //Logger.LogErrorFormat("id = {0} can not be found in AchievementGroupSecondMenuTable !!!", iId);
                return;
            }

            ActiveGroupMainFrame.CommandOpen(new ActiveGroupMainFrameData { iTabID = iTabId });
        }

        public int GetBelongTabByID(int iId)
        {
            var table = TableManager.GetInstance().GetTable<AchievementGroupSecondMenuTable>();
            if(null != table)
            {
                var enumerator = table.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    AchievementGroupSecondMenuTable tabItem = enumerator.Current.Value as AchievementGroupSecondMenuTable;
                    if(null != tabItem)
                    {
                        for(int i = 0; i < tabItem.SubItemID.Count; ++i)
                        {
                            if(iId == tabItem.SubItemID[i])
                            {
                                return tabItem.ID;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        public void GetSubItemsByTag(AchievementGroupMainItemTable mainItem,AchievementGroupSecondMenuTable menuItem, ref List<AchievementGroupSubItemTable> subItems)
        {
            if(null == subItems)
            {
                subItems = new List<AchievementGroupSubItemTable>(32);
            }
            subItems.Clear();

            if (null != menuItem)
            {
                for(int i = 0; i < menuItem.SubItemID.Count; ++i)
                {
                    var subItem = TableManager.GetInstance().GetTableItem<AchievementGroupSubItemTable>(menuItem.SubItemID[i]);
                    if(null != subItem)
                    {
                        subItems.Add(subItem);
                    }
                }
            }
            else
            {
                if(null != mainItem)
                {
                    if(mainItem.ChildTabs.Count <= 0 || mainItem.ChildTabs.Count == 1 && mainItem.ChildTabs[0] == 0)
                    {
                        var talbelItems = TableManager.GetInstance().GetTable<AchievementGroupSecondMenuTable>();
                        var enumerator = talbelItems.GetEnumerator();
                        while(enumerator.MoveNext())
                        {
                            menuItem = enumerator.Current.Value as AchievementGroupSecondMenuTable;
                            if (null != menuItem)
                            {
                                for (int j = 0; j < menuItem.SubItemID.Count; ++j)
                                {
                                    var subItem = TableManager.GetInstance().GetTableItem<AchievementGroupSubItemTable>(menuItem.SubItemID[j]);
                                    if (null != subItem)
                                    {
                                        subItems.Add(subItem);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < mainItem.ChildTabs.Count; ++i)
                        {
                            menuItem = TableManager.GetInstance().GetTableItem<AchievementGroupSecondMenuTable>(mainItem.ChildTabs[i]);
                            if (null != menuItem)
                            {
                                for (int j = 0; j < menuItem.SubItemID.Count; ++j)
                                {
                                    var subItem = TableManager.GetInstance().GetTableItem<AchievementGroupSubItemTable>(menuItem.SubItemID[j]);
                                    if (null != subItem)
                                    {
                                        subItems.Add(subItem);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public int GetSubItemsAValue(AchievementGroupMainItemTable mainItem, AchievementGroupSecondMenuTable menuItem,bool bFinish)
        {
            int av = 0;
            List<AchievementGroupSubItemTable> subItems = null;

            if (null == mainItem)
            {
                if (null == mListItems)
                {
                    _InitAllListItems();
                }

                subItems = mListItems;
            }
            else
            {
                GetSubItemsByTag(mainItem, menuItem, ref subItems);
            }

            if (null != subItems)
            {
                for (int i = 0; i < subItems.Count; ++i)
                {
                    if (bFinish)
                    {
                        var missionValue = MissionManager.GetInstance().GetMission((uint)subItems[i].ID);
                        if (null != missionValue && null != missionValue.missionItem)
                        {
                            if (missionValue.status == (int)Protocol.TaskStatus.TASK_OVER)
                            {
                                av += missionValue.missionItem.IntParam0;
                            }
                        }
                    }
                    else
                    {
                        var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(subItems[i].ID);
                        if(null != missionItem)
                        {
                            av += missionItem.IntParam0;
                        }
                    }
                }
            }

            return av;
        }

        List<AchievementGroupSubItemTable> mListItems = null;
        void _InitAllListItems()
        {
            if(null == mListItems)
            {
                mListItems = new List<AchievementGroupSubItemTable>(32);
                var enumerator = TableManager.GetInstance().GetTable<AchievementGroupSubItemTable>().GetEnumerator();
                while(enumerator.MoveNext())
                {
                    AchievementGroupSubItemTable subItem = enumerator.Current.Value as AchievementGroupSubItemTable;
                    mListItems.Add(subItem);
                }
            }
        }

        public void GetAllItems(ref List<AchievementGroupSubItemTable> items)
        {
            if(null == mListItems)
            {
                _InitAllListItems();
            }
            if(null == items)
            {
                items = new List<AchievementGroupSubItemTable>();
            }
            items.Clear();
            items.AddRange(mListItems);
        }

        public AchievementLevelInfoTable GetAchievementLevelByPoint(int iPoint)
        {
            AchievementLevelInfoTable lastItem = null;
            var table = TableManager.GetInstance().GetTable<AchievementLevelInfoTable>();
            if (null != table)
            {
                var enumerator = table.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var levelItem = enumerator.Current.Value as AchievementLevelInfoTable;
                    lastItem = levelItem;
                    if (null != levelItem && levelItem.Min <= iPoint && levelItem.Max >= iPoint)
                    {
                        return levelItem;
                    }
                }
            }

            return lastItem;
        }

        private int _SortItemByFlag(AchievementGroupSubItemTable left, AchievementGroupSubItemTable right)
        {
            return left.sort1 - right.sort1;
        }

        private int _SortItemByAll(AchievementGroupSubItemTable left, AchievementGroupSubItemTable right)
        {
            return left.sort0 - right.sort0;
        }

        public void SendGetAward(int id)
        {
            SceneAchievementScoreRewardReq kSend = new SceneAchievementScoreRewardReq();
            kSend.rewardId = (uint)id;

            Network.NetManager.Instance().SendCommand(Network.ServerType.GATE_SERVER, kSend);
        }

        public int GetFirstUnAcquiredID()
        {
            var property = PlayerBaseData.GetInstance().AchievementMaskProperty;
            if(null != property)
            {
                int iSize = TableManager.GetInstance().GetTable<ProtoTable.AchievementLevelInfoTable>().Count;
                for (uint i = 0; i < property.maskSize && i < iSize; ++i)
                {
                    int iId = (int)(i + 1);
                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.AchievementLevelInfoTable>(iId);
                    if (null == item)
                    {
                        continue;
                    }

                    if (!property.CheckMask((uint)iId))
                    {
                        return iId;
                    }
                }
            }
            return 0;
        }

        int _LastId = 0;
        public int GetLastAcquiredID()
        {
            if(0 == _LastId)
            {
                var table = TableManager.GetInstance().GetTable<AchievementLevelInfoTable>();
                if (null != table)
                {
                    var enumerator = table.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var levelItem = enumerator.Current.Value as AchievementLevelInfoTable;
                        if(null != levelItem)
                        {
                            _LastId = levelItem.ID;
                        }
                    }
                }
            }
            return _LastId;
        }

        public bool IsAchievementFinished(int iId)
        {
            var item = TableManager.GetInstance().GetTableItem<ProtoTable.AchievementLevelInfoTable>(iId);
            if(null == item)
            {
                return false;
            }

            var property = PlayerBaseData.GetInstance().AchievementMaskProperty;
            if (null == property)
            {
                return false;
            }

            if (property.CheckMask(((uint)iId)))
            {
                return true;
            }

            return false;
        }

        public bool IsAllAchievementFinished()
        {
            return 0 == GetFirstUnAcquiredID();
        }
    }
}