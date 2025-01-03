using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class ArtifactJarToggleView : MonoBehaviour
    {
        [SerializeField]
        private Toggle mToggle;

        [SerializeField]
        private Text mName;

        [SerializeField]
        private GameObject mSelect;

        [SerializeField]
        private Button mGrayBtn;

        public delegate void Callback(ArtifactJarBuy jarData);
        private Callback mCallback;
        private ArtifactJarBuy mCurJarData;
        private List<ArtifactJarBuy> allJarData = new List<ArtifactJarBuy>();
        public void Init(ArtifactJarBuy jarData)
        {
            mCurJarData = jarData;
            ProtoTable.JarBonus jarBonusData = TableManager.GetInstance().GetTableItem<ProtoTable.JarBonus>((int)jarData.jarId);
            if(jarBonusData != null)
            {
                mName.text = jarBonusData.Name;
            }
            if(canPreviewJar())
            {
                mGrayBtn.CustomActive(false);
            }
            else
            {
                mGrayBtn.CustomActive(true);
            }
            mGrayBtn.onClick.RemoveAllListeners();
            mGrayBtn.onClick.AddListener(() =>
            {
                GameClient.SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("artifact_jar_toggle_tips", jarBonusData.Filter[0]));
            });
            mToggle.onValueChanged.RemoveAllListeners();
            mToggle.onValueChanged.AddListener((flag) =>
            {
                if(flag)
                {
                    if(canPreviewJar())
                    {
                        mSelect.CustomActive(true);
                        if (mCallback != null)
                        {
                            mCallback(jarData);
                        }
                    }
                    else
                    {
                        GameClient.SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("artifact_jar_toggle_tips", jarBonusData.Filter[0]));
                    }
                }
                else
                {
                    mSelect.CustomActive(false);
                }
            });
        }
        public void Dispose()
        {

        }
        public void SetCallback(Callback mCallback)
        {
            if(mCallback != null)
            {
                this.mCallback = mCallback;
            }
        }
        
        public void SetToggleIsOn(bool flag)
        {
            if(flag)
            {
                if(mToggle.isOn == true)
                {
                    mToggle.isOn = false;
                }
                mToggle.isOn = true;
            }
            else
            {
                mToggle.isOn = false;
            }
        }

        public bool canPreviewJar()
        {
            ProtoTable.JarBonus jarBonusData = TableManager.GetInstance().GetTableItem<ProtoTable.JarBonus>((int)mCurJarData.jarId);
            if (jarBonusData != null)
            {
                if(jarBonusData.Filter[0] <= PlayerBaseData.GetInstance().Level)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}