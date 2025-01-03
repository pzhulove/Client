using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class SkillTalentFrame : ClientFrame
    {
        [SerializeField] private SkillTalentFrameView mView;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Skill/SkillTalentFrame";
        }
        protected override void _OnOpenFrame()
        {
            _BindUIEvent();
            if(null != userData && userData is int)
            {
                int skillId = (int)userData;
                mView.OnInit(skillId);
            }
        }
        protected override void _OnCloseFrame()
        {
            _UnBindUIEvent();
            mView.OnUninit();
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SkillLearnedLevelChanged, _OnSkillLearnedChanged);
            // UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CloseSkillConfigFrame, _OnCloseSkillConfigFrame);
        }

        private void _OnSkillLearnedChanged(UIEvent uiEvent)
        {
            if (null != mView)
                mView.OnTalentChanged();
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SkillLearnedLevelChanged, _OnSkillLearnedChanged);
            // UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CloseSkillConfigFrame, _OnCloseSkillConfigFrame);
        }
        protected override void _bindExUI()
        {
            mView = mBind.GetCom<SkillTalentFrameView>("View");
        }
        protected override void _unbindExUI()
        {
            mView = null;
        }
    }
}
