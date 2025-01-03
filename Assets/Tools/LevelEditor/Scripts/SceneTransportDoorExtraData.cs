using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransportDoorExtraData : ITransportDoorExtraData
{
    FBTransportDoorExtraData.DTransportDoorExtraData mData;

    private Vector3 mTop    = Vector3.zero;
    private Vector3 mButtom = Vector3.zero;
    private Vector3 mRight  = Vector3.zero;
    private Vector3 mLeft   = Vector3.zero;

    public SceneTransportDoorExtraData(FBTransportDoorExtraData.DTransportDoorExtraData data)
    {
        mData = data;
        _init();
    }

    private void _init()
    {
        if (null == mData)
        {
            return;
        }

        mTop    = new Vector3(mData.Top.X   , mData.Top.Y, mData.Top.Z);
        mButtom = new Vector3(mData.Buttom.X, mData.Buttom.Y, mData.Buttom.Z);
        mLeft   = new Vector3(mData.Left.X  , mData.Left.Y, mData.Left.Z);
        mRight  = new Vector3(mData.Right.X , mData.Right.Y, mData.Right.Z);
    }


    public Vector3 GetRegionPos(TransportDoorType type)
    {
        switch (type)
        {
            case TransportDoorType.Left:
                return mLeft;
            case TransportDoorType.Top:
                return mTop;
            case TransportDoorType.Right:
                return mRight;
            case TransportDoorType.Buttom:
                return mButtom;
            default:
                return Vector3.zero;
        }
    }
}
