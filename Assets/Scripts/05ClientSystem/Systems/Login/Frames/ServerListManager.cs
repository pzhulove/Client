using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;
using Protocol;
using Network;
using System.Net;
using System.IO;
using System.Text;
using XUPorterJSON;

namespace GameClient
{

    class ServerAddressManager : Singleton<ServerAddressManager>
    {
        public override void Init()
        {

        }

        void TrySetAddress(ref string dest, Hashtable table, string key)
        {
            if (table != null && !string.IsNullOrEmpty(key))
            {
                if (table.ContainsKey(key))
                {
                    var value = table[key];

                    if (value != null)
                    {
                        dest = value.ToString();
                    }
                }

            }
        }
        public void GetServerList()
        {

#if _TEST_MD5_
            Global.ROLE_SAVEDATA_SERVER_ADDRESS = "120.132.26.173:59765";
#endif 


            if (!Global.Settings.isUsingSDK)
               return;


            string SPEICY_FILE_NAME = "serverList_hack.xml";

#if MG_TEST || MG_TEST2
            SPEICY_FILE_NAME = "serverList_mgtest_hack.xml";
#endif

            byte[] data = null;
            if (FileArchiveAccessor.LoadFileInPersistentFileArchive(SPEICY_FILE_NAME, out data))
            {
                string content = System.Text.ASCIIEncoding.Default.GetString(data);
                var serverList = MiniJSON.jsonDecode(content) as Hashtable;
                try
                {
                    TrySetAddress(ref Global.STATISTIC_SERVER_ADDRESS, serverList, "statistic");
                    TrySetAddress(ref Global.VERIFY_BIND_PHONE_ADDRESS, serverList, "bindphone");
                    
#if UNITY_ANDROID
                    TrySetAddress(ref Global.STATISTIC_SERVER_ADDRESS,serverList,"android_statistic");
                    TrySetAddress(ref Global.PUBLISH_SERVER_ADDRESS,serverList,"android_publish");
                    TrySetAddress(ref Global.ROLE_SAVEDATA_SERVER_ADDRESS,serverList,"android_rolesave");
                    TrySetAddress(ref Global.LOGIN_SERVER_ADDRESS,serverList,"android_loginvef");
                    TrySetAddress(ref Global.ANDROID_MG_CHARGE,serverList,"android_mg_charge");
                    TrySetAddress(ref Global.ROLE_SAVEDATA_SERVER_ADDRESS_HW, serverList, "android_hw_rolesave");

					TrySetAddress(ref Global.USER_AGREEMENT_SERVER_ADDRESS,serverList,"android_user_agreement");
					
#elif UNITY_IOS || UNITY_IPHONE
                    TrySetAddress(ref Global.PUBLISH_SERVER_ADDRESS,serverList,"publish");
                    TrySetAddress(ref Global.ROLE_SAVEDATA_SERVER_ADDRESS,serverList,"rolesave");
                    TrySetAddress(ref Global.LOGIN_SERVER_ADDRESS,serverList,"loginvef");
                    TrySetAddress(ref Global.IOS_ZY_CHARGE,serverList,"ios_zy_charge");

					TrySetAddress(ref Global.USER_AGREEMENT_SERVER_ADDRESS,serverList,"ios_user_agreement");

                    TrySetAddress(ref Global.IOS_BANQUAN_ADDRESS, serverList, "ios_banquan");

#endif
                    TrySetAddress(ref Global.VOICE_SERVER_ADDRESS, serverList, "voice");
                    TrySetAddress(ref Global.ROLE_QUERY_OPENID_ADDRESS, serverList, "queryopenid");

					TrySetAddress(ref Global.ONLINE_SERVICE_ADDRESS, serverList, "online_service");
					TrySetAddress(ref Global.ONLINE_SERVICE_REQ_ADDRESS, serverList, "online_service_unread");

                    TrySetAddress(ref Global.ONLINE_SERVICE_VIP_CHECK_ADDRESS, serverList, "online_service_vip_auth");
                    TrySetAddress(ref Global.BANGBANGEVERISK_SERVICE_ADDRESS, serverList, "bangbangeverisk_post_address");

                    TrySetAddress(ref Global.RECORDLOG_GET_ADDRESS, serverList, "record_get_address");
                    TrySetAddress(ref Global.RECORDLOG_POST_ADDRESS, serverList, "record_post_address");

                    TrySetAddress(ref Global.BATTLE_PERFORMANCE_POST_ADDRESS, serverList, "battle_perf_post_address");
                }
                catch (Exception e)
                {
                    Logger.LogErrorFormat("serverList_hack.xml出错 {0}", e.ToString());
                }

                //华为平台服务器特殊设置
                if (Global.Settings.sdkChannel == SDKChannel.HuaWei)
                {
                    Global.ROLE_SAVEDATA_SERVER_ADDRESS = Global.ROLE_SAVEDATA_SERVER_ADDRESS_HW;
                }

                return;
            }


            {
                string filePath = "Environment/" + SDKInterface.Instance.GetServerListName();


#if MG_TEST || MG_TEST2
                filePath = "Environment/serverList_mgtest.xml";
#endif

                UnityEngine.Object obj = AssetLoader.instance.LoadRes(filePath).obj;
                if (obj == null)
                {
                    return;
                }

                string content = System.Text.ASCIIEncoding.Default.GetString((obj as TextAsset).bytes);
                var serverList = MiniJSON.jsonDecode(content) as Hashtable;
                try
                {
                    TrySetAddress(ref Global.STATISTIC_SERVER_ADDRESS, serverList, "statistic");
                    TrySetAddress(ref Global.VERIFY_BIND_PHONE_ADDRESS, serverList, "bindphone");
                    
#if UNITY_ANDROID
                    TrySetAddress(ref Global.STATISTIC_SERVER_ADDRESS,serverList,"android_statistic");
                    TrySetAddress(ref Global.PUBLISH_SERVER_ADDRESS,serverList,"android_publish");
                    TrySetAddress(ref Global.ROLE_SAVEDATA_SERVER_ADDRESS,serverList,"android_rolesave");
                    TrySetAddress(ref Global.LOGIN_SERVER_ADDRESS,serverList,"android_loginvef");
                    TrySetAddress(ref Global.ANDROID_MG_CHARGE, serverList, "android_mg_charge");
                    TrySetAddress(ref Global.ROLE_SAVEDATA_SERVER_ADDRESS_HW, serverList, "android_hw_rolesave");

					TrySetAddress(ref Global.USER_AGREEMENT_SERVER_ADDRESS,serverList,"android_user_agreement");

#elif UNITY_IOS || UNITY_IPHONE
                    TrySetAddress(ref Global.PUBLISH_SERVER_ADDRESS,serverList,"publish");
                    TrySetAddress(ref Global.ROLE_SAVEDATA_SERVER_ADDRESS,serverList,"rolesave");
                    TrySetAddress(ref Global.LOGIN_SERVER_ADDRESS,serverList,"loginvef");
                    TrySetAddress(ref Global.IOS_ZY_CHARGE,serverList,"ios_zy_charge");

					TrySetAddress(ref Global.USER_AGREEMENT_SERVER_ADDRESS,serverList,"ios_user_agreement");
					
					TrySetAddress(ref Global.IOS_BANQUAN_ADDRESS, serverList, "ios_banquan");

#endif
                    TrySetAddress(ref Global.VOICE_SERVER_ADDRESS, serverList, "voice");
                    TrySetAddress(ref Global.ROLE_QUERY_OPENID_ADDRESS, serverList, "queryopenid");
                    TrySetAddress(ref Global.STATISTIC2_SERVER_ADDRESS, serverList, "statistic2");

					TrySetAddress(ref Global.ONLINE_SERVICE_ADDRESS, serverList, "online_service");
					TrySetAddress(ref Global.ONLINE_SERVICE_REQ_ADDRESS, serverList, "online_service_unread");

                    TrySetAddress(ref Global.ONLINE_SERVICE_VIP_CHECK_ADDRESS, serverList, "online_service_vip_auth");
                    TrySetAddress(ref Global.BANGBANGEVERISK_SERVICE_ADDRESS, serverList, "bangbangeverisk_post_address");

                    TrySetAddress(ref Global.RECORDLOG_GET_ADDRESS, serverList, "record_get_address");
                    TrySetAddress(ref Global.RECORDLOG_POST_ADDRESS, serverList, "record_post_address");

                    TrySetAddress(ref Global.BATTLE_PERFORMANCE_POST_ADDRESS, serverList, "battle_perf_post_address");
                }
                catch (Exception e)
                {
                    Logger.LogErrorFormat("读取serverList.xml出错 {0}", e.ToString());
                }
            }


            //华为平台服务器特殊设置
            if (Global.Settings.sdkChannel == SDKChannel.HuaWei)
            {
                Global.ROLE_SAVEDATA_SERVER_ADDRESS = Global.ROLE_SAVEDATA_SERVER_ADDRESS_HW;
            }

#if TESTIN
            Global.ROLE_SAVEDATA_SERVER_ADDRESS = "101.37.173.236:58700";
#endif
        }

    }

