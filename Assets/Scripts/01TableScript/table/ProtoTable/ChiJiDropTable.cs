// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ChiJiDropTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 598615558,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ChiJiDropTable GetRootAsChiJiDropTable(ByteBuffer _bb) { return GetRootAsChiJiDropTable(_bb, new ChiJiDropTable()); }
  public static ChiJiDropTable GetRootAsChiJiDropTable(ByteBuffer _bb, ChiJiDropTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ChiJiDropTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Name { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DropID { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<ChiJiDropTable> CreateChiJiDropTable(FlatBufferBuilder builder,
      int ID = 0,
      int Name = 0,
      int DropID = 0) {
    builder.StartObject(3);
    ChiJiDropTable.AddDropID(builder, DropID);
    ChiJiDropTable.AddName(builder, Name);
    ChiJiDropTable.AddID(builder, ID);
    return ChiJiDropTable.EndChiJiDropTable(builder);
  }

  public static void StartChiJiDropTable(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, int Name) { builder.AddInt(1, Name, 0); }
  public static void AddDropID(FlatBufferBuilder builder, int DropID) { builder.AddInt(2, DropID, 0); }
  public static Offset<ChiJiDropTable> EndChiJiDropTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ChiJiDropTable>(o);
  }
  public static void FinishChiJiDropTableBuffer(FlatBufferBuilder builder, Offset<ChiJiDropTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

