using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine.UI;
using Network;
using System;
using Protocol;
using ProtoTable;

namespace AdsPush
{
    public class LoginPushManager : Singleton<LoginPushManager>
    {
        public delegate void FinishCallBack();

        private int PushIndex = 0;
        private bool FirstRoot = false;
        public bool HavePush = false;
        bool OpenNextPush = false;
        float TimeIntrval = 0.0f;

        public class LoginPushData
        {
            /// <summary>
            ///  名称
            /// </summary>
            public string name;
            /// <summary>
            ///  图标路径
            /// </summary>
            public string iconPath;
            /// <summary>
            ///  链接位置
            /// </summary>
            public string linkInfo;
            /// <summary>
            ///  loading预制体路径
            /// </summary>
            public string loadingIconPath;
            /// <summary>
            ///  排序序号
            /// </summary>
            public byte sortNum;
            /// <summary>
            ///  解锁等级
            /// </summary>
            public UInt16 unlockLevel;

            /// <summary>
            /// 是否设置背景图片原比例
            /// </summary>
            public int IsSetNative;

            public int startTime;

            public int endTime;

            public bool needTime;
        }

        public List<LoginPushData> LoginPushList = new List<LoginPushData>();

        private List<LoginPushData> FinalLoginPushList = new List<LoginPushData>();

        private FinishCallBack callback = null;

        public FinishCallBack Callback
        {
            set
            {
                callback = value;
            }
            get
            {
                return callback;
            }
        }


        public override void Init()
        {
            UnInit();
            PushIndex = 0;
            FinalLoginPushList.Clear();
            OpenNextPush = false;
            TimeIntrval = 0.0f;
            NetProcess.AddMsgHandler(SyncPlayerLoginStatus.MsgID, OnSyncPlayerLoginStatus);
        }

        public override void UnInit()
        {
            FinalLoginPushList.Clear();
            OpenNextPush = false;
            TimeIntrval = 0.0f;
            NetProcess.RemoveMsgHandler(SyncPlayerLoginStatus.MsgID, OnSyncPlayerLoginStatus);
        }

        public void ClearPushList()
        {
            LoginPushList.Clear();
        }

        public void SetLoginPushList(List<LoginPushData> DataList)
        {
#if APPLE_STORE
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.ADS_PUSH))
            {
                return;
            }
#endif
            LoginPushList.Clear();
            PushIndex = 0;
            for (int i = 0; i < DataList.Count; i++)
            {
                LoginPushData loginPushData = new LoginPushData();
                loginPushData.name = DataList[i].name;
                loginPushData.iconPath = DataList[i].iconPath;
                loginPushData.linkInfo = DataList[i].linkInfo;
                loginPushData.loadingIconPath = DataList[i].loadingIconPath;
                loginPushData.sortNum = DataList[i].sortNum;
                loginPushData.unlockLevel = DataList[i].unlockLevel;
                loginPushData.startTime = DataList[i].startTime;
                loginPushData.endTime = DataList[i].endTime;
                loginPushData.needTime = DataList[i].needTime;
                loginPushData.IsSetNative= DataList[i].IsSetNative;
                LoginPushList.Add(loginPushData);
            }
            if(LoginPushList.Count>=2)
            {
                SortLoginPushList();
            }
        }

        public void SortLoginPushList()
        {
            LoginPushList.Sort((x, y) =>
            {
                int result;
                if (x.sortNum.CompareTo(y.sortNum) < 0)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
                return result;
            });
        }

        public string GetLoadingIconPath()
        {
#if APPLE_STORE
            //add for ios appstore
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.ADS_PUSH))
            {
                return "";
            }
