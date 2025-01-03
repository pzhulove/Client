using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using GameClient;
using ProtoTable;
using System;

public class ChapterChange : Singleton<ChapterChange>
{
    public static void Init()
    {
        ChapterChange.instance.onInit();
    }

    public static void UnInit()
    {
        ChapterChange.instance.onClear();
    }


    UInt32 submittaskid;

    List<MissionManager.SingleMissionInfo> getChapterChangeMission()
    {
        return MissionManager.GetInstance().GetAllTaskByType(MissionTable.eTaskType.TT_MAIN, new MissionTable.eSubType[] {MissionTable.eSubType.Chapter_Change});
    }

    bool isInTown()
    {
        return ClientSystemManager.instance.CurrentSystem is ClientSystemTown;
    }

    void ShowChapterChange(UInt32 iTaskID,string comicPath)
    {
        if(CheckMissionCanSubmit(iTaskID))
        {
            submittaskid = iTaskID;
            MissionManager.GetInstance().sendCmdSubmitTask(iTaskID, TaskSubmitType.TASK_SUBMIT_UI, 0);
            ClientSystemManager.GetInstance().OpenFrame<ChapterChangeComic>(GameClient.FrameLayer.Top,comicPath);
        } 
    }   

    bool isChapterChangeMission(MissionTable table)
    {
        return table.TaskType == MissionTable.eTaskType.TT_MAIN && table.SubType == MissionTable.eSubType.Chapter_Change;
    }

    void CheckChapterChangeMission(UInt32 iTaskID)
    {
        var info = MissionManager.GetInstance().GetMissionInfo(iTaskID);
        var infotable =  TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iTaskID);

        if(info != null && infotable != null)
        {
            if( info.status == (int)TaskStatus.TASK_FINISHED && isChapterChangeMission(infotable) && isInTown() )
            {
               ShowChapterChange(iTaskID,infotable.MissionParam);
            }
        }
    }

    void OnDeleteMission(UInt32 iTaskID)
    {
        if(submittaskid == iTaskID)
        {
            submittaskid = 0;
        }
    }

    bool CheckMissionCanSubmit(UInt32 iTaskID)
    {
        if(submittaskid == iTaskID)
        {
            return false;
        }

        return true;
    }

    void OnSceneLoadEnd()
    {
        var missionInfos = getChapterChangeMission();
        for(int i = 0; i < missionInfos.Count; ++i)
        {
            CheckChapterChangeMission(missionInfos[i].taskID);
        }
    }

    void CheckNewMission()
    {

    }

    public void onInit()
    {
        MissionManager.GetInstance().onAddNewMission += CheckChapterChangeMission;
        MissionManager.GetInstance().onDeleteMission += OnDeleteMission;
        ClientSystemManager.instance.OnSwitchSystemFinished.AddListener(OnSceneLoadEnd);

    }

    public void onClear()
    {
        MissionManager.GetInstance().onAddNewMission -= CheckChapterChangeMission;
        MissionManager.GetInstance().onDeleteMission -= OnDeleteMission;
        ClientSystemManager.instance.OnSwitchSystemFinished.RemoveListener(OnSceneLoadEnd);
    }
}