using UnityEngine;
using System.Collections;
using DG.Tweening;
public enum ShockMode
{
    ER_NORMAL = 0,
    ER_BACK = 1,
}

[System.Serializable]
public struct ShockData
{
    public float time;
    public float xrange;
    public float yrange;
    public int mode;
    public float speed;
    public ShockData(float time, float speed, float xrange, float yrange, int mode = 0)
    {
        this.time = time;
        this.xrange = xrange;
        this.yrange = yrange;
        this.mode = mode;
        this.speed = speed;
    }
}

public class GeShockEffect
{
    private int num;
    private float xRange;
    private float yRange;
    private float totalTime;
    private int mode;
    public int Mode
    {
        get { return mode; }
    }

    private bool isRuning = false;
    private bool decelebrate = false;
    private Vector3 orginalPos;
    private Transform node;
    private float m_fCurTime = 0;

    private float singleTime = 0;
    private State state = State.NONE;

    private float goTime = 0;
    private float backTime = 0;
    private float negGoTime = 0;
    private float negBackTime = 0;

    private float tempXrange;
    private float tempYrange;
    private float xReduce = 0;
    private float yReduce = 0;

    private float speed;
    private float radius = 0;
    enum State
    {
        GO,
        BACK,
        NEG_GO,
        NEG_BACK,
        NONE
    }


    public void Init(GameObject obj, int mode)
    {
        this.mode = mode;
        if (obj != null)
        {
            node = obj.transform;
        }
    }

    public void SetMode(int mode)
    {
        this.mode = mode;
    }

    public void Start(float t, float s, float xr, float yr)
    {
        if (isRuning)
            return;

        isRuning = true;
        m_fCurTime = 0.0f;
        totalTime = t;
        xRange = xr;
        yRange = yr;
        speed = s;
        if (node != null)
            orginalPos = node.localPosition;
    }

    public void StartShock(float totalTime, int num, float xRange, float yRange, bool decelebrate, float xReduce, float yReduce, int mode, float radius = 1)
    {
        if (isRuning) return;
        this.num = num;
        this.xRange = xRange;
        this.yRange = yRange;
        this.tempXrange = xRange;
        this.tempYrange = yRange;
        this.totalTime = totalTime;

        this.decelebrate = decelebrate;
        this.xReduce = xReduce;
        this.yReduce = yReduce;
        isRuning = true;
        goTime = 0;
        backTime = 0;
        negGoTime = 0;
        negBackTime = 0;
        m_fCurTime = 0;
        singleTime = totalTime / num;
        state = State.GO;
        this.radius = radius;
        this.mode = mode;
        if (node != null)
            orginalPos = node.localPosition;
    }

    public void Stop()
    {
        isRuning = false;
        state = State.NONE;
        goTime = 0;
        backTime = 0;
        negGoTime = 0;
        negBackTime = 0;
        m_fCurTime = 0;
        tempXrange = xRange;
        tempYrange = yRange;
        orginalPos = Vector3.zero;
        if(node!=null)
        	node.localPosition = orginalPos;
    }

    public void Update(float deltaTime)
    {

        if (node == null) return;
        if (!isRuning) return;
        if (Mathf.Approximately(totalTime, 0))
        {
            Stop();
            return;
        }
       
        if (m_fCurTime >= totalTime)
        {
            Stop();
            return;
        }
        m_fCurTime += deltaTime;
        if (mode == 0)
        {
            float t = m_fCurTime * speed;
            float fx = Mathf.Sin(t) * xRange;
            float fy = Mathf.Cos(t) * yRange;

            node.localPosition = new Vector3(fx, fy, 0);

        }
        else if (mode == 2)
        {
            float fHalfTime = totalTime / 2;
            float t;
            if (m_fCurTime <= fHalfTime)
            {
                t = (fHalfTime - m_fCurTime) / fHalfTime;
                t = 1 - t;
            }
            else
            {
                t = (totalTime - m_fCurTime) / fHalfTime;
            }

            float fx = t * xRange;
            float fy = t * yRange;

            node.localPosition = new Vector3(fx, fy, 0);
        }
        else if (mode == 1)
        {
            float t;
            t = m_fCurTime / totalTime;
            float fx = t * xRange;
            float fy = t * yRange;

            node.localPosition = new Vector3(fx, fy, 0);
        }

        if (mode == 3)
        {
            if (state == State.GO)
            {
                goTime += deltaTime;
                float t;
                t = goTime / singleTime;
                float fx = t * tempXrange;
                float fy = t * tempYrange;
                if (goTime >= singleTime)
                {
                    goTime = 0;
                    state = State.BACK;
                }
                node.localPosition = new Vector3(fx, fy, 0);
            }
            else if (state == State.BACK)
            {
                backTime += deltaTime;
                float t;
                t = backTime / singleTime;
                t = 1 - t;
                float fx = t * tempXrange;
                float fy = t * tempYrange;
                if (backTime >= singleTime)
                {
                    backTime = 0;
                    state = State.GO;
                    if (decelebrate)
                    {
                        if (tempXrange >= xReduce)
                            tempXrange -= xReduce;
                        if (tempYrange >= yReduce)
                            tempYrange -= yReduce;
                    }
                }
                node.localPosition = new Vector3(fx, fy, 0);

            }
        }
        else if (mode == 4)
        {
            if (state == State.GO)
            {
                goTime += deltaTime;
                float t;
                t = goTime / singleTime;
                float fx = t * tempXrange;
                float fy = t * tempYrange;
                if (goTime >= singleTime)
                {
                    goTime = 0;
                    state = State.BACK;
                }
                node.localPosition = new Vector3(fx, fy, 0);
            }
            else if (state == State.BACK)
            {
                backTime += deltaTime;
                float t;
                t = backTime / singleTime;
                t = 1 - t;
                float fx = t * tempXrange;
                float fy = t * tempYrange;
                if (backTime >= singleTime)
                {
                    state = State.NEG_GO;
                    backTime = 0;
                }
                node.localPosition = new Vector3(fx, fy, 0);
            }
            else if (state == State.NEG_GO)
            {
                negGoTime += deltaTime;
                float t;
                t = negGoTime / singleTime;
                float fx = t * tempXrange;
                float fy = t * tempYrange;
                if (negGoTime >= singleTime)
                {
                    negGoTime = 0;
                    state = State.NEG_BACK;
                }
                node.localPosition = new Vector3(-fx, -fy, 0);
            }
            else if (state == State.NEG_BACK)
            {
                negBackTime += deltaTime;
                float t;
                t = negBackTime / singleTime;
                t = 1 - t;
                float fx = t * tempXrange;
                float fy = t * tempYrange;
                if (negBackTime >= singleTime)
                {
                    negBackTime = 0;
                    state = State.GO;
                    if (decelebrate)
                    {
                        if (tempXrange >= xReduce)
                            tempXrange -= xReduce;
                        if (tempYrange >= yReduce)
                            tempYrange -= yReduce;
                    }
                }
                node.localPosition = new Vector3(-fx, -fy, 0);
            }

        }
        else if (mode == 5)
        {
            if (decelebrate)
            {
                radius = Mathf.Lerp(radius, 0, m_fCurTime / totalTime);
            }
            Vector2 shockPos = Random.insideUnitCircle * radius;
            node.localPosition = new Vector3(shockPos.x, shockPos.y, 0);
        }
    }

}
