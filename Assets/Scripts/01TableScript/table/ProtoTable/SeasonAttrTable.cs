// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class SeasonAttrTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 1724784624,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static SeasonAttrTable GetRootAsSeasonAttrTable(ByteBuffer _bb) { return GetRootAsSeasonAttrTable(_bb, new SeasonAttrTable()); }
  public static SeasonAttrTable GetRootAsSeasonAttrTable(ByteBuffer _bb, SeasonAttrTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public SeasonAttrTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string DescsArray(int j) { int o = __p.__offset(6); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int DescsLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> DescsValue;
 public FlatBufferArray<string>  Descs
 {
  get{
  if (DescsValue == null)
  {
    DescsValue = new FlatBufferArray<string>(this.DescsArray, this.DescsLength);
  }
  return DescsValue;}
 }
  public int BuffIDsArray(int j) { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int BuffIDsLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetBuffIDsBytes() { return __p.__vector_as_arraysegment(8); }
 private FlatBufferArray<int> BuffIDsValue;
 public FlatBufferArray<int>  BuffIDs
 {
  get{
  if (BuffIDsValue == null)
  {
    BuffIDsValue = new FlatBufferArray<int>(this.BuffIDsArray, this.BuffIDsLength);
  }
  return BuffIDsValue;}
 }

  public static Offset<SeasonAttrTable> CreateSeasonAttrTable(FlatBufferBuilder builder,
      int ID = 0,
      VectorOffset DescsOffset = default(VectorOffset),
      VectorOffset BuffIDsOffset = default(VectorOffset)) {
    builder.StartObject(3);
    SeasonAttrTable.AddBuffIDs(builder, BuffIDsOffset);
    SeasonAttrTable.AddDescs(builder, DescsOffset);
    SeasonAttrTable.AddID(builder, ID);
    return SeasonAttrTable.EndSeasonAttrTable(builder);
  }

  public static void StartSeasonAttrTable(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddDescs(FlatBufferBuilder builder, VectorOffset DescsOffset) { builder.AddOffset(1, DescsOffset.Value, 0); }
  public static VectorOffset CreateDescsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDescsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBuffIDs(FlatBufferBuilder builder, VectorOffset BuffIDsOffset) { builder.AddOffset(2, BuffIDsOffset.Value, 0); }
  public static VectorOffset CreateBuffIDsVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartBuffIDsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<SeasonAttrTable> EndSeasonAttrTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SeasonAttrTable>(o);
  }
  public static void FinishSeasonAttrTableBuffer(FlatBufferBuilder builder, Offset<SeasonAttrTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
