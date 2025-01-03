using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EItemType = ProtoTable.ItemTable.eType;

namespace GameClient
{
    public class DeadLineReminderElement : MonoBehaviour
    {
        [SerializeField]private StateController mControl;
        [SerializeField]private Button mGoBtn;
        [SerializeField]private Button mRenewalBtn;
        [SerializeField]private Button mIgnoreBtn;
        [SerializeField]private Button mIconBtn;
        [SerializeField]private Image mBackGround;
        [SerializeField]private Image mIcon;
        [SerializeField]private Text mName;
        [SerializeField]private Text mDesc;
        [SerializeField]private Text mTimeRemain;
        [SerializeField]private Text mRenewalTxt;

        private DeadLineReminderModel data = null;
        private string strFuncName;
        private UnityEngine.Events.UnityAction func;
        private void Awake()
        {
            if (mGoBtn != null)
            {
                mGoBtn.onClick.RemoveAllListeners();
                mGoBtn.onClick.AddListener(OnGoBtnClick);
            }

            if (mRenewalBtn != null)
            {
                mRenewalBtn.onClick.RemoveAllListeners();
                mRenewalBtn.onClick.AddListener(()=> 
                {
                    if (func != null)
                    {
                        func.Invoke();
                    }
                });
            }

            if (mIgnoreBtn != null)
            {
                mIgnoreBtn.onClick.RemoveAllListeners();
                mIgnoreBtn.onClick.AddListener(OnIgnoreBtnClick);
            }
        }

        public void InitElement(DeadLineReminderModel model)
        {
            data = model;
            UpdateButtonState();
            InitInterface();
        }

        private void InitInterface()
        {
            if (data.type == DeadLineReminderType.DRT_NOTIFYINFO)
            {
                string sSprPath = string.Empty;
                string sDesc = string.Empty;

                sSprPath = DeadLineReminderDataManager.GetInstance().GetDeadlineReminderItemIcon((NotifyType)data.info.type);

                if (data.info.type == (uint)NotifyType.NT_MAGIC_INTEGRAL_EMPTYING)
                {
                    if (string.IsNullOrEmpty(sSprPath))
                    {
                        sSprPath = "UI/Image/Packed/p_MainUIIcon.png:UI_MainUI_Tubiao_Moguan";
                    }
                    sDesc = TR.Value("notice_mogicjar_integral_emptying");

                }
                else if (data.info.type == (uint)NotifyType.NT_MONTH_CARD_REWARD_EXPIRE_24H)
                {
                    sSprPath = "UI/Image/Packed/p_UI_Fuli.png:UI_Fuli_JiangLiZanCunXiang";
                    sDesc = TR.Value("notice_month_card_high_grade_expire_24h");
                }
                else if (data.info.type == (uint)NotifyType.NT_ADVENTURE_TEAM_BOUNTY_EMPTYING)
                {
                    if (string.IsNullOrEmpty(sSprPath))
                    {
                        sSprPath = "UI/Image/Packed/p_MainUI01.png:UI_MainUI_Tubiao_Yongbingtuan";
                    }
                    sDesc = TR.Value("notice_adventureteam_bounty_emptying");
                }
                else if (data.info.type == (uint)NotifyType.NT_ADVENTURE_TEAM_INHERIT_BLESS_EMPTYING)
                {
                    if (string.IsNullOrEmpty(sSprPath))
                    {
                        sSprPath = "UI/Image/Packed/p_MainUI01.png:UI_MainUI_Tubiao_Yongbingtuan";
                    }
                    sDesc = TR.Value("notice_adventureteam_inheritbless_emptying");
                }
                else if (data.info.type == (uint)NotifyType.NT_ADVENTURE_PASS_CARD_COIN_EMPTYING)
                {
                    if (string.IsNullOrEmpty(sSprPath))
                    {
                        sSprPath = "UI/Image/Packed/p_MainUI01.png:UI_MainUI_Tubiao_Yongbingtuan";
                    }
                    sDesc = TR.Value("adventurer_pass_card_shop_coin_to_do_empty",AdventurerPassCardDataManager.GetInstance().SeasonID);
                }

                if (mIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref mIcon, sSprPath);
                    mIcon.SetNativeSize();
                }

                if (mDesc != null)
                {
                    mDesc.text = sDesc;
                }
            }
            else
            {
                ItemData itemData = data.itemData;
                
                if (mBackGround != null)
                {
                    ETCImageLoader.LoadSprite(ref mBackGround, itemData.GetQualityInfo().Background);
                }

                if (mIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref mIcon, itemData.Icon);
                }

