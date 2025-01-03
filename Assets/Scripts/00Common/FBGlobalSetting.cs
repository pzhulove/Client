// automatically generated, do not modify

namespace FBGlobalSetting
{

using FlatBuffers;

public sealed class Color : Struct {
  public Color __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float A { get { return bb.GetFloat(bb_pos + 0); } }
  public float B { get { return bb.GetFloat(bb_pos + 4); } }
  public float G { get { return bb.GetFloat(bb_pos + 8); } }
  public float R { get { return bb.GetFloat(bb_pos + 12); } }

  public static Offset<Color> CreateColor(FlatBufferBuilder builder, float A, float B, float G, float R) {
    builder.Prep(4, 16);
    builder.PutFloat(R);
    builder.PutFloat(G);
    builder.PutFloat(B);
    builder.PutFloat(A);
    return new Offset<Color>(builder.Offset);
  }
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

public sealed class QualityAdjust : Struct {
  public QualityAdjust __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public bool BIsOpen { get { return 0!=bb.Get(bb_pos + 0); } }
  public float FInterval { get { return bb.GetFloat(bb_pos + 4); } }
  public int ITimes { get { return bb.GetInt(bb_pos + 8); } }

  public static Offset<QualityAdjust> CreateQualityAdjust(FlatBufferBuilder builder, bool BIsOpen, float FInterval, int ITimes) {
    builder.Prep(4, 12);
    builder.PutInt(ITimes);
    builder.PutFloat(FInterval);
    builder.Pad(3);
    builder.PutBool(BIsOpen);
    return new Offset<QualityAdjust>(builder.Offset);
  }
};

public sealed class Address : Table {
  public static Address GetRootAsAddress(ByteBuffer _bb) { return GetRootAsAddress(_bb, new Address()); }
  public static Address GetRootAsAddress(ByteBuffer _bb, Address obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Address __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public string Ip { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ushort Port { get { int o = __offset(8); return o != 0 ? bb.GetUshort(o + bb_pos) : (ushort)0; } }
  public uint Id { get { int o = __offset(10); return o != 0 ? bb.GetUint(o + bb_pos) : (uint)0; } }

  public static Offset<Address> CreateAddress(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      StringOffset ip = default(StringOffset),
      ushort port = 0,
      uint id = 0) {
    builder.StartObject(4);
    Address.AddId(builder, id);
    Address.AddIp(builder, ip);
    Address.AddName(builder, name);
    Address.AddPort(builder, port);
    return Address.EndAddress(builder);
  }

  public static void StartAddress(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddIp(FlatBufferBuilder builder, StringOffset ipOffset) { builder.AddOffset(1, ipOffset.Value, 0); }
  public static void AddPort(FlatBufferBuilder builder, ushort port) { builder.AddUshort(2, port, 0); }
  public static void AddId(FlatBufferBuilder builder, uint id) { builder.AddUint(3, id, 0); }
  public static Offset<Address> EndAddress(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Address>(o);
  }
};

public sealed class ShockData : Struct {
  public ShockData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float Time { get { return bb.GetFloat(bb_pos + 0); } }
  public float Speed { get { return bb.GetFloat(bb_pos + 4); } }
  public float Xrange { get { return bb.GetFloat(bb_pos + 8); } }
  public float Yrange { get { return bb.GetFloat(bb_pos + 12); } }
  public int Mode { get { return bb.GetInt(bb_pos + 16); } }

  public static Offset<ShockData> CreateShockData(FlatBufferBuilder builder, float Time, float Speed, float Xrange, float Yrange, int Mode) {
    builder.Prep(4, 20);
    builder.PutInt(Mode);
    builder.PutFloat(Yrange);
    builder.PutFloat(Xrange);
    builder.PutFloat(Speed);
    builder.PutFloat(Time);
    return new Offset<ShockData>(builder.Offset);
  }
};

public sealed class GlobalSetting : Table {
  public static GlobalSetting GetRootAsGlobalSetting(ByteBuffer _bb) { return GetRootAsGlobalSetting(_bb, new GlobalSetting()); }
  public static GlobalSetting GetRootAsGlobalSetting(ByteBuffer _bb, GlobalSetting obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public GlobalSetting __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public bool IsTestTeam { get { int o = __offset(4); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool IsPaySDKDebug { get { int o = __offset(6); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int SdkChannel { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool IsBanShuVersion { get { int o = __offset(10); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool IsDebug { get { int o = __offset(12); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool IsLogRecord { get { int o = __offset(14); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool IsRecordPVP { get { int o = __offset(16); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool ShowDebugBox { get { int o = __offset(18); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int FrameLock { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float FallgroundHitFactor { get { int o = __offset(22); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float HitTime { get { int o = __offset(24); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float DeadWhiteTime { get { int o = __offset(26); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string DefaultHitEffect { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public string DefaultProjectileHitEffect { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public string DefualtHitSfx { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public Vector3 _walkSpeed { get { return Get_walkSpeed(new Vector3()); } }
  public Vector3 Get_walkSpeed(Vector3 obj) { int o = __offset(34); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 _runSpeed { get { return Get_runSpeed(new Vector3()); } }
  public Vector3 Get_runSpeed(Vector3 obj) { int o = __offset(36); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 TownWalkSpeed { get { return GetTownWalkSpeed(new Vector3()); } }
  public Vector3 GetTownWalkSpeed(Vector3 obj) { int o = __offset(38); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 TownRunSpeed { get { return GetTownRunSpeed(new Vector3()); } }
  public Vector3 GetTownRunSpeed(Vector3 obj) { int o = __offset(40); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float TownActionSpeed { get { int o = __offset(42); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool TownPlayerRun { get { int o = __offset(44); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int MinHurtTime { get { int o = __offset(46); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MaxHurtTime { get { int o = __offset(48); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float FrozenPercent { get { int o = __offset(50); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public Vector2 JumpBackSpeed { get { return GetJumpBackSpeed(new Vector2()); } }
  public Vector2 GetJumpBackSpeed(Vector2 obj) { int o = __offset(52); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float JumpForce { get { int o = __offset(54); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float ClickForce { get { int o = __offset(56); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float SnapDuration { get { int o = __offset(58); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float _dunFuTime { get { int o = __offset(60); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float _pvpDunFuTime { get { int o = __offset(62); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int PVPHPScale { get { int o = __offset(64); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int TestLevel { get { int o = __offset(66); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int TestPlayerNum { get { int o = __offset(68); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool ShowBattleInfoPanel { get { int o = __offset(70); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int DefaultMonsterID { get { int o = __offset(72); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float _monsterWalkSpeedFactor { get { int o = __offset(74); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float _monsterSightFactor { get { int o = __offset(76); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool EnableAssetInstPool { get { int o = __offset(78); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool EnemyHasAI { get { int o = __offset(80); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool IsCreateMonsterLocal { get { int o = __offset(82); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool IsGiveEquips { get { int o = __offset(84); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public string EquipList { get { int o = __offset(86); return o != 0 ? __string(o + bb_pos) : null; } }
  public bool IsGuide { get { int o = __offset(88); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool DisplayHUD { get { int o = __offset(90); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool CloseTeamCondition { get { int o = __offset(92); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool IsLocalDungeon { get { int o = __offset(94); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int LocalDungeonID { get { int o = __offset(96); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool RecordResFile { get { int o = __offset(98); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool ProfileAssetLoad { get { int o = __offset(100); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public float _gravity { get { int o = __offset(102); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float _fallGravityReduceFactor { get { int o = __offset(104); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool SkillHasCooldown { get { int o = __offset(106); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public string ScenePath { get { int o = __offset(108); return o != 0 ? __string(o + bb_pos) : null; } }
  public int IpSelectedIndex { get { int o = __offset(110); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ISingleCharactorID { get { int o = __offset(112); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public Vector2 CameraInRange { get { return GetCameraInRange(new Vector2()); } }
  public Vector2 GetCameraInRange(Vector2 obj) { int o = __offset(114); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public int ButtonType { get { int o = __offset(116); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float _defaultFloatHurt { get { int o = __offset(118); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float _defaultFloatLevelHurat { get { int o = __offset(120); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float _defaultGroundHurt { get { int o = __offset(122); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float _defaultStandHurt { get { int o = __offset(124); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float _fallProtectGravityAddFactor { get { int o = __offset(126); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int _protectClearDuration { get { int o = __offset(128); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float BgmStart { get { int o = __offset(130); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float BgmTown { get { int o = __offset(132); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float BgmBattle { get { int o = __offset(134); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float _zDimFactor { get { int o = __offset(136); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float BullteScale { get { int o = __offset(138); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int BullteTime { get { int o = __offset(140); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int StartSystem { get { int o = __offset(142); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string GetLoggerFilter(int j) { int o = __offset(144); return o != 0 ? __string(__vector(o) + j * 4) : null; }
  public int LoggerFilterLength { get { int o = __offset(144); return o != 0 ? __vector_len(o) : 0; } }
  public bool ShowDialog { get { int o = __offset(146); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public Vector3 AvatarLightDir { get { return GetAvatarLightDir(new Vector3()); } }
  public Vector3 GetAvatarLightDir(Vector3 obj) { int o = __offset(148); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 ShadowLightDir { get { return GetShadowLightDir(new Vector3()); } }
  public Vector3 GetShadowLightDir(Vector3 obj) { int o = __offset(150); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 StartVel { get { return GetStartVel(new Vector3()); } }
  public Vector3 GetStartVel(Vector3 obj) { int o = __offset(152); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 EndVel { get { return GetEndVel(new Vector3()); } }
  public Vector3 GetEndVel(Vector3 obj) { int o = __offset(154); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 Offset { get { return GetOffset(new Vector3()); } }
  public Vector3 GetOffset(Vector3 obj) { int o = __offset(156); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float TimeAccerlate { get { int o = __offset(158); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int TotalTime { get { int o = __offset(160); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int TotalDist { get { int o = __offset(162); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool HeightAdoption { get { int o = __offset(164); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool DebugDrawBlock { get { int o = __offset(166); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool LoadFromPackage { get { int o = __offset(168); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool EnableHotFix { get { int o = __offset(170); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool HotFixUrlDebug { get { int o = __offset(172); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int REVIVESHOCKSKILLID { get { int o = __offset(174); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public Vector2 RollSpeed { get { return GetRollSpeed(new Vector2()); } }
  public Vector2 GetRollSpeed(Vector2 obj) { int o = __offset(176); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float RollRand { get { int o = __offset(178); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float NormalRollRand { get { int o = __offset(180); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float _pkDamageAdjustFactor { get { int o = __offset(182); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float _pkHPAdjustFactor { get { int o = __offset(184); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool PkUseMaxLevel { get { int o = __offset(186); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int BattleRunMode { get { int o = __offset(188); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool HasDoubleRun { get { int o = __offset(190); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int PlayerHP { get { int o = __offset(192); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PlayerRebornHP { get { int o = __offset(194); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MonsterHP { get { int o = __offset(196); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public Vector3 PlayerPos { get { return GetPlayerPos(new Vector3()); } }
  public Vector3 GetPlayerPos(Vector3 obj) { int o = __offset(198); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float TransportDoorRadius { get { int o = __offset(200); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float PetXMovingDis { get { int o = __offset(202); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float PetXMovingv1 { get { int o = __offset(204); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float PetXMovingv2 { get { int o = __offset(206); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float PetYMovingDis { get { int o = __offset(208); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float PetYMovingv1 { get { int o = __offset(210); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float PetYMovingv2 { get { int o = __offset(212); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string ServerListUrl { get { int o = __offset(214); return o != 0 ? __string(o + bb_pos) : null; } }
  public Address GetServerList(int j) { return GetServerList(new Address(), j); }
  public Address GetServerList(Address obj, int j) { int o = __offset(216); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ServerListLength { get { int o = __offset(216); return o != 0 ? __vector_len(o) : 0; } }
  public bool DebugNewAutofightAI { get { int o = __offset(218); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public float CharScale { get { int o = __offset(220); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public ShockData MonsterBeHitShockData { get { return GetMonsterBeHitShockData(new ShockData()); } }
  public ShockData GetMonsterBeHitShockData(ShockData obj) { int o = __offset(222); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public ShockData PlayerBeHitShockData { get { return GetPlayerBeHitShockData(new ShockData()); } }
  public ShockData GetPlayerBeHitShockData(ShockData obj) { int o = __offset(224); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public ShockData PlayerSkillCDShockData { get { return GetPlayerSkillCDShockData(new ShockData()); } }
  public ShockData GetPlayerSkillCDShockData(ShockData obj) { int o = __offset(226); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public ShockData PlayerHighFallTouchGroundShockData { get { return GetPlayerHighFallTouchGroundShockData(new ShockData()); } }
  public ShockData GetPlayerHighFallTouchGroundShockData(ShockData obj) { int o = __offset(228); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float HighFallHight { get { int o = __offset(230); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string CritialDeadEffect { get { int o = __offset(232); return o != 0 ? __string(o + bb_pos) : null; } }
  public string ImmediateDeadEffect { get { int o = __offset(234); return o != 0 ? __string(o + bb_pos) : null; } }
  public string NormalDeadEffect { get { int o = __offset(236); return o != 0 ? __string(o + bb_pos) : null; } }
  public bool EnableEffectLimit { get { int o = __offset(238); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int EffectLimitCount { get { int o = __offset(240); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ImmediateDeadHPPercent { get { int o = __offset(242); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool OpenBossShow { get { int o = __offset(244); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public float ShooterFitPercent { get { int o = __offset(246); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public Vector3 DisappearDis { get { return GetDisappearDis(new Vector3()); } }
  public Vector3 GetDisappearDis(Vector3 obj) { int o = __offset(248); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float KeepDis { get { int o = __offset(250); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float AccompanyRunTime { get { int o = __offset(252); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int _aiWanderRange { get { int o = __offset(254); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int _aiWAlkBackRange { get { int o = __offset(256); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int _aiMaxWalkCmdCount { get { int o = __offset(258); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int _aiMaxWalkCmdCountRANGED { get { int o = __offset(260); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int _aiMaxIdleCmdcount { get { int o = __offset(262); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int _aiSkillAttackPassive { get { int o = __offset(264); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float _monsterGetupBatiFactor { get { int o = __offset(266); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float _degangBackDistance { get { int o = __offset(268); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int _afThinkTerm { get { int o = __offset(270); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int _afFindTargetTerm { get { int o = __offset(272); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int _afChangeDestinationTerm { get { int o = __offset(274); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int _autoCheckRestoreInterval { get { int o = __offset(276); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool ForceUseAutoFight { get { int o = __offset(278); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool CanUseAutoFight { get { int o = __offset(280); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool CanUseAutoFightFirstPass { get { int o = __offset(282); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool LoadAutoFight { get { int o = __offset(284); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public float JumpAttackLimitHeight { get { int o = __offset(286); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float SkillCancelLimitTime { get { int o = __offset(288); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int DoublePressCheckDuration { get { int o = __offset(290); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int WalkAction { get { int o = __offset(292); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RunAction { get { int o = __offset(294); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float WalkAniFactor { get { int o = __offset(296); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float RunAniFactor { get { int o = __offset(298); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool ChangeFaceStop { get { int o = __offset(300); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public Vector3 _monsterWalkSpeed { get { return Get_monsterWalkSpeed(new Vector3()); } }
  public Vector3 Get_monsterWalkSpeed(Vector3 obj) { int o = __offset(302); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 _monsterRunSpeed { get { return Get_monsterRunSpeed(new Vector3()); } }
  public Vector3 Get_monsterRunSpeed(Vector3 obj) { int o = __offset(304); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public int TableLoadStep { get { int o = __offset(306); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LoadingProgressStepInEditor { get { int o = __offset(308); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LoadingProgressStep { get { int o = __offset(310); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string PvpDefaultSesstionID { get { int o = __offset(312); return o != 0 ? __string(o + bb_pos) : null; } }
  public int PetID { get { int o = __offset(314); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PetLevel { get { int o = __offset(316); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PetHunger { get { int o = __offset(318); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PetSkillIndex { get { int o = __offset(320); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool TestFashionEquip { get { int o = __offset(322); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public string GetEquipPropFactorsKey(int j) { int o = __offset(324); return o != 0 ? __string(__vector(o) + j * 4) : null; }
  public int EquipPropFactorsKeyLength { get { int o = __offset(324); return o != 0 ? __vector_len(o) : 0; } }
  public float GetEquipPropFactorsValue(int j) { int o = __offset(326); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int EquipPropFactorsValueLength { get { int o = __offset(326); return o != 0 ? __vector_len(o) : 0; } }
  public float GetEquipPropFactorValues(int j) { int o = __offset(328); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int EquipPropFactorValuesLength { get { int o = __offset(328); return o != 0 ? __vector_len(o) : 0; } }
  public string GetQuipThirdTypeFactorsKey(int j) { int o = __offset(330); return o != 0 ? __string(__vector(o) + j * 4) : null; }
  public int QuipThirdTypeFactorsKeyLength { get { int o = __offset(330); return o != 0 ? __vector_len(o) : 0; } }
  public float GetQuipThirdTypeFactorsValue(int j) { int o = __offset(332); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int QuipThirdTypeFactorsValueLength { get { int o = __offset(332); return o != 0 ? __vector_len(o) : 0; } }
  public float GetQuipThirdTypeFactorValues(int j) { int o = __offset(334); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int QuipThirdTypeFactorValuesLength { get { int o = __offset(334); return o != 0 ? __vector_len(o) : 0; } }
  public QualityAdjust QualityAdjust { get { return GetQualityAdjust(new QualityAdjust()); } }
  public QualityAdjust GetQualityAdjust(QualityAdjust obj) { int o = __offset(336); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float PetDialogLife { get { int o = __offset(338); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float PetDialogShowInterval { get { int o = __offset(340); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float PetSpecialIdleInterval { get { int o = __offset(342); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int NotifyItemTimeLess { get { int o = __offset(344); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool UseNewHurtAction { get { int o = __offset(346); return o != 0 ? 0 != bb.Get(o + bb_pos) : (bool)false; } }
  public bool UseNewGravity { get { int o = __offset(348); return o != 0 ? 0 != bb.Get(o + bb_pos) : (bool)false; } }
  public int SpeedAnchorArray(int j) { int o = __offset(350); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : 0; }
  public int SpeedAnchorArrayLength { get { int o = __offset(350); return o != 0 ? __vector_len(o) : 0; } }
  public int GravityRateArray(int j) { int o = __offset(352); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : 0; }
  public int GravityRateArrayLength { get { int o = __offset(352); return o != 0 ? __vector_len(o) : 0; } }

  public static void StartGlobalSetting(FlatBufferBuilder builder) { builder.StartObject(175); }
  public static void AddIsTestTeam(FlatBufferBuilder builder, bool isTestTeam) { builder.AddBool(0, isTestTeam, false); }
  public static void AddIsPaySDKDebug(FlatBufferBuilder builder, bool isPaySDKDebug) { builder.AddBool(1, isPaySDKDebug, false); }
  public static void AddSdkChannel(FlatBufferBuilder builder, int sdkChannel) { builder.AddInt(2, sdkChannel, 0); }
  public static void AddIsBanShuVersion(FlatBufferBuilder builder, bool isBanShuVersion) { builder.AddBool(3, isBanShuVersion, false); }
  public static void AddIsDebug(FlatBufferBuilder builder, bool isDebug) { builder.AddBool(4, isDebug, false); }
  public static void AddIsLogRecord(FlatBufferBuilder builder, bool isLogRecord) { builder.AddBool(5, isLogRecord, false); }
  public static void AddIsRecordPVP(FlatBufferBuilder builder, bool isRecordPVP) { builder.AddBool(6, isRecordPVP, false); }
  public static void AddShowDebugBox(FlatBufferBuilder builder, bool showDebugBox) { builder.AddBool(7, showDebugBox, false); }
  public static void AddFrameLock(FlatBufferBuilder builder, int frameLock) { builder.AddInt(8, frameLock, 0); }
  public static void AddFallgroundHitFactor(FlatBufferBuilder builder, float fallgroundHitFactor) { builder.AddFloat(9, fallgroundHitFactor, 0); }
  public static void AddHitTime(FlatBufferBuilder builder, float hitTime) { builder.AddFloat(10, hitTime, 0); }
  public static void AddDeadWhiteTime(FlatBufferBuilder builder, float deadWhiteTime) { builder.AddFloat(11, deadWhiteTime, 0); }
  public static void AddDefaultHitEffect(FlatBufferBuilder builder, StringOffset defaultHitEffectOffset) { builder.AddOffset(12, defaultHitEffectOffset.Value, 0); }
  public static void AddDefaultProjectileHitEffect(FlatBufferBuilder builder, StringOffset defaultProjectileHitEffectOffset) { builder.AddOffset(13, defaultProjectileHitEffectOffset.Value, 0); }
  public static void AddDefualtHitSfx(FlatBufferBuilder builder, StringOffset defualtHitSfxOffset) { builder.AddOffset(14, defualtHitSfxOffset.Value, 0); }
  public static void Add_walkSpeed(FlatBufferBuilder builder, Offset<Vector3> WalkSpeedOffset) { builder.AddStruct(15, WalkSpeedOffset.Value, 0); }
  public static void Add_runSpeed(FlatBufferBuilder builder, Offset<Vector3> RunSpeedOffset) { builder.AddStruct(16, RunSpeedOffset.Value, 0); }
  public static void AddTownWalkSpeed(FlatBufferBuilder builder, Offset<Vector3> townWalkSpeedOffset) { builder.AddStruct(17, townWalkSpeedOffset.Value, 0); }
  public static void AddTownRunSpeed(FlatBufferBuilder builder, Offset<Vector3> townRunSpeedOffset) { builder.AddStruct(18, townRunSpeedOffset.Value, 0); }
  public static void AddTownActionSpeed(FlatBufferBuilder builder, float townActionSpeed) { builder.AddFloat(19, townActionSpeed, 0); }
  public static void AddTownPlayerRun(FlatBufferBuilder builder, bool townPlayerRun) { builder.AddBool(20, townPlayerRun, false); }
  public static void AddMinHurtTime(FlatBufferBuilder builder, int minHurtTime) { builder.AddInt(21, minHurtTime, 0); }
  public static void AddMaxHurtTime(FlatBufferBuilder builder, int maxHurtTime) { builder.AddInt(22, maxHurtTime, 0); }
  public static void AddFrozenPercent(FlatBufferBuilder builder, float frozenPercent) { builder.AddFloat(23, frozenPercent, 0); }
  public static void AddJumpBackSpeed(FlatBufferBuilder builder, Offset<Vector2> jumpBackSpeedOffset) { builder.AddStruct(24, jumpBackSpeedOffset.Value, 0); }
  public static void AddJumpForce(FlatBufferBuilder builder, float jumpForce) { builder.AddFloat(25, jumpForce, 0); }
  public static void AddClickForce(FlatBufferBuilder builder, float clickForce) { builder.AddFloat(26, clickForce, 0); }
  public static void AddSnapDuration(FlatBufferBuilder builder, float snapDuration) { builder.AddFloat(27, snapDuration, 0); }
  public static void Add_dunFuTime(FlatBufferBuilder builder, float DunFuTime) { builder.AddFloat(28, DunFuTime, 0); }
  public static void Add_pvpDunFuTime(FlatBufferBuilder builder, float PvpDunFuTime) { builder.AddFloat(29, PvpDunFuTime, 0); }
  public static void AddPVPHPScale(FlatBufferBuilder builder, int PVPHPScale) { builder.AddInt(30, PVPHPScale, 0); }
  public static void AddTestLevel(FlatBufferBuilder builder, int TestLevel) { builder.AddInt(31, TestLevel, 0); }
  public static void AddTestPlayerNum(FlatBufferBuilder builder, int testPlayerNum) { builder.AddInt(32, testPlayerNum, 0); }
  public static void AddShowBattleInfoPanel(FlatBufferBuilder builder, bool showBattleInfoPanel) { builder.AddBool(33, showBattleInfoPanel, false); }
  public static void AddDefaultMonsterID(FlatBufferBuilder builder, int defaultMonsterID) { builder.AddInt(34, defaultMonsterID, 0); }
  public static void Add_monsterWalkSpeedFactor(FlatBufferBuilder builder, float MonsterWalkSpeedFactor) { builder.AddFloat(35, MonsterWalkSpeedFactor, 0); }
  public static void Add_monsterSightFactor(FlatBufferBuilder builder, float MonsterSightFactor) { builder.AddFloat(36, MonsterSightFactor, 0); }
  public static void AddEnableAssetInstPool(FlatBufferBuilder builder, bool enableAssetInstPool) { builder.AddBool(37, enableAssetInstPool, false); }
  public static void AddEnemyHasAI(FlatBufferBuilder builder, bool enemyHasAI) { builder.AddBool(38, enemyHasAI, false); }
  public static void AddIsCreateMonsterLocal(FlatBufferBuilder builder, bool isCreateMonsterLocal) { builder.AddBool(39, isCreateMonsterLocal, false); }
  public static void AddIsGiveEquips(FlatBufferBuilder builder, bool isGiveEquips) { builder.AddBool(40, isGiveEquips, false); }
  public static void AddEquipList(FlatBufferBuilder builder, StringOffset equipListOffset) { builder.AddOffset(41, equipListOffset.Value, 0); }
  public static void AddIsGuide(FlatBufferBuilder builder, bool isGuide) { builder.AddBool(42, isGuide, false); }
  public static void AddDisplayHUD(FlatBufferBuilder builder, bool displayHUD) { builder.AddBool(43, displayHUD, false); }
  public static void AddCloseTeamCondition(FlatBufferBuilder builder, bool CloseTeamCondition) { builder.AddBool(44, CloseTeamCondition, false); }
  public static void AddIsLocalDungeon(FlatBufferBuilder builder, bool isLocalDungeon) { builder.AddBool(45, isLocalDungeon, false); }
  public static void AddLocalDungeonID(FlatBufferBuilder builder, int localDungeonID) { builder.AddInt(46, localDungeonID, 0); }
  public static void AddRecordResFile(FlatBufferBuilder builder, bool recordResFile) { builder.AddBool(47, recordResFile, false); }
  public static void AddProfileAssetLoad(FlatBufferBuilder builder, bool profileAssetLoad) { builder.AddBool(48, profileAssetLoad, false); }
  public static void Add_gravity(FlatBufferBuilder builder, float Gravity) { builder.AddFloat(49, Gravity, 0); }
  public static void Add_fallGravityReduceFactor(FlatBufferBuilder builder, float FallGravityReduceFactor) { builder.AddFloat(50, FallGravityReduceFactor, 0); }
  public static void AddSkillHasCooldown(FlatBufferBuilder builder, bool skillHasCooldown) { builder.AddBool(51, skillHasCooldown, false); }
  public static void AddScenePath(FlatBufferBuilder builder, StringOffset scenePathOffset) { builder.AddOffset(52, scenePathOffset.Value, 0); }
  public static void AddIpSelectedIndex(FlatBufferBuilder builder, int ipSelectedIndex) { builder.AddInt(53, ipSelectedIndex, 0); }
  public static void AddISingleCharactorID(FlatBufferBuilder builder, int iSingleCharactorID) { builder.AddInt(54, iSingleCharactorID, 0); }
  public static void AddCameraInRange(FlatBufferBuilder builder, Offset<Vector2> cameraInRangeOffset) { builder.AddStruct(55, cameraInRangeOffset.Value, 0); }
  public static void AddButtonType(FlatBufferBuilder builder, int buttonType) { builder.AddInt(56, buttonType, 0); }
  public static void Add_defaultFloatHurt(FlatBufferBuilder builder, float DefaultFloatHurt) { builder.AddFloat(57, DefaultFloatHurt, 0); }
  public static void Add_defaultFloatLevelHurat(FlatBufferBuilder builder, float DefaultFloatLevelHurat) { builder.AddFloat(58, DefaultFloatLevelHurat, 0); }
  public static void Add_defaultGroundHurt(FlatBufferBuilder builder, float DefaultGroundHurt) { builder.AddFloat(59, DefaultGroundHurt, 0); }
  public static void Add_defaultStandHurt(FlatBufferBuilder builder, float DefaultStandHurt) { builder.AddFloat(60, DefaultStandHurt, 0); }
  public static void Add_fallProtectGravityAddFactor(FlatBufferBuilder builder, float FallProtectGravityAddFactor) { builder.AddFloat(61, FallProtectGravityAddFactor, 0); }
  public static void Add_protectClearDuration(FlatBufferBuilder builder, int ProtectClearDuration) { builder.AddInt(62, ProtectClearDuration, 0); }
  public static void AddBgmStart(FlatBufferBuilder builder, float bgmStart) { builder.AddFloat(63, bgmStart, 0); }
  public static void AddBgmTown(FlatBufferBuilder builder, float bgmTown) { builder.AddFloat(64, bgmTown, 0); }
  public static void AddBgmBattle(FlatBufferBuilder builder, float bgmBattle) { builder.AddFloat(65, bgmBattle, 0); }
  public static void Add_zDimFactor(FlatBufferBuilder builder, float ZDimFactor) { builder.AddFloat(66, ZDimFactor, 0); }
  public static void AddBullteScale(FlatBufferBuilder builder, float bullteScale) { builder.AddFloat(67, bullteScale, 0); }
  public static void AddBullteTime(FlatBufferBuilder builder, int bullteTime) { builder.AddInt(68, bullteTime, 0); }
  public static void AddStartSystem(FlatBufferBuilder builder, int startSystem) { builder.AddInt(69, startSystem, 0); }
  public static void AddLoggerFilter(FlatBufferBuilder builder, VectorOffset loggerFilterOffset) { builder.AddOffset(70, loggerFilterOffset.Value, 0); }
  public static VectorOffset CreateLoggerFilterVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLoggerFilterVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddShowDialog(FlatBufferBuilder builder, bool showDialog) { builder.AddBool(71, showDialog, false); }
  public static void AddAvatarLightDir(FlatBufferBuilder builder, Offset<Vector3> avatarLightDirOffset) { builder.AddStruct(72, avatarLightDirOffset.Value, 0); }
  public static void AddShadowLightDir(FlatBufferBuilder builder, Offset<Vector3> shadowLightDirOffset) { builder.AddStruct(73, shadowLightDirOffset.Value, 0); }
  public static void AddStartVel(FlatBufferBuilder builder, Offset<Vector3> startVelOffset) { builder.AddStruct(74, startVelOffset.Value, 0); }
  public static void AddEndVel(FlatBufferBuilder builder, Offset<Vector3> endVelOffset) { builder.AddStruct(75, endVelOffset.Value, 0); }
  public static void AddOffset(FlatBufferBuilder builder, Offset<Vector3> offsetOffset) { builder.AddStruct(76, offsetOffset.Value, 0); }
  public static void AddTimeAccerlate(FlatBufferBuilder builder, float TimeAccerlate) { builder.AddFloat(77, TimeAccerlate, 0); }
  public static void AddTotalTime(FlatBufferBuilder builder, int TotalTime) { builder.AddInt(78, TotalTime, 0); }
  public static void AddTotalDist(FlatBufferBuilder builder, int TotalDist) { builder.AddInt(79, TotalDist, 0); }
  public static void AddHeightAdoption(FlatBufferBuilder builder, bool heightAdoption) { builder.AddBool(80, heightAdoption, false); }
  public static void AddDebugDrawBlock(FlatBufferBuilder builder, bool debugDrawBlock) { builder.AddBool(81, debugDrawBlock, false); }
  public static void AddLoadFromPackage(FlatBufferBuilder builder, bool loadFromPackage) { builder.AddBool(82, loadFromPackage, false); }
  public static void AddEnableHotFix(FlatBufferBuilder builder, bool enableHotFix) { builder.AddBool(83, enableHotFix, false); }
  public static void AddHotFixUrlDebug(FlatBufferBuilder builder, bool hotFixUrlDebug) { builder.AddBool(84, hotFixUrlDebug, false); }
  public static void AddREVIVESHOCKSKILLID(FlatBufferBuilder builder, int REVIVESHOCKSKILLID) { builder.AddInt(85, REVIVESHOCKSKILLID, 0); }
  public static void AddRollSpeed(FlatBufferBuilder builder, Offset<Vector2> rollSpeedOffset) { builder.AddStruct(86, rollSpeedOffset.Value, 0); }
  public static void AddRollRand(FlatBufferBuilder builder, float rollRand) { builder.AddFloat(87, rollRand, 0); }
  public static void AddNormalRollRand(FlatBufferBuilder builder, float normalRollRand) { builder.AddFloat(88, normalRollRand, 0); }
  public static void Add_pkDamageAdjustFactor(FlatBufferBuilder builder, float PkDamageAdjustFactor) { builder.AddFloat(89, PkDamageAdjustFactor, 0); }
  public static void Add_pkHPAdjustFactor(FlatBufferBuilder builder, float PkHPAdjustFactor) { builder.AddFloat(90, PkHPAdjustFactor, 0); }
  public static void AddPkUseMaxLevel(FlatBufferBuilder builder, bool pkUseMaxLevel) { builder.AddBool(91, pkUseMaxLevel, false); }
  public static void AddBattleRunMode(FlatBufferBuilder builder, int battleRunMode) { builder.AddInt(92, battleRunMode, 0); }
  public static void AddHasDoubleRun(FlatBufferBuilder builder, bool hasDoubleRun) { builder.AddBool(93, hasDoubleRun, false); }
  public static void AddPlayerHP(FlatBufferBuilder builder, int playerHP) { builder.AddInt(94, playerHP, 0); }
  public static void AddPlayerRebornHP(FlatBufferBuilder builder, int playerRebornHP) { builder.AddInt(95, playerRebornHP, 0); }
  public static void AddMonsterHP(FlatBufferBuilder builder, int monsterHP) { builder.AddInt(96, monsterHP, 0); }
  public static void AddPlayerPos(FlatBufferBuilder builder, Offset<Vector3> playerPosOffset) { builder.AddStruct(97, playerPosOffset.Value, 0); }
  public static void AddTransportDoorRadius(FlatBufferBuilder builder, float transportDoorRadius) { builder.AddFloat(98, transportDoorRadius, 0); }
  public static void AddPetXMovingDis(FlatBufferBuilder builder, float petXMovingDis) { builder.AddFloat(99, petXMovingDis, 0); }
  public static void AddPetXMovingv1(FlatBufferBuilder builder, float petXMovingv1) { builder.AddFloat(100, petXMovingv1, 0); }
  public static void AddPetXMovingv2(FlatBufferBuilder builder, float petXMovingv2) { builder.AddFloat(101, petXMovingv2, 0); }
  public static void AddPetYMovingDis(FlatBufferBuilder builder, float petYMovingDis) { builder.AddFloat(102, petYMovingDis, 0); }
  public static void AddPetYMovingv1(FlatBufferBuilder builder, float petYMovingv1) { builder.AddFloat(103, petYMovingv1, 0); }
  public static void AddPetYMovingv2(FlatBufferBuilder builder, float petYMovingv2) { builder.AddFloat(104, petYMovingv2, 0); }
  public static void AddServerListUrl(FlatBufferBuilder builder, StringOffset serverListUrlOffset) { builder.AddOffset(105, serverListUrlOffset.Value, 0); }
  public static void AddServerList(FlatBufferBuilder builder, VectorOffset serverListOffset) { builder.AddOffset(106, serverListOffset.Value, 0); }
  public static VectorOffset CreateServerListVector(FlatBufferBuilder builder, Offset<Address>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartServerListVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDebugNewAutofightAI(FlatBufferBuilder builder, bool debugNewAutofightAI) { builder.AddBool(107, debugNewAutofightAI, false); }
  public static void AddCharScale(FlatBufferBuilder builder, float charScale) { builder.AddFloat(108, charScale, 0); }
  public static void AddMonsterBeHitShockData(FlatBufferBuilder builder, Offset<ShockData> monsterBeHitShockDataOffset) { builder.AddStruct(109, monsterBeHitShockDataOffset.Value, 0); }
  public static void AddPlayerBeHitShockData(FlatBufferBuilder builder, Offset<ShockData> playerBeHitShockDataOffset) { builder.AddStruct(110, playerBeHitShockDataOffset.Value, 0); }
  public static void AddPlayerSkillCDShockData(FlatBufferBuilder builder, Offset<ShockData> playerSkillCDShockDataOffset) { builder.AddStruct(111, playerSkillCDShockDataOffset.Value, 0); }
  public static void AddPlayerHighFallTouchGroundShockData(FlatBufferBuilder builder, Offset<ShockData> playerHighFallTouchGroundShockDataOffset) { builder.AddStruct(112, playerHighFallTouchGroundShockDataOffset.Value, 0); }
  public static void AddHighFallHight(FlatBufferBuilder builder, float highFallHight) { builder.AddFloat(113, highFallHight, 0); }
  public static void AddCritialDeadEffect(FlatBufferBuilder builder, StringOffset critialDeadEffectOffset) { builder.AddOffset(114, critialDeadEffectOffset.Value, 0); }
  public static void AddImmediateDeadEffect(FlatBufferBuilder builder, StringOffset immediateDeadEffectOffset) { builder.AddOffset(115, immediateDeadEffectOffset.Value, 0); }
  public static void AddNormalDeadEffect(FlatBufferBuilder builder, StringOffset normalDeadEffectOffset) { builder.AddOffset(116, normalDeadEffectOffset.Value, 0); }
  public static void AddEnableEffectLimit(FlatBufferBuilder builder, bool enableEffectLimit) { builder.AddBool(117, enableEffectLimit, false); }
  public static void AddEffectLimitCount(FlatBufferBuilder builder, int effectLimitCount) { builder.AddInt(118, effectLimitCount, 0); }
  public static void AddImmediateDeadHPPercent(FlatBufferBuilder builder, int immediateDeadHPPercent) { builder.AddInt(119, immediateDeadHPPercent, 0); }
  public static void AddOpenBossShow(FlatBufferBuilder builder, bool openBossShow) { builder.AddBool(120, openBossShow, false); }
  public static void AddShooterFitPercent(FlatBufferBuilder builder, float shooterFitPercent) { builder.AddFloat(121, shooterFitPercent, 0); }
  public static void AddDisappearDis(FlatBufferBuilder builder, Offset<Vector3> disappearDisOffset) { builder.AddStruct(122, disappearDisOffset.Value, 0); }
  public static void AddKeepDis(FlatBufferBuilder builder, float keepDis) { builder.AddFloat(123, keepDis, 0); }
  public static void AddAccompanyRunTime(FlatBufferBuilder builder, float accompanyRunTime) { builder.AddFloat(124, accompanyRunTime, 0); }
  public static void Add_aiWanderRange(FlatBufferBuilder builder, int AiWanderRange) { builder.AddInt(125, AiWanderRange, 0); }
  public static void Add_aiWAlkBackRange(FlatBufferBuilder builder, int AiWAlkBackRange) { builder.AddInt(126, AiWAlkBackRange, 0); }
  public static void Add_aiMaxWalkCmdCount(FlatBufferBuilder builder, int AiMaxWalkCmdCount) { builder.AddInt(127, AiMaxWalkCmdCount, 0); }
  public static void Add_aiMaxWalkCmdCountRANGED(FlatBufferBuilder builder, int AiMaxWalkCmdCountRANGED) { builder.AddInt(128, AiMaxWalkCmdCountRANGED, 0); }
  public static void Add_aiMaxIdleCmdcount(FlatBufferBuilder builder, int AiMaxIdleCmdcount) { builder.AddInt(129, AiMaxIdleCmdcount, 0); }
  public static void Add_aiSkillAttackPassive(FlatBufferBuilder builder, int AiSkillAttackPassive) { builder.AddInt(130, AiSkillAttackPassive, 0); }
  public static void Add_monsterGetupBatiFactor(FlatBufferBuilder builder, float MonsterGetupBatiFactor) { builder.AddFloat(131, MonsterGetupBatiFactor, 0); }
  public static void Add_degangBackDistance(FlatBufferBuilder builder, float DegangBackDistance) { builder.AddFloat(132, DegangBackDistance, 0); }
  public static void Add_afThinkTerm(FlatBufferBuilder builder, int AfThinkTerm) { builder.AddInt(133, AfThinkTerm, 0); }
  public static void Add_afFindTargetTerm(FlatBufferBuilder builder, int AfFindTargetTerm) { builder.AddInt(134, AfFindTargetTerm, 0); }
  public static void Add_afChangeDestinationTerm(FlatBufferBuilder builder, int AfChangeDestinationTerm) { builder.AddInt(135, AfChangeDestinationTerm, 0); }
  public static void Add_autoCheckRestoreInterval(FlatBufferBuilder builder, int AutoCheckRestoreInterval) { builder.AddInt(136, AutoCheckRestoreInterval, 0); }
  public static void AddForceUseAutoFight(FlatBufferBuilder builder, bool forceUseAutoFight) { builder.AddBool(137, forceUseAutoFight, false); }
  public static void AddCanUseAutoFight(FlatBufferBuilder builder, bool canUseAutoFight) { builder.AddBool(138, canUseAutoFight, false); }
  public static void AddCanUseAutoFightFirstPass(FlatBufferBuilder builder, bool canUseAutoFightFirstPass) { builder.AddBool(139, canUseAutoFightFirstPass, false); }
  public static void AddLoadAutoFight(FlatBufferBuilder builder, bool loadAutoFight) { builder.AddBool(140, loadAutoFight, false); }
  public static void AddJumpAttackLimitHeight(FlatBufferBuilder builder, float jumpAttackLimitHeight) { builder.AddFloat(141, jumpAttackLimitHeight, 0); }
  public static void AddSkillCancelLimitTime(FlatBufferBuilder builder, float skillCancelLimitTime) { builder.AddFloat(142, skillCancelLimitTime, 0); }
  public static void AddDoublePressCheckDuration(FlatBufferBuilder builder, int doublePressCheckDuration) { builder.AddInt(143, doublePressCheckDuration, 0); }
  public static void AddWalkAction(FlatBufferBuilder builder, int walkAction) { builder.AddInt(144, walkAction, 0); }
  public static void AddRunAction(FlatBufferBuilder builder, int runAction) { builder.AddInt(145, runAction, 0); }
  public static void AddWalkAniFactor(FlatBufferBuilder builder, float walkAniFactor) { builder.AddFloat(146, walkAniFactor, 0); }
  public static void AddRunAniFactor(FlatBufferBuilder builder, float runAniFactor) { builder.AddFloat(147, runAniFactor, 0); }
  public static void AddChangeFaceStop(FlatBufferBuilder builder, bool changeFaceStop) { builder.AddBool(148, changeFaceStop, false); }
  public static void Add_monsterWalkSpeed(FlatBufferBuilder builder, Offset<Vector3> MonsterWalkSpeedOffset) { builder.AddStruct(149, MonsterWalkSpeedOffset.Value, 0); }
  public static void Add_monsterRunSpeed(FlatBufferBuilder builder, Offset<Vector3> MonsterRunSpeedOffset) { builder.AddStruct(150, MonsterRunSpeedOffset.Value, 0); }
  public static void AddTableLoadStep(FlatBufferBuilder builder, int tableLoadStep) { builder.AddInt(151, tableLoadStep, 0); }
  public static void AddLoadingProgressStepInEditor(FlatBufferBuilder builder, int loadingProgressStepInEditor) { builder.AddInt(152, loadingProgressStepInEditor, 0); }
  public static void AddLoadingProgressStep(FlatBufferBuilder builder, int loadingProgressStep) { builder.AddInt(153, loadingProgressStep, 0); }
  public static void AddPvpDefaultSesstionID(FlatBufferBuilder builder, StringOffset pvpDefaultSesstionIDOffset) { builder.AddOffset(154, pvpDefaultSesstionIDOffset.Value, 0); }
  public static void AddPetID(FlatBufferBuilder builder, int petID) { builder.AddInt(155, petID, 0); }
  public static void AddPetLevel(FlatBufferBuilder builder, int petLevel) { builder.AddInt(156, petLevel, 0); }
  public static void AddPetHunger(FlatBufferBuilder builder, int petHunger) { builder.AddInt(157, petHunger, 0); }
  public static void AddPetSkillIndex(FlatBufferBuilder builder, int petSkillIndex) { builder.AddInt(158, petSkillIndex, 0); }
  public static void AddTestFashionEquip(FlatBufferBuilder builder, bool testFashionEquip) { builder.AddBool(159, testFashionEquip, false); }
  public static void AddEquipPropFactorsKey(FlatBufferBuilder builder, VectorOffset equipPropFactorsKeyOffset) { builder.AddOffset(160, equipPropFactorsKeyOffset.Value, 0); }
  public static VectorOffset CreateEquipPropFactorsKeyVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartEquipPropFactorsKeyVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEquipPropFactorsValue(FlatBufferBuilder builder, VectorOffset equipPropFactorsValueOffset) { builder.AddOffset(161, equipPropFactorsValueOffset.Value, 0); }
  public static VectorOffset CreateEquipPropFactorsValueVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartEquipPropFactorsValueVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEquipPropFactorValues(FlatBufferBuilder builder, VectorOffset equipPropFactorValuesOffset) { builder.AddOffset(162, equipPropFactorValuesOffset.Value, 0); }
  public static VectorOffset CreateEquipPropFactorValuesVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartEquipPropFactorValuesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddQuipThirdTypeFactorsKey(FlatBufferBuilder builder, VectorOffset quipThirdTypeFactorsKeyOffset) { builder.AddOffset(163, quipThirdTypeFactorsKeyOffset.Value, 0); }
  public static VectorOffset CreateQuipThirdTypeFactorsKeyVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartQuipThirdTypeFactorsKeyVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddQuipThirdTypeFactorsValue(FlatBufferBuilder builder, VectorOffset quipThirdTypeFactorsValueOffset) { builder.AddOffset(164, quipThirdTypeFactorsValueOffset.Value, 0); }
  public static VectorOffset CreateQuipThirdTypeFactorsValueVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartQuipThirdTypeFactorsValueVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddQuipThirdTypeFactorValues(FlatBufferBuilder builder, VectorOffset quipThirdTypeFactorValuesOffset) { builder.AddOffset(165, quipThirdTypeFactorValuesOffset.Value, 0); }
  public static VectorOffset CreateQuipThirdTypeFactorValuesVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartQuipThirdTypeFactorValuesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddQualityAdjust(FlatBufferBuilder builder, Offset<QualityAdjust> qualityAdjustOffset) { builder.AddStruct(166, qualityAdjustOffset.Value, 0); }
  public static void AddPetDialogLife(FlatBufferBuilder builder, float petDialogLife) { builder.AddFloat(167, petDialogLife, 0); }
  public static void AddPetDialogShowInterval(FlatBufferBuilder builder, float petDialogShowInterval) { builder.AddFloat(168, petDialogShowInterval, 0); }
  public static void AddPetSpecialIdleInterval(FlatBufferBuilder builder, float petSpecialIdleInterval) { builder.AddFloat(169, petSpecialIdleInterval, 0); }
  public static void AddNotifyItemTimeLess(FlatBufferBuilder builder, int notifyItemTimeLess) { builder.AddInt(170, notifyItemTimeLess, 0); }
  public static void AddUseNewHurtAction(FlatBufferBuilder builder, bool useNewHurtAction) { builder.AddBool(171, useNewHurtAction, false); }
  public static void AddUseNewGravity(FlatBufferBuilder builder, bool useNewGravity) { builder.AddBool(172, useNewGravity, false); }
  public static VectorOffset AddSpeedAnchorArray(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void AddSpeedAnchorArrayVector(FlatBufferBuilder builder, VectorOffset speedAnchorArray) { builder.AddOffset(173, speedAnchorArray.Value, 0); }
  public static VectorOffset AddGravityRateArray(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void AddGravityRateArrayVector(FlatBufferBuilder builder, VectorOffset gravityRateArray) { builder.AddOffset(174, gravityRateArray.Value, 0); }
  public static Offset<GlobalSetting> EndGlobalSetting(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GlobalSetting>(o);
  }
};


}
