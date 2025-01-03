using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComMonthCard : MonoBehaviour
    {
        [SerializeField] private Button buyBtn;
        [SerializeField] private Text buyBtnText;
        [SerializeField] private Image markImg;
        [SerializeField] private Text lastDayText;
        [SerializeField] private Image inactiveImg;

        private PayItemData mCarddata = null;

        private void Awake()
        {
            buyBtn.SafeAddOnClickListener(_onPayCardButtonClick);
        }

        private void OnDestroy()
        {
            buyBtn.SafeRemoveOnClickListener(_onPayCardButtonClick);
        }

        private void _onPayCardButtonClick()
        {
            if (mCarddata == null)
            {
                Logger.LogErrorFormat("Cannot get CardDatas");
                return;
            }
            /* put your code in here */
            MonthCardBuyResult.bBuyMonthCard = true;
            PayManager.GetInstance().lastMontchCardNeedOpenWindow = mCarddata.remainDays <= 0;
            PayManager.GetInstance().DoPay(mCarddata.ID, mCarddata.price);
        }


        public void SetView(PayItemData cardData)
        {
            this.mCarddata = cardData;
            if (mCarddata == null)
            {
                return;
            }
            if (markImg)
            {
                markImg.enabled = mCarddata.HasMark();
            }
            if (mCarddata.remainDays == 0)
            {
                //lastDayText.SafeSetText(TR.Value("vip_month_card_first_buy_cost_unactive")); //"未激活";
                if (lastDayText)
                {
                    lastDayText.enabled = false;
                }
                if (inactiveImg)
                {
                    inactiveImg.enabled = true;
                }
                buyBtnText.SafeSetText(TR.Value("vip_month_card_first_buy_cost_rmb_30"));//"¥ 30 购买";
            }
            else if (mCarddata.remainDays > 0)
            {
                if (lastDayText)
                {
                    lastDayText.enabled = true;
                }
                if (inactiveImg)
                {
                    inactiveImg.enabled = false;
                }
                lastDayText.SafeSetText(TR.Value("vip_month_card_remain_time", (mCarddata.remainDays - 1).ToString()));//月卡剩余{0}天
                buyBtnText.SafeSetText(TR.Value("vip_month_card_first_buy_cost_again_rmb_30"));//"¥ 30 续费";

                if (MonthCardBuyResult.bBuyMonthCard)
                {
                    MonthCardBuyResult.bBuyMonthCard = false;

                    ClientSystemManager.GetInstance().OpenFrame<MonthCardBuyResult>(FrameLayer.Middle, new MonthCardBuyResultUserData
                    {
                        //< color =#ffb400ff>战争主宰护腿</color>
                        strResultInfo = TR.Value("vip_month_card_first_buy_cost_success_remain_days", (mCarddata.remainDays - 1).ToString()), //购买月卡成功，您当前月卡时间剩余...
                        okCallBack = () =>
                        {
                            const int iConfigID = 9380;
                            string frameName = typeof(ActiveChargeFrame).Name + iConfigID.ToString();
                            if (ClientSystemManager.GetInstance().IsFrameOpen(frameName))
                            {
                                var frame = ClientSystemManager.GetInstance().GetFrame(frameName) as ActiveChargeFrame;
                                frame.Close(true);
                            }

                            ActiveManager.GetInstance().OpenActiveFrame(iConfigID, 6000);
                        }
                    });
                }
            }
            else
            {
                Logger.LogErrorFormat("数据有误");
            }
        }
    }
}
