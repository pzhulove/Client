// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class EffectInfoTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 1327756299,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static EffectInfoTable GetRootAsEffectInfoTable(ByteBuffer _bb) { return GetRootAsEffectInfoTable(_bb, new EffectInfoTable()); }
  public static EffectInfoTable GetRootAsEffectInfoTable(ByteBuffer _bb, EffectInfoTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public EffectInfoTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Description { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDescriptionBytes() { return __p.__vector_as_arraysegment(6); }
  public string Path { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetPathBytes() { return __p.__vector_as_arraysegment(8); }
  public string Locator { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLocatorBytes() { return __p.__vector_as_arraysegment(10); }
  public bool Loop { get { int o = __p.__offset(12); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public int Duration { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Scale { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<EffectInfoTable> CreateEffectInfoTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset DescriptionOffset = default(StringOffset),
      StringOffset PathOffset = default(StringOffset),
      StringOffset LocatorOffset = default(StringOffset),
      bool Loop = false,
      int Duration = 0,
      int Scale = 0) {
    builder.StartObject(7);
    EffectInfoTable.AddScale(builder, Scale);
    EffectInfoTable.AddDuration(builder, Duration);
    EffectInfoTable.AddLocator(builder, LocatorOffset);
    EffectInfoTable.AddPath(builder, PathOffset);
    EffectInfoTable.AddDescription(builder, DescriptionOffset);
    EffectInfoTable.AddID(builder, ID);
    EffectInfoTable.AddLoop(builder, Loop);
    return EffectInfoTable.EndEffectInfoTable(builder);
  }

  public static void StartEffectInfoTable(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddDescription(FlatBufferBuilder builder, StringOffset DescriptionOffset) { builder.AddOffset(1, DescriptionOffset.Value, 0); }
  public static void AddPath(FlatBufferBuilder builder, StringOffset PathOffset) { builder.AddOffset(2, PathOffset.Value, 0); }
  public static void AddLocator(FlatBufferBuilder builder, StringOffset LocatorOffset) { builder.AddOffset(3, LocatorOffset.Value, 0); }
  public static void AddLoop(FlatBufferBuilder builder, bool Loop) { builder.AddBool(4, Loop, false); }
  public static void AddDuration(FlatBufferBuilder builder, int Duration) { builder.AddInt(5, Duration, 0); }
  public static void AddScale(FlatBufferBuilder builder, int Scale) { builder.AddInt(6, Scale, 0); }
  public static Offset<EffectInfoTable> EndEffectInfoTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EffectInfoTable>(o);
  }
  public static void FinishEffectInfoTableBuffer(FlatBufferBuilder builder, Offset<EffectInfoTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

