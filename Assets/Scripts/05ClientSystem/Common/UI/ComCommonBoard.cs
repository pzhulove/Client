using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ComCommonBoard : MonoBehaviour
{
    public enum ePosition
    {
        Left,
        Right,
        Top,
        Buttom,
        Background,
    }

    // 背景
    public Image mBg;

    // 结构
    public GameObject mLeft;
    public GameObject mRight;
    public GameObject mTop;
    public GameObject mButtom;

    // 按钮
    public Button mClose;
    public Button mBack;

    public GameObject GetRoot(ePosition pos)
    {
        switch (pos)
        {
            case ePosition.Left:
                if (mLeft) return mLeft;
                break;
            case ePosition.Right:
                if (mRight) return mRight;
                break;
            case ePosition.Top:
                if (mTop) return mTop;
                break;
            case ePosition.Buttom:
                if (mButtom) return mButtom;
                break;
            case ePosition.Background:
                if (mBg) return mBg.gameObject;
                break;
            default:
                return null;
        }

        return null;
    }

    public GameObject AttachPrefab(string path, ePosition pos, int index = -1)
    {
        var root = GetRoot(pos);

        if (null == root)
        {
            return null;
        }

        var go = AssetLoader.instance.LoadResAsGameObject(path);

        if (null == go)
        {
            return null;
        }

        Utility.AttachTo(go, root);
        go.name = CFileManager.GetFullName(path);
        go.transform.SetSiblingIndex(index);

        return go;
    }

    public GameObject TopRoot()
    {
        return mTop;
    }

    //public void SetBackgroundImage(Sprite op)
    //{
    //    if (null != mBg)
    //    {
    //        mBg.sprite = op;
    //    }
    //}
    public void SetBackgroundImage(string spritePath)
    {
        if (null != mBg)
        {
            // mBg.sprite = op;
            ETCImageLoader.LoadSprite(ref mBg, spritePath);
        }
    }

    public void SetCloseImage(Sprite op)
    {
        if (null != mClose)
        {
            mClose.image.sprite = op;
        }
    }

    public void SetBackImage(Sprite op)
    {
        if (null != mBack)
        {
            mBack.image.sprite = op;
        }
    }

    public void OnClose(UnityAction action)
    {
        if (null != mClose)
        {
            mClose.onClick.AddListener(action);
        }
    }

    public void OnBack(UnityAction action)
    {
        if (null != mBack)
        {
            mBack.onClick.AddListener(action);
        }
    }
}
