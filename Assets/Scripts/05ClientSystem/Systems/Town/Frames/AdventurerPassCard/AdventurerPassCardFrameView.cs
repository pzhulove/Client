using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventurerPassCardFrameView : MonoBehaviour
    {
        [SerializeField] private Text mTextSeasonID;
        [SerializeField] private Text mTextCardLv;
        [SerializeField] private Text mTextCardExp;
        [SerializeField] private Button mBtnBuyLv;
        [SerializeField] private Button mBtnBuyHighLvCard;
        [SerializeField] private Text mTextSeason;
        [SerializeField] private Text mTextSeasonTime;
        [SerializeField] private Slider mSliderCardExp;
        [SerializeField] private Text mTextTodayActive;
        [SerializeField] private ComUIListScript mAwardListScript = null;
        [SerializeField] private GeAvatarRendererEx mGeAvatarRendererEx;
        [SerializeField] private Button mBtnGetAllAward;
        [SerializeField] private GameObject mObjLock;
        [SerializeField] private AdventurerPassCardAwardItem mHighawardItemShow;
        // [SerializeField] private Text mTextExpAddByActiveDesc;
        [SerializeField] private GameObject mCanGotEffRoot;
        [SerializeField] private Image mGou;
        [SerializeField] private GameObject mNameTitleRoot;
        [SerializeField] private Text mTitleName;
        [SerializeField] private SpriteAniRenderChenghao mTitleAnimation;
        [SerializeField] private Text mTextBigAwardName;
        [SerializeField] private Image mImgTitleAnimation;
        [SerializeField] private GameObject mObjGetAllAwardRedPoint;

        Dictionary<int, AdventurerPassCardDataManager.AwardItemInfo> awardItemInfo = null;
        bool mIsFristSetList = false;

        public void OnInit()
        {
            mIsFristSetList = true;
            awardItemInfo = null;
            InitAwards();
            InitAvatar();
            UpdateFashionAndTitle();
            _ShowFristOpen();
            RequestPassCardData();
            // if (firstMoney != null)
            // {
            //     firstMoney.InitMoneyItem(600002545);
            // }
        }

        private void _ShowFristOpen()
        {
            int seasonId = (int)AdventurerPassCardDataManager.GetInstance().SeasonID;
            int openSeasonId = PlayerPrefsManager.GetInstance().GetAccTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.AdventurePassCardOpenSeasonId);
            if (openSeasonId != seasonId)
            {
                PlayerPrefsManager.GetInstance().SetAccTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.AdventurePassCardOpenSeasonId, seasonId);
                ClientSystemManager.GetInstance().OpenFrame<AdventurerPassCardSeasonOpenFrame>();
            }
        }

        public void OnUninit()
        {
            awardItemInfo = null;
        }

        //请求通行证数据
        public void RequestPassCardData()
        {
            // // 请求冒险者通行证数据
            // AdventurerPassCardDataManager.GetInstance().SendWorldAventurePassStatusReq();
            // // 查询下经验包领取状态
            // AdventurerPassCardDataManager.GetInstance().SendWorldAventurePassExpPackReq(0);
            //直接显示界面即可
            OnUpdateAventurePassStatus(null);
            OnUpdateAventurePassExpPackStatus(null);
        }

        //初始化奖励列表
        private void InitAwards()
        {
            if (mAwardListScript == null)
            {
                return;
            }
            mAwardListScript.Initialize();
            mAwardListScript.onBindItem = (go) =>
            {
                return go;
            };
            mAwardListScript.onItemVisiable = (go) =>
            {
                if (go == null || awardItemInfo == null)
                {
                    return;
                }
                AdventurerPassCardAwardItem adventurerPassCardAwardItem = go.GetComponent<AdventurerPassCardAwardItem>();
                if (adventurerPassCardAwardItem == null)
                {
                    return;
                }
                int lv = go.m_index + 1;
                if (awardItemInfo.ContainsKey(lv))
                {
                    adventurerPassCardAwardItem.SetUp(awardItemInfo[lv]);
                }
            };
        }

        //初始化角色形象
        private void InitAvatar()
        {
            int jobID = PlayerBaseData.GetInstance().JobTableID;
            JobTable jobTable = TableManager.GetInstance().GetTableItem<JobTable>(jobID);
            if (jobTable == null)
            {
                Logger.LogErrorFormat("can not find JobTable with id:{0}", jobID);
            }
            else
            {
                ResTable resTable = TableManager.GetInstance().GetTableItem<ResTable>(jobTable.Mode);
                if (resTable == null)
                {
                    Logger.LogErrorFormat("can not find ResTable with id:{0}", jobTable.Mode);
                }
                else
                {
                    mGeAvatarRendererEx.ClearAvatar();
                    mGeAvatarRendererEx.LoadAvatar(resTable.ModelPath);

                    if (jobID == PlayerBaseData.GetInstance().JobTableID)
                    {
                        PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(mGeAvatarRendererEx);
                    }
                    mGeAvatarRendererEx.AttachAvatar("Aureole", "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS", "[actor]Orign", false);
                    mGeAvatarRendererEx.SuitAvatar();
                }
            }
        }

        //更新角色时装与称号
        void UpdateFashionAndTitle()
        {
            if (mGeAvatarRendererEx == null)
            {
                return;
            }

            Dictionary<int, object> dicts = TableManager.instance.GetTable<AdventurePassBuyRewardTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    AdventurePassBuyRewardTable adt = iter.Current.Value as AdventurePassBuyRewardTable;
                    if (adt == null)
                    {
                        continue;
                    }
                    if (adt.Season != AdventurerPassCardDataManager.GetInstance().SeasonID)
                    {
                        continue;
                    }
                    string[] giftBagIds = adt.GiftBagID.Split('|');
                    if (giftBagIds == null)
                    {
                        continue;
                    }
                    int titleItemID = 0;
                    List<int> fashionIDs = new List<int>();
                    if (fashionIDs == null)
                    {
                        continue;
                    }
                    fashionIDs.Clear();

                    List<int> ids = new List<int>();
                    if (ids == null)
                    {
                        continue;
                    }
                    for (int i = 0; i < giftBagIds.Length; i++)
                    {
                        CalcGiftBagItemIDs(Utility.ToInt(giftBagIds[i]), ref ids);
                    }
                    for (int i = 0; i < ids.Count; i++)
                    {
                        ItemTable itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(ids[i]);
                        if (itemTable == null)
                        {
                            continue;
                        }

                        if (itemTable.Type == ItemTable.eType.FASHION)
                        {
                            fashionIDs.Add(ids[i]);
                        }
                        else if (itemTable.Type == ItemTable.eType.FUCKTITTLE)
                        {
                            titleItemID = ids[i];
                        }
                    }

                    // 刷新时装和武器
                    for (int i = 0; i < fashionIDs.Count; i++)
                    {
                        UpdateHeroActorFashion(TableManager.GetInstance().GetTableItem<ItemTable>(fashionIDs[i]));
                    }

                    // 刷新称号
                    {
                        ItemTable itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(titleItemID);
                        if (itemTable != null)
                        {
                            SetNameTitleImage(itemTable);
                            SetTitleName(itemTable.Name, itemTable.Color);
                        }
                        mNameTitleRoot.CustomActive(itemTable != null);
                    }

                    break;
                }
            }
        }
        //取出礼包内的道具
        void CalcGiftBagItemIDs(int itemID, ref List<int> ids)
        {
            if (ids == null)
            {
                return;
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemID);
            if (itemData == null)
            {
                return;
            }

            List<GiftTable> giftTables = itemData.GetGifts();
            if (giftTables != null) // 如果是礼包，则继续检查里面是否嵌套了礼包
            {
                for (int i = 0; i < giftTables.Count; i++)
                {
                    CalcGiftBagItemIDs(giftTables[i].ItemID, ref ids);
                }
            }
            else // 不是礼包则add该ID
            {
                ids.Add(itemID);
            }

            return;
        }
        //更新角色时装
        private void UpdateHeroActorFashion(ItemTable itemTable)
        {
            if (itemTable == null)
            {
                return;
            }

            if (mGeAvatarRendererEx == null)
            {
                return;
            }

            GeAvatarRendererEx heroAvatarRenderer = mGeAvatarRendererEx;
            EFashionWearSlotType slotType = FashionMallUtility.GetEquipSlotType(itemTable);
            //显示武器时装
            if (itemTable.SubType == ItemTable.eSubType.FASHION_WEAPON)
            {
                var heroBaseJobId = CommonUtility.GetSelfBaseJobId();
                PlayerBaseData.GetInstance().AvatarEquipWeapon(heroAvatarRenderer, heroBaseJobId, itemTable.ID);
            }
            else
            {
                //首先清空对应部位的时装资源。避免有的道具存在资源有的道具不存在资源，造成某种资源没有卸载掉
                PlayerBaseData.GetInstance().AvatarEquipPart(heroAvatarRenderer, slotType, 0);
                //单品部位的时装
                PlayerBaseData.GetInstance().AvatarEquipPart(heroAvatarRenderer, slotType, itemTable.ID);
            }
            heroAvatarRenderer.ChangeAction("Anim_Show_Idle", 1.0f, true);
            int seasonId = (int)AdventurerPassCardDataManager.GetInstance().SeasonID;
            var seasonTable = TableManager.GetInstance().GetTableItem<AdventurePassBuyRewardTable>(seasonId);
            if (null != seasonTable)
            {
                var table = TableManager.GetInstance().GetTableItem<ItemTable>(seasonTable.GiftBagID);
                if (null != table)

                    mTextBigAwardName.SafeSetText(table.Name);
                    mTextBigAwardName.color = GameUtility.Item.GetItemColor(table.Color);
            }
        }
        //设置角色称号
        private void SetNameTitleImage(ItemTable itemTable)
        {
            if (itemTable != null && itemTable.Path2.Count == 4)
            {
                if (mTitleAnimation != null)
                {
                    mTitleAnimation.gameObject.CustomActive(true);
                    mTitleAnimation.Reset(itemTable.Path2[0], itemTable.Path2[1], int.Parse(itemTable.Path2[2]), float.Parse(itemTable.Path2[3]), itemTable.ModelPath);
                    if (mImgTitleAnimation != null)
                    {
                        mImgTitleAnimation.enabled = true;
                    }
                }
            }
        }
        //设置称号名称
        private void SetTitleName(string name, ItemTable.eColor color)
        {
            if (mTitleName != null)
            {
                mTitleName.SafeSetText(name, color);
            }
        }

        public void OnUpdate(float timeElapsed)
        {
            if (mGeAvatarRendererEx != null)
            {
                while (global::Global.Settings.avatarLightDir.x > 360)
                    global::Global.Settings.avatarLightDir.x -= 360;
                while (global::Global.Settings.avatarLightDir.x < 0)
                    global::Global.Settings.avatarLightDir.x += 360;

                while (global::Global.Settings.avatarLightDir.y > 360)
                    global::Global.Settings.avatarLightDir.y -= 360;
                while (global::Global.Settings.avatarLightDir.y < 0)
                    global::Global.Settings.avatarLightDir.y += 360;

                while (global::Global.Settings.avatarLightDir.z > 360)
                    global::Global.Settings.avatarLightDir.z -= 360;
                while (global::Global.Settings.avatarLightDir.z < 0)
                    global::Global.Settings.avatarLightDir.z += 360;

                mGeAvatarRendererEx.m_LightRot = global::Global.Settings.avatarLightDir;
            }

            int lv = 0;
            bool lastShow = false;
            if (mAwardListScript != null && mAwardListScript.IsInitialised())
            {
                for (int i = 0; i < mAwardListScript.GetElementAmount(); i++)
                {
                    if (mAwardListScript.IsElementInScrollArea(i))
                    {
                        lv = i + 1;
                        lastShow = true;
                    }
                    else
                    {
                        if (lastShow)
                        {
                            break;
                        }
                    }
                }
            }

            UpdateNextLvHighAward(lv);
        }
        void UpdateNextLvHighAward(int lv)
        {
            if (mHighawardItemShow == null || awardItemInfo == null)
            {
                return;
            }
            // 通行证界面每10级的奖励展示调整，改为展示距离当前等级最近的，《M-冒险通行证奖励表》王者版填了珍稀奖励的等级
            int lvTemp = 0;
            bool bFind = false;
            for (lvTemp = lv + 1; lvTemp < awardItemInfo.Count; lvTemp++)
            {
                if (!awardItemInfo.ContainsKey(lvTemp))
                {
                    continue;
                }

                if (awardItemInfo[lvTemp].highAwards == null)
                {
                    continue;
                }

                if (awardItemInfo[lvTemp].highAwards.FindIndex((item) => { return item.highValue; }) >= 0)
                {
                    mHighawardItemShow.SetUp(awardItemInfo[lvTemp], true);
                    bFind = true;
                    break;
                }
            }
            if (!bFind) // 从左往右没有找到，接下来从右往左找
            {
                for (lvTemp = lv; lvTemp >= 1; lvTemp--)
                {
                    if (!awardItemInfo.ContainsKey(lvTemp))
                    {
                        continue;
                    }

                    if (awardItemInfo[lvTemp].highAwards == null)
                    {
                        continue;
                    }

                    if (awardItemInfo[lvTemp].highAwards.FindIndex((item) => { return item.highValue; }) >= 0)
                    {
                        mHighawardItemShow.SetUp(awardItemInfo[lvTemp], true);
                        bFind = true;
                        break;
                    }
                }
            }
            if (!bFind) // 还是没有找到，呵呵
            {
            }
        }

        #region 消息事件
        // 刷新经验包状态
        public void OnUpdateAventurePassExpPackStatus(UIEvent uiEvent)
        {
            mCanGotEffRoot.CustomActive(AdventurerPassCardDataManager.GetInstance().GetExpPackState() == AdventurerPassCardDataManager.ExpPackState.CanGet);
            mGou.CustomActive(AdventurerPassCardDataManager.GetInstance().GetExpPackState() == AdventurerPassCardDataManager.ExpPackState.Got);
        }
        // 刷新通行证相关数据
        public void OnUpdateAventurePassStatus(UIEvent uiEvent)
        {
            mTextSeasonID.SafeSetText(TR.Value("adventurer_pass_card_level_season_id", AdventurerPassCardDataManager.GetInstance().SeasonID.ToString()));
            mTextCardLv.SafeSetText(AdventurerPassCardDataManager.GetInstance().CardLv.ToString());

            mTextCardExp.SafeSetText(TR.Value("adventurer_pass_card_exp_process", AdventurerPassCardDataManager.GetInstance().CardExp, AdventurerPassCardDataManager.GetInstance().GetNeedExpToNextLv((int)AdventurerPassCardDataManager.GetInstance().CardLv)));
            if (AdventurerPassCardDataManager.GetInstance().CardLv == AdventurerPassCardDataManager.GetInstance().GetAdventurerPassCardMaxLv(AdventurerPassCardDataManager.GetInstance().SeasonID))
            {
                mTextCardExp.SafeSetText(TR.Value("adventurer_pass_card_reach_max_lv"));
            }

            mTextTodayActive.SafeSetText(TR.Value("adventurer_pass_card_get_active_today", AdventurerPassCardDataManager.GetInstance().GetTodayActive(), GetExpAddByActive(AdventurerPassCardDataManager.GetInstance().GetTodayActive()), GetMaxExpAddUp()));
            mTextSeason.SafeSetText(TR.Value("adventurer_pass_card_season_info", AdventurerPassCardDataManager.GetInstance().SeasonID));
            mTextSeasonTime.SafeSetText(TR.Value("adventurer_pass_card_season_time_info",
                GetTimeStampDateString(AdventurerPassCardDataManager.GetInstance().GetSeasonStartTime()),
                GetTimeStampDateString(AdventurerPassCardDataManager.GetInstance().GetSeasonEndTime())));

            float percent = 0.0f;
            int needExp = AdventurerPassCardDataManager.GetInstance().GetNeedExpToNextLv((int)AdventurerPassCardDataManager.GetInstance().CardLv);
            if (needExp == 0)
            {
                percent = 1.0f;
            }
            else
            {
                percent = ((float)AdventurerPassCardDataManager.GetInstance().CardExp) / ((float)needExp);
            }
            mSliderCardExp.SafeSetValue(percent);

            mBtnBuyHighLvCard.CustomActive(AdventurerPassCardDataManager.GetInstance().GetPassCardType == AdventurerPassCardDataManager.PassCardType.Normal);
            mObjLock.CustomActive(AdventurerPassCardDataManager.GetInstance().GetPassCardType == AdventurerPassCardDataManager.PassCardType.Normal);

            bool canTotalGet = AdventurerPassCardFrame.CanOneKeyGetAwards();
            mBtnGetAllAward.SafeSetGray(!canTotalGet);
            mObjGetAllAwardRedPoint.CustomActive(canTotalGet);

            UpdateAwards();

            // mTextExpAddByActiveDesc.SafeSetText(GetExpAddDesc());

            mBtnBuyLv.SafeSetGray(AdventurerPassCardDataManager.GetInstance().CardLv >= AdventurerPassCardDataManager.GetInstance().GetAdventurerPassCardMaxLv(AdventurerPassCardDataManager.GetInstance().SeasonID));

            return;
        }
        string GetTimeStampDateString(int time)
        {
            DateTime dt = TimeUtility.GetDateTimeByTimeStamp(time);
            return string.Format("{0:D4}.{1:D2}.{2:D2}", dt.Year, dt.Month, dt.Day);
        }
        int GetExpAddByActive(int active)
        {
            int expAdd = 0;
            Dictionary<int, object> dicts = TableManager.instance.GetTable<AdventurePassActivityTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    AdventurePassActivityTable adt = iter.Current.Value as AdventurePassActivityTable;
                    if (adt == null)
                    {
                        continue;
                    }

                    if (active < adt.Activity)
                    {
                        break;
                    }

                    expAdd += adt.Exp;
                }
            }

            return expAdd;
        }
        int GetMaxExpAddUp()
        {
            int expAdd = 0;
            Dictionary<int, object> dicts = TableManager.instance.GetTable<AdventurePassActivityTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    AdventurePassActivityTable adt = iter.Current.Value as AdventurePassActivityTable;
                    if (adt == null)
                    {
                        continue;
                    }

                    expAdd += adt.Exp;
                }
            }

            return expAdd;
        }

        void UpdateAwards()
        {
            if (mAwardListScript == null)
            {
                return;
            }

            awardItemInfo = AdventurerPassCardDataManager.GetInstance().GetAdventurePassAwardsBySeasonID(AdventurerPassCardDataManager.GetInstance().SeasonID);
            if (awardItemInfo == null)
            {
                return;
            }

            mAwardListScript.SetElementAmount(awardItemInfo.Count);

            //第一次打开 需要将可领取的最小等级奖励显示在最左边
            if (mIsFristSetList)
            {
                mIsFristSetList = false;

                // 计算能领取奖励的最小等级
                int cardLv = (int)AdventurerPassCardDataManager.GetInstance().CardLv;
                for (int i = 1; i <= cardLv; i++)
                {
                    if ((AdventurerPassCardDataManager.GetInstance().GetPassCardType > AdventurerPassCardDataManager.PassCardType.Normal && !AdventurerPassCardDataManager.GetInstance().IsHighAwardReceived(i)) ||
                        !AdventurerPassCardDataManager.GetInstance().IsNormalAwardReceived(i))
                    {
                        cardLv = i;
                        break;
                    }
                }

                int index = cardLv - 1;
                if (index >= 0 && index < mAwardListScript.GetElementAmount())
                {
                    mAwardListScript.EnsureElementVisable(index);
                }
            }
        }

        // string GetExpAddDesc()
        // {
        //     string desc = "";
        //     int expAdd = 5;
        //     int index = 0;
        //     Dictionary<int, object> dicts = TableManager.instance.GetTable<AdventurePassActivityTable>();
        //     if (dicts != null)
        //     {
        //         var iter = dicts.GetEnumerator();
        //         while (iter.MoveNext())
        //         {
        //             AdventurePassActivityTable adt = iter.Current.Value as AdventurePassActivityTable;
        //             if (adt == null)
        //             {
        //                 continue;
        //             }
        //             if (index == 0)
        //             {
        //                 expAdd = adt.Exp;
        //             }

        //             if (index != 0)
        //             {
        //                 desc += "/";
        //             }
        //             desc += adt.Activity.ToString();

        //             index++;
        //         }
        //     }

        //     return TR.Value("adventurer_pass_card_exp_add_desc", desc, expAdd);
        // }

        //解锁王者版
        public void OnAdventureUnlockKing()
        {
            ClientSystemManager.GetInstance().CloseFrame<AdventurerPassCardBuyKingLevelFrame>();
            ClientSystemManager.GetInstance().OpenFrame<AdventurerPassCardUnlockKingFrame>();
        }

        #endregion

        #region  按钮事件
        //打开购买等级界面
        public void OnClickBuyLv()
        {
            if (AdventurerPassCardDataManager.GetInstance().CardLv >= AdventurerPassCardDataManager.GetInstance().GetAdventurerPassCardMaxLv(AdventurerPassCardDataManager.GetInstance().SeasonID))
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("adventurer_pass_card_lv_max"));
                return;
            }
            if (AdventurerPassCardDataManager.GetInstance().GetPassCardType == AdventurerPassCardDataManager.PassCardType.Normal)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("adventurer_pass_card_buy_lv_need_king"));
                return;
            }
            ClientSystemManager.GetInstance().OpenFrame<AdventurerPassCardBuyLevelFrame>();
        }

        //打开购买王者版通行证界面
        public void OnClickOpenBuyKingLevelFrame()
        {
            ClientSystemManager.GetInstance().OpenFrame<AdventurerPassCardBuyKingLevelFrame>();
        }

        //获取额外经验包
        public void OnClickGetExpPack()
        {
            AdventurerPassCardDataManager.ExpPackState expPackState = AdventurerPassCardDataManager.GetInstance().GetExpPackState();
            if (expPackState == AdventurerPassCardDataManager.ExpPackState.Lock)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("adventurer_pass_card_high_card_lock", Utility.GetSystemValueFromTable(SystemValueTable.eType3.SVT_ADVENTURE_PASS_EXP)));
            }
            else if (expPackState == AdventurerPassCardDataManager.ExpPackState.CanGet)
            {
                SystemNotifyManager.SysNotifyMsgBoxCancelOk(TR.Value("adventurer_pass_card_high_card_get_exp_tip", Utility.GetSystemValueFromTable(SystemValueTable.eType3.SVT_ADVENTURE_PASS_EXP)), null, () =>
                {
                    AdventurerPassCardDataManager.GetInstance().SendWorldAventurePassExpPackReq(1);
                });
            }
            else if (expPackState == AdventurerPassCardDataManager.ExpPackState.Got)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("adventurer_pass_card_high_card_has_get_exp"));
            }
        }

        //打开大奖预览界面
        public void OnClickOpenBigAwardFrame()
        {
            ClientSystemManager.GetInstance().OpenFrame<AdventurerPassCardShowAwardsFrame>();
        }

        //一键领取奖励
        public void OnClickGetAllAward()
        {
            AdventurerPassCardDataManager.GetInstance().SendWorldAventurePassRewardReq(0);
        }

        //打开兑换商店
        public void OnClickOpenShop()
        {
            AccountShopDataManager.GetInstance().OpenAccountShop(70);
        }

        //打开活跃度帮助
        public void OnClickOpenHelp()
        {
            ClientSystemManager.GetInstance().OpenFrame<AdventurerPassCardHelpFrame>();

        }
        #endregion
    }
}
