using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using DG.Tweening;
using System.Reflection;
using System.Collections;


namespace GameClient
{
    public class GuildActivityItem : MonoBehaviour
    {
        // Use this for initialization
        [SerializeField]
        Image icon = null;

        [SerializeField]
        Text name = null;

        [SerializeField]
        Text desc = null;

        [SerializeField]
        Text openTime = null;

        [SerializeField]
        Text limit = null;

        [SerializeField]
        Text state = null;

        [SerializeField]
        Button btnGo = null;

        [SerializeField]
        Text btnGoText = null;

        [SerializeField]
        GameObject redPoint = null;

        int id = 0;

        void Start()
        {
           
        }

        private void OnDestroy()
        {     
            id = 0;
        }

        private void OnDisable()
        {
            id = 0;
        }

        // Update is called once per frame
        void Update()
        {
            GuildActivityTable guildActivityTable = TableManager.GetInstance().GetTableItem<GuildActivityTable>(id);
            if (guildActivityTable != null)
            {
                MethodInfo updateUnLockFunc = GetMethodInfoFromString(guildActivityTable.activityUnLockCallBack);
                if (updateUnLockFunc != null && updateUnLockFunc.ReturnType == typeof(bool))
                {
                    bool bUnLock = (bool)updateUnLockFunc.Invoke(null, null);
                    limit.CustomActive(!bUnLock);
                    btnGo.SafeSetGray(!bUnLock);

                    if (bUnLock)
                    {
                        btnGoText.SafeSetText(guildActivityTable.btnDefaultText);
                    }
                    else
                    {
                        btnGoText.SafeSetText(TR.Value("guild_activity_unlock"));
                    }             
                }

                MethodInfo updateStateFunc = GetMethodInfoFromString(guildActivityTable.stateUpdateCallBack);
                if (updateStateFunc != null && updateStateFunc.ReturnType == typeof(string))
                {
                    string activityState = (string)updateStateFunc.Invoke(null, null);
                    state.SafeSetText(activityState);           
                }

                MethodInfo updateRedPointFunc = GetMethodInfoFromString(guildActivityTable.redPointUpdateCallBack);
                if (updateRedPointFunc != null && updateRedPointFunc.ReturnType == typeof(bool))
                {                 
                    redPoint.CustomActive((bool)updateRedPointFunc.Invoke(null, null));
                }
            }
        }
        
        static MethodInfo GetMethodInfoFromString(string methodStr)
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

                MethodInfo mf = type.GetMethod(funcStr);
                return mf;
            }

            return null;
        }

        public void SetUp(object data)
        {
            GuildActivityFrame.GuildActivityData guildActivityData = data as GuildActivityFrame.GuildActivityData;
            if(guildActivityData == null)
            {
                return;
            }

            id = guildActivityData.guildActivityTableID;

            GuildActivityTable guildActivityTable = TableManager.GetInstance().GetTableItem<GuildActivityTable>(guildActivityData.guildActivityTableID);
            if(guildActivityTable == null)
            {
                return;
            }

            icon.SafeSetImage(guildActivityTable.iconPath);
            name.SafeSetText(guildActivityTable.activityName);
            desc.SafeSetText(guildActivityTable.activityDesc);
            openTime.SafeSetText(guildActivityTable.openTime);
            limit.SafeSetText(guildActivityTable.activityUnLockConditon);      
            btnGoText.SafeSetText(guildActivityTable.btnDefaultText);
            state.SafeSetText("");

            limit.CustomActive(false);

            btnGo.SafeSetOnClickListener(() => 
            {
                MethodInfo methodInfo = GetMethodInfoFromString(guildActivityTable.btnCallBack);
                if(methodInfo != null)
                {
                    methodInfo.Invoke(null, null);
                }
            });

            return;
        }
    }
}


