using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EItemType = ProtoTable.ItemTable.eType;
using Network;
using Protocol;
using UnityEngine.EventSystems;
using ProtoTable;
using UnityEngine.Assertions;
using DG.Tweening;
using EItemBindAttr = ProtoTable.ItemTable.eOwner;
using EItemQuality = ProtoTable.ItemTable.eColor;
using Scripts.UI;
using UnityEngine.Events;

namespace GameClient
{
    enum EPackageOpenMode
    {
        Equip = 0,
        EquipWithDecompose = 1,
        Material = 2,
        Consumables = 3,
        Fashion = 4,
        Title = 5,
        EquipWithQuickSell = 6,
        Pet = 7,
        FashionWithDecompose = 8,
        Inscription = 9,
        Bxy = 10,
        Sinan = 11,
    }

    struct ComItemConfig
    {
        public string root;
    };

    class PackageNewFrame : ClientFrame
    {
		#region ExtraUIBind
		//private GameObject mPackageSwitchWeapon = null;
		private GameObject mPackagButtom = null;
		private GameObject mPackageItemListTabs = null;
		private GameObject mPackageActorShowRoot = null;
		private GameObject mPackageItemListView = null;
        private Button mPackageFulltips = null;
        private Text mPackageFulText = null;
        private GameObject mPetRoot = null;
        private GameObject fashionEquipDecomposeRoot = null;
        private ImageEx mWindBg = null;
        private HoleImage mOperateMask = null;


        protected override void _bindExUI()
        {
            //mPackageSwitchWeapon = mBind.GetGameObject("packageSwitchWeapon");
            mPackagButtom = mBind.GetGameObject("packagButtom");
            mPackageItemListTabs = mBind.GetGameObject("packageItemListTabs");
            mPackageActorShowRoot = mBind.GetGameObject("packageActorShow");
            mPackageItemListView = mBind.GetGameObject("packageItemListView");
            mPackageFulltips = mBind.GetCom<Button>("packageFulltips");
            mPackageFulltips.onClick.AddListener(_onPackageFulltipsButtonClick);
            mPackageFulText = mBind.GetCom<Text>("packageFulText");
            mPetRoot = mBind.GetGameObject("petRoot");
//             switchWeaponBtn = mBind.GetCom<Button>("SwitchWeaponBtn");
//             switchWeaponBtn.SafeAddOnClickListener(OnSwitchWeaponFrame);
		    m_scrollRect = mBind.GetCom<ScrollRect>("ItemScrollRect");
		    m_comItemList = mBind.GetCom<ComUIMultListScript>("ItemListView");
		    m_objShop = mBind.GetCom<Button>("Shop");
		    m_objShop.SafeAddOnClickListener(_OnShopClicked);
            m_objFashionMerge = mBind.GetCom<Button>("FashionMerge");
            m_objFashionMerge.SafeAddOnClickListener(_OnFashionMergeClicked);
            fashionEquipDecomposeRoot = mBind.GetGameObject("fashionEquipDecomposeRoot");
            mWindBg = mBind.GetCom<ImageEx>("WindBg");
            mOperateMask = mBind.GetCom<HoleImage>("ImageOperateMask");
        }

        protected override void _unbindExUI()
		{
			//mPackageSwitchWeapon = null;
			mPackagButtom = null;
			mPackageItemListTabs = null;
			mPackageActorShowRoot = null;
			mPackageItemListView = null;
            mPackageFulltips.onClick.RemoveListener(_onPackageFulltipsButtonClick);
            mPackageFulltips = null;
            mPackageFulText = null;
            mPetRoot = null;
		    m_scrollRect = null;
		    m_comItemList = null;
		    m_objShop.SafeRemoveOnClickListener(_OnShopClicked);
		    m_objShop = null;
		    m_objFashionMerge.SafeRemoveOnClickListener(_OnFashionMergeClicked);
		    m_objFashionMerge = null;
            fashionEquipDecomposeRoot = null;
            mWindBg = null;
            mOperateMask = null;
        }
        #region Callback
        private void _onPackageFulltipsButtonClick()
        {
            _SetPackageFullTipActive(false);
        }

        /*private void _onSkillDamageButtonClick()
        {
            SkillDamageFrame frame = ClientSystemManager.instance.OpenFrame<SkillDamageFrame>(FrameLayer.Middle) as SkillDamageFrame;
            if (frame != null)
            {
                frame.InitData(false);
            }
        }*/
		
        #endregion
        #endregion
        //[UIControl("Content/Tabs/Viewport/Content/Tab")]
        Toggle m_togEquipGroup;

        //[UIControl("Content/Tabs/Viewport/Content/Tab (1)")]
        Toggle m_togFashionGroup;

        //[UIControl("Content/Tabs/Viewport/Content/Tab (2)")]
        Toggle m_togTitleGroup;

        //[UIControl("Content/Tabs/Viewport/Content/BxyTab")]
        Toggle m_togBxyGroup;

        //[UIControl("Content/Tabs/Viewport/Content/PetTab")]
        Toggle m_togPetGroup;

        //[UIObject("Content/Tabs/Viewport/Content/PetTab")]
        GameObject m_objPetGroupRoot;

        GameObject m_goFashionRePoint;
        ScrollRect m_scrollRect;
        //ComUIListScriptEx m_comItemList;
        ComUIMultListScript m_comItemList;
        Text m_gridCount;
        ComButtonEnbale m_comBtnChapterPotionSet;
        ComButtonEnbale m_comBtnQuickDecompose;
        ComButtonEnbale m_comBtnQuickSell;
        Button btnFashionEquipDecompose = null;
        private Button mButtonArrage;
        Button m_objShop;
        Button m_objFashionMerge;
        Button m_objInscriptionMerge;
        Image mHonorImg;
        GameObject mTitleRoot;//头衔父节点
        PackageActorShow mActorShow;
        PackageAttrDetailView mDetailView;

        //[UIEventHandle("Content/Bottom/Ctrl/ChapterPotionSet")]
        void _OnOpenChapterPotionSetClicked()
        {
            ClientSystemManager.instance.OpenFrame<ChapterBattlePotionSetFrame>();
        }

        //[UIEventHandle("Content/Bottom/Ctrl/QuickDecompose")]
        void _OnOpenQuickDecomposeClicked()
        {
            _OpenQuickDecompose();
        }

        //[UIEventHandle("Content/Bottom/Ctrl/QuickSell")]
        void _OnOpenQuickSellClicked()
        {
            _OpenQuickSell();
        }

        //[UIEventHandle("Content/Bottom/Ctrl/Arrange")]
        void _OnArrangePackage()
        {
            if (m_eShowMode != EItemsShowMode.Normal)
            {
                return;
            }

            SceneTrimItem msg = new SceneTrimItem()
            {
                pack = (byte)m_currentItemType
            };
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneTrimItemRet>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    ProtoTable.CommonTipsDesc tableData = TableManager.GetInstance().GetTableItem<ProtoTable.CommonTipsDesc>((int)msgRet.code);
                    if (tableData != null)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.code);
                    }
                    else
                    {
                        SystemNotifyManager.SysNotifyMsgBoxOK(Utility.ProtocolErrorString(msgRet.code));
                    }
                }
                else
                {
                    ItemDataManager.GetInstance().ArrangeItemsInPackageFrame(m_currentItemType);
                    _RefreshItemTab();
                    _RefreshItemList();
                }
            });
        }

        void OnOpenActorAttTipShow()
        {
        }

