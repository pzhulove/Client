// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class TAPQuestionnaireTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -427165796,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static TAPQuestionnaireTable GetRootAsTAPQuestionnaireTable(ByteBuffer _bb) { return GetRootAsTAPQuestionnaireTable(_bb, new TAPQuestionnaireTable()); }
  public static TAPQuestionnaireTable GetRootAsTAPQuestionnaireTable(ByteBuffer _bb, TAPQuestionnaireTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public TAPQuestionnaireTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Type { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int QuestionIndex { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string QuestionDes { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetQuestionDesBytes() { return __p.__vector_as_arraysegment(10); }
  public int AnswerIndex { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string AnswerDes { get { int o = __p.__offset(14); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetAnswerDesBytes() { return __p.__vector_as_arraysegment(14); }

  public static Offset<TAPQuestionnaireTable> CreateTAPQuestionnaireTable(FlatBufferBuilder builder,
      int ID = 0,
      int Type = 0,
      int QuestionIndex = 0,
      StringOffset QuestionDesOffset = default(StringOffset),
      int AnswerIndex = 0,
      StringOffset AnswerDesOffset = default(StringOffset)) {
    builder.StartObject(6);
    TAPQuestionnaireTable.AddAnswerDes(builder, AnswerDesOffset);
    TAPQuestionnaireTable.AddAnswerIndex(builder, AnswerIndex);
    TAPQuestionnaireTable.AddQuestionDes(builder, QuestionDesOffset);
    TAPQuestionnaireTable.AddQuestionIndex(builder, QuestionIndex);
    TAPQuestionnaireTable.AddType(builder, Type);
    TAPQuestionnaireTable.AddID(builder, ID);
    return TAPQuestionnaireTable.EndTAPQuestionnaireTable(builder);
  }

  public static void StartTAPQuestionnaireTable(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddType(FlatBufferBuilder builder, int Type) { builder.AddInt(1, Type, 0); }
  public static void AddQuestionIndex(FlatBufferBuilder builder, int QuestionIndex) { builder.AddInt(2, QuestionIndex, 0); }
  public static void AddQuestionDes(FlatBufferBuilder builder, StringOffset QuestionDesOffset) { builder.AddOffset(3, QuestionDesOffset.Value, 0); }
  public static void AddAnswerIndex(FlatBufferBuilder builder, int AnswerIndex) { builder.AddInt(4, AnswerIndex, 0); }
  public static void AddAnswerDes(FlatBufferBuilder builder, StringOffset AnswerDesOffset) { builder.AddOffset(5, AnswerDesOffset.Value, 0); }
  public static Offset<TAPQuestionnaireTable> EndTAPQuestionnaireTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<TAPQuestionnaireTable>(o);
  }
  public static void FinishTAPQuestionnaireTableBuffer(FlatBufferBuilder builder, Offset<TAPQuestionnaireTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
