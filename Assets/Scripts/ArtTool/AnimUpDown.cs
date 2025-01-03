using UnityEngine;
using System.Collections;

public class AnimUpDown : MonoBehaviour
{
    public float Distance = 0.5f;
    //public float Speed = 0.1f;
    public float Rate = 1.0f;

    private float Speed = 0.1f;
    private Vector3 chuShiPos;
    private float zuiDaPosY;
    private float zuiXiaoPosY;
    private bool IsUp = false;

    void Start()
    {
        //Distance = Distance * 0.01f + Random.Range(0.0f, Speed);
        //Distance = Distance ;
        chuShiPos = this.transform.position;
        var shu = Random.value;
        if (shu < 0.5)
        {
            IsUp = true;

        }
        else
        {
            IsUp = false;
        }
        
    }

    void Update()
    {   
        zuiDaPosY = chuShiPos.y + Distance + Random.Range(0.0f, Speed);
        zuiXiaoPosY = chuShiPos.y - Distance + Random.Range(0.0f, Speed);

        if (!IsUp)
        {
            this.transform.Translate(Vector3.up * Time.deltaTime * Rate);
            if (this.transform.position.y >= zuiDaPosY)

            {
                IsUp = true;
            }
        }
        else
        {
            this.transform.Translate(-Vector3.up * Time.deltaTime * Rate);
            if (this.transform.position.y <= zuiXiaoPosY)
            {
                IsUp = false;
            }
        }

    }
}
