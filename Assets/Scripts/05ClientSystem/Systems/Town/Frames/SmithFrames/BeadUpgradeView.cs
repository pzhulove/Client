using Protocol;
using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class BeadUpgradeView : MonoBehaviour, ISmithShopNewView
    {
        [SerializeField] private ComCommonBind mBeadUpgradeBind;
        [SerializeField] private GameObject mgoBeadItemScrollView;

        GameObject mCanUpgradeRoot;
        GameObject mDoNotUpgradeRoot;
        GameObject mBeadItemParent;
        ComItem mBeadItemComItem;
        Text mBeadItemArrt;
        GameObject mExpendBeadItemParent;
        ComItem mExpendBeadItemComItem;
        Text mExpendBeadCount;
        Button mSelectExpendBeadBtn;
        GameObject mNextLevelItemParent;
        ComItem mNextLevelItemComItem;
        Text mNextLevelBeadArrt;
        GameObject mAppendArrtRoot;
        Button mAppendArrtBtn;
        GameObject mDoNotUpgradeItemParent;
        ComItem mDoNotUpgradeItemComItem;
        Text mDoNotUpgradeBeadArrt;
        UIGray mUpgradeBtnGray;
        Image mUpgradeBtnImage;
        Button mUpgradeBtn;
        GameObject mNoBeadRoot;
        RectTransform mNextLevelBeadItemScrollViewRect;
        ScrollRect mBeadItemScrollView;
        ScrollRect mNextLevelBeadScrollView;
        ScrollRect mDoNotUpgradeScrollView;
        Image mBeadItemTemplateBackImg;
        Image mBextBeadItemTemplateBackImg;
        BeadItemModel mCurrentSelectBeadItem;
        int mCurrentSelectExpendBeadID = 0;
        List<ExpendBeadItemData> mExpendBeadItemDatas = new List<ExpendBeadItemData>();

        private BeadItemList m_kBeadItemList = new BeadItemList();

        private void Awake()
        {
            RegisterEventHandler();
        }

        private void OnDestroy()
        {
            UnRegisterEventHandler();
        }

        public void InitView(SmithShopNewLinkData linkData)
        {
            InitInterface(linkData);
        }

        public void OnEnableView()
        {
            m_kBeadItemList.RefreshAllBeadItems();
            RefreshBeadItems();
        }

        public void OnDisableView()
        {
        }

        #region RegisterEventHandler

        private void RegisterEventHandler()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSelectExpendBeadItem, OnSelectExpendBeadItem);
        }

        private void UnRegisterEventHandler()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSelectExpendBeadItem, OnSelectExpendBeadItem);
        }

        private void OnSelectExpendBeadItem(UIEvent uiEvent)
        {
            ExpendBeadItemData data = uiEvent.Param1 as ExpendBeadItemData;

            if (data.TatleCount <= 0)
            {
                ItemComeLink.OnLink(119, 0, false);
                return;
            }

            if (mExpendBeadItemComItem == null)
            {
                mExpendBeadItemComItem = ComItemManager.Create(mExpendBeadItemParent);
            }

            var mItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.ItemID);
            if (mItemData == null)
            {
                return;
            }
            mItemData.Count = 0;
            mCurrentSelectExpendBeadID = mItemData.TableID;

            mExpendBeadItemComItem.Setup(mItemData, (GameObject obj, ItemData item) =>
            {
                OpenExpendBeadItemFrame(mCurrentSelectBeadItem);
            });

            int mExpendBeadItemTotalNum = GetBeadExpendItemNumber(mCurrentSelectBeadItem, mCurrentSelectExpendBeadID);

            if (mExpendBeadItemTotalNum <= 0)
            {
                mExpendBeadItemTotalNum = 0;
            }

            if (mExpendBeadItemTotalNum >= data.Count)
            {
                mExpendBeadCount.text = TR.Value("Bead_Expend_Green", mExpendBeadItemTotalNum, data.Count);
            }
            else
            {
                mExpendBeadCount.text = TR.Value("Bead_Expend_Red", mExpendBeadItemTotalNum, data.Count);
            }

            mExpendBeadCount.CustomActive(true);
            mSelectExpendBeadBtn.CustomActive(false);
        }

        #endregion

        #region InitInterface

        private void InitInterface(SmithShopNewLinkData linkData)
        {
            mCanUpgradeRoot = mBeadUpgradeBind.GetGameObject("CanUpgradeRoot");
            mDoNotUpgradeRoot = mBeadUpgradeBind.GetGameObject("DoNotUpgradeRoot");
            mBeadItemParent = mBeadUpgradeBind.GetGameObject("BeadItemParent");
            mBeadItemArrt = mBeadUpgradeBind.GetCom<Text>("BeadItemArrt");
            mExpendBeadItemParent = mBeadUpgradeBind.GetGameObject("ExpendBeadItemParent");
            mExpendBeadCount = mBeadUpgradeBind.GetCom<Text>("ExpendBeadCount");
            mExpendBeadCount.CustomActive(false);
            mSelectExpendBeadBtn = mBeadUpgradeBind.GetCom<Button>("SelectExpendBeadBtn");
            mSelectExpendBeadBtn.onClick.RemoveAllListeners();
            mSelectExpendBeadBtn.onClick.AddListener(() => { ItemComeLink.OnLink(119, 0, false); });
            mNextLevelItemParent = mBeadUpgradeBind.GetGameObject("NextLevelItemParent");
            mNextLevelBeadArrt = mBeadUpgradeBind.GetCom<Text>("NextLevelBeadArrt");
            mAppendArrtRoot = mBeadUpgradeBind.GetGameObject("AppendArrtRoot");
            mAppendArrtBtn = mBeadUpgradeBind.GetCom<Button>("AppendAttrBtn");
            mAppendArrtBtn.onClick.RemoveAllListeners();
            mAppendArrtBtn.onClick.AddListener(() =>
            {
                OpenRandomPropertiesFrame(mCurrentSelectBeadItem);
            });
            mDoNotUpgradeItemParent = mBeadUpgradeBind.GetGameObject("DoNotUpgradeItemParent");
            mDoNotUpgradeItemComItem = ComItemManager.Create(mDoNotUpgradeItemParent);
            mDoNotUpgradeBeadArrt = mBeadUpgradeBind.GetCom<Text>("DoNotUpgradeBeadArrt");
            mUpgradeBtnGray = mBeadUpgradeBind.GetCom<UIGray>("BtnUpgradeGray");
            mUpgradeBtnImage = mUpgradeBtnGray.GetComponent<Image>();
            mUpgradeBtn = mBeadUpgradeBind.GetCom<Button>("BtnUpgrade");
            mUpgradeBtn.onClick.RemoveAllListeners();
            mUpgradeBtn.onClick.AddListener(() =>
            {
                if (mUpgradeBtn != null)
                {
                    mUpgradeBtn.enabled = false;

                    InvokeMethod.Invoke(this, 0.50f, () => 
                    {
                        if (mUpgradeBtn != null)
                            mUpgradeBtn.enabled = true;
                    });
                }

                OnUpgradeBtnClick();
            });
            mNoBeadRoot = mBeadUpgradeBind.GetGameObject("NoBeadRoot");
            mNoBeadRoot.CustomActive(false);
            mNextLevelBeadItemScrollViewRect = mBeadUpgradeBind.GetCom<RectTransform>("NextLevelBeadItemScrollView");
            mBeadItemScrollView = mBeadUpgradeBind.GetCom<ScrollRect>("BeadItemScrollView");
            mNextLevelBeadScrollView = mBeadUpgradeBind.GetCom<ScrollRect>("NextLevelBeadScrollView");
            mDoNotUpgradeScrollView = mBeadUpgradeBind.GetCom<ScrollRect>("DoNotUpgradeScrollView");
            mBeadItemTemplateBackImg = mBeadUpgradeBind.GetCom<Image>("BeadItemTemplateBackImg");
            mBextBeadItemTemplateBackImg = mBeadUpgradeBind.GetCom<Image>("NextLevelBeadItemTemplateBackImg");

            if (!m_kBeadItemList.Initilized)
            {
                m_kBeadItemList.Initialize(mgoBeadItemScrollView, OnBeadItemSelected, linkData);
            }

            RefreshBeadItems();
        }

        private void OpenRandomPropertiesFrame(BeadItemModel model)
        {
            var beadItem = TableManager.GetInstance().GetTableItem<BeadTable>(model.beadItemData.TableID);
            if (beadItem != null)
            {
                int iNextLevelBeadId = 0;
                if (int.TryParse(beadItem.NextLevPrecbeadID, out iNextLevelBeadId))
                {
                    var mNextLevelBeadItem = TableManager.GetInstance().GetTableItem<BeadTable>(iNextLevelBeadId);
                    if (mNextLevelBeadItem != null)
                    {
                        if (mNextLevelBeadItem.BuffGroup > 0)
                        {
                            List<int> mRandomBuffIDList = new List<int>();
                            var mTableDic = TableManager.GetInstance().GetTable<BeadRandomBuff>().GetEnumerator();
                            while (mTableDic.MoveNext())
                            {
                                BeadRandomBuff item = mTableDic.Current.Value as BeadRandomBuff;
                                if (item == null)
                                {
                                    continue;
                                }

                                if (item.BuffGroup != mNextLevelBeadItem.BuffGroup)
                                {
                                    continue;
                                }

                                mRandomBuffIDList.Add(item.BuffinfoID);
                            }

                            if (mRandomBuffIDList != null)
                            {
                                ClientSystemManager.GetInstance().OpenFrame<RandomPropertiesFrame>(FrameLayer.Middle, mRandomBuffIDList);
                            }
                        }
                    }
                }
            }
        }

        private void OnUpgradeBtnClick()
        {
            if (mCurrentSelectExpendBeadID == 0)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("BeadUpgredeFrame_SelectExpand"));
                return;
            }

            int mExpendBeadItemTotalNum = GetBeadExpendItemNumber(mCurrentSelectBeadItem, mCurrentSelectExpendBeadID);
            if (mExpendBeadItemTotalNum <= 0)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("BeadUpgredeFrame_ExpandDeficiency"));
                return;
            }

            BeadCardManager.GetInstance().SendSceneUpgradePreciousbeadReq(mCurrentSelectBeadItem, mCurrentSelectExpendBeadID);
        }

        private int GetBeadExpendItemNumber(BeadItemModel model, int expendItemId)
        {
            int mExpendBeadItemTotalNum = 0;

            if (model.mountedType == (int)UpgradePrecType.Mounted)
            {
                mExpendBeadItemTotalNum = ItemDataManager.GetInstance().GetItemCountInPackage(expendItemId);
            }
            else
            {
                if (model.beadItemData.TableID == expendItemId)
                {
                    mExpendBeadItemTotalNum = ItemDataManager.GetInstance().GetItemCountInPackage(expendItemId) - 1;
                }
                else
                {
                    mExpendBeadItemTotalNum = ItemDataManager.GetInstance().GetItemCountInPackage(expendItemId);
                }
            }

            return mExpendBeadItemTotalNum;
        }

        private void OnBeadItemSelected(BeadItemModel model)
        {
            if (mCurrentSelectBeadItem == model)
            {
                return;
            }
            mCurrentSelectBeadItem = model;

            if (BeadItemIsCanUpgrade(model))
            {
                mUpgradeBtnGray.enabled = false;
                mUpgradeBtnImage.raycastTarget = true;
                mCanUpgradeRoot.CustomActive(true);
                mDoNotUpgradeRoot.CustomActive(false);
                InitCanUpgradeBeadItem(model);
                InitExpendBeadItem(model);
                InitCanUpgradeNextLevelBeadItem(model);
            }
            else
            {
                mCanUpgradeRoot.CustomActive(false);
                mDoNotUpgradeRoot.CustomActive(true);
                InitDoNotUpgradeBeadItem(model);
                mUpgradeBtnGray.enabled = true;
                mUpgradeBtnImage.raycastTarget = false;
            }
        }

        private bool BeadItemIsCanUpgrade(BeadItemModel model)
        {
            var mBeadTable = TableManager.GetInstance().GetTableItem<BeadTable>(model.beadItemData.TableID);
            if (mBeadTable != null)
            {
                int nextLevelId = 0;
                if (mBeadTable.NextLevPrecbeadID != null)
                {
                    int.TryParse(mBeadTable.NextLevPrecbeadID, out nextLevelId);
                }

                if (nextLevelId != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void InitCanUpgradeBeadItem(BeadItemModel model)
        {
            ItemData mBeadItemData = model.beadItemData;

            if (mBeadItemData == null)
            {
                return;
            }

            if (mBeadItemComItem == null)
            {
                mBeadItemComItem = ComItemManager.Create(mBeadItemParent);
            }
            mBeadItemComItem.Setup(mBeadItemData, (GameObject obj, ItemData item) =>
            {
                if (item != null)
                {
                    mBeadItemData.BeadPickNumber = model.beadPickNumber;
                    ItemTipManager.GetInstance().ShowTip(mBeadItemData);
                }
            });
            var beadItem = TableManager.GetInstance().GetTableItem<BeadTable>(model.beadItemData.TableID);
            if (beadItem != null)
            {
                mBeadItemArrt.text = string.Format("当前属性:\n{0}", BeadCardManager.GetInstance().GetAttributesDesc(beadItem.ID));
            }
            mBeadItemScrollView.verticalNormalizedPosition = 0;
            mBeadItemScrollView.verticalNormalizedPosition = 1;
            mBeadItemTemplateBackImg.raycastTarget = false;
        }

        private void InitExpendBeadItem(BeadItemModel model)
        {
            mCurrentSelectExpendBeadID = 0;

            var beadItem = TableManager.GetInstance().GetTableItem<BeadTable>(model.beadItemData.TableID);
            if (beadItem != null)
            {
                string[] mUpConsumes = beadItem.UpConsume.Split('|');
                if (mUpConsumes.Length > 1)
                {
                    mExpendBeadCount.CustomActive(false);
                    mSelectExpendBeadBtn.CustomActive(true);
                    mSelectExpendBeadBtn.onClick.RemoveAllListeners();
                    mSelectExpendBeadBtn.onClick.AddListener(() =>
                    {
                        OpenExpendBeadItemFrame(mCurrentSelectBeadItem);
                    });

                }
                else
                {
                    mSelectExpendBeadBtn.CustomActive(false);
                    mSelectExpendBeadBtn.onClick.RemoveAllListeners();
                    mExpendBeadCount.CustomActive(true);
                    string[] mStrs = mUpConsumes[0].Split('_');
                    if (mStrs != null)
                    {
                        int mExpendBeadItemId = 0;
                        int mExpendBeadItemNum = 0;
                        int.TryParse(mStrs[0], out mExpendBeadItemId);
                        int.TryParse(mStrs[1], out mExpendBeadItemNum);
                        ItemData mExpendBeadItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mExpendBeadItemId);
                        if (mExpendBeadItemData != null)
                        {
                            mExpendBeadItemData.Count = 0;
                            mCurrentSelectExpendBeadID = mExpendBeadItemData.TableID;

                            if (mExpendBeadItemComItem == null)
                            {
                                mExpendBeadItemComItem = ComItemManager.Create(mExpendBeadItemParent);
                            }

                            mExpendBeadItemComItem.Setup(mExpendBeadItemData, Utility.OnItemClicked);
                        }

                        int mExpendBeadItemTotalNum = GetBeadExpendItemNumber(mCurrentSelectBeadItem, mCurrentSelectExpendBeadID); ;

                        if (mExpendBeadItemTotalNum <= 0)
                        {
                            mExpendBeadItemTotalNum = 0;
                        }

                        if (mExpendBeadItemTotalNum >= mExpendBeadItemNum)
                        {
                            mExpendBeadCount.text = TR.Value("Bead_Expend_Green", mExpendBeadItemTotalNum, mExpendBeadItemNum);
                        }
                        else
                        {
                            mExpendBeadCount.text = TR.Value("Bead_Expend_Red", mExpendBeadItemTotalNum, mExpendBeadItemNum);
                        }
                    }
                }
            }
        }

        private void OpenExpendBeadItemFrame(BeadItemModel model)
        {
            var beadItem = TableManager.GetInstance().GetTableItem<BeadTable>(model.beadItemData.TableID);
            if (beadItem != null)
            {
                string[] mUpConsumes = beadItem.UpConsume.Split('|');
                if (mUpConsumes.Length > 1)
                {
                    mExpendBeadItemDatas.Clear();

                    for (int i = 0; i < mUpConsumes.Length; i++)
                    {
                        string[] mStrs = mUpConsumes[i].Split('_');
                        if (mStrs != null)
                        {
                            int mExpendBeadItemId = 0;
                            int mExpendBeadItemNum = 0;
                            int mExpendBeadItemTatleNum = 0;
                            int.TryParse(mStrs[0], out mExpendBeadItemId);
                            int.TryParse(mStrs[1], out mExpendBeadItemNum);
                            mExpendBeadItemTatleNum = GetBeadExpendItemNumber(mCurrentSelectBeadItem, mExpendBeadItemId);
                            ExpendBeadItemData mItemSimpleData = new ExpendBeadItemData(mExpendBeadItemId, mExpendBeadItemTatleNum, mExpendBeadItemNum);
                            mExpendBeadItemDatas.Add(mItemSimpleData);
                        }
                    }
                    mExpendBeadItemDatas.Sort();
                    ClientSystemManager.GetInstance().OpenFrame<ExpendBeadItemFrame>(FrameLayer.Middle, mExpendBeadItemDatas);
                }
            }
        }

        private void InitCanUpgradeNextLevelBeadItem(BeadItemModel model)
        {
            var beadItem = TableManager.GetInstance().GetTableItem<BeadTable>(model.beadItemData.TableID);
            if (beadItem != null)
            {
                int iNextLevelBeadId = 0;
                if (int.TryParse(beadItem.NextLevPrecbeadID, out iNextLevelBeadId))
                {
                    var mNextLevelBeadItem = TableManager.GetInstance().GetTableItem<BeadTable>(iNextLevelBeadId);
                    if (mNextLevelBeadItem != null)
                    {
                        ItemData mNextLevelBeadItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mNextLevelBeadItem.ID);
                        if (mNextLevelBeadItemData != null)
                        {
                            if (mNextLevelItemComItem == null)
                            {
                                mNextLevelItemComItem = ComItemManager.Create(mNextLevelItemParent);
                            }

                            mNextLevelItemComItem.Setup(mNextLevelBeadItemData, Utility.OnItemClicked);
                            mNextLevelBeadArrt.text = string.Format("升级属性:\n{0}", BeadCardManager.GetInstance().GetAttributesDesc(mNextLevelBeadItem.ID));
                        }

                        if (mNextLevelBeadItem.BuffGroup > 0)
                        {
                            mAppendArrtRoot.CustomActive(true);
                            mNextLevelBeadItemScrollViewRect.sizeDelta = new Vector2(397, 85);
                        }
                        else
                        {
                            mAppendArrtRoot.CustomActive(false);
                            mNextLevelBeadItemScrollViewRect.sizeDelta = new Vector2(397, 130);
                        }
                    }
                }
            }
            mNextLevelBeadScrollView.verticalNormalizedPosition = 0;
            mNextLevelBeadScrollView.verticalNormalizedPosition = 1;
            mBextBeadItemTemplateBackImg.raycastTarget = false;
        }

        private void InitDoNotUpgradeBeadItem(BeadItemModel model)
        {
            ItemData mBeadItemData = model.beadItemData;
            if (mBeadItemData == null)
            {
                return;
            }

            mDoNotUpgradeItemComItem.Setup(mBeadItemData, (GameObject obj, ItemData item) =>
            {
                if (item != null)
                {
                    mBeadItemData.BeadAdditiveAttributeBuffID = model.buffID;
                    mBeadItemData.BeadPickNumber = model.beadPickNumber;
                    ItemTipManager.GetInstance().ShowTip(mBeadItemData);
                }
            });
            var beadItem = TableManager.GetInstance().GetTableItem<BeadTable>(model.beadItemData.TableID);
            if (beadItem != null)
            {
                if (model.buffID > 0)
                {
                    mDoNotUpgradeBeadArrt.text = string.Format("当前属性:\n{0}", BeadCardManager.GetInstance().GetAttributesDesc(beadItem.ID)) + "\n" +
                       string.Format("附加属性:{0}", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(model.buffID));
                }
                else
                {
                    mDoNotUpgradeBeadArrt.text = string.Format("当前属性:\n{0}", BeadCardManager.GetInstance().GetAttributesDesc(beadItem.ID));
                }

            }
            mDoNotUpgradeScrollView.verticalNormalizedPosition = 0;
            mDoNotUpgradeScrollView.verticalNormalizedPosition = 1;
        }

        private void RefreshBeadItems()
        {
            if (m_kBeadItemList.GetBeadItemList().Count <= 0)
            {
                mNoBeadRoot.CustomActive(true);
                mAppendArrtRoot.CustomActive(false);
                mUpgradeBtnGray.enabled = true;
                mUpgradeBtnImage.raycastTarget = false;
            }
            else
            {
                mNoBeadRoot.CustomActive(false);
            }
        }

        #endregion
    }
}