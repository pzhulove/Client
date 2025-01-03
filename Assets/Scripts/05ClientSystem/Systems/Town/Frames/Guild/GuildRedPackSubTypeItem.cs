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
using DG.Tweening;
using System.Reflection;


namespace GameClient
{
    public class GuildRedPackSubTypeItem : MonoBehaviour
    {
        // Use this for initialization
        [SerializeField]
        SendRedPackType sendRedPackType = SendRedPackType.GuildMember;

        [SerializeField]
        Text name = null;

        [SerializeField]
        Text time = null;

        [SerializeField]
        Text receiverInfo = null;

        [SerializeField]
        Button btnSelect = null;

        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateGuildRedPacketSpecInfo, _OnUpdateGuildRedPacketSpecInfo);
        }

        void Start()
        {            
            UpdateInfo();
        }       

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateGuildRedPacketSpecInfo, _OnUpdateGuildRedPacketSpecInfo);
        }

        private void OnDisable()
        {
            
        }

        private void _OnUpdateGuildRedPacketSpecInfo(UIEvent uiEvent)
        {
            UpdateInfo();
        }

        // Update is called once per frame
        void Update()
        {
           
        }     

        public void SetUp(object data)
        {
           
        }

        void UpdateInfo()
        {
            GuildRedPacketSpecInfo guildRedPacketSpecInfo = RedPackDataManager.GetInstance().GetGuildRedPacketSpecInfo(sendRedPackType);

            btnSelect.SafeSetOnClickListener(() => 
            {
                if(guildRedPacketSpecInfo != null && guildRedPacketSpecInfo.joinNum == 0)
                {
                    return;
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSelectGuildRedPackType, sendRedPackType);
            });
                  
            if (guildRedPacketSpecInfo != null)
            {
                name.SafeSetText(SendRedPacketFrame.GetRedPackTypeName(sendRedPackType) + TR.Value("guild_red_packet_player_num", guildRedPacketSpecInfo.joinNum));

                DateTime dateTime = TimeUtility.GetDateTimeByTimeStamp((int)guildRedPacketSpecInfo.lastTime);
                time.SafeSetText(TR.Value("guild_red_packet_last_join_time", dateTime.Year,dateTime.Month,dateTime.Day));
                if (sendRedPackType == SendRedPackType.GuildWar)
                {                    
                    receiverInfo.SafeSetText(TR.Value("guild_red_packet_guild_war_receiver", dateTime.Month, dateTime.Day));
                }
                else if (sendRedPackType == SendRedPackType.CrossGuildWar)
                {
                    receiverInfo.SafeSetText(TR.Value("guild_red_packet_cross_guild_war_receiver", dateTime.Month, dateTime.Day));
                }
                else if (sendRedPackType == SendRedPackType.GuildDungeon)
                {
                    receiverInfo.SafeSetText(TR.Value("guild_red_packet_guild_dungeon_receiver", dateTime.Month, dateTime.Day));
                }
                else if(sendRedPackType == SendRedPackType.GuildMember)
                {
                    receiverInfo.SafeSetText(TR.Value("guild_red_packet_all_members_receiver"));
                }

                UIGray uIGray = this.gameObject.SafeAddComponent<UIGray>(false);
                if(uIGray != null)
                {
                    uIGray.enabled = guildRedPacketSpecInfo.joinNum == 0;
                }
            }

            if (sendRedPackType == SendRedPackType.GuildMember && GuildDataManager.GetInstance().HasSelfGuild())
            {                
                time.SafeSetText("");                
            }
        }
    }
}


