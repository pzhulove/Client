using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    // public interface ICustomClientFrameItem<T1,T2>
    // {
    //     void Create(T1 frame, GameObject parent);
    //     void Destroy();
    //     void RefreshView(T2 model);
    //     void Show();        //可能会有延迟显示的需求
    //     void Hide();
    //     T2 GetViewModel();
    // }

    public abstract class CustomClientFrameItem
    {        
        #region Model Params

        protected bool bInited = false;

        protected bool bShowed = false;
        public bool BShowed { get { return bShowed; } }
        
        #endregion
        
        #region View Params

        protected GameObject _mParentGo;
        protected GameObject _mSelfGo;
        protected ComCommonBind _mBind;
        
        #endregion
        
        #region PRIVATE METHODS

        protected abstract void _Init();

        protected abstract void _Clear();

        protected void _ClearBase()
        {
            _mParentGo = null;
            _mBind = null;
            _mSelfGo = null;

            bInited = false;
            bShowed = false;
        }

        #endregion
        
        #region  PUBLIC METHODS

        #endregion
    }
}