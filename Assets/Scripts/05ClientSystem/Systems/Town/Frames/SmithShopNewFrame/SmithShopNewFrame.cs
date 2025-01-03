using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public interface ISmithShopNewView
    {
        void InitView(SmithShopNewLinkData linkData);
        void OnEnableView();
        void OnDisableView();
    }
    
    public enum SmithShopNewOpenType
    {
        Material = 0,
        Special,
    }

    public class SmithShopNewLinkData
    {
        public ItemData itemData;
        public int iDefaultFirstTabId = 0;
        public int iDefaultSecondTabId = 0;
        public int iSmithShopNewOpenTypeId = 0;
    }

    public class EnchantmentsFunctionData
    {
        public ItemData leftItem;
        public ItemData rightItem;
    }

    public class SmithShopNewFrame : ClientFrame
    {
        public static SmithShopNewLinkData linkData;

        #region ExtraUIBind
        private Button mClose = null;
        private SmithShopNewFrameView mSmithShopNewFrameView = null;

        protected sealed override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mSmithShopNewFrameView = mBind.GetCom<SmithShopNewFrameView>("SmithShopNewFrameView");
        }

        protected sealed override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mSmithShopNewFrameView = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion

        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                SmithShopNewLinkData data = new SmithShopNewLinkData();
                string[] sLinks = strParam.Split('|');
                if (sLinks.Length == 1)
                {
                    data.itemData = null;
                    int.TryParse(sLinks[0], out data.iDefaultFirstTabId);
                }
                else if (sLinks.Length == 2)
                {
                    data.itemData = null;
                    int.TryParse(sLinks[0], out data.iDefaultFirstTabId);
                    int.TryParse(sLinks[1], out data.iDefaultSecondTabId);
                }
                else if (sLinks.Length == 3)
                {
                    data.itemData = null;
                    int.TryParse(sLinks[0], out data.iDefaultFirstTabId);
                    int.TryParse(sLinks[1], out data.iDefaultSecondTabId);
                    int.TryParse(sLinks[2], out data.iSmithShopNewOpenTypeId);
                }

                if (ClientSystemManager.GetInstance().IsFrameOpen<SmithShopNewFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<SmithShopNewFrame>();
                }

                ClientSystemManager.GetInstance().OpenFrame<SmithShopNewFrame>(FrameLayer.Middle, data);
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShopNewFrame/SmithShopNewFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            RegisterUIEventHandle();

            if (userData != null)
            {
                linkData = userData as SmithShopNewLinkData;
            }
            
            if (mSmithShopNewFrameView != null)
            {
                mSmithShopNewFrameView.InitView(linkData);
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            UnRegisterUIEventHandle();
            linkData = null;

            CloseAllFrame();
        }

        private void CloseAllFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<StrengthenConfirm>())
            {
                ClientSystemManager.GetInstance().CloseFrame<StrengthenConfirm>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<StrengthenSpecialConfirmFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<StrengthenSpecialConfirmFrame>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<CommonMsgBoxOKCancel>())
            {
                ClientSystemManager.GetInstance().CloseFrame<CommonMsgBoxOKCancel>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<AdjustResultFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<AdjustResultFrame>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<CommonMsgBoxOkCancelNewFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<CommonMsgBoxOkCancelNewFrame>();
            }

            CommonNewMessageBoxView._CloseFrame();

            if (ClientSystemManager.GetInstance().IsFrameOpen<MagicCardOneKeyMergeTipFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MagicCardOneKeyMergeTipFrame>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<AuctionNewMsgBoxOkCancelFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<AuctionNewMsgBoxOkCancelFrame>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<CommonPurChaseHintFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<CommonPurChaseHintFrame>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<InscriptionOperationFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<InscriptionOperationFrame>();
            }
        }

        private void RegisterUIEventHandle()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationCloseRelationFrame, OnReceiveTeamDuplicationCloseRelationFrame);
        }

        private void UnRegisterUIEventHandle()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationCloseRelationFrame, OnReceiveTeamDuplicationCloseRelationFrame);
        }

        private void OnReceiveTeamDuplicationCloseRelationFrame(UIEvent uiEvent)
        {
            EquipGrowthDataManager.MandatoryCloseSmithshopNewFrame();
        }
    }

    public sealed class CostMaterialItem : CachedObject
    {
        GameObject goLocal;
        GameObject goPrefab;
        GameObject goParent;
        GameObject goAcquired;
        Button btnAcquired;
        int iNeedCount;
        bool bForceShow;
        ItemData itemData;
        public ItemData ItemData
        {
            get { return itemData; }
        }
        ClientFrame frame;

        Text name;
        Text count;
        ComItemNew comItem;

        public override void OnDestroy()
        {
            comItem.Setup(null, null);
            comItem = null;
            itemData = null;
        }

        public override void OnCreate(object[] param)
        {
            goParent = param[0] as GameObject;
            goPrefab = param[1] as GameObject;
            itemData = param[2] as ItemData;
            frame = param[3] as ClientFrame;
            bForceShow = (bool)param[4];
            iNeedCount = (int)param[5];
            if (goPrefab == null) return;
            if (goLocal == null)
            {
                goLocal = GameObject.Instantiate(goPrefab);
                Utility.AttachTo(goLocal, goParent);

                name = Utility.FindComponent<Text>(goLocal, "Name");
                comItem = frame.CreateComItemNew(goLocal);
                comItem.gameObject.transform.SetAsFirstSibling();
                comItem.CustomActive(true);
                count = Utility.FindComponent<Text>(goLocal, "Count");
                goAcquired = Utility.FindChild(goLocal, "ItemComLink");
                btnAcquired = Utility.FindComponent<Button>(goLocal, "ItemComLink");
                btnAcquired.onClick.RemoveAllListeners();
                btnAcquired.onClick.AddListener(() =>
                {
                    if (null != itemData)
                    {
                        ItemComeLink.OnLink(itemData.TableID, 0);
                    }
                });
            }
            Enable();
            SetAsLastSibling();
            _Update();
        }
        public override void SetAsLastSibling()
        {
            if (goLocal != null)
            {
                goLocal.transform.SetAsLastSibling();
            }
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
            itemData = param[0] as ItemData;
            bForceShow = (bool)param[1];
            iNeedCount = (int)param[2];
            _Update();
        }
        public override void Enable()
        {
            if (goLocal != null)
            {
                goLocal.CustomActive(true);
            }
        }

        public void SetAsFirstSibling()
        {
            if (goLocal != null)
            {
                goLocal.transform.SetAsFirstSibling();
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
            if (bForceShow)
            {
                return false;
            }

            return iNeedCount <= 0;
        }

        void OnItemClicked(GameObject obj, IItemDataModel item)
        {
            if (item != null)
            {
                ItemTipManager.GetInstance().ShowTip(item as ItemData);
            }
        }

        void _Update()
        {
            comItem.Setup(itemData, OnItemClicked);
            name.text = itemData.GetColorName();
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)itemData.TableID);
            if (itemData.Type == ProtoTable.ItemTable.eType.INCOME)
            {
                count.text = string.Format("{0}", iNeedCount);
            }
            else
            {
                count.text = string.Format("{0}/{1}", iHasCount, iNeedCount);
            }

            if (iHasCount < iNeedCount && iNeedCount > 0)
            {
                count.color = Color.red;
            }
            else
            {
                count.color = Color.white;
            }
            if (itemData != null)
            {
                itemData.Count = 1;
            }
            goLocal.name = itemData.TableID.ToString();

            goAcquired.CustomActive(iHasCount < iNeedCount && iNeedCount > 0);

            comItem.SetShowNotEnoughState(goAcquired.activeSelf);
        }
    }
}