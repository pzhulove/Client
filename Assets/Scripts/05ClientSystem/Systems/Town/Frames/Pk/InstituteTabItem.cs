using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using GameClient;
using System;
public class InstituteTabItem : MonoBehaviour {

    public GameObject haveAward;
    public GameObject lockState;

    public Text title;
    public Button btn;
    public void InitTab(InstituteTable data,Action<InstituteTable> clickCallBack)
    {
        MissionManager.SingleMissionInfo info = MissionManager.GetInstance().GetMissionInfo((uint)data.MissionID);
        if (info != null)
        {
            haveAward.CustomActive(info.status == (int)Protocol.TaskStatus.TASK_FINISHED);
            lockState.CustomActive(info.status == (int)Protocol.TaskStatus.TASK_UNFINISH);
        }
        title.text = data.Title;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(()=> 
        {
            if (clickCallBack != null)
                clickCallBack(data);
        });
    }
}
