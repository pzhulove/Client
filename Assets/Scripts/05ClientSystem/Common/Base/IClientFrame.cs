using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;

namespace GameClient
{
    public enum FrameLayer
    {
        Invalid = -1,
        Background = 0,
        Bottom,
        BelowMiddle,
        Middle,
        HorseLamp,
        High,
        Top,
        TopMost,
        TopMoreMost,

        LayerMax
    }

    public interface IClientFrameManager
    {
        IClientFrame OpenFrame<T>(FrameLayer layer, object userData = null, string name = "") where T : class, IClientFrame;

        IClientFrame OpenFrame(Type t, FrameLayer layer, object userData = null, string name = "");

        IClientFrame OpenFrame<T>(GameObject root, object userData = null, string name = "") where T : class, IClientFrame;

        IClientFrame OpenFrame(Type type, GameObject root, object userData = null, string name = "", FrameLayer layer = FrameLayer.Invalid);

        void CloseFrame<T>(T frame = null, bool bImmediately = false) where T : class, IClientFrame;

        //void CloseFrame(Type type, bool bImmediately = false);
		void CloseFrameByType(Type type, bool bImmediately = false);

        bool IsFrameOpen<T>(T frame = null) where T : class, IClientFrame;

        bool IsFrameOpen(Type type);

        bool IsFrameHidden(Type type);

		bool IsFrameOpen(string name);

		IClientFrame GetFrame(string name);

        IClientFrame GetFrame(Type type);

        void ShowFrame(Type targetFrameType, Type currentFrameType, bool bShow);

        void ShowAllFrame(Type currentFrameType, bool bShow);

        void OnFrameClose(IClientFrame frame,bool removeRef);
    }

    public enum EFrameState
    {
        Close = 0,
        FadeIn,
        Open,
        FadeOut,
        Hidden,
    }

    public enum EFadeDelayState
    {
        EFDS_INVALID = 0,
        EFDS_IN,
        EFDS_OUT,
    }

    public interface IClientFrame
    {
        void Init();
        void Clear();

        void Open(GameObject root, object userData = null, FrameLayer layer = FrameLayer.Invalid);

        void Close(bool bCloseImmediately = false);

        void Update(float timeElapsed);

        void Show(bool isShow, Type type = null);

        bool IsOpen();

        bool IsHidden();

        bool IsNeedUpdate();

        bool IsGlobal();

        void SetManager(IClientFrameManager mgr);

        EFrameState GetState();

        void UpdateRoot();

        string GetFrameName();

        void SetFrameName(string name);

        void SetGlobal(bool isGlobal);

        string GetGroupTag();

        FrameLayer GetLayer();

        bool   NeedMutex();

        bool IsNeedClearWhenChangeScene();

        bool IsFullScreenFrameNeedBeClose();

        IEnumerator LoadingOpenPost();

        int GetSiblingIndex();

        void SetSiblingIndex(int index);

        GameObject GetFrame();

        eFrameType GetFrameType();
    }
}
