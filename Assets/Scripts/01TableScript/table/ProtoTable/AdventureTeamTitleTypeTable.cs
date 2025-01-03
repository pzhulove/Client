// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class AdventureTeamTitleTypeTable : IFlatbufferObject
{
public enum eLimitType : int
{
 None = 0,
 Ranking = 1,
};

public enum eCrypt : int
{
 code = -774939027,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static AdventureTeamTitleTypeTable GetRootAsAdventureTeamTitleTypeTable(ByteBuffer _bb) { return GetRootAsAdventureTeamTitleTypeTable(_bb, new AdventureTeamTitleTypeTable()); }
  public static AdventureTeamTitleTypeTable GetRootAsAdventureTeamTitleTypeTable(ByteBuffer _bb, AdventureTeamTitleTypeTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public AdventureTeamTitleTypeTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int TitleTableID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public ProtoTable.AdventureTeamTitleTypeTable.eLimitType LimitType { get { int o = __p.__offset(8); return o != 0 ? (ProtoTable.AdventureTeamTitleTypeTable.eLimitType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.AdventureTeamTitleTypeTable.eLimitType.None; } }
  public int RankingRangeMin { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int RankingRangeMax { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<AdventureTeamTitleTypeTable> CreateAdventureTeamTitleTypeTable(FlatBufferBuilder builder,
      int ID = 0,
      int TitleTableID = 0,
      ProtoTable.AdventureTeamTitleTypeTable.eLimitType LimitType = ProtoTable.AdventureTeamTitleTypeTable.eLimitType.None,
      int RankingRangeMin = 0,
      int RankingRangeMax = 0) {
    builder.StartObject(5);
    AdventureTeamTitleTypeTable.AddRankingRangeMax(builder, RankingRangeMax);
    AdventureTeamTitleTypeTable.AddRankingRangeMin(builder, RankingRangeMin);
    AdventureTeamTitleTypeTable.AddLimitType(builder, LimitType);
    AdventureTeamTitleTypeTable.AddTitleTableID(builder, TitleTableID);
    AdventureTeamTitleTypeTable.AddID(builder, ID);
    return AdventureTeamTitleTypeTable.EndAdventureTeamTitleTypeTable(builder);
  }

  public static void StartAdventureTeamTitleTypeTable(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddTitleTableID(FlatBufferBuilder builder, int TitleTableID) { builder.AddInt(1, TitleTableID, 0); }
  public static void AddLimitType(FlatBufferBuilder builder, ProtoTable.AdventureTeamTitleTypeTable.eLimitType LimitType) { builder.AddInt(2, (int)LimitType, 0); }
  public static void AddRankingRangeMin(FlatBufferBuilder builder, int RankingRangeMin) { builder.AddInt(3, RankingRangeMin, 0); }
  public static void AddRankingRangeMax(FlatBufferBuilder builder, int RankingRangeMax) { builder.AddInt(4, RankingRangeMax, 0); }
  public static Offset<AdventureTeamTitleTypeTable> EndAdventureTeamTitleTypeTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AdventureTeamTitleTypeTable>(o);
  }
  public static void FinishAdventureTeamTitleTypeTableBuffer(FlatBufferBuilder builder, Offset<AdventureTeamTitleTypeTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

