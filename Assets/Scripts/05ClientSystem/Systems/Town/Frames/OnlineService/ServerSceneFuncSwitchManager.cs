using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using Network;

namespace GameClient
{
    public class ServerSceneFuncSwitch
    {
        public Protocol.ServiceType sType;
        public bool sIsOpen;
    }

    /// <summary>
    /// 场景功能开关管理器  服务器控制
    /// </summary>
    public class ServerSceneFuncSwitchManager : DataManager<ServerSceneFuncSwitchManager>
    {
        public delegate void ServerSceneFuncSwitchHandler(ServerSceneFuncSwitch funcSwitch);

        private List<Protocol.ServiceType> mServerCloseFuncTypeList = new List<Protocol.ServiceType>();
        private Dictionary<int, List<ServerSceneFuncSwitchHandler>> mServerFuncSwitchListenerDic = new Dictionary<int, List<ServerSceneFuncSwitchHandler>>();

        #region PUBLIC_METHODS
        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.ServerSceneFuncSwitchManager;
        }
        public sealed override void Initialize()
        {
            Clear();
            BindNetEvent();
        }

        public sealed override void Clear()
        {
            UnBindNetEvent();
            mServerCloseFuncTypeList.Clear();
            mServerFuncSwitchListenerDic.Clear();
        }

