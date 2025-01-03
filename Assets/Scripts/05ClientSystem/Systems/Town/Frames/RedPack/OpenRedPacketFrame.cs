using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using System.Collections;

namespace GameClient
{
    class OpenRedPacketFrame : ClientFrame
    {
        [UIControl("Content/Head/Viewport/Image")]
        Image m_imgOwnerHead;

        [UIControl("Content/Head/Level")]
        Text m_labOwnerLevel;

        [UIControl("Content/Head/Name")]
        Text m_labOwnerName;

        [UIControl("Content/RedPackName")]
        Text m_labRedPacketName;

        RedPacketDetail m_data = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RedPack/OpenRedPack";
        }

        protected override void _OnOpenFrame()
        {
            m_data = userData as RedPacketDetail;
            if (m_data == null)
            {
                Logger.LogErrorFormat("open GuildOpenRedPacketFrame, userdata is invalid!");
                return;
            }

            string strGetMoneyIcon = RedPackDataManager.GetInstance().GetGetMoneyIcon(m_data.baseEntry.reason);
            string strCostMoneyIcon = RedPackDataManager.GetInstance().GetCostMoneyIcon(m_data.baseEntry.reason);

            // m_imgOwnerHead.sprite = AssetLoader.instance.LoadRes(_GetHeadByJobID(m_data.ownerIcon.occu), typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref m_imgOwnerHead, _GetHeadByJobID(m_data.ownerIcon.occu));
            m_labOwnerLevel.text = m_data.ownerIcon.level.ToString();
            m_labOwnerName.text = TR.Value("WhoistheRedPack", m_data.ownerIcon.name);
            m_labRedPacketName.text = m_data.baseEntry.name;

            StartCoroutine(_ShowDetail());
        }

        protected override void _OnCloseFrame()
        {
            m_data = null;
        }

        IEnumerator _ShowDetail()
        {
            float maxTime = 1.0f;

            float startTime = Time.time;
            float elapsed = 0.0f;
            while (elapsed < maxTime)
            {
                elapsed = Time.time - startTime;
                yield return Yielders.EndOfFrame;
            }

            frameMgr.OpenFrame<ShowOpenedRedPacketFrame>(FrameLayer.Middle, m_data);
            frameMgr.CloseFrame(this);
        }

        string _GetHeadByJobID(int a_nJobID)
        {
            string strHead = string.Empty;
            ProtoTable.JobTable table = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(a_nJobID);
            if (table != null)
            {
                ProtoTable.ResTable resTable = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(table.Mode);
                if (resTable != null)
                {
                    strHead = resTable.IconPath;
                }
            }
            return strHead;
        }

        [UIEventHandle("Content")]
        void _OnOpenClicked()
        {
            frameMgr.OpenFrame<ShowOpenedRedPacketFrame>(FrameLayer.Middle, m_data);
            frameMgr.CloseFrame(this);
        }
    }
}
