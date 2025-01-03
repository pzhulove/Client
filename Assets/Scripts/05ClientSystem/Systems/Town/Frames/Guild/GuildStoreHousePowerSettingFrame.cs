using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class GuildStoreHousePowerSettingFrameData
    {
        public List<int> toggle0s = new List<int>();
        public List<int> toggle1s = new List<int>();
    }

    public enum PowerSettingType
    {
        PST_INVALID = -1,
        PST_WIN_POWER = 0,
        PST_LOSE_POWER,
        PST_CONTRIBUTE_POWER,
        PST_DELETE_POWER,
        PST_COUNT,
    }

    class GuildStoreHousePowerSettingFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildStoreHousePowerSettingFrame";
        }

        [UIEventHandleAttribute("BtnClose")]
        void _OnClickCloseFrame()
        {
            frameMgr.CloseFrame(this);
        }

        public static void CommandOpen(object argv = null)
        {
            if (argv == null)
            {
                argv = new GuildStoreHousePowerSettingFrameData
                {
                    toggle0s = GuildDataManager.winPowerSetting,
                    toggle1s = GuildDataManager.losePowerSetting,
                };
            }

            ClientSystemManager.GetInstance().OpenFrame<GuildStoreHousePowerSettingFrame>(FrameLayer.Middle, argv);
        }

        GuildStoreHousePowerSettingFrameData data = null;
        protected override void _OnOpenFrame()
        {
            data = userData as GuildStoreHousePowerSettingFrameData;
            _InitToggle0();
            _InitToggle1();
            _InitToggle2();
            _InitToggle3();
            GuildDataManager.GetInstance().onGuildPowerChanged += _OnGuildPowerChanged;
        }

        void _OnGuildPowerChanged(PowerSettingType ePowerSettingType, int iPowerValue)
        {
            if(!(ePowerSettingType > PowerSettingType.PST_INVALID && ePowerSettingType < PowerSettingType.PST_COUNT))
            {
                return;
            }

            switch(ePowerSettingType)
            {
                case PowerSettingType.PST_WIN_POWER:
                    {
                        iPowerValue = GuildDataManager.GetInstance().translateWinPowerIndex(iPowerValue);
                        if (null != data && null != data.toggle0s && iPowerValue >= 0 && iPowerValue < data.toggle0s.Count && iPowerValue < m_akToggle0s.Count)
                        {
                            for(int i = 0; i < m_akToggle0s.Count; ++i)
                            {
                                int iIndex = i;
                                m_akToggle0s[i].onValueChanged.RemoveAllListeners();
                                m_akToggle0s[i].onValueChanged.AddListener((bool bValue) =>
                                {
                                    m_goSelecte0s[iIndex].CustomActive(bValue);
                                });
                            }
                            m_akToggle0s[iPowerValue].isOn = true;
                            for (int i = 0; i < m_akToggle0s.Count; ++i)
                            {
                                int iPowerIndex = i;
                                m_akToggle0s[i].onValueChanged.AddListener((bool bValue) =>
                                {
                                    if(bValue)
                                    {
                                        int iPower = GuildDataManager.getWinPowerByIndex(iPowerIndex);
                                        //Logger.LogErrorFormat("成功概率{0}",iPower);
                                        GuildDataManager.GetInstance().SendChangeGuildSettingPower(PowerSettingType.PST_WIN_POWER, iPower);
                                    }
                                });
                            }
                        }
                    }
                    break;
                case PowerSettingType.PST_LOSE_POWER:
                    {
                        iPowerValue = GuildDataManager.GetInstance().translateLosePowerIndex(iPowerValue);
                        if (null != data && null != data.toggle1s && iPowerValue >= 0 && iPowerValue < data.toggle1s.Count && iPowerValue < m_akToggle1s.Count)
                        {
                            for (int i = 0; i < m_akToggle1s.Count; ++i)
                            {
                                int iIndex = i;
                                m_akToggle1s[i].onValueChanged.RemoveAllListeners();
                                m_akToggle1s[i].onValueChanged.AddListener((bool bValue) =>
                                {
                                    m_goSelecte1s[iIndex].CustomActive(bValue);
                                });
                            }
                            m_akToggle1s[iPowerValue].isOn = true;
                            for (int i = 0; i < m_akToggle1s.Count; ++i)
                            {
                                int iPowerIndex = i;
                                m_akToggle1s[i].onValueChanged.AddListener((bool bValue) =>
                                {
                                    if(bValue)
                                    {
                                        int iPower = GuildDataManager.getLosePowerByIndex(iPowerIndex);
                                        //Logger.LogErrorFormat("失败概率{0}", iPower);
                                        GuildDataManager.GetInstance().SendChangeGuildSettingPower(PowerSettingType.PST_LOSE_POWER, iPower);
                                    }
                                });
                            }
                        }
                    }
                    break;
                case PowerSettingType.PST_CONTRIBUTE_POWER:
                    {
                        int iPowerIndex = 0;
                        GuildPost eEGuildDuty = (GuildPost)iPowerValue;
                        if(eEGuildDuty == GuildPost.GUILD_POST_ELDER)
                        {
                            iPowerIndex = 1;
                        }
                        else if(eEGuildDuty == GuildPost.GUILD_POST_NORMAL)
                        {
                            iPowerIndex = 0;
                        }

                        if(iPowerIndex >= 0 && iPowerIndex < m_akToggle2s.Count)
                        {
                            for (int i = 0; i < m_akToggle2s.Count; ++i)
                            {
                                int iIndex = i;
                                m_akToggle2s[i].onValueChanged.RemoveAllListeners();
                                m_akToggle2s[i].onValueChanged.AddListener((bool bValue) =>
                                {
                                    m_goSelecte2s[iIndex].CustomActive(bValue);
                                });
                            }
                            m_akToggle2s[iPowerIndex].isOn = true;
                            for (int i = 0; i < m_akToggle2s.Count; ++i)
                            {
                                GuildPost eResult = i == 0 ? GuildPost.GUILD_POST_NORMAL : GuildPost.GUILD_POST_ELDER;

                                m_akToggle2s[i].onValueChanged.AddListener((bool bValue) =>
                                {
                                    if(bValue)
                                    {
                                        //Logger.LogErrorFormat("捐献权限");
                                        GuildDataManager.GetInstance().SendChangeGuildSettingPower(PowerSettingType.PST_CONTRIBUTE_POWER, (int)eResult);
                                    }
                                });
                            }
                        }
                    }
                    break;
                case PowerSettingType.PST_DELETE_POWER:
                    {
                        int iPowerIndex = 0;
                        GuildPost eEGuildDuty = (GuildPost)iPowerValue;
                        if (eEGuildDuty == GuildPost.GUILD_POST_ASSISTANT)
                        {
                            iPowerIndex = 0;
                        }
                        else if (eEGuildDuty == GuildPost.GUILD_POST_ELDER)
                        {
                            iPowerIndex = 1;
                        }

                        if (iPowerIndex >= 0 && iPowerIndex < m_akToggle3s.Count)
                        {
                            for (int i = 0; i < m_akToggle3s.Count; ++i)
                            {
                                int iIndex = i;
                                m_akToggle3s[i].onValueChanged.RemoveAllListeners();
                                m_akToggle3s[i].onValueChanged.AddListener((bool bValue) =>
                                {
                                    m_goSelecte3s[iIndex].CustomActive(bValue);
                                });
                            }
                            m_akToggle3s[iPowerIndex].isOn = true;
                            for (int i = 0; i < m_akToggle3s.Count; ++i)
                            {
                                GuildPost eResult = i == 0 ? GuildPost.GUILD_POST_ASSISTANT : GuildPost.GUILD_POST_ELDER;

                                m_akToggle3s[i].onValueChanged.AddListener((bool bValue) =>
                                {
                                    if (bValue)
                                    {
                                        //Logger.LogErrorFormat("删除权限");
                                        GuildDataManager.GetInstance().SendChangeGuildSettingPower(PowerSettingType.PST_DELETE_POWER, (int)eResult);
                                    }
                                });
                            }
                        }
                    }
                    break;
            }
        }

        List<Toggle> m_akToggle0s = new List<Toggle>();
        List<GameObject> m_goSelecte0s = new List<GameObject>();
        void _InitToggle0()
        {
            GameObject goParent = Utility.FindChild(frame, "VP/TGP");
            GameObject goPrefab = Utility.FindChild(frame, "VP/TGP/TG0");
            goPrefab.CustomActive(false);
            for (int i = 0; i < data.toggle0s.Count; ++i)
            {
                var toggleData = (int)data.toggle0s[i];
                GameObject goCurrent = GameObject.Instantiate(goPrefab);
                if(null == goCurrent)
                {
                    continue;
                }
                Utility.AttachTo(goCurrent, goParent);
                goCurrent.CustomActive(true);
                Text desc = Utility.FindComponent<Text>(goCurrent, "Desc");
                if(null != desc)
                {
                    desc.text = string.Format("{0}%", toggleData);
                }
                GameObject goSelected = Utility.FindChild(goCurrent, "Selected");
                goSelected.CustomActive(false);
                Toggle toggle = goCurrent.GetComponent<Toggle>();
                if (null != toggle)
                {
                    m_akToggle0s.Add(toggle);
                    m_goSelecte0s.Add(goSelected);
                }
            }
            _OnGuildPowerChanged(PowerSettingType.PST_WIN_POWER, GuildDataManager.GetInstance().winPower);
        }
        void _UnInitToggle0()
        {
            for(int i = 0; i < m_akToggle0s.Count; ++i)
            {
                m_akToggle0s[i].onValueChanged.RemoveAllListeners();
            }
            m_akToggle0s.Clear();
            m_goSelecte0s.Clear();
        }

        void _SendChangeGuildPower(PowerSettingType ePowerSettingType,int iPower)
        {

        }

        List<Toggle> m_akToggle1s = new List<Toggle>();
        List<GameObject> m_goSelecte1s = new List<GameObject>();
        void _InitToggle1()
        {
            GameObject goParent = Utility.FindChild(frame, "FP/TGP");
            GameObject goPrefab = Utility.FindChild(frame, "FP/TGP/TG0");
            goPrefab.CustomActive(false);
            for (int i = 0; i < data.toggle0s.Count; ++i)
            {
                var toggleData = (int)data.toggle0s[i];
                GameObject goCurrent = GameObject.Instantiate(goPrefab);
                if (null == goCurrent)
                {
                    continue;
                }
                Utility.AttachTo(goCurrent, goParent);
                goCurrent.CustomActive(true);
                Text desc = Utility.FindComponent<Text>(goCurrent, "Desc");
                if (null != desc)
                {
                    desc.text = string.Format("{0}%", toggleData);
                }
                GameObject goSelected = Utility.FindChild(goCurrent, "Selected");
                goSelected.CustomActive(false);
                Toggle toggle = goCurrent.GetComponent<Toggle>();
                if (null != toggle)
                {
                    m_akToggle1s.Add(toggle);
                    m_goSelecte1s.Add(goSelected);
                }
            }
            _OnGuildPowerChanged(PowerSettingType.PST_LOSE_POWER, GuildDataManager.GetInstance().losePower);
        }
        void _UnInitToggle1()
        {
            for (int i = 0; i < m_akToggle1s.Count; ++i)
            {
                m_akToggle1s[i].onValueChanged.RemoveAllListeners();
            }
            m_akToggle1s.Clear();
            m_goSelecte1s.Clear();
        }

        List<Toggle> m_akToggle2s = new List<Toggle>();
        List<GameObject> m_goSelecte2s = new List<GameObject>();
        void _InitToggle2()
        {
            for (int i = 0; i < 2; ++i)
            {
                GameObject goCurrent = Utility.FindChild(frame, string.Format("PF/TGP0/TG{0}",i));
                goCurrent.CustomActive(true);
                GameObject goSelected = Utility.FindChild(goCurrent, "Selected");
                goSelected.CustomActive(false);
                Toggle toggle = goCurrent.GetComponent<Toggle>();
                if (null != toggle)
                {
                    m_akToggle2s.Add(toggle);
                    m_goSelecte2s.Add(goSelected);
                }
            }
            _OnGuildPowerChanged(PowerSettingType.PST_CONTRIBUTE_POWER,(int)GuildDataManager.GetInstance().contributePower);
        }
        void _UnInitToggle2()
        {
            for (int i = 0; i < m_akToggle2s.Count; ++i)
            {
                m_akToggle2s[i].onValueChanged.RemoveAllListeners();
            }
            m_akToggle2s.Clear();
            m_goSelecte2s.Clear();
        }

        List<Toggle> m_akToggle3s = new List<Toggle>();
        List<GameObject> m_goSelecte3s = new List<GameObject>();
        void _InitToggle3()
        {
            for (int i = 0; i < 2; ++i)
            {
                GameObject goCurrent = Utility.FindChild(frame, string.Format("PF/TGP1/TG{0}", i));
                goCurrent.CustomActive(true);
                GameObject goSelected = Utility.FindChild(goCurrent, "Selected");
                goSelected.CustomActive(false);
                Toggle toggle = goCurrent.GetComponent<Toggle>();
                if (null != toggle)
                {
                    m_akToggle3s.Add(toggle);
                    m_goSelecte3s.Add(goSelected);
                }
            }
            _OnGuildPowerChanged(PowerSettingType.PST_DELETE_POWER, (int)GuildDataManager.GetInstance().clearPower);
        }

        void _UnInitToggle3()
        {
            for (int i = 0; i < m_akToggle3s.Count; ++i)
            {
                m_akToggle3s[i].onValueChanged.RemoveAllListeners();
            }
            m_akToggle3s.Clear();
            m_goSelecte3s.Clear();
        }

        protected override void _OnCloseFrame()
        {
            _UnInitToggle0();
            _UnInitToggle1();
            _UnInitToggle2();
            _UnInitToggle3();
            GuildDataManager.GetInstance().onGuildPowerChanged -= _OnGuildPowerChanged;
            data = null;
        }
    }
}