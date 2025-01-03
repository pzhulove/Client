using DataModel;
using ProtoTable;
using Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    namespace ActivityTreasureLottery
    {
        /// <summary>
        /// 我的夺宝分页
        /// </summary>
        public class ActivityTreasureLotteryHistroyView : ActivityTreasureLotteryActivityViewBase<IActivityTreasureLotteryHistoryModel>
        {
            #region serialize field
            [SerializeField] private Text mTextNoRecord;
            [SerializeField] private GameObject mOtherGroup;
            [SerializeField] private ComUIListScript mScrollList;
            #endregion

            private IActivityTreasureLotteryHistoryModel mModel;

            protected override void OnInit()
            {
                mScrollList.Initialize();
                mScrollList.onItemVisiable = OnWinnerItemVisible;
            }

            protected override void OnDispose()
            {
                mScrollList.onItemVisiable = null;
            }

            void OnWinnerItemVisible(ComUIListElementScript item)
            {
                if (item != null && mModel != null && mModel.PlayerList != null && item.m_index >= 0 && item.m_index < mModel.PlayerList.Length)
                {
                    var text = item.gameObject.GetComponent<Text>();
                    if (text == null)
                    {
                        return;
                    }

                    text.text = "";
                    var data = mModel.PlayerList[item.m_index];
                    if (data.IsPlayer)
                    {
                        text.SafeSetText(TR.Value("activity_treasure_history_player_win_color"));
                    }

                    text.SafeSetText(text.text + string.Format(TR.Value("activity_treasure_history_winner_info").Replace("\\n", "\n"), data.GroupId, data.PlatformName, data.ServerName, data.Name, data.TotalInvestment, mModel.CurrencyName));
                    if (data.IsPlayer)
                    {
                        text.SafeSetText(text.text + "</color>");
                    }

                    if (item.m_index != mModel.PlayerList.Length - 1)
                    {
                        text.SafeSetText(text.text + "\n");
                    }

                }

            }

            protected override void OnSelectItem(IActivityTreasureLotteryHistoryModel model)
            {
                mModel = model;
                if (model != null )
                {
                    if (model.PlayerList != null && model.PlayerList.Length > 0)
                    {
                        mScrollList.SetElementAmount(model.PlayerList.Length);
                        mOtherGroup.CustomActive(!model.IsSellOut);
                    }
                    else
                    {
                        mScrollList.SetElementAmount(0);
                        mTextNoRecord.SafeSetText(TR.Value("activity_treasure_history_no_record"));
                        mOtherGroup.CustomActive(false);
                    }

                }
            }

        }
    }
}