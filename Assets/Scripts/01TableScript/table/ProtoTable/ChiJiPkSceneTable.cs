// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ChiJiPkSceneTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1069274954,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ChiJiPkSceneTable GetRootAsChiJiPkSceneTable(ByteBuffer _bb) { return GetRootAsChiJiPkSceneTable(_bb, new ChiJiPkSceneTable()); }
  public static ChiJiPkSceneTable GetRootAsChiJiPkSceneTable(ByteBuffer _bb, ChiJiPkSceneTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ChiJiPkSceneTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public int DungeonID { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SceneRangeArray(int j) { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int SceneRangeLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetSceneRangeBytes() { return __p.__vector_as_arraysegment(10); }
 private FlatBufferArray<int> SceneRangeValue;
 public FlatBufferArray<int>  SceneRange
 {
  get{
  if (SceneRangeValue == null)
  {
    SceneRangeValue = new FlatBufferArray<int>(this.SceneRangeArray, this.SceneRangeLength);
  }
  return SceneRangeValue;}
 }

  public static Offset<ChiJiPkSceneTable> CreateChiJiPkSceneTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      int DungeonID = 0,
      VectorOffset SceneRangeOffset = default(VectorOffset)) {
    builder.StartObject(4);
    ChiJiPkSceneTable.AddSceneRange(builder, SceneRangeOffset);
    ChiJiPkSceneTable.AddDungeonID(builder, DungeonID);
    ChiJiPkSceneTable.AddName(builder, NameOffset);
    ChiJiPkSceneTable.AddID(builder, ID);
    return ChiJiPkSceneTable.EndChiJiPkSceneTable(builder);
  }

  public static void StartChiJiPkSceneTable(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddDungeonID(FlatBufferBuilder builder, int DungeonID) { builder.AddInt(2, DungeonID, 0); }
  public static void AddSceneRange(FlatBufferBuilder builder, VectorOffset SceneRangeOffset) { builder.AddOffset(3, SceneRangeOffset.Value, 0); }
  public static VectorOffset CreateSceneRangeVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartSceneRangeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ChiJiPkSceneTable> EndChiJiPkSceneTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ChiJiPkSceneTable>(o);
  }
  public static void FinishChiJiPkSceneTableBuffer(FlatBufferBuilder builder, Offset<ChiJiPkSceneTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
