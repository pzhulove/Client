// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class SkillComboTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -1949726329,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static SkillComboTable GetRootAsSkillComboTable(ByteBuffer _bb) { return GetRootAsSkillComboTable(_bb, new SkillComboTable()); }
  public static SkillComboTable GetRootAsSkillComboTable(ByteBuffer _bb, SkillComboTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public SkillComboTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int roomID { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int skillGroupID { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int skillID { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int skillLevel { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int skillSlot { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string description { get { int o = __p.__offset(16); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDescriptionBytes() { return __p.__vector_as_arraysegment(16); }
  public int skillTime { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int startDialogID { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int endDialogID { get { int o = __p.__offset(22); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string guideTip { get { int o = __p.__offset(24); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetGuideTipBytes() { return __p.__vector_as_arraysegment(24); }
  public int joystick { get { int o = __p.__offset(26); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int waitInputTime { get { int o = __p.__offset(28); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int moveDir { get { int o = __p.__offset(30); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int moveTime { get { int o = __p.__offset(32); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int idletime { get { int o = __p.__offset(34); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int showUI { get { int o = __p.__offset(36); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int faceRight { get { int o = __p.__offset(38); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int sourceID { get { int o = __p.__offset(40); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int phase { get { int o = __p.__offset(42); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<SkillComboTable> CreateSkillComboTable(FlatBufferBuilder builder,
      int ID = 0,
      int roomID = 0,
      int skillGroupID = 0,
      int skillID = 0,
      int skillLevel = 0,
      int skillSlot = 0,
      StringOffset descriptionOffset = default(StringOffset),
      int skillTime = 0,
      int startDialogID = 0,
      int endDialogID = 0,
      StringOffset guideTipOffset = default(StringOffset),
      int joystick = 0,
      int waitInputTime = 0,
      int moveDir = 0,
      int moveTime = 0,
      int idletime = 0,
      int showUI = 0,
      int faceRight = 0,
      int sourceID = 0,
      int phase = 0) {
    builder.StartObject(20);
    SkillComboTable.AddPhase(builder, phase);
    SkillComboTable.AddSourceID(builder, sourceID);
    SkillComboTable.AddFaceRight(builder, faceRight);
    SkillComboTable.AddShowUI(builder, showUI);
    SkillComboTable.AddIdletime(builder, idletime);
    SkillComboTable.AddMoveTime(builder, moveTime);
    SkillComboTable.AddMoveDir(builder, moveDir);
    SkillComboTable.AddWaitInputTime(builder, waitInputTime);
    SkillComboTable.AddJoystick(builder, joystick);
    SkillComboTable.AddGuideTip(builder, guideTipOffset);
    SkillComboTable.AddEndDialogID(builder, endDialogID);
    SkillComboTable.AddStartDialogID(builder, startDialogID);
    SkillComboTable.AddSkillTime(builder, skillTime);
    SkillComboTable.AddDescription(builder, descriptionOffset);
    SkillComboTable.AddSkillSlot(builder, skillSlot);
    SkillComboTable.AddSkillLevel(builder, skillLevel);
    SkillComboTable.AddSkillID(builder, skillID);
    SkillComboTable.AddSkillGroupID(builder, skillGroupID);
    SkillComboTable.AddRoomID(builder, roomID);
    SkillComboTable.AddID(builder, ID);
    return SkillComboTable.EndSkillComboTable(builder);
  }

  public static void StartSkillComboTable(FlatBufferBuilder builder) { builder.StartObject(20); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddRoomID(FlatBufferBuilder builder, int roomID) { builder.AddInt(1, roomID, 0); }
  public static void AddSkillGroupID(FlatBufferBuilder builder, int skillGroupID) { builder.AddInt(2, skillGroupID, 0); }
  public static void AddSkillID(FlatBufferBuilder builder, int skillID) { builder.AddInt(3, skillID, 0); }
  public static void AddSkillLevel(FlatBufferBuilder builder, int skillLevel) { builder.AddInt(4, skillLevel, 0); }
  public static void AddSkillSlot(FlatBufferBuilder builder, int skillSlot) { builder.AddInt(5, skillSlot, 0); }
  public static void AddDescription(FlatBufferBuilder builder, StringOffset descriptionOffset) { builder.AddOffset(6, descriptionOffset.Value, 0); }
  public static void AddSkillTime(FlatBufferBuilder builder, int skillTime) { builder.AddInt(7, skillTime, 0); }
  public static void AddStartDialogID(FlatBufferBuilder builder, int startDialogID) { builder.AddInt(8, startDialogID, 0); }
  public static void AddEndDialogID(FlatBufferBuilder builder, int endDialogID) { builder.AddInt(9, endDialogID, 0); }
  public static void AddGuideTip(FlatBufferBuilder builder, StringOffset guideTipOffset) { builder.AddOffset(10, guideTipOffset.Value, 0); }
  public static void AddJoystick(FlatBufferBuilder builder, int joystick) { builder.AddInt(11, joystick, 0); }
  public static void AddWaitInputTime(FlatBufferBuilder builder, int waitInputTime) { builder.AddInt(12, waitInputTime, 0); }
  public static void AddMoveDir(FlatBufferBuilder builder, int moveDir) { builder.AddInt(13, moveDir, 0); }
  public static void AddMoveTime(FlatBufferBuilder builder, int moveTime) { builder.AddInt(14, moveTime, 0); }
  public static void AddIdletime(FlatBufferBuilder builder, int idletime) { builder.AddInt(15, idletime, 0); }
  public static void AddShowUI(FlatBufferBuilder builder, int showUI) { builder.AddInt(16, showUI, 0); }
  public static void AddFaceRight(FlatBufferBuilder builder, int faceRight) { builder.AddInt(17, faceRight, 0); }
  public static void AddSourceID(FlatBufferBuilder builder, int sourceID) { builder.AddInt(18, sourceID, 0); }
  public static void AddPhase(FlatBufferBuilder builder, int phase) { builder.AddInt(19, phase, 0); }
  public static Offset<SkillComboTable> EndSkillComboTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SkillComboTable>(o);
  }
  public static void FinishSkillComboTableBuffer(FlatBufferBuilder builder, Offset<SkillComboTable> offset) { builder.Finish(offset.Value); }
};


}


#endif

