using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    /// <summary>
    /// 头像框页签类型
    /// </summary>
    public enum HeadPortraitTabType
    {
        HPTT_ALL = 0, //全部
        HPTT_ROLE,    //角色
        HPTT_ACTIVITY, //活动
        HPTT_OTHER,   //其他
        HPTT_COUNT,
    }

    public class HeadPortraitItemData:System.IComparable<HeadPortraitItemData>
    {
        /// <summary>
        /// 道具ID
        /// </summary>
        public int itemID;
        /// <summary>
        /// 头像框名字
        /// </summary>
        public string Name;
        /// <summary>
		///  过期时间
		/// </summary>
		public int expireTime;
        /// <summary>
        /// 类型
        /// </summary>
        public HeadPortraitTabType tabType;
        /// <summary>
        /// 是否获得
        /// </summary>
        public bool isObtain;
        /// <summary>
        /// 获得条件
        /// </summary>
        public string conditions;
        /// <summary>
        /// 头像框路径
        /// </summary>
        public string iconPath;
        /// <summary>
        /// 是否新头像框
        /// </summary>
        public bool isNew;

        public HeadPortraitItemData(HeadPortraitTabType type, PictureFrameTable pictureFrameTable)
        {
            if (type == HeadPortraitTabType.HPTT_ALL)
            {
                tabType = HeadPortraitTabType.HPTT_ALL;
            }
            else
            {
                tabType = (HeadPortraitTabType)pictureFrameTable.TabID;
            }

            itemID = pictureFrameTable.ID;
            Name = pictureFrameTable.Name;
            conditions = pictureFrameTable.Conditions;
            iconPath = pictureFrameTable.IconPath;
            expireTime = 0;
            if (pictureFrameTable.ID == HeadPortraitFrameDataManager.iDefaultHeadPortraitID)
            {
                isObtain = true;
            }
            else
            {
                isObtain = false;
            }
            
            isNew = false;
        }

        public int CompareTo(HeadPortraitItemData other)
        {
            if (isObtain != other.isObtain)
            {
                return other.isObtain.CompareTo(isObtain);
            }

            return itemID - other.itemID;
        }
    }

    public class HeadPortraitFrameDataManager : DataManager<HeadPortraitFrameDataManager>
    {

        /// <summary>
        /// 根据页签类型保存头像框数据
        /// </summary>
        public Dictionary<HeadPortraitTabType, List<HeadPortraitItemData>> mHeadPortraitItemDataDict = new Dictionary<HeadPortraitTabType, List<HeadPortraitItemData>>();

        private static int wearHeadPortraitFrameID = 0;
        /// <summary>
        /// 现穿戴头像框ID
        /// </summary>
        public static int WearHeadPortraitFrameID
        {
            get { return wearHeadPortraitFrameID; }
            set
            {
                wearHeadPortraitFrameID = value;
            }
        }

        //默认头像框ID
        public static int iDefaultHeadPortraitID = 130194001;

        /// <summary>
        /// 记录每个页签下 新道具数量
        /// </summary>
        protected static int[] tabHasNew = new int[(int)HeadPortraitTabType.HPTT_COUNT];

        /// <summary>
        /// 根据页签类型得到头像框列表
        /// </summary>
        /// <param name="tabType"></param>
        /// <returns></returns>
        public List<HeadPortraitItemData> GetHeadPortraitItemList(HeadPortraitTabType tabType)
        {
            List<HeadPortraitItemData> list = null;
            if (mHeadPortraitItemDataDict.TryGetValue(tabType,out list))
            {
                return list;
            }

            return null;
        }

        public sealed override void Clear()
        {
            if (mHeadPortraitItemDataDict != null)
            {
                mHeadPortraitItemDataDict.Clear();
            }

            tabHasNew = new int[(int)HeadPortraitTabType.HPTT_COUNT];

            UnRegisterNetHandler();

            wearHeadPortraitFrameID = 0;
        }

        public sealed override void Initialize()
        {
            RegisterNetHandler();
            InitLocalData();
        }
        
        private void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(SceneHeadFrameRes.MsgID, OnSceneHeadFrameRes);
            NetProcess.AddMsgHandler(SceneHeadFrameUseRes.MsgID, OnSceneHeadFrameUseRes);
            NetProcess.AddMsgHandler(SceneHeadFrameNotify.MsgID, OnSceneHeadFrameNotify);
        }

        private void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(SceneHeadFrameRes.MsgID, OnSceneHeadFrameRes);
            NetProcess.RemoveMsgHandler(SceneHeadFrameUseRes.MsgID, OnSceneHeadFrameUseRes);
            NetProcess.RemoveMsgHandler(SceneHeadFrameNotify.MsgID, OnSceneHeadFrameNotify);
        }

        private void InitLocalData()
        {
            var pictureFrameTable = TableManager.GetInstance().GetTable<PictureFrameTable>().GetEnumerator();

            while (pictureFrameTable.MoveNext())
            {
                var table = pictureFrameTable.Current.Value as PictureFrameTable;
                if (table == null)
                {
                    continue;
                }
                
                if (!mHeadPortraitItemDataDict.ContainsKey((HeadPortraitTabType)table.TabID))
                {
                    mHeadPortraitItemDataDict.Add((HeadPortraitTabType)table.TabID, new List<HeadPortraitItemData>());
                }

                if (!mHeadPortraitItemDataDict.ContainsKey(HeadPortraitTabType.HPTT_ALL))
                {
                    mHeadPortraitItemDataDict.Add(HeadPortraitTabType.HPTT_ALL, new List<HeadPortraitItemData>());
                }

                HeadPortraitItemData data = new HeadPortraitItemData((HeadPortraitTabType)table.TabID, table);

                HeadPortraitItemData data2 = new HeadPortraitItemData(HeadPortraitTabType.HPTT_ALL, table);

                mHeadPortraitItemDataDict[(HeadPortraitTabType)table.TabID].Add(data);
                mHeadPortraitItemDataDict[HeadPortraitTabType.HPTT_ALL].Add(data2);
            }
        }

        /// <summary>
        /// 头像框请求
        /// </summary>
        public void OnSendSceneHeadFrameReq()
        {
            SceneHeadFrameReq req = new SceneHeadFrameReq();
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 使用头像框请求
        /// </summary>
        /// <param name="headFrameId"></param>
        public void OnSendSceneHeadFrameUseReq(uint headFrameId)
        {
            SceneHeadFrameUseReq req = new SceneHeadFrameUseReq();
            req.headFrameId = headFrameId;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 头像框返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnSceneHeadFrameRes(MsgDATA msg)
        {
            SceneHeadFrameRes res = new SceneHeadFrameRes();
            res.decode(msg.bytes);

            for (int i = 0; i < res.headFrameList.Length; i++)
            {
                var headItem = res.headFrameList[i];

                var headPortraitItemDataDict = mHeadPortraitItemDataDict.GetEnumerator();
                while (headPortraitItemDataDict.MoveNext())
                {
                    var list = headPortraitItemDataDict.Current.Value as List<HeadPortraitItemData>;
                    for (int j = 0; j < list.Count; j++)
                    {
                        var item = list[j];
                        if (item == null)
                        {
                            continue;
                        }

                        if (item.itemID != headItem.headFrameId)
                        {
                            continue;
                        }

                        item.expireTime = (int)headItem.expireTime;
                        item.isObtain = true;
                        item.isNew = false;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 使用头像框返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnSceneHeadFrameUseRes(MsgDATA msg)
        {
            SceneHeadFrameUseRes res = new SceneHeadFrameUseRes();
            res.decode(msg.bytes);

            if (res.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.retCode);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UseHeadPortraitFrameSuccess);
            }
        }

        /// <summary>
        /// 头像框通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnSceneHeadFrameNotify(MsgDATA msg)
        {
            SceneHeadFrameNotify res = new SceneHeadFrameNotify();
            res.decode(msg.bytes);

            var headPortraitItemDataDict = mHeadPortraitItemDataDict.GetEnumerator();
            while (headPortraitItemDataDict.MoveNext())
            {
                var list = headPortraitItemDataDict.Current.Value as List<HeadPortraitItemData>;
                for (int j = 0; j < list.Count; j++)
                {
                    var item = list[j];
                    if (item == null)
                    {
                        continue;
                    }
                    if (item.itemID != res.headFrame.headFrameId)
                    {
                        continue;
                    }

                    item.isObtain = res.isGet == 1;
                    item.expireTime = (int)res.headFrame.expireTime;
                    item.isNew = res.isGet == 1;

                    if (item.isNew)
                    {
                        if (item.tabType >= HeadPortraitTabType.HPTT_ALL && item.tabType < HeadPortraitTabType.HPTT_COUNT)
                        {
                            tabHasNew[(int)item.tabType] += 1;
                        }
                    }
                    else
                    {
                        if (item.tabType >= HeadPortraitTabType.HPTT_ALL && item.tabType < HeadPortraitTabType.HPTT_COUNT)
                        {
                            if (tabHasNew[(int)item.tabType] > 0)
                            {
                                tabHasNew[(int)item.tabType] -= 1;
                            }
                        }
                    }
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HeadPortraitItemStateChanged);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HeadPortraitFrameNotify);
        }

        /// <summary>
        /// 设置头像框为老的
        /// </summary>
        public void NotifyItemBeOld(HeadPortraitItemData item)
        {
            if (item != null && item.isNew)
            {
                item.isNew = false;

                if (item.tabType >= HeadPortraitTabType.HPTT_ALL && item.tabType < HeadPortraitTabType.HPTT_COUNT)
                {
                    tabHasNew[(int)item.tabType] -= 1;
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HeadPortraitItemStateChanged);
            }
        }

        /// <summary>
        /// 指定页签下是否有新头像框
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsHeadPortraitItemHasNew(HeadPortraitTabType type)
        {
            if (type >= HeadPortraitTabType.HPTT_ALL && type < HeadPortraitTabType.HPTT_COUNT)
            {
                return tabHasNew[(int)type] > 0 ? true : false;
            }

            return false;
        }

        /// <summary>
        /// 得到头像框路径
        /// </summary>
        /// <param name="id">头像框表ID</param>
        /// <returns></returns>
        public static string GetHeadPortraitFramePath(int id)
        {
            var table = TableManager.GetInstance().GetTableItem<PictureFrameTable>(id);
            if (table == null)
            {
                return "";
            }

            return table.IconPath;
        }
    }
}

