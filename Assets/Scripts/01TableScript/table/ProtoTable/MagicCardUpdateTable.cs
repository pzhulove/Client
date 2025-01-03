// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class MagicCardUpdateTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 356485126,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static MagicCardUpdateTable GetRootAsMagicCardUpdateTable(ByteBuffer _bb) { return GetRootAsMagicCardUpdateTable(_bb, new MagicCardUpdateTable()); }
  public static MagicCardUpdateTable GetRootAsMagicCardUpdateTable(ByteBuffer _bb, MagicCardUpdateTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public MagicCardUpdateTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MagicCardID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(8); }
  public int MinLevel { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MaxLevel { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string UpgradeMaterials_1Array(int j) { int o = __p.__offset(14); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int UpgradeMaterials_1Length { get { int o = __p.__offset(14); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> UpgradeMaterials_1Value;
 public FlatBufferArray<string>  UpgradeMaterials_1
 {
  get{
  if (UpgradeMaterials_1Value == null)
  {
    UpgradeMaterials_1Value = new FlatBufferArray<string>(this.UpgradeMaterials_1Array, this.UpgradeMaterials_1Length);
  }
  return UpgradeMaterials_1Value;}
 }
  public string BaseRate_1Array(int j) { int o = __p.__offset(16); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int BaseRate_1Length { get { int o = __p.__offset(16); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> BaseRate_1Value;
 public FlatBufferArray<string>  BaseRate_1
 {
  get{
  if (BaseRate_1Value == null)
  {
    BaseRate_1Value = new FlatBufferArray<string>(this.BaseRate_1Array, this.BaseRate_1Length);
  }
  return BaseRate_1Value;}
 }
  public string UpgradeMaterials_2Array(int j) { int o = __p.__offset(18); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int UpgradeMaterials_2Length { get { int o = __p.__offset(18); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> UpgradeMaterials_2Value;
 public FlatBufferArray<string>  UpgradeMaterials_2
 {
  get{
  if (UpgradeMaterials_2Value == null)
  {
    UpgradeMaterials_2Value = new FlatBufferArray<string>(this.UpgradeMaterials_2Array, this.UpgradeMaterials_2Length);
  }
  return UpgradeMaterials_2Value;}
 }
  public string BaseRate_2Array(int j) { int o = __p.__offset(20); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int BaseRate_2Length { get { int o = __p.__offset(20); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> BaseRate_2Value;
 public FlatBufferArray<string>  BaseRate_2
 {
  get{
  if (BaseRate_2Value == null)
  {
    BaseRate_2Value = new FlatBufferArray<string>(this.BaseRate_2Array, this.BaseRate_2Length);
  }
  return BaseRate_2Value;}
 }
  public string UpgradeMaterials_3Array(int j) { int o = __p.__offset(22); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int UpgradeMaterials_3Length { get { int o = __p.__offset(22); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> UpgradeMaterials_3Value;
 public FlatBufferArray<string>  UpgradeMaterials_3
 {
  get{
  if (UpgradeMaterials_3Value == null)
  {
    UpgradeMaterials_3Value = new FlatBufferArray<string>(this.UpgradeMaterials_3Array, this.UpgradeMaterials_3Length);
  }
  return UpgradeMaterials_3Value;}
 }
  public string BaseRate_3Array(int j) { int o = __p.__offset(24); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int BaseRate_3Length { get { int o = __p.__offset(24); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> BaseRate_3Value;
 public FlatBufferArray<string>  BaseRate_3
 {
  get{
  if (BaseRate_3Value == null)
  {
    BaseRate_3Value = new FlatBufferArray<string>(this.BaseRate_3Array, this.BaseRate_3Length);
  }
  return BaseRate_3Value;}
 }
  public string LevelAddRateArray(int j) { int o = __p.__offset(26); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int LevelAddRateLength { get { int o = __p.__offset(26); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> LevelAddRateValue;
 public FlatBufferArray<string>  LevelAddRate
 {
  get{
  if (LevelAddRateValue == null)
  {
    LevelAddRateValue = new FlatBufferArray<string>(this.LevelAddRateArray, this.LevelAddRateLength);
  }
  return LevelAddRateValue;}
 }
  public string SameCardIDArray(int j) { int o = __p.__offset(28); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int SameCardIDLength { get { int o = __p.__offset(28); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> SameCardIDValue;
 public FlatBufferArray<string>  SameCardID
 {
  get{
  if (SameCardIDValue == null)
  {
    SameCardIDValue = new FlatBufferArray<string>(this.SameCardIDArray, this.SameCardIDLength);
  }
  return SameCardIDValue;}
 }
  public string SameCardAddRateArray(int j) { int o = __p.__offset(30); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int SameCardAddRateLength { get { int o = __p.__offset(30); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> SameCardAddRateValue;
 public FlatBufferArray<string>  SameCardAddRate
 {
  get{
  if (SameCardAddRateValue == null)
  {
    SameCardAddRateValue = new FlatBufferArray<string>(this.SameCardAddRateArray, this.SameCardAddRateLength);
  }
  return SameCardAddRateValue;}
 }
  public string UpConsume { get { int o = __p.__offset(32); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetUpConsumeBytes() { return __p.__vector_as_arraysegment(32); }

  public static Offset<MagicCardUpdateTable> CreateMagicCardUpdateTable(FlatBufferBuilder builder,
      int ID = 0,
      int MagicCardID = 0,
      StringOffset NameOffset = default(StringOffset),
      int MinLevel = 0,
      int MaxLevel = 0,
      VectorOffset UpgradeMaterials_1Offset = default(VectorOffset),
      VectorOffset BaseRate_1Offset = default(VectorOffset),
      VectorOffset UpgradeMaterials_2Offset = default(VectorOffset),
      VectorOffset BaseRate_2Offset = default(VectorOffset),
      VectorOffset UpgradeMaterials_3Offset = default(VectorOffset),
      VectorOffset BaseRate_3Offset = default(VectorOffset),
      VectorOffset LevelAddRateOffset = default(VectorOffset),
      VectorOffset SameCardIDOffset = default(VectorOffset),
      VectorOffset SameCardAddRateOffset = default(VectorOffset),
      StringOffset UpConsumeOffset = default(StringOffset)) {
    builder.StartObject(15);
    MagicCardUpdateTable.AddUpConsume(builder, UpConsumeOffset);
    MagicCardUpdateTable.AddSameCardAddRate(builder, SameCardAddRateOffset);
    MagicCardUpdateTable.AddSameCardID(builder, SameCardIDOffset);
    MagicCardUpdateTable.AddLevelAddRate(builder, LevelAddRateOffset);
    MagicCardUpdateTable.AddBaseRate3(builder, BaseRate_3Offset);
    MagicCardUpdateTable.AddUpgradeMaterials3(builder, UpgradeMaterials_3Offset);
    MagicCardUpdateTable.AddBaseRate2(builder, BaseRate_2Offset);
    MagicCardUpdateTable.AddUpgradeMaterials2(builder, UpgradeMaterials_2Offset);
    MagicCardUpdateTable.AddBaseRate1(builder, BaseRate_1Offset);
    MagicCardUpdateTable.AddUpgradeMaterials1(builder, UpgradeMaterials_1Offset);
    MagicCardUpdateTable.AddMaxLevel(builder, MaxLevel);
    MagicCardUpdateTable.AddMinLevel(builder, MinLevel);
    MagicCardUpdateTable.AddName(builder, NameOffset);
    MagicCardUpdateTable.AddMagicCardID(builder, MagicCardID);
    MagicCardUpdateTable.AddID(builder, ID);
    return MagicCardUpdateTable.EndMagicCardUpdateTable(builder);
  }

  public static void StartMagicCardUpdateTable(FlatBufferBuilder builder) { builder.StartObject(15); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddMagicCardID(FlatBufferBuilder builder, int MagicCardID) { builder.AddInt(1, MagicCardID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(2, NameOffset.Value, 0); }
  public static void AddMinLevel(FlatBufferBuilder builder, int MinLevel) { builder.AddInt(3, MinLevel, 0); }
  public static void AddMaxLevel(FlatBufferBuilder builder, int MaxLevel) { builder.AddInt(4, MaxLevel, 0); }
  public static void AddUpgradeMaterials1(FlatBufferBuilder builder, VectorOffset UpgradeMaterials1Offset) { builder.AddOffset(5, UpgradeMaterials1Offset.Value, 0); }
  public static VectorOffset CreateUpgradeMaterials1Vector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartUpgradeMaterials1Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBaseRate1(FlatBufferBuilder builder, VectorOffset BaseRate1Offset) { builder.AddOffset(6, BaseRate1Offset.Value, 0); }
  public static VectorOffset CreateBaseRate1Vector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBaseRate1Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddUpgradeMaterials2(FlatBufferBuilder builder, VectorOffset UpgradeMaterials2Offset) { builder.AddOffset(7, UpgradeMaterials2Offset.Value, 0); }
  public static VectorOffset CreateUpgradeMaterials2Vector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartUpgradeMaterials2Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBaseRate2(FlatBufferBuilder builder, VectorOffset BaseRate2Offset) { builder.AddOffset(8, BaseRate2Offset.Value, 0); }
  public static VectorOffset CreateBaseRate2Vector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBaseRate2Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddUpgradeMaterials3(FlatBufferBuilder builder, VectorOffset UpgradeMaterials3Offset) { builder.AddOffset(9, UpgradeMaterials3Offset.Value, 0); }
  public static VectorOffset CreateUpgradeMaterials3Vector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartUpgradeMaterials3Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBaseRate3(FlatBufferBuilder builder, VectorOffset BaseRate3Offset) { builder.AddOffset(10, BaseRate3Offset.Value, 0); }
  public static VectorOffset CreateBaseRate3Vector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBaseRate3Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLevelAddRate(FlatBufferBuilder builder, VectorOffset LevelAddRateOffset) { builder.AddOffset(11, LevelAddRateOffset.Value, 0); }
  public static VectorOffset CreateLevelAddRateVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLevelAddRateVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSameCardID(FlatBufferBuilder builder, VectorOffset SameCardIDOffset) { builder.AddOffset(12, SameCardIDOffset.Value, 0); }
  public static VectorOffset CreateSameCardIDVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSameCardIDVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSameCardAddRate(FlatBufferBuilder builder, VectorOffset SameCardAddRateOffset) { builder.AddOffset(13, SameCardAddRateOffset.Value, 0); }
  public static VectorOffset CreateSameCardAddRateVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSameCardAddRateVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddUpConsume(FlatBufferBuilder builder, StringOffset UpConsumeOffset) { builder.AddOffset(14, UpConsumeOffset.Value, 0); }
  public static Offset<MagicCardUpdateTable> EndMagicCardUpdateTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MagicCardUpdateTable>(o);
  }
  public static void FinishMagicCardUpdateTableBuffer(FlatBufferBuilder builder, Offset<MagicCardUpdateTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

