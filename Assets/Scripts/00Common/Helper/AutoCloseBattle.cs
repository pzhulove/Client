using UnityEngine;
using System.Collections;

public class AutoCloseBattle : MonoBehaviour
{
    private GameObject self = null;
    private float deltaTime = 0.033f;
    private float closeTime = 0;
    private bool flag = false;
    
    void Start()
    {
        self = gameObject;
    }

    public void SetCloseTime(float time)
    {
        closeTime = time;
        flag = true;
    }

    void Update()
    {
        if (!flag || self==null)
            return;
        UpdateCloseTime();
    }

    protected void UpdateCloseTime()
    {
        if (closeTime > 0)
        {
            closeTime -= deltaTime;
        }
        else
        {
            CloseSelf();
        }
    }

    void CloseSelf()
    {
        if (self == null)
            return;
        flag = false;
        GameObject.Destroy(self);
        self = null;
    }
}
