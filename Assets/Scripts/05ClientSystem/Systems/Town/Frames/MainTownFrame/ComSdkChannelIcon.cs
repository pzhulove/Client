using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComSdkChannelIcon : MonoBehaviour
    {
        #region Model Params

        private bool bShow = false;
        
        #endregion
        
        #region View Params

        [SerializeField]
        private Image m_IcomImg;
        [SerializeField]
        private Text m_IcomText;
        [SerializeField]
        private GameObject m_IcomGo;
        
        #endregion
        
        #region PRIVATE METHODS

        void Start()
        {
            //UpdateShow();
            if (m_IcomGo)
            {
                m_IcomGo.CustomActive(bShow);
            }
        }

        void OnDestroy()
        {
            bShow = false;
        }

       

        #endregion
        
        #region  PUBLIC METHODS
        public void UpdateShow()
        {
            if (SDKInterface.Instance.IsOppoPlatform())
            {
                if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(Protocol.ServiceType.SERVICE_OPPO_COMMUNITY))
                {
                    return;
                }
            }
            else if (SDKInterface.Instance.IsVivoPlatForm())
            {
                if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(Protocol.ServiceType.SERVICE_VIVO_COMMUNITY))
                {
                    return;
                }
            }

            var sdkTableDic = TableManager.GetInstance().GetTable<ProtoTable.SDKClientResTable>();
            if (sdkTableDic == null)
            {
                Logger.LogError("[ComSdkChannelIcon] - can not find SDKClientResTable");
                return;
            }
            var sdkTableEnum = sdkTableDic.GetEnumerator();
            while (sdkTableEnum.MoveNext())
            {
                ProtoTable.SDKClientResTable sdkTableData = sdkTableEnum.Current.Value as ProtoTable.SDKClientResTable;
                if (sdkTableData == null)
                {
                    continue;
                }
                if (sdkTableData.SDK.ToString().Equals(SDKInterface.Instance.GetCurrentSDKChannel().ToString()) && sdkTableData.Open == true)
                {
                    bShow = true;
                    if (m_IcomGo)
                    {
                        m_IcomGo.CustomActive(bShow);
                    }

                    if (m_IcomImg)
                    {
                        ETCImageLoader.LoadSprite(ref m_IcomImg, sdkTableData.IconImgPath);
                    }
                    if (m_IcomText)
                    {
                        m_IcomText.text = sdkTableData.IconDesc;
                    }
                    break;
                }
            }
        }
        #endregion
    }
}