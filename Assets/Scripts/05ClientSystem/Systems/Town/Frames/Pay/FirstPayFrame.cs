using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Protocol;
using Network;
using System;
using Scripts.UI;

namespace GameClient
{
	public class FirstPayFrame : ClientFrame 
    {
        protected const string EffUI_shouchong_guang_Path = "UI/UIEffects/Skill_UI_Shouchong/Prefab/Skill_UI_Shouchong_guang01";
        protected const string EffUI_shouchong_anniu_Path = "UI/UIEffects/Skill_UI_Shouchong/Prefab/Skill_UI_Shouchong_anniu";

        protected GeObjectRenderer objectRenderer = null;
        string mToGetRewardText = "";
        string mNotGetRewardText = "";
        string mGotRewardText = "";

        GameObject effect_guang_go = null;
        GameObject effect_goPayBtn_go = null;

        //Data
        List<AwardItemData> itemDataList = new List<AwardItemData>();
        //UI Component
        List<PayRewardItem> payRewardItems = new List<PayRewardItem>();

		#region ExtraUIBind
		private Button mBtnClose = null;
		private GameObject mWeaponModelRoot = null;
		private GameObject mScrollContent = null;
		private Text mWeaponName = null;
		private Button mBtnGo = null;
		private Button mBtnGet = null;
		private Text mBtnGetText = null;
		private UIGray mBtnGetGray = null;
		private Button mGotoMonthCard = null;
		private Button mGotoMoneyplan = null;
		private ComUIListScript mMainView = null;
		private PayRewardItem mSpecialItem = null;
		private Text mHasChargeRMB = null;
		private GameObject mEffectRoot_Backlight = null;
		private GameObject mEffectRoot_GoPayBtn = null;

		private TextEx mRewardText = null;
       

        protected override void _bindExUI()
		{
			mBtnClose = mBind.GetCom<Button>("btnClose");
			if (null != mBtnClose)
			{
				mBtnClose.onClick.AddListener(_onBtnCloseButtonClick);
			}
			mWeaponModelRoot = mBind.GetGameObject("weaponModelRoot");
			mScrollContent = mBind.GetGameObject("scrollContent");
			mWeaponName = mBind.GetCom<Text>("weaponName");
			mBtnGo = mBind.GetCom<Button>("btnGo");
			if (null != mBtnGo)
			{
				mBtnGo.onClick.AddListener(_onBtnGoButtonClick);
			}
			mBtnGet = mBind.GetCom<Button>("btnGet");
			if (null != mBtnGet)
			{
				mBtnGet.onClick.AddListener(_onBtnGetButtonClick);
			}
			mBtnGetText = mBind.GetCom<Text>("btnGetText");
			mBtnGetGray = mBind.GetCom<UIGray>("btnGetGray");
			mGotoMonthCard = mBind.GetCom<Button>("gotoMonthCard");
			if (null != mGotoMonthCard)
			{
				mGotoMonthCard.onClick.AddListener(_onGotoMonthCardButtonClick);
			}
			mGotoMoneyplan = mBind.GetCom<Button>("gotoMoneyplan");
			if (null != mGotoMoneyplan)
			{
				mGotoMoneyplan.onClick.AddListener(_onGotoMoneyplanButtonClick);
			}
			mMainView = mBind.GetCom<ComUIListScript>("mainView");
			mSpecialItem = mBind.GetCom<PayRewardItem>("specialItem");
			mHasChargeRMB = mBind.GetCom<Text>("HasChargeRMB");
			mEffectRoot_Backlight = mBind.GetGameObject("EffectRoot_Backlight");
			mEffectRoot_GoPayBtn = mBind.GetGameObject("EffectRoot_GoPayBtn");

            mRewardText = mBind.GetCom<TextEx>("RewardText");
		}
		