                if (mIconBtn != null)
                {
                    mIconBtn.onClick.RemoveAllListeners();
                    mIconBtn.onClick.AddListener(() => { ItemTipManager.GetInstance().ShowTip(itemData); });
                }

                if (mRenewalTxt != null)
                {
                    mRenewalTxt.text = strFuncName;
                }

                if (mName != null)
                {
                    mName.text = itemData.GetColorName();
                }

                if (mTimeRemain != null)
                {
                    mTimeRemain.text = GetTimeLeftDesc(itemData);
                }
            }
        }

        private void UpdateButtonState()
        {
            if (data.type == DeadLineReminderType.DRT_NOTIFYINFO)
            {
                mControl.Key = "NotifyInfo";
            }
            else
            {
                if (GetSuitableFunc(data.itemData,out strFuncName,out func))
                {
                    mControl.Key = "Rebewal";
                }
                else
                {
                    mControl.Key = "Ignore";
                }
            }
        }

        private string GetTimeLeftDesc(ItemData a_itemData)
        {
            int timeLeft;
            bool bStartCountdown;
            a_itemData.GetTimeLeft(out timeLeft, out bStartCountdown);
            if (bStartCountdown)
            {
                if (timeLeft > 0)
                {
                    int second = 0;
                    int minute = 0;
                    int hour = 0;
                    second = timeLeft % 60;
                    int temp = timeLeft / 60;
                    if (temp > 0)
                    {
                        minute = temp % 60;
                        hour = temp / 60;
                    }

                    string value = "";
                    if (hour > 0)
                    {
                        value += string.Format("{0}小时", hour);
                    }
                    if (minute > 0)
                    {
                        value += string.Format("{0}分", minute);
                    }
                    if (second > 0)
                    {
                        value += string.Format("{0}秒", second);
                    }

                    return TR.Value("tip_color_bad", TR.Value("item_time_left", value));
                }
                else
                {
                    return TR.Value("tip_color_bad", TR.Value("item_time_left_none"));
                }
            }
            else
            {
                return string.Empty;
            }
        }

        private bool GetSuitableFunc(ItemData a_itemData, out string a_strName, out UnityEngine.Events.UnityAction a_func)
        {
            if (a_itemData.CanRenewal())
            {
                a_strName = TR.Value("tip_renewal");
                a_func = () => { OnRenewalItem(a_itemData); };
                return true;
            }
            else
            {
                int nTimeLeft;
                bool bStartCountdown;
                a_itemData.GetTimeLeft(out nTimeLeft, out bStartCountdown);
                if (bStartCountdown == true && nTimeLeft <= 0)
                {
                    a_strName = string.Empty;
                    a_func = null;
                    return false;
                }

                if (a_itemData.UseType == ProtoTable.ItemTable.eCanUse.UseOne ||
                    a_itemData.UseType == ProtoTable.ItemTable.eCanUse.UseTotal)
                {
                    if (
                        a_itemData.Type != EItemType.FUCKTITTLE &&
                        a_itemData.Type != EItemType.EQUIP &&
                        a_itemData.Type != EItemType.FASHION
                        )
                    {
                        //如果是限时佣兵团改名卡
                        if (a_itemData.PackageType == EPackageType.Consumable && 
                            a_itemData.SubType == (int)ItemTable.eSubType.ChangeName&&
                            a_itemData.ThirdType == ItemTable.eThirdType.ChangeAdventureName)
                        {
                            //查看
                            a_strName = TR.Value("tip_check");
                            a_func = () => { OnToViewClick(a_itemData); };

                            return true;
                        }
                        else
                        {
                            // 使用
                            a_strName = TR.Value("tip_use");
                            a_func = () => { TryOnUseItem(a_itemData); };

                            return true;
                        }
                    }
                }
                else
                {
                    var dataItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)a_itemData.TableID);
                    if (dataItem != null && !string.IsNullOrEmpty(dataItem.LinkInfo))
                    {
                        // 用途
                        a_strName = TR.Value("tip_itemLink");
                        a_func = () => { OnItemLink(a_itemData); };
                        return true;
                    }
                    else
                    {
                        if (
                            a_itemData.Type == ProtoTable.ItemTable.eType.EXPENDABLE &&
                            a_itemData.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard &&
                            Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Forge)
                            )
                        {
                            // 锻冶
                            if (a_itemData.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard)
                            {
                                a_strName = TR.Value("tip_enchanting");
                            }
                            else
                            {
                                a_strName = TR.Value("tip_forge");
                            }
                            a_func = () => { OnForgeItem(a_itemData); };
                            return true;
                        }
                    }
                }

                a_strName = TR.Value("tip_check");
                a_func = ()=> { OnToViewClick(a_itemData); };
                return true;
            }
        }

        private void OnRenewalItem(ItemData item)
        {
            ClientSystemManager.GetInstance().OpenFrame<RenewalItemFrame>(FrameLayer.Middle, item);
        }

        private void TryOnUseItem(ItemData item)
        {
            if (item.Type == EItemType.EQUIP)
            {
                int iEquipedMasterPriority = EquipMasterDataManager.GetInstance().GetMasterPriority(PlayerBaseData.GetInstance().JobTableID, (int)item.Quality, (int)item.EquipWearSlotType, (int)item.ThirdType);
                if (iEquipedMasterPriority == 2)
                {
                    SystemNotifyManager.SystemNotifyOkCancel(7019,
                        () =>
                        {
                            OnUseItem(item);
                        },
                        null);
                    return;
                }
            }

            OnUseItem(item);
        }

        private void OnUseItem(ItemData item)
        {
            if (item != null)
            {
                if (item.PackID > 0)
                {
                    GiftPackTable giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(item.PackID);
                    if (giftPackTable != null)
                    {
                        if (giftPackTable.FilterType == GiftPackTable.eFilterType.Custom || giftPackTable.FilterType == GiftPackTable.eFilterType.CustomWithJob)
                        {
                            if (giftPackTable.FilterCount > 0)
                            {
                                ClientSystemManager.GetInstance().OpenFrame<SelectItemFrame>(FrameLayer.Middle, item);
                            }
                            else
                            {
                                Logger.LogErrorFormat("礼包{0}的FilterCount小于等于0", item.PackID);
                            }
                        }
                        else
                        {
                            ItemDataManager.GetInstance().UseItem(item);
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("道具{0}的礼包ID{1}不存在", item.TableID, item.PackID);
                    }
                }
                else
                {
                    ItemDataManager.GetInstance().UseItem(item);
                    if (item.PackageType == EPackageType.Equip || item.PackageType == EPackageType.Fashion)
                    {
                        AudioManager.instance.PlaySound(102);
                    }
                }
            }
        }

        private void OnItemLink(ItemData item)
        {
            var dataItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)item.TableID);
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.LinkInfo))
            {
                var FuncUnlockdata = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>(dataItem.FunctionID);
                if (FuncUnlockdata != null && FuncUnlockdata.FinishLevel > PlayerBaseData.GetInstance().Level)
                {
                    SystemNotifyManager.SystemNotify(FuncUnlockdata.CommDescID);
                    return;
                }

                ActiveManager.GetInstance().OnClickLinkInfo(dataItem.LinkInfo);
            }
        }

        private void OnForgeItem(ItemData a_item)
        {
            if (a_item != null)
            {
                SmithShopNewLinkData data = new SmithShopNewLinkData();
                data.itemData = a_item;

                if (a_item.SubType == (int)ItemTable.eSubType.EnchantmentsCard)
                {
                    data.iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_ENCHANTMENTCARD;
                    data.iDefaultSecondTabId = (int)EnchantmentCardSubTabType.ECSTT_EQUIPMENTENCHANT;
                }
                else
                {
                    data.iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_STRENGTHEN;
                }

                ClientSystemManager.GetInstance().CloseFrame<SmithShopNewFrame>(null, true);
                ClientSystemManager.GetInstance().OpenFrame<SmithShopNewFrame>(FrameLayer.Middle, data);
            }
        }

        private void OnGoBtnClick()
        {
            NotifyInfo info = data.info;
            if (info.type == (uint)NotifyType.NT_MAGIC_INTEGRAL_EMPTYING)
            {
                //打开积分商店
                ShopNewDataManager.GetInstance().OpenShopNewFrame(7);
                //if (ClientSystemManager.GetInstance().IsFrameOpen<PocketJarFrame>())
                //{
                //    ClientSystemManager.GetInstance().CloseFrame<PocketJarFrame>();
                //}
                //ClientSystemManager.GetInstance().OpenFrame<PocketJarFrame>(FrameLayer.Middle);
                DeadLineReminderDataManager.GetInstance().DeleteActivityNotice(info);
            }
            else if (info.type == (uint)NotifyType.NT_MONTH_CARD_REWARD_EXPIRE_24H)
            {
                GameClient.ActiveManager.GetInstance().OpenActiveFrame(MonthCardRewardLockersDataManager.FULI_ACTIVITY_TYPE_ID,
                                                                       MonthCardRewardLockersDataManager.FULI_ACTIVITY_TEMPLATE_TABLE_ID);
                DeadLineReminderDataManager.GetInstance().DeleteActivityNotice(info);
            }
            else if (info.type == (uint)NotifyType.NT_ADVENTURE_TEAM_BOUNTY_EMPTYING)
            {
                GameClient.AccountShopDataManager.GetInstance().OpenAccountShop(AccountShopDataManager.ADVENTURE_TEAM_SHOP_ID, AccountShopDataManager.ADVENTURE_TEAM_BOUNTY_CHILD_SHOP_ID);

                DeadLineReminderDataManager.GetInstance().DeleteActivityNotice(info);
            }
            else if (info.type == (uint)NotifyType.NT_ADVENTURE_TEAM_INHERIT_BLESS_EMPTYING)
            {
                GameClient.AdventureTeamDataManager.GetInstance().OpenAdventureTeamInfoFrame(AdventureTeamMainTabType.PassingBless);
                DeadLineReminderDataManager.GetInstance().DeleteActivityNotice(info);
            }
            else if (info.type == (uint)NotifyType.NT_ADVENTURE_PASS_CARD_COIN_EMPTYING)
            {
                ClientSystemManager.GetInstance().OpenFrame<AdventurerPassCardFrame>();
                DeadLineReminderDataManager.GetInstance().DeleteActivityNotice(info);
            }

            ClientSystemManager.GetInstance().CloseFrame<DeadLineReminderFrame>();
        }

        //查看
        private void OnToViewClick(ItemData a_item)
        {
            if (a_item != null)
            {
                EPackageOpenMode mode = EPackageOpenMode.Equip;
                if (a_item.Type == ProtoTable.ItemTable.eType.EQUIP)
                {
                    mode = EPackageOpenMode.Equip;
                }
                else if (a_item.Type == ItemTable.eType.EXPENDABLE)
                {
                    mode = EPackageOpenMode.Consumables;
                }
                else if (a_item.Type == ItemTable.eType.MATERIAL)
                {
                    mode = EPackageOpenMode.Material;
                }
                else if (a_item.Type == ItemTable.eType.FASHION)
                {
                    mode = EPackageOpenMode.Fashion;
                }
                else if (a_item.Type == ItemTable.eType.FUCKTITTLE)
                {
                    mode = EPackageOpenMode.Title;
                }

                OnIgnoreBtnClick();
                ClientSystemManager.GetInstance().OpenFrame<PackageNewFrame>(FrameLayer.Middle, mode);
                ClientSystemManager.GetInstance().CloseFrame<DeadLineReminderFrame>();
            }
        }

        private void OnIgnoreBtnClick()
        {
            DeadLineReminderDataManager.GetInstance().DeleteLimitTimeItem(data.itemData);
        }

        private void OnDestroy()
        {
            data = null;
            strFuncName = string.Empty;
            func = null;
        }
    }
}