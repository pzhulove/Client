// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class AchievementGroupSubItemTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -21208349,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static AchievementGroupSubItemTable GetRootAsAchievementGroupSubItemTable(ByteBuffer _bb) { return GetRootAsAchievementGroupSubItemTable(_bb, new AchievementGroupSubItemTable()); }
  public static AchievementGroupSubItemTable GetRootAsAchievementGroupSubItemTable(ByteBuffer _bb, AchievementGroupSubItemTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public AchievementGroupSubItemTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int sort0 { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int sort1 { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(10); }
  public int Type { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Icon { get { int o = __p.__offset(14); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetIconBytes() { return __p.__vector_as_arraysegment(14); }
  public string PointIcon { get { int o = __p.__offset(16); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetPointIconBytes() { return __p.__vector_as_arraysegment(16); }
  public string Desc { get { int o = __p.__offset(18); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDescBytes() { return __p.__vector_as_arraysegment(18); }
  public int FunctionID { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string LinkInfo { get { int o = __p.__offset(22); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLinkInfoBytes() { return __p.__vector_as_arraysegment(22); }

  public static Offset<AchievementGroupSubItemTable> CreateAchievementGroupSubItemTable(FlatBufferBuilder builder,
      int ID = 0,
      int sort0 = 0,
      int sort1 = 0,
      StringOffset NameOffset = default(StringOffset),
      int Type = 0,
      StringOffset IconOffset = default(StringOffset),
      StringOffset PointIconOffset = default(StringOffset),
      StringOffset DescOffset = default(StringOffset),
      int FunctionID = 0,
      StringOffset LinkInfoOffset = default(StringOffset)) {
    builder.StartObject(10);
    AchievementGroupSubItemTable.AddLinkInfo(builder, LinkInfoOffset);
    AchievementGroupSubItemTable.AddFunctionID(builder, FunctionID);
    AchievementGroupSubItemTable.AddDesc(builder, DescOffset);
    AchievementGroupSubItemTable.AddPointIcon(builder, PointIconOffset);
    AchievementGroupSubItemTable.AddIcon(builder, IconOffset);
    AchievementGroupSubItemTable.AddType(builder, Type);
    AchievementGroupSubItemTable.AddName(builder, NameOffset);
    AchievementGroupSubItemTable.AddSort1(builder, sort1);
    AchievementGroupSubItemTable.AddSort0(builder, sort0);
    AchievementGroupSubItemTable.AddID(builder, ID);
    return AchievementGroupSubItemTable.EndAchievementGroupSubItemTable(builder);
  }

  public static void StartAchievementGroupSubItemTable(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddSort0(FlatBufferBuilder builder, int sort0) { builder.AddInt(1, sort0, 0); }
  public static void AddSort1(FlatBufferBuilder builder, int sort1) { builder.AddInt(2, sort1, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(3, NameOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, int Type) { builder.AddInt(4, Type, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset IconOffset) { builder.AddOffset(5, IconOffset.Value, 0); }
  public static void AddPointIcon(FlatBufferBuilder builder, StringOffset PointIconOffset) { builder.AddOffset(6, PointIconOffset.Value, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset DescOffset) { builder.AddOffset(7, DescOffset.Value, 0); }
  public static void AddFunctionID(FlatBufferBuilder builder, int FunctionID) { builder.AddInt(8, FunctionID, 0); }
  public static void AddLinkInfo(FlatBufferBuilder builder, StringOffset LinkInfoOffset) { builder.AddOffset(9, LinkInfoOffset.Value, 0); }
  public static Offset<AchievementGroupSubItemTable> EndAchievementGroupSubItemTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AchievementGroupSubItemTable>(o);
  }
  public static void FinishAchievementGroupSubItemTableBuffer(FlatBufferBuilder builder, Offset<AchievementGroupSubItemTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

