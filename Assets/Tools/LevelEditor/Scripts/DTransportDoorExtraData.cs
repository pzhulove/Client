
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///
/// ControlDoorState 
///
/// OpenDoorObj_TOP;
/// OpenDoorObj_BOTTOM;
/// OpenDoorObj_LEFT;
/// OpenDoorObj_RIGHT;
/// 
/// </summary>
public class DTransportDoorExtraData : ScriptableObject, ITransportDoorExtraData
{
    public Vector3 top    = Vector3.zero;
    public Vector3 buttom = Vector3.zero;
    public Vector3 left   = Vector3.zero;
    public Vector3 right  = Vector3.zero;

    public Vector3 GetRegionPos(TransportDoorType type)
    {
        switch (type)
        {
            case TransportDoorType.Top:
                return top;
            case TransportDoorType.Buttom:
                return buttom;
            case TransportDoorType.Left:
                return left;
            case TransportDoorType.Right:
                return right;
            default:
                return Vector3.zero;
        }
    }
}
