// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class VipTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 501515545,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static VipTable GetRootAsVipTable(ByteBuffer _bb) { return GetRootAsVipTable(_bb, new VipTable()); }
  public static VipTable GetRootAsVipTable(ByteBuffer _bb, VipTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public VipTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int GiftID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int TotalRmb { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string GiftItemsArray(int j) { int o = __p.__offset(10); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int GiftItemsLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> GiftItemsValue;
 public FlatBufferArray<string>  GiftItems
 {
  get{
  if (GiftItemsValue == null)
  {
    GiftItemsValue = new FlatBufferArray<string>(this.GiftItemsArray, this.GiftItemsLength);
  }
  return GiftItemsValue;}
 }
  public int GiftPrePrice { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int GiftDiscountPrice { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string ArtifactJarDicountRateArray(int j) { int o = __p.__offset(16); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int ArtifactJarDicountRateLength { get { int o = __p.__offset(16); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> ArtifactJarDicountRateValue;
 public FlatBufferArray<string>  ArtifactJarDicountRate
 {
  get{
  if (ArtifactJarDicountRateValue == null)
  {
    ArtifactJarDicountRateValue = new FlatBufferArray<string>(this.ArtifactJarDicountRateArray, this.ArtifactJarDicountRateLength);
  }
  return ArtifactJarDicountRateValue;}
 }
  public string ArtifactJarDiscountProbArray(int j) { int o = __p.__offset(18); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int ArtifactJarDiscountProbLength { get { int o = __p.__offset(18); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> ArtifactJarDiscountProbValue;
 public FlatBufferArray<string>  ArtifactJarDiscountProb
 {
  get{
  if (ArtifactJarDiscountProbValue == null)
  {
    ArtifactJarDiscountProbValue = new FlatBufferArray<string>(this.ArtifactJarDiscountProbArray, this.ArtifactJarDiscountProbLength);
  }
  return ArtifactJarDiscountProbValue;}
 }
  public string ArtifactJarDiscountEffectTimesArray(int j) { int o = __p.__offset(20); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int ArtifactJarDiscountEffectTimesLength { get { int o = __p.__offset(20); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> ArtifactJarDiscountEffectTimesValue;
 public FlatBufferArray<string>  ArtifactJarDiscountEffectTimes
 {
  get{
  if (ArtifactJarDiscountEffectTimesValue == null)
  {
    ArtifactJarDiscountEffectTimesValue = new FlatBufferArray<string>(this.ArtifactJarDiscountEffectTimesArray, this.ArtifactJarDiscountEffectTimesLength);
  }
  return ArtifactJarDiscountEffectTimesValue;}
 }
  public string ArtifactJarDiscountETProbArray(int j) { int o = __p.__offset(22); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int ArtifactJarDiscountETProbLength { get { int o = __p.__offset(22); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> ArtifactJarDiscountETProbValue;
 public FlatBufferArray<string>  ArtifactJarDiscountETProb
 {
  get{
  if (ArtifactJarDiscountETProbValue == null)
  {
    ArtifactJarDiscountETProbValue = new FlatBufferArray<string>(this.ArtifactJarDiscountETProbArray, this.ArtifactJarDiscountETProbLength);
  }
  return ArtifactJarDiscountETProbValue;}
 }
  public string ArtifactJarDiscountEffectTimes1Array(int j) { int o = __p.__offset(24); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int ArtifactJarDiscountEffectTimes1Length { get { int o = __p.__offset(24); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> ArtifactJarDiscountEffectTimes1Value;
 public FlatBufferArray<string>  ArtifactJarDiscountEffectTimes1
 {
  get{
  if (ArtifactJarDiscountEffectTimes1Value == null)
  {
    ArtifactJarDiscountEffectTimes1Value = new FlatBufferArray<string>(this.ArtifactJarDiscountEffectTimes1Array, this.ArtifactJarDiscountEffectTimes1Length);
  }
  return ArtifactJarDiscountEffectTimes1Value;}
 }
  public string ArtifactJarDiscountETProb1Array(int j) { int o = __p.__offset(26); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int ArtifactJarDiscountETProb1Length { get { int o = __p.__offset(26); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> ArtifactJarDiscountETProb1Value;
 public FlatBufferArray<string>  ArtifactJarDiscountETProb1
 {
  get{
  if (ArtifactJarDiscountETProb1Value == null)
  {
    ArtifactJarDiscountETProb1Value = new FlatBufferArray<string>(this.ArtifactJarDiscountETProb1Array, this.ArtifactJarDiscountETProb1Length);
  }
  return ArtifactJarDiscountETProb1Value;}
 }

  public static Offset<VipTable> CreateVipTable(FlatBufferBuilder builder,
      int ID = 0,
      int GiftID = 0,
      int TotalRmb = 0,
      VectorOffset GiftItemsOffset = default(VectorOffset),
      int GiftPrePrice = 0,
      int GiftDiscountPrice = 0,
      VectorOffset ArtifactJarDicountRateOffset = default(VectorOffset),
      VectorOffset ArtifactJarDiscountProbOffset = default(VectorOffset),
      VectorOffset ArtifactJarDiscountEffectTimesOffset = default(VectorOffset),
      VectorOffset ArtifactJarDiscountETProbOffset = default(VectorOffset),
      VectorOffset ArtifactJarDiscountEffectTimes1Offset = default(VectorOffset),
      VectorOffset ArtifactJarDiscountETProb1Offset = default(VectorOffset)) {
    builder.StartObject(12);
    VipTable.AddArtifactJarDiscountETProb1(builder, ArtifactJarDiscountETProb1Offset);
    VipTable.AddArtifactJarDiscountEffectTimes1(builder, ArtifactJarDiscountEffectTimes1Offset);
    VipTable.AddArtifactJarDiscountETProb(builder, ArtifactJarDiscountETProbOffset);
    VipTable.AddArtifactJarDiscountEffectTimes(builder, ArtifactJarDiscountEffectTimesOffset);
    VipTable.AddArtifactJarDiscountProb(builder, ArtifactJarDiscountProbOffset);
    VipTable.AddArtifactJarDicountRate(builder, ArtifactJarDicountRateOffset);
    VipTable.AddGiftDiscountPrice(builder, GiftDiscountPrice);
    VipTable.AddGiftPrePrice(builder, GiftPrePrice);
    VipTable.AddGiftItems(builder, GiftItemsOffset);
    VipTable.AddTotalRmb(builder, TotalRmb);
    VipTable.AddGiftID(builder, GiftID);
    VipTable.AddID(builder, ID);
    return VipTable.EndVipTable(builder);
  }

  public static void StartVipTable(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddGiftID(FlatBufferBuilder builder, int GiftID) { builder.AddInt(1, GiftID, 0); }
  public static void AddTotalRmb(FlatBufferBuilder builder, int TotalRmb) { builder.AddInt(2, TotalRmb, 0); }
  public static void AddGiftItems(FlatBufferBuilder builder, VectorOffset GiftItemsOffset) { builder.AddOffset(3, GiftItemsOffset.Value, 0); }
  public static VectorOffset CreateGiftItemsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartGiftItemsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddGiftPrePrice(FlatBufferBuilder builder, int GiftPrePrice) { builder.AddInt(4, GiftPrePrice, 0); }
  public static void AddGiftDiscountPrice(FlatBufferBuilder builder, int GiftDiscountPrice) { builder.AddInt(5, GiftDiscountPrice, 0); }
  public static void AddArtifactJarDicountRate(FlatBufferBuilder builder, VectorOffset ArtifactJarDicountRateOffset) { builder.AddOffset(6, ArtifactJarDicountRateOffset.Value, 0); }
  public static VectorOffset CreateArtifactJarDicountRateVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArtifactJarDicountRateVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddArtifactJarDiscountProb(FlatBufferBuilder builder, VectorOffset ArtifactJarDiscountProbOffset) { builder.AddOffset(7, ArtifactJarDiscountProbOffset.Value, 0); }
  public static VectorOffset CreateArtifactJarDiscountProbVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArtifactJarDiscountProbVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddArtifactJarDiscountEffectTimes(FlatBufferBuilder builder, VectorOffset ArtifactJarDiscountEffectTimesOffset) { builder.AddOffset(8, ArtifactJarDiscountEffectTimesOffset.Value, 0); }
  public static VectorOffset CreateArtifactJarDiscountEffectTimesVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArtifactJarDiscountEffectTimesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddArtifactJarDiscountETProb(FlatBufferBuilder builder, VectorOffset ArtifactJarDiscountETProbOffset) { builder.AddOffset(9, ArtifactJarDiscountETProbOffset.Value, 0); }
  public static VectorOffset CreateArtifactJarDiscountETProbVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArtifactJarDiscountETProbVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddArtifactJarDiscountEffectTimes1(FlatBufferBuilder builder, VectorOffset ArtifactJarDiscountEffectTimes1Offset) { builder.AddOffset(10, ArtifactJarDiscountEffectTimes1Offset.Value, 0); }
  public static VectorOffset CreateArtifactJarDiscountEffectTimes1Vector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArtifactJarDiscountEffectTimes1Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddArtifactJarDiscountETProb1(FlatBufferBuilder builder, VectorOffset ArtifactJarDiscountETProb1Offset) { builder.AddOffset(11, ArtifactJarDiscountETProb1Offset.Value, 0); }
  public static VectorOffset CreateArtifactJarDiscountETProb1Vector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArtifactJarDiscountETProb1Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<VipTable> EndVipTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<VipTable>(o);
  }
  public static void FinishVipTableBuffer(FlatBufferBuilder builder, Offset<VipTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
