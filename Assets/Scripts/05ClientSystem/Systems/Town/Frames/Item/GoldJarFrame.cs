using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

using UnityEngine.Assertions;


namespace GameClient
{
    class GoldJarFrame : ClientFrame
    {
        [UIObject("Content/TreeList/Viewport/Content")]
        GameObject m_objMainTypeRoot;

        [UIObject("Content/TreeList/Viewport/Content/Group")]
        GameObject m_objMainTypeTemplate;

       // [UIObject("Content/TreeList/Viewport/Content/Group/SubTypes")]
        //GameObject m_objSubTypeRoot;

        [UIObject("Content/TreeList/Viewport/Content/Group/SubTypes/SubType")]
        GameObject m_objSubTypeTemplate;

        [UIObject("Content/TabGroup/Tabs")]
        GameObject m_objLevelTabRoot;

        [UIObject("Content/TabGroup/Tabs/Tab")]
        GameObject m_objLevelTabTemplate;


        [UIControl("Content/TabGroup/Page/Right/Items")]
        ComUIListScript m_comItemList;

        [UIControl("Content/TabGroup/Page/Left/Jar")]
        Image m_imgJarIcon;

        [UIControl("Content/TabGroup/Page/Left/Jar/Name")]
        Text m_labJarName;

        [UIControl("Content/TabGroup/Page/Left/BuyDesc/Title")]
        Text m_labBuyItemTitle;

        [UIObject("Content/TabGroup/Page/Left/BuyDesc/ItemRoot")]
        GameObject m_objBuyItemRoot;

        [UIControl("Content/TabGroup/Page/Left/BuyDesc/Desc")]
        Text m_labBuyItemDesc;

        [UIObject("Content/TabGroup/Page/Right/Buy")]
        GameObject m_objBuyRoot;

        [UIObject("Content/TabGroup/Page/Right/Buy/Func")]
        GameObject m_objBuyTemplate;

        [UIControl("Content/TabGroup/Page/Left/CommonConsume", typeof(ComCommonConsume))]
        ComCommonConsume m_comConsume;
        
        List<GameObject> m_arrLevelTypeObjs = new List<GameObject>();
        List<GameObject> m_arrBuyObjs = new List<GameObject>();
        JarData m_currentData = null;

        delegate void OnRedPointChanged();
        List<OnRedPointChanged> m_arrLevelRedPoints = new List<OnRedPointChanged>();


        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Jar/GoldJar";
        }

        protected override void _OnOpenFrame()
        {
            JarDataManager.GetInstance().RequestQuaryJarShopSocre();

            _InitUI();
            _UpdateRedPoint();
            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _ClearUI();
            _UnRegisterUIEvent();
        }

