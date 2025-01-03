using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;

namespace GameClient
{
    #region ACCOUNT SHOP

    public enum AccountShopPurchaseType
    {
        NotLimit            = 0,
        RoleBind            = 1,
        AccountBind         = 2,
    }

    //public enum AccountShopTabType
    //{
    //    None = 0,
    //    Goods,
    //}

    //public enum AccountShopJobType
    //{
    //    None = 0,
    //}

    public enum AccountShopFilterType
    {
        None = 0,
        First = 1,
        Second = 2,
        Count
    }

    public enum AccountShopRefreshType
    {
        None = 0,           //不刷新
        EachMonth,          //每月
        EachWeekend,
        EachDay,
        EachSeason,         // 每赛季
    }
    /*

    public class AccountShopLocalItemInfoDicByTabType
    {
        public Dictionary<int, AccountShopLocalItemInfoDicByJobType> tabTypeShopItemInfoDic;

        public AccountShopLocalItemInfoDicByTabType()
        {
            tabTypeShopItemInfoDic = new Dictionary<int, AccountShopLocalItemInfoDicByJobType>();
        }

        public void Clear()
        {
            if (tabTypeShopItemInfoDic != null)
            {
                var enumerator = tabTypeShopItemInfoDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var tempDic = enumerator.Current.Value as AccountShopLocalItemInfoDicByJobType;
                    if (tempDic == null) continue;
                    tempDic.Clear();
                }
                tabTypeShopItemInfoDic.Clear();
            }
        }
    }

    public class AccountShopLocalItemInfoDicByJobType
    {
        public Dictionary<int, List<AccountShopLocalItemInfo>> jobTypeShopItemInfoDic;

        public AccountShopLocalItemInfoDicByJobType()
        {
            jobTypeShopItemInfoDic = new Dictionary<int, List<AccountShopLocalItemInfo>>();
        }
        public void Clear()
        {
            if (jobTypeShopItemInfoDic != null)
            {
                jobTypeShopItemInfoDic.Clear();
            }
        }
    }

    public class AccountShopLocalItemInfo
    {
        public int ShopId;
        public AccountShopTabType TabType;
        public AccountShopJobType JobType;
        public int ShopItemId;
        public string ShopItemName;
        public int ItemDataId;
        public int ItemNum;
        public List<ItemReward> CostItems;
        public int AccountLimitBuyNum;
        public int AccountRestBuyNum;
        public int RoleLimitBuyNum;
        public int RoleRestBuyNum;
        public int ExtendParam1;
        public bool NeedPreviewBtn;

        public AccountShopLocalItemInfo()
        {
            CostItems = new List<ItemReward>();
        }

        public void Clear()
        {
            ShopItemName = "";
            if (CostItems != null)
            {
                CostItems.Clear();
            }
        }
    }
    */

    public class AccountShopPurchaseItemInfo
    {
        private uint _shopId;   
        public uint ShopId
        { 
            get { return _shopId; }
        }
        private AccountShopItemInfo _itemInfo = null;
        public AccountShopItemInfo ItemInfo 
        {
            get 
            {
                if (_itemInfo == null)
                {
                    _itemInfo = new AccountShopItemInfo();
                }
                return _itemInfo;
            }
        }

        public AccountShopPurchaseItemInfo(uint shopId, AccountShopItemInfo accShopItemInfo)
        {
            this._shopId = shopId;
            this._itemInfo = accShopItemInfo;
        }

        public AccountShopPurchaseItemInfo(AccountShopItemInfo accShopItemInfo)
        {
            this._shopId = accShopItemInfo.shopId;
            this._itemInfo = accShopItemInfo;
        }
    }

    #endregion
}