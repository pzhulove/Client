using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

using UnityEngine.Assertions;
using ProtoTable;

namespace GameClient
{
    class ShowGuildDungeonChestItemsFrame : ClientFrame
    {
        [UIObject("Explode")]
        GameObject m_objExplodeRoot;

        [UIControl("Explode/Progress")]
        Slider m_explodeProgress;

        [UIControl("Explode/Progress/Name")]
        Text m_explodeRandomName;

        [UIControl("Result/Desc")]
        Text m_labBuyResult;

        [UIObject("Result/ItemGroup/10")]
        GameObject m_objItem_x10;

        [UIObject("Result/ItemGroup/10/Final")]
        GameObject m_objFinalItem_x10;

        [UIObject("Result/ItemGroup/10/Effect")]
        GameObject m_objEffectItem_x10;

        [UIObject("Result/ItemGroup/1")]
        GameObject m_objItem_x1;

        [UIObject("Result/ItemGroup/1/Final")]
        GameObject m_objFinalItem_x1;

        [UIObject("Result/ItemGroup/1/Effect")]
        GameObject m_objEffectItem_x1;

        [UIObject("Result/Score")]
        GameObject m_objScore;

        [UIControl("Result/Score/Desc")]
        Text m_labScoreDesc;

        [UIControl("Result/Score/Rate")]
        Text m_labScoreRate;

        [UIObject("Result/Buy")]
        GameObject m_objBuyRoot;

        [UIObject("Result/Mask")]
        GameObject m_objMask;

        [UIObject("Result/CustomOpen")]
        GameObject m_objCustomOpen;

        [UIControl("Result/CustomOpen/Desc")]
        Text m_labCustomOpenDesc;

        [UIControl("Result/CustomOpen/Open")]
        Button m_btnCustomOpen;

        class ItemInfo
        {
            public JarBonus bonus;

            public Text labName;
            public ComItem comItem;

            public ComCardEffect comCardEffect;
            public RectTransform tranRoot;
            public Animator animator;
            public ComItem comItemEffect;
            public Text labNameEffect;
            public Button btnBack;
            public Image imgBack;

            public Transform tranParBack;
            public Transform tranParFront;
            public Transform tranParTurnover;
        }
        List<ItemInfo> m_multiItemInfos = new List<ItemInfo>();
        List<ItemInfo> m_singleItemInfo = new List<ItemInfo>();

        Coroutine m_coroutine;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/ShowActivityAwardItems";
        }

        protected override void _OnOpenFrame()
        {
            List<GuildDungeonActivityChestItem> data = userData as List<GuildDungeonActivityChestItem>;
            if (data == null)
            {
                Logger.LogError("open ShowGuildDungeonChestItemsFrame frame data is null");
                return;
            }

            StartCoroutine(_ExplodeJar(data));
            //_RegisterUIEvent();

            //bSkipExplode = false;
        }

        protected override void _OnCloseFrame()
        {

            m_multiItemInfos.Clear();
            m_singleItemInfo.Clear();

            if (m_coroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(m_coroutine);
                m_coroutine = null;
            }

            _UnRegisterUIEvent();

            //bSkipExplode = false;
        }

