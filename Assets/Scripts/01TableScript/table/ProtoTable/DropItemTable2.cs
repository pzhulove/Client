// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class DropItemTable2 : IFlatbufferObject
{
public enum eDropRandomFashionType : int
{
 DRFT_RANDOM_INVALID = 0,
 DRFT_RANDOM_EXTRACTION = 1,
 DRFT_RANDOM_COMPLETELY = 2,
};

public enum eCrypt : int
{
 code = 845765760,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static DropItemTable2 GetRootAsDropItemTable2(ByteBuffer _bb) { return GetRootAsDropItemTable2(_bb, new DropItemTable2()); }
  public static DropItemTable2 GetRootAsDropItemTable2(ByteBuffer _bb, DropItemTable2 obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public DropItemTable2 __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int GroupID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ChooseNumSetArray(int j) { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int ChooseNumSetLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetChooseNumSetBytes() { return __p.__vector_as_arraysegment(8); }
 private FlatBufferArray<int> ChooseNumSetValue;
 public FlatBufferArray<int>  ChooseNumSet
 {
  get{
  if (ChooseNumSetValue == null)
  {
    ChooseNumSetValue = new FlatBufferArray<int>(this.ChooseNumSetArray, this.ChooseNumSetLength);
  }
  return ChooseNumSetValue;}
 }
  public int NumProbSetArray(int j) { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int NumProbSetLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetNumProbSetBytes() { return __p.__vector_as_arraysegment(10); }
 private FlatBufferArray<int> NumProbSetValue;
 public FlatBufferArray<int>  NumProbSet
 {
  get{
  if (NumProbSetValue == null)
  {
    NumProbSetValue = new FlatBufferArray<int>(this.NumProbSetArray, this.NumProbSetLength);
  }
  return NumProbSetValue;}
 }
  public ProtoTable.DropItemTable2.eDropRandomFashionType DropRandomFashionType { get { int o = __p.__offset(12); return o != 0 ? (ProtoTable.DropItemTable2.eDropRandomFashionType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.DropItemTable2.eDropRandomFashionType.DRFT_RANDOM_INVALID; } }
  public int DataType { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ItemID { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ItemProb { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ItemNumArray(int j) { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int ItemNumLength { get { int o = __p.__offset(20); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetItemNumBytes() { return __p.__vector_as_arraysegment(20); }
 private FlatBufferArray<int> ItemNumValue;
 public FlatBufferArray<int>  ItemNum
 {
  get{
  if (ItemNumValue == null)
  {
    ItemNumValue = new FlatBufferArray<int>(this.ItemNumArray, this.ItemNumLength);
  }
  return ItemNumValue;}
 }
  public int IsRareControl { get { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int TaskID { get { int o = __p.__offset(24); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int OccuAdditionArray(int j) { int o = __p.__offset(26); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int OccuAdditionLength { get { int o = __p.__offset(26); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetOccuAdditionBytes() { return __p.__vector_as_arraysegment(26); }
 private FlatBufferArray<int> OccuAdditionValue;
 public FlatBufferArray<int>  OccuAddition
 {
  get{
  if (OccuAdditionValue == null)
  {
    OccuAdditionValue = new FlatBufferArray<int>(this.OccuAdditionArray, this.OccuAdditionLength);
  }
  return OccuAdditionValue;}
 }
  public int AdditionProb { get { int o = __p.__offset(28); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DropItemType { get { int o = __p.__offset(30); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DropNotice { get { int o = __p.__offset(32); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Text { get { int o = __p.__offset(34); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetTextBytes() { return __p.__vector_as_arraysegment(34); }
  public int ActivityID { get { int o = __p.__offset(36); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Mark { get { int o = __p.__offset(38); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetMarkBytes() { return __p.__vector_as_arraysegment(38); }
  public string Vip { get { int o = __p.__offset(40); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetVipBytes() { return __p.__vector_as_arraysegment(40); }
  public int VipDropLimitId { get { int o = __p.__offset(42); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MonthCard { get { int o = __p.__offset(44); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<DropItemTable2> CreateDropItemTable2(FlatBufferBuilder builder,
      int ID = 0,
      int GroupID = 0,
      VectorOffset ChooseNumSetOffset = default(VectorOffset),
      VectorOffset NumProbSetOffset = default(VectorOffset),
      ProtoTable.DropItemTable2.eDropRandomFashionType DropRandomFashionType = ProtoTable.DropItemTable2.eDropRandomFashionType.DRFT_RANDOM_INVALID,
      int DataType = 0,
      int ItemID = 0,
      int ItemProb = 0,
      VectorOffset ItemNumOffset = default(VectorOffset),
      int IsRareControl = 0,
      int TaskID = 0,
      VectorOffset OccuAdditionOffset = default(VectorOffset),
      int AdditionProb = 0,
      int DropItemType = 0,
      int DropNotice = 0,
      StringOffset TextOffset = default(StringOffset),
      int ActivityID = 0,
      StringOffset MarkOffset = default(StringOffset),
      StringOffset VipOffset = default(StringOffset),
      int VipDropLimitId = 0,
      int MonthCard = 0) {
    builder.StartObject(21);
    DropItemTable2.AddMonthCard(builder, MonthCard);
    DropItemTable2.AddVipDropLimitId(builder, VipDropLimitId);
    DropItemTable2.AddVip(builder, VipOffset);
    DropItemTable2.AddMark(builder, MarkOffset);
    DropItemTable2.AddActivityID(builder, ActivityID);
    DropItemTable2.AddText(builder, TextOffset);
    DropItemTable2.AddDropNotice(builder, DropNotice);
    DropItemTable2.AddDropItemType(builder, DropItemType);
    DropItemTable2.AddAdditionProb(builder, AdditionProb);
    DropItemTable2.AddOccuAddition(builder, OccuAdditionOffset);
    DropItemTable2.AddTaskID(builder, TaskID);
    DropItemTable2.AddIsRareControl(builder, IsRareControl);
    DropItemTable2.AddItemNum(builder, ItemNumOffset);
    DropItemTable2.AddItemProb(builder, ItemProb);
    DropItemTable2.AddItemID(builder, ItemID);
    DropItemTable2.AddDataType(builder, DataType);
    DropItemTable2.AddDropRandomFashionType(builder, DropRandomFashionType);
    DropItemTable2.AddNumProbSet(builder, NumProbSetOffset);
    DropItemTable2.AddChooseNumSet(builder, ChooseNumSetOffset);
    DropItemTable2.AddGroupID(builder, GroupID);
    DropItemTable2.AddID(builder, ID);
    return DropItemTable2.EndDropItemTable2(builder);
  }

  public static void StartDropItemTable2(FlatBufferBuilder builder) { builder.StartObject(21); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddGroupID(FlatBufferBuilder builder, int GroupID) { builder.AddInt(1, GroupID, 0); }
  public static void AddChooseNumSet(FlatBufferBuilder builder, VectorOffset ChooseNumSetOffset) { builder.AddOffset(2, ChooseNumSetOffset.Value, 0); }
  public static VectorOffset CreateChooseNumSetVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartChooseNumSetVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddNumProbSet(FlatBufferBuilder builder, VectorOffset NumProbSetOffset) { builder.AddOffset(3, NumProbSetOffset.Value, 0); }
  public static VectorOffset CreateNumProbSetVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartNumProbSetVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDropRandomFashionType(FlatBufferBuilder builder, ProtoTable.DropItemTable2.eDropRandomFashionType DropRandomFashionType) { builder.AddInt(4, (int)DropRandomFashionType, 0); }
  public static void AddDataType(FlatBufferBuilder builder, int DataType) { builder.AddInt(5, DataType, 0); }
  public static void AddItemID(FlatBufferBuilder builder, int ItemID) { builder.AddInt(6, ItemID, 0); }
  public static void AddItemProb(FlatBufferBuilder builder, int ItemProb) { builder.AddInt(7, ItemProb, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, VectorOffset ItemNumOffset) { builder.AddOffset(8, ItemNumOffset.Value, 0); }
  public static VectorOffset CreateItemNumVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartItemNumVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddIsRareControl(FlatBufferBuilder builder, int IsRareControl) { builder.AddInt(9, IsRareControl, 0); }
  public static void AddTaskID(FlatBufferBuilder builder, int TaskID) { builder.AddInt(10, TaskID, 0); }
  public static void AddOccuAddition(FlatBufferBuilder builder, VectorOffset OccuAdditionOffset) { builder.AddOffset(11, OccuAdditionOffset.Value, 0); }
  public static VectorOffset CreateOccuAdditionVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartOccuAdditionVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAdditionProb(FlatBufferBuilder builder, int AdditionProb) { builder.AddInt(12, AdditionProb, 0); }
  public static void AddDropItemType(FlatBufferBuilder builder, int DropItemType) { builder.AddInt(13, DropItemType, 0); }
  public static void AddDropNotice(FlatBufferBuilder builder, int DropNotice) { builder.AddInt(14, DropNotice, 0); }
  public static void AddText(FlatBufferBuilder builder, StringOffset TextOffset) { builder.AddOffset(15, TextOffset.Value, 0); }
  public static void AddActivityID(FlatBufferBuilder builder, int ActivityID) { builder.AddInt(16, ActivityID, 0); }
  public static void AddMark(FlatBufferBuilder builder, StringOffset MarkOffset) { builder.AddOffset(17, MarkOffset.Value, 0); }
  public static void AddVip(FlatBufferBuilder builder, StringOffset VipOffset) { builder.AddOffset(18, VipOffset.Value, 0); }
  public static void AddVipDropLimitId(FlatBufferBuilder builder, int VipDropLimitId) { builder.AddInt(19, VipDropLimitId, 0); }
  public static void AddMonthCard(FlatBufferBuilder builder, int MonthCard) { builder.AddInt(20, MonthCard, 0); }
  public static Offset<DropItemTable2> EndDropItemTable2(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DropItemTable2>(o);
  }
  public static void FinishDropItemTable2Buffer(FlatBufferBuilder builder, Offset<DropItemTable2> offset) { builder.Finish(offset.Value); }
};


}


#endif
