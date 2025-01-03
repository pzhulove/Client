using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;

namespace GameClient
{
    interface ActiveFrame
    {
        void OnUpdate();
        void OnCreate();
        void OnDestroy();
    }

    class ActiveSpecialFrame : ActiveFrame
    {
        IClientFrame parent = null;
        public void Intialize(IClientFrame parent,GameObject frame,int iActiveId)
        {
            this.parent = parent;
            this.frame = frame;
            this.iActiveId = iActiveId;
        }
        public void UnInitialize()
        {
            this.parent = null;
            this.frame = null;
            this.iActiveId = 0;
        }

        protected GameObject frame = null;
        int iActiveId = 0;
        public int ActiveID
        {
            get
            {
                return iActiveId;
            }
        }
        public ActiveManager.ActiveData data = null;
        public Protocol.ActivityInfo activityInfo = null;

        public virtual void OnUpdate()
        {
            //Logger.LogErrorFormat("OnUpdate");
        }

        public virtual void OnCreate()
        {
            //Logger.LogErrorFormat("OnCreate");
        }

        public virtual void OnDestroy()
        {
            //Logger.LogErrorFormat("OnDestroy");
        }
    }
}