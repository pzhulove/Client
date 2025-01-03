using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class NpcDialogSecondItem : MonoBehaviour
    {
        [SerializeField] private GameObject goLocal;
        [SerializeField] private Button btnClick;
        [SerializeField] private Text type = null;

        private NpcInteractionData data;

        public void Init(NpcInteractionData npcInteractionData, UnityAction<bool> unityAction)
        {
            data = npcInteractionData;

            if (goLocal != null)
            {
                btnClick = goLocal.GetComponent<Button>();
                btnClick.onClick.RemoveAllListeners();
                btnClick.onClick.AddListener(() =>
                {
                    if (null != data.onClickFunction)
                    {
                        data.onClickFunction.Invoke();
                        unityAction?.Invoke(false);
                    }
                });
            }
            Enable();
            OnUpdate();
        }

        public void OnDestroy()
        {
            goLocal = null;
            data = null;
            if(btnClick != null)
            {
                btnClick.onClick.RemoveAllListeners();
                btnClick = null;
            }
        }

        public void Enable()
        {
            if (goLocal != null)
            {
                goLocal.CustomActive(true);
            }
        }

        public void Disable()
        {
            if (goLocal != null)
            {
                goLocal.CustomActive(false);
            }
        }

        void OnUpdate()
        {
            type.SafeSetText(data.name);
        }
    }
}