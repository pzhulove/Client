// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class GlobalSettingTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1582321333,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static GlobalSettingTable GetRootAsGlobalSettingTable(ByteBuffer _bb) { return GetRootAsGlobalSettingTable(_bb, new GlobalSettingTable()); }
  public static GlobalSettingTable GetRootAsGlobalSettingTable(ByteBuffer _bb, GlobalSettingTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public GlobalSettingTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int walkSpeedArray(int j) { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int walkSpeedLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetWalkSpeedBytes() { return __p.__vector_as_arraysegment(6); }
 private FlatBufferArray<int> walkSpeedValue;
 public FlatBufferArray<int>  walkSpeed
 {
  get{
  if (walkSpeedValue == null)
  {
    walkSpeedValue = new FlatBufferArray<int>(this.walkSpeedArray, this.walkSpeedLength);
  }
  return walkSpeedValue;}
 }
  public int runSpeedArray(int j) { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int runSpeedLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetRunSpeedBytes() { return __p.__vector_as_arraysegment(8); }
 private FlatBufferArray<int> runSpeedValue;
 public FlatBufferArray<int>  runSpeed
 {
  get{
  if (runSpeedValue == null)
  {
    runSpeedValue = new FlatBufferArray<int>(this.runSpeedArray, this.runSpeedLength);
  }
  return runSpeedValue;}
 }
  public int townWalkSpeedArray(int j) { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int townWalkSpeedLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetTownWalkSpeedBytes() { return __p.__vector_as_arraysegment(10); }
 private FlatBufferArray<int> townWalkSpeedValue;
 public FlatBufferArray<int>  townWalkSpeed
 {
  get{
  if (townWalkSpeedValue == null)
  {
    townWalkSpeedValue = new FlatBufferArray<int>(this.townWalkSpeedArray, this.townWalkSpeedLength);
  }
  return townWalkSpeedValue;}
 }
  public int townRunSpeedArray(int j) { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int townRunSpeedLength { get { int o = __p.__offset(12); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetTownRunSpeedBytes() { return __p.__vector_as_arraysegment(12); }
 private FlatBufferArray<int> townRunSpeedValue;
 public FlatBufferArray<int>  townRunSpeed
 {
  get{
  if (townRunSpeedValue == null)
  {
    townRunSpeedValue = new FlatBufferArray<int>(this.townRunSpeedArray, this.townRunSpeedLength);
  }
  return townRunSpeedValue;}
 }
  public int hurtTimeArray(int j) { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int hurtTimeLength { get { int o = __p.__offset(14); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetHurtTimeBytes() { return __p.__vector_as_arraysegment(14); }
 private FlatBufferArray<int> hurtTimeValue;
 public FlatBufferArray<int>  hurtTime
 {
  get{
  if (hurtTimeValue == null)
  {
    hurtTimeValue = new FlatBufferArray<int>(this.hurtTimeArray, this.hurtTimeLength);
  }
  return hurtTimeValue;}
 }
  public int frozenPercent { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int jumpBackSpeedArray(int j) { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int jumpBackSpeedLength { get { int o = __p.__offset(18); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetJumpBackSpeedBytes() { return __p.__vector_as_arraysegment(18); }
 private FlatBufferArray<int> jumpBackSpeedValue;
 public FlatBufferArray<int>  jumpBackSpeed
 {
  get{
  if (jumpBackSpeedValue == null)
  {
    jumpBackSpeedValue = new FlatBufferArray<int>(this.jumpBackSpeedArray, this.jumpBackSpeedLength);
  }
  return jumpBackSpeedValue;}
 }
  public int gravity { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int fallGravityReduceFactor { get { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int defaultFloatHurt { get { int o = __p.__offset(24); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int defaultFloatLevelHurat { get { int o = __p.__offset(26); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int defaultGroundHurt { get { int o = __p.__offset(28); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int defaultStandHurt { get { int o = __p.__offset(30); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int fallProtectGravityAddFactor { get { int o = __p.__offset(32); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int protectClearDuration { get { int o = __p.__offset(34); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int zDimFactor { get { int o = __p.__offset(36); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int aiWanderRange { get { int o = __p.__offset(38); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int aiWAlkBackRange { get { int o = __p.__offset(40); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int aiMaxWalkCmdCount { get { int o = __p.__offset(42); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int aiMaxWalkCmdCount_RANGED { get { int o = __p.__offset(44); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int aiMaxIdleCmdcount { get { int o = __p.__offset(46); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int aiSkillAttackPassive { get { int o = __p.__offset(48); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int monsterGetupBatiFactor { get { int o = __p.__offset(50); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int degangBackDistance { get { int o = __p.__offset(52); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int monsterWalkSpeedArray(int j) { int o = __p.__offset(54); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int monsterWalkSpeedLength { get { int o = __p.__offset(54); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetMonsterWalkSpeedBytes() { return __p.__vector_as_arraysegment(54); }
 private FlatBufferArray<int> monsterWalkSpeedValue;
 public FlatBufferArray<int>  monsterWalkSpeed
 {
  get{
  if (monsterWalkSpeedValue == null)
  {
    monsterWalkSpeedValue = new FlatBufferArray<int>(this.monsterWalkSpeedArray, this.monsterWalkSpeedLength);
  }
  return monsterWalkSpeedValue;}
 }
  public int monsterRunSpeedArray(int j) { int o = __p.__offset(56); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int monsterRunSpeedLength { get { int o = __p.__offset(56); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetMonsterRunSpeedBytes() { return __p.__vector_as_arraysegment(56); }
 private FlatBufferArray<int> monsterRunSpeedValue;
 public FlatBufferArray<int>  monsterRunSpeed
 {
  get{
  if (monsterRunSpeedValue == null)
  {
    monsterRunSpeedValue = new FlatBufferArray<int>(this.monsterRunSpeedArray, this.monsterRunSpeedLength);
  }
  return monsterRunSpeedValue;}
 }
  public bool forceUseAutoFight { get { int o = __p.__offset(58); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public int afThinkTerm { get { int o = __p.__offset(60); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int afFindTargetTerm { get { int o = __p.__offset(62); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int afChangeDestinationTerm { get { int o = __p.__offset(64); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int autoCheckRestoreInterval { get { int o = __p.__offset(66); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int monsterWalkSpeedFactor { get { int o = __p.__offset(68); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int monsterSightFactor { get { int o = __p.__offset(70); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int dunFuTime { get { int o = __p.__offset(72); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int pvpDunFuTime { get { int o = __p.__offset(74); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int pkDamageAdjustFactor { get { int o = __p.__offset(76); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int pkHPAdjustFactor { get { int o = __p.__offset(78); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public bool pkUseMaxLevel { get { int o = __p.__offset(80); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public int pkHitRateAdd { get { int o = __p.__offset(82); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SwitchWeaponCD { get { int o = __p.__offset(84); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<GlobalSettingTable> CreateGlobalSettingTable(FlatBufferBuilder builder,
      int ID = 0,
      VectorOffset walkSpeedOffset = default(VectorOffset),
      VectorOffset runSpeedOffset = default(VectorOffset),
      VectorOffset townWalkSpeedOffset = default(VectorOffset),
      VectorOffset townRunSpeedOffset = default(VectorOffset),
      VectorOffset hurtTimeOffset = default(VectorOffset),
      int frozenPercent = 0,
      VectorOffset jumpBackSpeedOffset = default(VectorOffset),
      int gravity = 0,
      int fallGravityReduceFactor = 0,
      int defaultFloatHurt = 0,
      int defaultFloatLevelHurat = 0,
      int defaultGroundHurt = 0,
      int defaultStandHurt = 0,
      int fallProtectGravityAddFactor = 0,
      int protectClearDuration = 0,
      int zDimFactor = 0,
      int aiWanderRange = 0,
      int aiWAlkBackRange = 0,
      int aiMaxWalkCmdCount = 0,
      int aiMaxWalkCmdCount_RANGED = 0,
      int aiMaxIdleCmdcount = 0,
      int aiSkillAttackPassive = 0,
      int monsterGetupBatiFactor = 0,
      int degangBackDistance = 0,
      VectorOffset monsterWalkSpeedOffset = default(VectorOffset),
      VectorOffset monsterRunSpeedOffset = default(VectorOffset),
      bool forceUseAutoFight = false,
      int afThinkTerm = 0,
      int afFindTargetTerm = 0,
      int afChangeDestinationTerm = 0,
      int autoCheckRestoreInterval = 0,
      int monsterWalkSpeedFactor = 0,
      int monsterSightFactor = 0,
      int dunFuTime = 0,
      int pvpDunFuTime = 0,
      int pkDamageAdjustFactor = 0,
      int pkHPAdjustFactor = 0,
      bool pkUseMaxLevel = false,
      int pkHitRateAdd = 0,
      int SwitchWeaponCD = 0) {
    builder.StartObject(41);
    GlobalSettingTable.AddSwitchWeaponCD(builder, SwitchWeaponCD);
    GlobalSettingTable.AddPkHitRateAdd(builder, pkHitRateAdd);
    GlobalSettingTable.AddPkHPAdjustFactor(builder, pkHPAdjustFactor);
    GlobalSettingTable.AddPkDamageAdjustFactor(builder, pkDamageAdjustFactor);
    GlobalSettingTable.AddPvpDunFuTime(builder, pvpDunFuTime);
    GlobalSettingTable.AddDunFuTime(builder, dunFuTime);
    GlobalSettingTable.AddMonsterSightFactor(builder, monsterSightFactor);
    GlobalSettingTable.AddMonsterWalkSpeedFactor(builder, monsterWalkSpeedFactor);
    GlobalSettingTable.AddAutoCheckRestoreInterval(builder, autoCheckRestoreInterval);
    GlobalSettingTable.AddAfChangeDestinationTerm(builder, afChangeDestinationTerm);
    GlobalSettingTable.AddAfFindTargetTerm(builder, afFindTargetTerm);
    GlobalSettingTable.AddAfThinkTerm(builder, afThinkTerm);
    GlobalSettingTable.AddMonsterRunSpeed(builder, monsterRunSpeedOffset);
    GlobalSettingTable.AddMonsterWalkSpeed(builder, monsterWalkSpeedOffset);
    GlobalSettingTable.AddDegangBackDistance(builder, degangBackDistance);
    GlobalSettingTable.AddMonsterGetupBatiFactor(builder, monsterGetupBatiFactor);
    GlobalSettingTable.AddAiSkillAttackPassive(builder, aiSkillAttackPassive);
    GlobalSettingTable.AddAiMaxIdleCmdcount(builder, aiMaxIdleCmdcount);
    GlobalSettingTable.AddAiMaxWalkCmdCountRANGED(builder, aiMaxWalkCmdCount_RANGED);
    GlobalSettingTable.AddAiMaxWalkCmdCount(builder, aiMaxWalkCmdCount);
    GlobalSettingTable.AddAiWAlkBackRange(builder, aiWAlkBackRange);
    GlobalSettingTable.AddAiWanderRange(builder, aiWanderRange);
    GlobalSettingTable.AddZDimFactor(builder, zDimFactor);
    GlobalSettingTable.AddProtectClearDuration(builder, protectClearDuration);
    GlobalSettingTable.AddFallProtectGravityAddFactor(builder, fallProtectGravityAddFactor);
    GlobalSettingTable.AddDefaultStandHurt(builder, defaultStandHurt);
    GlobalSettingTable.AddDefaultGroundHurt(builder, defaultGroundHurt);
    GlobalSettingTable.AddDefaultFloatLevelHurat(builder, defaultFloatLevelHurat);
    GlobalSettingTable.AddDefaultFloatHurt(builder, defaultFloatHurt);
    GlobalSettingTable.AddFallGravityReduceFactor(builder, fallGravityReduceFactor);
    GlobalSettingTable.AddGravity(builder, gravity);
    GlobalSettingTable.AddJumpBackSpeed(builder, jumpBackSpeedOffset);
    GlobalSettingTable.AddFrozenPercent(builder, frozenPercent);
    GlobalSettingTable.AddHurtTime(builder, hurtTimeOffset);
    GlobalSettingTable.AddTownRunSpeed(builder, townRunSpeedOffset);
    GlobalSettingTable.AddTownWalkSpeed(builder, townWalkSpeedOffset);
    GlobalSettingTable.AddRunSpeed(builder, runSpeedOffset);
    GlobalSettingTable.AddWalkSpeed(builder, walkSpeedOffset);
    GlobalSettingTable.AddID(builder, ID);
    GlobalSettingTable.AddPkUseMaxLevel(builder, pkUseMaxLevel);
    GlobalSettingTable.AddForceUseAutoFight(builder, forceUseAutoFight);
    return GlobalSettingTable.EndGlobalSettingTable(builder);
  }

  public static void StartGlobalSettingTable(FlatBufferBuilder builder) { builder.StartObject(41); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddWalkSpeed(FlatBufferBuilder builder, VectorOffset walkSpeedOffset) { builder.AddOffset(1, walkSpeedOffset.Value, 0); }
  public static VectorOffset CreateWalkSpeedVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartWalkSpeedVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddRunSpeed(FlatBufferBuilder builder, VectorOffset runSpeedOffset) { builder.AddOffset(2, runSpeedOffset.Value, 0); }
  public static VectorOffset CreateRunSpeedVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartRunSpeedVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTownWalkSpeed(FlatBufferBuilder builder, VectorOffset townWalkSpeedOffset) { builder.AddOffset(3, townWalkSpeedOffset.Value, 0); }
  public static VectorOffset CreateTownWalkSpeedVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartTownWalkSpeedVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTownRunSpeed(FlatBufferBuilder builder, VectorOffset townRunSpeedOffset) { builder.AddOffset(4, townRunSpeedOffset.Value, 0); }
  public static VectorOffset CreateTownRunSpeedVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartTownRunSpeedVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHurtTime(FlatBufferBuilder builder, VectorOffset hurtTimeOffset) { builder.AddOffset(5, hurtTimeOffset.Value, 0); }
  public static VectorOffset CreateHurtTimeVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartHurtTimeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddFrozenPercent(FlatBufferBuilder builder, int frozenPercent) { builder.AddInt(6, frozenPercent, 0); }
  public static void AddJumpBackSpeed(FlatBufferBuilder builder, VectorOffset jumpBackSpeedOffset) { builder.AddOffset(7, jumpBackSpeedOffset.Value, 0); }
  public static VectorOffset CreateJumpBackSpeedVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartJumpBackSpeedVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddGravity(FlatBufferBuilder builder, int gravity) { builder.AddInt(8, gravity, 0); }
  public static void AddFallGravityReduceFactor(FlatBufferBuilder builder, int fallGravityReduceFactor) { builder.AddInt(9, fallGravityReduceFactor, 0); }
  public static void AddDefaultFloatHurt(FlatBufferBuilder builder, int defaultFloatHurt) { builder.AddInt(10, defaultFloatHurt, 0); }
  public static void AddDefaultFloatLevelHurat(FlatBufferBuilder builder, int defaultFloatLevelHurat) { builder.AddInt(11, defaultFloatLevelHurat, 0); }
  public static void AddDefaultGroundHurt(FlatBufferBuilder builder, int defaultGroundHurt) { builder.AddInt(12, defaultGroundHurt, 0); }
  public static void AddDefaultStandHurt(FlatBufferBuilder builder, int defaultStandHurt) { builder.AddInt(13, defaultStandHurt, 0); }
  public static void AddFallProtectGravityAddFactor(FlatBufferBuilder builder, int fallProtectGravityAddFactor) { builder.AddInt(14, fallProtectGravityAddFactor, 0); }
  public static void AddProtectClearDuration(FlatBufferBuilder builder, int protectClearDuration) { builder.AddInt(15, protectClearDuration, 0); }
  public static void AddZDimFactor(FlatBufferBuilder builder, int zDimFactor) { builder.AddInt(16, zDimFactor, 0); }
  public static void AddAiWanderRange(FlatBufferBuilder builder, int aiWanderRange) { builder.AddInt(17, aiWanderRange, 0); }
  public static void AddAiWAlkBackRange(FlatBufferBuilder builder, int aiWAlkBackRange) { builder.AddInt(18, aiWAlkBackRange, 0); }
  public static void AddAiMaxWalkCmdCount(FlatBufferBuilder builder, int aiMaxWalkCmdCount) { builder.AddInt(19, aiMaxWalkCmdCount, 0); }
  public static void AddAiMaxWalkCmdCountRANGED(FlatBufferBuilder builder, int aiMaxWalkCmdCountRANGED) { builder.AddInt(20, aiMaxWalkCmdCountRANGED, 0); }
  public static void AddAiMaxIdleCmdcount(FlatBufferBuilder builder, int aiMaxIdleCmdcount) { builder.AddInt(21, aiMaxIdleCmdcount, 0); }
  public static void AddAiSkillAttackPassive(FlatBufferBuilder builder, int aiSkillAttackPassive) { builder.AddInt(22, aiSkillAttackPassive, 0); }
  public static void AddMonsterGetupBatiFactor(FlatBufferBuilder builder, int monsterGetupBatiFactor) { builder.AddInt(23, monsterGetupBatiFactor, 0); }
  public static void AddDegangBackDistance(FlatBufferBuilder builder, int degangBackDistance) { builder.AddInt(24, degangBackDistance, 0); }
  public static void AddMonsterWalkSpeed(FlatBufferBuilder builder, VectorOffset monsterWalkSpeedOffset) { builder.AddOffset(25, monsterWalkSpeedOffset.Value, 0); }
  public static VectorOffset CreateMonsterWalkSpeedVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartMonsterWalkSpeedVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMonsterRunSpeed(FlatBufferBuilder builder, VectorOffset monsterRunSpeedOffset) { builder.AddOffset(26, monsterRunSpeedOffset.Value, 0); }
  public static VectorOffset CreateMonsterRunSpeedVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartMonsterRunSpeedVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddForceUseAutoFight(FlatBufferBuilder builder, bool forceUseAutoFight) { builder.AddBool(27, forceUseAutoFight, false); }
  public static void AddAfThinkTerm(FlatBufferBuilder builder, int afThinkTerm) { builder.AddInt(28, afThinkTerm, 0); }
  public static void AddAfFindTargetTerm(FlatBufferBuilder builder, int afFindTargetTerm) { builder.AddInt(29, afFindTargetTerm, 0); }
  public static void AddAfChangeDestinationTerm(FlatBufferBuilder builder, int afChangeDestinationTerm) { builder.AddInt(30, afChangeDestinationTerm, 0); }
  public static void AddAutoCheckRestoreInterval(FlatBufferBuilder builder, int autoCheckRestoreInterval) { builder.AddInt(31, autoCheckRestoreInterval, 0); }
  public static void AddMonsterWalkSpeedFactor(FlatBufferBuilder builder, int monsterWalkSpeedFactor) { builder.AddInt(32, monsterWalkSpeedFactor, 0); }
  public static void AddMonsterSightFactor(FlatBufferBuilder builder, int monsterSightFactor) { builder.AddInt(33, monsterSightFactor, 0); }
  public static void AddDunFuTime(FlatBufferBuilder builder, int dunFuTime) { builder.AddInt(34, dunFuTime, 0); }
  public static void AddPvpDunFuTime(FlatBufferBuilder builder, int pvpDunFuTime) { builder.AddInt(35, pvpDunFuTime, 0); }
  public static void AddPkDamageAdjustFactor(FlatBufferBuilder builder, int pkDamageAdjustFactor) { builder.AddInt(36, pkDamageAdjustFactor, 0); }
  public static void AddPkHPAdjustFactor(FlatBufferBuilder builder, int pkHPAdjustFactor) { builder.AddInt(37, pkHPAdjustFactor, 0); }
  public static void AddPkUseMaxLevel(FlatBufferBuilder builder, bool pkUseMaxLevel) { builder.AddBool(38, pkUseMaxLevel, false); }
  public static void AddPkHitRateAdd(FlatBufferBuilder builder, int pkHitRateAdd) { builder.AddInt(39, pkHitRateAdd, 0); }
  public static void AddSwitchWeaponCD(FlatBufferBuilder builder, int SwitchWeaponCD) { builder.AddInt(40, SwitchWeaponCD, 0); }
  public static Offset<GlobalSettingTable> EndGlobalSettingTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GlobalSettingTable>(o);
  }
  public static void FinishGlobalSettingTableBuffer(FlatBufferBuilder builder, Offset<GlobalSettingTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

