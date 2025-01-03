using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class QiXiQueQiaoItem : ActivityItemBase
    {
        [SerializeField] private Text mTextDescription;
        [SerializeField] private RectTransform mRewardItemRoot;
        [SerializeField] Vector2 mComItemSize = new Vector2(60f, 60f);
        [SerializeField] private GameObject mEffect;
        [SerializeField]
        private Text mNameDesTxt;
        private ComItem mComItem;

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            mEffect.CustomActive(false);

            if (data != null && data.State == OpActTaskState.OATS_FINISHED)
            {
                mEffect.CustomActive(true);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (mComItem != null)
            {
                ComItemManager.Destroy(mComItem);
                mComItem = null;
            }
        }

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            //只能有一个奖励
            if (data != null)
            {
                if (data.AwardDataList != null && data.AwardDataList.Count >= 1)
                {
                    mComItem = ComItemManager.Create(mRewardItemRoot.gameObject);
                    ItemData item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[0].id);
                    if (item == null)
                    {
                        return;
                    }
                    item.Count = (int)data.AwardDataList[0].num;
                    mComItem.Setup(item, Utility.OnItemClicked);
                    string nameStr = string.Format("{0}*{1}", item.Name , item.Count);
                    mNameDesTxt.SafeSetText(nameStr);
                    //((RectTransform) mComItem.transform).sizeDelta = mComItemSize;
                }

            }
            //倒着显示.....
            if (data.DataId == 1445001 || data.DataId == 1445002 || data.DataId == 1445003 || data.DataId == 1445004 || data.DataId == 1445005)
            {
                GetComponent<RectTransform>().SetAsFirstSibling();
            }
         
            mTextDescription.SafeSetText(data.Desc);
        }

    }
}
