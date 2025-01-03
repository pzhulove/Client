using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using DG.Tweening;
using ProtoTable;

namespace GameClient
{
    class BudoRewardsFrameData
    {
        public List<ItemData> datas = new List<ItemData>();
        public bool bJustPreView = false;
        public string title;
        public List<ReceiveItemDataModel> ReceiveItemDataModelList;
    }

    class BudoRewardsFrame : ClientFrame
    {
        public static void Open(BudoRewardsFrameData data)
        {
            if (data != null)
            {
                ClientSystemManager.GetInstance().CloseFrame<BudoResultFrame>();
                ClientSystemManager.GetInstance().OpenFrame<BudoRewardsFrame>(FrameLayer.Middle, data);
            }
        }

        public override string GetPrefabPath()
        {
            var data = userData as BudoRewardsFrameData;
            //添加data的null判断
            if (data != null)
            {
                if (!data.bJustPreView)
                {
                    return "UIFlatten/Prefabs/Budo/BudoRewardsFrame";
                }
            }
            return "UIFlatten/Prefabs/Budo/BudoRewardsFrame_Preview";
        }

        class BudoAwardItem : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            ItemData itemData;
            public ItemData ItemData
            {
                get { return itemData; }
            }
            BudoRewardsFrame frame;
            DOTweenAnimation tween;
            Text name;
            ComItem comItem;
            ComEffect comEffect;
            bool bPreView = false;

            public override void OnDestroy()
            {
                InvokeMethod.RemoveInvokeCall(this);

                comItem.Setup(null, null);
                comItem = null;
                itemData = null;
                frame = null;
                tween = null;
                goPrefab = null;
                goParent = null;
                goLocal = null;
                name = null;
                comEffect.Stop("Light");
                comEffect = null;
            }

            public override void OnCreate(object[] param)
            {
                CreateData createData = param[0] as CreateData;
                goParent = createData.goParent;
                goPrefab = createData.goPrefab;
                itemData = createData.itemData;
                frame = createData.frame;
                bPreView = createData.bPreView;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    name = Utility.FindComponent<Text>(goLocal, "Name");
                    comItem = frame.CreateComItem(goLocal);

                    comEffect = goLocal.GetComponent<ComEffect>();
                    comEffect.Stop("Light");

                    tween = goLocal.GetComponent<DOTweenAnimation>();
                }
                Enable();
                _Update();


