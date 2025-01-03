using System.Collections.Generic;
using Network;
using Protocol;
using ProtoTable;


namespace GameClient
{
    public enum ChijiShopType
    {
        None = -1,      
        Buy = 0,        //购买
        Sell,           //出售
    }

    public class ChijiShopItemDataModel
    {

        //商店类型
        public ChijiShopType ChijiShopType;

        //针对选中
        public int ItemIndex;           //道具的索引
        public bool IsSelected;

        //针对背包中的道具
        public ulong ItemGuid;      //拥有道具的guid

        public int ShopId;      //商店Id；
        public int ShopItemId;  //商品Id
        public ChiJiShopItemTable ShopItemTable;    //商品Id对应的商品数据
        public bool IsSoldOver;     //是否已经购买过（只对商品有左右)
    }

}
