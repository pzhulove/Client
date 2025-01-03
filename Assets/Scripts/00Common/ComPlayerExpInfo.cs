using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProtoTable;
using System;
using System.Text;

namespace GameClient
{
    class ComPlayerExpInfo : MonoBehaviour
    {
        [SerializeField]
        ComBattery comBattery = null;

        [SerializeField]
        Text serverTime = null;

        [SerializeField]
        ComExpBar expBar = null;

        object updateServerTimeObj = new object();
        object updateBatteryObj = new object();

        private void Awake()
        {
            updateServerTimeObj = new object();
            updateBatteryObj = new object();

            InvokeMethod.InvokeInterval(updateServerTimeObj, 0.0f, 0.5f, float.MaxValue, null, UpdateServerTime, null);
            InvokeMethod.InvokeInterval(updateBatteryObj, 0.0f, 5.0f, float.MaxValue, null, UpdateBattery, null);

            if (expBar != null)
            {
                expBar.TextFormat = (exp) =>
                {
                    var pair = TableManager.instance.GetCurRoleExp(exp);
                    double dRate = 0.0f;
                    if (pair.Value == 0)
                    {
                        dRate = 100.0f;
                    }
                    else
                    {
                        dRate = ((double)pair.Key) / ((double)pair.Value) * 100;
                    }

                    return string.Format("Exp:{0}/{1}({2}%)", pair.Key, pair.Value, dRate.ToString("F1"));
                };
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, OnLevelChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ExpChanged, OnExpChanged);
        }

        private void Start()
        {
            UpdateServerTime();
            UpdateBattery();
            UpdateExp();
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, OnLevelChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ExpChanged, OnExpChanged);

            InvokeMethod.RmoveInvokeIntervalCall(updateServerTimeObj);
            InvokeMethod.RmoveInvokeIntervalCall(updateBatteryObj);
            updateServerTimeObj = null;
            updateBatteryObj = null;
        }

        void OnLevelChanged(UIEvent uiEvent)
        {
            // ClientSystemTown currentSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            // if (currentSystem == null)
            // {
            //     return;
            // }

            UpdateExp(false);
        }

        void OnExpChanged(UIEvent uiEvent)
        {
            UpdateExp(false);
        }

        void UpdateExp(bool force = true)
        {
            DrawExpBar(force);
        }

        void UpdateServerTime()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Clear();

            DateTime dateTime = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());
            builder.AppendFormat("{0:00}:{1:00}:{2:00}", dateTime.Hour, dateTime.Minute, dateTime.Second);
            serverTime.SafeSetText(builder.ToString());

            StringBuilderCache.Release(builder);
        }

        void UpdateBattery()
        {
            if (comBattery != null)
            {
                comBattery.SetUp(PluginManager.GetInstance().GetBatteryPower());
            }
        }

        void DrawExpBar(bool force = true)
        {
            if (expBar == null)
            {
                return;
            }

            expBar.SetExp(PlayerBaseData.GetInstance().Exp, force, exp =>
            {
                return TableManager.instance.GetCurRoleExp(exp);
            });
        }
    }
}