                if(!bPreView)
                {
                    InvokeMethod.Invoke(this, 0.20f, () =>
                    {
                        if (comEffect != null)
                        {
                            comEffect.Play("Light");
                        }
                    });
                }
            }
            public override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }

                InvokeMethod.RemoveInvokeCall(this);
            }
            public override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }
            public override void OnRefresh(object[] param)
            {
                itemData = param[0] as ItemData;
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
                InvokeMethod.RemoveInvokeCall(this);
            }
            public override bool NeedFilter(object[] param)
            {
                return false;
            }

            void OnItemClicked(GameObject obj, ItemData item)
            {
                if (item != null)
                {
                    ItemTipManager.GetInstance().ShowTip(item);
                }
            }

            void _Update()
            {
                comItem.Setup(itemData, OnItemClicked);
                name.text = itemData.GetColorName();
            }
        }

        CachedObjectDicManager<ulong, BudoAwardItem> m_akAwardItems = new CachedObjectDicManager<ulong, BudoAwardItem>();
        BudoRewardsFrameData m_kData = null;
        GameObject goComItemsParent;
        GameObject goPrefab;
        private Text receiveItemLabel;
        private Text previewTipLabel;

        protected override void _OnOpenFrame()
        {
            m_kData = userData as BudoRewardsFrameData;
            goComItemsParent = Utility.FindChild(frame, "ScrollView/ViewPort/Content");
            goPrefab = Utility.FindChild(frame, "ScrollView/ViewPort/Content/ItemParent");
            goPrefab.CustomActive(false);
            Text title = Utility.FindComponent<Text>(frame, "Up/Title");
            title.text = m_kData.title;

            receiveItemLabel = Utility.FindComponent<Text>(frame, "ReceiveItemRoot/receiveItemLabel");
            previewTipLabel = Utility.FindComponent<Text>(frame, "ReceiveItemRoot/previewTipLabel");

            _SetData();

            //大会奖励的预览界面
            if (m_kData != null && m_kData.bJustPreView == true)
            {
                UpdateReceiveItemLabel();
                UpdatePreviewTipLabel();
            }
        }

        protected override void _OnCloseFrame()
        {
            m_kData.datas.Clear();
            m_kData.title = null;
            m_kData = null;
            m_akAwardItems.DestroyAllObjects();
            goComItemsParent = null;
            goPrefab = null;
            m_kTempData.Clear();

            receiveItemLabel = null;
            previewTipLabel = null;
        }

        IEnumerator _CreateAwards()
        {
            for (int i = 0; m_kData.datas != null && i < m_kData.datas.Count; ++i)
            {
                var current = m_kData.datas[i];
                if (current == null)
                {
                    continue;
                }

                if (!m_akAwardItems.HasObject((ulong)current.TableID))
                {
                    m_kTempData.goParent = goComItemsParent;
                    m_kTempData.goPrefab = goPrefab;
                    m_kTempData.itemData = current;
                    m_kTempData.frame = this;
                    m_kTempData.bPreView = false;
                    m_akAwardItems.Create((ulong)current.TableID, new object[] { m_kTempData });
                    yield return Yielders.GetWaitForSeconds(0.35f);
                }
            }

            yield return Yielders.EndOfFrame;
        }

        public class CreateData
        {
            public GameObject goParent;
            public GameObject goPrefab;
            public ItemData itemData;
            public BudoRewardsFrame frame;
            public bool bPreView;

            public void Clear()
            {
                goParent = null;
                goPrefab = null;
                itemData = null;
                frame = null;
                bPreView = false;
            }
        }
        CreateData m_kTempData = new CreateData();

        void _SetData()
        {
            if (m_kData != null)
            {
                if (!m_kData.bJustPreView)
                {
                    StartCoroutine(_CreateAwards());
                }
                else
                {
                    for (int i = 0; m_kData.datas != null && i < m_kData.datas.Count; ++i)
                    {
                        var current = m_kData.datas[i];
                        if (current == null)
                        {
                            continue;
                        }

                        if (!m_akAwardItems.HasObject((ulong)current.TableID))
                        {
                            m_kTempData.goParent = goComItemsParent;
                            m_kTempData.goPrefab = goPrefab;
                            m_kTempData.itemData = current;
                            m_kTempData.frame = this;
                            m_kTempData.bPreView = true;
                            m_akAwardItems.Create((ulong)current.TableID,new object[] { m_kTempData });
                        }
                    }
                }
            }
        }

        [UIEventHandle("Close")]
        void OnClickClose()
        {
            _BudoReturn();
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Btn")]
        void OnClickClose2()
        {
            _BudoReturn();
            frameMgr.CloseFrame(this);
        }

        void _BudoReturn()
        {
            if(m_kData.bJustPreView)
            {
                return;
            }

            //Logger.LogErrorFormat("m_kData.bJustPreView = {0}", m_kData.bJustPreView);
            BudoManager.GetInstance().SendReturnToTownRelation();

            ClientSystemManager.GetInstance().CloseFrame<BudoArenaFrame>();
        }

        

        //额外获得的展示
        private void UpdateReceiveItemLabel()
        {
            if (receiveItemLabel == null)
                return;

            var receiveItemDataModelList = m_kData.ReceiveItemDataModelList;
            if (receiveItemDataModelList == null
                || receiveItemDataModelList.Count <= 0)
                return;

            var receiveItemStr = "";
            for (var i = 0; i < receiveItemDataModelList.Count; i++)
            {
                var curReceiveItem = receiveItemDataModelList[i];
                if(curReceiveItem == null)
                    continue;

                if (curReceiveItem.MinNumber <= 0 || curReceiveItem.MaxNumber <= 0)
                    return;

                var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(curReceiveItem.ItemId);
                if(itemTable == null)
                    continue;

                //准备添加下一个的时候，添加一个顿号。（除第一个之外)
                if (i != 0)
                    receiveItemStr += TR.Value("Common_Format_Split_Flag");

                var itemName = CommonUtility.GetItemColorName(itemTable);
                var curItemStr = "";
                if (curReceiveItem.MinNumber == curReceiveItem.MaxNumber)
                {
                    curItemStr = TR.Value("Budo_Reward_Receive_One_Item_Format",
                        curReceiveItem.MinNumber,
                        itemName);
                }
                else
                {
                    curItemStr = TR.Value("Budo_Reward_Receive_Two_Item_Format",
                        curReceiveItem.MinNumber,
                        curReceiveItem.MaxNumber,
                        itemName);
                }
                receiveItemStr += curItemStr;
            }

            var finalReceiveItemStr = TR.Value("Budo_Reward_Must_Received_Format",
                receiveItemStr);

            receiveItemLabel.text = finalReceiveItemStr;

        }

        //奖励预览说明
        private void UpdatePreviewTipLabel()
        {
            if (previewTipLabel == null)
                return;

            previewTipLabel.text = TR.Value("Budo_Reward_Preview_TipLabel");
        }

    }
}