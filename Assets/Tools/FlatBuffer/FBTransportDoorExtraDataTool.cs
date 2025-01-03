using System;
using UnityEngine;

using FlatBuffers;

public class FBTransportDoorExtraDataTool
{

    private static Offset<FBTransportDoorExtraData.Vector3> _createVec3(FlatBufferBuilder builder, Vector3 pos)
    {
        return FBTransportDoorExtraData.Vector3.CreateVector3(builder,
                pos.x, pos.y, pos.z);
    }

    public static Offset<FBTransportDoorExtraData.DTransportDoorExtraData> CreateTransportDoorExtraData(FlatBufferBuilder builder, DTransportDoorExtraData data)
    {
        FBTransportDoorExtraData.DTransportDoorExtraData.StartDTransportDoorExtraData(builder);

        FBTransportDoorExtraData.DTransportDoorExtraData.AddButtom(builder, _createVec3(builder, data.buttom));
        FBTransportDoorExtraData.DTransportDoorExtraData.AddLeft(builder,   _createVec3(builder, data.left));
        FBTransportDoorExtraData.DTransportDoorExtraData.AddRight(builder,  _createVec3(builder, data.right));
        FBTransportDoorExtraData.DTransportDoorExtraData.AddTop(builder,    _createVec3(builder, data.top));

        return FBTransportDoorExtraData.DTransportDoorExtraData.EndDTransportDoorExtraData(builder);
    }
}