using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;

namespace GameClient
{
    public class FullLevelObject
    {
        public static Regex kvRegex = new Regex(@"<path=([A-Za-z0-9/]+) option=([A-Za-z0-9]+) type=(\d+)>", RegexOptions.Singleline);
        public GameObject gameObject;
        public void Update()
        {
            int iFullLevel = 50;
            var systemValueItem = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_PLAYER_MAX_LEVEL_LIMIT);
            if (systemValueItem != null)
            {
                iFullLevel = systemValueItem.Value;
            }
            bool bFull = PlayerBaseData.GetInstance().Level >= iFullLevel;
            gameObject.CustomActive(bFull);
        }
    }
}
