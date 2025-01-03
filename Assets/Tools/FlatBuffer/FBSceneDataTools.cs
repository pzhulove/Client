using System;
using FlatBuffers;
using System.Collections.Generic;
using UnityEngine;

public class FBSceneDataTools
{
    private static T Call<T>(Func<T> func)
    {
        return func();
    }

    private static StringOffset _createString(FlatBufferBuilder builder, string value)
    {
        if (null == value)
        {
            return default(StringOffset);
        }

        return builder.CreateString(value);
    }

    private static Offset<FBSceneData.Vector2> _createVec2(FlatBufferBuilder builder, Vector2 pos)
    {
        return FBSceneData.Vector2.CreateVector2(builder,
                pos.x, pos.y);
    }


    private static Offset<FBSceneData.Vector3> _createVec3(FlatBufferBuilder builder, Vector3 pos)
    {
        return FBSceneData.Vector3.CreateVector3(builder,
                pos.x, pos.y, pos.z);
    }

    private static Offset<FBSceneData.Color> _createColor(FlatBufferBuilder builder, Color color)
    {
        return FBSceneData.Color.CreateColor(builder, 
                color.r, color.g, color.b, color.a);
    }


    private static Offset<FBSceneData.DEntityInfo> _createFBEntityInfo(
            FlatBufferBuilder builder,
            DEntityInfo info)
    {
        if (null == info)
        {
            return default(Offset<FBSceneData.DEntityInfo>);
        }
        
        var name = _createString (builder, info.Name);
        var path = _createString(builder, info.path);
        var desc = _createString (builder, info.Description);
        var typename = _createString(builder, info.typename);


        FBSceneData.DEntityInfo.StartDEntityInfo(builder);
        FBSceneData.DEntityInfo.AddGlobalid(builder, info.globalid);
        FBSceneData.DEntityInfo.AddResid(builder, info.resid);
        FBSceneData.DEntityInfo.AddType(builder, (FBSceneData.DEntityType)info.type);
        FBSceneData.DEntityInfo.AddScale(builder, info.scale);
        FBSceneData.DEntityInfo.AddColor(builder, _createColor (builder, info.color));
        FBSceneData.DEntityInfo.AddPosition(builder, _createVec3 (builder, info.position));
		FBSceneData.DEntityInfo.AddName(builder, name);
        FBSceneData.DEntityInfo.AddPath(builder, path);
		FBSceneData.DEntityInfo.AddDescription(builder, desc);
        FBSceneData.DEntityInfo.AddTypeName(builder, typename);
        return FBSceneData.DEntityInfo.EndDEntityInfo(builder);
    }

    private static Offset<FBSceneData.DDecoratorInfo> _createFBDDecoratorInfo(
            FlatBufferBuilder builder,
            DDecoratorInfo info)
    {
        if (null == info)
        {
            return default(Offset<FBSceneData.DDecoratorInfo>);
        }

        var super =  _createFBEntityInfo(builder, info);

        FBSceneData.DDecoratorInfo.StartDDecoratorInfo(builder);
        FBSceneData.DDecoratorInfo.AddSuper(builder, super);
        FBSceneData.DDecoratorInfo.AddLocalScale(builder, _createVec3(builder, info.LocalScale));
        FBSceneData.DDecoratorInfo.AddRotation(builder, _createQuaternion(builder, info.Rotation));
        return FBSceneData.DDecoratorInfo.EndDDecoratorInfo(builder);
    }

    private static Offset<FBSceneData.Quaternion> _createQuaternion(
            FlatBufferBuilder builder,
            Quaternion quat)
    {
        return FBSceneData.Quaternion.CreateQuaternion(builder, quat.x, quat.y, quat.z, quat.w);
    }

