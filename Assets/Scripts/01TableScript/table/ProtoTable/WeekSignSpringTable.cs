// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class WeekSignSpringTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -6499428,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static WeekSignSpringTable GetRootAsWeekSignSpringTable(ByteBuffer _bb) { return GetRootAsWeekSignSpringTable(_bb, new WeekSignSpringTable()); }
  public static WeekSignSpringTable GetRootAsWeekSignSpringTable(ByteBuffer _bb, WeekSignSpringTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public WeekSignSpringTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int StartLv { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int EndLv { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DungeonIDArray(int j) { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int DungeonIDLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetDungeonIDBytes() { return __p.__vector_as_arraysegment(10); }
 private FlatBufferArray<int> DungeonIDValue;
 public FlatBufferArray<int>  DungeonID
 {
  get{
  if (DungeonIDValue == null)
  {
    DungeonIDValue = new FlatBufferArray<int>(this.DungeonIDArray, this.DungeonIDLength);
  }
  return DungeonIDValue;}
 }
  public string AcquiredMethod { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetAcquiredMethodBytes() { return __p.__vector_as_arraysegment(12); }

  public static Offset<WeekSignSpringTable> CreateWeekSignSpringTable(FlatBufferBuilder builder,
      int ID = 0,
      int StartLv = 0,
      int EndLv = 0,
      VectorOffset DungeonIDOffset = default(VectorOffset),
      StringOffset AcquiredMethodOffset = default(StringOffset)) {
    builder.StartObject(5);
    WeekSignSpringTable.AddAcquiredMethod(builder, AcquiredMethodOffset);
    WeekSignSpringTable.AddDungeonID(builder, DungeonIDOffset);
    WeekSignSpringTable.AddEndLv(builder, EndLv);
    WeekSignSpringTable.AddStartLv(builder, StartLv);
    WeekSignSpringTable.AddID(builder, ID);
    return WeekSignSpringTable.EndWeekSignSpringTable(builder);
  }

  public static void StartWeekSignSpringTable(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddStartLv(FlatBufferBuilder builder, int StartLv) { builder.AddInt(1, StartLv, 0); }
  public static void AddEndLv(FlatBufferBuilder builder, int EndLv) { builder.AddInt(2, EndLv, 0); }
  public static void AddDungeonID(FlatBufferBuilder builder, VectorOffset DungeonIDOffset) { builder.AddOffset(3, DungeonIDOffset.Value, 0); }
  public static VectorOffset CreateDungeonIDVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartDungeonIDVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAcquiredMethod(FlatBufferBuilder builder, StringOffset AcquiredMethodOffset) { builder.AddOffset(4, AcquiredMethodOffset.Value, 0); }
  public static Offset<WeekSignSpringTable> EndWeekSignSpringTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<WeekSignSpringTable>(o);
  }
  public static void FinishWeekSignSpringTableBuffer(FlatBufferBuilder builder, Offset<WeekSignSpringTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

