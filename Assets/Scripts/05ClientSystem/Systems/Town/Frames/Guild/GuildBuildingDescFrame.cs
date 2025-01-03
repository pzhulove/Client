using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class GuildBuildingDescFrame : ClientFrame
    {
        [UIObject("Content/ScrollView/Viewport/Content/Desc")]
        GameObject m_objDescTemplate;

        [UIObject("Content/ScrollView/Viewport/Content")]
        GameObject m_objDescRoot;

        class BuildingDesc
        {
            public string strName;
            public string strDesc;

            public BuildingDesc(string a_strName, string a_strDesc)
            {
                strName = a_strName;
                strDesc = a_strDesc;
            }
        }

        BuildingDesc[] m_arrDescs =        
        {
            new BuildingDesc(TR.Value("guild_building_main"), TR.Value("guild_building_main_desc")),
            new BuildingDesc(TR.Value("guild_building_welfare"), TR.Value("guild_building_welfare_desc")),
            new BuildingDesc(TR.Value("guild_building_skill"), TR.Value("guild_building_skill_desc")),
            new BuildingDesc(TR.Value("guild_building_shop"), TR.Value("guild_building_shop_desc")),
            new BuildingDesc(TR.Value("guild_building_table"), TR.Value("guild_building_table_desc")),
            new BuildingDesc(TR.Value("guild_building_dungeon"), TR.Value("guild_building_dungeon_desc")),
            new BuildingDesc(TR.Value("guild_building_statue"), TR.Value("guild_building_statue_desc")),
        };


        public override string GetPrefabPath()
        {
            return "UI/Prefabs/Guild/GuildBuildingDesc";
        }

        protected override void _OnOpenFrame()
        {
            m_objDescTemplate.SetActive(false);

            for (int i = 0; i < m_arrDescs.Length; ++i)
            {
                GameObject obj = GameObject.Instantiate(m_objDescTemplate);
                obj.transform.SetParent(m_objDescRoot.transform, false);
                obj.SetActive(true);

                Utility.GetComponetInChild<Text>(obj, "Title").text = string.Format("{0}:", m_arrDescs[i].strName);
                Utility.GetComponetInChild<Text>(obj, "Text").text = m_arrDescs[i].strDesc;
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
        }

        void _OnGuildDismissed(UIEvent a_event)
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
