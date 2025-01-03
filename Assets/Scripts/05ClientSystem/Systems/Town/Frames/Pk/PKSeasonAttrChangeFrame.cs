using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using DG.Tweening;

namespace GameClient
{
    class PKSeasonAttrChangeFrame : ClientFrame
    {
        [UIObject("RankRoot")]
        GameObject m_objRankRoot;

        [UIControl("Text_T")]
        Text m_labMyAttrTitle;

        [UIControl("Text_B")]
        Text m_labMyAttrContent;

        CommonPKRank m_comPKRank = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/PKSeasonAttrChange";
        }

        protected override void _OnOpenFrame()
        {
            _InitPKRank();
            _InitDesc();
        }

        protected override void _OnCloseFrame()
        {
            _ClearPKRank();
            _ClearDesc();
        }

        void _InitPKRank()
        {
            m_comPKRank = ComPKRank.CreateCommonPKRank(m_objRankRoot);
            if (m_comPKRank != null)
            {
                m_comPKRank.Initialize(SeasonDataManager.GetInstance().seasonAttrMappedSeasonID, 0);
            }
        }

        void _ClearPKRank()
        {
            if (m_comPKRank != null)
            {
                m_comPKRank.Clear();
                m_comPKRank = null;
            }
        }

        void _InitDesc()
        {
            m_labMyAttrTitle.text = TR.Value("pk_rank_detail_attr_desc",
                SeasonDataManager.GetInstance().GetRankName(SeasonDataManager.GetInstance().seasonAttrMappedSeasonID));
            SeasonLevel seasonLevel = SeasonDataManager.GetInstance().GetSeasonAttr(SeasonDataManager.GetInstance().seasonAttrID);
            if (seasonLevel != null)
            {
                m_labMyAttrContent.text = seasonLevel.strFormatAttr;
            }
            else
            {
                m_labMyAttrContent.text = string.Empty;
            }
        }

        void _ClearDesc()
        {

        }

        [UIEventHandle("Ok")]
        void _OnConfirmClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
