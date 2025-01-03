using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class CommonHelpNewView : MonoBehaviour
    {

        private int _helpId;
        private CommonHelpTable _commonHelpTable;

        [Space(10)]
        [HeaderAttribute("Content")]
        [Space(10)]
        [SerializeField] private Text titleText;
        [SerializeField] private Text descriptionText;

        [SerializeField] private Button closeButton;
        
        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }
        }

        private void UnBindEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _helpId = 0;
            _commonHelpTable = null;
        }

        public void InitView(int helpId)
        {
            _helpId = helpId;

            _commonHelpTable = TableManager.GetInstance().GetTableItem<CommonHelpTable>(_helpId);
            if (_commonHelpTable == null)
            {
                Logger.LogErrorFormat("CommonHelpTable is null and helpId is {0}", _helpId);
                return;
            }
            
            InitContent();
        }


        //加载挑战的数据：挑战的名字，地图，以及具体的副本内容
        private void InitContent()
        {
            if (titleText != null)
            {
                //表中不配置的标题，默认显示玩法说明
                if (string.IsNullOrEmpty(_commonHelpTable.TitleText) == false)
                    titleText.text = _commonHelpTable.TitleText;
            }

            if (descriptionText != null)
            {
                descriptionText.text = _commonHelpTable.Descs.Replace("\\n", "\n");
            }

        }
        
        private void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<CommonHelpNewFrame>();
        }

    }
}
