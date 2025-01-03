using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class HonorSystemView : MonoBehaviour
    {

        
        [Space(10)]
        [HeaderAttribute("Title")]
        [Space(10)]
        [SerializeField] private Text titleLabel = null;

        [Space(10)] [HeaderAttribute("ContentController")] [Space(10)]
        [SerializeField] private HonorCommonController honorCommonController;

        [SerializeField] private HonorTotalController honorTotalController;
        [SerializeField] private HonorPreHistoryController honorPreHistoryController;


        [Space(10)]
        [HeaderAttribute("Button")]
        [Space(10)]
        [SerializeField] private Button closeButton;

        
        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveHonorSystemResMessage,
                OnReceiveHonorSystemResMessage);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess,
                OnReceiveItemUseSucceedMessage);
        }

        private void UnBindEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveHonorSystemResMessage,
                OnReceiveHonorSystemResMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess,
                OnReceiveItemUseSucceedMessage);
        }

        private void ClearData()
        {
        }

        public void InitView()
        {
            //发送荣誉消息的请求
            HonorSystemDataManager.GetInstance().OnSendSceneHonorReq();

            InitBaseView();
        }

        private void InitBaseView()
        {
            if (titleLabel != null)
                titleLabel.text = TR.Value("Honor_System_Title");
        }
        

        #region UIEvent
        //收到荣誉消息的返回
        private void OnReceiveHonorSystemResMessage(UIEvent uiEvent)
        {
            InitHonorSystemController();
        }

        private void OnReceiveItemUseSucceedMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var useItemData = (ItemData) uiEvent.Param1;
            if (useItemData == null
                || useItemData.TableID <= 0)
                return;

            //使用了一个荣誉保护卡
            if (useItemData.TableID == HonorSystemDataManager.NormalHonorProtectCardId
                || useItemData.TableID == HonorSystemDataManager.HighHonorProtectCardId)
            {
                //设置标志
                HonorSystemDataManager.GetInstance().IsAlreadyUseProtectCard = true;
                UpdateHonorSystemProtectCardUseContent();
            }
        }



        //初始化荣誉消息的控制器
        private void InitHonorSystemController()
        {
            if (honorCommonController != null)
                honorCommonController.InitHonorCommonController();

            if (honorTotalController != null)
                honorTotalController.InitHonorTotalController();

            if (honorPreHistoryController != null)
                honorPreHistoryController.InitHonorPreHistoryController();
        }

        //荣誉系统保护卡使用情况
        private void UpdateHonorSystemProtectCardUseContent()
        {
            if(honorCommonController != null)
                honorCommonController.UpdateProtectCardUsedContent();
        }

        #endregion

        private void OnCloseFrame()
        {
            HonorSystemUtility.OnCloseHonorSystemFrame();
        }
    }
}