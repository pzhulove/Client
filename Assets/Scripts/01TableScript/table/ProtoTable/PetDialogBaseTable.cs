// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class PetDialogBaseTable : IFlatbufferObject
{
public enum eFilterType : int
{
 Invalid = 0,
 Random = 1,
};

public enum eCrypt : int
{
 code = 1391162910,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static PetDialogBaseTable GetRootAsPetDialogBaseTable(ByteBuffer _bb) { return GetRootAsPetDialogBaseTable(_bb, new PetDialogBaseTable()); }
  public static PetDialogBaseTable GetRootAsPetDialogBaseTable(ByteBuffer _bb, PetDialogBaseTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public PetDialogBaseTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public string Desc { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : FlatBufferConstants.DefaultString; } }
  public ArraySegment<byte>? GetDescBytes() { return __p.__vector_as_arraysegment(6); }
  public ProtoTable.PetDialogBaseTable.eFilterType FilterType { get { int o = __p.__offset(8); return o != 0 ? (ProtoTable.PetDialogBaseTable.eFilterType)__p.bb.GetInt(o + __p.bb_pos) : ProtoTable.PetDialogBaseTable.eFilterType.Invalid; } }
  public int DialogIDsArray(int j) { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int DialogIDsLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }
  public ArraySegment<byte>? GetDialogIDsBytes() { return __p.__vector_as_arraysegment(10); }
 private FlatBufferArray<int> DialogIDsValue;
 public FlatBufferArray<int>  DialogIDs
 {
  get{
  if (DialogIDsValue == null)
  {
    DialogIDsValue = new FlatBufferArray<int>(this.DialogIDsArray, this.DialogIDsLength);
  }
  return DialogIDsValue;}
 }

  public static Offset<PetDialogBaseTable> CreatePetDialogBaseTable(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset DescOffset = default(StringOffset),
      ProtoTable.PetDialogBaseTable.eFilterType FilterType = ProtoTable.PetDialogBaseTable.eFilterType.Invalid,
      VectorOffset DialogIDsOffset = default(VectorOffset)) {
    builder.StartObject(4);
    PetDialogBaseTable.AddDialogIDs(builder, DialogIDsOffset);
    PetDialogBaseTable.AddFilterType(builder, FilterType);
    PetDialogBaseTable.AddDesc(builder, DescOffset);
    PetDialogBaseTable.AddID(builder, ID);
    return PetDialogBaseTable.EndPetDialogBaseTable(builder);
  }

  public static void StartPetDialogBaseTable(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset DescOffset) { builder.AddOffset(1, DescOffset.Value, 0); }
  public static void AddFilterType(FlatBufferBuilder builder, ProtoTable.PetDialogBaseTable.eFilterType FilterType) { builder.AddInt(2, (int)FilterType, 0); }
  public static void AddDialogIDs(FlatBufferBuilder builder, VectorOffset DialogIDsOffset) { builder.AddOffset(3, DialogIDsOffset.Value, 0); }
  public static VectorOffset CreateDialogIDsVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartDialogIDsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<PetDialogBaseTable> EndPetDialogBaseTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<PetDialogBaseTable>(o);
  }
  public static void FinishPetDialogBaseTableBuffer(FlatBufferBuilder builder, Offset<PetDialogBaseTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
