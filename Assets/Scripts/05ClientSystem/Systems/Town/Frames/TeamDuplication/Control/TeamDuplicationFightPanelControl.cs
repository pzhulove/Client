using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;

namespace GameClient
{

    //攻坚面板控制器，控制面板的创建和FadeInOut动画
    public class TeamDuplicationFightPanelControl : MonoBehaviour
    {

        [Space(15)]
        [HeaderAttribute("FightPanel")]
        [Space(5)]
        [SerializeField] private Button fightPanelShowButton;
        
        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
        }

        private void BindEvents()
        {
            if (fightPanelShowButton != null)
            {
                fightPanelShowButton.onClick.RemoveAllListeners();
                fightPanelShowButton.onClick.AddListener(OnFightPanelShowButtonClick);
            }
        }

        private void UnBindEvents()
        {
            if (fightPanelShowButton != null)
                fightPanelShowButton.onClick.RemoveAllListeners();
        }

        public void InitFightPanelControl()
        {
        }
        
        #region FightPanelButton

        private void OnFightPanelShowButtonClick()
        {
            OnFightPanelShow();
        }

        public void OnFightPanelShow()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationFightStagePanelFrame(TeamDuplicationDataManager.GetInstance()
                .TeamDuplicationFightStageId);
        }

        //攻坚按钮和攻坚关闭按钮的更新
        public void UpdateFightPanelShowButton(bool flag)
        {
            CommonUtility.UpdateButtonVisible(fightPanelShowButton, flag);
        }

        #endregion

    }
}
