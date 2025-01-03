using UnityEngine;
using System;

public class DataBuryingPointTimer : MonoBehaviour
{
    [SerializeField]
    private string mFrameName;
    bool bBeginCal = false;
    
    float EndTime = 0;

    void Start()
    {
        EndTime = 0;
        bBeginCal = true;
    }
    void Update()
    {
        if(bBeginCal)
        {
            EndTime += Time.deltaTime;
        }     
    }
    
    void OnDestroy()
    {
        SendOpenFrameTime();
    }

    private void SendOpenFrameTime()
    {
        bBeginCal = false;
       // Logger.LogErrorFormat(string.Format("OpenFrmae|{0} Time|{1}s", mFrameName, (int)EndTime));
        GameStatisticManager.GetInstance().DoStartOpenFrameUseTime(mFrameName, (int)EndTime);
        EndTime = 0;
    }
}