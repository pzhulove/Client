using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;
using UnityEngine.EventSystems;

namespace GameClient
{

    //队伍房间的角色
    public class TeamDuplicationTeamRoomPlayerItem : MonoBehaviour
    {
        //数据
        private bool _isOtherTeam = false;          //其他队伍的详情信息
        private int _playerSeatId = 0;
        private TeamDuplicationPlayerDataModel _teamRoomPlayerDataModel;

        //聊天框
        private TeamDuplicationChatBubbleViewControl _chatViewBubbleControl;

        //对象
        private TeamDuplicationTeamRoomPlayerView _teamRoomPlayerView;
        private GameObject _teamRoomPlayerDragStillCoverImage;
        private GameObject _teamRoomPlayerDropCoverImage;

        [Space(15)]
        [HeaderAttribute("NoPlayer")]
        [Space(5)]
        [SerializeField] private GameObject noPlayerRoot;

        //用于创建被拖动的物体
        [Space(15)]
        [HeaderAttribute("PlayerViewRoot")]
        [Space(5)]
        [SerializeField] private GameObject teamRoomPlayerViewRoot;


        [Space(15)] [HeaderAttribute("CommonNewDragAndDrop")] [Space(15)]
        [SerializeField] private CommonNewDrag commonNewDrag;
        [SerializeField] private CommonNewDrop commonNewDrop;
        [SerializeField] private GameObject dragCanvasRoot;

        [Space(25)]
        [HeaderAttribute("ChatContent")]
        [Space(10)]
        [SerializeField] private GameObject chatRoot;

        //需要动态加载资源的预制体
        [Space(15)] [HeaderAttribute("AddInActionGameObject")] [Space(10)] [SerializeField]
        private GameObject teamRoomPlayerViewPrefab;
        [SerializeField] private GameObject dropCoverImage;
        [SerializeField] private GameObject dragStillCoverImage;
        [SerializeField] private GameObject dragMoveCoverImage;
        [SerializeField] private GameObject dragAndDropRoot;

        protected void Awake()
        {
            BindUiEvents();
        }

        protected void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
        }

        private void ClearData()
        {
            _teamRoomPlayerDataModel = null;
            _isOtherTeam = false;
            _playerSeatId = 0;

            _chatViewBubbleControl = null;

            _teamRoomPlayerView = null;
            _teamRoomPlayerDragStillCoverImage = null;
            _teamRoomPlayerDropCoverImage = null;
        }

        private void BindUiEvents()
        {
            if (commonNewDrag != null)
            {
                commonNewDrag.SetIsCanBeginDragAction(IsCanBeginDrag);
                commonNewDrag.SetDragBeginAction(DragBegin);
                commonNewDrag.SetDragEndAction(DragEnd);
            }

            if (commonNewDrop != null)
            {
                commonNewDrop.SetIsPointerItemCanDropDownAction(IsPointerItemCanDropDownAction);
                commonNewDrop.SetOnDropDownAction(OnDropDownAction);
                commonNewDrop.SetOnPointerEnterAction(OnPointerEnterAction);
                commonNewDrop.SetOnPointerExitAction(OnPointerExitAction);
            }
        }

        private void UnBindUiEvents()
        {
            if (commonNewDrag != null)
                commonNewDrag.ResetDragAction();

            if(commonNewDrop != null)
                commonNewDrop.ResetDropAction();
        }

        private void OnEnable()
        {
            //退出关闭
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTeamDuplicationForceQuitTeamByDragMessage,
                OnReceiveTeamDuplicationForceQuitTeamByDragMessage);

