// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class JuedouchangItemPropellingTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1702050784,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static JuedouchangItemPropellingTable GetRootAsJuedouchangItemPropellingTable(ByteBuffer _bb) { return GetRootAsJuedouchangItemPropellingTable(_bb, new JuedouchangItemPropellingTable()); }
  public static JuedouchangItemPropellingTable GetRootAsJuedouchangItemPropellingTable(ByteBuffer _bb, JuedouchangItemPropellingTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public JuedouchangItemPropellingTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public int NeedMinLevel { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int NeedMaxLevel { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ItemLevel { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<JuedouchangItemPropellingTable> CreateJuedouchangItemPropellingTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      int NeedMinLevel = 0,
      int NeedMaxLevel = 0,
      int ItemLevel = 0) {
    builder.StartObject(5);
    JuedouchangItemPropellingTable.AddItemLevel(builder, ItemLevel);
    JuedouchangItemPropellingTable.AddNeedMaxLevel(builder, NeedMaxLevel);
    JuedouchangItemPropellingTable.AddNeedMinLevel(builder, NeedMinLevel);
    JuedouchangItemPropellingTable.AddName(builder, NameOffset);
    JuedouchangItemPropellingTable.AddID(builder, ID);
    return JuedouchangItemPropellingTable.EndJuedouchangItemPropellingTable(builder);
  }

  public static void StartJuedouchangItemPropellingTable(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddNeedMinLevel(FlatBufferBuilder builder, int NeedMinLevel) { builder.AddInt(2, NeedMinLevel, 0); }
  public static void AddNeedMaxLevel(FlatBufferBuilder builder, int NeedMaxLevel) { builder.AddInt(3, NeedMaxLevel, 0); }
  public static void AddItemLevel(FlatBufferBuilder builder, int ItemLevel) { builder.AddInt(4, ItemLevel, 0); }
  public static Offset<JuedouchangItemPropellingTable> EndJuedouchangItemPropellingTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<JuedouchangItemPropellingTable>(o);
  }
  public static void FinishJuedouchangItemPropellingTableBuffer(FlatBufferBuilder builder, Offset<JuedouchangItemPropellingTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
