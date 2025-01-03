using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using ProtoTable;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using UnityEngine.Events;

namespace GameClient
{
    /*
    ComItem管理预制体如何显示，由使用者通过Setup接口传递需要的数据
    所需数据==>
    ItemData：道具
    Callback：点击回调
    SlotName：槽位名称
    CountFormatter：数量格式器
    */

    public interface IItemDataModel 
    {
        ulong GUID { get; }
        int TableID { get; }
        ItemTable.eColor Quality { get; }
        ItemTable.eType Type { get; }
        int SubType { get; }
        EEquipWearSlotType EquipWearSlotType { get; }
        ItemTable.eThirdType ThirdType { get; }
        EEquipType EquipType { get; }
        int StrengthenLevel { get; }
        string Icon { get; }
        string Name { get; }
        bool IsNew { get; }
        bool IsLevelFit();
        bool IsOccupationFit();
        bool CanEquip();
        bool CheckBetterThanEquip();
        void GetTimeLeft(out int a_timeLeft, out bool a_bStartCountdown);
        int LevelLimit { get; }
        int Count { get; }
        int CDGroupID { get;}
        bool bLocked { get; }
        bool IsLease { get; }
        bool bFashionItemLocked { get; }
        bool Packing { get; }
        bool IsTimeLimit { get; }
        bool isInSidePack { get; }
        ItemTable.eOwner BindAttr { get; }

        EPackageType PackageType { get; }

        int EffectType { get; }
    }

    [RequireComponent(typeof(CanvasGroup))]
    public class ComItemNew :MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image mImageBg;
        [SerializeField] private Image mImageIcon;
        [SerializeField] private ComButtonEnbale mButtonEnable;
        [SerializeField] private Transform mExtraImageRoot;
        [SerializeField] private Transform mExtraTextRoot;
        [SerializeField] private CanvasGroup mCanvasGroup;
        [SerializeField] private Color mUnfitColor;
        [SerializeField] private Color mNotEnoughColor;
        [SerializeField] private float mStrengthImgSpace = 23f;
		[SerializeField] private bool mIsCanDoubleClick = false;        
        [SerializeField] private ComDoubleClick mDoubleClick;

        private Sprite mBgDefaultSprite;
        private Material mBgDefaultMaterial;
        private Sprite mIconDefaultSprite;
        private Material mIconDefaultMaterial;

        private IItemDataModel mModel;
        private Action<GameObject, IItemDataModel> mOnItemClick;
        private Action<GameObject> mOnSlotClicked;
        const string DefaultName = "ComItemNew";
        private bool mIsDefaultClick = false;
        private UnityAction<ItemData> mDoubleClickCB;

        void Awake()
        {
            if (mBgDefaultSprite == null)
            {
                mBgDefaultSprite = mImageBg.sprite;
                mBgDefaultMaterial = mImageBg.material;
                mIconDefaultSprite = mImageIcon.sprite;
                mIconDefaultMaterial = mImageIcon.material;
            }

            _InitDoubleClickAction();
        }

        void Update()
        {
            if (mModel != null)
            {
                _UpdateCD();
            }
            if (mIsInitImageIcon)
            {
                mIsInitImageIcon = false;
            }
        }

        /// <summary>
        /// 初始化函数 必须在其他设置函数之前调用 因为此函数内会Reset
        /// </summary>
        /// <param name="model"></param>
        /// <param name="onItemClick"></param>
        /// <param name="isDefaultClick"></param>
        public void Setup(IItemDataModel model, Action<GameObject, IItemDataModel> onItemClick = null, bool isDefaultClick = false, bool isDoubleClick = false, UnityAction<ItemData> doubleClickCB = null)
        {
            //Reset();
            if (model == null)
            {
                ShowEmpty();
                return;
            }
            Reset();

            mIsDefaultClick = isDefaultClick;
            mModel = model;
            mOnItemClick = onItemClick;
            mDoubleClickCB = doubleClickCB;
            mIsCanDoubleClick = isDoubleClick;
            _InitDoubleClick();
            _Init();
        }


        public void SetActive(bool active)
        {
            mCanvasGroup.CustomActive(active);
        }

        public void SetEnable(bool isEnable)
        {
            mButtonEnable?.SetEnable(isEnable);
        }