    class WaitHttpRequest : BaseWaitHttpRequest
    {
        private void _setUrl(string op)
        {
            string url = string.Format("http://{0}/{1}", Global.ROLE_SAVEDATA_SERVER_ADDRESS, op);

            if (url.Contains("?"))
            {
                url += "&";
            }
            else
            {
                url += "?";
            }

            string openid = SDKInterface.Instance.NeedUriEncodeOpenid(ClientApplication.playerinfo.openuid);

            url += string.Format("version={0}&openid={1}", VersionManager.instance.Version(), openid);

            this.url = url;
        }

        public WaitHttpRequest(string op)
        {
            _setUrl(op);
            SetRequestWaitResult();

            UnityEngine.Debug.LogFormat("[WaitHttpRequest] 开始 url {0}, {1}", url, mResult);
        }
    }

    //public ServerListDataManager : BaseData
    public class ServerListManager : Singleton<ServerListManager>
    {
        private const string mBodyName = "body";

        /// 以下是必须需要的数据, 如果没有

        /// <summary>
        /// 大区列表
        /// </summary>
        public ArrayList tabs { private set; get; }

        /// <summary>
        /// 所有账户列表
        /// </summary>
        public ArrayList allusers { private set; get; }

        /// <summary>
        /// 用户保存数据
        /// </summary>
        public ArrayList savedata { private set; get; }

