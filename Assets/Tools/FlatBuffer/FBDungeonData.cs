// automatically generated, do not modify

namespace FBDungeonData
{

using FlatBuffers;

public sealed class DSceneDataConnect : Table {
  public static DSceneDataConnect GetRootAsDSceneDataConnect(ByteBuffer _bb) { return GetRootAsDSceneDataConnect(_bb, new DSceneDataConnect()); }
  public static DSceneDataConnect GetRootAsDSceneDataConnect(ByteBuffer _bb, DSceneDataConnect obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DSceneDataConnect __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public bool GetIsconnect(int j) { int o = __offset(4); return o != 0 ? 0!=bb.Get(__vector(o) + j * 1) : false; }
  public int IsconnectLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public int GetLinkAreaIndex(int j) { int o = __offset(6); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int LinkAreaIndexLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }
  public int Areaindex { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)-1; } }
  public int Id { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Sceneareapath { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Positionx { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)-1; } }
  public int Positiony { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)-1; } }
  public bool Isboss { get { int o = __offset(18); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool Isstart { get { int o = __offset(20); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool Isegg { get { int o = __offset(22); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool Isnothell { get { int o = __offset(24); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public byte TreasureType { get { int o = __offset(26); return o != 0 ? bb.Get(o + bb_pos) : (byte)0; } }

  public static Offset<DSceneDataConnect> CreateDSceneDataConnect(FlatBufferBuilder builder,
      VectorOffset isconnect = default(VectorOffset),
      VectorOffset linkAreaIndex = default(VectorOffset),
      int areaindex = -1,
      int id = 0,
      StringOffset sceneareapath = default(StringOffset),
      int positionx = -1,
      int positiony = -1,
      bool isboss = false,
      bool isstart = false,
      bool isegg = false,
      bool isnothell = false,
      byte treasureType = 0) {
    builder.StartObject(12);
    DSceneDataConnect.AddPositiony(builder, positiony);
    DSceneDataConnect.AddPositionx(builder, positionx);
    DSceneDataConnect.AddSceneareapath(builder, sceneareapath);
    DSceneDataConnect.AddId(builder, id);
    DSceneDataConnect.AddAreaindex(builder, areaindex);
    DSceneDataConnect.AddLinkAreaIndex(builder, linkAreaIndex);
    DSceneDataConnect.AddIsconnect(builder, isconnect);
    DSceneDataConnect.AddTreasureType(builder, treasureType);
    DSceneDataConnect.AddIsnothell(builder, isnothell);
    DSceneDataConnect.AddIsegg(builder, isegg);
    DSceneDataConnect.AddIsstart(builder, isstart);
    DSceneDataConnect.AddIsboss(builder, isboss);
    return DSceneDataConnect.EndDSceneDataConnect(builder);
  }

  public static void StartDSceneDataConnect(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddIsconnect(FlatBufferBuilder builder, VectorOffset isconnectOffset) { builder.AddOffset(0, isconnectOffset.Value, 0); }
  public static VectorOffset CreateIsconnectVector(FlatBufferBuilder builder, bool[] data) { builder.StartVector(1, data.Length, 1); for (int i = data.Length - 1; i >= 0; i--) builder.AddBool(data[i]); return builder.EndVector(); }
  public static void StartIsconnectVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
  public static void AddLinkAreaIndex(FlatBufferBuilder builder, VectorOffset linkAreaIndexOffset) { builder.AddOffset(1, linkAreaIndexOffset.Value, 0); }
  public static VectorOffset CreateLinkAreaIndexVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartLinkAreaIndexVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAreaindex(FlatBufferBuilder builder, int areaindex) { builder.AddInt(2, areaindex, -1); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(3, id, 0); }
  public static void AddSceneareapath(FlatBufferBuilder builder, StringOffset sceneareapathOffset) { builder.AddOffset(4, sceneareapathOffset.Value, 0); }
  public static void AddPositionx(FlatBufferBuilder builder, int positionx) { builder.AddInt(5, positionx, -1); }
  public static void AddPositiony(FlatBufferBuilder builder, int positiony) { builder.AddInt(6, positiony, -1); }
  public static void AddIsboss(FlatBufferBuilder builder, bool isboss) { builder.AddBool(7, isboss, false); }
  public static void AddIsstart(FlatBufferBuilder builder, bool isstart) { builder.AddBool(8, isstart, false); }
  public static void AddIsegg(FlatBufferBuilder builder, bool isegg) { builder.AddBool(9, isegg, false); }
  public static void AddIsnothell(FlatBufferBuilder builder, bool isnothell) { builder.AddBool(10, isnothell, false); }
  public static void AddTreasureType(FlatBufferBuilder builder, byte treasureType) { builder.AddByte(11, treasureType, 0); }
  public static Offset<DSceneDataConnect> EndDSceneDataConnect(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DSceneDataConnect>(o);
  }
};

public sealed class DDungeonData : Table {
  public static DDungeonData GetRootAsDDungeonData(ByteBuffer _bb) { return GetRootAsDDungeonData(_bb, new DDungeonData()); }
  public static DDungeonData GetRootAsDDungeonData(ByteBuffer _bb, DDungeonData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public static bool DDungeonDataBufferHasIdentifier(ByteBuffer _bb) { return __has_identifier(_bb, "DUNG"); }
  public DDungeonData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Height { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)3; } }
  public int Weidth { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)3; } }
  public int Startindex { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public DSceneDataConnect GetAreaconnectlist(int j) { return GetAreaconnectlist(new DSceneDataConnect(), j); }
  public DSceneDataConnect GetAreaconnectlist(DSceneDataConnect obj, int j) { int o = __offset(12); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AreaconnectlistLength { get { int o = __offset(12); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<DDungeonData> CreateDDungeonData(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      int height = 3,
      int weidth = 3,
      int startindex = 0,
      VectorOffset areaconnectlist = default(VectorOffset)) {
    builder.StartObject(5);
    DDungeonData.AddAreaconnectlist(builder, areaconnectlist);
    DDungeonData.AddStartindex(builder, startindex);
    DDungeonData.AddWeidth(builder, weidth);
    DDungeonData.AddHeight(builder, height);
    DDungeonData.AddName(builder, name);
    return DDungeonData.EndDDungeonData(builder);
  }

  public static void StartDDungeonData(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddHeight(FlatBufferBuilder builder, int height) { builder.AddInt(1, height, 3); }
  public static void AddWeidth(FlatBufferBuilder builder, int weidth) { builder.AddInt(2, weidth, 3); }
  public static void AddStartindex(FlatBufferBuilder builder, int startindex) { builder.AddInt(3, startindex, 0); }
  public static void AddAreaconnectlist(FlatBufferBuilder builder, VectorOffset areaconnectlistOffset) { builder.AddOffset(4, areaconnectlistOffset.Value, 0); }
  public static VectorOffset CreateAreaconnectlistVector(FlatBufferBuilder builder, Offset<DSceneDataConnect>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAreaconnectlistVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<DDungeonData> EndDDungeonData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DDungeonData>(o);
  }
  public static void FinishDDungeonDataBuffer(FlatBufferBuilder builder, Offset<DDungeonData> offset) { builder.Finish(offset.Value, "DUNG"); }
};


}
