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

    class RelicGroupView : CommonTabToggleView
    {
        class ActJarInfo
        {
            public ActivityInfo activity;
            public JarData jarData;
        }
        ActJarInfo m_selectEquipActJar = null;
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
        ActJarInfo m_currActJar = null;
        List<GameObject> m_arrLevelTypeObjs = new List<GameObject>();
        List<OnRedPointChanged> m_arrLevelRedPoints = new List<OnRedPointChanged>();
        private JarFrame mJarFrame = null;


        protected void _InitRelicJarUI()
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
            m_currActJar = null;
            m_arrLevelTypeObjs.Clear();
            m_arrLevelRedPoints.Clear();
        }

        void _InitGoldLevelTypes(int a_nSubType)
        {
            List<Toggle> toggles = new List<Toggle>();
            bool isToggleInited = false;
            List<ActJarInfo> arrLevelTypes = new List<ActJarInfo>(); ;
            var iter = TableManager.GetInstance().GetTable<ProtoTable.ActivityJarTable>().GetEnumerator();
            while (iter.MoveNext())
            {
                ActivityInfo info = null;
                ActiveManager.GetInstance().allActivities.TryGetValue(iter.Current.Key, out info);
                if (info != null && info.state != 0)
                {
                    ProtoTable.ActivityJarTable table = iter.Current.Value as ProtoTable.ActivityJarTable;
                    JarData jarData = JarDataManager.GetInstance().GetJarData(table.JarID);
                    if (jarData.eType == EJarType.EquipJar)
                    {
                        ActJarInfo data = new ActJarInfo();
                        data.activity = info;
                        data.jarData = jarData;

                        // 有个坑爹的需求
                        // 60级罐子不参加 神器罐派对！！！
                        // add by qxy 2019-07-23
                        bool bAdd = true;
                        //                         if(frameType == ActivityJarFrameType.Artifact && info.id == 9007)
                        //                         {
                        //                             bAdd = false;
                        //                         }

                        if (bAdd)
                        {
                            arrLevelTypes.Add(data);
                        }
                    }
                }
            }

            if (arrLevelTypes != null)
            {
                for (int i = 0; i < arrLevelTypes.Count; ++i)
                {
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
                            int idx = i;
                            toggle.onValueChanged.AddListener(var =>
                            {
                                if (isToggleInited)
                                {
                                    if (var)
                                    {
                                        m_currActJar = arrLevelTypes[idx];
                                        _InitRelicGoods(m_currActJar.jarData);

                                        if (true)
                                        {
                                            JarDataManager.GetInstance().RequestJarBuyRecord(m_currActJar.jarData.nID);
                                        }

                                        //Utility.DoStartFrameOperation("ActivityJarFrame", string.Format("{0}ArtifactTank_ItemId_{1}", m_currActJar.activity.level, var2.TableID));
                                    }
                                }
                            });
                            toggles.Add(toggle);
                        }

                        string strLevelName = arrLevelTypes[i].jarData.strName;

                        TextEx Checkmark = bind.GetCom<TextEx>("name");
                        if (Checkmark != null)
                        {
                            Checkmark.text = strLevelName;
                        }
                        var jardata = arrLevelTypes[i].jarData;
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


                    }

                    
                }
            }


            isToggleInited = true;

            int nIdx = _GetLevelMatchAct(arrLevelTypes);
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
        int _GetLevelMatchAct(List<ActJarInfo> a_arrData)
        {
            a_arrData.Sort((var1, var2) =>
            {
                return var1.jarData.arrFilters[0] - var2.jarData.arrFilters[0];
            });

            int nIndex = 0;
            int nPlayerLevel = PlayerBaseData.GetInstance().Level;
            for (int i = 0; i < a_arrData.Count; ++i)
            {
                int nLevel = a_arrData[i].jarData.arrFilters[0];
                if (nLevel <= nPlayerLevel)
                {
                    nIndex = i;
                }
            }

            if (nIndex >= 0 && nIndex < a_arrData.Count)
            {
                return nIndex;
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

        void _InitRelicGoods(JarData a_data)
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

            _InitRelicJarUI();
        }

        public void OnHistoryClick()
        {
            if (m_currActJar != null && m_currActJar.jarData != null && mJarFrame != null)
            {
                mJarFrame.OnHistoryClick(m_currActJar.jarData.nID);
            }
        }

        public void OnPreTotalViewClick()
        {
            if (m_currActJar == null || m_currActJar.jarData == null)
            {
                return;
            }

            if (m_currActJar.jarData.arrBonusItems == null)
            {
                return;
            }

            if (mJarFrame != null)
            {
                mJarFrame.OnPreTotalViewClick(m_currActJar.jarData.arrRealBonusItems);
            }
        }

        public void OnPreviewClicked()
        {
            if (m_currActJar == null || m_currActJar.jarData == null)
            {
                return;
            }

            if (m_currActJar.jarData.arrBonusItems == null)
            {
                return;
            }

            if (mJarFrame != null)
            {
                mJarFrame.OnPreviewClicked(m_currActJar.jarData.arrRealBonusItems);
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
