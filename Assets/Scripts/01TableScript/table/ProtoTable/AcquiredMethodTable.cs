// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class AcquiredMethodTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 1117580373,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static AcquiredMethodTable GetRootAsAcquiredMethodTable(ByteBuffer _bb) { return GetRootAsAcquiredMethodTable(_bb, new AcquiredMethodTable()); }
  public static AcquiredMethodTable GetRootAsAcquiredMethodTable(ByteBuffer _bb, AcquiredMethodTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public AcquiredMethodTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public string LinkZone { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLinkZoneBytes() { return __p.__vector_as_arraysegment(8); }
  public string ActionDesc { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetActionDescBytes() { return __p.__vector_as_arraysegment(10); }
  public int IsLink { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int FuncitonID { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string ProbilityDesc { get { int o = __p.__offset(16); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetProbilityDescBytes() { return __p.__vector_as_arraysegment(16); }
  public string LinkInfo { get { int o = __p.__offset(18); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLinkInfoBytes() { return __p.__vector_as_arraysegment(18); }
  public int ReLinksArray(int j) { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int ReLinksLength { get { int o = __p.__offset(20); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetReLinksBytes() { return __p.__vector_as_arraysegment(20); }
 private FlatBufferArray<int> ReLinksValue;
 public FlatBufferArray<int>  ReLinks
 {
  get{
  if (ReLinksValue == null)
  {
    ReLinksValue = new FlatBufferArray<int>(this.ReLinksArray, this.ReLinksLength);
  }
  return ReLinksValue;}
 }
  public int ItemId { get { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<AcquiredMethodTable> CreateAcquiredMethodTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      StringOffset LinkZoneOffset = default(StringOffset),
      StringOffset ActionDescOffset = default(StringOffset),
      int IsLink = 0,
      int FuncitonID = 0,
      StringOffset ProbilityDescOffset = default(StringOffset),
      StringOffset LinkInfoOffset = default(StringOffset),
      VectorOffset ReLinksOffset = default(VectorOffset),
      int ItemId = 0) {
    builder.StartObject(10);
    AcquiredMethodTable.AddItemId(builder, ItemId);
    AcquiredMethodTable.AddReLinks(builder, ReLinksOffset);
    AcquiredMethodTable.AddLinkInfo(builder, LinkInfoOffset);
    AcquiredMethodTable.AddProbilityDesc(builder, ProbilityDescOffset);
    AcquiredMethodTable.AddFuncitonID(builder, FuncitonID);
    AcquiredMethodTable.AddIsLink(builder, IsLink);
    AcquiredMethodTable.AddActionDesc(builder, ActionDescOffset);
    AcquiredMethodTable.AddLinkZone(builder, LinkZoneOffset);
    AcquiredMethodTable.AddName(builder, NameOffset);
    AcquiredMethodTable.AddID(builder, ID);
    return AcquiredMethodTable.EndAcquiredMethodTable(builder);
  }

  public static void StartAcquiredMethodTable(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddLinkZone(FlatBufferBuilder builder, StringOffset LinkZoneOffset) { builder.AddOffset(2, LinkZoneOffset.Value, 0); }
  public static void AddActionDesc(FlatBufferBuilder builder, StringOffset ActionDescOffset) { builder.AddOffset(3, ActionDescOffset.Value, 0); }
  public static void AddIsLink(FlatBufferBuilder builder, int IsLink) { builder.AddInt(4, IsLink, 0); }
  public static void AddFuncitonID(FlatBufferBuilder builder, int FuncitonID) { builder.AddInt(5, FuncitonID, 0); }
  public static void AddProbilityDesc(FlatBufferBuilder builder, StringOffset ProbilityDescOffset) { builder.AddOffset(6, ProbilityDescOffset.Value, 0); }
  public static void AddLinkInfo(FlatBufferBuilder builder, StringOffset LinkInfoOffset) { builder.AddOffset(7, LinkInfoOffset.Value, 0); }
  public static void AddReLinks(FlatBufferBuilder builder, VectorOffset ReLinksOffset) { builder.AddOffset(8, ReLinksOffset.Value, 0); }
  public static VectorOffset CreateReLinksVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartReLinksVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddItemId(FlatBufferBuilder builder, int ItemId) { builder.AddInt(9, ItemId, 0); }
  public static Offset<AcquiredMethodTable> EndAcquiredMethodTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AcquiredMethodTable>(o);
  }
  public static void FinishAcquiredMethodTableBuffer(FlatBufferBuilder builder, Offset<AcquiredMethodTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
