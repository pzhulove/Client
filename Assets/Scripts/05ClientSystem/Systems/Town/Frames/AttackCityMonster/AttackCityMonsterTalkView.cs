using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Scripts.UI;

namespace GameClient
{

    public class RewardItemDataModel
    {
        public int Id;
        public int Number;
    }

    public class AttackCityMonsterTalkView : MonoBehaviour
    {

        [SerializeField] private Text monsterName;
        [SerializeField] private Text monsterTalkContent;
        [SerializeField] private Text monsterLimit;
        [SerializeField] private Text closeButtonText;
        [SerializeField] private Text monsterBeatText;
        [SerializeField] private Text hardLevelText;
        [SerializeField] private Text dropItemsText;
        [SerializeField] private Text beatLimitLabel;

        [SerializeField] private Image monsterImage;
        [SerializeField] private Button monsterBeatButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private ComUIListScript rewardItemList = null;

        [SerializeField] private List<GameObject> starList;

        private UInt64 _monsterGuid = 0;            //怪物唯一guid，由服务器决定
        private int _monsterId = -1;                //怪物npcid，对应NPCTable表中的数据
        private SceneNpc _monsterInfo = null;
        private NpcTable _npcItem = null;
        private TalkTable _talkTable = null;
        private const string UserNameStr = "[UserName]";

        private static Coroutine _beatAttackCityMonsterCoroutine = null;
        private static bool _isSendMessage = false;

        private List<RewardItemDataModel> _rewardDataModelList = null;

        private void Awake()
        {
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (monsterBeatButton != null)
                monsterBeatButton.onClick.AddListener(OnMonsterBeatClick);
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseFrame);

            if (rewardItemList != null)
            {
                rewardItemList.onItemSelected += OnRewardItemSelected;
                rewardItemList.onItemVisiable += OnRewardItemVisible;
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SyncAttackCityMonsterUpdate,OnSyncSceneNpcUpdate);
        }

        public void InitData(UInt64 npcGuid)
        {

            StopBeatAttackCityMonster();

            _monsterGuid = npcGuid;

            _monsterInfo = AttackCityMonsterDataManager.GetInstance().GetSceneNpcByNpcGuid(_monsterGuid);
            if (_monsterInfo == null)
            {
                return;
            }

            _monsterId = (int)_monsterInfo.id;
            _npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(_monsterId);

            InitTalkViewInfo();

            InitHardLevelInfo();
            InitRewardItemList();
        }

        private void InitTalkViewInfo()
        {
            if (_npcItem == null)
                return;

            if (monsterImage != null)
            {
                ETCImageLoader.LoadSprite(ref monsterImage, _npcItem.NpcBody);
                monsterImage.SetNativeSize();
            }

            if (monsterName != null)
            {
                monsterName.text = _npcItem.NpcName.Replace(UserNameStr, PlayerBaseData.GetInstance().Name);
            }

            _talkTable = TableManager.GetInstance().GetTableItem<TalkTable>(_npcItem.FunctionIntParam2);
            UpdateMonsterTalkContent();

            beatLimitLabel.text = TR.Value("monster_attack_beat_limit_label");
            monsterLimit.text = string.Format(TR.Value("monster_attack_city_limit"),
                AttackCityMonsterDataManager.GetInstance().GetLeftBeatTimes(),
                AttackCityMonsterDataManager.GetInstance()._attackCityMonsterTotalTimes);

            closeButtonText.text = TR.Value("monster_attack_city_not_beat");
            monsterBeatText.text = TR.Value("monster_attack_city_beat");

            //任务怪物不显示限制条件
            if ((CityMonsterGenerate.eMonsterType)_monsterInfo.funcType
                == CityMonsterGenerate.eMonsterType.Task)
            {
                monsterLimit.gameObject.CustomActive(false);
                beatLimitLabel.gameObject.CustomActive(false);
            }
        }

        private void InitHardLevelInfo()
        {
            if (hardLevelText != null)
                hardLevelText.text = TR.Value("monster_attack_city_hard_level");

            if(starList == null || starList.Count <= 0)
                return;

            if(_npcItem == null)
                return;

            var hardLevel = _npcItem.Hard;
            if(hardLevel <= 0)
                return;

            for (var i = 0; i < starList.Count; i++)
            {
                if (i < hardLevel)
                {
                    starList[i].transform.gameObject.CustomActive(true);
                }
                else
                {
                    starList[i].transform.gameObject.CustomActive(false);
                }
            }

        }

        private void InitRewardItemList()
        {
            if (dropItemsText != null)
                dropItemsText.text = TR.Value("monster_attack_city_drop_items");

            if (rewardItemList == null)
                return;

            //读取服务器中的地下城ID
            var dungeonId = (int)_monsterInfo.dungeonId;

            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if(dungeonTable == null || dungeonTable.DropItems.Count <= 0)
                return;

            if (_rewardDataModelList == null)
                _rewardDataModelList = new List<RewardItemDataModel>();
            _rewardDataModelList.Clear();

            for (var i = 0; i < dungeonTable.DropItems.Count; i++)
            {
                var dropItemId = dungeonTable.DropItems[i];
                var curDataModel = new RewardItemDataModel
                {
                    Id = dropItemId,
                    Number = 1,
                };
                _rewardDataModelList.Add(curDataModel);
            }
            
            if (_rewardDataModelList == null || _rewardDataModelList.Count <= 0)
            {
                rewardItemList.transform.gameObject.CustomActive(false);
                return;
            }

            rewardItemList.transform.gameObject.CustomActive(true);
            rewardItemList.Initialize();
            rewardItemList.SetElementAmount(_rewardDataModelList.Count);
        }

