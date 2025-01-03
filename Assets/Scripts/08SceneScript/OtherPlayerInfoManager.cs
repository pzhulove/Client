using UnityEngine;
using System.Collections;
using Protocol;
using Network;
using System.Collections.Generic;

namespace GameClient
{
    class OtherPlayerInfoManager : DataManager<OtherPlayerInfoManager>
    {
        #region delegate
        #endregion

        #region process

        public override void Initialize()
        {
            NetProcess.AddMsgHandler(WorldQueryPlayerRet.MsgID, OnRecvWatchPlayerRet);
        }

        public override void Clear()
        {
            NetProcess.RemoveMsgHandler(WorldQueryPlayerRet.MsgID, OnRecvWatchPlayerRet);
        }
        #endregion

        public WorldQueryPlayerType QueryPlayerType = WorldQueryPlayerType.WQPT_WATCH_PLAYER_INTO;
        #region netmsg
        public void SendWatchOtherPlayerInfo(ulong roleID,
            uint queryType = 0,
            uint zoneId = 0)
        {
            WorldQueryPlayerReq kCmd = new WorldQueryPlayerReq();
            kCmd.name = "";
            kCmd.roleId = roleID;

            //queryType和zoneId字段只有在跨服中查找，才用到。
            kCmd.queryType = queryType;             //默认类型是0，表示本服；1：表示跨服
            kCmd.zoneId = zoneId;                   //跨服的服务器

            QueryPlayerType = (int)WorldQueryPlayerType.WQPT_WATCH_PLAYER_INTO;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, kCmd);
        }

        //同上架物品的拥有者进行密聊
        public void SendWatchOnShelfItemOwnerInfo(ulong itemOwnerId)
        {
            WorldQueryPlayerReq req = new WorldQueryPlayerReq();
            req.name = "";
            req.roleId = itemOwnerId;
            QueryPlayerType = WorldQueryPlayerType.WQPT_Query_ON_SHELF_ITEM_OWNER_INFO;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        //[MessageHandle(WorldQueryPlayerRet.MsgID)]
        void OnRecvWatchPlayerRet(MsgDATA msg)
        {
            WorldQueryPlayerRet ret = new WorldQueryPlayerRet();
            ret.decode(msg.bytes);


            ActorShowEquipData data = new ActorShowEquipData();
            data.m_iJob = ret.info.occu;
            data.m_guid = ret.info.id;
            data.m_iLevel = ret.info.level;
            data.m_kName = ret.info.name;
			data.vip = ret.info.vipLevel;
			data.guildName = ret.info.guildTitle.name;
			data.guildJob = ret.info.guildTitle.post;
            data.emblemLv = (int)ret.info.emblemLevel;
            data.totalEquipScore = ret.info.totalEquipScore;

            //加入佣兵团
            data.adventureTeamName = ret.info.adventureTeamName;
            data.adventureTeamGrade = ret.info.adventureTeamGrade;
            data.adventureTeamRank = ret.info.adventureTeamRanking;

            //查询区域和查询类型
            data.m_zoneId = ret.zoneId;
            data.m_queryPlayerType = ret.queryType;


            if (string.IsNullOrEmpty(data.m_kName))
            {
                Logger.LogErrorFormat("there is something wrong whit player name whose id = {0} job={1} level ={2}", data.m_guid, data.m_iJob,data.m_iLevel);
            }
            //加入装备
            data.m_akEquipts = new List<ItemData>();
            for (int i = 0; i < ret.info.equips.Length; ++i)
            {
                var curBaseEquip = ret.info.equips[i];
                ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)curBaseEquip.typeId);
                if (itemData != null)
                {
                    data.m_akEquipts.Add(itemData);
                    itemData.StrengthenLevel = curBaseEquip.strengthen;
                    itemData.GUID = curBaseEquip.id;
                    itemData.Packing = false;
                    itemData.EquipType = (EEquipType)curBaseEquip.equipType;
                    itemData.GrowthAttrType = (EGrowthAttrType)curBaseEquip.enhanceType;
                }
            }

