using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComMoneyRewardsTimePanel : MonoBehaviour
    {
        public Text[] mHints1 = new Text[6];
        public Text[] mHints2 = new Text[6];
        public int Length
        {
            get
            {
                return IntMath.Min(mHints1.Length, mHints2.Length);
            }
        }

        void Start()
        {
            var tableItems = TableManager.GetInstance().GetTable<ProtoTable.PremiumLeagueTimeTable>();
            if(null != tableItems)
            {
                int iIdx = 0;
                int iValueBeg = 2;
                int iValueEnd = 7;
                for(int i = iValueBeg; i <= iValueEnd; ++i)
                {
                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.PremiumLeagueTimeTable>(i);
                    if(null == item)
                    {
                        continue;
                    }

                    if (iIdx >= 0 && iIdx < mHints1.Length)
                    {
                        var text = mHints1[iIdx];
                        if (null != text)
                        {
                            text.text = item.Time;
                        }
                    }

                    if (iIdx >= 0 && iIdx < mHints2.Length)
                    {
                        var text = mHints2[iIdx];
                        if (null != text)
                        {
                            text.text = item.Desc;
                        }
                    }

                    ++iIdx;
                }
            }
        }
    }
}