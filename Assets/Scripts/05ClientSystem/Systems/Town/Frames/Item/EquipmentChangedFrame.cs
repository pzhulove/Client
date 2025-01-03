using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ProtoTable;
using Protocol;

namespace GameClient
{
    public class EquipmentChangedFrame : ClientFrame
    {
        ComItem comItem;
        GameObject goItemParent;
        ulong m_currentGuid;
        Text m_kName;
        Text m_kHint;
        ComEffect m_kComEffect;
        bool m_bLocked = false;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BetterEquipHint/EquipmentChangedFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            BindUIEvent();

            comItem = null;
            goItemParent = Utility.FindChild(frame,"back/ItemObject");
            m_currentGuid = 0;
            m_kName = Utility.FindComponent<Text>(frame, "back/BtnWearImmediately/text");
            m_kHint = Utility.FindComponent<Text>(frame, "back/front/Name");
            m_kComEffect = Utility.FindComponent<ComEffect>(frame, "back/EffectParent");

            frame.gameObject.transform.SetAsFirstSibling();

            ItemDataManager.GetInstance().onNeedPopEquipsChanged += OnNeedPopEquipsChanged;

            bool bVisible = IsFrameNeedShow();

            _UpdateFrameVisible(bVisible);

            m_bLocked = false;

