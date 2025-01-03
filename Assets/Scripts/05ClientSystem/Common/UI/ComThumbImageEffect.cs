using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class ComThumbImageEffect : MonoBehaviour 
{
    public GameObject mToggleRoot;
    public Image mSelectedFg;
    public Image mFg;

    public void Set(bool status)
    {
        //gameObject.SetActive(status);
        //mSelectedFg.gameObject.SetActive(status);
        mFg.enabled = !status;

        //if (status)
        //{
        //    
        //    //mToggleRoot.transform.localScale = Vector3.one * 1.3f;
        //    //gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        //}
        //else
        //{
        //    mToggleRoot.transform.localScale = Vector3.one;
        //    //gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        //}
    }
}
