// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class AchievementLevelInfoTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 182075077,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static AchievementLevelInfoTable GetRootAsAchievementLevelInfoTable(ByteBuffer _bb) { return GetRootAsAchievementLevelInfoTable(_bb, new AchievementLevelInfoTable()); }
  public static AchievementLevelInfoTable GetRootAsAchievementLevelInfoTable(ByteBuffer _bb, AchievementLevelInfoTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public AchievementLevelInfoTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public string Title { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetTitleBytes() { return __p.__vector_as_arraysegment(8); }
  public string Icon { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetIconBytes() { return __p.__vector_as_arraysegment(10); }
  public string TextIcon { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetTextIconBytes() { return __p.__vector_as_arraysegment(12); }
  public int Min { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Max { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int AwardIDArray(int j) { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int AwardIDLength { get { int o = __p.__offset(20); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetAwardIDBytes() { return __p.__vector_as_arraysegment(20); }
 private FlatBufferArray<int> AwardIDValue;
 public FlatBufferArray<int>  AwardID
 {
  get{
  if (AwardIDValue == null)
  {
    AwardIDValue = new FlatBufferArray<int>(this.AwardIDArray, this.AwardIDLength);
  }
  return AwardIDValue;}
 }
  public int AwardCountArray(int j) { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int AwardCountLength { get { int o = __p.__offset(22); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetAwardCountBytes() { return __p.__vector_as_arraysegment(22); }
 private FlatBufferArray<int> AwardCountValue;
 public FlatBufferArray<int>  AwardCount
 {
  get{
  if (AwardCountValue == null)
  {
    AwardCountValue = new FlatBufferArray<int>(this.AwardCountArray, this.AwardCountLength);
  }
  return AwardCountValue;}
 }

  public static Offset<AchievementLevelInfoTable> CreateAchievementLevelInfoTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      StringOffset TitleOffset = default(StringOffset),
      StringOffset IconOffset = default(StringOffset),
      StringOffset TextIconOffset = default(StringOffset),
      int Min = 0,
      int Max = 0,
      int Level = 0,
      VectorOffset AwardIDOffset = default(VectorOffset),
      VectorOffset AwardCountOffset = default(VectorOffset)) {
    builder.StartObject(10);
    AchievementLevelInfoTable.AddAwardCount(builder, AwardCountOffset);
    AchievementLevelInfoTable.AddAwardID(builder, AwardIDOffset);
    AchievementLevelInfoTable.AddLevel(builder, Level);
    AchievementLevelInfoTable.AddMax(builder, Max);
    AchievementLevelInfoTable.AddMin(builder, Min);
    AchievementLevelInfoTable.AddTextIcon(builder, TextIconOffset);
    AchievementLevelInfoTable.AddIcon(builder, IconOffset);
    AchievementLevelInfoTable.AddTitle(builder, TitleOffset);
    AchievementLevelInfoTable.AddName(builder, NameOffset);
    AchievementLevelInfoTable.AddID(builder, ID);
    return AchievementLevelInfoTable.EndAchievementLevelInfoTable(builder);
  }

  public static void StartAchievementLevelInfoTable(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddTitle(FlatBufferBuilder builder, StringOffset TitleOffset) { builder.AddOffset(2, TitleOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset IconOffset) { builder.AddOffset(3, IconOffset.Value, 0); }
  public static void AddTextIcon(FlatBufferBuilder builder, StringOffset TextIconOffset) { builder.AddOffset(4, TextIconOffset.Value, 0); }
  public static void AddMin(FlatBufferBuilder builder, int Min) { builder.AddInt(5, Min, 0); }
  public static void AddMax(FlatBufferBuilder builder, int Max) { builder.AddInt(6, Max, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int Level) { builder.AddInt(7, Level, 0); }
  public static void AddAwardID(FlatBufferBuilder builder, VectorOffset AwardIDOffset) { builder.AddOffset(8, AwardIDOffset.Value, 0); }
  public static VectorOffset CreateAwardIDVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartAwardIDVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAwardCount(FlatBufferBuilder builder, VectorOffset AwardCountOffset) { builder.AddOffset(9, AwardCountOffset.Value, 0); }
  public static VectorOffset CreateAwardCountVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartAwardCountVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<AchievementLevelInfoTable> EndAchievementLevelInfoTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AchievementLevelInfoTable>(o);
  }
  public static void FinishAchievementLevelInfoTableBuffer(FlatBufferBuilder builder, Offset<AchievementLevelInfoTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

