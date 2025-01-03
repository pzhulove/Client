using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ReplaceHeadPortraitFrame : MonoBehaviour
    {
        [SerializeField]private Image mImgHeadPortraitFrame;

        /// <summary>
        /// 替换相框
        /// </summary>
        /// <param name="id"></param>
        public void ReplacePhotoFrame(int id)
        {
            string sPath = HeadPortraitFrameDataManager.GetHeadPortraitFramePath(id);
            if (sPath == "")
            {
                return;
            }

            if (mImgHeadPortraitFrame != null)
            {
                ETCImageLoader.LoadSprite(ref mImgHeadPortraitFrame, sPath);
            }
        }
    }
}

