using GameClient;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
class OnOpenActiveFrame : MonoBehaviour
{
    Button m_kButton;
    public int iActiveTypeID = 0;

    void Start ()
    {
        m_kButton = GetComponent<Button>();
        m_kButton.onClick.RemoveAllListeners();
        m_kButton.onClick.AddListener(OnClick);
	}

    void OnClick()
    {
        //福利界面
        if (iActiveTypeID == 9380)
        {
            //请求任务完成情况
            WarriorRecruitDataManager.GetInstance().SendQueryTaskStatusReq();
        }

        GameClient.ActiveManager.GetInstance().OpenActiveFrame(iActiveTypeID);
       
    }
}