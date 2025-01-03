using System;
using FlatBuffers;
using System.Collections.Generic;

public class FBDungeonDataTools
{
#if UNITY_EDITOR
    static T Call<T>(Func<T> func)
    {
        return func();
    }

    private static StringOffset _createString(FlatBufferBuilder builder, string value)
    {
        if (string.IsNullOrEmpty(value) == false)
        {
            return builder.CreateString(value);
        }

		return builder.CreateString (string.Empty);
    }

    public static Offset<FBDungeonData.DDungeonData> CreateFBDungeonData(
        FlatBufferBuilder builder,
        DDungeonData editorData)
    {
        return FBDungeonData.DDungeonData.CreateDDungeonData(builder,
            _createString(builder, editorData.name),
            editorData.height,
			editorData.weidth,
            editorData.startindex,
            Call(()=>{
                if (editorData.areaconnectlist == null)
                {
                    return default(VectorOffset);
                }

                List<Offset<FBDungeonData.DSceneDataConnect>> list = new List<Offset<FBDungeonData.DSceneDataConnect>>();

                for (int i = 0; i < editorData.areaconnectlist.Length; ++i)
                {
                    var editorCon = editorData.areaconnectlist[i];
                    var data = FBDungeonData.DSceneDataConnect.CreateDSceneDataConnect(builder, 
                        //default(VectorOffset),
                        Call(() => {
                            if (null == editorCon.isconnect)
                            {
                                return default(VectorOffset);
                            }
                            return FBDungeonData.DSceneDataConnect.CreateIsconnectVector(
                                    builder, editorCon.isconnect);
                        }),
                        Call(() => {
                            if (null == editorCon.linkAreaIndex)
                            {
                                return default(VectorOffset);
                            }

                            return FBDungeonData.DSceneDataConnect.CreateLinkAreaIndexVector(
                                builder, editorCon.linkAreaIndex);
                        }),
                        editorCon.areaindex,
                        editorCon.id,
                        _createString(builder, editorCon.sceneareapath),
                        editorCon.positionx,
                        editorCon.positiony,
                        editorCon.isboss,
                        editorCon.isstart,
                        editorCon.isegg,
                        editorCon.isnothell,
                        editorCon.treasureType
                    );
                    list.Add(data);
                }

                return FBDungeonData.DDungeonData.CreateAreaconnectlistVector(builder, list.ToArray());
            })
        );
    }
#endif

}

