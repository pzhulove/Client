using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine.Assertions;
using Protocol;
using Scripts.UI;

namespace GameClient
{
    public class VideoInfo
    {
        public string sessionId = string.Empty;
        public GameObject btnGo = null;
        public bool isInBattle = false;
    };
    class PkVideoFrame : ClientFrame
    {
        [UIControl("Content/Groups/MyPkGroup/Records")]
        ComUIListScript m_comMyPkList;

        [UIControl("Content/Groups/PeakPkGroup/Records")]
        ComUIListScript m_comPeakPkList;

        [UIControl("Content/Groups/3V3PkGroup/Records")]
        ComUIListScript m_com3v3PkList;

        SceneReplayListRes m_myPkRecords = null;
        SceneReplayListRes m_peakPkRecords = null;
        List<ReplayInfo> m_myPkRecordsLst = new List<ReplayInfo>();
        List<ReplayInfo> m_3v3PkRecordLst = new List<ReplayInfo>();
        const string m_strPkResultWin = "UI/Image/NewPacked/Juedou_Luxiang.png:Juedou_Luxiang_Shengli";
        const string m_strPkResultLose = "UI/Image/NewPacked/Juedou_Luxiang.png:Juedou_Luxiang_Shibai";
		const string m_strPkResultDraw = "UI/Image/NewPacked/Juedou_Luxiang.png:Juedou_Luxiang_Pingju";

        Color winColor = Color.red;
        Color loseColor = Color.gray;
        Color drawColor = Color.white;


		private Text mPkNum = null;
		private Text mPkWinRate = null;

        private CommonTabToggleGroup CommonVerticalTab = null;

        static bool isShowMyVideo = true;
        static bool isShow3V3Video = false;
        protected override void _bindExUI()
		{
			mPkNum = mBind.GetCom<Text>("pkNum");
			mPkWinRate = mBind.GetCom<Text>("pkWinRate");

            CommonVerticalTab = mBind.GetCom<CommonTabToggleGroup>("CommonVerticalTab");

        }

		protected override void _unbindExUI()
		{
			mPkNum = null;
			mPkWinRate = null;

            CommonVerticalTab = null;

        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/PkVideo/PkVideo";
        }

        protected override void _OnOpenFrame()
        {
            _RegisterUIEvent();
            _InitUI();
        }

