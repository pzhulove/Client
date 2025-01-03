using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 一个通用的界面背景组件
// 可以指定大小
public class CommonPopupFrame : MonoBehaviour
{
    public enum SizeType
    {
        界面大小1,
        界面大小2,
        界面大小3,
        界面大小4,
        界面大小5,
        界面大小6,
        界面大小7,
    }   

    RectTransform rectTransform = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSize(SizeType sizeType)
    {
        CommonPopupFrameData config = this.gameObject.GetComponent<CommonPopupFrameData>();
        if(config == null)
        {
            return;
        }

        var sizeList = config.sizeList;
        if(sizeList == null)
        {
            return;
        }

        if ((int)sizeType >= sizeList.Count)
        {
            return;
        }

        rectTransform = this.gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = sizeList[(int)sizeType];
    }
    
    public void SetFrameTitle(string titleName)
    {
        CommonPopupFrameData config = this.gameObject.GetComponent<CommonPopupFrameData>();
        if (config == null)
        {
            return;
        }

        config.frameTitle.SafeSetText(titleName);
    }

    public void ShowHelpBtn(bool show)
    {
        CommonPopupFrameData config = this.gameObject.GetComponent<CommonPopupFrameData>();
        if (config == null)
        {
            return;
        }

        config.frameHelp.CustomActive(show);
    }

    public void ShowCloseBtn(bool show)
    {
        CommonPopupFrameData config = this.gameObject.GetComponent<CommonPopupFrameData>();
        if (config == null)
        {
            return;
        }

        config.frameClose.CustomActive(show);
    }

    public bool GetHelpBtnShowState()
    {
        CommonPopupFrameData config = this.gameObject.GetComponent<CommonPopupFrameData>();
        if (config == null)
        {
            return false;
        }

        if(config.frameHelp == null)
        {
            return false;
        }

        return config.frameHelp.activeSelf;
    }

    public bool GetCloseBtnShowState()
    {
        CommonPopupFrameData config = this.gameObject.GetComponent<CommonPopupFrameData>();
        if (config == null)
        {
            return false;
        }

        if (config.frameClose == null)
        {
            return false;
        }

        return config.frameClose.activeSelf;
    }
}


