using UnityEngine.UI;
using Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using EItemType = ProtoTable.ItemTable.eType;
using ProtoTable;
using System.Collections;

namespace GameClient
{
    public enum UpgradeType
    {
        stop = 0,//未开始
        begining = 1,//正在一键提升
        fail = 2,
        success = 3,
        returned = 4,//被主动点击返回
        immediately = 5,//跳过动画加速得到成功结果，对应一件提升按钮
    }
    class EquipUpgradeFrame : ClientFrame
    {
        int canUpgradeScore = 0;
        UpgradeType curUpgradeType = UpgradeType.stop;
        ulong upgradingGuid = 0;

        int beginScore = 0;
        int endScore = 0;
        int beginGold = 0;
        int endGold = 0;

        int totalCount = 0;
        List<int> upgradeResultList = new List<int>();
        int consume = 0;
        int upgradeScore = -1;

        List<ulong> upgradeEquipList = new List<ulong>();
        //Dictionary<ulong, int> upgradeScoreDic = new Dictionary<ulong, int>();
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/EquipRecovery/EquipUpgradeFrame";
        }
        protected sealed override void _OnOpenFrame()
        {
            _InitData();
            _RegisterUIEvent();
        }
        protected sealed override void _OnCloseFrame()
        {
            _ClearData();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipUpgradeSuccess, _OnEquipUpgradeSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipRecivertDeleteItem, _OnUpdateDisplayList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipUpgradeFail, _OnEquipUpgradeFail);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GoldChanged, _OnGoldChanged);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipUpgradeSuccess, _OnEquipUpgradeSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipRecivertDeleteItem, _OnUpdateDisplayList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipUpgradeFail, _OnEquipUpgradeFail);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GoldChanged, _OnGoldChanged);
        }

        void _InitData()
        {
            curUpgradeType = UpgradeType.stop;
            EquipRecoveryDataManager.GetInstance().isUpgradeing = false;
            upgradeEquipList.Clear();
            _InitReturnItemScrollListBind();
            _InitResultItemScrollListBind();
            _UpdateEquipList();
        }
        void _ClearData()
        {
            curUpgradeType = UpgradeType.stop;
            canUpgradeScore = 0;
            upgradeEquipList.Clear();
        }


        void _UpdateEquipList()
        {
            canUpgradeScore = 0;
            upgradeEquipList.Clear();
            List<ulong> tempdisplayList = new List<ulong>();
            tempdisplayList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.EquipRecovery);
            for (int i = 0; i < tempdisplayList.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(tempdisplayList[i]);
                if(itemData == null)
                {
                    continue;
                }
                int maxScore = EquipRecoveryDataManager.GetInstance()._GetEquipPrice(itemData, false);
                canUpgradeScore += maxScore - itemData.RecoScore;
                if(maxScore > itemData.RecoScore)
                {
                    upgradeEquipList.Add(tempdisplayList[i]);
                }
            }
            SortMyList();
            mTips.CustomActive(upgradeEquipList.Count == 0);
            mCanRedeemCount.text = canUpgradeScore.ToString();
            _RefreshItemListCount();
        }

        void _UpdateResultList()
        {
            _RefreshResultListCount();
            mTitleText.text = TR.Value("equip_upgrade_result_title", totalCount.ToString());
        }

        private void SortMyList()
        {
            upgradeEquipList.Sort((x, y) =>
            {
                int result;
                ItemData itemData1 = ItemDataManager.GetInstance().GetItem(x);
                int maxScore1 = EquipRecoveryDataManager.GetInstance()._GetEquipPrice(itemData1, false);
                ItemData itemData2 = ItemDataManager.GetInstance().GetItem(y);
                int maxScore2 = EquipRecoveryDataManager.GetInstance()._GetEquipPrice(itemData2, false);
                int result_x = maxScore1 - itemData1.RecoScore;
                int result_y = maxScore2 - itemData2.RecoScore;
                if (result_x.CompareTo(result_y) > 0)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
                return result;
            });
        }
        void _RefreshItemListCount()
        {
            mEquipUIList.SetElementAmount(upgradeEquipList.Count);
        }

        void _RefreshResultListCount()
        {
            mResultUIList.SetElementAmount(upgradeResultList.Count);
        }

        void _InitReturnItemScrollListBind()
        {
            mEquipUIList.Initialize();

            mEquipUIList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdateItemScrollListBind(item);
                }
            };

            mEquipUIList.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }
                var mUpgrade = combind.GetCom<Button>("upgrade");
                if (mUpgrade != null)
                {
                    mUpgrade.onClick.RemoveAllListeners();
                }
                var mYjUpgrade = mBind.GetCom<Button>("YjUpgrade");
                if(mYjUpgrade != null)
                {
                    mYjUpgrade.onClick.RemoveAllListeners();
                }
            };
        }

        void _InitResultItemScrollListBind()
        {
            mResultUIList.Initialize();

            mResultUIList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdateResultScrollListBind(item);
                }
            };
            mResultUIList.OnItemRecycle = (item) =>
            {
                ComCommonBind rBind = item.GetComponent<ComCommonBind>();
                var rLine = rBind.GetGameObject("Line");
                rLine.CustomActive(true);
            };
        }

        void _UpdateResultScrollListBind(ComUIListElementScript item)
        {
            ComCommonBind rBind = item.GetComponent<ComCommonBind>();
            if(rBind == null)
            {
                return;
            }
            if(item.m_index <0 || item.m_index >= upgradeResultList.Count)
            {
                return;
            }
            int result = upgradeResultList[item.m_index];
            if(result < 0)
            {
                return;
            }

            var rLine = rBind.GetGameObject("Line");
            var rResultText = rBind.GetCom<Text>("Result");
            var rConsumeText = rBind.GetCom<Text>("Consume");

            if(item.m_index == 0)
            {
                rLine.CustomActive(false);
            }
            else
            {
                rLine.CustomActive(true);
            }
            if(result > 0)
            {
                rResultText.text = TR.Value("equip_upgrade_succeed", result);
            }
            else
            {
                rResultText.text = TR.Value("equip_upgrade_fail", result);
            }
            rConsumeText.text = TR.Value("equip_upgrade_consume", consume);
        }

        void _UpdateItemScrollListBind(ComUIListElementScript item)
        {
            ComCommonBind mBind = item.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }

            if(upgradeEquipList == null)
            {
                return;
            }
            if (item.m_index < 0 || item.m_index >= upgradeEquipList.Count)
            {
                return;
            }
            ulong guid = upgradeEquipList[item.m_index];
            ItemData itemData = ItemDataManager.GetInstance().GetItem(guid);
            if (itemData == null)
            {
                return;
            }


            var mItemRoot = mBind.GetGameObject("itemRoot");
            var mExpectCount = mBind.GetCom<Text>("expectCount");
            var mCount = mBind.GetCom<Text>("count");
            var mUpgrade = mBind.GetCom<Button>("upgrade");
            var mCannotRedeem = mBind.GetGameObject("cannotRedeem");
            var mCanUpgradeScores = mBind.GetCom<Text>("canUpgradeScores");
            var mYjUpgrade = mBind.GetCom<Button>("YjUpgrade");
            int maxScore = EquipRecoveryDataManager.GetInstance()._GetEquipPrice(itemData, false);

            int money = -1;
            int count = CountDataManager.GetInstance().GetCount(CounterKeys.EQUIP_RECOVERY_WEEK_COUNT);
            var tabledata = TableManager.GetInstance().GetTable<EquipRecoScUpConsRtiTable>();
            if(tabledata == null)
            {
                return;
            }
            var enumerator = tabledata.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var tableItem = enumerator.Current.Value as EquipRecoScUpConsRtiTable;
                if(tableItem.TimesSection.Count < 2)
                {
                    break;
                }
                int leftSection = -1;
                int rightSection = -1;
                int.TryParse(tableItem.TimesSection[0], out leftSection);
                int.TryParse(tableItem.TimesSection[1], out rightSection);
                if(leftSection == -1 || rightSection == -1)
                {
                    break;
                }

                if (count >= leftSection && count <= rightSection || count>=leftSection && rightSection == -1)
                {
                    money = tableItem.Ratio * maxScore;
                }
            }

            
            mUpgrade.onClick.RemoveAllListeners();
            mUpgrade.onClick.AddListener(() =>
            {
                string notifyCount = "";
                if (money != -1)
                {
                    notifyCount = string.Format(TR.Value("equip_upgrade_tip"), money);
                }
                if (notifyCount != "" && EquipRecoveryDataManager.GetInstance().isUpgradeing != true)
                {
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCount, () =>
                    {
                        int goldCount = money;
                        consume = money;
                        int goldID = 0;
                        SystemValueTable goldIDData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_RECO_UPSHB);
                        if (goldIDData != null)
                        {
                            goldID = goldIDData.Value;
                        }
                        CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                        if(costInfo != null)
                        {
                        costInfo.nMoneyID = goldID;

                        ItemTipManager.GetInstance().CloseAll();

                        costInfo.nCount = goldCount;

                        CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                        {
                            //ui表现
                            beginScore = endScore = itemData.RecoScore;
                            beginGold = endGold = _GetUpgradeGoldCount();
                            mYjReturn.CustomActive(false);
                            mYjUpgradeRoot.CustomActive(true);
                            mUpgradeResult.CustomActive(false);
                            mImmediate.CustomActive(false);
                            mContinueButton.CustomActive(true);
                                if(mContinueButton != null)
                                {
                            mContinueButton.enabled = true;
                                }
                                if(mContinueButtonText != null)
                                {
                            mContinueButtonText.text = TR.Value("equip_upgrade_continue");
                                }
                                if(mContinueUIGray != null)
                                {
                            mContinueUIGray.enabled = false;
                                }                                
                            upgradingGuid = guid;

                                if(mBeginCount != null)
                                {
                            mBeginCount.text = beginScore.ToString();
                                }
                                if(mEndCount != null)
                                {
                            mEndCount.text = endScore.ToString();
                                }                                
                            endGold = (int)PlayerBaseData.GetInstance().Gold;
                                if(mConsumeCount != null)
                                {
                            mConsumeCount.text = (beginGold - endGold).ToString();
                                }

                            //变更一键提升相关的状态
                            EquipRecoveryDataManager.GetInstance().isUpgradeing = false;
                            curUpgradeType = UpgradeType.stop;

                            //发送协议
                            EquipRecoveryDataManager.GetInstance()._UpgradeEquip(guid);
                            totalCount += 1;
                        });
                        }                        
                    });
                }
                else
                {
                    //去掉log
                }
            });
            
            mYjUpgrade.onClick.RemoveAllListeners();
            mYjUpgrade.onClick.AddListener(() =>
            {
                if(EquipRecoveryDataManager.GetInstance().isUpgradeing != true)
                {
                    string yjUpgradeStr = TR.Value("equip_yj_upgrade_tip");
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(yjUpgradeStr, () =>
                    {
                        int goldCount = money;
                        consume = money;
                        int goldID = 0;
                        SystemValueTable goldIDData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_RECO_UPSHB);
                        if (goldIDData != null)
                        {
                            goldID = goldIDData.Value;
                        }
                        CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                        if(costInfo != null)
                        {
                        costInfo.nMoneyID = goldID;

                        ItemTipManager.GetInstance().CloseAll();

                        costInfo.nCount = goldCount;

                        CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                        {
                            //_StartYjUpgrade();
                            //一键提升相关的状态的改变
                            EquipRecoveryDataManager.GetInstance().isUpgradeing = true;
                            curUpgradeType = UpgradeType.begining;

                            //ui
                            mYjUpgradeRoot.CustomActive(true);
                            mYjReturn.CustomActive(true);
                            mImmediate.CustomActive(true);
                            mUpgradeResult.CustomActive(false);
                            upgradingGuid = guid;
                            beginScore = endScore = itemData.RecoScore;
                            beginGold = endGold = _GetUpgradeGoldCount();
                                if(mBeginCount != null)
                                {
                            mBeginCount.text = beginScore.ToString();
                                }
                                if(mEndCount != null)
                                {
                            mEndCount.text = endScore.ToString();
                                }                                
                            mYjUpgradeEffect.CustomActive(true);

                            //发送协议
                            EquipRecoveryDataManager.GetInstance()._UpgradeEquip(guid);
                            totalCount += 1;
                        });
                        }                        
                    });
                }
            });

            //图标
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(itemData.TableID);
            if (null == ItemDetailData)
            {

            }
            else
            {
                ComItem comitem = mItemRoot.GetComponentInChildren<ComItem>();
                if (comitem == null)
                {
                    comitem = CreateComItem(mItemRoot);
                }
                int result = itemData.TableID;
                comitem.Setup(itemData, (GameObject Obj, ItemData sitem) => { _OnShowTips(itemData); });
            }
            //预计数量
            if(mExpectCount != null)
            {
            mExpectCount.text = EquipRecoveryDataManager.GetInstance()._GetEquipPrice(itemData);
            }            

            //价格
            if(mCount != null)
            {
            mCount.text = itemData.RecoScore.ToString();
            }            

            int tempMaxPrice = EquipRecoveryDataManager.GetInstance()._GetEquipPrice(itemData, false);

            //还可提升
            if(mCanUpgradeScores != null)
            {
            mCanUpgradeScores.text = (tempMaxPrice - itemData.RecoScore).ToString() ;
            }            
        }
        void _OnShowTips(ItemData result)
        {
            //ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result);
            if(result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }

        void _PlayEffect(int upgradeScore)
        {
            if(upgradeScore > 0)
            {
                if(mVictoryEffect.activeSelf)
                {
                    mVictoryEffect.CustomActive(false);
                    mVictoryEffect.CustomActive(true);
                }
                else
                {
                    mVictoryEffect.CustomActive(true);
                }
            }
            if(upgradeScore == 0)
            {
                if (mDefeatEffect.activeSelf)
                {
                    mDefeatEffect.CustomActive(false);
                    mDefeatEffect.CustomActive(true);
                }
                else
                {
                    mDefeatEffect.CustomActive(true);
                }
            }
        }

        void _StartYjUpgrade()
        {
            
        }

        void _DisplayYjUpgrade()
        {
            if (EquipRecoveryDataManager.GetInstance().isUpgradeing == true)
            {
                mContinueButton.CustomActive(false);
                if ((int)PlayerBaseData.GetInstance().Gold < consume && upgradeScore == 0)
                {
                    mContinueUIGray.enabled = true;
                    mContinueButton.enabled = false;
                    mContinueButton.CustomActive(true);
                    mContinueButtonText.text = TR.Value("equip_upgrade_nomoney");
                    mContinueButton.CustomActive(true);
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("equip_upgrade_nomoney_notice"));
                }
            }
            else
            {
                if ((int)PlayerBaseData.GetInstance().Gold < consume && upgradeScore == 0) 
                {
                    mContinueUIGray.enabled = true;
                    mContinueButton.enabled = false;
                    mContinueButton.CustomActive(true);
                    mContinueButtonText.text = TR.Value("equip_upgrade_nomoney");
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("equip_upgrade_nomoney_notice"));
                }
                ItemData itemData1 = ItemDataManager.GetInstance().GetItem(upgradingGuid);
                int maxScore1 = EquipRecoveryDataManager.GetInstance()._GetEquipPrice(itemData1, false);
                if(maxScore1 == itemData1.RecoScore)
                {
                    mContinueUIGray.enabled = true;
                    mContinueButton.enabled = false;
                    mContinueButton.CustomActive(true);
                    mContinueButtonText.text = TR.Value("equip_upgrade_highscore");
                }
            }

            upgradeScore = -1;
            //_UpdateResultList();
            //mTitleText.text = TR.Value("equip_upgrade_result_title", totalCount.ToString());

            mYjReturn.CustomActive(false);
            mImmediate.CustomActive(false);
            mYjUpgradeRoot.CustomActive(true);

            curUpgradeType = UpgradeType.stop;
            mBeginCount.text = beginScore.ToString();
            mEndCount.text = endScore.ToString();
            //endGold = _GetUpgradeGoldCount();
            endGold = (int)PlayerBaseData.GetInstance().Gold;
            mConsumeCount.text = (beginGold - endGold).ToString();
            if (beginScore != endScore)
            {
                mEndCount.color = Color.green;
            }
            else
            {
                mEndCount.color = Color.white;
            }
            StartCoroutine(OpenRewardFrame());
        }

        IEnumerator OpenRewardFrame()
        {
            mYjUpgradeEffect.CustomActive(false);
            curUpgradeType = UpgradeType.stop;
            yield return new WaitForSeconds(1f);
            _UpdateResultList();
            mFinishUIGray.enabled = false;
            mBtOk.enabled = true;
            mUpgradeResult.CustomActive(true);
            yield return null;
        }

        int _GetUpgradeGoldCount()
        {
            int goldID = 0;
            SystemValueTable goldIDData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_RECO_UPSHB);
            if (goldIDData != null)
            {
                goldID = goldIDData.Value;
            }
            var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(goldID);
            if(itemTableData != null)
            {                
                mConsumeImage.SafeSetImage(itemTableData.Icon);
            }

            return ItemDataManager.GetInstance().GetOwnedItemCount(goldID, true);
        }

        void _OnEquipUpgradeSuccess(UIEvent eventID)
        {
            upgradeScore = (int)eventID.Param1;
            upgradeResultList.Add(upgradeScore);
            if (upgradeScore > 0) upgradeResultList.Reverse();

            //ui
            endScore = endScore + upgradeScore;
            if (beginScore < endScore)
            {
                mEndCount.color = Color.green;
            }
            else
            {
                mEndCount.color = Color.white;
            }
            mEndCount.text = endScore.ToString();
            
            //if (upgradeScore > 0 && EquipRecoveryDataManager.GetInstance().isUpgradeing)
            //{
            //    curUpgradeType = UpgradeType.success;
                
            //    _DisplayYjUpgrade();
            //}
            //else if(curUpgradeType == UpgradeType.returned)
            //{
            //    _DisplayYjUpgrade();
            //}
            //else if(upgradeScore == 0 && EquipRecoveryDataManager.GetInstance().isUpgradeing)
            //{
            //    StartCoroutine(tryUpgradeAgain());
            //}
            //else if(curUpgradeType == UpgradeType.stop)
            //{
            //    StartCoroutine(OpenRewardFrame());
            //}
            //如果是一键提升
            //if(EquipRecoveryDataManager.GetInstance().isUpgradeing)
            //{
            //    if(upgradeScore > 0)
            //    {
            //        _UpdateEquipList();
            //        _PlayEffect(upgradeScore);
            //        curUpgradeType = UpgradeType.success;

            //        _DisplayYjUpgrade();
            //        //EquipRecoveryDataManager.GetInstance().isUpgradeing = false;
            //    }
            //    else 
            //    {
            //        if(curUpgradeType == UpgradeType.begining)
            //        {
            //            _PlayEffect(upgradeScore);
            //            StartCoroutine(tryUpgradeAgain());
            //        }
            //        else if (curUpgradeType == UpgradeType.immediately)
            //        {
            //            upgradeAgain();
            //            _UpdateResultList();
            //        }
            //    }
            //}
            //else
            //{
            //    if (upgradeScore > 0)
            //    {
            //        _UpdateEquipList();
            //    }
            //    _PlayEffect(upgradeScore);
            //    _DisplayYjUpgrade();
            //}
        }
        IEnumerator tryUpgradeAgain()
        {
            if ((int)PlayerBaseData.GetInstance().Gold < consume) 
            {
                curUpgradeType = UpgradeType.stop;
                _DisplayYjUpgrade();
                yield return null;
            }
            yield return new WaitForSeconds(1f);
            
            if (curUpgradeType == UpgradeType.begining && EquipRecoveryDataManager.GetInstance().isUpgradeing && mYjUpgradeRoot.activeSelf && !mUpgradeResult.activeSelf)  
            {
                EquipRecoveryDataManager.GetInstance()._UpgradeEquip(upgradingGuid);
                totalCount += 1;
            }
            if (curUpgradeType == UpgradeType.immediately && EquipRecoveryDataManager.GetInstance().isUpgradeing && mYjUpgradeRoot.activeSelf ) 
            {
                upgradeAgain();
            }
            yield return null;
        }

        /// <summary>
        /// 不做协程等待
        /// </summary>
        void upgradeAgain()
        {
            if ((int)PlayerBaseData.GetInstance().Gold < consume) 
            {
                curUpgradeType = UpgradeType.stop;
                _DisplayYjUpgrade();
                return;
            }
            if (curUpgradeType == UpgradeType.immediately && EquipRecoveryDataManager.GetInstance().isUpgradeing && mYjUpgradeRoot.activeSelf) 
            {
                EquipRecoveryDataManager.GetInstance()._UpgradeEquip(upgradingGuid);
                totalCount += 1;
            }
        }

        void _OnCountValueChange(UIEvent eventID)
        {
            
        }

        void _OnUpdateDisplayList(UIEvent eventID)
        {
            _UpdateEquipList();
        }

        void _OnEquipUpgradeFail(UIEvent eventID)
        {
            upgradeScore = -1;
            if (curUpgradeType == UpgradeType.returned)
            {
                _DisplayYjUpgrade();
            }
            else
            {
                curUpgradeType = UpgradeType.fail;
                _DisplayYjUpgrade();
            }
        }

        void _OnGoldChanged(UIEvent eventID)
        {
            endGold = (int)PlayerBaseData.GetInstance().Gold;
            mConsumeCount.text = (beginGold - endGold).ToString();

            if (upgradeScore < 0)
            {
                return;
            }
            if (EquipRecoveryDataManager.GetInstance().isUpgradeing)
            {
                if (upgradeScore > 0)
                {
                    _UpdateEquipList();
                    _PlayEffect(upgradeScore);
                    curUpgradeType = UpgradeType.success;

                    _DisplayYjUpgrade();
                    //EquipRecoveryDataManager.GetInstance().isUpgradeing = false;
                }
                else
                {
                    if (curUpgradeType == UpgradeType.begining)
                    {
                        _PlayEffect(upgradeScore);
                        StartCoroutine(tryUpgradeAgain());
                    }
                    else if (curUpgradeType == UpgradeType.immediately)
                    {
                        _UpdateResultList();
                        upgradeAgain();
                    }
                }
            }
            else
            {
                if (upgradeScore > 0)
                {
                    _UpdateEquipList();
                }
                _PlayEffect(upgradeScore);
                _DisplayYjUpgrade();
            }
        }

        #region ExtraUIBind
        private ComUIListElementScript mEquipItem = null;
        private Text mCanRedeemCount = null;
        private Button mClose = null;
        private ComUIListScript mEquipUIList = null;
        private GameObject mVictoryEffect = null;
        private GameObject mDefeatEffect = null;
        private GameObject mTips = null;
        private GameObject mYjUpgradeRoot = null;
        private Button mYjReturn = null;
        private GameObject mUpgradeResult = null;
        private Text mBeginCount = null;
        private Text mEndCount = null;
        private Image mConsumeImage = null;
        private Text mConsumeCount = null;
        private Button mBtOk = null;
        private Button mImmediate = null;
        private Button mContinueButton = null;
        private Text mTitleText = null;
        private ComUIListElementScript mResultItem = null;
        private ComUIListScript mResultUIList = null;
        private UIGray mContinueUIGray = null;
        private Text mContinueButtonText = null;
        private UIGray mFinishUIGray = null;
        private GameObject mYjUpgradeEffect = null;

        protected override void _bindExUI()
        {
            mEquipItem = mBind.GetCom<ComUIListElementScript>("equipItem");
            mCanRedeemCount = mBind.GetCom<Text>("canRedeemCount");
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mEquipUIList = mBind.GetCom<ComUIListScript>("equipUIList");
            mVictoryEffect = mBind.GetGameObject("victoryEffect");
            mDefeatEffect = mBind.GetGameObject("defeatEffect");
            mTips = mBind.GetGameObject("tips");
            mYjUpgradeRoot = mBind.GetGameObject("YjUpgradeRoot");
            mYjReturn = mBind.GetCom<Button>("YjReturn");
            mYjReturn.onClick.AddListener(_onYjReturnButtonClick);
            mUpgradeResult = mBind.GetGameObject("upgradeResult");
            mBeginCount = mBind.GetCom<Text>("beginCount");
            mEndCount = mBind.GetCom<Text>("endCount");
            mConsumeImage = mBind.GetCom<Image>("consumeImage");
            mConsumeCount = mBind.GetCom<Text>("consumeCount");
            mBtOk = mBind.GetCom<Button>("btOk");
            mBtOk.onClick.AddListener(_onBtOkButtonClick);
            mImmediate = mBind.GetCom<Button>("immediate");
            mImmediate.onClick.AddListener(_onImmediatelyButtonClick);
            mContinueButton = mBind.GetCom<Button>("continueButton");
            mContinueButton.onClick.AddListener(_onContinueButtonClick);
            mTitleText = mBind.GetCom<Text>("titleText");
            mResultItem = mBind.GetCom<ComUIListElementScript>("resultItem");
            mResultUIList = mBind.GetCom<ComUIListScript>("resultUIList");
            mContinueUIGray = mBind.GetCom<UIGray>("continueUIGray");
            mContinueButtonText = mBind.GetCom<Text>("ContinueButtonText");
            mFinishUIGray = mBind.GetCom<UIGray>("finishUIGray");
            mYjUpgradeEffect = mBind.GetGameObject("YjUpgradeEffect");
        }

        protected override void _unbindExUI()
        {
            mEquipItem = null;
            mCanRedeemCount = null;
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mEquipUIList = null;
            mVictoryEffect = null;
            mDefeatEffect = null;
            mTips = null;
            mTips = null;
            mYjUpgradeRoot = null;
            mYjReturn.onClick.RemoveListener(_onYjReturnButtonClick);
            mYjReturn = null;
            mImmediate.onClick.RemoveListener(_onImmediatelyButtonClick);
            mImmediate = null;
            mUpgradeResult = null;
            mBeginCount = null;
            mEndCount = null;
            mConsumeImage = null;
            mConsumeCount = null;
            mBtOk.onClick.RemoveListener(_onBtOkButtonClick);
            mBtOk = null;
            mContinueButton.onClick.RemoveListener(_onContinueButtonClick);
            mContinueButton = null;
            mTitleText = null;
            mResultItem = null;
            mResultUIList = null;
            mContinueUIGray = null;
            mContinueButtonText = null;
            mFinishUIGray = null;
            mYjUpgradeEffect = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        private void _onYjReturnButtonClick()
        {
            /* put your code in here */
            curUpgradeType = UpgradeType.returned;
            _DisplayYjUpgrade();
        }
        private void _onBtOkButtonClick()
        {
            /* put your code in here */
            mYjUpgradeRoot.CustomActive(false);
            mUpgradeResult.CustomActive(false);
            mYjReturn.CustomActive(false);
            beginScore = 0;
            endScore = 0;
            beginGold = 0;
            endGold = 0;
            totalCount = 0;
            upgradeResultList.Clear();
            mConsumeCount.text = "";
            curUpgradeType = UpgradeType.stop;
            EquipRecoveryDataManager.GetInstance().isUpgradeing = false;
        }

        private void _onImmediatelyButtonClick()
        {
            /* put your code in here */
            curUpgradeType = UpgradeType.immediately;
            mYjReturn.CustomActive(false);
            mImmediate.CustomActive(false);
            mUpgradeResult.CustomActive(true);
            mContinueButton.CustomActive(false);
            mFinishUIGray.enabled = true;
            mBtOk.enabled = false;
            _UpdateResultList();
        }

        private void _onContinueButtonClick()
        {
            upgradeResultList.Clear();
            EquipRecoveryDataManager.GetInstance()._UpgradeEquip(upgradingGuid);
            totalCount = 1;
            mUpgradeResult.CustomActive(false);
        }
        #endregion
    }
}