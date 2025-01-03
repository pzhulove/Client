using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ProtoTable;

public class ComMonsterItem : MonoBehaviour {

    public Image mImage;
    public Image mBg;
    public Image mFg;

    public void SetMonster(int id)
    {
        var item = TableManager.instance.GetTableItem<UnitTable>(id);
        if (null != item)
        {
            mFg.enabled = (item.Type == UnitTable.eType.BOSS);

            var resItem = TableManager.instance.GetTableItem<ResTable>(item.Mode);
            if (null != resItem)
            {
                var sprite = AssetLoader.instance.LoadRes(resItem.IconPath, typeof(Sprite)).obj as Sprite;
                if (null != sprite)
                {
                    // mImage.sprite = sprite;
                    ETCImageLoader.LoadSprite(ref mImage, resItem.IconPath);
                    return;
                }
				else {
					Logger.LogWarningFormat("怪物{0}资源{1}没有头像", id, item.Mode);
				}
            }
        }

        mImage.sprite = null;
        mImage.color = Color.red;
    }

	public void SetVisible(bool flag)
	{
		if (mImage != null)
			mImage.enabled = flag;
		if (mBg != null)
			mBg.enabled = flag;
		if (mFg != null)
			mFg.enabled = flag;
	}
}
