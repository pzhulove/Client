// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class RewardAdapterTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1699960114,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static RewardAdapterTable GetRootAsRewardAdapterTable(ByteBuffer _bb) { return GetRootAsRewardAdapterTable(_bb, new RewardAdapterTable()); }
  public static RewardAdapterTable GetRootAsRewardAdapterTable(ByteBuffer _bb, RewardAdapterTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public RewardAdapterTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public string Level1 { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel1Bytes() { return __p.__vector_as_arraysegment(8); }
  public string Level2 { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel2Bytes() { return __p.__vector_as_arraysegment(10); }
  public string Level3 { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel3Bytes() { return __p.__vector_as_arraysegment(12); }
  public string Level4 { get { int o = __p.__offset(14); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel4Bytes() { return __p.__vector_as_arraysegment(14); }
  public string Level5 { get { int o = __p.__offset(16); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel5Bytes() { return __p.__vector_as_arraysegment(16); }
  public string Level6 { get { int o = __p.__offset(18); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel6Bytes() { return __p.__vector_as_arraysegment(18); }
  public string Level7 { get { int o = __p.__offset(20); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel7Bytes() { return __p.__vector_as_arraysegment(20); }
  public string Level8 { get { int o = __p.__offset(22); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel8Bytes() { return __p.__vector_as_arraysegment(22); }
  public string Level9 { get { int o = __p.__offset(24); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel9Bytes() { return __p.__vector_as_arraysegment(24); }
  public string Level10 { get { int o = __p.__offset(26); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel10Bytes() { return __p.__vector_as_arraysegment(26); }
  public string Level11 { get { int o = __p.__offset(28); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel11Bytes() { return __p.__vector_as_arraysegment(28); }
  public string Level12 { get { int o = __p.__offset(30); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel12Bytes() { return __p.__vector_as_arraysegment(30); }
  public string Level13 { get { int o = __p.__offset(32); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel13Bytes() { return __p.__vector_as_arraysegment(32); }
  public string Level14 { get { int o = __p.__offset(34); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel14Bytes() { return __p.__vector_as_arraysegment(34); }
  public string Level15 { get { int o = __p.__offset(36); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel15Bytes() { return __p.__vector_as_arraysegment(36); }
  public string Level16 { get { int o = __p.__offset(38); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel16Bytes() { return __p.__vector_as_arraysegment(38); }
  public string Level17 { get { int o = __p.__offset(40); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel17Bytes() { return __p.__vector_as_arraysegment(40); }
  public string Level18 { get { int o = __p.__offset(42); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel18Bytes() { return __p.__vector_as_arraysegment(42); }
  public string Level19 { get { int o = __p.__offset(44); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel19Bytes() { return __p.__vector_as_arraysegment(44); }
  public string Level20 { get { int o = __p.__offset(46); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel20Bytes() { return __p.__vector_as_arraysegment(46); }
  public string Level21 { get { int o = __p.__offset(48); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel21Bytes() { return __p.__vector_as_arraysegment(48); }
  public string Level22 { get { int o = __p.__offset(50); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel22Bytes() { return __p.__vector_as_arraysegment(50); }
  public string Level23 { get { int o = __p.__offset(52); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel23Bytes() { return __p.__vector_as_arraysegment(52); }
  public string Level24 { get { int o = __p.__offset(54); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel24Bytes() { return __p.__vector_as_arraysegment(54); }
  public string Level25 { get { int o = __p.__offset(56); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel25Bytes() { return __p.__vector_as_arraysegment(56); }
  public string Level26 { get { int o = __p.__offset(58); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel26Bytes() { return __p.__vector_as_arraysegment(58); }
  public string Level27 { get { int o = __p.__offset(60); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel27Bytes() { return __p.__vector_as_arraysegment(60); }
  public string Level28 { get { int o = __p.__offset(62); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel28Bytes() { return __p.__vector_as_arraysegment(62); }
  public string Level29 { get { int o = __p.__offset(64); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel29Bytes() { return __p.__vector_as_arraysegment(64); }
  public string Level30 { get { int o = __p.__offset(66); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel30Bytes() { return __p.__vector_as_arraysegment(66); }
  public string Level31 { get { int o = __p.__offset(68); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel31Bytes() { return __p.__vector_as_arraysegment(68); }
  public string Level32 { get { int o = __p.__offset(70); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel32Bytes() { return __p.__vector_as_arraysegment(70); }
  public string Level33 { get { int o = __p.__offset(72); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel33Bytes() { return __p.__vector_as_arraysegment(72); }
  public string Level34 { get { int o = __p.__offset(74); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel34Bytes() { return __p.__vector_as_arraysegment(74); }
  public string Level35 { get { int o = __p.__offset(76); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel35Bytes() { return __p.__vector_as_arraysegment(76); }
  public string Level36 { get { int o = __p.__offset(78); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel36Bytes() { return __p.__vector_as_arraysegment(78); }
  public string Level37 { get { int o = __p.__offset(80); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel37Bytes() { return __p.__vector_as_arraysegment(80); }
  public string Level38 { get { int o = __p.__offset(82); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel38Bytes() { return __p.__vector_as_arraysegment(82); }
  public string Level39 { get { int o = __p.__offset(84); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel39Bytes() { return __p.__vector_as_arraysegment(84); }
  public string Level40 { get { int o = __p.__offset(86); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel40Bytes() { return __p.__vector_as_arraysegment(86); }
  public string Level41 { get { int o = __p.__offset(88); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel41Bytes() { return __p.__vector_as_arraysegment(88); }
  public string Level42 { get { int o = __p.__offset(90); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel42Bytes() { return __p.__vector_as_arraysegment(90); }
  public string Level43 { get { int o = __p.__offset(92); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel43Bytes() { return __p.__vector_as_arraysegment(92); }
  public string Level44 { get { int o = __p.__offset(94); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel44Bytes() { return __p.__vector_as_arraysegment(94); }
  public string Level45 { get { int o = __p.__offset(96); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel45Bytes() { return __p.__vector_as_arraysegment(96); }
  public string Level46 { get { int o = __p.__offset(98); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel46Bytes() { return __p.__vector_as_arraysegment(98); }
  public string Level47 { get { int o = __p.__offset(100); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel47Bytes() { return __p.__vector_as_arraysegment(100); }
  public string Level48 { get { int o = __p.__offset(102); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel48Bytes() { return __p.__vector_as_arraysegment(102); }
  public string Level49 { get { int o = __p.__offset(104); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel49Bytes() { return __p.__vector_as_arraysegment(104); }
  public string Level50 { get { int o = __p.__offset(106); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel50Bytes() { return __p.__vector_as_arraysegment(106); }
  public string Level51 { get { int o = __p.__offset(108); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel51Bytes() { return __p.__vector_as_arraysegment(108); }
  public string Level52 { get { int o = __p.__offset(110); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel52Bytes() { return __p.__vector_as_arraysegment(110); }
  public string Level53 { get { int o = __p.__offset(112); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel53Bytes() { return __p.__vector_as_arraysegment(112); }
  public string Level54 { get { int o = __p.__offset(114); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel54Bytes() { return __p.__vector_as_arraysegment(114); }
  public string Level55 { get { int o = __p.__offset(116); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel55Bytes() { return __p.__vector_as_arraysegment(116); }
  public string Level56 { get { int o = __p.__offset(118); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel56Bytes() { return __p.__vector_as_arraysegment(118); }
  public string Level57 { get { int o = __p.__offset(120); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel57Bytes() { return __p.__vector_as_arraysegment(120); }
  public string Level58 { get { int o = __p.__offset(122); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel58Bytes() { return __p.__vector_as_arraysegment(122); }
  public string Level59 { get { int o = __p.__offset(124); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel59Bytes() { return __p.__vector_as_arraysegment(124); }
  public string Level60 { get { int o = __p.__offset(126); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetLevel60Bytes() { return __p.__vector_as_arraysegment(126); }

  public static Offset<RewardAdapterTable> CreateRewardAdapterTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      StringOffset Level1Offset = default(StringOffset),
      StringOffset Level2Offset = default(StringOffset),
      StringOffset Level3Offset = default(StringOffset),
      StringOffset Level4Offset = default(StringOffset),
      StringOffset Level5Offset = default(StringOffset),
      StringOffset Level6Offset = default(StringOffset),
      StringOffset Level7Offset = default(StringOffset),
      StringOffset Level8Offset = default(StringOffset),
      StringOffset Level9Offset = default(StringOffset),
      StringOffset Level10Offset = default(StringOffset),
      StringOffset Level11Offset = default(StringOffset),
      StringOffset Level12Offset = default(StringOffset),
      StringOffset Level13Offset = default(StringOffset),
      StringOffset Level14Offset = default(StringOffset),
      StringOffset Level15Offset = default(StringOffset),
      StringOffset Level16Offset = default(StringOffset),
      StringOffset Level17Offset = default(StringOffset),
      StringOffset Level18Offset = default(StringOffset),
      StringOffset Level19Offset = default(StringOffset),
      StringOffset Level20Offset = default(StringOffset),
      StringOffset Level21Offset = default(StringOffset),
      StringOffset Level22Offset = default(StringOffset),
      StringOffset Level23Offset = default(StringOffset),
      StringOffset Level24Offset = default(StringOffset),
      StringOffset Level25Offset = default(StringOffset),
      StringOffset Level26Offset = default(StringOffset),
      StringOffset Level27Offset = default(StringOffset),
      StringOffset Level28Offset = default(StringOffset),
      StringOffset Level29Offset = default(StringOffset),
      StringOffset Level30Offset = default(StringOffset),
      StringOffset Level31Offset = default(StringOffset),
      StringOffset Level32Offset = default(StringOffset),
      StringOffset Level33Offset = default(StringOffset),
      StringOffset Level34Offset = default(StringOffset),
      StringOffset Level35Offset = default(StringOffset),
      StringOffset Level36Offset = default(StringOffset),
      StringOffset Level37Offset = default(StringOffset),
      StringOffset Level38Offset = default(StringOffset),
      StringOffset Level39Offset = default(StringOffset),
      StringOffset Level40Offset = default(StringOffset),
      StringOffset Level41Offset = default(StringOffset),
      StringOffset Level42Offset = default(StringOffset),
      StringOffset Level43Offset = default(StringOffset),
      StringOffset Level44Offset = default(StringOffset),
      StringOffset Level45Offset = default(StringOffset),
      StringOffset Level46Offset = default(StringOffset),
      StringOffset Level47Offset = default(StringOffset),
      StringOffset Level48Offset = default(StringOffset),
      StringOffset Level49Offset = default(StringOffset),
      StringOffset Level50Offset = default(StringOffset),
      StringOffset Level51Offset = default(StringOffset),
      StringOffset Level52Offset = default(StringOffset),
      StringOffset Level53Offset = default(StringOffset),
      StringOffset Level54Offset = default(StringOffset),
      StringOffset Level55Offset = default(StringOffset),
      StringOffset Level56Offset = default(StringOffset),
      StringOffset Level57Offset = default(StringOffset),
      StringOffset Level58Offset = default(StringOffset),
      StringOffset Level59Offset = default(StringOffset),
      StringOffset Level60Offset = default(StringOffset)) {
    builder.StartObject(62);
    RewardAdapterTable.AddLevel60(builder, Level60Offset);
    RewardAdapterTable.AddLevel59(builder, Level59Offset);
    RewardAdapterTable.AddLevel58(builder, Level58Offset);
    RewardAdapterTable.AddLevel57(builder, Level57Offset);
    RewardAdapterTable.AddLevel56(builder, Level56Offset);
    RewardAdapterTable.AddLevel55(builder, Level55Offset);
    RewardAdapterTable.AddLevel54(builder, Level54Offset);
    RewardAdapterTable.AddLevel53(builder, Level53Offset);
    RewardAdapterTable.AddLevel52(builder, Level52Offset);
    RewardAdapterTable.AddLevel51(builder, Level51Offset);
    RewardAdapterTable.AddLevel50(builder, Level50Offset);
    RewardAdapterTable.AddLevel49(builder, Level49Offset);
    RewardAdapterTable.AddLevel48(builder, Level48Offset);
    RewardAdapterTable.AddLevel47(builder, Level47Offset);
    RewardAdapterTable.AddLevel46(builder, Level46Offset);
    RewardAdapterTable.AddLevel45(builder, Level45Offset);
    RewardAdapterTable.AddLevel44(builder, Level44Offset);
    RewardAdapterTable.AddLevel43(builder, Level43Offset);
    RewardAdapterTable.AddLevel42(builder, Level42Offset);
    RewardAdapterTable.AddLevel41(builder, Level41Offset);
    RewardAdapterTable.AddLevel40(builder, Level40Offset);
    RewardAdapterTable.AddLevel39(builder, Level39Offset);
    RewardAdapterTable.AddLevel38(builder, Level38Offset);
    RewardAdapterTable.AddLevel37(builder, Level37Offset);
    RewardAdapterTable.AddLevel36(builder, Level36Offset);
    RewardAdapterTable.AddLevel35(builder, Level35Offset);
    RewardAdapterTable.AddLevel34(builder, Level34Offset);
    RewardAdapterTable.AddLevel33(builder, Level33Offset);
    RewardAdapterTable.AddLevel32(builder, Level32Offset);
    RewardAdapterTable.AddLevel31(builder, Level31Offset);
    RewardAdapterTable.AddLevel30(builder, Level30Offset);
    RewardAdapterTable.AddLevel29(builder, Level29Offset);
    RewardAdapterTable.AddLevel28(builder, Level28Offset);
    RewardAdapterTable.AddLevel27(builder, Level27Offset);
    RewardAdapterTable.AddLevel26(builder, Level26Offset);
    RewardAdapterTable.AddLevel25(builder, Level25Offset);
    RewardAdapterTable.AddLevel24(builder, Level24Offset);
    RewardAdapterTable.AddLevel23(builder, Level23Offset);
    RewardAdapterTable.AddLevel22(builder, Level22Offset);
    RewardAdapterTable.AddLevel21(builder, Level21Offset);
    RewardAdapterTable.AddLevel20(builder, Level20Offset);
    RewardAdapterTable.AddLevel19(builder, Level19Offset);
    RewardAdapterTable.AddLevel18(builder, Level18Offset);
    RewardAdapterTable.AddLevel17(builder, Level17Offset);
    RewardAdapterTable.AddLevel16(builder, Level16Offset);
    RewardAdapterTable.AddLevel15(builder, Level15Offset);
    RewardAdapterTable.AddLevel14(builder, Level14Offset);
    RewardAdapterTable.AddLevel13(builder, Level13Offset);
    RewardAdapterTable.AddLevel12(builder, Level12Offset);
    RewardAdapterTable.AddLevel11(builder, Level11Offset);
    RewardAdapterTable.AddLevel10(builder, Level10Offset);
    RewardAdapterTable.AddLevel9(builder, Level9Offset);
    RewardAdapterTable.AddLevel8(builder, Level8Offset);
    RewardAdapterTable.AddLevel7(builder, Level7Offset);
    RewardAdapterTable.AddLevel6(builder, Level6Offset);
    RewardAdapterTable.AddLevel5(builder, Level5Offset);
    RewardAdapterTable.AddLevel4(builder, Level4Offset);
    RewardAdapterTable.AddLevel3(builder, Level3Offset);
    RewardAdapterTable.AddLevel2(builder, Level2Offset);
    RewardAdapterTable.AddLevel1(builder, Level1Offset);
    RewardAdapterTable.AddName(builder, NameOffset);
    RewardAdapterTable.AddID(builder, ID);
    return RewardAdapterTable.EndRewardAdapterTable(builder);
  }

  public static void StartRewardAdapterTable(FlatBufferBuilder builder) { builder.StartObject(62); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddLevel1(FlatBufferBuilder builder, StringOffset Level1Offset) { builder.AddOffset(2, Level1Offset.Value, 0); }
  public static void AddLevel2(FlatBufferBuilder builder, StringOffset Level2Offset) { builder.AddOffset(3, Level2Offset.Value, 0); }
  public static void AddLevel3(FlatBufferBuilder builder, StringOffset Level3Offset) { builder.AddOffset(4, Level3Offset.Value, 0); }
  public static void AddLevel4(FlatBufferBuilder builder, StringOffset Level4Offset) { builder.AddOffset(5, Level4Offset.Value, 0); }
  public static void AddLevel5(FlatBufferBuilder builder, StringOffset Level5Offset) { builder.AddOffset(6, Level5Offset.Value, 0); }
  public static void AddLevel6(FlatBufferBuilder builder, StringOffset Level6Offset) { builder.AddOffset(7, Level6Offset.Value, 0); }
  public static void AddLevel7(FlatBufferBuilder builder, StringOffset Level7Offset) { builder.AddOffset(8, Level7Offset.Value, 0); }
  public static void AddLevel8(FlatBufferBuilder builder, StringOffset Level8Offset) { builder.AddOffset(9, Level8Offset.Value, 0); }
  public static void AddLevel9(FlatBufferBuilder builder, StringOffset Level9Offset) { builder.AddOffset(10, Level9Offset.Value, 0); }
  public static void AddLevel10(FlatBufferBuilder builder, StringOffset Level10Offset) { builder.AddOffset(11, Level10Offset.Value, 0); }
  public static void AddLevel11(FlatBufferBuilder builder, StringOffset Level11Offset) { builder.AddOffset(12, Level11Offset.Value, 0); }
  public static void AddLevel12(FlatBufferBuilder builder, StringOffset Level12Offset) { builder.AddOffset(13, Level12Offset.Value, 0); }
  public static void AddLevel13(FlatBufferBuilder builder, StringOffset Level13Offset) { builder.AddOffset(14, Level13Offset.Value, 0); }
  public static void AddLevel14(FlatBufferBuilder builder, StringOffset Level14Offset) { builder.AddOffset(15, Level14Offset.Value, 0); }
  public static void AddLevel15(FlatBufferBuilder builder, StringOffset Level15Offset) { builder.AddOffset(16, Level15Offset.Value, 0); }
  public static void AddLevel16(FlatBufferBuilder builder, StringOffset Level16Offset) { builder.AddOffset(17, Level16Offset.Value, 0); }
  public static void AddLevel17(FlatBufferBuilder builder, StringOffset Level17Offset) { builder.AddOffset(18, Level17Offset.Value, 0); }
  public static void AddLevel18(FlatBufferBuilder builder, StringOffset Level18Offset) { builder.AddOffset(19, Level18Offset.Value, 0); }
  public static void AddLevel19(FlatBufferBuilder builder, StringOffset Level19Offset) { builder.AddOffset(20, Level19Offset.Value, 0); }
  public static void AddLevel20(FlatBufferBuilder builder, StringOffset Level20Offset) { builder.AddOffset(21, Level20Offset.Value, 0); }
  public static void AddLevel21(FlatBufferBuilder builder, StringOffset Level21Offset) { builder.AddOffset(22, Level21Offset.Value, 0); }
  public static void AddLevel22(FlatBufferBuilder builder, StringOffset Level22Offset) { builder.AddOffset(23, Level22Offset.Value, 0); }
  public static void AddLevel23(FlatBufferBuilder builder, StringOffset Level23Offset) { builder.AddOffset(24, Level23Offset.Value, 0); }
  public static void AddLevel24(FlatBufferBuilder builder, StringOffset Level24Offset) { builder.AddOffset(25, Level24Offset.Value, 0); }
  public static void AddLevel25(FlatBufferBuilder builder, StringOffset Level25Offset) { builder.AddOffset(26, Level25Offset.Value, 0); }
  public static void AddLevel26(FlatBufferBuilder builder, StringOffset Level26Offset) { builder.AddOffset(27, Level26Offset.Value, 0); }
  public static void AddLevel27(FlatBufferBuilder builder, StringOffset Level27Offset) { builder.AddOffset(28, Level27Offset.Value, 0); }
  public static void AddLevel28(FlatBufferBuilder builder, StringOffset Level28Offset) { builder.AddOffset(29, Level28Offset.Value, 0); }
  public static void AddLevel29(FlatBufferBuilder builder, StringOffset Level29Offset) { builder.AddOffset(30, Level29Offset.Value, 0); }
  public static void AddLevel30(FlatBufferBuilder builder, StringOffset Level30Offset) { builder.AddOffset(31, Level30Offset.Value, 0); }
  public static void AddLevel31(FlatBufferBuilder builder, StringOffset Level31Offset) { builder.AddOffset(32, Level31Offset.Value, 0); }
  public static void AddLevel32(FlatBufferBuilder builder, StringOffset Level32Offset) { builder.AddOffset(33, Level32Offset.Value, 0); }
  public static void AddLevel33(FlatBufferBuilder builder, StringOffset Level33Offset) { builder.AddOffset(34, Level33Offset.Value, 0); }
  public static void AddLevel34(FlatBufferBuilder builder, StringOffset Level34Offset) { builder.AddOffset(35, Level34Offset.Value, 0); }
  public static void AddLevel35(FlatBufferBuilder builder, StringOffset Level35Offset) { builder.AddOffset(36, Level35Offset.Value, 0); }
  public static void AddLevel36(FlatBufferBuilder builder, StringOffset Level36Offset) { builder.AddOffset(37, Level36Offset.Value, 0); }
  public static void AddLevel37(FlatBufferBuilder builder, StringOffset Level37Offset) { builder.AddOffset(38, Level37Offset.Value, 0); }
  public static void AddLevel38(FlatBufferBuilder builder, StringOffset Level38Offset) { builder.AddOffset(39, Level38Offset.Value, 0); }
  public static void AddLevel39(FlatBufferBuilder builder, StringOffset Level39Offset) { builder.AddOffset(40, Level39Offset.Value, 0); }
  public static void AddLevel40(FlatBufferBuilder builder, StringOffset Level40Offset) { builder.AddOffset(41, Level40Offset.Value, 0); }
  public static void AddLevel41(FlatBufferBuilder builder, StringOffset Level41Offset) { builder.AddOffset(42, Level41Offset.Value, 0); }
  public static void AddLevel42(FlatBufferBuilder builder, StringOffset Level42Offset) { builder.AddOffset(43, Level42Offset.Value, 0); }
  public static void AddLevel43(FlatBufferBuilder builder, StringOffset Level43Offset) { builder.AddOffset(44, Level43Offset.Value, 0); }
  public static void AddLevel44(FlatBufferBuilder builder, StringOffset Level44Offset) { builder.AddOffset(45, Level44Offset.Value, 0); }
  public static void AddLevel45(FlatBufferBuilder builder, StringOffset Level45Offset) { builder.AddOffset(46, Level45Offset.Value, 0); }
  public static void AddLevel46(FlatBufferBuilder builder, StringOffset Level46Offset) { builder.AddOffset(47, Level46Offset.Value, 0); }
  public static void AddLevel47(FlatBufferBuilder builder, StringOffset Level47Offset) { builder.AddOffset(48, Level47Offset.Value, 0); }
  public static void AddLevel48(FlatBufferBuilder builder, StringOffset Level48Offset) { builder.AddOffset(49, Level48Offset.Value, 0); }
  public static void AddLevel49(FlatBufferBuilder builder, StringOffset Level49Offset) { builder.AddOffset(50, Level49Offset.Value, 0); }
  public static void AddLevel50(FlatBufferBuilder builder, StringOffset Level50Offset) { builder.AddOffset(51, Level50Offset.Value, 0); }
  public static void AddLevel51(FlatBufferBuilder builder, StringOffset Level51Offset) { builder.AddOffset(52, Level51Offset.Value, 0); }
  public static void AddLevel52(FlatBufferBuilder builder, StringOffset Level52Offset) { builder.AddOffset(53, Level52Offset.Value, 0); }
  public static void AddLevel53(FlatBufferBuilder builder, StringOffset Level53Offset) { builder.AddOffset(54, Level53Offset.Value, 0); }
  public static void AddLevel54(FlatBufferBuilder builder, StringOffset Level54Offset) { builder.AddOffset(55, Level54Offset.Value, 0); }
  public static void AddLevel55(FlatBufferBuilder builder, StringOffset Level55Offset) { builder.AddOffset(56, Level55Offset.Value, 0); }
  public static void AddLevel56(FlatBufferBuilder builder, StringOffset Level56Offset) { builder.AddOffset(57, Level56Offset.Value, 0); }
  public static void AddLevel57(FlatBufferBuilder builder, StringOffset Level57Offset) { builder.AddOffset(58, Level57Offset.Value, 0); }
  public static void AddLevel58(FlatBufferBuilder builder, StringOffset Level58Offset) { builder.AddOffset(59, Level58Offset.Value, 0); }
  public static void AddLevel59(FlatBufferBuilder builder, StringOffset Level59Offset) { builder.AddOffset(60, Level59Offset.Value, 0); }
  public static void AddLevel60(FlatBufferBuilder builder, StringOffset Level60Offset) { builder.AddOffset(61, Level60Offset.Value, 0); }
  public static Offset<RewardAdapterTable> EndRewardAdapterTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<RewardAdapterTable>(o);
  }
  public static void FinishRewardAdapterTableBuffer(FlatBufferBuilder builder, Offset<RewardAdapterTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

