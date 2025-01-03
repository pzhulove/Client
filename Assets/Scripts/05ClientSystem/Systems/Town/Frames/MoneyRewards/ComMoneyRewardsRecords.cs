using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class AttachInfo
    {
        public int attachFlag;
        public string attachWords;
    }

    class ComMoneyRewardsRecordsData
    {
        public int iIndex;
        public uint time;
        public ulong srcId;
        public ulong tarId;
        public string srcName;
        public string tarName;
        public int srcBeatCount;
        public int dstBeatCount;
        public int scoreChanged;
        public bool measured = false;
        public float h;
        public float w;
        public string saveValue = string.Empty;
        public bool selfRelation
        {
            get
            {
                return srcId == PlayerBaseData.GetInstance().RoleID;
            }
        }

        public bool HasSelfInfo
        {
            get
            {
                return srcId == PlayerBaseData.GetInstance().RoleID ||
                    tarId == PlayerBaseData.GetInstance().RoleID;
            }
        }

        public string ToLeftName()
        {
            if(srcId == PlayerBaseData.GetInstance().RoleID)
            {
                return string.Format("<color=#00ff00>我</color>");
            }
            return string.Format("<color=#ffff00>{0}</color>",srcName);
        }
        public string ToRightName()
        {
            if (tarId == PlayerBaseData.GetInstance().RoleID)
            {
                return string.Format("<color=#00ff00>我</color>");
            }
            return string.Format("<color=#ffff00>{0}</color>", tarName);
        }

        public string ToRecords()
        {
            if(selfRelation)
            {
                if (dstBeatCount <= 1)
                {
                    if (srcBeatCount <= 1)
                    {
                        return TR.Value("money_rewards_records_self_once", new object[] { ToLeftName(), ToRightName(), TR.Value("money_rewards_my_score_add", scoreChanged) });//M fight b !
                    }
                    else
                    {
                        return TR.Value("money_rewards_records_self_mul", new object[] { ToLeftName(), ToRightName(), _ToWinTimes(srcBeatCount), TR.Value("money_rewards_my_score_add", scoreChanged) });//M fight b , already x wins !
                    }
                }
                else
                {
                    if (srcBeatCount <= 1)
                    {
                        return TR.Value("money_rewards_records_self_once_break", new object[] { ToLeftName(), ToRightName(), _ToWinTimes(dstBeatCount), TR.Value("money_rewards_my_score_add", scoreChanged) });//a over b x wins!
                    }
                    else
                    {
                        return TR.Value("money_rewards_records_self_mul_break", new object[] { ToLeftName(), ToRightName(), _ToWinTimes(dstBeatCount), _ToWinTimes(srcBeatCount), TR.Value("money_rewards_my_score_add", scoreChanged) });//a over b x wins, already y wins !
                    }
                }
            }
            else
            {
                if(dstBeatCount <= 1)
                {
                    if (srcBeatCount <= 1)
                    {
                        return TR.Value("money_rewards_records_other_once", ToLeftName(), ToRightName());//a fight b !
                    }
                    else
                    {
                        return TR.Value("money_rewards_records_other_mul", new object[] { ToLeftName(), ToRightName(), _ToWinTimes(srcBeatCount) });//a fight b , already x wins !
                    }
                }
                else
                {
                    if (srcBeatCount <= 1)
                    {
                        return TR.Value("money_rewards_records_other_once_break", new object[] { ToLeftName(), ToRightName(), _ToWinTimes(dstBeatCount) });//a over b x wins!
                    }
                    else
                    {
                        return TR.Value("money_rewards_records_other_mul_break", new object[] { ToLeftName(), ToRightName(), _ToWinTimes(dstBeatCount), _ToWinTimes(srcBeatCount) });//a over b x wins, already y wins !
                    }
                }
            }

            return string.Empty;
        }

        public static string[] ms_num_map = new string[10]
            {
                "零","一","二","三","四","五","六","七","八","九",
            };

        static AttachInfo[] attachs = new AttachInfo[3]
            {
                new AttachInfo
                {
                     attachFlag = 1022,
                     attachWords = string.Empty,
                },
                new AttachInfo
                {
                     attachFlag = 1021,
                     attachWords = "十",
                },
                new AttachInfo
                {
                     attachFlag = 1023,
                     attachWords = "百",
                },
            };


        public static string _ToWinTimes(int times)
        {
            string words = string.Empty;
            if(0 == times)
            {
                words = ms_num_map[0];
            }
            else
            {
                int iBit = 0;
                while (times > 0 && iBit < attachs.Length)
                {
                    var attach = attachs[iBit];
                    int iRes = times % 10;
                    if(!string.IsNullOrEmpty(attach.attachWords) && iRes != 0)
                    {
                        words = attach.attachWords + words;
                    }
                    if((attach.attachFlag & (1 << iRes)) == (1 << iRes))
                    {
                        words = ms_num_map[iRes] + words;
                    }
                    times /= 10;
                    ++iBit;
                }
            }
            return words;
        }
    }

    class ComMoneyRewardsRecords : MonoBehaviour
    {
        public Text content;

        ComMoneyRewardsRecordsData data = null;
        public void OnItemVisible(ComMoneyRewardsRecordsData value)
        {
            data = value;
            if(null != data)
            {
                if(null != content)
                {
                    content.text = data.saveValue;
                }
            }
        }

        public void OnDestroy()
        {
            data = null;
        }
    }
}