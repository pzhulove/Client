// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ExpeditionMapTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1661111065,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ExpeditionMapTable GetRootAsExpeditionMapTable(ByteBuffer _bb) { return GetRootAsExpeditionMapTable(_bb, new ExpeditionMapTable()); }
  public static ExpeditionMapTable GetRootAsExpeditionMapTable(ByteBuffer _bb, ExpeditionMapTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ExpeditionMapTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string MapName { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetMapNameBytes() { return __p.__vector_as_arraysegment(6); }
  public int PlayerLevelLimit { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int AdventureTeamLevelLimit { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int RolesCapacity { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string ExpeditionTime { get { int o = __p.__offset(14); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetExpeditionTimeBytes() { return __p.__vector_as_arraysegment(14); }
  public string BackgroundPath { get { int o = __p.__offset(16); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetBackgroundPathBytes() { return __p.__vector_as_arraysegment(16); }
  public string MiniMapPath { get { int o = __p.__offset(18); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetMiniMapPathBytes() { return __p.__vector_as_arraysegment(18); }

  public static Offset<ExpeditionMapTable> CreateExpeditionMapTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset MapNameOffset = default(StringOffset),
      int PlayerLevelLimit = 0,
      int AdventureTeamLevelLimit = 0,
      int RolesCapacity = 0,
      StringOffset ExpeditionTimeOffset = default(StringOffset),
      StringOffset BackgroundPathOffset = default(StringOffset),
      StringOffset MiniMapPathOffset = default(StringOffset)) {
    builder.StartObject(8);
    ExpeditionMapTable.AddMiniMapPath(builder, MiniMapPathOffset);
    ExpeditionMapTable.AddBackgroundPath(builder, BackgroundPathOffset);
    ExpeditionMapTable.AddExpeditionTime(builder, ExpeditionTimeOffset);
    ExpeditionMapTable.AddRolesCapacity(builder, RolesCapacity);
    ExpeditionMapTable.AddAdventureTeamLevelLimit(builder, AdventureTeamLevelLimit);
    ExpeditionMapTable.AddPlayerLevelLimit(builder, PlayerLevelLimit);
    ExpeditionMapTable.AddMapName(builder, MapNameOffset);
    ExpeditionMapTable.AddID(builder, ID);
    return ExpeditionMapTable.EndExpeditionMapTable(builder);
  }

  public static void StartExpeditionMapTable(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddMapName(FlatBufferBuilder builder, StringOffset MapNameOffset) { builder.AddOffset(1, MapNameOffset.Value, 0); }
  public static void AddPlayerLevelLimit(FlatBufferBuilder builder, int PlayerLevelLimit) { builder.AddInt(2, PlayerLevelLimit, 0); }
  public static void AddAdventureTeamLevelLimit(FlatBufferBuilder builder, int AdventureTeamLevelLimit) { builder.AddInt(3, AdventureTeamLevelLimit, 0); }
  public static void AddRolesCapacity(FlatBufferBuilder builder, int RolesCapacity) { builder.AddInt(4, RolesCapacity, 0); }
  public static void AddExpeditionTime(FlatBufferBuilder builder, StringOffset ExpeditionTimeOffset) { builder.AddOffset(5, ExpeditionTimeOffset.Value, 0); }
  public static void AddBackgroundPath(FlatBufferBuilder builder, StringOffset BackgroundPathOffset) { builder.AddOffset(6, BackgroundPathOffset.Value, 0); }
  public static void AddMiniMapPath(FlatBufferBuilder builder, StringOffset MiniMapPathOffset) { builder.AddOffset(7, MiniMapPathOffset.Value, 0); }
  public static Offset<ExpeditionMapTable> EndExpeditionMapTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ExpeditionMapTable>(o);
  }
  public static void FinishExpeditionMapTableBuffer(FlatBufferBuilder builder, Offset<ExpeditionMapTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

