// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class FashionComposeRateTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -907772120,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static FashionComposeRateTable GetRootAsFashionComposeRateTable(ByteBuffer _bb) { return GetRootAsFashionComposeRateTable(_bb, new FashionComposeRateTable()); }
  public static FashionComposeRateTable GetRootAsFashionComposeRateTable(ByteBuffer _bb, FashionComposeRateTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public FashionComposeRateTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Type { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Rate { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<FashionComposeRateTable> CreateFashionComposeRateTable(FlatBufferBuilder builder,
      int ID = 0,
      int Type = 0,
      int Rate = 0) {
    builder.StartObject(3);
    FashionComposeRateTable.AddRate(builder, Rate);
    FashionComposeRateTable.AddType(builder, Type);
    FashionComposeRateTable.AddID(builder, ID);
    return FashionComposeRateTable.EndFashionComposeRateTable(builder);
  }

  public static void StartFashionComposeRateTable(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddType(FlatBufferBuilder builder, int Type) { builder.AddInt(1, Type, 0); }
  public static void AddRate(FlatBufferBuilder builder, int Rate) { builder.AddInt(2, Rate, 0); }
  public static Offset<FashionComposeRateTable> EndFashionComposeRateTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FashionComposeRateTable>(o);
  }
  public static void FinishFashionComposeRateTableBuffer(FlatBufferBuilder builder, Offset<FashionComposeRateTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

