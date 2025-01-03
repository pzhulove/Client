// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class AdventureTeamTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 847890401,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static AdventureTeamTable GetRootAsAdventureTeamTable(ByteBuffer _bb) { return GetRootAsAdventureTeamTable(_bb, new AdventureTeamTable()); }
  public static AdventureTeamTable GetRootAsAdventureTeamTable(ByteBuffer _bb, AdventureTeamTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public AdventureTeamTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Exp { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetExpBytes() { return __p.__vector_as_arraysegment(6); }
  public int ClearDungeonExpAddition { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string ExpSource { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetExpSourceBytes() { return __p.__vector_as_arraysegment(10); }
  public string PropertyIncomeDesc { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetPropertyIncomeDescBytes() { return __p.__vector_as_arraysegment(12); }

  public static Offset<AdventureTeamTable> CreateAdventureTeamTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset ExpOffset = default(StringOffset),
      int ClearDungeonExpAddition = 0,
      StringOffset ExpSourceOffset = default(StringOffset),
      StringOffset PropertyIncomeDescOffset = default(StringOffset)) {
    builder.StartObject(5);
    AdventureTeamTable.AddPropertyIncomeDesc(builder, PropertyIncomeDescOffset);
    AdventureTeamTable.AddExpSource(builder, ExpSourceOffset);
    AdventureTeamTable.AddClearDungeonExpAddition(builder, ClearDungeonExpAddition);
    AdventureTeamTable.AddExp(builder, ExpOffset);
    AdventureTeamTable.AddID(builder, ID);
    return AdventureTeamTable.EndAdventureTeamTable(builder);
  }

  public static void StartAdventureTeamTable(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddExp(FlatBufferBuilder builder, StringOffset ExpOffset) { builder.AddOffset(1, ExpOffset.Value, 0); }
  public static void AddClearDungeonExpAddition(FlatBufferBuilder builder, int ClearDungeonExpAddition) { builder.AddInt(2, ClearDungeonExpAddition, 0); }
  public static void AddExpSource(FlatBufferBuilder builder, StringOffset ExpSourceOffset) { builder.AddOffset(3, ExpSourceOffset.Value, 0); }
  public static void AddPropertyIncomeDesc(FlatBufferBuilder builder, StringOffset PropertyIncomeDescOffset) { builder.AddOffset(4, PropertyIncomeDescOffset.Value, 0); }
  public static Offset<AdventureTeamTable> EndAdventureTeamTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AdventureTeamTable>(o);
  }
  public static void FinishAdventureTeamTableBuffer(FlatBufferBuilder builder, Offset<AdventureTeamTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

