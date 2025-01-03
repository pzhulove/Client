using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;

namespace GameClient
{
    class TownFullMapFrame : ClientFrame
    {
        [UIObject("content")]
        GameObject m_objMapRoot;

        TownMapFrame m_mapFrame;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TownMap/FullMap";
        }

        protected override void _OnOpenFrame()
        {
            m_mapFrame = frameMgr.OpenFrame<TownMapFrame>(m_objMapRoot, null, "full_map") as TownMapFrame;
        }

        protected override void _OnCloseFrame()
        {
            frameMgr.CloseFrame(m_mapFrame);
        }

        [UIEventHandle("close")]
        protected void _OnClose()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
