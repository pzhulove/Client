// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class MissionScoreTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 634797042,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static MissionScoreTable GetRootAsMissionScoreTable(ByteBuffer _bb) { return GetRootAsMissionScoreTable(_bb, new MissionScoreTable()); }
  public static MissionScoreTable GetRootAsMissionScoreTable(ByteBuffer _bb, MissionScoreTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public MissionScoreTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public string UnOpenedChestBoxIcon { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetUnOpenedChestBoxIconBytes() { return __p.__vector_as_arraysegment(8); }
  public string OpenedChestBoxIcon { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetOpenedChestBoxIconBytes() { return __p.__vector_as_arraysegment(10); }
  public int Score { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int TotalScore { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string AwardsArray(int j) { int o = __p.__offset(16); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int AwardsLength { get { int o = __p.__offset(16); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> AwardsValue;
 public FlatBufferArray<string>  Awards
 {
  get{
  if (AwardsValue == null)
  {
    AwardsValue = new FlatBufferArray<string>(this.AwardsArray, this.AwardsLength);
  }
  return AwardsValue;}
 }

  public static Offset<MissionScoreTable> CreateMissionScoreTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      StringOffset UnOpenedChestBoxIconOffset = default(StringOffset),
      StringOffset OpenedChestBoxIconOffset = default(StringOffset),
      int Score = 0,
      int TotalScore = 0,
      VectorOffset AwardsOffset = default(VectorOffset)) {
    builder.StartObject(7);
    MissionScoreTable.AddAwards(builder, AwardsOffset);
    MissionScoreTable.AddTotalScore(builder, TotalScore);
    MissionScoreTable.AddScore(builder, Score);
    MissionScoreTable.AddOpenedChestBoxIcon(builder, OpenedChestBoxIconOffset);
    MissionScoreTable.AddUnOpenedChestBoxIcon(builder, UnOpenedChestBoxIconOffset);
    MissionScoreTable.AddName(builder, NameOffset);
    MissionScoreTable.AddID(builder, ID);
    return MissionScoreTable.EndMissionScoreTable(builder);
  }

  public static void StartMissionScoreTable(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddUnOpenedChestBoxIcon(FlatBufferBuilder builder, StringOffset UnOpenedChestBoxIconOffset) { builder.AddOffset(2, UnOpenedChestBoxIconOffset.Value, 0); }
  public static void AddOpenedChestBoxIcon(FlatBufferBuilder builder, StringOffset OpenedChestBoxIconOffset) { builder.AddOffset(3, OpenedChestBoxIconOffset.Value, 0); }
  public static void AddScore(FlatBufferBuilder builder, int Score) { builder.AddInt(4, Score, 0); }
  public static void AddTotalScore(FlatBufferBuilder builder, int TotalScore) { builder.AddInt(5, TotalScore, 0); }
  public static void AddAwards(FlatBufferBuilder builder, VectorOffset AwardsOffset) { builder.AddOffset(6, AwardsOffset.Value, 0); }
  public static VectorOffset CreateAwardsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAwardsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<MissionScoreTable> EndMissionScoreTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MissionScoreTable>(o);
  }
  public static void FinishMissionScoreTableBuffer(FlatBufferBuilder builder, Offset<MissionScoreTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
