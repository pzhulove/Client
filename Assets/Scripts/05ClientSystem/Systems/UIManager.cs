
using GameClient;
using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using Tenmove.Runtime;
using UnityEngine;

namespace GameClient
{
    public class UIManager : Singleton<UIManager>
    {
        protected Dictionary<ulong, LoadParam> mLoadObjectsDic = new Dictionary<ulong, LoadParam>();
        protected UIObjectPoolManager mPoolManager = new UIObjectPoolManager();//对象池管理器
        private Font mFont;
        private HashSet<ClientFrame> mScreenShotFrames;

        public UIManager()
        {
            GlobalEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FrameOpen, _OnFrameOpen);
            GlobalEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FrameClose, _OnFrameClose);
            SceneCapMask.Preload();
        }

        protected struct LoadParam
        {
            public ClientFrame Frame;
            public Action<string, object, object> SuccessCB;
            public object UserData;
        }

        public void LoadObject(ClientFrame frame, string path, object userData, Action<string, object, object> onSuccess, Type type, bool isFromPool = false, bool isAsync = true)
        {
            var obj = AssetLoader.GetInstance().LoadResAsGameObject(path);
            onSuccess?.Invoke(path, obj, userData);
        }

        public void LoadObject(ClientFrame frame, string path, object userData, AssetLoadCallbacks<object> callbacks, Type type, bool isFromPool = false, bool isAsync = true)
        {
            var obj = AssetLoader.GetInstance().LoadResAsGameObject(path);
            callbacks?.OnAssetLoadSuccess?.Invoke(path, obj, 0, 0, userData);
        }

        public GameObject GetPredefinedAnimationObject(string animationName)
        {
            return mPoolManager.GetPredefinedAnimationObject(animationName);
        }


        public void EnableScreenShot(ClientFrame frame)
        {
            if (frame == null)
            {
                return;
            }

            if (mScreenShotFrames == null)
            {
                mScreenShotFrames = new HashSet<ClientFrame>();
            }

            if (mScreenShotFrames.Contains(frame))
            {
                return;
            }

            mScreenShotFrames.Add(frame);
            GameFrameWork.instance.SetMainCamera(false);
            SceneCapMask.Enable();
        }

        public void DisableScreenShot(ClientFrame frame)
        {
            if (frame == null)
            {
                return;
            }

            if (mScreenShotFrames == null)
            {
                return;
            }

            if (!mScreenShotFrames.Contains(frame))
            {
                return;
            }

            mScreenShotFrames.Remove(frame);
            SceneCapMask.Disable();
            if (mScreenShotFrames.Count == 0)
            {
                GameFrameWork.instance.SetMainCamera(!_IsHaveFullFrame());
            }
        }

        public Font GetTextFont()
        {
            if (mFont == null)
            {
                var data = TableManager.GetInstance().GetTableItem<ClientConstValueTable>((int)ClientConstValueTable.eKey.FONT_NAME);
                string path = "UIFlatten/Font/FZZBHJW_3.ttf";
                if (data.StrParamsLength > 0)
                {
                    path = data.StrParamsArray(0);
                }
                var res = AssetLoader.GetInstance().LoadRes(path);
                if (res != null)
                {
                    mFont = res.obj as Font;
                }
            }
            return mFont;
        }

        void _OnFrameOpen(UIEvent uiEvent)
        {
            var type = uiEvent.Param2 as Type;
            if (null != type)
            {

            }
        }

        void _OnFrameClose(UIEvent uiEvent)
        {
            var frame = uiEvent.Param3 as ClientFrame;
            if (null != frame && mScreenShotFrames != null && mScreenShotFrames.Contains(frame))
            {
                mScreenShotFrames.Remove(frame);
                SceneCapMask.Disable();
                if (mScreenShotFrames.Count == 0)
                {
                    GameFrameWork.instance.SetMainCamera(!_IsHaveFullFrame());
                }
            }
        }

        bool _IsHaveFullFrame()
        {
            var frames = ClientSystemManager.GetInstance().GetAllFrames();
            if (frames == null)
            {
                return false;
            }

            foreach(var obj in frames.Values)
            {
                ClientFrame frame = obj as ClientFrame;
                if (frame.GetFrameType() == eFrameType.FullScreen)
                {
                    return true;
                }
            }

            return false;

        }
    }
}