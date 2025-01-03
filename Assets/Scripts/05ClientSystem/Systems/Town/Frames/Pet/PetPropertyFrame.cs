using UnityEngine.UI;
using Scripts.UI;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace  GameClient
{
    class PetPropertyFrame : ClientFrame
    {
        string PetPropertyPath = "UIFlatten/Prefabs/Pet/PetPropertyEle";
        
        int ReSelPropertyIndex = -1;
        bool CurSelPetIsFree = true;

        PetInfo CurSelPetInfo = new PetInfo();
        List<PetInfo> PetInfoList = new List<PetInfo>();
        List<GameObject> PetPropObjList = new List<GameObject>();
        
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pet/PetPropertyFrame";
        }
        
        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (userData == null)
            {
                Logger.LogError("can`t get userData");
                return;
            }
            CurSelPetInfo = new PetInfo();
            PetDataManager.GetInstance().SetPetData(CurSelPetInfo, userData as PetInfo);
            InitData();
            InitInterface();
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
            base._OnCloseFrame();
        }
        
        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetItemsInfoUpdate, OnUpdatePetList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetPropertyReselect, OnPetPropertyReselect);

        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetItemsInfoUpdate, OnUpdatePetList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetPropertyReselect, OnPetPropertyReselect);
        }

        void InitData()
        {
            ReSelPropertyIndex = -1;
            //mPointIcon
            SystemValueTable selectKillItemIdData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_PET_SELECT_SKILL_ITEM);
            int selectKillItemId = selectKillItemIdData.Value;
            var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(selectKillItemId);
            if(itemTableData != null)
            {
                ETCImageLoader.LoadSprite(ref mPointIcon, itemTableData.Icon);
            }

            SystemValueTable selectKillItemCountData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_PET_SELECT_SKILL_ITEM_NUM);
            int selectKillItemCount = selectKillItemCountData.Value;
            mPointCount.text = selectKillItemCount.ToString();

            UpdatePetList(true);
        }

        void ClearData()
        {
            ReSelPropertyIndex = -1;
            
            PetInfoList.Clear();
            PetPropObjList.Clear();
            
            UnBindUIEvent();
        }
        

        void InitInterface()
        {
            InitData();
            InitPetItemScrollListBind();
            RefreshPetItemListCount();
            UpdateShowCurSelProperty();
            UpdatePropertyList();
            BindUIEvent();
        }

        void OnUpdatePetList(UIEvent iEvent)
        {
            UpdatePetList();
        }
        
        void UpdatePetList(bool bInit = false)
        {
            PetInfoList.Clear();

            List<PetInfo> OnUsePetItemList = PetDataManager.GetInstance().GetOnUsePetList();
            for (int i = 0; i < OnUsePetItemList.Count; i++)
            {
                PetInfo petinfo = new PetInfo();
                PetDataManager.GetInstance().SetPetData(petinfo, OnUsePetItemList[i]);

                PetInfoList.Add(petinfo);

                if(!bInit)
                {
                    if(OnUsePetItemList[i].id == CurSelPetInfo.id)
                    {
                        PetDataManager.GetInstance().SetPetData(CurSelPetInfo, OnUsePetItemList[i]);
                    }
                }
            }

            List<PetInfo> PacketPetItemList = PetDataManager.GetInstance().GetPetList();

            int iCount = 0;
            List<PetInfo> SortDataList = PetDataManager.GetInstance().GetPetSortListBySortType(PacketPetItemList, ref iCount);

            for(int i = 0; i < SortDataList.Count; i++)
            {
                PetInfo petinfo = new PetInfo();
                PetDataManager.GetInstance().SetPetData(petinfo, SortDataList[i]);

                PetInfoList.Add(petinfo);
            }

            if (!bInit)
            {
                for (int i = 0; i < PacketPetItemList.Count; i++)
                {
                    if (PacketPetItemList[i].id == CurSelPetInfo.id)
                    {
                        PetDataManager.GetInstance().SetPetData(CurSelPetInfo, PacketPetItemList[i]);
                    }
                }
            }
        }
        
        void OnPetPropertyReselect(UIEvent iEvent)
        {
            UInt64 PetID = (UInt64)iEvent.Param1;
            if(PetID != CurSelPetInfo.id)
            {
                return;
            }

            UpdateShowCurSelProperty();
            UpdatePropertyList();
            RefreshPetItemListCount();
        }
        
        void OnSelectPet(int iIndex, bool value)
        {
            if (iIndex < 0 || !value)
            {
                return;
            }

            if(iIndex >= PetInfoList.Count)
            {
                return;
            }

            ReSelPropertyIndex = -1;

            PetDataManager.GetInstance().SetPetData(CurSelPetInfo, PetInfoList[iIndex]);

            UpdateShowCurSelProperty();
            UpdatePropertyList();
        }
        
        void OnSelectProperty(int iIndex, bool value)
        {
            if (iIndex < 0 || !value)
            {
                ReSelPropertyIndex = -1;
                return;
            }

            ReSelPropertyIndex = iIndex;
        }
        
        void InitPetItemScrollListBind()
        {
            mPetScrollList.Initialize();

            mPetScrollList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    UpdatePetItemScrollListBind(item);
                }
            };

            mPetScrollList.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }

                Toggle tgSelect = combind.GetCom<Toggle>("tgSelect");
                tgSelect.onValueChanged.RemoveAllListeners();

                Button iconBtn = combind.GetCom<Button>("btPetItem");
                iconBtn.onClick.RemoveAllListeners();
            };
        }

        void UpdatePetItemScrollListBind(ComUIListElementScript item)
        {
            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }

            if (item.m_index < 0 || item.m_index >= PetInfoList.Count)
            {
                return;
            }

            PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)PetInfoList[item.m_index].dataId);
            if (petData == null)
            {
                return;
            }

            PetDataManager.GetInstance().SetPetItemData(item.gameObject, PetInfoList[item.m_index], PlayerBaseData.GetInstance().JobTableID);

            Text Name = combind.GetCom<Text>("Name");
            Text Level = combind.GetCom<Text>("Level2");
            Text Carry = combind.GetCom<Text>("Carry");

            Name.text = PetDataManager.GetInstance().GetColorName(petData.Name, petData.Quality);
            Level.text = PetInfoList[item.m_index].level.ToString();

            bool bSHowCarry = false;
            List<PetInfo> OnUsePetItemList = PetDataManager.GetInstance().GetOnUsePetList();
            for (int i = 0; i < OnUsePetItemList.Count; i++)
            {
                if(OnUsePetItemList[i].id == PetInfoList[item.m_index].id)
                {
                    Carry.gameObject.CustomActive(true);

                    bSHowCarry = true;
                    break;
                }
            }

            if(!bSHowCarry)
            {
                Carry.gameObject.CustomActive(false);
            }

            Toggle tgSelectPet = combind.GetCom<Toggle>("tgSelect");
            GameObject goSelectImage = combind.GetGameObject("SelectImage");

            if (PetInfoList[item.m_index].id == CurSelPetInfo.id)
            {
                tgSelectPet.isOn = true;

                if (goSelectImage != null)
                    goSelectImage.CustomActive(true);
            }
            else
            {
                if (goSelectImage != null)
                    goSelectImage.CustomActive(false);
            }
            
            tgSelectPet.onValueChanged.RemoveAllListeners();
            int iIndex = item.m_index;
            tgSelectPet.onValueChanged.AddListener((value) => 
            {
                OnSelectPet(iIndex, value);

                if (goSelectImage != null)
                    goSelectImage.CustomActive(true);
            });
        }

        void RefreshPetItemListCount()
        {
            mPetScrollList.SetElementAmount(PetInfoList.Count);
        }
        
        void UpdatePropertyList()
        {
            PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)CurSelPetInfo.dataId);
            if (petData == null)
            {
                return;
            }

            List<int> Skillids = PetDataManager.GetInstance().GetPetSkillIDsByJob(petData, PlayerBaseData.GetInstance().JobTableID);

            int iAddedNum = Skillids.Count - PetPropObjList.Count;

            for (int i = 0; i < iAddedNum; i++)
            {
                GameObject PropertyElement = AssetLoader.instance.LoadResAsGameObject(PetPropertyPath);
                if (PropertyElement == null)
                {
                    continue;
                }

                Utility.AttachTo(PropertyElement, mPropertyListRoot);
                PropertyElement.CustomActive(false);

                PetPropObjList.Add(PropertyElement);
            }
            bool haveSameSkill = false;//当服务器同步的技能数据不在规定范围内的时候，默认选中第一个技能
            for (int i = 0; i < PetPropObjList.Count; i++)
            {
                if (i < Skillids.Count)
                {
                    ComCommonBind combind = PetPropObjList[i].GetComponent<ComCommonBind>();
                    if (combind == null)
                    {
                        PetPropObjList[i].CustomActive(false);
                        continue;
                    }

                    SkillDescriptionTable skillDesTableData = TableManager.GetInstance().GetTableItem<SkillDescriptionTable>(Skillids[i]);
                    if(skillDesTableData == null)
                    {
                        PetPropObjList[i].CustomActive(false);
                        continue;
                    }
                    
                    PetPropObjList[i].CustomActive(true);

                    if (i == CurSelPetInfo.skillIndex)
                    {
                        PetPropObjList[i].CustomActive(false);
                        if (mCurPetProperty != null)
                        {
                            combind = mCurPetProperty.GetComponent<ComCommonBind>();
                            if (combind == null)
                            {
                                mCurPetProperty.gameObject.CustomActive(false);
                            }
                        }
                        
                    }

                    Text Name = combind.GetCom<Text>("Name");
                    Text Des = combind.GetCom<Text>("Des");
                    Text Skill = combind.GetCom<Text>("Skill");
                    Text CurProperty = combind.GetCom<Text>("CurProperty");
                    Toggle tgSelect = combind.GetCom<Toggle>("Select");      

                    Name.SafeSetText(string.Format("[{0}]", skillDesTableData.Name));
                    Des.SafeSetText(skillDesTableData.Description);

                    List<string> deslist = SkillDataManager.GetInstance().GetSkillDesList(Skillids[i], (byte)CurSelPetInfo.level);

                    Skill.text = "";
                    for (int j = 0; j < deslist.Count; j++)
                    {
                        if(j < deslist.Count - 1)
                        {
                            Skill.SafeSetText(string.Format("{0}\n", deslist[j]));
                        }
                        else
                        {
                            Skill.SafeSetText(string.Format("{0}", deslist[j]));
                        }              
                    }

                    int idex = i;

                    tgSelect.onValueChanged.RemoveAllListeners();
                    tgSelect.isOn = false;
                    tgSelect.onValueChanged.AddListener((value) => { OnSelectProperty(idex, value); });

                    if (i == CurSelPetInfo.skillIndex)
                    {
                        haveSameSkill = true;
                        CurProperty.gameObject.CustomActive(true);
                        tgSelect.gameObject.CustomActive(false);
                    }
                    else
                    {
                        CurProperty.gameObject.CustomActive(false);
                        tgSelect.gameObject.CustomActive(true);
                    }

                    tgSelect.group = mPropToggleGroup;
                }
                else
                {
                    PetPropObjList[i].CustomActive(false);
                }
            }

            if (!haveSameSkill)
            {
                ComCommonBind combind = PetPropObjList[0].GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }

                SkillDescriptionTable skillDesTableData = TableManager.GetInstance().GetTableItem<SkillDescriptionTable>(Skillids[0]);
                if (skillDesTableData == null)
                {
                    return;
                }
                Text CurProperty = combind.GetCom<Text>("CurProperty");
                Toggle tgSelect = combind.GetCom<Toggle>("Select");

                CurProperty.gameObject.CustomActive(true);
                tgSelect.gameObject.CustomActive(false);

            }
        }



        void UpdateShowCurSelProperty()
        {
            mFreeSelectTips.CustomActive(false);
            mPointSelectTips.CustomActive(false);

            PetTable PetData = TableManager.GetInstance().GetTableItem<PetTable>((int)CurSelPetInfo.dataId);
            if(PetData == null || PetData.Skills.Count <= CurSelPetInfo.skillIndex)
            {
                return;
            }

            int iSkillID = PetDataManager.GetInstance().GetPetSkillIDByIndex(PetData, CurSelPetInfo.skillIndex, PlayerBaseData.GetInstance().JobTableID);
            if(iSkillID <= 0)
            {
                return;
            }

            SkillDescriptionTable SkillDesData = TableManager.GetInstance().GetTableItem<SkillDescriptionTable>(iSkillID);
            if(SkillDesData == null)
            {
                return;
            }

            // mTitle.text = SkillDesData.Name;
            if (CurSelPetInfo.selectSkillCount == 0)
            {
                if (mReSelectGray != null)
                {
                    mReSelectGray.enabled = false;
                }
                if (mReSelect != null)
                {
                    mReSelect.interactable = true;
                }
                
                mReSelectText.SafeSetText("重选");
                mFreeSelectTips.CustomActive(true);
                CurSelPetIsFree = true;
            }
            else if (CurSelPetInfo.selectSkillCount > 0)
            {
                if (mReSelectGray != null)
                {
                    mReSelectGray.enabled = false;
                }
                if (mReSelect != null)
                {
                    mReSelect.interactable = true;
                }
                
                mReSelectText.SafeSetText("重选");
                mPointSelectTips.CustomActive(true);
                CurSelPetIsFree = false;
            }
        }
        
        void SendReselectPropertyReq()
        {
            if(CurSelPetIsFree)
            {
                SceneChangePetSkillReq req = new SceneChangePetSkillReq();

                req.id = CurSelPetInfo.id;
                req.skillIndex = (byte)ReSelPropertyIndex;

                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
            }
            else
            {
                SystemValueTable selectKillItemIdData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_PET_SELECT_SKILL_ITEM);
                int selectKillItemId = selectKillItemIdData.Value;
                var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(selectKillItemId);
                if (itemTableData == null)
                {
                    return;
                }
                string name = itemTableData.Name;
                SystemValueTable selectKillItemCountData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_PET_SELECT_SKILL_ITEM_NUM);
                int price = selectKillItemCountData.Value;
                var itemSubtype = itemTableData.SubType;
                string notifyCont = string.Format(TR.Value("pet_reselect_tips"), price, name);
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCont, () =>
                {
                    CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

                    costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(itemSubtype);

                    ItemTipManager.GetInstance().CloseAll();

                    costInfo.nCount = price;

                    CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                    {
                        SceneChangePetSkillReq req = new SceneChangePetSkillReq();

                        req.id = CurSelPetInfo.id;
                        req.skillIndex = (byte)ReSelPropertyIndex;

                        NetManager netMgr = NetManager.Instance();
                        netMgr.SendCommand(ServerType.GATE_SERVER, req);
                    });
                });
            }
            
        }
                
                
        #region ExtraUIBind
        private GameObject mFreeSelectTips = null;
        private GameObject mPointSelectTips = null;
        private Image mPointIcon = null;
        private Text mPointCount = null;
        private Text mReSelectNum = null;
        private ToggleGroup mPropToggleGroup = null;
        private GameObject mPropertyListRoot = null;
        private Text mReSelectText = null;
        private Button mBtClose = null;
        private ComUIListScript mPetScrollList = null;
        private UIGray mReSelectGray = null;
        private Button mReSelect = null;
        private GameObject mCurPetProperty = null;

        protected override void _bindExUI()
        {
	        mFreeSelectTips = mBind.GetGameObject("FreeSelectTips");
	        mPointSelectTips = mBind.GetGameObject("PointSelectTips");
	        mPointIcon = mBind.GetCom<Image>("PointIcon");
	        mPointCount = mBind.GetCom<Text>("PointCount");
	        mReSelectNum = mBind.GetCom<Text>("ReSelectNum");
	        mPropToggleGroup = mBind.GetCom<ToggleGroup>("PropToggleGroup");
	        mPropertyListRoot = mBind.GetGameObject("PropertyListRoot");
	        mReSelectText = mBind.GetCom<Text>("ReSelectText");
	        mBtClose = mBind.GetCom<Button>("btClose");
	        mBtClose.onClick.AddListener(_onBtCloseButtonClick);
	        mPetScrollList = mBind.GetCom<ComUIListScript>("PetScrollList");
	        mReSelectGray = mBind.GetCom<UIGray>("ReSelectGray");
	        mReSelect = mBind.GetCom<Button>("ReSelect");
	        mReSelect.onClick.AddListener(_onReSelectButtonClick);
	        mCurPetProperty = mBind.GetGameObject("CurPetProperty");
        }

        protected override void _unbindExUI()
        {
	        mFreeSelectTips = null;
	        mPointSelectTips = null;
	        mPointIcon = null;
	        mPointCount = null;
	        mReSelectNum = null;
	        mPropToggleGroup = null;
	        mPropertyListRoot = null;
	        mReSelectText = null;
	        mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
	        mBtClose = null;
	        mPetScrollList = null;
	        mReSelectGray = null;
	        mReSelect.onClick.RemoveListener(_onReSelectButtonClick);
	        mReSelect = null;
	        mCurPetProperty = null;
        }
        #endregion   

        #region Callback
        private void _onBtCloseButtonClick()
        {
            Close();

        }
        private void _onReSelectButtonClick()
        {
            if (ReSelPropertyIndex < 0 )
            {
                SystemNotifyManager.SystemNotify(9059);
                return;
            }
            if(ReSelPropertyIndex == CurSelPetInfo.skillIndex)
            {
                SystemNotifyManager.SystemNotify(9059);
                return;
            }
            
            SendReselectPropertyReq();

        }
        #endregion
        
    }
}
