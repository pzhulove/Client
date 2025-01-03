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
    public sealed class ServerListFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Login/ServerList/ServerList";
        }

#region ExtraUIBind
        private Button mClose = null;
        private GameObject mTabroot = null;
        private GameObject mServerroot = null;
        private GameObject mCharactorroot = null;
        private ToggleGroup mTabtogglegroup = null;
        private ToggleGroup mServertogglegroup = null;
        private ToggleGroup mCharactortogglegroup = null;
        private Toggle mAllcharator = null;
        private GameObject mWaittips = null;
        private GameObject mTips = null;
        private CommonBriefTabToggleGroup mCommonBriefTabToggleGroup = null;

        private List<CommonBriefTabData> mCommonBriefTabDatas = null;
        private Dictionary<int, string> mDicServerPath = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mTabroot = mBind.GetGameObject("tabroot");
            mServerroot = mBind.GetGameObject("serverroot");
            mCharactorroot = mBind.GetGameObject("charactorroot");
            mTabtogglegroup = mBind.GetCom<ToggleGroup>("tabtogglegroup");
            mServertogglegroup = mBind.GetCom<ToggleGroup>("servertogglegroup");
            mCharactortogglegroup = mBind.GetCom<ToggleGroup>("charactortogglegroup");
            mAllcharator = mBind.GetCom<Toggle>("allcharator");
            mAllcharator.onValueChanged.AddListener(_onAllcharatorToggleValueChange);
            mWaittips = mBind.GetGameObject("waittips");
            mTips = mBind.GetGameObject("Tips");
            mCommonBriefTabToggleGroup = mBind.GetCom<CommonBriefTabToggleGroup>("CommonVerticalWindowBriefTab");
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mTabroot = null;
            mServerroot = null;
            mCharactorroot = null;
            mTabtogglegroup = null;
            mServertogglegroup = null;
            mCharactortogglegroup = null;
            mAllcharator.onValueChanged.RemoveListener(_onAllcharatorToggleValueChange);
            mAllcharator = null;
            mWaittips = null;
            mTips = null;
            mCommonBriefTabToggleGroup = null;

            mCommonBriefTabDatas = null;
            mDicServerPath = null;
        }
#endregion   
  

#region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.instance.CloseFrame(this);
        }

        private void _onAllcharatorToggleValueChange(bool changed)
        {
            /* put your code in here */
            mTips.CustomActive(changed);
            if (changed)
            {
                if (mCommonBriefTabToggleGroup != null)
                {
                    mCommonBriefTabToggleGroup.UnSelectList();
                }

                mServerroot.CustomActive(false);
                mCharactorroot.CustomActive(true);

                _loadCharactorUnit();
            }
        }
