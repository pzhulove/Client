using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class ComChijiBattleProgressStage : MonoBehaviour
    {
        public ChiJiTimeTable.eBattleStage BattleStage = ChiJiTimeTable.eBattleStage.BS_NONE;
        public Slider slider = null;
        public Text ShowTime = null;

        private bool bStart = false;
        private uint StageStartTime = 0; // 阶段开始时间
        private float fTimeIntrval = 0.0f;
        private int LastTime = 0; // 持续时间
        private ChiJiTimeTable.eBattleStage NextStage = ChiJiTimeTable.eBattleStage.BS_NONE;

        private void Start()
        {
            bStart = false;

            _BindUIEvent();

            ChiJiTimeTable table = TableManager.GetInstance().GetTableItem<ChiJiTimeTable>((int)BattleStage);
            if (table != null)
            {
                LastTime = table.ProgressTime;
                NextStage = (ChiJiTimeTable.eBattleStage)table.NextStage;
            }

            fTimeIntrval = 0.0f;

            if (ChijiDataManager.GetInstance().CurBattleStage == BattleStage)
            {
                if (ChijiDataManager.GetInstance().StageStartTimeList != null && (int)BattleStage < ChijiDataManager.GetInstance().StageStartTimeList.Length)
                {
                    StageStartTime = ChijiDataManager.GetInstance().StageStartTimeList[(int)BattleStage];
                }

                //Logger.LogErrorFormat("吃鸡进度条初始化----ChijiDataManager.GetInstance().CurBattleStage = BattleStage, BattleStage = {0}， CurBattleStage = {1}", BattleStage, ChijiDataManager.GetInstance().CurBattleStage);
            }
            else if (ChijiDataManager.GetInstance().CurBattleStage > BattleStage)
            {
                if(NextStage > ChiJiTimeTable.eBattleStage.BS_NONE && ChijiDataManager.GetInstance().CurBattleStage < NextStage)
                {
                    if (ChijiDataManager.GetInstance().StageStartTimeList != null && (int)BattleStage < ChijiDataManager.GetInstance().StageStartTimeList.Length)
                    {
                        StageStartTime = ChijiDataManager.GetInstance().StageStartTimeList[(int)BattleStage];
                    }
                }
            }

            _SetSliderValue();
        }

        private void OnDestroy()
        {
            _UnBindUIEvent();

            BattleStage = ChiJiTimeTable.eBattleStage.BS_NONE;
            bStart = false;
            StageStartTime = 0;
            fTimeIntrval = 0.0f;
            LastTime = 0;
            NextStage = ChiJiTimeTable.eBattleStage.BS_NONE;
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChijiBattleStageChanged, _OnStageChanged);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChijiBattleStageChanged, _OnStageChanged);
        }

        private void _OnStageChanged(UIEvent iEvent)
        {
            if (BattleStage == ChijiDataManager.GetInstance().CurBattleStage)
            {
                StageStartTime = TimeManager.GetInstance().GetServerTime();
            }

            _SetSliderValue();
        }

        private void _SetSliderValue()
        {
            if (ChijiDataManager.GetInstance().CurBattleStage < BattleStage)
            {
                bStart = false;

                if (slider != null)
                {
                    slider.value = 0.0f;
                }
            }
            else if(ChijiDataManager.GetInstance().CurBattleStage == BattleStage)
            {
                if (slider != null)
                {
                    uint PassedTime = TimeManager.GetInstance().GetServerTime() - StageStartTime;
                    slider.value = PassedTime / (float)LastTime;
                }

                bStart = true;
            }
            else
            {
                if (NextStage > ChiJiTimeTable.eBattleStage.BS_NONE && ChijiDataManager.GetInstance().CurBattleStage < NextStage)
                {
                    if (slider != null)
                    {
                        uint PassedTime = TimeManager.GetInstance().GetServerTime() - StageStartTime;
                        slider.value = PassedTime / (float)LastTime;
                    }

                    bStart = true;
                }
                else
                {
                    bStart = false;

                    if (slider != null)
                    {
                        slider.value = 1.0f;
                    }
                }
            }
        }

        private void Update()
        {
            if (!bStart || LastTime == 0)
            {
                return;
            }

            fTimeIntrval += Time.deltaTime;

            if (fTimeIntrval < 0.35f)
            {
                return;
            }

            fTimeIntrval = 0.0f;

            if (ChijiDataManager.GetInstance().CurBattleStage < BattleStage)
            {
                return;
            }

            _SetSliderValue();

            if(ChijiDataManager.GetInstance().CurBattleStage == BattleStage || (NextStage > ChiJiTimeTable.eBattleStage.BS_NONE && ChijiDataManager.GetInstance().CurBattleStage < NextStage))
            {
                if (ShowTime != null)
                {
                    string temp = Function.GetLeftTime((int)StageStartTime + LastTime, (int)TimeManager.GetInstance().GetServerTime(), ShowtimeType.OnlineGift);
                    //Logger.LogErrorFormat("{0},stage = {1}", temp, BattleStage);
                    ShowTime.text = ChijiMainFrame.mStagename + temp;
                }
            }
        }
    }
}
