using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using ProtoTable;
using UnityEngine.Events;

namespace GameClient
{
    class ComItemManager
    {
        public static ComItem Create(GameObject parent)
        {
            if (parent == null)
            {
                Logger.LogError("ComItemManager Create function param parent is null!");
                return null;
            }

            GameObject item = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/Package/Item", enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);
            if (item != null)
            {
                ComItem comItem = item.GetComponent<ComItem>();
                if (comItem != null)
                {
                    comItem.Reset();
                    comItem.gameObject.transform.SetParent(parent.transform, false);
                    comItem.needRecycle = true;
                    comItem.needShowFashinMask = true;
                    return comItem;
                }
            }
            return null;
        }

        public static ComItemNew CreateNew(GameObject parent)
        {
            if (parent == null)
            {
                Logger.LogError("ComItemManager Create function param parent is null!");
                return null;
            }

            GameObject item = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/Package/ComItemNew", enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);
            if (item != null)
            {
                ComItemNew comItem = item.GetComponent<ComItemNew>();
                if (comItem != null)
                {
                    comItem.Reset();
                    comItem.gameObject.transform.SetParent(parent.transform, false);
                    //comItem.needRecycle = true;
                    return comItem;
                }
            }
            return null;
        }

        public static void Destroy(ComItem a_comItem)
        {
            if (a_comItem != null && a_comItem.gameObject != null)
            {
                a_comItem.Reset();
                if (a_comItem.needRecycle)
                {
                    CGameObjectPool.instance.RecycleGameObject(a_comItem.gameObject);
                    a_comItem.needRecycle = false;
                    a_comItem.needShowFashinMask = true;
                }
            }
        }

        public static void DestroyNew(ComItemNew a_comItem)
        {
            if (a_comItem != null && a_comItem.gameObject != null)
            {
                a_comItem.Reset();
                CGameObjectPool.instance.RecycleGameObject(a_comItem.gameObject);
            }
        }

        public static void Destroy(List<ComItem> a_arrComItems)
        {
            if (a_arrComItems != null)
            {
                for (int i = 0; i < a_arrComItems.Count; ++i)
                {
                    ComItem comItem = a_arrComItems[i];
                    if (comItem != null && comItem.gameObject != null)
                    {
                        comItem.Reset();
                        if (comItem.needRecycle)
                        {
                            CGameObjectPool.instance.RecycleGameObject(comItem.gameObject);
                            comItem.needRecycle = false;
                            comItem.needShowFashinMask = true;
                        }
                    }
                }
            }
        }

        public static void Destroy(ComItem[] a_arrComItems)
        {
            if (a_arrComItems != null)
            {
                for (int i = 0; i < a_arrComItems.Length; ++i)
                {
                    ComItem comItem = a_arrComItems[i];

                    if (comItem != null && comItem.gameObject != null)
                    {
                        comItem.Reset();
                        if (comItem.needRecycle)
                        {
                            CGameObjectPool.instance.RecycleGameObject(comItem.gameObject);
                            comItem.needRecycle = false;
                            comItem.needShowFashinMask = true;
                        }
                    }
                }
            }
        }
    }


    /*
    ComItem管理预制体如何显示，由使用者通过Setup接口传递需要的数据
    所需数据==>
    ItemData：道具
    Callback：点击回调
    SlotName：槽位名称
    CountFormatter：数量格式器
    */
    public class ComItem : MonoBehaviour
    {
        public delegate void OnSlotClicked(GameObject obj);
        public delegate void OnItemClicked(GameObject obj, ItemData item);
        public delegate string CountFormatter(ComItem a_comItem);
        public enum EColorType
        {
            Grey,
            Normal,
        }
        public enum ESlotType
        {
            Locked,
            Opened,
        }


        public GameObject objSlotGroup;
        public Image imgBackGround;
        public Button btnSlotLocked;
        public Text labSlot;
        public Image SlotIcon;

        public GameObject objItemGroup;
        public Button btnIcon;
        //public GameObject objIconFragment;
        public GameObject objIconFashion;
        //public Text labIconLevel;
        public Text labCount;
        public Text labStrengthenLevel;
        public Text textName;
        public Text labLevel;
        public Image imgSeal;
        public Image imgBetterArrow;
        public Image imgPunishArrow;
        public Image imgNewMark;
        public Button btnSelectGroup;
        public Image imgSelect;
        public Image imgCD;
        public GameObject objTimeLimit;
        public Image weaponIcon;
        public Image imgRedPoint;
        public Image imgLock;
        public Image imgStrengthLock;
        public GameObject objTreasure;
        public GameObject objBreathMark;//气息遮罩
        public Image imgStrengthAdd;
        public Image imgStrengthen1;
        public Image imgStrengthen2;
        public GameObject objStrengthen;
        public GameObject equipPlanRoot;        //装备方案Root
        public Image equipPlanImage;        //装备方案icon

