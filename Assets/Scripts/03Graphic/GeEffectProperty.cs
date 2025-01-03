using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeEffectProperty : MonoBehaviour {

    [Header("是否不随角色镜像(公告板)")]
    public bool isBillBord;
    //[Header("是否特效一直在地上")]
    [HideInInspector]
    public bool isTouchGround;
    //[Header("强制Z值")]
    [HideInInspector]
    public int forceZOrder = 0;

    [Header("是否不随角色旋转")]
    public bool isNoRotate;

    private Transform mTrasform;

    private bool firstAccess = false;
    private float delta = 0f;
	
	void Start () {
        firstAccess = false;

        if (mTrasform == null)
        {
            mTrasform = gameObject.transform;
        }
            
	}
	
	
	void LateUpdate () {
		if (mTrasform != null)
        {
            if (isBillBord)
            {
                if (mTrasform.lossyScale.x <= 0)
                {
                    var localScale = mTrasform.localScale;
                    localScale.x *= -1;
                    mTrasform.localScale = localScale;
                }
            }

            if (isNoRotate)
            {
                mTrasform.rotation = Quaternion.identity;
            }

            if (isTouchGround)
            {
                if (!firstAccess)
                {
                    delta = mTrasform.position.z - mTrasform.position.y;
                    firstAccess = true;
                }
                else
                {
                    if ((mTrasform.position.z - mTrasform.position.y) != delta)
                    {
                        var pos = mTrasform.position;
                        pos.y = mTrasform.position.z - delta;
                        mTrasform.position = pos;
                    }
                }
            }
        }
	}
}
