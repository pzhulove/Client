// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class EquipStrColorRatioTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1294104621,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static EquipStrColorRatioTable GetRootAsEquipStrColorRatioTable(ByteBuffer _bb) { return GetRootAsEquipStrColorRatioTable(_bb, new EquipStrColorRatioTable()); }
  public static EquipStrColorRatioTable GetRootAsEquipStrColorRatioTable(ByteBuffer _bb, EquipStrColorRatioTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public EquipStrColorRatioTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Lv { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int WhiteRatioArray(int j) { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int WhiteRatioLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetWhiteRatioBytes() { return __p.__vector_as_arraysegment(8); }
 private FlatBufferArray<int> WhiteRatioValue;
 public FlatBufferArray<int>  WhiteRatio
 {
  get{
  if (WhiteRatioValue == null)
  {
    WhiteRatioValue = new FlatBufferArray<int>(this.WhiteRatioArray, this.WhiteRatioLength);
  }
  return WhiteRatioValue;}
 }
  public int BlueRatioArray(int j) { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int BlueRatioLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetBlueRatioBytes() { return __p.__vector_as_arraysegment(10); }
 private FlatBufferArray<int> BlueRatioValue;
 public FlatBufferArray<int>  BlueRatio
 {
  get{
  if (BlueRatioValue == null)
  {
    BlueRatioValue = new FlatBufferArray<int>(this.BlueRatioArray, this.BlueRatioLength);
  }
  return BlueRatioValue;}
 }
  public int PurpleRatioArray(int j) { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int PurpleRatioLength { get { int o = __p.__offset(12); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetPurpleRatioBytes() { return __p.__vector_as_arraysegment(12); }
 private FlatBufferArray<int> PurpleRatioValue;
 public FlatBufferArray<int>  PurpleRatio
 {
  get{
  if (PurpleRatioValue == null)
  {
    PurpleRatioValue = new FlatBufferArray<int>(this.PurpleRatioArray, this.PurpleRatioLength);
  }
  return PurpleRatioValue;}
 }
  public int GreenRatioArray(int j) { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int GreenRatioLength { get { int o = __p.__offset(14); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetGreenRatioBytes() { return __p.__vector_as_arraysegment(14); }
 private FlatBufferArray<int> GreenRatioValue;
 public FlatBufferArray<int>  GreenRatio
 {
  get{
  if (GreenRatioValue == null)
  {
    GreenRatioValue = new FlatBufferArray<int>(this.GreenRatioArray, this.GreenRatioLength);
  }
  return GreenRatioValue;}
 }
  public int PinkRatioArray(int j) { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int PinkRatioLength { get { int o = __p.__offset(16); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetPinkRatioBytes() { return __p.__vector_as_arraysegment(16); }
 private FlatBufferArray<int> PinkRatioValue;
 public FlatBufferArray<int>  PinkRatio
 {
  get{
  if (PinkRatioValue == null)
  {
    PinkRatioValue = new FlatBufferArray<int>(this.PinkRatioArray, this.PinkRatioLength);
  }
  return PinkRatioValue;}
 }
  public int YellowRatioArray(int j) { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int YellowRatioLength { get { int o = __p.__offset(18); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetYellowRatioBytes() { return __p.__vector_as_arraysegment(18); }
 private FlatBufferArray<int> YellowRatioValue;
 public FlatBufferArray<int>  YellowRatio
 {
  get{
  if (YellowRatioValue == null)
  {
    YellowRatioValue = new FlatBufferArray<int>(this.YellowRatioArray, this.YellowRatioLength);
  }
  return YellowRatioValue;}
 }

  public static Offset<EquipStrColorRatioTable> CreateEquipStrColorRatioTable(FlatBufferBuilder builder,
      int ID = 0,
      int Lv = 0,
      VectorOffset WhiteRatioOffset = default(VectorOffset),
      VectorOffset BlueRatioOffset = default(VectorOffset),
      VectorOffset PurpleRatioOffset = default(VectorOffset),
      VectorOffset GreenRatioOffset = default(VectorOffset),
      VectorOffset PinkRatioOffset = default(VectorOffset),
      VectorOffset YellowRatioOffset = default(VectorOffset)) {
    builder.StartObject(8);
    EquipStrColorRatioTable.AddYellowRatio(builder, YellowRatioOffset);
    EquipStrColorRatioTable.AddPinkRatio(builder, PinkRatioOffset);
    EquipStrColorRatioTable.AddGreenRatio(builder, GreenRatioOffset);
    EquipStrColorRatioTable.AddPurpleRatio(builder, PurpleRatioOffset);
    EquipStrColorRatioTable.AddBlueRatio(builder, BlueRatioOffset);
    EquipStrColorRatioTable.AddWhiteRatio(builder, WhiteRatioOffset);
    EquipStrColorRatioTable.AddLv(builder, Lv);
    EquipStrColorRatioTable.AddID(builder, ID);
    return EquipStrColorRatioTable.EndEquipStrColorRatioTable(builder);
  }

  public static void StartEquipStrColorRatioTable(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddLv(FlatBufferBuilder builder, int Lv) { builder.AddInt(1, Lv, 0); }
  public static void AddWhiteRatio(FlatBufferBuilder builder, VectorOffset WhiteRatioOffset) { builder.AddOffset(2, WhiteRatioOffset.Value, 0); }
  public static VectorOffset CreateWhiteRatioVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartWhiteRatioVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBlueRatio(FlatBufferBuilder builder, VectorOffset BlueRatioOffset) { builder.AddOffset(3, BlueRatioOffset.Value, 0); }
  public static VectorOffset CreateBlueRatioVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartBlueRatioVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPurpleRatio(FlatBufferBuilder builder, VectorOffset PurpleRatioOffset) { builder.AddOffset(4, PurpleRatioOffset.Value, 0); }
  public static VectorOffset CreatePurpleRatioVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartPurpleRatioVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddGreenRatio(FlatBufferBuilder builder, VectorOffset GreenRatioOffset) { builder.AddOffset(5, GreenRatioOffset.Value, 0); }
  public static VectorOffset CreateGreenRatioVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartGreenRatioVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPinkRatio(FlatBufferBuilder builder, VectorOffset PinkRatioOffset) { builder.AddOffset(6, PinkRatioOffset.Value, 0); }
  public static VectorOffset CreatePinkRatioVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartPinkRatioVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddYellowRatio(FlatBufferBuilder builder, VectorOffset YellowRatioOffset) { builder.AddOffset(7, YellowRatioOffset.Value, 0); }
  public static VectorOffset CreateYellowRatioVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartYellowRatioVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<EquipStrColorRatioTable> EndEquipStrColorRatioTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EquipStrColorRatioTable>(o);
  }
  public static void FinishEquipStrColorRatioTableBuffer(FlatBufferBuilder builder, Offset<EquipStrColorRatioTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
