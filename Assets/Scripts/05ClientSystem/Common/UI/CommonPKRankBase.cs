using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class CommonPKRankBase : MonoBehaviour
    {
        [SerializeField] private RectTransform rectStarRoot;
        [SerializeField] private Image imgRankIcon;
        [SerializeField] private GameObject objRankMaxRoot;
        [SerializeField] private Text labRankMax;
        [SerializeField] private Text labRank;

        static string ms_strGetStarEffect = "GetEffect";
        static string ms_strLoseStarEffect = "LoseEffect";
        private void Start()
        {
            Initialize(SeasonDataManager.GetInstance().seasonLevel);
        }

        public void Initialize(int a_nRankID)
        {
            SetupRank(a_nRankID);
        }

        private void SetupRank(int a_nRankID, bool bClearStar = false)
        {
            SeasonLevelTable tableData = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nRankID);
            if (tableData != null)
            {
                 SetupRankIcon(tableData);
                 SetupRankName(tableData);
                 SetupRankStars(tableData, bClearStar);
            }
            else
            {
                Logger.LogErrorFormat("段位表不存在段位ID：{0}", a_nRankID);
            }
        }

        public void SetupRankIcon(SeasonLevelTable a_tableData)
        {
            if (a_tableData == null)
            {
                imgRankIcon.gameObject.SetActive(false);
            }
            else
            {
                imgRankIcon.gameObject.SetActive(true);
                ETCImageLoader.LoadSprite(ref imgRankIcon, a_tableData.Icon);
            }
        }

        public void SetupRankName(SeasonLevelTable a_tableData)
        {
            if (a_tableData == null)
            {
                labRank.text = string.Empty;
            }
            else
            {
                labRank.text = SeasonDataManager.GetInstance().GetRankName(a_tableData.ID);
            }
        }

        public void SetupRankStars(SeasonLevelTable a_tableData, bool a_bOnlySlots)
        {
            if (a_tableData == null)
            {
                objRankMaxRoot.SetActive(false);
                rectStarRoot.gameObject.SetActive(false);
                return;
            }


            if (a_tableData.AfterId == 0)
            {
                objRankMaxRoot.SetActive(true);
                rectStarRoot.gameObject.SetActive(false);
                labRankMax.text = string.Format("x{0}", SeasonDataManager.GetInstance().seasonStar);
            }
            else
            {
                objRankMaxRoot.SetActive(false);
                rectStarRoot.gameObject.SetActive(true);

                for (int i = 0; i < rectStarRoot.childCount; ++i)
                {
                    rectStarRoot.GetChild(i).gameObject.SetActive(false);
                }

                //Assert.IsTrue(a_tableData.MaxStar <= rectStarRoot.childCount);
                for (int i = 0; i < a_tableData.MaxStar; ++i)
                {
                
                    GameObject objStar;
                    objStar = rectStarRoot.GetChild(i).gameObject;

                    objStar.SetActive(true);
                    if (a_bOnlySlots)
                    {
                        Utility.FindGameObject(objStar, "Light").SetActive(false);
                    }
                    else
                    {
                        Utility.FindGameObject(objStar, "Light").SetActive((i + 1) <= a_tableData.Star);
                    }

                    GameObject objGetStarEffect = Utility.FindGameObject(objStar, ms_strGetStarEffect, false);
                    if (objGetStarEffect != null)
                    {
                        objGetStarEffect.SetActive(false);
                    }
                    GameObject objLoseStarEffect = Utility.FindGameObject(objStar, ms_strLoseStarEffect, false);
                    if (objLoseStarEffect != null)
                    {
                        objLoseStarEffect.SetActive(false);
                    }
                }
            }
        }

        public void ClearAllStars()
        {
            objRankMaxRoot.SetActive(false);
            rectStarRoot.gameObject.SetActive(false);
        }
    }
}