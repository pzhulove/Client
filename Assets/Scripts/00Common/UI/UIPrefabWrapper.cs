using GameClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPrefabWrapper : MonoBehaviour {

    public string m_PrefabName;
	public int IntParam;

    [SerializeField] private bool mIsAutoLoaded = false;
    [SerializeField] private ComClientFrame mComClientFrame;

    private bool mIsLoaded = false;
    private GameObject mLoadGo = null;
    private Action<string, object, object> mCallback;
    private ClientFrame mClientFrame;

    private void Awake()
    {
        mCallback += _CallBack;

        if (mIsAutoLoaded)
        {
            Load();
        }
    }

    private void _CallBack(string str, object asset, object userData)
    {
        if (asset == null)
        {
            return;
        }

        mLoadGo = asset as GameObject;
        mLoadGo.transform.SetParent(transform, false);
    }

    private void _InitClentFrame()
    {
        if (mClientFrame == null)
        {
            if (mComClientFrame == null)
            {
                mComClientFrame = GetComponentInParent<ComClientFrame>();
            }
            if (mComClientFrame != null)
            {
                mClientFrame = mComClientFrame.GetClientFrame() as ClientFrame;
            }
        }
    }

    public void Load()
    {
        if (!mIsLoaded)
        {
            _InitClentFrame();
            UIManager.instance.LoadObject(mClientFrame, m_PrefabName, null, mCallback, typeof(GameObject));     //用这个接口加载方便替换异步，以前的接口改不改看后续
            mIsLoaded = true;
        }
    }

    public void SetCallback(Action<string, object, object> callBack)
    {
        if (mLoadGo != null)
        {
            callBack(m_PrefabName, mLoadGo, null);
        }

        if (mCallback != null)
        {
            mCallback += callBack;
        }
        else
        {
            mCallback = callBack;
        }
    }

    public GameObject LoadUIPrefab()
    {
        return AssetLoader.GetInstance().LoadResAsGameObject(m_PrefabName);
    }

    public GameObject LoadUIPrefab(Transform placeHolder)
    {
        var go = AssetLoader.GetInstance().LoadResAsGameObject(m_PrefabName);

	    if (go == null)
	    {
			Logger.LogError("加载预制体失败,路径:" + m_PrefabName);
	    }

        if (go != null && placeHolder != null)
        {
            go.name = placeHolder.name;
            go.transform.SetParent(placeHolder.transform.parent, false);
	        go.transform.localPosition = placeHolder.transform.localPosition;
	        go.transform.localScale = placeHolder.transform.localScale;
            go.transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
            GameObject.Destroy(placeHolder.gameObject);
        }

        return go;
    }
}
