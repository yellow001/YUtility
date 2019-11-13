using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YDebuger:InsManager<YDebuger>
{
    bool isDebug = false;
    public override void Init()
    {
        isDebug = AppSetting.Ins.GetIntValue("IsDebug") == 1;
        base.Init();
    }

    public static void Log(string msg) {
        if (Ins.isDebug) {
            Debug.Log(msg);
        }
    }

    public static void LogError(string msg)
    {
        if (Ins.isDebug)
        {
            Debug.LogError(msg);
        }
    }
}
