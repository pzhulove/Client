using UnityEngine;
using System.Collections;

namespace GameClient
{
    class EnchantmentsRedBinder : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            _UpdateRedPoint();

            EnchantmentsCardManager.GetInstance().onTabMarkChanged += _OnTabMarkChanged;
            EnchantmentsCardManager.GetInstance().onUpdateCard += _OnUpdateCard;
        }

        void OnDestroy()
        {
            EnchantmentsCardManager.GetInstance().onTabMarkChanged -= _OnTabMarkChanged;
            EnchantmentsCardManager.GetInstance().onUpdateCard -= _OnUpdateCard;
        }

        void _OnTabMarkChanged(ulong guid)
        {
            _UpdateRedPoint();
        }

        void _OnUpdateCard(EnchantmentsCardData data)
        {
            _UpdateRedPoint();
        }

        void _UpdateRedPoint()
        {
            gameObject.CustomActive(EnchantmentsCardManager.GetInstance().HasNewCard());
        }
    }
}