            OnNeedPopEquipsChanged(ItemDataManager.GetInstance().NeedEquiptmentsID);
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NewFuncFrameOpen, OnNewFuncFrameOpen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NewFuncFrameClose, OnNewFuncFrameClose);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideStart, OnNewbieGuideStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideFinish, OnNewbieGuideFinish);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TipsAniStart, OnTipsAniStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TipsAniEnd, OnTipsAniEnd);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipSubmitSuccess, OnEquipSubmitSuccess);
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NewFuncFrameOpen, OnNewFuncFrameOpen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NewFuncFrameClose, OnNewFuncFrameClose);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideStart, OnNewbieGuideStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideFinish, OnNewbieGuideFinish);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TipsAniStart, OnTipsAniStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TipsAniEnd, OnTipsAniEnd);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipSubmitSuccess, OnEquipSubmitSuccess);
        }

        void OnNewFuncFrameOpen(UIEvent uiEvent)
        {
            _UpdateFrameVisible(false);
        }

        void OnNewFuncFrameClose(UIEvent uiEvent)
        {
            if (PlayerBaseData.GetInstance().NewUnlockFuncList.Count > 0)
            {
                _UpdateFrameVisible(false);
            }
            else
            {
                if(NewbieGuideManager.GetInstance().IsGuiding())
                {
                    _UpdateFrameVisible(false);
                }
                else
                {
                    _UpdateFrameVisible(true);
                }             
            }
        }

        void OnNewbieGuideStart(UIEvent uiEvent)
        {
            if((NewbieGuideTable.eNewbieGuideTask)uiEvent.Param1 == NewbieGuideTable.eNewbieGuideTask.QuickEquipGuide)
            {
                _UpdateFrameVisible(true);
            }
            else
            {
                _UpdateFrameVisible(false);
            }        
        }

        void OnNewbieGuideFinish(UIEvent uiEvent)
        {
            _UpdateFrameVisible(true);
        }

        void OnTipsAniStart(UIEvent uiEvent)
        {
            _UpdateFrameVisible(false);
        }

        void OnTipsAniEnd(UIEvent uiEvent)
        {
            _UpdateFrameVisible(true);
        }

        void OnEquipSubmitSuccess(UIEvent uiEvent)
        {
            EqRecScoreItem[] items = (EqRecScoreItem[])uiEvent.Param2;
            for(int i= 0;i<items.Length;i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData != null)
                {
                    ItemDataManager.GetInstance().NeedEquiptmentsID.Remove(items[i].uid);
                    OnNeedPopEquipsChanged(ItemDataManager.GetInstance().NeedEquiptmentsID);
                }
            }
        }

        //在打开的时候，判断Frame是否可以展示
        private bool IsFrameNeedShow()
        {
            //新的功能解锁
            if (PlayerBaseData.GetInstance().NewUnlockFuncList.Count > 0
                || ClientSystemManager.GetInstance().IsFrameOpen<FunctionUnlockFrame>())
            {
                return false;
            }

            //存在非装备的新手引导任务
            if (NewbieGuideManager.GetInstance().IsGuiding()
                && !NewbieGuideManager.GetInstance().IsGuidingTask(NewbieGuideTable.eNewbieGuideTask.QuickEquipGuide))
            {
                return false;
            }

            //是否右下角存在tips
            if (ClientSystemTownFrame.IsShowSkillTips == true
                || ClientSystemTownFrame.IsShowEquipHandBookTips == true
                || ClientSystemTownFrame.IsShowGuildTips == true)
            {
                return false;
            }
            
            return true;
        }

        void _UpdateFrameVisible(bool bVisible)
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                SetVisible(false);
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                SetVisible(false);
                return;
            }
            //
            if (scenedata.SceneType != CitySceneTable.eSceneType.NORMAL
                && scenedata.SceneType != CitySceneTable.eSceneType.SINGLE)
            {
                SetVisible(false);
                return;
            }

            if (ClientSystemTownFrame.IsShowSkillTips == true
                || ClientSystemTownFrame.IsShowEquipHandBookTips == true
                || ClientSystemTownFrame.IsShowGuildTips == true)
            {
                SetVisible(false);
                return;
            }
            
            SetVisible(bVisible);
        }

        void OnNeedPopEquipsChanged(List<ulong> equipts)
        {
            //如果装备当前装备成功
            if(OnEquipment(m_currentGuid))
            {
                return;
            }

            for(int i = 0; i < equipts.Count; ++i)
            {
                if(m_currentGuid != equipts[i] && OnEquipment(equipts[i]))
                {
                    return;
                }
            }

            frameMgr.CloseFrame(this);
        }

		void DisplayPrompt()
		{
			var candicates = ItemDataManager.GetInstance().NeedEquiptmentsID;
			while (candicates.Count > 0)
			{
				var id = ItemDataManager.GetInstance().NeedEquiptmentsID[0];
                if(OnEquipment(id))
                {
                    ItemData itemData = ItemDataManager.GetInstance().GetItem(id);
                    if (itemData != null && itemData.Count <= 1)
                    {
                        candicates.Remove(id);
                    }
                    break;
                }
                candicates.Remove(id);
            }

            if(comItem == null || comItem.ItemData == null)
            {
                frameMgr.CloseFrame(this);
            }
        }

        bool OnEquipment(ulong GUID)
        {
            m_currentGuid = GUID;
            if (!ItemDataManager.GetInstance().NeedEquiptmentsID.Contains(GUID))
            {
                m_currentGuid = 0;
                return false;
            }

            if (comItem == null)
            {
                comItem = CreateComItem(goItemParent);
            }
            if (comItem != null)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(m_currentGuid);
                comItem.Setup(itemData,(GameObject obj, ItemData item)=>
                {
                    ItemTipManager.GetInstance().ShowTip(item);
                });

                if(itemData != null)
                {
                    if (PlayerBaseData.GetInstance().Level < 10 && itemData.Type == ItemTable.eType.EQUIP ||
                       PlayerBaseData.GetInstance().Level < 10 && itemData.SubType == (int)ItemTable.eSubType.GiftPackage||
                       ((PlayerBaseData.GetInstance().Level < itemData.LevelLimit || PlayerBaseData.GetInstance().Level > itemData.MaxLevelLimit)))
                    {
                        OnClickClose();
                        return true;
                    }
                    m_kHint.text = itemData.GetColorName();
                    if (itemData.SubType == (int) ItemTable.eSubType.ExperiencePill)
                    {
                        ////经验丹：一键使用
                        m_kName.text = TR.Value("equipment_use");
                        var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int) itemData.TableID);
                        if (itemTable != null)
                        {
                            //一键使用
                            if (itemTable.CanUse == ItemTable.eCanUse.UseTotal)
                            {
                                m_kName.text = TR.Value("equipment_onekey_use");
                            }
                        }
                    }
                    else if (itemData.SubType == (int) ItemTable.eSubType.GiftPackage)
                    {
                        //礼包类型，分为使用和一键使用
                        //其他：使用
                        m_kName.text = TR.Value("equipment_use");

                        ItemTable itemTable =
                            TableManager.GetInstance().GetTableItem<ItemTable>((int) itemData.TableID);
                        if (itemTable != null)
                        {
                            if (itemTable.EPrompt == ItemTable.eEPrompt.EPT_NEW_EQUIP
                                && itemTable.CanUse == ItemTable.eCanUse.UseTotal
                                && itemData.Count > 1)
                            {
                                m_kName.text = TR.Value("equipment_onekey_use");
                            }
                        }
                    }
                    else if (itemData.Type == ItemTable.eType.EQUIP)
                    {
                        //装备：穿戴
                        m_kName.text = TR.Value("equipment_dress");
                    }
                    else
                    {
                        //其他：使用
                        m_kName.text = TR.Value("equipment_use");
                    }
                    return true;
                }
            }

            m_currentGuid = 0;
            return false;
        }

        protected sealed override void _OnCloseFrame()
        {
            UnBindUIEvent();
            comItem = null;
            ItemDataManager.GetInstance().onNeedPopEquipsChanged -= OnNeedPopEquipsChanged;
            InvokeMethod.RemoveInvokeCall(_UnLock);
        }

        [UIEventHandle("back/BtnClose")]
        void OnClickClose()
        {
			ItemData itemData = ItemDataManager.GetInstance().GetItem(m_currentGuid);
			if (itemData != null)
			{
                ItemDataManager.GetInstance().NeedEquiptmentsID.Remove(m_currentGuid);
			}

            OnNeedPopEquipsChanged(ItemDataManager.GetInstance().NeedEquiptmentsID);
        }

        [UIEventHandle("back/BtnWearImmediately")]
        void OnWearImmediately()
        {
            var item = ItemDataManager.GetInstance().GetItem(m_currentGuid);
            if (item.EquipType == EEquipType.ET_BREATH)
            {
                SystemNotifyManager.SysNotifyTextAnimation("带有虚空物质的装备无法穿戴");
                return;
            }
            if(item != null && item.PackageType != EPackageType.WearEquip && !m_bLocked)
            {
                m_bLocked = true;
                if (item.PackID > 0)
                {
                    GiftPackTable giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(item.PackID);
                    if (giftPackTable != null)
                    {
                        if (giftPackTable.FilterType == GiftPackTable.eFilterType.Custom || giftPackTable.FilterType == GiftPackTable.eFilterType.CustomWithJob)
                        {
                            if (giftPackTable.FilterCount > 0)
                            {
                                ClientSystemManager.GetInstance().OpenFrame<SelectItemFrame>(FrameLayer.Middle, item);
                            }
                            else
                            {
                                Logger.LogErrorFormat("礼包{0}的FilterCount小于等于0", item.PackID);
                            }
                        }
                        else if (item.SubType == (int)ItemTable.eSubType.MagicBox)
                        {
                            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);
                            if (itemGuids != null)
                            {
                                int magicHammer = 0;
                                for (int i = 0; i < itemGuids.Count; i++)
                                {
                                    ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                                    if (itemData == null)
                                    {
                                        continue;
                                    }

                                    if (itemData.SubType != (int)ItemTable.eSubType.MagicHammer)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        magicHammer = Mathf.FloorToInt((itemData.Count) / 4);
                                    }

                                }

                                if (magicHammer >= item.Count)
                                {
                                    //向服务器发送协议
                                    MagicBoxDataManager.GetInstance().AnsyOpenMagBox(OpenMagicBoxFrame, item.GUID, item.Count);
                                }
                                else if (magicHammer < item.Count && magicHammer != 0)
                                {
                                    MagicBoxDataManager.GetInstance().AnsyOpenMagBox(OpenMagicBoxFrame, item.GUID, magicHammer);
                                }
                                else
                                {
                                    ItemComeLink.OnLink(800002002, 0);
                                }

                            }
                            ItemTipManager.GetInstance().CloseAll();
                        }
                        else if (item.SubType == (int)ItemTable.eSubType.MagicHammer)
                        {
                            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);
                            if (itemGuids != null)
                            {
                                int magicBox = 0;
                                for (int i = 0; i < itemGuids.Count; i++)
                                {
                                    ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                                    if (itemData == null)
                                    {
                                        continue;
                                    }

                                    if (itemData.SubType != (int)ItemTable.eSubType.MagicBox)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        magicBox = itemData.Count;
                                    }

                                }

                                if (magicBox > 0)
                                {
                                    int num = Mathf.FloorToInt((item.Count) / 4);

                                    if (num >= magicBox)
                                    {
                                        //向服务器发送协议
                                        MagicBoxDataManager.GetInstance().AnsyOpenMagBox(OpenMagicBoxFrame, item.GUID, magicBox);
                                    }
                                    else if (num < magicBox && num != 0)
                                    {
                                        MagicBoxDataManager.GetInstance().AnsyOpenMagBox(OpenMagicBoxFrame, item.GUID, num);
                                    }
                                    else
                                    {
                                        ItemComeLink.OnLink(800002002, 0);
                                    }
                                }
                                else
                                {
                                    ItemComeLink.OnLink(800002001, 0);
                                }

                            }
                            ItemTipManager.GetInstance().CloseAll();
                        }
                        else if (item.SubType == (int) ItemTable.eSubType.GiftPackage)
                        {
                            //礼包一键使用
                            ItemDataManager.GetInstance().UseItem(item, true);
                            ItemTipManager.GetInstance().CloseAll();
                        }                        
                        else
                        {
                            ItemDataManager.GetInstance().UseItem(item);
                            if (item.Count <= 1 || item.CD > 0)
                            {
                                ItemTipManager.GetInstance().CloseAll();
                            }
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("道具{0}的礼包ID{1}不存在", item.TableID, item.PackID);
                    }
                }
                else
                {
                    if (item.Packing)
                    {
                        SystemNotifyManager.SystemNotify(2006,
                            () =>
                            {
                                _OnUseItem(item);
                            },
                            null,
                            item.GetColorName());
                    }
                    else
                    {
                        _OnUseItem(item);
                    }
                }

                InvokeMethod.Invoke(0.50f,_UnLock);
            }
        }

        void OpenMagicBoxFrame()
        {
            MagicBoxFrame.MagicBoxResultFrameData data = new MagicBoxFrame.MagicBoxResultFrameData();
            data.itemRewards = MagicBoxDataManager.GetInstance().itemRrewardList;
            data.ItemDoubleRewards = MagicBoxDataManager.GetInstance().itemDoubleRewardList;

            if (ClientSystemManager.GetInstance().IsFrameOpen<MagicBoxFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MagicBoxFrame>();
            }

            ClientFrame.OpenTargetFrame<MagicBoxFrame>(FrameLayer.Middle, data);
        }

        void _UnLock()
        {
            m_bLocked = false;
        }

        void _OnUseItem(ItemData item)
        {
            m_kComEffect.Play("UsedEffect");
            InvokeMethod.Invoke(0.40f, () => 
            {
                if(item != null)
                {
                    //礼包类型 类型的道具，采用一键使用所有；
                    if (item.SubType == (int) ProtoTable.ItemTable.eSubType.GiftPackage)
                    {
                        ItemDataManager.GetInstance().UseItem(item, true);
                    }
                    else if (item.SubType == (int) ProtoTable.ItemTable.eSubType.ExperiencePill)
                    {
                        //经验丹： 采用一键使用或者使用
                        var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)item.TableID);
                        if (itemTable != null)
                        {
                            //一键使用
                            if (itemTable.CanUse == ItemTable.eCanUse.UseTotal)
                            {
                                ItemDataManager.GetInstance().UseItem(item, true);
                            }
                            else
                            {
                                ItemDataManager.GetInstance().UseItem(item);
                            }
                        }
                    }
                    else if (item.SubType == (int)ProtoTable.ItemTable.eSubType.PetEgg && item.Type == ItemTable.eType.EXPENDABLE)
                    {
                        ItemDataManager.GetInstance().UseItem(item);
                        WaitNetMessageManager.GetInstance().Wait<SceneUseItemRet>(msgRet =>
                        {
                            if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                            {
                                SystemNotifyManager.SystemNotify((int)msgRet.code);
                            }
                            else
                            {
                                if (ClientSystemManager.GetInstance().IsFrameOpen<OpenPetEggFrame>())
                                {
                                    ClientSystemManager.GetInstance().CloseFrame<OpenPetEggFrame>();
                                }
                                ClientSystemManager.GetInstance().OpenFrame<OpenPetEggFrame>(FrameLayer.Middle, item);
                            }
                        });
                    }
                    else
                    {
                        ItemDataManager.GetInstance().UseItem(item);
                    }
                }
            });
        }
    }
}
