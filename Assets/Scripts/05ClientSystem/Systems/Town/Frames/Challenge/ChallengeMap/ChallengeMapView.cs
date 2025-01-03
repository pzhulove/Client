using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public delegate void OnContentEffectAction(bool flag);

    public class ChallengeMapView : MonoBehaviour
    {

        private ChallengeMapParamDataModel _challengeParamDataModel;
        
        [Space(10)]
        [HeaderAttribute("Close")]
        [Space(10)]
        [SerializeField]
        private Button closeButton;

        [Space(30)] [HeaderAttribute("ModelControl")] [Space(20)] [SerializeField]
        private ChallengeMapModelControl modelControl;
        [SerializeField] private ChallengeMapContentControl contentControl;

        [Space(30)] [HeaderAttribute("Title")] [Space(30)] [SerializeField]
        private GameObject titleRoot;
        [SerializeField] private ChallengeMapMoneyControl challengeMapMoneyControl;


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
        }

        private void UnBindEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _challengeParamDataModel = null;
        }

        public void InitView(ChallengeMapParamDataModel paramDataModel)
        {
            // marked by ckm
            // var defaultModelType = DungeonModelTable.eType.DeepModel;
            var defaultModelType = DungeonModelTable.eType.YunShangChangAnModel;
            if (paramDataModel != null && paramDataModel.ModelType != DungeonModelTable.eType.Type_None)
            {
                defaultModelType = paramDataModel.ModelType;
            }

            _challengeParamDataModel = paramDataModel;

            if (contentControl != null)
                contentControl.InitMapContentData(OnContentEffectAction);

            if (modelControl != null)
                modelControl.InitMapModelControl(defaultModelType, OnChallengeMapToggleClick);
        }

        private void OnChallengeMapToggleClick(DungeonModelTable.eType modelType)
        {
            UpdateTitleAndModelControl(true);

            if (challengeMapMoneyControl != null)
                challengeMapMoneyControl.Init(modelType);

            if (contentControl != null)
                contentControl.InitMapContentControl(modelType, _challengeParamDataModel);

            _challengeParamDataModel = null;
        }

        /// <summary>
        /// 点击具体的章节小地图进入关卡选择界面时触发的事件（主要用来隐藏title栏和页签栏）
        /// </summary>
        /// <param name="flag"></param>
        private void OnContentEffectAction(bool flag)
        {
            UpdateTitleAndModelControl(flag);
        }

        private void UpdateTitleAndModelControl(bool flag)
        {
            if (modelControl != null)
                modelControl.gameObject.CustomActive(flag);

            if (titleRoot != null)
                titleRoot.gameObject.CustomActive(flag);
        }
        
        private void OnCloseFrame()
        {
            ChallengeUtility.OnCloseChallengeMapFrame();
        }
   
    }
}
