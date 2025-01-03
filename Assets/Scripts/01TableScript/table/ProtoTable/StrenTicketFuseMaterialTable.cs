// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class StrenTicketFuseMaterialTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 522136054,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static StrenTicketFuseMaterialTable GetRootAsStrenTicketFuseMaterialTable(ByteBuffer _bb) { return GetRootAsStrenTicketFuseMaterialTable(_bb, new StrenTicketFuseMaterialTable()); }
  public static StrenTicketFuseMaterialTable GetRootAsStrenTicketFuseMaterialTable(ByteBuffer _bb, StrenTicketFuseMaterialTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public StrenTicketFuseMaterialTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Material { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetMaterialBytes() { return __p.__vector_as_arraysegment(6); }
  public string PickCondOfStrenLv { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetPickCondOfStrenLvBytes() { return __p.__vector_as_arraysegment(8); }

  public static Offset<StrenTicketFuseMaterialTable> CreateStrenTicketFuseMaterialTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset MaterialOffset = default(StringOffset),
      StringOffset PickCondOfStrenLvOffset = default(StringOffset)) {
    builder.StartObject(3);
    StrenTicketFuseMaterialTable.AddPickCondOfStrenLv(builder, PickCondOfStrenLvOffset);
    StrenTicketFuseMaterialTable.AddMaterial(builder, MaterialOffset);
    StrenTicketFuseMaterialTable.AddID(builder, ID);
    return StrenTicketFuseMaterialTable.EndStrenTicketFuseMaterialTable(builder);
  }

  public static void StartStrenTicketFuseMaterialTable(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddMaterial(FlatBufferBuilder builder, StringOffset MaterialOffset) { builder.AddOffset(1, MaterialOffset.Value, 0); }
  public static void AddPickCondOfStrenLv(FlatBufferBuilder builder, StringOffset PickCondOfStrenLvOffset) { builder.AddOffset(2, PickCondOfStrenLvOffset.Value, 0); }
  public static Offset<StrenTicketFuseMaterialTable> EndStrenTicketFuseMaterialTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<StrenTicketFuseMaterialTable>(o);
  }
  public static void FinishStrenTicketFuseMaterialTableBuffer(FlatBufferBuilder builder, Offset<StrenTicketFuseMaterialTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

