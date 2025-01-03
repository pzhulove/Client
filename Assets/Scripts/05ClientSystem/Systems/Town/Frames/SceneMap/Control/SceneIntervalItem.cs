using UnityEngine;

namespace GameClient
{
    class SceneIntervalItem : MonoBehaviour
    {
        [Space(10)] [HeaderAttribute("Item")] [Space(10)]
        public int FirstSceneId;
        public int SecondSceneId;

        public string GetIntervalItemStr()
        {
            return FirstSceneId.ToString() + "_" + SecondSceneId.ToString();
        }

    }
}