        public void SetShowSelectState(bool value)
        {
            if (value)
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.Select));
            }
            else
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.Select));
            }
        }

        public void ShowName()
        {
            if (mModel != null)
            {
                var text = _ShowExtraText(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.TextName), mModel.Name);
                if (text != null)
                {
                    text.color = GameUtility.Item.GetItemColor(mModel.Quality);
                }
            }
        }

        private bool mIsShowBetterState = false;
        public void SetShowBetterState(bool value)
        {
            mIsShowBetterState = value;
            _UpdateBetterState();
        }

        private void _UpdateBetterState()
        {
            if (mIsShowBetterState && mModel != null && mModel.CheckBetterThanEquip())
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.Better));
            }
            else
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.Better));
            }
        }

        private bool mIsShowUnusableState = false;
        public void SetShowUnusableState(bool value)
        {
            mIsShowUnusableState = value;
            _UpdateUnusableState();
        }

        private void _UpdateUnusableState()
        {
            if (!mIsShowUnusableState)
            {
                return;
            }

            mImageIcon.color = Color.white;
            if (mModel == null)
            {
                return;
            }

            if (mModel.Type == ProtoTable.ItemTable.eType.EXPENDABLE)
            {
                if (mModel.IsLevelFit() == false || mModel.IsOccupationFit() == false)
                {
                    mImageIcon.color = mUnfitColor;
                }
            }
            else if (mModel.Type == ProtoTable.ItemTable.eType.EQUIP || mModel.Type == ProtoTable.ItemTable.eType.FASHION)
            {
                if (mModel.CanEquip() == false)
                {
                    mImageIcon.color = mUnfitColor;
                }
            }
        }

        private bool mIsShowRedPoint = false;
        public void SetShowRedPoint(bool value)
        {
            mIsShowRedPoint = value;
            _UpdateRedPoint();
        }

        private void _UpdateRedPoint()
        {
            if (PackageDataManager.GetInstance().IsItemShowRedPoint(mModel as ItemData))
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.RedPoint));
            }
            else
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.RedPoint));
            }
        }

        public void SetShowTreasure(bool value)
        {
            if (value)
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.Treasure));
            }
            else
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.Treasure));
            }
        }

        public void SetShowNotEnoughState(bool value)
        {
            if (value)
            {
                mImageBg.color = mNotEnoughColor;
            }
            else
            {
                mImageBg.color = Color.white;
            }
        }

        private bool mIsInitImageIcon = false;
        public void SetupSlot(ComItem.ESlotType slotType, string slotName, Action<GameObject> slotClicked = null, string lockMaskPath = "")
        {
            mOnSlotClicked = slotClicked;
            if (slotType == ComItem.ESlotType.Locked)
            {
                var tableData = TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.Lock);
                _SetExtraRectTransform(tableData, mImageIcon.rectTransform);
                if (string.IsNullOrEmpty(lockMaskPath))
                {
                    ETCImageLoader.LoadSprite(ref mImageIcon, tableData.Path);
                }
                else
                {
                    ETCImageLoader.LoadSprite(ref mImageIcon, lockMaskPath);
                    mImageIcon.SetNativeSize();
                }
                mIsInitImageIcon = true;
            }
        }

        public void SetCount(string content)
        {
            _ShowExtraText(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.TextCount), content);
        }

        public void SetCountSize(int fontSize)
        {
            var text = _GetText(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.TextCount));
            if (text != null)
            {
                text.fontSize = fontSize;
            }
        }

        public void SetLabCountIsShow(bool isShow)
        {
            if (isShow)
            {
                _UpdateCount();
            }
            else
            {
                _HideExtraText(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.TextCount));
            }
        }

        public void ShowEmpty()
        {
            mIsInitImageIcon = false;
            Reset();
        }

        public List<CtrlFrame> EffectCtrls;

        public void Reset()
        {
            mIsShowBetterState = false;
            mIsShowUnusableState = false;
            mIsShowRedPoint = false;
            mButtonEnable?.SetEnable(true);
            mImageBg.sprite = mBgDefaultSprite;
            mImageBg.material = mBgDefaultMaterial;

            //RectTransform rect = gameObject.GetComponent<RectTransform>();
            //rect.anchorMin = new Vector2(0, 0);
            //rect.anchorMax = new Vector2(1, 1);
            //rect.anchoredPosition = new Vector2(0, 0);
            //rect.sizeDelta = new Vector2(0, 0);
            //rect.pivot = new Vector2(0.5f, 0.5f);

            if (!mIsInitImageIcon)
            {
                var tableData = TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.EmptyBg);
                if (tableData != null)
                {
                    mImageIcon.sprite = mIconDefaultSprite;
                    mImageIcon.material = mIconDefaultMaterial;
                    _SetExtraRectTransform(tableData, mImageIcon.rectTransform);
                }
                else
                {

                }
            }
            //mImageIcon.enabled = false;
            gameObject.name = DefaultName;
            mOnItemClick = null;
            mOnSlotClicked = null;
            _HideAllExtraImages();
            _HideAllExtraTexts();

            // add by ckm 20221114
			ResetEffect();

        }

        private void ResetEffect()
		{
			if (EffectCtrls != null) {
				for (int i = 0; i < EffectCtrls.Count; i++) {
					EffectCtrls[i].transform.gameObject.SetActive(false);
				}
			}
		}

        private void SetIconEffect(int key)
		{
			ResetEffect();
			if (key == -1 || key > 4) {
				return;
			}
			if (EffectCtrls != null && EffectCtrls.Count > 0 && EffectCtrls[key] != null) {
				EffectCtrls[key].transform.gameObject.SetActive(true);
				EffectCtrls[key].Play();
			}
		}

        private void _Init()
        {
            mImageIcon.enabled = true;
            _UpdateRedPoint();
            _UpdateUnusableState();
            _UpdateBetterState();
            _InitGameObjectName();
            _UpdateIcon();
            _UpdateLevel();
            _UpdateStrengthLv();
            _UpdateCount();
            _UpdateNewImage();
            _UpdateCD();
            _UpdateStrengthLock();
            _UpdateSafetyLock();
            _UpdateSeal();
            _UpdateTimeLimit();
            _UpdatePunishEquipHint();
            _UpdateBreathMark();
            _UpdateWeaponFlag();
            _UpdateFashion();
        }
        #region 更新文字

        private void _UpdateIcon()
        {
            if (mModel != null)
            {
                ETCImageLoader.LoadSprite(ref mImageBg, GameUtility.Item.GetItemQualityBg(mModel.Quality));
                ETCImageLoader.LoadSprite(ref mImageIcon, mModel.Icon);
                var tableData = TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.Icon);
                _SetExtraRectTransform(tableData, mImageIcon.rectTransform);

                SetIconEffect(mModel.EffectType);
            }
        }

        private void _UpdateLevel()
        {
            var tableData = TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.TextLevel);
            if (tableData != null && mModel != null && mModel.Type == ProtoTable.ItemTable.eType.EQUIP && mModel.LevelLimit > 0)
            {
                var text = _ShowExtraText(tableData, TR.Value("item_level", mModel.LevelLimit));
                if (mModel.LevelLimit > PlayerBaseData.GetInstance().Level)
                {
                    if (mModel.PackageType == EPackageType.Equip || mModel.PackageType == EPackageType.Storage || mModel.PackageType == EPackageType.RoleStorage)
                    {
                        text.color = Color.red;
                    }
                    else
                    {
                        text.color = Color.white;
                    }
                }
                else
                {
                    text.color = Color.white;
                }
            }
            else
            {
                _HideExtraText(tableData);
            }
        }

        private void _UpdateStrengthLv()
        {
            _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.StrengthNumBit));
            _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.StrengthNumTen));
            _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.StrengthAdd));
            _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.GrowthNumBit));
            _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.GrowthNumTen));
            _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.GrowthAdd));

            if (mModel.Type == ItemTable.eType.EQUIP)
            {
                //强化装备 强化等级大于0显示
                if (mModel.EquipType == EEquipType.ET_COMMON)
                {
                    if(mModel.StrengthenLevel > 0)
                    {
                        int iBits = mModel.StrengthenLevel % 10;//个位强化数字
                        var bitImage = _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.StrengthNumBit), iBits);
                        var pos = bitImage.rectTransform.anchoredPosition;
                        if (bitImage != null)
                        {
                            if (mModel.StrengthenLevel >= 10)
                            {
                                int iTen = mModel.StrengthenLevel / 10; //十位强化数字
                                var tenImage = _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.StrengthNumTen), iTen);
                                pos.x -= mStrengthImgSpace;
                                tenImage.rectTransform.anchoredPosition = pos;
                            }

                            var addImg = _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.StrengthAdd));
                            pos.x -= mStrengthImgSpace;
                            addImg.rectTransform.anchoredPosition = pos;
                        }
                    }
                }
                //激化装备 强化等级大于等于0显示
                else if (mModel.EquipType == EEquipType.ET_REDMARK)
                {
                    int iBits = mModel.StrengthenLevel % 10;//个位强化数字
                    var bitImage = _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.GrowthNumBit), iBits);
                    var pos = bitImage.rectTransform.anchoredPosition;
                    if (bitImage != null)
                    {
                        if (mModel.StrengthenLevel >= 10)
                        {
                            int iTen = mModel.StrengthenLevel / 10; //十位强化数字
                            var tenImage = _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.GrowthNumTen), iTen);
                            pos.x -= mStrengthImgSpace;
                            tenImage.rectTransform.anchoredPosition = pos;
                        }

                        var addImg = _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.GrowthAdd));
                        pos.x -= mStrengthImgSpace;
                        addImg.rectTransform.anchoredPosition = pos;
                    }
                }
            }
        }

        private void _UpdateCount()
        {
            if (mModel == null || mModel.Count <= 1)
            {
                _HideExtraText(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.TextCount));
                return;
            }
            SetCount(mModel.Count.ToString());
        }

        #endregion
        #region 更新图片

        private void _UpdateFashion()
        {
            if (mModel == null || mModel.Type != ItemTable.eType.FASHION)
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.FashionIcon));
            }
            else
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.FashionIcon));
            }
        }
        public void SetFashionMaskShow(bool value)
        {
            if (!value)
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.FashionIcon));
            }
            else
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.FashionIcon));
            }
        }

        private void _UpdateWeaponFlag()
        {
            //从之前的逻辑拷过来的.
            if (mModel == null || ClientSystemManager.GetInstance().IsFrameOpen<SmithShopNewFrame>() || !ClientSystemManager.GetInstance().IsFrameOpen<PackageNewFrame>())
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.AssistantWeapon));
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.MainWeapon));
                return;
            }

            if (mModel.isInSidePack)
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.AssistantWeapon));
            }
            else
            {
                ulong mainWeapon = ItemDataManager.GetInstance().GetMainWeapon();
                if (mainWeapon != 0 && mainWeapon == mModel.GUID)
                {
                    _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.MainWeapon));
                }
                else
                {
                    _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.AssistantWeapon));
                    _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.MainWeapon));
                }

            }
        }

        private void _UpdateBreathMark()
        {
            if (mModel == null || mModel.EquipType != EEquipType.ET_BREATH)
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.BreathMark));
            }
            else
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.BreathMark));
            }
        }

        //封装
        private void _UpdatePunishEquipHint()
        {
            if (mModel == null || mModel.PackageType != EPackageType.WearEquip)
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.PunishArrow));
                return;
            }

            bool bIsPunish = EquipMasterDataManager.GetInstance().IsPunish(PlayerBaseData.GetInstance().JobTableID, (int)mModel.Quality, (int)mModel.EquipWearSlotType, (int)mModel.ThirdType);

            if (bIsPunish)
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.PunishArrow));
            }
            else
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.PunishArrow));
            }
        }

        private void _UpdateTimeLimit()
        {
            if (mModel == null)
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.TimeLimit));
                return;
            }
            int nTimeLeft;
            bool bStarted;
            mModel.GetTimeLeft(out nTimeLeft, out bStarted);
            if ((bStarted == true && nTimeLeft > 0 || mModel.IsTimeLimit == true))
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.TimeLimit));
                ItemDataManager.GetInstance().NotifyItemBeOld(mModel as ItemData);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.PackageMain);
            }
            else
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.TimeLimit));
            }
        }

        //封装
        private void _UpdateSeal()
        {
            if (mModel == null || !mModel.Packing)
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.Seal));
            }
            else
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.Seal));
            }
        }

        //安全锁
        private void _UpdateSafetyLock()
        {
            if (mModel == null || (!mModel.bLocked && !mModel.IsLease && !mModel.bFashionItemLocked))
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.SafetyLock));
            }
            else
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.SafetyLock));
            }
        }

        //强化锁
        private void _UpdateStrengthLock()
        {
            if(mModel == null || mModel.SubType != (int)ItemTable.eSubType.Coupon || mModel.BindAttr == ItemTable.eOwner.NOTBIND)
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.StrengthStampLock));
            }
            else
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.StrengthStampLock));
            }
        }

        //新装备
        private void _UpdateNewImage()
        {
            if (mModel != null && mModel.IsNew)
            {
                _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.New));
                ItemDataManager.GetInstance().NotifyItemBeOld(mModel as ItemData);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.PackageMain);
            }
            else
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.New));
            }
        }

        //CD
        private void _UpdateCD()
        {
            if (mModel == null)
            {
                _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.CDMask));
                return;
            }

            Protocol.ItemCD itemCD = ItemDataManager.GetInstance().GetItemCD(mModel.CDGroupID);
            if (itemCD != null)
            {
                double dFinishTime = (double)itemCD.endtime;
                double dTimeLeft = dFinishTime - TimeManager.GetInstance().GetServerDoubleTime();
                if (dTimeLeft >= 0)
                {
                    var image = _ShowExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.CDMask));
                    image.fillAmount = (float)(dTimeLeft / (double)itemCD.maxtime);
                    image.type = Image.Type.Filled;
                    image.fillMethod = Image.FillMethod.Radial360;
                    image.fillClockwise = false;
                    return;
                }
            }

            _HideExtraImage(TableManager.GetInstance().GetTableItem<ComItemConfigTable>((int)ComItemConfigTable.eKey.CDMask));
        }
        #endregion

        void _InitDoubleClick()
        {
            if (mIsCanDoubleClick)
            {
                if (mDoubleClick == null)
                {
                    mDoubleClick = transform.GetComponent<ComDoubleClick>();
                    if (mDoubleClick == null)
                    {
                        mDoubleClick = transform.gameObject.AddComponent<ComDoubleClick>();
                        _InitDoubleClickAction();
                    }
                }

                if (mDoubleClick != null)
                {
                    mDoubleClick.interactable = true;
                }
            }
            else
            {
                if (mDoubleClick != null)
                {
                    mDoubleClick.interactable = false;
                }
            }
        }

        private void _InitDoubleClickAction()
        {
            if (mDoubleClick != null)
            {
                mDoubleClick.onClick.RemoveAllListeners();
                mDoubleClick.onClick.AddListener(Click);
                mDoubleClick.onDoubleClick.RemoveAllListeners();
                mDoubleClick.onDoubleClick.AddListener(_DoubleClickAction);
            }
        }

        private void _DoubleClickAction()
        {
            if (mDoubleClickCB != null)
            {
                mDoubleClickCB(mModel as ItemData);
            }
        }

        //设置gameobject的名字 用于新手引导
        private void _InitGameObjectName()
        {
            if (mModel != null)
            {
                if (mModel.GUID != 0)
                {
                    gameObject.name = mModel.GUID.ToString();
                }
                else
                {
                    gameObject.name = mModel.TableID.ToString();
                }
            }
            else
            {
                gameObject.name = DefaultName;
            }
        }

        #region Extra Images

        private Dictionary<int, ExtraImageData> mExtraImages = new Dictionary<int, ExtraImageData>();

        struct ExtraImageData
        {
            public Image Com;
            public string Name;
            public bool IsActive;
        }

        Image _ShowExtraImage(ComItemConfigTable data, object param = null)
        {
            if (data == null)
            {
                return null;
            }

            if (!mExtraImages.ContainsKey(data.SiblingIndex))
            {
                GameObject obj = null;

                // obj.AddComponent<CanvasRenderer>()
                int siblingIndex = int.MaxValue;
                int getFromPoolId = -1;
                //先从缓存里找到Active为false的
                foreach (var imgData in mExtraImages)
                {
                    if (!imgData.Value.IsActive && getFromPoolId == -1)
                    {
                        obj = imgData.Value.Com.gameObject;
                        getFromPoolId = imgData.Key;
                    }
                    else if (imgData.Key > data.SiblingIndex && imgData.Value.Com.transform.GetSiblingIndex() < siblingIndex)
                    {
                        siblingIndex = imgData.Value.Com.transform.GetSiblingIndex();
                    }
                }

                if (getFromPoolId != -1)
                {
                    mExtraImages.Remove(getFromPoolId);
                }

                if (obj == null)
                {
                    obj = new GameObject(string.Format("ExtraImage{0}", data.SiblingIndex), new Type[] { typeof(ImageEx) , typeof(ComModifyColor)});
                }
                if (siblingIndex >= 0)
                {
                    obj.transform.SetParent(mExtraImageRoot, false);
                    obj.transform.SetSiblingIndex(siblingIndex);
                }
                else
                {
                    obj.transform.SetParent(transform, false);
                }

                ExtraImageData extraData = new ExtraImageData();
                extraData.Com = obj.GetComponent<ImageEx>();
                extraData.Name = data.Path;
                extraData.IsActive = true;
                mExtraImages[data.SiblingIndex] = extraData;
            }
            else
            {
                ExtraImageData extraData = new ExtraImageData();
                extraData.Com = mExtraImages[data.SiblingIndex].Com.GetComponent<ImageEx>();
                extraData.Name = data.Path;
                extraData.IsActive = true;
                mExtraImages[data.SiblingIndex] = extraData;
            }
            var image = mExtraImages[data.SiblingIndex].Com;
            image.raycastTarget = false;
            image.enabled = (true);
            image.material = null;
            image.type = Image.Type.Simple;
            string path = data.Path;
            if (param != null)
            {
                path = string.Format(path, param);
            }

            if (data.Color.Length > 3)
            {
                image.color = new Color(data.ColorArray(0) / 255f, data.ColorArray(1) / 255f, data.ColorArray(2) / 255f, data.ColorArray(3) / 255f);
            }
            else if (data.Color.Length == 3)
            {
                image.color = new Color(data.ColorArray(0) / 255f, data.ColorArray(1) / 255f, data.ColorArray(2) / 255f);
            }

            ETCImageLoader.LoadSprite(ref image, path);
            _SetExtraRectTransform(data, image.rectTransform);
            return image;
        }

        void _HideExtraImage(ComItemConfigTable data)
        {
            if (mExtraImages.ContainsKey(data.SiblingIndex))
            {
                var imgData = mExtraImages[data.SiblingIndex];
                if (imgData.IsActive && string.CompareOrdinal(imgData.Name, data.Path) == 0)
                {
                    imgData.Com.enabled = false;
                    ExtraImageData newData = new ExtraImageData();
                    newData.IsActive = false;
                    newData.Com = imgData.Com;
                    newData.Name = imgData.Name;
                    mExtraImages[data.SiblingIndex] = newData;
                }
            }
        }

        private void _HideAllExtraImages()
        {
            List<int> keys = new List<int>(mExtraImages.Keys);
            for (int i = 0; i < keys.Count; ++i)
            {
                var key = keys[i];
                var data = mExtraImages[key];
                data.Com.enabled = false;
                ExtraImageData newData = new ExtraImageData();
                newData.IsActive = false;
                newData.Com = data.Com;
                newData.Name = data.Name;
                mExtraImages[key] = newData;
            }
        }

        private void _SetExtraRectTransform(ComItemConfigTable data, RectTransform trans)
        {
            float anchorMinX = data.AnchorMinArray(0) / 1000f;
            float anchorMaxX = data.AnchorMaxArray(0) / 1000f;
            float anchorMinY = data.AnchorMinArray(1) / 1000f;
            float anchorMaxY = data.AnchorMaxArray(1) / 1000f;
            Vector2 v2 = Vector2.zero;
            v2.x = data.PivotArray(0) / 1000f;
            v2.y = data.PivotArray(1) / 1000f;
            trans.pivot = v2;

            v2.x = anchorMinX;
            v2.y = anchorMinY;
            trans.anchorMin = v2;

            v2.x = anchorMaxX;
            v2.y = anchorMaxY;
            trans.anchorMax = v2;

            if (anchorMinX != anchorMaxX || anchorMinY != anchorMaxY)
            {
                var offsetMin = trans.offsetMin;
                var offsetMax = trans.offsetMax;
                if (anchorMinX != anchorMaxX)
                {
                    offsetMin.x = data.RectTransformArray(0) / 1000f;
                    offsetMax.x = -data.RectTransformArray(2) / 1000f;

                    trans.offsetMin = offsetMin;
                    trans.offsetMax = offsetMax;
                }

                if (anchorMinY != anchorMaxY)
                {
                    offsetMin = trans.offsetMin;
                    offsetMax = trans.offsetMax;

                    offsetMin.y = data.RectTransformArray(3) / 1000f;
                    offsetMax.y = -data.RectTransformArray(1) / 1000f;
                    trans.offsetMin = offsetMin;
                    trans.offsetMax = offsetMax;

                }
                var size = trans.sizeDelta;
                var pos = trans.anchoredPosition;
                if (anchorMinX == anchorMaxX)
                {
                    pos.x = data.RectTransformArray(0) / 1000f;
                    size.x = data.RectTransformArray(2) / 1000f;
                    trans.sizeDelta = size;
                    trans.anchoredPosition = pos;
                }

                if (anchorMinY == anchorMaxY)
                {
                    pos.y = data.RectTransformArray(1) / 1000f;
                    size.y = data.RectTransformArray(3) / 1000f;
                    trans.sizeDelta = size;
                    trans.anchoredPosition = pos;
                }
            }
            else
            {
                v2.x = data.RectTransformArray(2) / 1000f;
                v2.y = data.RectTransformArray(3) / 1000f;
                trans.sizeDelta = v2;

                v2.x = data.RectTransformArray(0) / 1000f;
                v2.y = data.RectTransformArray(1) / 1000f;
                trans.anchoredPosition = v2;
            }
        }
        #endregion

        #region Extra Texts


        private Dictionary<int, ExtraTextData> mExtraTexts = new Dictionary<int, ExtraTextData>();

        struct ExtraTextData
        {
            public Text Com;
            public int Id;
            public bool IsActive;
        }

        private void _HideAllExtraTexts()
        {
            List<int> keys = new List<int>(mExtraTexts.Keys);
            for (int i = 0; i < keys.Count; ++i)
            {
                var key = keys[i];
                var data = mExtraTexts[key];
                data.Com.enabled = false;
                ExtraTextData newData = new ExtraTextData();
                newData.IsActive = false;
                newData.Com = data.Com;
                newData.Id = data.Id;
                mExtraTexts[key] = newData;
            }
        }

        private void _HideExtraText(ComItemConfigTable data)
        {
            if (mExtraTexts.ContainsKey(data.SiblingIndex))
            {
                var textData = mExtraTexts[data.SiblingIndex];
                if (textData.IsActive && textData.Id == data.ID)
                {
                    textData.Com.enabled = false;
                    ExtraTextData newData = new ExtraTextData();
                    newData.IsActive = false;
                    newData.Com = textData.Com;
                    newData.Id = textData.Id;
                    mExtraTexts[data.SiblingIndex] = newData;
                }
            }
        }

        private Text _GetText(ComItemConfigTable data)
        {
            if (mExtraTexts.ContainsKey(data.SiblingIndex) && mExtraTexts[data.SiblingIndex].Id == data.ID)
            {
                return mExtraTexts[data.SiblingIndex].Com;
            }

            return null;
        }

        private Text _ShowExtraText(ComItemConfigTable data, string content)
        {
            if (data == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(content))
            {
                _HideExtraText(data);
                return null;
            }

            if (!mExtraTexts.ContainsKey(data.SiblingIndex))
            {
                GameObject obj = null;

                // obj.AddComponent<CanvasRenderer>()
                int siblingIndex = int.MaxValue;
                int getFromPoolId = -1;
                //先从缓存里找到Active为false的
                foreach (var textData in mExtraTexts)
                {
                    if (!textData.Value.IsActive && getFromPoolId == -1)
                    {
                        obj = textData.Value.Com.gameObject;
                        getFromPoolId = textData.Key;
                    }
                    else if (textData.Key > data.SiblingIndex && textData.Value.Com.transform.GetSiblingIndex() < siblingIndex)
                    {
                        siblingIndex = textData.Value.Com.transform.GetSiblingIndex();
                    }
                }

                if (getFromPoolId != -1)
                {
                    mExtraTexts.Remove(getFromPoolId);
                }

                if (obj == null)
                {
                    obj = new GameObject(string.Format("ExtraText{0}", data.SiblingIndex), new Type[] { typeof(TextEx), typeof(ComModifyColor) });
                }
                if (siblingIndex >= 0)
                {
                    obj.transform.SetParent(mExtraTextRoot, false);
                    obj.transform.SetSiblingIndex(siblingIndex);
                }
                else
                {
                    obj.transform.SetParent(transform, false);
                }

                ExtraTextData extraData = new ExtraTextData();
                extraData.Com = obj.GetComponent<Text>();
                extraData.Id = data.ID;
                extraData.IsActive = true;
                mExtraTexts[data.SiblingIndex] = extraData;
            }
            else
            {
                ExtraTextData extraData = new ExtraTextData();
                extraData.Com = mExtraTexts[data.SiblingIndex].Com.GetComponent<Text>();
                extraData.Id = data.ID;
                extraData.IsActive = true;
                mExtraTexts[data.SiblingIndex] = extraData;
            }

            var text = mExtraTexts[data.SiblingIndex].Com;
            text.raycastTarget = false;
            text.supportRichText = false;
            text.enabled = (true);
            text.color = Color.white;
            if (data.Color.Length > 3)
            {
                text.color = new Color(data.ColorArray(0) / 255f, data.ColorArray(1) / 255f, data.ColorArray(2) / 255f, data.ColorArray(3) / 255f);
            }
            else if (data.Color.Length == 3)
            {
                text.color = new Color(data.ColorArray(0) / 255f, data.ColorArray(1) / 255f, data.ColorArray(2) / 255f);
            }
            text.fontSize = data.FontSize;
            text.horizontalOverflow = data.OverflowArray(0) == 1 ? HorizontalWrapMode.Overflow : HorizontalWrapMode.Wrap;
            text.verticalOverflow = data.OverflowArray(1) == 1 ? VerticalWrapMode.Overflow : VerticalWrapMode.Truncate;
            text.alignment = (TextAnchor)(data.TextAnchor);
            text.text = content;
            text.font = UIManager.GetInstance().GetTextFont();

            //outline
            if (data.OutlineColorLength > 3)
            {
                var outline = text.gameObject.GetOrAddComponent<NicerOutline>();
                outline.enabled = true;
                outline.effectColor = new Color(data.OutlineColorArray(0) / 255f, data.OutlineColorArray(1) / 255f, data.OutlineColorArray(2) / 255f, data.OutlineColorArray(3) / 255f);
            }
            else
            {
                var outline = text.gameObject.GetComponent<NicerOutline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
            }

            _SetExtraRectTransform(data, text.rectTransform);
            return text;
        }
        #endregion

        #region 点击事件

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!mIsCanDoubleClick || mDoubleClick == null)
            {
                Click();
            }
        }

        private void Click()
        {
            if (!mIsDefaultClick)
            {
                mOnItemClick?.Invoke(gameObject, mModel);
                mOnSlotClicked?.Invoke(gameObject);
            }
            else if (mModel != null)
            {
                ItemData compareItem = _GetCompareItem(mModel as ItemData);
                if (compareItem != null)
                {
                    ItemTipManager.GetInstance().ShowTipWithCompareItem(mModel as ItemData, compareItem, null);
                }
                else
                {
                    ItemTipManager.GetInstance().ShowTip(mModel as ItemData, null, TextAnchor.MiddleLeft);
                }
            }
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

        public void OnPointerDown(PointerEventData eventData)
        {
            var parentPointerDown = transform.parent.GetComponentInParent<IPointerDownHandler>();
            if (parentPointerDown != null)
                parentPointerDown.OnPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var parentPointerDown = transform.parent.GetComponentInParent<IPointerUpHandler>();
            if (parentPointerDown != null)
                parentPointerDown.OnPointerUp(eventData);
        }
#endregion

    }

}
