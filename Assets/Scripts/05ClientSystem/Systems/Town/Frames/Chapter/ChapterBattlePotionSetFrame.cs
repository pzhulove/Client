using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using Network;
using ProtoTable;
using System;

namespace GameClient
{
    public class ChapterBattlePotionSetFrame : ClientFrame 
    {
        private const int kMaxBattlePotionSetCount = 3;
        private const int kMaxBattlePotionCount = 3 * 7;

        private const ulong kDefaultBattlePotionId = ulong.MaxValue;
        
        private List<ChapterBattlePotionSetFrameData> mAllBattlePostions = new List<ChapterBattlePotionSetFrameData>();
        private bool mIsAllBattlePostionDirty = false;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chapter/Normal/ChapterBattlePotionSetFrame";
        }

        private void _bindEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemTakeSuccess, _onUpdateAllBattlePostion);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemCountChanged, _onUpdateAllBattlePostion);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStoreSuccess, _onUpdateAllBattlePostion);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess, _onUpdateAllBattlePostion);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemSellSuccess, _onUpdateAllBattlePostion);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyGet, _onUpdateAllBattlePostion);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyRemoved, _onUpdateAllBattlePostion);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonPotionSetChanged, _onUpdateBattlePostionSet);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, _onUpdateLevelLimitStatus);
        }


        private void _unbindEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemTakeSuccess, _onUpdateAllBattlePostion);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemCountChanged, _onUpdateAllBattlePostion);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStoreSuccess, _onUpdateAllBattlePostion);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess, _onUpdateAllBattlePostion);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemSellSuccess, _onUpdateAllBattlePostion);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyGet, _onUpdateAllBattlePostion);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyRemoved, _onUpdateAllBattlePostion);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonPotionSetChanged, _onUpdateBattlePostionSet);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, _onUpdateLevelLimitStatus);
        }

        private void _onUpdateAllBattlePostion(UIEvent ui)
        {
            mIsAllBattlePostionDirty = true;
        }


        private void _bindDragDropEvent()
        {
            for (int i = 0; i < mDropItems.Length; ++i)
            {
                mDropItems[i].OnDropEvent.AddListener(_onDropComDrop);
                mDropItems[i].OnEnterEvent.AddListener(_onDropComEnter);

                mDragItems[i].OnEndDragEvent.AddListener(_onDragMainComEnd);
                mDragItems[i].OnBeginDragEvent.AddListener(_onDragMainComBegin);
            }

            mCancelDrop.OnDropEvent.AddListener(_onDropComCancel);
        }

        private void _unbindDragDropEvent()
        {
            for (int i = 0; i < mDropItems.Length; ++i)
            {
                mDropItems[i].OnDropEvent.RemoveListener(_onDropComDrop);
                mDropItems[i].OnEnterEvent.RemoveListener(_onDropComEnter);

                mDragItems[i].OnEndDragEvent.RemoveListener(_onDragMainComEnd);
                mDragItems[i].OnBeginDragEvent.RemoveListener(_onDragMainComBegin);
            }

            mCancelDrop.OnDropEvent.RemoveListener(_onDropComCancel);
        }

        
        private void _onDropComEnter(ComDrop drop, ComDrag drag)
        {
            ChapterBattlePotionSetFrameData data = _findDataByGameObject(drag.gameObject);
            int idx = _getDropComIdx(drop);
        }

        string GetSlotNotityText(PlayerBaseData.PotionSlotType potionSlotType)
        {
            if(potionSlotType == PlayerBaseData.PotionSlotType.SlotMain || potionSlotType == PlayerBaseData.PotionSlotType.SlotExtend1)
            {
                return TR.Value("hp_slot_tips");
            }
            if(potionSlotType == PlayerBaseData.PotionSlotType.SlotExtend2)
            {
                return TR.Value("mp_slot_tips");
            }
            return "";
        }
        //判断药水是否能配置在该位置
        bool SlotCanSetPotion(PlayerBaseData.PotionSlotType potionSlotType, uint id)
        {
            if(id == 0)
            {
                return true;
            }
            ProtoTable.ItemTable itemTable = TableManager.instance.GetTableItem<ProtoTable.ItemTable>((int)id);
            if(itemTable == null)
            {
                return false;
            }
            ItemTable.eSubType subType = itemTable.SubType;
            if (subType == ItemTable.eSubType.HpMp || subType == ItemTable.eSubType.AttributeDrug)
            {
                return true;
            }
            if (subType == ItemTable.eSubType.Hp && (potionSlotType == PlayerBaseData.PotionSlotType.SlotMain || potionSlotType == PlayerBaseData.PotionSlotType.SlotExtend1))
            {
                return true;
            }
            if(subType == ItemTable.eSubType.Mp && potionSlotType == PlayerBaseData.PotionSlotType.SlotExtend2)
            {
                return true;
            }
            return false;
        }
        private void _onDropComDrop(ComDrop drop, ComDrag drag)
        {
            if (null != drag.DragObject)
            {
                drag.DragObject.SetActive(false);
            }

            int fstIdx = _getDropComIdx(drop);
            int sndIdx = _getDropComIdx(drag);

            ChapterBattlePotionSetFrameData data = _findDataByGameObject(drag.gameObject);

            if (data == null) // 交换
            {
                _onDropComDropSwapCB(fstIdx, sndIdx);
            }
            else // 设置
            {
                uint id = _convertItemGUID2TalbeID(data.id);
                sndIdx = ChapterBattlePotionSetUtiilty._getItemIdx(id);
                if(sndIdx >= 0)
                {
                    _onDropComDropSwapCB(fstIdx, sndIdx);
                }
                else
                {
                _onDropComDropCB(fstIdx, data);
                }                
            }
        }

        private void _onDragMainComBegin(ComDrag drag)
        {
            if (null == drag)
            {
                return ;
            }

            if (null == drag.DragObject)
            {
                return ;
            }

            drag.DragObject.SetActive(true);
        }

        private void _onDragMainComEnd(ComDrag drag)
        {
            if (null == drag)
            {
                return ;
            }

            if (null == drag.DragObject)
            {
                return ;
            }

            drag.DragObject.SetActive(false);
        }
        
        private void _onDropComCancel(ComDrop drop, ComDrag drag)
        {
            int cancelIdx = _getDropComIdx(drag);

            if (cancelIdx < 0)
            {
                Logger.LogProcessFormat("[PotionSet] 不是取消配置类型");
                return ;
            }

            _onDropComCancelCB(cancelIdx);
        }

        private int _getDropComIdx(ComDrop drop)
        {
            for (int i = 0; i < mDropItems.Length; ++i)
            {
                if (mDropItems[i] == drop)
                {
                    return i;
                }
            }

            return -1;
        }

        private int _getDropComIdx(ComDrag drag)
        {
            for (int i = 0; i < mDropItems.Length; ++i)
            {
                if (mDragItems[i] == drag)
                {
                    return i;
                }
            }

            return -1;
        }

        private ChapterBattlePotionSetFrameData _findDataByGameObject(GameObject root)
        {
            for (int i = 0; i < mAllBattlePostions.Count; ++i)
            {
                if (null != mAllBattlePostions[i].bind && mAllBattlePostions[i].bind.gameObject == root)
                {
                    return mAllBattlePostions[i];
                }
            }

            return null;
        }

        private void _bindNetMessage()
        {
            NetProcess.AddMsgHandler(SceneSetDungeonPotionRes.MsgID, _onSceneSetDungeonPotionRes);
        }

        private void _unBindNetMessage()
        {
            NetProcess.RemoveMsgHandler(SceneSetDungeonPotionRes.MsgID, _onSceneSetDungeonPotionRes);
        }

        private void _onSceneSetDungeonPotionRes(MsgDATA data)
        {
            SceneSetDungeonPotionRes res = new SceneSetDungeonPotionRes();
            res.decode(data.bytes);

            Logger.LogProcessFormat("[PotionSet] 收到消息 SceneSetDungeonPotionRes {0}", res.code);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

#region ExtraUIBind
        private Button mClose = null;
        private GameObject mDrugRoot = null;
        private ComDrop[] mDropItems = new ComDrop[kMaxBattlePotionSetCount];
        private Image[] mConfigItems = new Image[kMaxBattlePotionSetCount];
        private StateController[] mItemsCtrl = new StateController[kMaxBattlePotionSetCount];
        private ScrollRect mScroll = null;
        private ComDrag[] mDragItems = new ComDrag[kMaxBattlePotionSetCount];
        private Image[] mDragFgItems = new Image[kMaxBattlePotionSetCount];
        private Text[] mItemsCount = new Text[kMaxBattlePotionSetCount];
        private Button mBtnSet0;
        private Button mBtnSet1;
        private Button mBtnSet2;
        private Image mImgSelect0;
        private Image mImgSelect1;
        private Image mImgSelect2;
        private ComDrop mCancelDrop = null;
        private ToggleGroup mToggleGroup = null;
        private GameObject mObjPotionList = null;
        private Button mBtnSure = null;
        private Button mBtnCloseList = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mDrugRoot = mBind.GetGameObject("drugRoot");
            mCancelDrop = mBind.GetCom<ComDrop>("cancelDrop");
            mToggleGroup = mBind.GetCom<ToggleGroup>("toggleGroup");
            mScroll = mBind.GetCom<ScrollRect>("scroll");
            mObjPotionList = mBind.GetGameObject("drugs");
            mBtnSure = mBind.GetCom<Button>("sure");
            mBtnSure.onClick.AddListener(_SetPotion);
            mBtnCloseList = mBind.GetCom<Button>("closeList");
            mBtnCloseList.onClick.AddListener(_HidePotionList);
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mBtnSure.onClick.RemoveListener(_SetPotion);
            mBtnSure = null;
            mBtnCloseList.onClick.RemoveListener(_HidePotionList);
            mBtnCloseList = null;
            mDrugRoot = null;
            mCancelDrop     = null;
            mToggleGroup    = null;
            mScroll = null;
        }
        protected void _bindDragDropUI(ComCommonBind bind)
        {          
            mDropItems[0] = bind.GetCom<ComDrop>("dropItem0");
            mDropItems[1] = bind.GetCom<ComDrop>("dropItem1");
            mDropItems[2] = bind.GetCom<ComDrop>("dropItem2");
            mConfigItems[0] = bind.GetCom<Image>("configItem0");
            mConfigItems[1] = bind.GetCom<Image>("configItem1");
            mConfigItems[2] = bind.GetCom<Image>("configItem2");   
            mItemsCtrl[0] = bind.GetCom<StateController>("set0stateCtrl");
            mItemsCtrl[1] = bind.GetCom<StateController>("set1stateCtrl");
            mItemsCtrl[2] = bind.GetCom<StateController>("set2stateCtrl");   
                     
            mDragFgItems[0] = bind.GetCom<Image>("dragFgItem0");
            mDragFgItems[1] = bind.GetCom<Image>("dragFgItem1");
            mDragFgItems[2] = bind.GetCom<Image>("dragFgItem2");
            mItemsCount[0] = bind.GetCom<Text>("Item0Count");
            mItemsCount[1] = bind.GetCom<Text>("Item1Count");
            mItemsCount[2] = bind.GetCom<Text>("Item2Count");
            
            mDragItems[0] = bind.GetCom<ComDrag>("dragItem0");
            mDragItems[1] = bind.GetCom<ComDrag>("dragItem1");
            mDragItems[2] = bind.GetCom<ComDrag>("dragItem2");
            mBtnSet0 = bind.GetCom<ButtonEx>("BtnSet0");
            mBtnSet1 = bind.GetCom<ButtonEx>("BtnSet1");
            mBtnSet2 = bind.GetCom<ButtonEx>("BtnSet2");
            mImgSelect0 = bind.GetCom<Image>("ImgSelect0");
            mImgSelect1 = bind.GetCom<Image>("ImgSelect1");
            mImgSelect2 = bind.GetCom<Image>("ImgSelect2");
            mBtnSet0.onClick.AddListener(_OnClickSet0);
            mBtnSet1.onClick.AddListener(_OnClickSet1);
            mBtnSet2.onClick.AddListener(_OnClickSet2);
        }

        protected void _unbindDragDropUI()
        {           
            mDropItems[0] = null;
            mDropItems[1] = null;
            mDropItems[2] = null;
            mConfigItems[0] = null;
            mConfigItems[1] = null;
            mConfigItems[2] = null;
            mItemsCtrl[0] = null;
            mItemsCtrl[1] = null;
            mItemsCtrl[2] = null;   
            mDragFgItems[0] = null;
            mDragFgItems[1] = null;
            mDragFgItems[2] = null;
            mItemsCount[0] = null;
            mItemsCount[1] = null;
            mItemsCount[2] = null;
            mDragItems[0]   = null;
            mDragItems[1]   = null;
            mDragItems[2]   = null;
            mBtnSet0.onClick.RemoveAllListeners();
            mBtnSet1.onClick.RemoveAllListeners();
            mBtnSet2.onClick.RemoveAllListeners();
            mBtnSet0        = null;
            mBtnSet1        = null;
            mBtnSet2        = null;
            mObjPotionList  = null;
        }
