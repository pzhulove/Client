// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class TeamCopyFlopTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1981605017,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static TeamCopyFlopTable GetRootAsTeamCopyFlopTable(ByteBuffer _bb) { return GetRootAsTeamCopyFlopTable(_bb, new TeamCopyFlopTable()); }
  public static TeamCopyFlopTable GetRootAsTeamCopyFlopTable(ByteBuffer _bb, TeamCopyFlopTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public TeamCopyFlopTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int TeamCopyID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int TeamGrade { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Stage { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int TypeStage { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DropId { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<TeamCopyFlopTable> CreateTeamCopyFlopTable(FlatBufferBuilder builder,
      int ID = 0,
      int TeamCopyID = 0,
      int TeamGrade = 0,
      int Stage = 0,
      int TypeStage = 0,
      int DropId = 0) {
    builder.StartObject(6);
    TeamCopyFlopTable.AddDropId(builder, DropId);
    TeamCopyFlopTable.AddTypeStage(builder, TypeStage);
    TeamCopyFlopTable.AddStage(builder, Stage);
    TeamCopyFlopTable.AddTeamGrade(builder, TeamGrade);
    TeamCopyFlopTable.AddTeamCopyID(builder, TeamCopyID);
    TeamCopyFlopTable.AddID(builder, ID);
    return TeamCopyFlopTable.EndTeamCopyFlopTable(builder);
  }

  public static void StartTeamCopyFlopTable(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddTeamCopyID(FlatBufferBuilder builder, int TeamCopyID) { builder.AddInt(1, TeamCopyID, 0); }
  public static void AddTeamGrade(FlatBufferBuilder builder, int TeamGrade) { builder.AddInt(2, TeamGrade, 0); }
  public static void AddStage(FlatBufferBuilder builder, int Stage) { builder.AddInt(3, Stage, 0); }
  public static void AddTypeStage(FlatBufferBuilder builder, int TypeStage) { builder.AddInt(4, TypeStage, 0); }
  public static void AddDropId(FlatBufferBuilder builder, int DropId) { builder.AddInt(5, DropId, 0); }
  public static Offset<TeamCopyFlopTable> EndTeamCopyFlopTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<TeamCopyFlopTable>(o);
  }
  public static void FinishTeamCopyFlopTableBuffer(FlatBufferBuilder builder, Offset<TeamCopyFlopTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
