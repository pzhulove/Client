// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class UltimateChallengeBuffTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1636419005,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static UltimateChallengeBuffTable GetRootAsUltimateChallengeBuffTable(ByteBuffer _bb) { return GetRootAsUltimateChallengeBuffTable(_bb, new UltimateChallengeBuffTable()); }
  public static UltimateChallengeBuffTable GetRootAsUltimateChallengeBuffTable(ByteBuffer _bb, UltimateChallengeBuffTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public UltimateChallengeBuffTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int buffID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Description { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDescriptionBytes() { return __p.__vector_as_arraysegment(8); }
  public int target { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int sustain { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<UltimateChallengeBuffTable> CreateUltimateChallengeBuffTable(FlatBufferBuilder builder,
      int ID = 0,
      int buffID = 0,
      StringOffset DescriptionOffset = default(StringOffset),
      int target = 0,
      int sustain = 0) {
    builder.StartObject(5);
    UltimateChallengeBuffTable.AddSustain(builder, sustain);
    UltimateChallengeBuffTable.AddTarget(builder, target);
    UltimateChallengeBuffTable.AddDescription(builder, DescriptionOffset);
    UltimateChallengeBuffTable.AddBuffID(builder, buffID);
    UltimateChallengeBuffTable.AddID(builder, ID);
    return UltimateChallengeBuffTable.EndUltimateChallengeBuffTable(builder);
  }

  public static void StartUltimateChallengeBuffTable(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddBuffID(FlatBufferBuilder builder, int buffID) { builder.AddInt(1, buffID, 0); }
  public static void AddDescription(FlatBufferBuilder builder, StringOffset DescriptionOffset) { builder.AddOffset(2, DescriptionOffset.Value, 0); }
  public static void AddTarget(FlatBufferBuilder builder, int target) { builder.AddInt(3, target, 0); }
  public static void AddSustain(FlatBufferBuilder builder, int sustain) { builder.AddInt(4, sustain, 0); }
  public static Offset<UltimateChallengeBuffTable> EndUltimateChallengeBuffTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<UltimateChallengeBuffTable>(o);
  }
  public static void FinishUltimateChallengeBuffTableBuffer(FlatBufferBuilder builder, Offset<UltimateChallengeBuffTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
