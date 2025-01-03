using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class ChijiNpcData
    {
        public int npcTableId;
        public UInt64 guid;
    }

    public class ChijiNpcDialogFrame : ClientFrame
    {
        ChijiNpcData NpcData = new ChijiNpcData();
        int needHp = 0;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiNpcDialogFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                ChijiNpcData chijiNpcData = userData as ChijiNpcData;
                if (chijiNpcData != null)
                {
                    NpcData.guid = chijiNpcData.guid;
                    NpcData.npcTableId = chijiNpcData.npcTableId;
                }
            }

            _BindUIEvent();
            _InitInterface();
        }

        protected override void _OnCloseFrame()
        {
            _UnBindUIEvent();
            _ClearData();
        }

        private void _ClearData()
        {
            if (NpcData != null)
            {
                NpcData.guid = 0;
                NpcData.npcTableId = 0;
            }

            needHp = 0;
        }

        void _InitInterface()
        {
            NpcTable tableData = TableManager.GetInstance().GetTableItem<NpcTable>(NpcData.npcTableId);
            if(tableData == null)
            {
                Logger.LogErrorFormat("NpcTable tableData is null in Chiji, NpcID = {0}", NpcData.npcTableId);
                return;
            }

            if(mHeadIcon != null)
            {
                ETCImageLoader.LoadSprite(ref mHeadIcon, tableData.NpcBody);
            }

            if(mNpcName != null)
            {
                mNpcName.text = tableData.NpcName;
            }

            if(mBtCancelText != null)
            {
                if (tableData.SubType == NpcTable.eSubType.ShopNpc)
                {
                    mBtCancelText.text = "打扰了";
                }
                else
                {
                    mBtCancelText.text = "我点错了";
                }
            }

            if(mBtOKText != null)
            {
                if (tableData.SubType == NpcTable.eSubType.ShopNpc)
                {
                    mBtOKText.text = "上交装备";
                }
                else
                {
                    mBtOKText.text = "我来收拾你";
                }
            }

            List<int> rewardItemIdList = new List<int>();
            int needQualityScore = 0;
            string npcDialog = string.Empty; //npc对话

            var table = TableManager.GetInstance().GetTable<ChiJiNpcRewardTable>();
            if(table != null)
            {
                var data = table.GetEnumerator();

                while(data.MoveNext())
                {
                    var cur = data.Current.Value as ChiJiNpcRewardTable;

                    if(cur != null)
                    {
                        if(cur.npcId != NpcData.npcTableId)
                        {
                            continue;
                        }

                        if (tableData.SubType == NpcTable.eSubType.MonsterNpc)
                        {
                            needQualityScore = cur.param;
                            needHp = cur.param2;
                            npcDialog = cur.NpcDialogue;
                        }
                        
                        if(cur.rewards != null)
                        {
                            for (int i = 0; i < cur.rewards.Length; i++)
                            {
                                rewardItemIdList.Add(cur.rewards[i]);
                            }
                        }
                    }
                }
            }

            for(int i = 0; i < rewardItemIdList.Count; i++)
            {
                ItemData itemdata = ItemDataManager.CreateItemDataFromTable(rewardItemIdList[i]);

                ComItem ShowItem = CreateComItem(mContent);

                LayoutElement ele = ShowItem.gameObject.AddComponent<LayoutElement>();

                if(ele != null)
                {
                    ele.minWidth = 110;
                    ele.minHeight = 110;
                }

                if(itemdata != null && ShowItem != null)
                {
                    ShowItem.Setup(itemdata, _OnItemClicked);
                }            
            }

            if (mTalkContent != null)
            {
                if (tableData.SubType == NpcTable.eSubType.ShopNpc)
                {
                    mTalkContent.text = TR.Value("Chiji_npc_shop");
                }
                else
                {
                    int QualityScore = 0;

                    List<ulong> allEquipment = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                    for (int i = 0; i < allEquipment.Count; i++)
                    {
                        ItemData itemdata = ItemDataManager.GetInstance().GetItem(allEquipment[i]);
                        QualityScore += (int)itemdata.Quality;
                    }

                    if (QualityScore < needQualityScore)
                    {
                        mTalkContent.text = npcDialog;

                        mFirstLayer.CustomActive(false);
                        mSecondLayer.CustomActive(true);
                    }
                    else
                    {
                        mTalkContent.text = TR.Value("Chiji_npc_monster", needHp / 10);

                        mFirstLayer.CustomActive(true);
                        mSecondLayer.CustomActive(false);
                    }        
                }
            }
        }

        void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ExchangeSuccess, _OnExchangeSuccess);
        }

        void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ExchangeSuccess, _OnExchangeSuccess);
        }

        void _OnExchangeSuccess(UIEvent uiEvent)
        {
            mTalkContent.text = TR.Value("Chiji_npc_monster_fail");

            mFirstLayer.CustomActive(false);
            mSecondLayer.CustomActive(true);
        }

        void _OnItemClicked(GameObject obj, ItemData item)
        {
            if (item == null)
            {
                return;
            }

            ItemTipManager.GetInstance().ShowTip(item);
        }

        #region ExtraUIBind
        private Image mHeadIcon = null;
        private Text mNpcName = null;
        private Text mTalkContent = null;
        private GameObject mFirstLayer = null;
        private GameObject mSecondLayer = null;
        private GameObject mContent = null;
        private Text mBtCancelText = null;
        private Text mBtOKText = null;
        private Button mBtCancel = null;
        private Button mBtOK = null;
        private Button mBtReturn = null;
        private Button mBtBack = null;

        protected override void _bindExUI()
        {
            mHeadIcon = mBind.GetCom<Image>("HeadIcon");
            mNpcName = mBind.GetCom<Text>("NpcName");
            mTalkContent = mBind.GetCom<Text>("TalkContent");
            mFirstLayer = mBind.GetGameObject("FirstLayer");
            mSecondLayer = mBind.GetGameObject("SecondLayer");
            mContent = mBind.GetGameObject("Content");
            mBtCancelText = mBind.GetCom<Text>("btCancelText");
            mBtOKText = mBind.GetCom<Text>("btOKText");
            mBtCancel = mBind.GetCom<Button>("btCancel");
            if (null != mBtCancel)
            {
                mBtCancel.onClick.AddListener(_onBtCancelButtonClick);
            }
            mBtOK = mBind.GetCom<Button>("btOK");
            if (null != mBtOK)
            {
                mBtOK.onClick.AddListener(_onBtOKButtonClick);
            }
            mBtReturn = mBind.GetCom<Button>("btReturn");
            if (null != mBtReturn)
            {
                mBtReturn.onClick.AddListener(_onBtReturnButtonClick);
            }
            mBtBack = mBind.GetCom<Button>("btBack");
            if (null != mBtBack)
            {
                mBtBack.onClick.AddListener(_onBtBackButtonClick);
            }
        }

        protected override void _unbindExUI()
        {
            mHeadIcon = null;
            mNpcName = null;
            mTalkContent = null;
            mFirstLayer = null;
            mSecondLayer = null;
            mContent = null;
            mBtCancelText = null;
            mBtOKText = null;
            if (null != mBtCancel)
            {
                mBtCancel.onClick.RemoveListener(_onBtCancelButtonClick);
            }
            mBtCancel = null;
            if (null != mBtOK)
            {
                mBtOK.onClick.RemoveListener(_onBtOKButtonClick);
            }
            mBtOK = null;
            if (null != mBtReturn)
            {
                mBtReturn.onClick.RemoveListener(_onBtReturnButtonClick);
            }
            mBtReturn = null;
            if (null != mBtBack)
            {
                mBtBack.onClick.RemoveListener(_onBtBackButtonClick);
            }
            mBtBack = null;
        }
        #endregion

        #region Callback
        private void _onBtCancelButtonClick()
        {
            frameMgr.CloseFrame(this);
        }

        private void _onBtOKButtonClick()
        {
            NpcTable tableData = TableManager.GetInstance().GetTableItem<NpcTable>(NpcData.npcTableId);
            if (tableData == null)
            {
                return;
            }

            if (tableData.SubType == NpcTable.eSubType.ShopNpc)
            {
                ClientSystemManager.GetInstance().OpenFrame<ChijiHandInEquipmentFrame>(FrameLayer.Middle, NpcData);
            }
            else
            {
                if(needHp < PlayerBaseData.GetInstance().Chiji_HP_Percent)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("你当前血量不足");
                    return;
                }

                ChijiDataManager.GetInstance().SendNpcTradeReq((uint)NpcData.npcTableId, NpcData.guid);
            }
        }

        private void _onBtReturnButtonClick()
        {
            frameMgr.CloseFrame(this);
        }

        private void _onBtBackButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}