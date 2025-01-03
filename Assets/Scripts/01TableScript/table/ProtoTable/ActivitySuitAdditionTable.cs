// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ActivitySuitAdditionTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -872320888,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ActivitySuitAdditionTable GetRootAsActivitySuitAdditionTable(ByteBuffer _bb) { return GetRootAsActivitySuitAdditionTable(_bb, new ActivitySuitAdditionTable()); }
  public static ActivitySuitAdditionTable GetRootAsActivitySuitAdditionTable(ByteBuffer _bb, ActivitySuitAdditionTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ActivitySuitAdditionTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Type { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int EquipListArray(int j) { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int EquipListLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetEquipListBytes() { return __p.__vector_as_arraysegment(8); }
 private FlatBufferArray<int> EquipListValue;
 public FlatBufferArray<int>  EquipList
 {
  get{
  if (EquipListValue == null)
  {
    EquipListValue = new FlatBufferArray<int>(this.EquipListArray, this.EquipListLength);
  }
  return EquipListValue;}
 }
  public int ActivityListArray(int j) { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int ActivityListLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetActivityListBytes() { return __p.__vector_as_arraysegment(10); }
 private FlatBufferArray<int> ActivityListValue;
 public FlatBufferArray<int>  ActivityList
 {
  get{
  if (ActivityListValue == null)
  {
    ActivityListValue = new FlatBufferArray<int>(this.ActivityListArray, this.ActivityListLength);
  }
  return ActivityListValue;}
 }
  public int DoubleDropItemArray(int j) { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int DoubleDropItemLength { get { int o = __p.__offset(12); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetDoubleDropItemBytes() { return __p.__vector_as_arraysegment(12); }
 private FlatBufferArray<int> DoubleDropItemValue;
 public FlatBufferArray<int>  DoubleDropItem
 {
  get{
  if (DoubleDropItemValue == null)
  {
    DoubleDropItemValue = new FlatBufferArray<int>(this.DoubleDropItemArray, this.DoubleDropItemLength);
  }
  return DoubleDropItemValue;}
 }
  public int DoubleDropRate { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MonsterRate1 { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MonsterRate2 { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MonsterRate3 { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<ActivitySuitAdditionTable> CreateActivitySuitAdditionTable(FlatBufferBuilder builder,
      int ID = 0,
      int Type = 0,
      VectorOffset EquipListOffset = default(VectorOffset),
      VectorOffset ActivityListOffset = default(VectorOffset),
      VectorOffset DoubleDropItemOffset = default(VectorOffset),
      int DoubleDropRate = 0,
      int MonsterRate1 = 0,
      int MonsterRate2 = 0,
      int MonsterRate3 = 0) {
    builder.StartObject(9);
    ActivitySuitAdditionTable.AddMonsterRate3(builder, MonsterRate3);
    ActivitySuitAdditionTable.AddMonsterRate2(builder, MonsterRate2);
    ActivitySuitAdditionTable.AddMonsterRate1(builder, MonsterRate1);
    ActivitySuitAdditionTable.AddDoubleDropRate(builder, DoubleDropRate);
    ActivitySuitAdditionTable.AddDoubleDropItem(builder, DoubleDropItemOffset);
    ActivitySuitAdditionTable.AddActivityList(builder, ActivityListOffset);
    ActivitySuitAdditionTable.AddEquipList(builder, EquipListOffset);
    ActivitySuitAdditionTable.AddType(builder, Type);
    ActivitySuitAdditionTable.AddID(builder, ID);
    return ActivitySuitAdditionTable.EndActivitySuitAdditionTable(builder);
  }

  public static void StartActivitySuitAdditionTable(FlatBufferBuilder builder) { builder.StartObject(9); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddType(FlatBufferBuilder builder, int Type) { builder.AddInt(1, Type, 0); }
  public static void AddEquipList(FlatBufferBuilder builder, VectorOffset EquipListOffset) { builder.AddOffset(2, EquipListOffset.Value, 0); }
  public static VectorOffset CreateEquipListVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartEquipListVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddActivityList(FlatBufferBuilder builder, VectorOffset ActivityListOffset) { builder.AddOffset(3, ActivityListOffset.Value, 0); }
  public static VectorOffset CreateActivityListVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartActivityListVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDoubleDropItem(FlatBufferBuilder builder, VectorOffset DoubleDropItemOffset) { builder.AddOffset(4, DoubleDropItemOffset.Value, 0); }
  public static VectorOffset CreateDoubleDropItemVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartDoubleDropItemVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDoubleDropRate(FlatBufferBuilder builder, int DoubleDropRate) { builder.AddInt(5, DoubleDropRate, 0); }
  public static void AddMonsterRate1(FlatBufferBuilder builder, int MonsterRate1) { builder.AddInt(6, MonsterRate1, 0); }
  public static void AddMonsterRate2(FlatBufferBuilder builder, int MonsterRate2) { builder.AddInt(7, MonsterRate2, 0); }
  public static void AddMonsterRate3(FlatBufferBuilder builder, int MonsterRate3) { builder.AddInt(8, MonsterRate3, 0); }
  public static Offset<ActivitySuitAdditionTable> EndActivitySuitAdditionTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ActivitySuitAdditionTable>(o);
  }
  public static void FinishActivitySuitAdditionTableBuffer(FlatBufferBuilder builder, Offset<ActivitySuitAdditionTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

