using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using GameClient;
public class ComSystemHelper : MonoBehaviour {

        public void 播放音效(int soundID) {
            AudioManager.instance.PlaySound (soundID);
        }

        public void 发送GM命令(string command) {
            //Tenmove.Runtime.Client.GameUtility.Cheat.SendGM (command);
            BeUtility.SendGM(command);
        }

        public void 关闭界面(string frameName) {
            if (string.IsNullOrEmpty (frameName))
                return;

            Type frameType = TypeTable.GetType ("GameClient." + frameName.Trim ());
            if (frameType == null) {
                Logger.LogWarningFormat ("没有这个界面:{0}", frameName);
                return;
            }

            ClientSystemManager.GetInstance().CloseFrame (frameType);
        }

        public void 打开界面(string frameName) {
           // SystemHelper.OpenFrame (frameName);

            Type frameType = TypeTable.GetType ("GameClient." + frameName.Trim ());
            if (frameType == null) {
                Logger.LogWarningFormat ("没有这个界面:{0}", frameName);
                return;
            }
           ClientSystemManager.GetInstance().OpenFrame(frameType);
        }

        public void 打开通用提示(string text) {
            if (string.IsNullOrEmpty (text))
                return;

            int id = 0;
            if (!int.TryParse (text, out id)) {
                
                SystemNotifyManager.SysNotifyMsgBoxOK(text);

            } else {
                SystemNotifyManager.SystemNotify (id);
            }
        }

        public void 打开飘字提示(string text) {
            if (string.IsNullOrEmpty (text))
                return;

            int id = 0;
            if (!int.TryParse (text, out id)) {
                SystemNotifyManager.SysNotifyFloatingEffect (text);
            } else {
                SystemNotifyManager.SystemNotify (id);
            }
        }

        public void 更改速度(int speed)
        {
            if (speed != 0)
            {
                Time.timeScale = speed;
            }
        }

        public void 直接发送结算()
        {
            if (BattleMain.instance != null)
            {
                var battle = BattleMain.instance.GetBattle() as PVEBattle;
                if (battle == null)
                    return;
                
                battle.SendDungeonRaceEnd();
            }
        }
    }
