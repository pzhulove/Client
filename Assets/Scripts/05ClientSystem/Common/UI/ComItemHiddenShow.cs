using UnityEngine;
using System.Collections;

public class ComItemHiddenShow : MonoBehaviour {
    public bool mGlobalState = false;

    public GameObject[] mObjectList = new GameObject[0];
    public bool[] mObjectState = new bool[0];

    void Start()
    {
        _updateObjectState();
    }

    public void ChangeState()
    {
        mGlobalState = !mGlobalState;

        for (int i = 0; i < mObjectList.Length; i++)
        {
            if (i < mObjectState.Length)
            {
                mObjectState[i] = !mObjectState[i];
            }
        }

        _updateObjectState();
    }

    private void _updateObjectState()
    {
        for (int i = 0; i < mObjectList.Length; i++)
        {
            var obj = mObjectList[i];
            var state = mGlobalState;
            if (i < mObjectState.Length)
            {
                state = mObjectState[i];
            }

            obj.SetActive(state);
        }
    }
}
