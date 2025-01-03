using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    public class AchievementEffectPlayFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ActiveGroup/AchievementEffectPlayFrame";
        }

        [UIControl("", typeof(ComAchievementEffectPlayConfig))]
        ComAchievementEffectPlayConfig mComAchievementEffectPlayConfig;

        protected override void _OnOpenFrame()
        {
            _AddButton("Close", () => { frameMgr.CloseFrame(this); });
            InvokeMethod.InvokeInterval(this, 0.0f, 3.0f,9999.0f, _TriggerEffect, _TriggerEffect, null);
        }

        void _TriggerEffect()
        {
            if(!MissionManager.GetInstance().HasAchievementItem())
            {
                frameMgr.CloseFrame(this);
                return;
            }

            GetFrame().CustomActive(false);
            FunctionUnLock functionUnLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.AchievementG);
            if (functionUnLockData == null)
            {
                return;
            }

            //Logger.LogErrorFormat("成就解锁等级为{0},玩家等级为{1}", functionUnLockData.StartLevel, PlayerBaseData.GetInstance().Level);

            if (functionUnLockData.StartLevel > PlayerBaseData.GetInstance().Level)
            {
                return;
            }

            GetFrame().CustomActive(true);
            if (null != mComAchievementEffectPlayConfig)
            {
                mComAchievementEffectPlayConfig.Play();
            }
        }

        protected override void _OnCloseFrame()
        {
            InvokeMethod.RmoveInvokeIntervalCall(this);
        }
    }
}