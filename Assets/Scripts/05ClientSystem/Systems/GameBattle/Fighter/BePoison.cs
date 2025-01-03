using System;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
namespace GameClient
{
    public sealed class BePoison : BeBaseFighter
    {
        public sealed class BePoisonData : BeBaseActorData
        {
            public float radius;
            public int x;
            public int y;
            public Vector3 realPos;
            public uint startTime;
            public int durTime;
        }
        const float radiusFactor = 0.0125f;
        const string effect_chiji_duquan = "Effects/Scene_effects/Scene_Chiji/Prefab/Eff_Scene_Chiji_duquan";
        GeEffectEx mRingEffect = null;
        float curRadius = 0.0f;
        bool startShrink = false;
        float durShrinkTime = 0;
        Vector3 curScale = Vector3.one;
        Vector3 startPos = Vector3.zero;
        float startRadius = 1.0f;
        public BePoison(BePoisonData data, ClientSystemGameBattle systemTown)
            : base(data, systemTown)
        {

        }
        public override void Update(float timeElapsed)
        {
            base.Update(timeElapsed);
            if (_geActor == null) return;
            if (startShrink)
            {
                var data = _data as BePoisonData;
                if (data.durTime > durShrinkTime)
                {
                    float t = durShrinkTime / data.durTime;
                    data.MoveData.ServerPosition = Vector3.Lerp(startPos, data.realPos, t);
                    curRadius = Mathf.Lerp(startRadius, data.radius, t);
                    durShrinkTime += timeElapsed;
                    curScale.x = curRadius * radiusFactor;
                    curScale.y = 1.0f;
                    curScale.z = curRadius * radiusFactor;
                    _geActor.SetScaleV3(curScale);
                    //      Logger.LogErrorFormat("startPos {0} realpos {1} curpos {2} startRadius {3} curRadius {4} targetradius {5}", startPos, data.realPos, data.MoveData.ServerPosition, startRadius, curRadius, data.radius);
                }
                else
                {
                    startShrink = false;
                    durShrinkTime = 0;
                    data.MoveData.ServerPosition = data.realPos;
                    curRadius = data.radius;
                    curScale.x = curRadius * radiusFactor;
                    curScale.y = 1.0f;
                    curScale.z = curRadius * radiusFactor;
                    _geActor.SetScaleV3(curScale);
                }
            }
        }
        public override void InitGeActor(GeSceneEx geScene)
        {
            _geScene = geScene;
            if (_geScene == null) return;
            var data = _data as BePoisonData;
            if (data == null) return;
            _geActor = _geScene.CreateActor(60441);
            if (_geActor == null)
            {
                return;
            }
            if (data.durTime == 0)
            {
                data.MoveData.ServerPosition = data.realPos;
                curScale.x = data.radius * radiusFactor;
                curScale.y = 1.0f;
                curScale.z = data.radius * radiusFactor;
                curRadius = data.radius;
                startPos = data.realPos;
                startRadius = data.radius;
                _geActor.SetScaleV3(curScale);
            }
            base.InitGeActor(geScene);
            if (data.durTime != 0)
            {
                StartShrink();
            }

        }
        public override void Dispose()
        {
            base.Dispose();
        }
        public void ResetStartInfo(float startRadius, Vector2 startPos)
        {
            this.startPos.x = startPos.x;
            this.startPos.z = startPos.y;
            this.startRadius = startRadius;
            curRadius = startRadius;
            var data = _data as BePoisonData;
            data.MoveData.ServerPosition = this.startPos;
        }
        public void StartShrink()
        {
            var data = _data as BePoisonData;
            if (data == null) return;
            if (data.durTime == 0.0f) return;
            startShrink = true;
            durShrinkTime = data.startTime;
            startPos = data.MoveData.ServerPosition;
            startRadius = curRadius;
        }
        public void ResetCircle()
        {
            var data = _data as BePoisonData;
            if (data == null) return;
            data.MoveData.ServerPosition = data.realPos;
            curRadius = data.radius;
            curScale.x = curRadius * radiusFactor;
            curScale.y = 1.0f;
            curScale.z = curRadius * radiusFactor;
            startShrink = false;
            durShrinkTime = 0;
            if (_geActor == null) return;
            _geActor.SetScaleV3(curScale);
        }
    }
}
