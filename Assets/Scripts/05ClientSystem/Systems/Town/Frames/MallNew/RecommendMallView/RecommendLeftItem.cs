using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Protocol;
using ProtoTable;

namespace GameClient
{
    public class RecommendLeftItem : MonoBehaviour
    {
        [SerializeField] private Image mImg;
        private MallRecommendPageInfo mInfo;
        public void OnInit(MallRecommendPageInfo info)
        {
            mInfo = info;
            mImg.SafeSetImage(mInfo.adImagePath);
        }

        public void OnClick()
        {
            //如果是链接
            if (mInfo.linkFunctionType == (byte)MallRecommendPageTable.eLinkFunctionType.LINK_FUNCTION_TYPE_LINK)
            {
                ActiveManager.GetInstance().OnClickLinkInfo(mInfo.linkPath);
            }
            //购买
            else if (mInfo.linkFunctionType == (byte)MallRecommendPageTable.eLinkFunctionType.LINK_FUNCTION_TYPE_BUY)
            {
                // 走统一接口
                ShopNewDataManager.GetInstance().OpenBuyFrame(mInfo);
            }
        }
    }
}
