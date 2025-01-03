using Protocol;
using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TapLeaveMasterFrame : ClientFrame
    {
        private RelationData relationData;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TAP/TAPLeaveMasterFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            relationData = userData as RelationData;
            _InitUI();
        }

        protected sealed override void _OnCloseFrame()
        {

        }

        void _InitUI()
        {
            bool pupilLevelMatch = false;
            bool isTeacher = false;
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher)
            {
                var datas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
                if (datas != null && datas.Count > 0)
                {
                    isTeacher = true;
                }
            }

            if (isTeacher)
            {
                //是师父，拿传过来的那个徒弟的数据，徒弟可能有两个。
                if (relationData != null)
                {
                    if (relationData.level >= 50)
                    {
                        pupilLevelMatch = true;
                    }
                }
            }
            else
            {
                //徒弟直接拿自己等级
                pupilLevelMatch = (PlayerBaseData.GetInstance().Level >= 50);
            }
            mCheck1.CustomActive(pupilLevelMatch);

            bool teacherIsLeader = false;
            //有队伍
            var myTeam = TeamDataManager.GetInstance().GetMyTeam();
            if (myTeam != null)
            {
                if (isTeacher)
                {
                    //自己是师父，看自己是不是队伍leader
                    if (myTeam.leaderInfo.id == PlayerBaseData.GetInstance().RoleID)
                        teacherIsLeader = true;
                }
                else
                {
                    //自己是徒弟，看师父是不是队伍leader
                    var teacher = RelationDataManager.GetInstance().GetTeacher();
                    if (teacher != null)
                    {
                        if (teacher.uid == myTeam.leaderInfo.id)
                        {
                            teacherIsLeader = true;
                        }
                    }
                }
                //（师父是队长）
                mCheck2.CustomActive(teacherIsLeader);
                
                //由师父发起
            }
        }

        #region ExtraUIBind
        private GameObject mCheck1 = null;
        private RectTransform mCheck2 = null;
        private Button mGoBtn = null;
        private Button mCloseBtn = null;

        protected override void _bindExUI()
        {
            mCheck1 = mBind.GetGameObject("Check1");
            mCheck2 = mBind.GetCom<RectTransform>("Check2");
            mGoBtn = mBind.GetCom<Button>("GoBtn");
            mGoBtn.onClick.AddListener(_onGoBtnButtonClick);
            mCloseBtn = mBind.GetCom<Button>("CloseBtn");
            mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
        }

        protected override void _unbindExUI()
        {
            mCheck1 = null;
            mCheck2 = null;
            mGoBtn.onClick.RemoveListener(_onGoBtnButtonClick);
            mGoBtn = null;
            mCloseBtn.onClick.RemoveListener(_onCloseBtnButtonClick);
            mCloseBtn = null;
        }
        #endregion

        #region Callback
        private void _onGoBtnButtonClick()
        {
            /* put your code in here */
            Parser.NpcParser.OnClickLinkByNpcId(TAPNewDataManager.TAPNpc);
            ClientSystemManager.GetInstance().CloseFrame<RelationFrameNew>();
            frameMgr.CloseFrame(this);
        }
        private void _onCloseBtnButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