#endregion

        public enum eState 
        {
            None,
            Loading,
        }

        private eState mState = eState.None;

        protected override void _OnOpenFrame()
        {
            mAllcharator.group = mTabtogglegroup;
            GameFrameWork.instance.StartCoroutine(_waitForLoadTabs());
        }

        protected override void _OnCloseFrame()
        {
            mBind.ClearAllCacheBinds();
        }

        private void _loadTabs()
        {
            if (mCommonBriefTabToggleGroup == null)
            {
                return;
            }

            ArrayList tabs = ServerListManager.instance.tabs;
            if (mCommonBriefTabDatas == null)
            {
                mCommonBriefTabDatas = new List<CommonBriefTabData>();
            }
            else
            {
                mCommonBriefTabDatas.Clear();
            }

            int index = -1;
            if (tabs != null)
            {
                if (mDicServerPath == null)
                {
                    mDicServerPath = new Dictionary<int, string>();
                }
                else
                {
                    mDicServerPath.Clear();
                }

                for (int i = 0; i < tabs.Count; ++i)
                {
                    Hashtable tb = tabs[i] as Hashtable;
                    if (null == tb)
                    {
                        continue;
                    }

                    CommonBriefTabData tabData = new CommonBriefTabData();
                    tabData.id = i;
                    tabData.tabName = tb["name"].ToString();
                    mCommonBriefTabDatas.Add(tabData);

                    mDicServerPath.Add(tabData.id, tb["path"].ToString());

                    ArrayList ids = tb["ids"] as ArrayList;
                    if (null != ids)
                    {
                        uint saveid = ClientApplication.adminServer.id;

                        for (int j = 0; j < ids.Count; ++j)
                        {
                            uint cid = uint.Parse(ids[j].ToString());
                            if (saveid == cid)
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                }
            }

            

            mCommonBriefTabToggleGroup.InitComTab(_TabClick, index, mCommonBriefTabDatas);

            //if (null == mBind)
            //{
            //    return ;
            //}

            //string unitPath = mBind.GetPrefabPath("tabunit");
            //mBind.ClearCacheBinds(unitPath);

            //ArrayList tabs = ServerListManager.instance.tabs;
            //if (tabs != null)
            //{
            //    bool guideflag = false;
            //    for (int i = 0; i < tabs.Count; ++i)
            //    {
            //        Hashtable tb = tabs[i] as Hashtable;
            //        if (null != tb)
            //        {
            //            ComCommonBind bind = mBind.LoadExtraBind(unitPath);

            //            if (null != bind)
            //            {
            //                Utility.AttachTo(bind.gameObject, mTabroot);

            //                Image flag = bind.GetCom<Image>("flag");
            //                Text name = bind.GetCom<Text>("name");
            //                Text textNormal = bind.GetCom<Text>("NormalText");
            //                Toggle tab = bind.GetCom<Toggle>("tab");

            //                tab.group       = mTabtogglegroup;
            //                name.text       = tb["name"].ToString();
            //                textNormal.SafeSetText(tb["name"].ToString());
            //                int isNewflag = int.Parse(tb["is_new"].ToString());

            //                //int isRecommond = 0;
            //                //if (tb.ContainsKey("is_recommond"))
            //                //{
            //                //    isRecommond = int.Parse(tb["is_recommond"].ToString());
            //                //}

            //                // flag.sprite = mBind.GetSprite("newflag");
            //                mBind.GetSprite("newflag", ref flag);
            //                //flag.gameObject.SetActive(isNewflag == 0);
            //                flag.gameObject.SetActive(false);

            //                string serverpath = tb["path"].ToString();

            //                tab.onValueChanged.AddListener((isOn)=>
            //                {
            //                    if (isOn)
            //                    {
            //                        mServerroot.CustomActive(true);
            //                        mCharactorroot.CustomActive(false);
            //                        Logger.LogProcessFormat("tab click server path {0}", serverpath);
            //                        GameFrameWork.instance.StartCoroutine(_waitForLoadUnits(serverpath));
            //                    }
            //                });

            //                ArrayList ids = tb["ids"] as ArrayList;
            //                if (null != ids)
            //                {
            //                    uint saveid = ClientApplication.adminServer.id;

            //                    for (int j = 0; j < ids.Count; ++j)
            //                    {
            //                        uint cid = uint.Parse(ids[j].ToString());
            //                        if (saveid == cid)
            //                        {
            //                            tab.isOn = true;
            //                            guideflag = true;
            //                            break;
            //                        }
            //                    }
            //                }

                           
            //            }
            //        }
            //    }

            //    //{
            //    //    ComCommonBind bind= mBind.GetExistBind(unitPath, 0);
            //    //    Toggle tab = bind.GetCom<Toggle>("tab");
            //    //    tab.isOn = true;
            //    //}
            //}
        }

        private void _TabClick(CommonBriefTabData data)
        {
            if (data == null)
            {
                return;
            }

            string serverpath = string.Empty;
            if (mDicServerPath != null)
            {
                mDicServerPath.TryGetValue(data.id, out serverpath);
            }

            mServerroot.CustomActive(true);
            mCharactorroot.CustomActive(false);
            Logger.LogProcessFormat("tab click server path {0}", serverpath);
            GameFrameWork.instance.StartCoroutine(_waitForLoadUnits(serverpath));
        }

        private void _loadUnits()
        {
            if (null == mBind)
            {
                return ;
            }

            string unitPath = mBind.GetPrefabPath("serverunit");
            mBind.ClearCacheBinds(unitPath);

            ArrayList units = ServerListManager.instance.units;

            if (null != units)
            {
                for (int i = 0; i < units.Count; ++i)
                {
                    Hashtable tb = units[i] as Hashtable;
                    if (null != tb)
                    {
                        ComCommonBind bind = mBind.LoadExtraBind(unitPath);

                        if (null != bind)
                        {
                            Utility.AttachTo(bind.gameObject, mServerroot);

                            Image status = bind.GetCom<Image>("status");
                            Text name = bind.GetCom<Text>("name");
                            Text textRoleNum = bind.GetCom<Text>("TextRoleNum");
                            Toggle serverunit = bind.GetCom<Toggle>("serverunit");
                            //Image charactorflag = bind.GetCom<Image>("charactorflag");
                            Image typeflag = bind.GetCom<Image>("typeflag");
                            CanvasGroup roleNumCanvas = bind.GetCom<CanvasGroup>("RoleNum");

                            serverunit.group = mServertogglegroup;

                            name.text = tb["name"].ToString();

                            string ip = tb["ip"].ToString();
                            ushort port = ushort.Parse(tb["port"].ToString());
                            uint id = uint.Parse(tb["id"].ToString());
                            int cstatus = int.Parse(tb["status"].ToString());
                            string cname = tb["name"].ToString();
                            roleNumCanvas.CustomActive(false);

                            if (ServerListManager.instance.IsShowHasCharactorFlag((int)id))
							{
                                Hashtable tb2 = ServerListManager.instance.ShowHasCharatorFlag(id);
                                if(null!=tb2)
                                {
                                    ArrayList chs = tb2["ch"] as ArrayList;
                                    if(chs==null)
                                    {
                                        Logger.LogErrorFormat("chs id null");
                                        return;
                                    }

                                    roleNumCanvas.CustomActive(chs.Count > 0);
                                    textRoleNum.SafeSetText(string.Format("x{0}", chs.Count));
                                }
                                
							}

                            bool isrecommond = false;
                            if (tb.ContainsKey("is_recommend"))
                            {
                                isrecommond = 0 != int.Parse(tb["is_recommend"].ToString());
                            }

                            bool isnew = false;
                            if (tb.ContainsKey("is_new"))
                            {
                                isnew = 0 != int.Parse(tb["is_new"].ToString());
                            }

                            typeflag.gameObject.SetActive(isnew);

                            //charactorflag.enabled = ServerListManager.instance.IsShowHasCharactorFlag((int)id);

                            if (id == ClientApplication.adminServer.id)
                            {
                    bool isChanged = ClientApplication.adminServer.id != id;
                                ClientApplication.adminServer.ip   = ip;
                                ClientApplication.adminServer.port = port;
                                ClientApplication.adminServer.id   = id;
                                ClientApplication.adminServer.state = (eAdminServerStatus)cstatus;
                                ClientApplication.adminServer.name = cname;

                    _sendUIEvent(isChanged);

                                serverunit.isOn = true;
                            }

                            serverunit.onValueChanged.AddListener((isOn) =>
                            {
                                if (isOn)
                                {
                                    Logger.LogProcessFormat("选择服务器 ip {0}, port {1}", ip, port);

                        bool isChanged = ClientApplication.adminServer.id != id;
                        ClientApplication.adminServer.ip   = ip;
                        ClientApplication.adminServer.port = port;
                        ClientApplication.adminServer.id   = id;
                        ClientApplication.adminServer.state = (eAdminServerStatus)cstatus;
                        ClientApplication.adminServer.name = cname;

                        _sendUIEvent(isChanged);

                        _onCloseButtonClick();
                                }
                            });

                            _setServerStatus(status, (eAdminServerStatus)cstatus);
                        }
                    }
                }
            }
        }

        private void _loadCharactorUnit()
        {
            string unitPath = mBind.GetPrefabPath("charactorunit");
            mBind.ClearCacheBinds(unitPath);

            ArrayList savedata = ServerListManager.instance.savedata;
            if (null != savedata)
            {
                for (int i = 0; i < savedata.Count; ++i)
                {
                    Hashtable tb = savedata[i] as Hashtable;

                    if (null != tb)
                    {
                        ArrayList chs = tb["ch"] as ArrayList;

                        int serverid = int.Parse(tb["id"].ToString());
                        Hashtable utb = ServerListManager.instance.GetServerHashtableByID(serverid);
                        if (utb == null)
                        {
                            continue;
                        }

                        if (null != chs && chs.Count > 0)
                        {
                            ComCommonBind bind = mBind.LoadExtraBind(unitPath);

                            if (null != bind)
                            {
                                Utility.AttachTo(bind.gameObject, mCharactorroot);

                                CanvasGroup roleNumCanvas = bind.GetCom<CanvasGroup>("RoleNum");
                                roleNumCanvas.CustomActive(chs.Count > 0);
                                Text textRoleNum = bind.GetCom<Text>("TextRoleNum");
                                textRoleNum.SafeSetText(string.Format("x{0}", chs.Count));

                                Image status = bind.GetCom<Image>("status");
                                Text name = bind.GetCom<Text>("name");
                                Toggle charactor = bind.GetCom<Toggle>("charactor");

                                charactor.group = mCharactortogglegroup;
                                
                                if (null != utb)
                                {
                                    string ip = utb["ip"].ToString();
                                    ushort port = ushort.Parse(utb["port"].ToString());
                                    uint id = uint.Parse(utb["id"].ToString());
                                    int cstatus = int.Parse(utb["status"].ToString());
                                    string cname = utb["name"].ToString();


                                    name.text = cname;

                                    charactor.onValueChanged.AddListener((isOn)=>
                                    {
                                        Logger.LogProcessFormat("选择服务器 ip {0}, port {1}", ip, port);
                                bool isChanged = ClientApplication.adminServer.id != id;
                                        ClientApplication.adminServer.ip   = ip;
                                        ClientApplication.adminServer.port = port;
                                        ClientApplication.adminServer.id   = id;
                                        ClientApplication.adminServer.state = (eAdminServerStatus)cstatus;
                                        ClientApplication.adminServer.name = cname;

                                _sendUIEvent(isChanged);
                                _onCloseButtonClick();
                                    });

                                    _setServerStatus(status, (eAdminServerStatus)cstatus);
                                }
                            }
                        }
                    }
                }
            }
        }

   
        private void _sendUIEvent(bool isChanged)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerListSelectChanged, isChanged);
        }

        private void _setServerStatus(Image img, eAdminServerStatus status)
        {
            if (img == null) return;

            switch (status)
            {
                case eAdminServerStatus.Full:
                    // img.sprite = mBind.GetSprite("statusfull");
                    mBind.GetSprite("statusfull", ref img);
                    break;
                case eAdminServerStatus.Offline:
                    // img.sprite = mBind.GetSprite("statusoffline");
                    mBind.GetSprite("statusoffline", ref img);
                    img.gameObject.AddComponent<UIGray>();
                    break;
                case eAdminServerStatus.Ready:
                    // img.sprite = mBind.GetSprite("statusready");
                    mBind.GetSprite("statusready", ref img);
                    break;
                case eAdminServerStatus.Buzy:
                    // img.sprite = mBind.GetSprite("statusbuzy");
                    mBind.GetSprite("statusbuzy", ref img);
                    break;
            }
        }

        private IEnumerator _waitForLoadTabs()
        {
            mWaittips.CustomActive(true);
            if (!ServerListManager.instance.IsTabsReady())
            {
                yield return ServerListManager.instance.SendHttpReqTab();
            }
            mWaittips.CustomActive(false);

            _loadTabs();

            yield return Yielders.EndOfFrame;

            _loadUnits();
        }

        private IEnumerator _waitForLoadUnits(string serverpath)
        {
            mWaittips.CustomActive(true);
            yield return ServerListManager.instance.SendHttpReqServerUnit(serverpath);

            if (!ServerListManager.instance.IsTabsReady())
            {
                yield return Yielders.EndOfFrame;
            }
            mWaittips.CustomActive(false);

            _loadUnits();
        }

    }
}
