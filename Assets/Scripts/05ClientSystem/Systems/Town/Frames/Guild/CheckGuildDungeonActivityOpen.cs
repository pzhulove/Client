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
    public class CheckGuildDungeonActivityOpen : MonoBehaviour
    {

        [SerializeField]
        GameObject goShow = null;

        // Use this for initialization
        void Start()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonUpdateActivityData, _OnUpdateShow);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonSyncActivityState, _OnUpdateShow);

            _OnUpdateShow(null);
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonUpdateActivityData, _OnUpdateShow);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonSyncActivityState, _OnUpdateShow);
        }

        // Update is called once per frame
        void Update()
        {

        }       

        private void _OnUpdateShow(UIEvent uiEvent)
        {
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (data != null)
            {
                goShow.CustomActive(GuildDataManager.CheckActivityLimit() && data.nActivityState != (int)GuildDungeonStatus.GUILD_DUNGEON_END);
            }
        }       
    }
}