        protected void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MagicJarUseSuccess, _OnJarUseSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerDataBaseUpdated, _OnMoneyChanged);
        }

        protected void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MagicJarUseSuccess, _OnJarUseSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerDataBaseUpdated, _OnMoneyChanged);
        }

        void _InitUI()
        {
            m_objBuyTemplate.SetActive(false);
            m_objBuyTemplate.transform.SetParent(frame.transform, false);
            m_objMainTypeTemplate.SetActive(false);
            m_objMainTypeTemplate.transform.SetParent(frame.transform, false);
            m_objSubTypeTemplate.SetActive(false);
            m_objSubTypeTemplate.transform.SetParent(frame.transform, false);
            m_objLevelTabTemplate.SetActive(false);
            m_objLevelTabTemplate.transform.SetParent(frame.transform, false);

            m_comItemList.Initialize();

            m_comItemList.onBindItem = var =>
            {
                return CreateComItem(Utility.FindGameObject(var, "Item"));
            };

            m_comItemList.onItemVisiable = var =>
            {
                if (m_currentData != null)
                {
                    List<ItemSimpleData> items = m_currentData.arrBonusItems;

                    if (var.m_index >= 0 && var.m_index < items.Count)
                    {
                        ItemData item = ItemDataManager.CreateItemDataFromTable(items[var.m_index].ItemID);

                        if(item != null)
                        {
                            item.Count = items[var.m_index].Count;
                            ComItem comItem = var.gameObjectBindScript as ComItem;

                            comItem.Setup(item, (var1, var2) =>
                            {
                                ItemTipManager.GetInstance().ShowTip(var2);
                            });

                            Utility.GetComponetInChild<Text>(var.gameObject, "Name").text = item.GetColorName();
                        }
                    }
                }
            };
            
            _ClearLevelTypes();
            _ClearGoods();
        }

        void _ClearUI()
        {
            m_arrLevelTypeObjs.Clear();
            m_arrBuyObjs.Clear();
            m_currentData = null;
            m_arrLevelRedPoints.Clear();
        }

        void _InitLevelTypes(int a_nSubType)
        {
            m_arrLevelRedPoints.Clear();

            List<Toggle> toggles = new List<Toggle>();
            bool isToggleInited = false;
            List<JarTreeNode> arrLevelTypes = JarDataManager.GetInstance().GetGoldJarLevels(a_nSubType);
            for (int i = 0; i < arrLevelTypes.Count; ++i)
            {
                int nLevelType = arrLevelTypes[i].nKey;
                GameObject objLevelType;
                if (i < m_arrLevelTypeObjs.Count)
                {
                    objLevelType = m_arrLevelTypeObjs[i];
                }
                else
                {
                    objLevelType = GameObject.Instantiate(m_objLevelTabTemplate);
                    objLevelType.transform.SetParent(m_objLevelTabRoot.transform, false);
                    m_arrLevelTypeObjs.Add(objLevelType);
                }
                objLevelType.SetActive(true);
                Toggle toggle = objLevelType.GetComponent<Toggle>();
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(var =>
                {
                    if (isToggleInited)
                    {
                        if (var)
                        {
                            m_currentData = JarDataManager.GetInstance().GetGoldJarData(a_nSubType, nLevelType);
                            _ClearGoods();
                            _InitGoods(m_currentData);
                        }
                    }
                });
                toggles.Add(toggle);

                Utility.GetComponetInChild<Text>(objLevelType, "Label").text = TR.Value("goldjar_level_type", nLevelType);

                m_arrLevelRedPoints.Add(() =>
                {
                    Utility.FindGameObject(objLevelType, "RedPoint").SetActive(
                        JarDataManager.GetInstance().CheckGoldJarLevelRedPoint(a_nSubType, nLevelType));
                });
            }

            Toggle selectToggle = null;
            for (int i = 0; i < toggles.Count; ++i)
            {
                if (i == 0)
                {
                    toggles[i].group.SetAllTogglesOff();
                    selectToggle = toggles[i];
                }
                else
                {
                    toggles[i].isOn = true;
                }
            }

            isToggleInited = true;
            if (selectToggle != null)
            {
                selectToggle.isOn = true;
            }
        }

        void _ClearLevelTypes()
        {
            for (int i = 0; i < m_arrLevelTypeObjs.Count; ++i)
            {
                m_arrLevelTypeObjs[i].SetActive(false);
            }
        }

        void _InitGoods(JarData a_data)
        {
            if (a_data == null)
            {
                return;
            }

            m_comItemList.SetElementAmount(a_data.arrBonusItems.Count);

            // m_imgJarIcon.sprite = AssetLoader.GetInstance().LoadRes(a_data.strJarImagePath, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref m_imgJarIcon, a_data.strJarImagePath);
            m_labJarName.text = a_data.strName;
            ComItem comItem = CreateComItem(m_objBuyItemRoot);
            Assert.IsTrue(a_data.arrBuyItems != null && a_data.arrBuyItems.Count > 0);
            ItemData buyItem = a_data.arrBuyItems[0];
            comItem.Setup(buyItem, (var1, var2) =>
            {
                ItemTipManager.GetInstance().ShowTip(var2);
            });
            m_labBuyItemTitle.text = TR.Value("goldjar_buy_item_title", buyItem.GetColorName());
            m_labBuyItemDesc.text = TR.Value("goldjar_buy_desc");

            for (int i = 0; i < m_arrBuyObjs.Count; ++i)
            {
                m_arrBuyObjs[i].SetActive(false);
            }

            for (int i = 0; i < a_data.arrBuyInfos.Count; ++i)
            {
                GameObject objBuy;
                if (i < m_arrBuyObjs.Count)
                {
                    objBuy = m_arrBuyObjs[i];
                }
                else
                {
                    objBuy = GameObject.Instantiate(m_objBuyTemplate);
                    objBuy.transform.SetParent(m_objBuyRoot.transform, false);
                    m_arrBuyObjs.Add(objBuy);
                }
                objBuy.SetActive(true);
                JarBuyInfo buyInfo = a_data.arrBuyInfos[i];

                GameObject objEffect = Utility.FindGameObject(objBuy, "RedPoint");
                objEffect.SetActive(false);

                Button btnBuy = objBuy.GetComponent<Button>();
                btnBuy.onClick.RemoveAllListeners();
                btnBuy.onClick.AddListener(() =>
                {
                    ShowItemsFrame.bSkipExplode = false;

                    Assert.IsTrue(buyInfo.arrCosts.Count >= 1);
                    CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                    for (int j = 0; j < buyInfo.arrCosts.Count; ++j)
                    {
                        JarBuyCost cost = buyInfo.arrCosts[j];
                        int nCount = cost.GetRealCostCount(buyInfo.nBuyCount);
                        if (nCount <= ItemDataManager.GetInstance().GetOwnedItemCount((int)cost.item.TableID))
                        {
                            costInfo.nMoneyID = cost.item.TableID;
                            costInfo.nCount = nCount;
                            break;
                        }
                        else
                        {
                            if (j == buyInfo.arrCosts.Count - 1)
                            {
                                costInfo.nMoneyID = cost.item.TableID;
                                costInfo.nCount = nCount;
                            }
                        }
                    }

                    CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                    {
                        JarDataManager.GetInstance().RequestBuyJar(a_data, buyInfo);
                    });
                });

                Utility.GetComponetInChild<Text>(objBuy, "Time").text = TR.Value("magicjar_buy_times", buyInfo.nBuyCount);
                Text labCount = Utility.GetComponetInChild<Text>(objBuy, "Price/Count");
                Image imgIcon = Utility.GetComponetInChild<Image>(objBuy, "Price/Icon");
                for (int j = 0; j < buyInfo.arrCosts.Count; ++j)
                {
                    JarBuyCost cost = buyInfo.arrCosts[j];
                    int nCount = cost.GetRealCostCount(buyInfo.nBuyCount);
                    if (nCount <= ItemDataManager.GetInstance().GetOwnedItemCount((int)cost.item.TableID))
                    {
                        labCount.text = nCount.ToString();
                        // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);
                        break;
                    }
                    else
                    {
                        if (j == buyInfo.arrCosts.Count - 1)
                        {
                            labCount.text = TR.Value("color_red", nCount);
                            // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);
                        }
                    }
                }

                if (buyInfo.arrCosts.Count > 0)
                {
                    m_comConsume.SetData(ComCommonConsume.eType.Item, ComCommonConsume.eCountType.Fatigue, (int)buyInfo.arrCosts[0].item.TableID);
                }
            }
        }

        void _ClearGoods()
        {
            m_comItemList.SetElementAmount(0);
        }

        void _UpdateGoods(JarData a_data)
        {
            for (int i = 0; i < a_data.arrBuyInfos.Count; ++i)
            {
                GameObject objBuy;
                if (i < m_arrBuyObjs.Count)
                {
                    objBuy = m_arrBuyObjs[i];
                }
                else
                {
                    objBuy = GameObject.Instantiate(m_objBuyTemplate);
                    objBuy.transform.SetParent(m_objBuyRoot.transform, false);
                    m_arrBuyObjs.Add(objBuy);
                }
                objBuy.SetActive(true);
                JarBuyInfo buyInfo = a_data.arrBuyInfos[i];

                GameObject objEffect = Utility.FindGameObject(objBuy, "RedPoint");
                objEffect.SetActive(JarDataManager.GetInstance().CheckRedPoint(a_data, buyInfo));

                Text labCount = Utility.GetComponetInChild<Text>(objBuy, "Price/Count");
                Image imgIcon = Utility.GetComponetInChild<Image>(objBuy, "Price/Icon");
                for (int j = 0; j < buyInfo.arrCosts.Count; ++j)
                {
                    JarBuyCost cost = buyInfo.arrCosts[j];
                    int nCount = cost.GetRealCostCount(buyInfo.nBuyCount);
                    if (nCount <= ItemDataManager.GetInstance().GetOwnedItemCount((int)cost.item.TableID))
                    {
                        labCount.text = nCount.ToString();
                        // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);
                        break;
                    }
                    else
                    {
                        if (j == buyInfo.arrCosts.Count - 1)
                        {
                            labCount.text = TR.Value("color_red", nCount);
                            // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);
                        }
                    }
                }
            }
        }

        void _UpdateRedPoint()
        {
            for (int i = 0; i < m_arrLevelRedPoints.Count; ++i)
            {
                m_arrLevelRedPoints[i].Invoke();
            }
        }

        void _OnJarUseSuccess(UIEvent a_event)
        {
            _UpdateGoods(m_currentData);
            _UpdateRedPoint();
        }

        void _OnMoneyChanged(UIEvent a_event)
        {
            _UpdateGoods(m_currentData);
        }

        void _OnItemGet(UIEvent a_event)
        {
            _UpdateRedPoint();
        }

        [UIEventHandle("BG/Title/Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Content/TabGroup/Page/Left/Shop")]
        void _OnShopClicked()
        {
            //ShopMainFrame.OpenLinkFrame("8|-1|8|2");
        }
    }
}
