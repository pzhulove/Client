using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    //消耗品
    public class ChallengeChapterEntryControl : MonoBehaviour
    {
        private int _dungeonId;             //当前选择的地下城ID， 根据难度ID有所不同
        private DungeonTable _dungeonTable;
        private int _baseDungeonId;         //地下城基础的Id
        private DungeonID _mDungeonID;

        private ItemData _sinanItem = null;

        [Space(10)] [HeaderAttribute("StartButton")] [Space(10)] [SerializeField]
        private GameObject startButtonRoot;
        [SerializeField] private Button startButton;
        [SerializeField] private UIGray startButtonGray;
        [SerializeField] private GameObject startButtonEffect;

        [Space(10)] [HeaderAttribute("TeamButton")] [Space(10)] [SerializeField]
        private GameObject teamButtonRoot;
        [SerializeField] private Button teamButton;
        [SerializeField] private UIGray teamButtonGray;

        [Space(10)] [HeaderAttribute("ComsumeRoot")] [Space(10)] [SerializeField]
        private GameObject consumeRoot;
        [SerializeField] private Text powerCostValueText;
        [SerializeField] private Image ticketIcon;
        [SerializeField] private Text ticketCostValueText;
        [SerializeField] private GameObject ticketConsumeRoot;

        [Space(20)] [HeaderAttribute("CommonButtonRoot")] [Space(10)] [SerializeField]
        private GameObject commonStartPos;
        [SerializeField] private GameObject specialStartPos;
        [SerializeField] private GameObject commonConsumePos;
        [SerializeField] private GameObject specialConsumePos;

        [Space(20)] [HeaderAttribute("CommonHelpButton")] [Space(10)] [SerializeField]
        private CommonHelpNewAssistant commonHelpNewAssistant;

        UnityEngine.Events.UnityAction mOnStartButtonClick;
        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (teamButton != null)
            {
                teamButton.onClick.RemoveAllListeners();
                teamButton.onClick.AddListener(OnTeamButtonClick);
            }

            if (startButton != null)
            {
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(OnStartBtnClick);
            }
        }

        private void UnBindEvents()
        {
            if (startButton != null)
                startButton.onClick.RemoveAllListeners();

            if (teamButton != null)
                teamButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _dungeonId = 0;
            _dungeonTable = null;
            _mDungeonID = null;
            mOnStartButtonClick = null;
        }

        public void UpdateEntryControl(int dungeonId, DungeonTable dungeonTable, int baseDungeonId = 0, UnityEngine.Events.UnityAction OnStartClick = null, ItemData sinanItem = null)
        {
            _dungeonId = dungeonId;
            _dungeonTable = dungeonTable;
            _baseDungeonId = baseDungeonId;
            mOnStartButtonClick = OnStartClick;
            _sinanItem = sinanItem;

            if (_dungeonTable == null)
            {
                Logger.LogErrorFormat("ChallengeChapterEntryControl and dungeonTable is null dungeonId is {0}",
                    _dungeonId);
                return;
            }

            if (_mDungeonID == null)
                _mDungeonID = new DungeonID(_dungeonId);

            if (commonHelpNewAssistant != null)
                commonHelpNewAssistant.HelpId = ChallengeDataManager.GetInstance().ChallengeChapterHelpId;

            UpdateEntryView();
        }

        private void UpdateEntryView()
        {
            var isChapterLock = ChallengeUtility.IsDungeonLock(_dungeonId);

            //选择章节的按钮状态
            if (startButtonGray != null)
                startButtonGray.enabled = isChapterLock;

            if (startButtonEffect != null)
                startButtonEffect.gameObject.CustomActive(!isChapterLock);

            if (teamButtonGray != null)
            {
                var isTeamLocked = IsTeamButtonLocked();
                teamButtonGray.enabled = isTeamLocked;
            }
            if(teamButton != null)
                teamButton.CustomActive(Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Team));

            //选择章节的消耗数据
            var powerValue = _dungeonTable.CostFatiguePerArea * _dungeonTable.MostCostStamina;
            var isHellEntry = _dungeonTable.SubType == DungeonTable.eSubType.S_HELL_ENTRY;

            if (powerCostValueText != null)
            {
                if (isHellEntry == true)
                    powerCostValueText.text = string.Format(TR.Value("challenge_chapter_powerCost_more_than"), powerValue);
                else
                    powerCostValueText.text = string.Format(TR.Value("challenge_chapter_powerCost_interval"), powerValue);
            }

            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_dungeonTable.TicketID);
            if (ticketIcon != null)
            {
                if (itemTable != null)
                {
                    ETCImageLoader.LoadSprite(ref ticketIcon, itemTable.Icon);
                }
            }

            if (ticketCostValueText != null)
            {
                ticketCostValueText.text = _dungeonTable.TicketNum.ToString();
                string colorStr = "";
                if (_dungeonTable.SubType== DungeonTable.eSubType.S_HELL_ENTRY)//深渊
                {
                    if (ActivityDataManager.GetInstance().GettAnniverTaskIsFinish(EAnniverBuffPrayType.HellTicketMinus))
                    {
                        if (ActivityDataManager.GetInstance().IsLeftChallengeTimes(EAnniverBuffPrayType.HellTicketMinus,CounterKeys.DUNGEON_HELL_TIMES))
                        {
                            int value = ActivityDataManager.GetInstance().GetAnniverTaskValue(EAnniverBuffPrayType.HellTicketMinus);
                            colorStr = string.Format("<color=#00FF56FF> -{0}</color>", value);
                        }
                       
                    }
                }else if(_dungeonTable.SubType==DungeonTable.eSubType.S_YUANGU)//远古
                {
                    if (ActivityDataManager.GetInstance().GettAnniverTaskIsFinish(EAnniverBuffPrayType.YuanGUTicketMinus))
                    {
                        if (ActivityDataManager.GetInstance().IsLeftChallengeTimes(EAnniverBuffPrayType.HellTicketMinus,CounterKeys.DUNGEON_YUANGU_TIMES))
                        {
                            int value = ActivityDataManager.GetInstance().GetAnniverTaskValue(EAnniverBuffPrayType.YuanGUTicketMinus);
                            colorStr = string.Format("<color=#00FF56FF> -{0}</color>", value);
                        }
                    }
                }
                ticketCostValueText.text = string.Format("{0}{1}", ticketCostValueText.text, colorStr);
            }

            UpdateEntryRoot();
        }

        //更新按钮和消耗品的展示
        private void UpdateEntryRoot()
        {
            //周常深渊的前置关卡
            if(DungeonUtility.IsWeekHellPreDungeon(_dungeonId) == true)
            {
                //只显示Start按钮和体力消耗
                SetEntryRoot(false);

                if (startButtonRoot != null)
                {
                    startButtonRoot.gameObject.CustomActive(true);
                    //if (specialStartPos != null)
                    //    startButtonRoot.transform.localPosition = specialStartPos.transform.localPosition;
                }

                if (consumeRoot != null)
                {
                    consumeRoot.gameObject.CustomActive(true);
                    //if (specialConsumePos != null)
                    //    consumeRoot.transform.localPosition = specialConsumePos.transform.localPosition;
                }

                if (ticketConsumeRoot != null)
                    ticketConsumeRoot.gameObject.CustomActive(false);
            }
            else
            {
                SetEntryRoot(true);
                //if (startButtonRoot != null)
                //{
                //    if (commonStartPos != null)
                //    {
                //        startButtonRoot.transform.localPosition = commonStartPos.transform.localPosition;
                //    }
                //}

                if (consumeRoot != null)
                {
                    //if (commonConsumePos != null)
                    //    consumeRoot.transform.localPosition = commonConsumePos.transform.localPosition;
                }

                //地下城是免费限时深渊
                if(DungeonUtility.IsLimitTimeFreeHellDungeon(_dungeonId))
                {
                    if (ticketConsumeRoot != null)
                        ticketConsumeRoot.gameObject.CustomActive(false);
                }
                else
                {
                    if (ticketConsumeRoot != null && !ChallengeUtility.isYunShangChangAn(_dungeonId))
                        ticketConsumeRoot.gameObject.CustomActive(true);
                }
            }
        }

        private void SetEntryRoot(bool flag)
        {
            if (startButtonRoot != null)
                startButtonRoot.gameObject.CustomActive(flag);

            if (teamButtonRoot != null)
                teamButtonRoot.gameObject.CustomActive(flag);

            if (consumeRoot != null)
                consumeRoot.gameObject.CustomActive(flag);
        }
        
        private void OnTeamButtonClick()
        {
            if (_dungeonTable == null)
                return;

            if (IsTeamButtonLocked() == false)
            {
                Utility.OpenTeamFrame(_dungeonId);
            }
            else if (IsTeamBattleLevelLimit() == true)
            {

            }
            else
            {
                SystemNotifyManager.SystemNotify(3050);
            }
            
        }

        private void OnStartBtnClick()
        {
            if (mOnStartButtonClick != null)
            {
                mOnStartButtonClick.Invoke();
            }
        }

        public void OnStartButtonClick()
        {
            if (_dungeonTable == null)
                return;

            if(ChallengeUtility.isYunShangChangAn(_dungeonId))
            {
                if(_sinanItem == null)
                {
                    SystemNotifyManager.SysNotifyTextAnimation("请先放入司南！");
                    return;
                }
            }
            
            if (TeamDataManager.GetInstance().HasTeam())
            {

                if (TeamDataManager.GetInstance().IsTeamLeader())
                {
                    // 是否是组队副本
                    var teamDungeonTableId = 0;
                    if (!Utility.CheckIsTeamDungeon(_dungeonId, ref teamDungeonTableId))
                    {
                        SystemNotifyManager.SystemNotify(1106);
                        return;
                    }

                    // 各种条件判断
                    if (Utility.CheckTeamEnterDungeonCondition(teamDungeonTableId) == false)
                    {
                        return;
                    }
                }
                else
                {
                    SystemNotifyManager.SystemNotify(1105);
                    return;
                }
            }

            #region LevelResistMagicValue
            //关卡中存在侵蚀抗性，并且自身或者队员的侵蚀抗性不足，显示提示弹窗。否则不显示           
            var isShowResistMagicTip = false;
            var resistMagicTipContent = string.Empty;
            isShowResistMagicTip =
                DungeonUtility.IsShowDungeonResistMagicValueTip(_dungeonId, ref resistMagicTipContent);

            //显示抗魔值不足的提示
            if (isShowResistMagicTip == true)
            {
                var state = ChapterUtility.GetDungeonState(_dungeonId);
                var isLock = state == ComChapterDungeonUnit.eState.Locked;

                //关卡锁住不显示，关卡解锁，则显示提示弹框，并结束
                if (isLock == false)
                {
                    SystemNotifyManager.SysNotifyMsgBoxCancelOk(resistMagicTipContent, null,
                        MessageBoxOKCallBack);
                    return;
                }
            }
            #endregion

            ChapterUtility.OpenComsumeFatigueAddFrame(_dungeonId);

            GameFrameWork.instance.StartCoroutine(ChallengeChapterStart());

        }

        private void MessageBoxOKCallBack()
        {
            ChapterUtility.OpenComsumeFatigueAddFrame(_dungeonId);
            GameFrameWork.instance.StartCoroutine(ChallengeChapterStart());
        }

        private bool _isSendingMessage = false;

        protected IEnumerator ChallengeChapterStart()
        {
            if (_isSendingMessage == false)
            {
                if (ChapterUtility.GetDungeonCanEnter(_dungeonId) == false)
                {
                    yield break;
                }
                
                SceneDungeonStartReq req = new SceneDungeonStartReq
                {
                    dungeonId = (uint)_dungeonId,
                };

                if (_sinanItem != null)
                {
                    req.sinanId = _sinanItem.GUID;
                }

                // 使用buff药
                var costs = ChapterBuffDrugManager.GetInstance().GetAllMarkedBuffDrugsCost(_dungeonId);
                var isEnough2Cost = CostItemManager.GetInstance().IsEnough2Cost(costs);
                {
                    int result = -1;

                    CostItemManager.GetInstance().TryCostMoneiesDefault(costs,
                        () => { result = 1; },
                        () => { result = 0; });

                    while (isEnough2Cost && -1 == result)
                    {
                        yield return null;
                    }

                    if (result == 1)
                    {
                        req.buffDrugs = ChapterBuffDrugManager.GetInstance().GetAllMarkedBuffDrugsByDungeonID(_dungeonId).ToArray();
                        ChapterBuffDrugManager.GetInstance().ResetAllMarkedBuffDrugs();
                    }
                    else
                    {
                        yield break;
                    }
                }
                // 组队情况下，如果挑战目标和队伍目标不一致的话就则直接把队伍目标改掉
                if(TeamDataManager.GetInstance().HasTeam())
                {
                    TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)TeamDataManager.GetInstance().TeamDungeonID);
                    if(teamDungeonTable != null && teamDungeonTable.DungeonID != _dungeonId)
                    {
                        TeamDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.Target,TeamDataManager.GetTeamDungeonIDByDungeonID(_dungeonId));
                    }
                }

                var msg = new MessageEvents();
                var res = new SceneDungeonStartRes();

                _isSendingMessage = true;
                yield return (MessageUtility.Wait<SceneDungeonStartReq, SceneDungeonStartRes>(ServerType.GATE_SERVER, msg, req, res, false, 5));
                _isSendingMessage = false;
            }
        }


        private bool IsTeamButtonLocked()
        {
            var isChapterLock = ChallengeUtility.IsDungeonLock(_dungeonId);
            if (isChapterLock == true)
                return true;

            if (_mDungeonID != null && _mDungeonID.prestoryID > 0)
                return true;

            if (IsTeamBattleLevelLimit() == true)
                return true;

            return false;
        }

        private bool IsTeamBattleLevelLimit()
        {
            return !Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Team);
        }

    }
}
