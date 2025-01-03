// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class RandomDungeonTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 2129423425,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static RandomDungeonTable GetRootAsRandomDungeonTable(ByteBuffer _bb) { return GetRootAsRandomDungeonTable(_bb, new RandomDungeonTable()); }
  public static RandomDungeonTable GetRootAsRandomDungeonTable(ByteBuffer _bb, RandomDungeonTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public RandomDungeonTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int dungeonType { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string DungeonRoom { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDungeonRoomBytes() { return __p.__vector_as_arraysegment(8); }

  public static Offset<RandomDungeonTable> CreateRandomDungeonTable(FlatBufferBuilder builder,
      int ID = 0,
      int dungeonType = 0,
      StringOffset DungeonRoomOffset = default(StringOffset)) {
    builder.StartObject(3);
    RandomDungeonTable.AddDungeonRoom(builder, DungeonRoomOffset);
    RandomDungeonTable.AddDungeonType(builder, dungeonType);
    RandomDungeonTable.AddID(builder, ID);
    return RandomDungeonTable.EndRandomDungeonTable(builder);
  }

  public static void StartRandomDungeonTable(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddDungeonType(FlatBufferBuilder builder, int dungeonType) { builder.AddInt(1, dungeonType, 0); }
  public static void AddDungeonRoom(FlatBufferBuilder builder, StringOffset DungeonRoomOffset) { builder.AddOffset(2, DungeonRoomOffset.Value, 0); }
  public static Offset<RandomDungeonTable> EndRandomDungeonTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<RandomDungeonTable>(o);
  }
  public static void FinishRandomDungeonTableBuffer(FlatBufferBuilder builder, Offset<RandomDungeonTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

