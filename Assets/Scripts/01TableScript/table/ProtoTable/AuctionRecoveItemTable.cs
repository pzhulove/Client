// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class AuctionRecoveItemTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -359186944,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static AuctionRecoveItemTable GetRootAsAuctionRecoveItemTable(ByteBuffer _bb) { return GetRootAsAuctionRecoveItemTable(_bb, new AuctionRecoveItemTable()); }
  public static AuctionRecoveItemTable GetRootAsAuctionRecoveItemTable(ByteBuffer _bb, AuctionRecoveItemTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public AuctionRecoveItemTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public int SysRecoPriceRate { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<AuctionRecoveItemTable> CreateAuctionRecoveItemTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      int SysRecoPriceRate = 0) {
    builder.StartObject(3);
    AuctionRecoveItemTable.AddSysRecoPriceRate(builder, SysRecoPriceRate);
    AuctionRecoveItemTable.AddName(builder, NameOffset);
    AuctionRecoveItemTable.AddID(builder, ID);
    return AuctionRecoveItemTable.EndAuctionRecoveItemTable(builder);
  }

  public static void StartAuctionRecoveItemTable(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddSysRecoPriceRate(FlatBufferBuilder builder, int SysRecoPriceRate) { builder.AddInt(2, SysRecoPriceRate, 0); }
  public static Offset<AuctionRecoveItemTable> EndAuctionRecoveItemTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AuctionRecoveItemTable>(o);
  }
  public static void FinishAuctionRecoveItemTableBuffer(FlatBufferBuilder builder, Offset<AuctionRecoveItemTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

