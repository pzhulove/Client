// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class RemovejewelsTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -719475222,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static RemovejewelsTable GetRootAsRemovejewelsTable(ByteBuffer _bb) { return GetRootAsRemovejewelsTable(_bb, new RemovejewelsTable()); }
  public static RemovejewelsTable GetRootAsRemovejewelsTable(ByteBuffer _bb, RemovejewelsTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public RemovejewelsTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Colour { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Grades { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Material1 { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Num1 { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Success1 { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int BeadType { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int PickNum { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<RemovejewelsTable> CreateRemovejewelsTable(FlatBufferBuilder builder,
      int ID = 0,
      int Colour = 0,
      int Grades = 0,
      int Material1 = 0,
      int Num1 = 0,
      int Success1 = 0,
      int BeadType = 0,
      int PickNum = 0) {
    builder.StartObject(8);
    RemovejewelsTable.AddPickNum(builder, PickNum);
    RemovejewelsTable.AddBeadType(builder, BeadType);
    RemovejewelsTable.AddSuccess1(builder, Success1);
    RemovejewelsTable.AddNum1(builder, Num1);
    RemovejewelsTable.AddMaterial1(builder, Material1);
    RemovejewelsTable.AddGrades(builder, Grades);
    RemovejewelsTable.AddColour(builder, Colour);
    RemovejewelsTable.AddID(builder, ID);
    return RemovejewelsTable.EndRemovejewelsTable(builder);
  }

  public static void StartRemovejewelsTable(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddColour(FlatBufferBuilder builder, int Colour) { builder.AddInt(1, Colour, 0); }
  public static void AddGrades(FlatBufferBuilder builder, int Grades) { builder.AddInt(2, Grades, 0); }
  public static void AddMaterial1(FlatBufferBuilder builder, int Material1) { builder.AddInt(3, Material1, 0); }
  public static void AddNum1(FlatBufferBuilder builder, int Num1) { builder.AddInt(4, Num1, 0); }
  public static void AddSuccess1(FlatBufferBuilder builder, int Success1) { builder.AddInt(5, Success1, 0); }
  public static void AddBeadType(FlatBufferBuilder builder, int BeadType) { builder.AddInt(6, BeadType, 0); }
  public static void AddPickNum(FlatBufferBuilder builder, int PickNum) { builder.AddInt(7, PickNum, 0); }
  public static Offset<RemovejewelsTable> EndRemovejewelsTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<RemovejewelsTable>(o);
  }
  public static void FinishRemovejewelsTableBuffer(FlatBufferBuilder builder, Offset<RemovejewelsTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
