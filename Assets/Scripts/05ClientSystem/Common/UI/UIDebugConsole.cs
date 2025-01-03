using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIDebugConsole : MonoBehaviour {

    public Text text;
    public RectTransform rectText;

    private string errorMsg = "";
    private int iMsgNum = 0;

    private string ERROR_COLOR = "#ff0000ff";
    private string WARNING_COLOR = "#ff0000ff";
    private string INFO_COLOR = "#000000ff";

    public enum  LogType
    {
        Log,
        Warning,
        Error,
    }

    private int currentLogLevel = 0;

    private List<string> errorMsgList = new List<string>();
    private List<string> warningMsgList = new List<string>();
    private List<string> infoMsgList = new List<string>();

    public void SetCurretLogMode(int mode)
    {
        mode &= 0x7;

        if ((currentLogLevel & mode) > 0)
        {
            currentLogLevel -= mode;
        }
        else
        {
            currentLogLevel |= mode;
        }

        GenerateLog();
    }

    private void GenerateLog()
    {
        errorMsg = "";
        if ((currentLogLevel & 0x1) > 0)
        {
            for (int i = 0; i < infoMsgList.Count; i++)
            {
                errorMsg += infoMsgList[i];
            }
        }

        if ((currentLogLevel & 0x2) > 0)
        {
            for (int i = 0; i < warningMsgList.Count; i++)
            {
                errorMsg += warningMsgList[i];
            }
        }

        if ((currentLogLevel & 0x4) > 0)
        {
            for (int i = 0; i < errorMsgList.Count; i++)
            {
                errorMsg += errorMsgList[i];
            }
        }
        
        if (currentLogLevel > 0)
        {
            gameObject.SetActive(true);
            this.text.text = errorMsg;
            CalculateLineAndRow();
            UpdateHeight();
        }
    }

    public void SetText(string text, LogType type = LogType.Log)
    {
        switch (type)
        {
            case LogType.Log:
                text = string.Format("<color={0}>{1}:\n{2}</color>\n******************************\n", INFO_COLOR, iMsgNum++, text);
                infoMsgList.Add(text);
                break;
            case LogType.Warning:
                text = string.Format("<color={0}>{1}:\n{2}</color>\n******************************\n", WARNING_COLOR, iMsgNum++, text);
                warningMsgList.Add(text);
                break;
            case LogType.Error:
                currentLogLevel |= 0x4;
                text = string.Format("<color={0}>{1}:\n{2}</color>\n******************************\n", ERROR_COLOR, iMsgNum++, text);
                errorMsgList.Add(text);
                break;
        }

        GenerateLog();
    }

    int iLine = 0;
    int iRow = 0;

    public void SetEnable(bool iEnable)
    {
        gameObject.SetActive(iEnable);
    }

    void CalculateLineAndRow()
    {
        iLine = 0;
        foreach (var str in errorMsg.Split('\n'))
        {
            iLine++;

            if (iRow < str.Length)
            {
                iRow = str.Length;
            }
        }
    }

    void UpdateHeight()
    {
        var offsetMax = rectText.offsetMax;
        offsetMax.y = 20 * iLine;
        rectText.offsetMax = offsetMax;
    }

    public void ClearText()
    {
        errorMsg = "";
        this.text.text = "";

        errorMsgList.Clear();
        warningMsgList.Clear();
        infoMsgList.Clear();
    }

}
