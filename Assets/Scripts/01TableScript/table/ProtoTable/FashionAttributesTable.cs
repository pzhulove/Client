// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class FashionAttributesTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1878141935,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static FashionAttributesTable GetRootAsFashionAttributesTable(ByteBuffer _bb) { return GetRootAsFashionAttributesTable(_bb, new FashionAttributesTable()); }
  public static FashionAttributesTable GetRootAsFashionAttributesTable(ByteBuffer _bb, FashionAttributesTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public FashionAttributesTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int PropTypeArray(int j) { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int PropTypeLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetPropTypeBytes() { return __p.__vector_as_arraysegment(6); }
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
  public int PropValueArray(int j) { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int PropValueLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetPropValueBytes() { return __p.__vector_as_arraysegment(8); }
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
  public int BuffIDArray(int j) { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int BuffIDLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetBuffIDBytes() { return __p.__vector_as_arraysegment(10); }
 private FlatBufferArray<int> BuffIDValue;
 public FlatBufferArray<int>  BuffID
 {
  get{
  if (BuffIDValue == null)
  {
    BuffIDValue = new FlatBufferArray<int>(this.BuffIDArray, this.BuffIDLength);
  }
  return BuffIDValue;}
 }

  public static Offset<FashionAttributesTable> CreateFashionAttributesTable(FlatBufferBuilder builder,
      int ID = 0,
      VectorOffset PropTypeOffset = default(VectorOffset),
      VectorOffset PropValueOffset = default(VectorOffset),
      VectorOffset BuffIDOffset = default(VectorOffset)) {
    builder.StartObject(4);
    FashionAttributesTable.AddBuffID(builder, BuffIDOffset);
    FashionAttributesTable.AddPropValue(builder, PropValueOffset);
    FashionAttributesTable.AddPropType(builder, PropTypeOffset);
    FashionAttributesTable.AddID(builder, ID);
    return FashionAttributesTable.EndFashionAttributesTable(builder);
  }

  public static void StartFashionAttributesTable(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddPropType(FlatBufferBuilder builder, VectorOffset PropTypeOffset) { builder.AddOffset(1, PropTypeOffset.Value, 0); }
  public static VectorOffset CreatePropTypeVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartPropTypeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPropValue(FlatBufferBuilder builder, VectorOffset PropValueOffset) { builder.AddOffset(2, PropValueOffset.Value, 0); }
  public static VectorOffset CreatePropValueVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartPropValueVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBuffID(FlatBufferBuilder builder, VectorOffset BuffIDOffset) { builder.AddOffset(3, BuffIDOffset.Value, 0); }
  public static VectorOffset CreateBuffIDVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartBuffIDVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<FashionAttributesTable> EndFashionAttributesTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FashionAttributesTable>(o);
  }
  public static void FinishFashionAttributesTableBuffer(FlatBufferBuilder builder, Offset<FashionAttributesTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