		protected override void _unbindExUI()
		{
			if (null != mBtnClose)
			{
				mBtnClose.onClick.RemoveListener(_onBtnCloseButtonClick);
			}
			mBtnClose = null;
			mWeaponModelRoot = null;
			mScrollContent = null;
			mWeaponName = null;
			if (null != mBtnGo)
			{
				mBtnGo.onClick.RemoveListener(_onBtnGoButtonClick);
			}
			mBtnGo = null;
			if (null != mBtnGet)
			{
				mBtnGet.onClick.RemoveListener(_onBtnGetButtonClick);
			}
			mBtnGet = null;
			mBtnGetText = null;
			mBtnGetGray = null;
			if (null != mGotoMonthCard)
			{
				mGotoMonthCard.onClick.RemoveListener(_onGotoMonthCardButtonClick);
			}
			mGotoMonthCard = null;
			if (null != mGotoMoneyplan)
			{
				mGotoMoneyplan.onClick.RemoveListener(_onGotoMoneyplanButtonClick);
			}
			mGotoMoneyplan = null;
			mMainView = null;
			mSpecialItem = null;
			mHasChargeRMB = null;
			mEffectRoot_Backlight = null;
			mEffectRoot_GoPayBtn = null;
            mRewardText = null;
        }
		#endregion

        #region Callback
        private void _onBtnCloseButtonClick()
        {
            /* put your code in here */
            OnClickClose();
        }

