using UnityEngine.UI;
using Scripts.UI;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    class PetUpgradeFrame : ClientFrame
    {
        string EatPetItemPath = "UIFlatten/Prefabs/Pet/EatPetEle";
        string CritEffectPath = "Effects/Scene_effects/EffectUI/EffUI_cwkl_baoji";
        string CritNumPath = "UIFlatten/Prefabs/Pet/PetCritEffect";

        int MaxGoldFeedNum = 0;
        int MaxTicketFeedNum = 0;
        int ExpChangeCoeffi = 0;
        int MoneyChangeCoeffi = 0;

        UInt64 TotalNeedExp = 0;
        bool bSelAll = false;

        PetInfo CurSelPetInfo = new PetInfo();
        List<PetInfo> PetInfoList = new List<PetInfo>();
        List<PetInfo> EatPetIDList = new List<PetInfo>();
        List<PetInfo> EatPetInfoList = new List<PetInfo>();

        List<GameObject> PetEleObjList = new List<GameObject>();


        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pet/PetUpgradeFrame";
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
            InitInterFace();
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetItemsInfoUpdate, OnUpdatePetList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetFeedSuccess, OnPetFeedSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EatPetSuccess, OnPetFeedSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GoldChanged, OnUpdatePetBuyUI);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TicketChanged, OnUpdatePetBuyUI);

            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetItemsInfoUpdate, OnUpdatePetList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetFeedSuccess, OnPetFeedSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EatPetSuccess, OnPetFeedSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GoldChanged, OnUpdatePetBuyUI);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TicketChanged, OnUpdatePetBuyUI);

            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;
        }

        

        void InitData()
        {
            // List<PetInfo> OnUsePetItemList = PetDataManager.GetInstance().GetOnUsePetList();
            // if (OnUsePetItemList.Count > 0)
            // {
            //     PetDataManager.GetInstance().SetPetData(CurSelPetInfo, OnUsePetItemList[0]);
            // }

            SystemValueTable GoldFeedData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PET_GOLD_FEED_MAX);
            MaxGoldFeedNum = GoldFeedData.Value;

            SystemValueTable TicketFeedData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PET_POINT_FEED_MAX);
            MaxTicketFeedNum = TicketFeedData.Value;

            SystemValueTable Expdata = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PET_DEVOUR_EXP_RATIO);
            ExpChangeCoeffi = Expdata.Value;

            SystemValueTable Moneydata = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PET_FEED_GOLD_RATIO);
            MoneyChangeCoeffi = Moneydata.Value;

            UpdatePetList(true);
        }

        void ClearData()
        {
            UInt64 TotalNeedExp = 0;
            int MoneyChangeCoeffi = 0;
            bSelAll = false;

            PetInfoList.Clear();
            EatPetIDList.Clear();
            EatPetInfoList.Clear();

            for (int i = 0; i < PetEleObjList.Count; i++)
            {
                ComCommonBind combind = PetEleObjList[i].GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    continue;
                }

                Button btPetItem = combind.GetCom<Button>("btPetItem");
                Toggle tgSelectPet = combind.GetCom<Toggle>("tgSelect");
                if (btPetItem != null)
                    btPetItem.onClick.RemoveAllListeners();
                if (tgSelectPet != null)
                    tgSelectPet.onValueChanged.RemoveAllListeners();
            }
            UnBindUIEvent();
        }

        void InitInterFace()
        {
            
            InitPetItemScrollListBind();
            RefreshPetItemListCount();
            UpdateSelPetInfo();
            UpdateActor((int)CurSelPetInfo.dataId);
            BindUIEvent();
        }


        void OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            UpdateEatNeedMoney();
        }

        void OnUpdatePetList(UIEvent iEvent)
        {
            UpdatePetList();
        }

        void OnPetFeedSuccess(UIEvent iEvent)
        {
            TotalNeedExp = 0;
            bSelAll = false;
            EatPetIDList.Clear();

            UpdatePreAddedExp();
            UpdateSelPetInfo(false);

            RefreshPetItemListCount();

            if (mEatPet.isOn)
            {
                UpdateEatPetList();
                UpdateEatNeedMoney();
            }

            uint CritExp = (uint)iEvent.Param1;
            byte IsCritical = (byte)iEvent.Param2;

            if (CritExp > 0)
            {
                PlayCritEffect(CritExp, IsCritical);
            }

        }

        void OnUpdatePetBuyUI(UIEvent iEvent)
        {
            UpdateSelPetInfo();
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

                if (!bInit)
                {
                    if (OnUsePetItemList[i].id == CurSelPetInfo.id)
                    {
                        PetDataManager.GetInstance().SetPetData(CurSelPetInfo, OnUsePetItemList[i]);
                    }
                }
            }

            List<PetInfo> PacketPetItemList = PetDataManager.GetInstance().GetPetList();

            int iCount = 0;
            List<PetInfo> SortDataList = PetDataManager.GetInstance().GetPetSortListBySortType(PacketPetItemList, ref iCount);

            for (int i = 0; i < SortDataList.Count; i++)
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

        void OnSelectPet(int iIndex, bool value)
        {
            if (iIndex < 0 || !value)
            {
                return;
            }

            if (iIndex >= PetInfoList.Count)
            {
                return;
            }


            PetDataManager.GetInstance().SetPetData(CurSelPetInfo, PetInfoList[iIndex]);
            UpdateSelPetInfo();
            UpdateActor((int)CurSelPetInfo.dataId);

            if (mEatPet.isOn)
            {
                UpdateEatPetIDList();
                UpdateEatPetList();
                UpdateEatNeedMoney();
            }
        }


        void OnSelectEatPet(int iIndex, bool value, Toggle tgSelectPet)
        {
            if (iIndex < 0 || iIndex >= EatPetInfoList.Count)
            {
                return;
            }

            PetTable PetData = TableManager.GetInstance().GetTableItem<PetTable>((int)CurSelPetInfo.dataId);
            if (PetData == null)
            {
                return;
            }

            if (CurSelPetInfo.level >= PetData.MaxLv)
            {
                if (value)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("该宠物已满级,无法继续吞噬宠物");
                    tgSelectPet.isOn = false;
                }

                return;
            }

            bool bFind = false;
            for (int i = 0; i < EatPetIDList.Count; i++)
            {
                if (EatPetIDList[i].id == EatPetInfoList[iIndex].id)
                {
                    if (!value)
                    {
                        EatPetIDList.RemoveAt(i);
                        TotalNeedExp -= GetExpByEatPet(EatPetInfoList[iIndex]);
                    }

                    bFind = true;
                    break;
                }
            }

            if (!bFind)
            {
                if (value)
                {
                    int iPreAddedLv = GetAddedLv();

                    if ((CurSelPetInfo.level + iPreAddedLv) >= PetData.MaxLv)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("等级加成已达上限,无法继续吞噬宠物");
                        tgSelectPet.isOn = false;

                        return;
                    }
                    else
                    {
                        PetInfo petinfo = new PetInfo();
                        PetDataManager.GetInstance().SetPetData(petinfo, EatPetInfoList[iIndex]);

                        EatPetIDList.Add(petinfo);
                        TotalNeedExp += GetExpByEatPet(EatPetInfoList[iIndex]);
                    }
                }
            }

            UpdatePreAddedExp();
            UpdateEatNeedMoney();
        }


        void InitPetItemScrollListBind()
        {
            mPetList.Initialize();

            mPetList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    UpdatePetItemScrollListBind(item);
                }
            };

            mPetList.OnItemRecycle = (item) =>
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
                if (OnUsePetItemList[i].id == PetInfoList[item.m_index].id)
                {
                    Carry.gameObject.CustomActive(true);

                    bSHowCarry = true;
                    break;
                }
            }

            if (!bSHowCarry)
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
            mPetList.SetElementAmount(PetInfoList.Count);
        }

        void UpdateSelPetInfo(bool bForce = true)
        {
            PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)CurSelPetInfo.dataId);
            if (petData == null)
            {
                return;
            }

            mName.text = PetDataManager.GetInstance().GetColorName(petData.Name, petData.Quality);
            mLevel.text = string.Format("{0}/{1}", CurSelPetInfo.level, petData.MaxLv);
            

            DrawPetExpBar(CurSelPetInfo.level, CurSelPetInfo.exp, petData.Quality, bForce);

            List<int> Skillids = PetDataManager.GetInstance().GetPetSkillIDsByJob(petData, PlayerBaseData.GetInstance().JobTableID);

            if (Skillids.Count <= 0 || CurSelPetInfo.skillIndex >= Skillids.Count)
            {
                mLeftContent.text = "";
                mRightContent.text = "";
            }
            else
            {
                mLeftContent.text = SkillDataManager.GetInstance().UpdatePetSkillDescription(petData.InnateSkill, (byte)CurSelPetInfo.level, (byte)CurSelPetInfo.level);
                mLeftContent.text += " \n" + SkillDataManager.GetInstance().UpdatePetSkillDescription(Skillids[CurSelPetInfo.skillIndex], (byte)CurSelPetInfo.level, (byte)CurSelPetInfo.level);
                mRightContent.text = SkillDataManager.GetInstance().UpdatePetSkillDescription(petData.InnateSkill, (byte)CurSelPetInfo.level, (byte)(CurSelPetInfo.level + 1));
                mRightContent.text += " \n" + SkillDataManager.GetInstance().UpdatePetSkillDescription(Skillids[CurSelPetInfo.skillIndex], (byte)CurSelPetInfo.level, (byte)(CurSelPetInfo.level + 1));
            }

            int iGoldExp = 0;
            int iNeedGold = PetDataManager.GetInstance().GetFeedNeedMoney(CurSelPetInfo, PetFeedTable.eType.PET_FEED_GOLD, ref iGoldExp);

            if (iNeedGold < 0)
            {
                mGoldTrainCost.text = "0";
                mGoldTrainExp.text = "0";

                mGoldTrainGray.enabled = true;
                mGoldTrain.interactable = false;
            }
            else
            {
                if (CurSelPetInfo.goldFeedCount >= MaxGoldFeedNum)
                {
                    mGoldTrainExp.text = string.Format("<color=#9FA2B8FF>{0}</color>", iGoldExp);
                    mGoldTrainRestNum.text = string.Format("<color=#9FA2B8FF>{0}/{1}</color>", MaxGoldFeedNum - CurSelPetInfo.goldFeedCount, MaxGoldFeedNum);

                    mGoldTrainGray.enabled = true;
                    mGoldTrain.interactable = false;
                }
                else
                {
                    mGoldTrainExp.text = string.Format("<color=#E7BA78>{0}</color>", iGoldExp);
                    mGoldTrainRestNum.text = string.Format("<color=#E7BA78>{0}/{1}</color>", MaxGoldFeedNum - CurSelPetInfo.goldFeedCount, MaxGoldFeedNum);

                    mGoldTrainGray.enabled = false;
                    mGoldTrain.interactable = true;
                }

                if ((int)PlayerBaseData.GetInstance().Gold < iNeedGold)
                {
                    mGoldTrainCost.text = string.Format("<color=#FF0000FF>{0}</color>", iNeedGold);
                }
                else
                {
                    mGoldTrainCost.text = iNeedGold.ToString();
                }
            }

            int iTicketExp = 0;
            int iNeedTicket = PetDataManager.GetInstance().GetFeedNeedMoney(CurSelPetInfo, PetFeedTable.eType.PET_FEED_POINT, ref iTicketExp);

            if (iNeedTicket < 0)
            {
                mTicketTrainCost.text = "0";
                mTicketTrainExp.text = "0";

                mTicketTrainGray.enabled = true;
                mTicketTrain.interactable = false;
            }
            else
            {
                if (CurSelPetInfo.pointFeedCount >= MaxTicketFeedNum)
                {
                    mTicketTrainExp.text = string.Format("<color=#9FA2B8FF>{0}</color>", iTicketExp);
                    mTicketTrainRestNum.text = string.Format("<color=#9FA2B8FF>{0}/{1}</color>", MaxTicketFeedNum - CurSelPetInfo.pointFeedCount, MaxTicketFeedNum);

                    mTicketTrainGray.enabled = true;
                    mTicketTrain.interactable = false;
                }
                else
                {
                    mTicketTrainExp.text = string.Format("<color=#E7BA78>{0}</color>", iTicketExp);
                    mTicketTrainRestNum.text = string.Format("<color=#E7BA78>{0}/{1}</color>", MaxTicketFeedNum - CurSelPetInfo.pointFeedCount, MaxTicketFeedNum);

                    mTicketTrainGray.enabled = false;
                    mTicketTrain.interactable = true;
                }

                if ((int)PlayerBaseData.GetInstance().Ticket < iNeedTicket)
                {
                    mTicketTrainCost.text = string.Format("<color=#FF0000FF>{0}</color>", iNeedTicket);
                }
                else
                {
                    mTicketTrainCost.text = iNeedTicket.ToString();
                }
            }
        }

        void UpdateEatPetList()
        {
            EatPetInfoList.Clear();

            List<PetInfo> petList = PetDataManager.GetInstance().GetPetList();
            List<PetInfo> TempList = new List<PetInfo>();

            for (int i = 0; i < petList.Count; i++)
            {
                if (petList[i].id == CurSelPetInfo.id)
                {
                    continue;
                }

                PetInfo temp = new PetInfo();
                PetDataManager.GetInstance().SetPetData(temp, petList[i]);

                TempList.Add(temp);
            }

            int iAddedNum = TempList.Count - PetEleObjList.Count;

            for (int i = 0; i < iAddedNum; i++)
            {
                GameObject PetElement = AssetLoader.instance.LoadResAsGameObject(EatPetItemPath);
                if (PetElement == null)
                {
                    continue;
                }

                Utility.AttachTo(PetElement, mEatPetListObjRoot);
                PetElement.CustomActive(false);

                PetEleObjList.Add(PetElement);
            }

            int iCount = 0;
            EatPetInfoList = PetDataManager.GetInstance().GetPetSortListBySortType(TempList, ref iCount);

            for (int i = 0; i < PetEleObjList.Count; i++)
            {
                if (i < EatPetInfoList.Count)
                {
                    ComCommonBind combind = PetEleObjList[i].GetComponent<ComCommonBind>();
                    if (combind == null)
                    {
                        continue;
                    }

                    PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)EatPetInfoList[i].dataId);
                    if (petData == null)
                    {
                        continue;
                    }

                    PetDataManager.GetInstance().SetPetItemData(PetEleObjList[i].gameObject, EatPetInfoList[i], PlayerBaseData.GetInstance().JobTableID);

                    Toggle tgSelectPet = combind.GetCom<Toggle>("tgSelect");
                    tgSelectPet.onValueChanged.RemoveAllListeners();
                    int iIndex = i;
                    tgSelectPet.onValueChanged.AddListener((value) => { OnSelectEatPet(iIndex, value, tgSelectPet); });

                    if (bSelAll)
                    {
                        tgSelectPet.isOn = true;
                    }
                    else
                    {
                        bool bFind = false;
                        for (int j = 0; j < EatPetIDList.Count; j++)
                        {
                            if (EatPetIDList[j].id == EatPetInfoList[i].id)
                            {
                                tgSelectPet.isOn = true;
                                bFind = true;

                                break;
                            }
                        }

                        if (!bFind)
                        {
                            tgSelectPet.isOn = false;
                        }
                    }

                    PetEleObjList[i].CustomActive(true);
                }
                else
                {
                    PetEleObjList[i].CustomActive(false);
                }
            }

            if (petList.Count <= 0)
            {
                mNonePetRoot.CustomActive(true);
            }
            else
            {
                mNonePetRoot.CustomActive(false);
            }

        }


        void UpdateActor(int iPetID)
        {
            PetTable Pet = TableManager.instance.GetTableItem<PetTable>(iPetID);
            if (Pet == null)
            {
                Logger.LogErrorFormat("can not find PetTable with id:{0}", iPetID);
            }
            else
            {
                ResTable res = TableManager.instance.GetTableItem<ResTable>(Pet.ModeID);

                if (res == null)
                {
                    Logger.LogErrorFormat("can not find ResTable with id:{0}", Pet.ModeID);
                }
                else
                {
                    CommonUtility.LoadPetAvatarRenderEx(iPetID, mActorpos);

                    Vector3 avatarPos = mActorpos.avatarPos;
                    avatarPos.y = Pet.ChangedHeight / 1000.0f;
                    mActorpos.avatarPos = avatarPos;

                    Quaternion qua = mActorpos.avatarRoation;
                    mActorpos.avatarRoation = Quaternion.Euler(qua.x, Pet.ModelOrientation / 1000.0f, qua.z);

                    var vscale = mActorpos.avatarScale;

                    Vector3 avatarScale = mActorpos.avatarScale;
                    avatarScale.y = avatarScale.x = avatarScale.z = Pet.PetModelSize / 1000.0f;
                    mActorpos.avatarScale = avatarScale;
                }
            }
        }

        void DrawPetExpBar(int iLevel, UInt64 PetExp, PetTable.eQuality ePetQuality, bool force)
        {
            mExpBar.SetExp(PetExp, force, exp =>
            {
                return TableManager.instance.GetCurPetExpBar(iLevel, exp, ePetQuality);
            });
        }

        void UpdateEatNeedMoney()
        {
            if (PlayerBaseData.GetInstance().Gold < GetNeedTotalMoney())
            {
                mTotleEatCost.SafeSetText(string.Format("<color=#FF0000FF>{0}</color>", GetNeedTotalMoney()));
            }
            else
            {
                mTotleEatCost.SafeSetText(GetNeedTotalMoney().ToString());
            }
        }


        UInt64 GetNeedTotalMoney()
        {
            return (UInt64)(TotalNeedExp * (UInt64)MoneyChangeCoeffi / 100.0f);
        }

        void UpdateEatPetIDList()
        {
            EatPetIDList.Clear();
            TotalNeedExp = 0;

            UpdatePreAddedExp();
        }

        void UpdatePreAddedExp()
        {
            int iAddedLv = GetAddedLv();

            if (iAddedLv > 0)
            {
                mPreAddedLv.SafeSetText(string.Format("+{0}", iAddedLv));
            }
            else
            {
                mPreAddedLv.SafeSetText("");
            }

            PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)CurSelPetInfo.dataId);
            if (petData == null)
            {
                mPreAddedExp.SafeSetText("");
                return;
            }

            if (TotalNeedExp > 0 && CurSelPetInfo.level < petData.MaxLv)
            {
                mPreAddedExp.SafeSetText(string.Format("+{0}", TotalNeedExp / 100));
            }
            else
            {
                mPreAddedExp.SafeSetText("");
            }
        }

        void PlayCritEffect(uint CritExp, byte bIsCritical)
        {
            // 数字特效
            GameObject Numobj = AssetLoader.instance.LoadResAsGameObject(CritNumPath);
            if (Numobj == null)
            {
                return;
            }

            Utility.AttachTo(Numobj, ClientSystemManager.instance.MiddleLayer);

            ComCommonBind combind = Numobj.GetComponent<ComCommonBind>();

            GameObject CriticalRoot = combind.GetGameObject("CriticalRoot");
            Text Crit = combind.GetCom<Text>("Crit");
            Text NoCrit = combind.GetCom<Text>("NoCrit");

            if (bIsCritical == 1)
            {
                CriticalRoot.CustomActive(true);

                Crit.SafeSetText("＋" + CritExp);
                Crit.gameObject.CustomActive(true);

                NoCrit.gameObject.CustomActive(false);
            }
            else
            {
                CriticalRoot.CustomActive(false);
                Crit.gameObject.CustomActive(false);

                NoCrit.SafeSetText("＋" + CritExp);
                NoCrit.gameObject.CustomActive(true);
            }

            // 闪光特效
            GameObject obj = AssetLoader.instance.LoadResAsGameObject(CritEffectPath);
            if (obj == null)
            {
                return;
            }

            obj.name = "CritEffect";
            Utility.AttachTo(obj, mExp.gameObject);
        }


        UInt64 GetExpByEatPet(PetInfo pet)
        {
            UInt64 TotalExp = 0;
            UInt64 UpLvExp = 0;

            PetTable PetData = TableManager.GetInstance().GetTableItem<PetTable>((int)pet.dataId);
            if (PetData == null)
            {
                return 0;
            }

            TotalExp = (UInt64)PetData.ToDevourExp * 100;

            if (pet.level <= 1)
            {
                return TotalExp;
            }

            Dictionary<int, object> petLevelData = TableManager.GetInstance().GetTable<PetLevelTable>();
            if (petLevelData == null)
            {
                return TotalExp;
            }

            UpLvExp += pet.exp;

            var Enum = petLevelData.GetEnumerator();
            while (Enum.MoveNext())
            {
                var data = Enum.Current.Value as PetLevelTable;
                if (data == null)
                {
                    continue;
                }

                if (data.Quality != (int)PetData.Quality)
                {
                    continue;
                }

                if (data.Level < pet.level)
                {
                    UpLvExp += (UInt64)data.UplevelExp;
                }
            }

            TotalExp += (UInt64)(UpLvExp * (UInt64)ExpChangeCoeffi);

            return TotalExp;
        }

        int GetAddedLv()
        {
            PetTable PetData = TableManager.GetInstance().GetTableItem<PetTable>((int)CurSelPetInfo.dataId);
            if (PetData == null)
            {
                return 0;
            }

            UInt64 PreTotalExp = TotalNeedExp / 100 + CurSelPetInfo.exp;

            var TotalData = TableManager.GetInstance().GetTable<PetLevelTable>();
            var enumer = TotalData.GetEnumerator();

            UInt64 CurLvTotalExp = 0;
            int iAddedLv = 0;
            bool bIsMaxLv = true;

            while (enumer.MoveNext())
            {
                PetLevelTable data = enumer.Current.Value as PetLevelTable;
                if (data == null)
                {
                    continue;
                }

                if (data.Quality != (int)PetData.Quality)
                {
                    continue;
                }

                if (data.Level < CurSelPetInfo.level)
                {
                    continue;
                }

                CurLvTotalExp += (UInt64)data.UplevelExp;

                if (CurLvTotalExp <= PreTotalExp)
                {
                    continue;
                }

                iAddedLv = data.Level - CurSelPetInfo.level;
                bIsMaxLv = false;

                break;
            }

            if (bIsMaxLv)
            {
                iAddedLv = PetData.MaxLv - CurSelPetInfo.level;
            }

            return iAddedLv;
        }

        void SendEatPetReq()
        {
            SceneDevourPetReq req = new SceneDevourPetReq();

            req.id = CurSelPetInfo.id;
            req.petIds = new UInt64[EatPetIDList.Count];

            for (int i = 0; i < EatPetIDList.Count; i++)
            {
                req.petIds[i] = EatPetIDList[i].id;
            }

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }


        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (null != mActorpos)
            {
                while (global::Global.Settings.avatarLightDir.x > 360)
                    global::Global.Settings.avatarLightDir.x -= 360;
                while (global::Global.Settings.avatarLightDir.x < 0)
                    global::Global.Settings.avatarLightDir.x += 360;

                while (global::Global.Settings.avatarLightDir.y > 360)
                    global::Global.Settings.avatarLightDir.y -= 360;
                while (global::Global.Settings.avatarLightDir.y < 0)
                    global::Global.Settings.avatarLightDir.y += 360;

                while (global::Global.Settings.avatarLightDir.z > 360)
                    global::Global.Settings.avatarLightDir.z -= 360;
                while (global::Global.Settings.avatarLightDir.z < 0)
                    global::Global.Settings.avatarLightDir.z += 360;

                mActorpos.m_LightRot = global::Global.Settings.avatarLightDir;
            }
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            ClearData();
        }



        ////////////////////////////////////////////////////////////////////////////////
        #region ExtraUIBind
        private Button mClose = null;
        private ComUIListScript mPetList = null;
        private GeAvatarRendererEx mActorpos = null;
        private Text mName = null;
        private Text mLevel = null;
        private ComExpBar mExpBar = null;
        private Text mLeftContent = null;
        private Text mRightContent = null;
        private Text mExp = null;
        private Text mRestNum = null;
        private Text mNexExp = null;
        private RectTransform mNextRestNum = null;
        private Button mGoldTrain = null;
        private Button mTicketTrain = null;
        private Text mTotleEatCost = null;
        private Toggle mPet = null;
        private Toggle mEatPet = null;
        private GameObject mMiddleObjRoot = null;
        private GameObject mDownObjRoot = null;
        private GameObject mEatPetRoot = null;
        private Text mGoldTrainCost = null;
        private Text mTicketTrainCost = null;
        private UIGray mGoldTrainGray = null;
        private UIGray mTicketTrainGray = null;
        private Text mGoldTrainExp = null;
        private Text mGoldTrainRestNum = null;
        private Text mTicketTrainExp = null;
        private Text mTicketTrainRestNum = null;
        private GameObject mEatPetListObjRoot = null;
        private GameObject mNonePetRoot = null;
        private Text mPreAddedLv = null;
        private Text mPreAddedExp = null;
        private Button mPetWay = null;
        private Button mEat = null;
        private Button mOneKeySel = null;


        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mPetList = mBind.GetCom<ComUIListScript>("petList");
            mActorpos = mBind.GetCom<GeAvatarRendererEx>("actorpos");
            mName = mBind.GetCom<Text>("Name");
            mLevel = mBind.GetCom<Text>("level");
            mExpBar = mBind.GetCom<ComExpBar>("expBar");
            mLeftContent = mBind.GetCom<Text>("leftContent");
            mRightContent = mBind.GetCom<Text>("rightContent");
            mExp = mBind.GetCom<Text>("exp");
            mRestNum = mBind.GetCom<Text>("RestNum");
            mNexExp = mBind.GetCom<Text>("nexExp");
            mNextRestNum = mBind.GetCom<RectTransform>("nextRestNum");
            mGoldTrain = mBind.GetCom<Button>("GoldTrain");
            mGoldTrain.onClick.AddListener(_onGoldTrainButtonClick);
            mTicketTrain = mBind.GetCom<Button>("TicketTrain");
            mTicketTrain.onClick.AddListener(_onTicketTrainButtonClick);
            mTotleEatCost = mBind.GetCom<Text>("totleEatCost");
            mPet = mBind.GetCom<Toggle>("Pet");
            mPet.onValueChanged.AddListener(_onPetToggleValueChange);
            mEatPet = mBind.GetCom<Toggle>("EatPet");
            mEatPet.onValueChanged.AddListener(_onEatPetToggleValueChange);
            mMiddleObjRoot = mBind.GetGameObject("MiddleObjRoot");
            mDownObjRoot = mBind.GetGameObject("DownObjRoot");
            mEatPetRoot = mBind.GetGameObject("EatPetRoot");
            mGoldTrainCost = mBind.GetCom<Text>("GoldTrainCost");
            mTicketTrainCost = mBind.GetCom<Text>("TicketTrainCost");
            mGoldTrainGray = mBind.GetCom<UIGray>("GoldTrainGray");
            mTicketTrainGray = mBind.GetCom<UIGray>("TicketTrainGray");
            mGoldTrainExp = mBind.GetCom<Text>("GoldTrainExp");
            mGoldTrainRestNum = mBind.GetCom<Text>("GoldTrainRestNum");
            mTicketTrainExp = mBind.GetCom<Text>("TicketTrainExp");
            mTicketTrainRestNum = mBind.GetCom<Text>("TicketTrainRestNum");
            mEatPetListObjRoot = mBind.GetGameObject("EatPetListObjRoot");
            mNonePetRoot = mBind.GetGameObject("NonePetRoot");
            mPreAddedLv = mBind.GetCom<Text>("PreAddedLv");
            mPreAddedExp = mBind.GetCom<Text>("PreAddedExp");
            mPetWay = mBind.GetCom<Button>("PetWay");
            mPetWay.onClick.AddListener(_onPetWayButtonClick);
            mEat = mBind.GetCom<Button>("Eat");
            mEat.onClick.AddListener(_onEatButtonClick);
            mOneKeySel = mBind.GetCom<Button>("OneKeySel");
            mOneKeySel.onClick.AddListener(_onOneKeySelButtonClick);
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mPetList = null;
            mActorpos = null;
            mName = null;
            mLevel = null;
            mExpBar = null;
            mLeftContent = null;
            mRightContent = null;
            mExp = null;
            mRestNum = null;
            mNexExp = null;
            mNextRestNum = null;
            mGoldTrain.onClick.RemoveListener(_onGoldTrainButtonClick);
            mGoldTrain = null;
            mTicketTrain.onClick.RemoveListener(_onTicketTrainButtonClick);
            mTicketTrain = null;
            mTotleEatCost = null;
            mPet.onValueChanged.RemoveListener(_onPetToggleValueChange);
            mPet = null;
            mEatPet.onValueChanged.RemoveListener(_onEatPetToggleValueChange);
            mEatPet = null;
            mMiddleObjRoot = null;
            mDownObjRoot = null;
            mEatPetRoot = null;
            mGoldTrainCost = null;
            mTicketTrainCost = null;
            mGoldTrainGray = null;
            mTicketTrainGray = null;
            mGoldTrainExp = null;
            mGoldTrainRestNum = null;
            mTicketTrainExp = null;
            mTicketTrainRestNum = null;
            mEatPetListObjRoot = null;
            mNonePetRoot = null;
            mPreAddedLv = null;
            mPreAddedExp = null;
            mPetWay.onClick.RemoveListener(_onPetWayButtonClick);
            mPetWay = null;
            mEat.onClick.RemoveListener(_onEatButtonClick);
            mEat = null;
            mOneKeySel.onClick.RemoveListener(_onOneKeySelButtonClick);
            mOneKeySel = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            Close();

        }
        private void _onGoldTrainButtonClick()
        {
            if (CurSelPetInfo.goldFeedCount >= MaxGoldFeedNum)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("今日金币喂养次数已用完");
                return;
            }

            int iExp = 0;

            int iNeedMoney = PetDataManager.GetInstance().GetFeedNeedMoney(CurSelPetInfo, PetFeedTable.eType.PET_FEED_GOLD, ref iExp);
            if (iNeedMoney < 0)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("所需金币计算错误");
                return;
            }

            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

            costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.GOLD);
            costInfo.nCount = iNeedMoney;

            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                PetDataManager.GetInstance().SendFeedPetReq(PetFeedTable.eType.PET_FEED_GOLD, CurSelPetInfo.id);
            });

        }
        private void _onTicketTrainButtonClick()
        {
            if (CurSelPetInfo.pointFeedCount >= MaxTicketFeedNum)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("今日点券喂养次数已用完");
                return;
            }

            int iExp = 0;

            int iNeedMoney = PetDataManager.GetInstance().GetFeedNeedMoney(CurSelPetInfo, PetFeedTable.eType.PET_FEED_POINT, ref iExp);
            if (iNeedMoney < 0)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("所需点券计算错误");
                return;
            }

            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }
            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

            costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
            costInfo.nCount = iNeedMoney;

            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                PetDataManager.GetInstance().SendFeedPetReq(PetFeedTable.eType.PET_FEED_POINT, CurSelPetInfo.id);
            });

        }
        private void _onPetToggleValueChange(bool changed)
        {
            if (!changed)
            {
                return;
            }

            mMiddleObjRoot.CustomActive(true);
            mDownObjRoot.CustomActive(true);
            mEatPetRoot.CustomActive(false);

            TotalNeedExp = 0;
            bSelAll = false;
            EatPetIDList.Clear();

        }
        private void _onEatPetToggleValueChange(bool changed)
        {
            if (!changed)
            {
                return;
            }

            mMiddleObjRoot.CustomActive(false);
            mDownObjRoot.CustomActive(false);
            mEatPetRoot.CustomActive(true);

            UpdateEatPetList();
            UpdateEatNeedMoney();

        }

        private void _onPetWayButtonClick()
        {
            ItemComeLink.OnLink(102, 0);

        }
        private void _onEatButtonClick()
        {
            if (EatPetIDList.Count <= 0)
            {
                return;
            }

            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }
            if (PlayerBaseData.GetInstance().Gold < GetNeedTotalMoney())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("吞噬宠物所需金币不足");
                return;
            }

            bool bFind = false;
            for (int i = 0; i < EatPetIDList.Count; i++)
            {
                PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)EatPetIDList[i].dataId);
                if (petData == null)
                {
                    continue;
                }

                if (petData.Quality >= PetTable.eQuality.QL_PINK)
                {
                    bFind = true;
                    break;
                }
            }

            if (bFind)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel("吞噬的宠物中包含粉色品质，是否确认吞噬？", SendEatPetReq);
                return;
            }

            SendEatPetReq();

        }
        private void _onOneKeySelButtonClick()
        {
            bSelAll = !bSelAll;
            UpdateEatPetList();

        }
        #endregion
    }
}
