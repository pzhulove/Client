using UnityEngine;

public class ComElementTag : MonoBehaviour
{
    public GameObject light;
    public GameObject fire;
    public GameObject ice;

    public void SetElementActive(int type, bool active)
    {
        switch (type)
        {
            case 0:
                if(light != null)
                    light.SetActive(active);
                break;
            case 1:
                if(fire != null)
                    fire.SetActive(active);
                break;
            case 2:
                if(ice != null)
                    ice.SetActive(active);
                break;
        }
    }
}
