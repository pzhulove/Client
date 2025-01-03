using System.Collections.Generic;
using Network;
using Protocol;
using ProtoTable;

namespace GameClient
{
    public enum TAPType
    {
        Pupil = 1, //徒弟 在0 - 45级的玩家
        TeacherSoon = 2,//45级 - 50级的玩家，既不能是师傅，也不能是徒弟
        Teacher = 3, //师傅，50级以上的玩家 
    }

    public class TAPQuestionnaireInformation
    {
        public byte activeTimeType;
        public byte masterType;
        public byte regionId;
        public string declaration;

        public TAPQuestionnaireInformation(byte activeTimeType, byte masterType,byte regionId,string declaration)
        {
            this.activeTimeType = activeTimeType;
            this.masterType = masterType;
            this.regionId = regionId;
            this.declaration = declaration;
        }
    }
    public class TAPNewDataManager : DataManager<TAPNewDataManager>
    {
        public const int TAPNpc = 2020;
        public TAPQuestionnaireInformation tapQuestionnaireInformation = null;                                 //用于获得初始数据，并且在没有改变的时候不传给服务器。
        Dictionary<string, List<string>> firstLetterProvinceDic = new Dictionary<string, List<string>>();      //表格数据 省份首字母-省份
        Dictionary<string, int> provinceIDDic = new Dictionary<string, int>();                                 //表格数据 省份-表id

        Dictionary<ulong, ClassmateRelationData> classmateRelationDic = new Dictionary<ulong, ClassmateRelationData>();//同门数据

        List<MissionInfo> MyDailyMission = new List<MissionInfo>(); //我作为徒弟的时候，我自己的每日任务
        public MasterTaskShareData myTAPData = new MasterTaskShareData();
        Dictionary<ulong, MasterTaskShareData> MyPupilMissionDic = new Dictionary<ulong, MasterTaskShareData>(); //我，作为师傅的时候，徒弟的id  --  他们的师门任务列表

        private ulong teacherPublishTime = 0;//拜师的惩罚时间戳，到这个时间戳才能拜师 （徒弟用）
        private ulong pupilPublishTime = 0;//收徒的惩罚时间戳，到这个时间才能收徒（师傅用）
        
        private ulong curPupilId = 0;

        /// <summary>
        ///寻找师父是否请求服务器，解决拜师弹框点击寻找师傅打开界面为空
        /// </summary>
        public static bool FindmasterIsSendServer { get; set; }

        public override void Initialize()
        {
            tapQuestionnaireInformation = null;
            myTAPData = new MasterTaskShareData();
            provinceIDDic.Clear();
            firstLetterProvinceDic.Clear();
            classmateRelationDic.Clear();
            MyDailyMission.Clear();
            MyPupilMissionDic.Clear();
            teacherPublishTime = 0;
            curPupilId = 0;
            pupilPublishTime = 0;
            FindmasterIsSendServer = false;
            _RegisterNetHandler();
            AddSystemInvoke();
            _BindEvents();
            _GetAreaProvinceTableData();
        }
        
        public override void Clear()
        {
            tapQuestionnaireInformation = null;
            provinceIDDic.Clear();
            firstLetterProvinceDic.Clear();
            classmateRelationDic.Clear();
            MyDailyMission.Clear();
            MyPupilMissionDic.Clear();
            myTAPData = null;
            teacherPublishTime = 0;  
            curPupilId = 0;
            pupilPublishTime = 0;
            FindmasterIsSendServer = false;
            m_akQueriedIds.Clear();
            m_akIWantApplyedPupils.Clear();
            _UnRegisterNetHandler();
            RemoveSystemInvoke();
            _UnBindEvents();
        }
        
        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }
        void _RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(WorldNotifyNewMasterSectRelation.MsgID, _OnWorldNotifyNewMasterSectRelation);
            NetProcess.AddMsgHandler(WorldSyncMasterSectRelationData.MsgID, _OnWorldSyncMasterSectRelationData);
            NetProcess.AddMsgHandler(WorldSyncMasterSectRelationList.MsgID, _OnWorldSyncMasterSectRelationList);
            NetProcess.AddMsgHandler(WorldNotifyDelMasterSectRelation.MsgID, _OnWorldNotifyDelMasterSectRelation);
            NetProcess.AddMsgHandler(WorldSetDiscipleQuestionnaireRes.MsgID, _OnWorldSetDiscipleQuestionnaireRes);
            NetProcess.AddMsgHandler(WorldSetMasterQuestionnaireRes.MsgID, _OnWorldSetMasterQuestionnaireRes);
            NetProcess.AddMsgHandler(WorldSyncRelationQuestionnaire.MsgID, _OnWorldSyncRelationQuestionnaire);

            NetProcess.AddMsgHandler(WorldGetDiscipleMasterTasksRes.MsgID, _OnWorldGetDiscipleMasterTasksRes);

            //新增
            NetProcess.AddMsgHandler(SceneSyncMasterTaskList.MsgID, _OnSceneSyncMasterTaskListRes);
            NetProcess.AddMsgHandler(WorldReportMasterTaskRes.MsgID, _onWorldReportMasterTaskRes);
            NetProcess.AddMsgHandler(WorldSubmitMasterTaskRes.MsgID, _onWorldSubmitMasterTaskRes);
            NetProcess.AddMsgHandler(WorldUpdateMasterTaskSync.MsgID, _OnWorldUpdateMasterTaskSyncRes);
            
            NetProcess.AddMsgHandler(WorldReceiveMasterDailyTaskRewardRes.MsgID, _OnWorldReceiveMasterDailyTaskRewardRes);
            NetProcess.AddMsgHandler(WorldDiscipleFinishSchoolRes.MsgID, _OnWorldDiscipleFinishSchoolRes);
            NetProcess.AddMsgHandler(WorldAutomaticFinishSchoolRes.MsgID, _OnWorldAutomaticFinishSchoolRes);

