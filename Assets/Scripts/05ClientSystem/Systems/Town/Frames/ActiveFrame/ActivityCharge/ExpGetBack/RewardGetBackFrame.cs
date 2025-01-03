using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    public class RewardGetBackFrame : ClientFrame
    {
        private RewardGetBackFrameView mView;
        GetBackFrameParam mParam;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/Activities/RewardGetBackFrame";
        }

        protected override void _OnOpenFrame()
        {
            mCount = 0;
            var data = userData as GetBackFrameParam;
            if (data == null || frame == null)
            {
                Close();
            }
            else
            {
                mParam = data;
                mView = frame.GetComponent<RewardGetBackFrameView>();
                if (mView != null && data.ActiveData != null && data.ActiveData.OriginData != null && data.ActiveData.OriginData.activeItem != null)
                {
                    int costId = data.IsPerfect ? data.ActiveData.PerfectCostItemId : data.ActiveData.NormalCostItemId;
                    int costNum = data.IsPerfect ? data.ActiveData.PerfectCostNum : data.ActiveData.NormalCostNum;
                    string title = TR.Value("activity_reward_get_back_title", data.ActiveData.OriginData.activeItem.Param0);
                    List<int> rewardIds = data.IsPerfect ? data.ActiveData.PerfectRewardItemIds : data.ActiveData.NormalRewardIds;
                    List<int> rewardNums = data.IsPerfect ? data.ActiveData.PerfectRewardNums : data.ActiveData.NormalRewardNums;
                    mView.Init(title, data.ActiveData.Count, rewardIds, rewardNums, costId, costNum, _OnConfirmClick, _OnCloseClick);
                }
                else
                {
                    Close();
                }
            }
        }

        private void _OnCloseClick()
        {
            Close();
        }

        private int mCount;
        private void _OnConfirmClick(int count)
        {
            mCount = count;
            if (!mParam.IsPerfect && ActiveManager.GetInstance().IsNotifyNormalGetBack)
            {
                ClientSystemManager.GetInstance().OpenFrame<RewardGetBackConfirmFrame>(FrameLayer.Middle, (Action)_OnGetBack);
            }
            else
            {
                _OnGetBack();
            }
        }

        private void _OnGetBack()
        {
            if (mParam != null && !SecurityLockDataManager.GetInstance().CheckSecurityLock() && mCount <= mParam.ActiveData.Count)
            {
                int costId = mParam.IsPerfect ? mParam.ActiveData.PerfectCostItemId : mParam.ActiveData.NormalCostItemId;
                int costNum = mParam.IsPerfect ? mParam.ActiveData.PerfectCostNum : mParam.ActiveData.NormalCostNum;
                int iTargetID = mParam.ActiveData.OriginData.activeItem.ID % 100000000;
                int iHighMark = mParam.IsPerfect ? 1 : 0;
                int iParam = (iHighMark << 16) | (mCount & 0xFF);
                iParam |= (1 << 31);
                CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = costId, nCount = costNum * mCount }, () =>
                {
                    ActiveManager.GetInstance().SendSubmitActivity(iTargetID, (uint)iParam);
                    frameMgr.CloseFrame(this);
                });
            }
            else
            {
                Close();
            }
        }

    }
}
