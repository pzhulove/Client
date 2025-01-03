// automatically generated, do not modify

namespace FBTransportDoorExtraData
{

using FlatBuffers;

public sealed class Vector3 : Struct {
  public Vector3 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float X { get { return bb.GetFloat(bb_pos + 0); } }
  public float Y { get { return bb.GetFloat(bb_pos + 4); } }
  public float Z { get { return bb.GetFloat(bb_pos + 8); } }

  public static Offset<Vector3> CreateVector3(FlatBufferBuilder builder, float X, float Y, float Z) {
    builder.Prep(4, 12);
    builder.PutFloat(Z);
    builder.PutFloat(Y);
    builder.PutFloat(X);
    return new Offset<Vector3>(builder.Offset);
  }
};

public sealed class DTransportDoorExtraData : Table {
  public static DTransportDoorExtraData GetRootAsDTransportDoorExtraData(ByteBuffer _bb) { return GetRootAsDTransportDoorExtraData(_bb, new DTransportDoorExtraData()); }
  public static DTransportDoorExtraData GetRootAsDTransportDoorExtraData(ByteBuffer _bb, DTransportDoorExtraData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public static bool DTransportDoorExtraDataBufferHasIdentifier(ByteBuffer _bb) { return __has_identifier(_bb, "DTra"); }
  public DTransportDoorExtraData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Vector3 Top { get { return GetTop(new Vector3()); } }
  public Vector3 GetTop(Vector3 obj) { int o = __offset(4); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 Buttom { get { return GetButtom(new Vector3()); } }
  public Vector3 GetButtom(Vector3 obj) { int o = __offset(6); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 Left { get { return GetLeft(new Vector3()); } }
  public Vector3 GetLeft(Vector3 obj) { int o = __offset(8); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 Right { get { return GetRight(new Vector3()); } }
  public Vector3 GetRight(Vector3 obj) { int o = __offset(10); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartDTransportDoorExtraData(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddTop(FlatBufferBuilder builder, Offset<Vector3> topOffset) { builder.AddStruct(0, topOffset.Value, 0); }
  public static void AddButtom(FlatBufferBuilder builder, Offset<Vector3> buttomOffset) { builder.AddStruct(1, buttomOffset.Value, 0); }
  public static void AddLeft(FlatBufferBuilder builder, Offset<Vector3> leftOffset) { builder.AddStruct(2, leftOffset.Value, 0); }
  public static void AddRight(FlatBufferBuilder builder, Offset<Vector3> rightOffset) { builder.AddStruct(3, rightOffset.Value, 0); }
  public static Offset<DTransportDoorExtraData> EndDTransportDoorExtraData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DTransportDoorExtraData>(o);
  }
  public static void FinishDTransportDoorExtraDataBuffer(FlatBufferBuilder builder, Offset<DTransportDoorExtraData> offset) { builder.Finish(offset.Value, "DTra"); }
};


}
