// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ChiJiGiftPackTable : IFlatbufferObject
{
public enum eFilterType : int
{
 None = 0,
 Job = 1,
 Random = 2,
 Custom = 3,
 CustomWithJob = 4,
 ChiJiEquip = 5,
};

public enum eCrypt : int
{
 code = -829597410,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ChiJiGiftPackTable GetRootAsChiJiGiftPackTable(ByteBuffer _bb) { return GetRootAsChiJiGiftPackTable(_bb, new ChiJiGiftPackTable()); }
  public static ChiJiGiftPackTable GetRootAsChiJiGiftPackTable(ByteBuffer _bb, ChiJiGiftPackTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ChiJiGiftPackTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public ProtoTable.ChiJiGiftPackTable.eFilterType FilterType { get { int o = __p.__offset(6); return o != 0 ? (ProtoTable.ChiJiGiftPackTable.eFilterType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.ChiJiGiftPackTable.eFilterType.None; } }
  public int FilterCount { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int UIType { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<ChiJiGiftPackTable> CreateChiJiGiftPackTable(FlatBufferBuilder builder,
      int ID = 0,
      ProtoTable.ChiJiGiftPackTable.eFilterType FilterType = ProtoTable.ChiJiGiftPackTable.eFilterType.None,
      int FilterCount = 0,
      int UIType = 0) {
    builder.StartObject(4);
    ChiJiGiftPackTable.AddUIType(builder, UIType);
    ChiJiGiftPackTable.AddFilterCount(builder, FilterCount);
    ChiJiGiftPackTable.AddFilterType(builder, FilterType);
    ChiJiGiftPackTable.AddID(builder, ID);
    return ChiJiGiftPackTable.EndChiJiGiftPackTable(builder);
  }

  public static void StartChiJiGiftPackTable(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddFilterType(FlatBufferBuilder builder, ProtoTable.ChiJiGiftPackTable.eFilterType FilterType) { builder.AddInt(1, (int)FilterType, 0); }
  public static void AddFilterCount(FlatBufferBuilder builder, int FilterCount) { builder.AddInt(2, FilterCount, 0); }
  public static void AddUIType(FlatBufferBuilder builder, int UIType) { builder.AddInt(3, UIType, 0); }
  public static Offset<ChiJiGiftPackTable> EndChiJiGiftPackTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ChiJiGiftPackTable>(o);
  }
  public static void FinishChiJiGiftPackTableBuffer(FlatBufferBuilder builder, Offset<ChiJiGiftPackTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