        private void UnBindUiEventSystem()
        {
            if(monsterBeatButton != null)
                monsterBeatButton.onClick.RemoveAllListeners();
            if(closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (rewardItemList != null)
            {
                rewardItemList.onItemSelected -= OnRewardItemSelected;
                rewardItemList.onItemVisiable -= OnRewardItemVisible;
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SyncAttackCityMonsterUpdate, OnSyncSceneNpcUpdate);
        }
        
        private void OnDestroy()
        {
            ClearData();
            _monsterInfo = null;
            _npcItem = null;
            _talkTable = null;

            UnBindUiEventSystem();
            AttackCityMonsterDataManager.GetInstance().ResetOpenTalkFrameType();
        }

        private void ClearData()
        {
            if (_rewardDataModelList != null)
            {
                _rewardDataModelList.Clear();
                _rewardDataModelList = null;
            }
        }

        private void OnMonsterBeatClick()
        {
            OnMonsterBeat();

            OnCloseFrame();
        }

        private void OnMonsterBeat()
        {
            //等级不足，直接返回
            if (PlayerBaseData.GetInstance().Level < AttackCityMonsterDataManager.LimitLevel)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("monster_attack_city_level_not_satisfied"));
                return;
            }

            //活动性怪物，需要判断是否存在队伍，是否为队长，以及队员的等级情况
            if(_monsterInfo != null && 
               (CityMonsterGenerate.eMonsterType)_monsterInfo.funcType
               == CityMonsterGenerate.eMonsterType.Activity)
            {

                //不存在队伍
                Team myTeam = TeamDataManager.GetInstance().GetMyTeam();
                if (myTeam == null)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("monster_attack_city_no_army"));
                    return;
                }

                //不是队长
                if (false == TeamDataManager.GetInstance().IsTeamLeader())
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("monster_attack_city_not_be_captain"));
                    return;
                }

                //队员等级不足
                for (var i = 0; i < myTeam.currentMemberCount; i++)
                {
                    if(i >= myTeam.members.Length)
                        continue;

                    if (myTeam.members[i].avatarInfo != null && myTeam.members[i].id != 0)
                    {
                        if (myTeam.members[i].level < AttackCityMonsterDataManager.LimitLevel)
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("monster_attack_city_team_member_level_not_satisfied"));
                            return;
                        }
                    }
                }
            }

            //显示配置技能的提示框
            var isShowSkillConfigTipFrame =
                SkillDataManager.GetInstance().IsShowSkillTreeFrameTipBySkillConfig(OnEnterGame);
            if (isShowSkillConfigTipFrame == true)
                return;

            StartBeatAttackCityMonster(_monsterGuid);
        }

        private void OnEnterGame()
        {
            StartBeatAttackCityMonster(_monsterGuid);
        }

        private void OnRewardItemVisible(ComUIListElementScript item)
        {
            if(item == null)
                return;

            var itemIndex = item.m_index;
            if(itemIndex < 0 || itemIndex >= _rewardDataModelList.Count)
                return;

            var rewardDataModel = _rewardDataModelList[itemIndex];
            if(rewardDataModel == null)
                return;

            var attackCityMonsterRewardItem = item.GetComponent<AttackCityMonsterRewardItem>();
            if(attackCityMonsterRewardItem == null)
                return;

            attackCityMonsterRewardItem.InitData(rewardDataModel);
        }

        private void OnRewardItemSelected(ComUIListElementScript item)
        {
            if(item == null)
                return;
        }

        private void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<AttackCityMonsterTalkFrame>();
        }

        private void OnSyncSceneNpcUpdate(UIEvent uiEvent)
        {
            _monsterInfo = AttackCityMonsterDataManager.GetInstance().GetSceneNpcByNpcGuid(_monsterGuid);
            if(_monsterInfo == null)
                return;

            UpdateMonsterTalkContent();
        }

        private void UpdateMonsterTalkContent()
        {
            if(_talkTable == null)
                return;
            var talkText = _talkTable.TalkText.Replace(UserNameStr, PlayerBaseData.GetInstance().Name);
            var totalNumber = (int)_monsterInfo.totalTimes;
            var remainNumber = (int)_monsterInfo.remainTimes;
            monsterTalkContent.text = string.Format(talkText, remainNumber, totalNumber);
        }

        #region BeatAttackCityMonster
        private static void StartBeatAttackCityMonster(UInt64 guid)
        {
            _beatAttackCityMonsterCoroutine =
                GameFrameWork.instance.StartCoroutine(StartBeatAttackCityMonsterCoroutine(guid));
        }

        private static void StopBeatAttackCityMonster()
        {
            if (_beatAttackCityMonsterCoroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(_beatAttackCityMonsterCoroutine);
                _beatAttackCityMonsterCoroutine = null;
            }

            _isSendMessage = false;
        }

        private static IEnumerator StartBeatAttackCityMonsterCoroutine(UInt64 guid)
        {
            if (_isSendMessage == false)
            {
                var req = new SceneDungeonStartReq
                {
                    cityMonsterId = guid,
                };

                var msg = new MessageEvents();
                var res = new SceneDungeonStartRes();
                _isSendMessage = true;

                yield return (MessageUtility.Wait<SceneDungeonStartReq, SceneDungeonStartRes>(ServerType.GATE_SERVER,
                    msg, req, res, true, 10));
                _isSendMessage = false;
            }
        }
        #endregion

    }
}
