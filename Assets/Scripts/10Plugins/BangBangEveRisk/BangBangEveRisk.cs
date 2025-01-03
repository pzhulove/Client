#if BANGCLE_EVERISK
using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using XUPorterJSON;

namespace Assets.BangBangEveRisk
{
    public class BangBangEveRisk : Singleton<BangBangEveRisk>
    {

        private static bool isEmulator = false;
        private static string emulatorType = "";

        public bool IsEnumlator
        {
            get {  return isEmulator; }
        }

        public string EmulatorType
        {
            get { return emulatorType.Replace(" ", ""); }
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject currentActivity;
        private static AndroidJavaObject applicationContext;
        private const string FullClassName = "com.bangcle.everisk.core.RiskStubAPI";
        private const string bangbangkey = "zFQQNLRW9JWKfd6rfCISuaiNP4LX77avFDKkFlr9/WOc0d3INORqJhAkpG//dhXFALP5VrYs5zBeRthAYdfa3JqKgXoNstVzxmqfmwMUcbcZgZjp6ysd4MJPTsU6KMu5Pf8ewuVKdoqX85su7jU/2JcBMGEqpmmsCE+7yoVi+qxmIFv5UwVR1sy90x+7WVqgkDxkZqhcqlPqK6KJqdTrGGd9eO6IebVqnNq4cjfOtZHDwP4ghQg3lUVPchRHG07Oc4yv7CP9AQ0tMlO3EqBN7pvXXLNQR28kkD2f68VFm7w1LmfzuThHsofJoO4JlbUaunGnKOmgrI+wxsWRycTr7Q==";

        const string EMULATOR_TIPS = "EMULATOR";

        class BangBangEveRiskCallBack : AndroidJavaProxy
        {
            public BangBangEveRiskCallBack() : base("com.bangcle.everisk.core.CallBack")
            {

            }
            public void onResult(AndroidJavaObject var1, AndroidJavaObject var2)
            {
                try
                {
                    string tempEnum = var1.Call<string>("toString");
                    if (tempEnum != null && tempEnum == EMULATOR_TIPS)
                    {
                        isEmulator = true;
						emulatorType = EMULATOR_TIPS;
                        if (var2 != null) 
                        {
                            string var2String = var2.Call<string>("toString");
                            int startIndex = var2String.IndexOf("\"name\":\"");
                            int endIndex = var2String.IndexOf("\",\"msg_id");
                            emulatorType = var2String.Substring(startIndex + 8, endIndex - startIndex - 8);
                        }
						ReportEmulator(emulatorType);
                    }
                }
                catch(Exception e)
                {
                    Debug.Log(e);
                }
            }

            public int hashCode()
            {
                return this.GetHashCode();
            }

            public bool equals(AndroidJavaObject obj)
            {
                if (obj == null || GetType() != obj.GetType()) return false;
                return this.GetHashCode() == obj.GetHashCode();
            }

            public string toString()
            {
                return this.ToString();
            }
        }
#endif

        public static void ReportEmulator(string type)
		{
            Dictionary<string, string> jsondic = new Dictionary<string, string>();
            string openuid = ClientApplication.playerinfo.openuid;
            if (string.IsNullOrEmpty(openuid))
            {
                openuid = "None Openuid";
            }
            jsondic.Add("openuid", openuid);
            jsondic.Add("emulator", type);
            string json = jsondic.toJson();
            
            string url = string.Format("http://{0}", Global.BANGBANGEVERISK_SERVICE_ADDRESS);
            Debug.Log("url ---->" + url);
            Http.SendPostRequest(url, json);
            Debug.Log("Finished Http SendPostRequest");
		}

        public void Init()
        {
            GetActivity();
            GetApplicationContext();
            InitEveriskPermission();
            InitBangcleEverisk();
        }

        public void InitEveriskPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (null == currentActivity)
            {
                return;
            }
            try
            {
                AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
                if (null != targetClass)
                {
                    bool temp = targetClass.CallStatic<bool>("initEveriskPermission", currentActivity);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useUsageStatsAlways">是否申请OPSTR_GET_USAGE_STATS权限，三种情况：1，参数不填，权限不申请；2，参数填true，权限被拒绝后每次启动都提醒获取权限；3，参数填false，权限被拒绝后以后不再提醒获取权限。</param>
        /// <returns></returns>
        public void InitEveriskPermission(bool useUsageStatsAlways)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if(null == currentActivity)
            {
                return ;
            }
            try
            {
                AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
                if(null !=targetClass)
                {
                    targetClass.CallStatic<bool>("initEveriskPermission", currentActivity, useUsageStatsAlways);
                }
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
#endif
        }

        public void SetEveriskPluginURL(string[] urlList)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (null == currentActivity)
            {
                return;
            }
            try
            {
                AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
                if (null != targetClass)
                {
                    targetClass.CallStatic<bool>("setEveriskPluginURL", urlList);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
#endif
        }

        public void SetEveriskBusinessURL(string[] urlList)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (null == currentActivity)
            {
                return;
            }
            try
            {
                AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
                if (null != targetClass)
                {
                    targetClass.CallStatic<bool>("setEveriskBusinessURL", urlList);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
#endif
        }

        public void InitBangcleEverisk()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (null == currentActivity)
            {
                return;
            }
            try
            {
                AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
                if (null != targetClass && null != applicationContext)
                {
                    bool temp = targetClass.CallStatic<bool>("initBangcleEverisk", applicationContext, bangbangkey);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
#endif
        }

        public string GetEveriskUdid()
        {
            string temp = "";
#if UNITY_ANDROID && !UNITY_EDITOR
            if (null == currentActivity)
            {
                return temp;
            }
            try
            {
                AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
                if (null != targetClass && null != applicationContext)
                {
                    temp = targetClass.CallStatic<string>("getEveriskUdid");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
#endif

#if UNITY_IOS && !UNITY_EDITOR
			temp = _GetEveRiskUdid ();
#endif

            Debug.Log ("GetEveriskUdid --->>> " + temp);
            return temp;
        }

        public void AddExtraUserData(string key, string value)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (null == currentActivity)
            {
                return;
            }
            try
            {
                AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
                if (null != targetClass)
                {
                    targetClass.CallStatic<bool>("addExtraUserData", key, value);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
#endif

#if UNITY_IOS && !UNITY_EDITOR
			bool res = _AddExtraUserData(key,value);
			if (res) {
				Debug.Log ("AddExtraUserData Succeed!!!");
			} 
			else 
			{
				Debug.Log ("AddExtraUserData Fail!!!");
			}
#endif

        }

        public void RegisterServiceSTART()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using(AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName))
            {
                using (AndroidJavaObject STARTValue = new AndroidJavaClass("com.bangcle.everisk.core.Type").GetStatic<AndroidJavaObject>("START")) 
                {
                    bool temp = targetClass.CallStatic<bool>("registerService", new BangBangEveRiskCallBack(), STARTValue);
                }
            }
#endif
        }

        public void RegisterServiceEMULATOR()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using(AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName))
            {
                using (AndroidJavaObject STARTValue = new AndroidJavaClass("com.bangcle.everisk.core.Type").GetStatic<AndroidJavaObject>("EMULATOR")) 
                {
                    bool temp = targetClass.CallStatic<bool>("registerService", new BangBangEveRiskCallBack(), STARTValue);
                }
            }
#endif
        }

        private void GetActivity()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if(null == currentActivity)
            {
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            }
#endif
        }

        private void GetApplicationContext()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (null == applicationContext)
            {
                applicationContext = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            }
#endif
        }


#if UNITY_IOS && !UNITY_EDITOR
		// --- dllimport start ---
		[DllImport("__Internal")]
		private static extern string _GetEveRiskUdid ();

		[DllImport("__Internal")]
		private static extern bool _AddExtraUserData(string key,string value);
#endif

    }
}
#endif