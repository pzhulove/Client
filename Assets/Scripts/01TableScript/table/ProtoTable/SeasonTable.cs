// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class SeasonTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -767684625,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static SeasonTable GetRootAsSeasonTable(ByteBuffer _bb) { return GetRootAsSeasonTable(_bb, new SeasonTable()); }
  public static SeasonTable GetRootAsSeasonTable(ByteBuffer _bb, SeasonTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public SeasonTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SeasonEventType { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SeasonCyclesType { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int StartDay { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int StartTime { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<SeasonTable> CreateSeasonTable(FlatBufferBuilder builder,
      int ID = 0,
      int SeasonEventType = 0,
      int SeasonCyclesType = 0,
      int StartDay = 0,
      int StartTime = 0) {
    builder.StartObject(5);
    SeasonTable.AddStartTime(builder, StartTime);
    SeasonTable.AddStartDay(builder, StartDay);
    SeasonTable.AddSeasonCyclesType(builder, SeasonCyclesType);
    SeasonTable.AddSeasonEventType(builder, SeasonEventType);
    SeasonTable.AddID(builder, ID);
    return SeasonTable.EndSeasonTable(builder);
  }

  public static void StartSeasonTable(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddSeasonEventType(FlatBufferBuilder builder, int SeasonEventType) { builder.AddInt(1, SeasonEventType, 0); }
  public static void AddSeasonCyclesType(FlatBufferBuilder builder, int SeasonCyclesType) { builder.AddInt(2, SeasonCyclesType, 0); }
  public static void AddStartDay(FlatBufferBuilder builder, int StartDay) { builder.AddInt(3, StartDay, 0); }
  public static void AddStartTime(FlatBufferBuilder builder, int StartTime) { builder.AddInt(4, StartTime, 0); }
  public static Offset<SeasonTable> EndSeasonTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SeasonTable>(o);
  }
  public static void FinishSeasonTableBuffer(FlatBufferBuilder builder, Offset<SeasonTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
