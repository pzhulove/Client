// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ChijiSpawnItemTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -730451159,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ChijiSpawnItemTable GetRootAsChijiSpawnItemTable(ByteBuffer _bb) { return GetRootAsChijiSpawnItemTable(_bb, new ChijiSpawnItemTable()); }
  public static ChijiSpawnItemTable GetRootAsChijiSpawnItemTable(ByteBuffer _bb, ChijiSpawnItemTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ChijiSpawnItemTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Times { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int PackID { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ItemID { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Name { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Weight { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MinNumber { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MaxNumber { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MapID { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<ChijiSpawnItemTable> CreateChijiSpawnItemTable(FlatBufferBuilder builder,
      int ID = 0,
      int Times = 0,
      int PackID = 0,
      int ItemID = 0,
      int Name = 0,
      int Weight = 0,
      int MinNumber = 0,
      int MaxNumber = 0,
      int MapID = 0) {
    builder.StartObject(9);
    ChijiSpawnItemTable.AddMapID(builder, MapID);
    ChijiSpawnItemTable.AddMaxNumber(builder, MaxNumber);
    ChijiSpawnItemTable.AddMinNumber(builder, MinNumber);
    ChijiSpawnItemTable.AddWeight(builder, Weight);
    ChijiSpawnItemTable.AddName(builder, Name);
    ChijiSpawnItemTable.AddItemID(builder, ItemID);
    ChijiSpawnItemTable.AddPackID(builder, PackID);
    ChijiSpawnItemTable.AddTimes(builder, Times);
    ChijiSpawnItemTable.AddID(builder, ID);
    return ChijiSpawnItemTable.EndChijiSpawnItemTable(builder);
  }

  public static void StartChijiSpawnItemTable(FlatBufferBuilder builder) { builder.StartObject(9); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddTimes(FlatBufferBuilder builder, int Times) { builder.AddInt(1, Times, 0); }
  public static void AddPackID(FlatBufferBuilder builder, int PackID) { builder.AddInt(2, PackID, 0); }
  public static void AddItemID(FlatBufferBuilder builder, int ItemID) { builder.AddInt(3, ItemID, 0); }
  public static void AddName(FlatBufferBuilder builder, int Name) { builder.AddInt(4, Name, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int Weight) { builder.AddInt(5, Weight, 0); }
  public static void AddMinNumber(FlatBufferBuilder builder, int MinNumber) { builder.AddInt(6, MinNumber, 0); }
  public static void AddMaxNumber(FlatBufferBuilder builder, int MaxNumber) { builder.AddInt(7, MaxNumber, 0); }
  public static void AddMapID(FlatBufferBuilder builder, int MapID) { builder.AddInt(8, MapID, 0); }
  public static Offset<ChijiSpawnItemTable> EndChijiSpawnItemTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ChijiSpawnItemTable>(o);
  }
  public static void FinishChijiSpawnItemTableBuffer(FlatBufferBuilder builder, Offset<ChijiSpawnItemTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
