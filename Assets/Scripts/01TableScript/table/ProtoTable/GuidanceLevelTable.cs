// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class GuidanceLevelTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 1028261162,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static GuidanceLevelTable GetRootAsGuidanceLevelTable(ByteBuffer _bb) { return GetRootAsGuidanceLevelTable(_bb, new GuidanceLevelTable()); }
  public static GuidanceLevelTable GetRootAsGuidanceLevelTable(ByteBuffer _bb, GuidanceLevelTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public GuidanceLevelTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int RelationIdsArray(int j) { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int RelationIdsLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetRelationIdsBytes() { return __p.__vector_as_arraysegment(6); }
 private FlatBufferArray<int> RelationIdsValue;
 public FlatBufferArray<int>  RelationIds
 {
  get{
  if (RelationIdsValue == null)
  {
    RelationIdsValue = new FlatBufferArray<int>(this.RelationIdsArray, this.RelationIdsLength);
  }
  return RelationIdsValue;}
 }

  public static Offset<GuidanceLevelTable> CreateGuidanceLevelTable(FlatBufferBuilder builder,
      int ID = 0,
      VectorOffset RelationIdsOffset = default(VectorOffset)) {
    builder.StartObject(2);
    GuidanceLevelTable.AddRelationIds(builder, RelationIdsOffset);
    GuidanceLevelTable.AddID(builder, ID);
    return GuidanceLevelTable.EndGuidanceLevelTable(builder);
  }

  public static void StartGuidanceLevelTable(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddRelationIds(FlatBufferBuilder builder, VectorOffset RelationIdsOffset) { builder.AddOffset(1, RelationIdsOffset.Value, 0); }
  public static VectorOffset CreateRelationIdsVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartRelationIdsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<GuidanceLevelTable> EndGuidanceLevelTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GuidanceLevelTable>(o);
  }
  public static void FinishGuidanceLevelTableBuffer(FlatBufferBuilder builder, Offset<GuidanceLevelTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
