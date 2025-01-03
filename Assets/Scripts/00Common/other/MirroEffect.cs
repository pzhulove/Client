using UnityEngine;
using System.Collections;

public class MirroEffect : MonoBehaviour {

    public enum MirroType
    {
        MirroTransformRotation = 0,
        MirroParicleRotation
    }

    public MirroType             mtype;
    public Vector3               fValue;

    public void Apply(bool bMirror)
    {
        if(mtype == MirroType.MirroTransformRotation)
        {
            ApplyTransformRotation(bMirror);
        }
        else if(mtype == MirroType.MirroParicleRotation)
        {
            ApplyParicleRotation(bMirror);
        }
    }

    protected void ApplyParicleRotation(bool bMirror)
    {
        ParticleSystem ps = gameObject.GetComponentInChildren<ParticleSystem>();

        if( ps )
        {
            //ps.startRotation = ( bMirror ? fValue.y : fValue.x ) / 180.0f * Mathf.PI;
            ps.Stop();
            ps.startRotation = (bMirror ? fValue.y : fValue.x) / 180.0f * Mathf.PI;
            //ps.startSize = 1.0f;
        }
    }

    protected void ApplyTransformRotation(bool bMirror)
    {
        float angle = bMirror ? fValue.y : fValue.x;
        gameObject.transform.localRotation = Quaternion.Euler(0, angle, 0);
    }
}
