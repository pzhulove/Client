using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class SetTimeShowCloseBtn : MonoBehaviour
    {
        [SerializeField]private List<GameObject> mCloseRoots;
        [SerializeField]private float mTime;

        private bool isUpdate = false;
        private float timer = 0;
        void Update()
        {
            if (isUpdate == false)
            {
                timer += Time.deltaTime;

                if (timer >= mTime)
                {
                    for (int i = 0; i < mCloseRoots.Count; i++)
                    {
                        mCloseRoots[i].CustomActive(true);
                    }

                    isUpdate = true;
                }
            }
        }

        void OnDestroy()
        {
            isUpdate = false;
            timer = 0;
        }
    }
}