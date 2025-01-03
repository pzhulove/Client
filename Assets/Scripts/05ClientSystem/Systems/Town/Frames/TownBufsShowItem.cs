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
    // 城镇buf展示item
    internal class TownBufsShowItem : ComUIListTemplateItem
    {
        [SerializeField]
        Image bufIcon = null;

        [SerializeField]
        Text bufName = null;
        [SerializeField]
        SimpleTimer mTimer;

        [SerializeField]
        Text bufDesc = null;
        private int mEndTime;

        private void Start()
        {
            
        }

        private void OnDestroy()
        {
            
        }

        public override void SetUp(object data)
        {
            TownBufsShowFrame.TownBufInfo townBufInfo = data as TownBufsShowFrame.TownBufInfo;
            if(townBufInfo == null)
            {
                return;
            }
            var table = TableManager.GetInstance().GetTableItem<BuffInfoTable>(townBufInfo.bufID);
            if (null == table)
            {
                Logger.LogErrorFormat("buff信息表中找不到id={0}的buff信息", townBufInfo.bufID);
                return;
            }
            if (null != table.Name && table.Name.Length > 0)
                bufName.SafeSetText(table.Name[0]);
            bufIcon.SafeSetImage(table.Icon);
            if (null != table.Description && table.Description.Length > 0)
                bufDesc.SafeSetText(table.Description[0]);
            mEndTime = townBufInfo.endTime - (int)TimeManager.GetInstance().GetServerTime();
            mTimer.SetCountdown(mEndTime);
        }    
    }
}
