using UnityEngine;
using System.Collections.Generic;
using GameClient;
using System.Collections;

public class ComNewbieDoubleMove : MonoBehaviour
{
    public float delayRemove = 0;
    void Start()
    {
        StartCoroutine(CheckEnd());
    }

    IEnumerator CheckEnd()
    {
        while (FrameSync.instance.bInMoveMode == false)
        {
            yield return Yielders.EndOfFrame;
        }

        BattleMain.instance.GetDungeonManager().ResumeFight();

        yield return Yielders.GetWaitForSeconds(delayRemove);

        GameObject.Destroy(gameObject);

        yield break;
    }
}