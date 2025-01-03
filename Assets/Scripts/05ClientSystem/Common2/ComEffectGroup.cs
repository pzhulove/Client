using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class ComEffectGroup : MonoBehaviour
{
    public List<ComEffect> effectGroups = null;
    public void PlayEffect(string name)
    {
        //var current = effectGroups.Find(x => { return x.key == name; });
        //if(current != null)
        //{
        //    current.Play();
        //}
    }

    public void StopEffect(string name)
    {
        //var current = effectGroups.Find(x => { return x.key == name; });
        //if (current != null)
        //{
        //    current.Stop();
        //}
    }
}
