using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.ComponentModel;

namespace GameClient
{
    enum EnchantmentsType
    {
        [DescriptionAttribute("ItemTable.eColor.CL_NONE")]
        [MapIndex((int)ProtoTable.ItemTable.eColor.CL_NONE)]
        ET_INVALID = -1,
        [DescriptionAttribute("ItemTable.eColor.WHITE")]
        [MapIndex((int)ProtoTable.ItemTable.eColor.WHITE)]
        ET_NORMAL,
        [DescriptionAttribute("ItemTable.eColor.BLUE")]
        [MapIndex((int)ProtoTable.ItemTable.eColor.BLUE)]
        ET_HIGH,
        [DescriptionAttribute("ItemTable.eColor.PURPLE")]
        [MapIndex((int)ProtoTable.ItemTable.eColor.PURPLE)]
        ET_PURPLE,
        [DescriptionAttribute("ItemTable.eColor.GREEN")]
        [MapIndex((int)ProtoTable.ItemTable.eColor.GREEN)]
        ET_GREEN,
        [DescriptionAttribute("ItemTable.eColor.PINK")]
        [MapIndex((int)ProtoTable.ItemTable.eColor.PINK)]
        ET_PINK,
        [DescriptionAttribute("ItemTable.eColor.YELLOW")]
        [MapIndex((int)ProtoTable.ItemTable.eColor.YELLOW)]
        ET_ORANGE,
        [DescriptionAttribute("ItemTable.eColor.ALL")]
        ET_COUNT,
    }

