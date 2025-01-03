using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;


namespace GameClient
{   
    public interface IASyncOperation
    {
        void   SetProgress(float progress);
        float  GetProgress();
        void   SetError(string ErrorMsg);
        string GetErrorMessage();
        bool   IsError();

        void   SetProgressInfo(string info);
    }

    public interface IClientSystem
    {
        bool IsSystem<T>() where T : IClientSystem;

        string GetName();

        void SetName(string name);

        void GetExitCoroutine(AddCoroutine exit);

        void GetEnterCoroutine(AddCoroutine enter);

        void OnEnter();

        void OnExit();

        void OnStart(SystemContent systemContent);

        void Update(float timeElapsed);

        ClientSystem.eClientSystemState GetState();

        /// <summary>
        /// 开始进入系统前的初始化
        /// </summary>
        void BeforeEnter();
    }
}

