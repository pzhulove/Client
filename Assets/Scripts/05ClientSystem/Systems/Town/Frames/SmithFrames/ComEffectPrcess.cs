using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    [System.Serializable]
    public class EffectProcessConfig
    {
        public int index = 0;
        public float start = 0.0f;
        public float len = 1.0f;
        public UnityEvent onActionStart = null;
        public UnityEvent onActionEnd = null;
    }

    public class ComEffectPrcess : MonoBehaviour
    {
        public float startDelay = 0.0f;
        public ComEffectLoader comEffectLoader = null;
        public EffectProcessConfig[] mConfigs = new EffectProcessConfig[0];
        public void Play()
        {
            Stop();
            for (int i = 0; i < mConfigs.Length; ++i)
            {
                var config = mConfigs[i];
                if(null != config)
                {
                    InvokeMethod.InvokeInterval(this, startDelay + config.start, config.len, config.len,
                        () =>{
                            if (null != comEffectLoader)
                            {
                                comEffectLoader.LoadEffect(config.index);
                                comEffectLoader.ActiveEffect(config.index);
                            }
                            if(null != config.onActionStart)
                            {
                                config.onActionStart.Invoke();
                            }
                        },
                        null,
                        () =>{
                            if (null != comEffectLoader)
                            {
                                comEffectLoader.DeActiveEffect(config.index);
                            }
                            if (null != config.onActionEnd)
                            {
                                config.onActionEnd.Invoke();
                            }
                        });
                }
            }
        }

        public void Stop()
        {
            InvokeMethod.RmoveInvokeIntervalCall(this);
            for (int i = 0; i < mConfigs.Length; ++i)
            {
                var config = mConfigs[i];
                if(null != config)
                {
                    if (null != comEffectLoader)
                    {
                        comEffectLoader.DeActiveEffect(config.index);
                    }
                }
            }
        }

        void OnDestroy()
        {
            InvokeMethod.RmoveInvokeIntervalCall(this);
        }
    }
}