// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class FunctionUnLock : IFlatbufferObject
{
public enum eType : int
{
 Type_None = 0,
 Func = 1,
 Area = 2,
};

public enum eFuncType : int
{
 None = 0,
 Skill = 1,
 Forge = 3,
 Achievement = 4,
 Ranklist = 5,
 Welfare = 6,
 Duel = 7,
 Entourage = 8,
 EntourageLvUp = 9,
 DailyTask = 10,
 Title = 11,
 DeathTower = 14,
 Guild = 15,
 BeatCow = 16,
 Enchant = 17,
 Auction = 18,
 Degenerator = 19,
 MagicMale = 20,
 WashEntourage = 21,
 DeepDungeon = 23,
 AncientDungeon = 24,
 ArmorMastery = 25,
 Team = 30,
 Friend = 31,
 ActivitySevenDays = 32,
 FirstReChargeActivity = 34,
 Shop = 35,
 Jar = 36,
 Mall = 37,
 FashionMerge = 45,
 FashionAttrSel = 54,
 Fashion = 55,
 AutoFight = 56,
 OnLineGift = 57,
 ActivityJar = 58,
 Pet = 59,
 Legend = 60,
 BattleDrugs = 61,
 TAPSystem = 62,
 ActivityLimitTime = 63,
 FestivalActivity = 64,
 Bead = 66,
 AchievementG = 67,
 MagicJarLv55 = 68,
 SideWeapon = 69,
 EquipHandBook = 70,
 YijieAreaOpen = 75,
 RandomTreasure = 76,
 HorseGambling = 77,
 VanityFracture = 80,
 ReportingFunction = 82,
 WeaponLease = 83,
 EquipUpgrade = 84,
 BlackMarket = 85,
 AdventureTeam = 86,
 PVETrain = 87,
 PVEDamage = 88,
 DailyTodo = 89,
 KingTower = 90,
 TeamCopy = 91,
 Inscription = 92,
 AdventurePassSeason = 93,
 Questionnaire = 94,
 Honour = 95,
 Enhance = 96,
 RightDown = 98,
 LeftUp = 99,
 ChangeJob = 100,
 LevelGift = 101,
};

public enum eLocationType : int
{
 MainTown = 0,
 BottomRightExpand = 1,
 TopLeftExpand = 2,
 ComTalk = 3,
};

public enum eExpandType : int
{
 ET_Null = 0,
 ET_TopRight = 1,
};

public enum eBindType : int
{
 BT_RoleBind = 0,
 BT_AccBind = 1,
};

public enum eCrypt : int
{
 code = 1970490982,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static FunctionUnLock GetRootAsFunctionUnLock(ByteBuffer _bb) { return GetRootAsFunctionUnLock(_bb, new FunctionUnLock()); }
  public static FunctionUnLock GetRootAsFunctionUnLock(ByteBuffer _bb, FunctionUnLock obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public FunctionUnLock __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public ProtoTable.FunctionUnLock.eType Type { get { int o = __p.__offset(8); return o != 0 ? (ProtoTable.FunctionUnLock.eType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.FunctionUnLock.eType.Type_None; } }
  public ProtoTable.FunctionUnLock.eFuncType FuncType { get { int o = __p.__offset(10); return o != 0 ? (ProtoTable.FunctionUnLock.eFuncType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.FunctionUnLock.eFuncType.None; } }
  public int CommDescID { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int StartLevel { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int FinishLevel { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public bool IsOpen { get { int o = __p.__offset(18); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public int StartTaskID { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int FinishTaskID { get { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int PosPriority { get { int o = __p.__offset(24); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public ProtoTable.FunctionUnLock.eLocationType LocationType { get { int o = __p.__offset(26); return o != 0 ? (ProtoTable.FunctionUnLock.eLocationType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.FunctionUnLock.eLocationType.MainTown; } }
  public string TargetBtnPos { get { int o = __p.__offset(28); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetTargetBtnPosBytes() { return __p.__vector_as_arraysegment(28); }
  public int bPlayAnim { get { int o = __p.__offset(30); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int bShowBtn { get { int o = __p.__offset(32); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string IconPath { get { int o = __p.__offset(34); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetIconPathBytes() { return __p.__vector_as_arraysegment(34); }
  public int AreaID { get { int o = __p.__offset(36); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Award { get { int o = __p.__offset(38); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetAwardBytes() { return __p.__vector_as_arraysegment(38); }
  public string Explanation { get { int o = __p.__offset(40); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetExplanationBytes() { return __p.__vector_as_arraysegment(40); }
  public ProtoTable.FunctionUnLock.eExpandType ExpandType { get { int o = __p.__offset(42); return o != 0 ? (ProtoTable.FunctionUnLock.eExpandType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.FunctionUnLock.eExpandType.ET_Null; } }
  public ProtoTable.FunctionUnLock.eBindType BindType { get { int o = __p.__offset(44); return o != 0 ? (ProtoTable.FunctionUnLock.eBindType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.FunctionUnLock.eBindType.BT_RoleBind; } }
  public int ShowFunctionOpen { get { int o = __p.__offset(46); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int OpenArea { get { int o = __p.__offset(48); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ShowOpenEffect { get { int o = __p.__offset(50); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ShowNextOpen { get { int o = __p.__offset(52); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<FunctionUnLock> CreateFunctionUnLock(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      ProtoTable.FunctionUnLock.eType Type = ProtoTable.FunctionUnLock.eType.Type_None,
      ProtoTable.FunctionUnLock.eFuncType FuncType = ProtoTable.FunctionUnLock.eFuncType.None,
      int CommDescID = 0,
      int StartLevel = 0,
      int FinishLevel = 0,
      bool IsOpen = false,
      int StartTaskID = 0,
      int FinishTaskID = 0,
      int PosPriority = 0,
      ProtoTable.FunctionUnLock.eLocationType LocationType = ProtoTable.FunctionUnLock.eLocationType.MainTown,
      StringOffset TargetBtnPosOffset = default(StringOffset),
      int bPlayAnim = 0,
      int bShowBtn = 0,
      StringOffset IconPathOffset = default(StringOffset),
      int AreaID = 0,
      StringOffset AwardOffset = default(StringOffset),
      StringOffset ExplanationOffset = default(StringOffset),
      ProtoTable.FunctionUnLock.eExpandType ExpandType = ProtoTable.FunctionUnLock.eExpandType.ET_Null,
      ProtoTable.FunctionUnLock.eBindType BindType = ProtoTable.FunctionUnLock.eBindType.BT_RoleBind,
      int ShowFunctionOpen = 0,
      int OpenArea = 0,
      int ShowOpenEffect = 0,
      int ShowNextOpen = 0) {
    builder.StartObject(25);
    FunctionUnLock.AddShowNextOpen(builder, ShowNextOpen);
    FunctionUnLock.AddShowOpenEffect(builder, ShowOpenEffect);
    FunctionUnLock.AddOpenArea(builder, OpenArea);
    FunctionUnLock.AddShowFunctionOpen(builder, ShowFunctionOpen);
    FunctionUnLock.AddBindType(builder, BindType);
    FunctionUnLock.AddExpandType(builder, ExpandType);
    FunctionUnLock.AddExplanation(builder, ExplanationOffset);
    FunctionUnLock.AddAward(builder, AwardOffset);
    FunctionUnLock.AddAreaID(builder, AreaID);
    FunctionUnLock.AddIconPath(builder, IconPathOffset);
    FunctionUnLock.AddBShowBtn(builder, bShowBtn);
    FunctionUnLock.AddBPlayAnim(builder, bPlayAnim);
    FunctionUnLock.AddTargetBtnPos(builder, TargetBtnPosOffset);
    FunctionUnLock.AddLocationType(builder, LocationType);
    FunctionUnLock.AddPosPriority(builder, PosPriority);
    FunctionUnLock.AddFinishTaskID(builder, FinishTaskID);
    FunctionUnLock.AddStartTaskID(builder, StartTaskID);
    FunctionUnLock.AddFinishLevel(builder, FinishLevel);
    FunctionUnLock.AddStartLevel(builder, StartLevel);
    FunctionUnLock.AddCommDescID(builder, CommDescID);
    FunctionUnLock.AddFuncType(builder, FuncType);
    FunctionUnLock.AddType(builder, Type);
    FunctionUnLock.AddName(builder, NameOffset);
    FunctionUnLock.AddID(builder, ID);
    FunctionUnLock.AddIsOpen(builder, IsOpen);
    return FunctionUnLock.EndFunctionUnLock(builder);
  }

  public static void StartFunctionUnLock(FlatBufferBuilder builder) { builder.StartObject(25); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, ProtoTable.FunctionUnLock.eType Type) { builder.AddInt(2, (int)Type, 0); }
  public static void AddFuncType(FlatBufferBuilder builder, ProtoTable.FunctionUnLock.eFuncType FuncType) { builder.AddInt(3, (int)FuncType, 0); }
  public static void AddCommDescID(FlatBufferBuilder builder, int CommDescID) { builder.AddInt(4, CommDescID, 0); }
  public static void AddStartLevel(FlatBufferBuilder builder, int StartLevel) { builder.AddInt(5, StartLevel, 0); }
  public static void AddFinishLevel(FlatBufferBuilder builder, int FinishLevel) { builder.AddInt(6, FinishLevel, 0); }
  public static void AddIsOpen(FlatBufferBuilder builder, bool IsOpen) { builder.AddBool(7, IsOpen, false); }
  public static void AddStartTaskID(FlatBufferBuilder builder, int StartTaskID) { builder.AddInt(8, StartTaskID, 0); }
  public static void AddFinishTaskID(FlatBufferBuilder builder, int FinishTaskID) { builder.AddInt(9, FinishTaskID, 0); }
  public static void AddPosPriority(FlatBufferBuilder builder, int PosPriority) { builder.AddInt(10, PosPriority, 0); }
  public static void AddLocationType(FlatBufferBuilder builder, ProtoTable.FunctionUnLock.eLocationType LocationType) { builder.AddInt(11, (int)LocationType, 0); }
  public static void AddTargetBtnPos(FlatBufferBuilder builder, StringOffset TargetBtnPosOffset) { builder.AddOffset(12, TargetBtnPosOffset.Value, 0); }
  public static void AddBPlayAnim(FlatBufferBuilder builder, int bPlayAnim) { builder.AddInt(13, bPlayAnim, 0); }
  public static void AddBShowBtn(FlatBufferBuilder builder, int bShowBtn) { builder.AddInt(14, bShowBtn, 0); }
  public static void AddIconPath(FlatBufferBuilder builder, StringOffset IconPathOffset) { builder.AddOffset(15, IconPathOffset.Value, 0); }
  public static void AddAreaID(FlatBufferBuilder builder, int AreaID) { builder.AddInt(16, AreaID, 0); }
  public static void AddAward(FlatBufferBuilder builder, StringOffset AwardOffset) { builder.AddOffset(17, AwardOffset.Value, 0); }
  public static void AddExplanation(FlatBufferBuilder builder, StringOffset ExplanationOffset) { builder.AddOffset(18, ExplanationOffset.Value, 0); }
  public static void AddExpandType(FlatBufferBuilder builder, ProtoTable.FunctionUnLock.eExpandType ExpandType) { builder.AddInt(19, (int)ExpandType, 0); }
  public static void AddBindType(FlatBufferBuilder builder, ProtoTable.FunctionUnLock.eBindType BindType) { builder.AddInt(20, (int)BindType, 0); }
  public static void AddShowFunctionOpen(FlatBufferBuilder builder, int ShowFunctionOpen) { builder.AddInt(21, ShowFunctionOpen, 0); }
  public static void AddOpenArea(FlatBufferBuilder builder, int OpenArea) { builder.AddInt(22, OpenArea, 0); }
  public static void AddShowOpenEffect(FlatBufferBuilder builder, int ShowOpenEffect) { builder.AddInt(23, ShowOpenEffect, 0); }
  public static void AddShowNextOpen(FlatBufferBuilder builder, int ShowNextOpen) { builder.AddInt(24, ShowNextOpen, 0); }
  public static Offset<FunctionUnLock> EndFunctionUnLock(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FunctionUnLock>(o);
  }
  public static void FinishFunctionUnLockBuffer(FlatBufferBuilder builder, Offset<FunctionUnLock> offset) { builder.Finish(offset.Value); }
};


}


#endif
