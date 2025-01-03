// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class DigMapTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 2096015056,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static DigMapTable GetRootAsDigMapTable(ByteBuffer _bb) { return GetRootAsDigMapTable(_bb, new DigMapTable()); }
  public static DigMapTable GetRootAsDigMapTable(ByteBuffer _bb, DigMapTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public DigMapTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public int GoldDigMinNum { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int GoldDigMaxNum { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int GoldRefreshHour { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SilverDigMinNum { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SilverDigMaxNum { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SilverRefreshHour { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DigMaxNum { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string MapResPath { get { int o = __p.__offset(22); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetMapResPathBytes() { return __p.__vector_as_arraysegment(22); }
  public string AtlasResPath { get { int o = __p.__offset(24); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetAtlasResPathBytes() { return __p.__vector_as_arraysegment(24); }
  public string MapRouteResPath { get { int o = __p.__offset(26); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetMapRouteResPathBytes() { return __p.__vector_as_arraysegment(26); }

  public static Offset<DigMapTable> CreateDigMapTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      int GoldDigMinNum = 0,
      int GoldDigMaxNum = 0,
      int GoldRefreshHour = 0,
      int SilverDigMinNum = 0,
      int SilverDigMaxNum = 0,
      int SilverRefreshHour = 0,
      int DigMaxNum = 0,
      StringOffset MapResPathOffset = default(StringOffset),
      StringOffset AtlasResPathOffset = default(StringOffset),
      StringOffset MapRouteResPathOffset = default(StringOffset)) {
    builder.StartObject(12);
    DigMapTable.AddMapRouteResPath(builder, MapRouteResPathOffset);
    DigMapTable.AddAtlasResPath(builder, AtlasResPathOffset);
    DigMapTable.AddMapResPath(builder, MapResPathOffset);
    DigMapTable.AddDigMaxNum(builder, DigMaxNum);
    DigMapTable.AddSilverRefreshHour(builder, SilverRefreshHour);
    DigMapTable.AddSilverDigMaxNum(builder, SilverDigMaxNum);
    DigMapTable.AddSilverDigMinNum(builder, SilverDigMinNum);
    DigMapTable.AddGoldRefreshHour(builder, GoldRefreshHour);
    DigMapTable.AddGoldDigMaxNum(builder, GoldDigMaxNum);
    DigMapTable.AddGoldDigMinNum(builder, GoldDigMinNum);
    DigMapTable.AddName(builder, NameOffset);
    DigMapTable.AddID(builder, ID);
    return DigMapTable.EndDigMapTable(builder);
  }

  public static void StartDigMapTable(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddGoldDigMinNum(FlatBufferBuilder builder, int GoldDigMinNum) { builder.AddInt(2, GoldDigMinNum, 0); }
  public static void AddGoldDigMaxNum(FlatBufferBuilder builder, int GoldDigMaxNum) { builder.AddInt(3, GoldDigMaxNum, 0); }
  public static void AddGoldRefreshHour(FlatBufferBuilder builder, int GoldRefreshHour) { builder.AddInt(4, GoldRefreshHour, 0); }
  public static void AddSilverDigMinNum(FlatBufferBuilder builder, int SilverDigMinNum) { builder.AddInt(5, SilverDigMinNum, 0); }
  public static void AddSilverDigMaxNum(FlatBufferBuilder builder, int SilverDigMaxNum) { builder.AddInt(6, SilverDigMaxNum, 0); }
  public static void AddSilverRefreshHour(FlatBufferBuilder builder, int SilverRefreshHour) { builder.AddInt(7, SilverRefreshHour, 0); }
  public static void AddDigMaxNum(FlatBufferBuilder builder, int DigMaxNum) { builder.AddInt(8, DigMaxNum, 0); }
  public static void AddMapResPath(FlatBufferBuilder builder, StringOffset MapResPathOffset) { builder.AddOffset(9, MapResPathOffset.Value, 0); }
  public static void AddAtlasResPath(FlatBufferBuilder builder, StringOffset AtlasResPathOffset) { builder.AddOffset(10, AtlasResPathOffset.Value, 0); }
  public static void AddMapRouteResPath(FlatBufferBuilder builder, StringOffset MapRouteResPathOffset) { builder.AddOffset(11, MapRouteResPathOffset.Value, 0); }
  public static Offset<DigMapTable> EndDigMapTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DigMapTable>(o);
  }
  public static void FinishDigMapTableBuffer(FlatBufferBuilder builder, Offset<DigMapTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

