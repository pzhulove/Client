using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using DG.Tweening;
using Protocol;
using Network;

namespace GameClient
{
    class RoleRecoveryFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SelecteRoleNew/RoleRecoveryFrame";
        }

        protected override void _OnOpenFrame()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleRecoveryUpdate,_OnRecoveryUpdate);
            _UpdateRecoveryRoles();
        }

        void _UpdateRecoveryRoles()
        {
            GameObject goParent = Utility.FindChild(frame, "Dlg/ScrollView/Viewport/Content");
            GameObject goPrefab = Utility.FindChild(goParent, "RecoveryRoleInfo");
            goPrefab.CustomActive(false);

            m_akRolesRecovery.RecycleAllObject();

            var roles = ClientApplication.playerinfo.roleinfo;
            for (int i = 0; i < roles.Length; ++i)
            {
                if (RecoveryRoleCachedObject.OnFilter(roles[i]))
                {
                    m_akRolesRecovery.Create(new object[] {
                        goParent,
                        goPrefab,
                        new RoleData { roleInfo = roles[i] },
                        false,
                    });
                }
            }
        }

        void _OnRecoveryUpdate(UIEvent uiEvent)
        {
            frameMgr.CloseFrame(this);
            //_UpdateRecoveryRoles();
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleRecoveryUpdate, _OnRecoveryUpdate);
            m_akRolesRecovery.DestroyAllObjects();
        }

        [UIEventHandle("Dlg/Close")]
        void OnClickOk()
        {
            frameMgr.CloseFrame(this, true);
        }

        CachedObjectListManager<RecoveryRoleCachedObject> m_akRolesRecovery = new CachedObjectListManager<RecoveryRoleCachedObject>();
    }
}
