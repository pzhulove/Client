// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class PushExhibitionTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -7804209,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static PushExhibitionTable GetRootAsPushExhibitionTable(ByteBuffer _bb) { return GetRootAsPushExhibitionTable(_bb, new PushExhibitionTable()); }
  public static PushExhibitionTable GetRootAsPushExhibitionTable(ByteBuffer _bb, PushExhibitionTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public PushExhibitionTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public int FinishLevel { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string IconPath { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetIconPathBytes() { return __p.__vector_as_arraysegment(10); }
  public string LinkInfo { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLinkInfoBytes() { return __p.__vector_as_arraysegment(12); }
  public int StartTime { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int EndTime { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int AfterStartServer { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int AfterStartServerDays { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string OpenInterval { get { int o = __p.__offset(22); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetOpenIntervalBytes() { return __p.__vector_as_arraysegment(22); }
  public string CloseInterval { get { int o = __p.__offset(24); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetCloseIntervalBytes() { return __p.__vector_as_arraysegment(24); }
  public string LoadingIcon { get { int o = __p.__offset(26); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLoadingIconBytes() { return __p.__vector_as_arraysegment(26); }
  public int SortNum { get { int o = __p.__offset(28); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int IsShowTime { get { int o = __p.__offset(30); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int IsSetNative { get { int o = __p.__offset(32); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<PushExhibitionTable> CreatePushExhibitionTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      int FinishLevel = 0,
      StringOffset IconPathOffset = default(StringOffset),
      StringOffset LinkInfoOffset = default(StringOffset),
      int StartTime = 0,
      int EndTime = 0,
      int AfterStartServer = 0,
      int AfterStartServerDays = 0,
      StringOffset OpenIntervalOffset = default(StringOffset),
      StringOffset CloseIntervalOffset = default(StringOffset),
      StringOffset LoadingIconOffset = default(StringOffset),
      int SortNum = 0,
      int IsShowTime = 0,
      int IsSetNative = 0) {
    builder.StartObject(15);
    PushExhibitionTable.AddIsSetNative(builder, IsSetNative);
    PushExhibitionTable.AddIsShowTime(builder, IsShowTime);
    PushExhibitionTable.AddSortNum(builder, SortNum);
    PushExhibitionTable.AddLoadingIcon(builder, LoadingIconOffset);
    PushExhibitionTable.AddCloseInterval(builder, CloseIntervalOffset);
    PushExhibitionTable.AddOpenInterval(builder, OpenIntervalOffset);
    PushExhibitionTable.AddAfterStartServerDays(builder, AfterStartServerDays);
    PushExhibitionTable.AddAfterStartServer(builder, AfterStartServer);
    PushExhibitionTable.AddEndTime(builder, EndTime);
    PushExhibitionTable.AddStartTime(builder, StartTime);
    PushExhibitionTable.AddLinkInfo(builder, LinkInfoOffset);
    PushExhibitionTable.AddIconPath(builder, IconPathOffset);
    PushExhibitionTable.AddFinishLevel(builder, FinishLevel);
    PushExhibitionTable.AddName(builder, NameOffset);
    PushExhibitionTable.AddID(builder, ID);
    return PushExhibitionTable.EndPushExhibitionTable(builder);
  }

  public static void StartPushExhibitionTable(FlatBufferBuilder builder) { builder.StartObject(15); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddFinishLevel(FlatBufferBuilder builder, int FinishLevel) { builder.AddInt(2, FinishLevel, 0); }
  public static void AddIconPath(FlatBufferBuilder builder, StringOffset IconPathOffset) { builder.AddOffset(3, IconPathOffset.Value, 0); }
  public static void AddLinkInfo(FlatBufferBuilder builder, StringOffset LinkInfoOffset) { builder.AddOffset(4, LinkInfoOffset.Value, 0); }
  public static void AddStartTime(FlatBufferBuilder builder, int StartTime) { builder.AddInt(5, StartTime, 0); }
  public static void AddEndTime(FlatBufferBuilder builder, int EndTime) { builder.AddInt(6, EndTime, 0); }
  public static void AddAfterStartServer(FlatBufferBuilder builder, int AfterStartServer) { builder.AddInt(7, AfterStartServer, 0); }
  public static void AddAfterStartServerDays(FlatBufferBuilder builder, int AfterStartServerDays) { builder.AddInt(8, AfterStartServerDays, 0); }
  public static void AddOpenInterval(FlatBufferBuilder builder, StringOffset OpenIntervalOffset) { builder.AddOffset(9, OpenIntervalOffset.Value, 0); }
  public static void AddCloseInterval(FlatBufferBuilder builder, StringOffset CloseIntervalOffset) { builder.AddOffset(10, CloseIntervalOffset.Value, 0); }
  public static void AddLoadingIcon(FlatBufferBuilder builder, StringOffset LoadingIconOffset) { builder.AddOffset(11, LoadingIconOffset.Value, 0); }
  public static void AddSortNum(FlatBufferBuilder builder, int SortNum) { builder.AddInt(12, SortNum, 0); }
  public static void AddIsShowTime(FlatBufferBuilder builder, int IsShowTime) { builder.AddInt(13, IsShowTime, 0); }
  public static void AddIsSetNative(FlatBufferBuilder builder, int IsSetNative) { builder.AddInt(14, IsSetNative, 0); }
  public static Offset<PushExhibitionTable> EndPushExhibitionTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<PushExhibitionTable>(o);
  }
  public static void FinishPushExhibitionTableBuffer(FlatBufferBuilder builder, Offset<PushExhibitionTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
