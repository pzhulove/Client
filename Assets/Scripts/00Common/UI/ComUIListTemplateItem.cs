using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    // TemplateFrame中用的item
    // 具体界面中item可以继承并实现 SetUp 接口
    internal class ComUIListTemplateItem : MonoBehaviour
    {
        public virtual void SetUp(object data)
        {
            
        }
    }
}