            //加入时装
            data.m_akFashions = new List<ItemData>();
            for (int i = 0; i < ret.info.fashionEquips.Length; ++i)
            {
                var curBaseEquip = ret.info.fashionEquips[i];
                ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)curBaseEquip.typeId);
                if (itemData != null)
                {
                    data.m_akFashions.Add(itemData);
                    itemData.StrengthenLevel = curBaseEquip.strengthen;
                    itemData.GUID = curBaseEquip.id;
                    itemData.Packing = false;
                }
            }

            List<ItemData> items = GamePool.ListPool<ItemData>.Get();
            items.AddRange(data.m_akEquipts);
            items.AddRange(data.m_akFashions);
            data.m_dictEquipSuitObjs = EquipSuitDataManager.GetInstance().CalculateEquipSuitInfos(items);

            data.avatar = ret.info.avatar;

            //加入PK信息
            data.m_pkInfo = ret.info.pkInfo;
			data.pkValue = ret.info.seasonLevel; //ret.info.pkValue;
            data.matchScore = ret.info.matchScore;


			//宠物信息

			for(int i=0; i<ret.info.pets.Length; ++i)
			{
				var pdata = new ActorShowEquipData.PetData();
				pdata.dataID = (int)ret.info.pets[i].dataId;
				pdata.level = (int)ret.info.pets[i].level;
				pdata.hunger = (int)ret.info.pets[i].hunger;
				pdata.skillIndex = (int)ret.info.pets[i].skillIndex;
                pdata.petScore = (int)ret.info.pets[i].petScore;

				data.pets[i] = pdata;
			}

            switch(QueryPlayerType)
            {
                case WorldQueryPlayerType.WQPT_FRIEND:
                    {
                        FriendRecommendedFrame friendRecommendedFrame = ClientSystemManager.instance.GetFrame(typeof(FriendRecommendedFrame)) as FriendRecommendedFrame;
                        if (null != friendRecommendedFrame && friendRecommendedFrame.IsQuerying())
                        {
                            UIEventSystem.GetInstance().SendUIEvent(new UIEventRecvQueryPlayer(ret.info));
                        }
                    }
                    break;
                case WorldQueryPlayerType.WQPT_WATCH_PLAYER_INTO:
                    {
                        ClientSystemManager.instance.CloseFrame<ActorShowGroup>();
                        ClientSystemManager.instance.OpenFrame<ActorShowGroup>(FrameLayer.Middle, data);
                    }
                    break;
                case WorldQueryPlayerType.WQPT_TEACHER:
                    {
                        var rd = new RelationData
                        {
                            uid = ret.info.id,
                            name = ret.info.name,
                            level = ret.info.level,
                            occu = ret.info.occu,
                            isOnline = 1,
                            type = 0,
                            vipLv = ret.info.vipLevel,
                            status = 0,
                            seasonLv = ret.info.seasonLevel,
                            avatar = ret.info.avatar,
                            activeTimeType = ret.info.activeTimeType,
                            masterType = ret.info.masterType,
                            regionId = ret.info.regionId,
                            declaration = ret.info.declaration,
                        };
                        //RelationMenuFram._CheckGetTeacher(rd);
                        RelationDataManager.GetInstance().SetQueryedTeacherInfo(rd);
                    }
                    break;
                case WorldQueryPlayerType.WQPT_Query_ON_SHELF_ITEM_OWNER_INFO:
                    //获得上架物品的拥有者的信息，之后打开密聊
                    var relationData = new RelationData();
                    relationData.level = (ushort)data.m_iLevel;
                    relationData.uid = data.m_guid;
                    relationData.name = data.m_kName;
                    relationData.occu = (byte)data.m_iJob;
                    relationData.vipLv = (byte)data.vip;
                    //打开聊天框
                    AuctionNewUtility.OpenChatFrame(relationData);
                    break;
            }
            GamePool.ListPool<ItemData>.Release(items);

        }
        #endregion
    }
}