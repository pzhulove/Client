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
    class PKSeasonStartFrame : ClientFrame
    {
        [UIObject("TitleGroup/Name/Numbers")]
        GameObject m_objDigitRoot;

        [UIObject("TitleGroup/Name/Numbers/Number")]
        GameObject m_objDigitTemplate;

        [UIObject("RankRoot")]
        GameObject m_objRankRoot;

        [UIControl("DescGroup/Text")]
        Text m_labDesc;

		string m_strDigitPath = "UI/Image/Packed/p_UI_SeasonNumber.png:UI_Season_Number_0{0}";//"UIFlatten/Image/Packed/pck_UI_PKResult.png:UI_Jiesuan_{0}";
        CommonPKRank m_comPKRank = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/PKSeasonStart";
        }

        protected override void _OnOpenFrame()
        {
            _InitSeasonName();
            _InitPKRank();
            _InitDesc();
        }

        protected override void _OnCloseFrame()
        {
            _ClearSeasonName();
            _ClearPKRank();
            _ClearDesc();
        }

        void _InitSeasonName()
        {
            for (int i = 0; i < m_objDigitRoot.transform.childCount; ++i)
            {
                m_objDigitRoot.transform.GetChild(i).gameObject.SetActive(false);
            }
            int nIdx = 0;
            int nNumber = SeasonDataManager.GetInstance().seasonID;
            while (nNumber > 0)
            {
                int nDigit = nNumber % 10;
                GameObject objDigit = null;
                if (nIdx < m_objDigitRoot.transform.childCount)
                {
                    objDigit = m_objDigitRoot.transform.GetChild(nIdx).gameObject;
                }
                else
                {
                    objDigit = GameObject.Instantiate(m_objDigitTemplate);
                    objDigit.transform.SetParent(m_objDigitRoot.transform, false);
                    objDigit.transform.SetAsFirstSibling();
                }
                objDigit.SetActive(true);

                //objDigit.GetComponent<Image>().sprite =
                //    AssetLoader.GetInstance().LoadRes(string.Format(m_strDigitPath, nDigit), typeof(Sprite)).obj as Sprite;
                Image img = objDigit.GetComponent<Image>();
                ETCImageLoader.LoadSprite(ref img, string.Format(m_strDigitPath, nDigit));

                nNumber = nNumber / 10;
                nIdx++;
            }
        }

        void _ClearSeasonName()
        {

        }

        void _InitPKRank()
        {
            m_comPKRank = ComPKRank.CreateCommonPKRank(m_objRankRoot);
            if (m_comPKRank != null)
            {
                m_comPKRank.Initialize(SeasonDataManager.GetInstance().seasonLevel, SeasonDataManager.GetInstance().seasonExp);
                //DOTweenAnimation doTween = m_objRankRoot.GetComponent<DOTweenAnimation>();
                //if (doTween != null)
                //{
                //    if (doTween.onStepComplete != null)
                //    {
                //        doTween.onStepComplete.AddListener(() =>
                //        {
                //            m_comPKRank.StartRankChange(PKRankDataManager.GetInstance().GetMinRankID(),
                //                PKRankDataManager.GetInstance().pkRankID);
                //        });
                //    }
                //}
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
            m_labDesc.text = TR.Value("pk_rank_season_start_desc", 
                SeasonDataManager.GetInstance().GetRankName(SeasonDataManager.GetInstance().seasonLevel)
                );
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
