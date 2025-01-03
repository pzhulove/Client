using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum EquipmentDataState
    {
        PROPERTY_NO_CHANGE,
        PROPERTY_UP,
        PROPERTY_DOWN,
    }

    public struct BetterEquipmentData
    {
        public string name;
        public string PreData;
        public string CurData;
        public EquipmentDataState DataState;
    }

    class BetterEquipmentFrame : ClientFrame
    {
        string ElementPath = "UI/Prefabs/BetterEquipmentEle";

        int CreateEleObjNum = 0;
        float fTimeIntreval = 0.0f;

        List<GameObject> itemObjList = new List<GameObject>();

        protected override void _OnOpenFrame()
        {         
        }

        protected override void _OnCloseFrame()
        {
        }

        public override string GetPrefabPath()
        {
            return "UI/Prefabs/BetterEquipmentFrame";
        }

        [UIEventHandle("btClose")]
        void OnClose()
        {
            ClearData();
            frameMgr.CloseFrame(this);
        }

        void ClearData()
        {
            CreateEleObjNum = 0;
            fTimeIntreval = 0.0f;
            itemObjList.Clear();
        }

        public void UpdateInterface(List<BetterEquipmentData> data)
        {
            List<BetterEquipmentData> FilterData = new List<BetterEquipmentData>();

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].DataState == EquipmentDataState.PROPERTY_NO_CHANGE)
                {
                    continue;
                }

                FilterData.Add(data[i]);
            }

            if (FilterData.Count > CreateEleObjNum)
            {
                for(int i = 0; i < FilterData.Count - CreateEleObjNum; i++)
                {
                    GameObject Itemobj = AssetLoader.instance.LoadResAsGameObject(ElementPath);
                    if (Itemobj == null)
                    {
                        Logger.LogError("can't create obj in MailFrame");
                        return;
                    }

                    itemObjList.Add(Itemobj);
                    Utility.AttachTo(Itemobj, ObjRoot);
                }

                CreateEleObjNum = FilterData.Count;
            }

            for (int i = 0; i < CreateEleObjNum; i++)
            {
                if(i < FilterData.Count)
                {
                    BetterEquipmentData beData = FilterData[i];

                    itemObjList[i].SetActive(true);

                    Text[] texts = itemObjList[i].GetComponentsInChildren<Text>();
                    for(int j = 0; j < texts.Length; j++)
                    {
                        if(texts[j].name == "name")
                        {
                            texts[j].text = beData.name;
                        }
                        else if(texts[j].name == "PreData")
                        {
                            texts[j].text = beData.PreData;
                        }
                        else if (texts[j].name == "CurData")
                        {
                            texts[j].text = beData.CurData;

                            if(beData.DataState == EquipmentDataState.PROPERTY_UP)
                            {
                                texts[j].color = new Color(20/255f, 236/255f, 4/255f);
                            }
                            else if(beData.DataState == EquipmentDataState.PROPERTY_DOWN)
                            {
                                texts[j].color = new Color(243/255f, 0, 0);
                            }
                            else
                            {
                                texts[j].color = new Color(1f, 1f, 1f);
                            }
                        }
                    }

                    Image[] Imgs = itemObjList[i].GetComponentsInChildren<Image>();
                    for (int j = 0; j < Imgs.Length; j++)
                    {
                        if (Imgs[j].name == "up")
                        {
                            if(beData.DataState == EquipmentDataState.PROPERTY_UP)
                            {
                                Imgs[j].gameObject.SetActive(true);
                            }   
                            else
                            {
                                Imgs[j].gameObject.SetActive(false);
                            }         
                        }
                        else if (Imgs[j].name == "down")
                        {
                            if (beData.DataState == EquipmentDataState.PROPERTY_DOWN)
                            {
                                Imgs[j].gameObject.SetActive(true);
                            }
                            else
                            {
                                Imgs[j].gameObject.SetActive(false);
                            }
                        }
                    }
                }
                else
                {
                    itemObjList[i].SetActive(false);
                }
            }
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            fTimeIntreval += timeElapsed;

            if (fTimeIntreval > 1.20f)
            {
                OnClose();
            }
        }

        [UIObject("middle_back/Scroll View/Viewport/Content")]
        protected GameObject ObjRoot;
    }
}
