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
    public class GuildRedPacketFrame : ClientFrame
    {
        #region inner def
        public class GuildRedPackData
        {
            public RedPacketBaseEntry redPacketBaseEntry = new RedPacketBaseEntry();
        }

        #endregion

        #region val    

        List<GuildRedPackData> guildRedPackDatas = null;
        #endregion

        #region ui bind
        private Button Close = null;
        private ComUIListScript redPackList = null;
        private Button redPackRecord = null;
        private Button btSend = null;
        private Text vipLimit = null;
        private Text leftTime = null;  

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildRedPacket";
        }

        protected override void _OnOpenFrame()
        {
            BindUIEvent();

            guildRedPackDatas = new List<GuildRedPackData>();
            
            UpdateRedPackList();
            UpdateSendRedPacketLimitInfo();
        }

        protected override void _OnCloseFrame()
        {
            guildRedPackDatas = null;
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            Close = mBind.GetCom<Button>("Close");
            Close.SafeSetOnClickListener(() => 
            {
                frameMgr.CloseFrame(this);
            });

            redPackList = mBind.GetCom<ComUIListScript>("redPackList");

            redPackRecord = mBind.GetCom<Button>("redPackRecord");
            redPackRecord.SafeSetOnClickListener(() => 
            {
                frameMgr.OpenFrame<GuildMyRedPacketRecordFrame>(FrameLayer.Middle);
            });

            btSend = mBind.GetCom<Button>("btSend");
            btSend.SafeSetOnClickListener(() => 
            {
                SendRedPacketFrame.sendRedPackType = SendRedPackType.GuildMember;
                frameMgr.OpenFrame<SendRedPacketFrame>(FrameLayer.Middle);
            });

            vipLimit = mBind.GetCom<Text>("vipLimit");
            leftTime = mBind.GetCom<Text>("leftTime");
        }

        protected override void _unbindExUI()
        {
            Close = null;
            redPackList = null;
            redPackRecord = null;
            btSend = null;
            vipLimit = null;
            leftTime = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketGet, OnUpdateRedPack);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketDelete, OnUpdateRedPack);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketOpenSuccess, OnUpdateRedPack);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketSendSuccess, OnSendRedPackSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, OnSendRedPackSuccess);   
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketGet, OnUpdateRedPack);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketDelete, OnUpdateRedPack);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketOpenSuccess, OnUpdateRedPack);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketSendSuccess, OnSendRedPackSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, OnSendRedPackSuccess);
        } 

        void OnUpdateRedPack(UIEvent _uiEvent)
        {
            UpdateRedPackList();
        }

        void OnSendRedPackSuccess(UIEvent _uiEvent)
        {
            UpdateSendRedPacketLimitInfo();
        }

        void GetRedPackStates(RedPacketBaseEntry redPacketBaseEntry, ref bool bCanGet,ref bool bNotReach,ref bool bGot,ref bool bEmpty)
        {
            if(redPacketBaseEntry == null)
            {
                return;
            }

            RedPacketStatus redPacketStatus = (RedPacketStatus)(redPacketBaseEntry.status);
            if (redPacketStatus == RedPacketStatus.UNSATISFY)
            {
                bNotReach = true;
            }

            if (redPacketStatus == RedPacketStatus.WAIT_RECEIVE)
            {
                bCanGet = true;
            }

            if (redPacketStatus == RedPacketStatus.RECEIVED)
            {
                bGot = true;
            }

            if(redPacketStatus == RedPacketStatus.EMPTY)
            {
                bEmpty = true;
            }

            return;
        }

        void CalcGuildRedPackDatas()
        {
            guildRedPackDatas = new List<GuildRedPackData>();
            if(guildRedPackDatas == null)
            {
                return;
            }

            List<RedPacketBaseEntry> redPacketBaseEntries = RedPackDataManager.GetInstance().GetRedPacketsByType(RedPacketType.GUILD);
            if(redPacketBaseEntries == null)
            {
                return;
            }

            for(int i = 0;i < redPacketBaseEntries.Count;i++)
            {
                if(redPacketBaseEntries[i] == null)
                {
                    return;
                }

                GuildRedPackData guildRedPackData = new GuildRedPackData();
                if(guildRedPackData == null)
                {
                    return;
                }

                guildRedPackData.redPacketBaseEntry = redPacketBaseEntries[i];
                guildRedPackDatas.Add(guildRedPackData);
            }

            guildRedPackDatas.Sort((a, b) => 
            {
                if(a.redPacketBaseEntry == null || b.redPacketBaseEntry == null)
                {
                    return 0;
                }

                bool aIsCanGet = false;
                bool bIsCanGet = false;

                bool aIsNotReach = false;
                bool bIsNotReach = false;

                bool aIsGot = false;
                bool bIsGot = false;

                bool aIsEmpty = false;
                bool bIsEmpty = false;

                GetRedPackStates(a.redPacketBaseEntry,ref aIsCanGet,ref aIsNotReach,ref aIsGot,ref aIsEmpty);
                GetRedPackStates(b.redPacketBaseEntry, ref bIsCanGet, ref bIsNotReach, ref bIsGot,ref bIsEmpty);

                if(aIsCanGet != bIsCanGet)
                {
                    return bIsCanGet.CompareTo(aIsCanGet);
                }

                if(aIsNotReach != bIsNotReach)
                {
                    return bIsNotReach.CompareTo(aIsNotReach);
                }

                if(aIsGot != bIsGot)
                {
                    return bIsGot.CompareTo(aIsGot);
                }

                if(aIsEmpty != bIsEmpty)
                {
                    return bIsEmpty.CompareTo(aIsEmpty);
                }

                return b.redPacketBaseEntry.id.CompareTo(a.redPacketBaseEntry.id);
            });
            return;
        }

        void UpdateRedPackListItem(ComUIListElementScript item)
        {
            if(item ==  null)
            {
                return;
            }

            if(guildRedPackDatas == null)
            {
                return;
            }

            if(item.m_index >= guildRedPackDatas.Count)
            {
                return;
            }

            GuildRedPacketItem guildRedPacketItem = item.gameObjectBindScript as GuildRedPacketItem;
            if(guildRedPacketItem != null && guildRedPackDatas[item.m_index].redPacketBaseEntry != null)
            {
                guildRedPacketItem.SetUp(guildRedPackDatas[item.m_index].redPacketBaseEntry.id);
            }
        }

        void UpdateRedPackList()
        {
            if (redPackList == null)
            {
                return;
            }

            CalcGuildRedPackDatas();
            if (guildRedPackDatas == null)
            {
                return;
            }

            redPackList.Initialize();
            redPackList.onBindItem = (item) => 
            {
                if(item != null)
                {
                    return item.GetComponent<GuildRedPacketItem>();
                }
                return null;
            };

            redPackList.onItemVisiable = (item) => 
            {
                UpdateRedPackListItem(item);
            };

            redPackList.OnItemUpdate = (item) => 
            {
                UpdateRedPackListItem(item);
            };

            redPackList.UpdateElementAmount(guildRedPackDatas.Count);
        }

        void UpdateSendRedPacketLimitInfo()
        {
            vipLimit.CustomActive(false);
            leftTime.CustomActive(false);

            bool bCanSend = true;
            int minVipLv = Utility.GetSystemValueFromTable(SystemValueTable.eType2.SVT_GUILD_RED_PACKET_VIP_LV_LIMIT);
            int maxCount = GuildDataManager.GetInstance().GetDailySendRedPacketMaxCount();
            int left = GuildDataManager.GetInstance().GetDailySendRedPacketLeftCount();
            if (PlayerBaseData.GetInstance().VipLevel < minVipLv)
            {
                bCanSend = false;
                vipLimit.CustomActive(true);
                vipLimit.SafeSetText(TR.Value("guild_red_packet_need_vip_lv", minVipLv));
            }
            else
            {
                bCanSend = (left > 0);
                leftTime.CustomActive(true);
                leftTime.SafeSetText(TR.Value("guild_red_packet_daily_left_count", left, maxCount));
            }

            btSend.SafeSetGray(!bCanSend);
        }

        #endregion

        #region ui event       

        #endregion
    }
}
