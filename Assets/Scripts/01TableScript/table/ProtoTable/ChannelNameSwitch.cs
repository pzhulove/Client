// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ChannelNameSwitch : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1972744578,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ChannelNameSwitch GetRootAsChannelNameSwitch(ByteBuffer _bb) { return GetRootAsChannelNameSwitch(_bb, new ChannelNameSwitch()); }
  public static ChannelNameSwitch GetRootAsChannelNameSwitch(ByteBuffer _bb, ChannelNameSwitch obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ChannelNameSwitch __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string EnglishName { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetEnglishNameBytes() { return __p.__vector_as_arraysegment(6); }
  public string ChineseName { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetChineseNameBytes() { return __p.__vector_as_arraysegment(8); }

  public static Offset<ChannelNameSwitch> CreateChannelNameSwitch(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset EnglishNameOffset = default(StringOffset),
      StringOffset ChineseNameOffset = default(StringOffset)) {
    builder.StartObject(3);
    ChannelNameSwitch.AddChineseName(builder, ChineseNameOffset);
    ChannelNameSwitch.AddEnglishName(builder, EnglishNameOffset);
    ChannelNameSwitch.AddID(builder, ID);
    return ChannelNameSwitch.EndChannelNameSwitch(builder);
  }

  public static void StartChannelNameSwitch(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddEnglishName(FlatBufferBuilder builder, StringOffset EnglishNameOffset) { builder.AddOffset(1, EnglishNameOffset.Value, 0); }
  public static void AddChineseName(FlatBufferBuilder builder, StringOffset ChineseNameOffset) { builder.AddOffset(2, ChineseNameOffset.Value, 0); }
  public static Offset<ChannelNameSwitch> EndChannelNameSwitch(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ChannelNameSwitch>(o);
  }
  public static void FinishChannelNameSwitchBuffer(FlatBufferBuilder builder, Offset<ChannelNameSwitch> offset) { builder.Finish(offset.Value); }
};


}


#endif

