using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class ShopNewParamData
    {
        public int ShopId;

        //shopChildId 和 ShopItemType 用于设置商店的主按钮，只能有一个有意义
        public int ShopChildId;
        public int ShopItemType;

        public int NpcId;
        public int shopItemId;
    }

    public class ShopNewFrame : ClientFrame
    {

        //param : 
        //[0] : 商店表中的商店ID
        //[1] : 子商店选中的ID，如果对应商店没有子商店，则默认为0(对应商店的主页签）
        //[2] : SubType的类型，如果存在子商店，则该参数默认为0；如果不存在自商店，则表示选中的页签（对应商店的主页签）	
        //<type= framename param= 24 | 14 | 0 value= GameClient.ShopNewFrame >
        //< type = framename param= 25 | 0 | 13 value= GameClient.ShopNewFrame >

        //超链接跳转到商城
        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                var strParamArray = strParam.Split('|');
                //默认跳转的参数为3个
                if (strParamArray.Length < 3)
                {
                    Logger.LogErrorFormat("ShopNewFrame.OpenLinkFrame paramArrayLength is not Right");
                    return;
                }

                var shopId = int.Parse(strParamArray[0]);
                var shopChildrenId = int.Parse(strParamArray[1]);
                var shopItemType = int.Parse(strParamArray[2]);
                var shopItemId = 0;
                if (strParamArray.Length > 3)
                    shopItemId  = int.Parse(strParamArray[3]);
                //判断商城的ShopTable是否存在
                var shopTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
                if (shopTable == null)
                {
                    //不存在，直接返回
                    Logger.LogErrorFormat("ShopNewFrame OpenLinkFrame The shopTable is null and shop id is {0}", shopId);
                    return;
                }

                //判断商店的等级是否满足
                if (PlayerBaseData.GetInstance().Level < shopTable.OpenLevel)
                {
                    //等级不满足，飘字，直接返回
                    var exchangeNotOpenTip = string.Format(TR.Value("exchange_mall_not_open"), shopTable.OpenLevel
                        , shopTable.ShopName);
                    SystemNotifyManager.SysNotifyFloatingEffect(exchangeNotOpenTip);
                    return;
                }

                //等级满足，打开商城
				
				//帐号绑定商店
                if (shopTable.BindType == ShopTable.eBindType.ACCOUNT_BIND)
                {
                    if (AccountShopDataManager.GetInstance().CheckHasChildShop(shopId))
                    {
                        AccountShopDataManager.GetInstance().OpenAccountShop(shopId, shopChildrenId, 0, -1, shopItemId);
                    }
                    else
                    {
                        AccountShopDataManager.GetInstance().OpenAccountShop(shopId, 0, shopItemType, -1, shopItemId);
                    }
                    return;
                }
				//其他绑定商店
                ShopNewDataManager.GetInstance().OpenShopNewFrame(shopId, shopChildrenId, shopItemType, -1, shopItemId);
            }
            catch (System.Exception e)
            {
                Logger.LogErrorFormat("ShopNewFrame.OpenLinkFrame:=>{0}" + e.ToString());
            }
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ShopNew/ShopNewFrame";
        }


        //private static int _testItemType = 2;
        //private static int _testShopChildId = 13;
        protected override void _OnOpenFrame()
        {
            //var testItemType = 0;
            //if (shopId == 23)
            //{
            //    testItemType = _testItemType;
            //    _testItemType += 1;
            //}
            //if (_testItemType > 6)
            //{
            //    _testItemType = 2;
            //}

            //var testShopChildId = 0;
            //if (shopId == 24)
            //{
            //    testShopChildId = _testShopChildId;
            //    _testShopChildId += 1;
            //}

            //if (_testShopChildId > 17)
            //{
            //    _testShopChildId = 13;
            //}

            base._OnOpenFrame();

            var shopNewParamData = userData as ShopNewParamData;
            
            if (mShopNewView != null)
                mShopNewView.InitShop(shopNewParamData);

        }

        #region ExtraUIBind
        private ShopNewView mShopNewView = null;

        protected override void _bindExUI()
        {
            mShopNewView = mBind.GetCom<ShopNewView>("ShopNewView");
        }

        protected override void _unbindExUI()
        {
            mShopNewView = null;
        }
        #endregion
        

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnCloseShopFrame);
            ShopNewDataManager.GetInstance().ClearData();
        }


    }

}