            NetProcess.AddMsgHandler(SceneNotifyNewTaskRet.MsgID, _OnSceneNotifyNewTaskRet);
            NetProcess.AddMsgHandler(SceneNotifyDeleteTaskRet.MsgID, _OnSceneNotifyDeleteTaskRet);
            NetProcess.AddMsgHandler(SceneNotifyTaskVarRet.MsgID, _OnSceneNotifyTaskVarRet);
            NetProcess.AddMsgHandler(SceneNotifyTaskStatusRet.MsgID, _OnSceneNotifyTaskStatus);

            NetProcess.AddMsgHandler(WorldSyncMasterDisciplePunishTime.MsgID, _OnWorldSyncMasterDisciplePunishTime);
            NetProcess.AddMsgHandler(WorldSyncOnOffline.MsgID, _OnSyncOffline);
            NetProcess.AddMsgHandler(WorldNotifyFinSchEvent.MsgID, _OnWorldNotifyFinSchEvent);

            ////
            NetProcess.AddMsgHandler(QueryMasterSettingRes.MsgID, _OnRecvQueryMasterSettingRes);
            NetProcess.AddMsgHandler(SetRecvDiscipleStateRet.MsgID, _OnRecvSetRecvDiscipleStateRet);
            NetProcess.AddMsgHandler(SetMasterNoteRet.MsgID, _OnRecvSetMasterNoteRet);
            
        }

        void _UnRegisterNetHandler()
        {
            //同门关系
            NetProcess.RemoveMsgHandler(WorldNotifyNewMasterSectRelation.MsgID, _OnWorldNotifyNewMasterSectRelation); //通知新的同门关系
            NetProcess.RemoveMsgHandler(WorldSyncMasterSectRelationData.MsgID, _OnWorldSyncMasterSectRelationData);// 同步同门关系，WorldUpdateMasterSectRelationReq的返回
            NetProcess.RemoveMsgHandler(WorldSyncMasterSectRelationList.MsgID, _OnWorldSyncMasterSectRelationList);// 上线的时候同步同门关系
            NetProcess.RemoveMsgHandler(WorldNotifyDelMasterSectRelation.MsgID, _OnWorldNotifyDelMasterSectRelation);// 删除同门关系

            //问卷调查
            NetProcess.RemoveMsgHandler(WorldSetDiscipleQuestionnaireRes.MsgID, _OnWorldSetDiscipleQuestionnaireRes);// 徒弟问卷返回
            NetProcess.RemoveMsgHandler(WorldSetMasterQuestionnaireRes.MsgID, _OnWorldSetMasterQuestionnaireRes);// 师傅问卷的返回
            NetProcess.RemoveMsgHandler(WorldSyncRelationQuestionnaire.MsgID, _OnWorldSyncRelationQuestionnaire);//角色上线同步问卷调查信息

            //主界面任务相关的协议
            NetProcess.RemoveMsgHandler(WorldGetDiscipleMasterTasksRes.MsgID, _OnWorldGetDiscipleMasterTasksRes);//拉去徒弟师门任务数据返回 ,包括每日任务，和成长任务。
            NetProcess.RemoveMsgHandler(WorldReceiveMasterDailyTaskRewardRes.MsgID, _OnWorldReceiveMasterDailyTaskRewardRes);//领取每日任务的奖励

            NetProcess.RemoveMsgHandler(SceneSyncMasterTaskList.MsgID, _OnSceneSyncMasterTaskListRes);
            NetProcess.RemoveMsgHandler(WorldReportMasterTaskRes.MsgID, _onWorldReportMasterTaskRes);
            NetProcess.RemoveMsgHandler(WorldSubmitMasterTaskRes.MsgID, _onWorldSubmitMasterTaskRes);
            NetProcess.RemoveMsgHandler(WorldUpdateMasterTaskSync.MsgID, _OnWorldUpdateMasterTaskSyncRes);

            NetProcess.RemoveMsgHandler(WorldDiscipleFinishSchoolRes.MsgID, _OnWorldDiscipleFinishSchoolRes);//出师的返回
            NetProcess.RemoveMsgHandler(WorldAutomaticFinishSchoolRes.MsgID, _OnWorldAutomaticFinishSchoolRes);

            NetProcess.RemoveMsgHandler(SceneNotifyNewTaskRet.MsgID, _OnSceneNotifyNewTaskRet);//增加任务
            NetProcess.RemoveMsgHandler(SceneNotifyDeleteTaskRet.MsgID, _OnSceneNotifyDeleteTaskRet);//删除任务
            NetProcess.RemoveMsgHandler(SceneNotifyTaskVarRet.MsgID, _OnSceneNotifyTaskVarRet);//刷新
            NetProcess.RemoveMsgHandler(SceneNotifyTaskStatusRet.MsgID, _OnSceneNotifyTaskStatus);

            NetProcess.RemoveMsgHandler(WorldSyncMasterDisciplePunishTime.MsgID, _OnWorldSyncMasterDisciplePunishTime);//惩罚时间
            NetProcess.RemoveMsgHandler(WorldSyncOnOffline.MsgID, _OnSyncOffline);//同步上下线状态，这边专用于同门
            NetProcess.RemoveMsgHandler(WorldNotifyFinSchEvent.MsgID, _OnWorldNotifyFinSchEvent);//someone出师成功的广播。
            
            ///
            NetProcess.RemoveMsgHandler(QueryMasterSettingRes.MsgID, _OnRecvQueryMasterSettingRes);
            NetProcess.RemoveMsgHandler(SetRecvDiscipleStateRet.MsgID, _OnRecvSetRecvDiscipleStateRet);
            NetProcess.RemoveMsgHandler(SetMasterNoteRet.MsgID, _OnRecvSetMasterNoteRet);
            
        }

        void _BindEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSearchedTeacherListChanged, OnSearchedTeacherListChanged);
            PlayerBaseData.GetInstance().onLevelChanged += _OnLevelChanged;
        }

        void _UnBindEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSearchedTeacherListChanged, OnSearchedTeacherListChanged);
            PlayerBaseData.GetInstance().onLevelChanged -= _OnLevelChanged;
        }
        

        private void _GetAreaProvinceTableData()
        {
            var AreaProvinceTableData = TableManager.GetInstance().GetTable<AreaProvinceTable>();
            if(AreaProvinceTableData != null)
            {
                var enumerator = AreaProvinceTableData.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var AreaProvinceTableItem = enumerator.Current.Value as AreaProvinceTable;
                    if(AreaProvinceTableItem == null)
                    {
                        continue;
                    }

                    //为firstLetterProcinveDic赋值
                    if (firstLetterProvinceDic.ContainsKey(AreaProvinceTableItem.FirstLetter))
                    {
                        if (!firstLetterProvinceDic[AreaProvinceTableItem.FirstLetter].Contains(AreaProvinceTableItem.Name))
                        {
                            firstLetterProvinceDic[AreaProvinceTableItem.FirstLetter].Add(AreaProvinceTableItem.Name);
                        }
                    }
                    else
                    {
                        List<string> areaList = new List<string>();
                        areaList.Add(AreaProvinceTableItem.Name);
                        firstLetterProvinceDic[AreaProvinceTableItem.FirstLetter] = areaList;
                    }
                    provinceIDDic[AreaProvinceTableItem.Name] = AreaProvinceTableItem.ID;
                }
            }
        }

        #region public method
        ///一开始进入游戏，没打开过师徒界面不会发送徒弟任务，需要去请求下。
        public void GetPupilsTasks()
        {
            var pupilDatas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
            foreach (var pupil in pupilDatas)
            {
                if (pupil != null)
                {
                    GetPupilMissionList(pupil.uid);
                }
            }
        }



        /// <summary>
        /// 我是师父，判断徒弟是否有需要确认的任务
        /// </summary>
        /// <param name="relationData"></param>
        /// <returns></returns>
        public bool HaveCouldSubmitTasks(RelationData relationData)
        {
            if (relationData == null)
            {
                return false;
            }

            if (relationData.isOnline < 1)
            {
                return false;
            }

            var data = GetMyPupilData(relationData.uid);
            if (data == null)
            {
                return false;
            }

            var tasks = data.dailyTasks;
            if (tasks == null)
            {
                return false;
            }

            foreach (var mission in tasks)
            {
                if (mission != null && mission.status == (int)TaskStatus.TASK_FINISHED)
                {
                    for (int i = 0; i < mission.akMissionPairs.Length; i++)
                    {
                        if (mission.akMissionPairs[i].key == "report_status")
                        {
                            if (mission.akMissionPairs[i].value == "1")
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool HaveSubmitRedPoint()
        {
            var pupilDatas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
            foreach (var pupil in pupilDatas)
            {
                if (pupil != null)
                {
                    if (HaveCouldSubmitTasks(pupil))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public bool HaveLeaveMasterRedPoint()
        {
            var pupilDatas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
            foreach (var pupil in pupilDatas)
            {
                if (pupil != null)
                {
                    if (pupil.level >= 50)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HaveReportRedPoint()
        {
            if (canSearchTeacher())
            {
                return false;
            }
            if (MyDailyMission == null)
            {
                return false;
            }

            foreach (var task in MyDailyMission)
            {
                
                if (task != null && task.status == (int)TaskStatus.TASK_FINISHED)
                {
                    if (task.akMissionPairs.Length == 0)
                    {
                        return true;
                    }
                    else
                    {
                        bool haveKey = false;
                        for (int i = 0; i < task.akMissionPairs.Length; i++)
                        {
                            if (task.akMissionPairs[i].key == "report_status")
                            {
                                haveKey = true;
                            }
                        }

                        if (!haveKey)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        
        
        public bool HaveApplyRedPoint()
        {
            //当有徒弟或师父后，即使申请list有人申请，也不显示小红点了
            bool havePupilPoint = false;
            bool haveMasterPoint = false;
            if (RelationDataManager.GetInstance().ApplyPupils.Count > 0)
            {
                var pupilDatas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
                if (pupilDatas.Count <= 0)
                {
                    havePupilPoint = true;
                }
            }
            
            if (RelationDataManager.GetInstance().ApplyTeachers.Count > 0)
            {
                var teacherDatas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_MASTER);
                if (teacherDatas.Count <= 0)
                {
                    haveMasterPoint = true;
                }
            }
            if(havePupilPoint || haveMasterPoint)
            {
                return true;
            }
            return false;
        }
        
        //徒弟显示小红点用
        public bool HaveTAPDailyRedPointForID(RelationData data)
        {
            if (data == null)
            {
                return false;
            }

            bool canLeave = data.level >= 50;
            bool needSubMit = HaveCouldSubmitTasks(data);
            return (canLeave || needSubMit);
        }
        
        /// <summary>
        /// 获取首字母的链表
        /// </summary>
        /// <returns></returns>
        public List<string> GetFirstLetterList()
        {
            List<string> firstLetterList = new List<string>();
            foreach (string key in firstLetterProvinceDic.Keys)
            {
                firstLetterList.Add(key);
            }
            return firstLetterList;
        }

        /// <summary>
        /// 根据首字母获取地区链表
        /// </summary>
        /// <param name="firstLetter"></param>
        /// <returns></returns>
        public List<string> GetRegionList(string firstLetter)
        {
            if(firstLetterProvinceDic.ContainsKey(firstLetter))
            {
                return firstLetterProvinceDic[firstLetter];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据地区的string获得地区id
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public int GetRegionID(string region)
        {
            if(provinceIDDic.ContainsKey(region))
            {
                return provinceIDDic[region];
            }
            else
            {
                return 0;
            }
        }

        public TAPType IsTeacher()
        {
            int myLevel = PlayerBaseData.GetInstance().Level;
            if (myLevel > 0 && myLevel <= 45)
            {
                return TAPType.Pupil;
            }
            else if(myLevel > 45 && myLevel < 50)
            {
                return TAPType.TeacherSoon;
            }
            else
            {
                //到达50级可以成为师傅，但是不一定就是师傅
                return TAPType.Teacher;
            }
        }

        /// <summary>
        /// 获得同门字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<ulong, ClassmateRelationData> GetClassmateRelationDic()
        {
            return classmateRelationDic;
        }

        /// <summary>
        /// 通过徒弟的id获得徒弟的相关数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MasterTaskShareData GetMyPupilData(ulong id)
        {
            if(MyPupilMissionDic.ContainsKey(id))
            {
                return MyPupilMissionDic[id];
            }
            else
            {
                return null;
            }
        }
        

        /// <summary>
        /// 和某个人聊天
        /// </summary>
        /// <param name="relationData"></param>
        public void _TalkToPeople(RelationData relationData,string talkStr = null)
        {
            RelationDataManager.GetInstance().OnAddPriChatList(relationData, false);
            RelationFrameData relationFrameData = new RelationFrameData();
            relationFrameData.eCurrentRelationData = relationData;
            if (!ClientSystemManager.GetInstance().IsFrameOpen<RelationFrameNew>())
            {
                RelationFrameNew.CommandOpen(relationFrameData);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnCloseMenu);
            if(talkStr == null)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPStartTalk, relationFrameData,"");
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPStartTalk, relationFrameData, talkStr);
            }
        }
        
        
        //虽然没引用，给超链接用的
        public static void SendTeachers(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return;
            }

            ulong guid = 0;
            if (!ulong.TryParse(param, out guid))
            {
                return;
            }

            TAPNewDataManager.GetInstance().SendApplyTeacher(guid);
        }

        public static void SendPupils(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return;
            }

            ulong guid = 0;
            if (!ulong.TryParse(param, out guid))
            {
                return;
            }
            TAPNewDataManager.GetInstance().SendApplyPupil(guid);
        }



        public void SendApplyTeacher(ulong uid)
        {
            RelationDataManager.GetInstance().AddRelationByID(uid, RequestType.RequestMaster);
        }

        public void SendApplyPupil(ulong uid)
        {
            RelationDataManager.GetInstance().AddRelationByID(uid, RequestType.RequestDisciple);
        }

        public string getPublishTime(TAPType type)
        {
            if(type == TAPType.Pupil)
            {
                if(pupilPublishTime > TimeManager.GetInstance().GetServerTime())
                {
                    return string.Format("解除师徒关系惩罚时间：{0}", Function.SetShowTimeMin((int)pupilPublishTime));

                }
                else
                {
                    return "";
                }
            }
            else
            {
                if(teacherPublishTime > TimeManager.GetInstance().GetServerTime())
                {
                    return string.Format("解除师徒关系惩罚时间：{0}", Function.SetShowTimeMin((int)teacherPublishTime));

                }
                else
                {
                    return "";
                }
            }
        }

        public bool canSearchPupil()
        {
            var datas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
            if (datas.Count >= 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool canSearchTeacher()
        {
            var data = RelationDataManager.GetInstance().GetTeacher();
            if(data != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region 协议相关
        public void AnnounceWorld(RelationAnnounceType type)
        {
            WorldRelationAnnounceReq req = new WorldRelationAnnounceReq();
            req.type = (uint)type;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }
        

        /// <summary>
        /// 发送协议给服务器，玩家个人信息
        /// </summary>
        /// <param name="activeTimeIndex"></param>
        /// <param name="abilityIndex"></param>
        /// <param name="regionIndex"></param>
        /// <param name="declaration"></param>
        public void SendTAPInformation(int activeTimeIndex,int abilityIndex,int regionIndex,string declaration)
        {
            TAPType myType = IsTeacher();
            if ((int)myType > 1)
            {
                WorldSetMasterQuestionnaireReq req = new WorldSetMasterQuestionnaireReq();
                req.activeTimeType = (byte)activeTimeIndex;
                req.masterType = (byte)abilityIndex;
                req.regionId = (byte)regionIndex;
                req.declaration = declaration;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            }
            else
            {
                WorldSetDiscipleQuestionnaireReq req = new WorldSetDiscipleQuestionnaireReq();
                req.activeTimeType = (byte)activeTimeIndex;
                req.masterType = (byte)abilityIndex;
                req.regionId = (byte)regionIndex;
                req.declaration = declaration;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            }
        }
        

        /// <summary>
        /// 得到同门的信息,打开界面的时候，因为一开始同步的是初始数据，可能中途改变。
        /// </summary>
        public void GetClassmateInformation()
        {
            WorldUpdateMasterSectRelationReq req = new WorldUpdateMasterSectRelationReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 通过id拉去徒弟的师门任务数据
        /// </summary>
        /// <param name="id"></param>
        public void GetPupilMissionList(ulong id)
        {
            WorldGetDiscipleMasterTasksReq req = new WorldGetDiscipleMasterTasksReq();
            req.discipleId = id;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        //徒弟一键汇报任务请求
        public void SendReportMission(ulong id)
        {
            WorldReportMasterTaskReq req = new WorldReportMasterTaskReq();
            req.masterId = id;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }
        
        
        //师父确认师门任务请求
        public void SendSubmitMasterTaskReq(ulong id)
        {
            WorldSubmitMasterTaskReq req = new WorldSubmitMasterTaskReq();
            req.discipleId = id;
            curPupilId = id;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }
        

        /// <summary>
        /// 查找师傅或者徒弟的请求
        /// </summary>
        /// <param name="type"></param>
        public void SendChangeSearchedPupilList(RelationFindType type)
        {
            WorldRelationFindPlayersReq req = new WorldRelationFindPlayersReq();
            req.type = (byte)type;
            req.name = "";
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 出师协议
        /// </summary>
        /// <param name="id"></param>
        public void SendGraduation(ulong id)
        {
            WorldDiscipleFinishSchoolReq req = new WorldDiscipleFinishSchoolReq();
            req.discipleId = id;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendGraduationLate(ulong id)
        {
            WorldAutomaticFinishSchoolReq req = new WorldAutomaticFinishSchoolReq();
            req.targetId = id;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 通知新的同门关系
        /// </summary>
        /// <param name="msg"></param>
        void _OnWorldNotifyNewMasterSectRelation(MsgDATA msg)
        {
            int pos = 0;
            MasterSectRelation masterSectRelation = MasterSectRelationDecoder.DecodeNew(msg.bytes, ref pos, msg.bytes.Length);
            ClassmateRelationData classmateRelationData = new ClassmateRelationData();
            classmateRelationData.uid = masterSectRelation.uid;
            classmateRelationData.name = masterSectRelation.name;
            classmateRelationData.level = masterSectRelation.level;
            classmateRelationData.occu = masterSectRelation.occu;
            classmateRelationData.type = masterSectRelation.type;
            classmateRelationData.vipLv = masterSectRelation.vipLv;
            classmateRelationData.status = masterSectRelation.status;
            classmateRelationData.isFinSch = masterSectRelation.isFinSch;
            
            if(classmateRelationData.type == (int)MasterSectRelationType.MSRELATION_BROTHER)
            {
                classmateRelationDic[classmateRelationData.uid] = classmateRelationData;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshClassmateDic);

            }
        }

        /// <summary>
        /// 删除同门关系
        /// </summary>
        /// <param name="msg"></param>
        void _OnWorldNotifyDelMasterSectRelation(MsgDATA msg)
        {
            WorldNotifyDelMasterSectRelation msgData = new WorldNotifyDelMasterSectRelation();
            msgData.decode(msg.bytes);
            classmateRelationDic.Remove(msgData.id);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshClassmateDic);
        }

        /// <summary>
        /// 同步同门关系，WorldUpdateMasterSectRelationReq的返回
        /// </summary>
        /// <param name="msg"></param>
        void _OnWorldSyncMasterSectRelationData(MsgDATA msg)
        {

            int pos = 0; 
            MasterSectRelation masterSectRelation = MasterSectRelationDecoder.DecodeData(msg.bytes, ref pos, msg.bytes.Length);
            ClassmateRelationData classmateRelationData = new ClassmateRelationData();
            classmateRelationData.uid = masterSectRelation.uid;
            classmateRelationData.name = masterSectRelation.name;
            classmateRelationData.level = masterSectRelation.level;
            classmateRelationData.occu = masterSectRelation.occu;
            classmateRelationData.type = masterSectRelation.type;
            classmateRelationData.vipLv = masterSectRelation.vipLv;
            classmateRelationData.status = masterSectRelation.status;
            classmateRelationData.isFinSch = masterSectRelation.isFinSch;
            if (classmateRelationData.type == (int)MasterSectRelationType.MSRELATION_BROTHER)
            {
                classmateRelationDic[classmateRelationData.uid] = classmateRelationData;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshClassmateDic);
            }
            
        }

        /// <summary>
        /// 上线的时候同步同门关系
        /// </summary>
        /// <param name="msg"></param>
        void _OnWorldSyncMasterSectRelationList(MsgDATA msg)
        {
            int pos = 0;
            classmateRelationDic.Clear();
            //BaseDLL.decode_int8(msg.bytes, ref pos, ref type);
            List<MasterSectRelation> masterSectRelations = MasterSectRelationDecoder.DecodeList(msg.bytes, ref pos, msg.bytes.Length);
            for(int i = 0;i< masterSectRelations.Count;i++)
            {
                ClassmateRelationData classmateRelationData = new ClassmateRelationData();
                classmateRelationData.uid = masterSectRelations[i].uid;
                classmateRelationData.name = masterSectRelations[i].name;
                classmateRelationData.level = masterSectRelations[i].level;
                classmateRelationData.occu = masterSectRelations[i].occu;
                classmateRelationData.type = masterSectRelations[i].type;
                classmateRelationData.vipLv = masterSectRelations[i].vipLv;
                classmateRelationData.status = masterSectRelations[i].status;
                classmateRelationData.isFinSch = masterSectRelations[i].isFinSch;

                if (classmateRelationData.type == (int)MasterSectRelationType.MSRELATION_BROTHER)
                {
                    classmateRelationDic[classmateRelationData.uid] = classmateRelationData;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshClassmateDic);
                }
            }
            
        }

        /// <summary>
        /// 徒弟问卷返回
        /// </summary>
        /// <param name="msgData"></param>
        void _OnWorldSetDiscipleQuestionnaireRes(MsgDATA msgData)
        {
            WorldSetDiscipleQuestionnaireRes res = new WorldSetDiscipleQuestionnaireRes();
            res.decode(msgData.bytes);
            if (res.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
                return;
            }
            if (res.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
                return;
            }
        }

        /// <summary>
        /// 师傅问卷的返回
        /// </summary>
        /// <param name="msgData"></param>
        void _OnWorldSetMasterQuestionnaireRes(MsgDATA msgData)
        {
            WorldSetMasterQuestionnaireRes res = new WorldSetMasterQuestionnaireRes();
            res.decode(msgData.bytes);
            if (res.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
                return;
            }
        }

        /// <summary>
        /// 角色上线的时候同步问卷调查的信息
        /// </summary>
        /// <param name="msgData"></param>
        void _OnWorldSyncRelationQuestionnaire(MsgDATA msgData)
        {
            WorldSyncRelationQuestionnaire res = new WorldSyncRelationQuestionnaire();
            res.decode(msgData.bytes);
            TAPQuestionnaireInformation tempInformation = new TAPQuestionnaireInformation(res.activeTimeType, res.masterType, res.regionId, res.declaration);
            tapQuestionnaireInformation = tempInformation;
        }

        /// <summary>
        /// 同步师门日常任务列表
        /// </summary>
        /// <param name="msgData"></param>
        void _OnSceneSyncMasterTaskListRes(MsgDATA msgData)
        {
            SceneSyncMasterTaskList res = new SceneSyncMasterTaskList();
            res.decode(msgData.bytes);
            MyDailyMission.Clear();
            MyDailyMission = res.tasks.ToList();
            myTAPData.dailyTasks = res.tasks;
            //师父拿徒弟任务，顺带着请求了。
            GetPupilsTasks();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPLearningUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPTeacherValueChange);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTapPupilReportRedPoint);
        }

        /// <summary>
        /// 拉去徒弟师门任务数据返回
        /// </summary>
        /// <param name="msgData"></param>
        void _OnWorldGetDiscipleMasterTasksRes(MsgDATA msgData)
        {
            WorldGetDiscipleMasterTasksRes res = new WorldGetDiscipleMasterTasksRes();
            res.decode(msgData.bytes);
            if (res.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
                return;
            }
            ulong pupilID = res.discipleId;
            MyPupilMissionDic[pupilID] = res.masterTaskShareData;

            var datas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
            for(int i = 0;i<datas.Count;i++)
            {
                if(datas[i].uid == pupilID)
                {
                    RelationData tempRelationData = datas[i];
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPTeacherValueChange, tempRelationData);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPLearningUpdate, tempRelationData);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTapTeacherSubmitRedPoint);
                }
            }
        }


        /// <summary>
        /// 徒弟汇报任务返回
        /// /// <param name="msgData"></param>
        /// </summary>
        void _onWorldReportMasterTaskRes(MsgDATA msgData)
        {
            WorldReportMasterTaskRes res = new WorldReportMasterTaskRes();
            res.decode(msgData.bytes);
            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
                return;
            }
        }

        /// <summary>
        /// 师父领取师门任务返回
        /// /// <param name="msgData"></param>
        /// </summary>
        void _onWorldSubmitMasterTaskRes(MsgDATA msgData)
        {
            WorldSubmitMasterTaskRes res = new WorldSubmitMasterTaskRes();
            res.decode(msgData.bytes);
            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
                return;
            }

            if (curPupilId > 0)
            {
                GetPupilMissionList(curPupilId);   
            }
        }


        /// <summary>
        /// 服务器通知刷新师父数据
        /// </summary>
        /// <param name="msgData"></param>
        ///
        void _OnWorldUpdateMasterTaskSyncRes(MsgDATA msgData)
        {
            WorldUpdateMasterTaskSync res = new WorldUpdateMasterTaskSync();
            res.decode(msgData.bytes);
            ulong pupilID = res.discipleId;
            if (pupilID > 0)
            {
                GetPupilMissionList(pupilID);   
            }
        }


        void _OnWorldReceiveMasterDailyTaskRewardRes(MsgDATA msgData)
        {
            WorldReceiveMasterDailyTaskRewardRes res = new WorldReceiveMasterDailyTaskRewardRes();
            res.decode(msgData.bytes);
            if (res.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
                return;
            }
        }


        void _OnWorldDiscipleFinishSchoolRes(MsgDATA msgData)
        {
            WorldDiscipleFinishSchoolRes res = new WorldDiscipleFinishSchoolRes();
            res.decode(msgData.bytes);
            if (res.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
                return;
            }
        }

        void _OnWorldAutomaticFinishSchoolRes(MsgDATA msgData)
        {
            WorldAutomaticFinishSchoolRes res = new WorldAutomaticFinishSchoolRes();
            res.decode(msgData.bytes);
            if (res.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
                return;
            }
        }

        void _OnSceneNotifyNewTaskRet(MsgDATA msgData)
        {
            SceneNotifyNewTaskRet res = new SceneNotifyNewTaskRet();
            res.decode(msgData.bytes);
            var missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)res.taskInfo.taskID);
            if (missionItem == null)
            {
                return;
            }
            if(missionItem.TaskType == MissionTable.eTaskType.TASK_MASTER_DAILY)
            {
                for(int i = 0;i< MyDailyMission.Count;i++)
                {
                    if(MyDailyMission[i].taskID == res.taskInfo.taskID)
                    {
                        MyDailyMission.Remove(MyDailyMission[i]);
                    }
                }
                MyDailyMission.Add(res.taskInfo);
                myTAPData.dailyTasks = MyDailyMission.ToArray();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPLearningUpdate);
            }
            
        }

        void _OnSceneNotifyDeleteTaskRet(MsgDATA msgData)
        {
            SceneNotifyDeleteTaskRet res = new SceneNotifyDeleteTaskRet();
            res.decode(msgData.bytes);
            var missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)res.taskID);
            if (missionItem == null)
            {
                return;
            }
            if (missionItem.TaskType == MissionTable.eTaskType.TASK_MASTER_DAILY)
            {
                for (int i = 0; i < MyDailyMission.Count; i++)
                {
                    if (MyDailyMission[i].taskID == res.taskID)
                    {
                        MyDailyMission.Remove(MyDailyMission[i]);
                    }
                }

                myTAPData.dailyTasks = MyDailyMission.ToArray();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPLearningUpdate);
            }
        }

        void _OnSceneNotifyTaskVarRet(MsgDATA msgData)
        {
            SceneNotifyTaskVarRet res = new SceneNotifyTaskVarRet();
            res.decode(msgData.bytes);
            var missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)res.taskID);
            if (missionItem == null)
            {
                return;
            }
            if (missionItem.TaskType == MissionTable.eTaskType.TASK_MASTER_DAILY)
            {
                for (int i = 0; i < MyDailyMission.Count; i++)
                {
                    if (MyDailyMission[i].taskID == res.taskID)
                    {
                        //这里的任务一开始进度为0时候，key也没有值，所以特殊判断
                        if (MyDailyMission[i].akMissionPairs.Length == 0)
                        {
                            MissionPair[] tempMissionPair = new MissionPair [1];
                            tempMissionPair[0] = new MissionPair();
                            tempMissionPair[0].key = res.key;
                            tempMissionPair[0].value = res.value;
                            MyDailyMission[i].akMissionPairs = tempMissionPair;
                        }
                        else
                        {
                            bool haveThisKey = false;
                            for (int j = 0; j < MyDailyMission[i].akMissionPairs.Length; j++)
                            {
                                if (MyDailyMission[i].akMissionPairs[j].key == res.key)
                                {
                                    haveThisKey = true;
                                    MyDailyMission[i].akMissionPairs[j].value = res.value;
                                }
                            }
                            
                            //如果已经有一个进度key，再加一个任务状态key。这还是个数组。。。
                            if (!haveThisKey)
                            {
                                int count = MyDailyMission[i].akMissionPairs.Length;
                                MissionPair[] temp = new MissionPair[count+1];
                                for (int j = 0; j < MyDailyMission[i].akMissionPairs.Length; j++)
                                {
                                    temp[j] = MyDailyMission[i].akMissionPairs[j];
                                }
                                MissionPair pair = new MissionPair();
                                pair.key = res.key;
                                pair.value = res.value;
                                temp[count] = pair;
                                MyDailyMission[i].akMissionPairs = temp;
                            }
                        }
                    }
                }
                myTAPData.dailyTasks = MyDailyMission.ToArray();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTapTeacherSubmitRedPoint);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPLearningUpdate);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTapPupilReportRedPoint);
            }
            
        }

        void _OnSceneNotifyTaskStatus(MsgDATA msgData)
        {
            SceneNotifyTaskStatusRet res = new SceneNotifyTaskStatusRet();
            res.decode(msgData.bytes);
            var missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)res.taskID);
            if (missionItem == null)
            {
                return;
            }
            if (missionItem.TaskType == MissionTable.eTaskType.TASK_MASTER_DAILY)
            {
                for (int i = 0; i < MyDailyMission.Count; i++)
                {
                    if (MyDailyMission[i].taskID == res.taskID)
                    {
                        MyDailyMission[i].status = res.status;
                    }
                }
                myTAPData.dailyTasks = MyDailyMission.ToArray();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTapTeacherSubmitRedPoint);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPLearningUpdate);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTapPupilReportRedPoint);
            }
            
        }

        void _OnWorldSyncMasterDisciplePunishTime(MsgDATA msgData)
        {
            WorldSyncMasterDisciplePunishTime res = new WorldSyncMasterDisciplePunishTime();
            res.decode(msgData.bytes);
            teacherPublishTime = res.apprenticMasterPunishTime;
            pupilPublishTime = res.recruitDisciplePunishTime;
        }

        void _OnSyncOffline(MsgDATA msg)
        {
            WorldSyncOnOffline res = new WorldSyncOnOffline();
            res.decode(msg.bytes);
            if(classmateRelationDic.ContainsKey(res.id))
            {
                classmateRelationDic[res.id].status = res.isOnline;
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshClassmateDic); 
        }

        void _OnWorldNotifyFinSchEvent(MsgDATA msg)
        {
            WorldNotifyFinSchEvent res = new WorldNotifyFinSchEvent();
            res.decode(msg.bytes);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPGraduationSuccess);
        }
        #endregion
        
        
        //////////////////////////////////////////////////move from TapDataManager/////////////////////////////////////////////
        
        bool bLoadingScene = false;
        bool bFlagToOpenSearchTeacherFrame = false;
        
        private void OnSearchedTeacherListChanged(UIEvent uiEvent)
        {
            if (bFlagToOpenSearchTeacherFrame == true && bLoadingScene == false)
            {
                if (RelationDataManager.GetInstance().SearchedTeacherList.Count <= 0)
                {
                    bFlagToOpenSearchTeacherFrame = false;
                    return;
                }

                SystemNotifyManager.SystemNotifyOkCancel(7021,
                    () =>
                    {
                        OpenLinkFrame(string.Format("{0}", (int)TAPSystemTabType.TSTT_RELATION_INFO));
                        OpenSearchTeacherFrame();
                    },
                    null, FrameLayer.High, true);

                bFlagToOpenSearchTeacherFrame = false;
            }
        }

        public void OpenLinkFrame(string strParam)
        {
            int iTab = 0;
            if (int.TryParse(strParam, out iTab))
            {
                if (iTab >= 0 && iTab < (int)TAPSystemTabType.TSTT_COUNT)
                {
                    ClientSystemManager.GetInstance().OpenFrame<RelationFrameNew>();
                }
            }
        }

        void _OnLevelChanged(int iPreLv,int iCurLv)
        {
            if(iPreLv > 0 && iCurLv > iPreLv)
            {
                if (ComTAPOpenControl.IsOpen() && !hasTeacher && !PlayerBaseData.GetInstance().IsLevelFull)
                {
                    if (PlayerBaseData.GetInstance().Level % 5 == 0)
                    {
                        bFlagToOpenSearchTeacherFrame = true;
                        _TryPopTapSearchTeacherFrame();
                    }
                }
            }
        }

        public void AddSystemInvoke()
        {
            ClientSystemManager.GetInstance().OnSwitchSystemBegin.AddListener(OnSceneLoadBegin);
            ClientSystemManager.GetInstance().OnSwitchSystemFinished.AddListener(OnSceneLoadEnd);
        }

        public void RemoveSystemInvoke()
        {
            ClientSystemManager.GetInstance().OnSwitchSystemBegin.RemoveListener(OnSceneLoadBegin);
            ClientSystemManager.GetInstance().OnSwitchSystemFinished.RemoveListener(OnSceneLoadEnd);
        }

        public void OnSceneLoadBegin()
        {
            bLoadingScene = true;
        }

        public void OnSceneLoadEnd()
        {
            bLoadingScene = false;
            _TryPopTapSearchTeacherFrame();
        }

        void _TryPopTapSearchTeacherFrame()
        {
            var town = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            if (town == null)
            {
                return;
            }

            if(PlayerBaseData.GetInstance().IsFlyUpState)
            {
                return;
            }

            CitySceneTable tableData = TableManager.GetInstance().GetTableItem<CitySceneTable>(town.CurrentSceneID);
            if(tableData == null)
            {
                return;
            }

            if(tableData.SceneType == CitySceneTable.eSceneType.BATTLE || tableData.SceneType == CitySceneTable.eSceneType.BATTLEPEPARE)
            {
                return;
            }

            if(bLoadingScene)
            {
                return;
            }

            if(!bFlagToOpenSearchTeacherFrame)
            {
                return;
            }

            if((int)IsTeacher() > 1)
            {
                return;
            }

            if(PlayerBaseData.GetInstance().Level < 10)
            {
                return;
            }

            if (IsTeacher() == TAPType.Pupil)
            {
                SendChangeSearchedPupilList(RelationFindType.Master);
            }
        }
        
        public int teacherMinLevel
        {
            get
            {
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_TAP_SYSTEM_TEACHER_MIN_LEVEL);
                if (null != systemValue)
                {
                    return systemValue.Value;
                }
                else
                {
                    return 99;
                }
            }
        }

        public int apprentLevelMax
        {
            get
            {
                var sysytemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType2.SVT_TAP_SYSTEM_APPRENT_LEVEL_MAX);
                if (sysytemValue != null)
                {
                    return sysytemValue.Value;
                }
                else
                {
                    return 99;
                }
            }
        }

        public bool canGetpupil
        {
            get
            {
                return PlayerBaseData.GetInstance().Level >= teacherMinLevel;
            }
        }
        
        
        public bool isPupilFull
        {
            get
            {
                var datas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
                return null != datas && pupilMaxCount <= datas.Count;
            }
        }

        public bool hasTeacher
        {
            get
            {
                var datas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_MASTER);
                return null != datas && datas.Count > 0;
            }
        }

        public bool HasPupil(ulong uid)
        {
            var datas = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
            if(null != datas)
            {
                var find = datas.Find(x => { return x.uid == uid; });
                return null != find;
            }
            return false;
        }

        public int pupilMaxCount
        {
            get
            {
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_TAP_SYSTEM_PUPIL_MAX_COUNT);
                if (null != systemValue)
                {
                    return systemValue.Value;
                }
                return 0;
            }
        }
        
        
        public void OpenApplyPupilFrame()
        {
            //RelationDataManager.GetInstance().MakeDebugApplyPupilDatas();
            //ClientSystemManager.GetInstance().OpenFrame<TAPApplyPupilFrame>(FrameLayer.Middle);
            
            if (IsTeacher() == TAPType.TeacherSoon)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_enter_searchFrame"));
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<TAPSearchFrame>(FrameLayer.Middle, 1);
            }
        }

        public void OpenSearchPupilFrame()
        {
            //RelationDataManager.GetInstance().MakeDebugSearchedPupilListData();
            //ClientSystemManager.GetInstance().OpenFrame<TAPSearchPupilFrame>(FrameLayer.Middle, null);
            if (IsTeacher() == TAPType.TeacherSoon)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_enter_searchFrame"));
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<TAPSearchFrame>(FrameLayer.Middle, 0);
            }
        }

        public void OpenSearchTeacherFrame()
        {
            //RelationDataManager.GetInstance().MakeDebugSearchTeacherListData();
            //ClientSystemManager.GetInstance().OpenFrame<TAPSearchTeacherFrame>(FrameLayer.Middle);
            if (IsTeacher() == TAPType.TeacherSoon)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_enter_searchFrame"));
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<TAPSearchFrame>(FrameLayer.Middle, 0);
            }
        }

        public void SendQueryMasterSetting()
        {
            QueryMasterSettingReq req = new QueryMasterSettingReq();
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        //[MessageHandle(QueryMasterSettingRes.MsgID)]
        protected void _OnRecvQueryMasterSettingRes(MsgDATA msg)
        {
            QueryMasterSettingRes ret = new QueryMasterSettingRes();
            ret.decode(msg.bytes);

            PlayerBaseData.GetInstance().getPupil = ret.isRecv != 0;
            PlayerBaseData.GetInstance().Announcement = ret.masternote;
        }

        public bool CheckApplyPupil(bool bNeedPopMsg = false)
        {
            if (canGetpupil)
            {
                if(bNeedPopMsg)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("tap_self_get_pupil_need_lv", teacherMinLevel));
                }
                return false;
            }

            if (isPupilFull)
            {
                if (bNeedPopMsg)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("tap_get_pupil_full", pupilMaxCount));
                }
                return false;
            }

            return true;
        }

        List<ulong> m_akQueriedIds = new List<ulong>();

        public void AddQueryInfo(ulong uid)
        {
            if (!m_akQueriedIds.Contains(uid))
            {
                m_akQueriedIds.Add(uid);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnQueryTeacherChanged, uid);
        }

        public void RemoveQueryInfo(ulong uid)
        {
            if (m_akQueriedIds.Contains(uid))
            {
                m_akQueriedIds.Remove(uid);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnQueryTeacherChanged, uid);
        }

        public void ClearQueryInfo()
        {
            m_akQueriedIds.Clear();
        }

        public bool CanQuery(ulong uid)
        {
            return !m_akQueriedIds.Contains(uid);
        }

        List<ulong> m_akIWantApplyedPupils = new List<ulong>();
        public void AddApplyedPupil(ulong uid)
        {
            if (!m_akIWantApplyedPupils.Contains(uid))
            {
                m_akIWantApplyedPupils.Add(uid);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnWanApplyedPupilChanged, uid);
        }

        public void RemoveApplyedPupil(ulong uid)
        {
            if (m_akIWantApplyedPupils.Contains(uid))
            {
                m_akIWantApplyedPupils.Remove(uid);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnWanApplyedPupilChanged, uid);
        }

        public bool CanApplyedPupil(ulong uid)
        {
            return !m_akIWantApplyedPupils.Contains(uid);
        }

        public void ClearApplyedPupils()
        {
            m_akIWantApplyedPupils.Clear();
        }

        //[MessageHandle(SetRecvDiscipleStateRet.MsgID)]
        void _OnRecvSetRecvDiscipleStateRet(MsgDATA msg)
        {
            SetRecvDiscipleStateRet msgRet = new SetRecvDiscipleStateRet();
            msgRet.decode(msg.bytes);

            if(msgRet.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)msgRet.code);
            }
            else
            {
                PlayerBaseData.GetInstance().getPupil = msgRet.state != 0;
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("tap_get_pupil_modify_ok"));
            }
        }

        //[MessageHandle(SetMasterNoteRet.MsgID)]
        void _OnRecvSetMasterNoteRet(MsgDATA msg)
        {
            SetMasterNoteRet msgRet = new SetMasterNoteRet();
            msgRet.decode(msg.bytes);

            if (msgRet.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)msgRet.code);
            }
            else
            {
                PlayerBaseData.GetInstance().Announcement = msgRet.note;
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("tap_announce_modify_ok"));
            }
        }
        
        
    }

}