        /// <summary>
        /// 推荐服务器列表
        /// </summary>
        public ArrayList recommendServer { private set; get; }


        public int newServerID  { private set; get;}

        /// <summary>
        /// 当前打开的服务器列表
        /// </summary>
        public ArrayList units { private set; get; }

        public ServerListManager()
        {
            tabs = null;
            allusers = null;
            savedata = null;
            recommendServer = null;

            units = null;
        }

        public bool IsBasicDataReady()
        {
            return IsTabsReady() && IsUnitsReady() && IsSaveDataReady();
        }

        public bool IsTabsReady()
        {
            return tabs != null;
        }

        public bool IsUnitsReady()
        {
            return units != null;
        }

        public bool IsSaveDataReady()
        {
            return savedata != null;
        }

        public bool IsRecommendServerReady()
        {
            return null != recommendServer;
        }

        public bool IsAllUserReady()
        {
            return allusers != null;
        }

        public void SaveUserData(RoleInfo[] roles)
        {
            if (null == savedata)
            {
                return;
            }

            if (null == roles)
            {
                Logger.LogErrorFormat("[保存服务器列表] roles 为空");
                return ;
            }

            try 
            {
                ArrayList allroles = new ArrayList();

                for (int i = 0; i < roles.Length; ++i)
                {
                    Hashtable ntb = new Hashtable();
                    ntb.Add("t", (int)roles[i].occupation);
                    ntb.Add("l", (int)roles[i].level);
                    allroles.Add(ntb);
                }

                uint serverID = ClientApplication.adminServer.id;

                ArrayList list = savedata;
                if (list == null)
                {
                    list = new ArrayList();
                }

                int index = -1;
                for (int i = 0; i < list.Count; ++i)
                {
                    Hashtable tb = list[i] as Hashtable;
                    if (null != tb)
                    {
                        uint id = uint.Parse(tb["id"].ToString());

                        if (id == serverID)
                        {
                            index = i;
                            if (tb.ContainsKey("ch"))
                            {
                                tb["ch"] = allroles;
                            }
                            else
                            {
                                tb.Add("ch", allroles);
                            }
                            break;
                        }
                    }
                }

                // TODO 这里最好循环检查所有空的
                if (index < 0)
                {
                    if (allroles.Count > 0)
                    {
                        Hashtable tb = new Hashtable();
                        tb.Add("id", serverID);
                        tb.Add("ch", allroles);
                        list.Add(tb);
                    }
                }
                else if (allroles.Count <= 0)
                {
                    list.RemoveAt(index);
                }

                string jsonstr = MiniJSON.jsonEncode(list);

                string accountname = "";
                if (PlayerLocalSetting.GetValue("AccountDefault") != null)
                {
                    accountname = PlayerLocalSetting.GetValue("AccountDefault").ToString();

                    accountname = SDKInterface.Instance.NeedUriEncodeOpenid(accountname);
                }
                string dirSig = ClientApplication.adminServer.dirSig;
                string url = string.Format("http://{0}/save_data?id={1}&sig={2}", Global.ROLE_SAVEDATA_SERVER_ADDRESS, accountname, dirSig);

                Logger.LogProcessFormat("send {0} json file string {1}", url, jsonstr);

                savedata = list;

                //Http.SendPostRequest(url, jsonstr.ToString());
                HttpClient.Instance.BeginPostRequest();
                HttpClient.Instance.AddField(mBodyName, jsonstr);
                HttpClient.Instance.PostRequest(url, null);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("[保存服务器列表] 粗粗哦 {0}", e.ToString());
            }
        }

