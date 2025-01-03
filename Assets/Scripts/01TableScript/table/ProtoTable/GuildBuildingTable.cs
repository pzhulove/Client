// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class GuildBuildingTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 200448191,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static GuildBuildingTable GetRootAsGuildBuildingTable(ByteBuffer _bb) { return GetRootAsGuildBuildingTable(_bb, new GuildBuildingTable()); }
  public static GuildBuildingTable GetRootAsGuildBuildingTable(ByteBuffer _bb, GuildBuildingTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public GuildBuildingTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MainCost { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ShopCost { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int TableCost { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DungeonCost { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int StatueCost { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int BattleCost { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int WelfareCost { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int HonourCost { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int FeteCost { get { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ShopId { get { int o = __p.__offset(24); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int WelfareGiftId { get { int o = __p.__offset(26); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<GuildBuildingTable> CreateGuildBuildingTable(FlatBufferBuilder builder,
      int ID = 0,
      int MainCost = 0,
      int ShopCost = 0,
      int TableCost = 0,
      int DungeonCost = 0,
      int StatueCost = 0,
      int BattleCost = 0,
      int WelfareCost = 0,
      int HonourCost = 0,
      int FeteCost = 0,
      int ShopId = 0,
      int WelfareGiftId = 0) {
    builder.StartObject(12);
    GuildBuildingTable.AddWelfareGiftId(builder, WelfareGiftId);
    GuildBuildingTable.AddShopId(builder, ShopId);
    GuildBuildingTable.AddFeteCost(builder, FeteCost);
    GuildBuildingTable.AddHonourCost(builder, HonourCost);
    GuildBuildingTable.AddWelfareCost(builder, WelfareCost);
    GuildBuildingTable.AddBattleCost(builder, BattleCost);
    GuildBuildingTable.AddStatueCost(builder, StatueCost);
    GuildBuildingTable.AddDungeonCost(builder, DungeonCost);
    GuildBuildingTable.AddTableCost(builder, TableCost);
    GuildBuildingTable.AddShopCost(builder, ShopCost);
    GuildBuildingTable.AddMainCost(builder, MainCost);
    GuildBuildingTable.AddID(builder, ID);
    return GuildBuildingTable.EndGuildBuildingTable(builder);
  }

  public static void StartGuildBuildingTable(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddMainCost(FlatBufferBuilder builder, int MainCost) { builder.AddInt(1, MainCost, 0); }
  public static void AddShopCost(FlatBufferBuilder builder, int ShopCost) { builder.AddInt(2, ShopCost, 0); }
  public static void AddTableCost(FlatBufferBuilder builder, int TableCost) { builder.AddInt(3, TableCost, 0); }
  public static void AddDungeonCost(FlatBufferBuilder builder, int DungeonCost) { builder.AddInt(4, DungeonCost, 0); }
  public static void AddStatueCost(FlatBufferBuilder builder, int StatueCost) { builder.AddInt(5, StatueCost, 0); }
  public static void AddBattleCost(FlatBufferBuilder builder, int BattleCost) { builder.AddInt(6, BattleCost, 0); }
  public static void AddWelfareCost(FlatBufferBuilder builder, int WelfareCost) { builder.AddInt(7, WelfareCost, 0); }
  public static void AddHonourCost(FlatBufferBuilder builder, int HonourCost) { builder.AddInt(8, HonourCost, 0); }
  public static void AddFeteCost(FlatBufferBuilder builder, int FeteCost) { builder.AddInt(9, FeteCost, 0); }
  public static void AddShopId(FlatBufferBuilder builder, int ShopId) { builder.AddInt(10, ShopId, 0); }
  public static void AddWelfareGiftId(FlatBufferBuilder builder, int WelfareGiftId) { builder.AddInt(11, WelfareGiftId, 0); }
  public static Offset<GuildBuildingTable> EndGuildBuildingTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GuildBuildingTable>(o);
  }
  public static void FinishGuildBuildingTableBuffer(FlatBufferBuilder builder, Offset<GuildBuildingTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

