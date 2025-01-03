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


namespace GameClient
{
    using UIItemData = AwardItemData;

    public class GuildRedPacketItem : MonoBehaviour
    {
        // Use this for initialization
        [SerializeField]
        Image icon = null;

        [SerializeField]
        Text name = null;

        [SerializeField]
        Text owner = null;

        [SerializeField]
        Text leftCount = null;

        [SerializeField]
        Text state = null;

        [SerializeField]
        Text btnGetText = null;

        [SerializeField]
        Button btnGet = null;

        [SerializeField]
        Button btnDetail = null;

        [SerializeField]
        Button btnNotReach = null;

        [SerializeField]
        Image moneyIcon = null;

        [SerializeField]
        Text moneyNum = null;

        void Start()
        {
           
        }

        private void OnDestroy()
        {
          
        }

        // Update is called once per frame
        void Update()
        {
           
        }      

        public void SetUp(ulong guid)
        {
            RedPacketBaseEntry redPacketBaseEntry = RedPackDataManager.GetInstance().GetRedPacketBaseInfo(guid);
            if(redPacketBaseEntry == null)
            {
                return;
            }

            //icon.SafeSetImage("");
            name.SafeSetText(RedPackDataManager.GetInstance().GetGuildRedPacketTitleName(redPacketBaseEntry));
            leftCount.SafeSetText(TR.Value("guild_red_packet_left_count", redPacketBaseEntry.remainNum, redPacketBaseEntry.totalNum));
            owner.SafeSetText(TR.Value("guild_red_packet_owner", redPacketBaseEntry.ownerName));
            state.SafeSetText(RedPackDataManager.GetInstance().GetRedPacketStateText(redPacketBaseEntry.status));
            moneyNum.SafeSetText(redPacketBaseEntry.totalMoney.ToString());
            moneyIcon.SafeSetImage(RedPackDataManager.GetInstance().GetCostMoneyIcon(redPacketBaseEntry.reason));

            RedPacketStatus redPacketStatus = (RedPacketStatus)(redPacketBaseEntry.status);            
            //btnGetText.SafeSetText("");

            btnGet.SafeSetOnClickListener(() => 
            {
                ulong guidTemp = guid;

                RedPackDataManager.GetInstance().OpenRedPacket(guidTemp);
            });

            btnDetail.SafeSetOnClickListener(() => 
            {
                ulong guidTemp = guid;

                RedPackDataManager.GetInstance().CheckRedPacket(guidTemp);
            });

            btnNotReach.SafeSetOnClickListener(() =>
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guild_red_packet_not_reach"));
            });

            btnNotReach.CustomActive(false);
            btnGet.CustomActive(false);
            btnDetail.CustomActive(false);

            if(redPacketStatus == RedPacketStatus.UNSATISFY)
            {
                btnNotReach.CustomActive(true);
            }
            else if(redPacketStatus == RedPacketStatus.WAIT_RECEIVE)
            {
                btnGet.CustomActive(true);
            }
            else if(redPacketStatus == RedPacketStatus.RECEIVED || redPacketStatus == RedPacketStatus.EMPTY)
            {
                btnDetail.CustomActive(true);
            }
            
            if(redPacketStatus == RedPacketStatus.RECEIVED || redPacketStatus == RedPacketStatus.EMPTY)
            {
                state.CustomActive(true);
                leftCount.CustomActive(false);
            }
            else
            {
                state.CustomActive(false);
                leftCount.CustomActive(true);
            }

            return;
        }
    }
}


