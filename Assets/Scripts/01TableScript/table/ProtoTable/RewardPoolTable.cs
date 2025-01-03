// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class RewardPoolTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -2044683795,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static RewardPoolTable GetRootAsRewardPoolTable(ByteBuffer _bb) { return GetRootAsRewardPoolTable(_bb, new RewardPoolTable()); }
  public static RewardPoolTable GetRootAsRewardPoolTable(ByteBuffer _bb, RewardPoolTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public RewardPoolTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DrawPrizeTableID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ItemID { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ItemNum { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int StrengthenLevel { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ItemWeight { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DrawPrizeCountLimit { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ChargeCond { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int IsImportant { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int AnnounceNum { get { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<RewardPoolTable> CreateRewardPoolTable(FlatBufferBuilder builder,
      int ID = 0,
      int DrawPrizeTableID = 0,
      int ItemID = 0,
      int ItemNum = 0,
      int StrengthenLevel = 0,
      int ItemWeight = 0,
      int DrawPrizeCountLimit = 0,
      int ChargeCond = 0,
      int IsImportant = 0,
      int AnnounceNum = 0) {
    builder.StartObject(10);
    RewardPoolTable.AddAnnounceNum(builder, AnnounceNum);
    RewardPoolTable.AddIsImportant(builder, IsImportant);
    RewardPoolTable.AddChargeCond(builder, ChargeCond);
    RewardPoolTable.AddDrawPrizeCountLimit(builder, DrawPrizeCountLimit);
    RewardPoolTable.AddItemWeight(builder, ItemWeight);
    RewardPoolTable.AddStrengthenLevel(builder, StrengthenLevel);
    RewardPoolTable.AddItemNum(builder, ItemNum);
    RewardPoolTable.AddItemID(builder, ItemID);
    RewardPoolTable.AddDrawPrizeTableID(builder, DrawPrizeTableID);
    RewardPoolTable.AddID(builder, ID);
    return RewardPoolTable.EndRewardPoolTable(builder);
  }

  public static void StartRewardPoolTable(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddDrawPrizeTableID(FlatBufferBuilder builder, int DrawPrizeTableID) { builder.AddInt(1, DrawPrizeTableID, 0); }
  public static void AddItemID(FlatBufferBuilder builder, int ItemID) { builder.AddInt(2, ItemID, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int ItemNum) { builder.AddInt(3, ItemNum, 0); }
  public static void AddStrengthenLevel(FlatBufferBuilder builder, int StrengthenLevel) { builder.AddInt(4, StrengthenLevel, 0); }
  public static void AddItemWeight(FlatBufferBuilder builder, int ItemWeight) { builder.AddInt(5, ItemWeight, 0); }
  public static void AddDrawPrizeCountLimit(FlatBufferBuilder builder, int DrawPrizeCountLimit) { builder.AddInt(6, DrawPrizeCountLimit, 0); }
  public static void AddChargeCond(FlatBufferBuilder builder, int ChargeCond) { builder.AddInt(7, ChargeCond, 0); }
  public static void AddIsImportant(FlatBufferBuilder builder, int IsImportant) { builder.AddInt(8, IsImportant, 0); }
  public static void AddAnnounceNum(FlatBufferBuilder builder, int AnnounceNum) { builder.AddInt(9, AnnounceNum, 0); }
  public static Offset<RewardPoolTable> EndRewardPoolTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<RewardPoolTable>(o);
  }
  public static void FinishRewardPoolTableBuffer(FlatBufferBuilder builder, Offset<RewardPoolTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

