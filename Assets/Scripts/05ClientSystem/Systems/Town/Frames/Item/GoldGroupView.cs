using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using EJarType = ProtoTable.JarBonus.eType;
using Protocol;
using ProtoTable;

namespace GameClient
{

    class GoldGroupView : CommonTabToggleView
    {

        #region gold jar
        delegate void OnRedPointChanged();

        [SerializeField]
        GameObject m_objGoldLevelTabRoot;

        [SerializeField]
        GameObject m_objGoldLevelTabTemplate;

        [SerializeField]
        Image m_imgGoldJarIcon;

        [SerializeField]
        GameObject m_objGoldBuyFuncRoot;

        bool m_bGoldJarInited = false;
        JarData m_currGoldJarData = null;
        List<GameObject> m_arrLevelTypeObjs = new List<GameObject>();
        List<OnRedPointChanged> m_arrLevelRedPoints = new List<OnRedPointChanged>();
        private JarFrame mJarFrame = null;

        protected void _InitGoldJarUI()
        {
            JarDataManager.GetInstance().RequestQuaryJarShopSocre();

            m_objGoldLevelTabTemplate.SetActive(false);
            m_objGoldLevelTabTemplate.transform.SetParent(gameObject.transform, false);

            _ClearGoldLevelTypes();

            ProtoTable.JobTable jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobTable != null)
            {
                _InitGoldLevelTypes(jobTable.SuitArmorType);
            }
            else
            {
                Logger.LogErrorFormat("袖珍罐：不存在职业id{0}", PlayerBaseData.GetInstance().JobTableID);
            }
        }

        protected void _ClearGoldJarUI()
        {
            m_currGoldJarData = null;
            m_arrLevelTypeObjs.Clear();
            m_arrLevelRedPoints.Clear();
        }

        void _InitGoldLevelTypes(int a_nSubType)
        {
            if (m_arrLevelRedPoints != null)
            {
                m_arrLevelRedPoints.Clear();
            }

            List<Toggle> toggles = new List<Toggle>();
            bool isToggleInited = false;

            List<JarTreeNode> arrLevelTypes = JarDataManager.GetInstance().GetGoldJarLevels(a_nSubType);

            if (arrLevelTypes != null)
            {
                for (int i = 0; i < arrLevelTypes.Count; ++i)
                {
                    int nLevelType = arrLevelTypes[i].nKey;
                    GameObject objLevelType = null;

                    if (m_arrLevelTypeObjs == null)
                    {
                        m_arrLevelTypeObjs = new List<GameObject>();
                    }

                    if (i < m_arrLevelTypeObjs.Count)
                    {
                        objLevelType = m_arrLevelTypeObjs[i];
                    }
                    else
                    {
                        if (m_objGoldLevelTabTemplate != null && m_objGoldLevelTabRoot != null)
                        {
                            objLevelType = GameObject.Instantiate(m_objGoldLevelTabTemplate);
                            objLevelType.transform.SetParent(m_objGoldLevelTabRoot.transform, false);
                            m_arrLevelTypeObjs.Add(objLevelType);
                        }
                    }

                    if (objLevelType != null)
                    {
                        objLevelType.SetActive(true);
                        var bind = objLevelType.GetComponent<ComCommonBind>();
                        ToggleEx toggle = bind.GetCom<ToggleEx>("Toggle");

                        if (toggle != null)
                        {
                            toggle.onValueChanged.RemoveAllListeners();
                            toggle.onValueChanged.AddListener(var =>
                            {
                                if (isToggleInited)
                                {
                                    if (var)
                                    {
                                        m_currGoldJarData = JarDataManager.GetInstance().GetGoldJarData(a_nSubType, nLevelType);
                                        _InitGoldGoods(m_currGoldJarData);

                                        if (true)
                                        {
                                            JarDataManager.GetInstance().RequestJarBuyRecord(m_currGoldJarData.nID);
                                        }

                                        Utility.DoStartFrameOperation("PocketJarFrame", string.Format("GoldenPot_Level_{0}", TR.Value("goldjar_level_type", nLevelType)));
                                    }
                                }
                            });
                            toggles.Add(toggle);
                        }

                        string strLevelName = TR.Value("goldjar_level_type", nLevelType);

                        TextEx Checkmark = bind.GetCom<TextEx>("name");
                        if (Checkmark != null)
                        {
                            Checkmark.text = strLevelName;
                        }
                        var jardata = JarDataManager.GetInstance().GetGoldJarData(a_nSubType, nLevelType);
                        Image icon = bind.GetCom<ImageEx>("Icon");
                        if (icon != null && jardata != null)
                        {
                            ETCImageLoader.LoadSprite(ref icon, jardata.strJarImagePath);
                        }
                        //Text Background = Utility.GetComponetInChild<Text>(objLevelType, "Background/Label");
                        //if (Background != null)
                        //{
                        //    Background.text = strLevelName;
                        //}

                        if (m_arrLevelRedPoints != null)
                        {
                            m_arrLevelRedPoints.Add(() =>
                            {
                                GameObject RedPoint = Utility.FindGameObject(objLevelType, "RedPoint");

                                if (RedPoint != null)
                                {
                                    RedPoint.SetActive(JarDataManager.GetInstance().CheckGoldJarLevelRedPoint(a_nSubType, nLevelType));
                                }
                            });
                        }
                    }

                    
                }
            }

            if (m_arrLevelRedPoints != null)
            {
                for (int i = 0; i < m_arrLevelRedPoints.Count; ++i)
                {
                    if (m_arrLevelRedPoints[i] != null)
                    {
                        m_arrLevelRedPoints[i].Invoke();
                    }
                }
            }

            isToggleInited = true;

            int nIdx = _GetLevelMatchedJarIndex(arrLevelTypes);
            if (arrLevelTypes != null && nIdx >= 0 && nIdx < arrLevelTypes.Count && nIdx < toggles.Count)
            {
                toggles[nIdx].isOn = true;
            }
            else
            {
                if (toggles.Count > 0)
                {
                    toggles[0].isOn = true;
                }
            }
        }

