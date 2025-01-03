using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamExpedtionResultRoleInfo : MonoBehaviour
    {  

        #region VIEW PARAMS

        [SerializeField] private Image roleIcon;

        #endregion


        private void _SetRoleIcon(string imgPath)
        {
            roleIcon.SafeSetImage(imgPath);
        }

        #region  PUBLIC METHODS

        public void RefreshView(int jobId)
        {
            string roleIconPath = Utility.GetRoleIconByRoleId(jobId);
            _SetRoleIcon(roleIconPath);
        }

        #endregion
    }
}