        private void _onBtnGoButtonClick()
        {
            /* put your code in here */
            OnClickClose();

            //var frame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
            //frame.OpenPayTab();
            if (ClientSystemManager.GetInstance().IsFrameOpen<MallNewFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MallNewFrame>();
            }
            var mallNewFrame = ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.ReChargeMall}) as MallNewFrame;
        }
        private void _onBtnGetButtonClick()
        {
            /* put your code in here */
            PayManager.GetInstance().GetRewards(PayManager.FIRSY_PAY_SUB_ID);
            ClientSystemManager.GetInstance().delayCaller.DelayCall(300, () =>
            {
                ShowButton();

                OnGetForstPayReward();
            });
        }
        private void _onGotoMoneyplanButtonClick()
        {
            const int iConfigID = 9380;
            const int iFinancialPlanID = 8600;
            string frameName = typeof(ActiveChargeFrame).Name + iConfigID.ToString();
            if (ClientSystemManager.GetInstance().IsFrameOpen(frameName))
            {
                var frame = ClientSystemManager.GetInstance().GetFrame(frameName) as ActiveChargeFrame;
                frame.Close(true);
            }
            ActiveManager.GetInstance().OpenActiveFrame(iConfigID, iFinancialPlanID);
        }

        private void _onGotoMonthCardButtonClick()
        {
            //var frame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
            //frame.OpenPayTab();

            if (ClientSystemManager.GetInstance().IsFrameOpen<MallNewFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MallNewFrame>();
            }
            var mallNewFrame = ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.ReChargeMall }) as MallNewFrame;
        }

        #endregion

		public override string GetPrefabPath()
		{
			return "UIFlatten/Prefabs/Vip/FirstPayFrame";
		}

		public void OnUpdateData(UIEvent iEvent)
        {
            ShowReward();
            ShowSpecialItem();
            ShowButton();
            ShowHasChargedRMB();
        }
		protected override void _OnOpenFrame()
		{
            //PayManager.GetInstance().InitPayReturnDisplayTable();

            BindEvent();

            InitEffectRoot();
			ShowReward();
            ShowSpecialItem();
			ShowButton();
            ShowHasChargedRMB();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FirstPayFrameOpen);
		}

        protected override void _OnCloseFrame()
        {
            //PayManager.GetInstance().ClearPayReturnDisplayTable();

            UnBindEvent();

            if (itemDataList != null)
            {
                itemDataList.Clear();
            }

            //注意释放
            ClearAllPayRewardItems();
            ClearEffectRoot();

            mToGetRewardText = "";
            mNotGetRewardText = "";
            mGotRewardText = "";
        }

        void InitEffectRoot()
        {
            if (effect_guang_go == null)
            {
                effect_guang_go = AssetLoader.GetInstance().LoadResAsGameObject(EffUI_shouchong_guang_Path);
                Utility.AttachTo(effect_guang_go, mEffectRoot_Backlight);
            }
            if (effect_goPayBtn_go == null)
            {
                effect_goPayBtn_go = AssetLoader.GetInstance().LoadResAsGameObject(EffUI_shouchong_anniu_Path);
                Utility.AttachTo(effect_goPayBtn_go, mEffectRoot_GoPayBtn);
            }
        }

        void ClearEffectRoot()
        {
            if (effect_guang_go)
            {
                GameObject.Destroy(effect_guang_go);
                effect_guang_go = null;
            }
            if (effect_goPayBtn_go)
            {
                GameObject.Destroy(effect_goPayBtn_go);
                effect_goPayBtn_go = null;
            }
        }

        void BindEvent()
        {
            NetProcess.AddMsgHandler(SceneNotifyActiveTaskStatus.MsgID, OnRecvSceneNotifyActiveTaskStatus);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WelfareFrameClose, OnUpdateData);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMallFrameClosed, OnUpdateData);
        }

        void UnBindEvent()
        {
            NetProcess.RemoveMsgHandler(SceneNotifyActiveTaskStatus.MsgID, OnRecvSceneNotifyActiveTaskStatus);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WelfareFrameClose, OnUpdateData);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMallFrameClosed, OnUpdateData);
        }

        void ClearAllPayRewardItems()
        {
            if (payRewardItems != null)
            {
                for (int i = 0; i < payRewardItems.Count; i++)
                {
                    payRewardItems[i].Clear();
                }
                payRewardItems.Clear();
            }

            if (mSpecialItem != null)
            {
                mSpecialItem.Clear();
            }
        }

        void OnClickClose()
        {
            ClientSystemManager.instance.CloseFrame<FirstPayFrame>();
        }

		void ShowButton()
		{
            InitTRDesc();
            if (!PayManager.GetInstance().HasFirstPay())
            {
                if (mBtnGet)
                {
                    mBtnGet.gameObject.CustomActive(false);
                }
                if (mBtnGo)
                {
                    mBtnGo.gameObject.CustomActive(false);
                }

                if (mEffectRoot_GoPayBtn)
                {
                    mEffectRoot_GoPayBtn.CustomActive(false);
                }
            }

			if (PayManager.GetInstance().CanGetRewards(PayManager.FIRSY_PAY_SUB_ID))
			{
                if (mBtnGet)
                {
                    mBtnGet.gameObject.CustomActive(true);
                    mBtnGet.interactable = true;
                }
                if (mBtnGetText)
                {
                    mBtnGetText.text = mToGetRewardText;
                }
                if (mBtnGetGray)
                {
                    mBtnGetGray.enabled = false;
                }

                if (mBtnGo)
                {
                    mBtnGo.gameObject.CustomActive(false);
                }

                if (mEffectRoot_GoPayBtn)
                {
                    mEffectRoot_GoPayBtn.CustomActive(true);
                }
			}

            if (PayManager.GetInstance().HasFirstPayFinish())
            {
                if (mBtnGet)
                {
                    mBtnGet.gameObject.CustomActive(true);
                    mBtnGet.interactable = false;
                }
                if (mBtnGetText)
                {
                    mBtnGetText.text = mGotRewardText;
                }
                if (mBtnGetGray)
                {
                    mBtnGetGray.enabled = true;
                }

                if (mBtnGo)
                {
                    mBtnGo.gameObject.CustomActive(false);
                }

                if (mEffectRoot_GoPayBtn)
                {
                    mEffectRoot_GoPayBtn.CustomActive(false);
                }
            }
		}

        private void InitTRDesc()
        {
            mToGetRewardText = TR.Value("vip_month_card_first_buy_next_pay_return_toget");
            mNotGetRewardText = TR.Value("vip_month_card_first_buy_next_pay_return_notget");
            mGotRewardText = TR.Value("vip_month_card_first_buy_next_pay_return_got");
        }

		string GetWrapPath(string resPath)
		{
			string wrapObjectPath = null;
			string lowRes = resPath.ToLower();
			if (lowRes.Contains(Global.WEAPON_SWORD_NAME))
				wrapObjectPath = "UIFlatten/Prefabs/Vip/ShowSword";
			else if (lowRes.Contains(Global.WEAPON_GUN_NAME))
			{
				if (lowRes.Contains("cannon"))
					wrapObjectPath = "UIFlatten/Prefabs/Vip/ShowGun_cannon";
				else
					wrapObjectPath = "UIFlatten/Prefabs/Vip/ShowGun";
			}

			else if (lowRes.Contains(Global.WEAPON_MAGE_NAME))
				wrapObjectPath = "UIFlatten/Prefabs/Vip/ShowMage";
            else if (lowRes.Contains(Global.WEAPON_FIGHTER_NAME))
                wrapObjectPath = "UIFlatten/Prefabs/Vip/ShowFighter";

			return wrapObjectPath;
		}

		void ShowModel()
		{
			int occur = PlayerBaseData.GetInstance().JobTableID;
            ProtoTable.JobTable data = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(occur);
			if (data != null)
			{
				if (objectRenderer == null)
                    objectRenderer = mWeaponModelRoot.GetComponent<GeObjectRenderer>();

				var tokens = data.FirstPayWeapon.Split('_');
				if (tokens.Length == 2)
				{
					var itemID = Convert.ToInt32(tokens[0]);
					var strengthLevel = Convert.ToInt32(tokens[1]);

					string path = Utility.GetItemModulePath(itemID);
					//path = "testGun";
					objectRenderer.LoadObject(path, 30, GetWrapPath(path));

                    IList<int> akPosInfo = data.FirstPayModelTransform;
                    if(akPosInfo.Count >= 4)
                    {
                        objectRenderer.SetLocalScale(akPosInfo[0] / 1000.0f);
                        objectRenderer.SetLocalPosition(new Vector3(akPosInfo[1] / 1000.0f, akPosInfo[2] / 1000.0f, akPosInfo[3] / 1000.0f));
                    }
                    else
                    {
                        Logger.LogFormat("职业表表项FirstPayModelTransform填写错误,jobid = {0}", occur);
                    }

					//if (strengthLevel > 0)
					//	objectRenderer.ChangePhase(BeUtility.GetStrengthenEffectName(path), strengthLevel);

					var itemData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemID);
					if (itemData != null)
					{
						if (mWeaponName != null)
						{
							mWeaponName.text = itemData.Name;
							if (strengthLevel > 0)
							{
								mWeaponName.text += "\n(强化等级+" + strengthLevel + ")";
							}
						}
							
					}

				}
			}
		}

        void ShowSpecialItem()
        {
            if (itemDataList == null)
            {
                return;
            }
            if (mMainView == null)
            {
                return;
            }
            if (mMainView.IsInitialised() == false)
            {
                mMainView.Initialize();
                mMainView.onBindItem = (GameObject go) =>
                {
                    PayRewardItem payItem = null;
                    if (go)
                    {
                        payItem = go.GetComponent<PayRewardItem>();
                    }
                    return payItem;
                };
            }
            mMainView.onItemVisiable = (var) =>
            {
                if (var == null)
                {
                    return;
                }
                int iIndex = var.m_index;
                if (iIndex >= 0 && iIndex < itemDataList.Count)
                {
                    ItemData itemDetailData = ItemDataManager.CreateItemDataFromTable(itemDataList[iIndex].ID);
                    if (itemDetailData == null)
                    {
                        Logger.LogErrorFormat("Can find !!! Please Check item data id {0} !!!", itemDataList[iIndex].ID);
                        return;
                    }
                    itemDetailData.Count = itemDataList[iIndex].Num;

                    if (mMainView && itemDetailData.TableID == PayManager.GetInstance().weaponItemID)
                    {
                        itemDetailData.StrengthenLevel = PayManager.GetInstance().weaponStrengthLevel;
                    }

                    PayRewardItem payItem = var.gameObjectBindScript as PayRewardItem;
                    if (payItem != null)
                    {
                        payItem.Initialize(this, itemDetailData,true, false);
                        payItem.RefreshView();
                        if (payRewardItems != null && !payRewardItems.Contains(payItem))
                        {
                            payRewardItems.Add(payItem);
                        }
                    }
                }
            };
            mMainView.SetElementAmount(itemDataList.Count);

            if (mSpecialItem)
            {
                int specialItemDataId = PayManager.GetInstance().GetPayReturnSpecialResID(PayManager.FIRSY_PAY_SUB_ID, itemDataList);
                ItemData detailData = ItemDataManager.CreateItemDataFromTable(specialItemDataId);
                if (detailData == null)
                {
                    Logger.LogErrorFormat("Can find !!! Please Check item data id {0} !!!", specialItemDataId);
                    return;
                }
                mSpecialItem.Initialize(this, detailData, false);
                mSpecialItem.RefreshView(false, false);
                string specialItemIconPath = PayManager.GetInstance().GetPayReturnSpecialResPath(PayManager.FIRSY_PAY_SUB_ID, itemDataList);
                mSpecialItem.SetItemIcon(specialItemIconPath);
                mSpecialItem.onPayItemClick = () =>
                {
                    if (detailData.TableID == PayManager.GetInstance().weaponItemID)
                    {
                        detailData.StrengthenLevel = PayManager.GetInstance().weaponStrengthLevel;
                    }
                    ItemTipManager.GetInstance().ShowTip(detailData);
                };
                if (mRewardText)
                {
                    if (detailData.TableID == PayManager.GetInstance().weaponItemID)
                    {
                        detailData.StrengthenLevel = PayManager.GetInstance().weaponStrengthLevel;
                        mRewardText.text = string.Format("{0}+{1}", detailData.Name, detailData.StrengthenLevel);
                    }
                    else
                    {
                        mRewardText.text = detailData.Name;
                    }
                }
            }
        }

		void ShowReward()
		{
            var rewards = PayManager.GetInstance().GetFirstPayItems();

            if (rewards == null)
            {
                return;
            }

            if (itemDataList != null)
            {
                itemDataList.Clear();
            }

            AwardItemData fAward = new AwardItemData() { ID = (int)PayManager.GetInstance().weaponItemID, Num = 1 };
            if (itemDataList != null && !itemDataList.Contains(fAward))
            {
                itemDataList.Add(fAward);
            }

            Dictionary<uint, int>.Enumerator enumerator = rewards.GetEnumerator();
            while (enumerator.MoveNext())
            {
                uint itemID = enumerator.Current.Key;
                int itemNum = enumerator.Current.Value;

                if (itemDataList != null)
                {
                    itemDataList.Add(new AwardItemData() { ID = (int)itemID, Num = itemNum });
                }
            }

            //Utility.AddItemIcon(this, mScrollContent, PayManager.GetInstance().weaponItemID, 1, PayManager.GetInstance().weaponStrengthLevel);

            //if (rewards != null)
            //{
            //    foreach(var reward in rewards)
            //    {
            //        uint itemID = reward.Key;
            //        int itemNum = reward.Value;

            //        Utility.AddItemIcon(this, mScrollContent, itemID, itemNum); 
            //    }
            //}
		}

        void ShowHasChargedRMB()
        {
            if (mHasChargeRMB)
            {
                mHasChargeRMB.text = string.Format(TR.Value("vip_month_card_first_buy_first_has_pay"), PayManager.GetInstance().GetCurrentRolePayMoney());
            }
        }

        /// <summary>
        /// 首次充值超过6元 领取首充奖励后 再跳转到剩余奖励领取界面
        /// </summary>
        void OnGetForstPayReward()
        {
            if (PayManager.GetInstance().CanGetRewards(PayManager.FIRSY_PAY_SUB_ID) == false)
            {
                int canGetRewardCount = PayManager.GetInstance().GetCurrFinishActivityNum();
                if (canGetRewardCount > 1)
                {
                    //var frame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
                    //frame.SwitchPage(VipTabType.PAY_RETRN);
                    ClientSystemManager.GetInstance().OpenFrame<SecondPayFrame>(FrameLayer.Middle);
                }
                this.Close();
            }
        }

        #region EventCallback

        void OnRecvSceneNotifyActiveTaskStatus(MsgDATA data)
        {
            SceneNotifyActiveTaskStatus kRecv = new SceneNotifyActiveTaskStatus();
            kRecv.decode(data.bytes);
            if (kRecv.taskId == PayManager.FIRSY_PAY_SUB_ID)
            {
                ShowButton();
                ShowHasChargedRMB();
            }
        }

        #endregion
    }

}

