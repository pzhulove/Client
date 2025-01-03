using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class FatigueForBuffItem : ActivityItemBase
    {
        [SerializeField] private Text mTextDescription;
        //[SerializeField] private Text mTextProgress;
        [SerializeField] private Text mTextAim;
        [SerializeField] private UIGray mGray;
        //[SerializeField] private Image mImageBuff;
        [SerializeField] private Text mTextBuffDescription;

        [SerializeField] GameObject mHasTakenReward;
        [SerializeField] GameObject mNotFinishGO;
        [SerializeField] GameObject mReplacedGo;

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            switch (data.State)
            {
                case OpActTaskState.OATS_OVER:
                    mGray.SetEnable(true);
                    mNotFinishGO.CustomActive(false);
                    mHasTakenReward.CustomActive(false);
                    mReplacedGo.CustomActive(true);
                    break;
                case OpActTaskState.OATS_FINISHED:
                    mGray.SetEnable(false);
                    mNotFinishGO.CustomActive(false);
                    mHasTakenReward.CustomActive(true);
                    mReplacedGo.CustomActive(false);
                    break;
                default:
                    mGray.SetEnable(true);
                    mNotFinishGO.CustomActive(true);
                    mHasTakenReward.CustomActive(false);
                    mReplacedGo.CustomActive(false);
                    break;
            }
            mTextDescription.SafeSetText(TR.Value("activity_fatigue_for_buff_item_desp", data.DoneNum, data.TotalNum));
            mTextAim.SafeSetText(data.TotalNum.ToString());
            
	        if (data.ParamNums != null && data.ParamNums.Count > 0)
	        {
		        int buffID = (int)data.ParamNums[0];
		        var buffTable = TableManager.GetInstance().GetTableItem<BuffTable>(buffID);
		        if (buffTable == null)
		        {
			        return;
		        }

		        //ETCImageLoader.LoadSprite(ref mImageBuff, buffTable.Icon);
		        mTextBuffDescription.SafeSetText(buffTable.Name);
	        }
		}

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {

        }

    }
}
