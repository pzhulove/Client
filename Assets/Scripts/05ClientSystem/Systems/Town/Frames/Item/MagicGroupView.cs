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

    class MagicGroupView : CommonTabToggleView
    {
        

        #region magic jar
        [SerializeField]
        Image m_imgMagicJarIcon;


        [SerializeField]
        GameObject m_objMagicBuyRoot;

        [SerializeField]
        Text m_labMagicScore;

        [SerializeField]
        Text m_labMagicScoreDesc;

        EPocketJarType mCurPocketJarType = EPocketJarType.Magic;
        private JarFrame mJarFrame = null;

        JarData m_magicJarData
        {
            get
            {
                if (mCurPocketJarType == EPocketJarType.Magic_Lv55)
                {
                    return JarDataManager.GetInstance().GetMagicJarData_Lv55();
                }
                else
                {
                    return JarDataManager.GetInstance().GetMagicJarData();
                }
            }

            //set{ m_magicJarData = value; }
        }

        protected override void OnAddUIEvent()
        {
            base.OnAddUIEvent();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MagicJarUseSuccess, _OnUpdateJar);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerDataBaseUpdated, _OnUpdateJar);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MagicJarScoreChanged, _OnMagicJarScoreChanged);
        }

        protected override void OnRemoveUIEvent()
        {
            base.OnRemoveUIEvent();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MagicJarUseSuccess, _OnUpdateJar);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerDataBaseUpdated, _OnUpdateJar);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MagicJarScoreChanged, _OnMagicJarScoreChanged);
        }


        private void _OnUpdateJar(UIEvent a_event)
        {
            _InitMagicJarUI();
        }

        void _OnMagicJarScoreChanged(UIEvent a_event)
        {
            m_labMagicScore.SafeSetText(PlayerBaseData.GetInstance().MagicJarScore.ToString());
        }

        float m_fUpdateTime = 0.0f;

        void _InitMagicJarUI()
        {
            _OnMagicJarScoreChanged(null);

            if (mCurPocketJarType == EPocketJarType.Magic_Lv55)
            {
                m_labMagicScoreDesc.text = TR.Value("MagicScoreDesc_Lv55");
            }
            else
            {
                m_labMagicScoreDesc.text = TR.Value("MagicScoreDesc");
            }

            //m_magicJarData = JarDataManager.GetInstance().GetMagicJarData();
            _InitMagicJarGoods(m_magicJarData);
        }


        void _InitMagicJarGoods(JarData a_data)
        {
            if (a_data == null)
            {
                return;
            }

            ETCImageLoader.LoadSprite(ref m_imgMagicJarIcon, a_data.strJarImagePath);
            Assert.IsTrue(a_data.arrBuyItems != null && a_data.arrBuyItems.Count > 0);
            ItemData buyItem = a_data.arrBuyItems[0];



            for (int i = 0; i < m_objMagicBuyRoot.transform.childCount; ++i)
            {
                m_objMagicBuyRoot.transform.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = 0; i < a_data.arrBuyInfos.Count; ++i)
            {
                GameObject objBuy = m_objMagicBuyRoot.transform.GetChild(i).gameObject;
                objBuy.SetActive(true);
                JarBuyInfo buyInfo = a_data.arrBuyInfos[i];

                Button btnBuy = objBuy.GetComponent<Button>();
                btnBuy.onClick.RemoveAllListeners();
                btnBuy.onClick.AddListener(() =>
                {
                    ShowItemsFrame.bSkipExplode = false;

                    if (buyInfo.nFreeCount > 0)
                    {
                        JarDataManager.GetInstance().RequestBuyJar(a_data, buyInfo);
                        return;
                    }

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

                    Utility.DoStartFrameOperation("PocketJarFrame", string.Format("MagicPot_Buy{0}", buyInfo.nBuyCount));
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

                // 免费次数，时间
                _SetupFreeCD(objBuy, buyInfo);

            }
        }

        void _UpdateMagicJarGoods(JarData a_data)
        {
            for (int i = 0; i < a_data.arrBuyInfos.Count; ++i)
            {
                GameObject objBuy = m_objMagicBuyRoot.transform.GetChild(i).gameObject;
                objBuy.SetActive(true);
                JarBuyInfo buyInfo = a_data.arrBuyInfos[i];

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

                // 免费次数，时间
                _SetupFreeCD(objBuy, buyInfo);


            }
        }

        void _UpdateFreeCD(JarData a_data, float timeElapsed)
        {
            if (m_fUpdateTime <= 0)
            {
                return;
            }

            m_fUpdateTime -= timeElapsed;

            if (m_fUpdateTime <= 0)
            {
                for (int i = 0; i < a_data.arrBuyInfos.Count; ++i)
                {
                    JarBuyInfo buyInfo = a_data.arrBuyInfos[i];
                    if (buyInfo != null && buyInfo.nMaxFreeCount > 0)
                    {
                        _SetupFreeCD(m_objMagicBuyRoot.transform.GetChild(i).gameObject, buyInfo);
                    }
                }
            }
        }

        void _SetupFreeCD(GameObject a_objBuy, JarBuyInfo a_buyInfo)
        {
            GameObject objCost = Utility.FindGameObject(a_objBuy, "Price");
            GameObject objMagicFree = Utility.FindGameObject(a_objBuy, "Free", false);
            GameObject objFreeCD = Utility.FindGameObject(a_objBuy, "FreeCD", false);
            if (a_buyInfo.nMaxFreeCount > 0)
            {
                if (objMagicFree != null)
                {
                    objMagicFree.SetActive(a_buyInfo.nFreeCount > 0);

                    Text labFree = objMagicFree.GetComponent<Text>();
                    labFree.text = TR.Value("jar_free", a_buyInfo.nFreeCount, a_buyInfo.nMaxFreeCount);
                }
                objCost.gameObject.SetActive(a_buyInfo.nFreeCount <= 0);

                if (objFreeCD != null)
                {
                    if (a_buyInfo.nFreeCount < a_buyInfo.nMaxFreeCount)
                    {
                        objFreeCD.SetActive(true);
                        objFreeCD.GetComponent<Text>().text = _GetFreeTimeCDDesc(a_buyInfo.nFreeTimestamp);
                        m_fUpdateTime = 1.0f;
                    }
                    else
                    {
                        objFreeCD.SetActive(false);
                    }
                }
            }
            else
            {
                if (objMagicFree != null)
                {
                    objMagicFree.SetActive(false);
                }
                if (objFreeCD != null)
                {
                    objFreeCD.SetActive(false);
                }
                objCost.gameObject.SetActive(true);
            }
        }

        string _GetFreeTimeCDDesc(int a_timeStamp)
        {
            int nTimeLeft = a_timeStamp - (int)TimeManager.GetInstance().GetServerTime();
            if (nTimeLeft < 0)
            {
                nTimeLeft = 0;
            }

            int second = 0;
            int minute = 0;
            int hour = 0;
            second = nTimeLeft % 60;
            int temp = nTimeLeft / 60;
            if (temp > 0)
            {
                minute = temp % 60;
                hour = temp / 60;
            }

            return TR.Value("jar_free_cd", hour, minute, second);
        }

        public void OnShopClicked()
        {
            //ShopMainFrame.OpenLinkFrame("7|-1|7|2");

            //打开积分商店
            ShopNewDataManager.GetInstance().OpenShopNewFrame(7);
        }

        public void OnHistoryClick()
        {
            if (m_magicJarData != null && mJarFrame != null)
            {
                mJarFrame.OnHistoryClick(m_magicJarData.nID);
            }
        }

        public void OnPreTotalViewClick()
        {
            if (m_magicJarData == null)
            {
                return;
            }

            if (m_magicJarData.arrBonusItems == null)
            {
                return;
            }

            if (mJarFrame != null)
            {
                mJarFrame.OnPreTotalViewClick(m_magicJarData.arrRealBonusItems);
            }
        }

        public void OnPreviewClicked()
        {
            if(m_magicJarData == null)
            {
                return;
            }
            
            if(m_magicJarData.arrBonusItems == null)
            {
                return;
            }

            if (mJarFrame != null)
            {
                mJarFrame.OnPreviewClicked(m_magicJarData.arrRealBonusItems);
            }
        }

        protected override void OnInit(ClientFrame clientFrame = null)
        {
            if (clientFrame != null && clientFrame is JarFrame)
            {
                mJarFrame = clientFrame as JarFrame;
            }

            _InitMagicJarUI();
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
