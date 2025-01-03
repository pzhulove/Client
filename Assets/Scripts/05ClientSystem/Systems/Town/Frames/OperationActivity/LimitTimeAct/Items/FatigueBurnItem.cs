using System.Collections.Generic;
using ActivityLimitTime;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class FatigueBurnItem : ActivityItemBase
    {
        [SerializeField] private Text mTextName;
        [SerializeField] private Text mTextDescription;
        [SerializeField] private Text mTextGold;
        [SerializeField] private Text mTextTime;
        [SerializeField] private Image mImageGoldIcon;
        [SerializeField] private Button mButtonOpen;
        [SerializeField] private Button mButtonFreeze;
        [SerializeField] private Button mButtonThaw;
        [SerializeField] private SetButtonGrayCD mButtonCD;
        [SerializeField] private Color mColorAdvance;
        [SerializeField] private Color mColorNormal;
        [SerializeField] GameObject mOpenPanel;

        private int mLastTime = -1;//用于燃烧结束时间戳

        enum Operation
        {
            Open,//开启
            Freeze,//冻结
            Thaw//解冻
        }

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            if (data == null)
            {
                return;
            }
            CancelInvoke("_UpdateTime");
            switch (data.State)
            {
                case OpActTaskState.OATS_INIT:
                case OpActTaskState.OATS_UNFINISH:
                    mOpenPanel.CustomActive(true);
                    mTextTime.CustomActive(false);
                    mButtonFreeze.CustomActive(false);
                    mButtonThaw.CustomActive(false);
                    break;
                case OpActTaskState.OATS_FINISHED:
                    mLastTime = (int)data.DoneNum;
                    mButtonFreeze.CustomActive(true);
                    mTextTime.CustomActive(true);
                    mButtonThaw.CustomActive(false);
                    mOpenPanel.CustomActive(false);
                    InvokeRepeating("_UpdateTime", 0, 1);
                    break;
                case OpActTaskState.OATS_FAILED:
                    mButtonFreeze.CustomActive(false);
                    mTextTime.CustomActive(true);
                    mButtonThaw.CustomActive(true);
                    mOpenPanel.CustomActive(false);
                    mTextTime.SafeSetText(Function.GetLastsTimeStr(data.DoneNum));
                    break;
            }
            mTextDescription.SafeSetText(data.Desc.Replace("\\n", "\n"));
        }

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            var paramNums = data.ParamNums;
            int itemId = (int)paramNums[0];
            var tableData = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);

            if (paramNums.Count > 2)
            {
                mTextGold.SafeSetText(TR.Value("activity_fatigue_burn_cost_count", paramNums[1]));

                if (tableData != null)
                {
                    ETCImageLoader.LoadSprite(ref mImageGoldIcon, tableData.Icon);
                }
            }
            //第三个参数 1是高级精力燃烧，其他是普通燃烧
            if (paramNums.Count > 3)
            {
                bool isAdvance = paramNums[3] == 1;
                if (isAdvance)
                {
                    mTextName.color = mColorAdvance;
                    mTextName.SafeSetText(TR.Value("activity_fatigue_burn_name_advance"));
                }
                else
                {
                    mTextName.color = mColorNormal;
                    mTextName.SafeSetText(TR.Value("activity_fatigue_burn_name_normal"));
                }
            }
            mButtonOpen.SafeAddOnClickListener(_OnOpenClick);
            mButtonFreeze.SafeAddOnClickListener(_OnFreezeClick);
            mButtonThaw.SafeAddOnClickListener(_OnThawClick);
        }

        public override void Dispose()
        {
            base.Dispose();
            mButtonOpen.SafeRemoveOnClickListener(_OnOpenClick);
            mButtonFreeze.SafeRemoveOnClickListener(_OnFreezeClick);
            mButtonThaw.SafeRemoveOnClickListener(_OnThawClick);
        }

        void _UpdateTime()
        {
            if (mLastTime - (int)TimeManager.GetInstance().GetServerTime() > 0)
            {
                mTextTime.SafeSetText(Function.GetLastsTimeStr(mLastTime - (int)TimeManager.GetInstance().GetServerTime()));
                mTextTime.CustomActive(true);
            }
            else
            {
                mTextTime.CustomActive(false);
                CancelInvoke("_UpdateTime");
            }
        }

        void _OnOpenClick()
        {
            _OnOperation(Operation.Open);
        }

        void _OnFreezeClick()
        {
           _OnOperation(Operation.Freeze);
        }

        void _OnThawClick()
        {
            _OnOperation(Operation.Thaw);
        }

        void _OnOperation(Operation operation)
        {
            if (mOnItemClick != null)
            {
                mOnItemClick((int)mId, (int)operation);
            }
            mButtonCD.StartGrayCD();
        }
    }
}
