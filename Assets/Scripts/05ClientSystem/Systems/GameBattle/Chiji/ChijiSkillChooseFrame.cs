using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

public enum PickUpType
{
    None = 0,
    PickUpSkill,
    PickUpItem,
}

namespace GameClient
{
    public class ChijiSkillChooseFrame : ClientFrame
    {
        public static PickUpType pickUpType = PickUpType.None;
        ChiJiSkill[] skillList = null;
        UInt32[] equipList = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiSkillChooseFrame";
        }

        protected override void _OnOpenFrame()
        {
            _BindUIEvent();

            if (userData != null)
            {
                if(pickUpType == PickUpType.PickUpSkill)
                {
                    ChiJiSkill[] list = userData as ChiJiSkill[];
                    skillList = new ChiJiSkill[list.Length];
                    skillList = list;

                    if(mTitle != null)
                    {
                        mTitle.text = "技能选择";
                    }
                }
                else if(pickUpType == PickUpType.PickUpItem)
                {
                    UInt32[] list = userData as UInt32[];
                    equipList = new UInt32[list.Length];
                    equipList = list;

                    if (mTitle != null)
                    {
                        mTitle.text = "装备选择";
                    }
                }
            }

            _InitItemScrollListBind();
            _RefreshItemListCount();
        }

        protected override void _OnCloseFrame()
        {
            _UnBindUIEvent();
            _ClearData();
        }

        private void _ClearData()
        {
            if (skillList != null)
            {
                skillList = null;
            }

            if(equipList != null)
            {
                equipList = null;
            }
        }

        void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OpenChijiSkillChooseFrame, _OnOpenChijiSkillChooseFrame);
        }

        void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OpenChijiSkillChooseFrame, _OnOpenChijiSkillChooseFrame);
        }

        void _OnOpenChijiSkillChooseFrame(UIEvent iEvent)
        {
            pickUpType = (PickUpType)iEvent.Param1;

            if (pickUpType == PickUpType.PickUpSkill)
            {
                skillList = iEvent.Param2 as ChiJiSkill[];

                if (mTitle != null)
                {
                    mTitle.text = "技能选择";
                }
            }
            else if (pickUpType == PickUpType.PickUpItem)
            {
                equipList = iEvent.Param2 as UInt32[];

                if (mTitle != null)
                {
                    mTitle.text = "装备选择";
                }
            }

            _InitItemScrollListBind();
            _RefreshItemListCount();
        }

        void _InitItemScrollListBind()
        {
            mChijiSkillUIListScript.Initialize();

            mChijiSkillUIListScript.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdateItemScrollListBind(item);
                }
            };

            mChijiSkillUIListScript.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }

                Button iconBtn = combind.GetCom<Button>("select");
                iconBtn.onClick.RemoveAllListeners();
            };
        }

        void _UpdateItemScrollListBind(ComUIListElementScript item)
        {
            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }

            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }

            Text Name = combind.GetCom<Text>("name");
            Image skillIcon = combind.GetCom<Image>("skillIcon");
            GameObject equipIcon = combind.GetGameObject("EquipIcon");
            Text des = combind.GetCom<Text>("des");
            Button select = combind.GetCom<Button>("select");

            if (pickUpType == PickUpType.PickUpSkill)
            {
                if (skillList == null || item.m_index < 0 || item.m_index >= skillList.Length)
                {
                    return;
                }

                SkillTable tableData = TableManager.GetInstance().GetTableItem<SkillTable>((int)skillList[item.m_index].skillId);
                if (tableData == null)
                {
                    return;
                }

                if (Name != null)
                {
                    Name.text = tableData.Name + "Lv." + skillList[item.m_index].skillLvl;
                }

                if (skillIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref skillIcon, tableData.Icon);
                }

                if (des != null)
                {
                    des.text = SkillDataManager.GetInstance().GetSkillDescription(tableData);
                }
            }
            else if(pickUpType == PickUpType.PickUpItem)
            {
                if (equipList == null || item.m_index < 0 || item.m_index >= equipList.Length)
                {
                    return;
                }

                ItemTable tableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)equipList[item.m_index]);
                if (tableData == null)
                {
                    return;
                }

                if (skillIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref skillIcon, tableData.Icon);
                }

                ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)equipList[item.m_index]);
                if (itemData != null)
                {
                    if (Name != null)
                    {
                        Name.text = itemData.GetColorName();
                    }

                    if (des != null)
                    {
                        des.text = ChijiShopUtility.GetItemDetailStr(itemData);
                    }

                    if (equipIcon != null)
                    {
                        ItemData wearitemdata = ItemDataManager.GetInstance().GetWearEquipDataBySlotType(itemData.EquipWearSlotType);
                        equipIcon.CustomActive(wearitemdata != null && wearitemdata.TableID == itemData.TableID);
                    }
                }
            }

            select.onClick.RemoveAllListeners();
            int iIndex = item.m_index;
            select.onClick.AddListener(() => { OnSelectSkill(iIndex); });
        }

        void OnSelectSkill(int iIndex)
        {
            if(pickUpType == PickUpType.PickUpSkill)
            {
                if (skillList == null || iIndex < 0 || iIndex >= skillList.Length)
                {
                    return;
                }

                ChijiDataManager.GetInstance().SendSelectSkillReq(skillList[iIndex].skillId);
            }
            else if (pickUpType == PickUpType.PickUpItem)
            {
                if (equipList == null || iIndex < 0 || iIndex >= equipList.Length)
                {
                    return;
                }

                ChijiDataManager.GetInstance().SendSelectItemReq(equipList[iIndex]);
            }

            _onBtCloseButtonClick();
        }

        void _RefreshItemListCount()
        {
            if(pickUpType == PickUpType.PickUpSkill)
            {
                if (mChijiSkillUIListScript == null || skillList == null)
                {
                    return;
                }

                mChijiSkillUIListScript.SetElementAmount(skillList.Length);
            }
            else if (pickUpType == PickUpType.PickUpItem)
            {
                if (mChijiSkillUIListScript == null || equipList == null)
                {
                    return;
                }

                mChijiSkillUIListScript.SetElementAmount(equipList.Length);
            }
        }

        #region ExtraUIBind
        private Button mBtClose = null;
        private ComUIListScript mChijiSkillUIListScript = null;
        private Text mTitle = null;

        protected override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mChijiSkillUIListScript = mBind.GetCom<ComUIListScript>("ChijiSkillUIListScript");
            mTitle = mBind.GetCom<Text>("title");
        }

        protected override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mChijiSkillUIListScript = null;
            mTitle = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
