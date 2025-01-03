using UnityEngine;
using System.Collections;
using Network;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Reflection;

namespace  GameClient
{
    public enum eClientFrameStackCmd
    {
        OpenFrame,
        MissionTraceTarget,
    }

    public interface IClientFrameStackCmd
    {
        eClientFrameStackCmd CmdType();

        bool Do();
    }

    public class BaseClientFrameStackCmd
    {
        protected eClientFrameStackCmd mType;

        protected BaseClientFrameStackCmd(eClientFrameStackCmd type)
        {
            mType = type;
        }

        public eClientFrameStackCmd CmdType()
        {
            return mType;
        }
    }

    public class MissionTraceTargetCmd : IClientFrameStackCmd
    {
        protected int mMissionId;

        public MissionTraceTargetCmd(int missionId)
        {
            mMissionId = missionId;
        }

        public eClientFrameStackCmd CmdType()
        {
            return eClientFrameStackCmd.MissionTraceTarget;
        }

        public bool Do()
        {
            if (ClientSystemManager.GetInstance().CurrentSystem is ClientSystemTown)
            {
                MissionManager.GetInstance().AutoTraceTask(mMissionId);
            }
            return true;
        }
    }

    public class OpenClientFrameStackCmd : BaseClientFrameStackCmd, IClientFrameStackCmd
    {
        protected Type mFrame;
        protected object mData;

        public OpenClientFrameStackCmd(Type type, object data = null) : base (eClientFrameStackCmd.OpenFrame)
        {
            mFrame = type;
            mData = data;
        }

        public bool Do()
        {
            try
            {
                //ClientSystemManager.instance.OpenFrame(mFrame);
                if (ClientSystemManager.instance.CurrentSystem is ClientSystemTown)
                {
                    ClientSystemManager.instance.OpenFrame(mFrame, FrameLayer.Middle, mData);
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                return false;
            }
        }
    }

    public interface IClientSystemFrameStack
    {
        void Push2FrameStack(IClientFrameStackCmd cmd);

        void ClearFrameStack();
    }
}
