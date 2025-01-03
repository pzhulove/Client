using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace GameClient
{
    class FashionMergeResultFrame : ClientFrame
    {
        public static void Open(FashionResultData data)
        {
            if (data != null)
            {
                ClientSystemManager.GetInstance().CloseFrame<FashionMergeResultFrame>();
                ClientSystemManager.GetInstance().OpenFrame<FashionMergeResultFrame>(FrameLayer.Middle, data);
            }
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/FashionSmithShop/FashionResultFrame";
        }

        [UIControl("", typeof(ComFashionMergeResultDataBinder))]
        ComFashionMergeResultDataBinder comDataBinder;

        [UIControl("", typeof(ComEffectLoader))]
        ComEffectLoader comEffectLoader;

        FashionResultData m_kData = null;
        bool m_bLock = false;

        protected override void _OnOpenFrame()
        {
            m_kData = userData as FashionResultData;

            m_bLock = true;
            float lockValue = null == comDataBinder ? 1.0f : comDataBinder.lockTime;
            InvokeMethod.Invoke(this, lockValue, () => { m_bLock = false; });

            _AddButton("OK", () =>
            {
                if (m_bLock)
                    return;
                frameMgr.CloseFrame(this);
            });

            //AudioManager.instance.PlaySound(12);

            if(null != comEffectLoader)
            {
                comEffectLoader.LoadEffect(0);
                comEffectLoader.ActiveEffect(0);
                comEffectLoader.LoadEffect(2);
                comEffectLoader.ActiveEffect(2);

                if (null != m_kData &&  m_kData.eFashionMergeResultType == FashionMergeResultType.FMRT_SPECIAL)
                {
                    if (!FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
                    {
                        comEffectLoader.LoadEffect(1);
                        comEffectLoader.ActiveEffect(1);
                        comEffectLoader.LoadEffect(3);
                        comEffectLoader.ActiveEffect(3);
                    }
                    else
                    {
                        comEffectLoader.LoadEffect(1);
                        comEffectLoader.ActiveEffect(1);
                        comEffectLoader.LoadEffect(2);
                        comEffectLoader.ActiveEffect(2);
                    }
                }
            }

            _SetData();
        }

        protected override void _OnCloseFrame()
        {
            if(null != m_kData)
            {
                if(m_kData.eFashionMergeResultType == FashionMergeResultType.FMRT_SPECIAL)
                {
                    if(null != m_kData.datas && m_kData.datas.Count > 1)
                    {
                        var data = m_kData.datas[1];
                        if(null != data)
                        {
                            Vector3 worldPosition = comDataBinder.GetSkyWorldPosition();
                            ItemData windItem = (m_kData.datas.Count <= 2) ? null : m_kData.datas[2];

                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnFashionSpecialFly, worldPosition.x, worldPosition.y, data, windItem);
                        }
                    }
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnFashionAutoEquip);
            m_kData = null;
            m_bLock = false;
            InvokeMethod.RemoveInvokeCall(this);
        }

        void _SetData()
        {
            if(null != comDataBinder)
            {
                comDataBinder.SetValue(m_kData);
            }
        }
    }
}