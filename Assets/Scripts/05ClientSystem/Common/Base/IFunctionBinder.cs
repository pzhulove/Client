using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    public interface IFunctionBinder
    {
        void install(IClientFrame clientFrame, GameObject frame);
        void refresh();
        void uninstall();
    }

    class FunctionBinder<T> : IFunctionBinder where T : class,IClientFrame,new()
    {
        protected T clientFrame;
        protected GameObject frame;
        public void install(IClientFrame clientFrame,GameObject frame)
        {
            this.clientFrame = clientFrame as T;
            this.frame = frame;
            Initialize();
        }

        public void refresh()
        {
            Refresh();
        }

        public void uninstall()
        {
            UnInitialize();
            frame = null;
        }

        protected virtual void Initialize()
        {

        }

        protected virtual void Refresh()
        {

        }

        protected virtual void UnInitialize()
        {

        }
    }
};