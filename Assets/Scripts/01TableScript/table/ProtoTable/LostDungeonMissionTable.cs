// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class LostDungeonMissionTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 736771014,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static LostDungeonMissionTable GetRootAsLostDungeonMissionTable(ByteBuffer _bb) { return GetRootAsLostDungeonMissionTable(_bb, new LostDungeonMissionTable()); }
  public static LostDungeonMissionTable GetRootAsLostDungeonMissionTable(ByteBuffer _bb, LostDungeonMissionTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public LostDungeonMissionTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public string Desc { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDescBytes() { return __p.__vector_as_arraysegment(8); }
  public string Explain { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetExplainBytes() { return __p.__vector_as_arraysegment(10); }
  public string Icon { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetIconBytes() { return __p.__vector_as_arraysegment(12); }
  public int Type { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Value { get { int o = __p.__offset(16); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetValueBytes() { return __p.__vector_as_arraysegment(16); }
  public int AcceptLimit { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int FrontCond1 { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Score { get { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Grade { get { int o = __p.__offset(24); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<LostDungeonMissionTable> CreateLostDungeonMissionTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      StringOffset DescOffset = default(StringOffset),
      StringOffset ExplainOffset = default(StringOffset),
      StringOffset IconOffset = default(StringOffset),
      int Type = 0,
      StringOffset ValueOffset = default(StringOffset),
      int AcceptLimit = 0,
      int FrontCond1 = 0,
      int Score = 0,
      int Grade = 0) {
    builder.StartObject(11);
    LostDungeonMissionTable.AddGrade(builder, Grade);
    LostDungeonMissionTable.AddScore(builder, Score);
    LostDungeonMissionTable.AddFrontCond1(builder, FrontCond1);
    LostDungeonMissionTable.AddAcceptLimit(builder, AcceptLimit);
    LostDungeonMissionTable.AddValue(builder, ValueOffset);
    LostDungeonMissionTable.AddType(builder, Type);
    LostDungeonMissionTable.AddIcon(builder, IconOffset);
    LostDungeonMissionTable.AddExplain(builder, ExplainOffset);
    LostDungeonMissionTable.AddDesc(builder, DescOffset);
    LostDungeonMissionTable.AddName(builder, NameOffset);
    LostDungeonMissionTable.AddID(builder, ID);
    return LostDungeonMissionTable.EndLostDungeonMissionTable(builder);
  }

  public static void StartLostDungeonMissionTable(FlatBufferBuilder builder) { builder.StartObject(11); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset DescOffset) { builder.AddOffset(2, DescOffset.Value, 0); }
  public static void AddExplain(FlatBufferBuilder builder, StringOffset ExplainOffset) { builder.AddOffset(3, ExplainOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset IconOffset) { builder.AddOffset(4, IconOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, int Type) { builder.AddInt(5, Type, 0); }
  public static void AddValue(FlatBufferBuilder builder, StringOffset ValueOffset) { builder.AddOffset(6, ValueOffset.Value, 0); }
  public static void AddAcceptLimit(FlatBufferBuilder builder, int AcceptLimit) { builder.AddInt(7, AcceptLimit, 0); }
  public static void AddFrontCond1(FlatBufferBuilder builder, int FrontCond1) { builder.AddInt(8, FrontCond1, 0); }
  public static void AddScore(FlatBufferBuilder builder, int Score) { builder.AddInt(9, Score, 0); }
  public static void AddGrade(FlatBufferBuilder builder, int Grade) { builder.AddInt(10, Grade, 0); }
  public static Offset<LostDungeonMissionTable> EndLostDungeonMissionTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostDungeonMissionTable>(o);
  }
  public static void FinishLostDungeonMissionTableBuffer(FlatBufferBuilder builder, Offset<LostDungeonMissionTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
