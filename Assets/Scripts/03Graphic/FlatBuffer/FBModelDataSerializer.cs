using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;

using FlatBuffers;


public class FBModelDataSerializer
{
    private static StringOffset ToFBString(FlatBufferBuilder builder, string value)
    {
        if (string.IsNullOrEmpty(value) == false)
        {
            return builder.CreateString(value);
        }

        return builder.CreateString(string.Empty);
    }

    public static void SaveFBModelData(
        string FBDataPath,
        DModelData dataObj)
    {
        FlatBuffers.FlatBufferBuilder builder = new FlatBuffers.FlatBufferBuilder(1);

        StringOffset modelDataNameOff = ToFBString(builder, dataObj.modelDataName);
        StringOffset modelAvatarOff = ToFBString(builder, dataObj.modelAvatar.m_AssetPath);

        VectorOffset partChunkOff = default(VectorOffset);
        if (dataObj.partsChunk.Length > 0)
        {
            StringOffset[] modelPartAssetOff = new StringOffset[dataObj.partsChunk.Length];
            Offset<FBModelData.DModelPartChunk>[] modelPartChunk = new Offset<FBModelData.DModelPartChunk>[dataObj.partsChunk.Length];
            for (int i = 0, icnt = dataObj.partsChunk.Length; i < icnt; ++i)
            {
                DModelPartChunk curChunk = dataObj.partsChunk[i];
                modelPartAssetOff[i] = ToFBString(builder, curChunk.partAsset.m_AssetPath);
            }
            
            for (int i = 0, icnt = dataObj.partsChunk.Length; i < icnt; ++i)
            {
                DModelPartChunk curChunk = dataObj.partsChunk[i];
                modelPartChunk[i] = FBModelData.DModelPartChunk.CreateDModelPartChunk(
                    builder, modelPartAssetOff[i], (int)curChunk.partChannel);
            }

            partChunkOff = FBModelData.DModelData.CreatePartsChunkVector(builder, modelPartChunk);
        }
        
        StringOffset[] modelAttachmentAssetOff = new StringOffset[0];
        if (null != dataObj.attachChunk.attachments && dataObj.attachChunk.attachments.Length > 0)
        {
            modelAttachmentAssetOff = new StringOffset[dataObj.attachChunk.attachments.Length];
            for (int i = 0, icnt = dataObj.attachChunk.attachments.Length; i < icnt; ++i)
            {
                DModelAttachment curAttach = dataObj.attachChunk.attachments[i];
                modelAttachmentAssetOff[i] =  ToFBString(builder, curAttach.attahcmentAsset.m_AssetPath);
            }
        }

        VectorOffset animatOffset = default(VectorOffset);
        if (null != dataObj.animatChunk && dataObj.animatChunk.Length > 0)
        {
            Offset<FBModelData.DAnimatChunk>[] modelAnimatChunk = new Offset<FBModelData.DAnimatChunk>[dataObj.animatChunk.Length];
            for (int i = 0, icnt = dataObj.animatChunk.Length; i < icnt; ++i)
            {
                DAnimatChunk curChunk = dataObj.animatChunk[i];
                VectorOffset curVecOff = default(VectorOffset);
                if (null != curChunk.paramDesc && curChunk.paramDesc.Length > 0)
                {
                    Offset<FBModelData.DAnimatParamDesc>[] modelAnimatParamChunk = new Offset<FBModelData.DAnimatParamDesc>[curChunk.paramDesc.Length];
                    for (int j = 0, jcnt = curChunk.paramDesc.Length; j < jcnt; ++j)
                    {
                        DAnimatParamDesc curAnimParamDesc = curChunk.paramDesc[j];
                        Offset<FBModelData.DAnimatParamObj> animParamObjOffset = FBModelData.DAnimatParamObj.CreateDAnimatParamObj(
                            builder, ToFBString(builder, curAnimParamDesc.paramObj._texAsset.m_AssetPath));

                        float curFloat = 0.0f;
                        Color curColor = Color.white;
                        Vector4 curVec4 = Vector4.zero;

                        switch (curAnimParamDesc.paramType)
                        {
                            case AnimatParamType.Float:
                                curFloat = curAnimParamDesc.paramData._float;
                                break;
                            case AnimatParamType.Color:
                                curColor = curAnimParamDesc.paramData._color;
                                break;
                            case AnimatParamType.Vector:
                                curVec4 = curAnimParamDesc.paramData._vec4;
                                break;
                            default:
                                break;
                        }

                        FBModelData.DAnimatParamData.StartDAnimatParamData(builder);
                        FBModelData.DAnimatParamData.Add_float(builder, curFloat);
                        FBModelData.DAnimatParamData.Add_color(builder, FBModelData.Color.CreateColor(builder,
                             curColor.a, curColor.b, curColor.g, curColor.r));
                        FBModelData.DAnimatParamData.Add_vec4(builder, FBModelData.Vector4.CreateVector4(builder,
                             curVec4.x, curVec4.y, curVec4.z, curVec4.w));
                        Offset<FBModelData.DAnimatParamData> animParamData = FBModelData.DAnimatParamData.EndDAnimatParamData(builder);

                        modelAnimatParamChunk[j] = FBModelData.DAnimatParamDesc.CreateDAnimatParamDesc(
                            builder, ToFBString(builder, curAnimParamDesc.paramName), animParamData, animParamObjOffset, (int)curAnimParamDesc.paramType);
                    }

                    curVecOff = FBModelData.DAnimatChunk.CreateParamDescVector(builder, modelAnimatParamChunk);
                }

                modelAnimatChunk[i] = FBModelData.DAnimatChunk.CreateDAnimatChunk(
                    builder, ToFBString(builder, curChunk.name), ToFBString(builder, curChunk.shaderName), curVecOff);
            }

            animatOffset = FBModelData.DModelData.CreateAnimatChunkVector(builder, modelAnimatChunk);
        }

        VectorOffset blockBytesOffset = default(VectorOffset);
        if (null != dataObj.blockGridChunk.gridBlockData && dataObj.blockGridChunk.gridBlockData.Length > 0)
            blockBytesOffset = FBModelData.DBlockChunk.CreateGridBlockDataVector(builder, dataObj.blockGridChunk.gridBlockData);

        FBModelData.DBlockChunk.StartDBlockChunk(builder);
        FBModelData.DBlockChunk.AddBoundingBoxMin(builder, FBModelData.Vector3.CreateVector3(builder, dataObj.blockGridChunk.boundingBoxMin.x, dataObj.blockGridChunk.boundingBoxMin.y, dataObj.blockGridChunk.boundingBoxMin.z));
        FBModelData.DBlockChunk.AddBoundingBoxMax(builder, FBModelData.Vector3.CreateVector3(builder, dataObj.blockGridChunk.boundingBoxMax.x, dataObj.blockGridChunk.boundingBoxMax.y, dataObj.blockGridChunk.boundingBoxMax.z));

        FBModelData.DBlockChunk.AddGridHeight(builder, dataObj.blockGridChunk.gridHeight);
        FBModelData.DBlockChunk.AddGridWidth(builder, dataObj.blockGridChunk.gridWidth);

        FBModelData.DBlockChunk.AddGridBlockData(builder, blockBytesOffset);

        Offset <FBModelData.DBlockChunk> blockOffset = FBModelData.DBlockChunk.EndDBlockChunk(builder);

        FBModelData.DModelData.StartDModelData(builder);
        FBModelData.DModelData.AddModelDataName(builder, modelDataNameOff);
        FBModelData.DModelData.AddModelAvatar(builder, modelAvatarOff);
        FBModelData.DModelData.AddModelScale(builder, FBModelData.Vector3.CreateVector3(builder, dataObj.modelScale.x, dataObj.modelScale.y, dataObj.modelScale.z));
        FBModelData.DModelData.AddPreviewLightDir(builder, FBModelData.Vector3.CreateVector3(builder, dataObj.previewLightDir.x, dataObj.previewLightDir.y, dataObj.previewLightDir.z));
        FBModelData.DModelData.AddPreviewAmbient(builder, FBModelData.Color.CreateColor(builder, dataObj.previewAmbient.a, dataObj.previewAmbient.b, dataObj.previewAmbient.g, dataObj.previewAmbient.r));
        FBModelData.DModelData.AddPartsChunk(builder, partChunkOff);
        
        if (null != dataObj.attachChunk.attachments && dataObj.attachChunk.attachments.Length > 0 && modelAttachmentAssetOff.Length > 0)
        {
            Offset<FBModelData.DModelAttachment>[] modelAttachChunk = new Offset<FBModelData.DModelAttachment>[dataObj.attachChunk.attachments.Length];
            for (int i = 0, icnt = dataObj.attachChunk.attachments.Length; i < icnt; ++i)
            {
                DModelAttachment curAttach = dataObj.attachChunk.attachments[i];
                modelAttachChunk[i] = FBModelData.DModelAttachment.CreateDModelAttachment(builder,
                    modelAttachmentAssetOff[i]);
            }

            VectorOffset curVecOff = FBModelData.DModelAttachmentChunk.CreateAttachmentsVector(builder, modelAttachChunk);
            FBModelData.DModelData.AddAttachChunk(builder,
                FBModelData.DModelAttachmentChunk.CreateDModelAttachmentChunk(builder, curVecOff));
        }

        FBModelData.DModelData.AddAnimatChunk(builder,animatOffset);
        FBModelData.DModelData.AddBlockGridChunk(builder, blockOffset);
        Offset<FBModelData.DModelData> modelDataOffset = FBModelData.DModelData.EndDModelData(builder);

        builder.Finish(modelDataOffset.Value, "MDLD");

        using (var ms = new MemoryStream(builder.DataBuffer.Data, builder.DataBuffer.Position, builder.Offset))
            File.WriteAllBytes(FBDataPath, ms.ToArray());

    }

