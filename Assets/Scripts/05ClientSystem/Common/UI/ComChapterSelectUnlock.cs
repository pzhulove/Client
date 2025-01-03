using UnityEngine;
using UnityEngine.UI.Extensions;
using System.Collections.Generic;

public class ComChapterSelectUnlock : MonoBehaviour {

    public int mUnlockCount = 2;

    public Vector2 mOffset = Vector2.one * 50f;

    public UILineRenderer mUnlockLineRender;
    public UILineRenderer mLockLineRender;

    public Vector2[] mPositionList = new Vector2[0];

    private int _updateChild()
    {
        int count = transform.childCount;

        mPositionList = new Vector2[count];

        for (int i = 0; i < count; ++i)
        {
            var node = transform.GetChild(i);

            if (null != node)
            {
                mPositionList[i] = node.localPosition;
            }
            else 
            {
                mPositionList[i] = Vector3.zero;
            }

            mPositionList[i] += mOffset;
        }
        return 0;
    }


    private void _updateLine()
    {
        List<Vector2> pointList = new List<Vector2>();

        mUnlockCount = Mathf.Min(mUnlockCount, mPositionList.Length);

        for (int i = 0; i < mUnlockCount; ++i)
        {
            pointList.Add(mPositionList[i]);
        }

        if (pointList.Count > 1)
        {
            mUnlockLineRender.enabled = true;
            mUnlockLineRender.Points = pointList.ToArray();
            mUnlockLineRender.SetVerticesDirty();
        }
        else
        {
            mUnlockLineRender.enabled = false;
        }

        pointList.Clear();

        for (int i = Mathf.Max(0, mUnlockCount - 1); i < mPositionList.Length; ++i)
        {
            pointList.Add(mPositionList[i]);
        }

        if (pointList.Count > 1)
        {
            mLockLineRender.enabled = true;
            mLockLineRender.Points = pointList.ToArray();
            mLockLineRender.SetVerticesDirty();
        }
        else
        {
            mLockLineRender.enabled = false;
        }
        pointList.Clear();
    }

    public void SetUnlockCount(int unlockCount)
    {
        mUnlockCount = unlockCount;
        _updateChild();
        _updateLine();
    }

    public void SetVisible(bool isVisible)
    {
        mLockLineRender.enabled = isVisible;
        mUnlockLineRender.enabled = isVisible;
        if (isVisible)
        {
            SetUnlockCount(mUnlockCount);
        }
    }
	
	// Update is called once per frame
//	void Update ()
//    {
//#if UNITY_EDITOR
//        SetUnlockCount(mUnlockCount);
//#endif
//    }
}
