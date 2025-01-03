// automatically generated, do not modify

namespace FBSceneData
{

using FlatBuffers;

public enum DEntityType : sbyte
{
 NPC = 0,
 MONSTER = 1,
 DECORATOR = 2,
 DESTRUCTIBLE = 3,
 REGION = 4,
 TRANSPORTDOOR = 5,
 BOSS = 6,
 ELITE = 7,
 BIRTHPOSITION = 8,
 TOWNDOOR = 9,
 FUNCTION_PREFAB = 10,
 MONSTERDESTRUCT = 11,
 HELLBIRTHPOSITION = 12,
 MAX = 13,
};

public enum FunctionType : sbyte
{
 FT_FollowPet = 0,
 FT_FollowPet2 = 1,
 FT_COUNT = 2,
};

public enum MonsterSwapType : sbyte
{
 POINT_SWAP = 0,
 CIRCLE_SWAP = 1,
 RECT_SWAP = 2,
};

public enum FaceType : sbyte
{
 Right = 0,
 Left = 1,
 Random = 2,
};

public enum FlowRegionType : sbyte
{
 None = 0,
 Region = 1,
 Destruct = 2,
};

public enum RegionType : sbyte
{
 Circle = 0,
 Rectangle = 1,
};

public enum TransportDoorType : sbyte
{
 Left = 0,
 Top = 1,
 Right = 2,
 Buttom = 3,
 None = 4,
};

public enum DoorTargetType : sbyte
{
 Normal = 0,
 PVEPracticeCourt = 1,
};

public enum EWeatherMode : sbyte
{
 None = 0,
 Rain = 1,
 Wind = 2,
 Snow = 3,
 MaxModeNum = 4,
};

public sealed class Vector2 : Struct {
  public Vector2 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float X { get { return bb.GetFloat(bb_pos + 0); } }
  public float Y { get { return bb.GetFloat(bb_pos + 4); } }

  public static Offset<Vector2> CreateVector2(FlatBufferBuilder builder, float X, float Y) {
    builder.Prep(4, 8);
    builder.PutFloat(Y);
    builder.PutFloat(X);
    return new Offset<Vector2>(builder.Offset);
  }
};

public sealed class Vector3 : Struct {
  public Vector3 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float X { get { return bb.GetFloat(bb_pos + 0); } }
  public float Y { get { return bb.GetFloat(bb_pos + 4); } }
  public float Z { get { return bb.GetFloat(bb_pos + 8); } }

  public static Offset<Vector3> CreateVector3(FlatBufferBuilder builder, float X, float Y, float Z) {
    builder.Prep(4, 12);
    builder.PutFloat(Z);
    builder.PutFloat(Y);
    builder.PutFloat(X);
    return new Offset<Vector3>(builder.Offset);
  }
};

public sealed class Quaternion : Struct {
  public Quaternion __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float X { get { return bb.GetFloat(bb_pos + 0); } }
  public float Y { get { return bb.GetFloat(bb_pos + 4); } }
  public float Z { get { return bb.GetFloat(bb_pos + 8); } }
  public float W { get { return bb.GetFloat(bb_pos + 12); } }

  public static Offset<Quaternion> CreateQuaternion(FlatBufferBuilder builder, float X, float Y, float Z, float W) {
    builder.Prep(4, 16);
    builder.PutFloat(W);
    builder.PutFloat(Z);
    builder.PutFloat(Y);
    builder.PutFloat(X);
    return new Offset<Quaternion>(builder.Offset);
  }
};

public sealed class Color : Struct {
  public Color __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float R { get { return bb.GetFloat(bb_pos + 0); } }
  public float G { get { return bb.GetFloat(bb_pos + 4); } }
  public float B { get { return bb.GetFloat(bb_pos + 8); } }
  public float A { get { return bb.GetFloat(bb_pos + 12); } }

