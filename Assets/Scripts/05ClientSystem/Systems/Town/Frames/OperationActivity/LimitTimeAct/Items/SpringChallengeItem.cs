using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SpringChallengeItem :  MonoBehaviour, IActivityCommonItem
    {
        [SerializeField]private Text mNameTxt;
        [SerializeField]private Text mChallengeDesTxt;
        [SerializeField]private Text mIntegrationTxt;
        [SerializeField]private GameObject mImg1;
        [SerializeField]private GameObject mImg2;

        public void Init(uint id, uint activityId, ILimitTimeActivityTaskDataModel data, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            mNameTxt.SafeSetText(data.taskName);
            mChallengeDesTxt.SafeSetText(data.Desc);

            if (data.ParamNums2.Count >= 2)
            {
                mIntegrationTxt.SafeSetText(TR.Value("SpringChallenge_In", data.ParamNums2[1]));
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

        public void SetBackground(int index)
        {
            mImg1.CustomActive(index % 2 != 0);
            mImg2.CustomActive(index % 2 == 0);
        }
    }
}
