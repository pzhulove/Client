using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    public class AnniversaryBuffPrayItem : MonoBehaviour,IActivityCommonItem
    {
        [SerializeField]
        private Text mNameTxt;
        [SerializeField]
        private Text mTimeTxt;
        [SerializeField]
        private Text mDesTxt;


        public void Init(uint id, uint activityId, ILimitTimeActivityTaskDataModel data, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            mNameTxt.SafeSetText(data.taskName);
            string taskDes = "";

            if(data.TotalNum==2||data.TotalNum==3)
            {
                if(data.ParamNums!=null&& data.ParamNums2.Count >= 2)
                {
                    int totalNum =(int) data.ParamNums2[0];
                    int curNum = 0;
                    int leftNum = 0;
                    if (data.TotalNum==2)
                    {
                        curNum= CountDataManager.GetInstance().GetCount(CounterKeys.DUNGEON_HELL_TIMES);
                    }else if(data.TotalNum==3)
                    {
                        curNum = CountDataManager.GetInstance().GetCount(CounterKeys.DUNGEON_YUANGU_TIMES);
                    }
                    if(curNum>totalNum)
                    {
                        curNum = totalNum;
                    }
                    leftNum = totalNum - curNum;
                    taskDes = string.Format("{0}({1}/{2})", data.Desc, leftNum, totalNum);
                }
            }
            else
            {
                taskDes = data.Desc;
            }
            mDesTxt.SafeSetText(taskDes);

            //两周年祈福活动
            if (activityId == 1482)
            {
                if (data.TotalNum == 6 || data.TotalNum == 7)
                {
                    mTimeTxt.CustomActive(true);
                }
                else
                {
                    mTimeTxt.CustomActive(false);
                }
            }
            //新年祈福活动
            else if (activityId == 1584)
            {
                if (data.TotalNum == 5)
                {
                    mTimeTxt.CustomActive(true);
                }
                else
                {
                    mTimeTxt.CustomActive(false);
                }
            }
        }
        public void ShowTime(string timeStr)
        {
            mTimeTxt.SafeSetText(timeStr);

        }
        public void UpdateData(ILimitTimeActivityTaskDataModel data)
        {

        }


        public void Destroy()
        {
            Dispose();
            Destroy(gameObject);
        }
        

        public void InitFromMode(ILimitTimeActivityModel data, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {

        }

      
        public void Dispose()
        {
           
        }
       

    }
}