        protected void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MagicJarUseSuccess, _OnJarUseSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MagicJarUseFail, _OnJarUseFail);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerDataBaseUpdated, _OnMoneyChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JarFreeTimeChanged, _OnFreeTimeChanged);
        }

        protected void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MagicJarUseSuccess, _OnJarUseSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MagicJarUseFail, _OnJarUseFail);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerDataBaseUpdated, _OnMoneyChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JarFreeTimeChanged, _OnFreeTimeChanged);
        }

        IEnumerator _ExplodeJar(List<GuildDungeonActivityChestItem> a_frameData)
        {
            //m_objMask.SetActive(false);

            #region prepare result ui

            m_explodeProgress.gameObject.SetActive(false);
            m_explodeProgress.value = 0.0f;
            m_explodeRandomName.gameObject.SetActive(false);
            m_explodeRandomName.text = string.Empty;

            m_labBuyResult.gameObject.SetActive(false);

            m_objItem_x1.SetActive(false);
            m_objFinalItem_x1.SetActive(false);
            m_objEffectItem_x1.SetActive(false);

            m_objItem_x10.SetActive(!false);
            m_objFinalItem_x10.SetActive(false);
            m_objEffectItem_x10.SetActive(false);

            m_objScore.SetActive(false);
            m_objBuyRoot.SetActive(false);
            m_objCustomOpen.SetActive(false);
       
            for (int i = 0; i < 1; ++i)
            {
                if (i >= m_singleItemInfo.Count)
                {
                    m_singleItemInfo.Add(_InitItemUI(m_objItem_x1, i, "UIFlatten/Prefabs/Jar/JarItem_One"));
                    yield return Yielders.EndOfFrame;
                }
            }

            for (int i = 0; i < 10; ++i)
            {
                if (i >= m_multiItemInfos.Count)
                {
                    m_multiItemInfos.Add(_InitItemUI(m_objItem_x10, i, "UIFlatten/Prefabs/Jar/JarItem"));
                    yield return Yielders.EndOfFrame;
                }
            }

            DOTweenAnimation[] anims = m_labScoreRate.GetComponents<DOTweenAnimation>();
            for (int i = 0; i < anims.Length; ++i)
            {
                anims[i].DORewind();
                anims[i].isActive = false;
            }
            #endregion

            #region explode
            string strJarModelPath = "Actor/NPC_Guildbox/Prefabs/p_NPC_Guildbox_UI";
            //string strJarModelPath = "UIFlatten/Prefabs/Jar/EffUI_xiuzhenguan07";

            GameObject objJarEffect = AssetLoader.GetInstance().LoadRes(strJarModelPath).obj as GameObject;
            objJarEffect.transform.SetParent(m_objExplodeRoot.transform, false);
            objJarEffect.SetActive(true);

            //DOTweenAnimation animRandom = Utility.GetComponetInChild<DOTweenAnimation>(objJarEffect, "p_Pot02/Bone007/Dummy001");
            //Image imgRandomBG = Utility.GetComponetInChild<Image>(objJarEffect, "p_Pot02/Bone007/Dummy001/BG");
            //imgRandomBG.gameObject.SetActive(false);
            //Image imgRandomIcon = Utility.GetComponetInChild<Image>(objJarEffect, "p_Pot02/Bone007/Dummy001/Icon");
            //imgRandomIcon.gameObject.SetActive(false);          

            if(!bSkipExplode)
            {
                yield return Yielders.GetWaitForSeconds(0.5f);
            }           

            if(!bSkipExplode)
            {
                AudioManager.instance.PlaySound(21);
            }            

            m_objExplodeRoot.CustomActive(true);
            //m_explodeProgress.gameObject.SetActive(true);
            //m_explodeRandomName.gameObject.SetActive(true);

            if (btnExplodeBack != null)
            {
                btnExplodeBack.CustomActive(false);
            }

            if (txtSkip != null)
            {
                txtSkip.CustomActive(false);
            }

            //animRandom.tween.Restart();
            float maxTime = 1.2f;
            m_explodeProgress.value = 0.0f;
            float startTime = Time.time;
            float elapsed = 0.0f;
            while (elapsed < maxTime)
            {
                if(bSkipExplode)
                {               
                    break;
                }

                elapsed = Time.time - startTime;
                m_explodeProgress.value = elapsed / maxTime;

                yield return Yielders.EndOfFrame;
            }
            m_explodeProgress.value = 1.0f;            

            m_explodeProgress.gameObject.SetActive(false);
            m_explodeRandomName.gameObject.SetActive(false);
            //animRandom.gameObject.SetActive(false);

            if (btnExplodeBack != null)
            {
                btnExplodeBack.CustomActive(false);
            }

            if (txtSkip != null)
            {
                txtSkip.CustomActive(false);
            }

            if(!bSkipExplode)
            {
                //yield return Yielders.GetWaitForSeconds(0.7f);
            }            

            m_objExplodeRoot.CustomActive(false);             

            #endregion

            #region show result
            yield return Yielders.EndOfFrame;
            //yield return Yielders.GetWaitForSeconds(0.55f);

            frameMgr.CloseFrame(this);

            for (int i = 0; i < a_frameData.Count; i++)
            {
                SystemNotifyManager.SysNotifyGetNewItemEffect(a_frameData[i].itemData, a_frameData[i].isHighValue);
            }

            #endregion

            //m_objMask.SetActive(true);

        }

        ItemInfo _InitItemUI(GameObject a_objItemGroup, int a_nIdx,string strJarItemPrefab)
        {
            ItemInfo itemInfo = new ItemInfo();

            GameObject objFinalRoot = Utility.FindGameObject(a_objItemGroup, string.Format("Final/ItemRoot_{0}", a_nIdx));
            //itemInfo.labName = Utility.GetComponetInChild<Text>(objFinalRoot, "Name");
            //itemInfo.comItem = CreateComItem(Utility.FindGameObject(objFinalRoot, "Item"));

            GameObject objEffectRoot = Utility.FindGameObject(a_objItemGroup, string.Format("Final/ItemRoot_{0}", a_nIdx));

            GameObject objEffectCard = AssetLoader.GetInstance().LoadResAsGameObject(strJarItemPrefab);
            objEffectCard.transform.SetParent(objEffectRoot.transform, false);

            itemInfo.tranRoot = objEffectCard.GetComponent<RectTransform>();
            itemInfo.animator = objEffectCard.GetComponent<Animator>();
            itemInfo.comCardEffect = objEffectCard.GetComponent<ComCardEffect>();
            itemInfo.comItemEffect = CreateComItem(Utility.FindGameObject(objEffectCard, "Content/Front/Item"));
            itemInfo.labNameEffect = Utility.GetComponetInChild<Text>(objEffectCard, "Content/Front/Name");
            itemInfo.btnBack = Utility.GetComponetInChild<Button>(objEffectCard, "Content/Back");
            itemInfo.imgBack = Utility.GetComponetInChild<Image>(objEffectCard, "Content/Back");
            itemInfo.tranParFront = Utility.FindGameObject(objEffectCard, "Content/Front/ParFront").transform;
            itemInfo.tranParBack = Utility.FindGameObject(objEffectCard, "Content/Back/ParBack").transform;
            itemInfo.tranParTurnover = Utility.FindGameObject(objEffectCard, "ParTurnover").transform;
            itemInfo.tranRoot.gameObject.CustomActive(false);

            return itemInfo;
        }

        IEnumerator _PlayItemsEffect(ShowItemsFrameData a_frameData, List<ItemInfo> a_arrItemInfos, GameObject a_objItemsRoot, GameObject a_objJarEffect,bool bSingleJar)
        {
            float fInterval = 0.0f;
            float fStay = 0.0f;
            ShowItemsCfg cfg = GetFrame().GetComponent<ShowItemsCfg>();
            if (cfg != null)
            {
                fInterval = cfg.fAniInterval;
                fStay = cfg.fHighValueItemAniStayTime;
            }

            List<JarBonus> arrBonus = a_frameData.items.GetRange(1, a_frameData.items.Count - 1);
            for (int i = 0; i < a_arrItemInfos.Count; ++i)
            {
                _SetupItemInfo(a_arrItemInfos[i], arrBonus[i]);

                a_arrItemInfos[i].tranRoot.gameObject.CustomActive(true);
                DOTweenAnimation[] anims = a_arrItemInfos[i].tranRoot.GetComponents<DOTweenAnimation>();
                for (int j = 0; j < anims.Length; ++j)
                {
                    string strID = anims[j].id as string;
                    if (strID != null && strID == "scale")
                    {                      
                        anims[j].tween.OnComplete(() => 
                        {
                            Debug.LogWarning("scale complete");
                            _SetupCardPar(a_arrItemInfos[i]);
                        });
                        anims[j].tween.Restart();
                    }
                    else if (strID != null && strID == "move")
                    {
                        anims[j].tween.OnComplete(() =>
                        {
                            Debug.LogWarning("move complete");                            
                        });
                        anims[j].tween.Restart();
                    }
                }


                float fTime = fInterval;
                if (a_arrItemInfos[i].bonus.bHighValue)
                {
                    fTime += fStay;
                }

                yield return Yielders.GetWaitForSeconds(fTime);
            }           


//             Toggle toggle = a_objItemsRoot.GetComponent<Toggle>();
//             toggle.isOn = false;
// 
//             a_objItemsRoot.SetActive(true);
//             Animator anim = a_objItemsRoot.GetComponent<Animator>();
//             //anim.enabled = true;
// 
//             while (toggle.isOn == false)
//             {
//                 yield return Yielders.EndOfFrame;
//             }
// 
//             anim.enabled = false;
// 
//             // play turnover effects
//             _PlayTurnoverEffects(a_arrItemInfos);
// 
//             // wait for trunover end
//             while (true)
//             {
//                 bool bNeedWait = false;
//                 for (int i = 0; i < a_arrItemInfos.Count; ++i)
//                 {
//                     if (a_arrItemInfos[i].comCardEffect.bFinished == false)
//                     {
//                         bNeedWait = true;
//                         break;
//                     }
//                     else
//                     {
//                         a_arrItemInfos[i].animator.enabled = false;
//                     }
//                 }
// 
//                 if (bNeedWait)
//                 {
//                     yield return Yielders.GetWaitForSeconds(0.1f);
//                 }
//                 else
//                 {
//                     break;
//                 }
//             }
// 
            JarDataManager.GetInstance().RequestJarBuyRecord(a_frameData.data.nID);

            _CloseAutoOpen();

            GameObject.Destroy(a_objJarEffect);

            yield return 0;
        }

        IEnumerator _AutoOpenCountdown(List<ItemInfo> a_arrItemInfos)
        {
            for (int i = 5; i > 0; --i)
            {
                m_labCustomOpenDesc.text = TR.Value("jar_auto_open", i);
                yield return Yielders.GetWaitForSeconds(1.0f);
            }

            _OpenHighValueCard(a_arrItemInfos);
            m_objCustomOpen.SetActive(false);
            m_coroutine = null;
        }

        void _PlayTurnoverEffects(List<ItemInfo> a_arrItemInfos)
        {
            bool bHasHighValue = false;

            for (int i = 0; i < a_arrItemInfos.Count; ++i)
            {
                ItemInfo itemInfo = a_arrItemInfos[i];
                //_PlayItemParticle(itemInfo);
                // 非高价值道具，自动翻牌
                
                if (itemInfo.bonus.bHighValue == false)
                {
                    itemInfo.animator.enabled = true;
                    //itemInfo.animator.Play("Animation_fan_01");
                    itemInfo.btnBack.onClick.RemoveAllListeners();
                }
                else
                {
                    itemInfo.animator.enabled = false;
                    itemInfo.btnBack.onClick.RemoveAllListeners();
                    itemInfo.btnBack.onClick.AddListener(() =>
                    {
                        itemInfo.animator.enabled = true;
                    });

                    bHasHighValue = true;
                }
            }

            if (bHasHighValue)
            {
                m_objCustomOpen.SetActive(true);
                // 5秒后自动翻开
                if (m_coroutine != null)
                {
                    GameFrameWork.instance.StopCoroutine(m_coroutine);
                    m_coroutine = null;
                }
                m_coroutine = GameFrameWork.instance.StartCoroutine(_AutoOpenCountdown(a_arrItemInfos));

                m_btnCustomOpen.onClick.RemoveAllListeners();
                m_btnCustomOpen.onClick.AddListener(() =>
                {
                    _CloseAutoOpen();
                    _OpenHighValueCard(a_arrItemInfos);
                });
            }
            else
            {
                m_objCustomOpen.SetActive(false);
            }
        }

        void _SetupCardPar(ItemInfo a_itemInfo)
        {
            ProtoTable.ItemTable.eColor quality = a_itemInfo.bonus.item.GetQualityInfo().Quality;
            switch (quality)
            {
                case ProtoTable.ItemTable.eColor.GREEN:
                    {
                        {
                            GameObject ObjEffect = AssetLoader.GetInstance().LoadRes("Effects/UI/Prefab/EffUI_pinji/Prefab/EffUI_pinjiguang_lvse").obj as GameObject;
                            ObjEffect.transform.SetParent(a_itemInfo.tranParFront, false);
                            ObjEffect.SetActive(true);
                        }

                        break;
                    }
                case ProtoTable.ItemTable.eColor.PINK:
                    {

                        {
                            GameObject ObjEffect = AssetLoader.GetInstance().LoadRes("Effects/UI/Prefab/EffUI_pinji/Prefab/EffUI_pinjiguang_fense").obj as GameObject;
                            ObjEffect.transform.SetParent(a_itemInfo.tranParFront, false);
                            ObjEffect.SetActive(true);
                        }
                        break;
                    }
                case ProtoTable.ItemTable.eColor.YELLOW:
                    {

                        {
                            GameObject ObjEffect = AssetLoader.GetInstance().LoadRes("Effects/UI/Prefab/EffUI_pinji/Prefab/EffUI_pinjiguang_jinse").obj as GameObject;
                            ObjEffect.transform.SetParent(a_itemInfo.tranParFront, false);
                            ObjEffect.SetActive(true);
                        }
                        break;
                    }
                default:
                    {
                        if (a_itemInfo.bonus.bHighValue)
                        {

                            {
                                GameObject ObjEffect = AssetLoader.GetInstance().LoadRes("Effects/UI/Prefab/EffUI_pinji/Prefab/EffUI_pinjiguang_zise").obj as GameObject;
                                ObjEffect.transform.SetParent(a_itemInfo.tranParFront, false);
                                ObjEffect.SetActive(true);
                            }
                        }
                        break;
                    }
            }

            if (a_itemInfo.bonus.bHighValue)
            {
                GameObject ObjEffect = AssetLoader.GetInstance().LoadRes("Effects/Scene_effects/EffectUI/Fanka/EffUI_fan").obj as GameObject;
                ObjEffect.transform.SetParent(a_itemInfo.tranParTurnover, false);
                ObjEffect.SetActive(true);
            }
        }

        void _SetupItemInfo(ItemInfo a_itemInfo, JarBonus a_bonus)
        {
            for (int i = 0; i < a_itemInfo.tranParBack.childCount; ++i)
            {
                GameObject.Destroy(a_itemInfo.tranParBack.GetChild(i).gameObject);
            }
            for (int i = 0; i < a_itemInfo.tranParFront.childCount; ++i)
            {
                GameObject.Destroy(a_itemInfo.tranParFront.GetChild(i).gameObject);
            }
            for (int i = 0; i < a_itemInfo.tranParTurnover.childCount; ++i)
            {
                GameObject.Destroy(a_itemInfo.tranParTurnover.GetChild(i).gameObject);
            }

            a_itemInfo.btnBack.gameObject.SetActive(false);
            a_itemInfo.btnBack.onClick.RemoveAllListeners();
            a_itemInfo.comCardEffect.bFinished = false;

            string strCardBack;
            switch( a_bonus.item.Quality)
            {
                case ProtoTable.ItemTable.eColor.GREEN:
                    {
                        strCardBack = "UI/Image/Packed/p_UI_Kapai.png:UI_Kapai_Lvse";
                        break;
                    }
                case ProtoTable.ItemTable.eColor.PINK:
                    {
                        strCardBack = "UI/Image/Packed/p_UI_Kapai.png:UI_Kapai_Fense";
                        break;
                    }
                case ProtoTable.ItemTable.eColor.YELLOW:
                    {
                        strCardBack = "UI/Image/Packed/p_UI_Kapai.png:UI_Kapai_Jinse";
                        break;
                    }
                default:
                    {
                        strCardBack = "UI/Image/Packed/p_UI_Kapai.png:UI_Kapai_Huise";
                        break;
                    }
            }
            // a_itemInfo.imgBack.sprite = AssetLoader.GetInstance().LoadRes(strCardBack, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref a_itemInfo.imgBack, strCardBack);

            // TODO 重置动画
            a_itemInfo.tranRoot.localRotation = Quaternion.identity;
            a_itemInfo.tranRoot.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            for (int i = 0; i < a_itemInfo.tranRoot.childCount; ++i)
            {
                Transform temp = a_itemInfo.tranRoot.GetChild(i);
                temp.localRotation = Quaternion.identity;
                temp.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }

            a_itemInfo.bonus = a_bonus;          

//             a_itemInfo.comItem.Setup(a_itemInfo.bonus.item, (var1, var2) =>
//             {
//                 ItemTipManager.GetInstance().ShowTip(var2);
//             });
//             a_itemInfo.labName.text = a_itemInfo.bonus.item.GetColorName();

            a_itemInfo.comItemEffect.Setup(a_itemInfo.bonus.item, (var1, var2) =>
            {
                ItemTipManager.GetInstance().ShowTip(var2);
            });
            a_itemInfo.labNameEffect.text = a_itemInfo.bonus.item.GetColorName();

            //_SetupCardPar(a_itemInfo);
        }

        void _SetupBuybtns(ShowItemsFrameData a_frameData)
        {
            for (int i = 1; i < m_objBuyRoot.transform.childCount; ++i)
            {
                m_objBuyRoot.transform.GetChild(i).gameObject.SetActive(false);
            }

            if (a_frameData.buyInfo == null)
            {
                return;
            }

            GameObject objBuy = m_objBuyRoot.transform.GetChild(1).gameObject;
            objBuy.SetActive(true);
            mRewardItem.CustomActive(false);
            if (a_frameData.data.eType == ProtoTable.JarBonus.eType.EqrecoJar)
            {
                objBuy.SetActive(false);
                //if(a_frameData.items[0].)
                if ( a_frameData.items != null && a_frameData.items.Count != 0)
                {
                    var itemData = a_frameData.items[0].item;
                    if(itemData != null && itemData.TableID != 600000007)
                    {
                        var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(itemData.TableID);
                        if(itemTableData != null)
                        {
                            mText.text = itemTableData.Name;
                        }
                        mRewardItem.CustomActive(true);
                        ComItem comitem = mRewardItem.GetComponentInChildren<ComItem>();
                        if (comitem == null)
                        {
                            comitem = CreateComItem(mRewardItem);
                        }
                        int result = itemData.TableID;
                        comitem.Setup(itemData, (GameObject Obj, ItemData sitem) => { _OnShowTips(result); });

                    }
                }
                
            }
            JarBuyInfo buyInfo = a_frameData.buyInfo;

            Button btnBuy = objBuy.GetComponent<Button>();
            btnBuy.onClick.RemoveAllListeners();
            btnBuy.onClick.AddListener(() =>
            {
                ShowItemsFrame.bSkipExplode = true;

                if (buyInfo.nFreeCount > 0)
                {
                    JarDataManager.GetInstance().RequestBuyJar(a_frameData.data, buyInfo);
                    return;
                }
                
//                 if(SecurityLockDataManager.GetInstance().CheckSecurityLock())
//                 {
//                     return;
//                 }

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
                    JarDataManager.GetInstance().RequestBuyJar(a_frameData.data, buyInfo);
                    //frameMgr.CloseFrame(this);
                });
            });


            Utility.GetComponetInChild<Text>(objBuy, "Time").text = TR.Value("magicjar_buy_again_time", buyInfo.nBuyCount);
            Text labCount = Utility.GetComponetInChild<Text>(objBuy, "Price/Count");
            Image imgIcon = Utility.GetComponetInChild<Image>(objBuy, "Price/Icon");
            if(buyInfo.arrCosts != null)
            {
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
            

            _SetupFreeCD(objBuy, buyInfo);
        }
        void _OnShowTips(int result)
        {
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result);
            if (ItemDetailData == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }
        void _CloseAutoOpen()
        {
            m_objCustomOpen.SetActive(false);
            if (m_coroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(m_coroutine);
                m_coroutine = null;
            }
        }

        void _OpenHighValueCard(List<ItemInfo> a_arrItemInfos)
        {
            for (int i = 0; i < a_arrItemInfos.Count; ++i)
            {
                ItemInfo itemInfo = a_arrItemInfos[i];
                if (itemInfo.bonus.bHighValue && itemInfo.comCardEffect.bFinished == false)
                {
                    itemInfo.animator.enabled = true;
                    itemInfo.btnBack.onClick.RemoveAllListeners();
                }
            }
        }

        void _SetupFreeCD(GameObject a_objBuy, JarBuyInfo a_buyInfo)
        {
            GameObject objCost = Utility.FindGameObject(a_objBuy, "Price");
            GameObject objMagicFree = Utility.FindGameObject(a_objBuy, "Free", false);
            //GameObject objFreeCD = Utility.FindGameObject(a_objBuy, "FreeCD", false);
            if (a_buyInfo.nMaxFreeCount > 0)
            {
                if (objMagicFree != null)
                {
                    objMagicFree.SetActive(a_buyInfo.nFreeCount > 0);

                    Text labFree = objMagicFree.GetComponent<Text>();
                    labFree.text = TR.Value("jar_free", a_buyInfo.nFreeCount, a_buyInfo.nMaxFreeCount);
                }
                objCost.gameObject.SetActive(a_buyInfo.nFreeCount <= 0);

//                 if (objFreeCD != null)
//                 {
//                     if (a_buyInfo.nFreeCount < a_buyInfo.nMaxFreeCount)
//                     {
//                         objFreeCD.SetActive(true);
//                     }
//                     else
//                     {
//                         objFreeCD.SetActive(false);
//                     }
//                 }
            }
            else
            {
                if (objMagicFree != null)
                {
                    objMagicFree.SetActive(false);
                }
//                 if (objFreeCD != null)
//                 {
//                     objFreeCD.SetActive(false);
//                 }
                objCost.gameObject.SetActive(true);
            }
        }

        void _OnJarUseSuccess(UIEvent a_event)
        {
            //ShowItemsFrameData frameData = a_event.Param1 as ShowItemsFrameData;
            //userData = frameData;

            //StartCoroutine(_ExplodeJar(frameData));
            frameMgr.CloseFrame(this);
        }

        void _OnJarUseFail(UIEvent a_event)
        {
            frameMgr.CloseFrame(this);
        }

        void _OnMoneyChanged(UIEvent a_event)
        {
            _SetupBuybtns(userData as ShowItemsFrameData);
        }

        void _OnFreeTimeChanged(UIEvent a_event)
        {
            _SetupBuybtns(userData as ShowItemsFrameData);
        }

        [UIEventHandle("Result/Buy/Func")]
        void _OnReturnClicked()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Result/CustomOpen/Open")]
        void _OnCustomOpenClicked()
        {

        }
        #region ExtraUIBind
        private GameObject mRewardItem = null;
        private Text mText = null;
        private Button btnExplodeBack = null;
        private Text txtSkip = null;
        private GameObject goGongxihuode = null;
        private GameObject goBg1 = null;
        private GameObject goBg3 = null;

        private GameObject goGongxihuode_1 = null;
        private GameObject goBg1_1 = null;
        private GameObject goBg3_1 = null;

        public static bool bSkipExplode = false;

        protected override void _bindExUI()
        {
            mRewardItem = mBind.GetGameObject("rewardItem");
            mText = mBind.GetCom<Text>("text");

            float fTime = Time.time;
            btnExplodeBack = mBind.GetCom<Button>("btnExplodeBack");
            if (btnExplodeBack != null)
            {
                btnExplodeBack.onClick.RemoveAllListeners();
                btnExplodeBack.onClick.AddListener(() =>
                {
//                     if(Time.time - fTime < 1.0f)
//                     {
//                         return;
//                     }

                    //bSkipExplode = true;
                });
            }
            txtSkip = mBind.GetCom<Text>("txtSkip");

            goGongxihuode = mBind.GetGameObject("gongxihuode");
            goBg1 = mBind.GetGameObject("BG (1)");
            goBg3 = mBind.GetGameObject("BG (3)");

            goGongxihuode_1 = mBind.GetGameObject("gongxihuode_1");
            goBg1_1 = mBind.GetGameObject("BG (1)_1");
            goBg3_1 = mBind.GetGameObject("BG (3)_1");
        }

        protected override void _unbindExUI()
        {
            mRewardItem = null;
            mText = null;

            btnExplodeBack = null;
            txtSkip = null;

            goGongxihuode = null;
            goBg1 = null;
            goBg3 = null;

            goGongxihuode_1 = null;
            goBg1_1 = null;
            goBg3_1 = null;
        }
        #endregion
    }
}
