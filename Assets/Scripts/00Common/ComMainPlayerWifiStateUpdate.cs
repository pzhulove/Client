using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class ComMainPlayerWifiStateUpdate : MonoBehaviour
    {
        [SerializeField]
        ComPlayerWifiState playerWifiState = null;

        Ping ping = null;
        Coroutine coroutine = null;

        private void Awake()
        {
            coroutine = GameFrameWork.instance.StartCoroutine(ProcPing());
        }

        private void Start()
        {
            
        }   

        private void OnDestroy()
        {       
            GameFrameWork.instance.StopCoroutine(coroutine);
            coroutine = null;
        }

        static int[] bounds = new int[] {int.MaxValue, 500,200,0 };
        BattlePlayer.eNetQuality GetNetQuality(int pingTime)
        {            
            if(bounds == null)
            {
                return BattlePlayer.eNetQuality.Bad;
            }

            for(int i = 0;i < bounds.Length;i++)
            {
                if(pingTime >= bounds[i])
                {
                    return (BattlePlayer.eNetQuality)i;
                }
            }

            return BattlePlayer.eNetQuality.Bad;
        }

        IEnumerator ProcPing()
        {
            ping = new Ping(ClientApplication.adminServer.ip);

            while (true)
            {
                if (ping == null)
                {
                    yield break;
                }

                yield return Yielders.GetWaitForSeconds(1.0f);

                if (!ping.isDone)
                {
                    yield return null;
                }
                else
                {
                    //Debug.LogErrorFormat("ping time = {0}", ping.time);

                    if (playerWifiState != null)
                    {
                        playerWifiState.SetUp(GetNetQuality(ping.time));
                    }

                    ping.DestroyPing();
                    ping = null;
               
                    ping = new Ping(ClientApplication.adminServer.ip);
                }
            }

            yield return null;
        }
    }
}