// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class GuildDungeonDamageCoeffTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1838784585,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static GuildDungeonDamageCoeffTable GetRootAsGuildDungeonDamageCoeffTable(ByteBuffer _bb) { return GetRootAsGuildDungeonDamageCoeffTable(_bb, new GuildDungeonDamageCoeffTable()); }
  public static GuildDungeonDamageCoeffTable GetRootAsGuildDungeonDamageCoeffTable(ByteBuffer _bb, GuildDungeonDamageCoeffTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public GuildDungeonDamageCoeffTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public int coefficient { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<GuildDungeonDamageCoeffTable> CreateGuildDungeonDamageCoeffTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      int coefficient = 0) {
    builder.StartObject(3);
    GuildDungeonDamageCoeffTable.AddCoefficient(builder, coefficient);
    GuildDungeonDamageCoeffTable.AddName(builder, NameOffset);
    GuildDungeonDamageCoeffTable.AddID(builder, ID);
    return GuildDungeonDamageCoeffTable.EndGuildDungeonDamageCoeffTable(builder);
  }

  public static void StartGuildDungeonDamageCoeffTable(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddCoefficient(FlatBufferBuilder builder, int coefficient) { builder.AddInt(2, coefficient, 0); }
  public static Offset<GuildDungeonDamageCoeffTable> EndGuildDungeonDamageCoeffTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GuildDungeonDamageCoeffTable>(o);
  }
  public static void FinishGuildDungeonDamageCoeffTableBuffer(FlatBufferBuilder builder, Offset<GuildDungeonDamageCoeffTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
