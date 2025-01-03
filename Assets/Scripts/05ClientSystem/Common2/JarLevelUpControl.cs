using UnityEngine;
using System.Collections;
using System;

namespace GameClient
{
    [Serializable]
    class AnimationConfig
    {
        public string key;
        public Animation[] ani = new Animation[0];
        public GameObject[] akEffectObjects = new GameObject[0];
        public float fLastTime = 1.0f;
        public float GetAnimationTime()
        {
            return fLastTime;
        }
        public void Stop()
        {
            if(!string.IsNullOrEmpty(key))
            {
                for (int i = 0; i < ani.Length; ++i)
                {
                    if (ani[i] != null)
                    {
                        ani[i].Stop();
                    }
                }
            }

            for(int i = 0; i < akEffectObjects.Length; ++i)
            {
                if(akEffectObjects[i] != null)
                {
                    akEffectObjects[i].CustomActive(false);
                }
            }
        }
        public void Play()
        {
            for(int i = 0; i < akEffectObjects.Length; ++i)
            {
                if(akEffectObjects[i] != null)
                {
                    akEffectObjects[i].CustomActive(false);
                }
            }

            if(!string.IsNullOrEmpty(key))
            {
                for (int i = 0; i < ani.Length; ++i)
                {
                    if (ani[i] != null)
                    {
                        ani[i].Play(key);
                    }
                }
            }
        }
    }

    [Serializable]
    class JarConfig
    {
        public AnimationConfig[] akConfigs = new AnimationConfig[0];
        public float GetAnimationLength(string key)
        {
            AnimationConfig find = null;
            for(int i = 0; i < akConfigs.Length; ++i)
            {
                if(akConfigs[i].key == key)
                {
                    find = akConfigs[i];
                    break;
                }
            }
            if(find != null)
            {
                return find.fLastTime;
            }
            return 0.0f;
        }
    }

    class JarLevelUpControl : MonoBehaviour
    {
        public JarConfig jarConfig = null;
        public float GetAnimationLength(string key)
        {
            if(jarConfig != null)
            {
                return jarConfig.GetAnimationLength(key);
            }
            return 0.0f;
        }
        public void Play(string key)
        {
            if(jarConfig != null)
            {
                for(int i = 0; i < jarConfig.akConfigs.Length; ++i)
                {
                    if(jarConfig.akConfigs[i] != null && jarConfig.akConfigs[i].key == key)
                    {
                        jarConfig.akConfigs[i].Play();
                        break;
                    }
                }
            }
        }

        public void PlayAll()
        {
            if (jarConfig != null)
            {
                for (int i = 0; i < jarConfig.akConfigs.Length; ++i)
                {
                    if (jarConfig.akConfigs[i] != null)
                    {
                        jarConfig.akConfigs[i].Play();
                    }
                }
            }
        }

        public void Stop(string key)
        {
            if (jarConfig != null)
            {
                for (int i = 0; i < jarConfig.akConfigs.Length; ++i)
                {
                    if (jarConfig.akConfigs[i] != null && jarConfig.akConfigs[i].key == key)
                    {
                        jarConfig.akConfigs[i].Play();
                        break;
                    }
                }
            }
        }

        public void StopAll()
        {
            if (jarConfig != null)
            {
                for (int i = 0; i < jarConfig.akConfigs.Length; ++i)
                {
                    if (jarConfig.akConfigs[i] != null)
                    {
                        jarConfig.akConfigs[i].Stop();
                    }
                }
            }
        }
    }
}