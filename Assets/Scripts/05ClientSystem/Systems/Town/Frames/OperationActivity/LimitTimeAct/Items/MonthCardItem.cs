using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class MonthCardItem : DailyLoginItem
    {
        [SerializeField] private Button mButtonGoToBuy;


        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            base.OnInit(data);
            mButtonGoToBuy.SafeAddOnClickListener(_OnButtonGoToBuyClick);
        }

        public override void Dispose()
        {
            base.Dispose();
            mButtonGoToBuy.SafeAddOnClickListener(_OnButtonGoToBuyClick);
        }

        void _OnButtonGoToBuyClick()
        {
            if (mOnItemClick != null)
            {
                //0表示正常购买，1表示前往商城
                mOnItemClick((int)mId, 1);
            }
        }
    }
}
