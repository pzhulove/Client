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
    // npc对话文本item
    internal class NpcTalkInfoItem : ComUIListTemplateItem
    {
        [SerializeField]
        Text npcName = null;

        [SerializeField]
        Text talkText = null;

        [SerializeField]
        Image icon = null;

        private void Start()
        {
            
        }

        private void OnDestroy()
        {
            
        }

        public override void SetUp(object data)
        {
           if(!(data is int))
            {
                return;
            }

            int tableID = (int)data;
            var table = TableManager.GetInstance().GetTableItem<TalkTable>(tableID);
            if(table == null)
            {
                return;
            }

            var npcTable = TableManager.GetInstance().GetTableItem<NpcTable>(table.NpcID);
            if(npcTable != null)
            {              
                npcName.SafeSetText(npcTable.NpcName.Replace("[UserName]", PlayerBaseData.GetInstance().Name));
            }

            talkText.SafeSetText(table.TalkText.Replace("[UserName]", PlayerBaseData.GetInstance().Name));

            if(npcTable != null)
            {
                if(npcTable.NpcName.IndexOf("[UserName]") != -1) // 玩家
                {
                    string path = "";

                    JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
                    if (jobData != null)
                    {
                        ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                        if (resData != null)
                        {
                            path = resData.IconPath;
                        }
                    }
                    icon.SafeSetImage(path);
                }
                else // NPC
                {
                    icon.SafeSetImage(npcTable.NpcIcon);
                }
            }
        }
    }
}
