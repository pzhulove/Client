// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class MallShopMultiITable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1291395942,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static MallShopMultiITable GetRootAsMallShopMultiITable(ByteBuffer _bb) { return GetRootAsMallShopMultiITable(_bb, new MallShopMultiITable()); }
  public static MallShopMultiITable GetRootAsMallShopMultiITable(ByteBuffer _bb, MallShopMultiITable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public MallShopMultiITable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string StartTime { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetStartTimeBytes() { return __p.__vector_as_arraysegment(6); }
  public string EndTime { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetEndTimeBytes() { return __p.__vector_as_arraysegment(8); }
  public string Malls { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetMallsBytes() { return __p.__vector_as_arraysegment(10); }
  public int Multiple { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<MallShopMultiITable> CreateMallShopMultiITable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset StartTimeOffset = default(StringOffset),
      StringOffset EndTimeOffset = default(StringOffset),
      StringOffset MallsOffset = default(StringOffset),
      int Multiple = 0) {
    builder.StartObject(5);
    MallShopMultiITable.AddMultiple(builder, Multiple);
    MallShopMultiITable.AddMalls(builder, MallsOffset);
    MallShopMultiITable.AddEndTime(builder, EndTimeOffset);
    MallShopMultiITable.AddStartTime(builder, StartTimeOffset);
    MallShopMultiITable.AddID(builder, ID);
    return MallShopMultiITable.EndMallShopMultiITable(builder);
  }

  public static void StartMallShopMultiITable(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddStartTime(FlatBufferBuilder builder, StringOffset StartTimeOffset) { builder.AddOffset(1, StartTimeOffset.Value, 0); }
  public static void AddEndTime(FlatBufferBuilder builder, StringOffset EndTimeOffset) { builder.AddOffset(2, EndTimeOffset.Value, 0); }
  public static void AddMalls(FlatBufferBuilder builder, StringOffset MallsOffset) { builder.AddOffset(3, MallsOffset.Value, 0); }
  public static void AddMultiple(FlatBufferBuilder builder, int Multiple) { builder.AddInt(4, Multiple, 0); }
  public static Offset<MallShopMultiITable> EndMallShopMultiITable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MallShopMultiITable>(o);
  }
  public static void FinishMallShopMultiITableBuffer(FlatBufferBuilder builder, Offset<MallShopMultiITable> offset) { builder.Finish(offset.Value); }
};


}


#endif