        public List<CtrlFrame> EffectCtrls;

        public object UserData { get; set; }
        public ItemData ItemData {
            get
            {
                return m_item;
            }
        }
        [SerializeField] private bool mIsCanDoubleClick = false;
        [SerializeField] private ComDoubleClick mDoubleClick;
        public Image BackIcon = null;
        public Image imgLockMask = null;
        const string defaultLockMaskImgPath = "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Tubiao_Suotou";
        bool m_needRecycle = false;
        public bool needRecycle { get { return m_needRecycle; } set { m_needRecycle = value; } }

        bool mNeedShowFashinMask = true;
        public bool needShowFashinMask { get { return mNeedShowFashinMask; } set { mNeedShowFashinMask = value; } }

        

        bool m_dirty = false;
        ItemData m_item;
        OnSlotClicked m_slotCallback;
        OnItemClicked m_callback;
        UnityAction<ItemData> m_doubleClickCB;
        CountFormatter m_countFormatter;
        ESlotType m_eSlotType = ESlotType.Opened;
        string m_lockMaskPath;
        string m_strSlotName;
        bool m_bEnable = true;
        bool m_bShowSelectState = false;
        bool m_bShowBetterState = false;
        bool m_bShowUnusableState = false;
        bool m_bShowNotEnoughState = false;
        bool m_bShowRedPoint = false;
        bool m_bShowTreasureState = false;
        string sideWeaponIconPath = "UI/Image/Packed/p_UI_Package.png:UI_Package_Tubiao_Fu";
        string mainWeaponIconPath = "UI/Image/Packed/p_UI_Package.png:UI_Package_Tubiao_Zhu";

        string sStrengthenAddIconPath = "UI/Image/NewPacked/Number_Zhuangbei.png:Number_Qianghua_+";
        string sGrowthAddIconPath = "UI/Image/NewPacked/Number_Zhuangbei.png:Number_Jihua_+";

        string sStrengthenNumberIconPath = "UI/Image/NewPacked/Number_Zhuangbei.png:Number_Qianghua_{0}";
        string sGrowthNumberIconPath = "UI/Image/NewPacked/Number_Zhuangbei.png:Number_Jihua_{0}";

        string equipPlanOnePath = "UI/Image/Packed/p_UI_Package.png:UI_Package_Tubiao_01";
        string equipPlanTwoPath = "UI/Image/Packed/p_UI_Package.png:UI_Package_Tubiao_02";

        void Awake()
        {
            _InitDoubleClickAction();
        }

