using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;

namespace GameClient
{
    public class SkillConfigurationFrameView : MonoBehaviour
    {
        [SerializeField] private ComActiveAwakeSkillSlot mAwakeSkill;
        [SerializeField] private Drop_Me mDelectDropMe;
        public void OnInit()
        {
            mAwakeSkill.Show();
            mDelectDropMe.ResponseDrop = SkillDataManager.GetInstance().DealDeleteDrop;
        }

        public void OnUninit()
        {
            mDelectDropMe.ResponseDrop = null;
        }

        //初始化
        public void OnClickInitSkill()
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = TR.Value("skill_new_initialize_config"),
                IsShowNotify = false,
                LeftButtonText = TR.Value("skill_new_enter_skill_cancel"),
                RightButtonText = TR.Value("skill_new_enter_skill_sure"),
                OnRightButtonClickCallBack = EquipInitSkillConfig,
            };
            if (!SkillDataManager.GetInstance().IsHaveSetFairDueSkillBar)
            {
                SkillDataManager.GetInstance().SendSetSkillConfigReq(1);
            }
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }
        private void EquipInitSkillConfig()
        {
            SkillDataManager.GetInstance().OnSendSceneInitSkillsReqByCurType();
        }

        //推荐配置
        public void OnClickRecommendSkill()
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = TR.Value("skill_new_recommend_config_make_sure"),
                IsShowNotify = false,
                LeftButtonText = TR.Value("skill_new_enter_skill_cancel"),
                RightButtonText = TR.Value("skill_new_enter_skill_sure"),
                OnRightButtonClickCallBack = EquipRecommendSkillConfig,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }
        private void EquipRecommendSkillConfig()
        {
            SkillDataManager.GetInstance().OnSendRecommendSkillsReqByCurType();
        }

        //完成配置
        public void OnClickFinish()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.CloseSkillConfigFrame);
            ClientSystemManager.GetInstance().CloseFrame<SkillConfigurationFrame>();
        }
    }
}