#endregion   

#region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            _close();
        }
#endregion


        private void _close()
        {
            ClientSystemManager.instance.CloseFrame(this);
        }

        private void _onUpdateBattlePostionSet(UIEvent ui)
        {
            Logger.LogProcessFormat("[PotionSet] postion set updated");
            _updateBattlePostionSet();
        }

        private void _onUpdateLevelLimitStatus(UIEvent ui)
        {
            for (int i = 0; i < mAllBattlePostions.Count; ++i)
            {
                _updateLevelLimitStatus(mAllBattlePostions[i]);
            }
        }

        private void _onDropComDropCB(int idx, ChapterBattlePotionSetFrameData data)
        {
            Logger.LogProcessFormat("[PotionSet] drop idx:{0}, id {1}", idx, data.id);

            uint id = _convertItemGUID2TalbeID(data.id);

            if (!SlotCanSetPotion((PlayerBaseData.PotionSlotType)idx, id))
            {
                SystemNotifyManager.SysNotifyFloatingEffect(GetSlotNotityText((PlayerBaseData.PotionSlotType)idx));
                return;
            }
            ChapterBattlePotionSetUtiilty.Save(idx, id);

            _updateBattlePostionSetByIdx(idx, id);
        }

        private void _onDropComDropSwapCB(int fstIdx, int sndIdx)
        {
            Logger.LogProcessFormat("[PotionSet] swap idx:{0}, sndIdx {1}", fstIdx, sndIdx);
            if (fstIdx < PlayerBaseData.GetInstance().potionSets.Count && sndIdx < PlayerBaseData.GetInstance().potionSets.Count)
            {
                uint fstPotion = PlayerBaseData.GetInstance().potionSets[fstIdx];
                uint sndPotion = PlayerBaseData.GetInstance().potionSets[sndIdx];
                // fst不能放到snd上去 直接弹错误提示
                if (!SlotCanSetPotion((PlayerBaseData.PotionSlotType)fstIdx, sndPotion))
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(GetSlotNotityText((PlayerBaseData.PotionSlotType)fstIdx));
                    return;
                }
                // snd不能放到fst上去 则将先将fst放到snd上，然后将snd置空           
                if (!SlotCanSetPotion((PlayerBaseData.PotionSlotType)sndIdx, fstPotion))
                {
                    ChapterBattlePotionSetUtiilty.Save(fstIdx, sndPotion);
                    ChapterBattlePotionSetUtiilty.Cancel(sndIdx);
                    return;
                }
                // 走到这里说明fst和snd能交换，走正常的交换流程
            }

            if (ChapterBattlePotionSetUtiilty.Swap(fstIdx, sndIdx))
            {
                uint fstPotion = PlayerBaseData.GetInstance().potionSets[fstIdx];
                uint sndPotion = PlayerBaseData.GetInstance().potionSets[sndIdx];

                _updateBattlePostionSetByIdx(fstIdx, fstPotion);
                _updateBattlePostionSetByIdx(sndIdx, sndPotion);
            }
        }

        private void _onDropComCancelCB(int cancelIdx)
        {
            Logger.LogProcessFormat("[PotionSet] cancel idx:{0}", cancelIdx);

            ChapterBattlePotionSetUtiilty.Cancel(cancelIdx);
        }

        private uint _convertItemGUID2TalbeID(ulong id)
        {
            ItemData itemData = ItemDataManager.GetInstance().GetItem(id);

            if (null == itemData)
            {
                return 0;
            }

            return (uint)itemData.TableID;
        }

        protected override void _OnOpenFrame()
        {
            string unitPath = mBind.GetPrefabPath("potionSet");
            mBind.ClearCacheBinds(unitPath);
            ComCommonBind bind = mBind.LoadExtraBind(unitPath);
            Utility.AttachTo(bind.gameObject, mBind.GetGameObject("setParent"));
            _bindDragDropUI(bind);
            _bindNetMessage();
            _bindEvents();
            _bindDragDropEvent();

            _initAllBattlePostionData();

            _updateBattlePostionSet();
            _updateAllBattlePostions();
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ClosePotionSetFrame);
            _clearAllBattlePostionData();

            _unBindNetMessage();
            _unbindDragDropEvent();
            _unbindEvents();
            _unbindDragDropUI();
        }

        protected override void _OnUpdate(float delta)
        {
            if (mIsAllBattlePostionDirty)
            {
                _updateAllBattlePostions();
                mIsAllBattlePostionDirty = false;
            }
        }

        private void _updateBattlePostionSet()
        {
            List<uint> potionSets = PlayerBaseData.GetInstance().potionSets;
            for (int i = 0; i < kMaxBattlePotionSetCount; ++i)
            {
                uint id = 0;

                if (i < potionSets.Count)
                {
                    id = potionSets[i];
                }

                _updateBattlePostionSetByIdx(i, id);
            }
        }

        //设置i位置上的药水为id
        private void _updateBattlePostionSetByIdx(int i, uint id)
        {
            Logger.LogProcessFormat("[PotionSet] 配置 {0}, {1}", i, id);

            ProtoTable.ItemTable itemTable = TableManager.instance.GetTableItem<ProtoTable.ItemTable>((int)id);
            if (null == itemTable)
            {
                mItemsCtrl[i].Key = "noset";
                mConfigItems[i].color = Color.clear;
                mDragFgItems[i].color = Color.clear;
                mItemsCount[i].SafeSetText("");
            }
            else
            {
                mItemsCtrl[i].Key = "set";
                mConfigItems[i].color = Color.white;
                // mConfigItems[i].sprite = AssetLoader.instance.LoadRes(itemTable.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref mConfigItems[i], itemTable.Icon);

                mDragFgItems[i].color = mConfigItems[i].color;
                mDragFgItems[i].sprite = mConfigItems[i].sprite;
                mDragFgItems[i].material = mConfigItems[i].material;
                mItemsCount[i].SafeSetText(ItemDataManager.GetInstance().GetOwnedItemCount((int)id).ToString());
            }

            _updateBattlePostionStatus();
        }

        private void _updateBattlePostionStatus()
        {
            for (int i = 0; i < mAllBattlePostions.Count; ++i)
            {
                ChapterBattlePotionSetFrameData data = mAllBattlePostions[i];
                ComCommonBind bind = data.bind;
                if (null == bind)
                {
                    continue;
                }

                CanvasGroup select = bind.GetCom<CanvasGroup>("select");

                ItemData itemdata  = ItemDataManager.GetInstance().GetItem(data.id);

                if (null == itemdata)
                {
                    select.alpha = 0.0f;
                }
                else
                {
                    select.alpha = _isAlreadyInPotionSet(itemdata.TableID) ? 1.0f : 0.0f;
                }
            }
        }


        private ChapterBattlePotionSetFrameData _findDataById(ulong id)
        {
            for (int i = 0; i < mAllBattlePostions.Count; ++i)
            {
                if (mAllBattlePostions[i].id == id)
                {
                    return mAllBattlePostions[i];
                }
            }

            return null;
        }

        private void _addDataById(ulong id)
        {
            Logger.LogProcessFormat("[PotionSet] add battle drug data {0}", id);
            ChapterBattlePotionSetFrameData data = _findDataById(id);

            if (null == data)
            {
                data    = _findDataById(kDefaultBattlePotionId);

                if (null != data)
                {
                    data.id = id;
                }
            }
        }

        private void _removeDataById(ulong id)
        {
           ChapterBattlePotionSetFrameData data = _findDataById(id);

           if (null == data)
           {
               return ;
           }

           data.id = kDefaultBattlePotionId;
        }

        private void _clearAllBattlePostionData()
        {
            Logger.LogProcessFormat("[PotionSet] clear all cache data");

            for (int i = 0; i < mAllBattlePostions.Count; ++i)
            {
                ComCommonBind bind = mAllBattlePostions[i].bind;

                ComDrag dragCom = bind.GetCom<ComDrag>("dragCom");

                dragCom.OnBeginDragEvent.RemoveAllListeners();
                dragCom.OnEndDragEvent.RemoveAllListeners();
                
                Toggle btnSelect = bind.GetCom<Toggle>("toggle");
                btnSelect.onValueChanged.RemoveAllListeners();
                Button btnMask = bind.GetCom<Button>("Mask");
                btnMask.onClick.RemoveAllListeners();
            }

            mAllBattlePostions.Clear();
        }

        private void _initAllBattlePostionData()
        {
            string path = mBind.GetPrefabPath("unit");
            mBind.ClearCacheBinds(path);

            for (int i = 0; i < kMaxBattlePotionCount; ++i)
            {
                _loadOneUnit();
            }

            mScroll.verticalNormalizedPosition = 1.0f;

            LayoutRebuilder.ForceRebuildLayoutImmediate(mScroll.content);
        }

        private void _loadOneUnit()
        {
            string path = mBind.GetPrefabPath("unit");
            Logger.LogProcessFormat("[PotionSet] load with path {0}", path);

            ComCommonBind bind = mBind.LoadExtraBind(path);
            ComDrag dragCom = bind.GetCom<ComDrag>("dragCom");
            Toggle toggle = bind.GetCom<Toggle>("toggle");

            ChapterBattlePotionSetFrameData data = new ChapterBattlePotionSetFrameData(bind, kDefaultBattlePotionId);

            dragCom.OnBeginDragEvent.RemoveAllListeners();
            dragCom.OnBeginDragEvent.AddListener(_onDragComBeginDrag);

            dragCom.OnEndDragEvent.RemoveAllListeners();
            dragCom.OnEndDragEvent.AddListener(_onDragComEndDrag);

            toggle.group = mToggleGroup;

            Utility.AttachTo(bind.gameObject, mDrugRoot);

            mAllBattlePostions.Add(data);
        }

        private void _onDragComBeginDrag(ComDrag drag)
        {
            ChapterBattlePotionSetFrameData data = _findDataByGameObject(drag.gameObject);
            if (null == data)
            {
                return ;
            }

            if (null == drag.DragObject)
            {
                return ;
            }

            drag.DragObject.SetActive(true);

            if (null != data.bind)
            {
                data.bind.GetCom<Toggle>("toggle").isOn = true;
            }
        }

        private void _onDragComEndDrag(ComDrag drag)
        {
            ChapterBattlePotionSetFrameData data = _findDataByGameObject(drag.gameObject);

            if (null == data)
            {
                return ;
            }

            if (null == drag.DragObject)
            {
                return ;
            }

            drag.DragObject.SetActive(false);
        }
        
        private void _updateAllBattlePostions()
        {
            List<ulong> allBattles = _getAllBattleDrugs();

            List<ulong> One = new List<ulong>();
            List<ulong> CanUse = new List<ulong>();
            List<ulong> NoUse = new List<ulong>();

            for (int i = 0; i < allBattles.Count; i++)
            {
                ItemData data = ItemDataManager.GetInstance().GetItem(allBattles[i]);
                if (data == null)
                {
                    continue;
                }
                ItemTable table = TableManager.GetInstance().GetTableItem<ItemTable>(data.TableID);
                if (table != null)
                {
                    if (table.NeedLevel <= PlayerBaseData.GetInstance().Level)
                    {
                        if (table.ID== 200001001)
                        {
                            One.Add(allBattles[i]);
                        }
                        else
                        {
                            CanUse.Add(allBattles[i]);
                        }

                    }
                    else
                    {
                        NoUse.Add(allBattles[i]);
                    }
                }
            }

            Sortmethod(CanUse);
            allBattles.Clear();

            for (int i = 0; i < One.Count; i++)
            {
                allBattles.Add(One[i]);
            }

            for (int i = 0; i < CanUse.Count; i++)
            {
                allBattles.Add(CanUse[i]);
            }

            for (int i = 0; i < NoUse.Count; i++)
            {
                allBattles.Add(NoUse[i]);
            }


            for (int i = 0; i < allBattles.Count; ++i)
            {
                Logger.LogProcessFormat("[PotionSet] get battle drug {0}", allBattles[i]);

                if (null == _findDataById(allBattles[i]))
                {
                    _addDataById(allBattles[i]);
                }
            }

            for (int i = 0; i < mAllBattlePostions.Count; ++i)
            {
                bool find = false;
                for (int j = 0; j < allBattles.Count; ++j)
                {
                    if (allBattles[j] == mAllBattlePostions[i].id)
                    {
                        find = true;
                        break;
                    }
                }

                if (!find)
                {
                    _removeDataById(mAllBattlePostions[i].id);
                }
            }

            _sortAllBattlePosition();
        }

        private List<ulong> _getAllBattleDrugs()
        {
            ProtoTable.ItemTable.eSubType[] alltypes = new ProtoTable.ItemTable.eSubType[] {
                ProtoTable.ItemTable.eSubType.Hp,
                ProtoTable.ItemTable.eSubType.Mp,
                ProtoTable.ItemTable.eSubType.AttributeDrug,
                ProtoTable.ItemTable.eSubType.HpMp
            };

            List<ulong> drugs = new List<ulong>();

            for (int i = 0; i < alltypes.Length; ++i)
            {
                drugs.AddRange(ItemDataManager.GetInstance().GetItemsByPackageSubType(
                        EPackageType.Consumable,
                        alltypes[i]));

            }

            return drugs;
        }

        private void _sortAllBattlePosition()
        {
           // mAllBattlePostions.Sort();
            for (int i = 0; i < mAllBattlePostions.Count; ++i)
            {
                //mAllBattlePostions[i].bind.transform.SetAsFirstSibling();
                _initOnePotionConfig(mAllBattlePostions[i]);
            }
        }
        private void Sortmethod(List<ulong> setData)
        {
            ulong temp;
            for (int i = 0; i < setData.Count; ++i)
            {
                for (int j = 0; j < setData.Count - 1 - i; ++j)
                {
                    ItemData data1 = ItemDataManager.GetInstance().GetItem(setData[j]);
                    if (data1 == null)
                    {
                        continue;
                    }
                    ItemTable table1 = TableManager.GetInstance().GetTableItem<ItemTable>(data1.TableID);
                    if (data1 == null)
                    {
                        continue;
                    }
                    ItemData data2 = ItemDataManager.GetInstance().GetItem(setData[j + 1]);
                    if (data2 == null)
                    {
                        continue;
                    }
                    ItemTable table2 = TableManager.GetInstance().GetTableItem<ItemTable>(data2.TableID);
                    if (table2 == null)
                    {
                        continue;
                    }
                    if (table1.ID< table2.ID)
                    {
                        temp = setData[j];
                        setData[j] = setData[j + 1];
                        setData[j + 1] = temp;
                    }
                }
            }
        }
        private void _initOnePotionConfig(ChapterBattlePotionSetFrameData data)
        {
            if (null == data)
            {
                Logger.LogErrorFormat("[PotionData] frame data is nil");
                return ;
            }

            ComCommonBind bind = data.bind;
            ItemData itemdata  = ItemDataManager.GetInstance().GetItem(data.id);


            Image board = bind.GetCom<Image>("board");
            Image icon = bind.GetCom<Image>("icon");
            Text cnt = bind.GetCom<Text>("cnt");
            GameObject root = bind.GetGameObject("root");
            ComDrag dragCom = bind.GetCom<ComDrag>("dragCom");
            Image dragIcon = bind.GetCom<Image>("dragIcon");
            CanvasGroup select = bind.GetCom<CanvasGroup>("select");
            //UIGray grayCom = bind.GetCom<UIGray>("grayCom");
            GameObject lockRoot = bind.GetGameObject("lockRoot");
            Toggle btnSelect = bind.GetCom<Toggle>("toggle");
            btnSelect.onValueChanged.RemoveAllListeners();
            btnSelect.onValueChanged.AddListener((value)=>{
                if (value)
                    mCurSelectItemId = (uint)itemdata.TableID;
            });
            Button btnMask = bind.GetCom<Button>("Mask");
            btnMask.onClick.RemoveAllListeners();
            btnMask.onClick.AddListener(()=>{
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("package_potion_set_type_error_tip"));
            });
            
            root.SetActive(null != itemdata);
            select.alpha = 0.0f;

            if (null == itemdata)
            {
                //grayCom.enabled = false;
                lockRoot.SetActive(false);
                dragCom.enabled = false;
                Logger.LogProcessFormat("[PotionSet] 没有数据，使用空的栏位");
                return;
            }
 
            btnMask.CustomActive(SlotCanSetPotion((PlayerBaseData.PotionSlotType)mCurSelectIndex, (uint)itemdata.TableID));
            bool isLimted = _isLevelLimite(data);
            lockRoot.SetActive(isLimted);
            dragCom.enabled = !isLimted;

            select.alpha = _isAlreadyInPotionSet(itemdata.TableID) ? 1.0f : 0.0f;

            // icon.sprite = AssetLoader.instance.LoadRes(itemdata.Icon, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref icon, itemdata.Icon);
            dragIcon.sprite = icon.sprite;
            dragIcon.material = icon.material;
            // board.sprite = AssetLoader.instance.LoadRes(itemdata.GetQualityInfo().Background, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref board, itemdata.GetQualityInfo().Background);
            cnt.text = itemdata.Count.ToString();
        }

        private void _updateLevelLimitStatus(ChapterBattlePotionSetFrameData data)
        {
            if (null == data)
            {
                return ;
            }

            ComCommonBind bind = data.bind;

            if (null == bind)
            {
                return ;
            }

            ComDrag dragCom = bind.GetCom<ComDrag>("dragCom");
            //UIGray grayCom = bind.GetCom<UIGray>("grayCom");
            GameObject lockRoot = bind.GetGameObject("lockRoot");

            ItemData itemdata  = ItemDataManager.GetInstance().GetItem(data.id);

            if (null == itemdata)
            {
                dragCom.enabled = false;
                //grayCom.enabled = false;
                lockRoot.SetActive(false);
                return ;
            }

            bool isLimited = _isLevelLimite(data);

            lockRoot.SetActive(isLimited);
            //grayCom.enabled = isLimited;
            dragCom.enabled = !isLimited;

            if (isLimited)
            {
                Logger.LogProcessFormat("[PotionSet] id {0}, 等级限制", data.id);
            }
            else
            {
                Logger.LogProcessFormat("[PotionSet] id {0}, 没有等级限制", data.id);
            }
        }

        private bool _isLevelLimite(ChapterBattlePotionSetFrameData data)
        {
            if (null == data)
            {
                return false;
            }

            ItemData itemdata  = ItemDataManager.GetInstance().GetItem(data.id);
            if (null == itemdata)
            {
                return false;
            }

            return itemdata.LevelLimit > PlayerBaseData.GetInstance().Level;
        }

        private bool _isAlreadyInPotionSet(int tableID)
        {
            List<uint> potionSets = PlayerBaseData.GetInstance().potionSets;

            for (int i = 0; i < potionSets.Count; ++i)
            {
                if (tableID == potionSets[i])
                {
                    return true;
                }
            }

            return false;
        }

        //设置药水
        private int mCurSelectIndex = 0;
        private uint mCurSelectItemId = 0;
        //打开药水列表
        private void _OnClickSet2()
        {
            mCurSelectIndex = 2;
            mCurSelectItemId = 0;
            mImgSelect2.CustomActiveAlpha(true);
            mImgSelect0.CustomActiveAlpha(false);
            mImgSelect1.CustomActiveAlpha(false);
            _ShowPotionList();
        }
        private void _OnClickSet1()
        {
            mCurSelectIndex = 1;
            mCurSelectItemId = 0;
            mImgSelect1.CustomActiveAlpha(true);
            mImgSelect0.CustomActiveAlpha(false);
            mImgSelect2.CustomActiveAlpha(false);
            _ShowPotionList();
        }
        private void _OnClickSet0()
        {
            mCurSelectIndex = 0;
            mCurSelectItemId = 0;
            mImgSelect0.CustomActiveAlpha(true);
            mImgSelect1.CustomActiveAlpha(false);
            mImgSelect2.CustomActiveAlpha(false);
            _ShowPotionList();
        }

        //发送协议设置药水
        private void _SetPotion()
        {
            if (0 == mCurSelectItemId)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("package_potion_set_no_select"));
                return;
            }
            ChapterBattlePotionSetUtiilty.Save(mCurSelectIndex, mCurSelectItemId);
            _updateBattlePostionSetByIdx(mCurSelectIndex, mCurSelectItemId);
            mImgSelect0.CustomActiveAlpha(false);
            mImgSelect1.CustomActiveAlpha(false);
            mImgSelect2.CustomActiveAlpha(false);
            mObjPotionList.CustomActive(false);
        }

        //显示药水列表
        private void _ShowPotionList()
        {
            mObjPotionList.CustomActive(true);
            _sortAllBattlePosition();
            for (int i = 0; i < mAllBattlePostions.Count; ++i)
            {
                ChapterBattlePotionSetFrameData data = mAllBattlePostions[i];
                ComCommonBind bind = data.bind;
                ItemData itemdata = ItemDataManager.GetInstance().GetItem(data.id);
                Button btnMask = bind.GetCom<Button>("Mask");
                if (null != itemdata)
                {
                    btnMask.CustomActive(!SlotCanSetPotion((PlayerBaseData.PotionSlotType)mCurSelectIndex, (uint)itemdata.TableID));
                }
            }
        }

        //隐藏药水列表
        private void _HidePotionList()
        {
            mObjPotionList.CustomActive(false);
            mImgSelect0.CustomActiveAlpha(false);
            mImgSelect1.CustomActiveAlpha(false);
            mImgSelect2.CustomActiveAlpha(false);
        }
    }
}
