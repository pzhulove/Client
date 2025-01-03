// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class FatigueMakeUp : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 1519808350,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static FatigueMakeUp GetRootAsFatigueMakeUp(ByteBuffer _bb) { return GetRootAsFatigueMakeUp(_bb, new FatigueMakeUp()); }
  public static FatigueMakeUp GetRootAsFatigueMakeUp(ByteBuffer _bb, FatigueMakeUp obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public FatigueMakeUp __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int LowEXP { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int LowNeedMoneyItem { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int HiEXP { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int HiNeedMoneyItem { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int FatigueMax { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int VipFatigueMax { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int VipLevel { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<FatigueMakeUp> CreateFatigueMakeUp(FlatBufferBuilder builder,
      int ID = 0,
      int LowEXP = 0,
      int LowNeedMoneyItem = 0,
      int HiEXP = 0,
      int HiNeedMoneyItem = 0,
      int FatigueMax = 0,
      int VipFatigueMax = 0,
      int VipLevel = 0) {
    builder.StartObject(8);
    FatigueMakeUp.AddVipLevel(builder, VipLevel);
    FatigueMakeUp.AddVipFatigueMax(builder, VipFatigueMax);
    FatigueMakeUp.AddFatigueMax(builder, FatigueMax);
    FatigueMakeUp.AddHiNeedMoneyItem(builder, HiNeedMoneyItem);
    FatigueMakeUp.AddHiEXP(builder, HiEXP);
    FatigueMakeUp.AddLowNeedMoneyItem(builder, LowNeedMoneyItem);
    FatigueMakeUp.AddLowEXP(builder, LowEXP);
    FatigueMakeUp.AddID(builder, ID);
    return FatigueMakeUp.EndFatigueMakeUp(builder);
  }

  public static void StartFatigueMakeUp(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddLowEXP(FlatBufferBuilder builder, int LowEXP) { builder.AddInt(1, LowEXP, 0); }
  public static void AddLowNeedMoneyItem(FlatBufferBuilder builder, int LowNeedMoneyItem) { builder.AddInt(2, LowNeedMoneyItem, 0); }
  public static void AddHiEXP(FlatBufferBuilder builder, int HiEXP) { builder.AddInt(3, HiEXP, 0); }
  public static void AddHiNeedMoneyItem(FlatBufferBuilder builder, int HiNeedMoneyItem) { builder.AddInt(4, HiNeedMoneyItem, 0); }
  public static void AddFatigueMax(FlatBufferBuilder builder, int FatigueMax) { builder.AddInt(5, FatigueMax, 0); }
  public static void AddVipFatigueMax(FlatBufferBuilder builder, int VipFatigueMax) { builder.AddInt(6, VipFatigueMax, 0); }
  public static void AddVipLevel(FlatBufferBuilder builder, int VipLevel) { builder.AddInt(7, VipLevel, 0); }
  public static Offset<FatigueMakeUp> EndFatigueMakeUp(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FatigueMakeUp>(o);
  }
  public static void FinishFatigueMakeUpBuffer(FlatBufferBuilder builder, Offset<FatigueMakeUp> offset) { builder.Finish(offset.Value); }
};


}


#endif

