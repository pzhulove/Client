using System;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Tenmove.Runtime;

namespace GameClient
{
	public class ComUIAsyncComponent
	{
	    public GameObject Obj { get; private set; }

	    private AssetLoadCallbacks<object> mCallbacks;
	    private Action<bool> _OnSetActive;
	    private Action<object> _OnSetValue;
	    private object mValue;
	    private Action<GameObject> _OnLoaded;
	    private ClientFrame mFrame;
	    private string mPath;
	    private bool mIsLoading = false;
	    private bool mIsActive = false;

        public ComUIAsyncComponent(string path, Action<bool> onSetActive, ClientFrame frame, Action<GameObject> onLoaded, Action<object> onSetValue = null)
	    {
	        mPath = path;
            _OnSetActive = onSetActive;
	        mFrame = frame;
	        _OnLoaded = onLoaded;
	        _OnSetValue = onSetValue;
            mCallbacks = new AssetLoadCallbacks<object>(_OnLoadSuccess, _OnLoadFailure);
	    }

	    public void Load()
	    {
	        if (Obj == null && !mIsLoading)
	        {
	            //UIManager.GetInstance().LoadObject(mFrame, mPath, typeof(GameObject), null, mCallbacks);
	        }
        }

        public void SetValue<T>(T value)
        {
            mValue = value;
            if (Obj == null && !mIsLoading)
            {
                Load();
            }
            else
            {
                if (_OnSetValue != null)
                {
                    _OnSetValue(mValue);
                }
            }
        }

        public void SetActive(bool value)
	    {
	        if (Obj != null && _OnSetActive != null)
	        {
	            _OnSetActive(value);
	        }
	        else
	        {
	            mIsActive = value;
	        }
	    }

	    private void _OnLoadSuccess(string path, object asset, int taskgrpid, float duration, object userdata)
	    {
	        Obj = asset as GameObject;
            // todo: 修改显隐
            if(Obj != null)
                Obj.SetActive(true);
            if (_OnLoaded != null)
	            _OnLoaded(Obj);  // todo: 此处_OnLoaded()项目中暂无引用

            if (mValue != null && _OnSetValue != null)
	        {
	            _OnSetValue(mValue);
	        }

            if (_OnSetActive != null)
	            _OnSetActive(mIsActive);
	        mIsLoading = false;
	    }

	    private void _OnLoadFailure(string path, int taskGrpID, AssetLoadErrorCode errorCode, string message, object userData)
	    {
	        mIsLoading = false;
	    }
    }
}