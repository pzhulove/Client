using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameClient;

[RequireComponent(typeof(Button))]
public class ComGoToShopButton : MonoBehaviour
{
    [SerializeField] private int mShopId;
    [SerializeField] private int mShopLinkID;
    [SerializeField] private int mShopTabID;
    private Button mButton;

    void Awake()
    {
        mButton = GetComponent<Button>();
        mButton.onClick.AddListener(OnButtonClick);
    }

    void OnDestroy()
    {
        mButton.onClick.RemoveListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        ShopDataManager.GetInstance().OpenShop(mShopId, mShopLinkID, mShopTabID);
    }

}
