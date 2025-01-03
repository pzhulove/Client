using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class FairDuelDataManager : DataManager<FairDuelDataManager>
    {
        private List<EqualPvPEuqipTable> mEqualPvPEuqipTableList = new List<EqualPvPEuqipTable>();

        public List<int> EquipIdList = new List<int>();
        public Dictionary<int, EqualPvPEuqipTable> EquipDic = new Dictionary<int, EqualPvPEuqipTable>();
        public List<int> FashioIdList = new List<int>();
        public Dictionary<int, EqualPvPEuqipTable> FashionDic = new Dictionary<int, EqualPvPEuqipTable>();

        private List<int> mFairDuelActivityIDs = new List<int> {2026,2027, 2028, 2029,2030,2031,2032 };//公平竞技场的活动ID
        private const string FAIRDUEL_BATTLEFIELD = "FairDuelField";
        public override void Initialize()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityUpdate, OnActivityUpdate);
        }

        public override void Clear()
        {
            mEqualPvPEuqipTableList.Clear();
            EquipIdList.Clear();
            FashioIdList.Clear();
            EquipDic.Clear();
            FashionDic.Clear();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityUpdate, OnActivityUpdate);
        }
        
      
        public void IintEqualPvPEuqipTableList()
        {

            Dictionary<int, object> dicts = TableManager.instance.GetTable<EqualPvPEuqipTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    EqualPvPEuqipTable adt = iter.Current.Value as EqualPvPEuqipTable;
                    if (adt == null)
                    {
                        continue;
                    }
                    mEqualPvPEuqipTableList.Add(adt);
                }
            }
            for (int i = 0; i < mEqualPvPEuqipTableList.Count; i++)
            {
                if (mEqualPvPEuqipTableList[i].Occu == PlayerBaseData.GetInstance().JobTableID)
                //if (mEqualPvPEuqipTableList[i].Occu == 10)
                {
                    if (mEqualPvPEuqipTableList[i].Type == EqualPvPEuqipTable.eType.EQUIP)
                    {
                        if (!EquipDic.ContainsKey(mEqualPvPEuqipTableList[i].EquipID))
                        {
                            EquipIdList.Add(mEqualPvPEuqipTableList[i].EquipID);
                            EquipDic.Add(mEqualPvPEuqipTableList[i].EquipID, mEqualPvPEuqipTableList[i]);
                        }

                    }
                    else if (mEqualPvPEuqipTableList[i].Type == EqualPvPEuqipTable.eType.FASHION)
                    {
                        if (!FashionDic.ContainsKey(mEqualPvPEuqipTableList[i].EquipID))
                        {
                            FashioIdList.Add(mEqualPvPEuqipTableList[i].EquipID);
                            FashionDic.Add(mEqualPvPEuqipTableList[i].EquipID, mEqualPvPEuqipTableList[i]);
                        }

                    }
                }
            }
        }


        public List<ItemProperty> GetEquipedEquipments()
        {
            List<ItemProperty> equipmentsProperty = new List<ItemProperty>();
            //装备的装备
            var tmpEquip = EquipIdList;
            if (tmpEquip != null)
            {
                foreach (var id in tmpEquip)
                {
                    ItemData item = ItemDataManager.CreateItemDataFromTable(id);
                    if (item != null)
                    {
                        ItemProperty ip = item.GetBattleProperty();
                        ip.itemID = (int)item.TableID;
                        ip.guid = item.GUID;
                        equipmentsProperty.Add(ip);
                    }
                }
            }
            //装备的时装
            tmpEquip = FashioIdList;
            if (tmpEquip != null)
            {
                foreach (var id in tmpEquip)
                {
                    ItemData item = ItemDataManager.CreateItemDataFromTable(id);
                    if (item != null)
                    {
                        ItemProperty ip = item.GetBattleProperty();
                        //时装暂时不需要
                        ip.itemID = (int)item.TableID;
                        equipmentsProperty.Add(ip);
                    }
                }
            }
            return equipmentsProperty;
        }


        /// <summary>
        /// 判断公平竞技场的活动是否开启
        /// </summary>
        /// <returns></returns>
        public bool IsShowFairDuelEnterBtn()
        {
            bool isShow = false;
            for (int i = 0; i < mFairDuelActivityIDs.Count; i++)
            {
                int activityId = mFairDuelActivityIDs[i];

                if (!ActiveManager.GetInstance().allActivities.ContainsKey(activityId))
                {
                    continue;
                }

                var activityInfo = ActiveManager.GetInstance().allActivities[activityId];

                if (activityInfo.state == (byte)StateType.Running)
                {
                    isShow = true;
                }
            }
            return isShow;
        }

        /// <summary>
        /// 判断字符串是否为公平竞技场的字符串
        /// </summary>
        /// <param name="judgeStr"></param>
        /// <returns></returns>
        public bool IsFairDuelFieldStr(string judgeStr)
        {
            if (string.IsNullOrEmpty(judgeStr))
                return false;
            if (judgeStr.Equals(FAIRDUEL_BATTLEFIELD))
                return true;
            return false;
        }

        /// <summary>
        /// 是否有公平竞技场的活动ID
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        private bool IsContainsFairDuelActivityID(int activityID)
        {
            if(mFairDuelActivityIDs!=null)
            {
                return mFairDuelActivityIDs.Contains(activityID);
            }
            return false;
        }

       

        private void OnActivityUpdate(UIEvent uiEvent)
        {
            var activityId = (uint)uiEvent.Param1;
            if (IsContainsFairDuelActivityID((int)activityId))
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateFairDuelEntryState, IsShowFairDuelEnterBtn());
            }
        }

    }
}

