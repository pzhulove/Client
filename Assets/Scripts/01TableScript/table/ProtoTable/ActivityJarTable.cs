// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class ActivityJarTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 79917002,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ActivityJarTable GetRootAsActivityJarTable(ByteBuffer _bb) { return GetRootAsActivityJarTable(_bb, new ActivityJarTable()); }
  public static ActivityJarTable GetRootAsActivityJarTable(ByteBuffer _bb, ActivityJarTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ActivityJarTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int JarID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string ActivityBG { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetActivityBGBytes() { return __p.__vector_as_arraysegment(8); }
  public string ActivityRule { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetActivityRuleBytes() { return __p.__vector_as_arraysegment(10); }

  public static Offset<ActivityJarTable> CreateActivityJarTable(FlatBufferBuilder builder,
      int ID = 0,
      int JarID = 0,
      StringOffset ActivityBGOffset = default(StringOffset),
      StringOffset ActivityRuleOffset = default(StringOffset)) {
    builder.StartObject(4);
    ActivityJarTable.AddActivityRule(builder, ActivityRuleOffset);
    ActivityJarTable.AddActivityBG(builder, ActivityBGOffset);
    ActivityJarTable.AddJarID(builder, JarID);
    ActivityJarTable.AddID(builder, ID);
    return ActivityJarTable.EndActivityJarTable(builder);
  }

  public static void StartActivityJarTable(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddJarID(FlatBufferBuilder builder, int JarID) { builder.AddInt(1, JarID, 0); }
  public static void AddActivityBG(FlatBufferBuilder builder, StringOffset ActivityBGOffset) { builder.AddOffset(2, ActivityBGOffset.Value, 0); }
  public static void AddActivityRule(FlatBufferBuilder builder, StringOffset ActivityRuleOffset) { builder.AddOffset(3, ActivityRuleOffset.Value, 0); }
  public static Offset<ActivityJarTable> EndActivityJarTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ActivityJarTable>(o);
  }
  public static void FinishActivityJarTableBuffer(FlatBufferBuilder builder, Offset<ActivityJarTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

