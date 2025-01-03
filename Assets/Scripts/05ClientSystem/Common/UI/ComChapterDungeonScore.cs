using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteAlways]
public class ComChapterDungeonScore : MonoBehaviour 
{
    public Protocol.DungeonScore mScore;
    public ComCommonBind         mBind;

    public void SetScore(Protocol.DungeonScore score)
    {
        mScore = score;

        if (null != mBind)
        {
            Image scoreImage  = mBind.GetCom<Image>("score");
            Image scoreButton = mBind.GetCom<Image>("scorebutton");

            string str = score.ToString().ToLower();
            // Sprite sprite = mBind.GetSprite(str);

            //if (sprite == null)
            //{
            //    sprite = mBind.GetSprite("unpass");
            //}

            if (null != scoreButton)
            {
                // scoreButton.sprite = sprite;
                mBind.GetSprite(str, ref scoreButton);
                if (scoreButton.sprite == null)
                    mBind.GetSprite("unpass", ref scoreButton);
                scoreButton.SetNativeSize();
            }

            if (null != scoreImage)
            {
                // scoreImage.sprite = sprite;
                mBind.GetSprite(str, ref scoreImage);
                if(scoreImage == null)
                    mBind.GetSprite("unpass", ref scoreImage);
                scoreImage.SetNativeSize();
            }
        }
    }
}
