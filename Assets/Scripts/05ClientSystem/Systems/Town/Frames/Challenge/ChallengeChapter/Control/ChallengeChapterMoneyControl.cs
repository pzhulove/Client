using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{
    //消耗品
    public class ChallengeChapterMoneyControl : MonoBehaviour
    {
        private DungeonTable _dungeonTable;

        [Space(10)]
        [HeaderAttribute("Title")]
        [Space(10)]
        [SerializeField] private ComCommonConsume bindTicketConsume;
        [SerializeField] private ComCommonConsume ticketConsume;

        private void Awake()
        {
        }

        private void OnDestroy()
        {
            ClearData();
        }

        private void ClearData()
        {
            _dungeonTable = null;
        }

        public void InitMoneyControl(DungeonTable dungeonTable)
        {

            _dungeonTable = dungeonTable;
            
            if(_dungeonTable == null)
                return;

            InitChapterMoney();
        }

        private void InitChapterMoney()
        {
            switch (_dungeonTable.SubType)
            {
                //深渊相关
                case DungeonTable.eSubType.S_HELL:
                case DungeonTable.eSubType.S_HELL_ENTRY:
                case DungeonTable.eSubType.S_LIMIT_TIME_HELL:
                case DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL:
                    SetMoneyInfo(ChallengeDataManager.GetInstance().BindDeepTicket,
                        ChallengeDataManager.GetInstance().DeepTicket);
                    break;
                case DungeonTable.eSubType.S_WEEK_HELL_PER:
                    SetMoneyInfo(0, 0);
                    break;
                case DungeonTable.eSubType.S_WEEK_HELL:
                case DungeonTable.eSubType.S_WEEK_HELL_ENTRY:
                    SetWeekHellMoneyInfo();
                    break;
                case DungeonTable.eSubType.S_YUANGU:
                    SetMoneyInfo(ChallengeDataManager.GetInstance().BindAncientTicket,
                        ChallengeDataManager.GetInstance().AncientTicket);
                    break;
            }
        }

        private void SetMoneyInfo(int bindTicketId, int ticketId)
        {
            if (bindTicketConsume != null)
            {
                if (bindTicketId <= 0)
                {
                    bindTicketConsume.gameObject.CustomActive(false);
                }
                else
                {
                    bindTicketConsume.gameObject.CustomActive(true);
                    bindTicketConsume.SetData(ComCommonConsume.eType.Item,
                        ComCommonConsume.eCountType.Fatigue,
                        bindTicketId);
                }
            }

            if (ticketConsume != null)
            {
                if (ticketId <= 0)
                {
                    ticketConsume.gameObject.CustomActive(false);
                }
                else
                {
                    ticketConsume.gameObject.CustomActive(true);
                    ticketConsume.SetData(ComCommonConsume.eType.Item,
                        ComCommonConsume.eCountType.Fatigue,
                        ticketId);
                }
            }
        }

        private void SetWeekHellMoneyInfo()
        {
            var bindTeamTicketNumber = ItemDataManager.GetInstance()
                .GetOwnedItemCount(ChallengeDataManager.GetInstance().BindTeamTicket, false);

            //BindTeamTicket 混沌凭证数量不大于0的时候，不显示
            if (bindTeamTicketNumber <= 0)
                SetMoneyInfo(0, ChallengeDataManager.GetInstance().TeamTicket);
            else
                SetMoneyInfo(ChallengeDataManager.GetInstance().BindTeamTicket,
                    ChallengeDataManager.GetInstance().TeamTicket);
        }


    }
}
