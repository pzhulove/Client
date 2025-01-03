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
    public class CheckHaveGotArtifactJarDiscount : MonoBehaviour
    {
        GameObject goShow = null;

        // Use this for initialization
        void Start()
        {
            goShow = this.transform.gameObject;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ArtifactJarDataUpdate, _OnUpdateShow);           

            _OnUpdateShow(null);
        }

        private void OnDestroy()
        {
            goShow = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ArtifactJarDataUpdate, _OnUpdateShow);            
        }

        // Update is called once per frame
        void Update()
        {

        }   
       
        private void _OnUpdateShow(UIEvent uiEvent)
        {
            goShow.CustomActive(!ActivityJarFrame.IsHaveGotArtifactJarDiscount());
            
            // 活动展示阶段不显示折扣红点了
            if(ArtifactFrame.IsArtifactJarShowActivityOpen())
            {
                goShow.CustomActive(false);
            }
        }       
    }
}


