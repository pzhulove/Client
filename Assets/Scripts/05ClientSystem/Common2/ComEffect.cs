using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class ComEffect : MonoBehaviour
{
    [Serializable]
    public class Effect
    {
        public string key = "";
        public GameObject parent = null;
        public List<GeUIEffectParticle> particles = null;
    }

    public List<Effect> effects = null;

    void OnDestroy()
    {
        effects.Clear();
        effects = null;
    }

    public void Play(string key)
    {
        if(effects != null)
        {
            var current = effects.Find(x => { return x.key == key; });
            if(current != null)
            {
                if(current.parent != null)
                {
                    var tweens = GamePool.ListPool<DOTweenAnimation>.Get();
                    current.parent.GetComponentsInChildren<DOTweenAnimation>(tweens);

                    for(int i = 0; i < tweens.Count; ++i)
                    {
                        tweens[i].autoKill = false;
                    }

                    current.parent.CustomActive(true);
                    for (int i = 0; i < current.particles.Count; ++i)
                    {
                        current.particles[i].RestartEmit();
                    }

                    for(int i = 0; i < tweens.Count; ++i)
                    {
                        tweens[i].DORestart();
                    }

                    GamePool.ListPool<DOTweenAnimation>.Release(tweens);
                }
            }
            else
            {
                Logger.LogErrorFormat("effect has not find named {0}", key);
            }
        }
    }

    public void Stop(string key)
    {
        if (effects != null)
        {
            var current = effects.Find(x => { return x.key == key; });
            if (current != null)
            {
                if (current.parent != null)
                {
                    for (int i = 0; i < current.particles.Count; ++i)
                    {
                        current.particles[i].StopEmit();
                    }
                    current.parent.CustomActive(false);
                }
            }
        }
    }
}