    private static Offset<FBSceneData.DNPCInfo> _createFBNpcInfo(
            FlatBufferBuilder builder,
            DNPCInfo info)
    {
        if (null == info)
        {
            return default(Offset<FBSceneData.DNPCInfo>);
        }
        var super = _createFBEntityInfo(builder, info);

        FBSceneData.DNPCInfo.StartDNPCInfo(builder);
        FBSceneData.DNPCInfo.AddSuper(builder, super);
        FBSceneData.DNPCInfo.AddRotation(builder, _createQuaternion(builder, info.rotation));
        FBSceneData.DNPCInfo.AddMinFindRange(builder, _createVec2(builder, info.minFindRange));
        FBSceneData.DNPCInfo.AddMaxFindRange(builder, _createVec2(builder, info.maxFindRange));
        return FBSceneData.DNPCInfo.EndDNPCInfo(builder);
    }

    private static Offset<FBSceneData.DRegionInfo> _createRegionInfo(
            FlatBufferBuilder builder,
            DRegionInfo info)
    {
        if (null == info)
        {
            return default(Offset<FBSceneData.DRegionInfo>);
        }

        Offset<FBSceneData.DEntityInfo> super = _createFBEntityInfo(builder, info);

        FBSceneData.DRegionInfo.StartDRegionInfo(builder);
        FBSceneData.DRegionInfo.AddSuper(builder, super);
        FBSceneData.DRegionInfo.AddRegiontype(builder, (FBSceneData.RegionType)info.regiontype);
        FBSceneData.DRegionInfo.AddRect(builder, _createVec2(builder, info.rect));
        FBSceneData.DRegionInfo.AddRadius(builder, info.radius);
        FBSceneData.DRegionInfo.AddRotation(builder, _createQuaternion(builder, info.rotation));

        return FBSceneData.DRegionInfo.EndDRegionInfo(builder);

    }

    private static Offset<FBSceneData.FunctionPrefab> _createFunctionPrefab(
            FlatBufferBuilder builder,
            FunctionPrefab info)
    {
        if (null == info)
        {
            return default(Offset<FBSceneData.FunctionPrefab>);
        }

        Offset<FBSceneData.DEntityInfo> super = _createFBEntityInfo(builder, info);

        return FBSceneData.FunctionPrefab.CreateFunctionPrefab(
                builder,
                super,
                (FBSceneData.FunctionType)info.eFunctionType);
    }

    private static Offset<FBSceneData.DTownDoor> _createTownDoor(
            FlatBufferBuilder builder,
            DTownDoor info)
    {
        if (null == info)
        {
            return default(Offset<FBSceneData.DTownDoor>);
        }

        Offset<FBSceneData.DRegionInfo> super = _createRegionInfo(builder, info);

        FBSceneData.DTownDoor.StartDTownDoor(builder);
        FBSceneData.DTownDoor.AddSuper(builder, super);
        FBSceneData.DTownDoor.AddSceneID(builder, info.SceneID);
        FBSceneData.DTownDoor.AddDoorID(builder, info.DoorID);
        FBSceneData.DTownDoor.AddTargetDoorID(builder, info.TargetDoorID);
        FBSceneData.DTownDoor.AddTargetSceneID(builder, info.TargetSceneID);
        FBSceneData.DTownDoor.AddBirthPos(builder, _createVec3(builder, info.BirthPos));
        return FBSceneData.DTownDoor.EndDTownDoor(builder);
    }

    private static Offset<FBSceneData.DTransferInfo> _createAirPos(
           FlatBufferBuilder builder,
           DTransferInfo info)
    {
        if (null == info)
        {
            return default(Offset<FBSceneData.DTransferInfo>);
        }       
        Offset<FBSceneData.DEntityInfo> super = _createFBEntityInfo(builder, info);
        return FBSceneData.DTransferInfo.CreateDTransferInfo(builder, super, info.regionId);
    }


