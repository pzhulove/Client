// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class SeasonDailyTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 1570894362,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static SeasonDailyTable GetRootAsSeasonDailyTable(ByteBuffer _bb) { return GetRootAsSeasonDailyTable(_bb, new SeasonDailyTable()); }
  public static SeasonDailyTable GetRootAsSeasonDailyTable(ByteBuffer _bb, SeasonDailyTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public SeasonDailyTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MatchScoreArray(int j) { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int MatchScoreLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetMatchScoreBytes() { return __p.__vector_as_arraysegment(6); }
 private FlatBufferArray<int> MatchScoreValue;
 public FlatBufferArray<int>  MatchScore
 {
  get{
  if (MatchScoreValue == null)
  {
    MatchScoreValue = new FlatBufferArray<int>(this.MatchScoreArray, this.MatchScoreLength);
  }
  return MatchScoreValue;}
 }
  public string RewardsArray(int j) { int o = __p.__offset(8); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int RewardsLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> RewardsValue;
 public FlatBufferArray<string>  Rewards
 {
  get{
  if (RewardsValue == null)
  {
    RewardsValue = new FlatBufferArray<string>(this.RewardsArray, this.RewardsLength);
  }
  return RewardsValue;}
 }

  public static Offset<SeasonDailyTable> CreateSeasonDailyTable(FlatBufferBuilder builder,
      int ID = 0,
      VectorOffset MatchScoreOffset = default(VectorOffset),
      VectorOffset RewardsOffset = default(VectorOffset)) {
    builder.StartObject(3);
    SeasonDailyTable.AddRewards(builder, RewardsOffset);
    SeasonDailyTable.AddMatchScore(builder, MatchScoreOffset);
    SeasonDailyTable.AddID(builder, ID);
    return SeasonDailyTable.EndSeasonDailyTable(builder);
  }

  public static void StartSeasonDailyTable(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddMatchScore(FlatBufferBuilder builder, VectorOffset MatchScoreOffset) { builder.AddOffset(1, MatchScoreOffset.Value, 0); }
  public static VectorOffset CreateMatchScoreVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartMatchScoreVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddRewards(FlatBufferBuilder builder, VectorOffset RewardsOffset) { builder.AddOffset(2, RewardsOffset.Value, 0); }
  public static VectorOffset CreateRewardsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartRewardsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<SeasonDailyTable> EndSeasonDailyTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SeasonDailyTable>(o);
  }
  public static void FinishSeasonDailyTableBuffer(FlatBufferBuilder builder, Offset<SeasonDailyTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
