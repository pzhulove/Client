using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    public enum RelationTabType
    {
        /// <summary>
        /// 最近
        /// </summary>
        [System.ComponentModel.Description("relation_recently")]
        RTT_RECENTLY = 0,
        /// <summary>
        /// 好友
        /// </summary>
        [System.ComponentModel.Description("relation_friend")]
        RTT_FRIEND,
        /// <summary>
        /// 黑名单
        /// </summary>
        [System.ComponentModel.Description("relation_black_friend")]
        RTT_BLACK,
        RTT_COUNT,
    }
    enum RelationOptionType
    {
        [System.ComponentModel.Description("relation_my_friend")]
        ROT_MY_FRIEND = 0,
        //[System.ComponentModel.Description("relation_alloc_friend")]
        //ROT_ADD_FRIEND,
        //[System.ComponentModel.Description("relation_black_friend")]
        //ROT_BLACK,
        //[System.ComponentModel.Description("relation_recommand_friend")]
        //ROT_RECOMMAND,
        [System.ComponentModel.Description("relation_teacherandpupil")]
        ROT_TEACHERANDPUPIL,
        ROT_COUNT,
    }

    class RelationOptionData
    {
        public RelationOptionType eRelationOptionType = RelationOptionType.ROT_MY_FRIEND;
        public RelationTabType eRelationTabType = RelationTabType.RTT_COUNT;
        public RelationData eCurrentRelationData = null;
        public string mTalk = "";
    }
    
    class RelationOption : CachedSelectedObject<RelationOptionData, RelationOption>
    {
        // private string friendImagePath = "UI/Image/Packed/p_UI_Social.png:UI_Social_Tubiao_Haoyou";
        // private string teacherPupilPath= "UI/Image/Packed/p_UI_Social.png:UI_Social_Tubiao_Shitu";
        public Text label;
        public Text checkLabel;
        // public Image image;
        // public Image checkImage;
        public GameObject goCheckMark;
        public GameObject goRedPoint;

        public sealed override void Initialize()
        {
            label = Utility.FindComponent<Text>(goLocal,"Text");
            checkLabel = Utility.FindComponent<Text>(goLocal, "CheckMark/Text");
            // image = Utility.FindComponent<Image>(goLocal, "Image");
            // checkImage = Utility.FindComponent<Image>(goLocal, "CheckMark/Image");
            goCheckMark = Utility.FindChild(goLocal,"CheckMark");
            goRedPoint = Utility.FindChild(goLocal, "RedPoint");

            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshInviteList, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPrivateChat, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnApplyPupilListChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRelationChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTAPApplyToggleRedPointUpdate, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnApplyTeacherListChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FriendComMenuRemoveList, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTapPupilReportRedPoint, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTapTeacherSubmitRedPoint, _OnRefreshInviteList);
        }

        public sealed override void UnInitialize()
        {
            label = null;
            checkLabel = null;
            // image = null;
            // checkImage = null;
            goCheckMark = null;
            goRedPoint = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPrivateChat, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnApplyPupilListChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRelationChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTAPApplyToggleRedPointUpdate, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnApplyTeacherListChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FriendComMenuRemoveList, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTapPupilReportRedPoint, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTapTeacherSubmitRedPoint, _OnRefreshInviteList);
        }

        public sealed override void OnUpdate()
        {
            label.text = TR.Value(Utility.GetEnumDescription(Value.eRelationOptionType));
            checkLabel.text = label.text;
            // if (Value.eRelationOptionType == RelationOptionType.ROT_MY_FRIEND)
            // {
            //     ETCImageLoader.LoadSprite(ref image, friendImagePath);
            //     ETCImageLoader.LoadSprite(ref checkImage, friendImagePath);
            // }
            // else
            // {
            //     ETCImageLoader.LoadSprite(ref image, teacherPupilPath);
            //     ETCImageLoader.LoadSprite(ref checkImage, teacherPupilPath);
            // }
            // image.SetNativeSize();
            // checkImage.SetNativeSize();
            CheckRedPoint();
        }

        public sealed override void OnDisplayChanged(bool bShow)
        {
            goCheckMark.CustomActive(bShow);
        }

        public void CheckRedPoint()
        {
            if(Value.eRelationOptionType == RelationOptionType.ROT_MY_FRIEND)
            {
                //var inviteFriends = RelationDataManager.GetInstance().GetInviteFriendData();
                goRedPoint.CustomActive(RelationDataManager.GetInstance().GetPriDirty());
            }
            else if (Value.eRelationOptionType == RelationOptionType.ROT_TEACHERANDPUPIL)
            {
                bool haveApply = TAPNewDataManager.GetInstance().HaveApplyRedPoint();
                bool haveSubmit = TAPNewDataManager.GetInstance().HaveSubmitRedPoint();
                bool canReport = TAPNewDataManager.GetInstance().HaveReportRedPoint();
                bool canLeave = TAPNewDataManager.GetInstance().HaveLeaveMasterRedPoint();
                goRedPoint.CustomActive(haveApply || haveSubmit || canReport || canLeave);
            }
            else
            {
                goRedPoint.CustomActive(false);
            }
        }

        void _OnRefreshInviteList(UIEvent uiEvent)
        {
            CheckRedPoint();
        }
    }
}