using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameClient;

[RequireComponent(typeof(Button))]
public class ComGoToMallButton : MonoBehaviour
{
    [SerializeField] OutComeData Param;
    private Button mButton;

    void Awake()
    {
        mButton = GetComponent<Button>();
        mButton.onClick.AddListener(OnButtonClick);
    }

    void OnDestroy()
    {
        mButton.onClick.RemoveListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        //新老数据转换
        var paramData = new MallNewFrameParamData();
        if (Param.MainTab == MallType.Gift)
            paramData.MallNewType = MallNewType.LimitTimeMall;
        else if (Param.MainTab == MallType.Recharge)
            paramData.MallNewType = MallNewType.ReChargeMall;
        else if (Param.MainTab == MallType.FashionMall)
            paramData.MallNewType = MallNewType.FashionMall;
        else
            paramData.MallNewType = MallNewType.PropertyMall;

        paramData.Index = Param.SubTab;


        ClientSystemManager.GetInstance().OpenFrame<MallNewFrame>(FrameLayer.Middle, paramData);

        //ClientSystemManager.GetInstance().OpenFrame<MallFrame>(FrameLayer.Middle, Param);
    }

}
