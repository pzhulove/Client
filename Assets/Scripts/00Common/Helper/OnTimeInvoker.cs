using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;
using System.Reflection;

namespace GameClient
{
    // 这个是用来在某个时刻调用某个方法的脚本
    // 一般用来在某个时间点刷新界面，比如每天6点刷新下活跃度数据
    // 目前只支持每天
    public class OnTimeInvoker : MonoBehaviour
    {
        [SerializeField]
        int hour = 6;

        [SerializeField]
        int minute = 0;

        [SerializeField]
        int second = 0;

        [SerializeField]
        string callName = ""; // 静态函数名称，需要fullname

        [SerializeField]
        bool needUpdate = false;

        MethodInfo mf = null;

        // Use this for initialization
        void Start()
        {
            needUpdate = false;
            mf = GetMethodInfoFromString(callName);
            TryInvoke();
        }

        private void OnDestroy()
        {
            needUpdate = false;
            mf = null;
        }

        void TryInvoke()
        {
            DateTime dateTime = TimeUtility.GetDateTimeByTimeStamp((int)TimeManager.GetInstance().GetServerTime());
            if (dateTime == null)
            {
                return;               
            }

            if (dateTime.Hour == hour && dateTime.Minute == minute && dateTime.Second == second)
            {
                if (mf != null)
                {
                    mf.Invoke(null, null);
                }

                needUpdate = false;
            }
            else
            {
                needUpdate = true;
            }
        }

        private void Update()
        {
            if(needUpdate)
            {
                TryInvoke();
            }
        }

        MethodInfo GetMethodInfoFromString(string methodStr)
        {
            string[] types = methodStr.Split(new char[] { '.' });
            if (types != null && types.Length >= 2)
            {
                string funcStr = types[types.Length - 1];
                string typeStr = "";
                for (int i = 0; i < types.Length - 1; i++)
                {
                    if (i != 0)
                    {
                        typeStr += ".";
                    }

                    typeStr += types[i];
                }

                Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
                Type type = assembly.GetType(typeStr);
                if(type == null)
                {
                    return null;
                }

                MethodInfo mf = type.GetMethod(funcStr);
                return mf;
            }

            return null;
        }
    }
}


