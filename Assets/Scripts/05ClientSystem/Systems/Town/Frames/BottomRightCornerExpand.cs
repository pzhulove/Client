using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    // 右下角扩展
    internal class BottomRightCornerExpand : MonoBehaviour
    {
        [SerializeField]
        Button Achievement = null;

        [SerializeField]
        Button AdventureTeam = null;

        [SerializeField]
        Button EquipHandBook = null;

        [SerializeField]
        Button MagicCard = null;

        [SerializeField]
        Button fashionMerge = null;

        [SerializeField]
        Button equipRecovery = null;

        [SerializeField]
        Button WeaponLease = null;

        [SerializeField]
        GameObject mAdventureTeamRedPoint = null;

        [SerializeField]
        GameObject mAdventureTeamRoot = null;

        [SerializeField]
        private Button mBtEquipForge = null;

        [SerializeField]
        private Image mEquipForgeRedPoint = null;

        [SerializeField]
        private Button PVETrainDamageBtn = null;

        [SerializeField]
        private Button btnTitleBook = null;

        [SerializeField]
        private Button btnMeterials = null; 

        private void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamFuncChanged, _OnAdventureTeamFuncChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnFashionMergeSwich, _OnFashionMergeSwitchFunc);

            AdventureTeam.SafeSetOnClickListener(() => 
            {
                AdventureTeamDataManager.GetInstance().OpenAdventureTeamInfoFrame();
                GameStatisticManager.GetInstance().DoStartUIButton("AdventureTeam_1");
            });

            EquipHandBook.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.instance.OpenFrame<EquipHandbookFrame>(FrameLayer.Middle);
                GameStatisticManager.GetInstance().DoStartUIButton("EquipHandBook_1");
            });

            MagicCard.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.GetInstance().OpenFrame<EnchantmentsBookFrame>();
                GameStatisticManager.GetInstance().DoStartUIButton("MagicCardBook_1");
            });

            fashionMerge.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.GetInstance().CloseFrame<FashionMergeNewFrame>();
                FashionMergeNewFrame.OpenLinkFrame(string.Format("1|0|{0}|{1}|{2}", (int)FashionMergeManager.GetInstance().FashionType, (int)FashionMergeManager.GetInstance().FashionPart, 0));           
                GameStatisticManager.GetInstance().DoStartUIButton("FashionMerge_1");
            });

            equipRecovery.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.GetInstance().OpenFrame<EquipRecoveryFrame>();
                GameStatisticManager.GetInstance().DoStartUIButton("EquipRecovery_1");
            });

            WeaponLease.SafeSetOnClickListener(() => 
            {
                WeaponLeaseShopFrame.OpenLinkFrame("26");    
                GameStatisticManager.GetInstance().DoStartUIButton("WeaponLease_1");
            });

            mBtEquipForge.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.instance.OpenFrame<EquipForgeFrame>(FrameLayer.Middle);
                GameStatisticManager.GetInstance().DoStartUIButton("EquipForge");
            });      

            PVETrainDamageBtn.SafeSetOnClickListener(() => 
            {
                SkillDamageFrame frame = ClientSystemManager.instance.OpenFrame<SkillDamageFrame>(FrameLayer.Middle) as SkillDamageFrame;
                if (frame != null)
                {
                    frame.InitData(true);
                }
                GameStatisticManager.GetInstance().DoStartUIButton("PVETrainDamage_1");
            });

            btnTitleBook.SafeSetOnClickListener(() =>
            {
                ClientSystemManager.GetInstance().OpenFrame<TitleBookFrame>();
                GameStatisticManager.GetInstance().DoStartUIButton("TitleBook");
            });

            btnMeterials.SafeSetOnClickListener(() =>
            {
                ClientSystemManager.GetInstance().OpenFrame<MaterialSynthesisFrame>();
                GameStatisticManager.GetInstance().DoStartUIButton("Meterials");
            });
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamFuncChanged, _OnAdventureTeamFuncChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnFashionMergeSwich, _OnFashionMergeSwitchFunc);
        }

        private void Start()
        {
            InitRedPoint();
            _InitAdventureTeamShow();
            UpdateFashionMergeBtnState();
        }

        void OnRedPointChanged(UIEvent a_event)
        {
            ERedPoint redPointType = (ERedPoint)a_event.Param1;
            if (redPointType == ERedPoint.AdventureTeam)
            {
                mAdventureTeamRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.AdventureTeam));
            }
            else if (redPointType == ERedPoint.EquipForge)
            {
                mEquipForgeRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.EquipForge));
            }
        }

        void InitRedPoint()
        {            
            mAdventureTeamRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.AdventureTeam));
            mEquipForgeRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.EquipForge));
        }

        void _InitAdventureTeamShow()
        {
            bool bFuncOn = AdventureTeamDataManager.GetInstance().BFuncOpened;
            mAdventureTeamRoot.CustomActive(bFuncOn);
        }

        void _UpdateAdventureTeamShow()
        {
            bool bFuncOn = AdventureTeamDataManager.GetInstance().BFuncOpened;
            mAdventureTeamRoot.CustomActive(bFuncOn);
        }

        void _OnAdventureTeamFuncChanged(UIEvent uiEvent)
        {
            _UpdateAdventureTeamShow();
        }

        private void _OnFashionMergeSwitchFunc(UIEvent uiEvent)
        {
            UpdateFashionMergeBtnState();
        }

        void UpdateFashionMergeBtnState()
        {
            bool isFlag = !ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_FASHION_MERGO) && Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.FashionMerge);
            if (fashionMerge)
            {
                fashionMerge.CustomActive(isFlag);
            }
        }
    }
}