#endif
            if(FinalLoginPushList!=null && FinalLoginPushList.Count != 0)
            {
                return GetFirstEffectiveIconPath();
            }
            else
            {
                return "";
            }
            
        }

        private string GetFirstEffectiveIconPath()
        {
            // 这里有一个潜规则,取数组里的第一组数据作为loading内容是因为SortLoginPushList()函数已经排过序了，保证了数组的第一组数据就是最先推送的活动内容，sortNum越小越靠前
            // 所以，要想把指定的活动背景图作为loading图，就要把它的推送排序sortNum设置为最小值即可
            for (int i = 0; i < FinalLoginPushList.Count; i++)
            {
                if (FinalLoginPushList[i].sortNum <= 0|| FinalLoginPushList[i].unlockLevel>PlayerBaseData.GetInstance().Level)
                {
                    continue;
                }
                else
                {
                    return FinalLoginPushList[i].loadingIconPath;
                }
            }
            return "";
        }

        /// <summary>
        /// get Icon Path From index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetPushIconPath()
        {
            if(PushIndexIsOutOfRange())
            {
                Logger.LogErrorFormat("PushIndex is out of range");
                return "";
            }
            return FinalLoginPushList[PushIndex].iconPath;
            
        }

        /// <summary>
        /// 是否设置背景图片的原比例
        /// </summary>
        /// <returns></returns>
        public bool IsSetNative()
        {
            if (PushIndexIsOutOfRange())
            {
                return false;
            }
            int isSetNative = FinalLoginPushList[PushIndex].IsSetNative;
            return isSetNative == 1;
        }

        public string GetPushTime()
        {
            if (PushIndexIsOutOfRange())
            {
                return null;
            }
            if(FinalLoginPushList[PushIndex].needTime)
            {
                return Function.GetMonthDate(FinalLoginPushList[PushIndex].startTime, FinalLoginPushList[PushIndex].endTime);
            }
            else
            {
                return null;
            }
        }

        public Type GetCurrAdsDataFrameType(string framelink)
        {
            if (string.IsNullOrEmpty(framelink))
                return null;
            var regex = new System.Text.RegularExpressions.Regex(@"<type=framename value=(.+)>");
            bool isMatch = regex.IsMatch(framelink);
            if (isMatch == false)
                return null;
            var match = regex.Match(framelink);
            if (!string.IsNullOrEmpty(match.Groups[0].Value))
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                Type type = assembly.GetType(match.Groups[1].Value);
                return type;
            }
            return null;
        }
        public Type getLinkType()
        {
            if(PushIndexIsOutOfRange())
            {
                return null;
            }
            var linkType = GetCurrAdsDataFrameType(FinalLoginPushList[PushIndex].linkInfo);
            return linkType;
        }
        private bool CheckFramesOpen(Type frameType)
        {
            if (frameType == null)
                return false;
            if (frameType == typeof(ActivityJarFrame))
            {
                bool hasJar = JarDataManager.GetInstance().HasActivityJar() && Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.ActivityJar);
                if (hasJar)
                {
                    return true;
                }
            }
            return false;
        }
        public bool EffectiveData()
        {
            if(PushIndexIsOutOfRange())
            {
                Logger.LogErrorFormat("PushIndex is out of range");
                return false;
            }
            if (FinalLoginPushList[PushIndex].linkInfo != "" && FinalLoginPushList[PushIndex].linkInfo != "-")
            {
                var linkType = GetCurrAdsDataFrameType(FinalLoginPushList[PushIndex].linkInfo);
                if(!CheckFramesOpen(linkType))
                {
                    return false;
                }
            }
            return true;
        }

        void OpenLoginPushFrame()
        {
            if(!IsFirstLogin())
            {
                if (Callback != null)
                {
                    Callback();
                    Callback = null;
                }

                return;
            }

            if(PushIndexIsOutOfRange())
            {
                return;
            }

            TimeIntrval = 0.0f;
            OpenNextPush = true;
        }

         void FliterFinalLoginPushList()
        {
            FinalLoginPushList.Clear();
            for (int i = 0; i < LoginPushList.Count; i++)
            {
                if(LoginPushList[i].iconPath == "-" || LoginPushList[i].iconPath == "")
                {
                    continue;
                }
                if(LoginPushList[i].unlockLevel > PlayerBaseData.GetInstance().Level)
                {
                    continue;
                }

                FinalLoginPushList.Add(LoginPushList[i]);
            }
        }

        public int getUnlockLevel()
        {
            if(PushIndexIsOutOfRange())
            {
                return 0;
            }
            return FinalLoginPushList[PushIndex].unlockLevel;
        }

        public void SetFirstRoot(bool flag)
        {
            FirstRoot = flag;
        }
        public bool IsFirstLogin()
        {
            return FirstRoot;
        }

        public void TryOpenLoginPushFrame()
        {
            if(PushIndex == 0)
            {
                FliterFinalLoginPushList();
            }
            if(PushIndexIsOutOfRange())
            {
                if (Callback != null)
                {
                    Callback();
                    Callback = null;
                }

                return;
            }
            if (!EffectiveData())
            {
                return;
            }

            if (getUnlockLevel() > PlayerBaseData.GetInstance().Level)
            {
                return;
            }
#if APPLE_STORE
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.ADS_PUSH))
            {
                return;
            }
#endif
            OpenLoginPushFrame();
        }
        void OnSyncPlayerLoginStatus(MsgDATA msg)
        {
#if APPLE_STORE
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.ADS_PUSH))
            {
                return;
            }
#endif
            SyncPlayerLoginStatus ret = new SyncPlayerLoginStatus();
            ret.decode(msg.bytes);

            if(ret.playerLoginStatus == (byte)PlayerLoginStatus.PLS_FIRST_LOGIN_DAILY)
            {
                SetFirstRoot(true);
                HavePush = true;
            }
            else
            {
                SetFirstRoot(false);
                HavePush = false;
            }
        }


        public void OnUpdate(float timeElapsed)
        {
            if(OpenNextPush)
            {
                TimeIntrval += timeElapsed;

                if (TimeIntrval > 0.3f)
                {
                    if (!PushIndexIsOutOfRange())
                    {
                        ClientSystemManager.GetInstance().OpenFrame<LoginPushFrame>(FrameLayer.Middle);

                        OpenNextPush = false;
                        PushIndex++;

                        TimeIntrval = 0.0f;
                    }
                }
            }
        }

        private bool PushIndexIsOutOfRange()
        {
            return (PushIndex < 0 || PushIndex >= FinalLoginPushList.Count);
        }
    }
}
    