            //聊天
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationChatContentMessage,
                OnReceiveTeamDuplicationChatBubbleContentMessage);

            //角色离线时间
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationPlayerExpireTimeMessage,
                OnReceiveTeamDuplicationPlayerExpireTimeMessage);

            //团本状态更新
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamStatusNotifyMessage,
                OnReceiveTeamDuplicationTeamStatusNotifyMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTeamDuplicationForceQuitTeamByDragMessage,
                OnReceiveTeamDuplicationForceQuitTeamByDragMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationChatContentMessage,
                OnReceiveTeamDuplicationChatBubbleContentMessage);

            //角色终止
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationPlayerExpireTimeMessage,
                OnReceiveTeamDuplicationPlayerExpireTimeMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamStatusNotifyMessage,
                OnReceiveTeamDuplicationTeamStatusNotifyMessage);
        }

        public void InitItem(TeamDuplicationPlayerDataModel playerDataModel,
            bool isOtherTeam = false,
            int playerSeatId = 0)
        {
            _isOtherTeam = isOtherTeam;
            _playerSeatId = playerSeatId;
            _teamRoomPlayerDataModel = playerDataModel;

            InitTeamRoomPlayerItem();
        }
        
        private void InitTeamRoomPlayerItem()
        {
            //角色不存在
            if (_teamRoomPlayerDataModel == null)
            {
                CommonUtility.UpdateGameObjectVisible(noPlayerRoot, true);
                CommonUtility.UpdateGameObjectVisible(teamRoomPlayerViewRoot, false);

                //拖拽强制结束
                ResetTeamRoomPlayerItemDrag();

                //可能存在的倒计时，进行重置
                if(_teamRoomPlayerView != null)
                    _teamRoomPlayerView.ResetPlayerGrayView();

            }
            else
            {
                //角色存在
                CommonUtility.UpdateGameObjectVisible(noPlayerRoot, false);
                CommonUtility.UpdateGameObjectVisible(teamRoomPlayerViewRoot, true);

                InitTeamRoomPlayerView();
            }

            //更新对应的ChatView
            UpdateChatBubbleViewControl();
        }

        //初始化teamRoomPlayerView
        private void InitTeamRoomPlayerView()
        {
            //已经初始化，直接更新
            if (_teamRoomPlayerView != null)
            {
                _teamRoomPlayerView.InitItem(_teamRoomPlayerDataModel, _isOtherTeam);
                return;
            }
            
            if (teamRoomPlayerViewPrefab == null)
                return;

            if (teamRoomPlayerViewRoot == null)
                return;

            //创建并初始化
            if (_teamRoomPlayerView == null)
            {
                var teamRoomPlayerGameObject = GameObject.Instantiate(teamRoomPlayerViewPrefab) as GameObject;
                if (teamRoomPlayerGameObject != null)
                {
                    CommonUtility.UpdateGameObjectVisible(teamRoomPlayerGameObject, true);
                    teamRoomPlayerGameObject.transform.SetParent(teamRoomPlayerViewRoot.transform, false);
                    _teamRoomPlayerView = teamRoomPlayerGameObject.GetComponent<TeamDuplicationTeamRoomPlayerView>();
                    if (_teamRoomPlayerView != null)
                    {
                        _teamRoomPlayerView.InitItem(_teamRoomPlayerDataModel, _isOtherTeam);
                    }
                }
            }
        }

        #region DragDropAction
        private bool IsCanBeginDrag(PointerEventData pointerEventData)
        {
            //详情页面的Item
            if (_isOtherTeam == true)
                return false;

            //对象不存在
            if (_teamRoomPlayerDataModel == null)
                return false;

            //非团长
            if (TeamDuplicationUtility.IsSelfPlayerIsTeamLeaderInTeamDuplication() == false)
                return false;

            return true;
        }

        //成员退出队伍的时候，强制结束拖拽
        private void ResetTeamRoomPlayerItemDrag()
        {
            if (commonNewDrag == null)
                return;

            //不存在拖拽行为，不进行操作
            if (commonNewDrag.GetDraggingState() == false)
                return;

            //强制停止拖拽
            commonNewDrag.OnForceEndDrag();

            //正在拖拽， 现在成员被踢出，拖拽强制结束
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_change_seat_termination"));
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTeamDuplicationForceQuitTeamByDragMessage);
        }

        private void DragBegin(PointerEventData pointerEventData)
        {
            //开始拖拽，隐藏聊天
            ResetChatBubbleViewControl();

            //设置DragCanvasRoot
            if (dragCanvasRoot == null)
                return;

            CommonUtility.UpdateGameObjectVisible(dragCanvasRoot, true);
            commonNewDrag.SetDragCanvasRoot(dragCanvasRoot);

            if (teamRoomPlayerViewRoot == null)
                return;

            //创建拖动的物体
            var dragGameObject = GameObject.Instantiate(teamRoomPlayerViewRoot) as GameObject;
            if (dragGameObject == null)
                return;

            var canvasGroup = dragGameObject.AddComponent<CanvasGroup>();
            if (canvasGroup != null)
                canvasGroup.blocksRaycasts = false;

            CommonUtility.UpdateGameObjectVisible(dragGameObject, true);
            commonNewDrag.SetDragGameObject(dragGameObject);

            //拖动的物体发光
            var moveCoverImage = GameObject.Instantiate(dragMoveCoverImage) as GameObject;
            if (moveCoverImage != null)
            {
                //显示，设置父节点和层级关系
                CommonUtility.UpdateGameObjectVisible(moveCoverImage, true);
                moveCoverImage.transform.SetParent(dragGameObject.transform, false);
                moveCoverImage.transform.SetAsFirstSibling();
            }

            //删除CanvasRoot下的子物体
            CommonUtility.RemoveChildGameObject(dragCanvasRoot);
            //拖动物体放置在CanvasRoot
            dragGameObject.transform.SetParent(dragCanvasRoot.transform, false);

            //原始的物体（创建拖动物体的Prefab）原地发光
            ShowTeamRoomPlayerDragStillCoverImage();
        }

        private void DragEnd(PointerEventData pointEventData)
        {
            ResetChatBubbleViewControl();
            //原始物体的发光隐藏
            CommonUtility.UpdateGameObjectVisible(_teamRoomPlayerDragStillCoverImage, false);

            //隐藏dragCanvasRoot
            if (dragCanvasRoot != null)
                CommonUtility.UpdateGameObjectVisible(dragCanvasRoot, false);
            commonNewDrag.SetDragCanvasRoot(null);

            //被拖动的物体销毁
            var dragGameObject = commonNewDrag.GetDragGameObject();
            if (dragGameObject == null)
                return;

            GameObject.Destroy(dragGameObject);
            commonNewDrag.SetDragGameObject(null);
        }

        //当前拖动的物体是否可以放下
        private bool IsPointerItemCanDropDownAction(PointerEventData pointerEventData)
        {
            var originalObj = pointerEventData.pointerDrag;
            if (originalObj == null)
            {
                return false;
            }

            var newDrag = originalObj.GetComponent<CommonNewDrag>();
            if (newDrag == null)
            {
                return false;
            }

            var teamRoomPlayerItem =
                originalObj.GetComponent<TeamDuplicationTeamRoomPlayerItem>();

            if (teamRoomPlayerItem == null)
                return false;

            var isCanDropDown = teamRoomPlayerItem.IsCanDropDown(pointerEventData);

            return isCanDropDown;
        }

        //准备放置
        private void OnDropDownAction(PointerEventData pointerEventData)
        {
            CommonUtility.UpdateGameObjectVisible(_teamRoomPlayerDropCoverImage, false);
            //拖拽物体的座位Id
            var dragPlayerSeatId = GetDragPlayerSeatId(pointerEventData);

            if (dragPlayerSeatId == 0 || _playerSeatId == 0
                                   || _playerSeatId == dragPlayerSeatId)
                return;

            //放置位置的角色是否在战斗中
            bool isDropPlayerInFighting =
                TeamDuplicationUtility.IsTeamDuplicationPlayerInFightingStatus(_teamRoomPlayerDataModel);
            if (isDropPlayerInFighting == true)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_can_not_change_position"));
                return;
            }

            //拖拽物体的角色是否处在战斗中
            var dragPlayerDataModel = GetDragPlayerDataModel(pointerEventData);
            bool isDragPlayerInFighting = TeamDuplicationUtility.IsTeamDuplicationPlayerInFightingStatus(dragPlayerDataModel);
            if (isDragPlayerInFighting == true)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_can_not_change_position"));
                return;
            }

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyChangeSeatReq(dragPlayerSeatId, _playerSeatId);
        }

        //拖动进入到Item
        private void OnPointerEnterAction(PointerEventData pointerEventData)
        {
            var dragPlayerSeatId = GetDragPlayerSeatId(pointerEventData);
            //移动到自己的位置的时候，不显示
            if (dragPlayerSeatId != 0 
                && dragPlayerSeatId != _playerSeatId)
            {
                ShowTeamRoomPlayerDropCoverImage();
            }
        }

        //拖动退出Item
        private void OnPointerExitAction(PointerEventData pointerEventData)
        {
            CommonUtility.UpdateGameObjectVisible(_teamRoomPlayerDropCoverImage, false);
        }

        //是否可以下落
        public bool IsCanDropDown(PointerEventData pointerEventData)
        {
            //详情页面的Item
            if (_isOtherTeam == true)
                return false;

            //对象不存在
            if (_teamRoomPlayerDataModel == null)
                return false;

            //非团长
            if (TeamDuplicationUtility.IsSelfPlayerIsTeamLeaderInTeamDuplication() == false)
                return false;

            return true;
        }

        //有队员在拖动的过程中，被强制退队，更新dropCover
        private void OnReceiveTeamDuplicationForceQuitTeamByDragMessage(UIEvent uiEvent)
        {
            OnPointerExitAction(null);
        }


        //得到拖拽物体的座位Id
        private int GetDragPlayerSeatId(PointerEventData pointerEventData)
        {
            var originalObj = pointerEventData.pointerDrag;
            if (originalObj == null)
            {
                return 0;
            }

            var dragComponent = originalObj.GetComponent<CommonNewDrag>();
            if (dragComponent == null)
            {
                return 0;
            }

            var teamRoomPlayerItem = originalObj.GetComponent<TeamDuplicationTeamRoomPlayerItem>();
            if (teamRoomPlayerItem == null)
                return 0;

            if (teamRoomPlayerItem.GetPlayerDataModel() == null)
                return 0;

            var dragPlayerItemSeatId = teamRoomPlayerItem.GetPlayerSeatId();
            return dragPlayerItemSeatId;
        }

        //得到拖拽物体对应的角色数据
        private TeamDuplicationPlayerDataModel GetDragPlayerDataModel(PointerEventData pointerEventData)
        {
            var originalObj = pointerEventData.pointerDrag;
            if (originalObj == null)
            {
                return null;
            }

            var dragComponent = originalObj.GetComponent<CommonNewDrag>();
            if (dragComponent == null)
            {
                return null;
            }

            var teamRoomPlayerItem = originalObj.GetComponent<TeamDuplicationTeamRoomPlayerItem>();
            if (teamRoomPlayerItem == null)
                return null;

            return teamRoomPlayerItem.GetPlayerDataModel();
        }

        //显示拖拽原地物体的Cover
        private void ShowTeamRoomPlayerDragStillCoverImage()
        {
            if (_teamRoomPlayerDragStillCoverImage == null)
            {
                _teamRoomPlayerDragStillCoverImage = GameObject.Instantiate(dragStillCoverImage) as GameObject;
                if (_teamRoomPlayerDragStillCoverImage != null && dragAndDropRoot != null)
                {
                    _teamRoomPlayerDragStillCoverImage.transform.SetParent(dragAndDropRoot.transform, false);
                }
            }
            CommonUtility.UpdateGameObjectVisible(_teamRoomPlayerDragStillCoverImage, true);
        }

        //显示拖动到某个Item,对应Item的Cover
        private void ShowTeamRoomPlayerDropCoverImage()
        {
            if (_teamRoomPlayerDropCoverImage == null)
            {
                _teamRoomPlayerDropCoverImage = GameObject.Instantiate(dropCoverImage) as GameObject;
                if (_teamRoomPlayerDropCoverImage != null
                    && dragAndDropRoot != null)
                {
                    _teamRoomPlayerDropCoverImage.transform.SetParent(dragAndDropRoot.transform, false);
                }
            }

            CommonUtility.UpdateGameObjectVisible(_teamRoomPlayerDropCoverImage, true);
        }
        #endregion

        #region ChatContent

        //聊天相关
        private void OnReceiveTeamDuplicationChatBubbleContentMessage(UIEvent uiEvent)
        {
            //队伍详情界面，不展示
            if (_isOtherTeam == true)
                return;

            //空对象
            if (_teamRoomPlayerDataModel == null)
                return;

            //对象正在被拖动
            if (_teamRoomPlayerDragStillCoverImage != null 
                && _teamRoomPlayerDragStillCoverImage.activeSelf == true)
                return;

            if (uiEvent == null || uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            //两者的Guid不同
            var guid = (ulong)uiEvent.Param1;
            if (guid != _teamRoomPlayerDataModel.Guid)
                return;

            var chatContent = (string)uiEvent.Param2;

            ShowTeamDuplicationChatBubbleContent(guid, chatContent);
        }

        private void ShowTeamDuplicationChatBubbleContent(ulong guid, string chatContent)
        {
            if (_chatViewBubbleControl == null)
                _chatViewBubbleControl = TeamDuplicationUtility.LoadTeamDuplicationChatBubbleViewControl(chatRoot);
            
            if (_chatViewBubbleControl == null)
                return;

            _chatViewBubbleControl.SetChatPlayerGuid(guid);
            _chatViewBubbleControl.SetMessage(chatContent);
        }

        private void UpdateChatBubbleViewControl()
        {
            if (_chatViewBubbleControl == null)
                return;

            //角色不存在
            if (_teamRoomPlayerDataModel == null)
            {
                ResetChatBubbleViewControl();
                return;
            }

            //角色已经更换
            if (_teamRoomPlayerDataModel.Guid != _chatViewBubbleControl.GetChatPlayerGuid())
                ResetChatBubbleViewControl();
        }

        private void ResetChatBubbleViewControl()
        {
            if (_chatViewBubbleControl == null)
                return;

            _chatViewBubbleControl.ShowRoot(false);
        }
        #endregion

        #region UIEvent

        //自己的团队，战斗开始的时候，刷新门票不足的状态
        private void OnReceiveTeamDuplicationTeamStatusNotifyMessage(UIEvent uiEvent)
        {
            if (_isOtherTeam == true)
                return;

            if (_teamRoomPlayerDataModel == null)
                return;

            if (_teamRoomPlayerView == null)
                return;

            _teamRoomPlayerView.UpdatePlayerTicketInfo();
        }


        private void OnReceiveTeamDuplicationPlayerExpireTimeMessage(UIEvent uiEvent)
        {
            //其他队伍，不刷新
            if (_isOtherTeam == true)
                return;

            if (_teamRoomPlayerDataModel == null)
                return;

            if (uiEvent == null || uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            ulong playerGuid = (ulong)uiEvent.Param1;
            ulong playerExpireTime = (ulong)uiEvent.Param2;

            if (_teamRoomPlayerDataModel.Guid != playerGuid)
                return;

            if (_teamRoomPlayerView != null)
                _teamRoomPlayerView.UpdatePlayerGrayView(playerExpireTime);
        }

        #endregion

        #region Helper

        public int GetPlayerSeatId()
        {
            return _playerSeatId;
        }

        public TeamDuplicationPlayerDataModel GetPlayerDataModel()
        {
            return _teamRoomPlayerDataModel;
        }


        #endregion


    }
}
