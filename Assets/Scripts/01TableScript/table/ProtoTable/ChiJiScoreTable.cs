// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ChiJiScoreTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 880083663,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ChiJiScoreTable GetRootAsChiJiScoreTable(ByteBuffer _bb) { return GetRootAsChiJiScoreTable(_bb, new ChiJiScoreTable()); }
  public static ChiJiScoreTable GetRootAsChiJiScoreTable(ByteBuffer _bb, ChiJiScoreTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ChiJiScoreTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Score { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<ChiJiScoreTable> CreateChiJiScoreTable(FlatBufferBuilder builder,
      int ID = 0,
      int Score = 0) {
    builder.StartObject(2);
    ChiJiScoreTable.AddScore(builder, Score);
    ChiJiScoreTable.AddID(builder, ID);
    return ChiJiScoreTable.EndChiJiScoreTable(builder);
  }

  public static void StartChiJiScoreTable(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddScore(FlatBufferBuilder builder, int Score) { builder.AddInt(1, Score, 0); }
  public static Offset<ChiJiScoreTable> EndChiJiScoreTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ChiJiScoreTable>(o);
  }
  public static void FinishChiJiScoreTableBuffer(FlatBufferBuilder builder, Offset<ChiJiScoreTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