        public Hashtable GetServerHashtableByID(int id)
        {
            if (null == allusers)
            {
                return null;
            }

            for (int i = 0; i < allusers.Count; ++i)
            {
                Hashtable tb = allusers[i] as Hashtable;
                if (null != tb)
                {
                    int nid = int.Parse(tb["id"].ToString());
                    if (nid == id)
                    {
                        return tb;
                    }
                }
            }
            return null;
        }

        public bool IsShowHasCharactorFlag(int serverid)
        {
            return GetServerHashtableByID(serverid) != null;
        }

        public Hashtable ShowHasCharatorFlag(uint serverID)
        {
            if (null == savedata)
            {
                Logger.LogErrorFormat("savedata is null");
                return null;
            }

            for (int i = 0; i < savedata.Count; ++i)
            {
                Hashtable tb = savedata[i] as Hashtable;
                if (null != tb)
                {
                    int nid = int.Parse(tb["id"].ToString());
                    if (nid == serverID)
                    {
                        return tb;
                    }
                }
            }
            return null;
        }


        public IEnumerator SendHttpReqCharactorUnit()
        {
            savedata = null;

            {
                var originAccountObj = GameClient.PlayerLocalSetting.GetValue("AccountDefault");
                string accountname = originAccountObj == null ? string.Empty : originAccountObj.ToString();

                WaitHttpRequest req = new WaitHttpRequest(string.Format("get_data?id={0}", accountname));
                yield return req;

                if (req.GetResult() == BaseWaitHttpRequest.eState.Success)
                {
                    string str = System.Web.HttpUtility.UrlDecode(req.GetResultString().Replace(mBodyName + "=", ""));
                    savedata = MiniJsonExtensions.arrayListFromJson(str);

                    if (null == savedata)
                    {
                        savedata = new ArrayList();
                    }
                }
            }

            yield return Yielders.EndOfFrame;

            if (null != savedata)
            {
                allusers = null;

                List<string> allIds = new List<string>();

                for (int i = 0; i < savedata.Count; ++i)
                {
                    Hashtable tb = savedata[i] as Hashtable;

                    if (null != tb)
                    {
                        string id = string.Empty;
                        ArrayList ch = null;

                        if (tb.ContainsKey("id"))
                        {
                            id = tb["id"].ToString();
                        }

                        if (tb.ContainsKey("ch"))
                        {
                            ch = tb["ch"] as ArrayList;
                        }

                        if (!string.IsNullOrEmpty(id))
                        {
                            allIds.Add(id);
                        }
                    }
                }

                string param = string.Format("nodes?ids={0}", string.Join(",", allIds.ToArray()));

                WaitHttpRequest req = new WaitHttpRequest(param);

                yield return req;

                if (req.GetResult() == BaseWaitHttpRequest.eState.Success)
                {
                    allusers = req.GetResultJson();
                }
            }
        }

        public IEnumerator SendHttpReqTab()
        {
            tabs = null;

            WaitHttpRequest req = new WaitHttpRequest("zone_list");

            yield return req;

            if (req.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                tabs = req.GetResultJson();
            }
        }

        public IEnumerator SendHttpReqNodeMap(int originID)
        {
            WaitHttpRequest req = new WaitHttpRequest(string.Format("node_map?id={0}", originID));

            newServerID = -1;

            yield return req;

            if (req.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                try 
                {
                   newServerID = int.Parse(req.GetResultString());
                }
                catch (Exception e)
                {
                }
            }
        }

        public IEnumerator SendHttpReqRecommondServer()
        {
            WaitHttpRequest req = new WaitHttpRequest("recommend");

            yield return req;

            if (req.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                ArrayList serverIds = req.GetResultJson();

                List<string> allIds = new List<string>();
                if (null != serverIds)
                {
                    string sid = string.Empty;
                    for (int i = 0; i < serverIds.Count; ++i)
                    {
                        sid = serverIds[i].ToString();
                        allIds.Add(sid);
                    }
                }

                if (allIds.Count > 0)
                {
                    recommendServer = null;

                    string param = string.Format("nodes?ids={0}", string.Join(",", allIds.ToArray()));

                    WaitHttpRequest recommendReq = new WaitHttpRequest(param);

                    yield return recommendReq;

                    if (recommendReq.GetResult() == BaseWaitHttpRequest.eState.Success)
                    {
                        recommendServer = recommendReq.GetResultJson();
                    }
                }
            }
        }


        public IEnumerator SendHttpReqServerUnit(string serverpath)
        {
            units = null;

            string param = string.Format("zone_nodes?path={0}", serverpath);

            WaitHttpRequest req = new WaitHttpRequest(param);

            yield return req;

            if (req.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                units = req.GetResultJson();
            }
        }
    }
}
