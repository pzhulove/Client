// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ActiveMainTable : IFlatbufferObject
{
public enum eActivityType : int
{
 None = 0,
 ExchangeActivity = 1,
 KillBossActivity = 2,
 QuestActivity = 3,
};

public enum eCrypt : int
{
 code = 1839457437,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ActiveMainTable GetRootAsActiveMainTable(ByteBuffer _bb) { return GetRootAsActiveMainTable(_bb, new ActiveMainTable()); }
  public static ActiveMainTable GetRootAsActiveMainTable(ByteBuffer _bb, ActiveMainTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ActiveMainTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SortID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public ProtoTable.ActiveMainTable.eActivityType ActivityType { get { int o = __p.__offset(8); return o != 0 ? (ProtoTable.ActiveMainTable.eActivityType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.ActiveMainTable.eActivityType.None; } }
  public string Name { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(10); }
  public string RedPointPath { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetRedPointPathBytes() { return __p.__vector_as_arraysegment(12); }
  public string RedPointLocalPath { get { int o = __p.__offset(14); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetRedPointLocalPathBytes() { return __p.__vector_as_arraysegment(14); }
  public string UpdateMainKeys { get { int o = __p.__offset(16); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetUpdateMainKeysBytes() { return __p.__vector_as_arraysegment(16); }
  public string TabInitDesc { get { int o = __p.__offset(18); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetTabInitDescBytes() { return __p.__vector_as_arraysegment(18); }
  public string FrameTemplate { get { int o = __p.__offset(20); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetFrameTemplateBytes() { return __p.__vector_as_arraysegment(20); }
  public string PurDesc { get { int o = __p.__offset(22); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetPurDescBytes() { return __p.__vector_as_arraysegment(22); }
  public string ParticularDesc { get { int o = __p.__offset(24); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetParticularDescBytes() { return __p.__vector_as_arraysegment(24); }
  public string Desc { get { int o = __p.__offset(26); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDescBytes() { return __p.__vector_as_arraysegment(26); }
  public string prefabDesc { get { int o = __p.__offset(28); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetPrefabDescBytes() { return __p.__vector_as_arraysegment(28); }
  public string awardparent { get { int o = __p.__offset(30); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetAwardparentBytes() { return __p.__vector_as_arraysegment(30); }
  public string templateName { get { int o = __p.__offset(32); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetTemplateNameBytes() { return __p.__vector_as_arraysegment(32); }
  public int bUseTemplate { get { int o = __p.__offset(34); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string MainStatusDesc { get { int o = __p.__offset(36); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetMainStatusDescBytes() { return __p.__vector_as_arraysegment(36); }
  public string PrefabStatusDesc { get { int o = __p.__offset(38); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetPrefabStatusDescBytes() { return __p.__vector_as_arraysegment(38); }
  public int PrefabStatusShowDescArray(int j) { int o = __p.__offset(40); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int PrefabStatusShowDescLength { get { int o = __p.__offset(40); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetPrefabStatusShowDescBytes() { return __p.__vector_as_arraysegment(40); }
 private FlatBufferArray<int> PrefabStatusShowDescValue;
 public FlatBufferArray<int>  PrefabStatusShowDesc
 {
  get{
  if (PrefabStatusShowDescValue == null)
  {
    PrefabStatusShowDescValue = new FlatBufferArray<int>(this.PrefabStatusShowDescArray, this.PrefabStatusShowDescLength);
  }
  return PrefabStatusShowDescValue;}
 }
  public string MainInitDesc { get { int o = __p.__offset(42); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetMainInitDescBytes() { return __p.__vector_as_arraysegment(42); }
  public string FunctionParse { get { int o = __p.__offset(44); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetFunctionParseBytes() { return __p.__vector_as_arraysegment(44); }
  public string ActiveFrame { get { int o = __p.__offset(46); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetActiveFrameBytes() { return __p.__vector_as_arraysegment(46); }
  public string ActiveFrameTabPath { get { int o = __p.__offset(48); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetActiveFrameTabPathBytes() { return __p.__vector_as_arraysegment(48); }
  public int ActiveTypeID { get { int o = __p.__offset(50); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string BossId { get { int o = __p.__offset(52); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetBossIdBytes() { return __p.__vector_as_arraysegment(52); }
  public string BgPath { get { int o = __p.__offset(54); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetBgPathBytes() { return __p.__vector_as_arraysegment(54); }
  public string TownBtIconPath { get { int o = __p.__offset(56); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetTownBtIconPathBytes() { return __p.__vector_as_arraysegment(56); }
  public string TownBtText { get { int o = __p.__offset(58); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetTownBtTextBytes() { return __p.__vector_as_arraysegment(58); }

  public static Offset<ActiveMainTable> CreateActiveMainTable(FlatBufferBuilder builder,
      int ID = 0,
      int SortID = 0,
      ProtoTable.ActiveMainTable.eActivityType ActivityType = ProtoTable.ActiveMainTable.eActivityType.None,
      StringOffset NameOffset = default(StringOffset),
      StringOffset RedPointPathOffset = default(StringOffset),
      StringOffset RedPointLocalPathOffset = default(StringOffset),
      StringOffset UpdateMainKeysOffset = default(StringOffset),
      StringOffset TabInitDescOffset = default(StringOffset),
      StringOffset FrameTemplateOffset = default(StringOffset),
      StringOffset PurDescOffset = default(StringOffset),
      StringOffset ParticularDescOffset = default(StringOffset),
      StringOffset DescOffset = default(StringOffset),
      StringOffset prefabDescOffset = default(StringOffset),
      StringOffset awardparentOffset = default(StringOffset),
      StringOffset templateNameOffset = default(StringOffset),
      int bUseTemplate = 0,
      StringOffset MainStatusDescOffset = default(StringOffset),
      StringOffset PrefabStatusDescOffset = default(StringOffset),
      VectorOffset PrefabStatusShowDescOffset = default(VectorOffset),
      StringOffset MainInitDescOffset = default(StringOffset),
      StringOffset FunctionParseOffset = default(StringOffset),
      StringOffset ActiveFrameOffset = default(StringOffset),
      StringOffset ActiveFrameTabPathOffset = default(StringOffset),
      int ActiveTypeID = 0,
      StringOffset BossIdOffset = default(StringOffset),
      StringOffset BgPathOffset = default(StringOffset),
      StringOffset TownBtIconPathOffset = default(StringOffset),
      StringOffset TownBtTextOffset = default(StringOffset)) {
    builder.StartObject(28);
    ActiveMainTable.AddTownBtText(builder, TownBtTextOffset);
    ActiveMainTable.AddTownBtIconPath(builder, TownBtIconPathOffset);
    ActiveMainTable.AddBgPath(builder, BgPathOffset);
    ActiveMainTable.AddBossId(builder, BossIdOffset);
    ActiveMainTable.AddActiveTypeID(builder, ActiveTypeID);
    ActiveMainTable.AddActiveFrameTabPath(builder, ActiveFrameTabPathOffset);
    ActiveMainTable.AddActiveFrame(builder, ActiveFrameOffset);
    ActiveMainTable.AddFunctionParse(builder, FunctionParseOffset);
    ActiveMainTable.AddMainInitDesc(builder, MainInitDescOffset);
    ActiveMainTable.AddPrefabStatusShowDesc(builder, PrefabStatusShowDescOffset);
    ActiveMainTable.AddPrefabStatusDesc(builder, PrefabStatusDescOffset);
    ActiveMainTable.AddMainStatusDesc(builder, MainStatusDescOffset);
    ActiveMainTable.AddBUseTemplate(builder, bUseTemplate);
    ActiveMainTable.AddTemplateName(builder, templateNameOffset);
    ActiveMainTable.AddAwardparent(builder, awardparentOffset);
    ActiveMainTable.AddPrefabDesc(builder, prefabDescOffset);
    ActiveMainTable.AddDesc(builder, DescOffset);
    ActiveMainTable.AddParticularDesc(builder, ParticularDescOffset);
    ActiveMainTable.AddPurDesc(builder, PurDescOffset);
    ActiveMainTable.AddFrameTemplate(builder, FrameTemplateOffset);
    ActiveMainTable.AddTabInitDesc(builder, TabInitDescOffset);
    ActiveMainTable.AddUpdateMainKeys(builder, UpdateMainKeysOffset);
    ActiveMainTable.AddRedPointLocalPath(builder, RedPointLocalPathOffset);
    ActiveMainTable.AddRedPointPath(builder, RedPointPathOffset);
    ActiveMainTable.AddName(builder, NameOffset);
    ActiveMainTable.AddActivityType(builder, ActivityType);
    ActiveMainTable.AddSortID(builder, SortID);
    ActiveMainTable.AddID(builder, ID);
    return ActiveMainTable.EndActiveMainTable(builder);
  }

  public static void StartActiveMainTable(FlatBufferBuilder builder) { builder.StartObject(28); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddSortID(FlatBufferBuilder builder, int SortID) { builder.AddInt(1, SortID, 0); }
  public static void AddActivityType(FlatBufferBuilder builder, ProtoTable.ActiveMainTable.eActivityType ActivityType) { builder.AddInt(2, (int)ActivityType, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(3, NameOffset.Value, 0); }
  public static void AddRedPointPath(FlatBufferBuilder builder, StringOffset RedPointPathOffset) { builder.AddOffset(4, RedPointPathOffset.Value, 0); }
  public static void AddRedPointLocalPath(FlatBufferBuilder builder, StringOffset RedPointLocalPathOffset) { builder.AddOffset(5, RedPointLocalPathOffset.Value, 0); }
  public static void AddUpdateMainKeys(FlatBufferBuilder builder, StringOffset UpdateMainKeysOffset) { builder.AddOffset(6, UpdateMainKeysOffset.Value, 0); }
  public static void AddTabInitDesc(FlatBufferBuilder builder, StringOffset TabInitDescOffset) { builder.AddOffset(7, TabInitDescOffset.Value, 0); }
  public static void AddFrameTemplate(FlatBufferBuilder builder, StringOffset FrameTemplateOffset) { builder.AddOffset(8, FrameTemplateOffset.Value, 0); }
  public static void AddPurDesc(FlatBufferBuilder builder, StringOffset PurDescOffset) { builder.AddOffset(9, PurDescOffset.Value, 0); }
  public static void AddParticularDesc(FlatBufferBuilder builder, StringOffset ParticularDescOffset) { builder.AddOffset(10, ParticularDescOffset.Value, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset DescOffset) { builder.AddOffset(11, DescOffset.Value, 0); }
  public static void AddPrefabDesc(FlatBufferBuilder builder, StringOffset prefabDescOffset) { builder.AddOffset(12, prefabDescOffset.Value, 0); }
  public static void AddAwardparent(FlatBufferBuilder builder, StringOffset awardparentOffset) { builder.AddOffset(13, awardparentOffset.Value, 0); }
  public static void AddTemplateName(FlatBufferBuilder builder, StringOffset templateNameOffset) { builder.AddOffset(14, templateNameOffset.Value, 0); }
  public static void AddBUseTemplate(FlatBufferBuilder builder, int bUseTemplate) { builder.AddInt(15, bUseTemplate, 0); }
  public static void AddMainStatusDesc(FlatBufferBuilder builder, StringOffset MainStatusDescOffset) { builder.AddOffset(16, MainStatusDescOffset.Value, 0); }
  public static void AddPrefabStatusDesc(FlatBufferBuilder builder, StringOffset PrefabStatusDescOffset) { builder.AddOffset(17, PrefabStatusDescOffset.Value, 0); }
  public static void AddPrefabStatusShowDesc(FlatBufferBuilder builder, VectorOffset PrefabStatusShowDescOffset) { builder.AddOffset(18, PrefabStatusShowDescOffset.Value, 0); }
  public static VectorOffset CreatePrefabStatusShowDescVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartPrefabStatusShowDescVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMainInitDesc(FlatBufferBuilder builder, StringOffset MainInitDescOffset) { builder.AddOffset(19, MainInitDescOffset.Value, 0); }
  public static void AddFunctionParse(FlatBufferBuilder builder, StringOffset FunctionParseOffset) { builder.AddOffset(20, FunctionParseOffset.Value, 0); }
  public static void AddActiveFrame(FlatBufferBuilder builder, StringOffset ActiveFrameOffset) { builder.AddOffset(21, ActiveFrameOffset.Value, 0); }
  public static void AddActiveFrameTabPath(FlatBufferBuilder builder, StringOffset ActiveFrameTabPathOffset) { builder.AddOffset(22, ActiveFrameTabPathOffset.Value, 0); }
  public static void AddActiveTypeID(FlatBufferBuilder builder, int ActiveTypeID) { builder.AddInt(23, ActiveTypeID, 0); }
  public static void AddBossId(FlatBufferBuilder builder, StringOffset BossIdOffset) { builder.AddOffset(24, BossIdOffset.Value, 0); }
  public static void AddBgPath(FlatBufferBuilder builder, StringOffset BgPathOffset) { builder.AddOffset(25, BgPathOffset.Value, 0); }
  public static void AddTownBtIconPath(FlatBufferBuilder builder, StringOffset TownBtIconPathOffset) { builder.AddOffset(26, TownBtIconPathOffset.Value, 0); }
  public static void AddTownBtText(FlatBufferBuilder builder, StringOffset TownBtTextOffset) { builder.AddOffset(27, TownBtTextOffset.Value, 0); }
  public static Offset<ActiveMainTable> EndActiveMainTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ActiveMainTable>(o);
  }
  public static void FinishActiveMainTableBuffer(FlatBufferBuilder builder, Offset<ActiveMainTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
