using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class AutoPlay : MonoBehaviour {

    protected Animation[] ans;
    protected ParticleSystem[] pss;
    protected Animator[] animators;
    protected bool      _play;
    // Use this for initialization
    void Start () {
	   
	}

    void Awake() {
        ans = gameObject.GetComponentsInChildren<Animation>(true);
        pss = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        animators = gameObject.GetComponentsInChildren<Animator>(true);
    }

    void OnEnable()
    {
        Play();
    }

    void Play()
    {
        if(gameObject == null)
        {
            return;
        }

        foreach (ParticleSystem ps in pss)
        {
            if (ps)
            {
                //ps.Stop();
                ps.Play();
            }

        }

        foreach (Animation an in ans)
        {
            if (an)
            {
                //an.Stop();
                an.Play();
            }

        }

        if (animators != null)
        {
            foreach (var ani in animators)
            {
                if (ani != null)
                {
                    int hashname = ani.GetCurrentAnimatorStateInfo(0).fullPathHash;
                    ani.Play(hashname);
                }
            }
        }
    }
 
}
