// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class BeadRandomBuff : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -803912890,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static BeadRandomBuff GetRootAsBeadRandomBuff(ByteBuffer _bb) { return GetRootAsBeadRandomBuff(_bb, new BeadRandomBuff()); }
  public static BeadRandomBuff GetRootAsBeadRandomBuff(ByteBuffer _bb, BeadRandomBuff obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public BeadRandomBuff __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int BuffGroup { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int WeightedValue { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int BuffinfoID { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int BuffExplain { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int PropTypeArray(int j) { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int PropTypeLength { get { int o = __p.__offset(14); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetPropTypeBytes() { return __p.__vector_as_arraysegment(14); }
 private FlatBufferArray<int> PropTypeValue;
 public FlatBufferArray<int>  PropType
 {
  get{
  if (PropTypeValue == null)
  {
    PropTypeValue = new FlatBufferArray<int>(this.PropTypeArray, this.PropTypeLength);
  }
  return PropTypeValue;}
 }
  public int PropValueArray(int j) { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int PropValueLength { get { int o = __p.__offset(16); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetPropValueBytes() { return __p.__vector_as_arraysegment(16); }
 private FlatBufferArray<int> PropValueValue;
 public FlatBufferArray<int>  PropValue
 {
  get{
  if (PropValueValue == null)
  {
    PropValueValue = new FlatBufferArray<int>(this.PropValueArray, this.PropValueLength);
  }
  return PropValueValue;}
 }

  public static Offset<BeadRandomBuff> CreateBeadRandomBuff(FlatBufferBuilder builder,
      int ID = 0,
      int BuffGroup = 0,
      int WeightedValue = 0,
      int BuffinfoID = 0,
      int BuffExplain = 0,
      VectorOffset PropTypeOffset = default(VectorOffset),
      VectorOffset PropValueOffset = default(VectorOffset)) {
    builder.StartObject(7);
    BeadRandomBuff.AddPropValue(builder, PropValueOffset);
    BeadRandomBuff.AddPropType(builder, PropTypeOffset);
    BeadRandomBuff.AddBuffExplain(builder, BuffExplain);
    BeadRandomBuff.AddBuffinfoID(builder, BuffinfoID);
    BeadRandomBuff.AddWeightedValue(builder, WeightedValue);
    BeadRandomBuff.AddBuffGroup(builder, BuffGroup);
    BeadRandomBuff.AddID(builder, ID);
    return BeadRandomBuff.EndBeadRandomBuff(builder);
  }

  public static void StartBeadRandomBuff(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddBuffGroup(FlatBufferBuilder builder, int BuffGroup) { builder.AddInt(1, BuffGroup, 0); }
  public static void AddWeightedValue(FlatBufferBuilder builder, int WeightedValue) { builder.AddInt(2, WeightedValue, 0); }
  public static void AddBuffinfoID(FlatBufferBuilder builder, int BuffinfoID) { builder.AddInt(3, BuffinfoID, 0); }
  public static void AddBuffExplain(FlatBufferBuilder builder, int BuffExplain) { builder.AddInt(4, BuffExplain, 0); }
  public static void AddPropType(FlatBufferBuilder builder, VectorOffset PropTypeOffset) { builder.AddOffset(5, PropTypeOffset.Value, 0); }
  public static VectorOffset CreatePropTypeVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartPropTypeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPropValue(FlatBufferBuilder builder, VectorOffset PropValueOffset) { builder.AddOffset(6, PropValueOffset.Value, 0); }
  public static VectorOffset CreatePropValueVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartPropValueVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<BeadRandomBuff> EndBeadRandomBuff(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BeadRandomBuff>(o);
  }
  public static void FinishBeadRandomBuffBuffer(FlatBufferBuilder builder, Offset<BeadRandomBuff> offset) { builder.Finish(offset.Value); }
};


}


#endif

