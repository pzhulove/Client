// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class MagicCardUpgradeTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -498303197,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static MagicCardUpgradeTable GetRootAsMagicCardUpgradeTable(ByteBuffer _bb) { return GetRootAsMagicCardUpgradeTable(_bb, new MagicCardUpgradeTable()); }
  public static MagicCardUpgradeTable GetRootAsMagicCardUpgradeTable(ByteBuffer _bb, MagicCardUpgradeTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public MagicCardUpgradeTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MagicCardTableID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MaxNum { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SameColorAndStageCardAddRate { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SameCardAddRate { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int CostItemId { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int CostNum { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<MagicCardUpgradeTable> CreateMagicCardUpgradeTable(FlatBufferBuilder builder,
      int ID = 0,
      int MagicCardTableID = 0,
      int Level = 0,
      int MaxNum = 0,
      int SameColorAndStageCardAddRate = 0,
      int SameCardAddRate = 0,
      int CostItemId = 0,
      int CostNum = 0) {
    builder.StartObject(8);
    MagicCardUpgradeTable.AddCostNum(builder, CostNum);
    MagicCardUpgradeTable.AddCostItemId(builder, CostItemId);
    MagicCardUpgradeTable.AddSameCardAddRate(builder, SameCardAddRate);
    MagicCardUpgradeTable.AddSameColorAndStageCardAddRate(builder, SameColorAndStageCardAddRate);
    MagicCardUpgradeTable.AddMaxNum(builder, MaxNum);
    MagicCardUpgradeTable.AddLevel(builder, Level);
    MagicCardUpgradeTable.AddMagicCardTableID(builder, MagicCardTableID);
    MagicCardUpgradeTable.AddID(builder, ID);
    return MagicCardUpgradeTable.EndMagicCardUpgradeTable(builder);
  }

  public static void StartMagicCardUpgradeTable(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddMagicCardTableID(FlatBufferBuilder builder, int MagicCardTableID) { builder.AddInt(1, MagicCardTableID, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int Level) { builder.AddInt(2, Level, 0); }
  public static void AddMaxNum(FlatBufferBuilder builder, int MaxNum) { builder.AddInt(3, MaxNum, 0); }
  public static void AddSameColorAndStageCardAddRate(FlatBufferBuilder builder, int SameColorAndStageCardAddRate) { builder.AddInt(4, SameColorAndStageCardAddRate, 0); }
  public static void AddSameCardAddRate(FlatBufferBuilder builder, int SameCardAddRate) { builder.AddInt(5, SameCardAddRate, 0); }
  public static void AddCostItemId(FlatBufferBuilder builder, int CostItemId) { builder.AddInt(6, CostItemId, 0); }
  public static void AddCostNum(FlatBufferBuilder builder, int CostNum) { builder.AddInt(7, CostNum, 0); }
  public static Offset<MagicCardUpgradeTable> EndMagicCardUpgradeTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MagicCardUpgradeTable>(o);
  }
  public static void FinishMagicCardUpgradeTableBuffer(FlatBufferBuilder builder, Offset<MagicCardUpgradeTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
