using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComTags : MonoBehaviour
{
    public Image[] mImageArray;
    public Sprite mDefaulteSprite;

    private int mCount = -1;

    void Start()
    {
        SetSprite(mDefaulteSprite);
    }

    public void SetSprite(Sprite sprite)
    {
        for (int i = 0; i < mImageArray.Length; ++i)
        {
            mImageArray[i].sprite = sprite;
        }
    }


    public void SetTag(int cnt)
    {
        if (cnt >= 0 && cnt != mCount)
        {
            var count = mImageArray.Length;
            mCount = cnt;

            for (int i = 0; i < cnt; ++i)
            {
                mImageArray[i].gameObject.SetActive(true);
            }

            for (int i = cnt; i < count; ++i)
            {
                mImageArray[i].gameObject.SetActive(false);
            }
        }
    }
}
