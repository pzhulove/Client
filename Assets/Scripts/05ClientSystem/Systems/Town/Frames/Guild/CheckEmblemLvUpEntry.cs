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
    public class CheckEmblemLvUpEntry : MonoBehaviour
    {

        [SerializeField]
        GameObject goShow = null;

        // Use this for initialization
        void Start()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateGuildEmblemLvUpEntry, _OnUpdateShow);            

            PlayerBaseData.GetInstance().onLevelChanged += _OnLevelChanged;

            _OnUpdateShow(null);
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateGuildEmblemLvUpEntry, _OnUpdateShow);            

            PlayerBaseData.GetInstance().onLevelChanged -= _OnLevelChanged;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void _OnLevelChanged(int iPre, int iCur)
        {
            _OnUpdateShow(null);
        }

        private void _OnUpdateShow(UIEvent uiEvent)
        {
            bool bShow = (PlayerBaseData.GetInstance().Level >= GuildDataManager.GetInstance().GetEmblemLvUpPlayerLvLimit()) 
                && (GuildDataManager.GetInstance().myGuild != null && GuildDataManager.GetInstance().myGuild.nLevel >= GuildDataManager.GetInstance().GetEmblemLvUpGuildLvLimit())
                && (GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.HONOUR) >= GuildDataManager.GetInstance().GetEmblemLvUpHonourLvLimit());

            goShow.CustomActive(bShow);
        }       
    }
}


