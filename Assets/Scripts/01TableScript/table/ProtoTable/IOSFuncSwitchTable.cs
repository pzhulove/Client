// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class IOSFuncSwitchTable : IFlatbufferObject
{
public enum eType : int
{
 NONE = 0,
 SEVEN_AWARDS = 1,
 GAME_CDK = 2,
 GAME_SERVICE = 3,
 SERVICE_LIST = 4,
 PAY_CHANNEL_CHANGE = 5,
 LIMITTIME_GIFT = 6,
 LIMITTIME_ACTIVITY = 7,
 ADS_PUSH = 8,
 LIMITTIME_JAR = 9,
 SHARE_TEXT_CHANGE = 10,
 SELECT_CREATE_ROLE_TAB = 11,
 MAINTAIN_TIPS = 12,
 GAME_WAIGUA = 13,
 Bounty_League = 14,
};

public enum eCrypt : int
{
 code = 1359211187,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static IOSFuncSwitchTable GetRootAsIOSFuncSwitchTable(ByteBuffer _bb) { return GetRootAsIOSFuncSwitchTable(_bb, new IOSFuncSwitchTable()); }
  public static IOSFuncSwitchTable GetRootAsIOSFuncSwitchTable(ByteBuffer _bb, IOSFuncSwitchTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public IOSFuncSwitchTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public ProtoTable.IOSFuncSwitchTable.eType Type { get { int o = __p.__offset(8); return o != 0 ? (ProtoTable.IOSFuncSwitchTable.eType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.IOSFuncSwitchTable.eType.NONE; } }
  public int Value { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<IOSFuncSwitchTable> CreateIOSFuncSwitchTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      ProtoTable.IOSFuncSwitchTable.eType Type = ProtoTable.IOSFuncSwitchTable.eType.NONE,
      int Value = 0) {
    builder.StartObject(4);
    IOSFuncSwitchTable.AddValue(builder, Value);
    IOSFuncSwitchTable.AddType(builder, Type);
    IOSFuncSwitchTable.AddName(builder, NameOffset);
    IOSFuncSwitchTable.AddID(builder, ID);
    return IOSFuncSwitchTable.EndIOSFuncSwitchTable(builder);
  }

  public static void StartIOSFuncSwitchTable(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, ProtoTable.IOSFuncSwitchTable.eType Type) { builder.AddInt(2, (int)Type, 0); }
  public static void AddValue(FlatBufferBuilder builder, int Value) { builder.AddInt(3, Value, 0); }
  public static Offset<IOSFuncSwitchTable> EndIOSFuncSwitchTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<IOSFuncSwitchTable>(o);
  }
  public static void FinishIOSFuncSwitchTableBuffer(FlatBufferBuilder builder, Offset<IOSFuncSwitchTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
