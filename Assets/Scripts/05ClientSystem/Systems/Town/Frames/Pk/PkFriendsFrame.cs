using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using ProtoTable;
using UnityEngine.UI;

namespace GameClient
{
    public class PkFriendsFrame : ClientFrame
    {
        string ElementPath = "UIFlatten/Prefabs/Pk/PkFriendsElement";
        int ShowNum = 7;

        List<GameObject> FriendsItemList = new List<GameObject>();

        List<Image> Icons = new List<Image>();  
        List<Text> names = new List<Text>();
        List<Text> Levels = new List<Text>();
        List<Text> Jobs = new List<Text>();
        List<Text> states = new List<Text>();
        List<Image> pkLv = new List<Image>();

        List<FriendMatchStatusInfo> FriendInfoList = new List<FriendMatchStatusInfo>();
        private RequestType mCurRequsetType = RequestType.Request_Challenge_PK;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/PkFriendsFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            mCurRequsetType = (RequestType)userData;
            InitInterface();
            SendQueryFriendStatus();
            BindUIEvent();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideEnd);
        }

        protected sealed override void _OnCloseFrame()
        {
            ClearData();
            UnBindUIEvent();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideStart);
        }

        void ClearData()
        {
            for(int i = 0; i < FriendsItemList.Count; i++)
            {
                Button pk = FriendsItemList[i].GetComponentInChildren<Button>();
                if(pk != null)
                {
                    pk.onClick.RemoveAllListeners();
                }
            }

            FriendsItemList.Clear();
            Icons.Clear();
            names.Clear();
            Levels.Clear();
            Jobs.Clear();
            states.Clear();
            FriendInfoList.Clear();
            pkLv.Clear();
        }

        [UIEventHandle("middle/title/btClose")]
        void OnClose()
        {
            frameMgr.CloseFrame(this);
        }

        protected void BindUIEvent()
        {
        }

        protected void UnBindUIEvent()
        {
        }

        void InitInterface()
        {
            CreateFriendsListPreferb();
        }

        void OnClickPk(int iIndex)
        {
            if (iIndex < 0 || iIndex >= FriendInfoList.Count)
            {
                return;
            }

            int iNeedLv = Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Duel);
            RelationData data = RelationDataManager.GetInstance().GetRelationByRoleID(FriendInfoList[iIndex].roleId);

            if (data.level < iNeedLv)
            {
                SystemNotifyManager.SysNotifyTextAnimation("该好友尚未解锁pk功能");

                return;
            }

            SceneRequest req = new SceneRequest
            {
                type = (byte)mCurRequsetType,
                target = FriendInfoList[iIndex].roleId,
                targetName = ""
            };

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            SystemNotifyManager.SysNotifyTextAnimation("邀请已发送");

            OnClose();
        }

        void CreateFriendsListPreferb()
        {
            for (int i = 0; i < ShowNum; i++)
            {
                CreateSinglePrefab(i);
            }
        }

        void SendQueryFriendStatus()
        {
            WorldMatchQueryFriendStatusReq req = new WorldMatchQueryFriendStatusReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        [MessageHandle(WorldMatchQueryFriendStatusRes.MsgID)]
        void OnQueryFriendStatusRes(MsgDATA msg)
        {
            WorldMatchQueryFriendStatusRes res = new WorldMatchQueryFriendStatusRes();
            res.decode(msg.bytes);

            for(int i = 0; i < res.infoes.Length; i++)
            {
                FriendInfoList.Add(res.infoes[i]);
            }
            FriendInfoList.Sort((x, y) =>
            {
                if(x.status<y.status)
                {
                    return 1;
                }
                else if(x.status>y.status)
                {
                    return -1;
                }
                return 0;
            });
            for (int i = 0; i < ShowNum && i < FriendInfoList.Count; i++)
            {
                RelationData data = RelationDataManager.GetInstance().GetRelationByRoleID(FriendInfoList[i].roleId);
                ShowFriendsData(i, FriendInfoList[i], data);
            }

            if (FriendInfoList.Count > ShowNum)
            {
                for (int i = FriendInfoList.Count - ShowNum - 1; i < FriendInfoList.Count; i++)
                {
                    CreateSinglePrefab(i);
                    RelationData data = RelationDataManager.GetInstance().GetRelationByRoleID(FriendInfoList[i].roleId);
                    ShowFriendsData(i, FriendInfoList[i], data);
                }
            }
        }

        void CreateSinglePrefab(int idx)
        {
            GameObject FriendsItemobj = AssetLoader.instance.LoadResAsGameObject(ElementPath);
            if (FriendsItemobj == null)
            {
                return;
            }

            Utility.AttachTo(FriendsItemobj, ContentObjRoot);

            Text[] texts = FriendsItemobj.GetComponentsInChildren<Text>();
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].name == "name")
                {
                    names.Add(texts[i]);
                }
                else if (texts[i].name == "level")
                {
                    Levels.Add(texts[i]);
                }
                else if (texts[i].name == "job")
                {
                    Jobs.Add(texts[i]);
                }
                else if (texts[i].name == "state")
                {
                    states.Add(texts[i]);
                }
            }

            Image[] images = FriendsItemobj.GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].name == "icon")
                {
                    Icons.Add(images[i]);
                }
                else if(images[i].name == "pkLv")
                {
                    pkLv.Add(images[i]);
                }
            }

            int iIndex = idx;
            Button pk = FriendsItemobj.GetComponentInChildren<Button>();
            pk.onClick.AddListener(() => { OnClickPk(iIndex); });

            FriendsItemList.Add(FriendsItemobj);
            FriendsItemobj.SetActive(false);
        }

        void ShowFriendsData(int index, FriendMatchStatusInfo info, RelationData data)
        {
            //加入判断越界
            if(index >= FriendsItemList.Count || index >= names.Count || index >= Levels.Count || index >= Jobs.Count || index >= pkLv.Count || index < 0)
            {
                return;
            }
            FriendsItemList[index].gameObject.SetActive(true);

            names[index].text = data.name;
            Levels[index].text = "lv." + data.level.ToString();

            Text Job = Jobs[index];
            SetJobDataByJobID(index, data.occu, ref Job);

            // 段位
            int RemainPoints = 0;
            int TotalPoints = 0;
            int pkIndex = 0;
            bool bMaxLv = false;

            string pkPath = Utility.GetPathByPkPoints(PlayerBaseData.GetInstance().pkPoints, ref RemainPoints, ref TotalPoints, ref pkIndex, ref bMaxLv);
            if (pkPath != "" && pkPath != "-" && pkPath != "0")
            {
                //Sprite Icon = AssetLoader.instance.LoadRes(pkPath, typeof(Sprite)).obj as Sprite;

                //if (Icon != null)
                //{
                //    pkLv[index].sprite = Icon;
                //}
                Image img = pkLv[index];
                ETCImageLoader.LoadSprite(ref img, pkPath);
            }

            // 在线状态
            states[index].text = GetStatesStrByType(info.status);
        }

        void SetJobDataByJobID(int iIndex, int JobId, ref Text Job)
        {
            string path = "";

            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(JobId);
            if (jobData == null)
            {
                return;
            }

            ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
            if (resData == null)
            {
                return;
            }

            path = resData.IconPath;
            // Icons[iIndex].sprite = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
            Image img = Icons[iIndex];
            ETCImageLoader.LoadSprite(ref img, path);

            Job.text = jobData.Name;
        }

        string GetStatesStrByType(byte state)
        {
            string str = "";

            if ((FriendMatchStatus)state == FriendMatchStatus.Busy)
            {
                str = string.Format(TR.Value("Friens_Busy_State"), "忙碌");
            }
            else if((FriendMatchStatus)state == FriendMatchStatus.Offlie)
            {
                str = "下线";
            }
            else
            {
                str = "空闲";
            }

            return str;
        }

        [UIObject("middle/Scroll View/Viewport/Content")]
        protected GameObject ContentObjRoot;
    }
}
