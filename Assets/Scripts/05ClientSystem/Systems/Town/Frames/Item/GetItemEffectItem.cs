using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using DG.Tweening;
using System.Collections;

namespace GameClient
{
    public class GetItemEffectItem : MonoBehaviour
    {
        [SerializeField]
        GameObject goItemParent = null;

        [SerializeField]
        Text txtItemName = null;

        [SerializeField]
        GameObject goHighValue = null;
        // Use this for initialization
        void Start()
        {
    
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetUp(ItemData itemData,float fDelay, GetItemEffectFrame frame,bool bHighValue = false)
        {
            //             transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            //             Tweener tweener = transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.5f);
            //             tweener.SetEase(Ease.OutBack);
            //             tweener.SetDelay(fDelay);


            if(frame != null && goItemParent != null)
            {
                ComItem comItem = frame.CreateComItem(goItemParent);
                if(comItem != null)
                {
                    comItem.Setup(itemData,(GameObject obj, ItemData item) => 
                    {
                        ItemTipManager.GetInstance().CloseAll();
                        ItemTipManager.GetInstance().ShowTip(item);
                    });
                }

                if (txtItemName != null && itemData != null)
                {
                    txtItemName.text = itemData.GetColorName();
                }
                if(bHighValue)
                {
                    GameFrameWork.instance.StartCoroutine(SetUpHighValueEffect());
                }
            }
        }

        private IEnumerator SetUpHighValueEffect()
        {
            yield return Yielders.GetWaitForSeconds(0.3f);
            if(goHighValue != null)
            {
                GameObject ObjEffect = AssetLoader.GetInstance().LoadRes("Effects/UI/Prefab/EffUI_pinji/Prefab/EffUI_pinjiguang_zise").obj as GameObject;
                ObjEffect.transform.SetParent(goHighValue.transform, false);
                ObjEffect.SetActive(true);
            }           
            yield return 0;
        }
}
}


