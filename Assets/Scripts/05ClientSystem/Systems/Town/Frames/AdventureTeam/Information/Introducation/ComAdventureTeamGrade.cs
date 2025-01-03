using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComAdventureTeamGrade : MonoBehaviour
    {
        public const string GRADE_C_RESPATH = "UI/Image/NewPacked/Paihangbang.png:Paihangbang_Icon_C";
        public const string GRADE_B_RESPATH = "UI/Image/NewPacked/Paihangbang.png:Paihangbang_Icon_B";
        public const string GRADE_A_RESPATH = "UI/Image/NewPacked/Paihangbang.png:Paihangbang_Icon_A";
        public const string GRADE_S_RESPATH = "UI/Image/NewPacked/Paihangbang.png:Paihangbang_Icon_S";

        [SerializeField]
        private Image[] images;

        [SerializeField]
        private LayoutElement[] layoutElements;

        [SerializeField]
        private int childCount = 3;

        private string[,] gradeEnumWithResPath = new string[,]
            {
                { ProtoTable.AdventureTeamGradeTable.eGradeEnum.S.ToString(), GRADE_S_RESPATH},
                { ProtoTable.AdventureTeamGradeTable.eGradeEnum.A.ToString(), GRADE_A_RESPATH},
                { ProtoTable.AdventureTeamGradeTable.eGradeEnum.B.ToString(), GRADE_B_RESPATH},
                { ProtoTable.AdventureTeamGradeTable.eGradeEnum.C.ToString(), GRADE_C_RESPATH},
            };

        private int[,] gradeEnumWithImgNum = new int[,]
            {
                { (int)ProtoTable.AdventureTeamGradeTable.eGradeEnum.SSS, 3, 0},
                { (int)ProtoTable.AdventureTeamGradeTable.eGradeEnum.SS, 2, 0},
                { (int)ProtoTable.AdventureTeamGradeTable.eGradeEnum.S, 1, 0},
                { (int)ProtoTable.AdventureTeamGradeTable.eGradeEnum.A, 1, 1},
                { (int)ProtoTable.AdventureTeamGradeTable.eGradeEnum.B, 1, 2},
                { (int)ProtoTable.AdventureTeamGradeTable.eGradeEnum.C, 1, 3},
            };

        private void Awake()
        {           
            childCount = transform.childCount;
            images = new Image[childCount];
            layoutElements = new LayoutElement[childCount];
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child == null)
                    continue;
                Image childImg = child.GetComponent<Image>();
                if (childImg)
                {
                    images[i] = childImg;
                }
                LayoutElement childLayoutEle = child.GetComponent<LayoutElement>();
                if (childLayoutEle)
                {
                    layoutElements[i] = childLayoutEle;
                }
            }
        }

        private void OnDestroy()
        {
            gradeEnumWithResPath = null;
            gradeEnumWithImgNum = null;
        }


        public bool SetGradeImg(ProtoTable.AdventureTeamGradeTable.eGradeEnum gradeEnum)
        {
            if (images == null || images.Length <= 0)
            {
                return false;
            }
            if (layoutElements == null || layoutElements.Length <= 0)
            {
                return false;
            }

            if (gradeEnum == ProtoTable.AdventureTeamGradeTable.eGradeEnum.GradeEnum_None)
            {
                return false;
            }

            int imgNum = 0;
            int imgResPathIndex = -1;

            for (int i = 0; i < gradeEnumWithImgNum.GetLength(0); i++)
            {
                int gradeEnumIndex = gradeEnumWithImgNum[i, 0];
                if (gradeEnumIndex == (int)gradeEnum)
                {
                    int gradeImgNum = gradeEnumWithImgNum[i, 1];
                    if (gradeImgNum > childCount)
                    {
                        Logger.LogError("ComAdventureTeamGrade Grade Img num length is too longer !");
                        break;
                    }
                    imgNum = gradeImgNum;
                    imgResPathIndex = gradeEnumWithImgNum[i, 2];
                    break;
                }
            }

            if (imgResPathIndex < 0 || imgResPathIndex >= gradeEnumWithResPath.GetLength(0))
            {
                return false;
            }
            var imgResPath = gradeEnumWithResPath[imgResPathIndex, 1];
            if (string.IsNullOrEmpty(imgResPath))
            {
                return false;
            }

            if (imgNum <= 0 || imgNum > images.Length || imgNum > layoutElements.Length)
            {
                return false;
            }

            bool loadImgSucc = false;
            for (int i = 0; i < images.Length; i++)
            {
                var img = images[i];
                if (img == null) continue;
                if (i < imgNum)
                {                    
                    loadImgSucc = ETCImageLoader.LoadSprite(ref img, imgResPath);
                    img.enabled = true;
                }
                else
                {
                    img.sprite = null;
                    img.enabled = false;
                }
            }

            for (int i = 0; i < layoutElements.Length; i++)
            {
                var layoutEle = layoutElements[i];
                if (layoutEle == null) continue;
                if (i < imgNum)
                {
                    layoutEle.ignoreLayout = false;
                }
                else
                {
                    layoutEle.ignoreLayout = true;
                }
            }

            return loadImgSucc;
        }

    }
}