    public static void LoadFBModelData(string FBDataPath,out DModelData modelDataAsset)
    {
        modelDataAsset = null;
        FBDataPath = FBDataPath.ToLower();

        if (!File.Exists(FBDataPath))
        {
            modelDataAsset = null;
            return;
        }

        byte[] newPathData = System.IO.File.ReadAllBytes(FBDataPath);

#if LOGIC_SERVER
        modelDataAsset = new DModelData();
#else
        modelDataAsset = ScriptableObject.CreateInstance<DModelData>();
#endif

        if (null != modelDataAsset)
        {
            FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(newPathData);
            FBModelData.DModelData modeldata = FBModelData.DModelData.GetRootAsDModelData(buffer);
            if (null != modeldata)
            {
                modelDataAsset.modelDataName = modeldata.ModelDataName;
                modelDataAsset.modelAvatar.m_AssetPath = modeldata.ModelAvatar;
                modelDataAsset.modelAvatar.m_AssetObj = null;

                modelDataAsset.modelScale = new Vector3(modeldata.ModelScale.X, modeldata.ModelScale.Y, modeldata.ModelScale.Z);
                modelDataAsset.previewLightDir = new Vector3(modeldata.PreviewLightDir.X, modeldata.PreviewLightDir.Y, modeldata.PreviewLightDir.Z);
                modelDataAsset.previewAmbient = new Color(modeldata.PreviewAmbient.R, modeldata.PreviewAmbient.G, modeldata.PreviewAmbient.B, modeldata.PreviewAmbient.A);

                int chunkCnt = modeldata.PartsChunkLength;
                if(chunkCnt > 0)
                {
                    modelDataAsset.partsChunk = new DModelPartChunk[chunkCnt];
                    for(int i = 0,icnt = chunkCnt;i<icnt;++i)
                    {
                        FBModelData.DModelPartChunk cur = modeldata.GetPartsChunk(i);

                        modelDataAsset.partsChunk[i].partAsset.m_AssetPath = cur.PartAsset;
                        modelDataAsset.partsChunk[i].partAsset.m_AssetObj = null;
                        modelDataAsset.partsChunk[i].partChannel = (EModelPartChannel)cur.PartChannel;
                    }
                }
                else
                    modelDataAsset.partsChunk = new DModelPartChunk[0];

                if (null != modeldata.AttachChunk && modeldata.AttachChunk.AttachmentsLength > 0)
                {
                    modelDataAsset.attachChunk.attachments = new DModelAttachment[modeldata.AttachChunk.AttachmentsLength];
                    for (int i = 0, icnt = modelDataAsset.attachChunk.attachments.Length; i < icnt; ++i)
                    {
                        FBModelData.DModelAttachment cur = modeldata.AttachChunk.GetAttachments(i);

                        modelDataAsset.attachChunk.attachments[i].attahcmentAsset.m_AssetPath = cur.AttahcmentAsset;
                        modelDataAsset.attachChunk.attachments[i].attahcmentAsset.m_AssetObj = null;
                    }
                }
                else
                    modelDataAsset.attachChunk.attachments = new DModelAttachment[0];

                if (modeldata.AnimatChunkLength > 0)
                {
                    modelDataAsset.animatChunk = new  DAnimatChunk[modeldata.AnimatChunkLength];
                    for (int i = 0, icnt = modelDataAsset.animatChunk.Length; i < icnt; ++i)
                    {
                        FBModelData.DAnimatChunk cur = modeldata.GetAnimatChunk(i);

                        modelDataAsset.animatChunk[i].name = cur.Name;
                        modelDataAsset.animatChunk[i].shaderName = cur.ShaderName;

                        modelDataAsset.animatChunk[i].paramDesc = new DAnimatParamDesc[cur.ParamDescLength]; 
                        for(int j = 0,jcnt = cur.ParamDescLength;j<jcnt;++j)
                        {
                            FBModelData.DAnimatParamDesc data = cur.GetParamDesc(j);
                            modelDataAsset.animatChunk[i].paramDesc[j].paramType = (AnimatParamType) data.ParamType;
                            switch(modelDataAsset.animatChunk[i].paramDesc[j].paramType)
                            {
                                case AnimatParamType.Float:
                                    modelDataAsset.animatChunk[i].paramDesc[j].paramData._float = data.ParamData._float;
                                    break;
                                case AnimatParamType.Vector:
                                    modelDataAsset.animatChunk[i].paramDesc[j].paramData._vec4 = new Vector4(data.ParamData._vec4.X, data.ParamData._vec4.Y, data.ParamData._vec4.Z, data.ParamData._vec4.W);
                                    break;
                                case AnimatParamType.Color:
                                    modelDataAsset.animatChunk[i].paramDesc[j].paramData._color = new Color(data.ParamData._color.R, data.ParamData._color.G, data.ParamData._color.B, data.ParamData._color.A); 
                                    break;
                                default:
                                    break;
                            }
                            modelDataAsset.animatChunk[i].paramDesc[j].paramObj._texAsset.m_AssetPath = data.ParamObj._texAsset;
                            modelDataAsset.animatChunk[i].paramDesc[j].paramObj._texAsset.m_AssetObj = null;
                            modelDataAsset.animatChunk[i].paramDesc[j].paramName = data.ParamName;
                        }
                    }
                }
                else
                    modelDataAsset.attachChunk.attachments = new DModelAttachment[0];

                modelDataAsset.blockGridChunk.boundingBoxMin = new Vector3(modeldata.BlockGridChunk.BoundingBoxMin.X, modeldata.BlockGridChunk.BoundingBoxMin.Y, modeldata.BlockGridChunk.BoundingBoxMin.Z);
                modelDataAsset.blockGridChunk.boundingBoxMax = new Vector3(modeldata.BlockGridChunk.BoundingBoxMax.X, modeldata.BlockGridChunk.BoundingBoxMax.Y, modeldata.BlockGridChunk.BoundingBoxMax.Z);

                modelDataAsset.blockGridChunk.gridHeight = modeldata.BlockGridChunk.GridHeight;
                modelDataAsset.blockGridChunk.gridWidth = modeldata.BlockGridChunk.GridWidth;

                modelDataAsset.blockGridChunk.gridBlockData = new byte[modeldata.BlockGridChunk.GridBlockDataLength];
                for(int i = 0,icnt = modelDataAsset.blockGridChunk.gridBlockData.Length; i<icnt; ++i)
                    modelDataAsset.blockGridChunk.gridBlockData[i] = modeldata.BlockGridChunk.GetGridBlockData(i);
            }
        }
    }