        /// <summary>
        /// 服务器开关类型 功能是否关闭 返回为True是关闭
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Obsolete("IsTypeFuncLock is Obsolete, Please use IsServiceTypeSwitchOpen")]
        public bool IsTypeFuncLock(Protocol.ServiceType type)
        {
            if (mServerCloseFuncTypeList == null)
            {
                return false;
            }
            if (mServerCloseFuncTypeList.Contains(type))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 表示该类型开关 是否打开
        /// </summary>
        /// <param name="type">服务器下发类型</param>
        /// <returns> true 表示 开关：开</returns>
        public bool IsServiceTypeSwitchOpen(Protocol.ServiceType type)
        {
            if (mServerCloseFuncTypeList == null)
            {
                return true;
            }
            if (mServerCloseFuncTypeList.Contains(type))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 添加 服务器开关类型的 监听
        /// 
        /// 注意调用对象和当前管理器的初始化先后顺序 ！
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public void AddServerFuncSwitchListener(Protocol.ServiceType type, ServerSceneFuncSwitchHandler handler)
        {
            if (mServerFuncSwitchListenerDic == null)
            {
                return;
            }
            List<ServerSceneFuncSwitchHandler> mServerFuncSwitchHandlerList = null;
            int tKey = (int)type;
            if (!mServerFuncSwitchListenerDic.ContainsKey(tKey))
            {
                mServerFuncSwitchListenerDic.Add(tKey, new List<ServerSceneFuncSwitchHandler>(){handler});
            }
            else
            {
                mServerFuncSwitchHandlerList = mServerFuncSwitchListenerDic[tKey];
                if (mServerFuncSwitchHandlerList == null)
                {
                    return;
                }
                if (!mServerFuncSwitchHandlerList.Contains(handler))
                {
                    mServerFuncSwitchHandlerList.Add(handler);
                }
            }
        }

        /// <summary>
        /// 移除 服务器开关类型的 监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public void RemoveServerFuncSwitchListener(Protocol.ServiceType type, ServerSceneFuncSwitchHandler handler)
        {
            if (mServerFuncSwitchListenerDic == null)
            {
                return;
            }
            int tKey = (int)type;
            if (mServerFuncSwitchListenerDic.ContainsKey(tKey))
            {
                var switchHandlerList = mServerFuncSwitchListenerDic[tKey];
                if (switchHandlerList == null)
                {
                    return;
                }
                if (switchHandlerList.Contains(handler))
                {
                    switchHandlerList.Remove(handler);
                }
            }
        }

        /// <summary>
        /// 移除全部类型监听
        /// </summary>
        private void RemoveAllServerFuncSwitchListener()
        {
            if (mServerFuncSwitchListenerDic == null)
            {
                return;
            }
            mServerFuncSwitchListenerDic.Clear();
        }

        #endregion


        #region PRIVATE_METHODS
        private void BindNetEvent()
        {
            NetProcess.AddMsgHandler(SceneSyncServiceSwitch.MsgID, OnSceneSyncServiceSwitch);
            NetProcess.AddMsgHandler(SceneUpdateServiceSwitch.MsgID, OnSceneUpdateServiceSwitch);
        }

        private void UnBindNetEvent()
        {
            NetProcess.RemoveMsgHandler(SceneSyncServiceSwitch.MsgID, OnSceneSyncServiceSwitch);
            NetProcess.RemoveMsgHandler(SceneUpdateServiceSwitch.MsgID, OnSceneUpdateServiceSwitch);
        }

        #region EVENT CALLBACK

        private void OnSceneSyncServiceSwitch(MsgDATA data)
        {
            //Logger.LogError("[SceneSyncServiceSwitch 2]");
            SceneSyncServiceSwitch syncSwitch = new SceneSyncServiceSwitch();
            syncSwitch.decode(data.bytes);
            var syncClosedSwitches = syncSwitch.closedServices;
            if (syncClosedSwitches == null)
            {
                //Logger.LogError("Server sync closed func array is null");
                return;
            }
            for (int i = 0; i < syncClosedSwitches.Length; i++)
            {
                if (!Enum.IsDefined(typeof(Protocol.ServiceType), (int)syncClosedSwitches[i]))
                {
                    //Logger.LogError("Enum : ServerType do not contain num : " + syncClosedSwitches[i]);
                    continue;
                }
                Protocol.ServiceType sType = (Protocol.ServiceType)syncClosedSwitches[i];
                if (!mServerCloseFuncTypeList.Contains(sType))
                {
                    mServerCloseFuncTypeList.Add(sType);
                }
            }
        }

        private void OnSceneUpdateServiceSwitch(MsgDATA data)
        {
            //Logger.LogError("[SceneUpdateServiceSwitch 2]");
            SceneUpdateServiceSwitch updateSwitch = new SceneUpdateServiceSwitch();
            updateSwitch.decode(data.bytes);
            if (!Enum.IsDefined(typeof(Protocol.ServiceType), (int)updateSwitch.type))
            {
                //Logger.LogError("Enum : ServerType do not contain num : "+updateSwitch.type);
            }
            Protocol.ServiceType sType = (Protocol.ServiceType)updateSwitch.type;
            bool isSwitchOn = updateSwitch.open == 1 ? true : false;
            if (mServerCloseFuncTypeList.Contains(sType) && isSwitchOn)
            {
                mServerCloseFuncTypeList.Remove(sType);
            }
            else if (!mServerCloseFuncTypeList.Contains(sType) && !isSwitchOn)
            {
                mServerCloseFuncTypeList.Add(sType);
            }
            
            TriggerServerFuncSwitchListener(sType, isSwitchOn);
        }

        #endregion

        /// <summary>
        /// 触发 开关类型的 监听事件
        /// </summary>
        /// <param name="type"></param>
        private void TriggerServerFuncSwitchListener(Protocol.ServiceType type, bool isOpen)
        {
            if (mServerFuncSwitchListenerDic == null)
            {
                return;
            }
            int tKey = (int)type;
            if (mServerFuncSwitchListenerDic.ContainsKey(tKey))
            {
                List<ServerSceneFuncSwitchHandler> handlerList = mServerFuncSwitchListenerDic[tKey];
                if (handlerList == null)
                {
                    return;
                }
                for (int i = 0; i < handlerList.Count; i++)
                {
                    ServerSceneFuncSwitchHandler handler = handlerList[i];
                    if (handler != null)
                    {
                        handler(new ServerSceneFuncSwitch() { sType = type, sIsOpen = isOpen});
                    }
                }
            }
        }

        #endregion

    }

}