using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using ProtoTable;

public class ComDungeonComboUnit : MonoBehaviour
{
    public Image mFg;

    public void SetSkill(int id)
    {
        SkillTable item = TableManager.instance.GetTableItem<SkillTable>(id);
        if (null != item)
        {
            // mFg.sprite = AssetLoader.instance.LoadRes(item.Icon, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref mFg, item.Icon);
        }
    }

    public void PlayState(bool isRight)
    {
        if (isRight)
        {
            var obj = CGameObjectPool.instance.GetGameObject("Effects/Scene_effects/EffectUI/EffUI_xinshou_lianji", enResourceType.UIPrefab, (uint)GameObjectPoolFlag.None);
            Utility.AttachTo(obj, gameObject);

            mFg.color = Color.green;
        }
        else
        {
            mFg.color = Color.red;
        }
    }

    public void Reset()
    {
        mFg.color = Color.white;
    }
}
