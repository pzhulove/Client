// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class MonthSignAward : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -187394098,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static MonthSignAward GetRootAsMonthSignAward(ByteBuffer _bb) { return GetRootAsMonthSignAward(_bb, new MonthSignAward()); }
  public static MonthSignAward GetRootAsMonthSignAward(ByteBuffer _bb, MonthSignAward obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public MonthSignAward __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Month { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Day { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int VIPDouble { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ItemID { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ItemNum { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<MonthSignAward> CreateMonthSignAward(FlatBufferBuilder builder,
      int ID = 0,
      int Month = 0,
      int Day = 0,
      int VIPDouble = 0,
      int ItemID = 0,
      int ItemNum = 0) {
    builder.StartObject(6);
    MonthSignAward.AddItemNum(builder, ItemNum);
    MonthSignAward.AddItemID(builder, ItemID);
    MonthSignAward.AddVIPDouble(builder, VIPDouble);
    MonthSignAward.AddDay(builder, Day);
    MonthSignAward.AddMonth(builder, Month);
    MonthSignAward.AddID(builder, ID);
    return MonthSignAward.EndMonthSignAward(builder);
  }

  public static void StartMonthSignAward(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddMonth(FlatBufferBuilder builder, int Month) { builder.AddInt(1, Month, 0); }
  public static void AddDay(FlatBufferBuilder builder, int Day) { builder.AddInt(2, Day, 0); }
  public static void AddVIPDouble(FlatBufferBuilder builder, int VIPDouble) { builder.AddInt(3, VIPDouble, 0); }
  public static void AddItemID(FlatBufferBuilder builder, int ItemID) { builder.AddInt(4, ItemID, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int ItemNum) { builder.AddInt(5, ItemNum, 0); }
  public static Offset<MonthSignAward> EndMonthSignAward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MonthSignAward>(o);
  }
  public static void FinishMonthSignAwardBuffer(FlatBufferBuilder builder, Offset<MonthSignAward> offset) { builder.Finish(offset.Value); }
};


}


#endif

