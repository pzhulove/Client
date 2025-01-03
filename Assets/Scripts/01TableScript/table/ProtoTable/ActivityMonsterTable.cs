// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ActivityMonsterTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 322105103,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ActivityMonsterTable GetRootAsActivityMonsterTable(ByteBuffer _bb) { return GetRootAsActivityMonsterTable(_bb, new ActivityMonsterTable()); }
  public static ActivityMonsterTable GetRootAsActivityMonsterTable(ByteBuffer _bb, ActivityMonsterTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ActivityMonsterTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public string StartDate { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetStartDateBytes() { return __p.__vector_as_arraysegment(8); }
  public string EndDate { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetEndDateBytes() { return __p.__vector_as_arraysegment(10); }
  public string StartTime { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetStartTimeBytes() { return __p.__vector_as_arraysegment(12); }
  public int PerRollMonsterNum { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int PerRollDurningSec { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int PointType { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int GroupID { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Prob { get { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string DropIDsArray(int j) { int o = __p.__offset(24); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int DropIDsLength { get { int o = __p.__offset(24); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> DropIDsValue;
 public FlatBufferArray<string>  DropIDs
 {
  get{
  if (DropIDsValue == null)
  {
    DropIDsValue = new FlatBufferArray<string>(this.DropIDsArray, this.DropIDsLength);
  }
  return DropIDsValue;}
 }
  public string DropItems { get { int o = __p.__offset(26); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDropItemsBytes() { return __p.__vector_as_arraysegment(26); }
  public int StartNoticeArray(int j) { int o = __p.__offset(28); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int StartNoticeLength { get { int o = __p.__offset(28); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetStartNoticeBytes() { return __p.__vector_as_arraysegment(28); }
 private FlatBufferArray<int> StartNoticeValue;
 public FlatBufferArray<int>  StartNotice
 {
  get{
  if (StartNoticeValue == null)
  {
    StartNoticeValue = new FlatBufferArray<int>(this.StartNoticeArray, this.StartNoticeLength);
  }
  return StartNoticeValue;}
 }
  public int KillNoticeArray(int j) { int o = __p.__offset(30); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int KillNoticeLength { get { int o = __p.__offset(30); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetKillNoticeBytes() { return __p.__vector_as_arraysegment(30); }
 private FlatBufferArray<int> KillNoticeValue;
 public FlatBufferArray<int>  KillNotice
 {
  get{
  if (KillNoticeValue == null)
  {
    KillNoticeValue = new FlatBufferArray<int>(this.KillNoticeArray, this.KillNoticeLength);
  }
  return KillNoticeValue;}
 }
  public int ClearNoticeArray(int j) { int o = __p.__offset(32); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int ClearNoticeLength { get { int o = __p.__offset(32); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetClearNoticeBytes() { return __p.__vector_as_arraysegment(32); }
 private FlatBufferArray<int> ClearNoticeValue;
 public FlatBufferArray<int>  ClearNotice
 {
  get{
  if (ClearNoticeValue == null)
  {
    ClearNoticeValue = new FlatBufferArray<int>(this.ClearNoticeArray, this.ClearNoticeLength);
  }
  return ClearNoticeValue;}
 }
  public int OverNoticeArray(int j) { int o = __p.__offset(34); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int OverNoticeLength { get { int o = __p.__offset(34); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetOverNoticeBytes() { return __p.__vector_as_arraysegment(34); }
 private FlatBufferArray<int> OverNoticeValue;
 public FlatBufferArray<int>  OverNotice
 {
  get{
  if (OverNoticeValue == null)
  {
    OverNoticeValue = new FlatBufferArray<int>(this.OverNoticeArray, this.OverNoticeLength);
  }
  return OverNoticeValue;}
 }
  public int NeedDungeonLevelArray(int j) { int o = __p.__offset(36); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int NeedDungeonLevelLength { get { int o = __p.__offset(36); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetNeedDungeonLevelBytes() { return __p.__vector_as_arraysegment(36); }
 private FlatBufferArray<int> NeedDungeonLevelValue;
 public FlatBufferArray<int>  NeedDungeonLevel
 {
  get{
  if (NeedDungeonLevelValue == null)
  {
    NeedDungeonLevelValue = new FlatBufferArray<int>(this.NeedDungeonLevelArray, this.NeedDungeonLevelLength);
  }
  return NeedDungeonLevelValue;}
 }

  public static Offset<ActivityMonsterTable> CreateActivityMonsterTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      StringOffset StartDateOffset = default(StringOffset),
      StringOffset EndDateOffset = default(StringOffset),
      StringOffset StartTimeOffset = default(StringOffset),
      int PerRollMonsterNum = 0,
      int PerRollDurningSec = 0,
      int PointType = 0,
      int GroupID = 0,
      int Prob = 0,
      VectorOffset DropIDsOffset = default(VectorOffset),
      StringOffset DropItemsOffset = default(StringOffset),
      VectorOffset StartNoticeOffset = default(VectorOffset),
      VectorOffset KillNoticeOffset = default(VectorOffset),
      VectorOffset ClearNoticeOffset = default(VectorOffset),
      VectorOffset OverNoticeOffset = default(VectorOffset),
      VectorOffset NeedDungeonLevelOffset = default(VectorOffset)) {
    builder.StartObject(17);
    ActivityMonsterTable.AddNeedDungeonLevel(builder, NeedDungeonLevelOffset);
    ActivityMonsterTable.AddOverNotice(builder, OverNoticeOffset);
    ActivityMonsterTable.AddClearNotice(builder, ClearNoticeOffset);
    ActivityMonsterTable.AddKillNotice(builder, KillNoticeOffset);
    ActivityMonsterTable.AddStartNotice(builder, StartNoticeOffset);
    ActivityMonsterTable.AddDropItems(builder, DropItemsOffset);
    ActivityMonsterTable.AddDropIDs(builder, DropIDsOffset);
    ActivityMonsterTable.AddProb(builder, Prob);
    ActivityMonsterTable.AddGroupID(builder, GroupID);
    ActivityMonsterTable.AddPointType(builder, PointType);
    ActivityMonsterTable.AddPerRollDurningSec(builder, PerRollDurningSec);
    ActivityMonsterTable.AddPerRollMonsterNum(builder, PerRollMonsterNum);
    ActivityMonsterTable.AddStartTime(builder, StartTimeOffset);
    ActivityMonsterTable.AddEndDate(builder, EndDateOffset);
    ActivityMonsterTable.AddStartDate(builder, StartDateOffset);
    ActivityMonsterTable.AddName(builder, NameOffset);
    ActivityMonsterTable.AddID(builder, ID);
    return ActivityMonsterTable.EndActivityMonsterTable(builder);
  }

  public static void StartActivityMonsterTable(FlatBufferBuilder builder) { builder.StartObject(17); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddStartDate(FlatBufferBuilder builder, StringOffset StartDateOffset) { builder.AddOffset(2, StartDateOffset.Value, 0); }
  public static void AddEndDate(FlatBufferBuilder builder, StringOffset EndDateOffset) { builder.AddOffset(3, EndDateOffset.Value, 0); }
  public static void AddStartTime(FlatBufferBuilder builder, StringOffset StartTimeOffset) { builder.AddOffset(4, StartTimeOffset.Value, 0); }
  public static void AddPerRollMonsterNum(FlatBufferBuilder builder, int PerRollMonsterNum) { builder.AddInt(5, PerRollMonsterNum, 0); }
  public static void AddPerRollDurningSec(FlatBufferBuilder builder, int PerRollDurningSec) { builder.AddInt(6, PerRollDurningSec, 0); }
  public static void AddPointType(FlatBufferBuilder builder, int PointType) { builder.AddInt(7, PointType, 0); }
  public static void AddGroupID(FlatBufferBuilder builder, int GroupID) { builder.AddInt(8, GroupID, 0); }
  public static void AddProb(FlatBufferBuilder builder, int Prob) { builder.AddInt(9, Prob, 0); }
  public static void AddDropIDs(FlatBufferBuilder builder, VectorOffset DropIDsOffset) { builder.AddOffset(10, DropIDsOffset.Value, 0); }
  public static VectorOffset CreateDropIDsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDropIDsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDropItems(FlatBufferBuilder builder, StringOffset DropItemsOffset) { builder.AddOffset(11, DropItemsOffset.Value, 0); }
  public static void AddStartNotice(FlatBufferBuilder builder, VectorOffset StartNoticeOffset) { builder.AddOffset(12, StartNoticeOffset.Value, 0); }
  public static VectorOffset CreateStartNoticeVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartStartNoticeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddKillNotice(FlatBufferBuilder builder, VectorOffset KillNoticeOffset) { builder.AddOffset(13, KillNoticeOffset.Value, 0); }
  public static VectorOffset CreateKillNoticeVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartKillNoticeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddClearNotice(FlatBufferBuilder builder, VectorOffset ClearNoticeOffset) { builder.AddOffset(14, ClearNoticeOffset.Value, 0); }
  public static VectorOffset CreateClearNoticeVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartClearNoticeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddOverNotice(FlatBufferBuilder builder, VectorOffset OverNoticeOffset) { builder.AddOffset(15, OverNoticeOffset.Value, 0); }
  public static VectorOffset CreateOverNoticeVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartOverNoticeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddNeedDungeonLevel(FlatBufferBuilder builder, VectorOffset NeedDungeonLevelOffset) { builder.AddOffset(16, NeedDungeonLevelOffset.Value, 0); }
  public static VectorOffset CreateNeedDungeonLevelVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartNeedDungeonLevelVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ActivityMonsterTable> EndActivityMonsterTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ActivityMonsterTable>(o);
  }
  public static void FinishActivityMonsterTableBuffer(FlatBufferBuilder builder, Offset<ActivityMonsterTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

