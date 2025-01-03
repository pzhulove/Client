// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class EquipMasterTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = 1673379914,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static EquipMasterTable GetRootAsEquipMasterTable(ByteBuffer _bb) { return GetRootAsEquipMasterTable(_bb, new EquipMasterTable()); }
  public static EquipMasterTable GetRootAsEquipMasterTable(ByteBuffer _bb, EquipMasterTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public EquipMasterTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int JobID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Quality { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Part { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int MaterialType { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int AtkArray(int j) { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int AtkLength { get { int o = __p.__offset(14); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetAtkBytes() { return __p.__vector_as_arraysegment(14); }
 private FlatBufferArray<int> AtkValue;
 public FlatBufferArray<int>  Atk
 {
  get{
  if (AtkValue == null)
  {
    AtkValue = new FlatBufferArray<int>(this.AtkArray, this.AtkLength);
  }
  return AtkValue;}
 }
  public int MagicAtkArray(int j) { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int MagicAtkLength { get { int o = __p.__offset(16); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetMagicAtkBytes() { return __p.__vector_as_arraysegment(16); }
 private FlatBufferArray<int> MagicAtkValue;
 public FlatBufferArray<int>  MagicAtk
 {
  get{
  if (MagicAtkValue == null)
  {
    MagicAtkValue = new FlatBufferArray<int>(this.MagicAtkArray, this.MagicAtkLength);
  }
  return MagicAtkValue;}
 }
  public int DefArray(int j) { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int DefLength { get { int o = __p.__offset(18); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetDefBytes() { return __p.__vector_as_arraysegment(18); }
 private FlatBufferArray<int> DefValue;
 public FlatBufferArray<int>  Def
 {
  get{
  if (DefValue == null)
  {
    DefValue = new FlatBufferArray<int>(this.DefArray, this.DefLength);
  }
  return DefValue;}
 }
  public int MagicDefArray(int j) { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int MagicDefLength { get { int o = __p.__offset(20); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetMagicDefBytes() { return __p.__vector_as_arraysegment(20); }
 private FlatBufferArray<int> MagicDefValue;
 public FlatBufferArray<int>  MagicDef
 {
  get{
  if (MagicDefValue == null)
  {
    MagicDefValue = new FlatBufferArray<int>(this.MagicDefArray, this.MagicDefLength);
  }
  return MagicDefValue;}
 }
  public int StrenthArray(int j) { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int StrenthLength { get { int o = __p.__offset(22); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetStrenthBytes() { return __p.__vector_as_arraysegment(22); }
 private FlatBufferArray<int> StrenthValue;
 public FlatBufferArray<int>  Strenth
 {
  get{
  if (StrenthValue == null)
  {
    StrenthValue = new FlatBufferArray<int>(this.StrenthArray, this.StrenthLength);
  }
  return StrenthValue;}
 }
  public int IntellectArray(int j) { int o = __p.__offset(24); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int IntellectLength { get { int o = __p.__offset(24); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetIntellectBytes() { return __p.__vector_as_arraysegment(24); }
 private FlatBufferArray<int> IntellectValue;
 public FlatBufferArray<int>  Intellect
 {
  get{
  if (IntellectValue == null)
  {
    IntellectValue = new FlatBufferArray<int>(this.IntellectArray, this.IntellectLength);
  }
  return IntellectValue;}
 }
  public int SpiritArray(int j) { int o = __p.__offset(26); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int SpiritLength { get { int o = __p.__offset(26); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetSpiritBytes() { return __p.__vector_as_arraysegment(26); }
 private FlatBufferArray<int> SpiritValue;
 public FlatBufferArray<int>  Spirit
 {
  get{
  if (SpiritValue == null)
  {
    SpiritValue = new FlatBufferArray<int>(this.SpiritArray, this.SpiritLength);
  }
  return SpiritValue;}
 }
  public int StaminaArray(int j) { int o = __p.__offset(28); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int StaminaLength { get { int o = __p.__offset(28); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetStaminaBytes() { return __p.__vector_as_arraysegment(28); }
 private FlatBufferArray<int> StaminaValue;
 public FlatBufferArray<int>  Stamina
 {
  get{
  if (StaminaValue == null)
  {
    StaminaValue = new FlatBufferArray<int>(this.StaminaArray, this.StaminaLength);
  }
  return StaminaValue;}
 }
  public int HPMaxArray(int j) { int o = __p.__offset(30); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int HPMaxLength { get { int o = __p.__offset(30); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetHPMaxBytes() { return __p.__vector_as_arraysegment(30); }
 private FlatBufferArray<int> HPMaxValue;
 public FlatBufferArray<int>  HPMax
 {
  get{
  if (HPMaxValue == null)
  {
    HPMaxValue = new FlatBufferArray<int>(this.HPMaxArray, this.HPMaxLength);
  }
  return HPMaxValue;}
 }
  public int MPMaxArray(int j) { int o = __p.__offset(32); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int MPMaxLength { get { int o = __p.__offset(32); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetMPMaxBytes() { return __p.__vector_as_arraysegment(32); }
 private FlatBufferArray<int> MPMaxValue;
 public FlatBufferArray<int>  MPMax
 {
  get{
  if (MPMaxValue == null)
  {
    MPMaxValue = new FlatBufferArray<int>(this.MPMaxArray, this.MPMaxLength);
  }
  return MPMaxValue;}
 }
  public int HPRecoverArray(int j) { int o = __p.__offset(34); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int HPRecoverLength { get { int o = __p.__offset(34); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetHPRecoverBytes() { return __p.__vector_as_arraysegment(34); }
 private FlatBufferArray<int> HPRecoverValue;
 public FlatBufferArray<int>  HPRecover
 {
  get{
  if (HPRecoverValue == null)
  {
    HPRecoverValue = new FlatBufferArray<int>(this.HPRecoverArray, this.HPRecoverLength);
  }
  return HPRecoverValue;}
 }
  public int MPRecoverArray(int j) { int o = __p.__offset(36); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int MPRecoverLength { get { int o = __p.__offset(36); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetMPRecoverBytes() { return __p.__vector_as_arraysegment(36); }
 private FlatBufferArray<int> MPRecoverValue;
 public FlatBufferArray<int>  MPRecover
 {
  get{
  if (MPRecoverValue == null)
  {
    MPRecoverValue = new FlatBufferArray<int>(this.MPRecoverArray, this.MPRecoverLength);
  }
  return MPRecoverValue;}
 }
  public int AttackSpeedRateArray(int j) { int o = __p.__offset(38); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int AttackSpeedRateLength { get { int o = __p.__offset(38); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetAttackSpeedRateBytes() { return __p.__vector_as_arraysegment(38); }
 private FlatBufferArray<int> AttackSpeedRateValue;
 public FlatBufferArray<int>  AttackSpeedRate
 {
  get{
  if (AttackSpeedRateValue == null)
  {
    AttackSpeedRateValue = new FlatBufferArray<int>(this.AttackSpeedRateArray, this.AttackSpeedRateLength);
  }
  return AttackSpeedRateValue;}
 }
  public int FireSpeedRateArray(int j) { int o = __p.__offset(40); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int FireSpeedRateLength { get { int o = __p.__offset(40); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetFireSpeedRateBytes() { return __p.__vector_as_arraysegment(40); }
 private FlatBufferArray<int> FireSpeedRateValue;
 public FlatBufferArray<int>  FireSpeedRate
 {
  get{
  if (FireSpeedRateValue == null)
  {
    FireSpeedRateValue = new FlatBufferArray<int>(this.FireSpeedRateArray, this.FireSpeedRateLength);
  }
  return FireSpeedRateValue;}
 }
  public int MoveSpeedRateArray(int j) { int o = __p.__offset(42); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int MoveSpeedRateLength { get { int o = __p.__offset(42); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetMoveSpeedRateBytes() { return __p.__vector_as_arraysegment(42); }
 private FlatBufferArray<int> MoveSpeedRateValue;
 public FlatBufferArray<int>  MoveSpeedRate
 {
  get{
  if (MoveSpeedRateValue == null)
  {
    MoveSpeedRateValue = new FlatBufferArray<int>(this.MoveSpeedRateArray, this.MoveSpeedRateLength);
  }
  return MoveSpeedRateValue;}
 }
  public int HitRateArray(int j) { int o = __p.__offset(44); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int HitRateLength { get { int o = __p.__offset(44); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetHitRateBytes() { return __p.__vector_as_arraysegment(44); }
 private FlatBufferArray<int> HitRateValue;
 public FlatBufferArray<int>  HitRate
 {
  get{
  if (HitRateValue == null)
  {
    HitRateValue = new FlatBufferArray<int>(this.HitRateArray, this.HitRateLength);
  }
  return HitRateValue;}
 }
  public int AvoidRateArray(int j) { int o = __p.__offset(46); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int AvoidRateLength { get { int o = __p.__offset(46); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetAvoidRateBytes() { return __p.__vector_as_arraysegment(46); }
 private FlatBufferArray<int> AvoidRateValue;
 public FlatBufferArray<int>  AvoidRate
 {
  get{
  if (AvoidRateValue == null)
  {
    AvoidRateValue = new FlatBufferArray<int>(this.AvoidRateArray, this.AvoidRateLength);
  }
  return AvoidRateValue;}
 }
  public int PhysicCritArray(int j) { int o = __p.__offset(48); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int PhysicCritLength { get { int o = __p.__offset(48); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetPhysicCritBytes() { return __p.__vector_as_arraysegment(48); }
 private FlatBufferArray<int> PhysicCritValue;
 public FlatBufferArray<int>  PhysicCrit
 {
  get{
  if (PhysicCritValue == null)
  {
    PhysicCritValue = new FlatBufferArray<int>(this.PhysicCritArray, this.PhysicCritLength);
  }
  return PhysicCritValue;}
 }
  public int MagicCritArray(int j) { int o = __p.__offset(50); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int MagicCritLength { get { int o = __p.__offset(50); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetMagicCritBytes() { return __p.__vector_as_arraysegment(50); }
 private FlatBufferArray<int> MagicCritValue;
 public FlatBufferArray<int>  MagicCrit
 {
  get{
  if (MagicCritValue == null)
  {
    MagicCritValue = new FlatBufferArray<int>(this.MagicCritArray, this.MagicCritLength);
  }
  return MagicCritValue;}
 }
  public int IsMaster { get { int o = __p.__offset(52); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SpasticityArray(int j) { int o = __p.__offset(54); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int SpasticityLength { get { int o = __p.__offset(54); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetSpasticityBytes() { return __p.__vector_as_arraysegment(54); }
 private FlatBufferArray<int> SpasticityValue;
 public FlatBufferArray<int>  Spasticity
 {
  get{
  if (SpasticityValue == null)
  {
    SpasticityValue = new FlatBufferArray<int>(this.SpasticityArray, this.SpasticityLength);
  }
  return SpasticityValue;}
 }
  public string DescsArray(int j) { int o = __p.__offset(56); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : FlatBufferConstants.DefaultString; }
  public int DescsLength { get { int o = __p.__offset(56); return o != 0 ? __p.__vector_len(o) : 0; } }
 private FlatBufferArray<string> DescsValue;
 public FlatBufferArray<string>  Descs
 {
  get{
  if (DescsValue == null)
  {
    DescsValue = new FlatBufferArray<string>(this.DescsArray, this.DescsLength);
  }
  return DescsValue;}
 }

  public static Offset<EquipMasterTable> CreateEquipMasterTable(FlatBufferBuilder builder,
      int ID = 0,
      int JobID = 0,
      int Quality = 0,
      int Part = 0,
      int MaterialType = 0,
      VectorOffset AtkOffset = default(VectorOffset),
      VectorOffset MagicAtkOffset = default(VectorOffset),
      VectorOffset DefOffset = default(VectorOffset),
      VectorOffset MagicDefOffset = default(VectorOffset),
      VectorOffset StrenthOffset = default(VectorOffset),
      VectorOffset IntellectOffset = default(VectorOffset),
      VectorOffset SpiritOffset = default(VectorOffset),
      VectorOffset StaminaOffset = default(VectorOffset),
      VectorOffset HPMaxOffset = default(VectorOffset),
      VectorOffset MPMaxOffset = default(VectorOffset),
      VectorOffset HPRecoverOffset = default(VectorOffset),
      VectorOffset MPRecoverOffset = default(VectorOffset),
      VectorOffset AttackSpeedRateOffset = default(VectorOffset),
      VectorOffset FireSpeedRateOffset = default(VectorOffset),
      VectorOffset MoveSpeedRateOffset = default(VectorOffset),
      VectorOffset HitRateOffset = default(VectorOffset),
      VectorOffset AvoidRateOffset = default(VectorOffset),
      VectorOffset PhysicCritOffset = default(VectorOffset),
      VectorOffset MagicCritOffset = default(VectorOffset),
      int IsMaster = 0,
      VectorOffset SpasticityOffset = default(VectorOffset),
      VectorOffset DescsOffset = default(VectorOffset)) {
    builder.StartObject(27);
    EquipMasterTable.AddDescs(builder, DescsOffset);
    EquipMasterTable.AddSpasticity(builder, SpasticityOffset);
    EquipMasterTable.AddIsMaster(builder, IsMaster);
    EquipMasterTable.AddMagicCrit(builder, MagicCritOffset);
    EquipMasterTable.AddPhysicCrit(builder, PhysicCritOffset);
    EquipMasterTable.AddAvoidRate(builder, AvoidRateOffset);
    EquipMasterTable.AddHitRate(builder, HitRateOffset);
    EquipMasterTable.AddMoveSpeedRate(builder, MoveSpeedRateOffset);
    EquipMasterTable.AddFireSpeedRate(builder, FireSpeedRateOffset);
    EquipMasterTable.AddAttackSpeedRate(builder, AttackSpeedRateOffset);
    EquipMasterTable.AddMPRecover(builder, MPRecoverOffset);
    EquipMasterTable.AddHPRecover(builder, HPRecoverOffset);
    EquipMasterTable.AddMPMax(builder, MPMaxOffset);
    EquipMasterTable.AddHPMax(builder, HPMaxOffset);
    EquipMasterTable.AddStamina(builder, StaminaOffset);
    EquipMasterTable.AddSpirit(builder, SpiritOffset);
    EquipMasterTable.AddIntellect(builder, IntellectOffset);
    EquipMasterTable.AddStrenth(builder, StrenthOffset);
    EquipMasterTable.AddMagicDef(builder, MagicDefOffset);
    EquipMasterTable.AddDef(builder, DefOffset);
    EquipMasterTable.AddMagicAtk(builder, MagicAtkOffset);
    EquipMasterTable.AddAtk(builder, AtkOffset);
    EquipMasterTable.AddMaterialType(builder, MaterialType);
    EquipMasterTable.AddPart(builder, Part);
    EquipMasterTable.AddQuality(builder, Quality);
    EquipMasterTable.AddJobID(builder, JobID);
    EquipMasterTable.AddID(builder, ID);
    return EquipMasterTable.EndEquipMasterTable(builder);
  }

  public static void StartEquipMasterTable(FlatBufferBuilder builder) { builder.StartObject(27); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddJobID(FlatBufferBuilder builder, int JobID) { builder.AddInt(1, JobID, 0); }
  public static void AddQuality(FlatBufferBuilder builder, int Quality) { builder.AddInt(2, Quality, 0); }
  public static void AddPart(FlatBufferBuilder builder, int Part) { builder.AddInt(3, Part, 0); }
  public static void AddMaterialType(FlatBufferBuilder builder, int MaterialType) { builder.AddInt(4, MaterialType, 0); }
  public static void AddAtk(FlatBufferBuilder builder, VectorOffset AtkOffset) { builder.AddOffset(5, AtkOffset.Value, 0); }
  public static VectorOffset CreateAtkVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartAtkVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMagicAtk(FlatBufferBuilder builder, VectorOffset MagicAtkOffset) { builder.AddOffset(6, MagicAtkOffset.Value, 0); }
  public static VectorOffset CreateMagicAtkVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartMagicAtkVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDef(FlatBufferBuilder builder, VectorOffset DefOffset) { builder.AddOffset(7, DefOffset.Value, 0); }
  public static VectorOffset CreateDefVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartDefVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMagicDef(FlatBufferBuilder builder, VectorOffset MagicDefOffset) { builder.AddOffset(8, MagicDefOffset.Value, 0); }
  public static VectorOffset CreateMagicDefVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartMagicDefVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddStrenth(FlatBufferBuilder builder, VectorOffset StrenthOffset) { builder.AddOffset(9, StrenthOffset.Value, 0); }
  public static VectorOffset CreateStrenthVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartStrenthVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddIntellect(FlatBufferBuilder builder, VectorOffset IntellectOffset) { builder.AddOffset(10, IntellectOffset.Value, 0); }
  public static VectorOffset CreateIntellectVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartIntellectVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSpirit(FlatBufferBuilder builder, VectorOffset SpiritOffset) { builder.AddOffset(11, SpiritOffset.Value, 0); }
  public static VectorOffset CreateSpiritVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartSpiritVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddStamina(FlatBufferBuilder builder, VectorOffset StaminaOffset) { builder.AddOffset(12, StaminaOffset.Value, 0); }
  public static VectorOffset CreateStaminaVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartStaminaVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHPMax(FlatBufferBuilder builder, VectorOffset HPMaxOffset) { builder.AddOffset(13, HPMaxOffset.Value, 0); }
  public static VectorOffset CreateHPMaxVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartHPMaxVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMPMax(FlatBufferBuilder builder, VectorOffset MPMaxOffset) { builder.AddOffset(14, MPMaxOffset.Value, 0); }
  public static VectorOffset CreateMPMaxVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartMPMaxVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHPRecover(FlatBufferBuilder builder, VectorOffset HPRecoverOffset) { builder.AddOffset(15, HPRecoverOffset.Value, 0); }
  public static VectorOffset CreateHPRecoverVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartHPRecoverVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMPRecover(FlatBufferBuilder builder, VectorOffset MPRecoverOffset) { builder.AddOffset(16, MPRecoverOffset.Value, 0); }
  public static VectorOffset CreateMPRecoverVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartMPRecoverVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAttackSpeedRate(FlatBufferBuilder builder, VectorOffset AttackSpeedRateOffset) { builder.AddOffset(17, AttackSpeedRateOffset.Value, 0); }
  public static VectorOffset CreateAttackSpeedRateVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartAttackSpeedRateVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddFireSpeedRate(FlatBufferBuilder builder, VectorOffset FireSpeedRateOffset) { builder.AddOffset(18, FireSpeedRateOffset.Value, 0); }
  public static VectorOffset CreateFireSpeedRateVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartFireSpeedRateVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMoveSpeedRate(FlatBufferBuilder builder, VectorOffset MoveSpeedRateOffset) { builder.AddOffset(19, MoveSpeedRateOffset.Value, 0); }
  public static VectorOffset CreateMoveSpeedRateVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartMoveSpeedRateVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHitRate(FlatBufferBuilder builder, VectorOffset HitRateOffset) { builder.AddOffset(20, HitRateOffset.Value, 0); }
  public static VectorOffset CreateHitRateVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartHitRateVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAvoidRate(FlatBufferBuilder builder, VectorOffset AvoidRateOffset) { builder.AddOffset(21, AvoidRateOffset.Value, 0); }
  public static VectorOffset CreateAvoidRateVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartAvoidRateVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPhysicCrit(FlatBufferBuilder builder, VectorOffset PhysicCritOffset) { builder.AddOffset(22, PhysicCritOffset.Value, 0); }
  public static VectorOffset CreatePhysicCritVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartPhysicCritVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMagicCrit(FlatBufferBuilder builder, VectorOffset MagicCritOffset) { builder.AddOffset(23, MagicCritOffset.Value, 0); }
  public static VectorOffset CreateMagicCritVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartMagicCritVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddIsMaster(FlatBufferBuilder builder, int IsMaster) { builder.AddInt(24, IsMaster, 0); }
  public static void AddSpasticity(FlatBufferBuilder builder, VectorOffset SpasticityOffset) { builder.AddOffset(25, SpasticityOffset.Value, 0); }
  public static VectorOffset CreateSpasticityVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartSpasticityVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDescs(FlatBufferBuilder builder, VectorOffset DescsOffset) { builder.AddOffset(26, DescsOffset.Value, 0); }
  public static VectorOffset CreateDescsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDescsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<EquipMasterTable> EndEquipMasterTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EquipMasterTable>(o);
  }
  public static void FinishEquipMasterTableBuffer(FlatBufferBuilder builder, Offset<EquipMasterTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