    static public string GetBlockDataResPath(int resID)
    {
        string modelDataRes = "";
        var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
        if (null != resData)
        {
            string unifiedPath = Path.ChangeExtension(resData.ModelPath,null).Replace('\\', '/');
            string[] splitTbl = unifiedPath.Split('/');
            string Name = splitTbl[splitTbl.Length - 1];
            splitTbl[splitTbl.Length - 1] = Name + "_ModelData";

            modelDataRes = string.Join("/", splitTbl);
        }
        return modelDataRes;
    }
    
    static public string GetBlockDataResPathByActionName(int resID, string acName)
    {
        string modelDataRes = "";
        var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
        if (null != resData)
        {
            string unifiedPath = Path.ChangeExtension(resData.ModelPath,null).Replace('\\', '/');
            string[] splitTbl = unifiedPath.Split('/');
            string Name = splitTbl[splitTbl.Length - 1];
            splitTbl[splitTbl.Length - 1] = Name + "_ModelData" + "_" + acName;

            modelDataRes = string.Join("/", splitTbl);
        }
        return modelDataRes;
    }

    protected static readonly byte[] DEFAULT_BLOCK_DATA = new byte[] { 1 };
    static public byte[] GetFBBlockData(int resID, out int width, out int height)
    {
        width = 1;
        height = 1;

        string modelDataRes = GetBlockDataResPath(resID);
        if (!string.IsNullOrEmpty(modelDataRes))
        {
            DModelData modelData = null;
#if USE_FB
            FBModelDataSerializer.LoadFBModelData(Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(modelDataRes, Utility.kRawDataExtension)), out modelData);
#else
			modelData = AssetLoader.instance.LoadRes(modelDataRes, false).obj as DModelData;
#endif
            if(null != modelData)
            {
                width = modelData.blockGridChunk.gridWidth;
                height = modelData.blockGridChunk.gridHeight;
                return modelData.blockGridChunk.gridBlockData;
            }
        }
        
        return DEFAULT_BLOCK_DATA;
    }
}

