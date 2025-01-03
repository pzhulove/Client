using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ConsumeRebateActivityView : LimitTimeActivityViewCommon
    {
      
        [SerializeField]
        private Text mHaveConsumePointTxt;
        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            base.Init(model, onItemClick);

            mHaveConsumePointTxt.SafeSetText(string.Format(TR.Value("ConsumePoint_TipContent"), GetCurConsumePoint(model)));

        }

        /// <summary>
        /// 得到当前消费的点卷 通过任务数据下发下来
        /// </summary>
        /// <returns></returns>
        private int GetCurConsumePoint(ILimitTimeActivityModel model)
        {
            int num = 0;
            if(model==null)
            {
                return num;
            }
            for (int i = 0; i < model.TaskDatas.Count; i++)
            {
                ILimitTimeActivityTaskDataModel taskData = model.TaskDatas[i];
                if(taskData==null)
                {
                    return num;
                }
                if(taskData.DoneNum>num)
                {
                    num = (int)taskData.DoneNum;
                }
            }
            return num;
        }
    }
}
