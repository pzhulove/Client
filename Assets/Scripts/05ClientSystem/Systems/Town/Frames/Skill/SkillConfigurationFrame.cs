using System;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ProtoTable;

namespace GameClient
{
    public class SkillConfigurationFrame : ClientFrame
    {
        public static int UnLockSkillSlotNum = 0;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Skill/SkillConfigurationFrame";
        }

        protected override void _OnOpenFrame()
        {
            //公平竞技场固定12个位置 不需要解锁任务
            if (SkillFrame.frameParam.frameType == SkillFrameType.FairDuel)
            {
                UnLockSkillSlotNum = Utility.GetClientIntValue(ClientConstValueTable.eKey.SKILL_CONFIG_SET_COUNT, 12);
            }
            else
            {
                //没有完成60级任务则读取上一级
                var lv = PlayerBaseData.GetInstance().Level;
                if (lv >= SkillDataManager.GetInstance().UnLockTaskLvl && !SkillDataManager.GetInstance().IsFinishUnlockTask && SkillFrame.frameParam.frameType == SkillFrameType.Normal)
                    --lv;
                ExpTable ExpData = TableManager.GetInstance().GetTableItem<ExpTable>(lv);
                if (ExpData != null)
                {
                    UnLockSkillSlotNum = ExpData.SkillNum;
                }
            }
            if (null != mView)
                mView.OnInit();
        }

        protected override void _OnCloseFrame()
        {
            UnLockSkillSlotNum = 0;
            if (null != mView)
                mView.OnUninit();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillPlanClose);
        }

        #region ExtraUIBind
        private SkillConfigurationFrameView mView = null;

        protected override void _bindExUI()
        {
            mView = mBind.GetCom<SkillConfigurationFrameView>("View");
        }

        protected override void _unbindExUI()
        {
            mView = null;
        }
        #endregion
    }
}
