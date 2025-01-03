using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using DG.Tweening;

namespace GameClient
{
    [RequireComponent(typeof(RectTransform))]
    public class CheckArtifactJarActivityOpen : MonoBehaviour
    {
        GameObject goShow = null;

        // Use this for initialization
        void Start()
        {
            goShow = this.transform.gameObject;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, _OnUpdateShow);           

            _OnUpdateShow(null);
        }

        private void OnDestroy()
        {
            goShow = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, _OnUpdateShow);            
        }

        // Update is called once per frame
        void Update()
        {

        }
   
        private bool IsActivityOpen()
        {
            return ArtifactFrame.IsArtifactJarDiscountActivityOpen() || ArtifactFrame.IsArtifactJarShowActivityOpen() || ArtifactFrame.IsArtifactJarRewardActivityOpen();
        }

        private void _OnUpdateShow(UIEvent uiEvent)
        {
            goShow.CustomActive(IsActivityOpen());            
        }       
    }
}


