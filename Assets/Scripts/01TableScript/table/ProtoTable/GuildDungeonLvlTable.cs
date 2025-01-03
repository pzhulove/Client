// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class GuildDungeonLvlTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -2111160045,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static GuildDungeonLvlTable GetRootAsGuildDungeonLvlTable(ByteBuffer _bb) { return GetRootAsGuildDungeonLvlTable(_bb, new GuildDungeonLvlTable()); }
  public static GuildDungeonLvlTable GetRootAsGuildDungeonLvlTable(ByteBuffer _bb, GuildDungeonLvlTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public GuildDungeonLvlTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int dungeonType { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int dungeonLvl { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DungeonId { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int bossId { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string bossBlood { get { int o = __p.__offset(14); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetBossBloodBytes() { return __p.__vector_as_arraysegment(14); }
  public int rewardType { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string dungeonName { get { int o = __p.__offset(18); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDungeonNameBytes() { return __p.__vector_as_arraysegment(18); }
  public int dropBuff { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string iconPath { get { int o = __p.__offset(22); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetIconPathBytes() { return __p.__vector_as_arraysegment(22); }
  public int oneStarDamage { get { int o = __p.__offset(24); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int twoStarDamage { get { int o = __p.__offset(26); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int threeStarDamage { get { int o = __p.__offset(28); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string playingDesc { get { int o = __p.__offset(30); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetPlayingDescBytes() { return __p.__vector_as_arraysegment(30); }
  public string weaknessDesc { get { int o = __p.__offset(32); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetWeaknessDescBytes() { return __p.__vector_as_arraysegment(32); }
  public string recommendDesc { get { int o = __p.__offset(34); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetRecommendDescBytes() { return __p.__vector_as_arraysegment(34); }
  public string miniIconPath { get { int o = __p.__offset(36); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetMiniIconPathBytes() { return __p.__vector_as_arraysegment(36); }

  public static Offset<GuildDungeonLvlTable> CreateGuildDungeonLvlTable(FlatBufferBuilder builder,
      int ID = 0,
      int dungeonType = 0,
      int dungeonLvl = 0,
      int DungeonId = 0,
      int bossId = 0,
      StringOffset bossBloodOffset = default(StringOffset),
      int rewardType = 0,
      StringOffset dungeonNameOffset = default(StringOffset),
      int dropBuff = 0,
      StringOffset iconPathOffset = default(StringOffset),
      int oneStarDamage = 0,
      int twoStarDamage = 0,
      int threeStarDamage = 0,
      StringOffset playingDescOffset = default(StringOffset),
      StringOffset weaknessDescOffset = default(StringOffset),
      StringOffset recommendDescOffset = default(StringOffset),
      StringOffset miniIconPathOffset = default(StringOffset)) {
    builder.StartObject(17);
    GuildDungeonLvlTable.AddMiniIconPath(builder, miniIconPathOffset);
    GuildDungeonLvlTable.AddRecommendDesc(builder, recommendDescOffset);
    GuildDungeonLvlTable.AddWeaknessDesc(builder, weaknessDescOffset);
    GuildDungeonLvlTable.AddPlayingDesc(builder, playingDescOffset);
    GuildDungeonLvlTable.AddThreeStarDamage(builder, threeStarDamage);
    GuildDungeonLvlTable.AddTwoStarDamage(builder, twoStarDamage);
    GuildDungeonLvlTable.AddOneStarDamage(builder, oneStarDamage);
    GuildDungeonLvlTable.AddIconPath(builder, iconPathOffset);
    GuildDungeonLvlTable.AddDropBuff(builder, dropBuff);
    GuildDungeonLvlTable.AddDungeonName(builder, dungeonNameOffset);
    GuildDungeonLvlTable.AddRewardType(builder, rewardType);
    GuildDungeonLvlTable.AddBossBlood(builder, bossBloodOffset);
    GuildDungeonLvlTable.AddBossId(builder, bossId);
    GuildDungeonLvlTable.AddDungeonId(builder, DungeonId);
    GuildDungeonLvlTable.AddDungeonLvl(builder, dungeonLvl);
    GuildDungeonLvlTable.AddDungeonType(builder, dungeonType);
    GuildDungeonLvlTable.AddID(builder, ID);
    return GuildDungeonLvlTable.EndGuildDungeonLvlTable(builder);
  }

  public static void StartGuildDungeonLvlTable(FlatBufferBuilder builder) { builder.StartObject(17); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddDungeonType(FlatBufferBuilder builder, int dungeonType) { builder.AddInt(1, dungeonType, 0); }
  public static void AddDungeonLvl(FlatBufferBuilder builder, int dungeonLvl) { builder.AddInt(2, dungeonLvl, 0); }
  public static void AddDungeonId(FlatBufferBuilder builder, int DungeonId) { builder.AddInt(3, DungeonId, 0); }
  public static void AddBossId(FlatBufferBuilder builder, int bossId) { builder.AddInt(4, bossId, 0); }
  public static void AddBossBlood(FlatBufferBuilder builder, StringOffset bossBloodOffset) { builder.AddOffset(5, bossBloodOffset.Value, 0); }
  public static void AddRewardType(FlatBufferBuilder builder, int rewardType) { builder.AddInt(6, rewardType, 0); }
  public static void AddDungeonName(FlatBufferBuilder builder, StringOffset dungeonNameOffset) { builder.AddOffset(7, dungeonNameOffset.Value, 0); }
  public static void AddDropBuff(FlatBufferBuilder builder, int dropBuff) { builder.AddInt(8, dropBuff, 0); }
  public static void AddIconPath(FlatBufferBuilder builder, StringOffset iconPathOffset) { builder.AddOffset(9, iconPathOffset.Value, 0); }
  public static void AddOneStarDamage(FlatBufferBuilder builder, int oneStarDamage) { builder.AddInt(10, oneStarDamage, 0); }
  public static void AddTwoStarDamage(FlatBufferBuilder builder, int twoStarDamage) { builder.AddInt(11, twoStarDamage, 0); }
  public static void AddThreeStarDamage(FlatBufferBuilder builder, int threeStarDamage) { builder.AddInt(12, threeStarDamage, 0); }
  public static void AddPlayingDesc(FlatBufferBuilder builder, StringOffset playingDescOffset) { builder.AddOffset(13, playingDescOffset.Value, 0); }
  public static void AddWeaknessDesc(FlatBufferBuilder builder, StringOffset weaknessDescOffset) { builder.AddOffset(14, weaknessDescOffset.Value, 0); }
  public static void AddRecommendDesc(FlatBufferBuilder builder, StringOffset recommendDescOffset) { builder.AddOffset(15, recommendDescOffset.Value, 0); }
  public static void AddMiniIconPath(FlatBufferBuilder builder, StringOffset miniIconPathOffset) { builder.AddOffset(16, miniIconPathOffset.Value, 0); }
  public static Offset<GuildDungeonLvlTable> EndGuildDungeonLvlTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GuildDungeonLvlTable>(o);
  }
  public static void FinishGuildDungeonLvlTableBuffer(FlatBufferBuilder builder, Offset<GuildDungeonLvlTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

