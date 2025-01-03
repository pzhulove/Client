using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    class ComRecommendFriendInfo : MonoBehaviour
    {
        public Image jobIcon;
        public Text roleName;
        public Text roleLv;
        public Image imgFightIcon;
        public Image imgFightLv;
        public Text fightLv;
        public UIGray grayAdd;
        public Button btnAdd;
        public GameObject similarLevelGo;
        public Text similarLevelText;
        public ReplaceHeadPortraitFrame replaceHeadPortraitFrame;
        public Button iconBtn;
        RelationData value;

        public void OnAddRecommendFriend()
        {
            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.Friend);
            if (null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_add_friend_need_lv", functionData.FinishLevel));
                return;
            }

            if (null != value && btnAdd.enabled)
            {
                grayAdd.enabled = true;
                btnAdd.enabled = false;

                SceneRequest req = new SceneRequest();
                req.type = (byte)RequestType.RequestFriendByName;
                req.targetName = value.name;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, req);

                RelationDataManager.GetInstance().AddQueryInfo(value.uid);
            }
        }

        public void OnItemVisible(RelationData friendInfo)
        {
            value = null;
            this.value = friendInfo;
            string path = "";
            var jobItem = TableManager.GetInstance().GetTableItem<JobTable>(friendInfo.occu);
            if (null != jobItem)
            {
                ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobItem.Mode);
                if (resData != null)
                {
                    path = resData.IconPath;
                }
            }
            if (jobIcon != null)
            {
                ETCImageLoader.LoadSprite(ref jobIcon, path);
            }
            if (roleName != null)
            {
                roleName.text = friendInfo.name;
            }
            if (roleLv != null)
            {
                roleLv.text = friendInfo.level.ToString();
            }
            if (PlayerBaseData.GetInstance().Level - friendInfo.level >= 0 && PlayerBaseData.GetInstance().Level - friendInfo.level <= 10 
                || friendInfo.level - PlayerBaseData.GetInstance().Level >= 0 && friendInfo.level - PlayerBaseData.GetInstance().Level <= 10)
            {
                similarLevelText.text = TR.Value("relation_recommend_similarlevel");
                similarLevelGo.CustomActive(true);
            }
            else if (friendInfo.level - PlayerBaseData.GetInstance().Level > 10 && friendInfo.level - PlayerBaseData.GetInstance().Level < 30)
            {
                similarLevelText.text = TR.Value("relation_recommend_gamerookie");
                similarLevelGo.CustomActive(true);
            }
            else if (friendInfo.level - PlayerBaseData.GetInstance().Level >= 30)
            {
                similarLevelText.text = TR.Value("relation_recommend_amazingpeople");
                similarLevelGo.CustomActive(true);
            }
            else
            {
                similarLevelText.text = TR.Value("relation_recommend_noviceplayer");
                similarLevelGo.CustomActive(true);
            }

            if (replaceHeadPortraitFrame != null)
            {
                if (friendInfo.playerLabelInfo.headFrame != 0)
                {
                    replaceHeadPortraitFrame.ReplacePhotoFrame((int)friendInfo.playerLabelInfo.headFrame);
                }
                else
                {
                    replaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                }
            }

            if (imgFightIcon != null)
            {
                ETCImageLoader.LoadSprite(ref imgFightIcon, SeasonDataManager.GetInstance().GetMainSeasonLevelSmallIcon((int)friendInfo.seasonLv));
            }

            if (imgFightLv != null)
            {
                ETCImageLoader.LoadSprite(ref imgFightLv, SeasonDataManager.GetInstance().GetSubSeasonLevelIcon((int)friendInfo.seasonLv));
                imgFightLv.SetNativeSize();
            }
            
            fightLv.SafeSetText(SeasonDataManager.GetInstance().GetRankName((int)friendInfo.seasonLv));
            bool bCanQuery = RelationDataManager.GetInstance().CanQuery(friendInfo.uid);
            if (btnAdd != null)
            {
                grayAdd.enabled = !bCanQuery;
                btnAdd.enabled = bCanQuery;    
            }
        }
        
        void Awake()
        {
            if (iconBtn != null)
            {
                iconBtn.onClick.RemoveAllListeners();
                iconBtn.onClick.AddListener(OnIconBtnClick);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RelationAddRecommendFriendMsgSended, _OnAddRecommendFriendMsgSended);
        }

        private void OnIconBtnClick()
        {
            if (value != null)
            {
                OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(value.uid);
            }
        }

        void _OnAddRecommendFriendMsgSended(UIEvent uiEvent)
        {
            if (null != value)
            {
                if ((ulong)uiEvent.Param1 == value.uid)
                {
                    var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.Friend);
                    if (null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_add_friend_need_lv", functionData.FinishLevel));
                        return;
                    }

                    if (null != value && btnAdd.enabled)
                    {
                        grayAdd.enabled = true;
                        btnAdd.enabled = false;
                    }
                }
            }
        }
        void OnDestroy()
        {
            value = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RelationAddRecommendFriendMsgSended, _OnAddRecommendFriendMsgSended);
        }
    }
}