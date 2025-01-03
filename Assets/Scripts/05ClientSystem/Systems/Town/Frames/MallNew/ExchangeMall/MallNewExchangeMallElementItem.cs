using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;

namespace GameClient
{
        
    public class MallNewExchangeMallElementItem : MonoBehaviour
    {

        private const int StoneShopId = 24;         //晶石商店，对应商店表中的ID。区别于其他商店的表现
        private ShopTable mShopTable;
        private bool mIsLocked = false;     //是否锁住

        [SerializeField] private Image mallIcon ;
        [SerializeField] private Image mallLock;
        [SerializeField] private UIGray uiGray;
        // [SerializeField] private Image mallPreName;
        [SerializeField] private Text mTextName;
        [SerializeField] private GameObject specialContent;
        [SerializeField] private Text specialContentLabel;
        [SerializeField] private GameObject mObjCoin;
        [SerializeField] private Image mImgCoin;
        [SerializeField] private GameObject mObjCoins;
        [SerializeField] private List<Image> mImgCoins;
        [SerializeField] private Text mTextCoinCount;
        [SerializeField] private GameObject mObjLockTip;
        [SerializeField] private Text mTextLockTip;
        
        // [SerializeField] private Button mallButton;

        private void Awake()
        {
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            // if (mallButton != null)
            // {
            //     mallButton.onClick.RemoveAllListeners();
            //     mallButton.onClick.AddListener(OnMallButtonClicked);
            // }
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCloseShopFrame, _onUpdate);
        }

        private void UnBindUiEventSystem()
        {
            // if (mallButton != null)
            // {
            //     mallButton.onClick.RemoveAllListeners();
            // }
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCloseShopFrame, _onUpdate);