        private void _InitDoubleClick()
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
                mDoubleClick.onClick.AddListener(() => {
                    if (!m_bShowSelectState)
                    {
                        _OnClickCallBack();
                    }
                });
                mDoubleClick.onDoubleClick.RemoveAllListeners();
                mDoubleClick.onDoubleClick.AddListener(_DoubleClickAction);
            }
        }

        private void _DoubleClickAction()
        {
            if (m_doubleClickCB != null)
            {
                m_doubleClickCB(m_item);
            }
        }

        //重置ComItem
        public void Reset()
        {
            gameObject.name = "item";
            m_dirty = false;
            m_item = null;
            m_slotCallback = null;
            m_callback = null;
            m_doubleClickCB = null;
            mIsCanDoubleClick = false;
            m_countFormatter = null;
            m_eSlotType = ESlotType.Opened;
            m_strSlotName = string.Empty;
            m_bEnable = true;
            m_bShowSelectState = false;
            m_bShowBetterState = false;
            m_bShowUnusableState = false;
            m_bShowNotEnoughState = false;
            m_bShowRedPoint = false;
            m_bShowTreasureState = false;


            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(0, 0);
            rect.sizeDelta = new Vector2(0, 0);
            rect.pivot = new Vector2(0.5f, 0.5f);

            Component[] allComs = gameObject.GetComponents<Component>();
            for (int i = 0; i < allComs.Length; i++)
            {
                if (!(
                    allComs[i] is ComItem ||
                    allComs[i] is RectTransform ||
                    allComs[i] is ComButtonEnbale || 
                    allComs[i] is AssetProxy || 
                    allComs[i] is CPooledGameObjectScript ||
                    allComs[i] is ComDoubleClick ||
                    allComs[i] is NullImage
                    ))
                {
                    GameObject.Destroy(allComs[i]);
                }
            }
            if(imgBackGround!=null)
            {
                imgBackGround.enabled = true;
            }
            Image img = objIconFashion.GetComponent<Image>();
            if (img != null)
            {
                img.enabled = true;
            }

            //重置字体大小
            ResetTextFont();

            // add by ckm 20221114
			ResetEffect();

            _Refresh();
        }

        //重置ComItem中字体的大小
        private void ResetTextFont()
        {
            //数量字体的大小重置，(与ComItem中设置的一致)
            if (labCount != null)
                // labCount.fontSize = 35;
                labCount.fontSize = 28;
        }

        public void SetActive(bool active)
        {
            gameObject.CustomActive(active);
        }

        public void SetEnable(bool a_enable)
        {
            if (m_bEnable != a_enable)
            {
                m_bEnable = a_enable;
                MarkDirty();
            }
        }

        /// <summary>
        /// 是否显示道具选中状态
        /// </summary>
        /// <param name="a_bShow"></param>
        public void SetShowSelectState(bool a_bShow)
        {
            if (m_bShowSelectState != a_bShow)
            {
                m_bShowSelectState = a_bShow;
                MarkDirty();
            }
        }

        public void SetShowBetterState(bool a_bShow)
        {
            if (m_bShowBetterState != a_bShow)
            {
                m_bShowBetterState = a_bShow;
                MarkDirty();
            }
        }

        public void SetShowUnusableState(bool a_bShow)
        {
            if (m_bShowUnusableState != a_bShow)
            {
                m_bShowUnusableState = a_bShow;
                MarkDirty();
            }
        }

        public void SetShowRedPoint(bool isShow)
        {
            if (m_bShowRedPoint != isShow)
            {
                m_bShowRedPoint = isShow;
                MarkDirty();
            }
        }

        public void SetShowTreasure(bool isShow)
        {
            if (m_bShowTreasureState != isShow)
            {
                m_bShowTreasureState = isShow;
                MarkDirty();
            }
        }

        public void SetShowNotEnoughState(bool a_bShow)
        {
            if (m_bShowNotEnoughState != a_bShow)
            {
                m_bShowNotEnoughState = a_bShow;
                MarkDirty();
            }
        }

        public void SetupSlot(ESlotType a_eSlotType, string a_strSlotName, OnSlotClicked a_slotClicked = null,string lockMaskPath = "")
        {
            m_eSlotType = a_eSlotType;
            m_strSlotName = a_strSlotName;
            m_slotCallback = a_slotClicked;
            if(string.IsNullOrEmpty(lockMaskPath))
            {
                m_lockMaskPath = defaultLockMaskImgPath;
            }
            else
            {
                m_lockMaskPath = lockMaskPath;
            }
            MarkDirty();
        }

        public void Setup(ItemData item, OnItemClicked callback, bool isDoubleClick = false, UnityAction<ItemData> doubleClickCB = null)
        {
            m_item = item;

            if(item != null)
            {
                if(item.GUID !=0)
                {
                    gameObject.name = item.GUID.ToString();
                }
                else
                {
                    gameObject.name = item.TableID.ToString();
                }
            }
            else
            {
                gameObject.name = "item";
            }

            mIsCanDoubleClick = isDoubleClick;
            m_doubleClickCB = doubleClickCB;
            _InitDoubleClick();
            m_callback = callback;       
            MarkDirty();
        }

        public void NeedShowName(bool showName)
        {
            textName.CustomActive(showName && null != m_item);
            if (showName && null != m_item)
            {
                textName.text = m_item.GetColorName();
            }
        }

        public void SetCountFormatter(CountFormatter a_formatter)
        {
            m_countFormatter = a_formatter;
            MarkDirty();
        }

        public void MarkDirty()
        {
            m_dirty = true;
        }

        void Start()
        {
            MarkDirty();
            Update();
        }

        void Update()
        {
            if (m_dirty)
            {
                _Refresh();
                m_dirty = false;
            }

            if (m_item != null)
            {
                _UpdateCD();
            }
        }

        void OnDestroy()
        {
            if (null != btnIcon)
            {
                btnIcon.image.sprite = null;
                btnIcon = null;
            }
            if (null != btnSelectGroup)
            {
                btnSelectGroup.image.sprite = null;
                btnSelectGroup = null;
            }

            if (null != imgBackGround)
            {
                imgBackGround.sprite = null;
                imgBackGround = null;
            }
            if (null != imgBetterArrow)
            {
                imgBetterArrow.sprite = null;
                imgBetterArrow = null;
            }
            if(null != imgPunishArrow)
            {
                imgPunishArrow.sprite = null;
                imgPunishArrow = null;
            }
            if (null != imgNewMark)
            {
                imgNewMark.sprite = null;
                imgNewMark = null;
            }
            if (null != imgSelect)
            {
                imgSelect.sprite = null;
                imgSelect = null;
            }
            if (null != imgSeal)
            {
                imgSeal.sprite = null;
                imgSeal = null;
            }

            if (null != imgRedPoint)
            {
                imgRedPoint.sprite = null;
                imgRedPoint = null;
            }

            if(BackIcon != null)
            {
                BackIcon = null;
            }
            
            labCount = null;
            labStrengthenLevel = null;
            labSlot = null;
            labLevel = null;

            if(SlotIcon != null)
            {
                SlotIcon = null;
            }
        }

        protected void _Refresh()
        {
            _UpdateSlotGroup();
            _UpdateItemGroup();
            _UpdateEnable();
        }

        void _UpdateSlotGroup()
        {
            if (m_item == null)
            {
                if (objSlotGroup != null)
                {
                    objSlotGroup.CustomActive(true);
                }

                if(BackIcon != null)
                {
                    BackIcon.CustomActive(true);
                }

                if(m_eSlotType == ESlotType.Locked)
                {           
                    if(imgLockMask != null)
                    {
                        imgLockMask.SafeSetImage(m_lockMaskPath);
                        imgLockMask.SetNativeSize();
                    }

                    if(BackIcon != null)
                    {
                        BackIcon.CustomActive(false);
                    }
                }
                if (btnSlotLocked != null)
                {
                    btnSlotLocked.gameObject.CustomActive(m_eSlotType == ESlotType.Locked);
                    btnSlotLocked.onClick.RemoveAllListeners();
                    if (btnSlotLocked.image != null)
                    {
                        if (m_slotCallback != null)
                        {
                            btnSlotLocked.image.raycastTarget = true;
                            btnSlotLocked.onClick.AddListener(() => { m_slotCallback(gameObject); });
                        }
                        else
                        {
                            btnSlotLocked.image.raycastTarget = false;
                        }
                    }
                }

//                 if (labSlot != null)
//                 {
//                     if (string.IsNullOrEmpty(m_strSlotName) == false)
//                     {
//                         labSlot.gameObject.CustomActive(true);
//                         labSlot.text = m_strSlotName;
// 
//                         if(BackIcon != null)
//                         {
//                             BackIcon.CustomActive(false);
//                         }
//                     }
//                     else
//                     {
//                         labSlot.gameObject.CustomActive(false);
//                     }
//                 }
                
                if(SlotIcon != null)
                {
                    if (string.IsNullOrEmpty(m_strSlotName) == false)
                    {
                        ETCImageLoader.LoadSprite(ref SlotIcon, _GetEquipTypeIconPath(m_strSlotName)); 
                        SlotIcon.CustomActive(true);

                        if (BackIcon != null)
                        {
                            BackIcon.CustomActive(false);
                        }
                    }
                    else
                    {
                        SlotIcon.CustomActive(false);
                    }
                }
            }
            else
            {
                if (objSlotGroup != null)
                {
                    objSlotGroup.CustomActive(false);
                }
            }
        }

        private string _GetEquipTypeIconPath(string name)
        {
            // 一个是装备一个是时装
            if(name == "肩部" || name == "头部")
            {
                return "UI/Image/NewPacked/Beibao.png:Beibao_Zhuangbei_Diweng_Toubu";
            }
            else if (name == "胸部" || name == "衣服")
            {
                return "UI/Image/NewPacked/Beibao.png:Beibao_Zhuangbei_Diweng_Yifu";
            }
            else if (name == "腰部" || name == "头饰")
            {
                return "UI/Image/NewPacked/Beibao.png:Beibao_Zhuangbei_Diweng_Yaobu";
            }
            else if (name == "腿部" || name == "裤子")
            {
                return "UI/Image/NewPacked/Beibao.png:Beibao_Zhuangbei_Diweng_Tuibu";
            }
            else if (name == "鞋子")
            {
                return "UI/Image/NewPacked/Beibao.png:Beibao_Zhuangbei_Diweng_Xiezhi";
            }
            else if (name == "武器")
            {
                return "UI/Image/NewPacked/Beibao.png:Beibao_Zhuangbei_Diweng_Wuqi";
            }
            else if (name == "戒指" || name == "手镯")
            {
                return "UI/Image/NewPacked/Beibao.png:Beibao_Zhuangbei_Diweng_Jiezhi";
            }
            else if (name == "项链" || name == "首饰")
            {
                return "UI/Image/NewPacked/Beibao.png:Beibao_Zhuangbei_Diweng_Xianglian";
            }
            else if (name == "称号" || name == "光环")
            {
                return "UI/Image/NewPacked/Beibao.png:Beibao_Zhuangbei_Diweng_Chenghao";
            }

            return "UI/Image/NewPacked/Beibao.png:Beibao_Zhuangbei_Diweng_Chenghao";
        }

        void _UpdateItemGroup()
        {
            if (m_item != null)
            {
                if (objItemGroup != null)
                {
                    objItemGroup.CustomActive(true);
                }

                _UpdateBackground();
                _UpdateIcon();
                _UpdateCount();
                _UpdateLevel();
                _UpdateStrengthenLevel();
                _UpdateStrengthenImg();
                _UpdateUnusableMask();
                _UpdateShowNotEnoughState();
                _UpdateCD();
                _UpdateBetterEquipHint();
                _UpdatePunishEquipHint();
                _UpdateSelectState();
                _UpdateNewEffect();
                _UpdateTimeLimit();
                _UpdateSeal();
                UpdateWeaponFlag();
                UpdateRedPoint();
                UpdateEquipLock();
                UpdateStrengthLock();
                _UpdateTreasure();
                _UpdateBreathMark();
                UpdateEquipPlanFlag();
            }
            else
            {
                if (objItemGroup != null)
                {
                    objItemGroup.CustomActive(false);
                }
            }
        }

        void _UpdateBackground()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (imgBackGround != null)
            {
                imgBackGround.CustomActive(true);
                // imgBackGround.sprite = AssetLoader.instance.LoadRes(m_item.GetQualityInfo().Background,typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref imgBackGround, m_item.GetQualityInfo().Background);
            }
        }

        void UpdateWeaponFlag()
        {
            weaponIcon.CustomActive(!ClientSystemManager.GetInstance().IsFrameOpen<SmithShopNewFrame>()&& ClientSystemManager.GetInstance().IsFrameOpen<PackageNewFrame>());
            if (m_item.isInSidePack)
            {
                ETCImageLoader.LoadSprite(ref weaponIcon, sideWeaponIconPath);
            }
            else
            {
                ulong mainWeapon = ItemDataManager.GetInstance().GetMainWeapon();

                if (mainWeapon != 0 && mainWeapon == m_item.GUID)
                {
                    ETCImageLoader.LoadSprite(ref weaponIcon, mainWeaponIconPath);
                }
                else
                {
                    weaponIcon.CustomActive(false);
                }
            }
                        
        }

        void ItemQuickTool()
        {
#if UNITY_EDITOR && UNITY_STANDALONE_WIN // 只有win平台的编辑器模式下可用，方便测试 add by qxy
            if (m_item != null)
            {
                if(Input.GetKey(KeyCode.LeftControl)) // 复制id
                {
                    GUIUtility.systemCopyBuffer = m_item.TableID.ToString();
                }
                else if(Input.GetKey(KeyCode.LeftAlt)) // 添加道具
                {
                    ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", m_item.TableID, 100));
                }                
            }
#endif
        }
        void _UpdateIcon()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (btnIcon != null)
            {
                btnIcon.CustomActive(true);

                //AssetLoader.instance.LoadResAync(item.Icon,typeof(Sprite),AssetLoadCallback,0,0);
                //btnIcon.image.color = new Color(0,0,0,0);
                // btnIcon.image.sprite = AssetLoader.instance.LoadRes(m_item.Icon, typeof(Sprite)).obj as Sprite;

                Image btnIconImage = btnIcon.image;
                ETCImageLoader.LoadSprite(ref btnIconImage, m_item.Icon);

                UIGray gray = this.GetComponentInParent<UIGray>();

                if(gray != null)
                {
                    UIGray.Refresh(gray);
                }        

                btnIcon.onClick.RemoveAllListeners();
                if (m_callback != null && !mIsCanDoubleClick)
                {
                    btnIcon.image.raycastTarget = true;
                    btnIcon.onClick.AddListener(_OnClickCallBack);
                    btnIcon.onClick.AddListener(ItemQuickTool);
                }
                else
                {
                    btnIcon.image.raycastTarget = false;
                }

                //if (objIconFragment != null)
                //{
                //    objIconFragment.CustomActive(
                //        m_item.Type == ProtoTable.ItemTable.eType.MATERIAL &&
                //        (m_item.SubType == (int)ProtoTable.ItemTable.eSubType.Fragment 
                //         || m_item.SubType == (int)ProtoTable.ItemTable.eSubType.MATERIAL_JINGPO)
                //        );
                //}

                if (objIconFashion != null)
                {
                    objIconFashion.CustomActive(m_item.Type == ProtoTable.ItemTable.eType.FASHION && mNeedShowFashinMask);
                }

                //if (labIconLevel != null)
                //{
                //    if (
                //        m_item.Type == ProtoTable.ItemTable.eType.MATERIAL &&
                //        m_item.SubType == (int)ProtoTable.ItemTable.eSubType.Drawing &&
                //        m_item.LevelLimit > 0
                //        )
                //    {
                //        labIconLevel.gameObject.CustomActive(true);
                //        labIconLevel.text = m_item.LevelLimit.ToString();
                //    }
                //    else
                //    {
                //        labIconLevel.gameObject.CustomActive(false);
                //    }
                //}
                
                SetIconEffect(m_item.EffectType);
            }
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

        public void SetFashionMaskShow(bool value)
        {
            mNeedShowFashinMask = false;
            if (objIconFashion != null)
                objIconFashion.CustomActive(value);
        }

        void _OnClickCallBack()
        {
            if (m_callback != null)
            {
                m_callback(gameObject, m_item);
            }
        }

        void _UpdateCount()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (labCount != null)
            {
                // 通过设置m_countFormatter，实现其他格式显示
                if (m_countFormatter != null)
                {
                    labCount.CustomActive(true);
                    labCount.text = m_countFormatter(this);
                }
                else
                {
                    if (m_item.Count > 1)
                    {
                        labCount.CustomActive(true);
                        labCount.text = m_item.Count.ToString();
                    }
                    else
                    {
                        labCount.CustomActive(false);
                    }
                }
            }
        }

        void _UpdateLevel()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (labLevel != null)
            {
                if (
                    m_item.Type == ProtoTable.ItemTable.eType.EQUIP &&
                    m_item.LevelLimit > 0
                    )
                {
                    labLevel.gameObject.CustomActive(true);
                    labLevel.text = string.Format("Lv.{0}", m_item.LevelLimit);
                    if(m_item.LevelLimit > PlayerBaseData.GetInstance().Level)
                    {
                        if (m_item.PackageType == EPackageType.Equip || m_item.PackageType == EPackageType.Storage || m_item.PackageType == EPackageType.RoleStorage)
                        {
                            labLevel.color = Color.red;
                        }
                        else
                        {
                            labLevel.color = Color.white;
                        }
                    }
                    else
                    {
                        labLevel.color = Color.white;
                    }
                }
                else
                {
                    labLevel.color = Color.white;
                    labLevel.gameObject.CustomActive(false);
                }
            }
        }

        public void _UpdateTreasure()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
          
            if (objTreasure != null)
            {
                objTreasure.CustomActive(false);
                objTreasure.CustomActive(m_bShowTreasureState);
            }
        }

        public void _UpdateBreathMark()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (objBreathMark != null)
            {
                objBreathMark.CustomActive(m_item.EquipType == EEquipType.ET_BREATH);
            }
        }

        void _UpdateStrengthenLevel()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (labStrengthenLevel != null)
            {
                //如果类型是附魔卡
                if (m_item.SubType == (int)ItemTable.eSubType.EnchantmentsCard)
                {
                    if (m_item.mPrecEnchantmentCard.iEnchantmentCardLevel > 0)
                    {
                        labStrengthenLevel.CustomActive(true);
                        labStrengthenLevel.text = string.Format("+{0}", m_item.mPrecEnchantmentCard.iEnchantmentCardLevel);
                    }
                    else
                    {
                        labStrengthenLevel.CustomActive(false);
                    }

                    return;
                }
                else
                {
                    labStrengthenLevel.CustomActive(false);
                }
            }
        }

        void _UpdateStrengthenImg()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            objStrengthen.CustomActive(false);
            
            if ( m_item.Type == ItemTable.eType.EQUIP)
            {
                //强化装备 强化等级大于0显示
                if (m_item.EquipType == EEquipType.ET_COMMON)
                {
                    if (m_item.StrengthenLevel > 0)
                    {
                        ETCImageLoader.LoadSprite(ref imgStrengthAdd, sStrengthenAddIconPath);
                        if (m_item.StrengthenLevel >= 10)
                        {
                            imgStrengthen2.CustomActive(true);
                            int iBits = m_item.StrengthenLevel % 10;//个位强化数字
                            int iTen = m_item.StrengthenLevel / 10; //十位强化数字
                            ETCImageLoader.LoadSprite(ref imgStrengthen2, string.Format(sStrengthenNumberIconPath, iBits));
                            ETCImageLoader.LoadSprite(ref imgStrengthen1, string.Format(sStrengthenNumberIconPath, iTen));
                        }
                        else
                        {
                            ETCImageLoader.LoadSprite(ref imgStrengthen1, string.Format(sStrengthenNumberIconPath, m_item.StrengthenLevel));
                            imgStrengthen2.CustomActive(false);
                        }

                        objStrengthen.CustomActive(true);
                    }
                    
                }//激化装备 强化等级大于等于0显示
                else if (m_item.EquipType == EEquipType.ET_REDMARK)
                {
                    ETCImageLoader.LoadSprite(ref imgStrengthAdd, sGrowthAddIconPath);
                    if (m_item.StrengthenLevel >= 10)
                    {
                        imgStrengthen2.CustomActive(true);
                        int iBits = m_item.StrengthenLevel % 10;//个位强化数字
                        int iTen = m_item.StrengthenLevel / 10; //十位强化数字
                        ETCImageLoader.LoadSprite(ref imgStrengthen2, string.Format(sGrowthNumberIconPath, iBits));
                        ETCImageLoader.LoadSprite(ref imgStrengthen1, string.Format(sGrowthNumberIconPath, iTen));
                    }
                    else
                    {
                        ETCImageLoader.LoadSprite(ref imgStrengthen1, string.Format(sGrowthNumberIconPath, m_item.StrengthenLevel));
                        imgStrengthen2.CustomActive(false);
                    }

                    objStrengthen.CustomActive(true);
                }
            }
        }

        void _UpdateSelectState()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (btnSelectGroup != null && imgSelect != null)
            {
                if (m_bShowSelectState)
                {
                    btnSelectGroup.gameObject.CustomActive(true);
                    btnSelectGroup.onClick.RemoveAllListeners();
                    if (m_callback != null)
                    {
                        btnSelectGroup.image.raycastTarget = true;
                        btnSelectGroup.onClick.AddListener(() => { m_callback(gameObject, m_item); });
                        btnSelectGroup.onClick.AddListener(ItemQuickTool);
                    }
                    else
                    {
                        btnSelectGroup.image.raycastTarget = false;
                    }

                    imgSelect.gameObject.CustomActive(m_item.IsSelected);
                    return;
                }
            }

            if (btnSelectGroup != null)
            {
                btnSelectGroup.gameObject.CustomActive(false);
            }
            if (imgSelect != null)
            {
                imgSelect.gameObject.CustomActive(false);
            }
        }

        void _UpdateNewEffect()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (imgNewMark != null)
            {
                if (m_item.IsNew)
                {
                    imgNewMark.gameObject.CustomActive(true);
                    ItemDataManager.GetInstance().NotifyItemBeOld(m_item);

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.PackageMain);
                }
                else
                {
                    imgNewMark.gameObject.CustomActive(false);
                }
            }
