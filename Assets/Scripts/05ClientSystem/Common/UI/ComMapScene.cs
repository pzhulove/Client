using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComMapScene : MonoBehaviour
    {
        /// <summary>
        /// 场景ID
        /// </summary>
        public int SceneID;

        /// <summary>
        /// 这个是一个枢纽, 作为跳转到下一个场景的入口
        /// </summary>
        public int JumpTownId = -1;

        public ComButtonEx btnScene;
        public GameObject objLockRoot;
        public Text labLockDesc;
        private Image imgBackground;
        private string backImgPath = ""; // 1.5项目暂时不需要切换大地图的背景图,需要的时候再开放 by Wangbo 2020.09.01

        bool m_bInited = false;
        Vector3 m_sizeRate = Vector3.zero;
        Vector3 m_vecOffset = Vector3.zero;
        public string sceneName { get; set; }
        public int levelLimit { get; private set; }

        public float XRate
        {
            get { return m_sizeRate.x; }
        }

        public float ZRate
        {
            get { return m_sizeRate.z; }
        }

        public Vector3 offset { get { return m_vecOffset; } }

        public void Initialize()
        {
            if (m_bInited == false)
            {
                var tableScene = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(SceneID);
                if (tableScene == null)
                {
                    Logger.LogErrorFormat("ComMapScene: table id {0} is not exist!!", SceneID);
                    return;
                }

                ISceneData sceneData = DungeonUtility.LoadSceneData(tableScene.ResPath);
                if (sceneData == null)
                {
                    Logger.LogErrorFormat("ComMapScene: table id {0} res path {1} is not exist!!", SceneID, tableScene.ResPath);
                    return;
                }

                sceneName = tableScene.Name;

                RectTransform rect = GetComponent<RectTransform>();
                
                m_sizeRate.x = rect.rect.width / (sceneData.GetLogicXSize().y - sceneData.GetLogicXSize().x);
                m_sizeRate.z = rect.rect.height / (sceneData.GetLogicZSize().y - sceneData.GetLogicZSize().x);

                m_vecOffset = new Vector3(sceneData.GetLogicXSize().x, 0.0f, sceneData.GetLogicZSize().x);

                levelLimit = tableScene.LevelLimit;

                SetLock(PlayerBaseData.GetInstance().Level < levelLimit);
                if (labLockDesc != null)
                {
                    labLockDesc.text = TR.Value("town_map_lock_desc", levelLimit);
                }

                m_bInited = true;
            }
        }

        public void SetLock(bool a_bLock)
        {
            if (objLockRoot != null)
            {
                objLockRoot.SetActive(a_bLock);
            }
        }

        public void LoadBackgroundImg()
        {
            // 1.5项目暂时不需要切换大地图的背景图,需要的时候再开放 by Wangbo 2020.09.01
            //imgBackground.SafeSetImage(backImgPath);
        }
    }
}
