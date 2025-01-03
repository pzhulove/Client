using UnityEngine;
using System.Collections;

public class NpcArrowComponent : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}

    public void Active()
    {
        gameObject.SetActive(true);
    }

    public void DeActive()
    {
        gameObject.SetActive(false);
    }

    public static void ActiveNpcArrow(System.Int32 iNpcID)
    {
        var current = GameClient.ClientSystem.GetTargetSystem<GameClient.ClientSystemTown>();
        if(current != null)
        {
            var townNPC = current.GetTownNpcByNpcId(iNpcID);
            if(townNPC != null && townNPC.GraphicActor != null)
            {
                townNPC.GraphicActor.ActiveArrow();
            }
        }
    }

    public static void DeActiveNpcArrow(System.Int32 iNpcID)
    {
        var current = GameClient.ClientSystem.GetTargetSystem<GameClient.ClientSystemTown>();
        if (current != null)
        {
            var townNPC = current.GetTownNpcByNpcId(iNpcID);
            if (townNPC != null && townNPC.GraphicActor != null)
            {
                townNPC.GraphicActor.DeActiveArrow();
            }
        }
    }
}