            PlayerBaseData.GetInstance().onLevelChanged -= OnExchangeMallItemLevelChanged;
        }

        private void _onUpdate(UIEvent uiEvent)
        {
            InitShowCoin();
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
        }

        //初始化
        public void InitData(ShopTable shopTable)
        {
            mShopTable = shopTable;

            // 公会商店的id是根据公会商店等级来决定的
            if(shopTable != null && shopTable.ShopKind == ShopTable.eShopKind.SK_Guild && GuildDataManager.GetInstance().HasSelfGuild())
            {
                if(GuildDataManager.GetInstance().myGuild.dictBuildings != null)
                {
                    int needShopLevel = GuildDataManager.GetInstance().myGuild.dictBuildings[GuildBuildingType.SHOP].nLevel;
                    ProtoTable.GuildBuildingTable buildingTable = TableManager.GetInstance().GetTableItem<ProtoTable.GuildBuildingTable>(needShopLevel);
                    if (buildingTable != null)
                    {
                        mShopTable = TableManager.GetInstance().GetTableItem<ShopTable>(buildingTable.ShopId);
                    }
                }
            }
            if(mShopTable == null)
                return;
            PlayerBaseData.GetInstance().onLevelChanged -= OnExchangeMallItemLevelChanged;
            PlayerBaseData.GetInstance().onLevelChanged += OnExchangeMallItemLevelChanged;
            InitElementView();
        }
        
        private void InitElementView()
        {
            UpdateMallElementItemState();
            InitMallPreName();
            InitSpecialContent();

            if (mallIcon != null)
            {
                ETCImageLoader.LoadSprite(ref mallIcon, mShopTable.ExchangeShopShowImage);
            }

        }

        //展示右上角的货币
        private void InitShowCoin()
        {
            mObjCoin.CustomActive(false);
            mObjCoins.CustomActive(false);
            if (mIsLocked)
                return;
            if (null != mShopTable && mShopTable.ShowExchangeCoinItem.Length > 0)
            {
                //只显示一个
                if (1 == mShopTable.ShowExchangeCoinItem.Length)
                {
                    int itemId = mShopTable.ShowExchangeCoinItem[0];
                    var table = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
                    mObjCoin.CustomActive(null != table);
                    if (null != table)
                    {
                        mImgCoin.SafeSetImage(table.Icon);
                        mTextCoinCount.SafeSetText(ItemDataManager.GetInstance().GetOwnedItemCount(itemId).ToString());
                    }
                }
                //显示多个
                else
                {
                    mObjCoins.CustomActive(true);
                    int index = 0;
                    for (; index < mShopTable.ShowExchangeCoinItem.Length; ++index)
                    {
                        if (index >= mImgCoins.Count)
                        {
                            Logger.LogError("商店展示货币太多 显示不下");
                            return;
                        }
                        int itemId = mShopTable.ShowExchangeCoinItem[index];
                        var table = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
                        mImgCoins[index].CustomActive(null != table);
                        if (null != table)
                        {
                            mImgCoins[index].SafeSetImage(table.Icon);
                        }
                    }
                    for (; index < mImgCoins.Count; ++index)
                    {
                        mImgCoins[index].CustomActive(false);
                    }
                }
            }
        }

        //根据商店表，来对应名字。应该由类型来决定，但是表格中不存在相关类型，所以采用了名字来决定
        private void InitMallPreName()
        {

            if(mShopTable == null)// || string.IsNullOrEmpty(mShopTable.ExchangeShopNameImage) == true)
                return;
            mTextName.SafeSetText(mShopTable.ShopName);
            // if(mallPreName == null)
            //     return;

            // ETCImageLoader.LoadSprite(ref mallPreName, mShopTable.ExchangeShopNameImage);

        }

        private void InitSpecialContent()
        {
            if(specialContent == null)
                return;

            if(mShopTable == null)
                return;

            if (mShopTable.ShopKind == ShopTable.eShopKind.SK_Magic)
            {
                //积分商店，显示特殊内容
                specialContent.gameObject.CustomActive(true);
                if (specialContentLabel != null)
                {
                    specialContentLabel.text = TR.Value("exchange_mall_special_description");
                }
            }
            else if(mShopTable.ShopKind == ShopTable.eShopKind.SK_Activity)
            {
                //活动商店，显示起止时间
                specialContent.gameObject.CustomActive(true);
                if (specialContentLabel != null)
                {
                    var activityShopTimeData = ShopNewUtility.GetActivityShopTimeData(mShopTable);
                    if (string.IsNullOrEmpty(activityShopTimeData) == true)
                    {
                        specialContent.gameObject.CustomActive(false);
                    }
                    else
                    {
                        specialContentLabel.text = activityShopTimeData;
                    }
                }
            }
            else if (mShopTable.ShopKind == ShopTable.eShopKind.SK_BlessCrystal)
            {
                specialContent.gameObject.CustomActive(true);
                if (specialContentLabel != null)
                {
                    specialContentLabel.text = TR.Value("adventure_team_shop_blesscrystal_clear_tip");
                }
            }
            else if (mShopTable.ShopKind == ShopTable.eShopKind.SK_AdventureCoin)
            {
                specialContent.gameObject.CustomActive(true);
                if (specialContentLabel != null)
                {
                    specialContentLabel.text = TR.Value("adventurer_pass_card_shop_coin_clear_tip");
                }
            }
            else
            {
                specialContent.gameObject.CustomActive(false);
            }
        }

        //等级变化时监听
        private void OnExchangeMallItemLevelChanged(int preLevel, int curLevel)
        {
            if(mShopTable == null)
                return;
            //已经解锁，直接返回
            if(mIsLocked == false)
                return;
            //依然没有解锁
            if(PlayerBaseData.GetInstance().Level < mShopTable.OpenLevel)
                return;
            UpdateMallElementItemState();
        }

        //根据等级来进行更新Item
        private void UpdateMallElementItemState()
        {
            //等级判断
            if (PlayerBaseData.GetInstance().Level < mShopTable.OpenLevel)
                mIsLocked = true;
            else
                mIsLocked = false;
            //冒险佣兵商城判断
            if(AccountShopDataManager.GetInstance().CheckIsAdventureTeamAccShop(mShopTable))
               mIsLocked = !AccountShopDataManager.GetInstance().CheckIsAdventureTeamAccShopOpen();
            // 玩家没有公会的时候公会商店锁住
            if(mShopTable != null && mShopTable.ShopKind == ShopTable.eShopKind.SK_Guild && !GuildDataManager.GetInstance().HasSelfGuild())
                mIsLocked = true;
            // 通行证赛季没有开启，商店会锁住
            if (mShopTable != null && mShopTable.ShopKind == ShopTable.eShopKind.SK_AdventureCoin && AdventurerPassCardDataManager.GetInstance().CardLv == 0)
                mIsLocked = true;

            if (mIsLocked == true)
            {
                if (mallLock != null)
                    mallLock.CustomActive(true);

                if (uiGray != null)
                {
                    uiGray.enabled = true;
                    uiGray.Refresh();
                    mallIcon.CustomActive(false);
                }
                
            }
            else
            {
                if (mallLock != null)
                    mallLock.CustomActive(false);

                if (uiGray != null)
                    uiGray.enabled = false;
                mallIcon.CustomActive(true);
            }
            ShowLockTip();
            InitShowCoin();
        }

        //上锁tip
        private void ShowLockTip()
        {
            mObjLockTip.CustomActive(mIsLocked);
            mTextLockTip.SafeSetText("");
            if (!mIsLocked)
                return;
            string strReason = "";
            if (mShopTable == null)
            {
                Logger.LogErrorFormat("shopTable is invalid ");
                return;
            }
            //公会商店
            if(mShopTable.ShopKind == ShopTable.eShopKind.SK_Guild)
            {
                strReason = TR.Value("have_no_guild");
            }
            //通行证商店
            else if (mShopTable.ShopKind == ShopTable.eShopKind.SK_AdventureCoin)
            {
                if(AdventurerPassCardDataManager.GetInstance().SeasonID == 0) // 赛季没开
                    strReason = TR.Value("adventurer_pass_card_shop_locked");
                else // 赛季开了，玩家依然无法使用该商店，则肯定是玩家没有解锁该功能
                    strReason = TR.Value("adventurer_pass_card_shop_locked1");
            }
            else
            {
                var exchangeNotOpenTip = string.Format(TR.Value("exchange_mall_not_open"), mShopTable.OpenLevel, mShopTable.ShopName);
                strReason = exchangeNotOpenTip;
            }
            mTextLockTip.SafeSetText(strReason);
        }

        public void OnMallButtonClicked()
        {
            //商店未解锁
            if (mIsLocked == true)
            {
                if (mShopTable == null)
                {
                    Logger.LogErrorFormat("shopTable is invalid ");
                    return;
                }
                //公会商店
                if(mShopTable.ShopKind == ShopTable.eShopKind.SK_Guild)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("have_no_guild"));
                }
                //通行证商店
                else if (mShopTable.ShopKind == ShopTable.eShopKind.SK_AdventureCoin)
                {
                    if(AdventurerPassCardDataManager.GetInstance().SeasonID == 0) // 赛季没开
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("adventurer_pass_card_shop_locked"));
                    else // 赛季开了，玩家依然无法使用该商店，则肯定是玩家没有解锁该功能
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("adventurer_pass_card_shop_locked1"));
                }
                else
                {
                    var exchangeNotOpenTip = string.Format(TR.Value("exchange_mall_not_open"), mShopTable.OpenLevel, mShopTable.ShopName);
                    SystemNotifyManager.SysNotifyFloatingEffect(exchangeNotOpenTip);
                }
            }
            else
            {
                if (mShopTable == null)
                {
                    Logger.LogErrorFormat("shopTable is invalid ");
                    return;
                }
                //添加埋点
                Utility.DoStartFrameOperation("MallNewExchangeMallElementItem",string.Format("ShopID/{0}",mShopTable.ID));
                //活动商店，活动已经结束
                if (mShopTable.ShopKind == ShopTable.eShopKind.SK_Activity)
                {
                    //没有处在开始状态，点击飘提示，之后返回
                    if (ShopNewUtility.IsActivityShopInStartState(mShopTable) == false)
                    {
                        //活动已经结束
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("shop_new_activity_end"));
                        return;
                    }
                }
                //帐号绑定商店
                if (mShopTable.BindType == ShopTable.eBindType.ACCOUNT_BIND)
                {
                    AccountShopDataManager.GetInstance().OpenAccountShop(mShopTable.ID);
                    return;
                }
                //商店可以点击
                ShopNewDataManager.GetInstance().OpenShopNewFrame(mShopTable.ID);
            }
        }

    }
}