    private static Offset<FBSceneData.DTransportDoor> _createTransportDoor(
            FlatBufferBuilder builder,
            DTransportDoor info)
    {
        if (null == info)
        {
            return default(Offset<FBSceneData.DTransportDoor>);
        }

        Offset<FBSceneData.DRegionInfo> super = _createRegionInfo(builder, info);
        StringOffset townPath = _createString(builder, info.townscenepath);

        FBSceneData.DTransportDoor.StartDTransportDoor(builder);
        FBSceneData.DTransportDoor.AddSuper(builder, super);
        FBSceneData.DTransportDoor.AddIseggdoor(builder, info.isEggDoor);
        FBSceneData.DTransportDoor.AddDoortype(builder, (FBSceneData.TransportDoorType)info.DoorType);
        FBSceneData.DTransportDoor.AddNextsceneid(builder, info.nextsceneid);
        FBSceneData.DTransportDoor.AddNextdoortype(builder, (FBSceneData.TransportDoorType)info.nextdoortype);
        FBSceneData.DTransportDoor.AddBirthposition(builder, _createVec3(builder, info.birthposition));
        FBSceneData.DTransportDoor.AddTownscenepath(builder, townPath);
        return FBSceneData.DTransportDoor.EndDTransportDoor(builder);
    }

    private static Offset<FBSceneData.DDestructibleInfo> _createDDestructibleInfo(
            FlatBufferBuilder builder,
            DDestructibleInfo info)
    {
        if (null == info)
        {
            return default(Offset<FBSceneData.DDestructibleInfo>);
        }

        Offset<FBSceneData.DEntityInfo> super = _createFBEntityInfo(builder, info);

        FBSceneData.DDestructibleInfo.StartDDestructibleInfo(builder);
        FBSceneData.DDestructibleInfo.AddSuper(builder, super);
        FBSceneData.DDestructibleInfo.AddRotation(builder, _createQuaternion(builder, info.rotation));
        FBSceneData.DDestructibleInfo.AddLevel(builder, info.level);
        FBSceneData.DDestructibleInfo.AddFlushGroupID(builder, info.flushGroupID);
        return FBSceneData.DDestructibleInfo.EndDDestructibleInfo(builder);
    }


    private static Offset<FBSceneData.DMonsterInfo> _createMonsterInfo(
            FlatBufferBuilder builder,
            DMonsterInfo info)
    {
        if (null == info)
        {
            return default(Offset<FBSceneData.DMonsterInfo>);
        }

		Offset<FBSceneData.DEntityInfo>       super      = _createFBEntityInfo(builder, info);
        Offset<FBSceneData.DRegionInfo>       regionInfo = _createRegionInfo(builder, info.regionInfo);
		Offset<FBSceneData.DDestructibleInfo> destruct   = _createDDestructibleInfo(builder, info.destructInfo);
        var aiActionPath = _createString(builder, info.aiActionPath);
        var aiScenarioPath = _createString(builder, info.aiScenarioPath);
        
        return FBSceneData.DMonsterInfo.CreateDMonsterInfo(builder,
                super,
                (FBSceneData.MonsterSwapType)info.swapType,
                info.isFaceLeft,
                info.swapNum,
                info.swapDelay,
                info.flushGroupID,
                info.subGroupID,
                info.MonsterInfoTableID,
                (FBSceneData.FlowRegionType)info.flowRegionType,
                regionInfo,
                destruct,
                info.sight,
                info.groupIndex,
                aiActionPath,
                aiScenarioPath
                );
    }

