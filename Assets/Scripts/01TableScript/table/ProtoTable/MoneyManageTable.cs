// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class MoneyManageTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -877491173,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static MoneyManageTable GetRootAsMoneyManageTable(ByteBuffer _bb) { return GetRootAsMoneyManageTable(_bb, new MoneyManageTable()); }
  public static MoneyManageTable GetRootAsMoneyManageTable(ByteBuffer _bb, MoneyManageTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public MoneyManageTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string ItemRewardArray(int j) { int o = __p.__offset(8); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int ItemRewardLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> ItemRewardValue;
 public FlatBufferArray<string>  ItemReward
 {
  get{
  if (ItemRewardValue == null)
  {
    ItemRewardValue = new FlatBufferArray<string>(this.ItemRewardArray, this.ItemRewardLength);
  }
  return ItemRewardValue;}
 }

  public static Offset<MoneyManageTable> CreateMoneyManageTable(FlatBufferBuilder builder,
      int ID = 0,
      int Level = 0,
      VectorOffset ItemRewardOffset = default(VectorOffset)) {
    builder.StartObject(3);
    MoneyManageTable.AddItemReward(builder, ItemRewardOffset);
    MoneyManageTable.AddLevel(builder, Level);
    MoneyManageTable.AddID(builder, ID);
    return MoneyManageTable.EndMoneyManageTable(builder);
  }

  public static void StartMoneyManageTable(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int Level) { builder.AddInt(1, Level, 0); }
  public static void AddItemReward(FlatBufferBuilder builder, VectorOffset ItemRewardOffset) { builder.AddOffset(2, ItemRewardOffset.Value, 0); }
  public static VectorOffset CreateItemRewardVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartItemRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<MoneyManageTable> EndMoneyManageTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MoneyManageTable>(o);
  }
  public static void FinishMoneyManageTableBuffer(FlatBufferBuilder builder, Offset<MoneyManageTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
