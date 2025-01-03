using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 战斗UI组件帮助类
    /// </summary>
    public class BattleUIHelper
    {
        public static T CreateBattleUIComponent<T>() where T : BattleUIBase
        {
#if !LOGIC_SERVER
            if (ClientSystemManager.instance == null) return null;
            ClientSystemBattle battle = null;
            if (ClientSystemManager.instance.TargetSystem != null)
                battle = ClientSystemManager.instance.TargetSystem as ClientSystemBattle;
            else if (ClientSystemManager.instance.CurrentSystem != null)
                battle = ClientSystemManager.instance.CurrentSystem as ClientSystemBattle;
            if (battle == null) return null;
            if (GetBattleUIComponent<T>() == null)
                return battle.BattleUIComponentManager.CreateBattleUIComponent<T>();
            else
                return GetBattleUIComponent<T>();
#else
            return default(T); ;
#endif
        }


        public static T GetBattleUIComponent<T>() where T : BattleUIBase
        {
#if !LOGIC_SERVER
            if (ClientSystemManager.instance == null) return null;
            ClientSystemBattle battle = null;
            if (ClientSystemManager.instance.TargetSystem != null)
                battle = ClientSystemManager.instance.TargetSystem as ClientSystemBattle;
            else if (ClientSystemManager.instance.CurrentSystem != null)
                battle = ClientSystemManager.instance.CurrentSystem as ClientSystemBattle;
            if (battle == null) return null;
            return battle.BattleUIComponentManager.GetBattleUIComponent<T>();
#else
            return default(T); ;
#endif
        }
    }
}