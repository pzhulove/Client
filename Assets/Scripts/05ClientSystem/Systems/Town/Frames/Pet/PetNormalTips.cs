using UnityEngine.UI;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
using System.Collections.Generic;

namespace GameClient
{
    public class PetNormalTips : ClientFrame
    {
        private PetInfo petinfo;
        private int PlayerJobID;

        public void ClearData()
        {
            petinfo = null;
            PlayerJobID = 0;
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pet/PetNormalTips";
        }

        protected override void _OnOpenFrame()
        {
            if (userData == null)
            {
                return;
            }
            petinfo = userData as PetInfo;
            PetDataManager.GetInstance().SetPetData(petinfo, userData as PetInfo);
            PlayerJobID = PlayerBaseData.GetInstance().JobTableID;
            InitInterface();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetItemsInfoUpdate, _OnUpdatePetList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetSlotChanged, _OnPetSlotChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetItemsInfoUpdate, _OnPetSelectChanged);
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetItemsInfoUpdate, _OnUpdatePetList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetSlotChanged, _OnPetSlotChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetItemsInfoUpdate, _OnPetSelectChanged);
            ClearData();
        }

        void InitInterface()
        {
            RefreshInfo();
        }

        void _OnPetSelectChanged(UIEvent uiEvent)
        {
            RefreshInfo();
        }

        void _OnUpdatePetList(UIEvent uiEvent)
        {
            RefreshInfo();
        }

        void _OnPetSlotChanged(UIEvent uiEvent)
        {
            RefreshInfo();
        }

        public void RefreshInfo()
        {
            PetDataManager.GetInstance().SetPetData(petinfo, PetDataManager.GetInstance().SelectPetInfo);
            if (petinfo == null)
            {
                return;
            }
            mScoreTitle.SafeSetText("宠物评分:"+petinfo.petScore.ToString());

            PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)petinfo.dataId);
            if (petData == null)
            {
                return;
            }

            if (petinfo.skillIndex <= petData.Skills.Count)
            {
                mProperty.text = PetDataManager.GetInstance().GetPetPropertyTips(petData, petinfo.level);
                mCurAtt.text = PetDataManager.GetInstance().GetPetCurSkillTips(petData, PlayerJobID, petinfo.skillIndex, petinfo.level,false);
                mCanChooseAtt.text = PetDataManager.GetInstance().GetCanSelectSkillTips(petData, PlayerJobID, petinfo.skillIndex, petinfo.level,false);
            }
        }

        #region ExtraUIBind
        private Text mProperty = null;
        private Text mCurAtt = null;
        private Text mCanChooseAtt = null;
        private Text mScoreTitle = null;

        protected override void _bindExUI()
        {
            mProperty = mBind.GetCom<Text>("Property");
            mCurAtt = mBind.GetCom<Text>("CurAtt");
            mCanChooseAtt = mBind.GetCom<Text>("CanChooseAtt");
            mScoreTitle = mBind.GetCom<Text>("ScoreTitle");
        }

        protected override void _unbindExUI()
        {
            mProperty = null;
            mCurAtt = null;
            mCanChooseAtt = null;
            mScoreTitle = null;
        }
        #endregion
    }
}