//             RectTransform rectRoot = GetComponent<RectTransform>();
//             if (NewEffect != null && rectRoot != null)
//             {
//                 if (a_item != null && a_item.IsNew)
//                 {
//                     float scaleX = rectRoot.sizeDelta.x / NewEffect.rect.width;
//                     float scaleY = rectRoot.sizeDelta.y / NewEffect.rect.height;
//                     NewEffect.localScale = new Vector3(scaleX, scaleY, 1.0f);
// 
//                     NewEffect.gameObject.CustomActive(false);
//                     NewEffect.gameObject.CustomActive(true);
// 
//                     ItemDataManager.GetInstance().NotifyItemBeOld(a_item);
//                 }
//                 else
//                 {
//                     NewEffect.gameObject.CustomActive(false);
//                 }
//             }
        }

        void _UpdateTimeLimit()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (objTimeLimit != null)
            {
                int nTimeLeft;
                bool bStarted;
                m_item.GetTimeLeft(out nTimeLeft, out bStarted);
                if (
                    (imgNewMark == null || imgNewMark.gameObject.activeSelf == false) && 
                    (bStarted == true && nTimeLeft > 0 || m_item.IsTimeLimit == true)
                    )
                {
                    objTimeLimit.CustomActive(true);
                    ItemDataManager.GetInstance().NotifyItemBeOld(m_item);

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.PackageMain);
                }
                else
                {
                    objTimeLimit.CustomActive(false);
                }
            }
        }

        void _UpdateEnable()
        {
            ComButtonEnbale buttonEnable = GetComponent<ComButtonEnbale>();
            if (buttonEnable != null)
            {
                buttonEnable.SetEnable(m_bEnable);
            }
        }

        void _UpdateSeal()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (imgSeal != null)
            {
                if (m_item.Packing)
                {
                    imgSeal.gameObject.CustomActive(true);
                }
                else
                {
                    imgSeal.gameObject.CustomActive(false);
                }
            }
        }

        private void UpdateEquipLock()
        {
            if (m_item == null)
            {
                return;
            }
            if(imgLock == null)
            {
                return;
            }

            imgLock.CustomActive(m_item.bLocked || m_item.IsLease || m_item.bFashionItemLocked);         
        }

        private void UpdateStrengthLock()
        {
            if (m_item == null)
            {
                return;
            }

            if (imgLock == null)
            {
                return;
            }

            imgStrengthLock.CustomActive(false);
            if (m_item.SubType == (int)ItemTable.eSubType.Coupon)
            {
                imgStrengthLock.CustomActive(m_item.BindAttr != ItemTable.eOwner.NOTBIND);
            }
        }

        private void UpdateRedPoint()
        {
            if (m_item == null)
                return;

            if (imgRedPoint != null)
            {
                if (true == m_bShowRedPoint)
                {
                    if (PackageDataManager.GetInstance() != null
                        && PackageDataManager.GetInstance().IsItemShowRedPoint(m_item) == true)
                    {
                        imgRedPoint.CustomActive(true);
                    }
                    else
                    {
                        imgRedPoint.CustomActive(false);
                    }
                }
                else
                {
                    imgRedPoint.CustomActive(false);
                }
            }
        }

        void _UpdateUnusableMask()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (btnIcon != null)
            {
                if (m_bShowUnusableState)
                {
                    btnIcon.image.color = Color.white;
                    Color unFitColor = new Color(220.0f / 255.0f, 0.0f, 0.0f, 150.0f / 255.0f);
                    if (m_item.Type == ProtoTable.ItemTable.eType.EXPENDABLE)
                    {
                        if (m_item.IsLevelFit() == false || m_item.IsOccupationFit() == false)
                        {
                            btnIcon.image.color = unFitColor;
                        }
                    }
                    else if (m_item.Type == ProtoTable.ItemTable.eType.EQUIP || m_item.Type == ProtoTable.ItemTable.eType.FASHION)
                    {
                        if (m_item.CanEquip() == false)
                        {
                            btnIcon.image.color = unFitColor;
                        }
                    }
                }
                else
                {
                    btnIcon.image.color = Color.white;
                }
            }
        }

        void _UpdateShowNotEnoughState()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (btnIcon != null)
            {
                if(m_bShowNotEnoughState)
                {
                    imgBackGround.color = btnIcon.image.color = Color.gray;
                }
                else
                {
                    imgBackGround.color = btnIcon.image.color = Color.white;
                }
            }
        }

        void _UpdateCD()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (imgCD != null)
            {
                Protocol.ItemCD itemCD = ItemDataManager.GetInstance().GetItemCD(m_item.CDGroupID);
                if (itemCD != null)
                {
                    double dFinishTime = (double)itemCD.endtime;
                    double dTimeLeft = dFinishTime - TimeManager.GetInstance().GetServerDoubleTime();
                    if (dTimeLeft >= 0)
                    {
                        imgCD.gameObject.CustomActive(true);
                        imgCD.fillAmount = (float)(dTimeLeft / (double)itemCD.maxtime);
                        imgCD.type = Image.Type.Filled;
                        imgCD.fillMethod = Image.FillMethod.Radial360;
                        imgCD.fillClockwise = false;
                        return;
                    }
                }

                imgCD.gameObject.CustomActive(false);
            }
        }

        void _UpdatePunishEquipHint()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (null != imgPunishArrow)
            {
                if (m_item.PackageType == EPackageType.WearEquip)
                {
                    bool bIsPunish = EquipMasterDataManager.GetInstance().IsPunish(PlayerBaseData.GetInstance().JobTableID, (int)m_item.Quality, (int)m_item.EquipWearSlotType, (int)m_item.ThirdType);
                    imgPunishArrow.CustomActive(bIsPunish);
                }
                else
                {
                    imgPunishArrow.CustomActive(false);
                }
            }
        }

        void _UpdateBetterEquipHint()
        {
            Assert.IsNotNull(m_item);
            if (m_item == null) return;
            if (imgBetterArrow != null)
            {
                if (m_bShowBetterState)
                {
                    if (m_item.CheckBetterThanEquip())
                    {
                        imgBetterArrow.gameObject.CustomActive(true);
                    }
                    else
                    {
                        imgBetterArrow.gameObject.CustomActive(false);
                    }
                }
                else
                {
                    imgBetterArrow.gameObject.CustomActive(false);
                }
            }
        }

        public void SetLabCountIsShow(bool isShow)
        {
            if (labCount != null)
            {
                labCount.gameObject.CustomActive(isShow);
            }
        }

        //未启用的装备方案的标志
        private void UpdateEquipPlanFlag()
        {

            if (equipPlanRoot == null)
                return;

            equipPlanRoot.CustomActive(false);
            
            var equipPlanId = 1;
            var isShowUnSelectedEquipPlanFlag = EquipPlanUtility.IsNeedShowUnUsedEquipPlanFlag(m_item, ref equipPlanId);
            
            if (isShowUnSelectedEquipPlanFlag == false)
            {
                //不存在换装方案
                equipPlanRoot.CustomActive(false);
            }
            else
            {
                //存在换装方案
                equipPlanRoot.CustomActive(true);

                //展示1,2
                var equipPlanIconPath = equipPlanOnePath;
                if (equipPlanId == 2)
                    equipPlanIconPath = equipPlanTwoPath;
                ETCImageLoader.LoadSprite(ref equipPlanImage, equipPlanIconPath);
            }
        }
    }
}
