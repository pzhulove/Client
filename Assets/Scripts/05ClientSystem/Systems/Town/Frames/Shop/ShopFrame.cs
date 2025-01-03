using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;
///////删除linq
using Protocol;
using ProtoTable;

namespace GameClient
{
    class ShopFrame : ClientFrame
    {
        public enum ShopFrameMode
        {
            SFM_MAIN_FRAME = 0,
            SFM_CHILD_FRAME,
            SFM_GUILD_CHILD_FRAME,
            SFM_MALL_CHILD_FRAME,
            SFM_QUERY_NON_FRAME,
        }
        public static void CloseMulteFrame(int iShopID)
        {
            IClientFrame frame = ClientSystemManager.GetInstance().GetFrame("ShopFrame" + iShopID);
            if (frame != null)
            {
                frame.Close(true);
            }
        }

        public class MoneySort
        {
            public int iId;
            public int iOrder;
        }

        public static MoneySort[] ms_money_sorts = new MoneySort[]
        {
            new MoneySort { iId = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.POINT), iOrder = 1 },
            new MoneySort { iId = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindPOINT), iOrder = 2 },
            new MoneySort { iId = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.GOLD), iOrder = 3 },
            new MoneySort { iId = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindGOLD), iOrder = 4 },

            new MoneySort { iId = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.MagicJarPoint), iOrder = 1 },
            new MoneySort { iId = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.GoldJarPoint), iOrder = 2 },
        };

        public static int[] ms_money_show_name = new int[]
        {
            600000064,600000065,
        };

        ProtoTable.ItemTable.eThirdType[] armorLists = new ProtoTable.ItemTable.eThirdType[]
        {
                        ProtoTable.ItemTable.eThirdType.CLOTH,
                        ProtoTable.ItemTable.eThirdType.SKIN,
                        ProtoTable.ItemTable.eThirdType.LIGHT,
                        ProtoTable.ItemTable.eThirdType.HEAVY,
                        ProtoTable.ItemTable.eThirdType.PLATE,
        };

        string[] armorNames = new string[]
        {
                        TR.Value("goldjar_sub_type_cloth"),
                        TR.Value("goldjar_sub_type_skin"),
                        TR.Value("goldjar_sub_type_lightd"),
                        TR.Value("goldjar_sub_type_heavy"),
                        TR.Value("goldjar_sub_type_plate"),
        };

        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                var tokens = strParam.Split('|');
                if (tokens.Length == 3)
                {
                    int iShopId = int.Parse(tokens[0]);
                    int iShopLinkID = int.Parse(tokens[1]);
                    int iShopTabID = int.Parse(tokens[2]);

                    var shopTable = TableManager.GetInstance().GetTableItem<ShopTable>(iShopId);
                    if (shopTable != null)
                    {
                        //小于解锁等级，弹出提示，之后返回
                        if (PlayerBaseData.GetInstance().Level < shopTable.OpenLevel)
                        {
                            var exchangeNotOpenTip = string.Format(TR.Value("exchange_mall_not_open"), shopTable.OpenLevel
                                , shopTable.ShopName);
                            SystemNotifyManager.SysNotifyFloatingEffect(exchangeNotOpenTip);
                            return;
                        }
                    }

                    ShopDataManager.GetInstance().OpenShop(iShopId, iShopLinkID, iShopTabID);
                }
            }
            catch(Exception e)
            {
                Logger.LogError("ShopFrame.OpenLinkFrame : ==>" + e.ToString());
            }
        }

        public delegate void OnShopReturn();
        public class ShopFrameData
        {
            public ShopData m_kShopData;
            public int m_iShopLinkID;
            public int m_iShopTabID;
            public OnShopReturn onShopReturn;
            public ShopFrameMode eShopFrameMode = ShopFrameMode.SFM_MAIN_FRAME;
        }

        [UIControl("middleback/Goods/Title/BtnRefresh", typeof(StatusBinder))]
        StatusBinder m_kRefreshBinder;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Shop/ShopFrame";
        }

        #region _declare
        ShopData m_kShopData;
        int m_iShopLinkID;
        int m_iShopTabID;
        ShopFrameMode m_eShopFrameMode = ShopFrameMode.SFM_MAIN_FRAME;
        public ShopFrameMode EShopFrameMode
        {
            get
            {
                return m_eShopFrameMode;
            }
        }
        Text m_kShopName;

        Dropdown m_kDropdown;

        [UIControl("middleback/Goods/JarScore", typeof(Text))]
        Text jarScore;
        /// <summary>
        /// scrollrange control
        /// </summary>
        //ClipScrollrect m_scrollrect;
        //Scrollbar m_scrollbar;
        bool m_bNeedMod;
        DOTweenAnimation m_animation;
        float m_fPreValue;

        GameObject m_goPointArray;
        GameObject m_goArrowLeft;
        GameObject m_goArrowRight;
        GameObject m_goScrollView;
        float m_fScrollViewWidth;
        int m_iCurrentPage;
        int m_iMaxPage;
        int m_iPageCount;
        float m_fCurMod;
        int m_iCurGoodsCount;
        public enum PageConfig : int
        {
            PC_COUNT = 8,
            PC_HALF = PC_COUNT / 2,
        }

        TimeRefresh comTimeRefresh;

        public class GoodOptionData : Dropdown.OptionData
        {
            public GoodOptionData()
            {

            }
            public ProtoTable.JobTable jobItem = null;
            public ProtoTable.ItemTable.eThirdType eFilter = ProtoTable.ItemTable.eThirdType.TT_NONE;
        }

        GoodOptionData m_kGoodOptionData = null;
        //void OnDragEnd(PointerEventData eventData)
        //{
        //    float fcurSpeedX = Mathf.Abs(eventData.delta.x / Time.deltaTime);
        //    if (float.IsNaN(fcurSpeedX))
        //    {
        //        fcurSpeedX = 1.0f;
        //    }
        //    if (m_scrollbar != null && m_bNeedMod)
        //    {
        //        Int32 iCurValue = (Int32)(m_scrollbar.value * 100.0f);
        //        Int32 iModValue = (Int32)(m_fCurMod * 100.0f);
        //        Int32 iKeepValue = (Int32)(iCurValue * 1.0f / iModValue);

        //        float fRealValue = 0.0f;
        //        if(m_scrollbar.value > m_fPreValue)
        //        {
        //            fRealValue = Mathf.Min(m_fPreValue + m_fCurMod * 4, 1.0f);
        //        }
        //        else if (m_scrollbar.value < m_fPreValue)
        //        {
        //            fRealValue = Mathf.Max(m_fPreValue - m_fCurMod * 4, 0.0f);
        //        }
        //        else
        //        {
        //            fRealValue = m_fPreValue;
        //        }

        //        m_fPreValue = fRealValue;
        //        fRealValue /= m_fCurMod;

        //        if (m_goGoodsDataParent != null)
        //        {
        //            if (m_animation == null)
        //            {
        //                m_animation = m_goGoodsDataParent.GetComponent<DOTweenAnimation>();
        //            }
        //            if (m_animation != null)
        //            {
        //                m_animation.endValueV3 = new Vector3(fRealValue * -(m_fScrollViewWidth/4.0f), 0.0f, 0.0f);
        //                m_animation.duration = Mathf.Abs(m_animation.endValueV3.x / fcurSpeedX);
        //                if (float.IsNaN(m_animation.duration))
        //                {
        //                    m_animation.duration = 0.0f;
        //                }
        //                m_animation.duration = Mathf.Clamp(m_animation.duration, 0.10f, 0.250f);
        //                m_animation.CreateTween();
        //                m_animation.DOPlay();
        //                InvokeMethod.Invoke(m_animation.duration + 0.10f, () =>
        //                {
        //                    m_scrollbar.value = m_fPreValue;
        //                    _OnSetPageInfo();
        //                });
        //            }
        //        }
        //    }
        //}

        void _FreshModeInfo(int iCount)
        {
            m_iCurGoodsCount = iCount;
            if (m_iCurGoodsCount <= (int)PageConfig.PC_COUNT)
            {
                m_bNeedMod = false;
                m_fCurMod = 1.0f;
            }
            else
            {
                m_bNeedMod = true;
                Int32 iCurCount = (m_iCurGoodsCount & 1) == 1 ? (m_iCurGoodsCount / 2 + 1) : (m_iCurGoodsCount / 2);
                iCurCount -= (int)PageConfig.PC_HALF;
                m_fCurMod = 1.0f / iCurCount;
            }
        }

        protected void _OnSetPageInfo()
        {
            if (!m_bNeedMod)
            {
                //m_goArrowLeft.gameObject.CustomActive(false);
                //m_goArrowRight.gameObject.CustomActive(false);
                //m_goPointArray.CustomActive(false);
            }
            else
            {
                //Int32 iCurValue = (Int32)(m_scrollbar.value * 100.0f);
                //Int32 iModValue = (Int32)(m_fCurMod * 100.0f);
                //Int32 iKeepValue = (Int32)(iCurValue * 1.0f / iModValue);
                //Int32 iCurCount = (m_iCurGoodsCount & 1) == 1 ? (m_iCurGoodsCount / 2 + 1) : (m_iCurGoodsCount / 2);

                //iKeepValue = iKeepValue / 4 + (iKeepValue % 4 == 0 ? 0 : 1);
                //Int32 iMaxPage = iCurCount / 4 + (iCurCount % 4 == 0 ? 0 : 1);
                //m_goArrowLeft.gameObject.CustomActive(iKeepValue > 0);
                //m_goArrowRight.gameObject.CustomActive(iKeepValue + 1 < iMaxPage);

                //m_goPointArray.CustomActive(true);
                //for(int i = 0; i < m_goPointArray.transform.childCount; ++i)
                //{
                //    var child = m_goPointArray.transform.GetChild(i);
                //    child.gameObject.CustomActive(i < iMaxPage);

                //    var imageComPonent = child.GetComponent<Image>();
                //    if(imageComPonent != null)
                //    {
                //        imageComPonent.enabled = (i != iKeepValue && i < iMaxPage);
                //    }

                //    child = child.transform.GetChild(0);
                //    child.gameObject.CustomActive(i == iKeepValue);
                //}
            }
        }

        void _SetFilter(ProtoTable.ShopTable.eFilter eFilter)
        {
            if (m_kDropdown != null)
            {
                m_kDropdown.CustomActive(eFilter != ProtoTable.ShopTable.eFilter.SF_NONE);
                m_kDropdown.onValueChanged.RemoveAllListeners();
                m_kDropdown.options.Clear();

                if (eFilter == ProtoTable.ShopTable.eFilter.SF_OCCU)
                {
                    var jobList = Utility.OrgJobTables;
                    if (jobList != null && jobList.Count > 0)
                    {
                        GoodOptionData current = null;
                        for (int i = 0; i < jobList.Count; ++i)
                        {
                            if (jobList[i] != null)
                            {
                                current = new GoodOptionData();
                                current.text = jobList[i].Name;
                                current.jobItem = jobList[i];

                                m_kDropdown.options.Add(current);

                                if (current.jobItem.ID == PlayerBaseData.GetInstance().JobTableID / 10 * 10)
                                {
                                    m_kDropdown.value = i;
                                }
                            }
                        }

                        if (jobList != null && jobList.Count > 0)
                        {
                            if (m_kDropdown != null && m_kDropdown.value >= 0 && m_kDropdown.value < m_kDropdown.options.Count)
                            {
                                var currentOption = m_kDropdown.options[m_kDropdown.value] as GoodOptionData;
                                m_kDropdown.captionText.text = currentOption.text;
                                m_kGoodOptionData = currentOption;
                            }
                        }
                    }

                    m_kDropdown.onValueChanged.AddListener(_OnDropDownValueChanged);
                }
                else if (eFilter == ProtoTable.ShopTable.eFilter.SF_ARMOR)
                {
                    GoodOptionData current = null;
                    bool bFind = false;
                    for (int i = 0; i < armorLists.Length; ++i)
                    {
                        //if(armorLists[i] == ProtoTable.ItemTable.eThirdType.LIGHT)
                        //{
                        //    continue;
                        //}

                        current = new GoodOptionData();
                        current.text = armorNames[i];
                        current.eFilter = armorLists[i];
                        m_kDropdown.options.Add(current);

                        var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID);
                        if (null != jobItem && !bFind)
                        {
                            if (0 == jobItem.SuitArmorType || jobItem.SuitArmorType - 4 + (int)ProtoTable.ItemTable.eThirdType.CLOTH == (int)current.eFilter)
                            {
                                m_kDropdown.value = i;
                                bFind = true;
                            }
                        }
                    }

                    if (m_kDropdown != null && m_kDropdown.value >= 0 && m_kDropdown.value < m_kDropdown.options.Count)
                    {
                        var currentOption = m_kDropdown.options[m_kDropdown.value] as GoodOptionData;
                        m_kDropdown.captionText.text = currentOption.text;
                        m_kGoodOptionData = currentOption;
                    }

                    m_kDropdown.onValueChanged.AddListener(_OnDropDownValueChanged);
                }
                else if (eFilter == ProtoTable.ShopTable.eFilter.SF_OCCU2)
                {
                    //小职业

                    var betterJobId = Utility.GetBetterJobId(PlayerBaseData.GetInstance().JobTableID);

                    var bettleJobList = Utility.BettleJobIds;
                    if (bettleJobList != null && bettleJobList.Count > 0)
                    {
                        GoodOptionData goodOptionData = null;
                        for (int i = 0; i < bettleJobList.Count; ++i)
                        {
                            if (bettleJobList[i] != null)
                            {
                                goodOptionData = new GoodOptionData();
                                goodOptionData.text = bettleJobList[i].Name;
                                goodOptionData.jobItem = bettleJobList[i];

                                m_kDropdown.options.Add(goodOptionData);

                                if (goodOptionData.jobItem.ID == betterJobId)
                                {
                                    m_kDropdown.value = i;
                                }
                            }
                        }

                        if (bettleJobList.Count > 0)
                        {
                            if (m_kDropdown != null
                                && m_kDropdown.value >= 0
                                && m_kDropdown.value < m_kDropdown.options.Count)
                            {
                                var currentOption = m_kDropdown.options[m_kDropdown.value] as GoodOptionData;
                                if (currentOption != null)
                                {
                                    m_kDropdown.captionText.text = currentOption.text;
                                    m_kGoodOptionData = currentOption;
                                }
                            }
                        }
                    }
                    m_kDropdown.onValueChanged.AddListener(_OnDropDownValueChanged);
                }
            }
        }

        protected void _PlayNpcSound(NpcVoiceComponent.SoundEffectType eSound)
        {
            ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (null == current)
            {
                return;
            }

            if(null == m_kShopData)
            {
                return;
            }

            var npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(m_kShopData.iLinkNpcId);
            if(null == npcItem)
            {
                return;
            }

            current.PlayNpcSound(m_kShopData.iLinkNpcId, eSound);
        }

        Text m_kHistroyRecord;
        protected void _InitDropDown()
        {
            comTimeRefresh = Utility.FindComponent<TimeRefresh>(frame, "middleback/Goods/Title/RefreshTime");
            //comTimeRefresh.Initialize();
            //comTimeRefresh.Time = m_kShopData.RefreshTime;
            //comTimeRefresh.Enable = true;
            m_kDropdown = Utility.FindComponent<Dropdown>(frame, "middleback/Goods/Title/job_select");
            //m_scrollrect = Utility.FindComponent<ClipScrollrect>(frame, "middleback/Goods/Scroll View");
            //m_scrollrect.onEndDrag.RemoveAllListeners();
            //m_scrollrect.onEndDrag.AddListener(OnDragEnd);
            //m_scrollbar = Utility.FindComponent<Scrollbar>(frame, "middleback/Goods/Scroll View/HorizenScrollbar");
            //m_scrollbar.value = 0.0f;
            m_fPreValue = 0.0f;
            m_bNeedMod = false;
            m_animation = Utility.FindComponent<DOTweenAnimation>(frame, "middleback/Goods/Scroll View/Viewport/Content");
            //m_goArrowLeft = Utility.FindChild(frame, "middleback/Goods/ArrowToL");
            //m_goArrowRight = Utility.FindChild(frame, "middleback/Goods/ArrowToR");
            m_goScrollView = Utility.FindChild(frame, "middleback/Goods/Scroll View");
            //m_goPointArray = Utility.FindChild(frame, "middleback/Goods/Scroll View/PointArray");
            //m_goPointArray.CustomActive(false);
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(m_goScrollView.transform);
            m_fScrollViewWidth = bounds.size.x;

            m_kHistroyRecord = Utility.FindComponent<Text>(frame, "middleback/Goods/HistroyRecord");
            m_kHistroyRecord.enabled = false;

            m_iCurrentPage = 0;
            m_iMaxPage = 1;
            m_iPageCount = (int)PageConfig.PC_COUNT;
            m_bNeedMod = false;

            _SetFilter(ProtoTable.ShopTable.eFilter.SF_NONE);
        }

        void _OnDropDownValueChanged(int iValue)
        {
            var shopTab = ShopTab.GetShopTab(this);
            if (null != shopTab)
            {
                if (m_kDropdown != null && m_kDropdown.value >= 0 && m_kDropdown.value < m_kDropdown.options.Count)
                {
                    GoodOptionData optionData = m_kDropdown.options[m_kDropdown.value] as GoodOptionData;
                    m_kGoodOptionData = optionData;

                    UpdateMallItemsByType();
                    m_akGoodsDataItems.Filter(new object[] { shopTab.tab.ShopType, optionData , GetShopTabFilter(shopTab.tab.ShopType) });
                }
            }
            _SortPage();
            _OnPageItemCountChanged();
        }

        void _OnPageItemCountChanged()
        {
            //m_fPreValue = m_scrollbar.value;
            //m_scrollbar.value = 0.0f;
            //_FreshModeInfo(GetEnabledCount());
            //_OnSetPageInfo();
        }

        int GetEnabledCount()
        {
            int iCount = 0;
            var enumerator = m_akGoodsDataItems.ActiveObjects.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var Value = enumerator.Current.Value;
                if (Value != null && Value.IsEnable())
                {
                    ++iCount;
                }
            }

            //var blanks = m_akBlankGoodsItems.ActiveObjects;
            //for(int i = 0; i < blanks.Count; ++i)
            //{
            //    if(blanks[i].IsEnable())
            //    {
            //        ++iCount;
            //    }
            //}
            return iCount;
        }
        #endregion

        #region shopTabs
        class ComShopTab
        {
            public IClientFrame target;
            public ShopTab tab;
        };
        class ShopTab : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            ProtoTable.ShopTable.eSubType eShopTab;
            public ProtoTable.ShopTable.eSubType ShopTabType
            {
                get
                {
                    return eShopTab;
                }
            }
            ShopFrame frame;

            Toggle toggle;
            Text label;
            Text labelCheck;
            GameObject goCheckMark;

            public override void OnDestroy()
            {
                if(toggle != null)
                {
                    toggle.onValueChanged.RemoveAllListeners();
                    toggle = null;
                }
            }

            public ProtoTable.ShopTable.eSubType ShopType
            {
               get { return eShopTab; }
            }
            public static List<ComShopTab> ms_akSelectedTabs = new List<ComShopTab>();
            public static ComShopTab GetShopTab(IClientFrame target)
            {
                var find = ms_akSelectedTabs.Find((x) =>
                {
                    return x.target == target;
                });
                return find;
            }

            public static void CreateTab(IClientFrame target)
            {
                var find = GetShopTab(target);
                if(find == null)
                {
                    find = new ComShopTab();
                    find.tab = null;
                    find.target = target;
                    ms_akSelectedTabs.Add(find);
                }
            }

            public static void Clear(IClientFrame target,bool bRemove = false)
            {
                var find = GetShopTab(target);
                if(find != null)
                {
                    if (find.tab != null)
                    {
                        find.tab.SetSelected(false);
                        find.tab = null;
                    }
                    if(bRemove)
                    {
                        find.target = null;
                        find = null;
                    }
                }
            }

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                eShopTab = (ProtoTable.ShopTable.eSubType)param[2];
                frame = param[3] as ShopFrame;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    toggle = goLocal.GetComponent<Toggle>();
                    toggle.onValueChanged.AddListener(
                        (bool bValue) =>
                        {
                            if (bValue)
                            {
                                OnSelected();
                            }
                        });

                    label = Utility.FindComponent<Text>(goLocal, "Label");
                    labelCheck = Utility.FindComponent<Text>(goLocal, "CheckMark/Label");

                    label.text = labelCheck.text = TR.Value(string.Format(TR.Value("shop_tab_format"), (int)eShopTab));

                    goCheckMark = Utility.FindChild(goLocal, "CheckMark");
                }
                Enable();
                _Update();
            }
            public override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }
            public override void OnRefresh(object[] param)
            {
                _Update();
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }
            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override bool NeedFilter(object[] param)
            {
                return false;
            }

            void _Update()
            {

            }

            void SetSelected(bool bSelected)
            {
                goCheckMark.CustomActive(bSelected);
            }

            public void OnSelected()
            {
                var shopTab = GetShopTab(frame);
                if(this != shopTab.tab)
                {
                    if(null != shopTab.tab)
                    {
                        shopTab.tab.SetSelected(false);
                    }
                    shopTab.tab = this;
                    SetSelected(true);
                }
                frame.OnShopTabChanged(eShopTab);
            }
        }
        CachedObjectDicManager<ProtoTable.ShopTable.eSubType, ShopTab> m_akShopTabObjects = new CachedObjectDicManager<ProtoTable.ShopTable.eSubType, ShopTab>();

        void _InitTabRefreshTime(ProtoTable.ShopTable.eSubType eShopTab)
        {
            bool bCanRefresh = false;
            int iFindIndex = -1;
            var shopTable = TableManager.GetInstance().GetTableItem<ProtoTable.ShopTable>(m_kShopData.ID.Value);
            if (shopTable != null && (shopTable.Refresh == 2 || shopTable.Refresh == 1) &&
                shopTable.NeedRefreshTabs.Count == shopTable.SubType.Count &&
                shopTable.SubType.Count == shopTable.RefreshCycle.Count)
            {
                for (int i = 0; i < shopTable.SubType.Count; ++i)
                {
                    if ((int)shopTable.SubType[i] == (int)eShopTab)
                    {
                        iFindIndex = i;
                        break;
                    }
                }
            }

            if (-1 != iFindIndex && shopTable.NeedRefreshTabs[iFindIndex] == 1)
            {
                bCanRefresh = true;
            }

            if (bCanRefresh)
            {
                m_resetText.enabled = false;
                switch (shopTable.RefreshCycle[iFindIndex])
                {
                    case ProtoTable.ShopTable.eRefreshCycle.REFRESH_CYCLE_DAILY:
                        m_resetText.enabled = true;
                        comTimeRefresh.Initialize();
                        comTimeRefresh.Time = m_kShopData.RefreshTime;
                        comTimeRefresh.Enable = true;
                        break;
                    case ProtoTable.ShopTable.eRefreshCycle.REFRESH_CYCLE_WEEK:
                        m_resetText.enabled = true;
                        m_resetText.text = TR.Value("shop_refresh_week_hint");
                        comTimeRefresh.Initialize();
                        comTimeRefresh.Time = m_kShopData.WeekRefreshTime;
                        comTimeRefresh.Enable = true;
                        break;
                    case ProtoTable.ShopTable.eRefreshCycle.REFRESH_CYCLE_MONTH:
                        m_resetText.enabled = true;
                        comTimeRefresh.Initialize();
                        comTimeRefresh.Time = m_kShopData.MonthRefreshTime;
                        comTimeRefresh.Enable = true;
                        break;
                }
            }
            else if(shopTable.Refresh == 1)
            {
                m_resetText.enabled = true;
                comTimeRefresh.Initialize();
                comTimeRefresh.Time = m_kShopData.RefreshTime;
                comTimeRefresh.Enable = true;
            }
            else
            {
                comTimeRefresh.Initialize();
                m_resetText.enabled = false;
            }
        }

        ProtoTable.ShopTable.eFilter GetShopTabFilter(ProtoTable.ShopTable.eSubType eShopTab)
        {
            ProtoTable.ShopTable.eFilter eFilter = ProtoTable.ShopTable.eFilter.SF_NONE;
            if(null != m_kShopData)
            {
                var shop = TableManager.GetInstance().GetTableItem<ProtoTable.ShopTable>((int)m_kShopData.ID.Value);
                if(null != shop)
                {
                    int iFindIndex = -1;
                    for(int i = 0; i < shop.SubType.Count; ++i)
                    {
                        if(shop.SubType[i] == eShopTab)
                        {
                            iFindIndex = i;
                            break;
                        }
                    }

                    if(iFindIndex >= 0 && iFindIndex < shop.Filter.Count)
                    {
                        eFilter = shop.Filter[iFindIndex];
                    }
                }
            }
            return eFilter;
        }

        void OnShopTabChanged(ProtoTable.ShopTable.eSubType eShopTab)
        {
            m_akMoneyIds.Clear();

            _SetFilter(GetShopTabFilter(eShopTab));

            var goodDatas = m_kShopData.Goods.FindAll(x => { return x.Type == eShopTab; });
            if(goodDatas != null)
            {
                for (int i = 0; i < goodDatas.Count; ++i)
                {
                    _TryToCreateMoneyObject((int)goodDatas[i].CostItemData.TableID);
                }

                if(m_kShopData.NeedRefresh.HasValue && m_kShopData.NeedRefresh.Value == 1)
                {
                    int iBindPoint = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindPOINT);
                    _TryToCreateMoneyObject(iBindPoint);
                }
            }

            _TryAddExtraMoneyToTab((int)eShopTab,ref m_akMoneyIds);

            m_akMoneyItemObjects.RecycleAllObject();
            m_akMoneyIds.Sort((x, y) => 
            {
                var left = System.Array.Find(ms_money_sorts, (target) => { return target.iId == x; });
                var right = System.Array.Find(ms_money_sorts, (target) => { return target.iId == y; });
                if((left == null) != (right == null))
                {
                    return left == null ? -1 : 1;
                }
                else if(left == null)
                {
                    return x - y;
                }

                return left.iOrder - right.iOrder;
            });
            m_akMoneyIds.ForEach(money =>
            {
                m_akMoneyItemObjects.Create(money, new object[]
                {
                            m_goMoneyParent,
                            m_goMoneyPrefab,
                            money,
                            (ms_money_show_name.ToList().Contains(money) ? MoneyBinder.MoneyShowType.MST_MONEY_NAME : MoneyBinder.MoneyShowType.MST_NORMAL),
                            this
                });
            });

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenShopRefreshConsumeItem, m_akMoneyIds);

            //m_akMoneyItemObjects.Filter(new object[] { goodDatas });
            UpdateMallItemsByType();
            m_akGoodsDataItems.Filter(new object[] { eShopTab, m_kGoodOptionData , GetShopTabFilter(eShopTab) });

            //_ControlRefreshTime(eShopTab);
            _InitTabRefreshTime(eShopTab);
            //m_akBlankGoodsItems.Filter(new object[] { eShopTab });

            _CheckSetParticular(goodDatas);

            _SortPage();
            _OnPageItemCountChanged();
        }

        void _CheckSetParticular(List<GoodsData> datas)
        {
            m_kHistroyRecord.enabled = false;
            for(int i = 0; i < datas.Count; ++i)
            {
                if(datas[i].eGoodsLimitButyType == GoodsLimitButyType.GLBT_FIGHT_SCORE)
                {
                    m_kHistroyRecord.enabled = true;
                    //int iCurValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_MAX_FIGHT_SCORE);
                    int iCurValue = SeasonDataManager.GetInstance().seasonLevel;
                    var rankName = SeasonDataManager.GetInstance().GetRankName(iCurValue);
                    m_kHistroyRecord.text = string.Format(TR.Value("shop_max_fight_score"), rankName);
                    break;
                }

                if(datas[i].eGoodsLimitButyType == GoodsLimitButyType.GLBT_TOWER_LEVEL)
                {
                    m_kHistroyRecord.enabled = true;
                    int iCurValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_MAX_DEATH_TOWER_LEVEL);
                    m_kHistroyRecord.text = string.Format(TR.Value("shop_max_tower_level"), iCurValue);
                    break;
                }
            }
        }

        void _TryToCreateMoneyObject(int iTableID)
        {
            if(!m_akMoneyIds.Contains(iTableID))
            {
                m_akMoneyIds.Add(iTableID);

                var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iTableID);
                if (itemTable != null)
                {
                    for (int j = 0; j < itemTable.RelationID.Count; ++j)
                    {
                        if (itemTable.RelationID[j] > 0 && !m_akMoneyIds.Contains((int)itemTable.RelationID[j]))
                        {
                            m_akMoneyIds.Add((int)itemTable.RelationID[j]);
                        }
                    }
                }
            }
        }

        void _TryAddExtraMoneyToTab(int iTab,ref List<int> MoneyIDs)
        {
            var shopItem = TableManager.GetInstance().GetTableItem<ProtoTable.ShopTable>(m_kShopData.ID.Value);
            if (null != shopItem)
            {
                var tokens = shopItem.ExtraShowMoneys.Split(',');
                if(tokens.Length != shopItem.SubType.Count)
                {
                    return;
                }

                int find = -1;
                for(int i = 0; i < shopItem.SubType.Count; ++i)
                {
                    if(iTab == (int)shopItem.SubType[i])
                    {
                        find = i;
                        break;
                    }
                }

                if(-1 == find)
                {
                    return;
                }

                var moneys = tokens[find].Split('|');
                for(int i = 0; i < moneys.Length; ++i)
                {
                    int moneyId = 0;
                    if(!int.TryParse(moneys[i],out moneyId))
                    {
                        continue;
                    }

                    var moneyItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(moneyId);
                    if(moneyItem != null)
                    {
                        if(!MoneyIDs.Contains(moneyId))
                        {
                            MoneyIDs.Add(moneyId);
                        }
                    }
                }
            }
        }

        ProtoTable.ShopTable.eSubType m_eGoodType = ProtoTable.ShopTable.eSubType.ST_NONE;
        void _InitShopTabs()
        {
            ShopTab.CreateTab(this);

            GameObject goParentChild = Utility.FindChild(frame, "middleback/MainTabChild");
            GameObject goPrefabsChild = Utility.FindChild(frame, "middleback/MainTabChild/Prefab");
            goPrefabsChild.CustomActive(false);
            GameObject goParent = Utility.FindChild(frame, "middleback/MainTab");
            GameObject goPrefabs = Utility.FindChild(frame, "middleback/MainTab/Prefab");
            goPrefabs.CustomActive(false);

            m_kShopData.GoodsTypes.Sort((x, y) =>
            {
                return x - y;
            });
            for (int i = 0; i < m_kShopData.GoodsTypes.Count; ++i)
            {
                if(m_eShopFrameMode != ShopFrameMode.SFM_MAIN_FRAME)
                {
                    m_akShopTabObjects.Create(m_kShopData.GoodsTypes[i], new object[]
                    {
                        goParentChild,goPrefabsChild,m_kShopData.GoodsTypes[i],this
                    });
                }
                else
                {
                    m_akShopTabObjects.Create(m_kShopData.GoodsTypes[i], new object[]
                    {
                        goParent,goPrefabs,m_kShopData.GoodsTypes[i],this
                    });
                }
            }
        }
        #endregion
        #region moneyObjects
        class MoneyItemObject : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            int iTableID;
            ShopFrame frame;

            Image kIcon;
            Text kNum;
            Text kName;
            ItemComeLink comItemLink;
            MoneyBinder comMoneyBinder;
            ItemData itemData;
            Button btnCoinTips;
            bool bLocked;
            MoneyBinder.MoneyShowType eMoneyBinder = MoneyBinder.MoneyShowType.MST_NORMAL;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                iTableID = (int)param[2];
                eMoneyBinder = (MoneyBinder.MoneyShowType)param[3];
                frame = param[4] as ShopFrame;
                itemData = GameClient.ItemDataManager.CreateItemDataFromTable(iTableID);
                bLocked = false;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    kIcon = Utility.FindComponent<Image>(goLocal, "Icon");
                    kNum = Utility.FindComponent<Text>(goLocal, "Cnt");
                    kName = Utility.FindComponent<Text>(goLocal, "Name");
                    comItemLink = Utility.FindComponent<ItemComeLink>(goLocal, "Add");
                    comItemLink.bNotEnough = false;
                    comItemLink.onClick += _OnClick;

                    btnCoinTips = Utility.FindComponent<Button>(goLocal, "btnCoinTips");
                    btnCoinTips.onClick.RemoveAllListeners();
                    btnCoinTips.onClick.AddListener(() =>
                    {
                        if(itemData != null && !bLocked)
                        {
                            ItemTipManager.GetInstance().CloseAll();
                            ItemTipManager.GetInstance().ShowTip(itemData);
                            bLocked = true;
                            InvokeMethod.Invoke(this,0.30f, _UnLock);
                        }
                    });
                }
                Enable();
                _Update();
            }

            void _UnLock()
            {
                bLocked = false;
            }

            void _OnClick()
            {
                if(frame != null)
                {
                    switch(frame.EShopFrameMode)
                    {
                        case ShopFrameMode.SFM_MAIN_FRAME:
                            {
                                frame.Close();
                            }
                            break;
                        case ShopFrameMode.SFM_CHILD_FRAME:
                            {
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ShopMainFrameClose);
                            }
                            break;
                        case ShopFrameMode.SFM_GUILD_CHILD_FRAME:
                            {
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ShopGuildFrameClose);
                            }
                            break;
                        case ShopFrameMode.SFM_MALL_CHILD_FRAME:
                            {
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ShopMallFrameClose);
                            }
                            break;
                    }
                }
            }

            public override void OnDestroy()
            {
                comItemLink.onClick -= _OnClick;
                btnCoinTips.onClick.RemoveAllListeners();
                btnCoinTips = null;
                InvokeMethod.RemoveInvokeCall(this);
                bLocked = false;
            }

            public override void OnRecycle()
            {
                InvokeMethod.RemoveInvokeCall(this);
                bLocked = false;
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }
            public override void OnRefresh(object[] param)
            {
                _Update();
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }
            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }

            public override bool NeedFilter(object[] param)
            {
                List<GoodsData> goodDatas = param[0] as List<GoodsData>;
                if(goodDatas != null)
                {
                    var find = goodDatas.Find(x => 
                    {
                        if(x.CostItemData.TableID == iTableID)
                        {
                            return true;
                        }

                        var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)x.CostItemData.TableID);
                        if(item != null)
                        {
                            return item.RelationID.Contains(iTableID);
                        }

                        return false;
                    });

                    return find == null;
                }
                return true;
            }

            void _Update()
            {
                comItemLink.iItemLinkID = iTableID;
                // kIcon.sprite = AssetLoader.instance.LoadRes(itemData.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref kIcon, itemData.Icon);
                comMoneyBinder = MoneyBinder.Create(goLocal, kIcon,kNum,kName,iTableID,eMoneyBinder);
            }
        }
        CachedObjectDicManager<int, MoneyItemObject> m_akMoneyItemObjects = new CachedObjectDicManager<int, MoneyItemObject>();
        List<int> m_akMoneyIds = new List<int>();
        GameObject m_goMoneyParent;
        GameObject m_goMoneyPrefab;
        void _InitMoney()
        {
            m_goMoneyParent = Utility.FindChild(frame, "ComMoneys/Title/Moneys");
            m_goMoneyPrefab = Utility.FindChild(m_goMoneyParent, "MoneyObject");
            m_goMoneyPrefab.CustomActive(false);

            _UpdateJarScore();
        }

        void _UpdateJarScore()
        {
            ShopFrameData frameData = userData as ShopFrameData;
            if (null != frameData && frameData.m_kShopData != null && jarScore != null)
            {
                jarScore.enabled = false;
                if (8 == frameData.m_kShopData.ID.Value)
                {
                    jarScore.text = TR.Value("jar_shop_gold_jar_score", PlayerBaseData.GetInstance().GoldJarScore);
                    jarScore.enabled = true;
                }
                else if (7 == frameData.m_kShopData.ID.Value)
                {
                    jarScore.text = TR.Value("jar_shop_magic_jar_score", PlayerBaseData.GetInstance().MagicJarScore);
                    jarScore.enabled = true;
                }
                jarScore.text = TR.Value("jar_shop_refresh_time");
            }
        }

        void _UpdateFriendlyHint()
        {
            bool bVisible = false;
            ShopFrameData frameData = userData as ShopFrameData;
            if(null != frameData && null != frameData.m_kShopData)
            {
                int iValue = frameData.m_kShopData.ID.HasValue ? frameData.m_kShopData.ID.Value : 0;
                bVisible = 11 == iValue;
            }
            m_friendlyHint.CustomActive(bVisible);
        }
        #endregion
        #region goodsData
        GameObject m_goGoodsDataParent;
        GameObject m_goGoodsDataPefabs;
        //GameObject m_goGoodsBlankPrefabs;

        void OnAddGoodsData(GoodsData data)
        {
            m_akGoodsDataItems.Create((ulong)data.ID, new object[] { m_goGoodsDataParent, m_goGoodsDataPefabs ,data,this});
        }
        class GoodsDataItem : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            GoodsData goodsData;
            public GoodsData GoodsData
            {
                get { return goodsData; }
            }
            ShopFrame frame;
            //UIGray comGray;
            public int SortID
            {
                get;set;
            }

            Text name;
            Text limitName;
            Text price;
            Color colorPrice;
            Image icon;
            Text moneyName;
            ComItem comItem;
            Button btnBuy2;
            UIGray gray2;
            //GameObject goCurrentPrice;
            //GameObject goNoneLmtPrice;

            GameObject goVip;
            Text vipLevel;

            GameObject goOtherHint;
            //Text otherPrice;
            //Color colorOtherPrice;
            //Image otherIcon;
            Text otherHintText;
            Text discount;
            GameObject goDisCount;

            GameObject goSellOver;
            GameObject goNormalPrice;

            GameObject timesLmt;
            Text timesHint;

            GameObject goOrgBtBuy;
            Button btnBuy;
            Button btnOtherHint;

            GoodsData.GoodsDataShowType showType;

            GameObject goCheckCanBuyMask;

            GameObject goOldChangeNew;

            public override void OnDestroy()
            {
                if(btnBuy != null)
                {
                    btnBuy.onClick.RemoveAllListeners();
                    btnBuy = null;
                }
                if(null != btnBuy2)
                {
                    btnBuy2.onClick.RemoveAllListeners();
                    btnBuy2 = null;
                }
                if(btnOtherHint != null)
                {
                    btnOtherHint.onClick.RemoveAllListeners();
                    btnOtherHint = null;
                }
                if(comItem != null)
                {
                    comItem.Setup(null, null);
                    comItem = null;
                }
            }

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                goodsData = param[2] as GoodsData;
                frame = param[3] as ShopFrame;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    name = Utility.FindComponent<Text>(goLocal, "titleback/name");
                    price = Utility.FindComponent<Text>(goLocal, "btBuy/Horizen/curprice");
                    colorPrice = price.color;
                    icon = Utility.FindComponent<Image>(goLocal, "btBuy/Horizen/TicketIcon");
                    moneyName = Utility.FindComponent<Text>(goLocal, "btBuy/Horizen/MoneyName");
                    comItem = frame.CreateComItem(Utility.FindChild(goLocal, "ItemParent"));

                    goVip = Utility.FindChild(goLocal, "VipMark");
                    vipLevel = Utility.FindComponent<Text>(goVip, "Text");

                    goOtherHint = Utility.FindChild(goLocal, "OtherHint");
                    otherHintText = Utility.FindComponent<Text>(goOtherHint, "Text");

                    goSellOver = Utility.FindChild(goLocal, "btBuy/SellOver");
                    goNormalPrice = Utility.FindChild(goLocal, "btBuy/Horizen");

                    timesLmt = Utility.FindChild(goLocal, "titleback/timesLmt");
                    timesHint = Utility.FindComponent<Text>(timesLmt, "Text");
                    limitName = Utility.FindComponent<Text>(timesLmt, "name");

                    btnBuy = goLocal.GetComponent<Button>();
                    btnBuy.onClick.AddListener(OnClickBuy);

                    btnBuy2 = Utility.FindComponent<Button>(goLocal, "btBuy");
                    btnBuy2.onClick.AddListener(OnClickBuy);
                    gray2 = Utility.FindComponent<UIGray>(goLocal, "btBuy");

                    goOrgBtBuy = Utility.FindChild(goLocal,"btBuy");

                    btnOtherHint = Utility.FindComponent<Button>(goLocal, "OtherHint");
                    btnOtherHint.onClick.AddListener(OnClickBuy);

                    //comGray = Utility.FindComponent<UIGray>(goLocal, "btBuy");

                    discount = Utility.FindComponent<Text>(goLocal, "vipCountInfo/discount");
                    goDisCount = Utility.FindChild(goLocal, "vipCountInfo");

                    goCheckCanBuyMask = Utility.FindChild(goLocal, "CanNotBuyMask");

                    goOldChangeNew = Utility.FindChild(goLocal, "btBuy/Horizen/oldChangeNew");
                }
                Enable();
                _Update();
            }
            public override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }
            public override void OnRefresh(object[] param)
            {
                if(param != null && param.Length > 0)
                {
                    goodsData = param[0] as GoodsData;
                }
                
                _Update();
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }

            public override void SetAsLastSibling()
            {
                if (goLocal != null)
                {
                    goLocal.transform.SetAsLastSibling();
                }
            }

            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override bool NeedFilter(object[] param)
            {
                if(goodsData == null)
                {
                    return true;
                }

                if(param == null || param.Length != 3)
                {
                    return true;
                }

                if(goodsData.Type != (ProtoTable.ShopTable.eSubType)param[0])
                {
                    return true;
                }

                var optionData = param[1] as GoodOptionData;
                if(optionData == null)
                {
                    return false;
                }

                ProtoTable.ShopTable.eFilter eFilter = (ProtoTable.ShopTable.eFilter)param[2];
                if(eFilter == ProtoTable.ShopTable.eFilter.SF_OCCU)
                {
                    bool bFind = false;
                    ProtoTable.ItemTable item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)goodsData.ItemData.TableID);
                    if (item != null && item.Occu.Count > 0)
                    {
                        for (int j = 0; j < item.Occu.Count; ++j)
                        {
                            if (item.Occu[j] / 10 * 10 == optionData.jobItem.ID || item.Occu[j] == 0)
                            {
                                bFind = true;
                                break;
                            }
                        }
                    }

                    if(!bFind)
                    {
                        return true;
                    }
                }
                else if (eFilter == ProtoTable.ShopTable.eFilter.SF_ARMOR)
                {
                    ProtoTable.ItemTable item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)goodsData.ItemData.TableID);
                    if(item != null)
                    {
                        return goodsData.ItemData.ThirdType != optionData.eFilter;
                    }
                    return true;
                }
                else if (eFilter == ProtoTable.ShopTable.eFilter.SF_OCCU2)
                {
                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)goodsData.ItemData.TableID);
                    if (item == null)
                        return true;
                    if (item.Occu2.Count <= 0)
                        return true;
                    for (var i = 0; i < item.Occu2.Count; i++)
                    {
                        if (optionData.jobItem.ID == item.Occu2[i])
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }

            public bool IsEnable()
            {
                return goLocal != null && goLocal.activeSelf;
            }

            void OnItemClicked(GameObject obj, ItemData item)
            {
                if (item != null)
                {
                    if(_CheckCanBuy(GoodsData))
                    {
                        List<TipFuncButon> funcs = new List<TipFuncButon>();
                        TipFuncButonSpecial tempfunc = new TipFuncButonSpecial();
                        tempfunc.text = TR.Value("tip_buy");
                        tempfunc.callback = (ItemData data, object param1)=>
                        {
                            if(null != data)
                            {
                                var goodsData = data.userData as GoodsData;
                                if (null != goodsData)
                                {
                                    this.OnClickBuy();
                                }
                            }
                        };
                        funcs.Add(tempfunc);
                        ItemTipManager.GetInstance().ShowTip(item);
                    }
                    else
                    {
                        ItemTipManager.GetInstance().ShowTip(item);
                    }
                }
            }

            public void OnClickBuy()
            {
                if (goodsData.eGoodsLimitButyType != GoodsLimitButyType.GLBT_NONE)
                {
                    if (goodsData.eGoodsLimitButyType == GoodsLimitButyType.GLBT_TOWER_LEVEL)
                    {
                        int iCurValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_MAX_DEATH_TOWER_LEVEL);
                        if (iCurValue < goodsData.iLimitValue)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("shop_buy_need_tower_level"), goodsData.iLimitValue));
                            return;
                        }
                    }
                    else if (goodsData.eGoodsLimitButyType == GoodsLimitButyType.GLBT_FIGHT_SCORE)
                    {
                        int iCurValue = SeasonDataManager.GetInstance().seasonLevel;
                        if (iCurValue < goodsData.iLimitValue)
                        {
                            var rankName = SeasonDataManager.GetInstance().GetRankName(goodsData.iLimitValue);
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("shop_buy_need_fight_score"), rankName));
                            return;
                        }
                    }
                    else if(goodsData.eGoodsLimitButyType == GoodsLimitButyType.GLBT_GUILD_LEVEL)
                    {
                        int iCurValue = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.SHOP);
                        if(iCurValue < goodsData.iLimitValue)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("shop_buy_need_guild_level"), goodsData.iLimitValue));
                            return;
                        }
                    }
                }

                if(goodsData.VipLimitLevel > 0 && goodsData.VipLimitLevel > PlayerBaseData.GetInstance().VipLevel)
                {
                    SystemNotifyManager.SystemNotify(1800011, () =>
                     {
                         var vipFrame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
                         vipFrame.OpenPayTab();
                     });
                    return;
                }
                frame._OnGoodsClicked(goodsData);
            }

            void _UpdateLimitBuy()
            {
                try
                {
                    bool bCanRefresh = false;
                    int iFindIndex = -1;
                    var shopTable = TableManager.GetInstance().GetTableItem<ProtoTable.ShopTable>(frame.m_kShopData.ID.Value);
                    if (shopTable != null && (shopTable.Refresh == 2 ||
                        shopTable.Refresh == 1)&&
                        shopTable.NeedRefreshTabs.Count == shopTable.SubType.Count &&
                        shopTable.SubType.Count == shopTable.RefreshCycle.Count)
                    {
                        for (int i = 0; i < shopTable.SubType.Count; ++i)
                        {
                            if ((int)shopTable.SubType[i] == (int)goodsData.shopItem.SubType)
                            {
                                iFindIndex = i;
                                break;
                            }
                        }
                    }

                    if (-1 != iFindIndex && shopTable.NeedRefreshTabs[iFindIndex] == 1)
                    {
                        bCanRefresh = true;
                    }

                    if (bCanRefresh)
                    {
                        switch (shopTable.RefreshCycle[iFindIndex])
                        {
                            case ProtoTable.ShopTable.eRefreshCycle.REFRESH_CYCLE_NONE:
                                timesHint.text = TR.Value("shop_item_limit_buy_forever", goodsData.LimitBuyTimes);
                                break;
                            case ProtoTable.ShopTable.eRefreshCycle.REFRESH_CYCLE_DAILY:
                                timesHint.text = TR.Value("shop_item_limit_buy_daily", goodsData.LimitBuyTimes);
                                if(shopTable.Refresh == 1)
                                {
                                    timesHint.text = TR.Value("shop_item_limit_buy_mystery", goodsData.LimitBuyTimes);
                                }
                                break;
                            case ProtoTable.ShopTable.eRefreshCycle.REFRESH_CYCLE_WEEK:
                                timesHint.text = TR.Value("shop_item_limit_buy_weekly", goodsData.LimitBuyTimes);
                                break;
                            case ProtoTable.ShopTable.eRefreshCycle.REFRESH_CYCLE_MONTH:
                                timesHint.text = TR.Value("shop_item_limit_buy_monthly", goodsData.LimitBuyTimes);
                                break;
                        }
                    }
                    else if (goodsData.LimitBuyTimes > 0)
                    {
                        timesHint.text = TR.Value("shop_item_limit_buy_forever", goodsData.LimitBuyTimes);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogErrorFormat(e.ToString());
                }
            }

            bool _CheckCanBuy(GoodsData goodsData)
            {
                bool bCanBuy = true;
                switch (goodsData.eGoodsLimitButyType)
                {
                    case GoodsLimitButyType.GLBT_NONE:
                        {

                        }
                        break;
                    case GoodsLimitButyType.GLBT_TOWER_LEVEL:
                        {
                            int iCurValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_MAX_DEATH_TOWER_LEVEL);
                            bCanBuy = iCurValue >= goodsData.iLimitValue;
                        }
                        break;
                    case GoodsLimitButyType.GLBT_FIGHT_SCORE:
                        {
                            //int iCurValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_MAX_FIGHT_SCORE);
                            int iCurValue = SeasonDataManager.GetInstance().seasonLevel;
                            bCanBuy = iCurValue >= goodsData.iLimitValue;
                        }
                        break;
                    case GoodsLimitButyType.GLBT_GUILD_LEVEL:
                        {
                            int iCurValue = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.SHOP);
                            bCanBuy = iCurValue >= goodsData.iLimitValue;
                            break;
                        }
                }
                //step_2 check vip level
                if (bCanBuy)
                {
                    if (GoodsData.VipLimitLevel.HasValue && GoodsData.VipLimitLevel.Value > 0 && GoodsData.VipLimitLevel.Value > PlayerBaseData.GetInstance().VipLevel)
                    {
                        bCanBuy = false;
                    }
                }

                if(bCanBuy)
                {
                    if(goodsData.LimitBuyTimes <= 0)
                    {
                        bCanBuy = false;
                    }
                }

                return bCanBuy;
            }

            void _Update()
            {
                _UpdateLimitBuy();

                showType = goodsData.GetShowType(PlayerBaseData.GetInstance().VipLevel);

                goodsData.ItemData.userData = goodsData;
                comItem.Setup(goodsData.ItemData, OnItemClicked);

                name.text = limitName.text = goodsData.ItemData.GetColorName();
                goodsData.ItemData.userData = goodsData;

                // icon.sprite = AssetLoader.instance.LoadRes(goodsData.CostItemData.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref icon, goodsData.CostItemData.Icon);
                moneyName.text = goodsData.CostItemData.Name;
                bool show = ms_money_show_name.ToList().Contains(goodsData.CostItemData.TableID);
                moneyName.CustomActive(show);
                icon.CustomActive(!show);

                bool bMoneyEnough = false;
                int iCurCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)goodsData.CostItemData.TableID);
                price.text = goodsData.CostItemCount.Value.ToString();
                price.color = iCurCount >= goodsData.CostItemCount ? colorPrice : Color.red;
                bMoneyEnough = iCurCount >= goodsData.CostItemCount;
                //otherPrice.text = price.text;
                //otherPrice.color = iCurCount >= goodsData.CostItemCount ? colorOtherPrice : Color.red;

                //hide all
                timesLmt.CustomActive(goodsData.LimitBuyTimes > 0);
                name.CustomActive(!timesLmt.activeSelf);
                //goCurrentPrice.CustomActive(timesLmt.activeSelf);
                //goNoneLmtPrice.CustomActive(!timesLmt.activeSelf);
                goSellOver.CustomActive(goodsData.LimitBuyTimes == 0);
                //comGray.enabled = goodsData.LimitBuyTimes == 0;
                btnBuy.enabled = !(goodsData.LimitBuyTimes == 0);
                btnBuy2.enabled = btnBuy.enabled;
                gray2.enabled = !btnBuy2.enabled;
                goNormalPrice.CustomActive(!goSellOver.activeSelf);

                bool bCanBuy = true;

                //购买安扭文本
                //step_1 check LimitBuyType
                switch (goodsData.eGoodsLimitButyType)
                {
                    case GoodsLimitButyType.GLBT_NONE:
                        {
                            goOtherHint.CustomActive(false);
                        }
                        break;
                    case GoodsLimitButyType.GLBT_TOWER_LEVEL:
                        {
                            int iCurValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_MAX_DEATH_TOWER_LEVEL);
                            goOtherHint.CustomActive(iCurValue < goodsData.iLimitValue);
                            otherHintText.text = string.Format(TR.Value("shop_refresh_need_tower_level"), goodsData.iLimitValue);
                            bCanBuy = iCurValue >= goodsData.iLimitValue;
                        }
                        break;
                    case GoodsLimitButyType.GLBT_FIGHT_SCORE:
                        {
                            //int iCurValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_MAX_FIGHT_SCORE);
                            int iCurValue = SeasonDataManager.GetInstance().seasonLevel;
                            goOtherHint.CustomActive(iCurValue < goodsData.iLimitValue);
                            var rankName = SeasonDataManager.GetInstance().GetRankName(goodsData.iLimitValue);
                            otherHintText.text = string.Format(TR.Value("shop_refresh_need_fight_score"),rankName);
                            bCanBuy = iCurValue >= goodsData.iLimitValue;
                        }
                        break;
                    case GoodsLimitButyType.GLBT_GUILD_LEVEL:
                        {
                            int iCurValue = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.SHOP);
                            goOtherHint.CustomActive(iCurValue < goodsData.iLimitValue);
                            otherHintText.text = string.Format(TR.Value("shop_refresh_need_guild_level"), goodsData.iLimitValue);
                            bCanBuy = iCurValue >= goodsData.iLimitValue;
                            break;
                        }
                }

                goVip.CustomActive(showType == GoodsData.GoodsDataShowType.GDST_VIP);
                goOrgBtBuy.CustomActive(!goOtherHint.activeSelf);

                //if (goodsData.shopItem.LimiteOnce == 0)
                //{
                //    timesHint.text = TR.Value("shop_item_limit_buy_forever", goodsData.LimitBuyTimes);
                //}
                //else
                //{
                //    timesHint.text = TR.Value("shop_item_limit_buy_daily", goodsData.LimitBuyTimes);
                //}

                if (showType == GoodsData.GoodsDataShowType.GDST_LIMIT_COUNT)
                {
                    //timesHint.text = string.Format("限{0}次", goodsData.LimitBuyTimes);
                }
                else if (showType == GoodsData.GoodsDataShowType.GDST_VIP)
                {
                    vipLevel.text = string.Format(TR.Value("VipFormat"), goodsData.VipLimitLevel);
                    //vipPrice.text = TR.Value("shop_goods_discount", goodsData.VipDiscount / 10.0f);
                    
                    //int iDiscount = Mathf.CeilToInt(goodsData.VipDiscount.Value / 100.0f * goodsData.CostItemCount.Value);
                    //price.text = iDiscount.ToString();
                    //price.color = iCurCount >= goodsData.CostItemCount ? colorPrice : Color.red;
                    //otherPrice.text = price.text;
                    //otherPrice.color = iCurCount >= goodsData.CostItemCount ? colorOtherPrice : Color.red;
                }

                //step_2 check vip level
                if (bCanBuy && GoodsData.VipLimitLevel.HasValue && GoodsData.VipLimitLevel.Value > 0 && GoodsData.VipLimitLevel.Value > PlayerBaseData.GetInstance().VipLevel)
                {
                    bCanBuy = false;
                }

                goDisCount.CustomActive(goodsData.VipDiscount.HasValue && goodsData.VipDiscount.Value < 100 && goodsData.VipDiscount.Value > 0);
                if (goodsData.VipDiscount.HasValue && goodsData.VipDiscount.Value < 100 && goodsData.VipDiscount.Value > 0)
                {
                    int iDiscount = Mathf.CeilToInt(goodsData.VipDiscount.Value / 100.0f * goodsData.CostItemCount.Value);
                    price.text = iDiscount.ToString();
                    price.color = iCurCount >= goodsData.CostItemCount ? colorPrice : Color.red;
                    discount.text = TR.Value("shop_item_discount_info", goodsData.VipDiscount.Value);
                    bMoneyEnough = iCurCount >= goodsData.CostItemCount;
                }

                goCheckCanBuyMask.CustomActive(!bCanBuy);

                bool bIsFlag = ShopDataManager.GetInstance()._IsShowOldChangeNew(goodsData) && !goSellOver.activeSelf;

                goOldChangeNew.CustomActive(bIsFlag);

                if (bIsFlag)
                {
                    ComOldChangeNewItem item = goOldChangeNew.GetComponent<ComOldChangeNewItem>();
                    item.SetItemId(goodsData.shopItem.ID);
                }
                
            }
        }

        CachedObjectDicManager<ulong, GoodsDataItem> m_akGoodsDataItems = new CachedObjectDicManager<ulong, GoodsDataItem>();
        IEnumerator m_iEnumerator = null;
        void _InitGoodsData()
        {
            if(m_iEnumerator != null)
            {
                StopCoroutine(m_iEnumerator);
                m_iEnumerator = null;
            }
            m_goGoodsDataParent = Utility.FindChild(frame, "middleback/Goods/Scroll View/Viewport/Content");
            m_goGoodsDataPefabs = Utility.FindChild(m_goGoodsDataParent, "Prefab");
            m_goGoodsDataPefabs.CustomActive(false);
            if (m_kShopData != null)
            {
                m_iEnumerator = StartCoroutine(_AnsyCreateGoodsData());
            }
        }

        IEnumerator _AnsyCreateGoodsData()
        {
            var shopTab = ShopTab.GetShopTab(this);

            m_kShopData.Goods.Sort((x, y) =>
            {
                if (m_iShopLinkID != 0)
                {
                    if (((int)x.ItemData.TableID == m_iShopLinkID) != ((int)y.ItemData.TableID == m_iShopLinkID))
                    {
                        return ((int)x.ItemData.TableID == m_iShopLinkID) ? -1 : 1;
                    }
                }

                if (x.shopItem.SubType != y.shopItem.SubType)
                {
                    if (m_iShopTabID != -1)
                    {
                        bool bLeftEqual = ((int)x.shopItem.SubType) == m_iShopTabID;
                        bool bRightEqual = ((int)y.shopItem.SubType) == m_iShopTabID;
                        if (bLeftEqual != bRightEqual)
                        {
                            bool bEqual = ((int)x.shopItem.SubType) == m_iShopTabID;
                            return bEqual ? -1 : 1;
                        }
                    }

                    return (int)x.shopItem.SubType - (int)y.shopItem.SubType;
                }

                return x.shopItem.SortID - y.shopItem.SortID;
            });
            yield return Yielders.EndOfFrame;

            if (m_kShopData.Goods.Count > 0)
            {
                ProtoTable.ShopTable.eSubType eCurrentTab = (ProtoTable.ShopTable.eSubType)m_kShopData.Goods[0].Type;
                //设置当前Tab页
                m_akShopTabObjects.GetObject(eCurrentTab).OnSelected();
                var Filter = GetShopTabFilter(eCurrentTab);

                //先创建当前Tab页相关物品
                m_akGoodsDataItems.RecycleAllObject();

                List<GoodsData> tempGoods = new List<GoodsData>();
                for (int i = 0; i < m_kShopData.Goods.Count; ++i)
                {
                    if (m_kShopData.Goods[i].Type != eCurrentTab)
                    {
                        continue;
                    }
                    //职业过滤
                    GoodOptionData optionData = null;
                    if (m_kDropdown.value >= 0 && m_kDropdown.value < m_kDropdown.options.Count)
                    {
                        optionData = m_kDropdown.options[m_kDropdown.value] as GoodOptionData;
                    }

                    ProtoTable.ItemTable item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)m_kShopData.Goods[i].ItemData.TableID);
                    if (item == null)
                    {
                        continue;
                    }

                    if (optionData != null)
                    {
                        if (Filter == ProtoTable.ShopTable.eFilter.SF_OCCU)
                        {
                            if (item.Occu.Count > 0)
                            {
                                bool bFind = false;
                                for (int j = 0; j < item.Occu.Count; ++j)
                                {
                                    if (item.Occu[j] / 10 * 10 == optionData.jobItem.ID || item.Occu[j] == 0)
                                    {
                                        bFind = true;
                                        break;
                                    }
                                }
                                if (!bFind)
                                {
                                    continue;
                                }
                            }
                        }
                        else if (Filter == ProtoTable.ShopTable.eFilter.SF_ARMOR)
                        {
                            if (optionData.eFilter != m_kShopData.Goods[i].ItemData.ThirdType)
                            {
                                continue;
                            }
                        }
                        else if (Filter == ProtoTable.ShopTable.eFilter.SF_OCCU2)
                        {

                            if (item.Occu2.Count > 0)
                            {
                                bool bFind = false;
                                for (int j = 0; j < item.Occu2.Count; j++)
                                {
                                    if (item.Occu2[j] == optionData.jobItem.ID)
                                    {
                                        bFind = true;
                                        break;
                                    }
                                }

                                if (bFind == false)
                                    continue;
                            }
                        }
                    }

                    OnAddGoodsData(m_kShopData.Goods[i]);

                    yield return Yielders.EndOfFrame;
                    yield return Yielders.EndOfFrame;
                }
                
            }

            ////创建其他的
            //for (int i = 0; i < m_kShopData.Goods.Count; ++i)
            //{
            //    if(m_akGoodsDataItems.HasObject((ulong)m_kShopData.Goods[i].ID))
            //    {
            //        continue;
            //    }

            //    OnAddGoodsData(m_kShopData.Goods[i]);

            //    var current = m_akGoodsDataItems.GetObject((ulong)m_kShopData.Goods[i].ID);
            //    if (current != null)
            //    {
            //        m_akGoodsDataItems.FilterObject((ulong)m_kShopData.Goods[i].ID, new object[] { shopTab.tab.ShopType, m_kGoodOptionData, GetShopTabFilter(shopTab.tab.ShopType) });
            //    }
            //    yield return Yielders.EndOfFrame;
            //}

            _OnPageItemCountChanged();

            if (m_akGoodsDataItems.ActiveObjects.Count > 0)
            {
                //Dictionary遍历Bug
                //if(m_iShopLinkID != 0 && m_akGoodsDataItems.ActiveObjects[0].GoodsData.ItemData.TableID == m_iShopLinkID)
                //{
                //    m_akGoodsDataItems.ActiveObjects[0].OnClickBuy();
                //}

                if (m_iShopLinkID != 0)
                {
                    foreach (var item in m_akGoodsDataItems.ActiveObjects)
                    {
                        if (item.Value.GoodsData.ItemData.TableID == m_iShopLinkID)
                        {
                            item.Value.OnClickBuy();
                            break;
                        }
                    }
                }

                m_iShopLinkID = 0;
            }

            yield return Yielders.EndOfFrame;
        }

        void _SortPage()
        {
            //m_akBlankGoodsItems.RecycleAllObject();
            GoodOptionData optionData = null;
            if(m_kDropdown.value >= 0 && m_kDropdown.value < m_kDropdown.options.Count)
            {
                optionData = m_kDropdown.options[m_kDropdown.value] as GoodOptionData;
            }
            
            var shopTab = ShopTab.GetShopTab(this);
            if(shopTab.tab == null)
            {
                Logger.LogErrorFormat("tap selected is null!");
                return;
            }

            var objectList = m_akGoodsDataItems.GetObjectListByFilter(new object[] { shopTab.tab.ShopType, optionData ,GetShopTabFilter(shopTab.tab.ShopType) });
            if (objectList != null)
            {
                objectList.Sort((left, right) =>
                {
                    var x = left.GoodsData;
                    var y = right.GoodsData;
                    if (m_iShopLinkID != 0)
                    {
                        if (((int)x.ItemData.TableID == m_iShopLinkID) != ((int)y.ItemData.TableID == m_iShopLinkID))
                        {
                            return ((int)x.ItemData.TableID == m_iShopLinkID) ? -1 : 1;
                        }
                    }

                    if (x.shopItem.SubType != y.shopItem.SubType)
                    {
                        if (m_iShopTabID != -1)
                        {
                            return (int)x.shopItem.SubType == m_iShopLinkID ? -1 : 1;
                        }
                        return (int)x.shopItem.SubType - (int)y.shopItem.SubType;
                    }

                    return x.shopItem.SortID - y.shopItem.SortID;
                });

                for (int i = 0; i < objectList.Count; ++i)
                {
                    objectList[i].SetAsLastSibling();
                }
            }
        }

        void _OnGoodsClicked(GoodsData goodsData)
        {
            if (!frameMgr.IsFrameOpen<PurchaseCommonFrame>())
            {
                frameMgr.OpenFrame<PurchaseCommonFrame>(FrameLayer.Middle);
            }

            UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
            if (uiEvent != null)
            {
                uiEvent.EventID = EUIEventID.PurchaseCommanUpdate;

                uiEvent.EventParams.kPurchaseCommonData.iShopID = m_kShopData.ID.HasValue ? m_kShopData.ID.Value : 0;
                uiEvent.EventParams.kPurchaseCommonData.iGoodID = goodsData.ID.HasValue ? goodsData.ID.Value : 0;
                uiEvent.EventParams.kPurchaseCommonData.iItemID = (int)goodsData.ItemData.TableID;
                uiEvent.EventParams.kPurchaseCommonData.iCount = goodsData.ItemData.Count;
                UIEventSystem.GetInstance().SendUIEvent(uiEvent);
            }
        }


        private void UpdateMallItemsByType()
        {
            var shopTab = ShopTab.GetShopTab(this);

            if (shopTab == null)
                return;

            if (m_kShopData.Goods.Count <= 0)
                return;

            GoodOptionData optionData = null;
            if (m_kDropdown.value >= 0 && m_kDropdown.value < m_kDropdown.options.Count)
            {
                optionData = m_kDropdown.options[m_kDropdown.value] as GoodOptionData;
            }

            var filter = GetShopTabFilter(shopTab.tab.ShopType);


            for (var i = 0; i < m_kShopData.Goods.Count; i++)
            {
                //非统一类型
                if (m_kShopData.Goods[i].Type != shopTab.tab.ShopType)
                    continue;

                var id = m_kShopData.Goods[i].ID;
                if (id == null)
                    continue;

                //已经存在
                if (m_akGoodsDataItems.HasObject((ulong)id) == true)
                    continue;

                ProtoTable.ItemTable item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)m_kShopData.Goods[i].ItemData.TableID);
                if (item == null)
                {
                    continue;
                }

                if (optionData != null)
                {
                    if (filter == ProtoTable.ShopTable.eFilter.SF_OCCU)
                    {
                        if (item.Occu.Count > 0)
                        {
                            bool bFind = false;
                            for (int j = 0; j < item.Occu.Count; ++j)
                            {
                                if (item.Occu[j] / 10 * 10 == optionData.jobItem.ID || item.Occu[j] == 0)
                                {
                                    bFind = true;
                                    break;
                                }
                            }
                            if (!bFind)
                            {
                                continue;
                            }
                        }
                    }
                    else if (filter == ProtoTable.ShopTable.eFilter.SF_ARMOR)
                    {
                        if (optionData.eFilter != m_kShopData.Goods[i].ItemData.ThirdType)
                        {
                            continue;
                        }
                    }
                    else if (filter == ProtoTable.ShopTable.eFilter.SF_OCCU2)
                    {

                        if (item.Occu2.Count > 0)
                        {
                            bool bFind = false;
                            for (int j = 0; j < item.Occu2.Count; j++)
                            {
                                if (item.Occu2[j] == optionData.jobItem.ID)
                                {
                                    bFind = true;
                                    break;
                                }
                            }

                            if (bFind == false)
                                continue;
                        }
                    }
                }

                OnAddGoodsData(m_kShopData.Goods[i]);
            }

        }
        #endregion

        Button m_refreshBtn;
        UIGray m_grayRefresh;
        Text m_refreshText;
        Text m_refreshPrefixed;
        Text m_resetText;
        Text m_refreshTimes;
        Color m_colOrg;
        Text m_friendlyHint;

        protected void _InitRefreshItems()
        {
            m_friendlyHint = Utility.FindComponent<Text>(frame, "middleback/Goods/FriendlyHint");
            m_refreshBtn = Utility.FindComponent<Button>(frame, "middleback/Goods/Title/BtnRefresh");
            m_grayRefresh = m_refreshBtn.GetComponent<UIGray>();
            m_refreshText = Utility.FindComponent<Text>(frame, "middleback/Goods/Title/BtnRefresh/gold");
            m_colOrg = m_refreshText.color;
            m_refreshPrefixed = Utility.FindComponent<Text>(frame, "middleback/Goods/Title/BtnRefresh/text");
            m_resetText = Utility.FindComponent<Text>(frame, "middleback/Goods/Title/RefreshTime");
            m_resetText.enabled = false;
            m_refreshTimes = Utility.FindComponent<Text>(frame, "middleback/Goods/RefreshTimeRoot/RefreshTimes");
            m_refreshBtn.onClick.RemoveAllListeners();
            m_refreshBtn.onClick.AddListener(() =>
            {
                int id = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindPOINT);
                if(m_kShopData.RefreshCost.Value <= 0)
                {
                    ShopDataManager.GetInstance().RefreshShop((int)m_kShopData.ID);
                }
                else
                {
                    CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = id, nCount = m_kShopData.RefreshCost.Value },
                        () =>
                        {
                            ShopDataManager.GetInstance().RefreshShop((int)m_kShopData.ID);
                        });
                }
            });
        }

        void _InitHelpAssistant()
        {
            //if(m_eShopFrameMode != ShopFrameMode.SFM_MALL_CHILD_FRAME)
            //{
            //    return;
            //}

            GameObject goComHelp = Utility.FindChild(frame, "ComWnd/Title/Horizen/Help");
            if (null != goComHelp)
            {
                var shopItem = TableManager.GetInstance().GetTableItem<ProtoTable.ShopTable>(m_kShopData.ID.Value);
                if (null != shopItem && shopItem.HelpID > 0)
                {
                    HelpAssistant comHelpAssitant = goComHelp.GetComponent<HelpAssistant>();
                    if (null == comHelpAssitant)
                    {
                        comHelpAssitant = goComHelp.AddComponent<HelpAssistant>();
                    }
                    if (null != comHelpAssitant)
                    {
                        comHelpAssitant.eType = (HelpFrameContentTable.eHelpType)shopItem.HelpID;
                    }
                    goComHelp.CustomActive(true);
                }
                else
                {
                    goComHelp.CustomActive(false);
                }
            }
        }

        protected void _RefreshButton()
        {
            //0 不刷新
            //1 可以手动刷新 也可以服务器定时刷新
            //2 只能定时刷新
            if(m_kShopData.RefreshCost.Value > 0)
            {
                m_kRefreshBinder.ChangeStatus(1);
            }
            else
            {
                m_kRefreshBinder.ChangeStatus(2);
            }

            if (m_kShopData.NeedRefresh != 0)
            {
                m_refreshTimes.CustomActive(m_kShopData.NeedRefresh == 1);
                m_refreshBtn.CustomActive(m_kShopData.NeedRefresh == 1 || m_kShopData.NeedRefresh == 3);
                if (m_refreshText != null)
                {
                    m_refreshText.text = m_kShopData.RefreshCost.ToString();
                    int iBindPoint = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindPOINT);
                    int iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(iBindPoint);
                    if (iOwnedCount >= m_kShopData.RefreshCost)
                    {
                        m_refreshText.color = m_colOrg;
                    }
                    else
                    {
                        m_refreshText.color = Color.red;
                    }
                    if (m_refreshPrefixed != null)
                    {
                        m_refreshPrefixed.color = m_refreshText.color;
                    }
                }

                //m_resetText.enabled = true;
                //m_resetText.text = string.Format(TR.Value("shop_refresh_time"), m_kShopData.RefreshTime);

                m_refreshTimes.text = string.Format(TR.Value(m_kShopData.RefreshLeftTimes > 0 ? "shop_refresh_time_enable" : "shop_refresh_time_disable"), m_kShopData.RefreshLeftTimes,m_kShopData.RefreshTotalTimes);
                m_refreshTimes.enabled = true;

                m_refreshBtn.enabled = m_kShopData.RefreshLeftTimes > 0;
                m_grayRefresh.enabled = !m_refreshBtn.enabled;
            }
            else
            {
                //m_resetText.enabled = false;
                m_refreshTimes.enabled = false;
                m_refreshBtn.CustomActive(false);
                m_refreshTimes.CustomActive(false);
            }
        }

        void _OnVipLvChanged(UIEvent uiEvent)
        {
            if (m_kShopData.NeedRefresh == 1)
            {
                SceneShopRefreshNumReq kSend = new SceneShopRefreshNumReq();
                kSend.shopId = (byte)m_kShopData.ID.Value;
                Network.NetManager.Instance().SendCommand(Network.ServerType.GATE_SERVER, kSend);

                Logger.LogErrorFormat("send refresh shopid = {0}", kSend.shopId);

                WaitNetMessageManager.GetInstance().Wait<SceneShopRefreshNumRes>(msgRet =>
                {
                    var shopData = ShopDataManager.GetInstance().GetGoodsDataFromShop(msgRet.shopId);
                    if(shopData != null)
                    {
                        shopData.RefreshLeftTimes = msgRet.restRefreshNum;
                        shopData.RefreshTotalTimes = msgRet.maxRefreshNum;

                        Logger.LogErrorFormat("shop refresh times changed lefttimes = {0} , maxTimes = {1}", msgRet.restRefreshNum, msgRet.maxRefreshNum);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ShopRefreshTimesChanged,shopData.ID.Value);
                    }
                });
            }
        }

        protected void _ControlRefreshTime(ProtoTable.ShopTable.eSubType eShopTab)
        {
            bool bEnable = false;
            if(m_kShopData.NeedRefresh != 0)
            {
                var shopItem = TableManager.GetInstance().GetTableItem<ProtoTable.ShopTable>(m_kShopData.ID.Value);
                if(shopItem != null)
                {
                    for(int j = 0; j < shopItem.SubType.Count; ++j)
                    {
                        if(shopItem.SubType[j] == eShopTab)
                        {
                            bEnable = shopItem.NeedRefreshTabs[j] == 1;
                            break;
                        }
                    }
                }
            }
            m_resetText.enabled = bEnable;
            //Logger.LogErrorFormat("eShopTab = {0} bEnable = {1}", eShopTab, bEnable);
        }

        Button btnVipAddTimes;
        void _OnAddVipTimes()
        {
            SystemNotifyManager.SystemNotify(7004,_OnAddVipOk);
        }

        void _OnAddVipOk()
        {
            var frame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
            if (frame != null)
            {
                frame.OpenPayTab();
            }
        }

        protected override void _OnOpenFrame()
        {
            m_iEnumerator = null;
            ShopFrameData frameData = userData as ShopFrameData;
            m_kShopData = frameData.m_kShopData;
            m_iShopLinkID = frameData.m_iShopLinkID;
            m_iShopTabID = frameData.m_iShopTabID;
            StateController comChildControl = frame.GetComponent<StateController>();
            m_eShopFrameMode = frameData.eShopFrameMode;
            comChildControl.Key = m_eShopFrameMode.ToString();

            m_kShopName = Utility.FindComponent<Text>(frame, "ComWnd/Title/Horizen/Name");
            m_kShopName.text = m_kShopData.Name;
            btnVipAddTimes = Utility.FindComponent<Button>(frame, "middleback/Goods/RefreshTimeRoot/RefreshTimes/BtnAddTimes");
            btnVipAddTimes.onClick.AddListener(_OnAddVipTimes);

            _PlayNpcSound(NpcVoiceComponent.SoundEffectType.SET_Start);

            _InitRefreshItems();
            _InitDropDown();

            _InitMoney();
            _InitShopTabs();
            _InitGoodsData();
            _RefreshButton();
            _InitHelpAssistant();
            _UpdateFriendlyHint();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideEnd);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopBuyGoodsSuccess, _RereshAllGoods);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopRefreshSuccess, _RebuildAllObjects);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopRefreshTimesChanged, _OnShopRefreshTimesChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerVipLvChanged, _OnVipLvChanged);

            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;

            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
        }

        void OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            _UpdateJarScore();
            _RefreshAllObjects();
        }

        void _OnShopRefreshTimesChanged(UIEvent uiEvent)
        {
            Logger.LogErrorFormat("event param1 = {0}", (int)uiEvent.Param1);
            if(m_kShopData.ID.Value == (int)uiEvent.Param1)
            {
                Logger.LogErrorFormat("_OnShopRefreshTimesChanged Invoke succeed !");
                _RefreshButton();
            }
        }

        protected override void _OnCloseFrame()
        {
            _PlayNpcSound(NpcVoiceComponent.SoundEffectType.SET_End);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideStart);

            ShopTab.Clear(this,true);
            m_akShopTabObjects.Clear();
            m_akMoneyItemObjects.Clear();
            m_akMoneyIds.Clear();
            m_akGoodsDataItems.DestroyAllObjects();

            if(btnVipAddTimes != null)
            {
                btnVipAddTimes.onClick.RemoveAllListeners();
                btnVipAddTimes = null;
            }
            //m_akBlankGoodsItems.Clear();

            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopBuyGoodsSuccess, _RereshAllGoods);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopRefreshSuccess, _RebuildAllObjects);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopRefreshTimesChanged, _OnShopRefreshTimesChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerVipLvChanged, _OnVipLvChanged);

            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;

            m_kShopData = null;
        }

        void OnAddNewItem(List<Item> items)
        {
            _RefreshAllObjects();
        }

        void OnUpdateItem(List<Item> items)
        {
            _RefreshAllObjects();
        }

        void OnRemoveItem(ItemData data)
        {
            _RefreshAllObjects();
        }

        void _RereshAllGoods(UIEvent uiEvent)
        {
            _RefreshAllObjects();
        }

        void _RefreshAllObjects()
        {
            m_akGoodsDataItems.RefreshAllObjects(null);
        }

        void _RebuildAllObjects(UIEvent uiEvent)
        {
            m_akGoodsDataItems.RecycleAllObject();
            //m_akBlankGoodsItems.RecycleAllObject();
            ShopTab.Clear(this,false);
            _InitGoodsData();
            _RefreshButton();
        }

        [UIEventHandle("ComWnd/Title/Close")]
        void _OnReturnClicked()
        {
            ShopFrameData data = userData as ShopFrameData;
            if (data.onShopReturn != null)
            {
                data.onShopReturn.Invoke();
            }
            frameMgr.CloseFrame(this);
        }
    }
}