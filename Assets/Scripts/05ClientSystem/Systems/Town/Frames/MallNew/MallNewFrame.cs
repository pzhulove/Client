using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum MallType
    {
        None = -1,
        TicketMall = 0,
        BindTicketMall,
        FashionMall,
        Gift,
        Shop_begin,
        Shop_end,
        Recharge,
    }

    public enum FashionMallMainIndex
    {
        None = -1,
        FashionAll = 0,
        FashionOne = 1,
        Count,
    }

    public struct MallItemDetailInfo
    {
        public UInt32 id;
        public UInt32 itemId;
        public UInt16 num;
        public UInt32 price;
        public UInt32 discountPrice;
        public byte moneytype;
        public EFashionWearSlotType WearSlot;
        public bool BelongSuit;
        public bool isLimit;
        public int leftNum;
        public int MallTableIndex;

        public void Cleardata()
        {
            id = 0;
            itemId = 0;
            num = 0;
            price = 0;
            discountPrice = 0;
            moneytype = 0;
            WearSlot = EFashionWearSlotType.Invalid;
            BelongSuit = false;
            MallTableIndex = -1;
        }
    }

    [Serializable]
    public struct OutComeData
    {
        public MallType MainTab;
        public int SubTab;
        public int ThirdTab;

        public void ClearData()
        {
            MainTab = MallType.TicketMall;
            SubTab = 0;
            ThirdTab = 0;
        }
    }

    //打开商城界面的时候，参数数据
    public class MallNewFrameParamData
    {
        public MallNewType MallNewType;
        public int Index;
        public int SecondIndex;
        public int ThirdIndex;

        public MallNewFrameParamData()
        {
            MallNewType = MallNewType.RecommendMall;
            Index = 0;
            SecondIndex = 0;
            ThirdIndex = 0;
        }
    }

    //优化后商城的类型
    public enum MallNewType
    {
        None = -1,
        PropertyMall = 0,       //道具商城
        LimitTimeMall = 1,      //限时商城
        FashionMall = 2,        //时装商城
        ExChangeMall = 3,       //兑换商店
        ReChargeMall = 4,       //充值商城
        IntegralMall = 5,       //积分商城
        RecommendMall = 6,       //推荐商城
    }
    
    public class MallNewFrame : ClientFrame
    {
        public static int DefaultMainTabIndex = 1;       //主页签索引
        public static int DefaultIndex = 0;     //默认页签的索引
        public static int SecondIndex = 0;      //二级页签的索引
        public static int ThirdIndex = 0;       //三级页签的索引

        //超链接：
        //0："0|0","0|1","0|2","0|3", 道具商城tab和子tab   ext: "0|tabId|0|道具id"默认选中道具
        //1："1|0", 限时商城的tab。子tab无意义
        //2："2|0","2|1","2|2","2|3", "2|4",时装商城tab和子tab：子tab对应人物的职业：（战士，枪手，魔术师等).
                //2(时装商城页签）| 0 （代表职业，0 表示本职业）| 0,1，2（套装，单品，武器）| 0,1,2,3,4（位置）| 
        //"2|0|1|0", 单品 头
        //"2|0|1|1",
        //"2|0|1|2",
        //"2|0|1|3",
        //"2|0|1|4",
        //3："3|0", 兑换商城的tab，子tab无意义
        //4："4|0", 充值商城的tab，子tab无意义
        public static void OpenLinkFrame(string strParam)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<MallNewFrame>())
                ClientSystemManager.GetInstance().CloseFrame<MallNewFrame>();
            try
            {
                var tabIndexData = strParam.Split(new char[] { '|' });
                //没有参数应该打开默认选项
                // if (tabIndexData.Length <= 0)
                //     return;

                // if ((MallNewType) int.Parse(tabIndexData[0]) == MallNewType.FashionMall)
                // {
                //     SystemNotifyManager.SysNotifyFloatingEffect("时装商城尚未开放");
                //     return;
                // }
                var mallNewFrameParamData = new MallNewFrameParamData();
                if (tabIndexData.Length > 0)
                    mallNewFrameParamData.MallNewType = (MallNewType) int.Parse(tabIndexData[0]);
                if (tabIndexData.Length > 1)
                    mallNewFrameParamData.Index = int.Parse(tabIndexData[1]);
                if (tabIndexData.Length > 2)
                    mallNewFrameParamData.SecondIndex = int.Parse(tabIndexData[2]);
                if (tabIndexData.Length > 3)
                    mallNewFrameParamData.ThirdIndex = int.Parse(tabIndexData[3]);
                ClientSystemManager.GetInstance().OpenFrame<MallNewFrame>(FrameLayer.Middle, mallNewFrameParamData);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("MallNewFrame OpenLinkFrame throw exception {0}", e.ToString());
            }
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MallNew/MallNewFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mMallNewView != null)
            {
                //初始化并获得参数
                MallNewFrameParamData mallNewFrameParamData = null;
                if (userData != null)
                {
                    mallNewFrameParamData = (MallNewFrameParamData) userData;
                }
                InitDefaultTabData(mallNewFrameParamData);

                //InitView
                mMallNewView.InitView();
            }
        }


        private void InitDefaultTabData(MallNewFrameParamData paramData)
        {

            // MallNewFrame.DefaultMainTabIndex = (int)MallNewType.RecommendMall;
            MallNewFrame.DefaultMainTabIndex = (int)MallNewType.LimitTimeMall;
            MallNewFrame.DefaultIndex = 0;
            MallNewFrame.SecondIndex = 0;
            MallNewFrame.ThirdIndex = 0;

            if(paramData == null)
                return;

            //约束参数的边界条件
            //类型的边界条件
            if (paramData.MallNewType > MallNewType.RecommendMall || paramData.MallNewType < MallNewType.PropertyMall)
                paramData.MallNewType = MallNewType.LimitTimeMall;
                // paramData.MallNewType = MallNewType.RecommendMall;



            MallNewFrame.DefaultMainTabIndex = (int) paramData.MallNewType;
            MallNewFrame.DefaultIndex = paramData.Index;
            MallNewFrame.SecondIndex = paramData.SecondIndex;
            MallNewFrame.ThirdIndex = paramData.ThirdIndex;
        }

        protected override void _OnCloseFrame()
        {
            //关闭商城的时候，清空商城相关的数据
            // MallNewDataManager.GetInstance().ClearData();
            MallNewFrame.CloseMallPayFrame();
            MallNewFrame.DefaultMainTabIndex = 0;
            MallNewFrame.DefaultIndex = 0;
            MallNewFrame.SecondIndex = 0;
            MallNewFrame.ThirdIndex = 0;
            //关闭界面的时候，刷新宠物口粮
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdatePetFoodNum);
        }

        #region ExtraUIBind
        private MallNewView mMallNewView = null;

        protected override void _bindExUI()
        {
            mMallNewView = mBind.GetCom<MallNewView>("MallNewView");
        }

        protected override void _unbindExUI()
        {
            mMallNewView = null;
        }
        #endregion

        public static void OpenMallPayFrame()
        {
            CloseMallPayFrame();
            ClientSystemManager.GetInstance().OpenFrame<MallPayFrame>(FrameLayer.Middle, VipTabType.PAY);
        }

        public static void CloseMallPayFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<MallPayFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MallPayFrame>();
            }
        }
    }

}
