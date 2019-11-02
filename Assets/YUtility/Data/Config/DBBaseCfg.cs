using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DBBaseCfg
{
    /// <summary>
    /// 初始化配置
    /// </summary>
    public abstract void InitDB();
    /// <summary>
    /// 释放配置内存
    /// </summary>
    public abstract void F_ReleaseDB();
}

