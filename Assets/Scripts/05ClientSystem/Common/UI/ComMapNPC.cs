using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComMapNPC : MonoBehaviour
    {
        public int NpcID = 0;
        public Image imgFace = null;

        public bool IsValid { get { return m_npcTable != null; } }
        public string NpcName { get { return IsValid ? m_npcTable.NpcName : ""; } }
        public string NpcIcon { get { return IsValid ? m_npcTable.NpcIcon : ""; } }
        public bool IsNormalNpc
        {
            get
            {
                if (IsValid)
                {
                    return m_npcTable.Function == ProtoTable.NpcTable.eFunction.none || 
                        m_npcTable.Function == ProtoTable.NpcTable.eFunction.clicknpc;
                }
                else
                {
                    return false;
                }
            }
        }

        ProtoTable.NpcTable m_npcTable = null;

        void OnStart()
        {
            Setup();
        }

        void OnValidate()
        {
            if(AssetLoader.IsAssetManagerReady())
                Setup();
        }

        public void Setup()
        {
            m_npcTable = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(NpcID);
            if (m_npcTable == null)
            {
                return;
            }

            if (imgFace != null)
            {
                // imgFace.sprite = AssetLoader.instance.LoadRes(m_npcTable.NpcIcon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref imgFace, m_npcTable.NpcIcon);
            }
        }
    }
}
