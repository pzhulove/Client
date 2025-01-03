using System;
using System.Collections.Generic;
using Network;
using Protocol;
using ProtoTable;

namespace GameClient
{
    public struct GiftPackItemData
    {
        public GiftPackItemData(GiftSyncInfo netData) : this()
        {
            if (netData == null)
            {
                Logger.LogError("数据为空");
                return;
            }

            ItemID = (int) netData.itemId;
            ID = (int) netData.id;
            ItemCount = (int) netData.itemNum;
            Weight = (int) netData.weight;
            Levels = new List<int>(netData.validLevels.Length);
            for (int i = 0; i < netData.validLevels.Length; ++i)
            {
                this.Levels.Add((int)netData.validLevels[i]);
            }

            RecommendJobs = new List<int>(netData.recommendJobs.Length);
            for (int i = 0; i < netData.recommendJobs.Length; ++i)
            {
                this.RecommendJobs.Add(netData.recommendJobs[i]);
            }

            EquipType = netData.equipType;
            Strengthen = (int)netData.strengthen;
            IsTimeLimit = netData.isTimeLimit == 1;
        }

        public int ItemID { get; private set; }
        public int ID { get; private set; }
        public int ItemCount { get; private set; }
        public List<int> RecommendJobs { get; private set; }
        public int Weight { get; private set; }
        public List<int> Levels { get; private set; }
        public int Strengthen { get; private set; }
        public bool IsTimeLimit { get; private set; }
        public int EquipType { get; private set; }
    }



    public class GiftPackDataManager : DataManager<GiftPackDataManager>
    {
        /// <summary>
        /// 以礼包ID为键值，缓存礼包数据，建立的快速道具索引表，key: 礼包ID，Value: GiftPackSyncInfo
        /// </summary>
        readonly Dictionary<int, GiftPackSyncInfo> mGiftPackItems = new Dictionary<int, GiftPackSyncInfo>();

        public static GiftPackItemData GetGiftDataFromNet(GiftSyncInfo netData)
        {
            return new GiftPackItemData(netData);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            Clear();
            NetProcess.AddMsgHandler(SceneGiftPackInfoRes.MsgID, OnGetGiftPackInfoResp);
        }

        /// <summary>
        /// 清理
        /// </summary>
        public override void Clear()
        {
            this.mGiftPackItems.Clear();
            NetProcess.RemoveMsgHandler(SceneGiftPackInfoRes.MsgID, OnGetGiftPackInfoResp);
        }

        /// <summary>
        /// 获取礼包数据
        /// </summary>
        /// <param name="giftPackId"></param>
        public void GetGiftPackItem(int giftPackId)
        {
            if (this.mGiftPackItems.ContainsKey(giftPackId))
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GetGiftData, this.mGiftPackItems[giftPackId]);
            }
            else
            {
                SceneGiftPackInfoReq request = new SceneGiftPackInfoReq
                {
                    giftPackIds = new UInt32[1]
                };
                request.giftPackIds[0] = (uint)giftPackId;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, request);
            }
        }

        void OnGetGiftPackInfoResp(MsgDATA data)
        {
            var pos = 0;
            var resp = new SceneGiftPackInfoRes();
            resp.decode(data.bytes, ref pos);

            if (resp.giftPacks == null || resp.giftPacks.Length == 0)
            {
                return;
            }
            //每次只会取一个
            if (!this.mGiftPackItems.ContainsKey((int)resp.giftPacks[0].id))
            {
                this.mGiftPackItems.Add((int)resp.giftPacks[0].id, resp.giftPacks[0]);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GetGiftData, resp.giftPacks[0]);
        }
    }
}