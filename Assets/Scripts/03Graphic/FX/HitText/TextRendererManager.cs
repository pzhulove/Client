using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class TextRendererManager
    {
        #region PrefabPath
        private string m_NormalHurtTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/NormalHurtTextRenderer";

        private string m_NormalHurtWhiteTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/NormalHurtWhiteTextRenderer";

        private string m_CriticleHurtTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/CriticleHurtTextRenderer";

        private string m_CriticleHurtWhiteTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/CriticleHurtWhiteTextRenderer";

        private string m_CriticleHurtImageRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/CriticleHurtImageRenderer";

        private string m_HurtOwnTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/HurtOwnTextRenderer";

        private string m_HurtOwnWhiteTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/HurtOwnWhiteTextRenderer";

        private string m_BuffHurtTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/BuffHurtTextRenderer";

        private string m_BuffHurtWhiteTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/BuffHurtWhiteTextRenderer";

        private string m_HurtFriendTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/HurtFriendTextRenderer";

        private string m_HPTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/HPTextRenderer";

        private string m_MPTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/MPTextRenderer";

        private string m_NormalAttachTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/NormalAttachTextRenderer";

        private string m_CriticleAttachTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/CriticleAttachTextRenderer";

        private string m_MissImageRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/MissImageRenderer";

        private string m_BuffNameTextRendererPrefabPath = "UIFlatten/Prefabs/Battle_Digit/New/BuffNameTextRenderer";

        #endregion

        #region TextRenderer Cache
        private TextRenderer m_NormalHurtTextRenderer;

        private TextRenderer m_NormalHurtWhiteTextRenderer;

        private TextRenderer m_CriticleHurtTextRenderer;

        private TextRenderer m_CriticleHurtWhiteTextRenderer;

        private TextRenderer m_HurtOwnTextRenderer;

        private TextRenderer m_HurtOwnWhiteTextRenderer;

        private TextRenderer m_BuffHurtTextRenderer;

        private TextRenderer m_BuffHurtWhiteTextRenderer;

        private TextRenderer m_HurtFriendTextRenderer;

        private TextRenderer m_HPTextRenderer;

        private TextRenderer m_MPTextRenderer;

        private TextRenderer m_NormalAttachTextRenderer;

        private TextRenderer m_CriticleAttachTextRenderer;

        private TextRenderer m_BuffNameTextRenderer;

        #endregion

        #region ImageRenderer Cache
        private ImageRenderer m_CriticleHurtImageRenderer;

        private ImageRenderer m_MissImageRenderer;

        #endregion

        public void Init()
        {
            //Normal Hurt
            m_NormalHurtTextRenderer = InstantiateTextRendererPrefab(m_NormalHurtTextRendererPrefabPath);
            m_NormalHurtWhiteTextRenderer = InstantiateTextRendererPrefab(m_NormalHurtWhiteTextRendererPrefabPath);
            //Criticle Hurt
            m_CriticleHurtTextRenderer = InstantiateTextRendererPrefab(m_CriticleHurtTextRendererPrefabPath);
            m_CriticleHurtWhiteTextRenderer = InstantiateTextRendererPrefab(m_CriticleHurtWhiteTextRendererPrefabPath);
            //Hurt Own
            m_HurtOwnTextRenderer = InstantiateTextRendererPrefab(m_HurtOwnTextRendererPrefabPath);
            m_HurtOwnWhiteTextRenderer = InstantiateTextRendererPrefab(m_HurtOwnWhiteTextRendererPrefabPath);
            //Buff Hurt
            m_BuffHurtTextRenderer = InstantiateTextRendererPrefab(m_BuffHurtTextRendererPrefabPath);
            m_BuffHurtWhiteTextRenderer = InstantiateTextRendererPrefab(m_BuffHurtWhiteTextRendererPrefabPath);
            //Hurt Friend
            m_HurtFriendTextRenderer = InstantiateTextRendererPrefab(m_HurtFriendTextRendererPrefabPath);
            //HP
            m_HPTextRenderer = InstantiateTextRendererPrefab(m_HPTextRendererPrefabPath);
            //MP
            m_MPTextRenderer = InstantiateTextRendererPrefab(m_MPTextRendererPrefabPath);
            //Normal Attach
            m_NormalAttachTextRenderer = InstantiateTextRendererPrefab(m_NormalAttachTextRendererPrefabPath);
            //Criticle Attach
            m_CriticleAttachTextRenderer = InstantiateTextRendererPrefab(m_CriticleAttachTextRendererPrefabPath);

            //Criticle Hurt Image
            m_CriticleHurtImageRenderer = InstantiateImageRendererPrefab(m_CriticleHurtImageRendererPrefabPath);

            //Miss Image
            m_MissImageRenderer = InstantiateImageRendererPrefab(m_MissImageRendererPrefabPath);

            //Buff Name 
            m_BuffNameTextRenderer = InstantiateTextRendererPrefab(m_BuffNameTextRendererPrefabPath);

            m_BuffNameTextRenderer.PreLoadFontCharacter("减加速眩晕石化出血感电睡眠冰冻束缚混乱中毒隐身灼伤失明物理魔法荆棘反伤攻速移速提升释放速度攻击强化暴击恢复防御提升吸血属性智力体力");
        }

        private TextRenderer InstantiateTextRendererPrefab(string path)
        {
            GameObject tempTextRendererGO = AssetLoader.instance.LoadResAsGameObject(path);
            Utility.AttachTo(tempTextRendererGO, ClientSystemManager.instance.Layer3DRoot);
            TextRenderer result = tempTextRendererGO.GetComponent<TextRenderer>();
            result.Init();

            return result;
        }

        private ImageRenderer InstantiateImageRendererPrefab(string path)
        {
            GameObject tempImageRendererGO = AssetLoader.instance.LoadResAsGameObject(path);
            Utility.AttachTo(tempImageRendererGO, ClientSystemManager.instance.Layer3DRoot);
            ImageRenderer result = tempImageRendererGO.GetComponent<ImageRenderer>();
            result.Init();
            return result;
        }

        public void Update()
        {
            CheckInit();

            //Normal Attach
            m_NormalAttachTextRenderer.UpdateMesh();
            //Criticle Attach
            m_CriticleAttachTextRenderer.UpdateMesh();

            //Normal Hurt
            m_NormalHurtTextRenderer.UpdateMesh();
            m_NormalHurtWhiteTextRenderer.UpdateMesh();
            //Criticle Hurt
            m_CriticleHurtTextRenderer.UpdateMesh();
            m_CriticleHurtWhiteTextRenderer.UpdateMesh();
            //Hurt Own
            m_HurtOwnTextRenderer.UpdateMesh();
            m_HurtOwnWhiteTextRenderer.UpdateMesh();
            //Buff Hurt
            m_BuffHurtTextRenderer.UpdateMesh();
            m_BuffHurtWhiteTextRenderer.UpdateMesh();
            //Hurt Friend
            m_HurtFriendTextRenderer.UpdateMesh();
            //HP
            m_HPTextRenderer.UpdateMesh();
            //MP
            m_MPTextRenderer.UpdateMesh();

            //Criticle Hurt Image
            m_CriticleHurtImageRenderer.UpdateMesh();
            //Miss Image
            m_MissImageRenderer.UpdateMesh();

            //Buff Name
            m_BuffNameTextRenderer.UpdateMesh();
        }

        public void AddText(int num,Vec3 position,int actorID, ShowHitComponent.HitResType resType,HitTextAniType animType,int hitEffectType,int animCurveIndex)
        {
            CheckInit();

            switch (resType)
            {
                //Normal
                case ShowHitComponent.HitResType.NORMAL:
                    m_NormalHurtTextRenderer.AddText(num,position, actorID,hitEffectType, (int)animType, 0);
                    m_NormalHurtWhiteTextRenderer.AddText(num, new Vec3(position.x,position.y,position.z - 0.1f), actorID, hitEffectType, (int)animType, 0);
                    break;

                //Normal Attach
                case ShowHitComponent.HitResType.NORMAL_ATTACH:
                    m_NormalAttachTextRenderer.AddText(num, position, actorID, hitEffectType, (int)animType, animCurveIndex);
                    break;

                //Critical
                case ShowHitComponent.HitResType.CRITICAL:
                    m_CriticleHurtTextRenderer.AddText(num, position, actorID, hitEffectType, (int)animType, 0);
                    m_CriticleHurtWhiteTextRenderer.AddText(num, new Vec3(position.x, position.y, position.z - 0.1f), actorID, hitEffectType, (int)animType, 0);
                    m_CriticleHurtImageRenderer.AddImage(position, actorID, hitEffectType, 0);
                    break;

                //Critical Attach
                case ShowHitComponent.HitResType.CRITICAL_ATTACH:
                    m_CriticleAttachTextRenderer.AddText(num, position, actorID, hitEffectType, (int)animType, 0);
                    break;

                //Miss
                case ShowHitComponent.HitResType.MISS:
                    m_MissImageRenderer.AddImage(position, actorID, hitEffectType, 0);
                    break;

                //OWN_HURT
                case ShowHitComponent.HitResType.OWN_HURT:
                    m_HurtOwnTextRenderer.AddText(num, position, actorID, hitEffectType, (int)animType, 0);
                    m_HurtOwnWhiteTextRenderer.AddText(num, new Vec3(position.x, position.y, position.z - 0.1f), actorID, hitEffectType, (int)animType, 0);
                    break;

                //Buff Hurt
                case ShowHitComponent.HitResType.BUFF_HURT:
                    m_BuffHurtTextRenderer.AddText(num, position, actorID, hitEffectType, (int)animType, 0);
                    m_BuffHurtWhiteTextRenderer.AddText(num, new Vec3(position.x, position.y, position.z - 0.1f), actorID, hitEffectType, (int)animType, 0);
                    break;

                //Friend Hurt
                case ShowHitComponent.HitResType.FRIEND_HURT:
                    m_HurtFriendTextRenderer.AddText(num, position, actorID, hitEffectType, (int)animType, 0);
                    break;

                //Text HP
                case ShowHitComponent.HitResType.TEXT_HP:
                    m_HPTextRenderer.AddText(num, position, actorID, hitEffectType, (int)animType, 0);
                    break;

                //Text MP
                case ShowHitComponent.HitResType.TEXT_MP:
                    m_MPTextRenderer.AddText(num, position, actorID, hitEffectType, (int)animType, 0);
                    break;

                //Text Buff Name
                case ShowHitComponent.HitResType.TEXT_BUFF_NAME:

                    break;

                default:
                    break;
            }
        }

        public void AddNameText(string text, Vec3 position, int actorID, ShowHitComponent.HitResType resType, HitTextAniType animType, int hitEffectType, int animCurveIndex)
        {
            CheckInit();
            switch (resType)
            {
                case ShowHitComponent.HitResType.TEXT_BUFF_NAME:
                    m_BuffNameTextRenderer.AddNameText(text, position, actorID, hitEffectType, (int)animType, 0);
                    break;
            }
        }

        public void MoveUpAll(int hitEffectType,int actorID,HitTextAniType animType)
        {
            m_NormalAttachTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);

            switch (hitEffectType)
            {
                case 0:
                    //if (animType == HitTextAniType.FRIEND)
                    {
                        m_HurtFriendTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                    }

                    //if (animType == HitTextAniType.OWN)
                    {
                        m_HurtOwnTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                        m_HurtOwnWhiteTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                    }

                    break;

                case 1:
                    //if (animType == HitTextAniType.NORMAL)
                    {
                        m_NormalHurtTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                        m_NormalHurtWhiteTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                    }

                   // if (animType == HitTextAniType.CRITIAL)
                    {
                        m_CriticleHurtTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                        m_CriticleHurtWhiteTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                        m_CriticleHurtImageRenderer.MoveUpAll(actorID, hitEffectType);

                        m_CriticleAttachTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                    }

                    //if (animType == HitTextAniType.OWN)
                    {
                        m_HurtOwnTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                        m_HurtOwnWhiteTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                    }

                    //if (animType == HitTextAniType.FRIEND)
                    {
                        m_HurtFriendTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                    }
                    break;

                case 2:
                    //if (animType == HitTextAniType.OWN)
                    {
                        m_HurtOwnTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                        m_HurtOwnWhiteTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                    }

                    //if (animType == HitTextAniType.BUFF)
                    {
                        m_BuffHurtTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                        m_BuffHurtWhiteTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                    }
                    break;

                case 3:
                    m_HPTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                    break;

                case 4:
                    m_MPTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                    break;

                case 5:
                    m_BuffNameTextRenderer.MoveUpAll(actorID, hitEffectType, (int)animType);
                    break;

                default:
                    break;
            }
        }

        private void CheckInit()
        {
            if (m_NormalHurtTextRenderer == null)
                Init();
        }
    }
}

