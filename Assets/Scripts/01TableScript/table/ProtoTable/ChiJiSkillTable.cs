// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ChiJiSkillTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 881621158,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ChiJiSkillTable GetRootAsChiJiSkillTable(ByteBuffer _bb) { return GetRootAsChiJiSkillTable(_bb, new ChiJiSkillTable()); }
  public static ChiJiSkillTable GetRootAsChiJiSkillTable(ByteBuffer _bb, ChiJiSkillTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ChiJiSkillTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MaxLvl { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Job { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetJobBytes() { return __p.__vector_as_arraysegment(8); }
  public string Name { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(10); }

  public static Offset<ChiJiSkillTable> CreateChiJiSkillTable(FlatBufferBuilder builder,
      int ID = 0,
      int MaxLvl = 0,
      StringOffset JobOffset = default(StringOffset),
      StringOffset NameOffset = default(StringOffset)) {
    builder.StartObject(4);
    ChiJiSkillTable.AddName(builder, NameOffset);
    ChiJiSkillTable.AddJob(builder, JobOffset);
    ChiJiSkillTable.AddMaxLvl(builder, MaxLvl);
    ChiJiSkillTable.AddID(builder, ID);
    return ChiJiSkillTable.EndChiJiSkillTable(builder);
  }

  public static void StartChiJiSkillTable(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddMaxLvl(FlatBufferBuilder builder, int MaxLvl) { builder.AddInt(1, MaxLvl, 0); }
  public static void AddJob(FlatBufferBuilder builder, StringOffset JobOffset) { builder.AddOffset(2, JobOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(3, NameOffset.Value, 0); }
  public static Offset<ChiJiSkillTable> EndChiJiSkillTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ChiJiSkillTable>(o);
  }
  public static void FinishChiJiSkillTableBuffer(FlatBufferBuilder builder, Offset<ChiJiSkillTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
