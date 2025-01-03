using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HGFrameTween : MonoBehaviour
{
    public AnimationCurve alphaCurve;
    public float          timeLength = 1.0f;
    private float         startTime;
    
    private CanvasRenderer[] canvasRenders;
    void Awake()
    {
        canvasRenders = gameObject.GetComponentsInChildren<CanvasRenderer>();
    }
    
    void Start()
    {
        startTime = Time.realtimeSinceStartup;
    }
    void Update()
    {
        float time = Time.realtimeSinceStartup - startTime;
        bool bAutoDestory = true;
#if UNITY_EDITOR
        if(Application.isPlaying == false)
        {
            bAutoDestory = false;
        }
#endif

        if(bAutoDestory && time > timeLength)
        {
            Destroy(gameObject);  
        }

        time = Mathf.Repeat(time,timeLength) / timeLength;
        float alpha = alphaCurve.Evaluate(time);
        
        for(int i = 0; i < canvasRenders.Length; ++i)
        {
            canvasRenders[i].SetAlpha(alpha);
        }

        
    }
}