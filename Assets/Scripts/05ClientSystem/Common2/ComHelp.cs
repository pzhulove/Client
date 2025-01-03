using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class ComHelp : MonoBehaviour
    {  
        public GameObject textObj;
        // Use this for initialization  

        public void SetType(uint type)
        {
            if(textObj == null)
            {
                return;
            }

            Text txt = textObj.GetComponent<Text>();
            if(txt == null)
            {
                return;
            }

            var table = TableManager.GetInstance().GetTableItem<HelpFrameContentTable>((int)type);
            if(table != null)
            {
                txt.text = table.Content.Replace("\\n", "\n");
            }
            else
            {
                txt.text = string.Empty;
            } 
        }

        public void SetExtraTypeContent(uint type)
        {
            if (textObj == null)
            {
                return;
            }

            Text txt = textObj.GetComponent<Text>();
            if (txt == null)
            {
                return;
            }

            var table = TableManager.GetInstance().GetTableItem<HelpFrameContentTable>((int)type);
            if (table != null)
            {
                txt.text = table.ExtraContent.Replace("\\n", "\n");
            }
            else
            {
                txt.text = string.Empty;
            }
        }
    }
}