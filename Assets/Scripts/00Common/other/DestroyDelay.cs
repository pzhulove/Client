using UnityEngine;
using System.Collections;

public class DestroyDelay : MonoBehaviour
{
    public float Delay = 0.5F;

    void Update()
    {
        Delay -= Time.deltaTime;
        if (Delay <= 0)
        {
            gameObject.transform.SetParent(null, false);
            CGameObjectPool.instance.RecycleGameObject(gameObject);
            //Destroy(gameObject);
        }
    }
}