        int _GetLevelMatchedJarIndex(List<JarTreeNode> a_arrLevelTypes)
        {
            if (a_arrLevelTypes == null)
            {
                return -1;
            }

            int nCurrLevel = 0;
            int nIndex = -1;

            int nPlayerLevel = PlayerBaseData.GetInstance().Level;
            for (int i = 0; i < a_arrLevelTypes.Count; ++i)
            {
                int nLevelType = a_arrLevelTypes[i].nKey;
                JarData jarData = a_arrLevelTypes[i].value as JarData;

                if (nLevelType <= nPlayerLevel && nLevelType > nCurrLevel)
                {
                    nCurrLevel = nLevelType;
                    nIndex = i;
                }
            }

            return nIndex;
        }

        void _ClearGoldLevelTypes()
        {
            for (int i = 0; i < m_arrLevelTypeObjs.Count; ++i)
            {
                m_arrLevelTypeObjs[i].SetActive(false);
            }
        }

        void _InitGoldGoods(JarData a_data)
        {
            if (a_data == null)
            {
                return;
            }


            // m_imgGoldJarIcon.sprite = AssetLoader.GetInstance().LoadRes(a_data.strJarImagePath, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref m_imgGoldJarIcon, a_data.strJarImagePath);
            Assert.IsTrue(a_data.arrBuyItems != null && a_data.arrBuyItems.Count > 0);

            for (int i = 0; i < a_data.arrBuyInfos.Count; ++i)
            {
                GameObject objBuy = m_objGoldBuyFuncRoot.transform.GetChild(i).gameObject;
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

                    Utility.DoStartFrameOperation("PocketJarFrame", string.Format("GoldenPot_Buy{0}", buyInfo.nBuyCount));
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
            }
        }

        void _UpdateGoldGoods(JarData a_data)
        {
            for (int i = 0; i < a_data.arrBuyInfos.Count; ++i)
            {
                GameObject objBuy = m_objGoldBuyFuncRoot.transform.GetChild(i).gameObject;
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

            for (int i = 0; i < m_arrLevelRedPoints.Count; ++i)
            {
                m_arrLevelRedPoints[i].Invoke();
            }
        }

        protected override void OnInit(ClientFrame clientFrame = null)
        {
            if (clientFrame != null && clientFrame is JarFrame)
            {
                mJarFrame = clientFrame as JarFrame;
            }

            _InitGoldJarUI();
        }

        public void OnHistoryClick()
        {
            if (m_currGoldJarData != null && mJarFrame != null)
            {
                mJarFrame.OnHistoryClick(m_currGoldJarData.nID);
            }
        }

        public void OnPreTotalViewClick()
        {
            if (m_currGoldJarData == null)
            {
                return;
            }

            if (m_currGoldJarData.arrBonusItems == null)
            {
                return;
            }

            if (mJarFrame != null)
            {
                mJarFrame.OnPreTotalViewClick(m_currGoldJarData.arrRealBonusItems);
            }
        }

        public void OnPreviewClicked()
        {
            if (m_currGoldJarData == null)
            {
                return;
            }

            if (m_currGoldJarData.arrBonusItems == null)
            {
                return;
            }

            if (mJarFrame != null)
            {
                mJarFrame.OnPreviewClicked(m_currGoldJarData.arrRealBonusItems);
            }
        }

        protected override void OnVisible()
        {
        }

        
        protected override void OnInvisible()
        {
        }
        #endregion
    }
}
