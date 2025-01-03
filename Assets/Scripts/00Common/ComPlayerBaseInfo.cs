using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class ComPlayerBaseInfo : MonoBehaviour
    {
        [SerializeField]
        Image headIcon = null;

        [SerializeField]
        Text playerName = null;

        [SerializeField]
        Text playerLv = null;

        [SerializeField]
        Text tiliText = null;

        [SerializeField]
        Text dianjuanText = null;       

        [SerializeField]
        Button addTili = null;

        [SerializeField]
        Button btnHeadIcon = null;       

        [SerializeField]
        ReplaceHeadPortraitFrame headFrame = null;

        [SerializeField]
        Button btBufsShow = null;

        private void Awake()
        {
            
        }

        private void Start()
        {
            addTili.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.instance.OpenFrame<ComsumeFatigueAddFrame>();
            });

            btnHeadIcon.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.instance.OpenFrame<SettingFrame>(FrameLayer.Middle);               
       
                GameStatisticManager.GetInstance().DoStartUIButton("HeadIcon");
            });

            btBufsShow.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.instance.OpenFrame<TownBufsShowFrame>();
            });
           

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NameChanged, _OnNameChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JobIDChanged, _OnJobIDChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FatigueChanged, _OnFagureChanged);       
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UseHeadPortraitFrameSuccess, _OnUpdateHeadPortraitFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HeadPortraitFrameChange, _OnHeadPortraitFrameChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SystemChanged, _OnSystemChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateTownBuf, _OnUpdateTownBuf);

            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;

            UpdateUI();
        }   

        private void OnDestroy()
        {
            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NameChanged, _OnNameChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JobIDChanged, _OnJobIDChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FatigueChanged, _OnFagureChanged);    
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UseHeadPortraitFrameSuccess, _OnUpdateHeadPortraitFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HeadPortraitFrameChange, _OnHeadPortraitFrameChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SystemChanged, _OnSystemChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateTownBuf, _OnUpdateTownBuf);
        }   

        private void OnMoneyChanged(PlayerBaseData.MoneyBinderType eTarget)
        {
            if(eTarget != PlayerBaseData.MoneyBinderType.MBT_POINT)
            {
                return;
            }

            UpdateUI();
        }

        void _OnFagureChanged(UIEvent uiEvent)
        {
            UpdateUI();
        }

        void _OnJobIDChanged(UIEvent iEvent)
        {
            UpdateUI();
        }

        void _OnNameChanged(UIEvent a_event)
        {
            UpdateUI();
        }

        void OnLevelChanged(int iPreLv, int iCurLv)
        {
            UpdateUI();
        }

        void UpdateTownBufBtnState()
        {
            bool hasTownBuf = false;
            btBufsShow.CustomActive(hasTownBuf || 
                (ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null && ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.GetCurUseFatigueData() != null));
        }

        void UpdateUI()
        {
            string path = "";

            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobData != null)
            {
                ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                if (resData != null)
                {
                    path = resData.IconPath;
                }
            }               

            headIcon.SafeSetImage(path);

            playerName.SafeSetText(PlayerBaseData.GetInstance().Name);
            playerLv.SafeSetText(string.Format("Lv.{0}", PlayerBaseData.GetInstance().Level.ToString()));
            tiliText.SafeSetText(string.Format("{0}/{1}",PlayerBaseData.GetInstance().fatigue,PlayerBaseData.GetInstance().MaxFatigue));
            dianjuanText.SafeSetText(Utility.ToThousandsSeparator(PlayerBaseData.GetInstance().Ticket));

            UpdatePlayerHeadPortraitFrame();

            UpdateTownBufBtnState();
        }

        void _OnSystemChanged(UIEvent a_event)
        {
            var system = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if(system == null)
            {
                return;
            }

            //头像框请求
            HeadPortraitFrameDataManager.GetInstance().OnSendSceneHeadFrameReq();
        }

        private void _OnUpdateTownBuf(UIEvent uiEvent)
        {
            UpdateTownBufBtnState();
        }

        private void _OnUpdateHeadPortraitFrame(UIEvent uiEvent)
        {
            UpdatePlayerHeadPortraitFrame();
        }

        private void _OnHeadPortraitFrameChanged(UIEvent uiEvent)
        {
            UpdatePlayerHeadPortraitFrame();
        }

        void UpdatePlayerHeadPortraitFrame()
        {
            if (headFrame != null)
            {
                if (HeadPortraitFrameDataManager.WearHeadPortraitFrameID != 0)
                {
                    headFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.WearHeadPortraitFrameID);
                }
                else
                {
                    headFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                }
            }
        }
    }
}