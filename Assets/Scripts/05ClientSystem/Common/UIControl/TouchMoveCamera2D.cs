using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMoveCamera2D : MonoBehaviour
{
    public bool bEnabled = false;               // �Ƿ�����
    public float offsetTime = 0.1f;             // �жϵ�ʱ����
    public Vector2 offsetLimitWidth = Vector2.zero;  // ˮƽ�����ƶ��߽�
    public Vector2 offsetLimitHieght = Vector2.zero; // ��ֱ�����ƶ��߽�

    private Vector2 beginPos = Vector2.zero;    // ����һ�����µ�  
    private Vector2 endPos = Vector2.zero;      // ���ڶ���λ�ã���קλ�ã�  
    private Vector3 speed = Vector3.zero;       // �����ٶ�
    private float timer = 0.0f;                 // ʱ�������

    private Vector2 offset = Vector2.zero;
    private Vector3 originPos;
    private Transform playerTransform;
    private int count = 0;

    public Transform PlayerTransform
    {
        set
        {
            playerTransform = value;
        }
        get
        {
            return playerTransform;
        }
    }

    public void Start()
    {
        originPos = transform.localPosition;
    }

    private void _limitPos()
    {
        var localp = transform.localPosition;

        localp.x = Mathf.Clamp(localp.x, originPos.x + offsetLimitWidth.x, originPos.x + offsetLimitWidth.y);
        localp.y = Mathf.Clamp(localp.y, originPos.y + offsetLimitHieght.x, originPos.y + offsetLimitHieght.y);

        transform.localPosition = localp;
    }

    public void UpdateMapPos()
    {
        if (playerTransform != null && bEnabled)
            _setMapPosByPlayerPos(playerTransform.position);
    }

    private void _setMapPosByPlayerPos(Vector3 playerWorldPos)
    {
        var playerScreenPos = GameClient.ClientSystemManager.GetInstance().UICamera.WorldToScreenPoint(playerWorldPos);
      //  Logger.LogErrorFormat("playerscreenPos = {0}", playerScreenPos);

        var f1 = Screen.height * 1 / 3f;
        var f2 = Screen.height * 2 / 3f;

        var f = Screen.height / 2f;

        if (playerScreenPos.y < f1 || playerScreenPos.y > f2)
        {
            var localP = transform.localPosition;

            localP.y += (f - playerScreenPos.y);

            transform.localPosition = localP;

            _limitPos();
        }


    }

    //��ʼ��λ�ã�Ϊ��������move��׼��
    void MoveBegin(Vector3 point)
    {
        beginPos = point;
        speed = Vector3.zero;

        timer = 0.0f;
    }

    //����Ŀ��λ��
    void Moveing(Vector3 point)
    {
        //��¼����϶���λ�� 
        endPos = point;

        Vector3 fir = new Vector3(beginPos.x, beginPos.y, 0);
        Vector3 sec = new Vector3(endPos.x, endPos.y, 0);

        speed = (sec - fir);    //��Ҫ�ƶ�������
    }

    //Move�������������
    void MoveEnd(Vector3 point)
    {
        MoveBegin(point);
    }

    //�ƶ�����
    void UpdateTargetPositionInComputer()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveBegin(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            timer += Time.deltaTime;  //��ʱ��

            if(timer > offsetTime)
            {
                Moveing(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            MoveEnd(Input.mousePosition);
        }
    }

    void UpdateTargetPositonInMobile()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                MoveBegin(Input.GetTouch(0).position);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                timer += Time.deltaTime;  //��ʱ��

                if (timer > offsetTime)
                {
                    Moveing(Input.GetTouch(0).position);
                }            
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                MoveEnd(Input.GetTouch(0).position);
            }
        }
    }

    public void Update()
    {
        if(!bEnabled)
        {
            return;
        }

        if (count == 0)
            UpdateMapPos();

        count++;

#if UNITY_EDITOR

        UpdateTargetPositionInComputer();

#elif (UNITY_IOS || UNITY_ANDROID)

        UpdateTargetPositonInMobile();

#endif
        if (speed == Vector3.zero)
        {
            return;
        }

        var x = transform.position.x;
        var y = transform.position.y;

        x += speed.x;//����ƫ��  
        y += speed.y;

        var newPos = new Vector3(x, y, transform.position.z);
        transform.position = newPos;

        _limitPos();

        beginPos = endPos;
        if (Mathf.Abs(speed.x - 0) < 0.00001 && Mathf.Abs(speed.y - 0) < 0.00001)
        {
            speed = Vector3.zero;
        }
    }
}