// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class LevelAdapterTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -815458749,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static LevelAdapterTable GetRootAsLevelAdapterTable(ByteBuffer _bb) { return GetRootAsLevelAdapterTable(_bb, new LevelAdapterTable()); }
  public static LevelAdapterTable GetRootAsLevelAdapterTable(ByteBuffer _bb, LevelAdapterTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public LevelAdapterTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
  public int Level1 { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level2 { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level3 { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level4 { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level5 { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level6 { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level7 { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level8 { get { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level9 { get { int o = __p.__offset(24); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level10 { get { int o = __p.__offset(26); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level11 { get { int o = __p.__offset(28); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level12 { get { int o = __p.__offset(30); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level13 { get { int o = __p.__offset(32); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level14 { get { int o = __p.__offset(34); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level15 { get { int o = __p.__offset(36); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level16 { get { int o = __p.__offset(38); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level17 { get { int o = __p.__offset(40); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level18 { get { int o = __p.__offset(42); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level19 { get { int o = __p.__offset(44); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level20 { get { int o = __p.__offset(46); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level21 { get { int o = __p.__offset(48); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level22 { get { int o = __p.__offset(50); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level23 { get { int o = __p.__offset(52); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level24 { get { int o = __p.__offset(54); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level25 { get { int o = __p.__offset(56); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level26 { get { int o = __p.__offset(58); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level27 { get { int o = __p.__offset(60); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level28 { get { int o = __p.__offset(62); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level29 { get { int o = __p.__offset(64); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level30 { get { int o = __p.__offset(66); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level31 { get { int o = __p.__offset(68); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level32 { get { int o = __p.__offset(70); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level33 { get { int o = __p.__offset(72); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level34 { get { int o = __p.__offset(74); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level35 { get { int o = __p.__offset(76); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level36 { get { int o = __p.__offset(78); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level37 { get { int o = __p.__offset(80); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level38 { get { int o = __p.__offset(82); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level39 { get { int o = __p.__offset(84); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level40 { get { int o = __p.__offset(86); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level41 { get { int o = __p.__offset(88); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level42 { get { int o = __p.__offset(90); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level43 { get { int o = __p.__offset(92); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level44 { get { int o = __p.__offset(94); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level45 { get { int o = __p.__offset(96); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level46 { get { int o = __p.__offset(98); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level47 { get { int o = __p.__offset(100); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level48 { get { int o = __p.__offset(102); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level49 { get { int o = __p.__offset(104); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level50 { get { int o = __p.__offset(106); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level51 { get { int o = __p.__offset(108); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level52 { get { int o = __p.__offset(110); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level53 { get { int o = __p.__offset(112); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level54 { get { int o = __p.__offset(114); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level55 { get { int o = __p.__offset(116); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level56 { get { int o = __p.__offset(118); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level57 { get { int o = __p.__offset(120); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level58 { get { int o = __p.__offset(122); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level59 { get { int o = __p.__offset(124); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Level60 { get { int o = __p.__offset(126); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<LevelAdapterTable> CreateLevelAdapterTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      int Level1 = 0,
      int Level2 = 0,
      int Level3 = 0,
      int Level4 = 0,
      int Level5 = 0,
      int Level6 = 0,
      int Level7 = 0,
      int Level8 = 0,
      int Level9 = 0,
      int Level10 = 0,
      int Level11 = 0,
      int Level12 = 0,
      int Level13 = 0,
      int Level14 = 0,
      int Level15 = 0,
      int Level16 = 0,
      int Level17 = 0,
      int Level18 = 0,
      int Level19 = 0,
      int Level20 = 0,
      int Level21 = 0,
      int Level22 = 0,
      int Level23 = 0,
      int Level24 = 0,
      int Level25 = 0,
      int Level26 = 0,
      int Level27 = 0,
      int Level28 = 0,
      int Level29 = 0,
      int Level30 = 0,
      int Level31 = 0,
      int Level32 = 0,
      int Level33 = 0,
      int Level34 = 0,
      int Level35 = 0,
      int Level36 = 0,
      int Level37 = 0,
      int Level38 = 0,
      int Level39 = 0,
      int Level40 = 0,
      int Level41 = 0,
      int Level42 = 0,
      int Level43 = 0,
      int Level44 = 0,
      int Level45 = 0,
      int Level46 = 0,
      int Level47 = 0,
      int Level48 = 0,
      int Level49 = 0,
      int Level50 = 0,
      int Level51 = 0,
      int Level52 = 0,
      int Level53 = 0,
      int Level54 = 0,
      int Level55 = 0,
      int Level56 = 0,
      int Level57 = 0,
      int Level58 = 0,
      int Level59 = 0,
      int Level60 = 0) {
    builder.StartObject(62);
    LevelAdapterTable.AddLevel60(builder, Level60);
    LevelAdapterTable.AddLevel59(builder, Level59);
    LevelAdapterTable.AddLevel58(builder, Level58);
    LevelAdapterTable.AddLevel57(builder, Level57);
    LevelAdapterTable.AddLevel56(builder, Level56);
    LevelAdapterTable.AddLevel55(builder, Level55);
    LevelAdapterTable.AddLevel54(builder, Level54);
    LevelAdapterTable.AddLevel53(builder, Level53);
    LevelAdapterTable.AddLevel52(builder, Level52);
    LevelAdapterTable.AddLevel51(builder, Level51);
    LevelAdapterTable.AddLevel50(builder, Level50);
    LevelAdapterTable.AddLevel49(builder, Level49);
    LevelAdapterTable.AddLevel48(builder, Level48);
    LevelAdapterTable.AddLevel47(builder, Level47);
    LevelAdapterTable.AddLevel46(builder, Level46);
    LevelAdapterTable.AddLevel45(builder, Level45);
    LevelAdapterTable.AddLevel44(builder, Level44);
    LevelAdapterTable.AddLevel43(builder, Level43);
    LevelAdapterTable.AddLevel42(builder, Level42);
    LevelAdapterTable.AddLevel41(builder, Level41);
    LevelAdapterTable.AddLevel40(builder, Level40);
    LevelAdapterTable.AddLevel39(builder, Level39);
    LevelAdapterTable.AddLevel38(builder, Level38);
    LevelAdapterTable.AddLevel37(builder, Level37);
    LevelAdapterTable.AddLevel36(builder, Level36);
    LevelAdapterTable.AddLevel35(builder, Level35);
    LevelAdapterTable.AddLevel34(builder, Level34);
    LevelAdapterTable.AddLevel33(builder, Level33);
    LevelAdapterTable.AddLevel32(builder, Level32);
    LevelAdapterTable.AddLevel31(builder, Level31);
    LevelAdapterTable.AddLevel30(builder, Level30);
    LevelAdapterTable.AddLevel29(builder, Level29);
    LevelAdapterTable.AddLevel28(builder, Level28);
    LevelAdapterTable.AddLevel27(builder, Level27);
    LevelAdapterTable.AddLevel26(builder, Level26);
    LevelAdapterTable.AddLevel25(builder, Level25);
    LevelAdapterTable.AddLevel24(builder, Level24);
    LevelAdapterTable.AddLevel23(builder, Level23);
    LevelAdapterTable.AddLevel22(builder, Level22);
    LevelAdapterTable.AddLevel21(builder, Level21);
    LevelAdapterTable.AddLevel20(builder, Level20);
    LevelAdapterTable.AddLevel19(builder, Level19);
    LevelAdapterTable.AddLevel18(builder, Level18);
    LevelAdapterTable.AddLevel17(builder, Level17);
    LevelAdapterTable.AddLevel16(builder, Level16);
    LevelAdapterTable.AddLevel15(builder, Level15);
    LevelAdapterTable.AddLevel14(builder, Level14);
    LevelAdapterTable.AddLevel13(builder, Level13);
    LevelAdapterTable.AddLevel12(builder, Level12);
    LevelAdapterTable.AddLevel11(builder, Level11);
    LevelAdapterTable.AddLevel10(builder, Level10);
    LevelAdapterTable.AddLevel9(builder, Level9);
    LevelAdapterTable.AddLevel8(builder, Level8);
    LevelAdapterTable.AddLevel7(builder, Level7);
    LevelAdapterTable.AddLevel6(builder, Level6);
    LevelAdapterTable.AddLevel5(builder, Level5);
    LevelAdapterTable.AddLevel4(builder, Level4);
    LevelAdapterTable.AddLevel3(builder, Level3);
    LevelAdapterTable.AddLevel2(builder, Level2);
    LevelAdapterTable.AddLevel1(builder, Level1);
    LevelAdapterTable.AddName(builder, NameOffset);
    LevelAdapterTable.AddID(builder, ID);
    return LevelAdapterTable.EndLevelAdapterTable(builder);
  }

  public static void StartLevelAdapterTable(FlatBufferBuilder builder) { builder.StartObject(62); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddLevel1(FlatBufferBuilder builder, int Level1) { builder.AddInt(2, Level1, 0); }
  public static void AddLevel2(FlatBufferBuilder builder, int Level2) { builder.AddInt(3, Level2, 0); }
  public static void AddLevel3(FlatBufferBuilder builder, int Level3) { builder.AddInt(4, Level3, 0); }
  public static void AddLevel4(FlatBufferBuilder builder, int Level4) { builder.AddInt(5, Level4, 0); }
  public static void AddLevel5(FlatBufferBuilder builder, int Level5) { builder.AddInt(6, Level5, 0); }
  public static void AddLevel6(FlatBufferBuilder builder, int Level6) { builder.AddInt(7, Level6, 0); }
  public static void AddLevel7(FlatBufferBuilder builder, int Level7) { builder.AddInt(8, Level7, 0); }
  public static void AddLevel8(FlatBufferBuilder builder, int Level8) { builder.AddInt(9, Level8, 0); }
  public static void AddLevel9(FlatBufferBuilder builder, int Level9) { builder.AddInt(10, Level9, 0); }
  public static void AddLevel10(FlatBufferBuilder builder, int Level10) { builder.AddInt(11, Level10, 0); }
  public static void AddLevel11(FlatBufferBuilder builder, int Level11) { builder.AddInt(12, Level11, 0); }
  public static void AddLevel12(FlatBufferBuilder builder, int Level12) { builder.AddInt(13, Level12, 0); }
  public static void AddLevel13(FlatBufferBuilder builder, int Level13) { builder.AddInt(14, Level13, 0); }
  public static void AddLevel14(FlatBufferBuilder builder, int Level14) { builder.AddInt(15, Level14, 0); }
  public static void AddLevel15(FlatBufferBuilder builder, int Level15) { builder.AddInt(16, Level15, 0); }
  public static void AddLevel16(FlatBufferBuilder builder, int Level16) { builder.AddInt(17, Level16, 0); }
  public static void AddLevel17(FlatBufferBuilder builder, int Level17) { builder.AddInt(18, Level17, 0); }
  public static void AddLevel18(FlatBufferBuilder builder, int Level18) { builder.AddInt(19, Level18, 0); }
  public static void AddLevel19(FlatBufferBuilder builder, int Level19) { builder.AddInt(20, Level19, 0); }
  public static void AddLevel20(FlatBufferBuilder builder, int Level20) { builder.AddInt(21, Level20, 0); }
  public static void AddLevel21(FlatBufferBuilder builder, int Level21) { builder.AddInt(22, Level21, 0); }
  public static void AddLevel22(FlatBufferBuilder builder, int Level22) { builder.AddInt(23, Level22, 0); }
  public static void AddLevel23(FlatBufferBuilder builder, int Level23) { builder.AddInt(24, Level23, 0); }
  public static void AddLevel24(FlatBufferBuilder builder, int Level24) { builder.AddInt(25, Level24, 0); }
  public static void AddLevel25(FlatBufferBuilder builder, int Level25) { builder.AddInt(26, Level25, 0); }
  public static void AddLevel26(FlatBufferBuilder builder, int Level26) { builder.AddInt(27, Level26, 0); }
  public static void AddLevel27(FlatBufferBuilder builder, int Level27) { builder.AddInt(28, Level27, 0); }
  public static void AddLevel28(FlatBufferBuilder builder, int Level28) { builder.AddInt(29, Level28, 0); }
  public static void AddLevel29(FlatBufferBuilder builder, int Level29) { builder.AddInt(30, Level29, 0); }
  public static void AddLevel30(FlatBufferBuilder builder, int Level30) { builder.AddInt(31, Level30, 0); }
  public static void AddLevel31(FlatBufferBuilder builder, int Level31) { builder.AddInt(32, Level31, 0); }
  public static void AddLevel32(FlatBufferBuilder builder, int Level32) { builder.AddInt(33, Level32, 0); }
  public static void AddLevel33(FlatBufferBuilder builder, int Level33) { builder.AddInt(34, Level33, 0); }
  public static void AddLevel34(FlatBufferBuilder builder, int Level34) { builder.AddInt(35, Level34, 0); }
  public static void AddLevel35(FlatBufferBuilder builder, int Level35) { builder.AddInt(36, Level35, 0); }
  public static void AddLevel36(FlatBufferBuilder builder, int Level36) { builder.AddInt(37, Level36, 0); }
  public static void AddLevel37(FlatBufferBuilder builder, int Level37) { builder.AddInt(38, Level37, 0); }
  public static void AddLevel38(FlatBufferBuilder builder, int Level38) { builder.AddInt(39, Level38, 0); }
  public static void AddLevel39(FlatBufferBuilder builder, int Level39) { builder.AddInt(40, Level39, 0); }
  public static void AddLevel40(FlatBufferBuilder builder, int Level40) { builder.AddInt(41, Level40, 0); }
  public static void AddLevel41(FlatBufferBuilder builder, int Level41) { builder.AddInt(42, Level41, 0); }
  public static void AddLevel42(FlatBufferBuilder builder, int Level42) { builder.AddInt(43, Level42, 0); }
  public static void AddLevel43(FlatBufferBuilder builder, int Level43) { builder.AddInt(44, Level43, 0); }
  public static void AddLevel44(FlatBufferBuilder builder, int Level44) { builder.AddInt(45, Level44, 0); }
  public static void AddLevel45(FlatBufferBuilder builder, int Level45) { builder.AddInt(46, Level45, 0); }
  public static void AddLevel46(FlatBufferBuilder builder, int Level46) { builder.AddInt(47, Level46, 0); }
  public static void AddLevel47(FlatBufferBuilder builder, int Level47) { builder.AddInt(48, Level47, 0); }
  public static void AddLevel48(FlatBufferBuilder builder, int Level48) { builder.AddInt(49, Level48, 0); }
  public static void AddLevel49(FlatBufferBuilder builder, int Level49) { builder.AddInt(50, Level49, 0); }
  public static void AddLevel50(FlatBufferBuilder builder, int Level50) { builder.AddInt(51, Level50, 0); }
  public static void AddLevel51(FlatBufferBuilder builder, int Level51) { builder.AddInt(52, Level51, 0); }
  public static void AddLevel52(FlatBufferBuilder builder, int Level52) { builder.AddInt(53, Level52, 0); }
  public static void AddLevel53(FlatBufferBuilder builder, int Level53) { builder.AddInt(54, Level53, 0); }
  public static void AddLevel54(FlatBufferBuilder builder, int Level54) { builder.AddInt(55, Level54, 0); }
  public static void AddLevel55(FlatBufferBuilder builder, int Level55) { builder.AddInt(56, Level55, 0); }
  public static void AddLevel56(FlatBufferBuilder builder, int Level56) { builder.AddInt(57, Level56, 0); }
  public static void AddLevel57(FlatBufferBuilder builder, int Level57) { builder.AddInt(58, Level57, 0); }
  public static void AddLevel58(FlatBufferBuilder builder, int Level58) { builder.AddInt(59, Level58, 0); }
  public static void AddLevel59(FlatBufferBuilder builder, int Level59) { builder.AddInt(60, Level59, 0); }
  public static void AddLevel60(FlatBufferBuilder builder, int Level60) { builder.AddInt(61, Level60, 0); }
  public static Offset<LevelAdapterTable> EndLevelAdapterTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LevelAdapterTable>(o);
  }
  public static void FinishLevelAdapterTableBuffer(FlatBufferBuilder builder, Offset<LevelAdapterTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
