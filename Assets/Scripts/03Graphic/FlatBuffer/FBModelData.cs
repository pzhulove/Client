// automatically generated, do not modify

namespace FBModelData
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

public sealed class Vector4 : Struct {
  public Vector4 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float X { get { return bb.GetFloat(bb_pos + 0); } }
  public float Y { get { return bb.GetFloat(bb_pos + 4); } }
  public float Z { get { return bb.GetFloat(bb_pos + 8); } }
  public float W { get { return bb.GetFloat(bb_pos + 12); } }

  public static Offset<Vector4> CreateVector4(FlatBufferBuilder builder, float X, float Y, float Z, float W) {
    builder.Prep(4, 16);
    builder.PutFloat(W);
    builder.PutFloat(Z);
    builder.PutFloat(Y);
    builder.PutFloat(X);
    return new Offset<Vector4>(builder.Offset);
  }
};

public sealed class DModelPartChunk : Table {
  public static DModelPartChunk GetRootAsDModelPartChunk(ByteBuffer _bb) { return GetRootAsDModelPartChunk(_bb, new DModelPartChunk()); }
  public static DModelPartChunk GetRootAsDModelPartChunk(ByteBuffer _bb, DModelPartChunk obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DModelPartChunk __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string PartAsset { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int PartChannel { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<DModelPartChunk> CreateDModelPartChunk(FlatBufferBuilder builder,
      StringOffset partAsset = default(StringOffset),
      int partChannel = 0) {
    builder.StartObject(2);
    DModelPartChunk.AddPartChannel(builder, partChannel);
    DModelPartChunk.AddPartAsset(builder, partAsset);
    return DModelPartChunk.EndDModelPartChunk(builder);
  }

  public static void StartDModelPartChunk(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddPartAsset(FlatBufferBuilder builder, StringOffset partAssetOffset) { builder.AddOffset(0, partAssetOffset.Value, 0); }
  public static void AddPartChannel(FlatBufferBuilder builder, int partChannel) { builder.AddInt(1, partChannel, 0); }
  public static Offset<DModelPartChunk> EndDModelPartChunk(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DModelPartChunk>(o);
  }
};

public sealed class DModelAttachment : Table {
  public static DModelAttachment GetRootAsDModelAttachment(ByteBuffer _bb) { return GetRootAsDModelAttachment(_bb, new DModelAttachment()); }
  public static DModelAttachment GetRootAsDModelAttachment(ByteBuffer _bb, DModelAttachment obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DModelAttachment __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string AttahcmentAsset { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }

  public static Offset<DModelAttachment> CreateDModelAttachment(FlatBufferBuilder builder,
      StringOffset attahcmentAsset = default(StringOffset)) {
    builder.StartObject(1);
    DModelAttachment.AddAttahcmentAsset(builder, attahcmentAsset);
    return DModelAttachment.EndDModelAttachment(builder);
  }

  public static void StartDModelAttachment(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddAttahcmentAsset(FlatBufferBuilder builder, StringOffset attahcmentAssetOffset) { builder.AddOffset(0, attahcmentAssetOffset.Value, 0); }
  public static Offset<DModelAttachment> EndDModelAttachment(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DModelAttachment>(o);
  }
};

public sealed class DModelAttachmentChunk : Table {
  public static DModelAttachmentChunk GetRootAsDModelAttachmentChunk(ByteBuffer _bb) { return GetRootAsDModelAttachmentChunk(_bb, new DModelAttachmentChunk()); }
  public static DModelAttachmentChunk GetRootAsDModelAttachmentChunk(ByteBuffer _bb, DModelAttachmentChunk obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DModelAttachmentChunk __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public DModelAttachment GetAttachments(int j) { return GetAttachments(new DModelAttachment(), j); }
  public DModelAttachment GetAttachments(DModelAttachment obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AttachmentsLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<DModelAttachmentChunk> CreateDModelAttachmentChunk(FlatBufferBuilder builder,
      VectorOffset attachments = default(VectorOffset)) {
    builder.StartObject(1);
    DModelAttachmentChunk.AddAttachments(builder, attachments);
    return DModelAttachmentChunk.EndDModelAttachmentChunk(builder);
  }

  public static void StartDModelAttachmentChunk(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddAttachments(FlatBufferBuilder builder, VectorOffset attachmentsOffset) { builder.AddOffset(0, attachmentsOffset.Value, 0); }
  public static VectorOffset CreateAttachmentsVector(FlatBufferBuilder builder, Offset<DModelAttachment>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAttachmentsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<DModelAttachmentChunk> EndDModelAttachmentChunk(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DModelAttachmentChunk>(o);
  }
};

public sealed class DModelAnimClipChunk : Struct {
  public DModelAnimClipChunk __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }


  public static Offset<DModelAnimClipChunk> CreateDModelAnimClipChunk(FlatBufferBuilder builder) {
    builder.Prep(1, 0);
    return new Offset<DModelAnimClipChunk>(builder.Offset);
  }
};

public sealed class DAnimatParamData : Table {
  public static DAnimatParamData GetRootAsDAnimatParamData(ByteBuffer _bb) { return GetRootAsDAnimatParamData(_bb, new DAnimatParamData()); }
  public static DAnimatParamData GetRootAsDAnimatParamData(ByteBuffer _bb, DAnimatParamData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DAnimatParamData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float _float { get { int o = __offset(4); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public Color _color { get { return Get_color(new Color()); } }
  public Color Get_color(Color obj) { int o = __offset(6); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector4 _vec4 { get { return Get_vec4(new Vector4()); } }
  public Vector4 Get_vec4(Vector4 obj) { int o = __offset(8); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartDAnimatParamData(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void Add_float(FlatBufferBuilder builder, float Float) { builder.AddFloat(0, Float, 0); }
  public static void Add_color(FlatBufferBuilder builder, Offset<Color> ColorOffset) { builder.AddStruct(1, ColorOffset.Value, 0); }
  public static void Add_vec4(FlatBufferBuilder builder, Offset<Vector4> Vec4Offset) { builder.AddStruct(2, Vec4Offset.Value, 0); }
  public static Offset<DAnimatParamData> EndDAnimatParamData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DAnimatParamData>(o);
  }
};

public sealed class DAnimatParamObj : Table {
  public static DAnimatParamObj GetRootAsDAnimatParamObj(ByteBuffer _bb) { return GetRootAsDAnimatParamObj(_bb, new DAnimatParamObj()); }
  public static DAnimatParamObj GetRootAsDAnimatParamObj(ByteBuffer _bb, DAnimatParamObj obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DAnimatParamObj __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string _texAsset { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }

  public static Offset<DAnimatParamObj> CreateDAnimatParamObj(FlatBufferBuilder builder,
      StringOffset _texAsset = default(StringOffset)) {
    builder.StartObject(1);
    DAnimatParamObj.Add_texAsset(builder, _texAsset);
    return DAnimatParamObj.EndDAnimatParamObj(builder);
  }

  public static void StartDAnimatParamObj(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void Add_texAsset(FlatBufferBuilder builder, StringOffset TexAssetOffset) { builder.AddOffset(0, TexAssetOffset.Value, 0); }
  public static Offset<DAnimatParamObj> EndDAnimatParamObj(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DAnimatParamObj>(o);
  }
};

public sealed class DAnimatParamDesc : Table {
  public static DAnimatParamDesc GetRootAsDAnimatParamDesc(ByteBuffer _bb) { return GetRootAsDAnimatParamDesc(_bb, new DAnimatParamDesc()); }
  public static DAnimatParamDesc GetRootAsDAnimatParamDesc(ByteBuffer _bb, DAnimatParamDesc obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DAnimatParamDesc __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string ParamName { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public DAnimatParamData ParamData { get { return GetParamData(new DAnimatParamData()); } }
  public DAnimatParamData GetParamData(DAnimatParamData obj) { int o = __offset(6); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public DAnimatParamObj ParamObj { get { return GetParamObj(new DAnimatParamObj()); } }
  public DAnimatParamObj GetParamObj(DAnimatParamObj obj) { int o = __offset(8); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public int ParamType { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<DAnimatParamDesc> CreateDAnimatParamDesc(FlatBufferBuilder builder,
      StringOffset paramName = default(StringOffset),
      Offset<DAnimatParamData> paramData = default(Offset<DAnimatParamData>),
      Offset<DAnimatParamObj> paramObj = default(Offset<DAnimatParamObj>),
      int paramType = 0) {
    builder.StartObject(4);
    DAnimatParamDesc.AddParamType(builder, paramType);
    DAnimatParamDesc.AddParamObj(builder, paramObj);
    DAnimatParamDesc.AddParamData(builder, paramData);
    DAnimatParamDesc.AddParamName(builder, paramName);
    return DAnimatParamDesc.EndDAnimatParamDesc(builder);
  }

  public static void StartDAnimatParamDesc(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddParamName(FlatBufferBuilder builder, StringOffset paramNameOffset) { builder.AddOffset(0, paramNameOffset.Value, 0); }
  public static void AddParamData(FlatBufferBuilder builder, Offset<DAnimatParamData> paramDataOffset) { builder.AddOffset(1, paramDataOffset.Value, 0); }
  public static void AddParamObj(FlatBufferBuilder builder, Offset<DAnimatParamObj> paramObjOffset) { builder.AddOffset(2, paramObjOffset.Value, 0); }
  public static void AddParamType(FlatBufferBuilder builder, int paramType) { builder.AddInt(3, paramType, 0); }
  public static Offset<DAnimatParamDesc> EndDAnimatParamDesc(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DAnimatParamDesc>(o);
  }
};

public sealed class DAnimatChunk : Table {
  public static DAnimatChunk GetRootAsDAnimatChunk(ByteBuffer _bb) { return GetRootAsDAnimatChunk(_bb, new DAnimatChunk()); }
  public static DAnimatChunk GetRootAsDAnimatChunk(ByteBuffer _bb, DAnimatChunk obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DAnimatChunk __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public string ShaderName { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public DAnimatParamDesc GetParamDesc(int j) { return GetParamDesc(new DAnimatParamDesc(), j); }
  public DAnimatParamDesc GetParamDesc(DAnimatParamDesc obj, int j) { int o = __offset(8); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ParamDescLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<DAnimatChunk> CreateDAnimatChunk(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      StringOffset shaderName = default(StringOffset),
      VectorOffset paramDesc = default(VectorOffset)) {
    builder.StartObject(3);
    DAnimatChunk.AddParamDesc(builder, paramDesc);
    DAnimatChunk.AddShaderName(builder, shaderName);
    DAnimatChunk.AddName(builder, name);
    return DAnimatChunk.EndDAnimatChunk(builder);
  }

  public static void StartDAnimatChunk(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddShaderName(FlatBufferBuilder builder, StringOffset shaderNameOffset) { builder.AddOffset(1, shaderNameOffset.Value, 0); }
  public static void AddParamDesc(FlatBufferBuilder builder, VectorOffset paramDescOffset) { builder.AddOffset(2, paramDescOffset.Value, 0); }
  public static VectorOffset CreateParamDescVector(FlatBufferBuilder builder, Offset<DAnimatParamDesc>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartParamDescVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<DAnimatChunk> EndDAnimatChunk(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DAnimatChunk>(o);
  }
};

public sealed class DBlockChunk : Table {
  public static DBlockChunk GetRootAsDBlockChunk(ByteBuffer _bb) { return GetRootAsDBlockChunk(_bb, new DBlockChunk()); }
  public static DBlockChunk GetRootAsDBlockChunk(ByteBuffer _bb, DBlockChunk obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DBlockChunk __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int GridWidth { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GridHeight { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public byte GetGridBlockData(int j) { int o = __offset(8); return o != 0 ? bb.Get(__vector(o) + j * 1) : (byte)0; }
  public int GridBlockDataLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }
  public Vector3 BoundingBoxMin { get { return GetBoundingBoxMin(new Vector3()); } }
  public Vector3 GetBoundingBoxMin(Vector3 obj) { int o = __offset(10); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 BoundingBoxMax { get { return GetBoundingBoxMax(new Vector3()); } }
  public Vector3 GetBoundingBoxMax(Vector3 obj) { int o = __offset(12); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartDBlockChunk(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddGridWidth(FlatBufferBuilder builder, int gridWidth) { builder.AddInt(0, gridWidth, 0); }
  public static void AddGridHeight(FlatBufferBuilder builder, int gridHeight) { builder.AddInt(1, gridHeight, 0); }
  public static void AddGridBlockData(FlatBufferBuilder builder, VectorOffset gridBlockDataOffset) { builder.AddOffset(2, gridBlockDataOffset.Value, 0); }
  public static VectorOffset CreateGridBlockDataVector(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); for (int i = data.Length - 1; i >= 0; i--) builder.AddByte(data[i]); return builder.EndVector(); }
  public static void StartGridBlockDataVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
  public static void AddBoundingBoxMin(FlatBufferBuilder builder, Offset<Vector3> boundingBoxMinOffset) { builder.AddStruct(3, boundingBoxMinOffset.Value, 0); }
  public static void AddBoundingBoxMax(FlatBufferBuilder builder, Offset<Vector3> boundingBoxMaxOffset) { builder.AddStruct(4, boundingBoxMaxOffset.Value, 0); }
  public static Offset<DBlockChunk> EndDBlockChunk(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DBlockChunk>(o);
  }
};

public sealed class DModelData : Table {
  public static DModelData GetRootAsDModelData(ByteBuffer _bb) { return GetRootAsDModelData(_bb, new DModelData()); }
  public static DModelData GetRootAsDModelData(ByteBuffer _bb, DModelData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DModelData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string ModelDataName { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public string ModelAvatar { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public Vector3 ModelScale { get { return GetModelScale(new Vector3()); } }
  public Vector3 GetModelScale(Vector3 obj) { int o = __offset(8); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 PreviewLightDir { get { return GetPreviewLightDir(new Vector3()); } }
  public Vector3 GetPreviewLightDir(Vector3 obj) { int o = __offset(10); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Color PreviewAmbient { get { return GetPreviewAmbient(new Color()); } }
  public Color GetPreviewAmbient(Color obj) { int o = __offset(12); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public DModelPartChunk GetPartsChunk(int j) { return GetPartsChunk(new DModelPartChunk(), j); }
  public DModelPartChunk GetPartsChunk(DModelPartChunk obj, int j) { int o = __offset(14); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int PartsChunkLength { get { int o = __offset(14); return o != 0 ? __vector_len(o) : 0; } }
  public DModelAttachmentChunk AttachChunk { get { return GetAttachChunk(new DModelAttachmentChunk()); } }
  public DModelAttachmentChunk GetAttachChunk(DModelAttachmentChunk obj) { int o = __offset(16); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public DModelAnimClipChunk AnimClipChunk { get { return GetAnimClipChunk(new DModelAnimClipChunk()); } }
  public DModelAnimClipChunk GetAnimClipChunk(DModelAnimClipChunk obj) { int o = __offset(18); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public DAnimatChunk GetAnimatChunk(int j) { return GetAnimatChunk(new DAnimatChunk(), j); }
  public DAnimatChunk GetAnimatChunk(DAnimatChunk obj, int j) { int o = __offset(20); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AnimatChunkLength { get { int o = __offset(20); return o != 0 ? __vector_len(o) : 0; } }
  public DBlockChunk BlockGridChunk { get { return GetBlockGridChunk(new DBlockChunk()); } }
  public DBlockChunk GetBlockGridChunk(DBlockChunk obj) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }

  public static void StartDModelData(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddModelDataName(FlatBufferBuilder builder, StringOffset modelDataNameOffset) { builder.AddOffset(0, modelDataNameOffset.Value, 0); }
  public static void AddModelAvatar(FlatBufferBuilder builder, StringOffset modelAvatarOffset) { builder.AddOffset(1, modelAvatarOffset.Value, 0); }
  public static void AddModelScale(FlatBufferBuilder builder, Offset<Vector3> modelScaleOffset) { builder.AddStruct(2, modelScaleOffset.Value, 0); }
  public static void AddPreviewLightDir(FlatBufferBuilder builder, Offset<Vector3> previewLightDirOffset) { builder.AddStruct(3, previewLightDirOffset.Value, 0); }
  public static void AddPreviewAmbient(FlatBufferBuilder builder, Offset<Color> previewAmbientOffset) { builder.AddStruct(4, previewAmbientOffset.Value, 0); }
  public static void AddPartsChunk(FlatBufferBuilder builder, VectorOffset partsChunkOffset) { builder.AddOffset(5, partsChunkOffset.Value, 0); }
  public static VectorOffset CreatePartsChunkVector(FlatBufferBuilder builder, Offset<DModelPartChunk>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartPartsChunkVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAttachChunk(FlatBufferBuilder builder, Offset<DModelAttachmentChunk> attachChunkOffset) { builder.AddOffset(6, attachChunkOffset.Value, 0); }
  public static void AddAnimClipChunk(FlatBufferBuilder builder, Offset<DModelAnimClipChunk> animClipChunkOffset) { builder.AddStruct(7, animClipChunkOffset.Value, 0); }
  public static void AddAnimatChunk(FlatBufferBuilder builder, VectorOffset animatChunkOffset) { builder.AddOffset(8, animatChunkOffset.Value, 0); }
  public static VectorOffset CreateAnimatChunkVector(FlatBufferBuilder builder, Offset<DAnimatChunk>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAnimatChunkVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBlockGridChunk(FlatBufferBuilder builder, Offset<DBlockChunk> blockGridChunkOffset) { builder.AddOffset(9, blockGridChunkOffset.Value, 0); }
  public static Offset<DModelData> EndDModelData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DModelData>(o);
  }
};


}