    public static Offset<FBSceneData.DSceneData> CreateFBSceneData(
        FlatBufferBuilder builder,
        DSceneData editorData)
    {
        if (null == editorData)
        {
            return default(Offset<FBSceneData.DSceneData>);
        }
        var name = _createString(builder, editorData.name);
        var prefabpath = _createString(builder, editorData._prefabpath);
        var lightmap = _createString(builder, editorData._LightmapsettingsPath);

        List<Offset<FBSceneData.DEntityInfo>> entityinfos = new List<Offset<FBSceneData.DEntityInfo>>();
        for (int i = 0; i < editorData._entityinfo.Length; ++i)
        {
            entityinfos.Add( _createFBEntityInfo(builder, editorData._entityinfo[i]) );
        }

        var entityinfoOffset = FBSceneData.DSceneData.CreateEntityinfoVector(builder, entityinfos.ToArray());

        var blocklayerOffset = FBSceneData.DSceneData.CreateBlocklayerVector(builder, editorData._blocklayer);


        List<Offset<FBSceneData.DNPCInfo>> npcInfos = new List<Offset<FBSceneData.DNPCInfo>>();
        for (int i = 0; i < editorData._npcinfo.Length; ++i)
        {
            npcInfos.Add(_createFBNpcInfo(builder, editorData._npcinfo[i]));
        }
        var npcInfoOffset = FBSceneData.DSceneData.CreateNpcinfoVector(builder, npcInfos.ToArray());

        List<Offset<FBSceneData.DMonsterInfo>> monsterInfos = new List<Offset<FBSceneData.DMonsterInfo>>();
        for (int i = 0; i < editorData._monsterinfo.Length; ++i)
        {
            monsterInfos.Add(_createMonsterInfo(builder, editorData._monsterinfo[i]));
        }
        var monseterOffset = FBSceneData.DSceneData.CreateMonsterinfoVector(builder, monsterInfos.ToArray());


        List<Offset<FBSceneData.DDecoratorInfo>> decoratorInfos = new List<Offset<FBSceneData.DDecoratorInfo>>();
        for (int i = 0; i < editorData._decoratorinfo.Length; ++i)
        {
            decoratorInfos.Add(_createFBDDecoratorInfo(builder, editorData._decoratorinfo[i]));
        }
        var decoratorInfosOffset = FBSceneData.DSceneData.CreateDecoratorinfoVector(builder, decoratorInfos.ToArray());


        List<Offset<FBSceneData.DDestructibleInfo>> desctrucibleinfos = new List<Offset<FBSceneData.DDestructibleInfo>>();
        for (int i = 0; i < editorData._desructibleinfo.Length; ++i)
        {
            desctrucibleinfos.Add(_createDDestructibleInfo(builder, editorData._desructibleinfo[i]));
        }
        var destructInfoOffset = FBSceneData.DSceneData.CreateDesructibleinfoVector(builder, desctrucibleinfos.ToArray());


        List<Offset<FBSceneData.DRegionInfo>> regionInfos = new List<Offset<FBSceneData.DRegionInfo>>();
        for (int i = 0; i < editorData._regioninfo.Length; ++i)
        {
            regionInfos.Add(_createRegionInfo(builder, editorData._regioninfo[i]));
        }
        var regionInfoOffset = FBSceneData.DSceneData.CreateRegioninfoVector(builder, regionInfos.ToArray());

        List<Offset<FBSceneData.DTransportDoor>> transportdoors = new List<Offset<FBSceneData.DTransportDoor>>();
        for (int i = 0; i < editorData._transportdoor.Length; ++i)
        {
            transportdoors.Add(_createTransportDoor(builder, editorData._transportdoor[i]));
        }
        var transportdoorOffset = FBSceneData.DSceneData.CreateTransportdoorVector(builder, transportdoors.ToArray());;
        var birthposition = _createFBEntityInfo(builder, editorData._birthposition);
        var hellbirthposition = _createFBEntityInfo(builder, editorData._hellbirthposition);


        List<Offset<FBSceneData.DTownDoor>> townDoor = new List<Offset<FBSceneData.DTownDoor>>();
        for (int i = 0; i < editorData._townDoor.Length; ++i)
        {
            townDoor.Add(_createTownDoor(builder, editorData._townDoor[i]));
        }
        var townDoorOffset = FBSceneData.DSceneData.CreateTownDoorVector(builder, townDoor.ToArray());

        List<Offset<FBSceneData.DTransferInfo>> airPos = new List<Offset<FBSceneData.DTransferInfo>>();
        for (int i = 0; i < editorData._fighterBornPosition.Length; ++i)
        {
            airPos.Add(_createAirPos(builder, editorData._fighterBornPosition[i]));
        }
        var airPosOffset = FBSceneData.DSceneData.CreateFighterBornPositionVector(builder, airPos.ToArray());


        List<Offset<FBSceneData.FunctionPrefab>> functionPrefabs = new List<Offset<FBSceneData.FunctionPrefab>>();
        for (int i = 0; i < editorData._FunctionPrefab.Length; ++i)
        {
            functionPrefabs.Add(_createFunctionPrefab(builder, editorData._FunctionPrefab[i]));
        }
        var functionPrefabOffset = FBSceneData.DSceneData.CreateFunctionPrefabVector(builder, functionPrefabs.ToArray());

        FBSceneData.DSceneData.StartDSceneData(builder);
        FBSceneData.DSceneData.AddName(builder, name);
        FBSceneData.DSceneData.AddPrefabpath(builder, prefabpath);
        FBSceneData.DSceneData.AddLightmapsettingsPath(builder, lightmap);
        FBSceneData.DSceneData.AddId(builder, editorData._id);
        FBSceneData.DSceneData.AddCameraLookHeight(builder, editorData._CameraLookHeight);
        FBSceneData.DSceneData.AddCameraDistance(builder, editorData._CameraDistance);
        FBSceneData.DSceneData.AddCameraAngle(builder, editorData._CameraAngle);
        FBSceneData.DSceneData.AddCameraNearClip(builder, editorData._CameraNearClip);
        FBSceneData.DSceneData.AddCameraFarClip(builder, editorData._CameraFarClip);
        FBSceneData.DSceneData.AddCameraSize(builder, editorData._CameraSize); 
        FBSceneData.DSceneData.AddCameraZRange(builder, _createVec2(builder, editorData._CameraZRange));
        FBSceneData.DSceneData.AddCameraXRange(builder, _createVec2(builder, editorData._CameraXRange));
        FBSceneData.DSceneData.AddCameraPersp(builder, editorData._CameraPersp); 
        FBSceneData.DSceneData.AddCenterPostionNew(builder, _createVec3(builder, editorData._CenterPostionNew));
        FBSceneData.DSceneData.AddScenePostion(builder, _createVec3(builder, editorData._ScenePostion));
        FBSceneData.DSceneData.AddSceneUScale(builder, editorData._SceneUScale); 
        FBSceneData.DSceneData.AddGridSize(builder, _createVec2(builder, editorData._GridSize));
        FBSceneData.DSceneData.AddLogicXSize(builder, _createVec2(builder, editorData._LogicXSize));
        FBSceneData.DSceneData.AddLogicZSize(builder,  _createVec2(builder, editorData._LogicZSize));
        FBSceneData.DSceneData.AddObjectDyeColor(builder,  _createColor(builder, editorData._ObjectDyeColor));
        FBSceneData.DSceneData.AddLogicPos(builder, _createVec3(builder, editorData._LogicPos ));
        FBSceneData.DSceneData.AddWeatherMode(builder, (FBSceneData.EWeatherMode)editorData._WeatherMode); 
        FBSceneData.DSceneData.AddTipsID(builder, editorData._TipsID); 
        FBSceneData.DSceneData.AddLogicXmin(builder, editorData._LogicXmin); 
        FBSceneData.DSceneData.AddLogicXmax(builder, editorData._LogicXmax); 
        FBSceneData.DSceneData.AddLogicZmin(builder, editorData._LogicZmin); 
        FBSceneData.DSceneData.AddLogicZmax(builder, editorData._LogicZmax); 
        FBSceneData.DSceneData.AddEntityinfo(builder, entityinfoOffset);
        FBSceneData.DSceneData.AddBlocklayer(builder, blocklayerOffset);
        FBSceneData.DSceneData.AddNpcinfo(builder, npcInfoOffset);
        FBSceneData.DSceneData.AddMonsterinfo(builder, monseterOffset);
        FBSceneData.DSceneData.AddDecoratorinfo(builder, decoratorInfosOffset);
        FBSceneData.DSceneData.AddDesructibleinfo(builder, destructInfoOffset);
        FBSceneData.DSceneData.AddRegioninfo(builder, regionInfoOffset);
        FBSceneData.DSceneData.AddTransportdoor(builder, transportdoorOffset);
        FBSceneData.DSceneData.AddBirthposition(builder, birthposition);
        FBSceneData.DSceneData.AddHellbirthposition(builder, hellbirthposition);
        FBSceneData.DSceneData.AddTownDoor(builder, townDoorOffset);
        FBSceneData.DSceneData.AddFunctionPrefab(builder, functionPrefabOffset);
        FBSceneData.DSceneData.AddFighterBornPosition(builder, airPosOffset);
        return FBSceneData.DSceneData.EndDSceneData(builder);
    }
}
