using UnityEngine;
using System.Collections;

public class testScript : MonoBehaviour
{
    public AnimationCurve anim = new AnimationCurve();
    void Start()
    {
        Keyframe[] ks = new Keyframe[2];
        ks[0] = new Keyframe(0, 0)
        {
            inTangent = 2,
            outTangent = 2
        };
        ks[1] = new Keyframe(1, 1)
        {
            inTangent = 0,
            outTangent = 0
        };
        anim = new AnimationCurve(ks);
    }
    void Update()
    {
        transform.position = new Vector3(Time.time, anim.Evaluate(Time.time), 0);
        Logger.LogErrorFormat("anim.Evaluate(Time.time) = {0}", anim.Evaluate(Time.time));
    }
}