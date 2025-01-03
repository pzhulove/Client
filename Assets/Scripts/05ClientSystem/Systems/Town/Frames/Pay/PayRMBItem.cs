using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PayRMBItem : MonoBehaviour
{
    const string VIP_PAY_RMB_IMAGE_PATH = "UI/Image/NewPacked/Shouchong.png:UI_Shouchong_ShuZi_{0}";
    const string VIP_PAY_RMB_IMAGE_PATH_NEW = "UI/Image/NewPacked/Shouchong.png:Shouchong_Txt_Leichong{0}";

    [SerializeField]
    private Image[] rmbImgs;

    [SerializeField]
    private Image rmbImg;

    public void SetRMBNum(int num)
    {
        int tempNum = num;
        int numCount = Mathf.Abs(tempNum).ToString().Length;

        if (rmbImgs == null)
        {
            Logger.LogError("Try to set rmb image is failed, image component is null");
            return;
        }
        if (numCount > rmbImgs.Length)
        {
            Logger.LogErrorFormat("Please Fix , rmb num count is {0}, rmb image length is {1}", numCount , rmbImgs.Length);
            return;
        }
        for (int i = numCount; i < rmbImgs.Length; i++)
        {
            rmbImgs[i].gameObject.CustomActive(false);
        }
        for (int i = 0; i < numCount; i++)
        {
            int r1 = 0;
            int count = numCount - 1 - i;
            int pow = (int)Mathf.Pow(10, count);
            if (count == 0)
            {
                r1 = tempNum;
            }
            else
            {
                r1 = tempNum / pow;
                tempNum = tempNum % pow;
            }
            ETCImageLoader.LoadSprite(ref rmbImgs[i], string.Format(VIP_PAY_RMB_IMAGE_PATH, r1));
            rmbImgs[i].gameObject.CustomActive(true);
        }
    }

    public void SetRMBNum(string numStr)
    {
        if (string.IsNullOrEmpty(numStr))
        {
            return;
        }
        int tempNum = 0;
        if (!int.TryParse(numStr, out tempNum))
        {
            Logger.LogErrorFormat("Please Fix , rmb string {0} convert to int is error", numStr);
            return;
        }
        int numCount = numStr.ToString().Length;
        if (rmbImgs == null)
        {
            Logger.LogError("Try to set rmb image is failed, image component is null");
            return;
        }
        if (numCount > rmbImgs.Length)
        {
            Logger.LogErrorFormat("Please Fix , rmb num count is {0}, rmb image length is {1}", numCount, rmbImgs.Length);
            return;
        }
        for (int i = numCount; i < rmbImgs.Length; i++)
        {
            rmbImgs[i].gameObject.CustomActive(false);
        }
        for (int i = 0; i < numCount; i++)
        {
            int r1 = 0;
            int count = numCount - 1 - i;
            int pow = (int)Mathf.Pow(10, count);
            if (count == 0)
            {
                r1 = tempNum;
            }
            else
            {
                r1 = tempNum / pow;
                tempNum = tempNum % pow;
            }
            ETCImageLoader.LoadSprite(ref rmbImgs[i], string.Format(VIP_PAY_RMB_IMAGE_PATH, r1));
            rmbImgs[i].gameObject.CustomActive(true);
        }
    }

    public void SetRMBNum_Ex(string numStr)
    {
        if (string.IsNullOrEmpty(numStr))
        {
            return;
        }
        ETCImageLoader.LoadSprite(ref rmbImg, string.Format(VIP_PAY_RMB_IMAGE_PATH_NEW, numStr));
    }
}
