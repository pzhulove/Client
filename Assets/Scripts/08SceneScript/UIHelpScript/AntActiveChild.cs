using UnityEngine;
using System.Collections;

public class AntActiveChild : MonoBehaviour {
    public bool bActiveChild = false;
    public delegate void DelegateOnStatusChanged(AntActiveChild child,bool bActive);
    public DelegateOnStatusChanged onStatusChanged;
    void Start()
    {
        Active();
    }

    void Active()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(bActiveChild);
        }
    }

    public void AntActive()
    {
        bActiveChild = !bActiveChild;
        Active();
        if(onStatusChanged != null)
        {
            onStatusChanged(this,bActiveChild);
        }
    }
}
