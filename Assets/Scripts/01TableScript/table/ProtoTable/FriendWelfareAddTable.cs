// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class FriendWelfareAddTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1924892469,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static FriendWelfareAddTable GetRootAsFriendWelfareAddTable(ByteBuffer _bb) { return GetRootAsFriendWelfareAddTable(_bb, new FriendWelfareAddTable()); }
  public static FriendWelfareAddTable GetRootAsFriendWelfareAddTable(ByteBuffer _bb, FriendWelfareAddTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public FriendWelfareAddTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string IntimacySpan { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetIntimacySpanBytes() { return __p.__vector_as_arraysegment(6); }
  public int ExpAddProb { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string IntimacyName { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetIntimacyNameBytes() { return __p.__vector_as_arraysegment(10); }

  public static Offset<FriendWelfareAddTable> CreateFriendWelfareAddTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset IntimacySpanOffset = default(StringOffset),
      int ExpAddProb = 0,
      StringOffset IntimacyNameOffset = default(StringOffset)) {
    builder.StartObject(4);
    FriendWelfareAddTable.AddIntimacyName(builder, IntimacyNameOffset);
    FriendWelfareAddTable.AddExpAddProb(builder, ExpAddProb);
    FriendWelfareAddTable.AddIntimacySpan(builder, IntimacySpanOffset);
    FriendWelfareAddTable.AddID(builder, ID);
    return FriendWelfareAddTable.EndFriendWelfareAddTable(builder);
  }

  public static void StartFriendWelfareAddTable(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddIntimacySpan(FlatBufferBuilder builder, StringOffset IntimacySpanOffset) { builder.AddOffset(1, IntimacySpanOffset.Value, 0); }
  public static void AddExpAddProb(FlatBufferBuilder builder, int ExpAddProb) { builder.AddInt(2, ExpAddProb, 0); }
  public static void AddIntimacyName(FlatBufferBuilder builder, StringOffset IntimacyNameOffset) { builder.AddOffset(3, IntimacyNameOffset.Value, 0); }
  public static Offset<FriendWelfareAddTable> EndFriendWelfareAddTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FriendWelfareAddTable>(o);
  }
  public static void FinishFriendWelfareAddTableBuffer(FlatBufferBuilder builder, Offset<FriendWelfareAddTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