//         private Button mButtonClose;
//         protected void _OnClose()
//         {
//             frameMgr.CloseFrame(this);
// 
//             ClientSystemManager.instance.CloseFrame<PetPacketFrame>();
//         }

        //[UIEventHandle("Content/Shop")]
        void _OnShopClicked()
        {
            var paramData = new MallNewFrameParamData
            {
                MallNewType = MallNewType.FashionMall,
            };

            frameMgr.OpenFrame<MallNewFrame>(FrameLayer.Middle, paramData);
        }

        //[UIEventHandle("Content/FashionMerge")]
        void _OnFashionMergeClicked()
        {
            FashionMergeNewFrame.OpenLinkFrame(string.Format("1|0|{0}|{1}|{2}", (int)FashionMergeManager.GetInstance().FashionType, (int)FashionMergeManager.GetInstance().FashionPart, 0));
        }

        void _OnInscriptionMergeClicked()
        {
            SmithShopNewLinkData data = new SmithShopNewLinkData();
            data.iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_INSCRIPTION;
            data.iDefaultSecondTabId = (int)InscriptionTabType.InscriptionSynthesis;

            ClientSystemManager.GetInstance().OpenFrame<SmithShopNewFrame>(FrameLayer.Middle, data);
        }

        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                int data = int.Parse(strParam);
                EPackageOpenMode mode = (EPackageOpenMode)data;
                ClientSystemManager.GetInstance().OpenFrame<PackageNewFrame>(FrameLayer.Middle, mode);
            }
            catch (System.Exception e)
            {
                Logger.LogError("PackageFrame.OpenLinkFrame : ==>" + e.ToString());
            }
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Package/PackageNew";
        }

        public sealed override bool IsNeedUpdate()
        {
            return true;
        }

        protected sealed override void _OnUpdate(float timeElapsed)
        {
            mActorShow?.OnUpdate(timeElapsed);
        }

        void OnAddNewItem(List<Item> items)
        {
        }
        void OnUpdateItem(List<Item> items)
        {
            _OnOnlyUpdateItemList(null);
            mActorShow?.RefreshBaseInfo();
        }
        void OnRemoveItem(ItemData data)
        {
        }
        protected sealed override void _OnOpenFrame()
        {
            m_bInited = true;
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;

            StartCoroutine(_InitUIDelay());
        }

        private void _switch2Group(EPackageOpenMode mode)
        {
            switch (mode)
            {
                case EPackageOpenMode.Equip:
                    {
                        m_togEquipGroup.isOn = true;
                        _GetItemTabInfo(EPackageType.Equip).toggle.isOn = true;
                        break;
                    }
                case EPackageOpenMode.EquipWithDecompose:
                    {
                        m_togEquipGroup.isOn = true;
                        _GetItemTabInfo(EPackageType.Equip).toggle.isOn = true;
                        _OpenQuickDecompose();
                        break;
                    }
                case EPackageOpenMode.EquipWithQuickSell:
                    {
                        m_togEquipGroup.isOn = true;
                        _GetItemTabInfo(EPackageType.Equip).toggle.isOn = true;
                        _OpenQuickSell();
                        break;
                    }
                case EPackageOpenMode.Material:
                    {
                        m_togEquipGroup.isOn = true;
                        _GetItemTabInfo(EPackageType.Material).toggle.isOn = true;
                        break;
                    }
                case EPackageOpenMode.Consumables:
                    {
                        m_togEquipGroup.isOn = true;
                        _GetItemTabInfo(EPackageType.Consumable).toggle.isOn = true;
                        break;
                    }
                case EPackageOpenMode.Fashion:
                    {
                        m_togFashionGroup.isOn = true;
                        _GetItemTabInfo(EPackageType.Fashion).toggle.isOn = true;
                        break;
                    }
                case EPackageOpenMode.Inscription:
                    {
                        m_togFashionGroup.isOn = true;
                        _GetItemTabInfo(EPackageType.Inscription).toggle.isOn = true;
                        break;
                    }
                case EPackageOpenMode.FashionWithDecompose:
                    {
                        m_togEquipGroup.isOn = true;
                        _GetItemTabInfo(EPackageType.Fashion).toggle.isOn = true;
                        _OpenFashionDecompose();
                        break;
                    }
                case EPackageOpenMode.Title:
                    {
                        m_togTitleGroup.isOn = true;
                        _GetItemTabInfo(EPackageType.Title).toggle.isOn = true;
                        break;
                    }
                case EPackageOpenMode.Bxy:
                    {
                        m_togBxyGroup.isOn = true;
                        _GetItemTabInfo(EPackageType.Bxy).toggle.isOn = true;
                        break;
                    }
                case EPackageOpenMode.Sinan:
                    {
                        m_togBxyGroup.isOn = true;
                        _GetItemTabInfo(EPackageType.Sinan).toggle.isOn = true;
                        break;
                    }    
                case EPackageOpenMode.Pet:
                    {
                        m_togPetGroup.isOn = true;
                    }
                    break;
            }
        }

        private void _OnPackageSwitch2OneGroup(UIEvent ui)
        {
            try
            {
                EPackageOpenMode mode = (EPackageOpenMode)ui.Param1;
                _switch2Group(mode);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("[PackageNewFrame] _OnPackageSwitch2OneGroup {0} not in the EPackageOpenMode, with error {1}", ui.Param1, e.ToString());
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
            ClientSystemManager.instance.CloseFrame<PetPacketFrame>();
            if (mActorShow != null)
            {
                mActorShow.Clear();
            }
            _ClearQuickSell();
            _ClearQuickDecompose();
            _ClearItemTab();
            _ClearItemList();

            _ClearActorShow();
            _ClearTabs();
            _ClearBottom();
            _UnRegisterUIEvent();
            m_bInited = false;

            if(Tempitem  != null)
            {
                Tempitem = null;
            }

            ClientSystemManager.GetInstance().CloseFrame<FashionEquipDecomposeFrame>();
            SkillDataManager.GetInstance().UpdateSkillLevelAddInfo();
        }

        IEnumerator _InitUIDelay()
        {
            _InitTabs();
            _InitItemTab();
            _InitActorShow();
	        _InitPetTab();

			//if(EngineConfig.usePackageAsyncLoad)
			//	yield return null;

			_InitItemList();

	        //if (EngineConfig.usePackageAsyncLoad)
			//	yield return null;

			_InitBottom();

	        //if (EngineConfig.usePackageAsyncLoad)
			//	yield return null;
			//_InitBgPanel();

	        //if (EngineConfig.usePackageAsyncLoad)
			//	yield return null;

            _InitMainTabs();
            _InitPackageFullTips();

            _RegisterUIEvent();

            EPackageOpenMode mode = EPackageOpenMode.Equip;
            if (userData != null)
            {
                mode = (EPackageOpenMode)userData;
            }

            _switch2Group(mode);
            yield return null;
        }

//         void _InitBgPanel()
//         {
//             var bgGo = frame.transform.Find(mBind.GetPrefabPath("BgRoot"));
//             if (bgGo != null)
//             {
//                 var bgWrapper = bgGo.GetComponent<UIPrefabWrapper>();
//                 if (bgWrapper != null)
//                 {
//                     var prefab = bgWrapper.LoadUIPrefab(bgGo);
//                     if (prefab != null)
//                     {
//                         var bind = prefab.gameObject.GetComponent<ComCommonBind>();
//                         if (bind != null)
//                         {
//                             mButtonClose = bind.GetCom<Button>("ButtonClose");
//                             mButtonClose.SafeAddOnClickListener(_OnClose);
//                         }
//                     }
//                 }
//             }
//         }

        void _InitActorShow()
        {
            var actorShowWrapper = mPackageActorShowRoot.GetComponent<UIPrefabWrapper>();
            if (actorShowWrapper != null)
            {
                var prefab = actorShowWrapper.LoadUIPrefab(mPackageActorShowRoot.transform);
                if (prefab != null)
                {
                    mPackageActorShowRoot = prefab;
                    mActorShow = mPackageActorShowRoot.GetComponent<PackageActorShow>();
                    if (mActorShow != null)
                    {
                        mActorShow.Init(this, _OnWearedItemClicked, OnAttrToggleValueChanged);
                    }
                }
            }
        }


        private void OnFashionTipToggleValueChanged(bool value)
        {
        }

        private void OnIsShowFashionWeapon(UIEvent uievent)
        {
            if (mActorShow != null)
            {
                mActorShow.RefreshFashionWeapon();
            }
        }

        private void OnTitleNameUpdate(UIEvent uievent)
        {
            if (mActorShow != null)
            {
                mActorShow.RefreshHead();
            }
        }

        private void OnPetPropertyReselect(UIEvent uiEvent)
        {
            _RefreshDetailAttrs();
        }

        private void OnCloseFashionEquipDecompose(UIEvent uiEvent)
        {
            _CloseFashionDecompose();
        }

        private void _onTitleButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<PersonalSettingFrame>(FrameLayer.Middle, 0);
        }

        void _ClearActorShow()
        {
            if (mActorShow != null)
            {
                mActorShow.Clear();
            }
        }

        void _InitTabs()
        {
            var GO = frame.transform.Find(mBind.GetPrefabPath("TabsPlaceHolder"));
            if (GO != null)
            {
                var wrapper = GO.GetComponent<UIPrefabWrapper>();
                if (wrapper != null)
                {
                    var prefab = wrapper.LoadUIPrefab(GO);
                    if (prefab != null)
                    {
                        var bind = prefab.gameObject.GetComponent<ComCommonBind>();
                        if (bind != null)
                        {
                            m_goFashionRePoint = bind.GetGameObject("FashionRedPoint");
                            m_togEquipGroup = bind.GetCom<Toggle>("EquipGroup");
                            m_togFashionGroup = bind.GetCom<Toggle>("FashionGroup");
                            m_togTitleGroup = bind.GetCom<Toggle>("TitleGroup");
                            m_togPetGroup = bind.GetCom<Toggle>("PetGroup");

                            m_togBxyGroup = bind.GetCom<Toggle>("BxyGroup");

                            m_objPetGroupRoot = m_togPetGroup.gameObject;
                        }
                    }
                }
            }
        }

        void _ClearTabs()
        {
            m_goFashionRePoint = null;
            m_togEquipGroup = null;
            m_togFashionGroup = null;
            m_togTitleGroup = null;

            m_togBxyGroup = null;

            m_togPetGroup = null;
            m_objPetGroupRoot = null;
        }

        void _InitBottom()
        {
            var GO = frame.transform.Find(mBind.GetPrefabPath("BottomPlaceHolder"));
            if (GO != null)
            {
                var wrapper = GO.GetComponent<UIPrefabWrapper>();
                if (wrapper != null)
                {
                    var prefab = wrapper.LoadUIPrefab(GO);
                    if (prefab != null)
                    {
                        var bind = prefab.gameObject.GetComponent<ComCommonBind>();
                        mPackagButtom = prefab;
                        if (bind != null)
                        {
                            m_gridCount = bind.GetCom<Text>("GridCount");
                            m_comBtnChapterPotionSet = bind.GetCom<ComButtonEnbale>("ChapterPotionSet");
                            m_comBtnQuickDecompose = bind.GetCom<ComButtonEnbale>("QuickDecompose");
                            m_comBtnQuickSell = bind.GetCom<ComButtonEnbale>("QuickSell");
                            mButtonArrage = bind.GetCom<Button>("Arrange");
                            mButtonArrage.SafeAddOnClickListener(_OnArrangePackage);
                            var buttonQuickSell = m_comBtnQuickSell.GetComponent<Button>();
                            buttonQuickSell.SafeAddOnClickListener(_OnOpenQuickSellClicked);
                            var buttonQuickDecompose = m_comBtnQuickDecompose.GetComponent<Button>();
                            buttonQuickDecompose.SafeAddOnClickListener(_OnOpenQuickDecomposeClicked);
                            var buttonChapterPotionSet = m_comBtnChapterPotionSet.GetComponent<Button>();
                            buttonChapterPotionSet.SafeAddOnClickListener(_OnOpenChapterPotionSetClicked);

                            btnFashionEquipDecompose = bind.GetCom<Button>("btnFashionEquipDecompose");
                            btnFashionEquipDecompose.SafeSetOnClickListener(() => 
                            {
                                _OpenFashionDecompose();
                            });

                            m_objFashionMerge = bind.GetCom<Button>("FashionMerge");
                            m_objFashionMerge.SafeAddOnClickListener(_OnFashionMergeClicked);

                            m_objInscriptionMerge = bind.GetCom<Button>("InscriptionMerge");
                            m_objInscriptionMerge.SafeAddOnClickListener(_OnInscriptionMergeClicked);
                        }
                    }
                }
            }
        }


        void _ClearBottom()
        {
            if (m_comBtnQuickSell != null)
            {
                var buttonQuickSell = m_comBtnQuickSell.GetComponent<Button>();
                buttonQuickSell.SafeRemoveOnClickListener(_OnOpenQuickSellClicked);
            }

            if (m_comBtnQuickDecompose != null)
            {
                var buttonQuickDecompose = m_comBtnQuickDecompose.GetComponent<Button>();
                buttonQuickDecompose.SafeRemoveOnClickListener(_OnOpenQuickDecomposeClicked);
            }

            if (m_comBtnChapterPotionSet != null)
            {
                var buttonChapterPotionSet = m_comBtnChapterPotionSet.GetComponent<Button>();
                buttonChapterPotionSet.SafeRemoveOnClickListener(_OnOpenChapterPotionSetClicked);
            }
            m_gridCount = null;
            m_comBtnChapterPotionSet = null;
            m_comBtnQuickDecompose = null;
            m_comBtnQuickSell = null;
            mButtonArrage.SafeRemoveOnClickListener(_OnArrangePackage);
            mButtonArrage = null;
        }

        public enum EItemsShowMode
        {
            Normal,
            Decompose,
            QuickSell,
            FashionDecompose,
        }

        #region main tabs
        bool m_bInited = false;

        public enum TabMode
        {
            TM_EQUIP = 0,
            TM_FASHION,
            TM_TITLE,
            TM_PET,
            TM_BXY,
        }

        private static bool[,] sTabPackageTypeFlag = new bool[,]
        {                       
                              /* PackageType */ 
                              /* Equip */ /* Material */ /* Consumable */ /* Fashion * / /* Title */ /* Inscription */ /* bxy */ /* sinan */ 
            /* Tab.Equip   */ {  true,         true,        true,            false,         false,      false,           false,     false},
            /* Tab.Fashion */ {  false,        false,       false,           true,          false,      true,            false,     false},
            /* Tab.Title   */ {  false,        false,       false,           false,         true,       false,           false,     false},
            /* Tab.Pet     */ {  false,        false,       false,           false,         false,      false,           false,     false},
            /* Tab.Bxy     */ {  false,        false,       false,           false,         false,      false,           true,      true}          
        };

        private void _setTabActive(TabMode tabmode, EPackageType packageType)
        {
            int tabIndex = (int)tabmode;
            int packageIndex = _getEPackageTypeIndex(packageType);
            int tabLen = sTabPackageTypeFlag.GetUpperBound(0);
            if (tabIndex < 0 || tabIndex > tabLen)
            {
                return;
            }

            tabLen = sTabPackageTypeFlag.GetUpperBound(1);
            if (packageIndex < 0 || packageIndex > tabLen)
            {
                return;
            }

            ItemTabInfo itemTabInfo = _GetItemTabInfo(packageType);
            if (itemTabInfo != null && itemTabInfo.toggle != null)
            {
                itemTabInfo.toggle.CustomActive(sTabPackageTypeFlag[tabIndex, packageIndex]);

                if (!sTabPackageTypeFlag[tabIndex, packageIndex])
                {
                    itemTabInfo.toggle.isOn = false;
                }
            }

//             _GetItemTabInfo(packageType).toggle.gameObject.CustomActive(sTabPackageTypeFlag[tabIndex, packageIndex]);
// 
//             if (!sTabPackageTypeFlag[tabIndex, packageIndex])
//             {
//                 _GetItemTabInfo(packageType).toggle.isOn = false;
//             }
        }

        private int _getEPackageTypeIndex(EPackageType type)
        {
            switch (type)
            {
                case EPackageType.Equip:
                    return 0;
                case EPackageType.Material:
                    return 1;
                case EPackageType.Consumable:
                    return 2;
                case EPackageType.Fashion:
                    return 3;
                case EPackageType.Title:
                    return 4;
                case EPackageType.Inscription:
                    return 5;
                case EPackageType.Bxy:
                    return 6;
                case EPackageType.Sinan:
                    return 7;    
                default:
                    break;
            }

            return -1;
        }

        void _ChangeMode(TabMode eTabMode)
        {
            /// pet
            bool bFlag = ChijiDataManager.GetInstance().CheckCurrentSystemIsClientSystemGameBattle();
            if (bFlag == false)
            {
                //mPackageSwitchWeapon.CustomActive(eTabMode != TabMode.TM_PET);
            }

            mPackageActorShowRoot.CustomActive(eTabMode != TabMode.TM_PET);
            mPackageItemListTabs.CustomActive(eTabMode != TabMode.TM_PET);
            mPackagButtom.CustomActive(eTabMode != TabMode.TM_PET);
            mPackageItemListView.CustomActive(eTabMode != TabMode.TM_PET);

            Image bg = mWindBg as Image;
            if (eTabMode == TabMode.TM_PET)
            {
                if (mActorShow != null)
                {
                    mActorShow.Clear();
                }
                ClientSystemManager.instance.OpenFrame<PetPacketFrame>(mPetRoot);
                
                ETCImageLoader.LoadSprite(ref bg, "UI/Image/NewBackground/Img_Bg_Beibao_Pet.png:Img_Bg_Beibao_Pet");
            }
            else
            {
                if (mActorShow != null)
                {
                    mActorShow.ShowAvatar();
                }
                ClientSystemManager.instance.CloseFrame<PetPacketFrame>();
                ETCImageLoader.LoadSprite(ref bg, "UI/Image/NewBackground/Img_Panel_Zhuangbei_Bg01.jpg:Img_Panel_Zhuangbei_Bg01");
            }

            /// origin
            //m_objFashionTipRoot.CustomActive(eTabMode == TabMode.TM_FASHION);
            m_objShop.CustomActive(eTabMode == TabMode.TM_FASHION);

            PackageDataManager.GetInstance().ResetSendFashionWeaponReqFlag();

            _setTabActive(eTabMode, EPackageType.Equip);
            _setTabActive(eTabMode, EPackageType.Material);
            _setTabActive(eTabMode, EPackageType.Consumable);
            _setTabActive(eTabMode, EPackageType.Fashion);
            _setTabActive(eTabMode, EPackageType.Title);
            _setTabActive(eTabMode, EPackageType.Inscription);
            _setTabActive(eTabMode, EPackageType.Bxy);
            _setTabActive(eTabMode, EPackageType.Sinan);
            if (mActorShow != null)
            {
                mActorShow.RefreshMode((int)eTabMode);
            }
            switch (eTabMode)
            {
                case TabMode.TM_EQUIP:
                    {
                        if (m_bInited)
                        {
                            // 切换时，默认选中装备
                            _GetItemTabInfo(EPackageType.Equip).toggle.isOn = true;
                        }
                    }
                    break;
                case TabMode.TM_FASHION:
                    {
                        if (m_bInited)
                        {
                            // 切换时，默认选中时装
                            _GetItemTabInfo(EPackageType.Fashion).toggle.isOn = true;
                        }
                    }
                    break;
                case TabMode.TM_TITLE:
                    {
                        if (m_bInited)
                        {
                            // 切换时，默认选中装备
                            _GetItemTabInfo(EPackageType.Title).toggle.isOn = true;
                        }
                    }
                    break;
                case TabMode.TM_BXY:
                    {
                        if (m_bInited)
                        {
                            // 切换时，默认选中装备
                            _GetItemTabInfo(EPackageType.Bxy).toggle.isOn = true;
                        }
                    }
                    break;
            }
        }

        void _InitMainTabs()
        {
            m_togEquipGroup.onValueChanged.AddListener(var =>
            {
                if(var)
                {
                    _ChangeMode(TabMode.TM_EQUIP);
                }
            });

            m_togFashionGroup.onValueChanged.AddListener(var =>
            {
                if (var)
                {
                    _ChangeMode(TabMode.TM_FASHION);
                }
            });

            m_togTitleGroup.onValueChanged.AddListener(var =>
            {
                if (var)
                {
                    _ChangeMode(TabMode.TM_TITLE);
                }
            });

            m_togBxyGroup.onValueChanged.AddListener(var =>
            {
                if (var)
                {
                    _ChangeMode(TabMode.TM_BXY);
                }
            });

            m_togPetGroup.onValueChanged.AddListener(var =>
            {
                if (var)
                {
                    _ChangeMode(TabMode.TM_PET);
                }
            });
        }
        void _InitPetTab()
        {
           if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Pet))
           {
                m_objPetGroupRoot.CustomActive(true);
           }
            else
            {
                m_objPetGroupRoot.CustomActive(false);
            }
        }
        #endregion

        #region player baseinfo
        private bool SwitchWeaponOpen()
        {
            return Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.SideWeapon);
        }
        #endregion

        #region player model

        void _OnAvatarChagned(UIEvent e)
        {
            if (mActorShow != null)
            {
                mActorShow.RefreshAvatar();
            }
        }

        #endregion

        #region player attr
        public static void SetPersonalInfo(DisplayAttribute attribute, GameObject objLeftRoot, GameObject objRightRoot)
        {
        }
        

        bool m_bDetailInfosUIInited = false;
        Text m_labPlayerNameDetail;
        Text m_labPlayerJobDetail;
        Text m_labPlayerLevelDetail;
        Text m_labVipLevelDetail;
        Image m_imgSeasonMainLVDetail;
        Image m_imgSeasonSubLVDetail;
        ComExpBar m_comExpBar;
        DOTweenAnimation m_detailAttrDotween;
        GameObject m_objAttrLeft;
        GameObject m_objAttrRight;
        Button m_OpenRoleAttrTipsBtn;
        Text m_WeaponAttackAttributeTypeText;
        Text m_EquipScoreTxt;
        Button m_ClosePersonal;

        void OnAttrToggleValueChanged(bool value)
        {
            if (m_bDetailInfosUIInited == false)
            {
                _LoadDetailInfoUI();
                _RefreshDetailAttrs();
            }

            mDetailView?.PlayForward();
            mActorShow?.SetAttrDetailToggleActive(false);
        }

        void _LoadDetailInfoUI()
        {
            if (m_bDetailInfosUIInited == false)
            {
                GameObject obj = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Package/PersonalInformation");
                mDetailView = obj.GetComponent<PackageAttrDetailView>();
                mDetailView.SetCloseCB(OnClickClosePersonal);
                obj.transform.SetParent(frame.transform, false);
                m_bDetailInfosUIInited = true;
            }
        }

        void OnClickClosePersonal()
        {
            mDetailView?.PlayBackwards();
            mActorShow?.SetAttrDetailToggleActive(true);
        }

        //面板属性
        void _RefreshDetailAttrs()
        {
            mDetailView?.Refresh();
        }

        #endregion

        #region quick sell

        void _ClearQuickSell()
        {
            _CloseQuickSell();
        }

        void _CloseQuickSell()
        {
            if (m_eShowMode == EItemsShowMode.QuickSell)
            {
                m_eShowMode = EItemsShowMode.Normal;
                _RefreshItemList();

                _ClearSelectState();
                mOperatePanel?.Hide();
                mOperateMask.enabled = false;
            }
        }

        void _QuickSellEquip(Action a_okCallback, ItemData item)
        {
            PackageOperateConfirmFrameParam param = new PackageOperateConfirmFrameParam();
            param.Mode = (int)EItemsShowMode.QuickSell;
            param.Equip = item;
            param.CallBack = a_okCallback;
            if (item != null)
            {
                param.IsPrecious = _IsPrecious(item);
            }
            ClientSystemManager.GetInstance().OpenFrame<PackageOperateConfirmFrame>(FrameLayer.Middle, param);
        }

        void _OnQuickSellClicked()
        {
            List<ItemData> selectItems = new List<ItemData>();
            List<ulong> guids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);
            for (int i = 0; i < guids.Count; ++i)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(guids[i]);
                if (itemData != null && itemData.IsSelected)
                {
                    selectItems.Add(itemData);
                }
            }

            if (SecurityLockDataManager.GetInstance().CheckSecurityLock(() => 
            {
                for (int i = 0; i < selectItems.Count; i++)
                {
                    ItemData itemData = selectItems[i];
                    if (itemData != null && itemData.Quality >= EItemQuality.PURPLE)
                    {
                        return true;
                    }
                }

                return false;
            }))
            {
                return;
            }

            if (selectItems.Count > 0)
            {
                _OperateEquips(() =>
                {
                    _CloseQuickSell();
                }, selectItems.ToArray());
            }
            else
            {
                SystemNotifyManager.SysNotifyMsgBoxOK(TR.Value("package_quick_sell_no_select"));
            }
        }

        void _OpenQuickSell()
        {
            if (m_eShowMode == EItemsShowMode.QuickSell)
            {
                return;
            }

            _InitOperatePanel("UIFlatten/Prefabs/Package/SellResultItem");

            if (mOperatePanel != null)
            {
                mOperatePanel.SetEvents(_OnSelectQualityChanged, _OnQuickSellClicked, _CloseQuickSell);
                mOperatePanel.SetTexts(TR.Value("package_sell_title"), TR.Value("package_sell_top_tip"), TR.Value("package_sell_empty_tip"), 
                    TR.Value("package_sell_result_tip"), TR.Value("package_sell_precious_tip"), TR.Value("package_sell_confirm"), false);
            }

            if (m_eShowMode == EItemsShowMode.Normal)
            {
                m_eShowMode = EItemsShowMode.QuickSell;
                _RefreshItemList();
                _ClearSelectState();
                mOperatePanel.Show();
                mOperateMask.enabled = true;
            }
        }
        #endregion

        #region quick decompose
        private PackageDecomposePanel mOperatePanel;
        GameObject m_objQuickDecomposeMask = null;
        GameObject m_objQuickDecomposeRoot = null;

        EItemsShowMode m_eShowMode = EItemsShowMode.Normal;

        void _InitOperatePanel(string itemPath)
        {
            if (mOperatePanel != null)
            {
                GameObject.Destroy(mOperatePanel.gameObject);
                mOperatePanel = null;
            }

            if (mOperatePanel == null)
            {
                m_objQuickDecomposeRoot = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Package/DecomposeGroup");
                GameObject objParent = Utility.FindGameObject("Content/DecomposeRoot");
                m_objQuickDecomposeRoot.transform.SetParent(objParent.transform, false);
                mOperatePanel = m_objQuickDecomposeRoot.GetComponent<PackageDecomposePanel>();
                //if (LeanTween.instance.frameBlackMask != null)
                //{
                //    m_objQuickDecomposeMask = GameObject.Instantiate(LeanTween.instance.frameBlackMask);
                //    m_objQuickDecomposeMask.transform.SetParent(ClientSystemManager.GetInstance().GetLayer(FrameLayer.Middle).transform, false);

                //    m_objQuickDecomposeMask.transform.SetParent(m_objQuickDecomposeRoot.transform, true);
                //    m_objQuickDecomposeMask.transform.SetAsFirstSibling();
                //}
                mOperateMask.SetHoleRect(mOperatePanel.HoleRect);
                m_eShowMode = EItemsShowMode.Normal;
                mOperatePanel.Init(itemPath);
                mOperatePanel.Hide();
                mOperateMask.enabled = false;
                _ClearSelectState();
            }
        }

        private void _OnItemSelect(List<ulong> guids)
        {
            if (guids == null)
            {
                mOperatePanel.UpdateResultList(null);
                mOperatePanel.UpdatePreciousItemList(null);
                return;
            }

            List<ItemData> selectItems = new List<ItemData>();
            List<ItemData> preciousItems = new List<ItemData>();

            int lv = 10;
            var configData = TableManager.GetInstance().GetTableItem<ClientConstValueTable>((int)ClientConstValueTable.eKey.EQUIP_DECOMPOSE_TIP_STRENGTH_LV);
            if (configData != null && configData.IntParamsLength > 0)
            {
                lv = configData.IntParamsArray(0);
            }

            for (int i = 0; i < guids.Count; ++i)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(guids[i]);
                if (itemData != null && itemData.IsSelected)
                {
                    selectItems.Add(itemData);

                    if (itemData.Quality > EItemQuality.PURPLE || itemData.StrengthenLevel >= lv)
                    {
                        preciousItems.Add(itemData);
                    }
                }
            }

            switch (m_eShowMode)
            {
                case EItemsShowMode.Decompose:
                    _UpdateDecomposePanel(selectItems);
                    mOperatePanel.UpdatePreciousItemList(preciousItems);
                    break;
                case EItemsShowMode.QuickSell:
                    _UpdateSellPanel(selectItems);
                    mOperatePanel.UpdatePreciousItemList(preciousItems);
                    break;
                case EItemsShowMode.FashionDecompose:
                    _UpdateFashionDecomposePanel(selectItems);
                    mOperatePanel.UpdatePreciousItemList(preciousItems);
                    break;
            }
        }

        private void _UpdateFashionDecomposePanel(List<ItemData> selectItems)
        {
            Dictionary<int, KeyValuePair<int, int>> dic = new Dictionary<int, KeyValuePair<int, int>>();
            for (int i = 0; i < selectItems.Count; ++i)
            {
                var data = GameUtility.Item.GetFashionDecomposeData(selectItems[i]);
                if (data != null)
                {
                    for (int j = 0; j < data.TextLength; ++j)
                    {
                        if (!dic.ContainsKey(data.TextArray(j)))
                        {
                            dic.Add(data.TextArray(j), new KeyValuePair<int, int>(1, 1));
                        }
                    }
                }
            }
            mOperatePanel?.SetShowName(true);
            mOperatePanel?.UpdateResultList(dic);
        }

        private void _UpdateSellPanel(List<ItemData> selectItems)
        {
            Dictionary<int, KeyValuePair<int, int>> dic = new Dictionary<int, KeyValuePair<int, int>>();
            for (int i = 0; i < selectItems.Count; ++i)
            {
                int id = selectItems[i].TableData.SellItemID;
                int price = selectItems[i].TableData.Price;
                if (id > 0 && price > 0)
                {
                    if (dic.ContainsKey(id))
                    {
                        dic[id] = new KeyValuePair<int, int>(dic[id].Key + price, dic[id].Key + price);
                    }
                    else
                    {
                        dic.Add(id, new KeyValuePair<int, int>(price, price));
                    }
                }
            }
            mOperatePanel?.SetShowName(false);
            mOperatePanel?.UpdateResultList(dic);
        }

        private void _UpdateDecomposePanel(List<ItemData> selectItems)
        {
            Dictionary<int, KeyValuePair<int, int>> dic = new Dictionary<int, KeyValuePair<int, int>>();
            for (int i = 0; i < selectItems.Count; ++i)
            {
                var decomposeTable = GameUtility.Item.GetDecomposeData(selectItems[i]);
                if (decomposeTable != null)
                {
                    if(selectItems[i].SubType == (int)ItemTable.eSubType.ST_BXY_EQUIP)
                    {
                        if((int)selectItems[i].Quality == 3)
                        {
                            dic.Add(910000024, new KeyValuePair<int, int>(50, 100));
                        }
                        else if((int)selectItems[i].Quality == 4)
                        {
                            dic.Add(910000024, new KeyValuePair<int, int>(100, 200));
                        }
                        else if((int)selectItems[i].Quality == 5)
                        {
                            dic.Add(910000024, new KeyValuePair<int, int>(200, 320));
                        }
                        else
                        {
                            dic.Add(910000024, new KeyValuePair<int, int>(320, 640));
                        }
                    }
                    else
                    {
                        //无色材料
                        GameUtility.Item.UpdateNumDecomposeMaterial(decomposeTable.ColorLessMatNumLength, decomposeTable.ColorLessMatNumArray, decomposeTable.ColorLessMatId, dic);
                        //有色材料
                        GameUtility.Item.UpdateNumDecomposeMaterial(decomposeTable.ColorMatNumLength, decomposeTable.ColorMatNumArray, decomposeTable.ColorMatId, dic);
                        //宇宙之眼
                        GameUtility.Item.UpdateNumDecomposeMaterial(decomposeTable.DogEyeNumLength, decomposeTable.DogEyeNumArray, decomposeTable.DogEyeId, dic);
                        //异界材料
                        GameUtility.Item.UpdateStringDecomposeMaterial(decomposeTable.MagicItemNumLength, decomposeTable.MagicItemNumArray, null, decomposeTable.MagicItemId, dic);
                        //特殊材料
                        GameUtility.Item.UpdateStringDecomposeMaterial(1, null, decomposeTable.PinkItemNum, decomposeTable.PinkItemId, dic);
                        //异界气息材料
                        GameUtility.Item.UpdateStringDecomposeMaterial(1, null, decomposeTable.RedItemNum, decomposeTable.RedItemId, dic);
                    }
                }

            }

            mOperatePanel?.SetShowName(false);
            mOperatePanel?.UpdateResultList(dic);
        }

        public enum OperateMask
        {
            OM_WHITE = (1 << 0),
            OM_BLUE = (1 << 1),
            OM_PURPLE = (1 << 2),
            OM_DEFAULT = OM_WHITE,
        }
        int mDecomposeMask = (int)OperateMask.OM_DEFAULT;
        void _ApplyDefaultDecomposeSetting()
        {
            if (mOperatePanel == null)
            {
                return;
            }
            mDecomposeMask &= ~((int)OperateMask.OM_PURPLE);
            mDecomposeMask |= ((int)OperateMask.OM_DEFAULT);
        }

        void _SaveDecomposeSetting()
        {
            if (mOperatePanel == null)
            {
                return;
            }

            for (int i = 0; i < mOperatePanel.GetToggleCount(); ++i)
            {
                if(mOperatePanel.GetToggleValue(i))
                {
                    mDecomposeMask |= (1 << i);
                }
                else
                {
                    mDecomposeMask &= ~(1 << i);
                }
            }
        }

        void _ClearQuickDecompose()
        {
            _CloseQuickDecompose();

            m_objQuickDecomposeMask = null;
            m_objQuickDecomposeMask = null;
            m_objQuickDecomposeRoot = null;

            m_eShowMode = EItemsShowMode.Normal;
            mOperateMask.enabled = false;
        }

        void _OpenQuickDecompose()
        {
            if (m_eShowMode == EItemsShowMode.Decompose)
            {
                return;
            }
            _InitOperatePanel("UIFlatten/Prefabs/Package/DecomposeResultItem");


            if (mOperatePanel != null)
            {
                mOperatePanel.SetEvents(_OnSelectQualityChanged, _OnQuickDecomposeClicked, _OnReturnClicked);
                mOperatePanel.SetTexts(TR.Value("package_decompose_title"), TR.Value("package_decompose_top_tip"), TR.Value("package_decompose_empty_tip"), TR.Value("package_decompose_result_tip"),
                    TR.Value("package_decompose_precious_tip"), TR.Value("package_decompose_confirm"), false);
            }

            if (m_eShowMode == EItemsShowMode.Normal)
            {
                m_eShowMode = EItemsShowMode.Decompose;
                _ClearSelectState();
                mOperatePanel.Show();
                mOperateMask.enabled = true;
            }
            if (PlayerBaseData.GetInstance().Level >= 15)
            {
                _ApplyDefaultDecomposeSetting();
            }
            _RefreshItemList();
        }

        void _OpenFashionDecompose(List<ulong> GUIDs = null)
        {
            if (m_eShowMode == EItemsShowMode.FashionDecompose)
            {
                return;
            }

            _InitOperatePanel("UIFlatten/Prefabs/Package/DecomposeResultItem");

            if (mOperatePanel != null)
            {
                mOperatePanel.SetEvents(null, _OnFashionDecomposeClicked, _CloseFashionDecompose);
                mOperatePanel.SetTexts(TR.Value("package_fashion_decompose_title"), TR.Value("package_fashion_decompose_top_tip"), 
                    TR.Value("package_fashion_decompose_empty_tip"), TR.Value("package_fashion_decompose_result_tip"), 
                    TR.Value("package_fashion_decompose_precious_tip"), TR.Value("package_fashion_decompose_confirm"), false);
                mOperatePanel.Show();
                mOperateMask.enabled = true;
            }

            if (m_eShowMode == EItemsShowMode.Normal)
            {
                m_eShowMode = EItemsShowMode.FashionDecompose;
                _ClearSelectState();
                if(GUIDs != null)
                {
                    for(int i = 0;i < GUIDs.Count;i++)
                    {
                        ItemData itemData = ItemDataManager.GetInstance().GetItem(GUIDs[i]);
                        if(itemData != null)
                        {
                            itemData.IsSelected = true;
                        }
                    }
                }
            }
            _OnItemSelect(GUIDs);
            _RefreshItemList();
        }

        private void _OnFashionDecomposeClicked()
        {
            List<ulong> itemIDs = new List<ulong>();
            List<ulong> highValueEquipID = new List<ulong>();
            List<ulong> guids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);
            for (int i = 0; i < guids.Count; ++i)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(guids[i]);
                if (itemData != null && itemData.IsSelected)
                {
                    itemIDs.Add(itemData.GUID);

                    //itemData.SuitID == 101139，天穹套装添加到确认提示里面
                    if (itemData != null && itemData.Quality > ItemTable.eColor.PURPLE || itemData.ThirdType == ItemTable.eThirdType.FASHION_FESTIVAL || itemData.SuitID == 101139)
                    {
                        highValueEquipID.Add(itemData.GUID);
                    }
                }
            }

            if (itemIDs.Count == 0)
            {
                SystemNotifyManager.SysNotifyMsgBoxOK(TR.Value("package_fashion_decompse_null"));
                return;
            }

            highValueEquipID.Sort((x, y) =>
            {
                var left = ItemDataManager.GetInstance().GetItem(x);
                var right = ItemDataManager.GetInstance().GetItem(y);

                if (left != null && right != null)
                {
                    if (left.Quality != right.Quality)
                    {
                        return right.Quality - left.Quality;
                    }

                    return left.TableID - right.TableID;
                }

                return -1;
            });

            ulong[] ids = itemIDs.ToArray();
            if (highValueEquipID.Count == 0)
            {
                ItemDataManager.GetInstance().SendDecomposeItem(ids, true);
            }
            else
            {
                var systemValue = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_CONFIRT_COUNTDOWN_TIME);
                int countDownTime = systemValue.Value;

                SystemNotifyManager.SysNotifyMsgBoxCancelOk(_GetMultiDecomposeTipDesc(highValueEquipID), "", "", null, () =>
                {
                    ItemDataManager.GetInstance().SendDecomposeItem(ids, true);
                }, countDownTime, false, null, true);
            }


            if (highValueEquipID.Count == 0)
            {
                ItemDataManager.GetInstance().SendDecomposeItem(ids, true);
            }
            else
            {
                var systemValue = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_CONFIRT_COUNTDOWN_TIME);
                int countDownTime = systemValue.Value;

                SystemNotifyManager.SysNotifyMsgBoxCancelOk(_GetMultiDecomposeTipDesc(highValueEquipID), "", "", null, () =>
                {
                    ItemDataManager.GetInstance().SendDecomposeItem(ids, true);
                }, countDownTime, false, null, true);
            }

            _CloseFashionDecompose();
        }

        string _GetMultiDecomposeTipDesc(List<ulong> guids)
        {
            if (guids == null)
            {
                return "";
            }

            if (guids.Count == 0)
            {
                return "";
            }

            if (guids.Count == 1)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(guids[0]);
                if (itemData != null)
                {
                    return TR.Value("decompose_one_fashion_tips", itemData.GetQualityDesc(), itemData.GetColorName());
                }
            }
            else
            {
                string nameStr = "";
                for (int i = 0; i < guids.Count && i < 6; i++) // 最多只显示6个时装名字
                {
                    ItemData itemData = ItemDataManager.GetInstance().GetItem(guids[i]);
                    if (itemData == null)
                    {
                        continue;
                    }

                    nameStr += TR.Value("decompose_fashion_name", itemData.GetColorName());
                }

                return TR.Value("decompose_multiple_fashion_tips", nameStr, guids.Count);
            }

            return "";
        }

        void _CloseQuickDecompose()
        {
            if (m_eShowMode == EItemsShowMode.Decompose)
            {
                m_eShowMode = EItemsShowMode.Normal;
                _RefreshItemList();

                _ClearSelectState();
                mOperatePanel?.Hide();
                mOperateMask.enabled = false;
            }
        }

        void _CloseFashionDecompose()
        {
            if (m_eShowMode == EItemsShowMode.FashionDecompose)
            {
                m_eShowMode = EItemsShowMode.Normal;
                _RefreshItemList();

                _ClearSelectState();
                mOperatePanel?.Hide();
                mOperateMask.enabled = false;
            }
        }

        List<ItemData> _GetItemsByQuality(EItemQuality a_quality)
        {
            List<ItemData> arrItems = new List<ItemData>();
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);
            for (int i = 0; i < itemGuids.Count; ++i)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                if (itemData != null && itemData.Quality == a_quality && itemData.CanDecompose)
                {
                    arrItems.Add(itemData);
                }
            }
            return arrItems;
        }

        void _ClearSelectState()
        {
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);
            for (int i = 0; i < itemGuids.Count; ++i)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                if (itemData != null && itemData != null)
                {
                    itemData.IsSelected = false;
                }
            }
        }

        void _DecomposeEquip(Action a_okCallback, ItemData item)
        {
            PackageOperateConfirmFrameParam param = new PackageOperateConfirmFrameParam();
            param.Mode = (int)EItemsShowMode.Decompose;
            param.Equip = item;
            param.CallBack = a_okCallback;
            param.IsPrecious = _IsPrecious(item);
            ClientSystemManager.GetInstance().OpenFrame<PackageOperateConfirmFrame>(FrameLayer.Middle, param);
        }


        void _OperateEquips(Action a_okCallback, params ItemData[] a_arrItems)
        {
            if (a_arrItems == null || a_arrItems.Length == 0)
            {
                return;
            }

            List<ItemData> arrHighValueItems = new List<ItemData>();

            for (int i = 0; i < a_arrItems.Length; ++i)
            {
                ItemData itemData = a_arrItems[i];
                if (itemData != null && itemData.IsSelected)
                {
                    if (_IsPrecious(itemData))
                    {
                        arrHighValueItems.Add(itemData);
                    }
                }
            }

            if (arrHighValueItems.Count > 0)
            {
                PackagePreciousConfirmFrameParam param = new PackagePreciousConfirmFrameParam();
                param.Mode = (int)m_eShowMode;
                param.ItemPreciousList = arrHighValueItems;
                param.ItemList = a_arrItems;
                param.CallBack = a_okCallback;
                ClientSystemManager.GetInstance().OpenFrame<PackagePreciousConfirmFrame>(FrameLayer.Middle, param);
            }
            else
            {
                ulong[] ids = new ulong[a_arrItems.Length];
                for (int i = 0; i < ids.Length; ++i)
                {
                    ids[i] = a_arrItems[i].GUID;
                }
                if (m_eShowMode == EItemsShowMode.QuickSell)
                {
                    ItemDataManager.GetInstance().SendSellItem(ids);
                }
                else if (m_eShowMode == EItemsShowMode.Decompose)
                {
                    ItemDataManager.GetInstance().SendDecomposeItem(ids);
                }
                a_okCallback?.Invoke();
            }
        }

        bool _IsPrecious(ItemData itemData)
        {
            int lv = 10;
            var configData = TableManager.GetInstance().GetTableItem<ClientConstValueTable>((int)ClientConstValueTable.eKey.EQUIP_DECOMPOSE_TIP_STRENGTH_LV);
            if (configData != null && configData.IntParamsLength > 0)
            {
                lv = configData.IntParamsArray(0);
            }

            if (itemData != null && (itemData.Quality > EItemQuality.PURPLE || itemData.StrengthenLevel >= lv || itemData.EquipType == EEquipType.ET_REDMARK))
            {
                return true;
            }

            return false;
        }

        void _OnReturnClicked()
        {
            _CloseQuickDecompose();
        }

        void _OnQuickDecomposeClicked()
        {
            List<ItemData> selectItems = new List<ItemData>();
            List<ulong> guids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);
            for (int i = 0; i < guids.Count; ++i)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(guids[i]);
                if (itemData != null && itemData.IsSelected)
                {
                    selectItems.Add(itemData);
                }
            }

            if (SecurityLockDataManager.GetInstance().CheckSecurityLock(() =>
             {
                 for (int i = 0; i < selectItems.Count; i++)
                 {
                     ItemData itemData = selectItems[i];
                     if (itemData != null && itemData.Quality >= EItemQuality.PURPLE)
                     {
                         return true;
                     }
                 }

                 return false;
             }))
            {
                return;
            }

            if (selectItems.Count > 0)
            {
                _OperateEquips(() =>
                {
                    _SaveDecomposeSetting();
                    _CloseQuickDecompose();
                }, selectItems.ToArray());
            }
            else
            {
                SystemNotifyManager.SysNotifyMsgBoxOK(TR.Value("package_quick_decompose_no_select"));
            }
        }

        void _OnSelectQualityChanged(int index, bool isChecked)
        {
            if (m_eShowMode == EItemsShowMode.Decompose ||
                m_eShowMode == EItemsShowMode.QuickSell)
            {
                List<ItemData> arrSelectItems;
                if (index == 0)
                {
                    arrSelectItems = _GetItemsByQuality(EItemQuality.WHITE);
                }
                else if (index == 1)
                {
                    arrSelectItems = _GetItemsByQuality(EItemQuality.BLUE);
                }
                else if (index == 2)
                {
                    arrSelectItems = _GetItemsByQuality(EItemQuality.PURPLE);
                }
                else
                {
                    arrSelectItems = new List<ItemData>();
                }

                List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);
                for (int i = 0; i < arrSelectItems.Count; ++i)
                {
                    arrSelectItems[i].IsSelected = isChecked && arrSelectItems[i].StrengthenLevel < 10 
                        && !arrSelectItems[i].bLocked; // 被安全锁锁住的道具无法进行一键出售或者分解 add by qxy 2019-04-11
                }

                if (arrSelectItems.Count > 0)
                {
                    ulong uGUID = arrSelectItems[0].GUID;
                    for (int i = 0; i < itemGuids.Count; ++i)
                    {
                        if (uGUID == itemGuids[i])
                        {
                            m_comItemList.EnsureElementVisable(i);
                        }
                    }
                }
                _OnItemSelect(ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType));
                m_comItemList.SetElementAmount(_GetRefreshMaxNum());
            }
        }
        #endregion 

        #region itemTab
        class ItemTabInfo
        {
            public EPackageType ePackageType;
            public Toggle toggle;
            public GameObject objRedPoint;
        }

        ItemTabInfo[] m_arrItemTabInfos =
        {
            new ItemTabInfo { ePackageType = EPackageType.Equip },
            new ItemTabInfo { ePackageType = EPackageType.Material },
            new ItemTabInfo { ePackageType = EPackageType.Consumable },
            new ItemTabInfo { ePackageType = EPackageType.Fashion },
            new ItemTabInfo { ePackageType = EPackageType.Title },
            new ItemTabInfo { ePackageType = EPackageType.Inscription },
            new ItemTabInfo { ePackageType = EPackageType.Bxy },
            new ItemTabInfo { ePackageType = EPackageType.Sinan },
        };

        EPackageType m_currentItemType = EPackageType.Invalid;

        void _InitItemTab()
        {
            for (int i = 0; i < m_arrItemTabInfos.Length; ++i)
            {
                ItemTabInfo info = m_arrItemTabInfos[i];
                int nID = (int)info.ePackageType;
                info.toggle = Utility.GetComponetInChild<Toggle>(frame,
                    string.Format("Content/ItemListTabs/Title{0}", nID));

                info.toggle.onValueChanged.RemoveAllListeners();
                info.toggle.onValueChanged.AddListener(var =>
                {
                    if (var)
                    {
                        EPackageType newPackageType = info.ePackageType;
                        if (m_currentItemType != newPackageType)
                        {
                            m_currentItemType = newPackageType;
                            ItemDataManager.GetInstance().ArrangeItemsInPackageFrame(m_currentItemType);
                            _RefreshItemTab();
                            _RefreshItemList(true);
                        }
                    }
                    else
                    {
                        info.objRedPoint.SetActive(IsItemTabShowRedPoint(info.ePackageType));
                    }
                });

                info.objRedPoint = Utility.FindGameObject(frame,
                    string.Format("Content/ItemListTabs/Title{0}/RedPoint", nID));
                info.objRedPoint.SetActive(IsItemTabShowRedPoint(info.ePackageType));

                info.toggle.gameObject.SetActive(false);
            }

            _UpdateFashionRedPoint();

            m_objShop.CustomActive(false);
        }

        void _RefreshItemTab()
        {

            for (int i = 0; i < m_arrItemTabInfos.Length; ++i)
            {
                ItemTabInfo info = m_arrItemTabInfos[i];
                info.objRedPoint.CustomActive(IsItemTabShowRedPoint(info.ePackageType));
            }
        }

        //背包的页签是否显示红点
        private bool IsItemTabShowRedPoint(EPackageType packageType)
        {
            //如果存在新的Item，直接返回true
            if (true == ItemDataManager.GetInstance().IsPackageHasNew(packageType))
                return true;

            //如果为消耗品页签，则判断消费品是否存在显示红点的物品
            if (packageType == EPackageType.Consumable)
            {
                return PackageDataManager.GetInstance().IsPackageTabShowRedPoint(packageType);
            }
            return false;
        }

        void _ClearItemTab()
        {
            for (int i = 0; i < m_arrItemTabInfos.Length; ++i)
            {
                ItemTabInfo info = m_arrItemTabInfos[i];
                info.objRedPoint = null;
                info.toggle = null;
            }

            m_currentItemType = EPackageType.Invalid;
        }

        ItemTabInfo _GetItemTabInfo(EPackageType a_type)
        {
            for (int i = 0; i < m_arrItemTabInfos.Length; ++i)
            {
                if (m_arrItemTabInfos[i].ePackageType == a_type)
                {
                    return m_arrItemTabInfos[i];
                }
            }
            return null;
        }
        #endregion

        #region itemList
        bool m_bItemlistInited = false;

        const string addPackSizeImgPath = "UI/Image/Packed/p_UI_Set.png:UI_Shezhi_Tubiao_Jiahao";

        int _OnSelectPrefab(int index)
        { 
            var packTotalSize = PlayerBaseData.GetInstance().PackTotalSize[(int)m_currentItemType];
            if (index < packTotalSize)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        void _InitItemList()
        {
            m_comItemList.Initialize();

            m_comItemList.OnSelectPrefab = _OnSelectPrefab;
            m_comItemList.onItemVisiable = (item) =>
            {
                List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);

                int MaxPackSize = _GetRefreshMaxNum();

                if (item.m_index >= 0 && item.m_index < MaxPackSize)
                {
                    if (item.m_index < itemGuids.Count)
                    {
                        item.name = itemGuids[item.m_index].ToString();
                        var bind = item.GetComponent<ComGridBindItem>();
                        var comItem = item.GetComponentInChildren<ComItemNew>();
                        ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[item.m_index]);
                        if (itemData != null)
                        {
                            comItem?.Setup(itemData, _OnPackageItemClicked);
                            comItem?.SetupSlot(ComItem.ESlotType.Opened, string.Empty);
                            comItem?.SetShowBetterState(true);
                            if (m_eShowMode == EItemsShowMode.Decompose)
                            {
                                comItem?.SetEnable(itemData.CanDecompose && itemData.StrengthenLevel < 10);
                            }
                            else if (m_eShowMode == EItemsShowMode.QuickSell)
                            {
                                comItem?.SetEnable(itemData.CanSell && itemData.StrengthenLevel < 10);
                            }
                            else if (m_eShowMode == EItemsShowMode.FashionDecompose)
                            {
                                comItem?.SetEnable(itemData.CanDecompose && !itemData.bFashionItemLocked &&itemData.DeadTimestamp <= 0);
                            }
                            else
                            {
                                comItem?.SetEnable(true);
                            }

                            comItem?.SetShowSelectState(itemData.IsSelected && (m_eShowMode == EItemsShowMode.Decompose || m_eShowMode == EItemsShowMode.QuickSell || m_eShowMode == EItemsShowMode.FashionDecompose));

                            if (bind != null)
                            {
                                bind.param1 = item.gameObject.name;
                                bind.param2 = itemData.GUID;
                            }
                        }
                        else
                        {
                            comItem?.Setup(null, null);
                            comItem?.SetupSlot(ComItem.ESlotType.Opened, string.Empty);
                            comItem?.SetEnable(true);


                            if (bind != null)
                            {
                                bind.param1 = null;
                                bind.param2 = 0;
                            }
                        }
                    }
                    else if (item.m_index < PlayerBaseData.GetInstance().PackTotalSize[(int)m_currentItemType])
                    {
                        var bind = item.GetComponent<ComGridBindItem>();
                        var comItem = item.GetComponentInChildren<ComItemNew>();
                        comItem?.Setup(null, null);
                        comItem?.SetupSlot(ComItem.ESlotType.Opened, string.Empty);
                        comItem?.SetEnable(true);

                        if (bind != null)
                        {
                            bind.param1 = null;
                            bind.param2 = 0;
                        }
                    }
                    else
                    {
                        
                        var bind = item.GetComponent<ComGridBindItem>();
                        var lockitem = item.GetComponent<PackageLockItem>();
                        lockitem?.Init(_UpgradePackageSize);
                        if (bind != null)
                        {
                            bind.param1 = null;
                            bind.param2 = 0;
                        }
                    }
                }

            };

            m_bItemlistInited = true;
        }

        void _RefreshItemList(bool resetScrollPos = false)
        {
            if (m_bItemlistInited == false)
            {
                return;
            }

            m_comItemList.SetElementAmount(_GetRefreshMaxNum());

            if (resetScrollPos && m_scrollRect)
            {
                m_scrollRect.verticalNormalizedPosition = 1.0f;
            }

            // grid count
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);

            if ((int)m_currentItemType < PlayerBaseData.GetInstance().PackTotalSize.Count)
                m_gridCount.text = TR.Value("grid_info", itemGuids.Count, PlayerBaseData.GetInstance().PackTotalSize[(int)m_currentItemType]);

            m_comBtnChapterPotionSet.CustomActive(false);
            btnFashionEquipDecompose.CustomActive(false);
            m_objFashionMerge.CustomActive(false);
            m_objInscriptionMerge.CustomActive(false);
            m_comBtnQuickDecompose.CustomActive(m_currentItemType == EPackageType.Equip);
            m_comBtnQuickSell.CustomActive(m_currentItemType == EPackageType.Equip);

            if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.BattleDrugs))
            {
                // func
                m_comBtnChapterPotionSet.CustomActive(m_currentItemType == EPackageType.Consumable);
            }

            if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Inscription))
            {
                btnFashionEquipDecompose.CustomActive(m_currentItemType == EPackageType.Fashion);
            }

            bool isFlag = Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.FashionMerge) &&
               !ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_FASHION_MERGO)&&
               m_currentItemType == EPackageType.Fashion;
            m_objFashionMerge.CustomActive(isFlag);

            bool isInscriptionMergeFlag = Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.Inscription) && m_currentItemType == EPackageType.Inscription;
            m_objInscriptionMerge.CustomActive(isInscriptionMergeFlag);
        }

        int _GetRefreshMaxNum()
        {
            var maxSizeData = TableManager.GetInstance().GetTableItem<ClientConstValueTable>((int)ClientConstValueTable.eKey.PACKAGE_EQUIP_SIZE);
            int maxSize = 1000;
            if (maxSizeData != null && maxSizeData.IntParamsLength > 0)
            {
                maxSize = maxSizeData.IntParamsArray(0);
            }

            if (PlayerBaseData.GetInstance().PackTotalSize[(int)m_currentItemType] < maxSize)
            {
                return PlayerBaseData.GetInstance().PackTotalSize[(int)m_currentItemType] + 1;
            }

            return maxSize;
        }

        void _ClearItemList()
        {
            m_bItemlistInited = false;
        }

        void _UpgradePackageSize()
        {
            int key = PlayerBaseData.GetInstance().PackBaseSize + 8;
            if (key <= 100)
            {
                ClientSystemManager.GetInstance().OpenFrame<PackageExpandFrame>();
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("package_unlock_max"));
            }
        }

        private void _OnUpdateGrids(UIEvent uievent)
        {
            _RefreshItemTab();
            _RefreshItemList();
            if (m_comItemList != null)
            {
                m_comItemList.MoveElementInScrollArea(m_comItemList.m_elementAmount - 1, true);
            }
        }

        #endregion

        #region item click
        void _OnWearedItemClicked(GameObject obj, IItemDataModel model)
        {
            ItemData item = model as ItemData;
            if (item == null)
            {
                return;
            }

            List<TipFuncButon> funcs = new List<TipFuncButon>();
            TipFuncButon tempFunc = null;

            ClientSystemGameBattle ChijiTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (ChijiTown != null)
            {
                var dataItem = TableManager.GetInstance().GetTableItem<ItemTable>(item.TableID);
                if (dataItem != null)
                {
                    if (item.Type == EItemType.EQUIP)
                    {
                        TipFuncButonSpecial tempfunc = new TipFuncButonSpecial
                        {
                            text = TR.Value("tip_takeoff"),
                            callback = _OnUnWear
                        };

                        funcs.Add(tempfunc);
                    }
                }
            }
            else
            {
                // 原有流程
                if (item.Type == EItemType.EQUIP || item.Type == EItemType.FASHION || item.Type == EItemType.FUCKTITTLE)
                {
                    int nTimeLeft;
                    bool bStartCountdown;
                    item.GetTimeLeft(out nTimeLeft, out bStartCountdown);

                    //一键卸下
                    if (bStartCountdown == false || nTimeLeft > 0)
                    {
                        if (item.Type == EItemType.FASHION && _CanAllUse(item.TableID))
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_all_takeoff"),
                                callback = _OnTakeOffAllFashion
                            };
                            funcs.Add(tempFunc);
                        }
                    }


                    // 已经开始计时的，才有可能续费
                    if (bStartCountdown && item.CanRenewal())
                    {
                        tempFunc = new TipFuncButon
                        {
                            text = TR.Value("tip_renewal"),
                            name = "Renewal",
                            callback = _OnItemRenewal
                        };
                        funcs.Add(tempFunc);
                    }

                    // 没有失效的道具，才能有其他功能
                    if (bStartCountdown == false || nTimeLeft > 0)
                    {
                        {
                            TipFuncButonSpecial tempfunc = new TipFuncButonSpecial
                            {
                                text = TR.Value("tip_takeoff"),
                                callback = _OnUnWear
                            };
                            funcs.Add(tempfunc);
                        }

                        if (item.Type == EItemType.EQUIP)
                        {
                            if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Forge))
                            {
                                // 辟邪玉没有锻冶功能
                                if (!item.bLocked && !item.IsLease && item.SubType != (int)ItemTable.eSubType.ST_BXY_EQUIP)
                                {
                                    tempFunc = new TipFuncButon
                                    {
                                        text = TR.Value("tip_forge"),
                                        name = "Forge",
                                        callback = _OnStrengthenItem
                                    };
                                    funcs.Add(tempFunc);
                                }

                                //tempFunc = new TipFuncButon();
                                //tempFunc.text = TR.Value("tip_enchanting");
                                //tempFunc.name = "Enchanting";
                                //tempFunc.callback = _OnEnchantingItem;
                                //funcs.Add(tempFunc);

                                //if (!item.IsLease)
                                //{
                                //    tempFunc = new TipFuncButon
                                //    {
                                //        text = TR.Value("tip_adjust_grade"),
                                //        name = "AdjustItemGrade",
                                //        callback = _OnAdjustItemGrade
                                //    };
                                //    funcs.Add(tempFunc);
                                //}
                            }
                        }

                        //镶嵌
                        if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Forge) && !item.isInSidePack && !item.bLocked && !item.IsLease)
                        {
                            if (item.Type == EItemType.FUCKTITTLE && item.SubType == (int)ItemTable.eSubType.TITLE && Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Bead))
                            {
                                if (item.DeadTimestamp == 0)
                                {
                                    tempFunc = new TipFuncButon
                                    {
                                        text = TR.Value("tip_BeadMosaic"),
                                        name = "BeadMosaic",
                                        callback = _OnForgeItem
                                    };
                                    funcs.Add(tempFunc);
                                }
                            }
                        }

                        //时装合成、、天穹套装不显示
                        if (Utility._CheckFashionCanMerge(item.GUID) && item.SuitID != 101139)
                        {
                            if (Utility.IsUnLockFunc((int)ProtoTable.FunctionUnLock.eFuncType.FashionMerge))
                            {
                                if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_FASHION_MERGO) == false)
                                {
                                    funcs.Add(FashionMergeManager.GetInstance().MergeFunction);
                                }
                            }
                        }

                        //属性重选
                        if (item.Type == EItemType.FASHION &&
                            item.SubType != (int)ItemTable.eSubType.FASHION_HAIR)
                        {
                            if (Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.FashionAttrSel))
                            {
                                if (item.HasFashionAttribute)
                                {
                                    tempFunc = new TipFuncButon
                                    {
                                        text = TR.Value("tip_fashion_attr_sel"),
                                        name = "fashion_attr_sel",
                                        callback = _OnFashionAttrSelItem
                                    };
                                    funcs.Add(tempFunc);
                                }
                            }
                        }

                        if (item.Quality > ItemTable.eColor.PURPLE ||
                            (item.Type == ItemTable.eType.FUCKTITTLE && item.Quality > ItemTable.eColor.BLUE))
                        { // 分享
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_share"),
                                callback = _OnShareClicked
                            };
                            funcs.Add(tempFunc);
                        }
                       
                    }

                    if (item.Type == EItemType.EQUIP)
                    {
                        if (!item.bLocked && !item.IsLease)
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_lock_item"),
                                name = "LockItem",
                                callback = _OnLockItem
                            };
                            funcs.Add(tempFunc);
                        }
                        else
                        {
                            if (!item.IsLease)
                            {
                                tempFunc = new TipFuncButon
                                {
                                    text = TR.Value("tip_unlock_item"),
                                    name = "UnLockItem",
                                    callback = _OnUnLockItem
                                };
                                funcs.Add(tempFunc);
                            }
                        }
                    }
                    else if (item.Type == EItemType.FASHION) // 时装也可以加锁或者解锁
                    {
                        if (item.bFashionItemLocked)
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_unlock_item"),
                                name = "UnLockItem",
                                callback = _OnUnLockItem
                            };
                            funcs.Add(tempFunc);
                        }
                        else
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_lock_item"),
                                name = "LockItem",
                                callback = _OnLockItem
                            };
                            funcs.Add(tempFunc);
                        }
                    }
                }
            }

            ItemTipManager.GetInstance().ShowTip(item, funcs, TextAnchor.MiddleRight);
        }

        void _OnPackageItemClicked(GameObject obj, IItemDataModel model)
        {
            var item = model as ItemData;
            if(item == null)
            {
                return;
            }
               
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftControl))
            {
                var req = new SceneChat();
                req.channel = 1;
                req.targetId = 0;
                req.voiceKey = "";
                req.word = string.Format("!!sellitem uid={0} num={1}", item.GUID, item.Count);

                NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);

                return;
            }