  public static Offset<Color> CreateColor(FlatBufferBuilder builder, float R, float G, float B, float A) {
    builder.Prep(4, 16);
    builder.PutFloat(A);
    builder.PutFloat(B);
    builder.PutFloat(G);
    builder.PutFloat(R);
    return new Offset<Color>(builder.Offset);
  }
};

public sealed class DEntityInfo : Table {
  public static DEntityInfo GetRootAsDEntityInfo(ByteBuffer _bb) { return GetRootAsDEntityInfo(_bb, new DEntityInfo()); }
  public static DEntityInfo GetRootAsDEntityInfo(ByteBuffer _bb, DEntityInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DEntityInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Globalid { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Resid { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public string Path { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public string Description { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public DEntityType Type { get { int o = __offset(14); return o != 0 ? (DEntityType)bb.GetSbyte(o + bb_pos) : (DEntityType)0; } }
  public string TypeName { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public Vector3 Position { get { return GetPosition(new Vector3()); } }
  public Vector3 GetPosition(Vector3 obj) { int o = __offset(18); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float Scale { get { int o = __offset(20); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)1.0; } }
  public Color Color { get { return GetColor(new Color()); } }
  public Color GetColor(Color obj) { int o = __offset(22); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartDEntityInfo(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddGlobalid(FlatBufferBuilder builder, int globalid) { builder.AddInt(0, globalid, 0); }
  public static void AddResid(FlatBufferBuilder builder, int resid) { builder.AddInt(1, resid, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(2, nameOffset.Value, 0); }
  public static void AddPath(FlatBufferBuilder builder, StringOffset pathOffset) { builder.AddOffset(3, pathOffset.Value, 0); }
  public static void AddDescription(FlatBufferBuilder builder, StringOffset descriptionOffset) { builder.AddOffset(4, descriptionOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, DEntityType type) { builder.AddSbyte(5, (sbyte)(type), 0); }
  public static void AddTypeName(FlatBufferBuilder builder, StringOffset typeNameOffset) { builder.AddOffset(6, typeNameOffset.Value, 0); }
  public static void AddPosition(FlatBufferBuilder builder, Offset<Vector3> positionOffset) { builder.AddStruct(7, positionOffset.Value, 0); }
  public static void AddScale(FlatBufferBuilder builder, float scale) { builder.AddFloat(8, scale, 1.0); }
  public static void AddColor(FlatBufferBuilder builder, Offset<Color> colorOffset) { builder.AddStruct(9, colorOffset.Value, 0); }
  public static Offset<DEntityInfo> EndDEntityInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DEntityInfo>(o);
  }
};

public sealed class FunctionPrefab : Table {
  public static FunctionPrefab GetRootAsFunctionPrefab(ByteBuffer _bb) { return GetRootAsFunctionPrefab(_bb, new FunctionPrefab()); }
  public static FunctionPrefab GetRootAsFunctionPrefab(ByteBuffer _bb, FunctionPrefab obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public FunctionPrefab __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public DEntityInfo Super { get { return GetSuper(new DEntityInfo()); } }
  public DEntityInfo GetSuper(DEntityInfo obj) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public FunctionType EFunctionType { get { int o = __offset(6); return o != 0 ? (FunctionType)bb.GetSbyte(o + bb_pos) : (FunctionType)0; } }

  public static Offset<FunctionPrefab> CreateFunctionPrefab(FlatBufferBuilder builder,
      Offset<DEntityInfo> super = default(Offset<DEntityInfo>),
      FunctionType eFunctionType = (FunctionType)0) {
    builder.StartObject(2);
    FunctionPrefab.AddSuper(builder, super);
    FunctionPrefab.AddEFunctionType(builder, eFunctionType);
    return FunctionPrefab.EndFunctionPrefab(builder);
  }

  public static void StartFunctionPrefab(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddSuper(FlatBufferBuilder builder, Offset<DEntityInfo> superOffset) { builder.AddOffset(0, superOffset.Value, 0); }
  public static void AddEFunctionType(FlatBufferBuilder builder, FunctionType eFunctionType) { builder.AddSbyte(1, (sbyte)(eFunctionType), 0); }
  public static Offset<FunctionPrefab> EndFunctionPrefab(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FunctionPrefab>(o);
  }
};

public sealed class DNPCInfo : Table {
  public static DNPCInfo GetRootAsDNPCInfo(ByteBuffer _bb) { return GetRootAsDNPCInfo(_bb, new DNPCInfo()); }
  public static DNPCInfo GetRootAsDNPCInfo(ByteBuffer _bb, DNPCInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DNPCInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public DEntityInfo Super { get { return GetSuper(new DEntityInfo()); } }
  public DEntityInfo GetSuper(DEntityInfo obj) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public Quaternion Rotation { get { return GetRotation(new Quaternion()); } }
  public Quaternion GetRotation(Quaternion obj) { int o = __offset(6); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector2 MinFindRange { get { return GetMinFindRange(new Vector2()); } }
  public Vector2 GetMinFindRange(Vector2 obj) { int o = __offset(8); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector2 MaxFindRange { get { return GetMaxFindRange(new Vector2()); } }
  public Vector2 GetMaxFindRange(Vector2 obj) { int o = __offset(10); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartDNPCInfo(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddSuper(FlatBufferBuilder builder, Offset<DEntityInfo> superOffset) { builder.AddOffset(0, superOffset.Value, 0); }
  public static void AddRotation(FlatBufferBuilder builder, Offset<Quaternion> rotationOffset) { builder.AddStruct(1, rotationOffset.Value, 0); }
  public static void AddMinFindRange(FlatBufferBuilder builder, Offset<Vector2> minFindRangeOffset) { builder.AddStruct(2, minFindRangeOffset.Value, 0); }
  public static void AddMaxFindRange(FlatBufferBuilder builder, Offset<Vector2> maxFindRangeOffset) { builder.AddStruct(3, maxFindRangeOffset.Value, 0); }
  public static Offset<DNPCInfo> EndDNPCInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DNPCInfo>(o);
  }
};

public sealed class DMonsterInfo : Table {
  public static DMonsterInfo GetRootAsDMonsterInfo(ByteBuffer _bb) { return GetRootAsDMonsterInfo(_bb, new DMonsterInfo()); }
  public static DMonsterInfo GetRootAsDMonsterInfo(ByteBuffer _bb, DMonsterInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DMonsterInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public DEntityInfo Super { get { return GetSuper(new DEntityInfo()); } }
  public DEntityInfo GetSuper(DEntityInfo obj) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public MonsterSwapType SwapType { get { int o = __offset(6); return o != 0 ? (MonsterSwapType)bb.GetSbyte(o + bb_pos) : (MonsterSwapType)0; } }
  public bool IsFaceLeft { get { int o = __offset(8); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int SwapNum { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SwapDelay { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int FlushGroupID { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SubGroupID { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MonsterInfoTableID { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public FlowRegionType FlowRegionType { get { int o = __offset(20); return o != 0 ? (FlowRegionType)bb.GetSbyte(o + bb_pos) : (FlowRegionType)0; } }
  public DRegionInfo RegionInfo { get { return GetRegionInfo(new DRegionInfo()); } }
  public DRegionInfo GetRegionInfo(DRegionInfo obj) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public DDestructibleInfo DestructInfo { get { return GetDestructInfo(new DDestructibleInfo()); } }
  public DDestructibleInfo GetDestructInfo(DDestructibleInfo obj) { int o = __offset(24); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public int Sight { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GroupIndex { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string AiActionPath { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public string AiScenarioPath { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }

  public static Offset<DMonsterInfo> CreateDMonsterInfo(FlatBufferBuilder builder,
      Offset<DEntityInfo> super = default(Offset<DEntityInfo>),
      MonsterSwapType swapType = (MonsterSwapType)0,
      bool isFaceLeft = false,
      int swapNum = 0,
      int swapDelay = 0,
      int flushGroupID = 0,
      int subGroupID = 0,
      int monsterInfoTableID = 0,
      FlowRegionType flowRegionType = (FlowRegionType)0,
      Offset<DRegionInfo> regionInfo = default(Offset<DRegionInfo>),
      Offset<DDestructibleInfo> destructInfo = default(Offset<DDestructibleInfo>),
      int sight = 0,
      int groupIndex = 0,
      StringOffset aiActionPath = default(StringOffset),
      StringOffset aiScenarioPath = default(StringOffset)) {
    builder.StartObject(15);
    DMonsterInfo.AddAiScenarioPath(builder, aiScenarioPath);
    DMonsterInfo.AddAiActionPath(builder, aiActionPath);
    DMonsterInfo.AddGroupIndex(builder, groupIndex);
    DMonsterInfo.AddSight(builder, sight);
    DMonsterInfo.AddDestructInfo(builder, destructInfo);
    DMonsterInfo.AddRegionInfo(builder, regionInfo);
    DMonsterInfo.AddMonsterInfoTableID(builder, monsterInfoTableID);
    DMonsterInfo.AddSubGroupID(builder, subGroupID);
    DMonsterInfo.AddFlushGroupID(builder, flushGroupID);
    DMonsterInfo.AddSwapDelay(builder, swapDelay);
    DMonsterInfo.AddSwapNum(builder, swapNum);
    DMonsterInfo.AddSuper(builder, super);
    DMonsterInfo.AddFlowRegionType(builder, flowRegionType);
    DMonsterInfo.AddIsFaceLeft(builder, isFaceLeft);
    DMonsterInfo.AddSwapType(builder, swapType);
    return DMonsterInfo.EndDMonsterInfo(builder);
  }

  public static void StartDMonsterInfo(FlatBufferBuilder builder) { builder.StartObject(15); }
  public static void AddSuper(FlatBufferBuilder builder, Offset<DEntityInfo> superOffset) { builder.AddOffset(0, superOffset.Value, 0); }
  public static void AddSwapType(FlatBufferBuilder builder, MonsterSwapType swapType) { builder.AddSbyte(1, (sbyte)(swapType), 0); }
  public static void AddIsFaceLeft(FlatBufferBuilder builder, bool isFaceLeft) { builder.AddBool(2, isFaceLeft, false); }
  public static void AddSwapNum(FlatBufferBuilder builder, int swapNum) { builder.AddInt(3, swapNum, 0); }
  public static void AddSwapDelay(FlatBufferBuilder builder, int swapDelay) { builder.AddInt(4, swapDelay, 0); }
  public static void AddFlushGroupID(FlatBufferBuilder builder, int flushGroupID) { builder.AddInt(5, flushGroupID, 0); }
  public static void AddSubGroupID(FlatBufferBuilder builder, int subGroupID) { builder.AddInt(6, subGroupID, 0); }
  public static void AddMonsterInfoTableID(FlatBufferBuilder builder, int monsterInfoTableID) { builder.AddInt(7, monsterInfoTableID, 0); }
  public static void AddFlowRegionType(FlatBufferBuilder builder, FlowRegionType flowRegionType) { builder.AddSbyte(8, (sbyte)(flowRegionType), 0); }
  public static void AddRegionInfo(FlatBufferBuilder builder, Offset<DRegionInfo> regionInfoOffset) { builder.AddOffset(9, regionInfoOffset.Value, 0); }
  public static void AddDestructInfo(FlatBufferBuilder builder, Offset<DDestructibleInfo> destructInfoOffset) { builder.AddOffset(10, destructInfoOffset.Value, 0); }
  public static void AddSight(FlatBufferBuilder builder, int sight) { builder.AddInt(11, sight, 0); }
  public static void AddGroupIndex(FlatBufferBuilder builder, int groupIndex) { builder.AddInt(12, groupIndex, 0); }
  public static void AddAiActionPath(FlatBufferBuilder builder, StringOffset aiActionPathOffset) { builder.AddOffset(13, aiActionPathOffset.Value, 0); }
  public static void AddAiScenarioPath(FlatBufferBuilder builder, StringOffset aiScenarioPathOffset) { builder.AddOffset(14, aiScenarioPathOffset.Value, 0); }
  public static Offset<DMonsterInfo> EndDMonsterInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DMonsterInfo>(o);
  }
};

public sealed class DTransferInfo : Table {
  public static DTransferInfo GetRootAsDTransferInfo(ByteBuffer _bb) { return GetRootAsDTransferInfo(_bb, new DTransferInfo()); }
  public static DTransferInfo GetRootAsDTransferInfo(ByteBuffer _bb, DTransferInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DTransferInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public DEntityInfo Super { get { return GetSuper(new DEntityInfo()); } }
  public DEntityInfo GetSuper(DEntityInfo obj) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public int RegionId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<DTransferInfo> CreateDTransferInfo(FlatBufferBuilder builder,
      Offset<DEntityInfo> super = default(Offset<DEntityInfo>),
      int regionId = 0) {
    builder.StartObject(2);
    DTransferInfo.AddRegionId(builder, regionId);
    DTransferInfo.AddSuper(builder, super);
    return DTransferInfo.EndDTransferInfo(builder);
  }

  public static void StartDTransferInfo(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddSuper(FlatBufferBuilder builder, Offset<DEntityInfo> superOffset) { builder.AddOffset(0, superOffset.Value, 0); }
  public static void AddRegionId(FlatBufferBuilder builder, int regionId) { builder.AddInt(1, regionId, 0); }
  public static Offset<DTransferInfo> EndDTransferInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DTransferInfo>(o);
  }
};

public sealed class DDecoratorInfo : Table {
  public static DDecoratorInfo GetRootAsDDecoratorInfo(ByteBuffer _bb) { return GetRootAsDDecoratorInfo(_bb, new DDecoratorInfo()); }
  public static DDecoratorInfo GetRootAsDDecoratorInfo(ByteBuffer _bb, DDecoratorInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DDecoratorInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public DEntityInfo Super { get { return GetSuper(new DEntityInfo()); } }
  public DEntityInfo GetSuper(DEntityInfo obj) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public Vector3 LocalScale { get { return GetLocalScale(new Vector3()); } }
  public Vector3 GetLocalScale(Vector3 obj) { int o = __offset(6); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Quaternion Rotation { get { return GetRotation(new Quaternion()); } }
  public Quaternion GetRotation(Quaternion obj) { int o = __offset(8); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartDDecoratorInfo(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddSuper(FlatBufferBuilder builder, Offset<DEntityInfo> superOffset) { builder.AddOffset(0, superOffset.Value, 0); }
  public static void AddLocalScale(FlatBufferBuilder builder, Offset<Vector3> localScaleOffset) { builder.AddStruct(1, localScaleOffset.Value, 0); }
  public static void AddRotation(FlatBufferBuilder builder, Offset<Quaternion> rotationOffset) { builder.AddStruct(2, rotationOffset.Value, 0); }
  public static Offset<DDecoratorInfo> EndDDecoratorInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DDecoratorInfo>(o);
  }
};

public sealed class DDestructibleInfo : Table {
  public static DDestructibleInfo GetRootAsDDestructibleInfo(ByteBuffer _bb) { return GetRootAsDDestructibleInfo(_bb, new DDestructibleInfo()); }
  public static DDestructibleInfo GetRootAsDDestructibleInfo(ByteBuffer _bb, DDestructibleInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DDestructibleInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public DEntityInfo Super { get { return GetSuper(new DEntityInfo()); } }
  public DEntityInfo GetSuper(DEntityInfo obj) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public Quaternion Rotation { get { return GetRotation(new Quaternion()); } }
  public Quaternion GetRotation(Quaternion obj) { int o = __offset(6); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public int Level { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int FlushGroupID { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static void StartDDestructibleInfo(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddSuper(FlatBufferBuilder builder, Offset<DEntityInfo> superOffset) { builder.AddOffset(0, superOffset.Value, 0); }
  public static void AddRotation(FlatBufferBuilder builder, Offset<Quaternion> rotationOffset) { builder.AddStruct(1, rotationOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(2, level, 0); }
  public static void AddFlushGroupID(FlatBufferBuilder builder, int flushGroupID) { builder.AddInt(3, flushGroupID, 0); }
  public static Offset<DDestructibleInfo> EndDDestructibleInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DDestructibleInfo>(o);
  }
};

public sealed class DRegionInfo : Table {
  public static DRegionInfo GetRootAsDRegionInfo(ByteBuffer _bb) { return GetRootAsDRegionInfo(_bb, new DRegionInfo()); }
  public static DRegionInfo GetRootAsDRegionInfo(ByteBuffer _bb, DRegionInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DRegionInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public DEntityInfo Super { get { return GetSuper(new DEntityInfo()); } }
  public DEntityInfo GetSuper(DEntityInfo obj) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public RegionType Regiontype { get { int o = __offset(6); return o != 0 ? (RegionType)bb.GetSbyte(o + bb_pos) : (RegionType)0; } }
  public Vector2 Rect { get { return GetRect(new Vector2()); } }
  public Vector2 GetRect(Vector2 obj) { int o = __offset(8); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float Radius { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)1.0; } }
  public Quaternion Rotation { get { return GetRotation(new Quaternion()); } }
  public Quaternion GetRotation(Quaternion obj) { int o = __offset(12); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartDRegionInfo(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddSuper(FlatBufferBuilder builder, Offset<DEntityInfo> superOffset) { builder.AddOffset(0, superOffset.Value, 0); }
  public static void AddRegiontype(FlatBufferBuilder builder, RegionType regiontype) { builder.AddSbyte(1, (sbyte)(regiontype), 0); }
  public static void AddRect(FlatBufferBuilder builder, Offset<Vector2> rectOffset) { builder.AddStruct(2, rectOffset.Value, 0); }
  public static void AddRadius(FlatBufferBuilder builder, float radius) { builder.AddFloat(3, radius, 1.0); }
  public static void AddRotation(FlatBufferBuilder builder, Offset<Quaternion> rotationOffset) { builder.AddStruct(4, rotationOffset.Value, 0); }
  public static Offset<DRegionInfo> EndDRegionInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DRegionInfo>(o);
  }
};

public sealed class DTransportDoor : Table {
  public static DTransportDoor GetRootAsDTransportDoor(ByteBuffer _bb) { return GetRootAsDTransportDoor(_bb, new DTransportDoor()); }
  public static DTransportDoor GetRootAsDTransportDoor(ByteBuffer _bb, DTransportDoor obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DTransportDoor __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public DRegionInfo Super { get { return GetSuper(new DRegionInfo()); } }
  public DRegionInfo GetSuper(DRegionInfo obj) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public bool Iseggdoor { get { int o = __offset(6); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public TransportDoorType Doortype { get { int o = __offset(8); return o != 0 ? (TransportDoorType)bb.GetSbyte(o + bb_pos) : (TransportDoorType)0; } }
  public int Nextsceneid { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Townscenepath { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public TransportDoorType Nextdoortype { get { int o = __offset(14); return o != 0 ? (TransportDoorType)bb.GetSbyte(o + bb_pos) : (TransportDoorType)0; } }
  public Vector3 Birthposition { get { return GetBirthposition(new Vector3()); } }
  public Vector3 GetBirthposition(Vector3 obj) { int o = __offset(16); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartDTransportDoor(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddSuper(FlatBufferBuilder builder, Offset<DRegionInfo> superOffset) { builder.AddOffset(0, superOffset.Value, 0); }
  public static void AddIseggdoor(FlatBufferBuilder builder, bool iseggdoor) { builder.AddBool(1, iseggdoor, false); }
  public static void AddDoortype(FlatBufferBuilder builder, TransportDoorType doortype) { builder.AddSbyte(2, (sbyte)(doortype), 0); }
  public static void AddNextsceneid(FlatBufferBuilder builder, int nextsceneid) { builder.AddInt(3, nextsceneid, 0); }
  public static void AddTownscenepath(FlatBufferBuilder builder, StringOffset townscenepathOffset) { builder.AddOffset(4, townscenepathOffset.Value, 0); }
  public static void AddNextdoortype(FlatBufferBuilder builder, TransportDoorType nextdoortype) { builder.AddSbyte(5, (sbyte)(nextdoortype), 0); }
  public static void AddBirthposition(FlatBufferBuilder builder, Offset<Vector3> birthpositionOffset) { builder.AddStruct(6, birthpositionOffset.Value, 0); }
  public static Offset<DTransportDoor> EndDTransportDoor(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DTransportDoor>(o);
  }
};

public sealed class DTownDoor : Table {
  public static DTownDoor GetRootAsDTownDoor(ByteBuffer _bb) { return GetRootAsDTownDoor(_bb, new DTownDoor()); }
  public static DTownDoor GetRootAsDTownDoor(ByteBuffer _bb, DTownDoor obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DTownDoor __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public DRegionInfo Super { get { return GetSuper(new DRegionInfo()); } }
  public DRegionInfo GetSuper(DRegionInfo obj) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public int SceneID { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int DoorID { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public Vector3 BirthPos { get { return GetBirthPos(new Vector3()); } }
  public Vector3 GetBirthPos(Vector3 obj) { int o = __offset(10); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public int TargetSceneID { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int TargetDoorID { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public DoorTargetType DoorType { get { int o = __offset(16); return o != 0 ? (DoorTargetType)bb.GetSbyte(o + bb_pos) : (DoorTargetType)0; } }

  public static void StartDTownDoor(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddSuper(FlatBufferBuilder builder, Offset<DRegionInfo> superOffset) { builder.AddOffset(0, superOffset.Value, 0); }
  public static void AddSceneID(FlatBufferBuilder builder, int SceneID) { builder.AddInt(1, SceneID, 0); }
  public static void AddDoorID(FlatBufferBuilder builder, int DoorID) { builder.AddInt(2, DoorID, 0); }
  public static void AddBirthPos(FlatBufferBuilder builder, Offset<Vector3> BirthPosOffset) { builder.AddStruct(3, BirthPosOffset.Value, 0); }
  public static void AddTargetSceneID(FlatBufferBuilder builder, int TargetSceneID) { builder.AddInt(4, TargetSceneID, 0); }
  public static void AddTargetDoorID(FlatBufferBuilder builder, int TargetDoorID) { builder.AddInt(5, TargetDoorID, 0); }
  public static void AddDoorType(FlatBufferBuilder builder, DoorTargetType DoorType) { builder.AddSbyte(6, (sbyte)(DoorType), 0); }
  public static Offset<DTownDoor> EndDTownDoor(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DTownDoor>(o);
  }
};

public sealed class DSceneData : Table {
  public static DSceneData GetRootAsDSceneData(ByteBuffer _bb) { return GetRootAsDSceneData(_bb, new DSceneData()); }
  public static DSceneData GetRootAsDSceneData(ByteBuffer _bb, DSceneData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public static bool DSceneDataBufferHasIdentifier(ByteBuffer _bb) { return __has_identifier(_bb, "SCEN"); }
  public DSceneData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Id { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Prefabpath { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public float CameraLookHeight { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)1.0; } }
  public float CameraDistance { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)10.0; } }
  public float CameraAngle { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)20.0; } }
  public float CameraNearClip { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0.3; } }
  public float CameraFarClip { get { int o = __offset(18); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)50.0; } }
  public float CameraSize { get { int o = __offset(20); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)3.3; } }
  public Vector2 CameraZRange { get { return GetCameraZRange(new Vector2()); } }
  public Vector2 GetCameraZRange(Vector2 obj) { int o = __offset(22); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector2 CameraXRange { get { return GetCameraXRange(new Vector2()); } }
  public Vector2 GetCameraXRange(Vector2 obj) { int o = __offset(24); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public bool CameraPersp { get { int o = __offset(26); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public Vector3 CenterPostionNew { get { return GetCenterPostionNew(new Vector3()); } }
  public Vector3 GetCenterPostionNew(Vector3 obj) { int o = __offset(28); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 ScenePostion { get { return GetScenePostion(new Vector3()); } }
  public Vector3 GetScenePostion(Vector3 obj) { int o = __offset(30); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float SceneUScale { get { int o = __offset(32); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)1.0; } }
  public Vector2 GridSize { get { return GetGridSize(new Vector2()); } }
  public Vector2 GetGridSize(Vector2 obj) { int o = __offset(34); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector2 LogicXSize { get { return GetLogicXSize(new Vector2()); } }
  public Vector2 GetLogicXSize(Vector2 obj) { int o = __offset(36); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector2 LogicZSize { get { return GetLogicZSize(new Vector2()); } }
  public Vector2 GetLogicZSize(Vector2 obj) { int o = __offset(38); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Color ObjectDyeColor { get { return GetObjectDyeColor(new Color()); } }
  public Color GetObjectDyeColor(Color obj) { int o = __offset(40); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 LogicPos { get { return GetLogicPos(new Vector3()); } }
  public Vector3 GetLogicPos(Vector3 obj) { int o = __offset(42); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public EWeatherMode WeatherMode { get { int o = __offset(44); return o != 0 ? (EWeatherMode)bb.GetSbyte(o + bb_pos) : (EWeatherMode)0; } }
  public int TipsID { get { int o = __offset(46); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string LightmapsettingsPath { get { int o = __offset(48); return o != 0 ? __string(o + bb_pos) : null; } }
  public int LogicXmin { get { int o = __offset(50); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LogicXmax { get { int o = __offset(52); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LogicZmin { get { int o = __offset(54); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LogicZmax { get { int o = __offset(56); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public DEntityInfo GetEntityinfo(int j) { return GetEntityinfo(new DEntityInfo(), j); }
  public DEntityInfo GetEntityinfo(DEntityInfo obj, int j) { int o = __offset(58); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int EntityinfoLength { get { int o = __offset(58); return o != 0 ? __vector_len(o) : 0; } }
  public byte GetBlocklayer(int j) { int o = __offset(60); return o != 0 ? bb.Get(__vector(o) + j * 1) : (byte)0; }
  public int BlocklayerLength { get { int o = __offset(60); return o != 0 ? __vector_len(o) : 0; } }
  public DNPCInfo GetNpcinfo(int j) { return GetNpcinfo(new DNPCInfo(), j); }
  public DNPCInfo GetNpcinfo(DNPCInfo obj, int j) { int o = __offset(62); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int NpcinfoLength { get { int o = __offset(62); return o != 0 ? __vector_len(o) : 0; } }
  public DMonsterInfo GetMonsterinfo(int j) { return GetMonsterinfo(new DMonsterInfo(), j); }
  public DMonsterInfo GetMonsterinfo(DMonsterInfo obj, int j) { int o = __offset(64); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MonsterinfoLength { get { int o = __offset(64); return o != 0 ? __vector_len(o) : 0; } }
  public DDecoratorInfo GetDecoratorinfo(int j) { return GetDecoratorinfo(new DDecoratorInfo(), j); }
  public DDecoratorInfo GetDecoratorinfo(DDecoratorInfo obj, int j) { int o = __offset(66); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int DecoratorinfoLength { get { int o = __offset(66); return o != 0 ? __vector_len(o) : 0; } }
  public DDestructibleInfo GetDesructibleinfo(int j) { return GetDesructibleinfo(new DDestructibleInfo(), j); }
  public DDestructibleInfo GetDesructibleinfo(DDestructibleInfo obj, int j) { int o = __offset(68); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int DesructibleinfoLength { get { int o = __offset(68); return o != 0 ? __vector_len(o) : 0; } }
  public DRegionInfo GetRegioninfo(int j) { return GetRegioninfo(new DRegionInfo(), j); }
  public DRegionInfo GetRegioninfo(DRegionInfo obj, int j) { int o = __offset(70); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int RegioninfoLength { get { int o = __offset(70); return o != 0 ? __vector_len(o) : 0; } }
  public DTransportDoor GetTransportdoor(int j) { return GetTransportdoor(new DTransportDoor(), j); }
  public DTransportDoor GetTransportdoor(DTransportDoor obj, int j) { int o = __offset(72); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TransportdoorLength { get { int o = __offset(72); return o != 0 ? __vector_len(o) : 0; } }
  public DTransferInfo GetFighterBornPosition(int j) { return GetFighterBornPosition(new DTransferInfo(), j); }
  public DTransferInfo GetFighterBornPosition(DTransferInfo obj, int j) { int o = __offset(74); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int FighterBornPositionLength { get { int o = __offset(74); return o != 0 ? __vector_len(o) : 0; } }
  public DEntityInfo Birthposition { get { return GetBirthposition(new DEntityInfo()); } }
  public DEntityInfo GetBirthposition(DEntityInfo obj) { int o = __offset(76); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public DEntityInfo Hellbirthposition { get { return GetHellbirthposition(new DEntityInfo()); } }
  public DEntityInfo GetHellbirthposition(DEntityInfo obj) { int o = __offset(78); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public DTownDoor GetTownDoor(int j) { return GetTownDoor(new DTownDoor(), j); }
  public DTownDoor GetTownDoor(DTownDoor obj, int j) { int o = __offset(80); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TownDoorLength { get { int o = __offset(80); return o != 0 ? __vector_len(o) : 0; } }
  public FunctionPrefab GetFunctionPrefab(int j) { return GetFunctionPrefab(new FunctionPrefab(), j); }
  public FunctionPrefab GetFunctionPrefab(FunctionPrefab obj, int j) { int o = __offset(82); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int FunctionPrefabLength { get { int o = __offset(82); return o != 0 ? __vector_len(o) : 0; } }

  public static void StartDSceneData(FlatBufferBuilder builder) { builder.StartObject(40); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(1, id, 0); }
  public static void AddPrefabpath(FlatBufferBuilder builder, StringOffset prefabpathOffset) { builder.AddOffset(2, prefabpathOffset.Value, 0); }
  public static void AddCameraLookHeight(FlatBufferBuilder builder, float CameraLookHeight) { builder.AddFloat(3, CameraLookHeight, 1.0); }
  public static void AddCameraDistance(FlatBufferBuilder builder, float CameraDistance) { builder.AddFloat(4, CameraDistance, 10.0); }
  public static void AddCameraAngle(FlatBufferBuilder builder, float CameraAngle) { builder.AddFloat(5, CameraAngle, 20.0); }
  public static void AddCameraNearClip(FlatBufferBuilder builder, float CameraNearClip) { builder.AddFloat(6, CameraNearClip, 0.3); }
  public static void AddCameraFarClip(FlatBufferBuilder builder, float CameraFarClip) { builder.AddFloat(7, CameraFarClip, 50.0); }
  public static void AddCameraSize(FlatBufferBuilder builder, float CameraSize) { builder.AddFloat(8, CameraSize, 3.3); }
  public static void AddCameraZRange(FlatBufferBuilder builder, Offset<Vector2> CameraZRangeOffset) { builder.AddStruct(9, CameraZRangeOffset.Value, 0); }
  public static void AddCameraXRange(FlatBufferBuilder builder, Offset<Vector2> CameraXRangeOffset) { builder.AddStruct(10, CameraXRangeOffset.Value, 0); }
  public static void AddCameraPersp(FlatBufferBuilder builder, bool CameraPersp) { builder.AddBool(11, CameraPersp, false); }
  public static void AddCenterPostionNew(FlatBufferBuilder builder, Offset<Vector3> CenterPostionNewOffset) { builder.AddStruct(12, CenterPostionNewOffset.Value, 0); }
  public static void AddScenePostion(FlatBufferBuilder builder, Offset<Vector3> ScenePostionOffset) { builder.AddStruct(13, ScenePostionOffset.Value, 0); }
  public static void AddSceneUScale(FlatBufferBuilder builder, float SceneUScale) { builder.AddFloat(14, SceneUScale, 1.0); }
  public static void AddGridSize(FlatBufferBuilder builder, Offset<Vector2> GridSizeOffset) { builder.AddStruct(15, GridSizeOffset.Value, 0); }
  public static void AddLogicXSize(FlatBufferBuilder builder, Offset<Vector2> LogicXSizeOffset) { builder.AddStruct(16, LogicXSizeOffset.Value, 0); }
  public static void AddLogicZSize(FlatBufferBuilder builder, Offset<Vector2> LogicZSizeOffset) { builder.AddStruct(17, LogicZSizeOffset.Value, 0); }
  public static void AddObjectDyeColor(FlatBufferBuilder builder, Offset<Color> ObjectDyeColorOffset) { builder.AddStruct(18, ObjectDyeColorOffset.Value, 0); }
  public static void AddLogicPos(FlatBufferBuilder builder, Offset<Vector3> LogicPosOffset) { builder.AddStruct(19, LogicPosOffset.Value, 0); }
  public static void AddWeatherMode(FlatBufferBuilder builder, EWeatherMode WeatherMode) { builder.AddSbyte(20, (sbyte)(WeatherMode), 0); }
  public static void AddTipsID(FlatBufferBuilder builder, int TipsID) { builder.AddInt(21, TipsID, 0); }
  public static void AddLightmapsettingsPath(FlatBufferBuilder builder, StringOffset LightmapsettingsPathOffset) { builder.AddOffset(22, LightmapsettingsPathOffset.Value, 0); }
  public static void AddLogicXmin(FlatBufferBuilder builder, int LogicXmin) { builder.AddInt(23, LogicXmin, 0); }
  public static void AddLogicXmax(FlatBufferBuilder builder, int LogicXmax) { builder.AddInt(24, LogicXmax, 0); }
  public static void AddLogicZmin(FlatBufferBuilder builder, int LogicZmin) { builder.AddInt(25, LogicZmin, 0); }
  public static void AddLogicZmax(FlatBufferBuilder builder, int LogicZmax) { builder.AddInt(26, LogicZmax, 0); }
  public static void AddEntityinfo(FlatBufferBuilder builder, VectorOffset entityinfoOffset) { builder.AddOffset(27, entityinfoOffset.Value, 0); }
  public static VectorOffset CreateEntityinfoVector(FlatBufferBuilder builder, Offset<DEntityInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartEntityinfoVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBlocklayer(FlatBufferBuilder builder, VectorOffset blocklayerOffset) { builder.AddOffset(28, blocklayerOffset.Value, 0); }
  public static VectorOffset CreateBlocklayerVector(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); for (int i = data.Length - 1; i >= 0; i--) builder.AddByte(data[i]); return builder.EndVector(); }
  public static void StartBlocklayerVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
  public static void AddNpcinfo(FlatBufferBuilder builder, VectorOffset npcinfoOffset) { builder.AddOffset(29, npcinfoOffset.Value, 0); }
  public static VectorOffset CreateNpcinfoVector(FlatBufferBuilder builder, Offset<DNPCInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartNpcinfoVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMonsterinfo(FlatBufferBuilder builder, VectorOffset monsterinfoOffset) { builder.AddOffset(30, monsterinfoOffset.Value, 0); }
  public static VectorOffset CreateMonsterinfoVector(FlatBufferBuilder builder, Offset<DMonsterInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMonsterinfoVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDecoratorinfo(FlatBufferBuilder builder, VectorOffset decoratorinfoOffset) { builder.AddOffset(31, decoratorinfoOffset.Value, 0); }
  public static VectorOffset CreateDecoratorinfoVector(FlatBufferBuilder builder, Offset<DDecoratorInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDecoratorinfoVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDesructibleinfo(FlatBufferBuilder builder, VectorOffset desructibleinfoOffset) { builder.AddOffset(32, desructibleinfoOffset.Value, 0); }
  public static VectorOffset CreateDesructibleinfoVector(FlatBufferBuilder builder, Offset<DDestructibleInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDesructibleinfoVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddRegioninfo(FlatBufferBuilder builder, VectorOffset regioninfoOffset) { builder.AddOffset(33, regioninfoOffset.Value, 0); }
  public static VectorOffset CreateRegioninfoVector(FlatBufferBuilder builder, Offset<DRegionInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartRegioninfoVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTransportdoor(FlatBufferBuilder builder, VectorOffset transportdoorOffset) { builder.AddOffset(34, transportdoorOffset.Value, 0); }
  public static VectorOffset CreateTransportdoorVector(FlatBufferBuilder builder, Offset<DTransportDoor>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTransportdoorVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddFighterBornPosition(FlatBufferBuilder builder, VectorOffset fighterBornPositionOffset) { builder.AddOffset(35, fighterBornPositionOffset.Value, 0); }
  public static VectorOffset CreateFighterBornPositionVector(FlatBufferBuilder builder, Offset<DTransferInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartFighterBornPositionVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBirthposition(FlatBufferBuilder builder, Offset<DEntityInfo> birthpositionOffset) { builder.AddOffset(36, birthpositionOffset.Value, 0); }
  public static void AddHellbirthposition(FlatBufferBuilder builder, Offset<DEntityInfo> hellbirthpositionOffset) { builder.AddOffset(37, hellbirthpositionOffset.Value, 0); }
  public static void AddTownDoor(FlatBufferBuilder builder, VectorOffset townDoorOffset) { builder.AddOffset(38, townDoorOffset.Value, 0); }
  public static VectorOffset CreateTownDoorVector(FlatBufferBuilder builder, Offset<DTownDoor>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTownDoorVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddFunctionPrefab(FlatBufferBuilder builder, VectorOffset FunctionPrefabOffset) { builder.AddOffset(39, FunctionPrefabOffset.Value, 0); }
  public static VectorOffset CreateFunctionPrefabVector(FlatBufferBuilder builder, Offset<FunctionPrefab>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartFunctionPrefabVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<DSceneData> EndDSceneData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DSceneData>(o);
  }
  public static void FinishDSceneDataBuffer(FlatBufferBuilder builder, Offset<DSceneData> offset) { builder.Finish(offset.Value, "SCEN"); }
};


}
