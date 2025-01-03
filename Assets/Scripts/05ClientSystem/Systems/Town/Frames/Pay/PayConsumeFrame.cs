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
	public class PayConsumeItem2
	{
		GameObject root;
		GameObject parent;

		public const string res_consumeItem = "UIFlatten/Prefabs/Vip/PayConsumeItem2";

		ActiveManager.ActivityData data;
		ClientFrame THIS;

		private Text mTxtMoney = null;
		private Slider mSlider = null;
		private Text mTxtProgress = null;
		private GridLayoutGroup mGirdItem = null;
		private Text mTxtStaus = null;
		private Button mBtnGet = null;

		bool isFirstPay = false;

		public PayConsumeItem2(ActiveManager.ActivityData data, GameObject parent, ClientFrame THIS)
		{
			this.data = data;
			this.parent = parent;
			this.THIS = THIS;

			root = AssetLoader.instance.LoadResAsGameObject(res_consumeItem);
			if (root != null)
			{
				var mBind = root.GetComponent<ComCommonBind>();

				mTxtMoney = mBind.GetCom<Text>("txtMoney");
				mSlider = mBind.GetCom<Slider>("slider");
				mTxtProgress = mBind.GetCom<Text>("txtProgress");
				mGirdItem = mBind.GetCom<GridLayoutGroup>("girdItem");
				mTxtStaus = mBind.GetCom<Text>("txtStaus");
				mBtnGet = mBind.GetCom<Button>("btnGet");
				mBtnGet.onClick.AddListener(OnClickGet);

				mBtnGet.gameObject.CustomActive(true);

				mTxtStaus.gameObject.CustomActive(false);

				Utility.AttachTo(root, parent);
			}

			SetData();
		}

		public void SetStat()
		{
			bool canGet = PayManager.GetInstance().CanGetRewards(data.ID);
			var comGray = mBtnGet.gameObject.GetComponent<UIGray>();
			if (comGray == null)
				comGray = mBtnGet.gameObject.AddComponent<UIGray>();
			comGray.enabled = !canGet;

			int curPay = 0;
			int nextPay = 0;

			//mTxtStaus.text = "未达成";
			if (data.akActivityValues.Count == 2)
			{
				curPay = Convert.ToInt32(data.akActivityValues[0].value);
				nextPay = Convert.ToInt32(data.akActivityValues[1].value);

				if (data.status > PayManager.STATUS_TASK_UNFINISH)
				{
					curPay = curPay+nextPay;
					nextPay = 0;
				}

				mTxtMoney.text = (curPay + nextPay).ToString();/*data.activeItem.Desc*/;
				if ((curPay + nextPay)<=6)
					isFirstPay = true;
			}
			else {
				if (data.status <= PayManager.STATUS_TASK_UNFINISH)
				{
					curPay = 0;
					nextPay = 6;
				}
				else {
					curPay = 6;
					nextPay = 0;
				}
				mTxtMoney.text = "6";
				isFirstPay = true;
			}

			mTxtProgress.text = string.Format("{0}/{1}", curPay, curPay + nextPay);
			mSlider.value = curPay / (float)(curPay + nextPay);

			if (data.status >= PayManager.STATUS_TASK_OVER)
			{
				//mTxtStaus.text = "已领取";
				mTxtStaus.gameObject.CustomActive(true);
				mBtnGet.gameObject.CustomActive(false);
			}
				
			
		}

		void OnClickGet()
		{
			PayManager.GetInstance().GetRewards(data.ID);
			ClientSystemManager.GetInstance().delayCaller.DelayCall(300, ()=>{
				SetStat();	
			});
		}

		public void SetData()
		{
			if (data == null)
				return;

			SetStat();

			var items = PayManager.GetInstance().GetAwardItems(data);
			if (isFirstPay)
			{
				items = PayManager.GetInstance().GetFirstPayItems();
				Utility.AddItemIcon(THIS, mGirdItem.gameObject, PayManager.GetInstance().weaponItemID, 1, PayManager.GetInstance().weaponStrengthLevel);
			}

			int width = 628;
			int cellWidth = 120;

			if (!isFirstPay)
			{
				/*
				float space = (width - items.Count*cellWidth) / (float)(items.Count + 1);
				var tmp = mGirdItem.spacing;
				tmp.x = space;
				mGirdItem.spacing = tmp;
				*/
			}
				

			Dictionary<uint, int>.Enumerator enumerator = items.GetEnumerator();
			while(enumerator.MoveNext())
			{
				uint itemID = enumerator.Current.Key;
				int itemNum = enumerator.Current.Value;

				Utility.AddItemIcon(THIS, mGirdItem.gameObject, itemID, itemNum); 

/*				var item = THIS.CreateComItem(mGirdItem.gameObject);
				var itemData = ItemDataManager.CreateItemDataFromTable((int)itemID);
				itemData.Count = itemNum;
				item.Setup(itemData, (GameObject obj, ItemData item1) => { ItemTipManager.GetInstance().ShowTip(item1); });*/
			}

		}
	}

	public class PayConsumeItem 
	{
		GameObject root;
		GameObject parent;

		public const string res_consumeItem = "UIFlatten/Prefabs/Vip/PayConsumeItem";

		ActiveManager.ActivityData data;
		ClientFrame THIS;

		Text txtMoney;
		GameObject rootItems;
		Text txtStatus;
		GameObject goBtnGet;

		public PayConsumeItem(ActiveManager.ActivityData data, GameObject parent, ClientFrame THIS)
		{
			this.data = data;
			this.parent = parent;
			this.THIS = THIS;

			root = AssetLoader.instance.LoadResAsGameObject(res_consumeItem);
			if (root != null)
			{
				txtMoney = Utility.FindComponent<Text>(root, "title/tmp/Money", false);
				rootItems = Utility.FindGameObject(root, "item", false);
				txtStatus = Utility.FindComponent<Text>(root, "status", false);
				goBtnGet =  Utility.FindGameObject(root, "btnGet", false);
				goBtnGet.GetComponent<Button>().onClick.AddListener(OnClickGet);
				goBtnGet.CustomActive(false);

				goBtnGet.CustomActive(true);
				txtStatus.CustomActive(false);

				Utility.AttachTo(root, parent);
			}

			SetData();
		}

		public void SetStat()
		{
			if (!ClientSystemManager.GetInstance().IsFrameOpen<PayConsumeFrame>())
				return;

			bool canGet = PayManager.GetInstance().CanGetRewards(data.ID);

			var comGray = goBtnGet.GetComponent<UIGray>();
			if (comGray == null)
				comGray = goBtnGet.AddComponent<UIGray>();
			comGray.enabled = !canGet;

			//goBtnGet.CustomActive();

			txtMoney.text = data.activeItem.Desc;

			var curPay = 0;
			var nextPay = 0;
			//txtStatus.text = "未达成";

			if (data.status >= PayManager.STATUS_TASK_OVER)
			{
				txtStatus.CustomActive(true);
				txtStatus.text = "已领取";
				goBtnGet.CustomActive(false);
			}
				
		}

		void OnClickGet()
		{
			PayManager.GetInstance().GetRewards(data.ID);
			ClientSystemManager.GetInstance().delayCaller.DelayCall(300, ()=>{
				SetStat();	
			});
		}

		public void SetData()
		{
			if (data == null)
				return;

			SetStat();

			var items = PayManager.GetInstance().GetAwardItems(data);
			Dictionary<uint, int>.Enumerator enumerator = items.GetEnumerator();
			while(enumerator.MoveNext())
			{
				uint itemID = enumerator.Current.Key;
				int itemNum = enumerator.Current.Value;

				var item = THIS.CreateComItem(rootItems);
				var itemData = ItemDataManager.CreateItemDataFromTable((int)itemID);
				itemData.Count = itemNum;
				item.Setup(itemData, (GameObject obj, ItemData item1) => { ItemTipManager.GetInstance().ShowTip(item1); });
			}
			
		}
	}

    public class PayConsumeItem3
    {
        GameObject root;
        GameObject parent;

        public const string res_consumeItem = "UIFlatten/Prefabs/Vip/PayConsumeItem3";

        ActiveManager.ActivityData data;
        ClientFrame THIS;

        private Text mTxtMoney = null;
        private GameObject mGirdItem = null;
        private GameObject mTxtStaus = null;
        private Button mBtnGet = null;

        bool isFirstPay = false;
        int awardItemWidth = 50;

       public PayConsumeItem3(ActiveManager.ActivityData data, GameObject parent, ClientFrame THIS)
		{
			this.data = data;
			this.parent = parent;
			this.THIS = THIS;

			root = AssetLoader.instance.LoadResAsGameObject(res_consumeItem);
			if (root != null)
			{
				var mBind = root.GetComponent<ComCommonBind>();

				mTxtMoney = mBind.GetCom<Text>("txtMoney");
                mGirdItem = mBind.GetGameObject("girdItem");
				mTxtStaus = mBind.GetGameObject("txtStaus");
				mBtnGet = mBind.GetCom<Button>("btnGet");
                mBtnGet.onClick.RemoveListener(OnClickGet);
				mBtnGet.onClick.AddListener(OnClickGet);

				mBtnGet.gameObject.CustomActive(true);
				mTxtStaus.gameObject.CustomActive(false);

				Utility.AttachTo(root, parent);
			}

			SetData();
		}

       void OnClickGet()
       {
           PayManager.GetInstance().GetRewards(data.ID);
           ClientSystemManager.GetInstance().delayCaller.DelayCall(300, () =>
           {
               SetStat();
           });
       }

       public void SetData()
       {
           if (data == null)
               return;

           SetStat();

           var items = PayManager.GetInstance().GetAwardItems(data);
           if (isFirstPay)
           {
               items = PayManager.GetInstance().GetFirstPayItems();
               Utility.AddItemIcon(THIS, mGirdItem.gameObject, PayManager.GetInstance().weaponItemID, 1, PayManager.GetInstance().weaponStrengthLevel);
           }

           if (!isFirstPay)
           {
               /*
               float space = (width - items.Count*cellWidth) / (float)(items.Count + 1);
               var tmp = mGirdItem.spacing;
               tmp.x = space;
               mGirdItem.spacing = tmp;
               */
           }

           Dictionary<uint, int>.Enumerator enumerator = items.GetEnumerator();
           if (items.Count <= 4)
           {
               awardItemWidth = 65;
           }
           else if (items.Count > 4)
           {
               awardItemWidth = 50;
           }
           while (enumerator.MoveNext())
           {
               uint itemID = enumerator.Current.Key;
               int itemNum = enumerator.Current.Value;

               var comItem = Utility.AddItemIcon(THIS, mGirdItem.gameObject, itemID, itemNum);
               if (comItem != null)
               {
                   PayAwardItem payAwardItem = new PayAwardItem(comItem, mGirdItem.gameObject);
                   comItem.ItemData.Count = 1;
                   if (payAwardItem != null)
                   {
                       var itemEle = payAwardItem.GetCurrAwardItemEle();
                       if (itemEle != null)
                           itemEle.preferredWidth = itemEle.preferredHeight = awardItemWidth;
                   }
               }
           }

       }

       public void SetStat()
       {
           bool canGet = PayManager.GetInstance().CanGetRewards(data.ID);
            mBtnGet.interactable = canGet;
           var comGray = mBtnGet.gameObject.GetComponent<UIGray>();
           if (comGray == null)
               comGray = mBtnGet.gameObject.AddComponent<UIGray>();
           comGray.enabled = !canGet;

           //var btnImg = mBtnGet.gameObject.GetComponent<Image>();
           //if (btnImg)
           //{
           //    btnImg.color = canGet ? Color.white : Color.gray;
           //    mBtnGet.interactable = canGet;
           //}

           int curPay = 0;
           int nextPay = 0;

           //mTxtStaus.text = "未达成";
           if (data.akActivityValues.Count == 2)
           {
               curPay = Convert.ToInt32(data.akActivityValues[0].value);
               nextPay = Convert.ToInt32(data.akActivityValues[1].value);

               if (data.status > PayManager.STATUS_TASK_UNFINISH)
               {
                   curPay = curPay + nextPay;
                   nextPay = 0;
               }

               mTxtMoney.text = (curPay + nextPay).ToString();/*data.activeItem.Desc*/;
               if ((curPay + nextPay) <= 6)
                   isFirstPay = true;
           }
           else
           {
               if (data.status <= PayManager.STATUS_TASK_UNFINISH)
               {
                   curPay = 0;
                   nextPay = 6;
               }
               else
               {
                   curPay = 6;
                   nextPay = 0;
               }
               mTxtMoney.text = "6";
               isFirstPay = true;
           }

           if (data.status >= PayManager.STATUS_TASK_OVER)
           {
               //mTxtStaus.text = "已领取";
               mTxtStaus.gameObject.CustomActive(true);
               mBtnGet.gameObject.CustomActive(false);
           }
       }
    }

    public class PayConsumeItem4
    {
        GameObject root;
        GameObject parent;

        public const string res_consumeItem = "UIFlatten/Prefabs/Vip/PayConsumeItem4";

        ActiveManager.ActivityData data;
        ClientFrame THIS;

        private Text mTxtMoney = null;
        private GameObject mGirdItem = null;
        private Slider mSlider = null;
        private Text mTxtProgress = null;
        private GameObject mTxtStaus = null;
        private Button mBtnGet = null;

        bool isFirstPay = false;
        int awardItemWidth = 50;

       public PayConsumeItem4(ActiveManager.ActivityData data, GameObject parent, ClientFrame THIS)
		{
			this.data = data;
			this.parent = parent;
			this.THIS = THIS;

			root = AssetLoader.instance.LoadResAsGameObject(res_consumeItem);
			if (root != null)
			{
				var mBind = root.GetComponent<ComCommonBind>();

				mTxtMoney = mBind.GetCom<Text>("txtMoney");
                mGirdItem = mBind.GetGameObject("girdItem");
                mSlider = mBind.GetCom<Slider>("slider");
                mTxtProgress = mBind.GetCom<Text>("txtProgress");
				mTxtStaus = mBind.GetGameObject("txtStaus");
				mBtnGet = mBind.GetCom<Button>("btnGet");
                mBtnGet.onClick.RemoveListener(OnClickGet);
				mBtnGet.onClick.AddListener(OnClickGet);

				mBtnGet.gameObject.CustomActive(true);
				mTxtStaus.gameObject.CustomActive(false);

				Utility.AttachTo(root, parent);
			}

			SetData();
		}

       void OnClickGet()
       {
           PayManager.GetInstance().GetRewards(data.ID);
           ClientSystemManager.GetInstance().delayCaller.DelayCall(300, () =>
           {
               SetStat();
               var frame_mall = ClientSystemManager.instance.GetFrame(typeof(MallPayFrame)) as MallPayFrame;
               var frame_vip = ClientSystemManager.instance.GetFrame(typeof(VipFrame)) as VipFrame;
               if (frame_mall != null && frame_vip != null)
                   UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdatePayData);
           });
       }

       public void SetData()
       {
           if (data == null)
               return;

           SetStat();

           ComItem firstPayComItem = null;

           var items = PayManager.GetInstance().GetAwardItems(data);
           if (isFirstPay)
           {
               items = PayManager.GetInstance().GetFirstPayItems();
               firstPayComItem = Utility.AddItemIcon(THIS, mGirdItem.gameObject, PayManager.GetInstance().weaponItemID, 1, PayManager.GetInstance().weaponStrengthLevel);
           }

           if (!isFirstPay)
           {
               /*
               float space = (width - items.Count*cellWidth) / (float)(items.Count + 1);
               var tmp = mGirdItem.spacing;
               tmp.x = space;
               mGirdItem.spacing = tmp;
               */
           }

           Dictionary<uint, int>.Enumerator enumerator = items.GetEnumerator();

           if (items.Count <= 4)
           {
               awardItemWidth = 85;
           }
           else if (items.Count > 4)
           {
               awardItemWidth = 72;
           }

           //挂载首充奖励特殊物品
           if (firstPayComItem != null)
           {
                //这里首冲里会多一个特殊物品这里特殊处理自适应问题
                if (items.Count > 3)
                {
                    awardItemWidth = 72;
                    firstPayComItem.labLevel.fontSize = 25;
                    firstPayComItem.labStrengthenLevel.fontSize = 25;
                }
                PayAwardItem payAwardItem = new PayAwardItem(firstPayComItem, mGirdItem.gameObject);
                firstPayComItem.ItemData.Count = 1;
                if (payAwardItem != null)
                {
                    var itemEle = payAwardItem.GetCurrAwardItemEle();
                    if (itemEle != null)
                        itemEle.preferredWidth = itemEle.preferredHeight = awardItemWidth;
                }
           }

           while (enumerator.MoveNext())
           {
               uint itemID = enumerator.Current.Key;
               int itemNum = enumerator.Current.Value;

               var comItem = Utility.AddItemIcon(THIS, mGirdItem.gameObject, itemID, itemNum);
               if (comItem != null)
               {
                    PayAwardItem payAwardItem = new PayAwardItem(comItem, mGirdItem.gameObject);
                    comItem.ItemData.Count = 1;
                    if (items.Count > 4)
                    {
                        comItem.labLevel.fontSize = 25;
                        comItem.labStrengthenLevel.fontSize = 25;
                    }
                    if (payAwardItem != null)
                    {
                       var itemEle = payAwardItem.GetCurrAwardItemEle();
                       if (itemEle != null)
                           itemEle.preferredWidth = itemEle.preferredHeight = awardItemWidth;
                    }
               }
           }

       }

       public void SetStat()
       {
           bool canGet = PayManager.GetInstance().CanGetRewards(data.ID);
            mBtnGet.interactable = canGet;
           var comGray = mBtnGet.gameObject.GetComponent<UIGray>();
           if (comGray == null)
               comGray = mBtnGet.gameObject.AddComponent<UIGray>();
           comGray.enabled = !canGet;

           //var btnImg = mBtnGet.gameObject.GetComponent<Image>();
           //if (btnImg)
           //{
           //    btnImg.color = canGet ? Color.white : Color.gray;
           //    mBtnGet.interactable = canGet;
           //}

           int curPay = 0;
           int nextPay = 0;

           //mTxtStaus.text = "未达成";
           if (data.akActivityValues.Count == 2)
           {
               curPay = Convert.ToInt32(data.akActivityValues[0].value);
               nextPay = Convert.ToInt32(data.akActivityValues[1].value);

               if (data.status > PayManager.STATUS_TASK_UNFINISH)
               {
                   curPay = curPay + nextPay;
                   nextPay = 0;
               }

               mTxtMoney.text = (curPay + nextPay).ToString();/*data.activeItem.Desc*/;
               if ((curPay + nextPay) <= 6)
                   isFirstPay = true;
           }
           else
           {
               if (data.status <= PayManager.STATUS_TASK_UNFINISH)
               {
                   curPay = 0;
                   nextPay = 6;
               }
               else
               {
                   curPay = 6;
                   nextPay = 0;
               }
               mTxtMoney.text = "6";
               isFirstPay = true;
           }

           if (data.status >= PayManager.STATUS_TASK_OVER)
           {
               //mTxtStaus.text = "已领取";
               mTxtStaus.gameObject.CustomActive(true);
               mBtnGet.gameObject.CustomActive(false);
           }

           mTxtProgress.text = string.Format("{0}/{1}", curPay, curPay + nextPay);
           mSlider.value = curPay / (float)(curPay + nextPay);
       }

        
       //public void ResetAwardItemSizeByHeight(int awardItemCount,GameObject awardItemContainer)
       //{
       //    if(awardItemContainer==null)
       //        return;
       //    float awardContainerHeight = 0f;
       //    var awardItemContainerRect = awardItemContainer.GetComponent<RectTransform>();
       //    var awardItemContainerVGruop = awardItemContainer.GetComponent<VerticalLayoutGroup>();
       //    if (awardItemContainerRect != null && awardItemContainerVGruop !=null)
       //    {
       //        float value = awardItemContainerRect.rect.width - awardItemContainerVGruop.spacing * awardItemCount;
       //        if (value > 0)
       //            awardContainerHeight = value / awardItemCount;
       //    }
       //    awardItemWidth = (int)awardContainerHeight;
       //}
    }

    /// <summary>
    /// 201807改版 - 充值礼包领取界面
    /// </summary>
    public class PayConsumeItem5
    {
        protected const string EffUI_shouchong_guang_Path = "Effects/UI/Prefab/EffUI_shouchong/Prefab/EffUI_shouchong_guang";
        protected const string EffUI_chongzhifanli_Path = "Effects/UI/Prefab/EffUI_shouchong/Prefab/EffUI_chongzhifanli";

        public const string RES_CONUSME_ITEM5_PATH = "UIFlatten/Prefabs/Vip/PayConsumeItem5";

        private ComArtLettering mTxtMoney = null;
        private PayRewardItem mSpecialItem = null;
        private GameObject mGirdItem = null;
        private Slider mSlider = null;
        private Text mTxtProgress = null;
        private ComUIListScript mComScrollView = null;
        private Button mBtnGet = null;
        private UIGray mBtnGetGray = null;
        private Text mBtnGetText = null;
        private GameObject mEffectRoot_Backlight = null;
        private GameObject mEffectRoot_Envior = null;

        GameObject effect_guang_go = null;
        GameObject effect_fanli_go = null;

        GameObject root;
        GameObject parent;

        ActiveManager.ActivityData data;
        ClientFrame THIS;

        //Data
        List<AwardItemData> itemDataList = new List<AwardItemData>();
        //UI Component
        List<PayRewardItem> payRewardItems = new List<PayRewardItem>();

        //是否是展示到首充返利
        bool bFirstPayReturn = false;

        string mToGetRewardText = "";
        string mNotGetRewardText = "";
        string mGotRewardText = "";

        public PayConsumeItem5(ActiveManager.ActivityData data, GameObject parent, ClientFrame THIS)
        {
            Clear();

            this.data = data;
            this.parent = parent;
            this.THIS = THIS;

            if (root == null)
            {
                root = AssetLoader.instance.LoadResAsGameObject(RES_CONUSME_ITEM5_PATH);
                if (root != null)
                {
                    var mBind = root.GetComponent<ComCommonBind>();
                    mTxtMoney = mBind.GetCom<ComArtLettering>("txtMoney");
                    mSpecialItem = mBind.GetCom<PayRewardItem>("specialItem");
                    mGirdItem = mBind.GetGameObject("girdItem");
                    mSlider = mBind.GetCom<Slider>("slider");
                    mTxtProgress = mBind.GetCom<Text>("txtProgress");
                    mComScrollView = mBind.GetCom<ComUIListScript>("comScrollView");
                    mBtnGet = mBind.GetCom<Button>("btnGet");
                    if (null != mBtnGet)
                    {
                        mBtnGet.onClick.AddListener(_onBtnGetButtonClick);
                    }
                    mBtnGetGray = mBind.GetCom<UIGray>("btnGetGray");
                    mBtnGetText = mBind.GetCom<Text>("btnGetText");
                    mEffectRoot_Backlight = mBind.GetGameObject("EffectRoot_Backlight");
                    mEffectRoot_Envior = mBind.GetGameObject("EffectRoot_Envior");

                    if (effect_guang_go == null)
                    {
                        effect_guang_go = AssetLoader.GetInstance().LoadResAsGameObject(EffUI_shouchong_guang_Path);
                        Utility.AttachTo(effect_guang_go, mEffectRoot_Backlight);
                    }
                    if (effect_fanli_go == null)
                    {
                        effect_fanli_go = AssetLoader.GetInstance().LoadResAsGameObject(EffUI_chongzhifanli_Path);
                        Utility.AttachTo(effect_fanli_go, mEffectRoot_Envior);
                    }

                    mBtnGet.gameObject.CustomActive(true);
                    Utility.AttachTo(root, parent);
                }
            }
            InitTRDesc();
            RefreshView();
        }

        public void Clear()
        {
            //注意释放
            ClearAllPayRewardItems();

            mTxtMoney = null;
            mSlider = null;
            mTxtProgress = null;
            mComScrollView = null;
            mSpecialItem = null;
            mGirdItem = null;
            if (null != mBtnGet)
            {
                mBtnGet.onClick.RemoveListener(_onBtnGetButtonClick);
            }
            mBtnGet = null;
            mBtnGetText = null;
            mBtnGetGray = null;
            mEffectRoot_Backlight = null;
            mEffectRoot_Envior = null;

            root = null;
            parent = null;
            data = null;
            THIS = null;

            if (null != itemDataList)
            {
                itemDataList.Clear();
            }

            mToGetRewardText = "";
            mNotGetRewardText = "";
            mGotRewardText = "";

            bFirstPayReturn = false;

            if (effect_guang_go)
            {
                GameObject.Destroy(effect_guang_go);
                effect_guang_go = null;
            }
        }

        public void RefreshView(ActiveManager.ActivityData newData = null)
        {
            if (newData != null)
            {
                this.data = newData;
            }

            //先刷新其他数据，再刷新主要内容
            SetViewStatus();

            RefreshAwardData();

            SetMainView();
        }

        /// <summary>
        /// 不包括首充武器
        /// </summary>
        void RefreshAwardData()
        {
            if (data == null)
            {
                return;
            }
            var items = PayManager.GetInstance().GetAwardItems(data);
            if (items == null)
            {
                Logger.LogError("[Pay Consume Item Set Data] - Get award items is null");
                return;
            }

            if (itemDataList != null)
            {
                itemDataList.Clear();
            }

            if (bFirstPayReturn)
            {
                // PayManager.GetInstance().weaponStrengthLevel
                AwardItemData fAward = new AwardItemData() { ID = (int)PayManager.GetInstance().weaponItemID, Num = 1 };
                if (itemDataList != null && !itemDataList.Contains(fAward))
                {
                    itemDataList.Add(fAward);
                }
            }

            Dictionary<uint, int>.Enumerator enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                uint itemID = enumerator.Current.Key;
                int itemNum = enumerator.Current.Value;

                if (itemDataList != null)
                {
                    itemDataList.Add(new AwardItemData() { ID = (int)itemID, Num = itemNum });
                }
            }
        }

        void SetMainView()
        {
            if (itemDataList == null)
            {
                return;
            }
            if (mComScrollView == null)
            {
                return;
            }
            if (mComScrollView.IsInitialised() == false)
            {
                mComScrollView.Initialize();
                mComScrollView.onBindItem = (GameObject go) =>
                {
                    PayRewardItem payItem = null;
                    if (go)
                    {
                        payItem = go.GetComponent<PayRewardItem>();
                    }
                    return payItem;
                };
            }
            mComScrollView.onItemVisiable = (var) =>
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

                    if (bFirstPayReturn && itemDetailData.TableID == PayManager.GetInstance().weaponItemID)
                    {
                        itemDetailData.StrengthenLevel = PayManager.GetInstance().weaponStrengthLevel;
                    }

                    PayRewardItem payItem = var.gameObjectBindScript as PayRewardItem;
                    if (payItem != null)
                    {
                        payItem.Initialize(THIS, itemDetailData);
                        payItem.RefreshView();
                        if (payRewardItems != null && !payRewardItems.Contains(payItem))
                        {
                            payRewardItems.Add(payItem);
                        }
                    }
                }
            };
            mComScrollView.SetElementAmount(itemDataList.Count);
            mComScrollView.ResetContentPosition();

            if (mSpecialItem)
            {
                int specialItemDataId = PayManager.GetInstance().GetPayReturnSpecialResID(data.ID, itemDataList);
                ItemData detailData = ItemDataManager.CreateItemDataFromTable(specialItemDataId);
                if (detailData == null)
                {
                    Logger.LogErrorFormat("Can find !!! Please Check item data id {0} !!!", specialItemDataId);
                    return;
                }
                mSpecialItem.Initialize(THIS, detailData, false);
                mSpecialItem.RefreshView(true, false);
                string specialItemIconPath = PayManager.GetInstance().GetPayReturnSpecialResPath(data.ID, itemDataList);
                mSpecialItem.SetItemIcon(specialItemIconPath);
                mSpecialItem.onPayItemClick = () => {

                    if (detailData.TableID == PayManager.GetInstance().weaponItemID)
                    {
                        detailData.StrengthenLevel = PayManager.GetInstance().weaponStrengthLevel;
                    }
                    ItemTipManager.GetInstance().ShowTip(detailData);
                };
            }
        }

        void SetViewStatus()
        {
            //bool canGet = PayManager.GetInstance().CanGetRewards(data.ID);

            //if (mBtnGet != null)
            //{
            //    mBtnGet.interactable = canGet;
            //    var comGray = mBtnGet.gameObject.GetComponent<UIGray>();
            //    if (comGray == null)
            //        comGray = mBtnGet.gameObject.AddComponent<UIGray>();
            //    comGray.enabled = !canGet;
            //}

            int curPay = 0;
            int nextPay = 0;

            if (data.akActivityValues.Count == 2)
            {
                curPay = Convert.ToInt32(data.akActivityValues[0].value);
                nextPay = Convert.ToInt32(data.akActivityValues[1].value);

                //当前额度支付已完成
                if (data.status > PayManager.STATUS_TASK_UNFINISH)
                {
                    curPay = curPay + nextPay;
                    nextPay = 0;
                }

                mTxtMoney.SetNum(curPay + nextPay);/*data.activeItem.Desc*/;

                //!!!!!!!  如果新加充值档位在6元以下，则注意 如果此时活动会有两个值返回 则要考虑两个值相加的值小于6元  算首充吗 !!!!!!!!!!
                if ((curPay + nextPay) <= PayManager.FIRST_PAT_RMB_NUM)
                {
                    bFirstPayReturn = true;
                }
                else
                {
                    bFirstPayReturn = false;
                }
            }
            else
            {
                //以下为首充刷新 首充默认 6元
                if (data.status <= PayManager.STATUS_TASK_UNFINISH)
                {
                    curPay = 0;
                    nextPay = PayManager.FIRST_PAT_RMB_NUM;
                }
                else
                {
                    curPay = PayManager.FIRST_PAT_RMB_NUM;
                    nextPay = 0;
                }
                mTxtMoney.SetNum(PayManager.FIRST_PAT_RMB_NUM);//TR.Value("vip_month_card_first_buy_next_pay_num");  //"6"
                bFirstPayReturn = true;
            }

            if (data.status == PayManager.STATUS_TASK_CANGET)
            {
                SetGetBtnStatus(true);
                SetGetBtnText(mToGetRewardText);
            }
            else if (data.status < PayManager.STATUS_TASK_CANGET)
            {
                SetGetBtnStatus(false);
                SetGetBtnText(mNotGetRewardText);
            }
            else if (data.status > PayManager.STATUS_TASK_CANGET)
            {
                SetGetBtnStatus(false);
                SetGetBtnText(mGotRewardText);
            }

            if (mTxtProgress)
            {
                mTxtProgress.text = string.Format("{0}/{1}", curPay, curPay + nextPay);
            }
            if (mSlider)
            {
                mSlider.value = curPay / (float)(curPay + nextPay);
            }
        }

        void _onBtnGetButtonClick()
        {
            PayManager.GetInstance().GetRewards(data.ID);
            ClientSystemManager.GetInstance().delayCaller.DelayCall(300, () =>
            {
                SetViewStatus();
                var frame_mall = ClientSystemManager.instance.GetFrame(typeof(MallPayFrame)) as MallPayFrame;
                var frame_vip = ClientSystemManager.instance.GetFrame(typeof(VipFrame)) as VipFrame;
                if (frame_mall != null && frame_vip != null)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdatePayData);
                }
            });
        }

        void SetGetBtnStatus(bool active)
        {
            if (mBtnGetGray)
            {
                mBtnGetGray.enabled = !active;
            }
            if (mBtnGet)
            {
                mBtnGet.interactable = active;
            }
        }

        void SetGetBtnText(string desc)
        {
            if (mBtnGetText)
            {
                mBtnGetText.text = desc;
            }
        }

        void InitTRDesc()
        {
            mToGetRewardText = TR.Value("vip_month_card_first_buy_next_pay_return_toget");
            mNotGetRewardText = TR.Value("vip_month_card_first_buy_next_pay_return_notget");
            mGotRewardText = TR.Value("vip_month_card_first_buy_next_pay_return_got");
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
    }

    public class PayAwardItem
    {
        public const string res_awardItem = "UIFlatten/Prefabs/Vip/PayAwardItem";
        GameObject root;
        GameObject parent;
        ComItem comItem;

        private GameObject awardItemGo;
        private LayoutElement awardEle;
        private Text awardName;

        public PayAwardItem(ComItem comItem, GameObject parent)
        {
            this.parent = parent;
            this.comItem = comItem;

            this.root = AssetLoader.instance.LoadResAsGameObject(res_awardItem);
            if (root)
            {
                Utility.AttachTo(root,parent);
                
                awardItemGo = Utility.FindGameObject(root, "Image", false);
                awardEle = awardItemGo.GetComponent<LayoutElement>();
                awardName = Utility.FindComponent<Text>(root, "Text",false);
            }

            SetData();
        }

        public void SetData()
        {
            if (comItem == null)
                return;
            if (awardItemGo)
            {
                Utility.AttachTo(comItem.gameObject, awardItemGo);
            }
            if (awardName)
            {
                if (comItem.ItemData != null)
                {
                    string showName = comItem.ItemData.Name +" x"+comItem.ItemData.Count;
                    showName = showName.Replace("（","(");
                    showName = showName.Replace("）",")");
                    awardName.text = showName;
                    
                    var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(comItem.ItemData.TableID);
                    SetNameColorByQuality(itemTable, awardName);
                }
            }
        }

        void SetNameColorByQuality(ProtoTable.ItemTable item,Text text)
        {
            if (text == null)
                return;
            string colorStr = "white";
            if (item != null)
            {
                colorStr = Parser.ItemParser.GetItemColor(item);
            }
            string textStr = text.text;
            text.text = string.Format("<color={0}>", colorStr) + textStr + "</color>";
        }

        public LayoutElement GetCurrAwardItemEle()
        {
            return awardEle;
        }
    }

	public class PayConsumeFrame : ClientFrame {

		[UIControl("content/ScrollView")]
        ComSelectScorllRect scroll;

		[UIObject("content/ScrollView/Viewport/Content")]
		GameObject scrollContent;

        [UIObject("content/ScrollView/ScrollbarH")]
        GameObject scrollBar;

		[UIControl("nextPay/curPay")]
		Text txtCurPay;

		[UIControl("nextPay/nextPay")]
		Text txtNextPay;

		[UIObject("nextPay")]
		GameObject objNextPay;

        [UIObject("content/selectLights")]
        GameObject selectScrollGo;

/*		[UIObject("finishPay")]
		GameObject objFinishPay;*/

		List<PayConsumeItem3> consumeItems = new List<PayConsumeItem3>();

		public override string GetPrefabPath()
		{
			return "UIFlatten/Prefabs/Vip/PayConsumeFrame";
		}

		[UIEventHandle("btnClose")]
		void OnClickClose()
		{
			ClientSystemManager.instance.CloseFrame<PayConsumeFrame>();
			consumeItems.Clear();
		}

		[UIEventHandle("btnGoPay")]
		void OnClickPay()
		{
			OnClickClose();

			var frame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
			frame.OpenPayTab();
		}

		protected override void _OnOpenFrame()
		{
			bool flag = false;
			int index = -1;
			string curPay = "", nextPay = "";
		
			var items = PayManager.GetInstance().GetConsumeItems();
			//PayManager.GetInstance().get

			if (items != null)
			{
				for(int i=0; i<items.Count; ++i)
				{
					var consumeItem = new PayConsumeItem3(items[i], scrollContent, this);
					consumeItems.Add(consumeItem);

					if (!flag && (items[i].status == PayManager.STATUS_TASK_UNFINISH))
					{
						flag = true;
						curPay = items[i].akActivityValues[0].value;
						nextPay = items[i].akActivityValues[1].value;
						index = i;
					}
				}
					
				for(int i=0; i<items.Count; ++i)
				{
					if (items[i].status == PayManager.STATUS_TASK_CANGET)
					{
						index = i;
						break;
					}
				}
			}

			if (!flag)
			{
				objNextPay.CustomActive(false);
				//objFinishPay.CustomActive(true);
			}
			else {
				objNextPay.CustomActive(true);
				//objFinishPay.CustomActive(false);

				txtCurPay.text = curPay;
				txtNextPay.text = nextPay;	
			}

            if (scroll)
            {
                scroll.SelectedGameObject = selectScrollGo;
                scroll.SelectedGameObject.CustomActive(false);
            }

            ClientSystemManager.GetInstance().delayCaller.DelayCall(250, () =>
            {
                SetScrollPositon(scroll, index, items.Count, 0.18f);
            });

            //if (scrollBar)
            //{
            //    if (scrollBar.GetComponent<Scrollbar>() != null)
            //    {
            //        if (index > 0 && items.Count > 0)
            //        {
            //            scrollBar.GetComponent<Scrollbar>().value = ((float)index / (float)items.Count);
            //        }
            //        else
            //        {
            //            scrollBar.GetComponent<Scrollbar>().value = 0;
            //        }
            //    }
            //}
		}

		public static void SetScrollPositon(ScrollRect scroll, int index, int count, float interval=0.15f)
		{
            scroll.horizontalNormalizedPosition = Mathf.Max(0, interval * index); 

			//scroll.verticalNormalizedPosition = Mathf.Max(0, 1.0f - interval * index); 
			/*
			if (index < 0)
				scroll.verticalNormalizedPosition = 0f;
			else if (index <= 1)
				scroll.verticalNormalizedPosition = 1f;
			else if (index == count-1 || index == count-2)
				scroll.verticalNormalizedPosition = 0f;
			else 
			{
				//float interval = 0.2f;
				scroll.verticalNormalizedPosition = Mathf.Max(0, 1.0f - interval * (index-1)); 
			}*/
		}
    }

}

