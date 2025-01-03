using GameClient;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

public class IOSFunctionSwitchManager : Singleton<IOSFunctionSwitchManager>
{
    private int firstIndex = (int)IOSFuncSwitchTable.eType.NONE;

    private IOSFuncSwitchTable.eType[] iosFuncSwitchTypes;
    public override void Init()
    {
        iosFuncSwitchTypes = null;
    }

    public override void UnInit()
    {
        if (iosFuncSwitchTypes != null)
            iosFuncSwitchTypes = null;
    }

    public void AddClosedFunctions(GateNotifySysSwitch msg)
    {
        var sysSwitchCfgs = msg.cfg;
        if (sysSwitchCfgs != null)
        {
            iosFuncSwitchTypes = new IOSFuncSwitchTable.eType[sysSwitchCfgs.Length];
            if (iosFuncSwitchTypes == null)
                return;
            for (int i = 0; i < sysSwitchCfgs.Length; i++)
            {
                var cfg = sysSwitchCfgs[i];
                if (cfg.switchValue == 0)
                {
                    iosFuncSwitchTypes[i] = (IOSFuncSwitchTable.eType)cfg.sysType;
                }
            }
        }
    }

    public bool IsFunctionClosed(IOSFuncSwitchTable.eType funcType)
    {
        bool isClosed = false;

        if (iosFuncSwitchTypes == null)
            return false;
        for (int i = firstIndex; i < iosFuncSwitchTypes.Length; i++)
        {
            var type = iosFuncSwitchTypes[i];
            if (type == funcType)
            {
                isClosed = true;
                break;
            }
        }
        return isClosed;
    }
}
