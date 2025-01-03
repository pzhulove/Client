using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    // 公会领地奖励领取控制脚本
    internal class GuildManorAwardGetControl : MonoBehaviour
    {
        [SerializeField]
        private Button dailyAward = null;

        [SerializeField]
        private GameObject dailyAwardRedPoint = null;

        [SerializeField]
        private int manorID = 0;

        private void Start()
        {
            dailyAward.SafeSetOnClickListener(() =>
            {
                GuildDataManager.GetInstance().SendWorldGuildGetTerrDayRewardReq();
            });

            OnRedPointChanged(null);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);
        }

        void OnRedPointChanged(UIEvent a_event)
        {
            ERedPoint redPointType = ERedPoint.GuildTerrDayReward;

            bool show = RedPointDataManager.GetInstance().HasRedPoint(redPointType) &&
                    GuildDataManager.GetInstance().HasSelfGuild() &&
                    GuildDataManager.GetInstance().myGuild.nSelfManorID == manorID;

            dailyAwardRedPoint.CustomActive(show);
            dailyAward.CustomActive(show);
        }
    }
}
