// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class RedPacketTable : IFlatbufferObject
{
public enum eType : int
{
 Type_None = 0,
 GUILD = 1,
 NEW_YEAR = 2,
};

public enum eSubType : int
{
 SubType_None = 0,
 Buy = 1,
 System = 2,
};

public enum eThirdType : int
{
 INVALID = 0,
 GUILD_ALL = 1,
 GUILD_BATTLE = 2,
 GUILD_CROSS_BATTLE = 3,
 GUILD_DUNGEON = 4,
};

public enum eCrypt : int
{
 code = -860454163,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static RedPacketTable GetRootAsRedPacketTable(ByteBuffer _bb) { return GetRootAsRedPacketTable(_bb, new RedPacketTable()); }
  public static RedPacketTable GetRootAsRedPacketTable(ByteBuffer _bb, RedPacketTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public RedPacketTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Desc { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDescBytes() { return __p.__vector_as_arraysegment(6); }
  public ProtoTable.RedPacketTable.eType Type { get { int o = __p.__offset(8); return o != 0 ? (ProtoTable.RedPacketTable.eType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.RedPacketTable.eType.Type_None; } }
  public int TotalMoney { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int NumArray(int j) { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int NumLength { get { int o = __p.__offset(12); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetNumBytes() { return __p.__vector_as_arraysegment(12); }
 private FlatBufferArray<int> NumValue;
 public FlatBufferArray<int>  Num
 {
  get{
  if (NumValue == null)
  {
    NumValue = new FlatBufferArray<int>(this.NumArray, this.NumLength);
  }
  return NumValue;}
 }
  public int MinMoney { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MaxMoney { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int CostMoneyID { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int GetMoneyID { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public ProtoTable.RedPacketTable.eSubType SubType { get { int o = __p.__offset(22); return o != 0 ? (ProtoTable.RedPacketTable.eSubType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.RedPacketTable.eSubType.SubType_None; } }
  public ProtoTable.RedPacketTable.eThirdType ThirdType { get { int o = __p.__offset(24); return o != 0 ? (ProtoTable.RedPacketTable.eThirdType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.RedPacketTable.eThirdType.INVALID; } }

  public static Offset<RedPacketTable> CreateRedPacketTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset DescOffset = default(StringOffset),
      ProtoTable.RedPacketTable.eType Type = ProtoTable.RedPacketTable.eType.Type_None,
      int TotalMoney = 0,
      VectorOffset NumOffset = default(VectorOffset),
      int MinMoney = 0,
      int MaxMoney = 0,
      int CostMoneyID = 0,
      int GetMoneyID = 0,
      ProtoTable.RedPacketTable.eSubType SubType = ProtoTable.RedPacketTable.eSubType.SubType_None,
      ProtoTable.RedPacketTable.eThirdType ThirdType = ProtoTable.RedPacketTable.eThirdType.INVALID) {
    builder.StartObject(11);
    RedPacketTable.AddThirdType(builder, ThirdType);
    RedPacketTable.AddSubType(builder, SubType);
    RedPacketTable.AddGetMoneyID(builder, GetMoneyID);
    RedPacketTable.AddCostMoneyID(builder, CostMoneyID);
    RedPacketTable.AddMaxMoney(builder, MaxMoney);
    RedPacketTable.AddMinMoney(builder, MinMoney);
    RedPacketTable.AddNum(builder, NumOffset);
    RedPacketTable.AddTotalMoney(builder, TotalMoney);
    RedPacketTable.AddType(builder, Type);
    RedPacketTable.AddDesc(builder, DescOffset);
    RedPacketTable.AddID(builder, ID);
    return RedPacketTable.EndRedPacketTable(builder);
  }

  public static void StartRedPacketTable(FlatBufferBuilder builder) { builder.StartObject(11); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset DescOffset) { builder.AddOffset(1, DescOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, ProtoTable.RedPacketTable.eType Type) { builder.AddInt(2, (int)Type, 0); }
  public static void AddTotalMoney(FlatBufferBuilder builder, int TotalMoney) { builder.AddInt(3, TotalMoney, 0); }
  public static void AddNum(FlatBufferBuilder builder, VectorOffset NumOffset) { builder.AddOffset(4, NumOffset.Value, 0); }
  public static VectorOffset CreateNumVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartNumVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMinMoney(FlatBufferBuilder builder, int MinMoney) { builder.AddInt(5, MinMoney, 0); }
  public static void AddMaxMoney(FlatBufferBuilder builder, int MaxMoney) { builder.AddInt(6, MaxMoney, 0); }
  public static void AddCostMoneyID(FlatBufferBuilder builder, int CostMoneyID) { builder.AddInt(7, CostMoneyID, 0); }
  public static void AddGetMoneyID(FlatBufferBuilder builder, int GetMoneyID) { builder.AddInt(8, GetMoneyID, 0); }
  public static void AddSubType(FlatBufferBuilder builder, ProtoTable.RedPacketTable.eSubType SubType) { builder.AddInt(9, (int)SubType, 0); }
  public static void AddThirdType(FlatBufferBuilder builder, ProtoTable.RedPacketTable.eThirdType ThirdType) { builder.AddInt(10, (int)ThirdType, 0); }
  public static Offset<RedPacketTable> EndRedPacketTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<RedPacketTable>(o);
  }
  public static void FinishRedPacketTableBuffer(FlatBufferBuilder builder, Offset<RedPacketTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
