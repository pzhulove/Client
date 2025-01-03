using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class IntegrationChallengeItem :  MonoBehaviour, IActivityCommonItem
    {
        [SerializeField]
        private Text mNameTxt;
        [SerializeField]
        private Text mChallengeTimeTxt;
        [SerializeField]
        private Text mSingleIntegrationTxt;
        [SerializeField]
        private Text mMutilIntegrationTxt;
        public void Init(uint id, uint activityId, ILimitTimeActivityTaskDataModel data, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            mNameTxt.SafeSetText(data.Desc);
            if(data.ParamNums2.Count>=2)
            {
                int num1 =(int) data.ParamNums2[0];
                int num2 = (int)data.ParamNums2[1];
                if(num1>0)
                {
                    mSingleIntegrationTxt.SafeSetText(string.Format(TR.Value("IntegrationChallenge_In", num1)));
                }
                else
                {
                    mSingleIntegrationTxt.SafeSetText("/");
                }
            }

            mMutilIntegrationTxt.SafeSetText(data.taskName);

            if (data.ParamNums.Count>0)
            {
                switch ((OpActivityChallengeType)data.ParamNums[0])
                {
                    case OpActivityChallengeType.OACT_MONSTER_ATTACK:
                        mChallengeTimeTxt.SafeSetText(TR.Value("IntegrationChallenge_Time1"));
                        break;
                    case OpActivityChallengeType.OACT_ELITE_DUNGEON:
                        mChallengeTimeTxt.SafeSetText(TR.Value("IntegrationChallenge_Time2"));
                        break;
                    case OpActivityChallengeType.OACT_ABYESS_DUNGEON:
                        mChallengeTimeTxt.SafeSetText(TR.Value("IntegrationChallenge_Time3"));
                        break;
                    case OpActivityChallengeType.OACT_ASCENT_DUNGEON:
                        mChallengeTimeTxt.SafeSetText(TR.Value("IntegrationChallenge_Time4"));
                        break;
                    case OpActivityChallengeType.OACT_TEAM_DUNGEON:
                        mChallengeTimeTxt.SafeSetText(TR.Value("IntegrationChallenge_Time5"));
                        break;
                    case OpActivityChallengeType.OACT_3V3_PK:
                        mChallengeTimeTxt.SafeSetText(TR.Value("IntegrationChallenge_Time6"));
                        break;
                    case OpActivityChallengeType.OACT_GUILD_BATTLE:
                        mChallengeTimeTxt.SafeSetText(TR.Value("IntegrationChallenge_Time7"));
                        break;
                    case OpActivityChallengeType.OACT_GUILD_DUNGEON:
                        mChallengeTimeTxt.SafeSetText(TR.Value("IntegrationChallenge_Time8"));
                        break;
                    case OpActivityChallengeType.OACT_2V2_PK:
                        mChallengeTimeTxt.SafeSetText(TR.Value("IntegrationChallenge_Time9"));
                        break;
                }
            }
           
        }
        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void Dispose()
        {
           
        }


        public void UpdateData(ILimitTimeActivityTaskDataModel data)
        {

        }

        public void InitFromMode(ILimitTimeActivityModel data, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {

        }
    }
}
