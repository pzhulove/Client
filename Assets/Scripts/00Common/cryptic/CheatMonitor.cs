using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using Protocol;
using Network;

/// <summary>
/// Client cheat monitor. When encryption value have been cheated, It can report to server.
/// </summary>
public class CheatMonitor : MonoSingleton<CheatMonitor>
{
    /// <summary>
    /// report to server. only report once per startup.
    /// </summary>
    private bool isReportCheated = false;

    public override void Init()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// cheat detected call back.
    /// </summary>
    public void OnCheatingDetected()
    {
        if (!isReportCheated)
        {
            Debug.LogWarning("Report to server...");
            isReportCheated = true;
            SystemNotifyManager.SysNotifyMsgBoxOK("检测到内存非法修改！");
            ReportToServer();
            StartCoroutine(WaitForQuitGame());
        }
    }

    /// <summary>
    /// report cheat player info to server.
    /// </summary>
    private void ReportToServer()
    {
        WorldRelationReportCheat msg = new WorldRelationReportCheat();
        NetManager.instance.SendCommand(ServerType.GATE_SERVER, msg);
    }

    /// <summary>
    /// wait for quit game when cheat detected.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForQuitGame()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        ForceQuitGame();
    }

    private void ForceQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
