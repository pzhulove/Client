using UnityEngine;

public interface IGeAvatarActor
{
	void ChangeAction(string actionName, float speed = 1.0f, bool loop = false);
	string GetCurActionName();
    void ChangeAvatar(GeAvatarChannel eChannel, string modulePath, bool isAsync = false,bool highPriority = false);
    void ChangeAvatar(GeAvatarChannel eChannel, DAssetObject asset, bool isAsync = false, bool highPriority = false);

    void SuitAvatar(bool isAsync = false,bool highPriority = false);

    GeAttach AttachAvatar(string attachmentName, string attachRes, string attachNode, bool needClear = true, bool asyncLoad = true, bool highPriority = false, float fAttHeight = 0.0f);
    void RemoveAttach(GeAttach attachment);
    GeAttach GetAttachment(string name);
    GeEffectEx CreateEffect(string effectRes, string attachNode, float fTime, EffectTimeType timeType, Vector3 localPos,Quaternion localRot, float initScale = 1.0f, float fSpeed = 1.0f, bool isLoop = false);
    void OnUpdate(float fDelta);
}

