// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class EquipBaseScoreModTable : IFlatbufferObject
{
public enum eItemSubType : int
{
 ST_NONE = 0,
 WEAPON = 1,
 SHOULDER = 2,
 CHEST = 3,
 BELT = 4,
 LEG = 5,
 BOOT = 6,
 RING = 7,
 NECKLASE = 8,
 BRACELET = 9,
 ST_ASSIST_EQUIP = 99,
 ST_MAGICSTONE_EQUIP = 100,
 ST_EARRINGS_EQUIP = 101,
 // marked by ckm
 ST_BXY_EQUIP = 199,
};

public enum eItemQuality : int
{
 CL_NONE = 0,
 WHITE = 1,
 BLUE = 2,
 PURPLE = 3,
 GREEN = 4,
 PINK = 5,
 YELLOW = 6,
};

public enum eCrypt : int
{
 code = 669918333,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static EquipBaseScoreModTable GetRootAsEquipBaseScoreModTable(ByteBuffer _bb) { return GetRootAsEquipBaseScoreModTable(_bb, new EquipBaseScoreModTable()); }
  public static EquipBaseScoreModTable GetRootAsEquipBaseScoreModTable(ByteBuffer _bb, EquipBaseScoreModTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public EquipBaseScoreModTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public ProtoTable.EquipBaseScoreModTable.eItemSubType ItemSubType { get { int o = __p.__offset(6); return o != 0 ? (ProtoTable.EquipBaseScoreModTable.eItemSubType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.EquipBaseScoreModTable.eItemSubType.ST_NONE; } }
  public ProtoTable.EquipBaseScoreModTable.eItemQuality ItemQuality { get { int o = __p.__offset(8); return o != 0 ? (ProtoTable.EquipBaseScoreModTable.eItemQuality)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.EquipBaseScoreModTable.eItemQuality.CL_NONE; } }
  public int AttrMod1 { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int AttrMod2 { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int PowerMod1 { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int PowerMod2 { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int PowerMod3 { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DefMod1 { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DefMod2 { get { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int StrenthQualityMod1 { get { int o = __p.__offset(24); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int StrenthQualityMod2 { get { int o = __p.__offset(26); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int StrengthMod1Array(int j) { int o = __p.__offset(28); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int StrengthMod1Length { get { int o = __p.__offset(28); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetStrengthMod1Bytes() { return __p.__vector_as_arraysegment(28); }
 private FlatBufferArray<int> StrengthMod1Value;
 public FlatBufferArray<int>  StrengthMod1
 {
  get{
  if (StrengthMod1Value == null)
  {
    StrengthMod1Value = new FlatBufferArray<int>(this.StrengthMod1Array, this.StrengthMod1Length);
  }
  return StrengthMod1Value;}
 }
  public int StrengthMod2Array(int j) { int o = __p.__offset(30); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int StrengthMod2Length { get { int o = __p.__offset(30); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetStrengthMod2Bytes() { return __p.__vector_as_arraysegment(30); }
 private FlatBufferArray<int> StrengthMod2Value;
 public FlatBufferArray<int>  StrengthMod2
 {
  get{
  if (StrengthMod2Value == null)
  {
    StrengthMod2Value = new FlatBufferArray<int>(this.StrengthMod2Array, this.StrengthMod2Length);
  }
  return StrengthMod2Value;}
 }
  public int StrengthMod3Array(int j) { int o = __p.__offset(32); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int StrengthMod3Length { get { int o = __p.__offset(32); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetStrengthMod3Bytes() { return __p.__vector_as_arraysegment(32); }
 private FlatBufferArray<int> StrengthMod3Value;
 public FlatBufferArray<int>  StrengthMod3
 {
  get{
  if (StrengthMod3Value == null)
  {
    StrengthMod3Value = new FlatBufferArray<int>(this.StrengthMod3Array, this.StrengthMod3Length);
  }
  return StrengthMod3Value;}
 }

  public static Offset<EquipBaseScoreModTable> CreateEquipBaseScoreModTable(FlatBufferBuilder builder,
      int ID = 0,
      ProtoTable.EquipBaseScoreModTable.eItemSubType ItemSubType = ProtoTable.EquipBaseScoreModTable.eItemSubType.ST_NONE,
      ProtoTable.EquipBaseScoreModTable.eItemQuality ItemQuality = ProtoTable.EquipBaseScoreModTable.eItemQuality.CL_NONE,
      int AttrMod1 = 0,
      int AttrMod2 = 0,
      int PowerMod1 = 0,
      int PowerMod2 = 0,
      int PowerMod3 = 0,
      int DefMod1 = 0,
      int DefMod2 = 0,
      int StrenthQualityMod1 = 0,
      int StrenthQualityMod2 = 0,
      VectorOffset StrengthMod1Offset = default(VectorOffset),
      VectorOffset StrengthMod2Offset = default(VectorOffset),
      VectorOffset StrengthMod3Offset = default(VectorOffset)) {
    builder.StartObject(15);
    EquipBaseScoreModTable.AddStrengthMod3(builder, StrengthMod3Offset);
    EquipBaseScoreModTable.AddStrengthMod2(builder, StrengthMod2Offset);
    EquipBaseScoreModTable.AddStrengthMod1(builder, StrengthMod1Offset);
    EquipBaseScoreModTable.AddStrenthQualityMod2(builder, StrenthQualityMod2);
    EquipBaseScoreModTable.AddStrenthQualityMod1(builder, StrenthQualityMod1);
    EquipBaseScoreModTable.AddDefMod2(builder, DefMod2);
    EquipBaseScoreModTable.AddDefMod1(builder, DefMod1);
    EquipBaseScoreModTable.AddPowerMod3(builder, PowerMod3);
    EquipBaseScoreModTable.AddPowerMod2(builder, PowerMod2);
    EquipBaseScoreModTable.AddPowerMod1(builder, PowerMod1);
    EquipBaseScoreModTable.AddAttrMod2(builder, AttrMod2);
    EquipBaseScoreModTable.AddAttrMod1(builder, AttrMod1);
    EquipBaseScoreModTable.AddItemQuality(builder, ItemQuality);
    EquipBaseScoreModTable.AddItemSubType(builder, ItemSubType);
    EquipBaseScoreModTable.AddID(builder, ID);
    return EquipBaseScoreModTable.EndEquipBaseScoreModTable(builder);
  }

  public static void StartEquipBaseScoreModTable(FlatBufferBuilder builder) { builder.StartObject(15); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddItemSubType(FlatBufferBuilder builder, ProtoTable.EquipBaseScoreModTable.eItemSubType ItemSubType) { builder.AddInt(1, (int)ItemSubType, 0); }
  public static void AddItemQuality(FlatBufferBuilder builder, ProtoTable.EquipBaseScoreModTable.eItemQuality ItemQuality) { builder.AddInt(2, (int)ItemQuality, 0); }
  public static void AddAttrMod1(FlatBufferBuilder builder, int AttrMod1) { builder.AddInt(3, AttrMod1, 0); }
  public static void AddAttrMod2(FlatBufferBuilder builder, int AttrMod2) { builder.AddInt(4, AttrMod2, 0); }
  public static void AddPowerMod1(FlatBufferBuilder builder, int PowerMod1) { builder.AddInt(5, PowerMod1, 0); }
  public static void AddPowerMod2(FlatBufferBuilder builder, int PowerMod2) { builder.AddInt(6, PowerMod2, 0); }
  public static void AddPowerMod3(FlatBufferBuilder builder, int PowerMod3) { builder.AddInt(7, PowerMod3, 0); }
  public static void AddDefMod1(FlatBufferBuilder builder, int DefMod1) { builder.AddInt(8, DefMod1, 0); }
  public static void AddDefMod2(FlatBufferBuilder builder, int DefMod2) { builder.AddInt(9, DefMod2, 0); }
  public static void AddStrenthQualityMod1(FlatBufferBuilder builder, int StrenthQualityMod1) { builder.AddInt(10, StrenthQualityMod1, 0); }
  public static void AddStrenthQualityMod2(FlatBufferBuilder builder, int StrenthQualityMod2) { builder.AddInt(11, StrenthQualityMod2, 0); }
  public static void AddStrengthMod1(FlatBufferBuilder builder, VectorOffset StrengthMod1Offset) { builder.AddOffset(12, StrengthMod1Offset.Value, 0); }
  public static VectorOffset CreateStrengthMod1Vector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartStrengthMod1Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddStrengthMod2(FlatBufferBuilder builder, VectorOffset StrengthMod2Offset) { builder.AddOffset(13, StrengthMod2Offset.Value, 0); }
  public static VectorOffset CreateStrengthMod2Vector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartStrengthMod2Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddStrengthMod3(FlatBufferBuilder builder, VectorOffset StrengthMod3Offset) { builder.AddOffset(14, StrengthMod3Offset.Value, 0); }
  public static VectorOffset CreateStrengthMod3Vector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartStrengthMod3Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<EquipBaseScoreModTable> EndEquipBaseScoreModTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EquipBaseScoreModTable>(o);
  }
  public static void FinishEquipBaseScoreModTableBuffer(FlatBufferBuilder builder, Offset<EquipBaseScoreModTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
