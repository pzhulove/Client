// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if  USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public enum UnionCellType : int
{
 union_helper = 0,
 union_fix = 1,
 union_fixGrow = 2,
 union_everyvalue = 3,
};

public class EveryValue : IFlatbufferObject
{
  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static EveryValue GetRootAsEveryValue(ByteBuffer _bb) { return GetRootAsEveryValue(_bb, new EveryValue()); }
  public static EveryValue GetRootAsEveryValue(ByteBuffer _bb, EveryValue obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public EveryValue __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int everyValuesArray(int j) { int o = __p.__offset(4); return o != 0 ? (int)__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int everyValuesLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetEveryValuesBytes() { return __p.__vector_as_arraysegment(4); }
 private FlatBufferArray<int> everyValuesValue;
 public FlatBufferArray<int>  everyValues
 {
  get{
  if (everyValuesValue == null)
  {
    everyValuesValue = new FlatBufferArray<int>(this.everyValuesArray, this.everyValuesLength);
  }
  return everyValuesValue;}
 }

  public static Offset<EveryValue> CreateEveryValue(FlatBufferBuilder builder,
      VectorOffset everyValuesOffset = default(VectorOffset)) {
    builder.StartObject(1);
    EveryValue.AddEveryValues(builder, everyValuesOffset);
    return EveryValue.EndEveryValue(builder);
  }

  public static void StartEveryValue(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddEveryValues(FlatBufferBuilder builder, VectorOffset everyValuesOffset) { builder.AddOffset(0, everyValuesOffset.Value, 0); }
  public static VectorOffset CreateEveryValuesVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartEveryValuesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<EveryValue> EndEveryValue(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EveryValue>(o);
  }
};

public class UnionCell : IFlatbufferObject
{
  static UnionCell def;
  public static UnionCell Default(){
    if(def == null){
      FlatBufferBuilder builder = new FlatBufferBuilder(64);
      var offset = CreateUnionCell(builder);
      builder.Finish(offset.Value);
      def = GetRootAsUnionCell(new ByteBuffer(builder.SizedByteArray()));
    }
    return def;
  }
  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static UnionCell GetRootAsUnionCell(ByteBuffer _bb) { return GetRootAsUnionCell(_bb, new UnionCell()); }
  public static UnionCell GetRootAsUnionCell(ByteBuffer _bb, UnionCell obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public UnionCell __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public UnionCellType valueType { get { int o = __p.__offset(4); return o != 0 ? (UnionCellType)__p.bb.GetInt(o + __p.bb_pos) : UnionCellType.union_helper; } }
  public EveryValue eValues { get { int o = __p.__offset(6); return o != 0 ? (EveryValue)(new EveryValue()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) :(EveryValue)(object) null; } }
  public int fixValue { get { int o = __p.__offset(8); return o != 0 ? (int)__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int fixInitValue { get { int o = __p.__offset(10); return o != 0 ? (int)__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int fixLevelGrow { get { int o = __p.__offset(12); return o != 0 ? (int)__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<UnionCell> CreateUnionCell(FlatBufferBuilder builder,
      UnionCellType valueType = UnionCellType.union_helper,
      Offset<EveryValue> eValuesOffset = default(Offset<EveryValue>),
      int fixValue = 0,
      int fixInitValue = 0,
      int fixLevelGrow = 0) {
    builder.StartObject(5);
    UnionCell.AddFixLevelGrow(builder, fixLevelGrow);
    UnionCell.AddFixInitValue(builder, fixInitValue);
    UnionCell.AddFixValue(builder, fixValue);
    UnionCell.AddEValues(builder, eValuesOffset);
    UnionCell.AddValueType(builder, valueType);
    return UnionCell.EndUnionCell(builder);
  }

  public static void StartUnionCell(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddValueType(FlatBufferBuilder builder, UnionCellType valueType) { builder.AddInt(0, (int)valueType, 0); }
  public static void AddEValues(FlatBufferBuilder builder, Offset<EveryValue> eValuesOffset) { builder.AddOffset(1, eValuesOffset.Value, 0); }
  public static void AddFixValue(FlatBufferBuilder builder, int fixValue) { builder.AddInt(2, fixValue, 0); }
  public static void AddFixInitValue(FlatBufferBuilder builder, int fixInitValue) { builder.AddInt(3, fixInitValue, 0); }
  public static void AddFixLevelGrow(FlatBufferBuilder builder, int fixLevelGrow) { builder.AddInt(4, fixLevelGrow, 0); }
  public static Offset<UnionCell> EndUnionCell(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<UnionCell>(o);
  }
};


}


#endif