// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class VoiceTable : IFlatbufferObject
{
public enum eVoiceType : int
{
 VoiceType_None = 0,
 SELECTROLE = 1,
 GETROLE = 2,
 DUNGEONFINISH = 3,
 DUNGEONDEAD = 4,
 DUNGEONKILLPOWER = 5,
 DUNGEONCLEARROOM = 6,
 DUNGEONPOWERSKILL = 7,
 DIALOGBEGIN = 8,
 DIALOGEND = 9,
 NEWBIEGUIDE = 10,
 ASIDE = 11,
 BIRTHROLE = 12,
};

public enum eCrypt : int
{
 code = -1319156132,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static VoiceTable GetRootAsVoiceTable(ByteBuffer _bb) { return GetRootAsVoiceTable(_bb, new VoiceTable()); }
  public static VoiceTable GetRootAsVoiceTable(ByteBuffer _bb, VoiceTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public VoiceTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string VoicePath { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetVoicePathBytes() { return __p.__vector_as_arraysegment(6); }
  public string VoiceContent { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetVoiceContentBytes() { return __p.__vector_as_arraysegment(8); }
  public ProtoTable.VoiceTable.eVoiceType VoiceType { get { int o = __p.__offset(10); return o != 0 ? (ProtoTable.VoiceTable.eVoiceType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.VoiceTable.eVoiceType.VoiceType_None; } }
  public int VoiceWeight { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int VoiceRate { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int VoiceUnitIDArray(int j) { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int VoiceUnitIDLength { get { int o = __p.__offset(16); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetVoiceUnitIDBytes() { return __p.__vector_as_arraysegment(16); }
 private FlatBufferArray<int> VoiceUnitIDValue;
 public FlatBufferArray<int>  VoiceUnitID
 {
  get{
  if (VoiceUnitIDValue == null)
  {
    VoiceUnitIDValue = new FlatBufferArray<int>(this.VoiceUnitIDArray, this.VoiceUnitIDLength);
  }
  return VoiceUnitIDValue;}
 }
  public string VoiceTag { get { int o = __p.__offset(18); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetVoiceTagBytes() { return __p.__vector_as_arraysegment(18); }

  public static Offset<VoiceTable> CreateVoiceTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset VoicePathOffset = default(StringOffset),
      StringOffset VoiceContentOffset = default(StringOffset),
      ProtoTable.VoiceTable.eVoiceType VoiceType = ProtoTable.VoiceTable.eVoiceType.VoiceType_None,
      int VoiceWeight = 0,
      int VoiceRate = 0,
      VectorOffset VoiceUnitIDOffset = default(VectorOffset),
      StringOffset VoiceTagOffset = default(StringOffset)) {
    builder.StartObject(8);
    VoiceTable.AddVoiceTag(builder, VoiceTagOffset);
    VoiceTable.AddVoiceUnitID(builder, VoiceUnitIDOffset);
    VoiceTable.AddVoiceRate(builder, VoiceRate);
    VoiceTable.AddVoiceWeight(builder, VoiceWeight);
    VoiceTable.AddVoiceType(builder, VoiceType);
    VoiceTable.AddVoiceContent(builder, VoiceContentOffset);
    VoiceTable.AddVoicePath(builder, VoicePathOffset);
    VoiceTable.AddID(builder, ID);
    return VoiceTable.EndVoiceTable(builder);
  }

  public static void StartVoiceTable(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddVoicePath(FlatBufferBuilder builder, StringOffset VoicePathOffset) { builder.AddOffset(1, VoicePathOffset.Value, 0); }
  public static void AddVoiceContent(FlatBufferBuilder builder, StringOffset VoiceContentOffset) { builder.AddOffset(2, VoiceContentOffset.Value, 0); }
  public static void AddVoiceType(FlatBufferBuilder builder, ProtoTable.VoiceTable.eVoiceType VoiceType) { builder.AddInt(3, (int)VoiceType, 0); }
  public static void AddVoiceWeight(FlatBufferBuilder builder, int VoiceWeight) { builder.AddInt(4, VoiceWeight, 0); }
  public static void AddVoiceRate(FlatBufferBuilder builder, int VoiceRate) { builder.AddInt(5, VoiceRate, 0); }
  public static void AddVoiceUnitID(FlatBufferBuilder builder, VectorOffset VoiceUnitIDOffset) { builder.AddOffset(6, VoiceUnitIDOffset.Value, 0); }
  public static VectorOffset CreateVoiceUnitIDVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartVoiceUnitIDVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddVoiceTag(FlatBufferBuilder builder, StringOffset VoiceTagOffset) { builder.AddOffset(7, VoiceTagOffset.Value, 0); }
  public static Offset<VoiceTable> EndVoiceTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<VoiceTable>(o);
  }
  public static void FinishVoiceTableBuffer(FlatBufferBuilder builder, Offset<VoiceTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

