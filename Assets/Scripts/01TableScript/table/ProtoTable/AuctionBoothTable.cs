// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class AuctionBoothTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1347207501,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static AuctionBoothTable GetRootAsAuctionBoothTable(ByteBuffer _bb) { return GetRootAsAuctionBoothTable(_bb, new AuctionBoothTable()); }
  public static AuctionBoothTable GetRootAsAuctionBoothTable(ByteBuffer _bb, AuctionBoothTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public AuctionBoothTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int CostItemID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Num { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<AuctionBoothTable> CreateAuctionBoothTable(FlatBufferBuilder builder,
      int ID = 0,
      int CostItemID = 0,
      int Num = 0) {
    builder.StartObject(3);
    AuctionBoothTable.AddNum(builder, Num);
    AuctionBoothTable.AddCostItemID(builder, CostItemID);
    AuctionBoothTable.AddID(builder, ID);
    return AuctionBoothTable.EndAuctionBoothTable(builder);
  }

  public static void StartAuctionBoothTable(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddCostItemID(FlatBufferBuilder builder, int CostItemID) { builder.AddInt(1, CostItemID, 0); }
  public static void AddNum(FlatBufferBuilder builder, int Num) { builder.AddInt(2, Num, 0); }
  public static Offset<AuctionBoothTable> EndAuctionBoothTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AuctionBoothTable>(o);
  }
  public static void FinishAuctionBoothTableBuffer(FlatBufferBuilder builder, Offset<AuctionBoothTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
