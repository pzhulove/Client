using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    class JarBuyResultFrame : ClientFrame
    {
        [UIControl("Text")]
        Text m_labContent;
        [UIControl("CanNotify")]
        Toggle m_togCanNotify;

        Coroutine m_coroutine;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Jar/JarBuyResult";
        }

        protected override void _OnOpenFrame()
        {
            ShowItemsFrameData data = userData as ShowItemsFrameData;
            if (data == null)
            {
                Logger.LogError("open ShowItemsFrame frame data is null");
                return;
            }
            m_togCanNotify.onValueChanged.RemoveAllListeners();
            m_togCanNotify.isOn = !JarDataManager.GetInstance().isNotify;
            m_togCanNotify.onValueChanged.AddListener(var =>
            {
                JarDataManager.GetInstance().isNotify = !var;
            });

            m_coroutine = GameFrameWork.instance.StartCoroutine(_SetupContent());
        }

        protected override void _OnCloseFrame()
        {
            if (m_coroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(m_coroutine);
                m_coroutine = null;
            }
        }

        IEnumerator _SetupContent()
        {
            bool needThisFrame = true;
            ShowItemsFrameData data = userData as ShowItemsFrameData;
            if (data.data.eType == ProtoTable.JarBonus.eType.EqrecoJar)
            {
                needThisFrame = false;
            }
            if (JarDataManager.GetInstance().isNotify && needThisFrame)
            {
                for (int i = 5; i > 0; --i)
                {
                    
                    if (data.buyInfo == null)
                    {
                        m_labContent.text = TR.Value("jar_use_result", data.items[0].item.Name, data.items[0].item.Count, i);
                    }
                    else
                    {
                        if (data.items.Count > 0)
                        {
                            m_labContent.text = TR.Value("jar_buy_result", data.items[0].item.Name, data.items[0].item.Count, data.buyInfo.nBuyCount, i);
                        }
                        else
                        {
                            m_labContent.text = string.Empty;
                        }
                    }
                    yield return Yielders.GetWaitForSeconds(1.0f);
                }

                if (ClientSystemManager.GetInstance().IsFrameOpen<ShowItemsFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<ShowItemsFrame>();
                }
                ClientSystemManager.GetInstance().OpenFrame<ShowItemsFrame>(FrameLayer.Middle, userData);
                m_coroutine = null;
                frameMgr.CloseFrame(this);
            }
            else
            {
                frame.CustomActive(false);
                yield return Yielders.GetWaitForSeconds(0);
                _OnOkClicked();
            }
        }

        [UIEventHandle("OK")]
        void _OnOkClicked()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<ShowItemsFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ShowItemsFrame>();
            }
            ClientSystemManager.GetInstance().OpenFrame<ShowItemsFrame>(FrameLayer.Middle, userData);
            frameMgr.CloseFrame(this);
        }
    }
}
