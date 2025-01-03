using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamContentBaseView : MonoBehaviour
    {

        private void OnDisable()
        {
            OnDisableView();
        }

        public virtual void InitData()
        {

        }

        public virtual void OnEnableView()
        {

        }

        public virtual void OnDisableView()
        {

        }

    }
}