        protected override void _OnCloseFrame()
        {
            
            _UnRegisterUIEvent();
            _ClearUI();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PKMyRecordUpdated, _OnMyPkUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PKPeakRecordUpdated, _OnPeakPkUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUploadFileSucc, _onUpLoadSucc);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUploadFileClose, _onUpLoadClose);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PKMyRecordUpdated, _OnMyPkUpdated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PKPeakRecordUpdated, _OnPeakPkUpdated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUploadFileSucc, _onUpLoadSucc);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUploadFileClose, _onUpLoadClose);
        }
        void _onUpLoadSucc(UIEvent a_event)
        {
            var info = a_event.Param1 as VideoInfo;
            if (info != null && info.btnGo != null)
            {
                info.btnGo.CustomActive(false);
            }
        }
        void _onUpLoadClose(UIEvent a_event)
        {

        }

        enum TabType
        {
            PK_My = 0,
            PK_Peak,
            PK_3V3,
        }      

        void CommonTabToggleOnClick(CommonTabData tabData)
        {
            if(tabData == null)
            {
                return;
            }

            TabType tabType = (TabType)tabData.id;
            if(tabType == TabType.PK_My)
            {
                SeasonDataManager.GetInstance().RequsetMyPkRecord();
                isShowMyVideo = true;
                isShow3V3Video = false;
            }
            else if(tabType == TabType.PK_Peak)
            {
                SeasonDataManager.GetInstance().RequsetPeakPkRecord();
                isShowMyVideo = false;
                isShow3V3Video = false;
            }
            else if(tabType == TabType.PK_3V3)
            {
                SeasonDataManager.GetInstance().RequsetMyPkRecord();
                isShow3V3Video = true;
                isShowMyVideo = false;
            }
        }

        void _InitUI()
        {
            if(CommonVerticalTab != null)
            {           
                CommonVerticalTab.InitComTab(CommonTabToggleOnClick, 0);
            }

            _InitMyPkUI();
            _InitPeakPkUI();
            _Init3V3PkUI();           

			SetPKInfo();
        }

		void SetPKInfo()
		{
			int pkTotalNum = 0;
			int pkWinNum = 0;
			for(int i=0; i<=(int)PkType.Pk_Friends; ++i)
			{
				var data = PlayerBaseData.GetInstance().GetPkStatisticDataByPkType((PkType)i);
				if (data != null)
				{
					pkTotalNum += (int)data.totalNum;
					pkWinNum += (int)data.totalWinNum;
				}
			}

			mPkNum.text = pkTotalNum.ToString();
			if (pkTotalNum <= 0)
				mPkWinRate.text = "0%";
			else
				mPkWinRate.text = string.Format("{0:F1}%", pkWinNum/(float)pkTotalNum * 100);
		}

        void _ClearUI()
        {
            _ClearMyPkUI();
            _ClearPeakPkUI();
        }

        void _InitMyPkUI()
        {
            m_comMyPkList.Initialize();

            m_comMyPkList.onItemVisiable = var =>
            {
                if (m_myPkRecords != null)
                {
                    if (var.m_index >= 0 && var.m_index < m_myPkRecordsLst.Count)
                    {
                        ComMyPkRecord comUI = var.GetComponent<ComMyPkRecord>();
                        if (comUI != null)
                        {
							ReplayInfo recordData = m_myPkRecordsLst[(m_myPkRecordsLst.Count - var.m_index) - 1];
                            
                            ReplayFighterInfo myInfo = null;
                            ReplayFighterInfo otherInfo = null;
							bool myPosIsRight = false;
                            if (recordData.fighters[0].roleId == PlayerBaseData.GetInstance().RoleID)
                            {
                                myInfo = recordData.fighters[0];
                                otherInfo = recordData.fighters[1];
                            }
                            else
                            {
                                myInfo = recordData.fighters[1];
                                otherInfo = recordData.fighters[0];
								myPosIsRight = true;
                            }
                            // left
                            _SetupPlayerInfo(comUI.imgLeftSeasonIcon, comUI.labLeftName, comUI.labLeftJob, comUI.labLeftSeasonName, myInfo);

                            // right
                            _SetupPlayerInfo(comUI.imgRightSeasonIcon, comUI.labRightName, comUI.labRightJob, comUI.labRightSeasonName, otherInfo);

							PKVideoResult ePKResult = (PKVideoResult)recordData.result;

							if (myPosIsRight)
							{
								if (ePKResult == PKVideoResult.WIN)
									ePKResult = PKVideoResult.LOSE;
								else if (ePKResult == PKVideoResult.LOSE)
									ePKResult = PKVideoResult.WIN;
							}

                            switch (ePKResult)
                            {
								case PKVideoResult.WIN:
                                    {
                                        // comUI.imgPkResult.sprite = AssetLoader.GetInstance().LoadRes(m_strPkResultWin, typeof(Sprite)).obj as Sprite;
                                        ETCImageLoader.LoadSprite(ref comUI.imgPkResult, m_strPkResultWin);
                                        comUI.imgPkResult.SetNativeSize();
                                        comUI.resultBg.color = winColor;
                                        break;
                                    }
								case PKVideoResult.LOSE:
                                    {
                                        // comUI.imgPkResult.sprite = AssetLoader.GetInstance().LoadRes(m_strPkResultLose, typeof(Sprite)).obj as Sprite;
                                        ETCImageLoader.LoadSprite(ref comUI.imgPkResult, m_strPkResultLose);
                                        comUI.imgPkResult.SetNativeSize();
                                        comUI.resultBg.color = loseColor;
                                        break;
                                    }
								case PKVideoResult.DRAW:
                                    {
                                        // comUI.imgPkResult.sprite = AssetLoader.GetInstance().LoadRes(m_strPkResultDraw, typeof(Sprite)).obj as Sprite;
                                        ETCImageLoader.LoadSprite(ref comUI.imgPkResult, m_strPkResultDraw);
                                        comUI.imgPkResult.SetNativeSize();
                                        comUI.resultBg.color = drawColor;
                                        break;
                                    }
                                default:
                                    {
                                        ETCImageLoader.LoadSprite(ref comUI.imgPkResult, "UI/Image/Packed/p_UI_SeasonNumber.png:UI_Season_Duanwen_Jieguoyichang");
                                        comUI.imgPkResult.SetNativeSize();
                                   //     comUI.imgPkResult.sprite = null;
                                        break;
                                    }
                            }

                            _SetupCreateTime(comUI.labCreateTime, recordData.recordTime);

                            comUI.btnPlay.onClick.RemoveAllListeners();
                            comUI.btnUpLoad.onClick.RemoveAllListeners();
                            comUI.btnPlay.onClick.AddListener(() =>
                            {
								PlayReplay(recordData.raceId, true);
                            });
                            comUI.btnUpLoad.onClick.AddListener(() =>
                            {
                                onUpLoadRecord(recordData.raceId, comUI.btnUpLoad.gameObject, true);
                            });
                        }
                    }
                }
            };
        }

        void onUpLoadRecord(UInt64 a_raceID, GameObject btn, bool normal = true, uint version = 0)
        {
            string sessionID = a_raceID.ToString();

            if (ReplayServer.GetInstance().HasReplay(sessionID))
            {
                var ret = ReplayServer.GetInstance().CompressRecord(sessionID);
                if (ret == ReplayErrorCode.SUCCEED)
                {
                    var info = new VideoInfo
                    {
                        sessionId = sessionID,
                        btnGo = btn
                    };
                    ClientSystemManager.GetInstance().OpenFrame<PKReporterFrame>(FrameLayer.Middle, info);
                }
                else
                {
                    ShowNotify(ret);
                }
            }
            else
            {
                if (normal)
                    ShowNotify(ReplayErrorCode.FILE_NOT_FOUND);
                else
                {
                    //uint verion = 0;
                    if (!ReplayServer.GetInstance().CheckVersion(version))
                        ShowNotify(ReplayErrorCode.VERSION_NOT_MATCH);
                    else
                    {
                        StartDownloadReplayFileAndUpLoad(a_raceID, btn);
                    }
                }
            }
        }
        void StartDownloadReplayFileAndUpLoad(UInt64 a_raceID, GameObject btnGo)
        {
            string sessionID = a_raceID.ToString();

            if (null != mCurrentLoadServerCo) GameFrameWork.instance.StopCoroutine(mCurrentLoadServerCo);
            mCurrentLoadServerCo = GameFrameWork.instance.StartCoroutine(SendHttpReqReplayFile(sessionID, (sid) =>
            {

                ClientSystemManager.GetInstance().delayCaller.DelayCall(2000, () =>
                {
                    var ret = ReplayServer.GetInstance().CompressRecord(sid);
                    if (ret == ReplayErrorCode.SUCCEED)
                    {
                        var info = new VideoInfo
                        {
                            sessionId = sessionID,
                            btnGo = btnGo
                        };
                        ClientSystemManager.GetInstance().OpenFrame<PKReporterFrame>(FrameLayer.Middle, info);
                    }
                    else
                    {
                        ShowNotify(ret);
                    }
                });
            }));
        }



        void _UpdateMyPkUI(SceneReplayListRes a_data)
        {
            m_3v3PkRecordLst.Clear();
            m_myPkRecordsLst.Clear();
            m_myPkRecords = a_data;
            for (int i = 0; i < m_myPkRecords.replays.Length; i++)
            {
                if ((PkType)m_myPkRecords.replays[i].type == PkType.Pk_3V3_ROOM ||
                    (PkType)m_myPkRecords.replays[i].type == PkType.Pk_3V3_MATCH)
                {
                    m_3v3PkRecordLst.Add(m_myPkRecords.replays[i]);
                }
                else
                {
                    m_myPkRecordsLst.Add(m_myPkRecords.replays[i]);
                }
            }
            if (m_myPkRecords != null && m_myPkRecords.replays != null && m_myPkRecordsLst.Count > 0)
            {
                m_comMyPkList.SetElementAmount(m_myPkRecordsLst.Count);
            }
            else
            {
                m_comMyPkList.SetElementAmount(0);
            }

            if (m_myPkRecords != null && m_myPkRecords.replays != null && m_3v3PkRecordLst.Count > 0)
            {
                m_com3v3PkList.SetElementAmount(m_3v3PkRecordLst.Count);
            }
            else
            {
                m_com3v3PkList.SetElementAmount(0);
            }
        }

        void _ClearMyPkUI()
        {
            m_myPkRecords = null;
            m_myPkRecordsLst.Clear();
            m_3v3PkRecordLst.Clear();
        }
        void _Init3V3PkUI()
        {
            m_com3v3PkList.Initialize();
            m_com3v3PkList.onItemVisiable = var =>
            {
                if (m_3v3PkRecordLst != null)
                {
                    if (var.m_index >= 0 && var.m_index < m_3v3PkRecordLst.Count)
                    {
                        ComMyPkRecord comUI = var.GetComponent<ComMyPkRecord>();
                        if (comUI != null)
                        {
                            ReplayInfo recordData = m_3v3PkRecordLst[(m_3v3PkRecordLst.Count - var.m_index) - 1];
                            bool myPosIsRight = false;
                            if (comUI.playerNames != null)
                            {
                                bool bFirstLeft = false;
                                bool bFirstRight = false;
                                int leftFighterCount = 0;
                                int rightFighgerCount = 0;
                                bool isLeft = false;
                                for (int i = 0; i < recordData.fighters.Length; i++)
                                {
                                    isLeft = false;
                                    var fighter = recordData.fighters[i];
                                    if (fighter.pos >= 0 && fighter.pos <= 2)
                                    {
                                        leftFighterCount++;
                                        isLeft = true;
                                        if (!bFirstLeft)
                                        {
                                            bFirstLeft = true;
                                            ETCImageLoader.LoadSprite(ref comUI.imgLeftSeasonIcon, SeasonDataManager.GetInstance().GetMainSeasonLevelIcon((int)fighter.seasonLevel));
                                        }
                                        if (fighter.roleId == PlayerBaseData.GetInstance().RoleID)
                                        {
                                            myPosIsRight = false;
                                        }
                                    }
                                    else
                                    {
                                        rightFighgerCount++;
                                        if (fighter.roleId == PlayerBaseData.GetInstance().RoleID)
                                        {
                                            myPosIsRight = true;
                                        }
                                        if (!bFirstRight)
                                        {
                                            bFirstRight = true;
                                            ETCImageLoader.LoadSprite(ref comUI.imgRightSeasonIcon, SeasonDataManager.GetInstance().GetMainSeasonLevelIcon((int)fighter.seasonLevel));
                                        }
                                    }
                                    if (fighter.pos > comUI.playerNames.Length)
                                    {
                                        Logger.LogErrorFormat("fighter id {0} name {1} Index [2] pos {3} out of playerNames Length {4}", fighter.roleId, fighter.name, i, fighter.pos, comUI.playerNames.Length);
                                        comUI.playerNames[fighter.pos].text = "";
                                    }
                                    else
                                    {
                                        if (isLeft)
                                        {
                                            comUI.playerNames[leftFighterCount - 1].text = fighter.name;
                                        }
                                        else
                                        {
                                            comUI.playerNames[rightFighgerCount - 1 + 3].text = fighter.name;
                                        }
                                    }
                                }
                                if (comUI.playerNames.Length != 6)
                                {
                                    Logger.LogErrorFormat("3v3 playerNameLength number is not right {0}", comUI.playerNames.Length);
                                }
                                else
                                {
                                    for (int i = leftFighterCount; i < 3; i++)
                                    {
                                        comUI.playerNames[i].text = "";
                                    }
                                    for (int i = rightFighgerCount; i < 3; i++)
                                    {
                                        comUI.playerNames[i + 3].text = "";
                                    }
                                }
                            }

                            PKVideoResult ePKResult = (PKVideoResult)recordData.result;

                            if (myPosIsRight)
                            {
                                if (ePKResult == PKVideoResult.WIN)
                                    ePKResult = PKVideoResult.LOSE;
                                else if (ePKResult == PKVideoResult.LOSE)
                                    ePKResult = PKVideoResult.WIN;
                            }

                            switch (ePKResult)
                            {
                                case PKVideoResult.WIN:
                                    {
                                        // comUI.imgPkResult.sprite = AssetLoader.GetInstance().LoadRes(m_strPkResultWin, typeof(Sprite)).obj as Sprite;
                                        ETCImageLoader.LoadSprite(ref comUI.imgPkResult, m_strPkResultWin);
                                        comUI.imgPkResult.SetNativeSize();
                                        break;
                                    }
                                case PKVideoResult.LOSE:
                                    {
                                        // comUI.imgPkResult.sprite = AssetLoader.GetInstance().LoadRes(m_strPkResultLose, typeof(Sprite)).obj as Sprite;
                                        ETCImageLoader.LoadSprite(ref comUI.imgPkResult, m_strPkResultLose);
                                        comUI.imgPkResult.SetNativeSize();
                                        break;
                                    }
                                case PKVideoResult.DRAW:
                                    {
                                        // comUI.imgPkResult.sprite = AssetLoader.GetInstance().LoadRes(m_strPkResultDraw, typeof(Sprite)).obj as Sprite;
                                        ETCImageLoader.LoadSprite(ref comUI.imgPkResult, m_strPkResultDraw);
                                        comUI.imgPkResult.SetNativeSize();
                                        break;
                                    }
                                default:
                                    {
                                        ETCImageLoader.LoadSprite(ref comUI.imgPkResult, "UI/Image/Packed/p_UI_SeasonNumber.png:UI_Season_Duanwen_Jieguoyichang");
                                        comUI.imgPkResult.SetNativeSize();
                                        //     comUI.imgPkResult.sprite = null;
                                        break;
                                    }
                            }

                            _SetupCreateTime(comUI.labCreateTime, recordData.recordTime);

                            comUI.btnPlay.onClick.RemoveAllListeners();
                            comUI.btnUpLoad.onClick.RemoveAllListeners();
                            comUI.btnPlay.onClick.AddListener(() =>
                            {
                                PlayReplay(recordData.raceId, true);
                            });
                            comUI.btnUpLoad.onClick.AddListener(() =>
                            {
                                onUpLoadRecord(recordData.raceId, comUI.btnUpLoad.gameObject, true);
                            });
                        }
                    }
                };
            };
        }
        void _InitPeakPkUI()
        {
            m_comPeakPkList.Initialize();

            m_comPeakPkList.onItemVisiable = var =>
            {
                if (m_peakPkRecords != null)
                {
                    ReplayInfo[] arrRecords = m_peakPkRecords.replays;
                    Array.Sort(arrRecords, (x, y) =>
                    {
                        if (x.viewNum != y.viewNum)
                        {
                            return x.viewNum.CompareTo(y.viewNum);
                        }
                        else if (x.recordTime != y.recordTime)
                        {
                            return x.recordTime.CompareTo(y.recordTime);
                        }
                        return 0;
                    });
                    if (var.m_index >= 0 && var.m_index < arrRecords.Length)
                    {
                        ComPeakPkRecord comUI = var.GetComponent<ComPeakPkRecord>();
                        if (comUI != null)
                        {
							ReplayInfo recordData = arrRecords[(arrRecords.Length - var.m_index) - 1];
                            // left
                            _SetupPlayerInfo(comUI.imgLeftSeasonIcon, comUI.labLeftName, comUI.labLeftJob, comUI.labLeftSeasonName, recordData.fighters[0]);

                            // right
                            _SetupPlayerInfo(comUI.imgRightSeasonIcon, comUI.labRightName, comUI.labRightJob, comUI.labRightSeasonName, recordData.fighters[1]);

							PKVideoResult ePKResult = (PKVideoResult)recordData.result;
                            switch (ePKResult)
                            {
                                case PKVideoResult.WIN:
                                    {
                                        // comUI.imgPkResult.sprite = AssetLoader.GetInstance().LoadRes(m_strPkResultWin, typeof(Sprite)).obj as Sprite;
                                        ETCImageLoader.LoadSprite(ref comUI.imgPkResult, m_strPkResultWin);
                                        comUI.imgPkResult.SetNativeSize();
                                        comUI.resultBg.color = winColor;
                                        break;
                                    }
                                case PKVideoResult.LOSE:
                                    {
                                        // comUI.imgPkResult.sprite = AssetLoader.GetInstance().LoadRes(m_strPkResultLose, typeof(Sprite)).obj as Sprite;
                                        ETCImageLoader.LoadSprite(ref comUI.imgPkResult, m_strPkResultLose);
                                        comUI.imgPkResult.SetNativeSize();
                                        comUI.resultBg.color = loseColor;
                                        break;
                                    }
                                case PKVideoResult.DRAW:
                                    {
                                        // comUI.imgPkResult.sprite = AssetLoader.GetInstance().LoadRes(m_strPkResultDraw, typeof(Sprite)).obj as Sprite;
                                        ETCImageLoader.LoadSprite(ref comUI.imgPkResult, m_strPkResultDraw);
                                        comUI.imgPkResult.SetNativeSize();
                                        comUI.resultBg.color = drawColor;
                                        break;
                                    }
                                default:
                                    {
                                        ETCImageLoader.LoadSprite(ref comUI.imgPkResult, "UI/Image/Packed/p_UI_SeasonNumber.png:UI_Season_Duanwen_Jieguoyichang");
                                        comUI.imgPkResult.SetNativeSize();
                                        //     comUI.imgPkResult.sprite = null;
                                        break;
                                    }
                            }

                            _SetupCreateTime(comUI.labCreateTime, recordData.recordTime);

                            comUI.labPlayTime.text = TR.Value("pk_record_play_time", recordData.viewNum);
                            comUI.labScore.text = TR.Value("pk_record_score", recordData.score);

                            comUI.btnPlay.onClick.RemoveAllListeners();
                            comUI.btnUpload.onClick.RemoveAllListeners();
                            comUI.btnPlay.onClick.AddListener(() =>
                            {
							    PlayReplay(recordData.raceId, false, recordData.version);
                            });
                            comUI.btnUpload.onClick.AddListener(() =>
                            {
                                onUpLoadRecord(recordData.raceId, comUI.btnUpload.gameObject, false, recordData.version);
                            });
                        }
                    }
                }
            };
        }

        void _UpdatePeakPkUI(SceneReplayListRes a_data)
        {
            m_peakPkRecords = a_data;
            if (m_peakPkRecords != null && m_peakPkRecords.replays != null)
            {
                m_comPeakPkList.SetElementAmount(m_peakPkRecords.replays.Length);
            }
            else
            {
                m_comPeakPkList.SetElementAmount(0);
            }
        }

        void _ClearPeakPkUI()
        {
            m_peakPkRecords = null;
        }

        void _SetupCreateTime(Text a_lab, uint a_nRecordTime)
        {
            int nDelta = 0;
            uint nServerTime = TimeManager.GetInstance().GetServerTime();
            if (nServerTime > a_nRecordTime)
            {
                nDelta = (int)(nServerTime - a_nRecordTime);
            }
            
            if (nDelta < 3600)
            {
                int nValue = nDelta / 60;
                a_lab.text = TR.Value("pk_record_time_minute", nValue);
            }
            else if (nDelta < 3600 * 24)
            {
                int nValue = nDelta / 3600;
                a_lab.text = TR.Value("pk_record_time_hour", nValue);
            }
            else
            {
                int nValue = nDelta / (3600 * 24);
                if (nValue > 30)
                {
                    nValue = 30;
                }
                a_lab.text = TR.Value("pk_record_time_day", nValue);
            }
        }

        void _SetupPlayerInfo(Image a_imgSeasonIcon, Text a_labName, Text a_labJob, Text a_labSeasonName, ReplayFighterInfo a_data)
        {
            //a_imgSeasonIcon.sprite = AssetLoader.GetInstance().LoadRes(
            //    SeasonDataManager.GetInstance().GetMainSeasonLevelIcon((int)a_data.seasonLevel),
            //    typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref a_imgSeasonIcon, SeasonDataManager.GetInstance().GetMainSeasonLevelIcon((int)a_data.seasonLevel));

            a_labName.text = a_data.name;
            JobTable jobTable = TableManager.GetInstance().GetTableItem<JobTable>((int)a_data.occu);
            if (jobTable != null)
            {
                a_labJob.text = jobTable.Name;
            }
            else
            {
                a_labJob.text = string.Empty;
            }
            a_labSeasonName.text = SeasonDataManager.GetInstance().GetRankName((int)a_data.seasonLevel);
        }


        void _OnMyPkUpdated(UIEvent a_event)
        {
            _UpdateMyPkUI(a_event.Param1 as SceneReplayListRes);
        }

        void _OnPeakPkUpdated(UIEvent a_event)
        {
            _UpdatePeakPkUI(a_event.Param1 as SceneReplayListRes);
        }

        [UIEventHandle("BG/FullScreenFrameTitle/Title/Close")]
        void _OnCloseClicked()
        {
            isShowMyVideo = true;
            isShow3V3Video = false;
            frameMgr.CloseFrame(this);
        }


		void PlayReplay(UInt64 a_raceID, bool normal=true, uint version = 0)
		{
            string sessionID = a_raceID.ToString();

            if (ReplayServer.GetInstance().HasReplay(sessionID))
			{
				var ret = ReplayServer.GetInstance().StartReplay(sessionID);
                if (ret == ReplayErrorCode.SUCCEED)
                {
                    SeasonDataManager.GetInstance().NotifyVideoPlayed(a_raceID);
                }
                else
                {
                    ReplayServer.GetInstance().Clear();
                }
                ShowNotify(ret);
			}
			else
			{
				if (normal)
					ShowNotify(ReplayErrorCode.FILE_NOT_FOUND);
				else
				{
					//uint verion = 0;
					if (!ReplayServer.GetInstance().CheckVersion(version))
						ShowNotify(ReplayErrorCode.VERSION_NOT_MATCH);
					else
					{
						StartDownloadReplayFile(a_raceID);
						Logger.LogProcessFormat("开始下载录像并且播放");	
					}
				}
			}
		}

		void ShowNotify(ReplayErrorCode code)
		{
            MoneyRewardsDataManager.ShowErrorNotify(code);
        }


		#region DOWNLOAD
		private UnityEngine.Coroutine mCurrentLoadServerCo = null;
		void StartDownloadReplayFile(UInt64 a_raceID)
		{
            string sessionID = a_raceID.ToString();

            if (null != mCurrentLoadServerCo) GameFrameWork.instance.StopCoroutine(mCurrentLoadServerCo);
			mCurrentLoadServerCo = GameFrameWork.instance.StartCoroutine(SendHttpReqReplayFile(sessionID, (sid)=>{

                ClientSystemManager.GetInstance().delayCaller.DelayCall(2000, () =>
                {
                    var ret = ReplayServer.GetInstance().StartReplay(sid);
                    if (ret == ReplayErrorCode.SUCCEED)
                    {
                        SeasonDataManager.GetInstance().NotifyVideoPlayed(a_raceID);
                    }
                    else
                    {
                        ReplayServer.GetInstance().Clear();
                    }
                    ShowNotify(ret);
                });

				
			}));
		}

		IEnumerator SendHttpReqReplayFile(string sessionID, System.Action<string> cb = null)
		{
			ReplayWaitHttpRequest req = new ReplayWaitHttpRequest(sessionID);
			yield return req;

			if (req.GetResult() == BaseWaitHttpRequest.eState.Success)
			{
				var contents = req.GetResultBytes();
				if (contents == null || contents.Length<=0)
				{
					ShowNotify(ReplayErrorCode.DOWNLOAD_FAILED);
					yield break;
				}
                var decompressedContents = contents;//CompressHelper.Uncompress(contents, contents.Length);

				RecordData.SaveReplayFile(sessionID, decompressedContents, decompressedContents.Length);

				decompressedContents = null;
				contents = null;

				if (cb != null)
				{
					cb(sessionID);
				}
			}
			else{
				ShowNotify(ReplayErrorCode.DOWNLOAD_FAILED);
			}
		}
		#endregion
    }

	class ReplayWaitHttpRequest : BaseWaitHttpRequest
	{
		private void _setUrl(string sessionID)
		{
            string url = string.Format("http://{0}/replay?serverid={2}&raceid={1}", ClientApplication.replayServer, sessionID, ClientApplication.adminServer.id);
			this.url = url;
		}

		public ReplayWaitHttpRequest(string sessionID)
		{
			_setUrl(sessionID);
			SetRequestWaitResult();
		}
	}
}
