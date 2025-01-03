using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    class TeamRequesterListFrame : ClientFrame
    {
        List<GameObject> RequestersObj = new List<GameObject>();
        List<Button> RejectList = new List<Button>();
        List<Button> AgreeList = new List<Button>();
        List<Image> IconList = new List<Image>();
        List<Text> LevelList = new List<Text>();
        List<Text> NameList = new List<Text>();
        List<Text> JobList = new List<Text>();

        List<TeammemberBaseInfo> requesters = new List<TeammemberBaseInfo>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamRequesterList";
        }

        protected override void _OnOpenFrame()
        {
            requesters = userData as List<TeammemberBaseInfo>;
            InitInterface();

            TeamDataManager.GetInstance().ClearNewRequesterMark();
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.TeamNewRequester);
        }

        protected override void _OnCloseFrame()
        {
            _Clear();
        }

        void _Clear()
        {
            for (int i = 0; i < RejectList.Count; i++)
            {
                RejectList[i].onClick.RemoveAllListeners();
            }

            for (int i = 0; i < AgreeList.Count; i++)
            {
                AgreeList[i].onClick.RemoveAllListeners();
            }

            IconList.Clear();
            LevelList.Clear();
            NameList.Clear();
            JobList.Clear();
            RejectList.Clear();
            AgreeList.Clear();
            RequestersObj.Clear();
            requesters.Clear();
        }

        [UIEventHandle("middle/Title/Close")]
        void OnClose()
        {
            frameMgr.CloseFrame(this);
        }

        void OnReject(int iIndex)
        {
            if (iIndex < 0 || requesters.Count <= 0 || iIndex >= requesters.Count)
            {
                return;
            }

            SendDealResult(requesters[iIndex].id, 0);
        }

        void OnAgree(int iIndex)
        {
            if (iIndex < 0 || requesters.Count <= 0 || iIndex >= requesters.Count)
            {
                return;
            }

            SendDealResult(requesters[iIndex].id, 1);
        }

        [UIEventHandle("middle/OneKeyClear")]
        void OnOneKeyClear()
        {
            for(int i = 0; i < requesters.Count; i++)
            {
                SendDealResult(requesters[i].id, 0);
            }
        }

        void InitInterface()
        {
            CreatePrefabs();
            UpdateInterface();
        }

        void CreatePrefabs()
        {
            for(int i = 0; i < requesters.Count; i++)
            {
                GameObject RequesterObj = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Team/TeamRequesterEle");
                if (RequesterObj == null)
                {
                    continue;
                }

                Utility.AttachTo(RequesterObj, rootObj);

                RequestersObj.Add(RequesterObj);

                Image[] icons = RequesterObj.GetComponentsInChildren<Image>();
                for(int j = 0; j < icons.Length; j++)
                {
                    if(icons[j].name == "Icon")
                    {
                        IconList.Add(icons[j]);
                    }
                }
               
                Text[] texts = RequesterObj.GetComponentsInChildren<Text>();
                for(int j = 0; j < texts.Length; j++)
                {
                    if(texts[j].name == "Level")
                    {
                        LevelList.Add(texts[j]);
                    }
                    else if(texts[j].name == "Name")
                    {
                        NameList.Add(texts[j]);
                    }
                    else if(texts[j].name == "Job")
                    {
                        JobList.Add(texts[j]);
                    }
                }

                Button[] buttons = RequesterObj.GetComponentsInChildren<Button>();
                for(int j = 0; j < buttons.Length; j++)
                {
                    if(buttons[j].name == "reject")
                    {
                        int Idx = i;
                        buttons[j].onClick.AddListener(() => { OnReject(Idx); });

                        RejectList.Add(buttons[j]);
                    }
                    else if(buttons[j].name == "agree")
                    {
                        int Idx = i;
                        buttons[j].onClick.AddListener(() => { OnAgree(Idx); });

                        AgreeList.Add(buttons[j]);
                    }
                }
            }
        }

        void UpdateInterface()
        {
            for(int i = 0; i < RequestersObj.Count; i++)
            {
                if(i < requesters.Count)
                {
                    NameList[i].text = requesters[i].name;
                    LevelList[i].text = string.Format("Lv.{0}", requesters[i].level);

                    JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(requesters[i].occu);
                    if(jobData != null)
                    {
                        JobList[i].text = jobData.Name;

                        ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                        if (resData != null)
                        {
                            //Sprite spr = AssetLoader.instance.LoadRes(resData.IconPath, typeof(Sprite)).obj as Sprite;
                            //if (spr != null)
                            //{
                            //    IconList[i].sprite = spr;
                            //}
                            Image image = IconList[i];
                            ETCImageLoader.LoadSprite(ref image, resData.IconPath);
                        }
                    }
                    ComCommonBind bind = RequestersObj[i].GetComponent<ComCommonBind>();
                    StaticUtility.SafeSetVisible(bind, "returnPlayer", false);
                    StaticUtility.SafeSetVisible(bind, "myFriend", false);
                    StaticUtility.SafeSetVisible(bind, "myGuild", false);
                    TeammemberBaseInfo teammemberBaseInfo = requesters[i];
                    if(teammemberBaseInfo != null)
                    {
                        RelationData relationData = null;
                        bool isMyFriend = RelationDataManager.GetInstance().FindPlayerIsRelation(teammemberBaseInfo.id, ref relationData);
                        bool isMyGuild = GuildDataManager.GetInstance().IsSameGuild(teammemberBaseInfo.playerLabelInfo.guildId);
                        if (teammemberBaseInfo.playerLabelInfo.returnStatus == 1)
                        {
                            StaticUtility.SafeSetVisible(bind, "returnPlayer", true);
                        }
                        else if (isMyFriend)
                        {
                            StaticUtility.SafeSetVisible(bind, "myFriend", true);
                        }
                        else if (isMyGuild)
                        {
                            StaticUtility.SafeSetVisible(bind, "myGuild", true);
                        }
                        if (bind != null)
                        {
                            Button btnIcon = bind.GetCom<Button>("btnIcon");
                            btnIcon.SafeSetOnClickListener(() =>
                            {
                                OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(teammemberBaseInfo.id);
                            });
                    }
                    }                   

                    RequestersObj[i].SetActive(true);
                }
                else
                {
                    RequestersObj[i].SetActive(false);
                }
            }
        }

        void SendDealResult(UInt64 targetid, byte agree)
        {
            WorldTeamProcessRequesterReq req = new WorldTeamProcessRequesterReq();

            req.targetId = targetid;
            req.agree = agree;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        [MessageHandle(WorldTeamProcessRequesterRes.MsgID)]
        void OnTeamRequestersListRes(MsgDATA msg)
        {
            WorldTeamProcessRequesterRes res = new WorldTeamProcessRequesterRes();
            res.decode(msg.bytes);

            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
                return;
            }

            for (int i = 0; i < requesters.Count; i++)
            {
                if(requesters[i].id == res.targetId)
                {
                    requesters.RemoveAt(i);
                    break;
                }
            }

            UpdateInterface();
        }

        [UIObject("middle/Scroll View/Viewport/Content")]
        GameObject rootObj;
    }
}
