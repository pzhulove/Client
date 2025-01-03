using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public interface IChapterCharactor
{
    // void SetCharactor(Sprite sprite);
    void SetCharactor(string spritePath);
}

public class ComChapterCharactor : MonoBehaviour, IChapterCharactor
{
    public Image mImage;

    //public void SetCharactor(Sprite sprite)
    //{
    //    if (null != mImage)
    //    {
    //        mImage.sprite = sprite;
    //    }
    //}
    public void SetCharactor(string spritePath)
    {
        if (null != mImage)
        {
            // mImage.sprite = sprite;
            ETCImageLoader.LoadSprite(ref mImage, spritePath);
        }
    }
}
