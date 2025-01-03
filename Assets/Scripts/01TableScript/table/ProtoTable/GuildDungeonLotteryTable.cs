// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class GuildDungeonLotteryTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 17442364,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static GuildDungeonLotteryTable GetRootAsGuildDungeonLotteryTable(ByteBuffer _bb) { return GetRootAsGuildDungeonLotteryTable(_bb, new GuildDungeonLotteryTable()); }
  public static GuildDungeonLotteryTable GetRootAsGuildDungeonLotteryTable(ByteBuffer _bb, GuildDungeonLotteryTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public GuildDungeonLotteryTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int rewardGroup { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int itemID { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int itemNum { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int weight { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int isHighVal { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<GuildDungeonLotteryTable> CreateGuildDungeonLotteryTable(FlatBufferBuilder builder,
      int ID = 0,
      int rewardGroup = 0,
      int itemID = 0,
      int itemNum = 0,
      int weight = 0,
      int isHighVal = 0) {
    builder.StartObject(6);
    GuildDungeonLotteryTable.AddIsHighVal(builder, isHighVal);
    GuildDungeonLotteryTable.AddWeight(builder, weight);
    GuildDungeonLotteryTable.AddItemNum(builder, itemNum);
    GuildDungeonLotteryTable.AddItemID(builder, itemID);
    GuildDungeonLotteryTable.AddRewardGroup(builder, rewardGroup);
    GuildDungeonLotteryTable.AddID(builder, ID);
    return GuildDungeonLotteryTable.EndGuildDungeonLotteryTable(builder);
  }

  public static void StartGuildDungeonLotteryTable(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddRewardGroup(FlatBufferBuilder builder, int rewardGroup) { builder.AddInt(1, rewardGroup, 0); }
  public static void AddItemID(FlatBufferBuilder builder, int itemID) { builder.AddInt(2, itemID, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(3, itemNum, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(4, weight, 0); }
  public static void AddIsHighVal(FlatBufferBuilder builder, int isHighVal) { builder.AddInt(5, isHighVal, 0); }
  public static Offset<GuildDungeonLotteryTable> EndGuildDungeonLotteryTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GuildDungeonLotteryTable>(o);
  }
  public static void FinishGuildDungeonLotteryTableBuffer(FlatBufferBuilder builder, Offset<GuildDungeonLotteryTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

