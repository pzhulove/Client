using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using System.Reflection;

namespace GameClient
{   
    public class GuildDungeonHelpFrame : ClientFrame
    {
        #region inner def
        
        #endregion

        #region val
       
        #endregion

        #region ui bind
        
        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildDungeonHelp";
        }

        protected override void _OnOpenFrame()
        {           
            BindUIEvent(); 
        }

        protected override void _OnCloseFrame()
        {                    
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            
        }

        protected override void _unbindExUI()
        {
            
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            
        }

        void UnBindUIEvent()
        {
           
        }  

        #endregion

        #region ui event       

        #endregion
    }
}
