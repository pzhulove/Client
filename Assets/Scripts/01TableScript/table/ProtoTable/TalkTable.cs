// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class TalkTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 182457828,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static TalkTable GetRootAsTalkTable(ByteBuffer _bb) { return GetRootAsTalkTable(_bb, new TalkTable()); }
  public static TalkTable GetRootAsTalkTable(ByteBuffer _bb, TalkTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public TalkTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string ObjectName { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetObjectNameBytes() { return __p.__vector_as_arraysegment(6); }
  public int NpcID { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string TalkText { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetTalkTextBytes() { return __p.__vector_as_arraysegment(10); }
  public int NextID { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MissionID { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int TakeFinish { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string FrameClassName { get { int o = __p.__offset(18); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetFrameClassNameBytes() { return __p.__vector_as_arraysegment(18); }
  public string AttachClassName { get { int o = __p.__offset(20); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetAttachClassNameBytes() { return __p.__vector_as_arraysegment(20); }
  public string TalkAbbreviation { get { int o = __p.__offset(22); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetTalkAbbreviationBytes() { return __p.__vector_as_arraysegment(22); }

  public static Offset<TalkTable> CreateTalkTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset ObjectNameOffset = default(StringOffset),
      int NpcID = 0,
      StringOffset TalkTextOffset = default(StringOffset),
      int NextID = 0,
      int MissionID = 0,
      int TakeFinish = 0,
      StringOffset FrameClassNameOffset = default(StringOffset),
      StringOffset AttachClassNameOffset = default(StringOffset),
      StringOffset TalkAbbreviationOffset = default(StringOffset)) {
    builder.StartObject(10);
    TalkTable.AddTalkAbbreviation(builder, TalkAbbreviationOffset);
    TalkTable.AddAttachClassName(builder, AttachClassNameOffset);
    TalkTable.AddFrameClassName(builder, FrameClassNameOffset);
    TalkTable.AddTakeFinish(builder, TakeFinish);
    TalkTable.AddMissionID(builder, MissionID);
    TalkTable.AddNextID(builder, NextID);
    TalkTable.AddTalkText(builder, TalkTextOffset);
    TalkTable.AddNpcID(builder, NpcID);
    TalkTable.AddObjectName(builder, ObjectNameOffset);
    TalkTable.AddID(builder, ID);
    return TalkTable.EndTalkTable(builder);
  }

  public static void StartTalkTable(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddObjectName(FlatBufferBuilder builder, StringOffset ObjectNameOffset) { builder.AddOffset(1, ObjectNameOffset.Value, 0); }
  public static void AddNpcID(FlatBufferBuilder builder, int NpcID) { builder.AddInt(2, NpcID, 0); }
  public static void AddTalkText(FlatBufferBuilder builder, StringOffset TalkTextOffset) { builder.AddOffset(3, TalkTextOffset.Value, 0); }
  public static void AddNextID(FlatBufferBuilder builder, int NextID) { builder.AddInt(4, NextID, 0); }
  public static void AddMissionID(FlatBufferBuilder builder, int MissionID) { builder.AddInt(5, MissionID, 0); }
  public static void AddTakeFinish(FlatBufferBuilder builder, int TakeFinish) { builder.AddInt(6, TakeFinish, 0); }
  public static void AddFrameClassName(FlatBufferBuilder builder, StringOffset FrameClassNameOffset) { builder.AddOffset(7, FrameClassNameOffset.Value, 0); }
  public static void AddAttachClassName(FlatBufferBuilder builder, StringOffset AttachClassNameOffset) { builder.AddOffset(8, AttachClassNameOffset.Value, 0); }
  public static void AddTalkAbbreviation(FlatBufferBuilder builder, StringOffset TalkAbbreviationOffset) { builder.AddOffset(9, TalkAbbreviationOffset.Value, 0); }
  public static Offset<TalkTable> EndTalkTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<TalkTable>(o);
  }
  public static void FinishTalkTableBuffer(FlatBufferBuilder builder, Offset<TalkTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

