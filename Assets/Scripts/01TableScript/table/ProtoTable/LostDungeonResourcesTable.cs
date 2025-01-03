// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class LostDungeonResourcesTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1182957717,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static LostDungeonResourcesTable GetRootAsLostDungeonResourcesTable(ByteBuffer _bb) { return GetRootAsLostDungeonResourcesTable(_bb, new LostDungeonResourcesTable()); }
  public static LostDungeonResourcesTable GetRootAsLostDungeonResourcesTable(ByteBuffer _bb, LostDungeonResourcesTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public LostDungeonResourcesTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Name { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Desc { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ResourceGroupId { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Items { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int InitRefreshTime { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int IntervalRefreshTime { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MapId { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<LostDungeonResourcesTable> CreateLostDungeonResourcesTable(FlatBufferBuilder builder,
      int ID = 0,
      int Name = 0,
      int Desc = 0,
      int ResourceGroupId = 0,
      int Items = 0,
      int InitRefreshTime = 0,
      int IntervalRefreshTime = 0,
      int MapId = 0) {
    builder.StartObject(8);
    LostDungeonResourcesTable.AddMapId(builder, MapId);
    LostDungeonResourcesTable.AddIntervalRefreshTime(builder, IntervalRefreshTime);
    LostDungeonResourcesTable.AddInitRefreshTime(builder, InitRefreshTime);
    LostDungeonResourcesTable.AddItems(builder, Items);
    LostDungeonResourcesTable.AddResourceGroupId(builder, ResourceGroupId);
    LostDungeonResourcesTable.AddDesc(builder, Desc);
    LostDungeonResourcesTable.AddName(builder, Name);
    LostDungeonResourcesTable.AddID(builder, ID);
    return LostDungeonResourcesTable.EndLostDungeonResourcesTable(builder);
  }

  public static void StartLostDungeonResourcesTable(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, int Name) { builder.AddInt(1, Name, 0); }
  public static void AddDesc(FlatBufferBuilder builder, int Desc) { builder.AddInt(2, Desc, 0); }
  public static void AddResourceGroupId(FlatBufferBuilder builder, int ResourceGroupId) { builder.AddInt(3, ResourceGroupId, 0); }
  public static void AddItems(FlatBufferBuilder builder, int Items) { builder.AddInt(4, Items, 0); }
  public static void AddInitRefreshTime(FlatBufferBuilder builder, int InitRefreshTime) { builder.AddInt(5, InitRefreshTime, 0); }
  public static void AddIntervalRefreshTime(FlatBufferBuilder builder, int IntervalRefreshTime) { builder.AddInt(6, IntervalRefreshTime, 0); }
  public static void AddMapId(FlatBufferBuilder builder, int MapId) { builder.AddInt(7, MapId, 0); }
  public static Offset<LostDungeonResourcesTable> EndLostDungeonResourcesTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostDungeonResourcesTable>(o);
  }
  public static void FinishLostDungeonResourcesTableBuffer(FlatBufferBuilder builder, Offset<LostDungeonResourcesTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