    class EnchantmentsBookFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/EnchantmentsFrame/EnchantmentsFrame";
        }

        IEnumerator _AnsyInitEnchantmentsCard()
        {
            m_goParent = Utility.FindChild(frame, "tittles/ScrollView/Viewport/content");
            m_goPrefab = Utility.FindChild(m_goParent, "prefabs");
            m_goPrefab.CustomActive(false);
            m_akEnchantCardObjects.Clear();
            yield return Yielders.EndOfFrame;

            var magicCards = EnchantmentsCardManager.GetInstance().EnchantmentsCardDataList;
            for (int i = 0; i < magicCards.Count; ++i)
            {
                OnAddCard(magicCards[i]);
                if(i % 5 == 0)
                {
                    yield return Yielders.EndOfFrame;
                }
            }
            _OnCheckAllTabMarks();
            yield return Yielders.EndOfFrame;
        }

        protected override void _OnOpenFrame()
        {
            m_kBtnComLink = Utility.FindComponent<Button>(frame, "tittles/Detail/ButtonAcquire");
            m_kComGray = Utility.FindComponent<UIGray>(frame, "tittles/Detail/ButtonAcquire");

            CardItemObject.Clear();
            _InitTabs();
            StartCoroutine(_AnsyInitEnchantmentsCard());
            EnchantmentsCardManager.GetInstance().onUpdateCard += this._OnUpdateCard;
            EnchantmentsCardManager.GetInstance().onTabMarkChanged += this.OnTabMarkChanged;
            m_kComItem = null;
        }

        protected override void _OnCloseFrame()
        {
            EnchantmentsCardManager.GetInstance().onUpdateCard -= this._OnUpdateCard;
            EnchantmentsCardManager.GetInstance().onTabMarkChanged -= this.OnTabMarkChanged;
            m_akEnchantCardObjects.Clear();
            CardItemObject.Clear();
        }

        #region _InitTabs
        GameObject m_goTabPrefab;
        EnchantmentsType m_eEnchantmentsType = EnchantmentsType.ET_INVALID;
        Toggle[] m_akToggles = new Toggle[(int)EnchantmentsType.ET_COUNT];
        GameObject[] m_akTabMark = new GameObject[(int)EnchantmentsType.ET_COUNT];

        void _InitTabs()
        {
            m_eEnchantmentsType = EnchantmentsType.ET_INVALID;
            m_goTabPrefab = Utility.FindChild(frame, "tabs/tab");
            m_goTabPrefab.CustomActive(false);
            for (int i = 0; i < (int)EnchantmentsType.ET_COUNT; ++i)
            {
                if(!EnchantmentsCardManager.GetInstance().HasQualityCard((EnchantmentsType)i))
                {
                    continue;
                }

                GameObject goCurrent = GameObject.Instantiate(m_goTabPrefab);
                Utility.AttachTo(goCurrent, m_goTabPrefab.transform.parent.gameObject);
                string content = Utility.GetEnumDescription((EnchantmentsType)i);
                string convertedContent = TR.Value(content);
                if(!string.IsNullOrEmpty(convertedContent))
                {
                    content = convertedContent;
                }

                Text text = Utility.FindComponent<Text>(goCurrent, "Label");
                text.text = content;
                text = Utility.FindComponent<Text>(goCurrent, "Checkmark/Label");
                text.text = content;
                goCurrent.CustomActive(true);
                Toggle toggle = goCurrent.GetComponent<Toggle>();
                toggle.onValueChanged.RemoveAllListeners();
                int k = i;
                toggle.onValueChanged.AddListener((bool bValue) =>
                {
                    if (bValue)
                    {
                        _OnTabChanged((EnchantmentsType)k);
                    }
                });
                m_akToggles[i] = toggle;
                m_akTabMark[i] = Utility.FindChild(toggle.gameObject, "tabMark");
            }

            m_akToggles[(int)EnchantmentsType.ET_NORMAL].isOn = true;
        }

        void _OnTabChanged(EnchantmentsType eEnchantmentsType)
        {
            m_eEnchantmentsType = eEnchantmentsType;
            m_akEnchantCardObjects.Filter(new object[] { m_eEnchantmentsType });
            _OnTabMarkChanged(eEnchantmentsType);
        }

        void _OnTabMarkChanged(EnchantmentsType eEnchantmentsType)
        {
            if(m_akTabMark[(int)eEnchantmentsType] != null)
            {
                m_akTabMark[(int)eEnchantmentsType].CustomActive(EnchantmentsCardManager.GetInstance().HasNewQualityCard(eEnchantmentsType));
            }
        }

        void _OnCheckAllTabMarks()
        {
            for(int i = 0; i < (int)EnchantmentsType.ET_COUNT; ++i)
            {
                if(m_akTabMark[i] != null)
                {
                    m_akTabMark[i].CustomActive(EnchantmentsCardManager.GetInstance().HasNewQualityCard((EnchantmentsType)i));
                }
            }
        }
        #endregion

        #region uievent
        [UIEventHandle("tittle/close")]
        void OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("ToGet")]
        void OnToGet()
        {
            
             ItemComeLink.OnLink(tableID, 0);
            
        }
        #endregion

        #region cardItemObject
        class CardItemObject : CachedObject
        {
            static CardItemObject ms_selected;
            public static void Clear()
            {
                ms_selected = null;
            }

            protected GameObject goLocal;
            protected GameObject goParent;
            protected GameObject goPrefab;
            protected EnchantmentsCardData data;
            protected EnchantmentsBookFrame THIS;
            protected EnchantmentsType eEnchantmentsType;
            protected GameObject itemParent;
            protected ComItem comItem;
            protected GameObject goSelected;
            public EnchantmentsType EnchantmentsType
            {
                get
                {
                    return eEnchantmentsType;
                }
            }

            Text m_kName;
            Text m_kLv;
            GameObject m_goNewMark;
            UIGray comGray;

            Button button;

            public EnchantmentsCardData EnchantmentsCardData
            {
                get { return data; }
            }

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                data = param[2] as EnchantmentsCardData;
                THIS = param[3] as EnchantmentsBookFrame;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);
                    itemParent = Utility.FindChild(goLocal, "EnableMark/Icon");
                    comItem = THIS.CreateComItem(itemParent);
                    goSelected = Utility.FindChild(goLocal,"SelectMark");
                    m_kName = Utility.FindComponent<Text>(goLocal, "EnableMark/Name");
                    m_kLv = Utility.FindComponent<Text>(goLocal, "EnableMark/Lv");
                    button = goLocal.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(_OnClick);
                    m_goNewMark = Utility.FindChild(goLocal, "EnableMark/NewMark");
                    comGray = Utility.FindComponent<UIGray>(goLocal, "EnableMark");
                }

                Enable();
                _UpdateItem();
            }

            public override void OnRecycle()
            {
                Disable();
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
            public override void OnDecycle(object[] param) { OnCreate(param); }
            public override void OnRefresh(object[] param)
            {
                data = param[0] as EnchantmentsCardData;
                _UpdateItem();
            }
            public override bool NeedFilter(object[] param)
            {
                return eEnchantmentsType != (EnchantmentsType)param[0];
            }

            public void _UpdateItem()
            {
                eEnchantmentsType = GameClient.EnchantmentsCardManager.GetQuality(data.itemData.Quality);
                comItem.Setup(data.itemData, OnItemClicked);
                m_kName.text = data.itemData.GetColorName();
                m_kLv.text = "lv" + data.itemData.LevelLimit;
                SetSelected(this == ms_selected);
                m_goNewMark.CustomActive(data.itemData.IsNew);
                comGray.enabled = data.itemData.Count <= 0;
            }

            void OnItemClicked(GameObject obj, ItemData item)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }

            void SetSelected(bool bSelected)
            {
                goSelected.CustomActive(bSelected);
            }

            void _OnClick()
            {
                EnchantmentsCardManager.GetInstance().RemoveNewMark((ulong)data.itemData.TableID);
                OnSelected();
            }

            void OnSelected()
            {
                if(ms_selected != this)
                {
                    if(ms_selected != null)
                    {
                        ms_selected.SetSelected(false);
                    }
                    ms_selected = this;
                    SetSelected(true);
                }
                THIS.OnSetTarget(this);
            }

            public void TrySelected()
            {
                if(ms_selected == null && eEnchantmentsType == THIS.m_eEnchantmentsType)
                {
                    OnSelected();
                }
            }
        }

        CachedObjectDicManager<ulong, CardItemObject> m_akEnchantCardObjects = new CachedObjectDicManager<ulong, CardItemObject>();

        GameObject m_goParent;
        GameObject m_goPrefab;

        void _InitEnchantmentsCard()
        {
            m_goParent = Utility.FindChild(frame, "tittles/ScrollView/Viewport/content");
            m_goPrefab = Utility.FindChild(m_goParent, "prefabs");
            m_goPrefab.CustomActive(false);
            m_akEnchantCardObjects.Clear();

            var magicCards = EnchantmentsCardManager.GetInstance().EnchantmentsCardDataList;
            for(int i = 0; i < magicCards.Count; ++i)
            {
                OnAddCard(magicCards[i]);
            }
            _OnCheckAllTabMarks();
        }

        CardItemObject m_kCurrent = null;
        [UIControl("tittles/Detail/condition/name", typeof(Text))]
        Text m_kCondition;
        [UIControl("tittles/Detail/card/name", typeof(Text))]
        Text m_kCardName;
        //[UIControl("tittles/Detail/card/title/lv/text", typeof(Text))]
        //Text m_kCardLv;
        [UIControl("tittles/Detail/card/slot", typeof(Image))]
        Image m_kCardIcon;
        [UIControl("tittles/Detail/card/ScrollView/Viewport/content/Text", typeof(Text))]
        Text m_kCardDesc;
        ComItem m_kComItem;
        [UIControl("tittles/Detail/ButtonAcquire",typeof(ItemComeLink))]
        ItemComeLink m_kCardComeLink;
        Button m_kBtnComLink;
        UIGray m_kComGray;
        int tableID = 0;
        void OnSetTarget(CardItemObject cardItemObject)
        {
            m_kCurrent = cardItemObject;

            if (m_kComItem == null)
            {
                m_kComItem = CreateComItem(m_kCardIcon.gameObject);
            }

            if (m_kCurrent != null)
            {
                EnchantmentsCardData data = m_kCurrent.EnchantmentsCardData;
                tableID = data.itemData.TableID;
                m_kCondition.text = EnchantmentsCardManager.GetInstance().GetCondition(data);
                m_kCardName.text = data.itemData.GetColorName();
                //m_kCardLv.text = data.itemData.LevelLimit.ToString();
                m_kCardDesc.text = EnchantmentsCardManager.GetInstance().GetDefaultDescribe(data);
                m_kCardIcon.enabled = false;
                m_kComItem.Setup(data.itemData, OnItemClicked);
                m_kComGray.enabled = false;
                m_kBtnComLink.enabled = true;
                m_kCardComeLink.bNotEnough = false;
                m_kCardComeLink.iItemLinkID = data.magicItem.ID;
            }
            else
            {
                m_kCardIcon.enabled = true;
                m_kComItem.Setup(null,null);
                m_kComGray.enabled = true;
                m_kBtnComLink.enabled = false;
                m_kCardComeLink.iItemLinkID = 0;
            }
            
        }

        void OnItemClicked(GameObject obj, ItemData item)
        {
            ItemTipManager.GetInstance().ShowTip(item);
        }
        #endregion

        #region callback
        void OnAddCard(EnchantmentsCardData data, bool bNeedUpdateTabMark = false)
        {
            if (data != null)
            {
                if (m_akEnchantCardObjects.HasObject((ulong)data.itemData.TableID))
                {
                    OnUpdateCard(data, bNeedUpdateTabMark);
                }
                else
                {
                    var current = m_akEnchantCardObjects.Create((ulong)data.itemData.TableID, new object[] { m_goParent, m_goPrefab, data, this });
                    if (current != null)
                    {
                        m_akEnchantCardObjects.FilterObject((ulong)data.itemData.TableID, new object[] { m_eEnchantmentsType });
                        current.TrySelected();
                        if(bNeedUpdateTabMark)
                        {
                            _OnTabMarkChanged(current.EnchantmentsType);
                        }
                    }
                }
            }
        }

        void OnUpdateCard(EnchantmentsCardData data, bool bNeedUpdateTabMark = false)
        {
            if(data != null)
            {
                m_akEnchantCardObjects.RefreshObject((ulong)data.itemData.TableID, new object[] { data });
                if(bNeedUpdateTabMark)
                {
                    var current = m_akEnchantCardObjects.GetObject((ulong)data.itemData.TableID);
                    if(current != null)
                    {
                        _OnTabMarkChanged(current.EnchantmentsType);
                    }
                }
            }
        }

        void _OnUpdateCard(EnchantmentsCardData data)
        {
            OnUpdateCard(data, true);
        }

        void OnTabMarkChanged(ulong iTableID)
        {
            var current = m_akEnchantCardObjects.GetObject(iTableID);
            if(current != null)
            {
                current._UpdateItem();
                _OnTabMarkChanged(current.EnchantmentsType);
            }
        }
        #endregion
    }
}