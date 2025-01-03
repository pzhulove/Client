using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    // 购买王者版冒险者通行证界面
    public class AdventurerPassCardBuyKingLevelFrame : ClientFrame
    {
        int kingPrice = 0;

        #region ui bind
        private Button mBtBuy = null;
        private Text mMoneyNum = null;
        private Button btBuyUseRMB = null;
        private Text moneyNumRMB = null;
        private GeAvatarRendererEx mGeAvatarRendererEx = null;
        private GameObject mNameTitleRoot = null;
        private Text mTitleName = null;
        private SpriteAniRenderChenghao mTitleAnimation = null;
        private Image mTitleAnimationImage = null;
        #endregion


        #region override

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AdventurerPassCard/AdventurerPassCardBuyKingLevel";
        }

        protected override void _OnOpenFrame()
        {
            UpdateUI();
        }

        protected override void _OnCloseFrame()
        {
        }

        protected override void _bindExUI()
        {
            mBtBuy = mBind.GetCom<Button>("btBuy");
            mBtBuy.SafeSetOnClickListener(() =>
            {
                int moneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
                CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo
                {
                    nMoneyID = moneyID,
                    nCount = kingPrice,
                },
                () =>
                {
                    SystemNotifyManager.SysNotifyMsgBoxCancelOk(TR.Value("adventurer_pass_card_buy_high_card_tip", kingPrice), null, () =>
                    {
                        AdventurerPassCardDataManager.GetInstance().SendWorldAventurePassBuyReq(1);
                    });
                },
                "common_money_cost",
                null);
                return;
            });

            btBuyUseRMB = mBind.GetCom<Button>("btBuyUseRMB");
            btBuyUseRMB.SafeSetOnClickListener(() =>
            {
                SystemNotifyManager.SysNotifyMsgBoxCancelOk(TR.Value("adventurer_pass_card_buy_high_card_tip_rmb", AdventurerPassCardDataManager.GetInstance().KingCardItemPrice), null, () =>
                {
                    AdventurerPassCardDataManager.GetInstance().SendBuyKingCardUseRmb();
                });
            });
            mMoneyNum = mBind.GetCom<Text>("moneyNum");
            moneyNumRMB = mBind.GetCom<Text>("moneyNumRMB");
            mGeAvatarRendererEx = mBind.GetCom<GeAvatarRendererEx>("Actorpos");
            mNameTitleRoot = mBind.GetGameObject("nameTitleRoot");
            mTitleName = mBind.GetCom<Text>("titleName");
            mTitleAnimation = mBind.GetCom<SpriteAniRenderChenghao>("titleAnimation");
            mTitleAnimationImage = mBind.GetCom<Image>("titleAnimationImage");
        }

        protected override void _unbindExUI()
        {
            mBtBuy = null;
            mMoneyNum = null;
            mGeAvatarRendererEx = null;

            mNameTitleRoot = null;
            mTitleName = null;
            mTitleAnimation = null;
            mTitleAnimationImage = null;
            btBuyUseRMB = null;
            moneyNumRMB = null;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
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
        }

        #endregion

        #region method

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

        private void SetTitleName(string name, ItemTable.eColor color)
        {
            if (mTitleName != null)
            {
                mTitleName.SafeSetText(name);
                mTitleName.color = GameUtility.Item.GetItemColor(color);
            }
        }

        private void SetNameTitleImage(ItemTable itemTable)
        {
            if (itemTable != null && itemTable.Path2.Count == 4)
            {
                if (mTitleAnimation != null)
                {
                    mTitleAnimation.gameObject.CustomActive(true);
                    mTitleAnimation.Reset(itemTable.Path2[0], itemTable.Path2[1], int.Parse(itemTable.Path2[2]), float.Parse(itemTable.Path2[3]), itemTable.ModelPath);
                    if (mTitleAnimationImage != null)
                    {
                        mTitleAnimationImage.enabled = true;
                    }
                }
            }
        }

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
                    if (adt == null || adt.Season != AdventurerPassCardDataManager.GetInstance().SeasonID)
                    {
                        continue;
                    }

                    string[] giftBagIds = adt.GiftBagID.Split('|');
                    if (giftBagIds == null)
                    {
                        continue;
                    }
                    int titleItemID = 0;
                    List<int> fashionIDs = new List<int>() /*{ 531003631, 531003632, 531003633, 531003634, 531003635, 535001124 }*/;
                    List<int> ids = new List<int>();
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
        }

        void UpdateUI()
        {
            InitAvatar();
            UpdateFashionAndTitle();
            mBtBuy.SafeSetGray(AdventurerPassCardDataManager.GetInstance().GetPassCardType != AdventurerPassCardDataManager.PassCardType.Normal);
            btBuyUseRMB.SafeSetGray(AdventurerPassCardDataManager.GetInstance().GetPassCardType != AdventurerPassCardDataManager.PassCardType.Normal);

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

                    if (adt.Season == AdventurerPassCardDataManager.GetInstance().SeasonID)
                    {
                        mMoneyNum.SafeSetText(adt.KingPrice.ToString());
                        kingPrice = adt.KingPrice;
                        break;
                    }
                }
            }
            moneyNumRMB.SafeSetText(TR.Value("adventurer_pass_card_rmb", AdventurerPassCardDataManager.GetInstance().KingCardItemPrice));
            bool useRmb = AdventurerPassCardDataManager.GetInstance().IsBuyKingCardUseRMB((int)AdventurerPassCardDataManager.GetInstance().SeasonID);
            mBtBuy.CustomActive(!useRmb);
            btBuyUseRMB.CustomActive(useRmb);
        }

        #endregion
    }
}