#endif

            List<TipFuncButon> funcs = new List<TipFuncButon>();
            TipFuncButon tempFunc = null;

            ClientSystemGameBattle ChijiTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (ChijiTown != null)
            {
                var dataItem = TableManager.GetInstance().GetTableItem<ItemTable>(item.TableID);
                if (dataItem != null)
                {
                    // 使用，穿戴
                    if (item.IsCooling() == false)
                    {
                        if (item.PackageType == EPackageType.Equip)
                        {
                            tempFunc = new TipFuncButonSpecial();
                            tempFunc.text = TR.Value("tip_wear");
                            tempFunc.callback = _TryOnUseItem;

                            funcs.Add(tempFunc);
                        }
                        else if (item.ThirdType != ItemTable.eThirdType.UseToOther && item.ThirdType != ItemTable.eThirdType.ST_CHIJI_MIANZHAN)
                        {
                            tempFunc = new TipFuncButonSpecial();
                            tempFunc.text = TR.Value("tip_use");
                            tempFunc.callback = _TryOnUseItem;

                            funcs.Add(tempFunc);
                        }

                        //吃鸡场景中，出售道具
                        if (item.CanSell == true)
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_sell"),
                                name = "Sell",
                                callback = _OnSellItemInChijiScene,
                                tipFuncButtonType = TipFuncButtonType.Trigger
                            };
                            funcs.Add(tempFunc);
                        }
                    }

                    // 穿戴使用要new TipFuncButonSpecial，其他的new TipFuncButon
                    tempFunc = new TipFuncButon();

                    tempFunc.text = TR.Value("tip_throwoff");
                    tempFunc.name = "ThrowOff";
                    tempFunc.callback = _TryOnThrowOffItem;

                    funcs.Add(tempFunc);
                }
            }
            else
            {
                //道具是否在未启用的装备方案中
                var isItemInUnUsedEquipPlan = item.IsItemInUnUsedEquipPlan;

                //原来流程
                //当魔锤和魔盒正在全部使用的时候，点击魔锤和魔盒没有反应
                if (item.SubType == (int)ItemTable.eSubType.MagicBox)
                {
                    if (PackageDataManager.GetInstance().AlreadyUseTotalMagicBox == true)
                        return;
                }

                if (item.SubType == (int)ItemTable.eSubType.MagicHammer)
                {
                    if (PackageDataManager.GetInstance().AlreadyUseTotalMagicHammer == true)
                        return;
                }
                #endregion

                if (m_eShowMode == EItemsShowMode.Decompose)
                {
                    if (item.CanDecompose)
                    {
                        item.IsSelected = !item.IsSelected;
                        if (item.bLocked) // 安全锁锁住的道具无法被选中
                        {
                            item.IsSelected = false;
                        }
                        obj.GetComponent<ComItemNew>().SetShowSelectState(item.IsSelected);
                        _OnItemSelect(ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType));
                    }
                    return;
                }

                if (m_eShowMode == EItemsShowMode.FashionDecompose)
                {
                    if (item.CanDecompose)
                    {
                        item.IsSelected = !item.IsSelected;

                        if (item.bFashionItemLocked) // 时装锁锁住的道具无法被选中
                        {
                            item.IsSelected = false;
                        }

                        obj.GetComponent<ComItemNew>().SetShowSelectState(item.IsSelected);
                        _OnItemSelect(ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType));
                    }
                    return;
                }

                if (m_eShowMode == EItemsShowMode.QuickSell)
                {
                    if (item.CanSell)
                    {
                        item.IsSelected = !item.IsSelected;
                        if (item.bLocked) // 安全锁锁住的道具无法被选中
                        {
                            item.IsSelected = false;
                        }
                        obj.GetComponent<ComItemNew>().SetShowSelectState(item.IsSelected);
                        _OnItemSelect(ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType));
                    }
                    return;
                }

                {
                    int nTimeLeft;
                    bool bStartCountdown;
                    item.GetTimeLeft(out nTimeLeft, out bStartCountdown);
                    //时装一键穿戴
                    if (bStartCountdown == false || nTimeLeft > 0)
                    {
                        if (item.Type == EItemType.FASHION && item.OccupationLimit[0] == Utility.GetBaseJobID(PlayerBaseData.GetInstance().JobTableID) && _CanAllUse(item.TableID))
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_all_wear"),
                                callback = _OnWearAllFashion
                            };
                            funcs.Add(tempFunc);
                        }
                    }
                    // 已经开始计时的，才有可能续费
                    if (bStartCountdown && item.CanRenewal())
                    {
                        tempFunc = new TipFuncButon
                        {
                            text = TR.Value("tip_renewal"),
                            name = "Renewal",
                            callback = _OnItemRenewal
                        };
                        funcs.Add(tempFunc);
                    }

                    // 没有失效的道具，才能有其他功能
                    if (bStartCountdown == false || nTimeLeft > 0)
                    {
                        // 来源
                        var dataItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)item.TableID);
                        if (dataItem != null && !string.IsNullOrEmpty(dataItem.LinkInfo))
                        {
                            string textdes = TR.Value("tip_itemLink");

                            if (GameClient.EquipTransferUtility.IsTransferStone(item.TableID))
                            {
                                textdes = TR.Value("tip_itemLink_transfer");
                            }
                            if (dataItem.SubType == ItemTable.eSubType.Perfect_washing)
                            {
                                textdes = TR.Value("tip_perfectquality");
                            }
                            tempFunc = new TipFuncButon
                            {
                                text = textdes,
                                name = "itemLink",
                                callback = _OnItemLink
                            };
                            funcs.Add(tempFunc);
                        }


                        // 药品配置
                        var func = ItemDataManager.GetInstance().GetAddDrugUseTipFunc(item) as TipFuncButon;
                        if (null != func)
                        {
                            funcs.Add(func);
                        }

                        //赠送好友
                        if (item.SubType==(int)ItemTable.eSubType.ST_FRIEND_PRESENT)
                        {
                            // 赠送
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_sendGift"),
                                callback = _OnSendGiftCick
                            };
                            funcs.Add(tempFunc);
                        }
                        // 使用，穿戴
                        if (item.UseType == ProtoTable.ItemTable.eCanUse.UseOne ||
                            item.UseType == ProtoTable.ItemTable.eCanUse.UseTotal)
                        {
                            if (item.SubType == (int)ItemTable.eSubType.ST_HONOR_GUARD_CARD)
                            {
                                //todo
                                //荣誉保护卡在ItemTip上不显示使用按钮
                            }
                            else
                            {
                                if (item.IsCooling() == false && !item.isInSidePack && item.EquipType != EEquipType.ET_BREATH)
                                {
                                    //if(item.SubType != SubType)
                                    tempFunc = new TipFuncButonSpecial();
                                    if (item.PackageType == EPackageType.Equip || item.PackageType == EPackageType.Fashion || item.PackageType == EPackageType.Title || item.PackageType == EPackageType.Bxy)
                                    {
                                        tempFunc.text = TR.Value("tip_wear");
                                        if (item.HasTransfered)
                                        {
                                            tempFunc.callback = _TryDeTransferClicked;
                                        }
                                        else if (item.Packing)
                                        {
                                            tempFunc.callback = _TryDeSealClicked;
                                        }
                                        else
                                        {
                                            tempFunc.callback = _TryOnUseItem;
                                        }
                                    }
                                    else if (item.PackageType == EPackageType.Consumable && item.SubType == (int)ItemTable.eSubType.ChangeName)
                                    {
                                        tempFunc.text = TR.Value("tip_use");
                                        tempFunc.callback = _OnUseChangeName;
                                    }
                                    else if (item.PackageType == EPackageType.Consumable && item.SubType == (int)ItemTable.eSubType.MagicBox)
                                    {
                                        tempFunc.text = TR.Value("tip_use");
                                        tempFunc.callback = _OnUseMagicBox;
                                    }
                                    else if (item.PackageType == EPackageType.Consumable && item.SubType == (int)ItemTable.eSubType.MagicHammer)
                                    {
                                        tempFunc.text = TR.Value("tip_use");
                                        tempFunc.callback = _OnUseMagicHammer;
                                    }
                                    else if (item.PackageType == EPackageType.Consumable && item.SubType == (int)ItemTable.eSubType.ST_EXTENSIBLE_ROLE_CARD)
                                    {
                                        tempFunc.text = TR.Value("tip_use");
                                        tempFunc.callback = _OnUseExtendRoleFieldCard;
                                    }
                                    else
                                    {
                                        tempFunc.text = TR.Value("tip_use");
                                        tempFunc.callback = _TryOnUseItem;
                                    }

                                    funcs.Add(tempFunc);
                                }
                            }
                        }

                        // 副武器穿戴
                        if ((item.UseType == ItemTable.eCanUse.UseOne ||
                            item.UseType == ItemTable.eCanUse.UseTotal)
                            && PlayerBaseData.GetInstance().JobTableID == 15
                            && item.SubType == (int)ProtoTable.ItemTable.eSubType.WEAPON)        //只有剑斗士才生效
                        {
                            if (item.IsCooling() == false)
                            {
                                tempFunc = new TipFuncButon();
                                if (item.PackageType == EPackageType.Equip && item.EquipType != EEquipType.ET_BREATH)
                                {
                                    tempFunc.text = TR.Value("tip_second_Wear");
                                    if (item.HasTransfered)
                                    {
                                        tempFunc.callback = _SecondTryDeTransferClicked;
                                    }
                                    else if (item.Packing)
                                    {
                                        tempFunc.callback = _SecondTryDeSealClicked;
                                    }
                                    else
                                    {
                                        tempFunc.callback = _SecondTryOnUseItem;
                                    }
                                }

                                funcs.Add(tempFunc);
                            }
                        }

                        if (
                            item.UseType == ProtoTable.ItemTable.eCanUse.UseTotal &&
                            item.CD <= 0 &&
                            item.IsCooling() == false &&
                            item.Count > 1
                            )
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_use_total"),
                                callback = _OnUseTotalItem
                            };
                            funcs.Add(tempFunc);
                        }

                        //合成
                        if (item.PackageType == EPackageType.Title)
                        {
                            if (TittleBookManager.GetInstance().CanAsMergeMaterial(item))
                            {
                                tempFunc = new TipFuncButon
                                {
                                    text = TR.Value("tip_title_merge"),
                                    callback = TittleBookManager.GetInstance().OnGotoMerge
                                };
                                funcs.Add(tempFunc);
                            }
                        }

                        // 获取
                        if ((dataItem != null && dataItem.ComeLink != null && dataItem.ComeLink.Count > 0))
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_try_get_item"),
                                callback = _OnTryGetItem
                            };
                            funcs.Add(tempFunc);
                        }

                        if (
                            item.Type == EItemType.EQUIP ||
                            (item.Type == EItemType.EXPENDABLE && item.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard
                            || item.Type == EItemType.EXPENDABLE && item.SubType == (int)ItemTable.eSubType.Bead) ||
                            (item.Type == EItemType.FUCKTITTLE && item.SubType == (int)ItemTable.eSubType.TITLE)
                        )
                        {

                            if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Forge) && !item.isInSidePack && !item.bLocked && !item.IsLease)
                            {
                                tempFunc = new TipFuncButon();
                                if (item.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard && Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Enchant))
                                {
                                    tempFunc.text = TR.Value("tip_enchanting");
                                    tempFunc.name = "Enchanting";
                                    tempFunc.callback = _OnForgeItem;
                                    funcs.Add(tempFunc);
                                }
                                else if ((item.SubType == (int)ItemTable.eSubType.Bead || (item.Type == EItemType.FUCKTITTLE && item.SubType == (int)ItemTable.eSubType.TITLE && item.DeadTimestamp == 0)) && Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Bead))
                                {
                                    tempFunc.text = TR.Value("tip_BeadMosaic");
                                    tempFunc.name = "BeadMosaic";
                                    tempFunc.callback = _OnForgeItem;
                                    funcs.Add(tempFunc);
                                }
                                else if (item.Type == ItemTable.eType.EQUIP && item.SubType != (int)ItemTable.eSubType.ST_BXY_EQUIP)
                                {
                                    tempFunc.text = TR.Value("tip_forge");
                                    tempFunc.name = "Forge";
                                    tempFunc.callback = _OnForgeItem;
                                    funcs.Add(tempFunc);
                                }
                            }
                        }

                        if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Forge) && Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Enchant) && !item.isInSidePack && !item.bLocked && !item.IsLease)
                        {
                            if (item.Type == EItemType.EXPENDABLE && item.SubType == (int)ItemTable.eSubType.EnchantmentsCard)
                            {
                                if (EnchantmentsCardManager.GetInstance().CheckEnchantmentCardIsUpgrade(item))
                                {
                                    tempFunc = new TipFuncButon();
                                    tempFunc.text = TR.Value("tip_BeadUpgrade");
                                    tempFunc.name = "EnchantmentCardUpgrade";
                                    tempFunc.callback = _OnEmchantmentCardUpgradeClick;
                                    funcs.Add(tempFunc);
                                }
                            }
                        }
                        //宝珠升级
                        //if ( item.Type == EItemType.EXPENDABLE && item.SubType == (int)ItemTable.eSubType.Bead)
                        //{
                        //    if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Forge) && Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Bead) && !item.isInSidePack && !item.bLocked && !item.IsLease)
                        //    {
                        //        tempFunc = new TipFuncButon();
                        //        tempFunc.text = TR.Value("tip_BeadUpgrade");
                        //        tempFunc.name = "BeadUpgrade";
                        //        tempFunc.callback = _OnBeadUpgrade;
                        //        funcs.Add(tempFunc);
                        //    }
                        //}

                        //时装合成
                        if (Utility._CheckFashionCanMerge(item.GUID) && item.SuitID != 101139)
                        {
                            if (Utility.IsUnLockFunc((int)ProtoTable.FunctionUnLock.eFuncType.FashionMerge))
                            {
                                if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_FASHION_MERGO) == false)
                                {
                                    funcs.Add(FashionMergeManager.GetInstance().MergeFunction);
                                }
                            }
                        }

                        //属性重选
                        if (item.Type == EItemType.FASHION &&
                            item.SubType != (int)ItemTable.eSubType.FASHION_HAIR)
                        {
                            if (Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.FashionAttrSel))
                            {
                                if (item.HasFashionAttribute)
                                {
                                    tempFunc = new TipFuncButon
                                    {
                                        text = TR.Value("tip_fashion_attr_sel"),
                                        name = "fashion_attr_sel",
                                        callback = _OnFashionAttrSelItem
                                    };
                                    funcs.Add(tempFunc);
                                }
                            }
                        }

                        //卸下副武器
                        if (item.isInSidePack)
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_takeoff"),
                                callback = _UnloadWeapon
                            };
                            funcs.Add(tempFunc);
                        }
                    }

                    if (bStartCountdown == false || nTimeLeft > 0)
                    {
                        //拍卖
                        if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Auction) && !item.bLocked && !item.IsLease
                            && !item.CheckEquipIsMosaicInscription())
                        {
                            List<EPackageType> TypeList = new List<EPackageType>();

                            TypeList.Add(EPackageType.Equip);
                            TypeList.Add(EPackageType.Material);
                            TypeList.Add(EPackageType.Consumable);
                            TypeList.Add(EPackageType.Task);
                            TypeList.Add(EPackageType.Fashion);
                            TypeList.Add(EPackageType.Title);
                            TypeList.Add(EPackageType.Inscription);
                            TypeList.Add(EPackageType.Bxy);
                            TypeList.Add(EPackageType.Sinan);

                            if (ItemDataManager.GetInstance().TradeItemTypeFliter(TypeList, item.PackageType) 
                                && ItemDataManager.GetInstance().TradeItemStateFliter(item) 
                                && !item.isInSidePack
                                && !isItemInUnUsedEquipPlan)
                            {
                                tempFunc = new TipFuncButon
                                {
                                    text = TR.Value("tip_auction"),
                                    name = "Auction",
                                    callback = _OnAuction
                                };
                                funcs.Add(tempFunc);
                            }
                        }
                    }

                    if (item.Type == EItemType.EQUIP)
                    {
                        if (!item.bLocked && !item.IsLease)
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_lock_item"),
                                name = "LockItem",
                                callback = _OnLockItem
                            };
                            funcs.Add(tempFunc);
                        }
                        else
                        {
                            if (!item.IsLease)
                            {
                                tempFunc = new TipFuncButon
                                {
                                    text = TR.Value("tip_unlock_item"),
                                    name = "UnLockItem",
                                    callback = _OnUnLockItem
                                };
                                funcs.Add(tempFunc);
                            }
                        }
                    }
                    else if (item.Type == EItemType.FASHION) // 时装也可以加锁或者解锁
                    {
                        if (item.bFashionItemLocked)
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_unlock_item"),
                                name = "UnLockItem",
                                callback = _OnUnLockItem
                            };
                            funcs.Add(tempFunc);
                        }
                        else
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_lock_item"),
                                name = "LockItem",
                                callback = _OnLockItem
                            };
                            funcs.Add(tempFunc);
                        }
                    }
                    
                    // 出售
                    if (item.CanSell 
                        && !item.bLocked 
                        && !item.IsLease
                        && !isItemInUnUsedEquipPlan)
                    {
                        tempFunc = new TipFuncButon
                        {
                            text = TR.Value("tip_sell"),
                            name = "Sell",
                            callback = _OnSellItem,
                            tipFuncButtonType = TipFuncButtonType.Trigger
                        };
                        funcs.Add(tempFunc);
                    }

                    // 分解
                    if (item.CanDecompose 
                        && !item.bLocked 
                        && !item.IsLease 
                        && !item.bFashionItemLocked
                        && !isItemInUnUsedEquipPlan)
                    {
                        if (item.Type == ItemTable.eType.EQUIP 
                            || item.Type == ItemTable.eType.FASHION && item.DeadTimestamp <= 0 && Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Inscription))
                        {
                            tempFunc = new TipFuncButon
                            {
                                text = TR.Value("tip_decompose"),
                                name = "Decompose",
                                callback = _OnDecomposeClicked,
                                tipFuncButtonType = TipFuncButtonType.Trigger
                            };
                            funcs.Add(tempFunc);
                        }
                    }

                    if (item.Quality > ItemTable.eColor.PURPLE ||
                        (item.Type == ItemTable.eType.FUCKTITTLE && item.Quality > ItemTable.eColor.BLUE))
                    {  // 分享
                        tempFunc = new TipFuncButon
                        {
                            text = TR.Value("tip_share"),
                            name = "Share",
                            callback = _OnShareClicked,
                            tipFuncButtonType = TipFuncButtonType.Trigger
                        };
                        funcs.Add(tempFunc);
                    }
                  
                    if (item.Type == EItemType.EQUIP)
                    {
                        // 更多
                        tempFunc = new TipFuncButtonOther
                        {
                            text = TR.Value("tip_more_and_more"),
                            name = "More",
                            callback = _OnMoreAndMoreClick,
                            tipFuncButtonType = TipFuncButtonType.Other
                        };
                        funcs.Add(tempFunc);
                    }
                    
                }
            }

            ItemData compareItem = _GetCompareItem(item);
            if (compareItem != null)
            {
                ItemTipManager.GetInstance().ShowTipWithCompareItem(item, compareItem, funcs);
            }
            else
            {
                ItemTipManager.GetInstance().ShowTip(item, funcs, TextAnchor.MiddleLeft);
            }
        }


        private void _OnSendGiftCick(ItemData item, object param1)
        {
            ItemTipManager.GetInstance().CloseTip(0);
            ClientSystemManager.GetInstance().OpenFrame<SendGiftFrame>(FrameLayer.Middle,item);  
        }

        void _OnAuction(ItemData item, object data)
        {
            AuctionNewUtility.OpenAuctionNewFrame(item);
        }

        void _OnLockItem(ItemData item, object data)
        {

            SceneItemLockReq req = new SceneItemLockReq();
            req.itemUid = item.GUID;
            req.opType = 1;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            ItemTipManager.GetInstance().CloseAll();
        }

        void _OnMoreAndMoreClick(ItemData item , object data)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoreAndMoreBtnHandle);
            DoStartFrameOperation("ItemTipsFrame", "MoreAndMore_3");
        }
        void _OnUnLockItem(ItemData item, object data)
        {
            if(SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }
            SceneItemLockReq req = new SceneItemLockReq();
            req.itemUid = item.GUID;
            req.opType = 0;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
            ItemTipManager.GetInstance().CloseAll();
        }
        ItemData _GetCompareItem(ItemData item)
        {
            ItemData compareItem = null;
            if (item != null && item.WillCanEquip())
            {
                List<ulong> guids = null;
                if (item.PackageType == EPackageType.Equip)
                {
                    guids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                }
                else if (item.PackageType == EPackageType.Fashion)
                {
                    guids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
                }
                if (guids != null)
                {
                    for (int i = 0; i < guids.Count; ++i)
                    {
                        ItemData tempItem = ItemDataManager.GetInstance().GetItem(guids[i]);
                        if (
                            tempItem != null &&
                            tempItem.GUID != item.GUID &&
                            tempItem.IsWearSoltEqual(item)
                            )
                        {
                            compareItem = tempItem;
                            break;
                        }
                    }
                }
            }
            return compareItem;
        }

        void _OnItemRenewal(ItemData item, object data)
        {
            ClientSystemManager.GetInstance().OpenFrame<RenewalItemFrame>(FrameLayer.Middle, item);
            ItemTipManager.GetInstance().CloseAll();
        }

        void _OnUnWear(ItemData item, object data)
        {
            if (item != null)
            {
                ItemDataManager.GetInstance().UseItem(item);
                AudioManager.instance.PlaySound(103);
                ItemTipManager.GetInstance().CloseAll();

                DoStartFrameOperation("ItemTipsFrame", "UnWear_3");
            }
        }

        ItemData Tempitem = null;
        void _TryOnThrowOffItem(ItemData item, object data)
        {
            Tempitem = item;
            SystemNotifyManager.SysNotifyMsgBoxOkCancel("丢弃后装备将被删除，是否确定丢弃？", _OnClickThrowOffOk);
        }

        void _OnClickThrowOffOk()
        {
            if (Tempitem != null)
            {
                ChijiDataManager.GetInstance().SendDelItemReq(Tempitem.GUID);
                AudioManager.instance.PlaySound(103);
                ItemTipManager.GetInstance().CloseAll();
            }
        }

        void _OnTakeOffAllFashion(ItemData a_item , object a_data)
        {
            var equipmentRelationTableData = TableManager.GetInstance().GetTableItem<EquipmentRelationTable>(a_item.TableID);
            if (equipmentRelationTableData == null)
            {
                return;
            }
            var thisItemType = equipmentRelationTableData.ItemType;
            var thisSubType = equipmentRelationTableData.SubType;
            Dictionary<int, ItemData> itemDic = new Dictionary<int, ItemData>();
            itemDic.Clear();
            itemDic[(int)thisSubType] = ItemDataManager.GetInstance().GetItem(a_item.GUID);
            var equipmentRelationData = TableManager.GetInstance().GetTable<EquipmentRelationTable>();
            var enumeratorEquipmentRelation = equipmentRelationData.GetEnumerator();
            while (enumeratorEquipmentRelation.MoveNext())
            {
                var equipmentRelationItem = enumeratorEquipmentRelation.Current.Value as EquipmentRelationTable;
                if (equipmentRelationItem.SubType != thisSubType && equipmentRelationItem.ItemType == thisItemType)
                {
                    int tempSubTypeID = (int)equipmentRelationItem.SubType;
                    int tempPriority = equipmentRelationItem.Priority;
                    ulong tempguid = ItemDataManager.GetInstance().GetItemGUIDForType(equipmentRelationItem.ID, EPackageType.WearFashion);
                    if (tempguid > 0)
                    {
                        itemDic[tempSubTypeID] = ItemDataManager.GetInstance().GetItem(tempguid);
                    }
                }
            }
            foreach (var item in itemDic)
            {
                ItemDataManager.GetInstance().UseItem(item.Value);
            }
            AudioManager.instance.PlaySound(103);
            ItemTipManager.GetInstance().CloseAll();
        }

        bool _CanAllUse(int tableID)
        {
            var tableData = TableManager.GetInstance().GetTableItem<EquipmentRelationTable>(tableID);
            if(tableData == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        void _OnStrengthenItem(ItemData a_item, object a_data)
        {
            if (a_item != null)
            {
                SmithShopNewLinkData data = new SmithShopNewLinkData
                {
                    itemData = a_item,
                    iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_STRENGTHEN
                };

                OpenTargetFrame<SmithShopNewFrame>(FrameLayer.Middle, data);

                ItemTipManager.GetInstance().CloseAll();
            }
        }

        void _OnEnchantingItem(ItemData a_item, object a_data)
        {
            if (a_item != null)
            {
                SmithShopNewLinkData data = new SmithShopNewLinkData
                {
                    itemData = a_item,
                    iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_ENCHANTMENTCARD,
                    iDefaultSecondTabId = (int)EnchantmentCardSubTabType.ECSTT_EQUIPMENTENCHANT
                };

                OpenTargetFrame<SmithShopNewFrame>(FrameLayer.Middle, data);

                ItemTipManager.GetInstance().CloseAll();
            }
        }

        void _OnAdjustItemGrade(ItemData a_item, object a_data)
        {
            if (a_item != null)
            {
                SmithShopNewLinkData data = new SmithShopNewLinkData
                {
                    itemData = a_item,
                    iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_ADJUST
                };

                OpenTargetFrame<SmithShopNewFrame>(FrameLayer.Middle, data);

                ItemTipManager.GetInstance().CloseAll();
            }
        }

        void _OnFashionAttrSelItem(ItemData a_item, object a_data)
        {
            if (null != a_item)
            {
                FashionSmithShopFrame.OpenLinkFrame(string.Format("0_{0}", a_item.GUID));
                ItemTipManager.GetInstance().CloseAll();
            }
        }

        void _OnWearAllFashion(ItemData a_item, object a_data)
        {
            if(null != a_item)
            {
                //_TryOnUseItem
                var equipmentRelationTableData = TableManager.GetInstance().GetTableItem<EquipmentRelationTable>(a_item.TableID);
                if(equipmentRelationTableData == null)
                {
                    return;
                }
                var thisItemType = equipmentRelationTableData.ItemType;
                var thisOccu = equipmentRelationTableData.Occu;
                var thisSubType = equipmentRelationTableData.SubType;
                Dictionary<int, ItemData> itemDic = new Dictionary<int, ItemData>();
                Dictionary<int, int> itemPriority = new Dictionary<int, int>();
                itemDic.Clear();
                itemPriority.Clear();
                itemDic[(int)thisSubType] = ItemDataManager.GetInstance().GetItem(a_item.GUID);
                var equipmentRelationData = TableManager.GetInstance().GetTable<EquipmentRelationTable>();
                var enumeratorEquipmentRelation = equipmentRelationData.GetEnumerator();
                while(enumeratorEquipmentRelation.MoveNext())
                {
                    var equipmentRelationItem = enumeratorEquipmentRelation.Current.Value as EquipmentRelationTable;
                    if (equipmentRelationItem.SubType == thisSubType || equipmentRelationItem.ItemType != thisItemType)
                    {
                        continue;
                    }
                    int tempSubTypeID = (int)equipmentRelationItem.SubType;
                    int tempPriority = equipmentRelationItem.Priority;
                    if (!(!itemPriority.ContainsKey(tempSubTypeID) || itemPriority[tempSubTypeID] < tempPriority))
                    {
                        continue;
                    }
                    ulong tempguid = ItemDataManager.GetInstance().GetItemGUIDForType(equipmentRelationItem.ID, EPackageType.Fashion);
                    if (tempguid <= 0)
                    {
                        continue;
                    }
                    int equipFashionPriority = ItemDataManager.GetInstance().GetEqualFashionPriority((int)equipmentRelationItem.ItemType, (int)equipmentRelationItem.SubType);


                    if (tempPriority > equipFashionPriority || equipFashionPriority == 0)
                    {
                        var tempItemData = ItemDataManager.GetInstance().GetItem(tempguid);
                        if (tempItemData == null)
                        {
                            continue;
                        }
                        if (tempItemData.OccupationLimit[0] != Utility.GetBaseJobID(PlayerBaseData.GetInstance().JobTableID))
                        {
                            continue;
                        }
                        itemPriority[tempSubTypeID] = tempPriority;
                        itemDic[tempSubTypeID] = ItemDataManager.GetInstance().GetItem(tempguid);
                    }

                }
                foreach(var item in itemDic)
                {
                    //ItemDataManager.GetInstance().UseItem(item.Value);
                    _OnUseItem(item.Value, null,false);
                }
            }
        }

        void _OnUseMagicBox(ItemData item, object data)
        {
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);
            if (itemGuids != null)
            {
                int index = 0;
                for (int i = 0; i < itemGuids.Count; i++)
                {
                    ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                    if (itemData == null)
                    {
                        continue;
                    }

                    if (itemData.SubType != (int)ItemTable.eSubType.MagicHammer)
                    {
                        continue;
                    }
                    else
                    {
                        //index = itemData.Count;
                        index = ItemDataManager.GetInstance().GetItemCountInPackage(itemData.TableID);
                    }
                }

                if (index >= 4)
                {
                    //向服务器发送协议
                    MagicBoxDataManager.GetInstance().AnsyOpenMagBox(OpenMagicBoxFrame, item.GUID, 1);
                }
                else
                {
                    ItemComeLink.OnLink(800002002, 0);
                }

            }
            ItemTipManager.GetInstance().CloseAll();
        }

        void _OnUseMagicHammer(ItemData item, object data)
        {
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);
            if (itemGuids != null)
            {
                int magicBoxIndex = 0;
                for (int i = 0; i < itemGuids.Count; i++)
                {
                    ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                    if (itemData == null)
                    {
                        continue;
                    }

                    if (itemData.SubType != (int)ItemTable.eSubType.MagicBox)
                    {
                        continue;
                    }
                    else
                    {
                        magicBoxIndex = itemData.Count;
                    }

                }

                if (magicBoxIndex >= 1)
                {
                    int magicHammer = ItemDataManager.GetInstance().GetItemCountInPackage(item.TableID);
                    //int magicHammer = 0;
                    //for (int i = 0; i < itemGuids.Count; i++)
                    //{
                    //    ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                    //    if (itemData == null)
                    //    {
                    //        continue;
                    //    }

                    //    if (itemData.SubType != (int)ItemTable.eSubType.MagicHammer)
                    //    {
                    //        continue;
                    //    }
                    //    else
                    //    {
                    //        magicHammer = itemData.Count;
                    //    }
                    //}

                    if (magicHammer >= 4)
                    {
                        //向服务器发送协议  
                        MagicBoxDataManager.GetInstance().AnsyOpenMagBox(OpenMagicBoxFrame, item.GUID, 1);
                    }
                    else
                    {
                        ItemComeLink.OnLink(800002002, 0);
                    }

                }
                else
                {
                    ItemComeLink.OnLink(800002001, 0);
                }

            }
            ItemTipManager.GetInstance().CloseAll();

        }
        void OpenMagicBoxFrame()
        {
            MagicBoxFrame.MagicBoxResultFrameData data = new MagicBoxFrame.MagicBoxResultFrameData
            {
                itemRewards = MagicBoxDataManager.GetInstance().itemRrewardList,
                ItemDoubleRewards = MagicBoxDataManager.GetInstance().itemDoubleRewardList
            };

            if (ClientSystemManager.GetInstance().IsFrameOpen<MagicBoxFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MagicBoxFrame>();
            }

            ClientFrame.OpenTargetFrame<MagicBoxFrame>(FrameLayer.Middle, data);

            PackageDataManager.GetInstance().ResetMagicBoxAndMagicHammer();
        }

        void _OnUseExtendRoleFieldCard(ItemData a_item, object data)
        {
            if (a_item == null)
            {
                return;
            }
            AdventureTeamDataManager.GetInstance().ReqExtendRoleFieldUnlock(a_item.GUID, a_item.TableID);
            ItemTipManager.GetInstance().CloseAll();
        }

        void _TryOnUseItem(ItemData item, object data)
        {
            _RealTryOnUseItem(item,data);
        }

        //副武器穿戴
        void _SecondTryOnUseItem(ItemData item, object data)
        {
            _RealTryOnUseItem(item, data, true);
        }

        void _RealTryOnUseItem(ItemData item, object data,bool isSecondWear = false)
        {
            if (item.Type == EItemType.EQUIP)
            {
                int iEquipedMasterPriority = EquipMasterDataManager.GetInstance().GetMasterPriority(PlayerBaseData.GetInstance().JobTableID, (int)item.Quality, (int)item.EquipWearSlotType, (int)item.ThirdType);
                if(iEquipedMasterPriority == 2)
                {
                    SystemNotifyManager.SystemNotifyOkCancel(7019,
                        () =>
                        {
                            _OnUseItem(item, data, isSecondWear);
                        },
                        null);
                    return;
                }
            }

            // 吃鸡满血不使用血瓶
            if (item.ThirdType == ItemTable.eThirdType.UseToSelf)
            {
                if (PlayerBaseData.GetInstance().Chiji_HP_Percent >= 1.0f)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("你的血量已满，无需补充！");
                    return;
                }
            }

            //虚空通行证（3101000：虚空地下城普通ID）
            if (item.ThirdType == ItemTable.eThirdType.VoidCrackTicket)
            {
                var finishedTimes = DungeonUtility.GetDungeonDailyFinishedTimes(3101000);
                var dailyMaxTime = DungeonUtility.GetDungeonDailyMaxTimes(3101000);
                var leftTimes = dailyMaxTime - finishedTimes;
                if (leftTimes < 0)
                    leftTimes = 0;

                //虚空地下城基础次数
                int dungeonDailyBaseTimes = DungeonUtility.GetDungeonDailyBaseTimes(3101000);

                //当前虚空挑战次数大于等于基础次数
                if (leftTimes >= dungeonDailyBaseTimes)
                {
                    if (ItemDataManager.GetInstance().bUseVoidCrackTicketIsPlayPrompt == false)
                    {
                        CommonMsgBoxOkCancelNewParamData comconMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData();
                        comconMsgBoxOkCancelParamData.ContentLabel = TR.Value("item_use_vanity_pass_desc", dungeonDailyBaseTimes, item.GetColorName());
                        comconMsgBoxOkCancelParamData.IsShowNotify = true;
                        comconMsgBoxOkCancelParamData.OnCommonMsgBoxToggleClick = OnUpdateseVoidCrackTicketIsPlayPrompt;
                        comconMsgBoxOkCancelParamData.LeftButtonText = TR.Value("common_data_cancel");
                        comconMsgBoxOkCancelParamData.RightButtonText = TR.Value("common_data_sure_2");
                        comconMsgBoxOkCancelParamData.OnRightButtonClickCallBack +=()=> { _OnUseItem(item, data, isSecondWear); };

                        SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(comconMsgBoxOkCancelParamData);

                        return;
                    }
                }
            }

            _OnUseItem(item, data, isSecondWear);
        }

        void _OnUseItem(ItemData item, object data,bool isSecondWear)
        {
            if (item != null)
            {
                int secondWear = isSecondWear ? 1 : 0;
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
                            ItemDataManager.GetInstance().UseItem(item, false, secondWear);
                            if (item.Count <= 1 || item.CD > 0)
                            {
                                ItemTipManager.GetInstance().CloseAll();
                            }
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("道具{0}的礼包ID{1}不存在", item.TableID, item.PackID);
                    }
                }
                else
                {
                    ItemDataManager.GetInstance().UseItem(item, false, secondWear);

                    if (item.PackageType == EPackageType.Equip || item.PackageType == EPackageType.Fashion)
                    {
                        AudioManager.instance.PlaySound(102);
                    }

                    if (item.Count <= 1 || item.CD > 0)
                    {
                        ItemTipManager.GetInstance().CloseAll();
                    }
                }
            }
        }

        private void OnUpdateseVoidCrackTicketIsPlayPrompt(bool value)
        {
            ItemDataManager.GetInstance().bUseVoidCrackTicketIsPlayPrompt = value;
        }

        void _OnUseChangeName(ItemData a_item, object a_data)
        {
            if (a_item == null)
            {
                return;
            }

            if (a_item.ThirdType == ItemTable.eThirdType.ChangeGuildName)
            {
                GuildCommonModifyData data = new GuildCommonModifyData
                {
                    bHasCost = false,
                    nMaxWords = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_NAME_MAX_WORDS).Value,
                    onOkClicked = (string a_strValue) =>
                    {
                        GuildDataManager.GetInstance().ChangeName((uint)a_item.TableID, a_item.GUID, a_strValue);
                    },
                    strTitle = TR.Value("guild_change_name"),
                    strEmptyDesc = TR.Value("guild_change_name_desc"),
                    strDefultContent = GuildDataManager.GetInstance().HasSelfGuild() ? GuildDataManager.GetInstance().myGuild.strName : string.Empty,
                    eMode = EGuildCommonModifyMode.Short
                };
                frameMgr.OpenFrame<GuildCommonModifyFrame>(FrameLayer.Middle, data);
            }
            else if (a_item.ThirdType == ItemTable.eThirdType.ChangePlayerName)
            {
                GuildCommonModifyData data = new GuildCommonModifyData
                {
                    bHasCost = false,
                    nMaxWords = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_NAME_MAX_WORDS).Value,
                    onOkClicked = (string a_strValue) =>
                    {
                        PlayerBaseData.GetInstance().CheckNameValid(a_item.GUID, a_strValue);
                    },
                    strTitle = TR.Value("player_change_name"),
                    strEmptyDesc = TR.Value("player_change_name_desc"),
                    strDefultContent = PlayerBaseData.GetInstance().Name,
                    eMode = EGuildCommonModifyMode.Short
                };
                frameMgr.OpenFrame<GuildCommonModifyFrame>(FrameLayer.Middle, data);
            }
            else if (a_item.ThirdType == ItemTable.eThirdType.ChangeAdventureName)
            {
                AdventureTeamRenameModel model = new AdventureTeamRenameModel{
                    renameItemGUID = a_item.GUID,
                    renameItemTableId = (uint)a_item.TableID
                };
                frameMgr.OpenFrame<AdventureTeamChangeNameFrame>(FrameLayer.Middle, model);
            }
            
            ItemTipManager.GetInstance().CloseAll();
        }

        void _OnUseTotalItem(ItemData item, object data)
        {
            if (item != null)
            {
                if (item.SubType ==(int)ItemTable.eSubType.MagicBox)
                {
                    List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);
                    if (itemGuids != null)
                    {
                        int magicHammer = 0;
                        for (int i = 0; i < itemGuids.Count; i++)
                        {
                            ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                            if (itemData ==null )
                            {
                                continue;
                            }

                            if (itemData.SubType != (int)ItemTable.eSubType.MagicHammer)
                            {
                                continue;
                            }
                            else
                            {
                                int num= ItemDataManager.GetInstance().GetItemCountInPackage(itemData.TableID);
                                magicHammer = Mathf.FloorToInt(num / 4);
                            }
                                   
                        }

                        if (magicHammer >= item.Count)
                        {
                            //向服务器发送协议
                            MagicBoxDataManager.GetInstance().AnsyOpenMagBox(OpenMagicBoxFrame, item.GUID, item.Count);
                            PackageDataManager.GetInstance().UsingMagicBoxAndMagicHammer();
                        }
                        else if (magicHammer < item.Count && magicHammer != 0)
                        {
                            MagicBoxDataManager.GetInstance().AnsyOpenMagBox(OpenMagicBoxFrame, item.GUID, magicHammer);
                            PackageDataManager.GetInstance().UsingMagicBoxAndMagicHammer();
                        }
                        else
                        {
                            ItemComeLink.OnLink(800002002, 0);
                        }
                    }
                    ItemTipManager.GetInstance().CloseAll();
                }
                else if(item.SubType==(int)ItemTable.eSubType.MagicHammer)
                {
                    List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);
                    if (itemGuids != null)
                    {
                        int magicBox = 0;
                        for (int i = 0; i < itemGuids.Count; i++)
                        {
                            ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                            if (itemData == null)
                            {
                                continue;
                            }

                            if (itemData.SubType != (int)ItemTable.eSubType.MagicBox)
                            {
                                continue;
                            }
                            else
                            {
                                magicBox = itemData.Count;
                            }

                        }

                        if (magicBox > 0)
                        {
                            int count = ItemDataManager.GetInstance().GetItemCountInPackage(item.TableID);
                            int num = Mathf.FloorToInt(count / 4);

                            if (num >= magicBox)
                            {
                                //向服务器发送协议
                                MagicBoxDataManager.GetInstance().AnsyOpenMagBox(OpenMagicBoxFrame, item.GUID, magicBox);
                                PackageDataManager.GetInstance().UsingMagicBoxAndMagicHammer();
                            }
                            else if (num < magicBox && num != 0)
                            {
                                MagicBoxDataManager.GetInstance().AnsyOpenMagBox(OpenMagicBoxFrame, item.GUID, num);
                                PackageDataManager.GetInstance().UsingMagicBoxAndMagicHammer();
                            }
                            else
                            {
                                ItemComeLink.OnLink(800002002, 0);
                            }
                        }
                        else
                        {
                            ItemComeLink.OnLink(800002001, 0);
                        }
                        
                    }
                    ItemTipManager.GetInstance().CloseAll();
                }
                else
                {
                    ItemDataManager.GetInstance().UseItem(item, true);
                    ItemTipManager.GetInstance().CloseAll();
                }
               
            }
        }

        void _OnTryGetItem(ItemData item, object data)
        {
            if (item != null)
            {
                var dataItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)item.TableID);
                ItemComeLink.OnLink(item.TableID, 0, false, null, false, dataItem.bNeedJump > 0);
                ItemTipManager.GetInstance().CloseAll();
            }
        }

        void _OnForgeItem(ItemData a_item, object a_data)
        {
            if (a_item != null)
            {
                SmithShopNewLinkData data = new SmithShopNewLinkData
                {
                    itemData = a_item
                };

                if (a_item.SubType == (int)ItemTable.eSubType.EnchantmentsCard)
                {
                    data.iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_ENCHANTMENTCARD;
                }
                else if (a_item.SubType == (int)ItemTable.eSubType.Bead || a_item.SubType == (int)ItemTable.eSubType.TITLE)
                {
                    data.iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_BEAD;
                }
                else
                {
                    data.iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_STRENGTHEN;
                }

                PlayerBaseData.GetInstance().IsSelectedPerfectWashingRollTab = false;

                ClientSystemManager.GetInstance().CloseFrame<SmithShopNewFrame>(null, true);
                ClientSystemManager.GetInstance().OpenFrame<SmithShopNewFrame>(FrameLayer.Middle, data);
                ItemTipManager.GetInstance().CloseAll();

                if (a_item.Type == ItemTable.eType.EQUIP)
                {
                    DoStartFrameOperation("ItemTipsFrame", "Forge_3");
                }
            }
        }
        void _OnEmchantmentCardUpgradeClick(ItemData a_item, object a_data)
        {
            if (a_item != null)
            {
                ClientSystemManager.GetInstance().OpenFrame<EnchantmentCardUpgradeFrame>(FrameLayer.Middle);
                ItemTipManager.GetInstance().CloseAll();
            }
        }

        void _OnBeadUpgrade(ItemData a_item,object a_data)
        {
            if (a_item != null)
            {
                SmithShopNewLinkData data = new SmithShopNewLinkData
                {
                    itemData = a_item,
                    iDefaultFirstTabId = (int)SmithShopNewTabType.SSNIT_BEADUPGRADE,
                };

                ClientSystemManager.GetInstance().CloseFrame<SmithShopNewFrame>(null, true);
                ClientSystemManager.GetInstance().OpenFrame<SmithShopNewFrame>(FrameLayer.Middle, data);
                ItemTipManager.GetInstance().CloseAll();
            }
        }

        private void _OnSellItemInChijiScene(ItemData item, object data)
        {
            ChijiShopUtility.OnSellItemInChijiScene(item);
        }

        void _OnSellItem(ItemData item, object data)
        {
            if (item != null)
            {
                // 加锁了的时装无法出售
                if(item.Type == EItemType.FASHION && item.bFashionItemLocked)
                {
                    SystemNotifyManager.SystemNotify(10008);
                    return;
                }

                if (SecurityLockDataManager.GetInstance().CheckSecurityLock(() =>
                {
                    return (item.Quality >= EItemQuality.PURPLE);
                }))
                {
                    return;
                }

                frameMgr.OpenFrame<SellItemFrame>(FrameLayer.Middle, item);

                DoStartFrameOperation("ItemTipsFrame", "Sell_3");
            }
        }
        
        void _UnloadWeapon(ItemData item, object data)
        {
            SwitchWeaponDataManager.GetInstance().TakeOnSideWeapon(1, 0);
            ItemTipManager.GetInstance().CloseAll();
        }

        void _OnShareClicked(ItemData item, object data)
        {
            ChatManager.GetInstance().ShareEquipment(item);
        }

        void _TryDeTransferClicked(ItemData item, object data)
        {
            _RealTryDeTransferClicked(item,data);
        }

        //对应副武器穿戴
        void _SecondTryDeTransferClicked(ItemData item,object data)
        {
            _RealTryDeTransferClicked(item, data);
        }

        void _RealTryDeTransferClicked(ItemData item, object data,bool isSecondWear = false)
        {
            if (null == item)
            {
                return ;
            }
            int secondWear = isSecondWear ? 1 : 0;
            SystemNotifyManager.SystemNotify(9066,
                () =>
                {
                    ItemDataManager.GetInstance().UseItem(item, false, secondWear);
                    AudioManager.instance.PlaySound(102);
                    ItemTipManager.GetInstance().CloseAll();
                },
                null,
                item.GetColorName()
            );
        }

        void _TryDeSealClicked(ItemData item, object data)
        {
            _RealTryDeSealClicked(item,data);
        }

        void _SecondTryDeSealClicked(ItemData item, object data)
        {
            _RealTryDeSealClicked(item, data,true);
        }

        void _RealTryDeSealClicked(ItemData item, object data,bool isSecondWear = false)
        {
            if (item.Type == EItemType.EQUIP && item.PackageType == EPackageType.Equip)
            {
                int iEquipedMasterPriority = EquipMasterDataManager.GetInstance().GetMasterPriority(PlayerBaseData.GetInstance().JobTableID, (int)item.Quality, (int)item.EquipWearSlotType, (int)item.ThirdType);
                if (iEquipedMasterPriority == 2)
                {
                    SystemNotifyManager.SystemNotifyOkCancel(7019,
                        () =>
                        {
                            _OnDeSealClicked(item, data,isSecondWear);
                        },
                        null);
                    return;
                }
            }

            _OnDeSealClicked(item, data, isSecondWear);
        }

        void _OnDeSealClicked(ItemData item, object data, bool isSecondWear = false)
        {
            if (item != null && item.Packing == true)
            {
                if (item.CanEquip())
                {
                    int secondWear = isSecondWear ? 1 : 0;
                    SystemNotifyManager.SystemNotify(2006,
                        () =>
                        {
                            ItemDataManager.GetInstance().UseItem(item,false, secondWear);
                            AudioManager.instance.PlaySound(102);
                            ItemTipManager.GetInstance().CloseAll();
                        },
                        null,
                        item.GetColorName()
                        );
                }
                else
                {
                    SystemNotifyManager.SysNotifyMsgBoxOK(TR.Value("equip_deseal_notify_cannot", item.GetColorName()));
                }
            }
        }

        void _OnDecomposeClicked(ItemData item, object data)
        {
            if (item != null && item.CanDecompose)
            {
                if(SecurityLockDataManager.GetInstance().CheckSecurityLock(() => 
                {
                    if (item.Quality >= EItemQuality.PURPLE)
                    {
                        return true;
                    }
                    return false;
                }))
                {
                    return;
                }

                if(item.Type == EItemType.FASHION)
                {
                    ItemTipManager.GetInstance().CloseAll();
                    _OpenFashionDecompose(new List<ulong>() { item.GUID});
                    return;
                }

                _DecomposeEquip(() =>
                {
                    ItemTipManager.GetInstance().CloseAll();
                }, item);

                DoStartFrameOperation("ItemTipsFrame", "Decompose_3");
            }
        }

        void _OnItemLink(ItemData item, object data)
        {
            //这里做换装券的特殊处理
            if(item.TableID == 330000201 && !ActivityManager.GetInstance().IsActivityOpen(10000))
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("cannot_use_fashion_ticket"));
                return;
            }
            var dataItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)item.TableID);
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.LinkInfo))
            {
                var FuncUnlockdata = TableManager.GetInstance().GetTableItem<FunctionUnLock>(dataItem.FunctionID);
                if (FuncUnlockdata != null && FuncUnlockdata.FinishLevel > PlayerBaseData.GetInstance().Level)
                {
                    SystemNotifyManager.SystemNotify(FuncUnlockdata.CommDescID);
                    return;
                }

                //如果选中的是铭文
                if (dataItem.Type == ItemTable.eType.ITEM_INSCRIPTION)
                {
                    //判断锻冶是否打开，如果打开了弹提示
                    if (ClientSystemManager.GetInstance().IsFrameOpen<SmithShopNewFrame>())
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Inscriptions_Purposes_Desc"));
                        return;
                    }
                }

                if (dataItem.Type == ItemTable.eType.MATERIAL && dataItem.SubType == ItemTable.eSubType.Perfect_washing)
                {
                    PlayerBaseData.GetInstance().IsSelectedPerfectWashingRollTab = true;
                }
                else
                {
                    PlayerBaseData.GetInstance().IsSelectedPerfectWashingRollTab = false;
                }

                ActiveManager.GetInstance().OnClickLinkInfo(dataItem.LinkInfo);
                ItemTipManager.GetInstance().CloseAll();
            }
        }

        void _InitPackageFullTips()
        {
            bool isFlag = _checkIsFullByType(EPackageType.Equip);

            if (isFlag)
            {
                DOTweenAnimation[] EquipHandBookTipsdotween = mPackageFulltips.GetComponents<DOTweenAnimation>();
                DOTweenAnimation[] EquipHandBookTipsTextdotween = mPackageFulText.GetComponents<DOTweenAnimation>();

                for (int i = 0; i < EquipHandBookTipsdotween.Length; i++)
                {
                    EquipHandBookTipsdotween[i].DORestart();
                    EquipHandBookTipsTextdotween[i].DORestart();
                }
                
                _SetPackageFullTipActive(true);
            }
            else
            {
                _SetPackageFullTipActive(false);
            }
        }
        private bool _checkIsFullByType(EPackageType type)
        {
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(type);
            int iCount = 0;
            if (null != itemGuids)
            {
                iCount = itemGuids.Count;
            }

            if (PlayerBaseData.GetInstance().PackTotalSize.Count > (int)type)
            {
                return PlayerBaseData.GetInstance().PackTotalSize[(int)type] <= iCount;
            }

            return false;
        }

        void _SetPackageFullTipActive(bool bActive)
        {
            if (bActive)
            {
                mPackageFulltips.gameObject.CustomActive(true);
            }
            else
            {
                mPackageFulltips.gameObject.CustomActive(false);
            }
        }

        #region ui event
        protected void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AvatarChanged, _OnAvatarChagned);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemsAttrChanged, _OnItemAttrChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStrengthenSuccess, _OnItemStrengthFinished);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStrengthenFail, _OnItemStrengthFinished);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ExpChanged, _OnExpChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffListChanged, _OnBuffChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffRemoved, _OnBuffChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NameChanged, _OnNameChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SwitchEquipSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemTakeSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStoreSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemSellSuccess, _OnItemSellSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyGet, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyRemoved, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemQualityChanged, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DecomposeFinished, _OnItemDecomposeFinished);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PackageSwitch2OneGroup, _OnPackageSwitch2OneGroup);


            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JobIDChanged, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnlyUpdateItemList, _OnOnlyUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshPackageProperty, _OnRefreshPackageProperty);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnIsShowFashionWeapon, OnIsShowFashionWeapon);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TitleNameUpdate, OnTitleNameUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetPropertyReselect, OnPetPropertyReselect);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CloseFashionEquipDecompose, OnCloseFashionEquipDecompose);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateEquipmentScore, OnUpdateEquipmentScore);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveEquipPlanItemEndTimeMessage,
                OnReceiveEquipPlanItemEndTimeMessage);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdatePackageGrids, _OnUpdateGrids);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NewFuncUnlock, _OnFuncUnlock);
            
        }

        //功能解锁
        private void _OnFuncUnlock(UIEvent uiEvent)
        {
            if (null != mActorShow)
                mActorShow.UpdatePvpIconShow();
        }

        protected void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AvatarChanged, _OnAvatarChagned);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SwitchEquipSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemsAttrChanged, _OnItemAttrChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStrengthenSuccess, _OnItemStrengthFinished);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStrengthenFail, _OnItemStrengthFinished);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ExpChanged, _OnExpChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffListChanged, _OnBuffChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffRemoved, _OnBuffChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NameChanged, _OnNameChanged);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemTakeSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStoreSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemSellSuccess, _OnItemSellSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyGet, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyRemoved, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemQualityChanged, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DecomposeFinished, _OnItemDecomposeFinished);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JobIDChanged, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnlyUpdateItemList, _OnOnlyUpdateItemList);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PackageSwitch2OneGroup, _OnPackageSwitch2OneGroup);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshPackageProperty, _OnRefreshPackageProperty);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnIsShowFashionWeapon, OnIsShowFashionWeapon);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TitleNameUpdate, OnTitleNameUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetPropertyReselect, OnPetPropertyReselect);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CloseFashionEquipDecompose, OnCloseFashionEquipDecompose);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateEquipmentScore, OnUpdateEquipmentScore);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveEquipPlanItemEndTimeMessage,
                OnReceiveEquipPlanItemEndTimeMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdatePackageGrids, _OnUpdateGrids);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NewFuncUnlock, _OnFuncUnlock);
        }
        void _OnRedPointChanged(UIEvent uiEvent)
        {
            _UpdateFashionRedPoint();
        }
        void _UpdateFashionRedPoint()
        {
            m_goFashionRePoint.CustomActive(ItemDataManager.GetInstance().IsPackageHasNew(EPackageType.Fashion) || ItemDataManager.GetInstance().IsPackageHasNew(EPackageType.Inscription));
        }
        void _OnItemAttrChanged(UIEvent uiEvent)
        {
            List<ItemData> arrItemDatas = (List<ItemData>)uiEvent.Param1;
            if (arrItemDatas == null)
            {
                return;
            }

            bool bNeedUpdate = false;
            for (int i = 0; i < arrItemDatas.Count; ++i)
            {
                ItemData tempItemData = arrItemDatas[i];
                if (tempItemData.PackageType == EPackageType.WearEquip || tempItemData.PackageType == EPackageType.WearFashion)
                {
                    bNeedUpdate = true;
                    break;
                }
            }

            if (bNeedUpdate)
            {
                //_RefreshModel();
                _RefreshDetailAttrs();
            }

            if (mActorShow != null)
            {
                mActorShow.RefreshEquips();
                mActorShow.RefreshFashionEquips();
            }
        }

        void _OnRefreshPackageProperty(UIEvent a_event)
        {
            _RefreshDetailAttrs();
        }
        void _OnItemStrengthFinished(UIEvent a_event)
        {
            if (mActorShow != null)
            {
                mActorShow.RefreshEquips();
                mActorShow.RefreshFashionEquips();
            }
            //_RefreshModel();
            _RefreshDetailAttrs();

            _OnUpdateItemList(a_event);
        }

        private void OnUpdateEquipmentScore(UIEvent uiEvent)
        {
            mDetailView?.RefreshEquipScore();
        }

        void _OnExpChanged(UIEvent uiEvent)
        {
        }

        void _OnBuffChanged(UIEvent a_event)
        {
            _RefreshDetailAttrs();
        }

        void _OnNameChanged(UIEvent uiEvent)
        {
            mActorShow?.RefreshBaseInfo();
        }

        void _OnItemDecomposeFinished(UIEvent a_event)
        {
            _ClearSelectState();
            _RefreshItemTab();
            _RefreshItemList();
        }

        void _OnItemSellSuccess(UIEvent a_event)
        {
            //SystemNotifyManager.SysNotifyTextAnimation(TR.Value("package_sell_item_success"));
            _RefreshItemTab();
            _RefreshItemList();
        }

        void _OnOnlyUpdateItemList(UIEvent a_event)
        {
            _RefreshItemTab();
            _RefreshItemList();
        }
        void _OnUpdateItemList(UIEvent a_event)
        {
            _RefreshItemTab();
            _RefreshItemList();
            mActorShow?.RefreshEquips();
            //bool bFlag = ChijiDataManager.GetInstance().CheckCurrentSystemIsClientSystemGameBattle();
            //if (bFlag == false)
            {
                _InitPetTab();
            }

            if (a_event.EventID == EUIEventID.ItemUseSuccess)
            {
                ItemData item = (ItemData)a_event.Param1;

                if (item.PackageType == EPackageType.Consumable)
                {
                    var TotalData = TableManager.GetInstance().GetTable<PetTable>();
                    var enumer = TotalData.GetEnumerator();

                    bool bIsPet = false;
                    while (enumer.MoveNext())
                    {
                        var data = enumer.Current.Value as PetTable;

                        if (data == null)
                        {
                            continue;
                        }

                        if (data.PetEggID == item.TableID)
                        {
                            bIsPet = true;
                            break;
                        }
                    }

                    if (bIsPet)
                    {
                        if (ClientSystemManager.GetInstance().IsFrameOpen<OpenPetEggFrame>())
                        {
                            ClientSystemManager.GetInstance().CloseFrame<OpenPetEggFrame>();

                        }
                        ClientSystemManager.GetInstance().OpenFrame<OpenPetEggFrame>(FrameLayer.Middle, item);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 记录打开某界面的操作
        /// </summary>
        /// <param name="sFrameName">界面名</param>
        /// <param name="sButtonName">操作的button或toggle</param>
        private void DoStartFrameOperation(string mFrameName,string mName)
        {
            string sCurrentTime = Function.GetDateTime((int)TimeManager.GetInstance().GetServerTime());
            GameStatisticManager.GetInstance().DoStartFrameOperation(mFrameName, mName, sCurrentTime);
        }

        #region EquipPlan

        private void OnReceiveEquipPlanItemEndTimeMessage(UIEvent uiEvent)
        {
            //不是装备和称号页签，不用刷新
            if (m_currentItemType != EPackageType.Equip
               && m_currentItemType != EPackageType.Title
               && m_currentItemType != EPackageType.Bxy)
                return;

            //刷新
            _RefreshItemList();
        }

        #endregion

    }
}
