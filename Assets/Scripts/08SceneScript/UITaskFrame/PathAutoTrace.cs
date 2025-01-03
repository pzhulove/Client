using UnityEngine;
using System.Collections;

public class PathAutoTrace : MonoBehaviour
{
    public void Initialize()
    {
        transform.gameObject.SetActive(true);
    }

    public void SetVisible(bool bVisible)
    {
        transform.gameObject.SetActive(bVisible);
    }
}
