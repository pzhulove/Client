using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum ComPetTipsUnitType
    {
        CPTUT_SPERATE_LINE = 0,
        CPTUT_TITLE,
        CPTUT_CONTENT,
        CPTUT_COUNT,
    }

    class ComPetTipsUnitData
    {
        public ComPetTipsUnitType eComPetTipsUnitType;
        public string content;
    }

    class ComPetTipsUnit : MonoBehaviour
    {
        public GameObject goSperateLine;
        public GameObject goTitle;
        public GameObject goContent;
        public GameObject goParent;

        List<GameObject>[] pools = new List<GameObject>[(int)ComPetTipsUnitType.CPTUT_COUNT]
        {
            new List<GameObject>(),
            new List<GameObject>(),
            new List<GameObject>(),
        };

        List<GameObject>[] actives = new List<GameObject>[(int)ComPetTipsUnitType.CPTUT_COUNT]
        {
            new List<GameObject>(),
            new List<GameObject>(),
            new List<GameObject>(),
        };

        void _OnCreate(ComPetTipsUnitData data,GameObject goLocal)
        {
            if(null != goLocal)
            {
                switch (data.eComPetTipsUnitType)
                {
                    case ComPetTipsUnitType.CPTUT_TITLE:
                    case ComPetTipsUnitType.CPTUT_CONTENT:
                        {
                            var texts = goLocal.GetComponentsInChildren<Text>();
                            if(null != texts && texts.Length > 0)
                            {
                                texts[0].text = data.content;
                            }
                        }
                        break;
                }
            }
        }

        GameObject _allocateFromPoos(ComPetTipsUnitType eComPetTipsUnitType)
        {
            int iIndex = (int)eComPetTipsUnitType;
            if(iIndex>=0&&iIndex<pools.Length && pools[iIndex].Count>0)
            {
                GameObject goLocal = pools[iIndex][0];
                pools[iIndex].RemoveAt(0);
                actives[iIndex].Add(goLocal);
                return goLocal;
            }
            return null;
        }

        public void setTips(List<ComPetTipsUnitData> datas)
        {
            _recycleTips();

            goSperateLine.CustomActive(false);
            goContent.CustomActive(false);
            goTitle.CustomActive(false);

            for (int i = 0; i < datas.Count; ++i)
            {
                if(null == datas[i] || datas[i].eComPetTipsUnitType == ComPetTipsUnitType.CPTUT_COUNT)
                {
                    continue;
                }

                GameObject goLocal = _allocateFromPoos(datas[i].eComPetTipsUnitType);
                if(null == goLocal)
                {
                    GameObject goPrefab = null;
                    switch (datas[i].eComPetTipsUnitType)
                    {
                        case ComPetTipsUnitType.CPTUT_CONTENT:
                            {
                                goPrefab = goContent;
                            }
                            break;
                        case ComPetTipsUnitType.CPTUT_SPERATE_LINE:
                            {
                                goPrefab = goSperateLine;
                            }
                            break;
                        case ComPetTipsUnitType.CPTUT_TITLE:
                            {
                                goPrefab = goTitle;
                            }
                            break;
                    }
                    if (null == goPrefab)
                    {
                        continue;
                    }

                    goLocal = GameObject.Instantiate(goPrefab);
                    if(null != goLocal)
                    {
                        actives[(int)datas[i].eComPetTipsUnitType].Add(goLocal);
                    }
                }

                if(null != goLocal)
                {
                    Utility.AttachTo(goLocal, goParent);
                    _OnCreate(datas[i], goLocal);
                    goLocal.CustomActive(true);
                    goLocal.transform.SetSiblingIndex(i+ (int)ComPetTipsUnitType.CPTUT_COUNT);
                }
            }
        }

        void _recycleTips()
        {
            for(int i = 0; i < actives.Length; ++i)
            {
                for(int j = 0; j < actives[i].Count;++j)
                {
                    actives[i][j].CustomActive(false);
                    pools[i].Add(actives[i][j]);
                }
                actives[i].Clear();
            }
        }
    }
}