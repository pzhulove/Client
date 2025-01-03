using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Protocol;
using Network;

namespace GameClient
{
	public class PayItemData
	{
		public int ID;
		public int price;
		public string desc;
		public uint itemID;
		public int itemNum;
		public int firstBonusNum;
		public int bonusNum;
		public bool hasFirstBonus;
		public int remainDays;
		public int limit;

		public string icon;
		public uint tags;

		public bool isMonthCard;
		public string itemContent;

		public PayItemData()
		{
			
		}

		public PayItemData(ChargeGoods good)
		{
			ID = good.id;
			price = good.money;
			desc = good.desc;
			itemID = good.itemId;
			itemNum = good.num;
			firstBonusNum = good.firstAddNum;
			bonusNum = good.unfirstAddNum;
			icon = good.icon;
			hasFirstBonus = good.isFirstCharge > 0;
			tags = good.tags;
			remainDays = (int)good.remainDays;
			limit = (int)good.remainTimes;

			var itemData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)itemID);
			if (itemData != null && itemData.SubType == ProtoTable.ItemTable.eSubType.MonthCard)
			{
				isMonthCard = true;
				itemContent = itemData.Description;
			}
		}

		public bool HasMark()
		{
			return (tags & (1<<((int)ChargeGoodsTag.Recommend))) != 0;
		}
	}

	public class PayItem 
	{
		GameObject root;
		GameObject parent;
		public static string res_payItem = "UIFlatten/Prefabs/Vip/PayItem";
		public const string dianjuanRes = "UI/Image/Icon/Icon_Duanwei/UI_Tongyong_Tubiao_Dianjuan.png:UI_Tongyong_Tubiao_Dianjuan";
		public const int MONTH_CARD_BUY_AGAIN_NUM = 3;

		ComCommonBind mBind;

/*		Button btnPay;
		Text txtPrice;
		Text txtDescribe;
		GameObject objDescribe2;//额外信息
		Text txtBonusDes;
		Text txtBonus;*/

		//Text txtLeftDay;

/*		Image icon;
		Image mark;
		Image icon1;
		Image icon2;*/

		private Text mTxtLeftDay = null;
		private Image mImgBack = null;
		private Image mImgIcon = null;
		private GameObject mObjMark = null;
		private GameObject mObjBonus = null;
		private Text mTxtBonus = null;
		private GameObject mDescNormalRoot = null;
		private Text mTxtLimit = null;
		private GameObject mBgEffectRoot = null;
		private GameObject mForeEffectRoot = null;
		private UIGray mComGray = null;
		private Text mTxtGetDesc = null;

		private Button mBtnPay = null;
		private Text mTxtPrice = null;


		private Text mDescRoot = null;
		private GameObject mObjDescribe = null;


		public PayItemData data;

		public PayItem(PayItemData data, GameObject parent)
		{
			this.data = data;
			this.parent = parent;

			root = AssetLoader.instance.LoadResAsGameObject(res_payItem);
			if (root != null)
			{
				mBind = root.GetComponent<ComCommonBind>();
				//txtLeftDay = mBind.GetCom<Text>("txtLeftDay");
				//txtItemContent = mBind.GetCom<Text>("txtItemContent");
				//mDescRoot = mBind.GetCom<Text>("descRoot");
				//mObjDescribe = mBind.GetGameObject("objDescribe");
				//mDescNormalRoot = mBind.GetGameObject("descNormalRoot");
				//mTxtLimit = mBind.GetCom<Text>("txtLimit");
				//mComGray = mBind.GetCom<UIGray>("comGray");


				mTxtLeftDay = mBind.GetCom<Text>("txtLeftDay");
				mImgBack = mBind.GetCom<Image>("imgBack");
				mImgIcon = mBind.GetCom<Image>("imgIcon");
				mObjMark = mBind.GetGameObject("objMark");
				mObjBonus = mBind.GetGameObject("objBonus");
				mTxtBonus = mBind.GetCom<Text>("txtBonus");
				mDescNormalRoot = mBind.GetGameObject("descNormalRoot");
				mTxtLimit = mBind.GetCom<Text>("txtLimit");
				mBgEffectRoot = mBind.GetGameObject("bgEffectRoot");
				mForeEffectRoot = mBind.GetGameObject("foreEffectRoot");
				mComGray = mBind.GetCom<UIGray>("comGray");
				mTxtGetDesc = mBind.GetCom<Text>("txtGetDesc");
				mBtnPay = mBind.GetCom<Button>("btnPay");
				//mBtnPay.onClick.AddListener(_onBtnPayButtonClick);
				mTxtPrice = mBind.GetCom<Text>("txtPrice");


				mBgEffectRoot = mBind.GetGameObject("bgEffectRoot");
				mForeEffectRoot = mBind.GetGameObject("foreEffectRoot");

				//btnPay = Utility.FindComponent<Button>(root, "BtnPay", false);
				mBtnPay.onClick.AddListener(()=>{

					/*
					if (data.isMonthCard && data.remainDays > 0 && data.remainDays > MONTH_CARD_BUY_AGAIN_NUM)
					{
						SystemNotifyManager.SystemNotify(1205);
						return;
					}*/

					if (data.limit <= 0)
					{
						SystemNotifyManager.SystemNotify(1121);
					}
					else
						DoPay();
				});

				/*txtPrice = Utility.FindComponent<Text>(root, "BtnPay/price", false);
				txtDescribe = mBind.GetCom<Text>("descText");
				objDescribe2 = Utility.FindGameObject(root, "descibe2", false);
				objDescribe2.CustomActive(false);

				txtBonusDes = Utility.FindComponent<Text>(objDescribe2, "text", false);
				txtBonus = Utility.FindComponent<Text>(objDescribe2, "Bonus/text", false);

				icon = Utility.FindComponent<Image>(root, "icon", false);
				mark = Utility.FindComponent<Image>(root, "mark", false);

				icon1 = Utility.FindComponent<Image>(root, "descibe2/Bonus/Image", false);
				icon2 = mBind.GetCom<Image>("descImage");*/

				//Utility.SetImageIcon(icon1.transform.gameObject, dianjuanRes);
				//Utility.SetImageIcon(icon2.transform.gameObject, dianjuanRes);

				mObjMark.CustomActive(false);

				if (data.isMonthCard)
				{
					Utility.SetImageIcon(mImgBack.gameObject, "UI/Image/Packed/p_UI_Vip.png:UI_Vip_Item_02");
					mObjBonus.CustomActive(false);
					mImgIcon.gameObject.CustomActive(false);
					mDescNormalRoot.CustomActive(false);

				}
					

				Utility.AttachTo(root, parent);

				/*
				var tableData = TableManager.GetInstance().GetTableItem<ProtoTable.ChargeMallTable>(data.ID);
				if (tableData != null)
				{
					if (Utility.IsStringValid(tableData.Effect1))
					{
						var effect = AssetLoader.instance.LoadResAsGameObject(tableData.Effect1, false);
						if (effect != null)
							Utility.AttachTo(effect, mForeEffectRoot);
					}

					if (Utility.IsStringValid(tableData.Effect2))
					{
						var effect = AssetLoader.instance.LoadResAsGameObject(tableData.Effect2, false);
						if (effect != null)
							Utility.AttachTo(effect, mBgEffectRoot);
					}
				}*/
			}

			SetData();
		}
			

		public void UpdateData(PayItemData data)
		{
			this.data = data;
			SetData();
		}

		public void SetData()
		{
			if (data == null)
				return;

            mTxtPrice.text = TR.Value("vip_charge_item_price_format", data.price);// "+" + data.price;
            mTxtGetDesc.text = data.desc;

            // mTxtGetDesc.text = TR.Value("vip_charge_item_reward_num_format", data.itemNum);//string.Format("*{0}",data.itemNum.ToString());


			//mComGray.enabled = false;
			if (data.limit <= 0)
			{
                mTxtLimit.text = TR.Value("vip_charge_daily_limit_num_max");//"今日购买次数已达上限";
				mTxtLimit.color = Color.red;
				//mComGray.enabled = true;
			}

			if (data.HasMark())
				mObjMark.CustomActive(true);
			mObjBonus.CustomActive(false);


			if (!data.isMonthCard)
			{
				if (data.limit >= byte.MaxValue)
					mTxtLimit.gameObject.CustomActive(false);
				else if (data.limit > 0)
				{
					mTxtLimit.gameObject.CustomActive(true);
                    mTxtLimit.text = TR.Value("vip_charge_daily_limit_num_format", data.limit);//"今日限购次数:" + data.limit;
				}
				if ((data.hasFirstBonus && data.firstBonusNum > 0 && !data.isMonthCard) || data.bonusNum > 0)
				{

					mObjBonus.CustomActive(true);
                    //mTxtBonus.text = data.firstBonusNum + "";
                    mTxtBonus.text = TR.Value("vip_charge_first_present_more", data.firstBonusNum);
                    /*
					objDescribe2.CustomActive(true);
					if (data.hasFirstBonus && data.firstBonusNum > 0)
					{
						txtBonusDes.text = "首充赠";
						txtBonus.text = data.firstBonusNum + "";
					}
					else if (data.bonusNum > 0)
					{
						txtBonusDes.text = "赠";
						txtBonus.text = data.bonusNum + "";
					}*/
                }
				Utility.SetImageIcon(mImgIcon.transform.gameObject, data.icon);
			}
			else
			{
				mTxtPrice.gameObject.CustomActive(true);
				mTxtLeftDay.gameObject.CustomActive(false);
				if (data.remainDays > 0)
				{
					mTxtPrice.gameObject.CustomActive(false);
					mTxtLeftDay.gameObject.CustomActive(true);
					mTxtLeftDay.text = TR.Value("vip_month_card_remain_time", data.remainDays-1);
				}


				//mDescRoot.CustomActive(true);
				//mDescNormalRoot.CustomActive(false);

				//objDescribe2.CustomActive(true);
				//txtBonus.text = data.desc;

               // txtLeftDay.text = "";
				/*
                if (data.remainDays > 0)
                {
					if (data.remainDays <= MONTH_CARD_BUY_AGAIN_NUM)
					{
						objDescribe2.CustomActive(true);
						txtBonusDes.text = "续费得";
						txtPrice.text = TR.Value("vip_month_card_continue_buy_cost_desc", data.price);
					}
					else {
						objDescribe2.CustomActive(false);
						txtPrice.text = "已购买";
					}
						
					txtLeftDay.text = TR.Value("vip_month_card_remain_time", data.remainDays-1);
                }
                else
                {
					txtBonusDes.text = "购买得";
                    txtPrice.text = TR.Value("vip_month_card_first_buy_cost_desc", data.price);
                }*/
			}


		}

		public void DoPay()
		{
			PayManager.GetInstance().lastPayIsMonthCard = data.isMonthCard;
			if (data.isMonthCard)
			{
				PayManager.GetInstance().lastMontchCardNeedOpenWindow = data.remainDays <= 0;
			}
			PayManager.GetInstance().DoPay(data.ID, data.price);
		}
	}

	public class PayFrame {

		List<PayItem> payItems = null;

		GameObject goScrollContent;
		GameObject root;

		public bool isOpened = false;

		public PayFrame(GameObject root, GameObject scrollRoot)
		{
			goScrollContent = scrollRoot;
			this.root = root;
		}

		public void Open()
		{
			isOpened = true;           
            PluginManager.GetInstance().TryGetIOSAppstoreProductIds();
			SendPayItemReq();
			Bind();
			Show(true);
		}

		public void Close()
		{
			if (isOpened)
			{
				UnBind();
				if (payItems != null)
				{
					payItems.Clear();
					payItems = null;
				}
			}
		}

		public void Show(bool show)
		{
			root.CustomActive(show);
		}

		void Bind()
		{
			NetProcess.AddMsgHandler(WorldBillingGoodsRes.MsgID, OnReceivePayItem);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPayResultNotify, OnReceivePayResult);
		}

		void UnBind()
		{
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPayResultNotify, OnReceivePayResult);
			NetProcess.RemoveMsgHandler(WorldBillingGoodsRes.MsgID, OnReceivePayItem);
		}

		void SendPayItemReq()
		{
#if MG_TEST || MG_TEST2
            return;
#endif

            WorldBillingGoodsReq msg = new WorldBillingGoodsReq();
			NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
		}

		void OnReceivePayItem(MsgDATA msg)
		{
			WorldBillingGoodsRes msgData = new WorldBillingGoodsRes();
			msgData.decode(msg.bytes);

			PayItemData[] datas = new PayItemData[msgData.goods.Length];

			for(int i=0; i<msgData.goods.Length; ++i)
			{
				ChargeGoods originItem = msgData.goods[i];

				PayItemData data = new PayItemData(originItem);
				datas[i] = data;
			}

			if (payItems != null)
				UpdatePayItems(datas);
			else
				CreatePayItems(datas);
		}

		void OnReceivePayResult(UIEvent uiEvent)
		{
			const int notifySucceed = 2600001;
			const int notifyFail = 2600002;
			const int notifyMonthCardSucceed = 1202;


			string result = (string)uiEvent.Param1;

			if (result == "0")
			{
				if (PayManager.GetInstance().lastPayIsMonthCard)
				{
					SystemNotifyManager.SystemNotify(notifyMonthCardSucceed);
					if (PayManager.GetInstance().lastMontchCardNeedOpenWindow)
						OpenMonthCard();
				}
					
				else
					SystemNotifyManager.SystemNotify(notifySucceed);
			}
				
			else if (result == "1")
				SystemNotifyManager.SystemNotify(notifyFail);

			//收到充值结果再重新刷一下界面
			SendPayItemReq();
		}

		void OpenMonthCard()
		{
			ActiveManager.GetInstance().OpenActiveFrame(9380, 6000);
		}

		void CreatePayItems(PayItemData[] datas)
		{
			if (payItems == null)
			{
				payItems = new List<PayItem>();
			}
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.CardData, datas[0]);
			for(int i=1; i<datas.Length; ++i)//这边服务器传回来的数据要把第一个去掉，这边先将i改成1，之后配合服务器再进行更改。
            {
				var payItem = new PayItem(datas[i], goScrollContent);
				payItems.Add(payItem);
			}
		}

		void UpdatePayItems(PayItemData[] datas)
		{
			for(int i=0; i<payItems.Count; ++i)
			{
				var payItem = payItems[i];
				for(int j=0; j<datas.Length; ++j)
				{
					if (payItem.data.ID == datas[j].ID)
						payItem.UpdateData(datas[j]);
				}
			}
		}

	}
}


