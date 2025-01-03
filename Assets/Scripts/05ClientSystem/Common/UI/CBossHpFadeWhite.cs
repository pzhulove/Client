using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CBossHpFadeWhite : MonoBehaviour {

    private const string kPrefabUnit      = "UIFlatten/Prefabs/BattleUI/DungeonBar/HPBar_White";

    private List<CBossHpWhiteBar> mBars = new List<CBossHpWhiteBar>();

    public void SetValue(float minValue, float maxValue, float timeout, UnityAction fn)
    {
        GameObject go = CGameObjectPool.instance.GetGameObject(kPrefabUnit, enResourceType.UIPrefab, (uint)GameObjectPoolFlag.None);

        Utility.AttachTo(go, this.gameObject);

        CBossHpWhiteBar bar = go.GetComponent<CBossHpWhiteBar>();
        bar.SetValue(minValue, maxValue);
        bar.StartFadeOut(timeout, ()=>
        {
            if (null != fn)
            {
                fn.Invoke();
            }

            CGameObjectPool.instance.RecycleGameObject(go);

            if (null != bar)
            {
                mBars.Remove(bar);
            }
        });

        for (int i = 0; i < mBars.Count; ++i)
        {
            if (null != mBars[i])
            {
                mBars[i].SetMinValue(minValue);
            }
        }

        mBars.Add(bar);
    }

    public void OnDestroy()
    {
        mBars.Clear();
    }
}
