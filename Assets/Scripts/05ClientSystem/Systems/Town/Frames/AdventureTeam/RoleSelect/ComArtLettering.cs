using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComArtLettering : MonoBehaviour
    {        
        #region View Params

        [Header("选择一张合图并确认该合图中有连续后缀名的美术字图片路径，序号用{0}替代")]
        [SerializeField]
        private string M_ArtTexturePath;

        [Space(10)]
        [Header("目前只支持五位数 0 ~  99999 ")]
        [SerializeField]
        private Image[] rmbImgs;

        #endregion

        #region  PUBLIC METHODS

        public void SetNum(int num)
        {
            if (string.IsNullOrEmpty(M_ArtTexturePath))
            {
                Logger.LogError("Please Fix ,ComArtLettering : M_ArtTexturePath is null or empty");
                return;
            }

            int tempNum = num;
            int numCount = Mathf.Abs(tempNum).ToString().Length;

            if (rmbImgs == null || rmbImgs.Length <= 0)
            {
                Logger.LogError("Try to set rmb image is failed, image component is null");
                return;
            }
            if (numCount > rmbImgs.Length)
            {
                //Logger.LogErrorFormat("Please Fix , rmb num count is {0}, rmb image length is {1}", numCount, rmbImgs.Length);
                int lessNum = numCount - rmbImgs.Length;
                _CreateNewImgs(lessNum);
            }
            for (int i = numCount; i < rmbImgs.Length; i++)
            {
                rmbImgs[i].gameObject.CustomActive(false);
            }
            for (int i = 0; i < numCount; i++)
            {
                int r1 = 0;
                int count = numCount - 1 - i;
                int pow = (int)Mathf.Pow(10, count);
                if (count == 0)
                {
                    r1 = tempNum;
                }
                else
                {
                    r1 = tempNum / pow;
                    tempNum = tempNum % pow;
                }
                bool bLoad = ETCImageLoader.LoadSprite(ref rmbImgs[i], string.Format(M_ArtTexturePath, r1));
                if (!bLoad)
                {
                    Logger.LogErrorFormat("Can not load Asset res {0} failed", string.Format(M_ArtTexturePath, r1));
                }
                else
                {
                    rmbImgs[i].CustomActive(true);
                }
            }
        }

        public void SetNum(string numStr)
        {
            if (string.IsNullOrEmpty(M_ArtTexturePath))
            {
                Logger.LogError("Please Fix ,ComArtLettering : M_ArtTexturePath is null or empty");
                return;
            }
            if (string.IsNullOrEmpty(numStr))
            {
                return;
            }
            int tempNum = 0;
            if (!int.TryParse(numStr, out tempNum))
            {
                Logger.LogErrorFormat("Please Fix , rmb string {0} convert to int is error", numStr);
                return;
            }
            SetNum(tempNum);
        }

        private void _CreateNewImgs(int count)
        {
            if (rmbImgs == null || rmbImgs.Length <= 0)
            {
                Logger.LogError("Try to set rmb image is failed, image component is null");
                return;
            }
            Image[] newImgs = new Image[count];
            for (int i = 0; i < newImgs.Length; i++)
            {
                GameObject imgGo = GameObject.Instantiate(rmbImgs[0].gameObject);
                if (imgGo == null)
                    continue;
                Image img = imgGo.GetComponent<Image>();
                if (img == null)
                    continue;
                newImgs[i] = img;
            }
            newImgs.CopyTo(rmbImgs, rmbImgs.Length);
        }
        
        #endregion
    }
}