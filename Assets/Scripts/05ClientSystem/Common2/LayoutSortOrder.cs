using UnityEngine;
using System.Collections;
namespace GameClient
{
    class LayoutSortOrder : MonoBehaviour
    {
        public int iOrder = 0;
        public int SortID
        {
            get
            {
                return iOrder;
            }
            set
            {
                iOrder = value;
            }
        }
    }
}