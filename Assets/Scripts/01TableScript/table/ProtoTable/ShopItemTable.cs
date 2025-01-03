// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ShopItemTable : IFlatbufferObject
{
public enum eSubType : int
{
 ST_NONE = 0,
 ST_MATERIAL = 1,
 ST_WEAPON = 2,
 ST_ARMOR = 3,
 ST_JEWELRY = 4,
 ST_COST = 5,
 ST_VALUABLE = 6,
 ST_RETINUE = 7,
 ST_TITLE = 8,
 ST_ENERGY = 9,
 ST_FASHION = 10,
 ST_ORDINARY = 11,
 ST_DAILY = 12,
 ST_GOODS = 13,
 ST_EQUIP = 14,
};

public enum eCrypt : int
{
 code = -1236346225,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ShopItemTable GetRootAsShopItemTable(ByteBuffer _bb) { return GetRootAsShopItemTable(_bb, new ShopItemTable()); }
  public static ShopItemTable GetRootAsShopItemTable(ByteBuffer _bb, ShopItemTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ShopItemTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string CommodityName { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetCommodityNameBytes() { return __p.__vector_as_arraysegment(6); }
  public int ShopID { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int UseEqualItem { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ItemID { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SortID { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public ProtoTable.ShopItemTable.eSubType SubType { get { int o = __p.__offset(16); return o != 0 ? (ProtoTable.ShopItemTable.eSubType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.ShopItemTable.eSubType.ST_NONE; } }
  public int CostItemID { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int CostNum { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string OtherCostItems { get { int o = __p.__offset(22); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetOtherCostItemsBytes() { return __p.__vector_as_arraysegment(22); }
  public int VIP { get { int o = __p.__offset(24); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int VIPLimite { get { int o = __p.__offset(26); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int NumLimite { get { int o = __p.__offset(28); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int LimiteOnce { get { int o = __p.__offset(30); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int GroupNum { get { int o = __p.__offset(32); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Weight { get { int o = __p.__offset(34); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ExLimite { get { int o = __p.__offset(36); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ExValue { get { int o = __p.__offset(38); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string OldChangeNewItemID { get { int o = __p.__offset(40); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetOldChangeNewItemIDBytes() { return __p.__vector_as_arraysegment(40); }
  public string PlayerLevelLimit { get { int o = __p.__offset(42); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetPlayerLevelLimitBytes() { return __p.__vector_as_arraysegment(42); }
  public string VipLevelLimit { get { int o = __p.__offset(44); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetVipLevelLimitBytes() { return __p.__vector_as_arraysegment(44); }
  public string DungeonHardLimit { get { int o = __p.__offset(46); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDungeonHardLimitBytes() { return __p.__vector_as_arraysegment(46); }
  public string DungeonSubTypeLimit { get { int o = __p.__offset(48); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDungeonSubTypeLimitBytes() { return __p.__vector_as_arraysegment(48); }
  public string DungeonIdLimit { get { int o = __p.__offset(50); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDungeonIdLimitBytes() { return __p.__vector_as_arraysegment(50); }
  public string DiscountRate { get { int o = __p.__offset(52); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDiscountRateBytes() { return __p.__vector_as_arraysegment(52); }
  public string DiscountRateWeight { get { int o = __p.__offset(54); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDiscountRateWeightBytes() { return __p.__vector_as_arraysegment(54); }
  public int MallGoodID { get { int o = __p.__offset(56); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int AttFit { get { int o = __p.__offset(58); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ShowLevelLimit { get { int o = __p.__offset(60); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int BuyLimit { get { int o = __p.__offset(62); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<ShopItemTable> CreateShopItemTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset CommodityNameOffset = default(StringOffset),
      int ShopID = 0,
      int UseEqualItem = 0,
      int ItemID = 0,
      int SortID = 0,
      ProtoTable.ShopItemTable.eSubType SubType = ProtoTable.ShopItemTable.eSubType.ST_NONE,
      int CostItemID = 0,
      int CostNum = 0,
      StringOffset OtherCostItemsOffset = default(StringOffset),
      int VIP = 0,
      int VIPLimite = 0,
      int NumLimite = 0,
      int LimiteOnce = 0,
      int GroupNum = 0,
      int Weight = 0,
      int ExLimite = 0,
      int ExValue = 0,
      StringOffset OldChangeNewItemIDOffset = default(StringOffset),
      StringOffset PlayerLevelLimitOffset = default(StringOffset),
      StringOffset VipLevelLimitOffset = default(StringOffset),
      StringOffset DungeonHardLimitOffset = default(StringOffset),
      StringOffset DungeonSubTypeLimitOffset = default(StringOffset),
      StringOffset DungeonIdLimitOffset = default(StringOffset),
      StringOffset DiscountRateOffset = default(StringOffset),
      StringOffset DiscountRateWeightOffset = default(StringOffset),
      int MallGoodID = 0,
      int AttFit = 0,
      int ShowLevelLimit = 0,
      int BuyLimit = 0) {
    builder.StartObject(30);
    ShopItemTable.AddBuyLimit(builder, BuyLimit);
    ShopItemTable.AddShowLevelLimit(builder, ShowLevelLimit);
    ShopItemTable.AddAttFit(builder, AttFit);
    ShopItemTable.AddMallGoodID(builder, MallGoodID);
    ShopItemTable.AddDiscountRateWeight(builder, DiscountRateWeightOffset);
    ShopItemTable.AddDiscountRate(builder, DiscountRateOffset);
    ShopItemTable.AddDungeonIdLimit(builder, DungeonIdLimitOffset);
    ShopItemTable.AddDungeonSubTypeLimit(builder, DungeonSubTypeLimitOffset);
    ShopItemTable.AddDungeonHardLimit(builder, DungeonHardLimitOffset);
    ShopItemTable.AddVipLevelLimit(builder, VipLevelLimitOffset);
    ShopItemTable.AddPlayerLevelLimit(builder, PlayerLevelLimitOffset);
    ShopItemTable.AddOldChangeNewItemID(builder, OldChangeNewItemIDOffset);
    ShopItemTable.AddExValue(builder, ExValue);
    ShopItemTable.AddExLimite(builder, ExLimite);
    ShopItemTable.AddWeight(builder, Weight);
    ShopItemTable.AddGroupNum(builder, GroupNum);
    ShopItemTable.AddLimiteOnce(builder, LimiteOnce);
    ShopItemTable.AddNumLimite(builder, NumLimite);
    ShopItemTable.AddVIPLimite(builder, VIPLimite);
    ShopItemTable.AddVIP(builder, VIP);
    ShopItemTable.AddOtherCostItems(builder, OtherCostItemsOffset);
    ShopItemTable.AddCostNum(builder, CostNum);
    ShopItemTable.AddCostItemID(builder, CostItemID);
    ShopItemTable.AddSubType(builder, SubType);
    ShopItemTable.AddSortID(builder, SortID);
    ShopItemTable.AddItemID(builder, ItemID);
    ShopItemTable.AddUseEqualItem(builder, UseEqualItem);
    ShopItemTable.AddShopID(builder, ShopID);
    ShopItemTable.AddCommodityName(builder, CommodityNameOffset);
    ShopItemTable.AddID(builder, ID);
    return ShopItemTable.EndShopItemTable(builder);
  }

  public static void StartShopItemTable(FlatBufferBuilder builder) { builder.StartObject(30); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddCommodityName(FlatBufferBuilder builder, StringOffset CommodityNameOffset) { builder.AddOffset(1, CommodityNameOffset.Value, 0); }
  public static void AddShopID(FlatBufferBuilder builder, int ShopID) { builder.AddInt(2, ShopID, 0); }
  public static void AddUseEqualItem(FlatBufferBuilder builder, int UseEqualItem) { builder.AddInt(3, UseEqualItem, 0); }
  public static void AddItemID(FlatBufferBuilder builder, int ItemID) { builder.AddInt(4, ItemID, 0); }
  public static void AddSortID(FlatBufferBuilder builder, int SortID) { builder.AddInt(5, SortID, 0); }
  public static void AddSubType(FlatBufferBuilder builder, ProtoTable.ShopItemTable.eSubType SubType) { builder.AddInt(6, (int)SubType, 0); }
  public static void AddCostItemID(FlatBufferBuilder builder, int CostItemID) { builder.AddInt(7, CostItemID, 0); }
  public static void AddCostNum(FlatBufferBuilder builder, int CostNum) { builder.AddInt(8, CostNum, 0); }
  public static void AddOtherCostItems(FlatBufferBuilder builder, StringOffset OtherCostItemsOffset) { builder.AddOffset(9, OtherCostItemsOffset.Value, 0); }
  public static void AddVIP(FlatBufferBuilder builder, int VIP) { builder.AddInt(10, VIP, 0); }
  public static void AddVIPLimite(FlatBufferBuilder builder, int VIPLimite) { builder.AddInt(11, VIPLimite, 0); }
  public static void AddNumLimite(FlatBufferBuilder builder, int NumLimite) { builder.AddInt(12, NumLimite, 0); }
  public static void AddLimiteOnce(FlatBufferBuilder builder, int LimiteOnce) { builder.AddInt(13, LimiteOnce, 0); }
  public static void AddGroupNum(FlatBufferBuilder builder, int GroupNum) { builder.AddInt(14, GroupNum, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int Weight) { builder.AddInt(15, Weight, 0); }
  public static void AddExLimite(FlatBufferBuilder builder, int ExLimite) { builder.AddInt(16, ExLimite, 0); }
  public static void AddExValue(FlatBufferBuilder builder, int ExValue) { builder.AddInt(17, ExValue, 0); }
  public static void AddOldChangeNewItemID(FlatBufferBuilder builder, StringOffset OldChangeNewItemIDOffset) { builder.AddOffset(18, OldChangeNewItemIDOffset.Value, 0); }
  public static void AddPlayerLevelLimit(FlatBufferBuilder builder, StringOffset PlayerLevelLimitOffset) { builder.AddOffset(19, PlayerLevelLimitOffset.Value, 0); }
  public static void AddVipLevelLimit(FlatBufferBuilder builder, StringOffset VipLevelLimitOffset) { builder.AddOffset(20, VipLevelLimitOffset.Value, 0); }
  public static void AddDungeonHardLimit(FlatBufferBuilder builder, StringOffset DungeonHardLimitOffset) { builder.AddOffset(21, DungeonHardLimitOffset.Value, 0); }
  public static void AddDungeonSubTypeLimit(FlatBufferBuilder builder, StringOffset DungeonSubTypeLimitOffset) { builder.AddOffset(22, DungeonSubTypeLimitOffset.Value, 0); }
  public static void AddDungeonIdLimit(FlatBufferBuilder builder, StringOffset DungeonIdLimitOffset) { builder.AddOffset(23, DungeonIdLimitOffset.Value, 0); }
  public static void AddDiscountRate(FlatBufferBuilder builder, StringOffset DiscountRateOffset) { builder.AddOffset(24, DiscountRateOffset.Value, 0); }
  public static void AddDiscountRateWeight(FlatBufferBuilder builder, StringOffset DiscountRateWeightOffset) { builder.AddOffset(25, DiscountRateWeightOffset.Value, 0); }
  public static void AddMallGoodID(FlatBufferBuilder builder, int MallGoodID) { builder.AddInt(26, MallGoodID, 0); }
  public static void AddAttFit(FlatBufferBuilder builder, int AttFit) { builder.AddInt(27, AttFit, 0); }
  public static void AddShowLevelLimit(FlatBufferBuilder builder, int ShowLevelLimit) { builder.AddInt(28, ShowLevelLimit, 0); }
  public static void AddBuyLimit(FlatBufferBuilder builder, int BuyLimit) { builder.AddInt(29, BuyLimit, 0); }
  public static Offset<ShopItemTable> EndShopItemTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ShopItemTable>(o);
  }
  public static void FinishShopItemTableBuffer(FlatBufferBuilder builder, Offset<ShopItemTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

