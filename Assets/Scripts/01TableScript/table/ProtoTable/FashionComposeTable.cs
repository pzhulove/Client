// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

#if USE_FB_TABLE

namespace ProtoTable
{

using global::System;
using global::FlatBuffers;

public class FashionComposeTable : IFlatbufferObject
{
public enum eCrypt : int
{
 code = -263444806,
};

  private Table __p = new Table();
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static FashionComposeTable GetRootAsFashionComposeTable(ByteBuffer _bb) { return GetRootAsFashionComposeTable(_bb, new FashionComposeTable()); }
  public static FashionComposeTable GetRootAsFashionComposeTable(ByteBuffer _bb, FashionComposeTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public FashionComposeTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Occu { get { int o = __p.__offset(6); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int SuitID { get { int o = __p.__offset(8); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Color { get { int o = __p.__offset(10); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Part { get { int o = __p.__offset(12); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int ComposeColor { get { int o = __p.__offset(14); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int Weight { get { int o = __p.__offset(16); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int AvatarType { get { int o = __p.__offset(18); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }
  public int DefaultAvatar { get { int o = __p.__offset(20); return o != 0 ? (int)eCrypt.code^__p.bb.GetInt(o + __p.bb_pos) : 0; } }

  public static Offset<FashionComposeTable> CreateFashionComposeTable(FlatBufferBuilder builder,
      int ID = 0,
      int Occu = 0,
      int SuitID = 0,
      int Color = 0,
      int Part = 0,
      int ComposeColor = 0,
      int Weight = 0,
      int AvatarType = 0,
      int DefaultAvatar = 0) {
    builder.StartObject(9);
    FashionComposeTable.AddDefaultAvatar(builder, DefaultAvatar);
    FashionComposeTable.AddAvatarType(builder, AvatarType);
    FashionComposeTable.AddWeight(builder, Weight);
    FashionComposeTable.AddComposeColor(builder, ComposeColor);
    FashionComposeTable.AddPart(builder, Part);
    FashionComposeTable.AddColor(builder, Color);
    FashionComposeTable.AddSuitID(builder, SuitID);
    FashionComposeTable.AddOccu(builder, Occu);
    FashionComposeTable.AddID(builder, ID);
    return FashionComposeTable.EndFashionComposeTable(builder);
  }

  public static void StartFashionComposeTable(FlatBufferBuilder builder) { builder.StartObject(9); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddOccu(FlatBufferBuilder builder, int Occu) { builder.AddInt(1, Occu, 0); }
  public static void AddSuitID(FlatBufferBuilder builder, int SuitID) { builder.AddInt(2, SuitID, 0); }
  public static void AddColor(FlatBufferBuilder builder, int Color) { builder.AddInt(3, Color, 0); }
  public static void AddPart(FlatBufferBuilder builder, int Part) { builder.AddInt(4, Part, 0); }
  public static void AddComposeColor(FlatBufferBuilder builder, int ComposeColor) { builder.AddInt(5, ComposeColor, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int Weight) { builder.AddInt(6, Weight, 0); }
  public static void AddAvatarType(FlatBufferBuilder builder, int AvatarType) { builder.AddInt(7, AvatarType, 0); }
  public static void AddDefaultAvatar(FlatBufferBuilder builder, int DefaultAvatar) { builder.AddInt(8, DefaultAvatar, 0); }
  public static Offset<FashionComposeTable> EndFashionComposeTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FashionComposeTable>(o);
  }
  public static void FinishFashionComposeTableBuffer(FlatBufferBuilder builder, Offset<FashionComposeTable> offset) { builder.Finish(offset.Value); }
};


}


#endif
